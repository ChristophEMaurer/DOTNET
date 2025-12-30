using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                        treeString += "[" + BitConverter.ToString(hash).Replace("-", string.Empty).Substring(0, 8) + "...] ";
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
        public void Left()
        {
            _currentDepth++;
            _currentIndex *= 2;
        }
        public void Right()
        {
            _currentDepth++;
            _currentIndex = _currentIndex * 2 + 1;
        }

        public byte[] Root()
        {
            return _nodes[0][0];
        }

        public void SetCurentNode(byte[] value)
        {
            _nodes[_currentDepth][_currentIndex] = value;
        }

        public byte[] GetCurrentNode()
        {
            return _nodes[_currentDepth][_currentIndex];
        }

        public byte[] GetLeftNode()
        {
            return _nodes[_currentDepth + 1][_currentIndex * 2];
        }

        public byte[] GetRightNode()
        {
            return _nodes[_currentDepth + 1][_currentIndex * 2 + 1];
        }

        public bool IsLeaf()
        {
            return _currentDepth == _maxDepth;
        }

        public bool HasRightNode()
        {
            return (_currentIndex * 2 + 1) < _nodes[_currentDepth + 1].Length;
        }
    }
}

