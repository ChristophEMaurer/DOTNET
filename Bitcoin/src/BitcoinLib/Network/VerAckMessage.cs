using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class VerAckMessage : NetworkMessage
    {
        public static string Command = "verack";

        public VerAckMessage() :
            base(VerAckMessage.Command)
        {
        }


        public static VerAckMessage Parse(byte[] raw)
        {
            return new VerAckMessage();
        }

        public static VerAckMessage Parse(BinaryReader input)
        {
            return new VerAckMessage();
        }

        public override byte[] Serialize()
        {
            return new byte[0];
        }
    }
}

