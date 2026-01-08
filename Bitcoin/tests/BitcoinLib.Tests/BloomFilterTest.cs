using BitcoinLib;
using BitcoinLib.Network;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BitcoinLib.Test
{
    public class BloomFilterTest : UnitTest
    {
        public static void test_chapter_12_p212()
        {
            int bit_field_size = 10;
            byte[] bit_field = new byte[bit_field_size];
            byte[] raw = Tools.AsciiStringToBytes("hello world");
            byte[] hash = Tools.Hash256(raw);
            BigInteger biIndex = Tools.BigIntegerFromBytes(hash, "big");
            biIndex = biIndex % bit_field_size;
            int index = (int)biIndex;
            bit_field[index] = 1;

            Console.WriteLine("[" + BitConverter.ToString(bit_field).Replace("-", ", ") + "]");
        }

        public static void test_chapter_12_p213()
        {
            int bit_field_size = 10;
            byte[] bit_field = new byte[bit_field_size];

            string[] data = ["hello world", "goodbye"];

            foreach (string b in data)
            {
                byte[] raw = Tools.AsciiStringToBytes(b);
                byte[] hash = Tools.Hash256(raw);
                BigInteger biIndex = Tools.BigIntegerFromBytes(hash, "big");
                biIndex = biIndex % bit_field_size;
                int index = (int)biIndex;
                bit_field[index] = 1;
            }

            Console.WriteLine("[" + BitConverter.ToString(bit_field).Replace("-", ", ") + "]");
        }

        public static void test_chapter_12_ex_1()
        {
            int bit_field_size = 10;
            byte[] bit_field = new byte[bit_field_size];

            string[] data = ["hello world", "goodbye"];

            foreach (string b in data)
            {
                byte[] raw = Tools.AsciiStringToBytes(b);
                byte[] hash = Tools.Hash160(raw);
                BigInteger biIndex = Tools.BigIntegerFromBytes(hash, "big");
                biIndex = biIndex % bit_field_size;
                int index = (int)biIndex;
                bit_field[index] = 1;
            }

            Console.WriteLine("[" + BitConverter.ToString(bit_field).Replace("-", ", ") + "]");
        }

        public static void test_chapter_12_p214()
        {
            Func<byte[], byte[]>[] hashFunctions = new Func<byte[], byte[]>[]
            {
                Tools.Hash256,
                Tools.Hash160,
            };

            int bit_field_size = 10;
            byte[] bit_field = new byte[bit_field_size];

            string[] data = ["hello world", "goodbye"];

            foreach (string b in data)
            {
                foreach (var hashFunction in hashFunctions)
                {
                    byte[] raw = Tools.AsciiStringToBytes(b);
                    byte[] hash = hashFunction(raw);
                    BigInteger biIndex = Tools.BigIntegerFromBytes(hash, "big");
                    biIndex = biIndex % bit_field_size;
                    int index = (int)biIndex;
                    bit_field[index] = 1;
                }
            }

            Console.WriteLine("[" + BitConverter.ToString(bit_field).Replace("-", ", ") + "]");
        }

        public static void test_chapter_12_p215()
        {
            UInt32 field_size = 2;
            UInt32 num_functions = 2;
            UInt32 tweak = 42;

            UInt32 bit_field_size = field_size * 8;
            byte[] bit_field = new byte[bit_field_size];

            string[] data = ["hello world", "goodbye"];

            for (UInt32 i = 0; i < data.Length; i++)
            {
                for (UInt32 j = 0; j < num_functions; j++)
                {
                    UInt32 seed = j * BloomFilter.BIP37_CONSTANT + tweak;

                    ReadOnlySpan<byte> inputSpan = Encoding.UTF8.GetBytes(data[i]).AsSpan();
                    var h = MurmurHash3.Hash32(ref inputSpan, seed);
                    UInt32 bit = h % bit_field_size;
                    bit_field[bit] = 1;
                }
            }

            Console.WriteLine("[" + BitConverter.ToString(bit_field).Replace("-", ", ") + "]");
        }

        public static void test_chapter_12_ex_2()
        {
            UInt32 field_size = 10;
            UInt32 num_functions = 5;
            UInt32 tweak = 99;
            string[] data = ["Hello World", "Goodbye!"];

            UInt32 bit_field_size = field_size * 8;
            byte[] bit_field = new byte[bit_field_size];

            for (UInt32 i = 0; i < data.Length; i++)
            {
                for (UInt32 j = 0; j < num_functions; j++)
                {
                    UInt32 seed = j * BloomFilter.BIP37_CONSTANT + tweak;

                    ReadOnlySpan<byte> inputSpan = Encoding.UTF8.GetBytes(data[i]).AsSpan();
                    var h = MurmurHash3.Hash32(ref inputSpan, seed);
                    UInt32 bit = h % bit_field_size;
                    bit_field[bit] = 1;
                }
            }

            byte[] bField = MerkleBlock.BitFieldToFlags(bit_field);
            string strField = Tools.BytesToHexString(bField);
            Console.WriteLine(strField);
        }

        public static void test_add()
        {
            BloomFilter filter = new BloomFilter(10, 5, 99);
            byte[] data = Tools.AsciiStringToBytes("Hello World");
            filter.Add(data);
            string expected = "0000000a080000000140";
            string actual = Tools.BytesToHexString(filter.FilterBytes());

            AssertEqual(expected, actual);
        }

        public static void test_chapter_12_ex_6() //ok
        {
            // this C# code ran through to the last line.... the python code was stuck and did not work...
            string last_block_hex = "00000000000000a03f9432ac63813c6710bfe41712ac5ef6faab093fe2917636";
            BigInteger secret = Tools.BigIntegerFromBytes(Tools.Hash256(Tools.AsciiStringToBytes("Jimmy Song")), "little");
            PrivateKey private_key = new PrivateKey(secret);
            string addr = private_key._point.Address(true, true);
            // mseRGXB89UTFVkWJhTRTzzZ9Ujj4ZPbGK5
            byte[] h160 = Base58Encoding.DecodeH160(addr);
            // 850af0029eb376691c3eef244c25eceb4e50c503
            string h160_str = Tools.BytesToHexString(h160);
            string target_address = "mwJn1YPMq7y5F8J3LkC5Hxg9PHyZ5K4cFv";
            byte[] target_h160 = Base58Encoding.DecodeH160(target_address);
            Script target_script = Script.Create_P2PKH_Script(target_h160);
            UInt64 fee = 5000;
            SimpleNode node = new SimpleNode("65.109.24.172", true, 3);
            node.Init();
            BloomFilter bf = new BloomFilter(30, 5, 90210);
            bf.Add(h160);
            node.Handshake();
            FilterLoadMessage filterLoadMsg = bf.CreateFilterLoadMessage(1);
            node.Send(filterLoadMsg);
            byte[] start_block = Tools.HexStringToBytes(last_block_hex);
            GetHeadersMessage getHeaders = new GetHeadersMessage(start_block);
            node.Send(getHeaders);
            HeadersMessage headers = (HeadersMessage) node.WaitFor(new() { HeadersMessage.Command });
            byte[] last_block = null;
            GetDataMessage getdata = new GetDataMessage();
            foreach (BlockHeader b in headers._blockHeaders)
            {
                if (!b.CheckPow())
                {
                    throw new Exception("proof of work is invalid");
                }
                if ((last_block != null) && (!b._prevBlockHash.SequenceEqual(last_block)))
                {
                    throw new Exception("chain broken");
                }
                getdata.Add(GetDataMessage.MSG_FILTERED_BLOCK, b.Hash());
                last_block = b.Hash();
            }
            node.Send(getdata);
            byte[] prev_tx = null;
            UInt32 prev_index = 0;
            UInt64 prev_amount = 0;
            bool found = false;
            while (prev_tx == null)
            {
                NetworkMessage message = node.WaitFor(new() { MerkleBlockMessage.Command, Tx.Command });
                if (message._command == MerkleBlockMessage.Command)
                {
                    MerkleBlockMessage merkleBlockMessage = (MerkleBlockMessage) message;
                    if (!merkleBlockMessage._merkleBlock.IsValid())
                    {
                        throw new Exception("invalid merkle proof");
                    }
                    else
                    {
                        Console.WriteLine("MerkleBlock is valid!");
                    }
                }
                else
                {
                    // TX
                    Tx tx = (Tx)message;
                    tx._testnet = true;
                    Tools.WriteLine("tx has " + tx._txOuts.Count + " txOuts");
                    for (int i = 0; i < tx._txOuts.Count; i++)
                    {
                        TxOut txOut = tx._txOuts[i];
                        if (txOut._script_pubkey.Address(true).Equals(addr))
                        {
                            prev_tx = tx.Hash();
                            prev_index = (UInt32) i;
                            prev_amount = txOut._amount;

                            Tools.WriteLine($"found {Tools.BytesToHexString(prev_tx)}:{prev_index}");
                            found = true;
                            break;
                            // found: b2cddd41d18d00910f88c31aa58c6816a190b8fc30fe7c665e1cd2ec60efdf3f:7
                        }
                    }
                }
            }

            TxIn tx_in = new TxIn(prev_tx, prev_index);
            UInt64 output_amount = prev_amount - fee;
            TxOut tx_out = new TxOut(output_amount, target_script);
            Tx tx_obj = new Tx(1, new() { tx_in }, new() { tx_out }, 0, false);
            tx_obj._testnet = true;
            bool success = tx_obj.SignInputPtpkh(0, private_key);
            Tools.WriteLine($"SignInputPtpkh success = {success}");
            string tx_obj_seralized = Tools.BytesToHexString(tx_obj.serialize());
            Tools.WriteLine($"tx_obj_seralized = {tx_obj_seralized}");
            // 01000000013fdfef60ecd21c5e667cfe30fcb890a116688ca51ac3880f91008dd141ddcdb2070000006b483045022100ff77d2559261df5490ed00d231099c4b8ea867e6ccfe8e3e6d077313ed4f1428022033a1db8d69eb0dc376f89684d1ed1be75719888090388a16f1e8eedeb8067768012103dc585d46cfca73f3a75ba1ef0c5756a21c1924587480700c6eb64e3f75d22083ffffffff019334e500000000001976a914ad346f8eb57dee9a37981716e498120ae80e44f788ac00000000
            node.Send(tx_obj);
            Thread.Sleep(2000);
            getdata = new GetDataMessage();
            getdata.Add(GetDataMessage.MSG_TX, tx_obj.Hash());
            node.Send(getdata);
            Tx received_tx = (Tx) node.WaitFor(new() { Tx.Command });
            if (received_tx.Id() == tx_obj.Id())
            {
                // "380c9484d577eec7339c98b5c09d7ab72561ff9344ac817c3aef7a9e05d48663"
                Tools.WriteLine("success!");
            }
        }
    }
}
