using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitcoinLib.Network
{
    public class MerkleBlockMessage : NetworkMessage
    {
        public static string Command = "merkleblock";

        /// <summary>
        /// 4 bytes, LE
        /// </summary>
        public UInt32 _version;

        /// <summary>
        /// 32 bytes, LE
        /// </summary>
        public byte[] _prev_block;

        /// <summary>
        /// 32 bytes, LE. The hashes must be done on the bytes in LE
        /// </summary>
        public byte[] _merkle_root;

        /// <summary>
        /// 4 bytes, LE
        /// </summary>
        public UInt32 _timestamp;

        /// <summary>
        /// 4 bytes, BE
        /// </summary>
        public byte[] _bits;

        /// <summary>
        /// 4 bytes, BE
        /// </summary>
        public byte[] _nonce;

        /// <summary>
        /// VarInt number of Transactions
        /// </summary>
        public UInt64 _totalTransactions;

        public UInt64 _numHashes;

        /// <summary>
        /// hashes, 32 byte each, LE. The hashes must be calculated on the LE bytes
        /// </summary>
        public byte[][] _hashes;

        /// <summary>
        /// Flag bits coded in bytes, these bytes start with one VarInt that indicates how many flag bytes follow
        /// </summary>
        public byte[] _flags;


        public MerkleBlockMessage(UInt32 version, byte[] prev_block, byte[] merkle_root, UInt32 timestamp,
            byte[] bits, byte[] nonce, UInt64 total, UInt64 numHashes, byte[][] hashes, byte[] flags) :
            base(MerkleBlockMessage.Command)
        {
            _version = version;
            _prev_block = prev_block;
            _merkle_root = merkle_root;
            _timestamp = timestamp;
            _bits = bits;
            _nonce = nonce;
            _totalTransactions = total;
            _numHashes = numHashes;
            _hashes = hashes;
            _flags = flags;
        }


        public static MerkleBlockMessage Parse(byte[] input)
        {
            return Parse(new BinaryReader(new MemoryStream(input)));
        }

        public static MerkleBlockMessage Parse(BinaryReader input)
        {
            UInt32 version = Tools.ReadUInt32LittleEndian(input);
            
            byte[] prev_block = new byte[32];
            Tools.ReadBytes(input, prev_block, 32);
            Tools.Reverse(prev_block);

            byte[] merkle_root = new byte[32];
            Tools.ReadBytes(input, merkle_root, 32);
            Tools.Reverse(merkle_root);
 
            UInt32 timestamp = Tools.ReadUInt32LittleEndian(input);

            byte[] bits = new byte[4];
            Tools.ReadBytes(input, bits, 4);

            byte[] nonce = new byte[4];
            Tools.ReadBytes(input, nonce, 4);

            UInt32 totalTransactions = Tools.ReadUInt32LittleEndian(input);

            UInt64 numHashes = Tools.ReadVarInt(input);
            byte[][] hashes = new byte[numHashes][];
            for (UInt64 i = 0; i < numHashes; i++)
            {
                byte[] hash = new byte[32];
                Tools.ReadBytes(input, hash, 32);
                Tools.Reverse(hash);
                hashes[i] = hash;
            }

            UInt64 flagsLength = Tools.ReadVarInt(input);
            byte[] flags = new byte[flagsLength];
            Tools.ReadBytes(input, flags, (int)flagsLength);

            MerkleBlockMessage msg = new MerkleBlockMessage(version, prev_block, merkle_root, timestamp,
                bits, nonce, totalTransactions, numHashes, hashes, flags);

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


        /// <summary>
        /// Returns the flags as bits (LSB first). The bits in each byte are ordered from least significant to most significant.
        /// [0x3d 0x43]= 00111101 01000011= [1,0,1,1,1,1,0,0,   1,1,0,0,0,0,1,0]
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>Each bbyte is either 0 or 1</returns>
        public static byte[] FlagsToBitField(byte[] flags)
        {
            List<byte> bits = new List<byte>();

            // read bytes sequentiallyleft to right
            for (int i = 0; i < flags.Length; i++)
            {
                byte b = flags[i];
                {
                    // take the bits from LSB to MSB and add them to the list, this reverses the order within each byte
                    for (int j = 0; j < 8; j++)
                    {
                        bits.Add((byte)(b & 1)); // LSB first
                        b >>= 1;
                    }
                }
            }

            return bits.ToArray();
        }

        /// <summary>
        /// returns a byte array where each byte represents 8 bits from the input bit array.
        /// [0x3d 0x43]= 00111101 01000011= [1,0,1,1,1,1,0,0,   1,1,0,0,0,0,1,0]
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] BitFieldToFlags(byte[] bits)
        {
            if (bits.Length % 8 != 0)
            {
                throw new ArgumentException("BitFieldToFlags: bits length must be multiple of 8:" + bits.Length);
            }

            byte[] flags = new byte[bits.Length / 8];

            // index into flag array for return values
            int byte_index = 0;

            // index into bit array for input bits
            int bit_index = 0;

            for (int i = 0; i < bits.Length; i += 8)
            {
                byte b = 0;
                for (int j = 0; j < 8; j++)
                {
                    b |= (byte) (bits[bit_index] << j);
                    bit_index++;
                }

                flags[byte_index] = b;
                byte_index++;
            }

            return flags;
        }

        /// <summary>
        /// Verifies whether the merkle tree information validates to the merkle root
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            byte[] bits = FlagsToBitField(_flags);

            MerkleTree tree = new MerkleTree((int) _totalTransactions);

            byte[][] reversedHashes = _hashes.Select(h => h.Reverse().ToArray()).ToArray();

            tree.PopulateTree(bits, reversedHashes);
            Tools.Reverse(tree.Root());

            string calculatedRoot = Tools.BytesToHexString(tree.Root());
            string root = Tools.BytesToHexString(_merkle_root);

            bool success = calculatedRoot.Equals(root);

            return success;
        }
    }
}

