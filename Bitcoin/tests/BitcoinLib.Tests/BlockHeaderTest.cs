using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace BitcoinLib.Test
{
    public class BlockHeaderTest : UnitTest
    {
        public static void test_chapter_9_p167_blockid()
        {
            byte[] block_hash = Tools.Hash256(Tools.HexStringToBytes("020000208ec39428b17323fa0ddec8e887b4a7c53b8c0a0a220cfd0000000000000000005b0750fce0a889502d40508d39576821155e9c9e3f5c3157f961db38fd8b25be1e77a759e93c0118a4ffd71d"));
            Tools.Reverse(block_hash);

            string strBlockHash = Tools.BytesToHexString(block_hash);
            ConsoleOutWriteLine("block id = " + strBlockHash);

            string want = "0000000000000000007e9e4c586439b0cdbe13b1370bdd9435d76a644d047523";

            AssertTrue(want.Equals(strBlockHash));
        }
        public static void test_block_header_parse()
        {
            string raw = "020000208ec39428b17323fa0ddec8e887b4a7c53b8c0a0a220cfd0000000000000000005b0750fce0a889502d40508d39576821155e9c9e3f5c3157f961db38fd8b25be1e77a759e93c0118a4ffd71d";
            byte[] raw_bytes = Tools.HexStringToBytes(raw);

            BlockHeader block = BlockHeader.Parse(raw_bytes);

            bool success = block._version == 0x20000002;
            AssertTrue(success);

            byte[] want = Tools.HexStringToBytes("000000000000000000fd0c220a0a8c3bc5a7b487e8c8de0dfa2373b12894c38e");
            AssertEqual(block._prevBlockHash, want);

            want = Tools.HexStringToBytes("be258bfd38db61f957315c3f9e9c5e15216857398d50402d5089a8e0fc50075b");
            AssertEqual(block._merkleRoot, want);

            AssertEqual(block._timestamp, 0x59a7771e);
            AssertEqual(block._bits, 0x18013ce9);
            AssertEqual(block._nonce, 0x1dd7ffa4);
        }

        public static void test_block_header_serialize()
        {
            byte[] block_raw = Tools.HexStringToBytes("020000208ec39428b17323fa0ddec8e887b4a7c53b8c0a0a220cfd0000000000000000005b0750fce0a889502d40508d39576821155e9c9e3f5c3157f961db38fd8b25be1e77a759e93c0118a4ffd71d");
            BlockHeader block = BlockHeader.Parse(block_raw);
            byte[] block_serialized = block.serialize();
            string strBlock_serialized = Tools.BytesToHexString(block_serialized);
            AssertEqual(block_raw, block_serialized);
        }

        public static void test_block_header_hash()
        {
            byte[] block_raw = Tools.HexStringToBytes("020000208ec39428b17323fa0ddec8e887b4a7c53b8c0a0a220cfd0000000000000000005b0750fce0a889502d40508d39576821155e9c9e3f5c3157f961db38fd8b25be1e77a759e93c0118a4ffd71d");
            BlockHeader block = BlockHeader.Parse(block_raw);

            byte[] want = Tools.HexStringToBytes("0000000000000000007e9e4c586439b0cdbe13b1370bdd9435d76a644d047523");
            byte[] hash = block.Hash();

            string strHash = Tools.BytesToHexString(hash);
            ConsoleOutWriteLine("hash = " + strHash);
            AssertEqual(want, hash);
        }

        public static void test_block_header_p168_bip()
        {
            byte[] block_raw = Tools.HexStringToBytes("020000208ec39428b17323fa0ddec8e887b4a7c53b8c0a0a220cfd0000000000000000005b0750fce0a889502d40508d39576821155e9c9e3f5c3157f961db38fd8b25be1e77a759e93c0118a4ffd71d");
            BlockHeader block = BlockHeader.Parse(block_raw);

            byte b = (byte) (block._version >> 29);
            ConsoleOutWriteLine("block: BIP9: " + (b == 0x1));
            bool success = block.Bip9();
            AssertTrue(success);

            b = (byte) ((block._version >> 4) & 1);
            ConsoleOutWriteLine("block: BIP91: " + (b == 0x1));
            success = block.Bip91();
            AssertFalse(success);

            b = (byte)((block._version >> 1) & 1);
            ConsoleOutWriteLine("block: BIP141: " + (b == 0x1));
            success = block.Bip141();
            AssertTrue(success);
        }

        public static void test_block_header_bits_to_target()
        {
            byte[] block_raw = Tools.HexStringToBytes("020000208ec39428b17323fa0ddec8e887b4a7c53b8c0a0a220cfd0000000000000000005b0750fce0a889502d40508d39576821155e9c9e3f5c3157f961db38fd8b25be1e77a759e93c0118a4ffd71d");
            BlockHeader block = BlockHeader.Parse(block_raw);
            string strBits = block._bits.ToString("x"); // 18013ce9

            byte[] bHash = block.Hash();
            BigInteger hash = Tools.BigIntegerFromBytes(bHash, "big");
            string strHash = Tools.BytesToHexString(bHash);

            BigInteger target = block.BitsToTarget();
            string strTarget = target.ToString("x64");

            ConsoleOutWriteLine("bits       = " + strBits);
            ConsoleOutWriteLine("block hash = " + strHash);
            ConsoleOutWriteLine("target     = " + strTarget);
            ConsoleOutWriteLine("hash < target: " + (hash < target));

            BigInteger want = Tools.MakeBigInteger("13ce9000000000000000000000000000000000000000000");
            AssertEqual(target, want);
        }

        public static void test_block_header_target_to_bits()
        {
            BigInteger target = Tools.MakeBigInteger("7615000000000000000000000000000000000000000000");

            UInt32 bits = BlockHeader.TargetToBits(target);
            //UInt32 want = Convert.ToUInt32("00157617", 16);
            UInt32 want = Convert.ToUInt32("17761500", 16);

            AssertEqual(bits, want);
        }

        public static void test_block_header_p173_difficulty()
        {
            byte[] block_raw = Tools.HexStringToBytes("020000208ec39428b17323fa0ddec8e887b4a7c53b8c0a0a220cfd0000000000000000005b0750fce0a889502d40508d39576821155e9c9e3f5c3157f961db38fd8b25be1e77a759e93c0118a4ffd71d");
            BlockHeader block = BlockHeader.Parse(block_raw);

            double fDifficulty = block.DifficultyAsDouble();
            double fWant = 888171856257.32056;
            BigInteger biDifficulty = block.DifficultyAsBigInteger();
            BigInteger biWant = new BigInteger(888171856257);

            AssertEqual(biDifficulty, biWant);
            AssertEqual(fDifficulty, fWant);
        }

        public static void test_block_header_checkpow()
        {
            byte[] block_raw = Tools.HexStringToBytes("020000208ec39428b17323fa0ddec8e887b4a7c53b8c0a0a220cfd0000000000000000005b0750fce0a889502d40508d39576821155e9c9e3f5c3157f961db38fd8b25be1e77a759e93c0118a4ffd71d");
            BlockHeader block = BlockHeader.Parse(block_raw);

            bool success = block.CheckPow();
            AssertTrue(success);

            block_raw = Tools.HexStringToBytes("04000000fbedbbf0cfdaf278c094f187f2eb987c86a199da22bbb20400000000000000007b7697b29129648fa08b4bcd13c9d5e60abb973a1efac9c8d573c71c807c56c3d6213557faa80518c3737ec1");
            block = BlockHeader.Parse(block_raw);
            success = block.CheckPow();
            AssertTrue(success);

            block_raw = Tools.HexStringToBytes("04000000fbedbbf0cfdaf278c094f187f2eb987c86a199da22bbb20400000000000000007b7697b29129648fa08b4bcd13c9d5e60abb973a1efac9c8d573c71c807c56c3d6213557faa80518c3737ec0");
            block = BlockHeader.Parse(block_raw);
            success = block.CheckPow();
            AssertFalse(success);
        }

        public static void test_block_header_calculate_new_bits()
        {

            //UInt32 prev_bits = Convert.ToUInt32("54d80118", 16);
            UInt32 prev_bits = Convert.ToUInt32("1801d854", 16);
            int time_differential = 302400;

            UInt32 newBits = BlockHeader.CalculateNewBits(prev_bits, time_differential);
            UInt32 want = Convert.ToUInt32("17761500", 16);
            //UInt32 want = Convert.ToUInt32("00157617", 16);

            AssertEqual(newBits, want);
        }

        public static void test_block_header_p174_new_target()
        {
            //
            // the calculation in the book MUST be wrong. This code does the same incorrect calculation
            // to check that the code as such works.
            //
            string strBlock1_hex = "00000020fdf740b0e49cf75bb3d5168fb3586f7613dcc5cd89675b0100000000000000002e37b144c0baced07eb7e7b64da916cd3121f2427005551aeb0ec6a6402ac7d7f0e4235954d801187f5da9f5";
            string strBlock2_hex = "000000201ecd89664fd205a37566e694269ed76e425803003628ab010000000000000000bfcade29d080d9aae8fd461254b041805ae442749f2a40100440fc0e3d5868e55019345954d80118a1721b2e";

            byte[] bBlock1 = Tools.HexStringToBytes(strBlock1_hex);
            byte[] bBlock2 = Tools.HexStringToBytes(strBlock2_hex);

            BlockHeader block1 = BlockHeader.Parse(bBlock1);
            BlockHeader block2 = BlockHeader.Parse(bBlock2);

            // in the book, this is calculated, which restuls in a negative number.
            Int32 time_differential = (Int32) block1._timestamp - (Int32) block2._timestamp;
            if (time_differential > BlockHeader.TWO_WEEKS * 4)
            {
                time_differential = BlockHeader.TWO_WEEKS * 4;
            }
            if (time_differential < BlockHeader.TWO_WEEKS / 4)
            {
                time_differential = BlockHeader.TWO_WEEKS / 4;
            }
            BigInteger target  = block1.Target();
            string strTarget = target.ToString("x64");

            BigInteger new_target = block1.Target() * time_differential / BlockHeader.TWO_WEEKS;
            string strnew_target = new_target.ToString("x64");
            ConsoleOutWriteLine("new_target=" + strnew_target);
            // in the book, the result is
            // 0000000000000000007615000000000000000000000000000000000000000000
            // and we get the same here
            BigInteger want = Tools.MakeBigInteger("07615000000000000000000000000000000000000000000");
            string strWant = want.ToString("x64");
            AssertTrue(strnew_target.Equals(strWant));
        }

        /// <summary>
        /// Using real bitcoin blockchain data, calculate the next difficulty from blocks 403200 and 405215 and check that that value is in block 405216 
        /// </summary>
        public static void test_chapter_9_new_bits()
        {
            //
            // all these blocks are from the real bitcoin blockchain.
            // you can calculate the next difficulty and compare it to the value which exists in the blockchain
            //
            string strBlock_403200_hex = "04000000473ed7b7ef2fce828c318fd5e5868344a5356c9e93b6040400000000000000004409cae5b7b2f8f18ea55f558c9bfa7c5f4778a1a53172a48fc57e172d0ed3d264c5eb56c3a40618af9bc1c7";
            string strBlock_405215_hex = "040000005f5560a04006b8a4a6fc85c7a7816c36f89a6da5b03aaa000000000000000000e74d25fadc646f6e0e2b781da6d172170ed1213afb9ac6f05c5a5dc482830e520914fe56c3a40618a6a1ffc0";
            string strBlock_405216_hex = "04000000f7ef2881b8a0cb415ba81e889c79bc5f1b098167c95646030000000000000000a48869fe8d6777821fa85525139cb77d12c440c16182c637e943dfea7d937daa7b16fe56f49606185628272d";

            byte[] bBlock_403200 = Tools.HexStringToBytes(strBlock_403200_hex);
            byte[] bBlock_405215 = Tools.HexStringToBytes(strBlock_405215_hex);
            byte[] bBlock_405216 = Tools.HexStringToBytes(strBlock_405216_hex);

            // block_403200.timestamp = 1458292068 = 0x56ebc564
            // block_403200.bits = 0x1806a4c3
            BlockHeader block_403200 = BlockHeader.Parse(bBlock_403200);

            // block_405215.timestamp = 1459491849 = 0x56fe1409
            // block_405215.bits = 0x1806a4c3
            BlockHeader block_405215 = BlockHeader.Parse(bBlock_405215);

            // block_405216.bits = 0x180696f4
            BlockHeader block_405216 = BlockHeader.Parse(bBlock_405216);

            // time_differential = 1.199.781
            UInt32 time_differential = block_405215._timestamp - block_403200._timestamp;

            if (time_differential > BlockHeader.TWO_WEEKS * 4)
            {
                time_differential = BlockHeader.TWO_WEEKS * 4;
            }
            if (time_differential < BlockHeader.TWO_WEEKS / 4)
            {
                time_differential = BlockHeader.TWO_WEEKS / 4;
            }
            // time_differential does not change: 1.199.781

            // oldTarget  = "000000000000000006a4c3000000000000000000000000000000000000000000"
            BigInteger oldTarget = block_405215.Target();
            string strOldTarget = oldTarget.ToString("x64");

            // new_target = "00000000000000000696f4a7b94b94b94b94b94b94b94b94b94b94b94b94b94b"
            BigInteger new_target = oldTarget  * time_differential / BlockHeader.TWO_WEEKS;
            string strNewTarget = new_target.ToString("x64");

            // new_Bits = 0x180696f4
            UInt32 new_bits = BlockHeader.TargetToBits(new_target);
            ConsoleOutWriteLine("new_bits:" + new_bits.ToString("x8"));

            AssertEqual(block_405216._bits, new_bits);

            // TODO: herausfinde, wie das gemacht wird mit den blocks
            // 403200 → 405215: die kommen von ChatGPT
            // input: block number, output: block hash: https://blockchair.com/bitcoin/block
            // input: block hash, output: block header: https://blockstream.info/api/block/0000000000000000012a85f9010f0e2cf696408300918f4b5df8ddd8809102a2/header
            //
            //
            // from 403200 to 405215
            //      block    hash                                                               header
            //      403200   000000000000000000c4272a5c68b4f55e5af734e88ceab09abf73e9ac3b6d01   04000000473ed7b7ef2fce828c318fd5e5868344a5356c9e93b6040400000000000000004409cae5b7b2f8f18ea55f558c9bfa7c5f4778a1a53172a48fc57e172d0ed3d264c5eb56c3a40618af9bc1c7
            //      405215   0000000000000000034656c96781091b5fbc799c881ea85b41cba0b88128eff7   040000005f5560a04006b8a4a6fc85c7a7816c36f89a6da5b03aaa000000000000000000e74d25fadc646f6e0e2b781da6d172170ed1213afb9ac6f05c5a5dc482830e520914fe56c3a40618a6a1ffc0
            //      405216   000000000000000006969473ee3a2126d9a953ad00eee6443b4d2d2e0881fc07   04000000f7ef2881b8a0cb415ba81e889c79bc5f1b098167c95646030000000000000000a48869fe8d6777821fa85525139cb77d12c440c16182c637e943dfea7d937daa7b16fe56f49606185628272d
            //
            // from 471744 to 473759
            //      471744   0000000000000000012a85f9010f0e2cf696408300918f4b5df8ddd8809102a2   000000203471101bbda3fe307664b3283a9ef0e97d9a38a7eacd8800000000000000000010c8aba8479bbaa5e0848152fd3c2289ca50e1c3e58c9a4faaafbdf5803c5448ddb845597e8b0118e43a81d3
            //      473759   000000000000000001389446206ebcd378c32cd00b4920a8a1ba7b540ca7d699   02000020f1472d9db4b563c35f97c428ac903f23b7fc055d1cfc26000000000000000000b3f449fcbe1bc4cfbcb8283a0d2c037f961a3fdf2b8bedc144973735eea707e1264258597e8b0118e5f00474
            //      473760   000000000000000000802ba879f1b7a638dcea6ff0ceb614d91afc8683ac0502   0200002099d6a70c547bbaa1a820490bd02cc378d3bc6e20469438010000000000000000b66a0b024cfdf07d0dd97e18ad6ef1a411b0452129d3bfe3e6ebae55defec4dd95425859308d0118bc260a08
            //
        }

        /// <summary>
        /// Using real bitcoin blockchain data, calculate the next difficulty from blocks 471744 and 473759 and check that that value is in block 473760 
        /// </summary>
        public static void test_chapter_9_ex_12()
        {
            //                                                                                                                                                                    timestamp           nonce
            //                                                                                                                                                                     xxxxxxxx          zzzzzzzz
            //                                                                                                                                                                                bits
            //                                                                                                                                                                              yyyyyyyy
            string strBlock_471744_hex = "000000203471101bbda3fe307664b3283a9ef0e97d9a38a7eacd8800000000000000000010c8aba8479bbaa5e0848152fd3c2289ca50e1c3e58c9a4faaafbdf5803c5448 ddb84559 7e8b0118 e43a81d3";
            string strBlock_473759_hex = "02000020f1472d9db4b563c35f97c428ac903f23b7fc055d1cfc26000000000000000000b3f449fcbe1bc4cfbcb8283a0d2c037f961a3fdf2b8bedc144973735eea707e1 26425859 7e8b0118 e5f00474";
            string strBlock_473760_hex = "0200002099d6a70c547bbaa1a820490bd02cc378d3bc6e20469438010000000000000000b66a0b024cfdf07d0dd97e18ad6ef1a411b0452129d3bfe3e6ebae55defec4dd 95425859 308d0118 bc260a08";

            byte[] bBlock_471744 = Tools.HexStringToBytes(strBlock_471744_hex.Replace(" ", ""));
            byte[] bBlock_473759 = Tools.HexStringToBytes(strBlock_473759_hex.Replace(" ", ""));
            byte[] bBlock_473760 = Tools.HexStringToBytes(strBlock_473760_hex.Replace(" ", ""));

            // block_471744.timestamp = 0x5945b8dd = 1.497.741.533
            // block_471744.bits = 0x18018b7e
            BlockHeader block_471744 = BlockHeader.Parse(bBlock_471744);

            // block_473759.timestamp = 0x59584226 = 1.498.956.326
            // block_473759.bits = 0x18018b7e
            BlockHeader block_473759 = BlockHeader.Parse(bBlock_473759);

            // block_473760.bits = 0x18018d30
            BlockHeader block_473760 = BlockHeader.Parse(bBlock_473760);

            // time_differential = 1.214.793
            UInt32 time_differential = block_473759._timestamp - block_471744._timestamp;

            if (time_differential > BlockHeader.TWO_WEEKS * 4)
            {
                time_differential = BlockHeader.TWO_WEEKS * 4;
            }
            if (time_differential < BlockHeader.TWO_WEEKS / 4)
            {
                time_differential = BlockHeader.TWO_WEEKS / 4;
            }
            // time_differential does not change: 1.199.781

            // oldTarget  = "18b7e000000000000000000000000000000000000000000""
            BigInteger oldTarget = block_473759.Target();
            string strOldTarget = oldTarget.ToString("x64");

            // new_target = "18d30aa2cdc67600f9a9342cdc67600f9a9342cdc67600f""
            BigInteger newTarget = oldTarget * time_differential / BlockHeader.TWO_WEEKS;
            string strNewTarget = newTarget.ToString("x64");

            // new_Bits = 0x18018d30
            UInt32 new_bits = BlockHeader.TargetToBits(newTarget);
            ConsoleOutWriteLine("new bits:" + new_bits.ToString("x8"));

            AssertEqual(block_473760._bits, new_bits);

            // input: block number, output: block hash: https://blockchair.com/bitcoin/block
            // input: block hash, output: block header: https://blockstream.info/api/block/0000000000000000012a85f9010f0e2cf696408300918f4b5df8ddd8809102a2/header
            //
            //      block    hash                                                               header
            // from 471744 to 473759
            //      471744   0000000000000000012a85f9010f0e2cf696408300918f4b5df8ddd8809102a2   000000203471101bbda3fe307664b3283a9ef0e97d9a38a7eacd8800000000000000000010c8aba8479bbaa5e0848152fd3c2289ca50e1c3e58c9a4faaafbdf5803c5448ddb845597e8b0118e43a81d3
            //      473759   000000000000000001389446206ebcd378c32cd00b4920a8a1ba7b540ca7d699   02000020f1472d9db4b563c35f97c428ac903f23b7fc055d1cfc26000000000000000000b3f449fcbe1bc4cfbcb8283a0d2c037f961a3fdf2b8bedc144973735eea707e1264258597e8b0118e5f00474
            //      473760   000000000000000000802ba879f1b7a638dcea6ff0ceb614d91afc8683ac0502   0200002099d6a70c547bbaa1a820490bd02cc378d3bc6e20469438010000000000000000b66a0b024cfdf07d0dd97e18ad6ef1a411b0452129d3bfe3e6ebae55defec4dd95425859308d0118bc260a08
            //
        }

        public static void test_validate_merkle_root()
        {
            string[] strHashes = [
                 "f54cb69e5dc1bd38ee6901e4ec2007a5030e14bdd60afb4d2f3428c88eea17c1",
                "c57c2d678da0a7ee8cfa058f1cf49bfcb00ae21eda966640e312b464414731c1",
                "b027077c94668a84a5d0e72ac0020bae3838cb7f9ee3fa4e81d1eecf6eda91f3",
                "8131a1b8ec3a815b4800b43dff6c6963c75193c4190ec946b93245a9928a233d",
                "ae7d63ffcb3ae2bc0681eca0df10dda3ca36dedb9dbf49e33c5fbe33262f0910",
                "61a14b1bbdcdda8a22e61036839e8b110913832efd4b086948a6a64fd5b3377d",
                "fc7051c8b536ac87344c5497595d5d2ffdaba471c73fae15fe9228547ea71881",
                "77386a46e26f69b3cd435aa4faac932027f58d0b7252e62fb6c9c2489887f6df",
                "59cbc055ccd26a2c4c4df2770382c7fea135c56d9e75d3f758ac465f74c025b8",
                "7c2bf5687f19785a61be9f46e031ba041c7f93e2b7e9212799d84ba052395195",
                "08598eebd94c18b0d59ac921e9ba99e2b8ab7d9fccde7d44f2bd4d5e2e726d2e",
                "f0bb99ef46b029dd6f714e4b12a7d796258c48fee57324ebdc0bbc4700753ab1"
            ];
            byte[][] hashes = strHashes.Select(s => Tools.HexStringToBytes(s)).ToArray();

            byte[] raw = Tools.HexStringToBytes("00000020fcb19f7895db08cadc9573e7915e3919fb76d59868a51d995201000000000000acbcab8bcc1af95d8d563b77d24c3d19b18f1486383d75a5085c4e86c86beed691cfa85916ca061a00000000");

            BlockHeader header = BlockHeader.Parse(raw);
            header._tx_hashes = hashes;
            bool success = header.ValidateMerkleRoot();
            AssertTrue(success);

        }
    }
}

