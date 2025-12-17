using BitcoinLib;
using System;
using System.Reflection;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

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
        public static void Main()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "Various");
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "RunAllTests");
            Tools.CallStaticMethod("BitcoinLib.Test.BitcoinLibTest", "RunChapters");
        }
    }
}
