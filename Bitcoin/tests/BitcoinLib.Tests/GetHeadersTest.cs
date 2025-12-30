using BitcoinLib.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace BitcoinLib.Test
{
    public class GetHeadersTest : UnitTest
    {
        public static void test_serialize()
        {
            byte[] raw_start_block = Tools.HexStringToBytes("0000000000000000001237f46acddf58578a37e213d2a6edc4884a2fcad05ba3");

            GetHeadersMessage gh = new GetHeadersMessage(70015, 1,raw_start_block);
            byte[] serialized = gh.Serialize();
            string strSerialized = Tools.BytesToHexString(serialized);
            string want = "7f11010001a35bd0ca2f4a88c4eda6d213e2378a5758dfcd6af437120000000000000000000000000000000000000000000000000000000000000000000000000000000000";

            bool success = want.Equals(strSerialized);
            AssertTrue(success);
        }
    }
}

