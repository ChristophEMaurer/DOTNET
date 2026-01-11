using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitcoinLib
{
    public class TxStatus
    {
        /// <summary>
        /// this field is from the block header. The tx cannot know this value, so 
        /// we set it from outside only when parsing a bblock_heightlock. 
        /// When the block read all Tx, we can set block_height, _block_hash and _block_time.
        /// </summary>
        private int? _block_height;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? block_height { get { return _block_height; } set { _block_height = value; } }

        private string _block_hash;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string block_hash { get { return _block_hash; } set { _block_hash = value; } }

        // _block_time is block._timestamp
        private UInt32? _block_time;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UInt32? block_time { get { return _block_time; } set { _block_time = value; } }


    }
}
