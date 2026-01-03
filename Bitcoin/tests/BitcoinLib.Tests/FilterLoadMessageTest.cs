using BitcoinLib;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net.Security;
using BitcoinLib.Network;

namespace BitcoinLib.Test
{
    public class FilterLoadMessageTest : UnitTest
    {

        public static void test_filterload()
        {
            BloomFilter filter = new BloomFilter(10, 5, 99);

            byte[] data = Tools.AsciiStringToBytes("Hello World");
            filter.Add(data);
            data = Tools.AsciiStringToBytes("Goodbye!");
            filter.Add(data);

            string expected = "0a4000600a080000010940050000006300000001";
            FilterLoadMessage msg = filter.CreateFilterLoadMessage(1);
            byte[] msgData = msg.Serialize();
            string actual = Tools.BytesToHexString(msgData);

            AssertEqual(expected, actual);
        }
    }
}
