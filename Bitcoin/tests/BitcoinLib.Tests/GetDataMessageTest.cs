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
    public class GetDataMessageTest : UnitTest
    {

        public static void test_serialize()
        {
            string strWant = "020300000030eb2540c41025690160a1014c577061596e32e426b712c7ca00000000000000030000001049847939585b0652fba793661c361223446b6fc41089b8be00000000000000";

            GetDataMessage msg = new GetDataMessage();

            byte[] block1 = Tools.HexStringToBytes("00000000000000cac712b726e4326e596170574c01a16001692510c44025eb30");
            msg.Add(GetDataMessage.MSG_FILTERED_BLOCK, block1);

            byte[] block2 = Tools.HexStringToBytes("00000000000000beb88910c46f6b442312361c6693a7fb52065b583979844910");
            msg.Add(GetDataMessage.MSG_FILTERED_BLOCK, block2);

            byte[] bActual = msg.Serialize();
            string strActual = Tools.BytesToHexString(bActual);
            AssertEqual(strWant, strActual);
        }
    }
}
