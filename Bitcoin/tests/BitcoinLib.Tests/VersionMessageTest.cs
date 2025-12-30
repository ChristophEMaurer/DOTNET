using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib.Network;

namespace BitcoinLib.Test
{
    public class VersionMessageTest : UnitTest
    {
        public static void test_serialize()
        {
            VersionMessage message = new VersionMessage("/programmingbitcoin:0.1/");

            byte[] bSerialized = message.Serialize();

            string strSerialized = Tools.BytesToHexString(bSerialized);
            string strWant = "7f11010000000000000000000000000000000000000000000000000000000000000000000000ffff00000000208d000000000000000000000000000000000000ffff00000000208d0000000000000000182f70726f6772616d6d696e67626974636f696e3a302e312f0000000000";
            
            bool success = strSerialized.Equals(strWant);

            AssertTrue(success);
        }
    }
}
