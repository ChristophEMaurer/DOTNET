using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class GetHeadersMessage : NetworkMessage
    {
        public static string Command = "getheaders";

        /// <summary>
        /// protocol version, little-endian
        /// </summary>
        public UInt32 _version;

        /// <summary>
        /// number of hashes in _block_locator_hashes, varint
        /// </summary>
        public UInt64 _hashCount;

        /// <summary>
        /// little endian, _hashCount * 32 bytes
        /// </summary>
        public byte[] _block_locator_hashes;

        /// <summary>
        /// little endian, 32 bytes
        /// </summary>
        public byte[] _hash_stop;

        public GetHeadersMessage(UInt32 version, UInt64 hashCount,
            byte[] block_locator_hashes,
            byte[] hash_stop = null)
            : base(GetHeadersMessage.Command)
        {
            _version = version;
            _hashCount = hashCount;
            _block_locator_hashes = block_locator_hashes;

            if (hash_stop == null)
            {
                // create 32 null bytes.
                _hash_stop = new byte[32];
            }
            else
            {
                if (hash_stop.Length != 32)
                {
                    throw new ArgumentException("hash_stop must be 32 bytes long, but length is " + hash_stop.Length);
                }
                _hash_stop = hash_stop;
            }
        }

        public GetHeadersMessage(byte[] block_locator_hashes) :
            this(70015, 1, block_locator_hashes, null)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            Tools.UIntToLittleEndian(_version, data, 4);
            Tools.EncodeVarInt(data, _hashCount);

            byte[] temp = Tools.ReverseCopy(_block_locator_hashes);
            data.AddRange(temp);

            temp = Tools.ReverseCopy(_hash_stop);
            data.AddRange(temp);

            return data.ToArray();
        }
    }
}
