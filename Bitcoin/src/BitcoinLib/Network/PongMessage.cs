using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class PongMessage : NetworkMessage
    {
        public static string Command = "pong";

        /// <summary>
        /// The payload of the pong message is just the nonce sent in the ping message
        /// </summary>
        public UInt64 _nonce;

        /// <summary>
        /// This is used when parsing bytes into a PongMessage
        /// </summary>
        /// <param name="nonce"></param>
        public PongMessage(UInt64 nonce) :
            base(PongMessage.Command)
        {
            _nonce = nonce;
        }

        /// <summary>
        /// This is used when creating a PongMessage to send in response to a PingMessage.
        /// </summary>
        /// <param name="data">The value from the PingMessage</param>
        public PongMessage(byte[] data) :
            base(PongMessage.Command)
        {
            UInt64 nonce = BinaryPrimitives.ReadUInt64LittleEndian(data);
            _nonce = nonce;
        }
        public static PongMessage Parse(BinaryReader input)
        {
            UInt64 nonce = Tools.ReadUInt64LittleEndian(input);

            PongMessage ping = new PongMessage(nonce);

            return ping;
        }

        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();
            
            Tools.UIntToLittleEndian(_nonce, data, 8);
         
            return data.ToArray();
        }
    }
}

