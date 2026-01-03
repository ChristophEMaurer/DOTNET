using BitcoinLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib
{
    /// <summary>
    /// A MerkleBlock message is sent from a full node to a client if the client requested data with getdata + bloom filter,
    /// the merkle block is the payload of the network message MerkleBlockMessage
    /// </summary>

    public class MerkleBlock
    {
        public BlockHeader _blockHeader;

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

        public MerkleBlock(BlockHeader header, UInt64 total, UInt64 numHashes, byte[][] hashes, byte[] flags)
        {
            _blockHeader = header;
            
            _totalTransactions = total;
            _numHashes = numHashes;
            _hashes = hashes;
            _flags = flags;
        }

        public static MerkleBlock Parse(byte[] input)
        {
            return Parse(new BinaryReader(new MemoryStream(input)));
        }

        public static MerkleBlock Parse(BinaryReader input)
        {
            BlockHeader header = BlockHeader.Parse(input);

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

            MerkleBlock msg = new MerkleBlock(header, totalTransactions, numHashes, hashes, flags);

            return msg;
        }

        public byte[] Serialize()
        {
            throw new NotImplementedException("MerkleBlock.Serialize(): we never send this message!");
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
                    b |= (byte)(bits[bit_index] << j);
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

            MerkleTree tree = new MerkleTree((int)_totalTransactions);

            byte[][] reversedHashes = _hashes.Select(h => h.Reverse().ToArray()).ToArray();

            tree.PopulateTree(bits, reversedHashes);
            Tools.Reverse(tree.Root());

            string calculatedRoot = Tools.BytesToHexString(tree.Root());
            string root = Tools.BytesToHexString(_blockHeader._merkleRoot);

            bool success = calculatedRoot.Equals(root);

            return success;
        }

        /// <summary>
        /// Calculate the Merkle parent of two hashes by concatenating them and hashing the result twice with SHA-256.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static byte[] MerkleParent(byte[] left, byte[] right)
        {
            byte[] concatenated = new byte[left.Length + right.Length];
            Buffer.BlockCopy(left, 0, concatenated, 0, left.Length);
            Buffer.BlockCopy(right, 0, concatenated, left.Length, right.Length);

            byte[] hash = Tools.Hash256(concatenated);
        
            return hash;
        }

        /// <summary>
        /// Computes the parent level of a Merkle tree from an array of hash values.
        /// </summary>
        /// <remarks>If the number of hashes is odd, the last hash is duplicated to form a pair. This
        /// method does not validate the length of individual hash arrays; all hashes are expected to be of the same
        /// length for correct Merkle tree construction.</remarks>
        /// <param name="hashes">An array of byte arrays representing the hash values at the current level of the Merkle tree. Each element
        /// must be a non-null hash of equal length.</param>
        /// <returns>An array of byte arrays containing the parent hashes for the given level. Returns an empty array if the
        /// input array is null or empty.</returns>
        public static byte[][] MerkleParentLevel(byte[][] hashes)
        {
            if (hashes == null || hashes.Length == 0)
                return Array.Empty<byte[]>();

            int count = hashes.Length;

            // Wenn ungerade → letztes Element virtuell duplizieren
            int parentCount = (count + 1) / 2;
            byte[][] parentLevel = new byte[parentCount][];

            int i = 0;
            int p = 0;

            while (i < count)
            {
                byte[] left = hashes[i];
                byte[] right = (i + 1 < count) ? hashes[i + 1] : hashes[i];

                parentLevel[p++] = MerkleParent(left, right);
                i += 2;
            }

            return parentLevel;
        }

        /// <summary>
        /// Computes the Merkle root from an array of hash values.
        /// </summary>
        /// <remarks>The method constructs the Merkle tree by repeatedly combining pairs of hashes until a
        /// single root hash remains. The input hashes are treated as the leaves of the tree.</remarks>
        /// <param name="hashes">An array of byte arrays representing the leaf hashes to be combined into a Merkle tree. The array must not
        /// be null or empty.</param>
        /// <returns>A byte array containing the Merkle root hash computed from the input hashes.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hashes"/> is null or empty.</exception>
        public static byte[] MerkleRoot(byte[][] hashes)
        {
            if (hashes == null || hashes.Length == 0)
                throw new ArgumentException("Hashes array cannot be null or empty.", nameof(hashes));

            byte[][] currentLevel = hashes;
            while (currentLevel.Length > 1)
            {
                currentLevel = MerkleParentLevel(currentLevel);
            }
            return currentLevel[0];
        }
    }
}
