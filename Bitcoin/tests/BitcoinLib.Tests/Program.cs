using BitcoinLib;
using BitcoinLib.BIP39;
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

using BitcoinLib;

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
            Script.DEBUG_DUMP_STACKS = false;
            Tools.LOGGING_TIME = false;
            Tools.LOGGING = 3;

            if (false)
            {
                //Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2pkh"); // ok
                //Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2sh"); // ok
                //Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2wpkh"); // ok
                //Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2sh_p2wpkh"); // OK
                //Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2wsh"); //ok
                Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2sh_p2wsh"); // ok
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
