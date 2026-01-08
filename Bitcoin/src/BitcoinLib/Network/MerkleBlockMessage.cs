using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitcoinLib.Network
{
    /// <summary>
    /// A MerkleBlockMessage message is sent from a full node to a client if the client requested data with getdata + bloom filter
    ///
    /// https://btcinformation.org/en/developer-reference#merkleblock:
    ///The merkleblock message is a reply to a getdata message which requested a block using the inventory type MSG_MERKLEBLOCK.
    ///        It is only part of the reply: if any matching transactions are found, they will be sent separately as tx messages.
    ///
    ///If a filter has been previously set with the filterload message,
    ///the merkleblock message will contain the TXIDs of any transactions in the requested block that matched the filter,
    ///        as well as any parts of the block’s merkle tree necessary to connect those transactions to the block header’s merkle root.
    ///The message also contains a complete copy of the block header to allow the client to hash it and confirm its proof of work.
    ///
    /// </summary>
    public class MerkleBlockMessage : NetworkMessage
    {
        public static string Command = "merkleblock";

        public MerkleBlock _merkleBlock;

        public MerkleBlockMessage(MerkleBlock merkleBlock) : base(Command)
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

