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
        // testnet-seed.bitcoin.sipa.be             -> host name unknown
        // testnet-seed.bluematt.me
        // seed.tbtc.petertodd.org
        //
        // mainnet port 8333:
        // seed.bitcoin.sipa.be:8333
        // seed.bitcoin.jonasschnelli.ch:8333
        // dnsseed.bluematt.me:8333
        //
        // 29.12.2025 testnet.programmingbitcoin.com geht nicht, auch nicht im Python code
        //private const string URL_NODE_TESTNET = "testnet.programmingbitcoin.com";
        private const string URL_NODE_TESTNET = "testnet-seed.bluematt.me"; // 29.12.2025: this worked in C#
        private const string URL_NODE_MAINNET = "dnsseed.bluematt.me"; // 29.12.2025: this worked in: Python

        public static void test_handshake()
        {
            //
            // testnet.programmingbitcoin.com: this works in the python code! 24.12.2025
            //
            SimpleNode node = new SimpleNode(URL_NODE_TESTNET, true, false);
            node.Init();
            node.Handshake();
        }

        public static void test_getheaders()
        {
            try
            {
                SimpleNode node = new SimpleNode(URL_NODE_TESTNET, true, false);
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

                SimpleNode node = new SimpleNode("dnsseed.bluematt.me", false, false); // 29.12.2025: this worked in: Python and C#
                node.Init();
                node.Handshake();

                for (int i = 0; i < 19; i++)
                {
                    byte[] start_block = previous.Hash();
                    GetHeadersMessage getheaders = new GetHeadersMessage(start_block);
                    bool success = node.Send(getheaders);
                    AssertTrue(success);

                    List<string> wait_for = new List<string>();
                    wait_for.Add("headers");
                    NetworkMessage message = node.WaitFor(wait_for);
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
    }
}

