using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class VersionMessage : NetworkMessage
    {
        public static string Command = "version";

        /// <summary>
        /// protocol version, little-endian
        /// </summary>
        public UInt32 _version;

        /// <summary>
        /// network services of sender, 8 bytes, little-endian
        /// </summary>
        public UInt64 _services;
        /// <summary>
        /// little endian
        /// </summary>
        public UInt64 _timestamp;

        /// <summary>
        /// little endian
        /// </summary>
        UInt64 _receiverServices;

        /// <summary>
        /// IPV4 is 10 00 bytes and 2 ff bytes then 4 bytes receiver ip, big endian
        /// </summary>
        byte[] _receiverIp;
        /// <summary>
        /// big endian
        /// </summary>
        UInt16 _receiverPort;

        /// <summary>
        /// little endian
        /// </summary>
        UInt64 _senderServices;
        /// <summary>
        /// IPV4 is 10 00 bytes and 2 ff bytes then 4 bytes receiver ip
        /// </summary>
        byte[] _senderIp;
        /// <summary>
        /// big endian
        /// </summary>
        UInt16 _senderPort;

        /// <summary>
        /// 8 bytes big endian
        /// </summary>
        UInt64 _nonce;

        UInt64 _userAgentLength;
        byte[] _userAgent;

        /// <summary>
        /// 4 bytes big endian
        /// </summary>
        UInt32 _latestBlock;

        byte _relay;

        public VersionMessage(UInt32 version, UInt64 services, UInt64 timestamp,
            UInt64 receiverServices, byte[] receiverIp, UInt16 receiverPort,
            UInt64 senderServices, byte[] senderIp, UInt16 senderPort,
            UInt64 nonce, UInt64 userAgentLength, byte[] userAgent, UInt32 latestBlock, byte relay)
            : base(VersionMessage.Command)
        {
            _version = version;
            _services = services;
            _timestamp = timestamp;
            _receiverServices = receiverServices;

            _receiverIp = receiverIp;

            _receiverPort = receiverPort;
            _senderServices = senderServices;
            _senderIp = senderIp;
            _senderPort = senderPort;
            _nonce = nonce;
            _userAgentLength = userAgentLength;
            _userAgent = userAgent;
            _latestBlock = latestBlock;
            _relay = relay;
        }

        public VersionMessage(string userAgent = "/Christoph:0.1/") :
            this(70015, 0, 0,
                0, new byte[4], 8333,
                0, new byte[4], 8333,
                0, (UInt64)userAgent.Length, Encoding.ASCII.GetBytes(userAgent), 0, 0x00)
        {
            // /programmingbitcoin:0.1/
            // /Christoph:0.0/
        }


        public override byte[] Serialize()
        {
            byte[] ipv4 = new byte[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0xff };

            List<byte> result = new List<byte>();

            Tools.UIntToLittleEndian(_version, result, 4);
            Tools.UIntToLittleEndian(_services, result, 8);
            Tools.UIntToLittleEndian(_timestamp, result, 8);

            Tools.UIntToLittleEndian(_receiverServices, result, 8);
            result.AddRange(ipv4);
            result.AddRange(_receiverIp);
            Tools.UInt16ToBigEndian(_receiverPort, result);

            Tools.UIntToLittleEndian(_senderServices, result, 8);
            result.AddRange(ipv4);
            result.AddRange(_senderIp);
            Tools.UInt16ToBigEndian(_senderPort, result);

            Tools.UIntToLittleEndian(_nonce, result, 8);

            Tools.EncodeVarInt(result, _userAgentLength);
            result.AddRange(_userAgent);

            Tools.UInt32ToBigEndian(_latestBlock, result);
            result.Add(_relay);

            return result.ToArray();
        }
    }
}
