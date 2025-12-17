using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

namespace BitcoinLib.Test
{
    public class SignatureTest : UnitTest
    {
        public static void test_der()
        {
            object[] data =
            {
                Tools.MakeBigIntegerBase10("1"), Tools.MakeBigIntegerBase10("2"), "3006020101020102",
                Tools.GetUnsafeRandom(BigInteger.Pow(2, 256)), Tools.GetUnsafeRandom(BigInteger.Pow(2, 255)), "",
            };

            ConsoleOutWriteLine("Testing " + data.Length / 3 + " conversions");

            for (int i = 0; i < data.Length;)
            {
                BigInteger r = (BigInteger)data[i++];
                BigInteger s = (BigInteger)data[i++];
                string der = (string)data[i++];

                Signature sig = new Signature(r, s);
                byte[] der2 = sig.Der();
                string strDer2 = Tools.BytesToHexString(der2);

                Signature sig2 = Signature.Parse(der2);
                if (!string.IsNullOrEmpty(der))
                {
                    AssertEqual(der, strDer2);
                }
                AssertEqual(sig2._r, r);
                AssertEqual(sig2._s, s);
            }
        }

        public static void test_chapter_4_ex_6()
        {
            ConsoleOutWriteLine("p. 67: testing private key WIF format");

            object[] data =
            {
                new BigInteger(5003), "cMahea7zqjxrtgAbB7LSGbcQUr1uX1ojuat9jZodMN8rFTv2sfUK", true, true,
                BigInteger.Pow(2021, 5), "91avARGdfge8E4tZfYLoxeJ5sGBdNJQH4kvjpWAxgzczjbCwxic", false, true,
                new BigInteger(0x054321deadbeef), "KwDiBf89QgGbjEhKnhXJuH7LrciVrZi3qYjgiuQJv1h8Ytr2S53a", true, false,
            };

            for (int i = 0; i < data.Length; i++)
            {
                BigInteger x = (BigInteger)data[i++];
                string expected = (string)data[i++];
                bool compressed = (bool)data[i++];
                bool testnet = (bool)data[i];

                PrivateKey pk = new PrivateKey(x);
                string calculated = pk.Wif(compressed, testnet);
                ConsoleOutWriteLine("WIF wallet import format of private key " + x + ": " + calculated);
                AssertTrue(calculated == expected);
            }
        }

        public static void test_chapter_4_ex_5()
        {
            ConsoleOutWriteLine("testing private key address creation");

            object[] data =
            {
                new BigInteger(5002),               "mmTPbXQFxboEtNRkwfh6K51jvdtHLxGeMA", false, true,
                BigInteger.Pow(2020, 5),            "mopVkxp8UhXqRYbCYJsbeE1h1fiF64jcoH", true, true,
                new BigInteger(0x12345deadbeef),    "1F1Pn2y6pDb68E5nYJJeba4TLg2U7B6KF1", true, false,
            };

            for (int i = 0; i < data.Length; i++)
            {
                BigInteger x = (BigInteger)data[i++];
                string expected = (string)data[i++];
                bool compressed = (bool)data[i++];
                bool testnet = (bool)data[i];

                PrivateKey pk = new PrivateKey(x);
                string calculated = pk._point.Address(compressed, testnet);
                ConsoleOutWriteLine("public address of private key " + x + ": " + calculated);
                AssertTrue(calculated == expected);
            }
        }

        public static void test_chapter_4_ex_4()
        {
            ConsoleOutWriteLine("testing Base58");

            string[] data =
            {
                "7c076ff316692a3d7eb3c3bb0f8b1488cf72e1afcd929e29307032997a838a3d","9MA8fRQrT4u8Zj8ZRd6MAiiyaxb2Y1CMpvVkHQu5hVM6",
                "eff69ef2b1bd93a66ed5219add4fb51e11a840f404876325a1e8ffe0529a2c", "4fE3H2E6XMp4SsxtwinF7w9a34ooUrwWe4WsW1458Pd",
                "c7207fee197d27c618aea621406f6bf5ef6fca38681d82b2f06fddbdce6feab6", "EQJsjkd6JaGwxrjEhfeqPenqHwrBmPQZjJGNSCHBkcF7"
            };

            for (int i = 0; i < data.Length; i++)
            {
                string hex = data[i++];
                string expected = data[i];

                string calculated = Base58Encoding.Encode(Tools.HexStringToBytes(hex));
                Console.WriteLine("Base58: " + hex + " => " + calculated);

                AssertTrue(calculated == expected);
            }
        }

