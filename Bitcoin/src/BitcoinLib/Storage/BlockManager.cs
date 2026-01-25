using BitcoinLib.Network;
using LevelDB;
using System.Collections.Generic;

namespace BitcoinLib.Storage
{
    public class BlockManager
    {
        /*
         We have this data:
            - BlockFileStorage: the blkxxxxx.dat files containing the raw block data
            - BlockIndex: the block index database mapping block hashes to their location in the blkxxxxx.dat files

            - mapping from height to BlockIndex
            - mapping from block hash to successor block hashes (to traverse the chain forward)
        */

        static BlockManager()
        {
            Initialize();
        }

        public static void Initialize()
        {
            BlockIndex.Initialize();
            BlockFileStorage.Initialize();
        }

        public static void AddGenesisBlock()
        {
            Block block = Block.Parse(Block.GENESIS_BLOCK);
            ProcessNewBlock(block);
        }

        /// <summary>
        /// Process the new block: store it if not already existing, and update the block index and height mappings.
        /// and successor mappings.
        /// 
        /// Example: we receive a (genesis block), then c1,c2,c3,d1,d2,d3,e:
        /// a   b   c   d   e   f   g   
        /// 0       -1  -1  -1
        ///         -1  -1
        ///         -1  -1
        ///
        /// now we receive b: we can set the height of b to 1, and then update c1,c2,c3,d1,d2,d3,e accordingly:
        /// a   b   c   d   e   f   g   
        /// 0   1   -1  -1  -1
        ///         -1  -1
        ///         -1  -1

        /// </summary>
        /// <param name="block"></param>
        public static void ProcessNewBlock(Block block)
        {
            byte[] bHash = block._blockHeader.Hash();
            BlockIndexEntry bieBlock;

            if (BlockIndex.HashExists(bHash, out bieBlock))
            {
                // we already received this block, do nothing
                return;
            }

            // store the block in the blkxxxxx.dat file and update the successor mapping
            BlockWriteResult blockWriteResult = BlockFileStorage.AddBlock(block);

            // create and store the block index entry: start with height = -1 (unknown)
            BlockIndexEntry blockIndexEntry = new BlockIndexEntry(block._blockHeader._prevBlockHash,
                -1, blockWriteResult.FileNumber, blockWriteResult.Offset, blockWriteResult.Length, 0);
            if (block.IsGenesisBlock())
            {
                // genesis block has height 0
                blockIndexEntry._height = 0;
                BlockIndex._heightToBlockIndex[0] = blockIndexEntry;
            }
            BlockIndex.Put(bHash, blockIndexEntry);

            // now go back to the previous block and update the heights of all children accordingly.
            // one of the children is this block we just added.
            CalculateChildrenHeight(block._blockHeader._prevBlockHash);
        }


        /// <summary>
        /// Example: we receive a (genesis block), then c1,c2,c3,d1,d2,d3,e:
        /// a   b   c   d   e   f   g   
        /// 0       -1  -1  -1
        ///         -1  -1
        ///         -1  -1
        ///
        /// now we receive b: we can set the height of b to 1, and then update c1,c2,c3,d1,d2,d3,e accordingly:
        /// a   b   c   d   e   f   g   
        /// 0   1   -1  -1  -1
        ///         -1  -1
        ///         -1  -1
        /// </summary>
        /// <param name="hashParent"></param>Parent)
        public static void CalculateChildrenHeight(byte[] hashParent)
        {
            BlockIndexEntry bieParent;
            if (!BlockIndex.HashExists(hashParent, out bieParent))
            {
                // genesis block parent (= 0x0) or missing block
                return;
            }

            // get all children of this parent
            string strHashParent = Tools.BytesToHexString(hashParent);
            List<string> children;

            if (!BlockFileStorage._successors.TryGetValue(strHashParent, out children))
            {
                return;
            }

            foreach (string strChildHash in children)
            {
                byte[] bChildHash = Tools.HexStringToBytes(strChildHash);
                BlockIndexEntry bieChild;
                if (BlockIndex.HashExists(bChildHash, out bieChild))
                {
                    if (bieChild._height > 0)
                    {
                        // if the child has a height already, we do not update it
                        continue;
                    }

                    // child does not have a height, so increase by one
                    int newHeight = bieParent._height + 1;
                    bieChild._height = newHeight;

                    //update the new height in the index
                    BlockIndex.Put(bChildHash, bieChild);

                    // the last one of the parallel blocks wins: this is not correct but good enough for me!
                    BlockIndex._heightToBlockIndex[newHeight] = bieChild;

                    // process the children of this child
                    CalculateChildrenHeight(bChildHash);
                }
            }
        }

