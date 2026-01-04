using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class NetworkEnvelope
    {
        private const UInt32 MAINNET_MAGIC = 0xf9beb4d9;
        private const UInt32 TESTNET_MAGIC = 0x0b110907;

        /// <summary>
        /// 4 bytes identifying the network, these remain in big-endian order
        /// </summary>
        public UInt32 _magic;

        /// <summary>
        /// max 12 ASCII chars, zero-padded at the end to 12 bytes
        /// </summary>
        public string _command;
        /// <summary>
        /// little endian
        /// </summary>
        public UInt32 _payloadSize;
        /// <summary>
        /// 4 bytes big endian
        /// </summary>
        public UInt32 _checksum;

        /// <summary>
        /// bytes in order of appearance. These bytes represent the actual message being sent. One of the message types such as VersionMessage, PingMessage, etc.
        /// </summary>
        public byte[] _payload;

        public bool _testnet;

        public NetworkEnvelope()
        {
        }

        /// <summary>
        /// This is the full constructor including the payload and all other fields.
        /// </summary>
        /// <param name="magic"></param>
        /// <param name="command"></param>
        /// <param name="payloadSize"></param>
        /// <param name="checksum"></param>
        /// <param name="payload"></param>
        /// <param name="testnet"></param>
        public NetworkEnvelope(UInt32 magic, string command, UInt32 payloadSize, UInt32 checksum, byte[] payload, bool testnet)
        {
            _magic = magic;
            _command = command;
            _payloadSize = payloadSize;
            _checksum = checksum;
            _payload = payload;
            _testnet = testnet;
        }

        /// <summary>
        /// Used when only the header is read. The payload is NOT read yet.
        /// </summary>
        /// <param name="magic"></param>
        /// <param name="command"></param>
        /// <param name="payloadSize"></param>
        /// <param name="checksum"></param>
        /// <param name="testnet"></param>
        public NetworkEnvelope(UInt32 magic, string command, UInt32 payloadSize, UInt32 checksum, bool testnet)
        {
            _magic = magic;
            _command = command;
            _payloadSize = payloadSize;
            _payload = new byte[0];
            _checksum = checksum;
            _testnet = testnet;
        }


        /// <summary>
        /// This is used when an instance is not created from reading a stream, but when sending a message.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="payload"></param>
        /// <param name="testnet"></param>
        public NetworkEnvelope(string command, byte[] payload, bool testnet)
        {
            _magic = testnet ? TESTNET_MAGIC : MAINNET_MAGIC;
            _command = command;
            _payloadSize = (UInt32)payload.Length;
            byte[] bChecksum = Tools.Hash256FirstFourBytes(payload);
            _checksum = BinaryPrimitives.ReadUInt32BigEndian(bChecksum);
            _payload = payload;
            _testnet = testnet;
        }

        /// <summary>
        /// Reads the entire data. First the header is read and then the payload.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="testnet"></param>
        /// <returns></returns>
        public static NetworkEnvelope Parse(BinaryReader reader, bool testnet)
        {
            NetworkEnvelope env = ParseHeader(reader, testnet);
            env.ParsePayload(reader);

            return env;
        }

        /// <summary>
        /// Reads and parses the network envelope header from the given BinaryReader and reads everything except the payload.
        /// The checksum is read but not verified as the payload has not been read yet.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="testnet"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static NetworkEnvelope ParseHeader(BinaryReader reader, bool testnet = false)
        {
            UInt32 magic = Tools.ReadUInt32BigEndian(reader);
            if (testnet)
            {
                if (magic != TESTNET_MAGIC)
                {
                    throw new Exception("Magic number does not match testnet");
                }
            }
            else
            {
                if (magic != MAINNET_MAGIC)
                {
                    throw new Exception("Magic number does not match mainnet");
                }
            }
            string command = Encoding.ASCII.GetString(reader.ReadBytes(12)).TrimEnd('\0');
            UInt32 payloadSize = Tools.ReadUInt32LittleEndian(reader);
            UInt32 checksum = Tools.ReadUInt32BigEndian(reader);

            if (Tools.LOGGING > 1)
            {
                Tools.WriteLine($"NetworkEnvelope.ParseHeader: magic={magic:X8}, command={command}, payloadSize={payloadSize}, checksum={checksum:X8}");
            }

            NetworkEnvelope env = new NetworkEnvelope(magic, command, payloadSize, checksum, false);

            return env;
        }

        /// <summary>
        /// Reads only the payload. The header must have been read before and _payloadSize must have been read.
        /// The checksum is verified here.
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="Exception"></exception>
        public void ParsePayload(BinaryReader reader)
        {
            if (Tools.LOGGING > 2)
            {
                Tools.WriteLine($"NetworkEnvelope.ParsePayload: _payloadSize={_payloadSize}");
            }
            int payloadSize = (int)_payloadSize;
            _payload = reader.ReadBytes(payloadSize);

            if (Tools.LOGGING > 2)
            {
                string strPayloadHex = Tools.BytesToHexString(_payload);
                Tools.WriteLine($"NetworkEnvelope.ParsePayload: byte[] _payload: {_payload.Length} bytes");
                //Tools.WriteLine($"NetworkEnvelope.ParsePayload: strPayloadHex: {strPayloadHex}");
            }
            byte[] bCalculatedChecksum = Tools.Hash256FirstFourBytes(_payload);
            UInt32 calculatedChecksum = BinaryPrimitives.ReadUInt32BigEndian(bCalculatedChecksum);

            if (Tools.LOGGING > 2)
            {
                Tools.WriteLine($"NetworkEnvelope.ParsePayload: calculatedChecksum={calculatedChecksum:x8}");
            }
            if (_checksum != calculatedChecksum)
            {
                Tools.WriteLine($"NetworkEnvelope.ParsePayload: calculatedChecksum={calculatedChecksum:x8}, _checksum={_checksum}");
                throw new Exception("Checksum does not match");
            }
        }

        public byte[] serialize()
        {
            List<byte> data = new List<byte>();
            serialize(data);
            byte[] bytes = data.ToArray();

            return bytes;
        }

        public void serialize(List<byte> data)
        {
            Tools.UInt32ToBigEndian(_magic, data);

            if (_command.Length > 12)
            {
                throw new ArgumentException("Command darf maximal 12 Zeichen haben.");
            }
            byte[] result = new byte[12];
            Encoding.ASCII.GetBytes(_command, 0, _command.Length, result, 0);
            data.AddRange(result);
            Tools.UIntToLittleEndian(_payloadSize, data, 4);
            Tools.UInt32ToBigEndian(_checksum, data);
            data.AddRange(_payload);
        }

        public byte[] Payload()
        {
            return _payload;
        }
        public override string ToString() 
        {
            return $"NetworkEnvelope(Command={_command}, PayloadSize={_payloadSize}, Checksum={_checksum})";
        }
    }
}


