using BitcoinLib.Network;
using LevelDB;
using System.Buffers.Binary;

/*
bitcoin /
│
├─ blocks /                # Blockchain-Daten
│   ├─ blk00000.dat        # Blockdaten (Raw Blocks)
│   ├─ blk00001.dat
│   ├─ rev00000.dat        # Revert-Daten (Undo-Infos für UTXO)
│   └─ index/              # bLevelDB for blockindex
│
├─ chainstate /            # LevelDB mit aktuellem UTXO-Set
│   ├─ CURRENT
│   ├─ *.ldb
│   └─ ...
│
├─ database/              # Optionale LevelDB für Indexe
├─ wallets/               # Wallet-Dateien (optional)
├─ bitcoin.conf           # Konfiguration
└─ debug.log
*/

namespace BitcoinLib.Storage
{
    public class BlockIndex
    {
        /// <summary>
        /// blkxxxxx.dat files may not be larger than this in bytes
        /// </summary>
        public static long MaxBlockFileSize = 128 * 1024 * 1024;


        private static UInt32 magic = 0xD9B4BEF9; // Mainnet

        /// <summary>
        /// Value of %appdata%,
        /// something like C:\Users\chmau\AppData\Roaming
        /// </summary>
        private static string _appdata;

        /// <summary>
        /// Something like C:\Users\chmau\AppData\Roaming\bitcoin\blocks\index
        /// </summary>
        private static string _blockIndexDbName;

        /// <summary>
        /// All the blk00000.dat files are stored in this folder.
        /// </summary>
        private static string _blocksDir = _appdata + @"\bitcoin\blocks";

        static BlockIndex()
        {
            // C:\Users\chmau\AppData\Roaming
            _appdata = Environment.GetEnvironmentVariable("appdata");

            // C:\Users\chmau\AppData\Roaming\bitcoin\blocks\index
            _blockIndexDbName = _appdata + @"\bitcoin\blocks\index";

            // C:\Users\chmau\AppData\Roaming\bitcoin\blocks
            _blocksDir = _appdata + @"\bitcoin\blocks";
        }

        public static void AddGenesisBlock()
        {
            Block block = Block.Parse(Block.GENESIS_BLOCK);
            BlockIndex.AddBlock(block);
        }

        public static void AddBlockWithPrevHash(string hash)
        {
            // we cannot check if we already have this block because we do not have the hash
            Block block = SimpleNode.GetBlockWithPrevHash(hash);

            Console.WriteLine("Received block online:");
            Block.PrintBlock(block);
            BlockIndex.AddBlock(block);
        }

        /// <summary>
        /// Add the block with the specified hash to our filesystem. 
        /// If the hash already exists, we do nothing.
        /// </summary>
        /// <param name="hash"></param>
        public static void AddBlockWithHash(string hash)
        {
            byte[] bHash = Tools.HexStringToBytes(hash);
            BlockIndexEntry entry;

            if (!BlockIndex.HashExists(bHash, out entry))
            {
                Block block = SimpleNode.GetBlockWithHash(hash);
                Console.WriteLine("Received block online:");
                Block.PrintBlock(block);
                BlockIndex.AddBlock(block);
            }
        }

        public static void PrintBlock(string hash)
        {
            byte[] bHash = Tools.HexStringToBytes(hash);
            BlockIndexEntry entry;

            if (BlockIndex.HashExists(bHash, out entry))
            {
                byte[] raw = BlockIndex.ReadBlockBytes(entry);
                Block block = Block.Parse(raw);
                Console.WriteLine("Block:");
                Tools.PrintJsonObject(block);
            }
        }

