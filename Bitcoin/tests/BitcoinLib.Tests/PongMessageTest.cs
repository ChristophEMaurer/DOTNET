using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib.Network;

namespace BitcoinLib.Test
{
    public class PongMessageTest : UnitTest
    {
        public static void test_serialize()
        {
            PongMessage message = new PongMessage(0x1234567887654321);

            byte[] bSerialized = message.Serialize();
            string strSerialized = Tools.BytesToHexString(bSerialized);

            string strWant = "2143658778563412";
            
            bool success = strSerialized.Equals(strWant);

            AssertTrue(success);
        }
    }
}
