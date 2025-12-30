using BitcoinLib;
using Org.BouncyCastle.Crypto.Digests;
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
using System.Threading.Tasks;

namespace BitcoinLib.Test
{
    /*
     * All references are to the book
     * Programming Bitcoin - learn how to program bitcoin from scratch
     * Jimmy Song
     * 2019
     */
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task Main()
        {
  

            if (true)
            {
                MerkleTreeTest.test_merkle_tree();

                MerkleTree tree = new MerkleTree(9);
                Console.WriteLine(tree.ToString());

            }
            else
            {
                Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "Various");
                Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "RunAllTests");
                Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "RunChapters");
            }
        }

    }
}
