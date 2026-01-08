using BitcoinLib.Test;
using System;

namespace BitcoinLib.Test
{
    public class BitcoinLibTest
    {
        public static void Various()
        {
            // keyword=modulo-normalization
            UnitTest.ConsoleOutWriteLine("modulo-normalization");
            Console.WriteLine("Modulo on negative numbers must yield a positive value, therefore we perform the modulo-normalization");
            Console.WriteLine("-3 % 5           = " + -3 % 5);
            Console.WriteLine("(-3 % 5 + 5) % 5 = " + (-3 % 5 + 5) % 5);
            Console.WriteLine("-32 % 5           = " + -32 % 5);
            Console.WriteLine("(-32 % 5 + 5) % 5 = " + (-32 % 5 + 5) % 5);
        }

        /// <summary>
        /// These tests should not output anything....
        /// They test the functionality
        /// </summary>
        public static void RunAllTests()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.SignatureTest", "test_der");
            Tools.CallStaticMethod("BitcoinLib.Test.SignatureTest", "test_chapter_7_p132");
            Tools.CallStaticMethod("BitcoinLib.Test.SignatureTest", "test_chapter_7_p134_mod_tx");
            Tools.CallStaticMethod("BitcoinLib.Test.SignatureTest", "test_chapter_7_p134_verify");

            Tools.CallStaticMethod("BitcoinLib.Test.OpTest", "test_encode_decode_num");
            Tools.CallStaticMethod("BitcoinLib.Test.OpTest", "test_op_checkmultisig");

