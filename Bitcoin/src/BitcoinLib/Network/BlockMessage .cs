using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class BlockMessage : NetworkMessage
    {
        public static string Command = "block";

        public Block _block;
        public BlockMessage(Block block) :
            base(Command)
        {
            _block = block;
        }

        public static BlockMessage Parse(byte[] data)
        {
            Block block = Block.Parse(data);

            BlockMessage blockMessage = new BlockMessage(block);

            return blockMessage;
        }
    }
}

