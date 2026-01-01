using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// 31.12.2025 16:27 this text was typed on the branew new keybaord Logitech Signature slim sollar+ K980

namespace BitcoinLib
{
    public class MerkleTree
    {
        public byte[][][] _nodes;
        public int _total;
        public int _maxDepth;
        public int _currentDepth;
        public int _currentIndex;

        /// <summary>
        /// Initializes a new instance of the MerkleTree class with the specified total number of leaf nodes.
        /// </summary>
        /// <remarks>The tree is constructed to accommodate the specified number of leaves, with internal
        /// structure sized accordingly. The depth and node allocation are determined based on the total count, ensuring
        /// the tree is balanced for efficient operations.</remarks>
        /// <param name="total">The total number of leaf nodes to include in the Merkle tree. Must be greater than zero.</param>
        public MerkleTree(int total)
        {
            _total = total;

            // compute max depth
            _maxDepth = (int)Math.Ceiling(Math.Log(total, 2));

            // initialize the nodes property to hold the actual tree
            _nodes = new byte[_maxDepth + 1][][];

            for (int depth = 0; depth <= _maxDepth; depth++)
            {
                // number of items at this depth
                int numItems = (int)Math.Ceiling((double)total / Math.Pow(2, _maxDepth - depth));

                // create this level's hashes list with the right number of items
                byte[][] levelHashes = new byte[numItems][];

                // append this level's hashes to the merkle tree
                _nodes[depth] = levelHashes;
            }

            // set the pointer to the root (depth=0, index=0)
            _currentDepth = 0;
            _currentIndex = 0;
        }

        public override string ToString()
        {
            string treeString = string.Empty;

            foreach (byte[][] level in _nodes)
            {
                treeString += $"{level.Length,4}: ";

                foreach (byte[] hash in level)
                {
                    if (hash != null)
                    {
                        treeString += "[" + BitConverter.ToString(hash).Replace("-", string.Empty).Substring(0, 8).ToLower() + "...] ";
                    }
                    else
                    {
                        treeString += "[null] ";
                    }
                }
                treeString += Environment.NewLine;
            }

            return treeString;
        }

        public void Up()
        {
            _currentDepth--;
            _currentIndex /= 2;
        }

        /// <summary>
        /// Moves the current position to the left child node in the binary tree traversal.
        /// </summary>
        public void Left()
        {
            _currentDepth++;
            _currentIndex *= 2;
        }

        /// <summary>
        /// Advances the current position to the right child node in a binary tree traversal.
        /// </summary>
        public void Right()
        {
            _currentDepth++;
            _currentIndex = _currentIndex * 2 + 1;
        }

        /// <summary>
        /// Gets the root node value as a byte array.
        /// </summary>
        /// <returns>A byte array containing the value of the root node.</returns>
        public byte[] Root()
        {
            byte[] node = _nodes[0][0];

            return node;
        }

        /// <summary>
        /// Sets the current node to the specified byte array value.
        /// </summary>
        /// <param name="value">The byte array to assign to the current node. Cannot be null.</param>
        public void SetCurentNode(byte[] value)
        {
            _nodes[_currentDepth][_currentIndex] = value;
        }

        /// <summary>
        /// Retrieves the data for the current node in the traversal.
        /// </summary>
        /// <returns>A byte array containing the data of the current node.</returns>
        public byte[] GetCurrentNode()
        {
            byte[] node = _nodes[_currentDepth][_currentIndex];

            return node;
        }

        /// <summary>
        /// Retrieves the left child node of the current node in the binary tree. The current position is not changed.
        /// </summary>
        /// <returns></returns>
        public byte[] GetLeftNode()
        {
            byte[] node = _nodes[_currentDepth + 1][_currentIndex * 2];

            return node;
        }

        /// <summary>
        /// Retrieves the byte array representing the right child node at the current position in the tree structure. The current position is not changed.
        /// </summary>
        /// <returns>A byte array containing the data of the right child node at the current depth and index.</returns>
        public byte[] GetRightNode()
        {
            byte[] node = _nodes[_currentDepth + 1][_currentIndex * 2 + 1];

            return node;
        }

        /// <summary>
        /// Determines whether the current node is a leaf node based on its depth.
        /// </summary>
        /// <returns>true if the current node is at the maximum depth and has no child nodes; otherwise, false.</returns>
        public bool IsLeaf()
        {
            bool isLeaf = _currentDepth == _maxDepth;

            return isLeaf;
        }

        /// <summary>
        /// Determines whether the current node has a right child node.
        /// </summary>
        /// <returns>true if the current node has a right child node; otherwise, false.</returns>
        public bool HasRightNode()
        {
            bool hasRightNode = (_currentIndex * 2 + 1) < _nodes[_currentDepth + 1].Length;

            return hasRightNode;
        }

        public void PopulateTree(byte[] bits, byte[][] hashes)
        {
            // populate until we have the root
            while (Root() == null)
            {
                if (IsLeaf())
                {
                    // if we are a leaf, we know this position's hash
                    // the bit is either 0 or one, but for a leaf, the hash is always present
                    byte bit = bits[0];
                    bits = bits[1..];
                    byte[] hash = hashes[0];
                    hashes = hashes[1..];

                    SetCurentNode(hash);
                    Up();
                }
                else
                {
                    byte[] leftHash = GetLeftNode();
                    if (leftHash == null)
                    {
                        // if the next flag bit is 0, the next hash is our current node
                        byte bit = bits[0];
                        bits = bits[1..];
                        if (bit == 0)
                        {
                            byte[] hash = hashes[0];
                            hashes = hashes[1..];
                            SetCurentNode(hash);
                            // sub-tree doesn't need calculation, go up
                            Up();
                        }
                        else
                        {
                            Left();
                        }
                    }
                    else if (this.HasRightNode())
                    {
                        byte[] rightHash = GetRightNode();
                        if (rightHash == null)
                        {
                            Right();
                        }
                        else
                        {
                            SetCurentNode(MerkleBlock.MerkleParent(leftHash, rightHash));
                            // we've completed this sub-tree, go up
                            Up();
                        }
                    }
                    else
                    {
                        SetCurentNode(MerkleBlock.MerkleParent(leftHash, leftHash));
                        // we've completed this sub-tree, go up
                        Up();
                    }
                }
            }

            if (hashes.Length != 0)
            {
                throw new Exception("Merkle tree population error: unused hashes remain after populating the tree: " + hashes.Length);
            }
            foreach (byte b in bits)
            {
                if (b != 0)
                {
                    throw new Exception("Merkle tree population error: unused flag bits with value 1 remain after populating the tree.");
                }
            }   
        }
    }
}

