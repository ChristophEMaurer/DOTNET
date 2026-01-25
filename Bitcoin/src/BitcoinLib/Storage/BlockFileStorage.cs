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

One block entry in a blkxxxxx.dat file:
[][magic] 4 bytes LE
[][block len in bytes] 4 bytes LE
[][raw block data]
*/

namespace BitcoinLib.Storage
{
    /// <summary>
    /// This class manages the blkxxxxx.dat files. 
    /// </summary>
    public class BlockFileStorage
    {
        /// <summary>
        /// key: block hash, value: list of successor block hashes
        /// </summary>
        public static Dictionary<string, List<string>> _successors = new Dictionary<string, List<string>>();

        /// <summary>
        /// blkxxxxx.dat files may not be larger than this in bytes
        /// </summary>
        private static long MaxBlockFileSize = 128 * 1024 * 1024;

        // each block entry in a blkxxxxx.dat file starts with this magic number (4 bytes, little-endian)
        private static UInt32 magic = 0xD9B4BEF9; // Mainnet

        /// <summary>
        /// Value of %appdata%,
        /// something like C:\Users\chmau\AppData\Roaming
        /// </summary>
        private static string _appdata;

        /// <summary>
        /// All the blk00000.dat files are stored in this folder.
        /// </summary>
        private static string _blocksDir = _appdata + @"\bitcoin\blocks";
        public static string BlocksDir => _blocksDir;

        static BlockFileStorage()
        {
            // C:\Users\chmau\AppData\Roaming
            _appdata = Environment.GetEnvironmentVariable("appdata");

            // C:\Users\chmau\AppData\Roaming\bitcoin\blocks
            _blocksDir = _appdata + @"\bitcoin\blocks";
        }

        public static void Initialize()
        {
            if (_successors.Count() > 0)
            {
                return;
            }

            var files = Directory.GetFiles(_blocksDir, "blk*.dat");
            int lastFileNumber = files.Count();

            for (int i = 0; i < lastFileNumber; i++)
            {
                // Datei-Name zusammenbauen, immer 5-stellig: blk00000.dat
                string fileName = $"blk{i:D5}.dat";
                string filePath = Path.Combine(_blocksDir, fileName);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Datei nicht gefunden: {filePath}");

                byte[] header = new byte[8];
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    while (fs.Position < fs.Length)
                    {
                        int bytesRead = fs.Read(header, 0, 8);
                        if (bytesRead < 8)
                        {
                            Tools.ConsoleWriteWarning($"Error: could not read header bytes (8 bytes)");
                            return;
                        }
                        UInt32 magicLE = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(0, 4));
                        UInt32 blockSizeLE = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4, 4));

                        if (magicLE != magic)
                        {
                            Tools.ConsoleWriteWarning($"Error: invalid magic number, expected {magic}, read {magicLE}");
                            return;
                        }

                        byte[] buffer = new byte[blockSizeLE];
                        bytesRead = fs.Read(buffer, 0, (int) blockSizeLE);
                        if (bytesRead < blockSizeLE)
                        {
                            Tools.ConsoleWriteWarning($"Error: could not read block bytes ({blockSizeLE} bytes)");
                            return;
                        }
                        Block block = Block.Parse(buffer);

                        if (!block.IsGenesisBlock())
                        {
                            // Genesis-Block hat keinen Vorgänger
                            string blockHash = Tools.BytesToHexString(block._blockHeader.Hash());
                            string prevBlockHash = Tools.BytesToHexString(block._blockHeader._prevBlockHash);

                            UpdateSuccessorHashes(prevBlockHash, blockHash);
                        }
                    }
                }
            }
        }

        public static int GetNumberOfBlockFiles()
        {
            var files = Directory.GetFiles(_blocksDir, "blk*.dat");
            return files.Length;
        }

        /// <summary>
        /// Store the binary block data in one of the blk*.dat files and update the in-memory index of successor hashes.
        /// </summary>
        /// <param name="block"></param>
        public static BlockWriteResult AddBlock(Block block)
        {
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

                if (Tools.DebugLogLevel > 0)
                {
                    Tools.ConsoleWriteLine($"Block ({blockLength} bytes) added to {lastFilePath}");
                }
                BlockWriteResult blockWriteResult = new BlockWriteResult();
                blockWriteResult.FileNumber = (UInt32) lastFileNumber;
                blockWriteResult.Offset = (UInt32) offset;
                blockWriteResult.Length = (UInt32) requiredSize;

                if (!block.IsGenesisBlock())
                {
                    // Genesis-Block hat keinen Vorgänger
                    string blockHash = Tools.BytesToHexString(block._blockHeader.Hash());
                    string prevBlockHash = Tools.BytesToHexString(block._blockHeader._prevBlockHash);
                    UpdateSuccessorHashes(prevBlockHash, blockHash);
                }

                return blockWriteResult;
            }
        }

        private static void UpdateSuccessorHashes(string prevBlockHash, string blockHash)
        {
            bool added = false;

            if (!_successors.ContainsKey(prevBlockHash))
            {
                _successors.Add(prevBlockHash, new List<string> { blockHash });
                added = true;
            }
            else
            {
                if (!_successors[prevBlockHash].Contains(blockHash))
                {
                    _successors[prevBlockHash].Add(blockHash);
                    added = true;
                }
            }

            if (Tools.DebugLogLevel > 0)
            {
                if (added)
                {
                    Tools.ConsoleWriteLine($"Added to RAM: {prevBlockHash} -> {blockHash}");
                }
                else
                {
                    Tools.ConsoleWriteLine($"Already in RAM: {prevBlockHash} -> {blockHash}");
                }
            }
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
        static public byte[] ReadBlockBytes(BlockWriteResult entry)
        {
            // Datei-Name zusammenbauen, immer 5-stellig: blk00000.dat
            string fileName = $"blk{entry.FileNumber:D5}.dat";
            string filePath = Path.Combine(_blocksDir, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Datei nicht gefunden: {filePath}");

            byte[] header = new byte[8];
            byte[] buffer = new byte[entry.Length];
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(entry.Offset, SeekOrigin.Begin);

                int bytesRead = fs.Read(header, 0, 8);
                if (bytesRead < 8)
                {
                    Tools.ConsoleWriteWarning($"Error: could not read header bytes (8 bytes)");
                    return null;
                }
                UInt32 magicLE = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(0, 4));
                UInt32 blockSizeLE = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4, 4));
                int requiredSize = (int)entry.Length - 8;

                if (magicLE != magic)
                {
                    Tools.ConsoleWriteWarning($"Error: invalid magic number, expected {magic}, read {magicLE}");
                    return null;
                }
                if (blockSizeLE != requiredSize)
                {
                    Tools.ConsoleWriteWarning($"Error: invalid block size, expected {requiredSize}, read {blockSizeLE}");
                    return null;
                }

                bytesRead = fs.Read(buffer, 0, requiredSize);
                if (bytesRead < requiredSize)
                {
                    Tools.ConsoleWriteWarning($"Error: could not read block bytes ({requiredSize} bytes)");
                    return null;
                }
            }

            return buffer;
        }
    }
}
