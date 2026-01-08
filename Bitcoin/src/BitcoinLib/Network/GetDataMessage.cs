
namespace BitcoinLib.Network
{
    /// <summary>
    /// getdata contains
    /// MSG_FILTERED_BLOCK and one pubkey hash
    /// 
    /// full node does this:
    /// for all txout in the specified blocks: check if the ScriptPubKey contains the specified pubkey hash (address)
    /// 
    /// full node returns:
    /// MerkleBlock that contains the with that public key hash
    /// </summary>
    public class GetDataMessage : NetworkMessage
    {
        public static string Command = "getdata";

        /// <summary>
        /// the hash is a TXID. The receiver sends a full TX
        /// </summary>
        public static UInt32 MSG_TX = 0x01;
        public static UInt32 MSG_WITNESS_TX = 0x01000040;

        /// <summary>
        /// the hash is of a block header. The receiver sends a full block with all transactions
        /// </summary>
        public static UInt32 MSG_BLOCK = 0x02;
        public static UInt32 MSG_WITNESS_BLOCK = 0x02000040;

        /// <summary>
        /// The hash is of a block header; identical to “MSG_BLOCK”.
        /// When used in a “getdata” message, this indicates the response should be a 
        /// “merkleblock” message rather than a “block” message (but this only works if a bloom filter was previously configured). 
        /// Only for use in“getdata” messages.
        /// </summary>
        public static UInt32 MSG_FILTERED_BLOCK = 0x03;
        public static UInt32 MSG_FILTERED_WITNESS_BLOCK = 0x03000040;

        public static UInt32 MSG_CMPCT_BLOCK = 0x04;


        /// <summary>
        /// Each item is 4 byte type and 32 byte hash LE
        /// </summary>
        public List<(UInt32 type, byte[] hash)> _items = new List<(UInt32 type, byte[] hash)> ();

        public GetDataMessage() :
            base(Command)
        {
        }

        public void Add(UInt32 type, byte[] hash)
        {
            _items.Add((type, hash));
        }

        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            Tools.EncodeVarInt(data, _items.Count);

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];

                UInt32 type = item.type;
                Tools.UIntToLittleEndian(type, data, 4);

                byte[] hashLE = Tools.ReverseCopy(item.hash);
                data.AddRange(hashLE);
            }

            return data.ToArray();
        }
    }
}

