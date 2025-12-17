using System;
using System.Security.Cryptography;

namespace BitcoinLib.Test
{
    public class Base58EncodingTests : UnitTest
    {
        // Test cases from https://github.com/bitcoin/bitcoin/blob/master/src/test/base58_tests.cpp
        static Tuple<string, byte[]>[] testCases = new Tuple<string, byte[]>[]{
            Tuple.Create("",new byte[]{}),
            Tuple.Create("1112",new byte[]{0x00, 0x00, 0x00, 0x01}),
            Tuple.Create("2g",new byte[]{0x61}),
            Tuple.Create("a3gV",new byte[]{0x62,0x62,0x62}),
            Tuple.Create("aPEr",new byte[]{0x63,0x63,0x63}),
            Tuple.Create("2cFupjhnEsSn59qHXstmK2ffpLv2",new byte[]{0x73,0x69,0x6d,0x70,0x6c,0x79,0x20,0x61,0x20,0x6c,0x6f,0x6e,0x67,0x20,0x73,0x74,0x72,0x69,0x6e,0x67}),
            Tuple.Create("1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L",new byte[]{0x00,0xeb,0x15,0x23,0x1d,0xfc,0xeb,0x60,0x92,0x58,0x86,0xb6,0x7d,0x06,0x52,0x99,0x92,0x59,0x15,0xae,0xb1,0x72,0xc0,0x66,0x47}),
            Tuple.Create("ABnLTmg",new byte[]{0x51,0x6b,0x6f,0xcd,0x0f}),
            Tuple.Create("3SEo3LWLoPntC",new byte[]{0xbf,0x4f,0x89,0x00,0x1e,0x67,0x02,0x74,0xdd}),
            Tuple.Create("3EFU7m",new byte[]{0x57,0x2e,0x47,0x94}),
            Tuple.Create("EJDM8drfXA6uyA",new byte[]{0xec,0xac,0x89,0xca,0xd9,0x39,0x23,0xc0,0x23,0x21}),
            Tuple.Create("Rt5zm",new byte[]{0x10,0xc8,0x51,0x1e}),
            Tuple.Create("1111111111",new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00})
        };

        public static void test1_encode()
        {
            foreach (var tuple in testCases)
            {
                var bytes = tuple.Item2;
                var expectedText = tuple.Item1;
                var actualText = Base58Encoding.Encode(bytes);
                AssertEqual(expectedText, actualText);
            }
        }

        public static void test2_decode()
        {
            foreach (var tuple in testCases)
            {
                var text = tuple.Item1;
                var expectedBytes = tuple.Item2;
                var actualBytes = Base58Encoding.Decode(text);
                AssertEqual(BitConverter.ToString(expectedBytes), BitConverter.ToString(actualBytes));
            }
        }

        public static void test3_DecodeInvalidChar()
        {
            UnitTest.AssertException(typeof(FormatException), () => Base58Encoding.Decode("ab0"));
        }

        // Example address from https://en.bitcoin.it/wiki/Technical_background_of_version_1_Bitcoin_addresses
        static byte[] addressBytes = new byte[]         { 0x00, 0x01, 0x09, 0x66, 0x77, 0x60, 0x06, 0x95, 0x3D, 0x55, 0x67, 0x43, 0x9E, 0x5E, 0x39, 0xF8, 0x6A, 0x0D, 0x27, 0x3B, 0xEE }; // 21 bytes
        static byte[] addressBytesHash160 = new byte[]  {       0x01, 0x09, 0x66, 0x77, 0x60, 0x06, 0x95, 0x3D, 0x55, 0x67, 0x43, 0x9E, 0x5E, 0x39, 0xF8, 0x6A, 0x0D, 0x27, 0x3B, 0xEE };
        static string addressText       = "16UwLL9Risc3QfPqBUvKofHmBQ7wMtjvM";
        static string brokenAddressText = "16UwLl9Risc3QfPqBUvKofHmBQ7wMtjvM";
        //                                      ^
        //                                      |

        public static void test4_EncodeBitcoinAddress()
        {
            var actualText = Base58Encoding.EncodeWithCheckSum(addressBytes);
            AssertEqual(addressText, actualText);
        }

        public static void test5_DecodeBitcoinAddress()
        {
            var actualBytes = Base58Encoding.DecodeWithCheckSum(addressText);
            AssertEqual(BitConverter.ToString(addressBytes), BitConverter.ToString(actualBytes));
        }

        public static void test6_DecodeBrokenBitcoinAddress()
        {
            UnitTest.AssertException(typeof(FormatException), () => Base58Encoding.DecodeWithCheckSum(brokenAddressText));
        }

        public static void test7_ExtractHash160()
        {
            byte[] hash160 = Base58Encoding.VerifyAndRemoveNetworkprefixAndCheckSum(addressText);

            AssertEqual(BitConverter.ToString(hash160), BitConverter.ToString(addressBytesHash160));
        }
        public static void test_p2pkh_address()
        {
            byte[] h160 = Tools.HexStringToBytes("74d691da1574e6b3c192ecfb52cc8984ee7b6c56");

            string want = "1BenRpVUFK65JFWcQSuHnJKzc4M8ZP8Eqa";
            string address = Base58Encoding.H160ToP2pkhAddress(h160, false);
            AssertEqual(address, want);

            want = "mrAjisaT4LXL5MzE81sfcDYKU3wqWSvf9q";
            address = Base58Encoding.H160ToP2pkhAddress(h160, true);
            AssertEqual(address, want);
        }
        public static void test_p2sh_address()
        {
            byte[] h160 = Tools.HexStringToBytes("74d691da1574e6b3c192ecfb52cc8984ee7b6c56");

            string want = "3CLoMMyuoDQTPRD3XYZtCvgvkadrAdvdXh";
            string address = Base58Encoding.H160ToP2shAddress(h160, false);
            AssertEqual(address, want);

            want = "2N3u1R6uwQfuobCqbCgBkpsgBxvr1tZpe7B";
            address = Base58Encoding.H160ToP2shAddress(h160, true);
            AssertEqual(address, want);
        }
    }
}
