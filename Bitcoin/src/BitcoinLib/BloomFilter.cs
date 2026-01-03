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

        public UInt32 _size;
        public UInt32 _functionCount;
        public UInt32 _tweak;
        public byte[] _bitField;

        public BloomFilter(UInt32 size, UInt32 function_count, uint tweak)
        {
            _size = size;
            _bitField = new byte[size * 8];
            _functionCount = function_count;
            _tweak = tweak;
        }

        public byte[] FilterBytes()
        {
            byte[] bytes = MerkleBlock.BitFieldToFlags(_bitField);

            return bytes;
        }

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

        public FilterLoadMessage CreateFilterLoadMessage(byte flag)
        {
            FilterLoadMessage msg = new FilterLoadMessage(_size, _functionCount, _tweak, _bitField, flag);

            return msg;
        }
    }
}
