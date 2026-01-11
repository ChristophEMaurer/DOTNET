using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib;

namespace BitcoinLib.Test
{
    public class BlockTest : UnitTest
    {
        public static void test_serialize()
        {
            // this is the genesis block!
            string filePath =  @"D:\Daten\Develop\bitcoin-blockchain\block.0"; // Pfad zur Datei
            byte[] wanted = File.ReadAllBytes(filePath);
            string strWanted = Tools.BytesToHexString(wanted);
            Block block = Block.Parse(wanted);
            byte[] actual = block.serialize_total();
            string strActual = Tools.BytesToHexString(actual);

            bool success = wanted.SequenceEqual(actual);

            AssertTrue(success);
        }
    }
}
