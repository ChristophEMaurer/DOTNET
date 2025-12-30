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
        string _host;
        int _port;
        bool _testnet;

        NetworkStream _stream;

        public SimpleNode(string host, bool testnet, bool logging)
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
            Tools.LOGGING = logging;
        }

        public void Init()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(_host, _port);
                _stream = new NetworkStream(socket, ownsSocket: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: in catch block");
                Console.WriteLine(ex);
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
            strData = Tools.BytesToHexString(bData);
            string want =                                                  "7f11010000000000000000000000000000000000000000000000000000000000000000000000ffff00000000208d000000000000000000000000000000000000ffff00000000208d0000000000000000182f70726f6772616d6d696e67626974636f696e3a302e312f0000000000";

            NetworkEnvelope envelope = new NetworkEnvelope(message._command, bData, _testnet);

            byte[] bEnvelope = envelope.serialize();
            strData = Tools.BytesToHexString(bEnvelope);
            //      testnet  version                  len      checksum
            //     "0b110907 76657273696f6e0000000000 6e000000 c46da2f5 7f11010000000000000000000000000000000000000000000000000000000000000000000000ffff00000000208d000000000000000000000000000000000000ffff00000000208d0000000000000000182f70726f6772616d6d696e67626974636f696e3a302e312f0000000000"
            want = "0b110907 76657273696f6e0000000000 6e000000 f5a26dc4 7f11010000000000000000000000000000000000000000000000000000000000000000000000ffff00000000208d000000000000000000000000000000000000ffff00000000208d0000000000000000182f70726f6772616d6d696e67626974636f696e3a302e312f0000000000".Replace(" ","");

            if (Tools.LOGGING)
            {
                Console.WriteLine("Sending NetworkEnvelope: " + envelope.ToString());
                Console.WriteLine(strData);
            }

            _stream.Write(bEnvelope, 0, bEnvelope.Length);
            _stream.Flush();

            return true;
        }

        public NetworkEnvelope Read()
        {
            if (Tools.LOGGING)
            {
                Console.WriteLine("--> NetworkEnvelope Read()");
            }
            if (_stream == null)
            {
                if (Tools.LOGGING)
                {
                    Console.WriteLine("NetworkEnvelope Read(): _stream == null , exiting");
                }
                return null;
            }

            if (Tools.LOGGING)
            {
                Console.WriteLine("Reading 24 bytes...");
            }

            byte[] header = Tools.ReadExactly(_stream, 24);
            NetworkEnvelope envelope = NetworkEnvelope.ParseHeader(new BinaryReader(new MemoryStream(header)), _testnet);

            if (Tools.LOGGING)
            {
                Console.WriteLine("Received NetworkEnvelope: " + envelope._command);
                Console.WriteLine("Reading payload: " + envelope._payloadSize + " bytes");
            }

            byte[] payload = Tools.ReadExactly(_stream, (int)envelope._payloadSize);
            if (Tools.LOGGING)
            {
                Console.WriteLine("Read byte[] payload: " + payload.Length + " bytes");
            }
            envelope.ParsePayload(new BinaryReader(new MemoryStream(payload)));

            if (Tools.LOGGING)
            {
                Console.WriteLine("<-- NetworkEnvelope Read()");
            }

            return envelope;
        }

        /// <summary>
        /// Waits for one of the specified commands to be received.
        /// </summary>
        /// <param name="commands"></param>
        /// <returns>A subclass of NetworkMessage</returns>
        public NetworkMessage WaitFor(List<string> commands)
        {
            if (Tools.LOGGING)
            {
                Console.WriteLine("--> NetworkMessage WaitFor()" + commands);
            }

            NetworkEnvelope envelope = new NetworkEnvelope();
            string command = "";

            while (!commands.Contains(command))
            {
                envelope = Read();
                if (envelope == null)
                {
                    continue;
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

            string strClassName = NetworkMessage.GetClassNameForCommand(command);;
            byte[] payload = envelope.Payload();
            NetworkMessage message = (NetworkMessage)Tools.CallStaticMethod(strClassName, "Parse", payload);

            if (Tools.LOGGING)
            {
                Console.WriteLine("<-- NetworkMessage WaitFor()" + commands);
            }

            return message;
        }

        /// <summary>
        /// Do a handshake with the other node. Handshake is sending a version message and getting a verack back
        /// </summary>
        public void Handshake()
        {
            if (_stream == null)
            {
                Console.Out.WriteLine("Error (_Stream == null): Unable to create stream, exiting...");
                return;
            }          
            
            //VersionMessage version = new VersionMessage("/programmingbitcoin:0.1/");
            VersionMessage version = new VersionMessage();
            Send(version);
            List<string> commandsToWaitFor = new List<string>() { VerAckMessage.Command };
            NetworkMessage message = WaitFor(commandsToWaitFor);
        }


    }
}
