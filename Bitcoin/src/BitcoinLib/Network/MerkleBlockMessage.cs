using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitcoinLib.Network
{
    /// <summary>
    /// A MerkleBlockMessage message is sent from a full node to a client if the client requested data with getdata + bloom filter
    /// </summary>
    public class MerkleBlockMessage : NetworkMessage
    {
        public static string Command = "merkleblock";

        public MerkleBlock _merkleBlock;

        public MerkleBlockMessage(MerkleBlock merkleBlock) : base(MerkleBlockMessage.Command)
        {
            _merkleBlock = merkleBlock;
        }

        public static MerkleBlockMessage Parse(byte[] input)
        {
            MerkleBlock merkleBlock = MerkleBlock.Parse(input);

            MerkleBlockMessage msg = new MerkleBlockMessage(merkleBlock);

            return msg;
        }

        /// <summary>
        /// We do not need the serialization for now because we never send merkleblock messages, we only receive them.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override byte[] Serialize()
        {
            throw new NotImplementedException("MerkleBlockMessage.Serialize(): we never send this message!");
        }
    }
}

