using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Numerics;
using System.Globalization;

namespace BitcoinLib.Test
{
    public class ToolsTest : UnitTest
    {
        public static void test_join_array()
        {
            object[] data =
            {
                new byte[]{ 1, 2, 3, 4 }, new byte[] { 6, 7, 8, 9 }, 0, -1, 0, -1, new byte[] { 1, 2, 3, 4, 6, 7, 8, 9 },
                new byte[]{ 1, 2, 3, 4 }, new byte[] { 6, 7, 8, 9 }, 0,  1, 2,  2, new byte[] { 1, 8, 9 },
                new byte[]{ 1, 2, 3, 4 }, new byte[] { 6, 7, 8, 9 }, 1,  3, 0,  2, new byte[] { 2, 3, 4, 6, 7 },
            };

            for (int i = 0; i < data.Length; i++)
            {
                byte[] data1 = (byte[])data[i++];
                byte[] data2 = (byte[])data[i++];
                int data1_start = (int)data[i++];
                int data1_stop = (int)data[i++];
                int data2_start = (int)data[i++];
                int data2_stop = (int)data[i++];
                byte[] expected = (byte[])data[i];

                byte[] calculated = ArrayHelpers.JoinArrays(data1, data1_start, data1_stop, data2, data2_start, data2_stop);

                AssertEqual(expected, calculated);
            }
        }

        public static void test_to_byte()
        {
            byte[] data_1 = { 0, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };
            byte[] data_1_little = { 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11, 0 };

            byte[] data_2 = { 0, 0, 0, 0, 0, 0, 0, 0x88 };
            byte[] data_2_little = { 0x88, 0, 0, 0, 0, 0, 0, 0 };

            BigInteger n = 1;

            byte[] data = Tools.ToBytes(0x1122334455667788, 9, "big");
            AssertEqual(data, data_1);
            data = Tools.ToBytes(0x1122334455667788, 9, "little");
            AssertEqual(data, data_1_little);

            data = Tools.ToBytes(0x88, 8, "big");
            AssertEqual(data, data_2);
            data = Tools.ToBytes(0x88, 8, "little");
            AssertEqual(data, data_2_little);

            AssertException(typeof(IndexOutOfRangeException), () => Tools.ToBytes(0x1122334455667788, 7, "big"));
            AssertException(typeof(IndexOutOfRangeException), () => Tools.ToBytes(0x1122334455667788, 7, "little"));
        }

        public static void test_hash256()
        {
            // this data
            string s1 = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";
            // must produce this hash value
            string s2 = "03ee4f7a4e68f802303bc659f8f817964b4b74fe046facc3ae1be4679d622c45";

            byte[] s1_bytes = Tools.HexStringToBytes(s1);
            byte[] s2_bytes = Tools.HexStringToBytes(s2);

            byte[] result = Tools.Hash256(s1_bytes);
            AssertEqual(result, s2_bytes);
        }

        public static void test_sha1_collision()
        {
            /// these two different inputs must produce the same SHA1 hash value
            string s1 = @"255044462d312e330a25e2e3cfd30a0a0a312030206f626a0a3c3c2f576964746820
32203020522f4865696768742033203020522f547970652034203020522f537562747970652035
203020522f46696c7465722036203020522f436f6c6f7253706163652037203020522f4c656e67
74682038203020522f42697473506572436f6d706f6e656e7420383e3e0a73747265616d0affd8
fffe00245348412d3120697320646561642121212121852fec092339759c39b1a1c63c4c97e1ff
fe017f46dc93a6b67e013b029aaa1db2560b45ca67d688c7f84b8c4c791fe02b3df614f86db169
0901c56b45c1530afedfb76038e972722fe7ad728f0e4904e046c230570fe9d41398abe12ef5bc
942be33542a4802d98b5d70f2a332ec37fac3514e74ddc0f2cc1a874cd0c78305a215664613097
89606bd0bf3f98cda8044629a1".Trim();

            string s2 = @"255044462d312e330a25e2e3cfd30a0a0a312030206f626a0a3c3c2f576964746820
32203020522f4865696768742033203020522f547970652034203020522f537562747970652035
203020522f46696c7465722036203020522f436f6c6f7253706163652037203020522f4c656e67
74682038203020522f42697473506572436f6d706f6e656e7420383e3e0a73747265616d0affd8
fffe00245348412d3120697320646561642121212121852fec092339759c39b1a1c63c4c97e1ff
fe017346dc9166b67e118f029ab621b2560ff9ca67cca8c7f85ba84c79030c2b3de218f86db3a9
0901d5df45c14f26fedfb3dc38e96ac22fe7bd728f0e45bce046d23c570feb141398bb552ef5a0
a82be331fea48037b8b5d71f0e332edf93ac3500eb4ddc0decc1a864790c782c76215660dd3097
91d06bd0af3f98cda4bc4629b1".Trim();

            s1 = String.Concat(s1.Where(c => !Char.IsWhiteSpace(c)));
            s2 = String.Concat(s2.Where(c => !Char.IsWhiteSpace(c)));

            byte[] c1 = Tools.HexStringToBytes(s1);
            byte[] c2 = Tools.HexStringToBytes(s2);

            byte[] sha1S1 = Tools.SHA1(c1);
            byte[] sha1S2 = Tools.SHA1(c2);

            AssertEqual(sha1S1, sha1S2);

        }

