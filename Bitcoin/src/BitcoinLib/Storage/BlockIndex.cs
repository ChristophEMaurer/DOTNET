using LevelDB;
using System.Data;

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
    /// <summary>
    /// This class manages the block index stored in LevelDB
    /// 
    /// key                                             -> value
    /// 
    /// 'b' || <32-byte blockhash>   (little endian)    -> BlockIndexEntry
    /// 'f' || <file number (varint)>                   -> CBlockFileInfo
    /// 'l'                                             -> <int file number>    last block file number
    /// 'R'                                             -> 1 Existiert nur während Reindex,
    ///                                                         Wenn Core crasht und 'R' existiert → Reindex wird beim Start fortgesetzt
    /// 't' || <32-byte txid>                           -> { blockfile, offset, txoffset }
    /// 
    /// we only use 'b' entries here, and even without the 'b' prefix
    /// </summary>
    public class BlockIndex
    {
        /// <summary>
        /// key: block height, value: BlockIndexEntry
        /// </summary>
        public static Dictionary<Int32, BlockIndexEntry> _heightToBlockIndex = new Dictionary<Int32, BlockIndexEntry>();

        /// <summary>
        /// Value of %appdata%,
        /// something like C:\Users\chmau\AppData\Roaming
        /// </summary>
        private static string _appdata;

        /// <summary>
        /// Something like C:\Users\chmau\AppData\Roaming\bitcoin\blocks\index
        /// </summary>
        public static string _blockIndexDbName;
        public static string BlockIndexDbName => _blockIndexDbName;

        static BlockIndex()
        {
            // C:\Users\chmau\AppData\Roaming
            _appdata = Environment.GetEnvironmentVariable("appdata");

            // C:\Users\chmau\AppData\Roaming\bitcoin\blocks\index
            _blockIndexDbName = _appdata + @"\bitcoin\blocks\index";
        }

        public static void Initialize()
        {
            if (_heightToBlockIndex.Count() > 0)
            {
                return;
            }

            var options = new Options
            {
                CreateIfMissing = true
            };
            var readOptions = new ReadOptions();

            using (DB db = DB.Open(BlockIndex._blockIndexDbName, options))
            {
                using (var iterator = db.NewIterator(readOptions))
                {
                    for (iterator.SeekToFirst(); iterator.Valid(); iterator.Next())
                    {
                        byte[] key = iterator.Key().ToArray();
                        if (Tools.DebugLogLevel > 0)
                        {
                            Console.WriteLine($"Block hash: {Tools.BytesToHexString(key)}");
                        }
                        {
                            byte[] value = iterator.Value().ToArray();
                            BlockIndexEntry entry = BlockIndexEntry.Parse(value);

                            // create this ampping if it does not exist yet, or update with the latest block with this height
                            _heightToBlockIndex[entry.height] = entry;
                        }
                    }
                }
            }
        }

        public static void Put(byte[] hash, BlockIndexEntry entry)
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

                //_heightToBlockIndex.Add(entry.height, entry);
            }
        }

        public static bool HashExists(byte[] hash, out BlockIndexEntry entry)
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
                    if (Tools.DebugLogLevel > 0)
                    {
                        Tools.ConsoleWriteLine($"Hash {Tools.BytesToHexString(hash)} exists:");
                        Tools.PrintJsonObject(entry);
                    }
                }
                else
                {
                    if (Tools.DebugLogLevel > 0)
                    {
                        Tools.ConsoleWriteLine($"Hash {Tools.BytesToHexString(hash)} does not exist.");
                    }
                }
            }

            return success;
        }

        public static byte[] GetPrevHash(byte[] hash)
        {
            BlockIndexEntry entry;
            if (BlockIndex.HashExists(hash, out entry))
            {
                return entry._prevBlockHash;
            }
            return null;
        }

        /// <summary>
        /// Create a block locator hash list from the specified tip hash.
        /// The result does not include the VarInt length prefix.
        /// The result is a concatenation of 32-byte hashes. 
        /// Each hash is in big-endian and must be reversed for serialization.
        /// </summary>
        /// <param name="tipHash"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] CreateBlockLocatorHashes(string tipHash)
        {
            byte[] bHash = Tools.HexStringToBytes(tipHash);
            if (bHash == null || bHash.Length != 32)
                throw new ArgumentException("Tip hash muss 32 Bytes haben.");

            List<byte[]> locator = new List<byte[]>();
            byte[] current = bHash;
            int step = 1;
            int count = 0;

            while (current != null)
            {
                locator.Add(current);
                count++;

                if (count > 10)
                {
                    step *= 2;
                }

                for (int i = 0; i < step; i++)
                {
                    current = GetPrevHash(current);
                    if (current == null)
                        break;
                }
            }

            // Genesis-Hash sicher hinzufügen, falls er noch nicht enthalten ist
            if (!locator[locator.Count - 1].SequenceEqual(Block.GENESIS_BLOCK_HASH))
                locator.Add(Block.GENESIS_BLOCK_HASH);

            int hashSize = 32;
            byte[] result = new byte[locator.Count * hashSize];

            for (int i = 0; i < locator.Count; i++)
            {
                Array.Copy(locator[i], 0, result, i * hashSize, hashSize);
            }

            return result.ToArray();
        }
    }
}
