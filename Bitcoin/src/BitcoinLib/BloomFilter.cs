using BitcoinLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib
{
    public class BloomFilter
    {
        public const UInt32 BIP37_CONSTANT = 0xfba4c795;

        /// <summary>
        /// how many bits expressed in chunks of bytes: 1 -> 8 bits, 2 -> 16 bits
        /// </summary>
        public UInt32 _size;

        /// <summary>
        /// how often do we hash
        /// </summary>
        public UInt32 _functionCount;

        public UInt32 _tweak;

        /// <summary>
        /// Each bit is stored in one byte for easier programming
        /// </summary>
        public byte[] _bitField;

        public BloomFilter(UInt32 size, UInt32 function_count, uint tweak)
        {
            _size = size;
            _bitField = new byte[size * 8];
            _functionCount = function_count;
            _tweak = tweak;
        }

        /// <summary>
        /// Each bit is stored in one byte. Collect them all into 8-bits-per byte
        /// </summary>
        /// <returns></returns>
        public byte[] FilterBytes()
        {
            byte[] bytes = MerkleBlock.BitFieldToFlags(_bitField);

            return bytes;
        }

        /// <summary>
        /// Every time the input is hashed, we set one bit. For each hash function, the bit is probably a different one.
        /// </summary>
        /// <param name="item"></param>
        public void Add(byte[] item)
        {
            for (UInt32 i = 0; i < _functionCount; i++)
            {
                UInt32 seed = i * BIP37_CONSTANT + _tweak;

                UInt32 hash = MurmurHash3.Hash32(item, seed);
                UInt32 bitIndex = hash % (_size * 8);

                _bitField[bitIndex] = 1;
            }
        }

        /// <summary>
        /// This message is sent before getdata to the node that returns a MerkleBlock or a Tx
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public FilterLoadMessage CreateFilterLoadMessage(byte flag)
        {
            FilterLoadMessage msg = new FilterLoadMessage(_size, _functionCount, _tweak, _bitField, flag);

            return msg;
        }
    }
}
