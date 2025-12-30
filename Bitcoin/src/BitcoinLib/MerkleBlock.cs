using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib
{
    public class MerkleBlock
    {
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