        public static void test_from_bytes()
        {
            byte[] data = { 0x12, 0x34 };

            BigInteger x = Tools.BigIntegerFromBytes(data, "big");
            AssertTrue(x == 0x1234);

            x = Tools.BigIntegerFromBytes(data, "small");
            AssertTrue(x == 0x3412);
        }


        public static void test_from_lstrip()
        {
            byte[] data1 = { 0x00, 0x00, 0x12 };
            byte[] data2 = { 0x00, 0x00, 0x00 };
            byte[] l0 = { };
            byte[] l1 = { 0x12 };

            byte[] x = Tools.Lstrip(data1, 0);
            AssertEqual(x, l1);

            x = Tools.Lstrip(data1, 0x12);
            AssertEqual(x, data1);

            x = Tools.Lstrip(data2, 0);
            AssertEqual(x, l0);

        }

        public static void test_chapter_4_ex_7_8_9()
        {
            ConsoleOutWriteLine("create a testnet address from a private key and remember this private key");

            object[] data =
                {
                new byte[] { 0x12, 0x34 }, "big", 0x1234,
                new byte[] { 0x12, 0x34 }, "little", 0x3412,
            };

            for (int i = 0; i < data.Length; i++)
            {
                byte[] bytes = (byte[])data[i++];
                string endian = (string)data[i++];
                int expected = (int)data[i];

                BigInteger calculated = Tools.BigIntegerFromBytes(bytes, endian);
                AssertTrue(expected == calculated);
            }

            // p 263
            string passphrase = "ch.maurer@gmx.de sein secret ist streng geheim";
            BigInteger secret = Tools.BigIntegerFromBytes(Tools.Hash256(Tools.AsciiStringToBytes(passphrase)), "little");
            PrivateKey priv = new PrivateKey(secret);
            string publicKey = priv._point.ToString();
            string address = priv._point.Address(true, true);

            //
            // https://bitcoinfaucet.uo1.net/send.php only accepts tb* - Bech32 coded addresses
            //
            // https://coinfaucet.eu/en/btc-testnet/ ok!
            // das ist meine Adresse für das Testen in diesem Buch:
            // 
            // Passphrase = 'ch.maurer@gmx.de sein secret ist streng geheim'
            // Private key = 14571969162455461715208563475110979223892067646684683083960296457821644624115
            // Public key = S256Point(0x21662d381ffe1d5d85c08e6d313735fb075ff1166a30e53230f6158e5fedf1a4, 0x1b8ce0333112898e7e53473b8e3e1b965303fadf3cfe4cb97117bb8d4d7d49bb)
            // Public address = mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx

            // ch.maurer@gmx.de sein secret ist streng geheim / 14571969162455461715208563475110979223892067646684683083960296457821644624115 / mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            // jimmy@programmingblockchain.com my secret      / 3732009510557459433907050332202055574431184625603108795736701456062809759494  / mft9LRNtaBNtpkknB8xgm17UvPedZ4ecYL p263
            Console.WriteLine("Passphrase='" + passphrase + "'");
            Console.WriteLine("Private key=" + secret);
            Console.WriteLine("Public key=" + publicKey);
            Console.WriteLine("Public address=" + address);
            
        }

        public static void test_reverse()
        {
            byte[] data = { 0x12, 0x34, 0x56, 0x78 };
            byte[] expected = { 0x78, 0x56, 0x34, 0x12 };

            Tools.Reverse(data);
            AssertEqual(expected, data);

            data = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            expected = new byte[] { 0x78, 0x56, 0x34, 0x12 };
            byte[] calculated = Tools.ReverseCopy(data);
            AssertEqual(expected, calculated);

        }
    }
}
