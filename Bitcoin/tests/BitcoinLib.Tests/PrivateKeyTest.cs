using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using System.Diagnostics;

namespace BitcoinLib.Test
{
    public class PrivateKeyTest : UnitTest
    {
        public static void test_sign_other()
        {
            BigInteger e = Tools.MakeBigInteger("08b387de39861728c92ec9f589c303b1038ff60eb3963b12cd212263a1d1e0f00");
            BigInteger z = Tools.MakeBigInteger("0231c6f3d980a6b0fb7152f85cee7eb52bf92433d9919b9c5218cb08e79cc9e78");

            PrivateKey pk = new PrivateKey(e);

            BigInteger k = pk.DeterministicK(z);

            Signature sig = pk.Sign(z);

            bool success = pk._point.Verify(z, sig);

            AssertTrue(success);
        }

        public static void test_sign()
        {
            BigInteger x = Tools.GetUnsafeRandom(S256Field.N);
            PrivateKey pk = new PrivateKey(x);
            BigInteger z = Tools.GetUnsafeRandom(BigInteger.Pow(2, 256));
            Signature sig = pk.Sign(z);
            AssertTrue(pk._point.Verify(z, sig));
        }

        public static void test_wif()
        {
            BigInteger bi = BigInteger.Pow(2, 256) - BigInteger.Pow(2, 199);
            PrivateKey pk = new PrivateKey(bi);
            string expected = "L5oLkpV3aqBJ4BgssVAsax1iRa77G5CVYnv9adQ6Z87te7TyUdSC";
            string wif = pk.Wif(true, false);
            AssertTrue(wif.Equals(expected));

            bi = BigInteger.Pow(2, 256) - BigInteger.Pow(2, 201);
            pk = new PrivateKey(bi);
            expected = "93XfLeifX7Jx7n7ELGMAf1SUR6f9kgQs8Xke8WStMwUtrDucMzn";
            wif = pk.Wif(false, true);
            AssertTrue(wif.Equals(expected));

            bi = Tools.MakeBigInteger("0dba685b4511dbd3d368e5c4358a1277de9486447af7b3604a69b8d9d8b7889d");
            pk = new PrivateKey(bi);
            expected = "5HvLFPDVgFZRK9cd4C5jcWki5Skz6fmKqi1GQJf5ZoMofid2Dty";
            wif = pk.Wif(false, false);
            AssertTrue(wif.Equals(expected));

            bi = Tools.MakeBigInteger("01cca23de92fd1862fb5b76e5f4f50eb082165e5191e116c18ed1a6b24be6a53f");
            pk = new PrivateKey(bi);
            expected = "cNYfWuhDpbNM1JWc3c6JTrtrFVxU4AGhUKgw5f93NP2QaBqmxKkg";
            wif = pk.Wif(true, true);
            AssertTrue(wif.Equals(expected));
        }

        public static void test_public_key_hash160()
        {
            BigInteger secret = 8675309;

            PrivateKey privateKey = new PrivateKey(secret);
            string strAddressCompressed = privateKey._point.Address(true, true);
            string strAddressUncompressed = privateKey._point.Address(false, true);

            Console.Out.WriteLine("privateKey=" + privateKey.ToString());
            Console.Out.WriteLine("strAddressCompressed=" + strAddressCompressed.ToString());
            Console.Out.WriteLine("strAddressUncompressed=" + strAddressUncompressed.ToString());
        }
    }
}
