using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitcoinLib.Storage
{
    public class BlockIndexEntry
    {
        /// <summary>
        /// _prevBlockHash is big endian here, stored as little endian
        /// </summary>
        public byte[] _prevBlockHash;    // 32
        public string prevBlockHash {  get { return Tools.BytesToHexString(_prevBlockHash); } }

        /// <summary>
        /// The height of the genesis block is 0, the height of the block after that is 1, and so on.
        /// </summary>
        public Int32 _height;
        public Int32 height { get { return _height; } }

        public UInt32 _fileNumber;        // n -> blk0000n.dat
        public UInt32 fileNumber { get { return _fileNumber; } }

        public UInt32 _offset;      // offset within the file
        public UInt32 offset { get { return _offset; } }

        public UInt32 _size;        // total size of block in bytes
        public UInt32 size { get { return _size; } }

        /// <summary>
        /// BLOCK_VALID_UNKNOWN      = 0,
        /// BLOCK_VALID_HEADER       = 1,
        /// BLOCK_VALID_TREE         = 2,
        /// BLOCK_VALID_TRANSACTIONS = 3,
        /// BLOCK_VALID_CHAIN        = 4,
        /// BLOCK_VALID_SCRIPTS      = 5,
        /// 
        /// BLOCK_VALID_MASK         = 7,
        /// 
        /// BLOCK_HAVE_DATA          = 1 << 3,
        /// BLOCK_HAVE_UNDO          = 1 << 4,
        /// 
        /// BLOCK_FAILED_VALID       = 1 << 5,
        /// BLOCK_FAILED_CHILD       = 1 << 6
        /// </summary>
        public byte _status;
        public byte status { get { return _status; } }

        public BlockIndexEntry(byte[] prevHash, Int32 height, UInt32 file, UInt32 offset, UInt32 size, byte status)
        {
            _prevBlockHash = prevHash;
            _height = height;
            _fileNumber = file;
            _offset = offset;
            _size = size;
            _status = status;
        }

        public byte[] serialize()
        {
            byte[] prevHash_le = (byte[])_prevBlockHash.Clone();
            prevHash_le.Reverse();

            byte[] data = new byte[32 + (4 * 4) + 1]; // 32 + 16 + 1 = 48 + 1 = 49

            prevHash_le.CopyTo(data, 0);
            BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(32), _height);
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(36), _fileNumber);
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(40), _offset);
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(44), _size);
            data[48] = _status;

            return data;
        }

        public static BlockIndexEntry Parse(byte[] data)
        {
            return Parse(new BinaryReader(new MemoryStream(data)));
        }

        public static BlockIndexEntry Parse(BinaryReader reader)
        {
            byte[] prevHash = new byte[32];

            Tools.ReadBytes(reader, prevHash, 32);
            prevHash.Reverse();

            Int32 height = Tools.ReadInt32LittleEndian(reader);
            UInt32 file = Tools.ReadUInt32LittleEndian(reader);
            UInt32 offset = Tools.ReadUInt32LittleEndian(reader);
            UInt32 size = Tools.ReadUInt32LittleEndian(reader);
            byte status = reader.ReadByte();

            BlockIndexEntry entry = new BlockIndexEntry(prevHash, height, file, offset, size, status);

            return entry;
        }
    }
}

