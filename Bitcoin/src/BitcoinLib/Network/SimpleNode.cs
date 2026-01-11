using BitcoinLib.Storage;
using BitcoinLib.Visitors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class SimpleNode
    {
        public static string DefaultHost = "65.109.24.172";
        public string _host = DefaultHost;
        public int _port;

        bool _testnet;

        NetworkStream _stream;

        public SimpleNode(bool testnet) : this(DefaultHost, testnet)
        {
        }

        public SimpleNode(string host, bool testnet)
        {
            _host = host;
            if (testnet)
            {
                _port = 18333;
            }
            else
            {
                _port = 8333;
            }
            _testnet = testnet;
        }

        public void Init()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (Tools.LOGGING > 0)
                {
                    Tools.WriteLine("connecting to " + _host + ":" + _port);
                }
                socket.Connect(_host, _port);
                _stream = new NetworkStream(socket, ownsSocket: false);
            }
            catch (Exception ex)
            {
                Tools.WriteLine("Error: in catch block");
                Tools.WriteLine(ex.Message);
            }
        }

        public bool Send(NetworkMessage message)
        {
            string strData;

            if (_stream == null)
            {
                return false;
            }

            byte[] bData = message.Serialize();
            //strData = Tools.BytesToHexString(bData);
            //string want =                                                  "7f11010000000000000000000000000000000000000000000000000000000000000000000000ffff00000000208d000000000000000000000000000000000000ffff00000000208d0000000000000000182f70726f6772616d6d696e67626974636f696e3a302e312f0000000000";

            NetworkEnvelope envelope = new NetworkEnvelope(message._command, bData, _testnet);

            byte[] bEnvelope = envelope.serialize();
            strData = Tools.BytesToHexString(bEnvelope);
            //      testnet  version                  len      checksum
            //     "0b110907 76657273696f6e0000000000 6e000000 c46da2f5 7f11010000000000000000000000000000000000000000000000000000000000000000000000ffff00000000208d000000000000000000000000000000000000ffff00000000208d0000000000000000182f70726f6772616d6d696e67626974636f696e3a302e312f0000000000"
            //want = "0b110907 76657273696f6e0000000000 6e000000 f5a26dc4 7f11010000000000000000000000000000000000000000000000000000000000000000000000ffff00000000208d000000000000000000000000000000000000ffff00000000208d0000000000000000182f70726f6772616d6d696e67626974636f696e3a302e312f0000000000".Replace(" ","");

            if (Tools.LOGGING > 3)
            {
                Tools.WriteLine("sending bytes for " + message._command);
                Tools.WriteLine(strData);
            }

            _stream.Write(bEnvelope, 0, bEnvelope.Length);
            _stream.Flush();

            if (Tools.LOGGING > 0)
            {
                Tools.WriteLine("sent " + envelope._command);
                //Tools.WriteLine(strData);
            }

            return true;
        }

        public NetworkEnvelope Read()
        {
            if (Tools.LOGGING > 2)
            {
                Tools.WriteLine("--> NetworkEnvelope Read()");
            }
            if (_stream == null)
            {
                if (Tools.LOGGING > 2)
                {
                    Tools.WriteLine("NetworkEnvelope Read(): _stream == null , exiting");
                }
                return null;
            }

            if (Tools.LOGGING > 2)
            {
                Tools.WriteLine("Reading 24 bytes...");
            }

            byte[] header = Tools.ReadExactly(_stream, 24);
            NetworkEnvelope envelope = NetworkEnvelope.ParseHeader(new BinaryReader(new MemoryStream(header)), _testnet);

            if (Tools.LOGGING > 1)
            {
                Tools.WriteLine("Received NetworkEnvelope: " + envelope._command);
                Tools.WriteLine("Reading payload: " + envelope._payloadSize + " bytes");
            }

            byte[] payload = Tools.ReadExactly(_stream, (int)envelope._payloadSize);
            if (Tools.LOGGING > 1)
            {
                Tools.WriteLine("Read byte[] payload: " + payload.Length + " bytes");
            }
            envelope.ParsePayload(new BinaryReader(new MemoryStream(payload)));

            if (Tools.LOGGING > 2)
            {
                Tools.WriteLine("<-- NetworkEnvelope Read()");
            }

            return envelope;
        }

        /// <summary>
        /// Waits for one of the specified commands to be received.
        /// While waiting, a ping is answered with a pong and a version with a verack.
        /// </summary>
        /// <param name="commands"></param>
        /// <returns>A subclass of NetworkMessage</returns>
        public NetworkMessage WaitFor(List<string> commands)
        {
            if (Tools.LOGGING > 2)
            {
                Tools.WriteLine("--> NetworkMessage WaitFor()" + commands);
            }

            NetworkEnvelope envelope = new NetworkEnvelope();
            string command = "";

            while (!commands.Contains(command))
            {
                if (Tools.LOGGING > 0)
                {
                    Tools.WriteLine("waiting for: " + string.Join(", ", commands));
                }

                // this reads the header and the payload.
                envelope = Read();
                if (envelope == null)
                {
                    continue;
                }

                if (Tools.LOGGING > 0)
                {
                    Tools.WriteLine("received " + envelope._command);
                }

                if (envelope._command == VersionMessage.Command)
                {
                    Send(new VerAckMessage());
                }
                else if (envelope._command == PingMessage.Command)
                {
                    PongMessage pong = new PongMessage(envelope.Payload());
                    Send(pong);
                }

                command = envelope._command;
            }

            // we only do this, if we have a more complicated network message with non-trivial data structures.
            // the class is created from the payload which has already been read.
            string strClassName = NetworkMessage.GetClassNameForCommand(command);;
            byte[] payload = envelope.Payload();
            NetworkMessage message = (NetworkMessage)Tools.CallStaticMethod(strClassName, "Parse", payload);

            if (Tools.LOGGING > 2)
            {
                Tools.WriteLine("<-- NetworkMessage WaitFor()" + commands);
            }

            return message;
        }

        /// <summary>
        /// Do a handshake with the other node. Handshake is sending a version message and getting a verack back
        /// </summary>
        public void Handshake()
        {
            if (Tools.LOGGING > 0)
            {
                Tools.WriteLine("--> SimpleNode::Handshake()");
            }

            if (_stream == null)
            {
                Tools.WriteLine("Error (_Stream == null): exiting...");
            }
            else
            {
                VersionMessage version = new VersionMessage();
                Send(version);
                NetworkMessage message = WaitFor(new() { VerAckMessage.Command  });
            }

            if (Tools.LOGGING > 0)
            {
                Tools.WriteLine("<-- SimpleNode::Handshake()");
            }
        }

        /// <summary>
        /// Return the block which has the specified previous hash in its block header.
        /// This is the successor of the specified hash.
        /// </summary>
        /// <param name="prevHash"></param>
        /// <exception cref="Exception"></exception>
        public static Block GetBlockWithPrevHash(string prevHash)
        {
            byte[] start_block = Tools.HexStringToBytes(prevHash);

            SimpleNode node = new SimpleNode(false); // 02.01.2026: 65.109.24.172 worked in: python, C#
            node.Init();
            node.Handshake();

            GetHeadersMessage getHeadersMessage = new GetHeadersMessage(start_block);
            node.Send(getHeadersMessage);
            NetworkMessage networkMessage = node.WaitFor(new() { HeadersMessage.Command });
            HeadersMessage headerMessage = (HeadersMessage)networkMessage;

            GetDataMessage getDataMessage = new GetDataMessage();
            foreach (BlockHeader blockHeader in headerMessage._blockHeaders)
            {
                byte[] currentHash = blockHeader.Hash();
                string strCurrentHash = Tools.BytesToHexString(currentHash);
                Console.WriteLine("header: " + strCurrentHash);
                if (blockHeader.CheckPow())
                {
                    Console.WriteLine("header is valid!");
                }
                else
                {
                    throw new Exception("proof of work is invalid");
                }

                if (blockHeader._prevBlockHash.SequenceEqual(start_block))
                {
                    Console.WriteLine("Found block with specified prevHash!");
                    getDataMessage.Add(GetDataMessage.MSG_BLOCK, currentHash);
                    break;
                }
            }

            node.Send(getDataMessage);
            networkMessage = node.WaitFor(new() { BlockMessage.Command });
            BlockMessage blockMessage = (BlockMessage)networkMessage;

            return blockMessage._block;
        }

        /// <summary>
        /// Return the block whose block header has this hash
        /// </summary>
        /// <param name="hash"></param>
        /// <exception cref="Exception"></exception>
        public static Block GetBlockWithHash(string hash)
        {
            byte[] start_block = Tools.HexStringToBytes(hash);

            SimpleNode node = new SimpleNode(false); // 02.01.2026: 65.109.24.172 worked in: python, C#
            node.Init();
            node.Handshake();

            GetHeadersMessage getHeadersMessage = new GetHeadersMessage(start_block);
            node.Send(getHeadersMessage);
            NetworkMessage networkMessage = node.WaitFor(new() { HeadersMessage.Command });
            HeadersMessage headerMessage = (HeadersMessage)networkMessage;

            GetDataMessage getDataMessage = new GetDataMessage();
            foreach (BlockHeader blockHeader in headerMessage._blockHeaders)
            {
                byte[] currentHash = blockHeader.Hash();
                string strCurrentHash = Tools.BytesToHexString(currentHash);
                Console.WriteLine("header: " + strCurrentHash);
                if (blockHeader.CheckPow())
                {
                    Console.WriteLine("header is valid!");
                }
                else
                {
                    throw new Exception("proof of work is invalid");
                }

                if (strCurrentHash.Equals(hash))
                {
                    Console.WriteLine("Found block with specified hash");
                    getDataMessage.Add(GetDataMessage.MSG_BLOCK, currentHash);
                    break;
                }
            }

            node.Send(getDataMessage);
            networkMessage = node.WaitFor(new() { BlockMessage.Command });
            BlockMessage blockMessage = (BlockMessage)networkMessage;

            return blockMessage._block;
        }
    }
}