        public static void PrintAllBlocks(bool includeBlockInfo)
        {
            var options = new Options();
            var readOptions = new ReadOptions();

            using (DB db = DB.Open(BlockIndex._blockIndexDbName, options))
            {
                using (var iterator = db.NewIterator(readOptions))
                {
                    for (iterator.SeekToFirst(); iterator.Valid(); iterator.Next())
                    {
                        byte[] key = iterator.Key().ToArray();
                        Console.WriteLine($"Block hash: {Tools.BytesToHexString(key)}");
                        if (includeBlockInfo)
                        {
                            byte[] value = iterator.Value().ToArray();
                            BlockIndexEntry entry = BlockIndexEntry.Parse(value);
                            Tools.PrintJsonObject(entry);

                            BlockWriteResult blockWriteResult = new BlockWriteResult
                            {
                                FileNumber = entry._fileNumber,
                                Offset = entry._offset,
                                Length = entry.size
                            };
                            byte[] raw = BlockFileStorage.ReadBlockBytes(blockWriteResult);
                            Block block = Block.Parse(raw);
                            Tools.PrintJsonObject(block);
                        }
                    }
                }
            }
        }

        public static void GetBlockWithPrevHash(string hash, bool add)
        {
            // we cannot check if we already have this block because we do not have the hash
            Block block = SimpleNode.GetBlockWithPrevHash(hash);

            Console.WriteLine("Received block online:");
            Block.PrintBlock(block);

            if (add)
            {
                ProcessNewBlock(block);
            }
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
                ProcessNewBlock(block);
            }
        }


        /// <summary>
        /// Get the block with the specified hash online.
        /// Then, store it if requested. If it already exists, we do not store.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="add"></param>
        public static void GetBlockWithHash(string hash, bool add)
        {
            Block block = SimpleNode.GetBlockWithHash(hash);
            Console.WriteLine("Received block online:");
            Block.PrintBlock(block);

            if (add)
            {
                ProcessNewBlock(block);
            }
        }

        public static void PrintBlock(string hash)
        {
            byte[] bHash = Tools.HexStringToBytes(hash);
            BlockIndexEntry entry;

            if (BlockIndex.HashExists(bHash, out entry))
            {
                BlockWriteResult blockWriteResult = new BlockWriteResult
                {
                    FileNumber = entry._fileNumber,
                    Offset = entry._offset
                };
                byte[] raw = BlockFileStorage.ReadBlockBytes(blockWriteResult);
                Block block = Block.Parse(raw);
                Console.WriteLine("Block:");
                Tools.PrintJsonObject(block);
            }
        }

        /// <summary>
        /// Request all blocks from our connected peer and add them to our storage.
        /// </summary>
        public static void AddAllBlocks()
        {
            AddGenesisBlock();

            byte[] currentHash = Block.GENESIS_BLOCK_HASH;
            while (true)
            {
                List <BlockHeader> blockHeaders = SimpleNode.GetBlockHeaders(Tools.BytesToHexString(currentHash));


                byte[] prevHash = BlockIndex.GetPrevHash(currentHash);
                if (prevHash == null)
                    break;
                Block block = SimpleNode.GetBlockWithPrevHash(Tools.BytesToHexString(currentHash));
                Console.WriteLine("Received block online:");
                Block.PrintBlock(block);
                ProcessNewBlock(block);
                currentHash = prevHash;
            }
        }
    }
}
