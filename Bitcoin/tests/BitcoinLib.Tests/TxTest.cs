using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BitcoinLib.Test
{
    public class TxTest : UnitTest
    {
        public static void test_chapter_5_ex_5()
        {
            ConsoleOutWriteLine("parse one transaction");

            string hex_transaction = "010000000456919960ac691763688d3d3bcea9ad6ecaf875df5339e148a1fc61c6ed7a069e010000006a47304402204585bcdef85e6b1c6af5c2669d4830ff86e42dd205c0e089bc2a821657e951c002201024a10366077f87d6bce1f7100ad8cfa8a064b39d4e8fe4ea13a7b71aa8180f012102f0da57e85eec2934a82a585ea337ce2f4998b50ae699dd79f5880e253dafafb7feffffffeb8f51f4038dc17e6313cf831d4f02281c2a468bde0fafd37f1bf882729e7fd3000000006a47304402207899531a52d59a6de200179928ca900254a36b8dff8bb75f5f5d71b1cdc26125022008b422690b8461cb52c3cc30330b23d574351872b7c361e9aae3649071c1a7160121035d5c93d9ac96881f19ba1f686f15f009ded7c62efe85a872e6a19b43c15a2937feffffff567bf40595119d1bb8a3037c356efd56170b64cbcc160fb028fa10704b45d775000000006a47304402204c7c7818424c7f7911da6cddc59655a70af1cb5eaf17c69dadbfc74ffa0b662f02207599e08bc8023693ad4e9527dc42c34210f7a7d1d1ddfc8492b654a11e7620a0012102158b46fbdff65d0172b7989aec8850aa0dae49abfb84c81ae6e5b251a58ace5cfeffffffd63a5e6c16e620f86f375925b21cabaf736c779f88fd04dcad51d26690f7f345010000006a47304402200633ea0d3314bea0d95b3cd8dadb2ef79ea8331ffe1e61f762c0f6daea0fabde022029f23b3e9c30f080446150b23852028751635dcee2be669c2a1686a4b5edf304012103ffd6f4a67e94aba353a00882e563ff2722eb4cff0ad6006e86ee20dfe7520d55feffffff0251430f00000000001976a914ab0c0b2e98b1ab6dbf67d4750b0a56244948a87988ac005a6202000000001976a9143c82d7df364eb6c75be8c80df2b3eda8db57397088ac46430600";

            Tx tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(hex_transaction))));

            Console.Out.WriteLine("Raw:");
            Console.Out.WriteLine(hex_transaction);
            string tx_serialized = Tools.BytesToHexString(tx.serialize());
            AssertTrue(hex_transaction == tx_serialized);
            Console.Out.WriteLine("Serialized:");
            Console.Out.WriteLine(tx_serialized);
            Console.Out.WriteLine(tx.ToString());

            Script script_sig = tx._txIns[1]._script_sig;
            Script script_pubkey = tx._txOuts[0]._script_pubkey;
            ulong amount = tx._txOuts[1]._amount;
            AssertTrue(amount == 40000000);
            Console.WriteLine("ScriptSig={0}\nScriptPubKey={1}\amount={2}",
                script_sig.ToString(), script_pubkey.ToString(), amount);
        }

        public static void test_parse_version()
        {
            string raw = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";

            Tx tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw))));
            AssertTrue(tx._version == 1);
        }

        public static void test_parse_inputs()
        {
            string raw = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";

            Tx tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw))));
            AssertTrue(tx._txIns.Count == 1);

            string want = "d1c789a9c60383bf715f3f6ad9d14b91fe55f3deb369fe5d9280cb1a01793f81";
            byte[] want_bytes = Tools.HexStringToBytes(want);
            AssertEqual(tx._txIns[0]._prev_tx, want_bytes);
            AssertTrue(tx._txIns[0]._prev_index == 0);

            want = "6b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278a";
            want_bytes = Tools.HexStringToBytes(want);
            List<byte> lst_bytes = new List<byte>();
            tx._txIns[0]._script_sig.serialize(lst_bytes);
            AssertEqual(lst_bytes, want_bytes);
            AssertTrue(tx._txIns[0]._sequence == 0xfffffffe);
        }

        public static void test_parse_outputs()
        {
            string raw = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";

            Tx tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw))));
            AssertTrue(tx._txOuts.Count == 2);

            AssertTrue(tx._txOuts[0]._amount == 32454049);
            string want = "1976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac";
            byte[] want_bytes = Tools.HexStringToBytes(want);
            List<byte> lst_bytes = new List<byte>();
            tx._txOuts[0]._script_pubkey.serialize(lst_bytes);
            AssertEqual(lst_bytes, want_bytes);

            AssertTrue(tx._txOuts[1]._amount == 10011545);
            want = "1976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac";
            want_bytes = Tools.HexStringToBytes(want);
            lst_bytes.Clear();
            tx._txOuts[1]._script_pubkey.serialize(lst_bytes);
            AssertEqual(lst_bytes, want_bytes);
        }

        public static void test_parse_locktime()
        {
            string raw = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";

            Tx tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw))));
            AssertTrue(tx._locktime == 410393);
        }

        public static void test_fee()
        {
            string raw = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";
            Tx tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw))));
            AssertTrue(tx.Fee() == 40000);

            raw = "010000000456919960ac691763688d3d3bcea9ad6ecaf875df5339e148a1fc61c6ed7a069e010000006a47304402204585bcdef85e6b1c6af5c2669d4830ff86e42dd205c0e089bc2a821657e951c002201024a10366077f87d6bce1f7100ad8cfa8a064b39d4e8fe4ea13a7b71aa8180f012102f0da57e85eec2934a82a585ea337ce2f4998b50ae699dd79f5880e253dafafb7feffffffeb8f51f4038dc17e6313cf831d4f02281c2a468bde0fafd37f1bf882729e7fd3000000006a47304402207899531a52d59a6de200179928ca900254a36b8dff8bb75f5f5d71b1cdc26125022008b422690b8461cb52c3cc30330b23d574351872b7c361e9aae3649071c1a7160121035d5c93d9ac96881f19ba1f686f15f009ded7c62efe85a872e6a19b43c15a2937feffffff567bf40595119d1bb8a3037c356efd56170b64cbcc160fb028fa10704b45d775000000006a47304402204c7c7818424c7f7911da6cddc59655a70af1cb5eaf17c69dadbfc74ffa0b662f02207599e08bc8023693ad4e9527dc42c34210f7a7d1d1ddfc8492b654a11e7620a0012102158b46fbdff65d0172b7989aec8850aa0dae49abfb84c81ae6e5b251a58ace5cfeffffffd63a5e6c16e620f86f375925b21cabaf736c779f88fd04dcad51d26690f7f345010000006a47304402200633ea0d3314bea0d95b3cd8dadb2ef79ea8331ffe1e61f762c0f6daea0fabde022029f23b3e9c30f080446150b23852028751635dcee2be669c2a1686a4b5edf304012103ffd6f4a67e94aba353a00882e563ff2722eb4cff0ad6006e86ee20dfe7520d55feffffff0251430f00000000001976a914ab0c0b2e98b1ab6dbf67d4750b0a56244948a87988ac005a6202000000001976a9143c82d7df364eb6c75be8c80df2b3eda8db57397088ac46430600";
            tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw))));
            AssertTrue(tx.Fee() == 140500);
        }

        public static void test_chapter_5()
        {
            Tx tx;
            string raw_tx;
            ulong fee;
            //string raw1 = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";
            string raw2 = "010000000456919960ac691763688d3d3bcea9ad6ecaf875df5339e148a1fc61c6ed7a069e010000006a47304402204585bcdef85e6b1c6af5c2669d4830ff86e42dd205c0e089bc2a821657e951c002201024a10366077f87d6bce1f7100ad8cfa8a064b39d4e8fe4ea13a7b71aa8180f012102f0da57e85eec2934a82a585ea337ce2f4998b50ae699dd79f5880e253dafafb7feffffffeb8f51f4038dc17e6313cf831d4f02281c2a468bde0fafd37f1bf882729e7fd3000000006a47304402207899531a52d59a6de200179928ca900254a36b8dff8bb75f5f5d71b1cdc26125022008b422690b8461cb52c3cc30330b23d574351872b7c361e9aae3649071c1a7160121035d5c93d9ac96881f19ba1f686f15f009ded7c62efe85a872e6a19b43c15a2937feffffff567bf40595119d1bb8a3037c356efd56170b64cbcc160fb028fa10704b45d775000000006a47304402204c7c7818424c7f7911da6cddc59655a70af1cb5eaf17c69dadbfc74ffa0b662f02207599e08bc8023693ad4e9527dc42c34210f7a7d1d1ddfc8492b654a11e7620a0012102158b46fbdff65d0172b7989aec8850aa0dae49abfb84c81ae6e5b251a58ace5cfeffffffd63a5e6c16e620f86f375925b21cabaf736c779f88fd04dcad51d26690f7f345010000006a47304402200633ea0d3314bea0d95b3cd8dadb2ef79ea8331ffe1e61f762c0f6daea0fabde022029f23b3e9c30f080446150b23852028751635dcee2be669c2a1686a4b5edf304012103ffd6f4a67e94aba353a00882e563ff2722eb4cff0ad6006e86ee20dfe7520d55feffffff0251430f00000000001976a914ab0c0b2e98b1ab6dbf67d4750b0a56244948a87988ac005a6202000000001976a9143c82d7df364eb6c75be8c80df2b3eda8db57397088ac46430600";

            Tx tx2 = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw2))));
            Console.Out.WriteLine();
            Console.Out.WriteLine("Raw:");
            Console.Out.WriteLine(raw2);
            Console.Out.WriteLine("Serialized:");
            string ser2 = Tools.BytesToHexString(tx2.serialize());
            Console.Out.WriteLine(ser2);
            Console.Out.WriteLine(tx2);
            AssertTrue(raw2 == ser2);

            raw_tx ="0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";
            tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw_tx))));
            fee = tx.Fee();
            AssertTrue(fee == 40000);
        
            raw_tx = "0100000001c228021e1fee6f158cc506edea6bad7ffa421dd14fb7fd7e01c50cc9693e8dbe02000000fdfe0000483045022100c679944ff8f20373685e1122b581f64752c1d22c67f6f3ae26333aa9c3f43d730220793233401f87f640f9c39207349ffef42d0e27046755263c0a69c436ab07febc01483045022100eadc1c6e72f241c3e076a7109b8053db53987f3fcc99e3f88fc4e52dbfd5f3a202201f02cbff194c41e6f8da762e024a7ab85c1b1616b74720f13283043e9e99dab8014c69522102b0c7be446b92624112f3c7d4ffc214921c74c1cb891bf945c49fbe5981ee026b21039021c9391e328e0cb3b61ba05dcc5e122ab234e55d1502e59b10d8f588aea4632102f3bd8f64363066f35968bd82ed9c6e8afecbd6136311bb51e91204f614144e9b53aeffffffff05a08601000000000017a914081fbb6ec9d83104367eb1a6a59e2a92417d79298700350c00000000001976a914677345c7376dfda2c52ad9b6a153b643b6409a3788acc7f341160000000017a914234c15756b9599314c9299340eaabab7f1810d8287c02709000000000017a91469be3ca6195efcab5194e1530164ec47637d44308740420f00000000001976a91487fadba66b9e48c0c8082f33107fdb01970eb80388ac00000000";
            tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw_tx))));

            raw_tx = "010000000456919960ac691763688d3d3bcea9ad6ecaf875df5339e148a1fc61c6ed7a069e010000006a47304402204585bcdef85e6b1c6af5c2669d4830ff86e42dd205c0e089bc2a821657e951c002201024a10366077f87d6bce1f7100ad8cfa8a064b39d4e8fe4ea13a7b71aa8180f012102f0da57e85eec2934a82a585ea337ce2f4998b50ae699dd79f5880e253dafafb7feffffffeb8f51f4038dc17e6313cf831d4f02281c2a468bde0fafd37f1bf882729e7fd3000000006a47304402207899531a52d59a6de200179928ca900254a36b8dff8bb75f5f5d71b1cdc26125022008b422690b8461cb52c3cc30330b23d574351872b7c361e9aae3649071c1a7160121035d5c93d9ac96881f19ba1f686f15f009ded7c62efe85a872e6a19b43c15a2937feffffff567bf40595119d1bb8a3037c356efd56170b64cbcc160fb028fa10704b45d775000000006a47304402204c7c7818424c7f7911da6cddc59655a70af1cb5eaf17c69dadbfc74ffa0b662f02207599e08bc8023693ad4e9527dc42c34210f7a7d1d1ddfc8492b654a11e7620a0012102158b46fbdff65d0172b7989aec8850aa0dae49abfb84c81ae6e5b251a58ace5cfeffffffd63a5e6c16e620f86f375925b21cabaf736c779f88fd04dcad51d26690f7f345010000006a47304402200633ea0d3314bea0d95b3cd8dadb2ef79ea8331ffe1e61f762c0f6daea0fabde022029f23b3e9c30f080446150b23852028751635dcee2be669c2a1686a4b5edf304012103ffd6f4a67e94aba353a00882e563ff2722eb4cff0ad6006e86ee20dfe7520d55feffffff0251430f00000000001976a914ab0c0b2e98b1ab6dbf67d4750b0a56244948a87988ac005a6202000000001976a9143c82d7df364eb6c75be8c80df2b3eda8db57397088ac46430600";
            tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw_tx))));
            fee = tx.Fee();
            AssertTrue(fee == 140500);

            // example with 4 inputs and a witness for input no 4.
            /*
VERSION 01000000
SEGWIT (BIP141):       this is a segwit tx, marker=00
       (BIP141):       flag=01
TX_IN COUNT [var_int]: hex=04, decimal=4
 TX_IN[0]
  TX_IN[0] OutPoint hash  08A1266CED5EF064741BD4BC51C1202456F22509AE030231860D6E9BEF4ACD5E
  TX_IN[0] OutPoint index hex=62000000, reversed=00000062, decimal=98
  TX_IN[0] Script Length  hex=FC, decimal=252
  TX_IN[0] Script Sig     0047304402207E38831ECA394E472E...555C0E2D7A9D9915D6986BFC200453AE 
  TX_IN[0] Sequence       FFFFFFFF
 TX_IN[1]
  TX_IN[1] OutPoint hash  AD4C8508B8D5EECE6FD100B58D667DEA9C7A8C178C1B06602C1E3358D8105C0B
  TX_IN[1] OutPoint index hex=7D000000, reversed=0000007D, decimal=125
  TX_IN[1] Script Length  hex=FC, decimal=252
  TX_IN[1] Script Sig     0047304402203299B925B1F2C87282...ABDC12E55B0F545FFF14667A515F53AE 
  TX_IN[1] Sequence       FFFFFFFF
 TX_IN[2]
  TX_IN[2] OutPoint hash  C8066B798384B502F225BD89F7EB405265357CB0BDC0C169FE96B013310629B2
  TX_IN[2] OutPoint index hex=A3000000, reversed=000000A3, decimal=163
  TX_IN[2] Script Length  hex=00FD, decimal=253
  TX_IN[2] Script Sig     0047304402204D4DA5303BE178D649...B5A43BC43D60C844F65DB8FB78AD53AE 
  TX_IN[2] Sequence       FFFFFFFF
 TX_IN[3]
  TX_IN[3] OutPoint hash  D80FF02D0D9EB2DA8C8A1C47AB099901F447DD197E34220EA13ECA72D7D6D21D
  TX_IN[3] OutPoint index hex=9A000000, reversed=0000009A, decimal=154
  TX_IN[3] Script Length  hex=23, decimal=35
  TX_IN[3] Script Sig     22002044C55C1DA36A576217259C3BC21B0C3943F7EB3FF4E3C381D9FD3502434B9E87 
  TX_IN[3] Sequence       FFFFFFFF
TX_OUT COUNT, hex=05, decimal=5
 TX_OUT[0]
  TX_OUT[0] Value         hex=C0D4010000000000, reversed_hex=000000000001D4C0, dec=120000
  TX_OUT[0] PK_Script Len hex=17, dec=23
  TX_OUT[0] pk_script     A914A1932CFD432D928311B4ADA550BBC468D1E909B787
 TX_OUT[1]
  TX_OUT[1] Value         hex=A086010000000000, reversed_hex=00000000000186A0, dec=100000
  TX_OUT[1] PK_Script Len hex=17, dec=23
  TX_OUT[1] pk_script     A9146B0E7A66416F1D8598B5956576ADB22DAF79853E87
 TX_OUT[2]
  TX_OUT[2] Value         hex=3A4A000000000000, reversed_hex=0000000000004A3A, dec=19002
  TX_OUT[2] PK_Script Len hex=17, dec=23
  TX_OUT[2] pk_script     A914EC4C73145428ABBE0B1C40FBF58C59F0EF3C29F487
 TX_OUT[3]
  TX_OUT[3] Value         hex=382C050000000000, reversed_hex=0000000000052C38, dec=339000
  TX_OUT[3] PK_Script Len hex=17, dec=23
  TX_OUT[3] pk_script     A914ABB18A298E5B629BF5652F341D2CD8207CCC214A87
 TX_OUT[4]
  TX_OUT[4] Value         hex=8010020000000000, reversed_hex=0000000000021080, dec=135296
  TX_OUT[4] PK_Script Len hex=19, dec=25
  TX_OUT[4] pk_script     76A91438D769CF2899983022B5611AB4D35BF7907DAE2088AC

WITNESS TXIN[0] stack elements: hex=00, decimal=0
WITNESS TXIN[1] stack elements: hex=00, decimal=0
WITNESS TXIN[2] stack elements: hex=00, decimal=0
WITNESS TXIN[3] stack elements: hex=04, decimal=4
 WITNESS data[0]: 00
 WITNESS data[1]: 4730440220...D207D14D45945A6901
 WITNESS data[2]: 4730440220...36B45FAFAEB96E0F01
 WITNESS data[3]: 695221021E...3234312D684C6553AE

LOCK_TIME 00000000             
             */
            string tx_id = "80975cddebaa93aa21a6477c0d050685d6820fa1068a2731db0f39b535cbd369";
            raw_tx = "010000000001045ecd4aef9b6e0d86310203ae0925f2562420c151bcd41b7464f05eed6c26a10862000000fc0047304402207e3e1158831eca394e472e43ec2a4c9f10d034a83f0f7142e6c38c243e6074f9022000b10a29bccf3c31f61e047a400d1a8d620cf8be7fb39ea5c51c6aeac83e7e6b0147304402205bc85c03a0f786bdf6a985911cf27d94d6f4c0f00295236a304967564cca492a022011e0d80900998f601290223240ed21d13dcce11b1d045383361978ac02e27c97014c69522102194e1b5671daff4edc82ce01589e7179a874f63d6e5157fa0def116acd2c3a522103a043861e123bc67ddcfcd887b167e7ff9d00702d1466524157cf3b28c7aca71b2102a49a62a9470a31ee51824f0ee859b0534a4f555c0e2d7a9d9915d6986bfc200453aeffffffff0b5c10d858331e2c60061b8c178c7a9cea7d668db500d16fceeed5b808854cad7d000000fc0047304402203299b925b1f2c87282d2889c2bb0e07372f916d7c4781f43f2e6d1403b2425fe0220466d075c56cdcf1d659dd40edcfc68298826f935beabe12f7404c7fd1e496c8601473044022048dfe509326808f9367c88da0f14968121d31b45461a11e6ed640e72f6a53a300220517914666f2f0f1d2c306de49599bf0a95f59cd57eaaef49e44a4e48a8d9e139014c69522103b5fd9803c0046386a9c7a1ac80c102ac4bc1e6dfaec036b0ca3adebe1ca961c92102b8b42d1c84d778c4fa78c609611a9cb847c3d7bff231e5751f380503c583d36321030d2c5aee1d650c2a3150e1c66a1f1e7236ecabdc12e55b0f545fff14667a515f53aeffffffffb229063113b096fe69c1c0bdb07c35655240ebf789bd25f202b58483796b06c8a3000000fdfd000047304402204d4da5303be178d649cfab85f4d6777c365934f015b773f2269e2cc4a819eaae02207f79285ddc34c6def51df243a5abc5f36179f407172bcae88feb04da1ab1b00001483045022100b831d970bc3ea88bc6b717bbd1ad8aca9bcc8e6545988ee9718db75891db2e1702200d6bf7c4b91abcc32a610cf52112e550ae853b2f216b88803b560f5adc0d9742014c69522102c44af6aea46b1b7a9373078437ecdf993b701efd2cc297414d8eab5063887dce2103546047f27105c7db32ebe5f3f8655856d2c27ecff80614b36da6e3cf84e88d8321022fa39834a8308abba605b1b2315b508a3268b5a43bc43d60c844f65db8fb78ad53aeffffffff1dd2d6d772ca3ea10e22347e19dd47f4019909ab471c8a8cdab29e0d2df00fd89a0000002322002044c55c1da36a576217259c3bc21b0c3943f7eb3ff4e3c381d9fd3502434b9e87ffffffff05c0d401000000000017a914a1932cfd432d928311b4ada550bbc468d1e909b787a08601000000000017a9146b0e7a66416f1d8598b5956576adb22daf79853e873a4a00000000000017a914ec4c73145428abbe0b1c40fbf58c59f0ef3c29f487382c05000000000017a914abb18a298e5b629bf5652f341d2cd8207ccc214a8780100200000000001976a91438d769cf2899983022b5611ab4d35bf7907dae2088ac000000040047304402202c3f94e5daf4057377d9f16d45b57e962de42fb42cb7e95a0382b7c66624980a02204098f6acd43b0391ea1b4a8102797e78895848fb7e883f98d207d14d45945a69014730440220448460edd5291a548c571ccf3a72caf47b02364035dc84f420d311e3a0c5494802205bb1cc89f20dc1e2c1f6eadb74898f8eecc46fbf488b676636b45fafaeb96e0f01695221021e6617e06bb90f621c3800e8c37ab081a445ae5527f6c5f68a022e7133f9b5fe2103bea1a8ce6369435bb74ff1584a136a7efeebfe4bc320b4d59113c92acd869f38210280631b27700baf7d472483fadfe1c4a7340a458f28bf6bae5d3234312d684c6553ae00000000";
            tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(raw_tx))));
            Console.WriteLine(tx);
            AssertTrue(tx_id == tx.Id());
        }

        public static void test_chapter_7_ex_1()
        {
            Tx tx = TxFetcher.Fetch("452c629d67e41baec3ac6f04fe744b4b9617f8f859c63b3002f8684e7a4fee03");
            BigInteger want = Tools.MakeBigInteger("27e0c5994dec7824e56dec6b2fcb342eb7cdb0d0957c2fce9882f715e85d81a6");

            BigInteger z = tx.SigHash(0, null);

            // with txOut length ok
            // 0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000001976a914a802fc56c704ce87c42d7c92eb75e7896bdc41ae88acfeffffff02

            // with all TxOuts: ok
            // 0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000001976a914a802fc56c704ce87c42d7c92eb75e7896bdc41ae88acfeffffff02
            // a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac

            // with SIGHASH_ALL: ok
            // 0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000001976a914a802fc56c704ce87c42d7c92eb75e7896bdc41ae88acfeffffff02     72 bytes
            // a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac             68 bytes
            // 1943060001000000                                                                                                                                      8 bytes -> 148 bytes

            // hash256 of s:
            // 27e0c5994dec7824e56dec6b2fcb342eb7cdb0d0957c2fce9882f715e85d81a6     32 bytes

            AssertEqual(want, z);
        }

        public static void test_chapter_7_ex_2()
        {
            // test_verify_p2pkh(self):
            Tx tx = TxFetcher.Fetch("452c629d67e41baec3ac6f04fe744b4b9617f8f859c63b3002f8684e7a4fee03");

            // alles vor evaluate ist ok!
            // the Tx was modified by the code in Tx.Verify and this also modifies the Tx in the cache, so it must be reloaded every time
            // call Verify() multiple time on the same Tx to make sure no code modifies the Tx data.
            AssertTrue(tx.Verify());
            AssertTrue(tx.Verify());

            tx = TxFetcher.Fetch("5418099cc755cb9dd3ebc6cf1a7888ad53a1a3beb5a025bce89eb1bf7f1650a2", true, true);
            AssertTrue(tx.Verify());
            AssertTrue(tx.Verify());
        }

        public static void test_chapter_7_p140_create_transaction()
        {
            // See p137:
            // transaction with ID = 0d6fe5213c0b3291f208cba8bfb59b7476dffacc4e5cb66f6eb20a080843a299 has 14 outputs
            // at output index 13, 0,44 BTC are paid to address mzx5YhAH9kNHtcN481u6WkjeHjYtVeKVh2 (compressed testnet address of private key 8675309, see test_public_key_hash160).
            //
            // this function does this:
            //
            // We pay 0,1 BTC from that output to address mnrVtF8DWjMu839VW3rBfgYaAfKk8983Xf
            // We pay 0,33 BTC from that output back to mzx5YhAH9kNHtcN481u6WkjeHjYtVeKVh2 (private key = 8675309)
            // This leaves a fee of 0,44 - (0,1 + 0,33) = 0,44 - 0,43 = 0,01 BTC
            //

            Console.WriteLine("private key 8675309 ==> compressed/testnet ==> public ke/address mzx5YhAH9kNHtcN481u6WkjeHjYtVeKVh2");

            byte[] prev_tx = Tools.HexStringToBytes("0d6fe5213c0b3291f208cba8bfb59b7476dffacc4e5cb66f6eb20a080843a299");
            UInt32 prev_index = 13;
            TxIn txin = new TxIn(prev_tx, prev_index);

            //
            // we pay 0,33 BTC back to mzx5YhAH9kNHtcN481u6WkjeHjYtVeKVh2 who received the initial 0,44 BTC in the previous transaction output at index 13
            //
            UInt64 change_amount = (UInt64)(0.33 * 100000000);
            byte[] change_h160 = Base58Encoding.DecodeH160("mzx5YhAH9kNHtcN481u6WkjeHjYtVeKVh2");
            Script change_script = Script.Create_P2PKH_Script(change_h160);
            TxOut change_output = new TxOut(change_amount, change_script);

            //
            // we pay 0,1 BTC to address mnrVtF8DWjMu839VW3rBfgYaAfKk8983Xf - don't know who that is and we also don't have that private key
            //
            UInt64 target_amount = (UInt64)(0.1 * 100000000);
            byte[] target_h160 = Base58Encoding.DecodeH160("mnrVtF8DWjMu839VW3rBfgYaAfKk8983Xf");
            Script target_script = Script.Create_P2PKH_Script(target_h160);
            TxOut target_output = new TxOut(target_amount, target_script);

            List<TxIn> inputs = new List<TxIn>();
            List<TxOut> outputs = new List<TxOut>();
            inputs.Add(txin);
            outputs.Add(change_output);
            outputs.Add(target_output);

            Tx tx = new Tx(1, 0, 0, inputs, outputs, 0);
            tx._testnet = true;

            Console.WriteLine("tx=");
            string strTx = tx.ToString();
            Console.WriteLine(strTx);
        }

        public static void test_chapter_7_ex_3()
        {
            PrivateKey private_key = new PrivateKey(Tools.MakeBigIntegerBase10("8675309"));
            string strTx = "010000000199a24308080ab26e6fb65c4eccfadf76749bb5bfa8cb08f291320b3c21e56f0d0d00000000ffffffff02408af701000000001976a914d52ad7ca9b3d096a38e752c2018e6fbc40cdf26f88ac80969800000000001976a914507b27411ccf7f16f10297de6cef3f291623eddf88ac00000000";
            Tx tx = Tx.Parse(strTx);
            tx._testnet = true;
            Console.WriteLine("tx:");
            Console.WriteLine(tx.ToString());

            Console.WriteLine("Before SignInput()");
            bool success = tx.SignInputPtpkh(0, private_key);
            Console.WriteLine("After SignInput()");
            AssertTrue(success);

            Console.WriteLine("Before SignInput()");
            success = tx.SignInputPtpkh(0, private_key);
            Console.WriteLine("After SignInput()");
            AssertTrue(success);

            Console.WriteLine("tx:");
            Console.WriteLine(tx.ToString());

            string want = "010000000199a24308080ab26e6fb65c4eccfadf76749bb5bfa8cb08f291320b3c21e56f0d0d0000006b4830450221008ed46aa2cf12d6d81065bfabe903670165b538f65ee9a3385e6327d80c66d3b502203124f804410527497329ec4715e18558082d489b218677bd029e7fa306a72236012103935581e52c354cd2f484fe8ed83af7a3097005b2f9c60bff71d35bd795f54b67ffffffff02408af701000000001976a914d52ad7ca9b3d096a38e752c2018e6fbc40cdf26f88ac80969800000000001976a914507b27411ccf7f16f10297de6cef3f291623eddf88ac00000000";
            string strTxSerialized = Tools.BytesToHexString(tx.serialize());
            AssertTrue(strTxSerialized.Equals(want));
        }

        public static void test_chapter_7_p141_get_some_coins()
        {
            //
            // https://coinfaucet.eu/en/btc-testnet/ ok!
            // das ist meine Adresse für das Testen in diesem Buch:
            // 
            // Passphrase = 'ch.maurer@gmx.de sein secret ist streng geheim'
            // Private key = 14571969162455461715208563475110979223892067646684683083960296457821644624115
            // Public key = S256Point(0x21662d381ffe1d5d85c08e6d313735fb075ff1166a30e53230f6158e5fedf1a4, 0x1b8ce0333112898e7e53473b8e3e1b965303fadf3cfe4cb97117bb8d4d7d49bb)
            // Public address = mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            //

            /*
            https://coinfaucet.eu/en/btc-testnet/

We sent 0.00297998 bitcoins to address
mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx

tx: 138a71b2bd9b6f5dcd6583662f3ba61666194078ff7ad9b282004b010cc57a89
Send coins back, when you don't need them anymore to the address

tb1qerzrlxcfu24davlur5sqmgzzgsal6wusda40er
             */
        }

        public static void test_chapter_7_ex_4()
        {
            /*
            https://coinfaucet.eu/en/btc-testnet/
                We sent 0.00297998 bitcoins to address mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx

                tx: 138a71b2bd9b6f5dcd6583662f3ba61666194078ff7ad9b282004b010cc57a89
                Send coins back, when you don't need them anymore to the address tb1qerzrlxcfu24davlur5sqmgzzgsal6wusda40er
             */

            //
            //one-input, two-outpts
            //
            Console.WriteLine("Create a transaction: one-input, two-output and post the raw data to the testnet");
            Console.WriteLine("Total is     0,00297998");
            Console.WriteLine("Pay          0,001787988 to unknown address mwJn1YPMq7y5F8J3LkC5Hxg9PHyZ5K4cFv");
            Console.WriteLine("Pay          0,001       to myself mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx");
            Console.WriteLine("fee is       0,000191992");

            //
            // Tx 138a...57a89 has 2 inputs, the second one sends 0.00297998 tBTC to mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            // 
            //  100 % = 0,00297998
            //   60 % = 0,001787988
            //   40 % = 0,001191992
            //
            //   60 % = 0,00178798      ->  mwJn1YPMq7y5F8J3LkC5Hxg9PHyZ5K4cFv      this goes to someone else
            //   40 % = 0,001           ->  mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx      this change goes back to me
            //          0,000192        ->  this is the fee
            //
            byte[] prev_tx = Tools.HexStringToBytes("138a71b2bd9b6f5dcd6583662f3ba61666194078ff7ad9b282004b010cc57a89");
            UInt32 prev_index = 1;

            string target_address = "mwJn1YPMq7y5F8J3LkC5Hxg9PHyZ5K4cFv";
            UInt64 target_satoshis = (UInt64)(0.00178798 * 100000000);

            string change_address = "mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx";
            UInt64 change_satoshis = (UInt64)(0.001 * 100000000);

            BigInteger secret = Tools.MakeBigIntegerBase10("14571969162455461715208563475110979223892067646684683083960296457821644624115");
            PrivateKey priv = new PrivateKey(secret);

            TxIn input = new TxIn(prev_tx, prev_index);

            byte[] h160_target = Base58Encoding.DecodeH160(target_address);
            Script script_pubkey_target = Script.Create_P2PKH_Script(h160_target);
            TxOut target_output = new TxOut(target_satoshis, script_pubkey_target);

            byte[] h160_change = Base58Encoding.DecodeH160(change_address);
            Script script_pubkey_change = Script.Create_P2PKH_Script(h160_change);
            TxOut change_output = new TxOut(change_satoshis, script_pubkey_change);

            List<TxIn> inputs = new List<TxIn>();
            List<TxOut> outputs = new List<TxOut>();
            inputs.Add(input);
            outputs.Add(target_output);
            outputs.Add(change_output);

            Tx tx = new Tx(1, inputs, outputs, 0);
            tx._testnet = true;
            bool success = tx.SignInputPtpkh(0, priv);

            Console.WriteLine("tx.SignInput(0, priv)=" + success);
            Console.WriteLine("tx.ToString()=");
            string strTx = tx.ToString();
            Console.WriteLine(strTx);
            Console.WriteLine("tx as bytes=");
            Console.WriteLine(Tools.BytesToHexString(tx.serialize()));

            //
            // this created transaction c1e03fd68b8fd89cea6ef2bf57f356396b0952f0aec6cfb9120afb047fe389c8
            // raw bytes: 0100000001897ac50c014b0082b2d97aff7840196616a63b2f668365cd5d6f9bbdb2718a13010000006a47304402206679123e767bc6d0eb7881a73b129278fb53e41411b5f60d02ae3478668975ff02206cbdb660e08baab33cf00358bc40babaca4c1c58f154993b2a6a645c3c09cb3101210321662d381ffe1d5d85c08e6d313735fb075ff1166a30e53230f6158e5fedf1a4ffffffff026eba0200000000001976a914ad346f8eb57dee9a37981716e498120ae80e44f788aca0860100000000001976a914194bb9f612d86cd02fea8ad5d95c0057abf2d2d088ac00000000
            //
            // 10.12.2025: I posted the raw bytes to https://blockstream.info/testnet/tx/push with success
            // and this can only be done once!
            //
            // check address mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            //
        }
        public static void test_chapter_7_ex_5()
        {
            /*
            We sent 0.00125044 bitcoins to address mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            tx: 5516a3d80eb45002bd1d631df7eee98c9c8fd241e1795d6f37b33102dd49dfe6
                    Send coins back, when you don't need them anymore to the address tb1qerzrlxcfu24davlur5sqmgzzgsal6wusda40er
            */

            Console.WriteLine("Create a transaction: two-input, one-output and post the raw data to the testnet");

            //
            // two-input, one-output
            //
            // input 1: Tx ID 5516a3d80eb45002bd1d631df7eee98c9c8fd241e1795d6f37b33102dd49dfe6, TxOut 0: 0.00125044 tBTC to mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            // input 2: Tx ID c1e03fd68b8fd89cea6ef2bf57f356396b0952f0aec6cfb9120afb047fe389c8, TxOut 1: 0.00100000 tBTC to mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            // 0,00125044
            // 0,00100000
            // 0,00225044   total
            //
            // 0,00225044
            // 0,0022       -> mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            // 0,00005044   -> fee
            //
            byte[] prev_tx_1 = Tools.HexStringToBytes("5516a3d80eb45002bd1d631df7eee98c9c8fd241e1795d6f37b33102dd49dfe6");
            UInt32 prev_index_1 = 0;

            // the one created in ex 4
            byte[] prev_tx_2 = Tools.HexStringToBytes("c1e03fd68b8fd89cea6ef2bf57f356396b0952f0aec6cfb9120afb047fe389c8");
            UInt32 prev_index_2 = 1;

            string target_address = "mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx";
            UInt64 target_satoshis = (UInt64)(0.0022 * 100000000);

            BigInteger secret = Tools.MakeBigIntegerBase10("14571969162455461715208563475110979223892067646684683083960296457821644624115");
            PrivateKey priv = new PrivateKey(secret);

            TxIn input_1 = new TxIn(prev_tx_1, prev_index_1);
            TxIn input_2 = new TxIn(prev_tx_2, prev_index_2);

            byte[] h160_target = Base58Encoding.DecodeH160(target_address);
            Script script_pubkey_target = Script.Create_P2PKH_Script(h160_target);
            TxOut target_output = new TxOut(target_satoshis, script_pubkey_target);

            List<TxIn> inputs = new List<TxIn>();
            List<TxOut> outputs = new List<TxOut>();
            inputs.Add(input_1);
            inputs.Add(input_2);
            outputs.Add(target_output);

            Tx tx = new Tx(1, inputs, outputs, 0);
            tx._testnet = true;
            bool success = tx.SignInputPtpkh(0, priv);
            success = tx.SignInputPtpkh(1, priv);
            
            //
            // 11.12.2025
            // this code created Tx fba591b36c27a59e878f7a67dc1d7e262d98ed99760e755683460d9a01e2e735
            // with raw bytes: 0100000002e6df49dd0231b3376f5d79e141d28f9c8ce9eef71d631dbd0250b40ed8a31655000000006a47304402205aedc320fb932b12e44e7c0e8b365040a7760e2ff805ffa7ce1aa8529a476ece022022a7a85f4886589c008b824954651c19ebe47802c860f5f075383bdd369dd91501210321662d381ffe1d5d85c08e6d313735fb075ff1166a30e53230f6158e5fedf1a4ffffffffc889e37f04fb0a12b9cfc6aef052096b3956f357bff26eea9cd88f8bd63fe0c1010000006a4730440220416261115d690f81ce287a6c7e91d32127e164296cf83efbe7a9efb631065fec022037d0456ca3c959b4782c3ff5defe11fd0826a5e79ba9ce70d7a00bcbc47ffc2e01210321662d381ffe1d5d85c08e6d313735fb075ff1166a30e53230f6158e5fedf1a4ffffffff01605b0300000000001976a914194bb9f612d86cd02fea8ad5d95c0057abf2d2d088ac00000000
            //
            // 11.12.2025: I posted the raw bytes to https://blockstream.info/testnet/tx/push with success
            // and this can only be done once!
            //
            // check address mhphuQunwspRc6tT4jFHYbH6m5JgXRbYMx
            //
        }

        public static void test_chapter_8_p_160()
        {
            byte[] modified_tx = Tools.HexStringToBytes("0100000001868278ed6ddfb6c1ed3ad5f8181eb0c7a385aa0836f01d5e4789e6bd304d87221a000000475221022626e955ea6ea6d98850c994f9107b036b1334f18ca8830bfff1295d21cfdb702103b287eaf122eea69030a0e9feed096bed8045c8b98bec453e1ffac7fbdbd4bb7152aeffffffff04d3b11400000000001976a914904a49878c0adfc3aa05de7afad2cc15f483a56a88ac7f400900000000001976a914418327e3f3dda4cf5b9089325a4b95abdfa0334088ac722c0c00000000001976a914ba35042cfe9fc66fd35ac2224eebdafd1028ad2788acdc4ace020000000017a91474d691da1574e6b3c192ecfb52cc8984ee7b6c56870000000001000000");
            byte[] s256 = Tools.Hash256(modified_tx);
            BigInteger z = Tools.BigIntegerFromBytes(s256, "big");

            BigInteger want = Tools.MakeBigInteger("0e71bfa115715d6fd33796948126f40a8cdd39f187e4afb03896795189fe1423c");
            Console.Out.WriteLine(z.ToString());
            AssertTrue(z.Equals(want));
        }

        public static void test_chapter_8_p_160_2()
        {
            byte[] modified_tx = Tools.HexStringToBytes("0100000001868278ed6ddfb6c1ed3ad5f8181eb0c7a385aa0836f01d5e4789e6bd304d87221a000000475221022626e955ea6ea6d98850c994f9107b036b1334f18ca8830bfff1295d21cfdb702103b287eaf122eea69030a0e9feed096bed8045c8b98bec453e1ffac7fbdbd4bb7152aeffffffff04d3b11400000000001976a914904a49878c0adfc3aa05de7afad2cc15f483a56a88ac7f400900000000001976a914418327e3f3dda4cf5b9089325a4b95abdfa0334088ac722c0c00000000001976a914ba35042cfe9fc66fd35ac2224eebdafd1028ad2788acdc4ace020000000017a91474d691da1574e6b3c192ecfb52cc8984ee7b6c56870000000001000000");
            byte[] s256 = Tools.Hash256(modified_tx);
            BigInteger z = Tools.BigIntegerFromBytes(s256, "big");
            BigInteger want = Tools.MakeBigInteger("0e71bfa115715d6fd33796948126f40a8cdd39f187e4afb03896795189fe1423c");
            AssertTrue(z.Equals(want));

            byte[] sec = Tools.HexStringToBytes("022626e955ea6ea6d98850c994f9107b036b1334f18ca8830bfff1295d21cfdb70");
            byte[] der = Tools.HexStringToBytes("3045022100dc92655fe37036f47756db8102e0d7d5e28b3beb83a8fef4f5dc0559bddfb94e02205a36d4e4e6c7fcd16658c50783e00c341609977aed3ad00937bf4ee942a89937");
            S256Point point = S256Point.Parse(sec);
            Signature sig = Signature.Parse(der);
            bool success = point.Verify(z, sig);

            AssertTrue(success);
        }

        public static void test_chapter_8_ex_4()
        {
            string hex_tx = "0100000001868278ed6ddfb6c1ed3ad5f8181eb0c7a385aa0836f01d5e4789e6bd304d87221a000000db00483045022100dc92655fe37036f47756db8102e0d7d5e28b3beb83a8fef4f5dc0559bddfb94e02205a36d4e4e6c7fcd16658c50783e00c341609977aed3ad00937bf4ee942a8993701483045022100da6bee3c93766232079a01639d07fa869598749729ae323eab8eef53577d611b02207bef15429dcadce2121ea07f233115c6f09034c0be68db99980b9a6c5e75402201475221022626e955ea6ea6d98850c994f9107b036b1334f18ca8830bfff1295d21cfdb702103b287eaf122eea69030a0e9feed096bed8045c8b98bec453e1ffac7fbdbd4bb7152aeffffffff04d3b11400000000001976a914904a49878c0adfc3aa05de7afad2cc15f483a56a88ac7f400900000000001976a914418327e3f3dda4cf5b9089325a4b95abdfa0334088ac722c0c00000000001976a914ba35042cfe9fc66fd35ac2224eebdafd1028ad2788acdc4ace020000000017a91474d691da1574e6b3c192ecfb52cc8984ee7b6c568700000000";
            string hex_sec = "03b287eaf122eea69030a0e9feed096bed8045c8b98bec453e1ffac7fbdbd4bb71";
            string hex_der = "3045022100da6bee3c93766232079a01639d07fa869598749729ae323eab8eef53577d611b02207bef15429dcadce2121ea07f233115c6f09034c0be68db99980b9a6c5e754022";
            string hex_redeem_script = "475221022626e955ea6ea6d98850c994f9107b036b1334f18ca8830bfff1295d21cfdb702103b287eaf122eea69030a0e9feed096bed8045c8b98bec453e1ffac7fbdbd4bb7152ae";
            
            byte[] sec = Tools.HexStringToBytes(hex_sec);
            byte[] der = Tools.HexStringToBytes(hex_der);
            byte[] bRedeemScript = Tools.HexStringToBytes(hex_redeem_script);
            Script redeem_script = Script.Parse(bRedeemScript);

            Tx tx = Tx.Parse(hex_tx);
            S256Point point = S256Point.Parse(sec);
            Signature sig = Signature.Parse(der);

            byte[] bModifiedTransaction = Tools.ToBytes(tx._version, 4, "little");
            byte[] bLen = Tools.EncodeVarIntToBytes(tx._txIns.Count);

            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bLen);

            //
            // inputs: there is only one input
            //
            TxIn txIn = tx._txIns[0];

            TxIn txInModified = new TxIn(txIn._prev_tx, txIn._prev_index, redeem_script, txIn._sequence);
            byte[] bTxInModified = txInModified.serialize();
            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bTxInModified);

            //
            // outputs: unchanged
            // 
            bLen = Tools.EncodeVarIntToBytes(tx._txOuts.Count);
            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bLen);
            for (int i = 0; i < tx._txOuts.Count; i++)
            {
                byte[] bTxOut = tx._txOuts[i].serialize();
                bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bTxOut);
            }

            byte[] bytes = Tools.ToBytes(tx._locktime, 4, "little");
            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bytes);

            bytes = Tools.ToBytes(Signature.SIGHASH_ALL, 4, "little");
            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bytes);

            bytes = Tools.Hash256(bModifiedTransaction);
            BigInteger z = Tools.BigIntegerFromBytes(bytes, "big");
            BigInteger z_want = Tools.MakeBigIntegerBase10("104533698797560496821845132535319230899254894228014507304915351202849162871356");
            bool success = point.Verify(z, sig);

            AssertTrue(success);
        }

        public static void test_chapter_8_ex_5()
        {
            Tx tx = TxFetcher.Fetch("46df1a9484d0a81d03ce0ee543ab6e1a23ed06175c104a178268fad381216c2b");

            bool success = tx.Verify();

            AssertTrue(success);
        }

        public static void test_chapter_9_ex_1_is_coinbase()
        {
            string raw_tx = "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff5e03d71b07254d696e656420627920416e74506f6f6c20626a31312f4542312f4144362f43205914293101fabe6d6d678e2c8c34afc36896e7d9402824ed38e856676ee94bfdb0c6c4bcd8b2e5666a0400000000000000c7270000a5e00e00ffffffff01faf20b58000000001976a914338c84849423992471bffb1a54a8d9b1d69dc28a88ac00000000";
            Tx tx = Tx.Parse(raw_tx);
            bool success = tx.IsCoinbase();

            AssertTrue(success);
        }

        public static void test_chapter_9_165()
        {
            byte[] raw = Tools.HexStringToBytes("4d04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73");
            Script script = Script.Parse(raw);

            OpItem item = script._cmds[2];

            string text = Encoding.UTF8.GetString(item._element);
            ConsoleOutWriteLine(text);

            string want = "The Times 03/Jan/2009 Chancellor on brink of second bailout for banks";

            AssertTrue(want.Equals(text));
        }

        public static void test_chapter_9_166()
        {
            byte[] raw = Tools.HexStringToBytes("5e03d71b07254d696e656420627920416e74506f6f6c20626a31312f4542312f4144362f43205914293101fabe6d6d678e2c8c34afc36896e7d9402824ed38e856676ee94bfdb0c6c4bcd8b2e5666a0400000000000000c7270000a5e00e00");
            Script script = Script.Parse(raw);
            OpItem item = script._cmds[0];
            int height = (int)Op.DecodeNum(item._element);
            ConsoleOutWriteLine("coinbase height=" + height);

            AssertTrue(height == 465879);
        }

        public static void test_chapter_9_ex_2_coinbase_height()
        {
            string raw_tx = "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff5e03d71b07254d696e656420627920416e74506f6f6c20626a31312f4542312f4144362f43205914293101fabe6d6d678e2c8c34afc36896e7d9402824ed38e856676ee94bfdb0c6c4bcd8b2e5666a0400000000000000c7270000a5e00e00ffffffff01faf20b58000000001976a914338c84849423992471bffb1a54a8d9b1d69dc28a88ac00000000";
            Tx tx = Tx.Parse(raw_tx);
            int height = tx.CoinbaseHeight();
            ConsoleOutWriteLine("coinbase height=" + height);
            AssertEqual(height, 465879);
            
            raw_tx = "0100000001813f79011acb80925dfe69b3def355fe914bd1d96a3f5f71bf8303c6a989c7d1000000006b483045022100ed81ff192e75a3fd2304004dcadb746fa5e24c5031ccfcf21320b0277457c98f02207a986d955c6e0cb35d446a89d3f56100f4d7f67801c31967743a9c8e10615bed01210349fc4e631e3624a545de3f89f5d8684c7b8138bd94bdd531d2e213bf016b278afeffffff02a135ef01000000001976a914bc3b654dca7e56b04dca18f2566cdaf02e8d9ada88ac99c39800000000001976a9141c4bc762dd5423e332166702cb75f40df79fea1288ac19430600";
            tx = Tx.Parse(raw_tx);
            height = tx.CoinbaseHeight();
            ConsoleOutWriteLine("coinbase height=" + height);
            AssertTrue(tx.CoinbaseHeight() == 0);
        }
    }
}
