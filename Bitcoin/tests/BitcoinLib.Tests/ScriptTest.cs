using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Globalization;
using System.Linq;

namespace BitcoinLib.Test
{
    public class ScriptTest: UnitTest
    {
        public static void test_parse()
        {
            string raw = "6a47304402207899531a52d59a6de200179928ca900254a36b8dff8bb75f5f5d71b1cdc26125022008b422690b8461cb52c3cc30330b23d574351872b7c361e9aae3649071c1a7160121035d5c93d9ac96881f19ba1f686f15f009ded7c62efe85a872e6a19b43c15a2937";
            Script script = Script.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw))));

            //
            // check the string values
            //
            string expected = "304402207899531a52d59a6de200179928ca900254a36b8dff8bb75f5f5d71b1cdc26125022008b422690b8461cb52c3cc30330b23d574351872b7c361e9aae3649071c1a71601";
            string computed = Tools.BytesToHexString(script._cmds[0]._element);
            AssertEqual(computed, expected);

            //
            // check byte arrays
            //
            byte[] expected_bytes = Tools.HexStringToBytes("035d5c93d9ac96881f19ba1f686f15f009ded7c62efe85a872e6a19b43c15a2937");
            byte[] computed_bytes = script._cmds[1]._element;
            AssertEqual(expected_bytes, computed_bytes);
        }

        public static void test_serialize()
        {
            string raw = "6a47304402207899531a52d59a6de200179928ca900254a36b8dff8bb75f5f5d71b1cdc26125022008b422690b8461cb52c3cc30330b23d574351872b7c361e9aae3649071c1a7160121035d5c93d9ac96881f19ba1f686f15f009ded7c62efe85a872e6a19b43c15a2937";
            byte[] raw_bytes = Tools.HexStringToBytes(raw);
            Script script = Script.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw))));

            List<byte> computed = new List<byte>();
            script.serialize(computed);

            AssertEqual(computed, raw_bytes);
        }

        public static void test_chapter_6_checksig_p115()
        {
            // p 115:
            BigInteger z = BigInteger.Parse("7c076ff316692a3d7eb3c3bb0f8b1488cf72e1afcd929e29307032997a838a3d", NumberStyles.AllowHexSpecifier);
            byte[] sec = Tools.HexStringToBytes("04887387e452b8eacc4acfde10d9aaf7f6d9a0f975aabb10d006e4da568744d06c61de6d95231cd89026e286df3b6ae4a894a3378e393e93a0f45b666329a0ae34");
            byte[] sig = Tools.HexStringToBytes("3045022000eff69ef2b1bd93a66ed5219add4fb51e11a840f404876325a1e8ffe0529a2c022100c7207fee197d27c618aea621406f6bf5ef6fca38681d82b2f06fddbdce6feab601");

            Script script_op_checksig = Script.Parse("AC");
            Script script_sec = new Script(sec);
            Script script_pubkey = script_sec.Add(script_op_checksig);
            Script script_sig = new Script(sig);

            Script combined_script = script_sig.Add(script_pubkey);

            Console.WriteLine("z                =" + z);
            Console.WriteLine("script_pubkey    =" + script_pubkey);
            Console.WriteLine("script_sig       =" + script_sig);
            Console.WriteLine("combined_script  =" + combined_script);

            Console.WriteLine("p 118: op_checksig : " + combined_script.Evaluate(z, null));
        }

        public static void test_chapter_6_ex_3()
        {
            Script script_pubkey = Script.Parse("767695935687");
            Script script_sig = Script.Parse("52");
            Script combined_script = script_sig.Add(script_pubkey);

            Console.WriteLine(combined_script);
            Console.WriteLine("Result x*x + x = 6 (x=2):" + combined_script.Evaluate(0, null));
        }

        public static void test_chapter_6_ex_4()
        {
            //Script.DebugDumpStacks = true;

            Script script_pubkey = Script.Parse("6e879169a77ca787");

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

            Script script_sig = new Script(c1, c2);
            Script combined_script = script_sig.Add(script_pubkey);
            Console.WriteLine("SHA1 collision p128: " + combined_script.Evaluate(0, null));
        }
        public static void test_chapter_7_p140()
        {
            string address = "mnrVtF8DWjMu839VW3rBfgYaAfKk8983Xf";
            string strExpected = "1976a914507b27411ccf7f16f10297de6cef3f291623eddf88ac";

            byte[] hash160 = Base58Encoding.DecodeH160(address);

            Script scriptPubKey = Script.Create_P2PKH_Script(hash160);

            byte[] bScriptPubKey2 = scriptPubKey.serialize();
            string strScriptPubKey2 = Tools.BytesToHexString(bScriptPubKey2);

            AssertTrue(strExpected.Equals(strScriptPubKey2));
        }

        public static void test_chapter_8_p2sh_p156()
        {
            BigInteger z = Tools.MakeBigInteger("0e71bfa115715d6fd33796948126f40a8cdd39f187e4afb03896795189fe1423c");

            byte[] sig1 = Tools.HexStringToBytes("3045022100dc92655fe37036f47756db8102e0d7d5e28b3beb83a8fef4f5dc0559bddfb94e02205a36d4e4e6c7fcd16658c50783e00c341609977aed3ad00937bf4ee942a8993701");
            byte[] sig2 = Tools.HexStringToBytes("3045022100da6bee3c93766232079a01639d07fa869598749729ae323eab8eef53577d611b02207bef15429dcadce2121ea07f233115c6f09034c0be68db99980b9a6c5e75402201");
            byte[] redeemScript = Tools.HexStringToBytes("5221022626e955ea6ea6d98850c994f9107b036b1334f18ca8830bfff1295d21cfdb702103b287eaf122eea69030a0e9feed096bed8045c8b98bec453e1ffac7fbdbd4bb7152ae");
            byte[] hash20 = Tools.HexStringToBytes("74d691da1574e6b3c192ecfb52cc8984ee7b6c56");

            OpItems stack = new OpItems();

            stack.Push(new OpItem(0));
            stack.Push(sig1);
            stack.Push(sig2);
            stack.Push(redeemScript);
            stack.Push(new OpItem(Script.OP_HASH160));
            stack.Push(hash20);
            stack.Push(new OpItem(Script.OP_EQUAL));

            Script script = new Script(stack);
            bool success = script.Evaluate(z, null);
            AssertTrue(success);
        }

        public static void test_address()
        {
            string address_1 = "1BenRpVUFK65JFWcQSuHnJKzc4M8ZP8Eqa";
            byte[] h160 = Base58Encoding.DecodeH160(address_1);

            Script script = Script.Create_P2PKH_Script(h160);
            string actual = script.Address(false);
            AssertEqual(address_1, actual);

            string address_2 = "mrAjisaT4LXL5MzE81sfcDYKU3wqWSvf9q";
            actual = script.Address(true);
            AssertEqual(address_2, actual);

            string address_3 = "3CLoMMyuoDQTPRD3XYZtCvgvkadrAdvdXh";
            h160 = Base58Encoding.DecodeH160(address_3);
            script = Script.Create_P2SH_Script(h160);
            actual = script.Address(false);
            AssertEqual(address_3, actual);

            string address_4 = "2N3u1R6uwQfuobCqbCgBkpsgBxvr1tZpe7B";
            actual = script.Address(true);
            AssertEqual(address_4, actual);
        }
    }
}
