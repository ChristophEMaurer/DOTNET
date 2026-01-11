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
        /// _prevHash is big endian here, stored as little endian
        /// </summary>
        public byte[] _prevHash;    // 32
        public string prevHash {  get { return Tools.BytesToHexString(_prevHash); } }

        public UInt32 _height;
        public UInt32 height { get { return _height; } }

        public UInt32 _file;        // n -> blk0000n.dat
        public UInt32 file { get { return _file; } }

        public UInt32 _offset;      // offset within the file
        public UInt32 offset { get { return _offset; } }

        public UInt32 _size;        // total size of block in bytes
        public UInt32 size { get { return _size; } }

        public byte _status;
        public byte status { get { return _status; } }

        public BlockIndexEntry(byte[] prevHash, UInt32 height, UInt32 file, UInt32 offset, UInt32 size, byte status)
        {
            _prevHash = prevHash;
            _height = height;
            _file = file;
            _offset = offset;
            _size = size;
            _status = status;
        }

        public byte[] serialize()
        {
            byte[] prevHash_le = (byte[])_prevHash.Clone();
            prevHash_le.Reverse();

            byte[] data = new byte[32 + (4 * 4) + 1]; // 32 + 16 + 1 = 48 + 1 = 49

            prevHash_le.CopyTo(data, 0);
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(32), _height);
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(36), _file);
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

            UInt32 height = Tools.ReadUInt32LittleEndian(reader);
            UInt32 file = Tools.ReadUInt32LittleEndian(reader);
            UInt32 offset = Tools.ReadUInt32LittleEndian(reader);
            UInt32 size = Tools.ReadUInt32LittleEndian(reader);
            byte status = reader.ReadByte();

            BlockIndexEntry entry = new BlockIndexEntry(prevHash, height, file, offset, size, status);

            return entry;
        }
    }
}