        public static void test_chapter_4_ex_3()
        {
            ConsoleOutWriteLine("Find the DER format to serialize a signature (r, s)");

            // Um sicherzustellen, dass eine hexadezimale Zeichenfolge ordnungsgemäß als positive Zahl interpretiert wird,
            // muss die erste Ziffer in value den Wert 0 aufweisen. Beispielsweise interpretiert die Methode als negativer 0x80 Wert,
            // interpretiert aber entweder 0x080 oder 0x0080 als positiver Wert. 
            //
            // 02 20 0x37 <= 0x80
            //                                    1                   2                   3
            //                                    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 
            BigInteger r = Tools.MakeBigInteger("037206a0610995c58074999cb9767b87af4c4978db68c06e8e6e81d282047a7c6");

            // 02 21 0x8c > 0x80
            BigInteger s = Tools.MakeBigInteger("08ca63759c1157ebeaec0d03cecca119fc9a75bf8e6d0fa65c841c8e2738cdaec");

            Signature sig = new Signature(r, s);
            string text = Tools.BytesToHexString(sig.Der());
            Console.WriteLine(sig.ToString() + "->DER ->" + text);
            AssertTrue(text == "3045022037206a0610995c58074999cb9767b87af4c4978db68c06e8e6e81d282047a7c60221008ca63759c1157ebeaec0d03cecca119fc9a75bf8e6d0fa65c841c8e2738cdaec");

            sig = new Signature(s, r);
            text = Tools.BytesToHexString(sig.Der());
            Console.WriteLine(sig.ToString() + "->DER ->" + text);
            AssertTrue(text == "30450221008ca63759c1157ebeaec0d03cecca119fc9a75bf8e6d0fa65c841c8e2738cdaec022037206a0610995c58074999cb9767b87af4c4978db68c06e8e6e81d282047a7c6");

            sig = new Signature(s, s);
            text = Tools.BytesToHexString(sig.Der());
            Console.WriteLine(sig.ToString() + "->DER ->" + text);
            AssertTrue(text == "30460221008ca63759c1157ebeaec0d03cecca119fc9a75bf8e6d0fa65c841c8e2738cdaec0221008ca63759c1157ebeaec0d03cecca119fc9a75bf8e6d0fa65c841c8e2738cdaec");

            sig = new Signature(r, r);
            text = Tools.BytesToHexString(sig.Der());
            Console.WriteLine(sig.ToString() + "->DER ->" + text);
            AssertTrue(text == "3044022037206a0610995c58074999cb9767b87af4c4978db68c06e8e6e81d282047a7c6022037206a0610995c58074999cb9767b87af4c4978db68c06e8e6e81d282047a7c6");
        }
        public static void test_chapter_7_p132()
        {
            ConsoleOutWriteLine("    check signature");
            S256Point point = S256Point.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes("0349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278a"))));
            //                                                                                  49fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278a
            // Präfix 0x03 → komprimierter Key, y ist ungerade
            // x = 49fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278a
            // y = a551672dd924a714b213dd11c40b1187584b14b8ca23951e0c6f0f2f40b46987
            Signature signature = Signature.Parse(Tools.HexStringToBytes("3045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed"));
            BigInteger z = Tools.MakeBigInteger("027e0c5994dec7824e56dec6b2fcb342eb7cdb0d0957c2fce9882f715e85d81a6");
            AssertTrue(point.Verify(z, signature));
        }

        public static void test_chapter_7_p134_mod_tx()
        {
            Console.WriteLine("Hash of modified transaction");

            string modified_tx = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000001976a914a802fc56c704ce87c42d7c92eb75e7896bdc41ae88acfeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac1943060001000000";

            byte[] hash256 = Tools.Hash256(Tools.HexStringToBytes(modified_tx));
            BigInteger z = Tools.BigIntegerFromBytes(hash256, "big");
            String strZ = Tools.BigIntegerToHexString(z);
            String result = "0x27e0c5994dec7824e56dec6b2fcb342eb7cdb0d0957c2fce9882f715e85d81a6";
            Console.WriteLine(result);

            AssertTrue(result.Equals(strZ));
        }

        public static void test_chapter_7_p134_verify()
        {
            byte[] sec = Tools.HexStringToBytes("0349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278a");
            byte[] der = Tools.HexStringToBytes("3045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed");
            BigInteger z = Tools.MakeBigInteger("027e0c5994dec7824e56dec6b2fcb342eb7cdb0d0957c2fce9882f715e85d81a6");

            S256Point point = S256Point.Parse(sec);
            Signature signature = Signature.Parse(der);
            
            bool success = point.Verify(z, signature);
            AssertTrue(success);
        }
    }
}

