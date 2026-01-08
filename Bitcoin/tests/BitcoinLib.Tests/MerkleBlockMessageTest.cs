using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib.Network;

namespace BitcoinLib.Test
{
    public class MerkleBlockMessageTest : UnitTest
    {
        public static void test_parse()
        {
            string strRaw = "00000020df3b053dc46f162a9b00c7f0d5124e2676d47bbe7c5d0793a500000000000000ef445fef2ed495c275892206ca533e7411907971013ab83e3b47bd0d692d14d4dc7c835b67d8001ac157e670bf0d00000aba412a0d1480e370173072c9562becffe87aa661c1e4a6dbc305d38ec5dc088a7cf92e6458aca7b32edae818f9c2c98c37e06bf72ae0ce80649a38655ee1e27d34d9421d940b16732f24b94023e9d572a7f9ab8023434a4feb532d2adfc8c2c2158785d1bd04eb99df2e86c54bc13e139862897217400def5d72c280222c4cbaee7261831e1550dbb8fa82853e9fe506fc5fda3f7b919d8fe74b6282f92763cef8e625f977af7c8619c32a369b832bc2d051ecd9c73c51e76370ceabd4f25097c256597fa898d404ed53425de608ac6bfe426f6e2bb457f1c554866eb69dcb8d6bf6f880e9a59b3cd053e6c7060eeacaacf4dac6697dac20e4bd3f38a2ea2543d1ab7953e3430790a9f81e1c67f5b58c825acf46bd02848384eebe9af917274cdfbb1a28a5d58a23a17977def0de10d644258d9c54f886d47d293a411cb6226103b55635";
            byte[] bRaw = Tools.HexStringToBytes(strRaw);
            MerkleBlock message = MerkleBlock.Parse(bRaw);

            UInt32 version = 0x20000000;
            AssertEqual(message._blockHeader._version, version);

            string strMerkleRoot = "ef445fef2ed495c275892206ca533e7411907971013ab83e3b47bd0d692d14d4";
            byte[] bMerkleRoot = Tools.HexStringToBytes(strMerkleRoot);
            Tools.Reverse(bMerkleRoot);
            AssertEqual(message._blockHeader._merkleRoot, bMerkleRoot);


            string strPrevBlock = "df3b053dc46f162a9b00c7f0d5124e2676d47bbe7c5d0793a500000000000000";
            byte[] bPrevBlock = Tools.HexStringToBytes(strPrevBlock);
            Tools.Reverse(bPrevBlock);
            AssertEqual(message._blockHeader._prevBlockHash, bPrevBlock);

            AssertEqual(message._blockHeader._timestamp, 0x5b837cdc); // bytes: dc7c835b

            AssertEqual(message._blockHeader._bits, 0x1a00d867); // 67d8001a

            AssertEqual(message._blockHeader._nonce, 0x70e657c1); // c157e670

            AssertEqual(message._totalTransactions, 0x00000dbf); // bytes: bf0d0000

            string[] strHashes = [
                "ba412a0d1480e370173072c9562becffe87aa661c1e4a6dbc305d38ec5dc088a",
                "7cf92e6458aca7b32edae818f9c2c98c37e06bf72ae0ce80649a38655ee1e27d",
                "34d9421d940b16732f24b94023e9d572a7f9ab8023434a4feb532d2adfc8c2c2",
                "158785d1bd04eb99df2e86c54bc13e139862897217400def5d72c280222c4cba",
                "ee7261831e1550dbb8fa82853e9fe506fc5fda3f7b919d8fe74b6282f92763ce",
                "f8e625f977af7c8619c32a369b832bc2d051ecd9c73c51e76370ceabd4f25097",
                "c256597fa898d404ed53425de608ac6bfe426f6e2bb457f1c554866eb69dcb8d",
                "6bf6f880e9a59b3cd053e6c7060eeacaacf4dac6697dac20e4bd3f38a2ea2543",
                "d1ab7953e3430790a9f81e1c67f5b58c825acf46bd02848384eebe9af917274c",
                "dfbb1a28a5d58a23a17977def0de10d644258d9c54f886d47d293a411cb62261",
            ];
            byte[][] hashes = strHashes.Select(s => Tools.HexStringToBytes(s).Reverse().ToArray()).ToArray();

            AssertEqual(message._hashes.Length, hashes.Length);
            AssertEqual(message._hashes, hashes);

            AssertEqual(message._flags, Tools.HexStringToBytes("b55635"));
        }

        public static void test_flags_and_bitfield()
        {
            byte[] bits = [0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0];
            byte[] flags = Tools.HexStringToBytes("4000600a080000010940");

            byte[] calculatedFlags = MerkleBlock.BitFieldToFlagBytes(bits);
            AssertEqual(calculatedFlags, flags);

            byte[] calculatedBits = MerkleBlock.FlagsToBitField(flags);
            AssertEqual(calculatedBits, bits);
        }

        public static void test_is_Valid()
        {
            string strData = "00000020 df3b053dc46f162a9b00c7f0d5124e2676d47bbe7c5d0793a500000000000000 ef445fef2ed495c275892206ca533e7411907971013ab83e3b47bd0d692d14d4dc7c835b 67d8001a c157e670 bf0d00000a ba412a0d1480e370173072c9562becffe87aa661c1e4a6dbc305d38ec5dc088a7cf92e6458aca7b32edae818f9c2c98c37e06bf72ae0ce80649a38655ee1e27d34d9421d940b16732f24b94023e9d572a7f9ab8023434a4feb532d2adfc8c2c2158785d1bd04eb99df2e86c54bc13e139862897217400def5d72c280222c4cbaee7261831e1550dbb8fa82853e9fe506fc5fda3f7b919d8fe74b6282f92763cef8e625f977af7c8619c32a369b832bc2d051ecd9c73c51e76370ceabd4f25097c256597fa898d404ed53425de608ac6bfe426f6e2bb457f1c554866eb69dcb8d6bf6f880e9a59b3cd053e6c7060eeacaacf4dac6697dac20e4bd3f38a2ea2543d1ab7953e3430790a9f81e1c67f5b58c825acf46bd02848384eebe9af917274cdfbb1a28a5d58a23a17977def0de10d644258d9c54f886d47d293a411cb622610 3b55635";
            strData = strData.Replace(" ", "");

            byte[] bData = Tools.HexStringToBytes(strData);

            MerkleBlock msg = MerkleBlock.Parse(bData);

            bool success = msg.IsValid();

            AssertTrue(success);
        }
    }
}
