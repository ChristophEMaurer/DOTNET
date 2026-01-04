using BitcoinLib;
using BitcoinLib.BIP39;
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


    /*
     * TODO: GetUrlContent() does not work from C#, works fine in Chrome or any other browser.
     * TODO: investigate the Murmur3 code!
     * TODO: BloomFilter is deprecated! what else?
     * TODO: check that all python code exists in C#
     */
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task Main()
        {
            Tools.LOGGING_TIME = false;

            if (false)
            {
                Tools.LOGGING = 3;

                Tools.CallStaticMethod("BitcoinLib.Test.Ripemd160Test", "test_ripemd160_chatgpt");
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