        /// <summary>
        /// Store the binary block data in one of the blk*.dat files
        /// Add the block header hash to the blockindex levelDB.
        /// </summary>
        /// <param name="block"></param>
        private static void AddBlock(Block block)
        {
            byte[] bHash = block._blockHeader.Hash();
            BlockIndexEntry entry;

            if (BlockIndex.HashExists(bHash, out entry))
            {
                return;
            }

            byte[] bBlock = block.serialize_total();
            int blockLength = bBlock.Length;

            // 1️ Letzte Datei ermitteln
            var files = Directory.GetFiles(_blocksDir, "blk*.dat");
            int lastFileNumber = 0;

            if (files.Length > 0)
            {
                lastFileNumber = files
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .Select(name => int.Parse(name.Substring(3)))
                    .Max();
            }

            string lastFilePath = Path.Combine(_blocksDir, $"blk{lastFileNumber:D5}.dat");
            FileInfo fi = new FileInfo(lastFilePath);
            long currentSize = fi.Exists ? fi.Length : 0;

            // 2️ Prüfen, ob neue Datei nötig ist
            long requiredSize = blockLength + 8; // +8 für Magic + Blocksize
            if (currentSize + requiredSize > MaxBlockFileSize)
            {
                lastFileNumber++;
                lastFilePath = Path.Combine(_blocksDir, $"blk{lastFileNumber:D5}.dat");
            }

            // 3️ Datei öffnen / erstellen in eigenem using
            using (FileStream fs = new FileStream(
                lastFilePath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None))
            {
                fs.Seek(0, SeekOrigin.End);

                long offset = fs.Position;

                // 4️ Magic + Blocksize vorbereiten
                byte[] header = new byte[8];
               
                BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0, 4), magic);
                BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4, 4), (UInt32)bBlock.Length);

                // 5️ Header schreiben 8 byte
                fs.Write(header, 0, header.Length);

                // 6️ Block-Bytes anhängen
                fs.Write(bBlock, 0, blockLength);
                fs.Flush();
                fs.Close();

                Console.WriteLine($"Block ({blockLength} bytes) added to {lastFilePath}");

                BlockIndexEntry bie = new BlockIndexEntry(block._blockHeader._prevBlockHash,
                    0, (UInt32)lastFileNumber, (UInt32)offset, (UInt32)requiredSize, 0);
                BlockIndex.Put(bHash, bie);
            }
        }

        private static void Put(byte[] hash, BlockIndexEntry entry)
        {
            var options = new Options
            {
                CreateIfMissing = true
            };

            byte[] value = entry.serialize();

            using (DB db = DB.Open(_blockIndexDbName, options))
            {
                WriteOptions writeOptions = new WriteOptions();
                writeOptions.Sync = true;

                db.Put(writeOptions, hash, value);
            }
        }

        private static bool HashExists(byte[] hash, out BlockIndexEntry entry)
        {
            bool success = false;
            entry = null;

            var options = new Options
            {
                CreateIfMissing = true
            };

            using (DB db = DB.Open(_blockIndexDbName, options))
            {
                ReadOptions readOptions = new ReadOptions();
                success = db.TryGet(readOptions, hash, out var slice);
                if (success)
                {
                    byte[] value = slice.ToArray();
                    entry = BlockIndexEntry.Parse(value);
                    Console.WriteLine($"Hash {Tools.BytesToHexString(hash)} exists:");
                    Tools.PrintJsonObject(entry);
                }
                else
                {
                    Console.WriteLine($"Hash {Tools.BytesToHexString(hash)} does not exist.");
                }
            }

            return success;
        }

        /// <summary>
        /// Read one entry from a blk00000.dat file.
        /// Each block has a header of 8 bytes, then the raw block data.
        /// [magic] 4 bytes
        /// [block len in bytes] 4 bytes
        /// [raw block data]
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        static private byte[] ReadBlockBytes(BlockIndexEntry entry)
        {
            // Datei-Name zusammenbauen, immer 5-stellig: blk00000.dat
            string fileName = $"blk{entry._file:D5}.dat";
            string filePath = Path.Combine(_blocksDir, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Datei nicht gefunden: {filePath}");

            byte[] header = new byte[8];
            byte[] buffer = new byte[entry._size];
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(entry._offset, SeekOrigin.Begin);

                int bytesRead = fs.Read(header, 0, 8);
                if (bytesRead < 8)
                {
                    Console.WriteLine($"Error: could not read header bytes (8 bytes)");
                    return null;
                }
                UInt32 magicLE = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(0, 4));
                UInt32 blockSizeLE = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4, 4));
                int requiredSize = (int)entry._size - 8;

                if (magicLE != magic)
                {
                    Console.WriteLine($"Error: invalid magic number, expected {magic}, read {magicLE}");
                    return null;
                }
                if (blockSizeLE != requiredSize)
                {
                    Console.WriteLine($"Error: invalid block size, expected {requiredSize}, read {blockSizeLE}");
                    return null;
                }

                bytesRead = fs.Read(buffer, 0, requiredSize);
                if (bytesRead < requiredSize)
                {
                    Console.WriteLine($"Error: could not read block bytes ({requiredSize} bytes)");
                    return null;
                }
            }

            return buffer;
        }
    }
}
