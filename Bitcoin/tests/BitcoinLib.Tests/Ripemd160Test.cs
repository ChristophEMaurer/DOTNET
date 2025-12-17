using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BitcoinLib;

namespace BitcoinLib.Test
{
    public class Ripemd160Test : UnitTest
    {
        public static void test_ripemd160() // TODO use RIPEMD160 and no BouncyCastle!
        {
            object[] data =
            {
                "", "9c1185a5c5e9fc54612808977ee8f548b2258d31",
                "a", "0bdc9d2d256b3ee9daae347be6f4dc835a467ffe",
                "abc", "8eb208f7e05d987a9b044a8e98c6b087f15a0bfc",
                "message digest", "5d0689ef49d2fae572b881b123a85ffa21595f36",
                "abcdefghijklmnopqrstuvwxyz", "f71c27109c692c1b56bbdceb5b9d2865b3708dbc",
                "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq", "12a053384a9c0c88e405a06c27dcf49ada62eb2b",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "b0e20b6e3116640286ed3a87a5713079b21f5189",
            };

            for (int i = 0; i < data.Length; i++)
            {
                string input = (string)data[i++];
                string want = (string)data[i];

                byte[] bInput = Tools.AsciiStringToBytes(input);
                byte[] result = Tools.RipeMD160(bInput);

                string hex = BitConverter.ToString(result).Replace("-", "").ToLower();

                AssertTrue(hex.Equals(want));
            }
        }
    }
}

