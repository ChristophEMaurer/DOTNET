using BitcoinLib;
using BitcoinLib;
using BitcoinLib.BIP39;
using BitcoinLib.Network;
using BitcoinLib.Storage;
using BitcoinLib.Visitors;
using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitcoinLib.Test
{
    /*
     * All references are to the book
     * Programming Bitcoin - learn how to program bitcoin from scratch
     * Jimmy Song
     * 2019
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
            ProcessArgs(args);
            //Console.WriteLine("Main finished");
        }


        public static void RunTests()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "Various");
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "RunAllTests");
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "RunChapters");
        }

        public static void Usage()
        {
            string exeName = Path.GetFileName(Environment.ProcessPath);

            Console.WriteLine($"{exeName} [options]" + Environment.NewLine +
@"    --debugDumpStack [1,0]
    --loggingTime [1,0]
    --logLevel [0, 1, 2, 3]
    --addGenesisBlock
    --addBlockWithPrevHash [hash]
    --addBlockWithHash [hash]
    --printBlock [hash]
    --host [65.109.24.172 | dnsseed.bluematt.me]
    --runTests
"
);
        }

        /// <summary>
        /// --debugDumpStack [1,0] --logTime [1,0] --logLevel [0, 1, 2, 3]
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool ProcessArgs(string[] args)
        {
            bool bAddGenesisBlock = false;
            bool bAddBlockWithPrevHash = false;
            string strAddPrevHash = "";
            bool bAddBlockWithHash = false;
            string strAddHash = "";
            bool bPrintBlock = false;
            string strPrintBlock = "";

            if (args.Length == 0)
            {
                Usage();
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];

                if (key.Length < 3)
                {
                    Usage();
                    return false;
                }
                if (!key.StartsWith("--"))
                {
                    Usage();
                    return false;
                }

                if (key == "--dumpScriptStacks")
                {
                    if (i < args.Length - 1)
                    {
                        string value = args[i++ + 1];
                        Script.DEBUG_DUMP_STACKS = (value == "1");
                    }
                    else
                    {
                        Usage();
                        return false;
                    }
                }
                else if (key == "--logTime")
                {
                    if (i < args.Length - 1)
                    {
                        string value = args[i++ + 1];
                        Tools.LOGGING_TIME = (value == "1");
                    }
                    else
                    {
                        Usage();
                        return false;
                    }
                }
                else if (key == "--logLevel")
                {
                    if (i < args.Length - 1)
                    {
                        string value = args[i++ + 1];
                        Tools.LOGGING = Int32.Parse(value);
                    }
                    else
                    {
                        Usage();
                        return false;
                    }
                }
                else if (key == "--addGenesisBlock")
                {
                    bAddGenesisBlock = true;
                }
                else if (key == "--runTests")
                {
                    RunTests();
                    return false;
                }
                else if (key == "--addBlockWithPrevHash")
                {
                    if (i < args.Length - 1)
                    {
                        bAddBlockWithPrevHash = true;
                        strAddPrevHash = args[i++ + 1];
                    }
                    else
                    {
                        Usage();
                        return false;
                    }
                }
                else if (key == "--addBlockWithHash")
                {
                    if (i < args.Length - 1)
                    {
                        bAddBlockWithHash = true;
                        strAddHash = args[i++ + 1];
                    }
                    else
                    {
                        Usage();
                        return false;
                    }
                }
                else if (key == "--printBlock")
                {
                    if (i < args.Length - 1)
                    {
                        bPrintBlock = true;
                        strPrintBlock = args[i++ + 1];
                    }
                    else
                    {
                        Usage();
                        return false;
                    }
                }
                else if (key == "--host")
                {
                    if (i < args.Length - 1)
                    {
                        SimpleNode.DefaultHost = args[i++ + 1];
                    }
                    else
                    {
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

            if (bAddGenesisBlock)
            {
                BlockIndex.AddGenesisBlock();
            }
            if (bAddBlockWithHash)
            {
                BlockIndex.AddBlockWithHash(strAddHash);
            }
            if (bAddBlockWithPrevHash)
            {
                BlockIndex.AddBlockWithPrevHash(strAddPrevHash);
            }
            if (bPrintBlock)
            {
                BlockIndex.PrintBlock(strPrintBlock);
            }

            return true;
        }
    }
}