            Tools.CallStaticMethod("BitcoinLib.Test.TxInTest", "test_get_url_content");
            Tools.CallStaticMethod("BitcoinLib.Test.TxInTest", "test_input_value");
            Tools.CallStaticMethod("BitcoinLib.Test.TxInTest", "test_input_pubkey");

            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_parse_version");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_parse_inputs");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_parse_outputs");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_parse_locktime");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_serialize");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_fee");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_sign_input");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_sig_hash");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2pkh");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2sh");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2wpkh");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2sh_p2wpkh");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2wsh");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_verify_p2sh_p2wsh");
            

            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "test_null0");
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "test_all");

            Tools.CallStaticMethod("BitcoinLib.Test.S256FieldTest", "test_plus");

            Tools.CallStaticMethod("BitcoinLib.Test.PointTest", "test_ne");
            Tools.CallStaticMethod("BitcoinLib.Test.PointTest", "test_on_curve");
            Tools.CallStaticMethod("BitcoinLib.Test.PointTest", "test_add0");
            Tools.CallStaticMethod("BitcoinLib.Test.PointTest", "test_add1");
            Tools.CallStaticMethod("BitcoinLib.Test.PointTest", "test_add2");

            Tools.CallStaticMethod("BitcoinLib.Test.EccTest", "test_add");
            Tools.CallStaticMethod("BitcoinLib.Test.EccTest", "test_rmul");
            Tools.CallStaticMethod("BitcoinLib.Test.EccTest", "test_on_curve");

            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_order");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_pubpoint");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_verify");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_sec");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_address");

            Tools.CallStaticMethod("BitcoinLib.Test.PrivateKeyTest", "test_sign");
            Tools.CallStaticMethod("BitcoinLib.Test.PrivateKeyTest", "test_sign_other");
            Tools.CallStaticMethod("BitcoinLib.Test.PrivateKeyTest", "test_wif");
            Tools.CallStaticMethod("BitcoinLib.Test.PrivateKeyTest", "test_public_key_hash160");

            Tools.CallStaticMethod("BitcoinLib.Test.ToolsTest", "test_join_array");
            Tools.CallStaticMethod("BitcoinLib.Test.ToolsTest", "test_to_byte");
            Tools.CallStaticMethod("BitcoinLib.Test.ToolsTest", "test_hash256");
            Tools.CallStaticMethod("BitcoinLib.Test.ToolsTest", "test_sha1_collision");
            Tools.CallStaticMethod("BitcoinLib.Test.ToolsTest", "test_from_bytes");
            Tools.CallStaticMethod("BitcoinLib.Test.ToolsTest", "test_from_lstrip");
            Tools.CallStaticMethod("BitcoinLib.Test.ToolsTest", "test_reverse");

            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test1_encode");
            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test2_decode");
            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test3_DecodeInvalidChar");
            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test4_EncodeBitcoinAddress");
            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test5_DecodeBitcoinAddress");
            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test6_DecodeBrokenBitcoinAddress");
            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test7_ExtractHash160");
            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test_p2pkh_address");
            Tools.CallStaticMethod("BitcoinLib.Test.Base58EncodingTests", "test_p2sh_address");

            Tools.CallStaticMethod("BitcoinLib.Test.ScriptTest", "test_parse");
            Tools.CallStaticMethod("BitcoinLib.Test.ScriptTest", "test_serialize");
            Tools.CallStaticMethod("BitcoinLib.Test.ScriptTest", "test_address");

            Tools.CallStaticMethod("BitcoinLib.Test.BIP39.BIP39Test", "test_bip39_mnemonic");

            Tools.CallStaticMethod("BitcoinLib.Test.Ripemd160Test", "test_ripemd160_bouncycastle");
            Tools.CallStaticMethod("BitcoinLib.Test.Ripemd160Test", "test_ripemd160_chatgpt");

            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_parse");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_serialize");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_hash");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_p168_bip");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_bits_to_target");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_target_to_bits");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_p173_difficulty");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_checkpow");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_calculate_new_bits");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_validate_merkle_root");

            Tools.CallStaticMethod("BitcoinLib.Test.NetworkEnvelopeTest", "test_parse");
            Tools.CallStaticMethod("BitcoinLib.Test.NetworkEnvelopeTest", "test_serialize");

            Tools.CallStaticMethod("BitcoinLib.Test.VersionMessageTest", "test_serialize");

            Tools.CallStaticMethod("BitcoinLib.Test.PingMessageTest", "test_serialize");

            Tools.CallStaticMethod("BitcoinLib.Test.PongMessageTest", "test_serialize");

            Tools.CallStaticMethod("BitcoinLib.Test.SimpleNodeTest", "test_handshake");
            Tools.CallStaticMethod("BitcoinLib.Test.SimpleNodeTest", "test_getheaders");
            Tools.CallStaticMethod("BitcoinLib.Test.SimpleNodeTest", "test_headers");
            Tools.CallStaticMethod("BitcoinLib.Test.SimpleNodeTest", "test_get_transaction_of_interest");

            Tools.CallStaticMethod("BitcoinLib.Test.GetHeadersTest", "test_serialize");

            Tools.CallStaticMethod("BitcoinLib.Test.HeadersTest", "test_parse");

            Tools.CallStaticMethod("BitcoinLib.Test.MerkleBlockTest", "test_merkle_parent");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleBlockTest", "test_merkle_parent_level");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleBlockTest", "test_merkle_root");

            Tools.CallStaticMethod("BitcoinLib.Test.MerkleTreeTest", "test_merkle_9");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleTreeTest", "test_merkle_16");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleTreeTest", "test_merkle_tree_p201");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleTreeTest", "test_merkle_tree_p203");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleTreeTest", "test_merkle_tree_p204");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleTreeTest", "test_merkle_tree_populate_1");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleTreeTest", "test_merkle_tree_populate_2");

            
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleBlockMessageTest", "test_parse");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleBlockMessageTest", "test_flags_and_bitfield");
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleBlockMessageTest", "test_is_Valid");

            Tools.CallStaticMethod("BitcoinLib.Test.BloomFilterTest", "test_chapter_12_p212");
            Tools.CallStaticMethod("BitcoinLib.Test.BloomFilterTest", "test_chapter_12_p215");
            Tools.CallStaticMethod("BitcoinLib.Test.BloomFilterTest", "test_add");

            Tools.CallStaticMethod("BitcoinLib.Test.MurmurHash3Test", "test_murmur3_1");
            Tools.CallStaticMethod("BitcoinLib.Test.MurmurHash3Test", "test_murmur3_2");

            Tools.CallStaticMethod("BitcoinLib.Test.FilterLoadMessageTest", "test_filterload");

            Tools.CallStaticMethod("BitcoinLib.Test.GetDataMessageTest", "test_serialize");

            Tools.CallStaticMethod("BitcoinLib.Test.Bech32Test", "test_bech32");
            
        }

        public static void RunChapters()
        {
            chapter_1();
            chapter_2();
            chapter_3();
            chapter_4();
            chapter_5();
            chapter_6();
            chapter_7();
            chapter_8();
            chapter_9();
            chapter_10();
            chapter_11();
            chapter_12();
        }

        public static void chapter_1()
        {
            //
            // Exercises and normal text in book
            //
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_1_ex_2");
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_1_ex_4");
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_1_ex_5");
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_1_ex_7");
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_1_ex_8");
        }

        public static void chapter_2()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.PointTest", "chapter_2_ex_1");
            Tools.CallStaticMethod("BitcoinLib.Test.PointTest", "chapter_2_ex_4");
            Tools.CallStaticMethod("BitcoinLib.Test.PointTest", "chapter_2_ex_5");
        }

        public static void chapter_3()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_3_ex_1");
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_3_ex_2");
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_3_page_46");
            Tools.CallStaticMethod("BitcoinLib.Test.EccTest", "test_chapter_3_ex_4");
            Tools.CallStaticMethod("BitcoinLib.Test.FieldElementTest", "chapter_3_page50");
            Tools.CallStaticMethod("BitcoinLib.Test.EccTest", "test_chapter_3_ex_5");
            Tools.CallStaticMethod("BitcoinLib.Test.EccTest", "test_chapter_3_p60");
            Tools.CallStaticMethod("BitcoinLib.Test.EccTest", "test_chapter_3_p60_b");
            Tools.CallStaticMethod("BitcoinLib.Test.EccTest", "test_chapter_3_p61");

            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_chapter_3_ex_6");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_chapter_3_page_65_dont_reveal_k");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_chapter_3_page_69_sign");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_chapter_3_ex_7");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_chapter_3_page_71_unique_k");
        }

        public static void chapter_4()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_chapter_4_ex_1");
            Tools.CallStaticMethod("BitcoinLib.Test.S256PointTest", "test_chapter_4_ex_2");
            Tools.CallStaticMethod("BitcoinLib.Test.SignatureTest", "test_chapter_4_ex_3");
            Tools.CallStaticMethod("BitcoinLib.Test.SignatureTest", "test_chapter_4_ex_4");
            Tools.CallStaticMethod("BitcoinLib.Test.SignatureTest", "test_chapter_4_ex_5");
            Tools.CallStaticMethod("BitcoinLib.Test.SignatureTest", "test_chapter_4_ex_6");
            Tools.CallStaticMethod("BitcoinLib.Test.ToolsTest", "test_chapter_4_ex_7_8_9");
        }

        public static void chapter_5()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_5_ex_5");
        }

        public static void chapter_6()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.ScriptTest", "test_chapter_6_checksig_p115");
            Tools.CallStaticMethod("BitcoinLib.Test.ScriptTest", "test_chapter_6_ex_3");
            Tools.CallStaticMethod("BitcoinLib.Test.ScriptTest", "test_chapter_6_ex_4");
        }

        public static void chapter_7()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_7_p140_create_transaction");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_7_p141_get_some_coins");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_7_ex_4");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_7_ex_5");
            Tools.CallStaticMethod("BitcoinLib.Test.ScriptTest", "test_chapter_7_p140");
        }
        public static void chapter_8()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.ScriptTest", "test_chapter_8_p2sh_p156");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_8_p_160");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_8_p_160_2");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_8_ex_4");
        }

        public static void chapter_9()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_9_ex_1_is_coinbase");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_9_165");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_9_166");
            Tools.CallStaticMethod("BitcoinLib.Test.TxTest", "test_chapter_9_ex_2_coinbase_height");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_chapter_9_p167_blockid");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_block_header_p174_new_target");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_chapter_9_new_bits");
            Tools.CallStaticMethod("BitcoinLib.Test.BlockHeaderTest", "test_chapter_9_ex_12");
        }
        public static void chapter_10()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.NetworkEnvelopeTest", "test_chapter_10_ex_2");
        }

        public static void chapter_11()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.MerkleTreeTest", "test_chapter_11_ex_5");
        }
        public static void chapter_12()
        {
            Tools.CallStaticMethod("BitcoinLib.Test.BloomFilterTest", "test_chapter_12_ex_1");
            Tools.CallStaticMethod("BitcoinLib.Test.BloomFilterTest", "test_chapter_12_ex_2");
            Tools.CallStaticMethod("BitcoinLib.Test.BloomFilterTest", "test_chapter_12_ex_6");
        }
    }
}

