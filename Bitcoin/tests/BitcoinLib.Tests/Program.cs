using BitcoinLib.Network;
using BitcoinLib.Storage;

namespace BitcoinLib.Test
{
    /*
     * All references are to the book
     * Programming Bitcoin - learn how to program bitcoin from scratch
     * Jimmy Song
     * 2019
     * https://github.com/jimmysong/programmingbitcoin
     */

    /*
     * TODO: GetUrlContent() does not work from C#, works fine in Chrome or any other browser.
     * TODO: investigate the Murmur3 code!
     * TODO: BloomFilter is deprecated! what else?
     * TODO: check that all python code exists in C#
     */
    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">--debugDumpStack true</param>
        /// <returns></returns>
        public static void Main(string[] args)
        {
            //Tools.CallStaticMethod("BitcoinLib.Test.PublicKeyTest", "test_decode");
            //return;

            ProcessArgs(args);
            //Console.WriteLine("Main finished");
        }

        public static void RunTests()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "Various");
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "RunAllTests");
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "RunChapters");
        }

        public static void PrintInfo()
        {
            SimpleNode nodeMainnet = new SimpleNode(false);
            SimpleNode nodeTestnet = new SimpleNode(true);
            Tools.ConsoleWriteHeader("Info:", false);
            Console.WriteLine(
@$"    genesis block hash: 000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f
    current mainnet host is: {nodeMainnet._host}:{nodeMainnet._port}
    current testnet host is: {nodeTestnet._host}:{nodeTestnet._port}
    {BlockFileStorage.GetNumberOfBlockFiles()} blkxxxxx.dat files are stored at: {BlockFileStorage.BlocksDir}
    Block index path: {BlockIndex.BlockIndexDbName}
    _blockHashToSuccessor has {BlockFileStorage._successors.Count} entries
    _heightToBlockIndex has {BlockIndex._heightToBlockIndex.Count} entries
"
);
            Tools.ConsoleWriteHeader("RAM Dictionary _heightToBlockIndex (height -> BlockIndexEntry):", false);
            Tools.PrintJsonObject(BlockIndex._heightToBlockIndex);

            Tools.ConsoleWriteHeader("RAM Dictionary _successors (hash -> List<hash>):", false);
            Tools.PrintJsonObject(BlockFileStorage._successors);

            Tools.ConsoleWriteHeader("All blocks from blkxxxxx.dat files:", false);
            BlockManager.PrintAllBlocks(false);
        }

        public static void Usage()
        {
            string exeName = Path.GetFileName(Environment.ProcessPath);

            Console.WriteLine($"{exeName} [options]" + Environment.NewLine +
@"    --debugDumpScriptStacks
        print thge script stacks during script execution
    --debugLogTime
        log time taken for various function calls
    --debugLogLevel [0, 1, 2, 3]
    --addGenesisBlock
        add the genesis block to the block index
    --getBlockWithPrevHash [block-hash]
        retrieve the block whose block header contains the given block from the bitcoin server
    --addBlockWithPrevHash [block-hash]
        same as --getBlockWithPrevHash but also saves the block in storage
    --getBlockWithHash [block-hash]
        add the block with the given block hash to the block index    
    --addBlockWithHash [block-hash]
        same as --getBlockWithHash but also saves the block in storage
    --printBlock [hash]
    --host [65.109.24.172 | dnsseed.bluematt.me]
    --runTests
    --runTest [BitcoinLib.Test.MerkleTreeTest] [test_merkle_tree_populate_2]
    --printAllBlocks [0 | 1: print entire block]
        print all blocks in the block index
    --decodePublicKey [0496b538e853519c726a2c91e61ec11600ae1390813a627c66fb8be7947be63c52da7589379515d4e0a604f8141781e62294721166bf621e73a82cbf2342c858ee]
                [1BoatSLRHtKNngkdXEeobR76b53LETtpyT] or [bc1qetlt40k2l6atajh7h2lv4l46hm90aw476h9vq9]
    --info

        Info:
        genesis block hash: 000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f

");
        }

        /// <summary>
        /// --debugDumpStack [1,0] --logTime [1,0] --logLevel [0, 1, 2, 3]
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool ProcessArgs(string[] args)
        {
            bool bAddGenesisBlock = false;
            bool bGetBlockWithPrevHash = false;
            string strGetPrevHash = "";
            bool bAddBlockWithPrevHash = false;
            string strAddPrevHash = "";
            bool bGetBlockWithHash = false;
            string strGetHash = "";
            bool bAddBlockWithHash = false;
            string strAddHash = "";
            bool bPrintBlock = false;
            string strPrintBlock = "";
            bool bPrintAllBlocks = false;
            string strPrintAllBlocks = "";
            bool bRunTests = false;
            bool bRunTest = false;
            string strRunTestClass = "";
            string strRunTestMethod = "";
            bool bDecodePublicKey = false;
            string strDecodePublicKey = "";
            bool bInfo = false;

            if (args.Length == 0)
            {
                Usage();
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];

                if (string.Equals(key, "--debugDumpScriptStacks", StringComparison.OrdinalIgnoreCase))
                {
                    Script.DebugDumpScriptStacks = true;
                }
                else if (string.Equals(key, "--debugNoOnline", StringComparison.OrdinalIgnoreCase))
                {
                    Tools.DebugNoOnline = true;
                }
                else if (string.Equals(key, "--debugLogTime", StringComparison.OrdinalIgnoreCase))
                {
                    Tools.DebugLogTime = true;
                }
                else if (string.Equals(key, "--debugLogLevel", StringComparison.OrdinalIgnoreCase))
                {
                    if (i < args.Length - 1)
                    {
                        string value = args[i++ + 1];
                        Int32.TryParse(value, out Tools.DebugLogLevel);
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --debugLogLevel");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--addGenesisBlock", StringComparison.OrdinalIgnoreCase))
                {
                    bAddGenesisBlock = true;
                }
                else if (string.Equals(key, "--printAllBlocks", StringComparison.OrdinalIgnoreCase))
                {
                    bPrintAllBlocks = true;
                    if (i < args.Length - 1)
                    {
                        strPrintAllBlocks = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --printAllBlocks");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--decodePublicKey", StringComparison.OrdinalIgnoreCase))
                {
                    bDecodePublicKey = true;
                    if (i < args.Length - 1)
                    {
                        strDecodePublicKey = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --decodePublicKey");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--runTests", StringComparison.OrdinalIgnoreCase))
                {
                    bRunTests = true;
                }
                else if (string.Equals(key, "--info", StringComparison.OrdinalIgnoreCase))
                {
                    bInfo = true;
                }
                else if (string.Equals(key, "--runTest", StringComparison.OrdinalIgnoreCase))
                {
                    bRunTest = true;
                    if (i < args.Length - 2)
                    {
                        strRunTestClass = args[i++ + 1];
                        strRunTestMethod = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --runTest");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--addBlockWithPrevHash", StringComparison.OrdinalIgnoreCase))
                {
                    if (i < args.Length - 1)
                    {
                        bAddBlockWithPrevHash = true;
                        strAddPrevHash = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --addBlockWithPrevHash");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--getBlockWithPrevHash", StringComparison.OrdinalIgnoreCase))
                {
                    if (i < args.Length - 1)
                    {
                        bGetBlockWithPrevHash = true;
                        strGetPrevHash = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --getBlockWithPrevHash");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--addBlockWithHash", StringComparison.OrdinalIgnoreCase))
                {
                    if (i < args.Length - 1)
                    {
                        bAddBlockWithHash = true;
                        strAddHash = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --addBlockWithHash");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--getBlockWithHash", StringComparison.OrdinalIgnoreCase))
                {
                    if (i < args.Length - 1)
                    {
                        bGetBlockWithHash = true;
                        strGetHash = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --getBlockWithHash");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--printBlock", StringComparison.OrdinalIgnoreCase))
                {
                    if (i < args.Length - 1)
                    {
                        bPrintBlock = true;
                        strPrintBlock = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --printBlock");
                        Usage();
                        return false;
                    }
                }
                else if (string.Equals(key, "--host", StringComparison.OrdinalIgnoreCase))
                {
                    if (i < args.Length - 1)
                    {
                        SimpleNode.DefaultHost = args[i++ + 1];
                    }
                    else
                    {
                        Console.WriteLine("missing value for option --host");
                        Usage();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Unknown switch '" + key + "'");
                    Usage();
                    return false;
                }
            }

            BlockManager.Initialize();

            if (bInfo)
            {
                PrintInfo();
            }
            if (bAddGenesisBlock)
            {
                BlockManager.AddGenesisBlock();
            }
            if (bPrintAllBlocks)
            {
                BlockManager.PrintAllBlocks(strPrintAllBlocks.Equals("1"));
            }
            if (bAddBlockWithHash)
            {
                BlockManager.AddBlockWithHash(strAddHash);
            }
            if (bGetBlockWithHash)
            {
                BlockManager.GetBlockWithHash(strGetHash, false);
            }
            if (bAddBlockWithPrevHash)
            {
                BlockManager.GetBlockWithPrevHash(strAddPrevHash, true);
            }
            if (bGetBlockWithPrevHash)
            {
                BlockManager.GetBlockWithPrevHash(strGetPrevHash, false);
            }
            if (bPrintBlock)
            {
                BlockManager.PrintBlock(strPrintBlock);
            }
            if (bRunTests)
            {
                RunTests();
            }
            if (bRunTest)
            {
                Tools.CallStaticMethod(strRunTestClass, strRunTestMethod);
            }
            if (bDecodePublicKey)
            {
                string actual = new PublicKey().DecodePublicKey(strDecodePublicKey);
                Console.WriteLine($"pubkey_hex {strDecodePublicKey}");
                Console.WriteLine($"decoded: {actual}");
            }

            return true;
        }
    }
}
