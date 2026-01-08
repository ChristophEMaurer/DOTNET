using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class FilterLoadMessage : NetworkMessage
    {
        public static string Command = "filterload";

        public UInt32 _size;
        public UInt32 _function_count;
        public UInt32 _tweak;
        public byte[] _bitField;

        /// <summary>
        /// 0x00    BLOOM_UPDATE_NONE
        /// 0x01    BLOOM_UPDATE_ALL
        /// 0x02    BLOOM_UPDATE_P2PUBKEY_ONLY
        /// </summary>
        public byte _flag;

        public FilterLoadMessage(UInt32 size, UInt32 functionCount, UInt32 tweak, byte[] bitField, byte flag) :
            base(Command)
        {
            _size = size;
            _function_count = functionCount;
            _tweak = tweak;
            _bitField = bitField;
            _flag = flag;
        }

        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            Tools.EncodeVarInt(data, _size);

            byte[] bytes = MerkleBlock.BitFieldToFlags(_bitField);
            data.AddRange(bytes);

            Tools.UIntToLittleEndian(_function_count, data, 4);
            Tools.UIntToLittleEndian(_tweak, data, 4);
            data.Add(_flag);
         
            return data.ToArray();
        }
    }
}

