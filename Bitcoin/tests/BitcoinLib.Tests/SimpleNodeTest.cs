using BitcoinLib.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BitcoinLib.Test
{
    public class SimpleNodeTest : UnitTest
    {
        // testnet port 18333
        // testnet-seed.bitcoin.jonasschnelli.ch
        // testnet-seed.bitcoin.sipa.be             -> host name unknown, timeout
        // testnet-seed.bluematt.me 
        // seed.tbtc.petertodd.org                  -> timeout
        //
        // mainnet port 8333:
        // seed.bitcoin.sipa.be:8333
        // seed.bitcoin.jonasschnelli.ch:8333
        // dnsseed.bluematt.me:8333
        //
        // 29.12.2025 testnet.programmingbitcoin.com geht nicht, auch nicht im Python code
        //private const string URL_NODE_TESTNET = "testnet.programmingbitcoin.com";
        //private const string URL_NODE_TESTNET = "testnet-seed.bluematt.me"; // 29.12.2025: this worked in C#
        private const string URL_NODE_TESTNET = "65.109.24.172"; // 29.12.2025: this worked in C#
        private const string URL_NODE_MAINNET = "dnsseed.bluematt.me"; // 29.12.2025: this worked in: Python

        public static void test_handshake()
        {
            //
            // testnet.programmingbitcoin.com: this works in the python code! 24.12.2025
            //
            SimpleNode node = new SimpleNode(URL_NODE_TESTNET, true, 1);
            node.Init();
            node.Handshake();
        }

        public static void test_getheaders()
        {
            try
            {
                SimpleNode node = new SimpleNode(URL_NODE_TESTNET, true, 0);
                node.Init();
                node.Handshake();
                BlockHeader genesis = BlockHeader.Parse(Block.GENESIS_BLOCK);

                byte[] start_block = genesis.Hash();
                GetHeadersMessage getheaders = new GetHeadersMessage(start_block);
                bool success = node.Send(getheaders);
                AssertTrue(success);
            }
            catch (Exception ex)
            {
                ConsoleOutWriteLine("SimpleNodeTest.test_getheaders() Exception: " + ex.Message);
                AssertTrue(false);
            }
        }
        public static void test_headers()
        {
            try
            {
                BlockHeader previous = BlockHeader.Parse(Block.GENESIS_BLOCK);
                UInt32 first_epoch_timestamp = previous._timestamp;
                UInt32 expected_bits = Block.LOWEST_BITS;
                Console.WriteLine(string.Format("expected bits: {0:x8}", expected_bits));

                int count = 1;

                SimpleNode node = new SimpleNode("dnsseed.bluematt.me", false, 1); // 29.12.2025: this worked in: Python and C#
                node.Init();
                node.Handshake();

                for (int i = 0; i < 19; i++)
                {
                    byte[] start_block = previous.Hash();
                    GetHeadersMessage getheaders = new GetHeadersMessage(start_block);
                    bool success = node.Send(getheaders);
                    AssertTrue(success);

                    NetworkMessage message = node.WaitFor(new() { HeadersMessage.Command });
                    HeadersMessage headers = (HeadersMessage)message;

                    foreach (BlockHeader header in headers._blockHeaders)
                    {
                        if (!header.CheckPow())
                        {
                            throw new Exception(string.Format("invalid proof of work at block {0}", count));
                        }
                        byte[] calculatedPreviousHash = previous.Hash();
                        if (!header._prevBlockHash.SequenceEqual(calculatedPreviousHash))
                        {
                            throw new Exception(string.Format("discontinuous block at {0}", count));
                        }
                        if (count % 2016 == 0)
                        {
                            UInt32 time_diff = previous._timestamp - first_epoch_timestamp;
                            expected_bits = BlockHeader.CalculateNewBits(previous._bits, (int)time_diff);
                            Console.WriteLine(string.Format("Block {0} expected bits: {1:x8}, actual bits: {2:x8}", count, expected_bits, header._bits));
                            first_epoch_timestamp = header._timestamp;
                        }
                        if (header._bits != expected_bits)
                        {
                            throw new Exception(string.Format("invalid bits at block {0}, expected {1:X8}, actual {2:X8}", count, expected_bits, header._bits));
                        }
                        previous = header;
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleOutWriteLine("SimpleNodeTest.test_headers() Exception: " + ex.Message);
                AssertTrue(false);
            }
        }

        /// <summary>
        /// page 218: we look at 2000 blocks after a particular block for UTXOs corresponding to a particular address.
        /// This function succeeded on 02.01.2026!!! It takes less than 1 second
        /// Output: found e3930e1e566ca9b75d53b0eb9acb7607f547e1182d1d22bd4b661cfe18dcddf1:0
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void test_get_transaction_of_interest()
        {
            string last_block_hex = "00000000000538d5c2246336644f9a4956551afb44ba47278759ec55ea912e19";
            string address = "mwJn1YPMq7y5F8J3LkC5Hxg9PHyZ5K4cFv";
            byte[] h160 = Base58Encoding.DecodeH160(address);
            //string actual = Tools.BytesToHexString(h160);
            //string want = "ad346f8eb57dee9a37981716e498120ae80e44f7";
            SimpleNode node = new SimpleNode("65.109.24.172", true, 1); // 02.01.2026: 65.109.24.172 worked in: python, C#
            BloomFilter bf = new BloomFilter(30, 5, 90210);
            bf.Add(h160);
            node.Init();
            node.Handshake();
            FilterLoadMessage filterLoadMessage = bf.CreateFilterLoadMessage(1);
            byte[] start_block = Tools.HexStringToBytes(last_block_hex);
            GetHeadersMessage getHeadersMessage = new GetHeadersMessage(start_block);
            node.Send(getHeadersMessage);
            NetworkMessage networkMessage = node.WaitFor(new () { HeadersMessage.Command });
            HeadersMessage headerMessage = (HeadersMessage)networkMessage;
            Tools.WriteLine("headerMessage has " + headerMessage._blockHeaders.Count + " block headers");
            GetDataMessage getDataMessage = new GetDataMessage();
            foreach (BlockHeader blockHeader in headerMessage._blockHeaders)
            {
                if (!blockHeader.CheckPow())
                {
                    throw new Exception("proof of work is invalid");
                }
                getDataMessage.Add(GetDataMessage.MSG_FILTERED_BLOCK, blockHeader.Hash());
            }

            //want = "1e000000000448000000000004000000000200000000000000000000000000050000006260010001";
            //actual = Tools.BytesToHexString(filterLoadMessage.Serialize());

            node.Send(filterLoadMessage);
            Console.WriteLine("getDataMessage has "+ getDataMessage._items.Count + " items");
            node.Send(getDataMessage);
            bool found = false;
            while (!found)
            {
                NetworkMessage msg = node.WaitFor(new () { MerkleBlockMessage.Command, Tx.Command });
                if (msg._command == MerkleBlockMessage.Command)
                {
                    MerkleBlockMessage merkleBlockMessage = (MerkleBlockMessage)msg;
                    if (!merkleBlockMessage._merkleBlock.IsValid())
                    {
                        throw new Exception("invalid merkle proof");
                    }
                    else
                    {
                        Console.WriteLine("MerkleBlock is valid!");
                    }
                }
                else if (msg._command == Tx.Command)
                {
                    Tx tx = (Tx)msg;
                    Tools.WriteLine("tx has " + tx._txOuts.Count + " txOuts");
                    for (int i = 0; i < tx._txOuts.Count; i++)
                    {
                        TxOut txOut = tx._txOuts[i];
                        if (txOut._script_pubkey.Address(true).Equals(address))
                        {
                            Tools.WriteLine($"found {tx.Id()}:{i}");
                            found = true;
                            //break; dont breek, there could be several tx of interest
                        }
                    }
                }
                else
                {
                    // not possible, do nothing
                }
            }
        }
    }
}

