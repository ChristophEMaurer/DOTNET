using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class PingMessage : NetworkMessage
    {
        public static string Command = "ping";

        public UInt64 _nonce;

        public PingMessage(UInt64 nonce) :
            base(Command)
        {
            _nonce = nonce;
        }

        /*
        public static PingMessage Parse(BinaryReader input)
        {
            UInt64 nonce = Tools.ReadUInt64LittleEndian(input);

            PingMessage ping = new PingMessage(nonce);

            return ping;
        }
        */

        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();
            
            Tools.UIntToLittleEndian(_nonce, data, 8);
         
            return data.ToArray();
        }
    }
}

