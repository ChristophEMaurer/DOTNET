using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib.Network;

namespace BitcoinLib.Test
{
    public class PingMessageTest : UnitTest
    {
        public static void test_serialize()
        {
            PingMessage message = new PingMessage(0x1234567898765432);

            byte[] bSerialized = message.Serialize();
            string strSerialized = Tools.BytesToHexString(bSerialized);

            string strWant = "3254769878563412";
            
            bool success = strSerialized.Equals(strWant);

            AssertTrue(success);
        }
    }
}
