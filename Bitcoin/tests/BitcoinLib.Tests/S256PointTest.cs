using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace BitcoinLib.Test
{
    public class S256PointTest : UnitTest
    {
        public static void test_chapter_3_page_65_dont_reveal_k()
        {
            ConsoleOutWriteLine("Do not reveal k when signing!");

            BigInteger e = Tools.MakeBigInteger("08b387de39861728c92ec9f589c303b1038ff60eb3963b12cd212263a1d1e0f00");
            BigInteger z = Tools.MakeBigInteger("0231c6f3d980a6b0fb7152f85cee7eb52bf92433d9919b9c5218cb08e79cce78");

            //
            // k has to be larger than u or it does not work because of k-u below
            //
            BigInteger k = Tools.MakeBigInteger("09999999999123456789002987450982370495782094572093478509238475908273405");

            Point R = k * S256Field.G;
            BigInteger r = R._x._num;
            BigInteger k_inv = BigInteger.ModPow(k, S256Field.N - 2, S256Field.N);
            BigInteger s = (z + (r * e)) * k_inv;
            s = BigInteger.ModPow(s, 1, S256Field.N);
            S256Point point = e * S256Field.G;

            //
            // we created the signature and calculated (r, s).
            //
            Console.WriteLine("We computed the signature (r, s) from private key:");
            Console.WriteLine(Tools.BigIntegerToHexString(e));

            //
            // we have (r, s), z and k: from this we calculate the private key e
            //
            // u = z / s
            //
            BigInteger u = z * BigInteger.ModPow(s, S256Field.N - 2, S256Field.N);
            u = BigInteger.ModPow(u, 1, S256Field.N);

            //
            // v = r / s
            //
            BigInteger v = r * BigInteger.ModPow(s, S256Field.N - 2, S256Field.N);
            v = BigInteger.ModPow(v, 1, S256Field.N);

            // e = (k - u) / v
            BigInteger e_calculated = (k - u) * BigInteger.ModPow(v, S256Field.N - 2, S256Field.N);
            e_calculated = BigInteger.ModPow(e_calculated, 1, S256Field.N);

            Console.WriteLine("We calculated the privated key e from (r, s), z and k:");
            Console.WriteLine(Tools.BigIntegerToHexString(e_calculated));

            AssertTrue(e == e_calculated);
        }

        public static void test_chapter_3_page_71_unique_k()
        {
            ConsoleOutWriteLine("Chapter 3, page 71, unique_k: Do not use the same k twice!");

            BigInteger z1 = Tools.MakeBigInteger("0111111111111111110fc1c19f708a9ca96fdeff3ac3f230bb4a7ba4aede4942ad003c0f60");
            BigInteger z2 = Tools.MakeBigInteger("022222222222222222efc1c19f708a9ca96fdeff3ac3f230bb4a7ba4aede4942ad003c0f60");
            BigInteger e = Tools.MakeBigInteger("08b387de39861728c92ec9f589c303b1038ff60eb3963b12cd212263a1d1e0f00");
            BigInteger k = Tools.MakeBigInteger("099999999999999999999999123456789002987450982370495782094572093478509238475908273405");

            Point R = k * S256Field.G;
            BigInteger r = R._x._num;

            BigInteger k_inv = BigInteger.ModPow(k, S256Field.N - 2, S256Field.N);

            BigInteger s1 = (z1 + (r * e)) * k_inv;
            s1 = BigInteger.ModPow(s1, 1, S256Field.N);
            BigInteger s2 = (z2 + (r * e)) * k_inv;
            s2 = BigInteger.ModPow(s2, 1, S256Field.N);

            Console.WriteLine("We calculated two signatures (r, s1), (r, s2) from two different messages z1 and z2 with the same k with private key e:");
            Console.WriteLine(Tools.BigIntegerToHexString(e));

            // now we have (r, s1) and (r, s2)

            //
            // if you use the same k twice for different messages z1 and z2, then:
            // e = (s2*z1 - s1*z2) / (r*s1 - r*s2)
            // (r, s1) and (r, s2) are the signatures, and we already had the messages z1 and z2
            //
            BigInteger inv = BigInteger.ModPow((r * s1) - (r * s2), S256Field.N - 2, S256Field.N);
            BigInteger e_calculated = ((s2 * z1) - (s1 * z2)) * inv;
            e_calculated = BigInteger.ModPow(e_calculated, 1, S256Field.N);

            Console.WriteLine("We calculated the privated key e from (r, s1), (r, s2), z1 and z2:");
            Console.WriteLine(Tools.BigIntegerToHexString(e_calculated));

            AssertTrue(e == e_calculated);
        }

        public static void test_chapter_3_ex_6()
        {
            ConsoleOutWriteLine("Chapter 3, ex_6: checking signatures");

            S256Point point = new S256Point(
                Tools.MakeBigInteger("0887387e452b8eacc4acfde10d9aaf7f6d9a0f975aabb10d006e4da568744d06c"),
                Tools.MakeBigInteger("061de6d95231cd89026e286df3b6ae4a894a3378e393e93a0f45b666329a0ae34"));

            BigInteger z = Tools.MakeBigInteger("0ec208baa0fc1c19f708a9ca96fdeff3ac3f230bb4a7ba4aede4942ad003c0f60");
            BigInteger r = Tools.MakeBigInteger("0ac8d1c87e51d0d441be8b3dd5b05c8795b48875dffe00b7ffcfac23010d3a395");
            BigInteger s = Tools.MakeBigInteger("068342ceff8935ededd102dd876ffd6ba72d6a427a3edb13d26eb0781cb423c4");
            BigInteger u = z * BigInteger.ModPow(s, S256Field.N - 2, S256Field.N);
            u = BigInteger.ModPow(u, 1, S256Field.N);
            BigInteger v = r * BigInteger.ModPow(s, S256Field.N - 2, S256Field.N);
            v = BigInteger.ModPow(v, 1, S256Field.N);
            Point R1 = (u * S256Field.G);
            Point R2 = (v * point);
            Point R = R1 + R2;
            Console.WriteLine("R._x._num == r:" + (R._x._num == r));

            z = Tools.MakeBigInteger("07c076ff316692a3d7eb3c3bb0f8b1488cf72e1afcd929e29307032997a838a3d");
            r = Tools.MakeBigInteger("0eff69ef2b1bd93a66ed5219add4fb51e11a840f404876325a1e8ffe0529a2c");
            s = Tools.MakeBigInteger("0c7207fee197d27c618aea621406f6bf5ef6fca38681d82b2f06fddbdce6feab6");
            u = z * BigInteger.ModPow(s, S256Field.N - 2, S256Field.N);
            u = BigInteger.ModPow(u, 1, S256Field.N);
            v = r * BigInteger.ModPow(s, S256Field.N - 2, S256Field.N);
            v = BigInteger.ModPow(v, 1, S256Field.N);
            R1 = (u * S256Field.G);
            R2 = (v * point);
            R = R1 + R2;
            Console.WriteLine("R._x._num == r:" + (R._x._num == r));
        }

        public static void test_chapter_3_page_69_sign()
        {
            ConsoleOutWriteLine("Chapter 3 page 69: signing");

            BigInteger e = Tools.MakeBigInteger("08b387de39861728c92ec9f589c303b1038ff60eb3963b12cd212263a1d1e0f00");
            BigInteger z = Tools.MakeBigInteger("0231c6f3d980a6b0fb7152f85cee7eb52bf92433d9919b9c5218cb08e79cce78");
            BigInteger k = 1234567890;
            Point R = k * S256Field.G;
            BigInteger r = R._x._num;
            BigInteger k_inv = BigInteger.ModPow(k, S256Field.N - 2, S256Field.N);

            BigInteger s = (z + (r * e)) * k_inv;
            s = BigInteger.ModPow(s, 1, S256Field.N);

            S256Point point = e * S256Field.G;

            Console.WriteLine("point:" + point);
            Console.WriteLine("e:" + Tools.BigIntegerToHexString(e));
            Console.WriteLine("z:" + Tools.BigIntegerToHexString(z));
            Console.WriteLine("r:" + Tools.BigIntegerToHexString(r));
            Console.WriteLine("s:" + Tools.BigIntegerToHexString(s));

            PrivateKey pk = new PrivateKey(e);
            Signature sign = pk.Sign(z);
        }

        public static void test_chapter_3_ex_7()
        {
            ConsoleOutWriteLine("Chapter 3, ex 7: signing");

            BigInteger e = 12345;
            BigInteger z = Tools.MakeBigInteger("0969f6056aa26f7d2795fd013fe88868d09c9f6aed96965016e1936ae47060d48");
            BigInteger k = 1234567890;
            Point R = k * S256Field.G;
            BigInteger r = R._x._num;
            BigInteger k_inv = BigInteger.ModPow(k, S256Field.N - 2, S256Field.N);

            BigInteger s = (z + (r * e)) * k_inv;
            s = BigInteger.ModPow(s, 1, S256Field.N);

            S256Point point = e * S256Field.G;

            Console.WriteLine("point:" + point);
            Console.WriteLine("e:" + e);
            Console.WriteLine("z:" + Tools.BigIntegerToHexString(z));
            Console.WriteLine("r:" + Tools.BigIntegerToHexString(r));
            Console.WriteLine("s:" + Tools.BigIntegerToHexString(s));
        }

        public static void test_order()
        {
            S256Point p = S256Field.N * S256Field.G;

            AssertIsNull(p._x);
        }

        public static void test_pubpoint()
        {
            BigInteger[] data =
            {
                // secret, x, y
                7,
                Tools.MakeBigInteger("05cbdf0646e5db4eaa398f365f2ea7a0e3d419b7e0330e39ce92bddedcac4f9bc"),
                Tools.MakeBigInteger("06aebca40ba255960a3178d6d861a54dba813d0b813fde7b5a5082628087264da"),
                1485,
                Tools.MakeBigInteger("0c982196a7466fbbbb0e27a940b6af926c1a74d5ad07128c82824a11b5398afda"),
                Tools.MakeBigInteger("07a91f9eae64438afb9ce6448a1c133db2d8fb9254e4546b6f001637d50901f55"),
                BigInteger.Pow(2, 128),
                Tools.MakeBigInteger("08f68b9d2f63b5f339239c1ad981f162ee88c5678723ea3351b7b444c9ec4c0da"),
                Tools.MakeBigInteger("0662a9f2dba063986de1d90c2b6be215dbbea2cfe95510bfdf23cbf79501fff82"),
                BigInteger.Pow(2, 240) + BigInteger.Pow(2, 31),
                Tools.MakeBigInteger("09577ff57c8234558f293df502ca4f09cbc65a6572c842b39b366f21717945116"),
                Tools.MakeBigInteger("010b49c67fa9365ad7b90dab070be339a1daf9052373ec30ffae4f72d5e66d053"),
            };

            for (int i = 0; i < data.Length; i++)
            {
                BigInteger secret = data[i++];
                S256Point p1 = new S256Point(data[i++], data[i]);
                S256Point p2 = secret * S256Field.G;

                Console.WriteLine("p1=" + p1);
                Console.WriteLine("p2=" + p2);
                AssertEqual(p1, p2);
            }
        }
        public static void test_verify()
        {
            S256Point point = new S256Point(
            Tools.MakeBigInteger("0887387e452b8eacc4acfde10d9aaf7f6d9a0f975aabb10d006e4da568744d06c"),
            Tools.MakeBigInteger("061de6d95231cd89026e286df3b6ae4a894a3378e393e93a0f45b666329a0ae34"));

            BigInteger z = Tools.MakeBigInteger("0ec208baa0fc1c19f708a9ca96fdeff3ac3f230bb4a7ba4aede4942ad003c0f60");
            BigInteger r = Tools.MakeBigInteger("0ac8d1c87e51d0d441be8b3dd5b05c8795b48875dffe00b7ffcfac23010d3a395");
            BigInteger s = Tools.MakeBigInteger("068342ceff8935ededd102dd876ffd6ba72d6a427a3edb13d26eb0781cb423c4");
            AssertTrue(point.Verify(z, new Signature(r, s)));
           
            z = Tools.MakeBigInteger("07c076ff316692a3d7eb3c3bb0f8b1488cf72e1afcd929e29307032997a838a3d");
            r = Tools.MakeBigInteger("0eff69ef2b1bd93a66ed5219add4fb51e11a840f404876325a1e8ffe0529a2c");
            s = Tools.MakeBigInteger("0c7207fee197d27c618aea621406f6bf5ef6fca38681d82b2f06fddbdce6feab6");
            AssertTrue(point.Verify(z, new Signature(r, s)));
        }

        public static void test_sec()
        {
            object[] data =
            {
                BigInteger.Pow(999, 3),
                    "049d5ca49670cbe4c3bfa84c96a8c87df086c6ea6a24ba6b809c9de234496808d56fa15cc7f3d38cda98dee2419f415b7513dde1301f8643cd9245aea7f3f911f9",
                    "039d5ca49670cbe4c3bfa84c96a8c87df086c6ea6a24ba6b809c9de234496808d5",
                Tools.MakeBigIntegerBase10("123"),
                    "04a598a8030da6d86c6bc7f2f5144ea549d28211ea58faa70ebf4c1e665c1fe9b5204b5d6f84822c307e4b4a7140737aec23fc63b65b35f86a10026dbd2d864e6b",
                    "03a598a8030da6d86c6bc7f2f5144ea549d28211ea58faa70ebf4c1e665c1fe9b5",
                Tools.MakeBigIntegerBase10("42424242"),
                    "04aee2e7d843f7430097859e2bc603abcc3274ff8169c1a469fee0f20614066f8e21ec53f40efac47ac1c5211b2123527e0e9b57ede790c4da1e72c91fb7da54a3",
                    "03aee2e7d843f7430097859e2bc603abcc3274ff8169c1a469fee0f20614066f8e",
            };

            for (int i = 0; i < data.Length; i++)
            {
                BigInteger coefficient = (BigInteger)data[i++];
                string uncompressed = (string)data[i++];
                string compressed = (string)data[i];

                S256Point point = coefficient * S256Field.G;
                byte[] bUncompressed = point.Sec(false);
                byte[] bCompressed = point.Sec(true);
                string strUncompressed = Tools.BytesToHexString(bUncompressed);
                string strCompressed = Tools.BytesToHexString(bCompressed);

                AssertEqual(strUncompressed, uncompressed);
                AssertEqual(strCompressed, compressed);
            }
        }

        public static void test_address()
        {
            object[] data =
            {
                true, BigInteger.Pow(888, 3), "148dY81A9BmdpMhvYEVznrM45kWN32vSCN", "mieaqB68xDCtbUBYFoUNcmZNwk74xcBfTP",
                false, Tools.MakeBigIntegerBase10("321"), "1S6g2xBJSED7Qr9CYZib5f4PYVhHZiVfj", "mfx3y63A7TfTtXKkv7Y6QzsPFY6QCBCXiP",
                false, Tools.MakeBigIntegerBase10("4242424242"), "1226JSptcStqn4Yq9aAmNXdwdc2ixuH9nb", "mgY3bVusRUL6ZB2Ss999CSrGVbdRwVpM8s"
            };

            for (int i = 0; i < data.Length; i++)
            {
                bool compressed = (bool)data[i++];
                BigInteger secret = (BigInteger)data[i++];
                string mainnetAddress = (string)data[i++];
                string testnetAddress = (string)data[i];

                S256Point point = secret * S256Field.G;
                string calculatedAddressMainnet = point.Address(compressed, false);
                string calculatedAddressTestnet = point.Address(compressed, true);
                AssertTrue(calculatedAddressMainnet == mainnetAddress);
                AssertTrue(calculatedAddressTestnet == testnetAddress);
            }
        }

        public static void test_chapter_4_ex_1()
        {
            ConsoleOutWriteLine("Chapter 4, ex 1: find the uncompressed format for the public key calculated from the provided private key...");

            PrivateKey priv = new PrivateKey(5000);
            // priv._poin is the public key calculated from the private key 5000
            string text = Tools.BytesToHexString(priv._point.Sec(false));
            Console.WriteLine(text);
            AssertTrue(text == "04ffe558e388852f0120e46af2d1b370f85854a8eb0841811ece0e3e03d282d57c315dc72890a4f10a1481c031b03b351b0dc79901ca18a00cf009dbdb157a1d10");

            priv = new PrivateKey(BigInteger.Pow(2018, 5));
            text = Tools.BytesToHexString(priv._point.Sec(false));
            Console.WriteLine(text);
            AssertTrue(text == "04027f3da1918455e03c46f659266a1bb5204e959db7364d2f473bdf8f0a13cc9dff87647fd023c13b4a4994f17691895806e1b40b57f4fd22581a4f46851f3b06");

            priv = new PrivateKey(0xdeadbeef12345);
            text = Tools.BytesToHexString(priv._point.Sec(false));
            Console.WriteLine(text);
            AssertTrue(text == "04d90cd625ee87dd38656dd95cf79f65f60f7273b67d3096e68bd81e4f5342691f842efa762fd59961d0e99803c61edba8b3e3f7dc3a341836f97733aebf987121");
        }

        public static void test_chapter_4_ex_2()
        {
            ConsoleOutWriteLine("Chapter 4, ex 2. find the compressed SEC format for the public key which is calculated from the provided private key");

            PrivateKey priv = new PrivateKey(5001);
            string text = Tools.BytesToHexString(priv._point.Sec());
            Console.WriteLine(text);
            AssertTrue(text == "0357a4f368868a8a6d572991e484e664810ff14c05c0fa023275251151fe0e53d1");

            priv = new PrivateKey(BigInteger.Pow(2019, 5));
            text = Tools.BytesToHexString(priv._point.Sec());
            Console.WriteLine(text);
            AssertTrue(text == "02933ec2d2b111b92737ec12f1c5d20f3233a0ad21cd8b36d0bca7a0cfa5cb8701");

            priv = new PrivateKey(0xdeadbeef54321);
            text = Tools.BytesToHexString(priv._point.Sec());
            Console.WriteLine(text);
            AssertTrue(text == "0296be5b1292f6c856b3c5654e886fc13511462059089cdf9c479623bfcbe77690");
        }
    }
}

