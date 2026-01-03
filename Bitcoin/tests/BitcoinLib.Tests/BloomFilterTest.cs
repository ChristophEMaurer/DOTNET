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
    }
}
