using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib;
using BitcoinLib.Test;

namespace BitcoinLib.Test
{
    public class MerkleTreeTest : UnitTest
    {
        public static void test_chapter_11_ex_5()
        {
            MerkleTree tree = new MerkleTree(27);

            Console.WriteLine(tree.ToString());
        }

        public static void test_merkle_16()
        {
            MerkleTree tree = new MerkleTree(16);

            AssertEqual(tree._nodes.Length, 5);

            AssertEqual(tree._nodes[0].Length, 1);
            AssertEqual(tree._nodes[1].Length, 2);
            AssertEqual(tree._nodes[2].Length, 4);
            AssertEqual(tree._nodes[3].Length, 8);
            AssertEqual(tree._nodes[4].Length, 16);

            Console.WriteLine(tree.ToString());
        }

        public static void test_merkle_9()
        {
            MerkleTree tree = new MerkleTree(9);

            AssertEqual(tree._nodes.Length, 5);

            AssertEqual(tree._nodes[0].Length, 1);
            AssertEqual(tree._nodes[1].Length, 2);
            AssertEqual(tree._nodes[2].Length, 3);
            AssertEqual(tree._nodes[3].Length, 5);
            AssertEqual(tree._nodes[4].Length, 9);

            Console.WriteLine(tree.ToString());
        }

        public static void test_merkle_tree_p201()
        {
            // 16 lines
            string[] strHashes = [
                "9745f7173ef14ee4155722d1cbf13304339fd00d900b759c6f9d58579b5765fb",
                "5573c8ede34936c29cdfdfe743f7f5fdfbd4f54ba0705259e62f39917065cb9b",
                "82a02ecbb6623b4274dfcab82b336dc017a27136e08521091e443e62582e8f05",
                "507ccae5ed9b340363a0e6d765af148be9cb1c8766ccc922f83e4ae681658308",
                "a7a4aec28e7162e1e9ef33dfa30f0bc0526e6cf4b11a576f6c5de58593898330",
                "bb6267664bd833fd9fc82582853ab144fece26b7a8a5bf328f8a059445b59add",
                "ea6d7ac1ee77fbacee58fc717b990c4fcccf1b19af43103c090f601677fd8836",
                "457743861de496c429912558a106b810b0507975a49773228aa788df40730d41",
                "7688029288efc9e9a0011c960a6ed9e5466581abf3e3a6c26ee317461add619a",
                "b1ae7f15836cb2286cdd4e2c37bf9bb7da0a2846d06867a429f654b2e7f383c9",
                "9b74f89fa3f93e71ff2c241f32945d877281a6a50a6bf94adac002980aafe5ab",
                "b3a92b5b255019bdaf754875633c2de9fec2ab03e6b8ce669d07cb5b18804638",
                "b5c0b915312b9bdaedd2b86aa2d0f8feffc73a2d37668fd9010179261e25e263",
                "c9d52c5cb1e557b92c84c52e7c4bfbce859408bedffc8a5560fd6e35e10b8800",
                "c555bc5fc3bc096df0a0c9532f07640bfb76bfe4fc1ace214b8b228a1297a4c2",
                "f9dbfafc3af3400954975da24eb325e326960a25b87fffe23eef3e7ed2fb610e",
            ];
            byte[][] hashes = strHashes.Select(s => Tools.HexStringToBytes(s)).ToArray();

            MerkleTree tree = new MerkleTree(strHashes.Length);

            tree._nodes[4] = hashes;
            tree._nodes[3] = MerkleBlock.MerkleParentLevel(tree._nodes[4]);
            tree._nodes[2] = MerkleBlock.MerkleParentLevel(tree._nodes[3]);
            tree._nodes[1] = MerkleBlock.MerkleParentLevel(tree._nodes[2]);
            tree._nodes[0] = MerkleBlock.MerkleParentLevel(tree._nodes[1]);

            string strTree = tree.ToString();
            Console.WriteLine(strTree);
        }

        /// <summary>
        /// This code only works because the number of leaves is a power of 2.
        /// </summary>
        public static void test_merkle_tree_p203()
        {
            // 16 lines
            string[] strHashes = [
                "9745f7173ef14ee4155722d1cbf13304339fd00d900b759c6f9d58579b5765fb",
                "5573c8ede34936c29cdfdfe743f7f5fdfbd4f54ba0705259e62f39917065cb9b",
                "82a02ecbb6623b4274dfcab82b336dc017a27136e08521091e443e62582e8f05",
                "507ccae5ed9b340363a0e6d765af148be9cb1c8766ccc922f83e4ae681658308",
                "a7a4aec28e7162e1e9ef33dfa30f0bc0526e6cf4b11a576f6c5de58593898330",
                "bb6267664bd833fd9fc82582853ab144fece26b7a8a5bf328f8a059445b59add",
                "ea6d7ac1ee77fbacee58fc717b990c4fcccf1b19af43103c090f601677fd8836",
                "457743861de496c429912558a106b810b0507975a49773228aa788df40730d41",
                "7688029288efc9e9a0011c960a6ed9e5466581abf3e3a6c26ee317461add619a",
                "b1ae7f15836cb2286cdd4e2c37bf9bb7da0a2846d06867a429f654b2e7f383c9",
                "9b74f89fa3f93e71ff2c241f32945d877281a6a50a6bf94adac002980aafe5ab",
                "b3a92b5b255019bdaf754875633c2de9fec2ab03e6b8ce669d07cb5b18804638",
                "b5c0b915312b9bdaedd2b86aa2d0f8feffc73a2d37668fd9010179261e25e263",
                "c9d52c5cb1e557b92c84c52e7c4bfbce859408bedffc8a5560fd6e35e10b8800",
                "c555bc5fc3bc096df0a0c9532f07640bfb76bfe4fc1ace214b8b228a1297a4c2",
                "f9dbfafc3af3400954975da24eb325e326960a25b87fffe23eef3e7ed2fb610e",
            ];
            byte[][] hashes = strHashes.Select(s => Tools.HexStringToBytes(s)).ToArray();

            MerkleTree tree = new MerkleTree(strHashes.Length);
            tree._nodes[4] = hashes;

            while (tree.Root() == null)
            {
                if (tree.IsLeaf())
                {
                    tree.Up();
                }
                else
                {
                    byte[] left_hash = tree.GetLeftNode();
                    byte[] right_hash = tree.GetRightNode();
                    if (left_hash == null)
                    {
                        tree.Left();
                    }
                    else if (right_hash == null)
                    {
                        tree.Right();
                    }
                    else 
                    {
                        tree.SetCurentNode(MerkleBlock.MerkleParent(left_hash, right_hash));
                        tree.Up();
                    }
                }
            }

            string strTree = tree.ToString();
            Console.WriteLine(strTree);
        }

        /// <summary>
        /// This code only works if if the number of leaves is not a power of 2.
        /// </summary>
        public static void test_merkle_tree_p204()
        {
            // 17 lines
            string[] strHashes = [
                "9745f7173ef14ee4155722d1cbf13304339fd00d900b759c6f9d58579b5765fb",
                "5573c8ede34936c29cdfdfe743f7f5fdfbd4f54ba0705259e62f39917065cb9b",
                "82a02ecbb6623b4274dfcab82b336dc017a27136e08521091e443e62582e8f05",
                "507ccae5ed9b340363a0e6d765af148be9cb1c8766ccc922f83e4ae681658308",
                "a7a4aec28e7162e1e9ef33dfa30f0bc0526e6cf4b11a576f6c5de58593898330",
                "bb6267664bd833fd9fc82582853ab144fece26b7a8a5bf328f8a059445b59add",
                "ea6d7ac1ee77fbacee58fc717b990c4fcccf1b19af43103c090f601677fd8836",
                "457743861de496c429912558a106b810b0507975a49773228aa788df40730d41",
                "7688029288efc9e9a0011c960a6ed9e5466581abf3e3a6c26ee317461add619a",
                "b1ae7f15836cb2286cdd4e2c37bf9bb7da0a2846d06867a429f654b2e7f383c9",
                "9b74f89fa3f93e71ff2c241f32945d877281a6a50a6bf94adac002980aafe5ab",
                "b3a92b5b255019bdaf754875633c2de9fec2ab03e6b8ce669d07cb5b18804638",
                "b5c0b915312b9bdaedd2b86aa2d0f8feffc73a2d37668fd9010179261e25e263",
                "c9d52c5cb1e557b92c84c52e7c4bfbce859408bedffc8a5560fd6e35e10b8800",
                "c555bc5fc3bc096df0a0c9532f07640bfb76bfe4fc1ace214b8b228a1297a4c2",
                "f9dbfafc3af3400954975da24eb325e326960a25b87fffe23eef3e7ed2fb610e",
                "38faf8c811988dff0a7e6080b1771c97bcc0801c64d9068cffb85e6e7aacaf51",
            ];
            byte[][] hashes = strHashes.Select(s => Tools.HexStringToBytes(s)).ToArray();

            MerkleTree tree = new MerkleTree(strHashes.Length);
            tree._nodes[5] = hashes;

            while (tree.Root() == null)
            {
                if (tree.IsLeaf())
                {
                    tree.Up();
                }
                else
                {
                    byte[] left_hash = tree.GetLeftNode();

                    if (left_hash == null)
                    {
                        tree.Left();
                    }
                    else if (tree.HasRightNode())
                    {
                        byte[] right_hash = tree.GetRightNode();
                        if (right_hash == null)
                        {
                            tree.Right();
                        }
                        else
                        {
                            tree.SetCurentNode(MerkleBlock.MerkleParent(left_hash, right_hash));
                            tree.Up();
                        }
                    }
                    else
                    {
                        tree.SetCurentNode(MerkleBlock.MerkleParent(left_hash, left_hash));
                        tree.Up();
                    }
                }
            }

            string strTree = tree.ToString();
            Console.WriteLine(strTree);
        }

        public static void test_merkle_tree_populate_1()
        {
            string[] strHashes = [
                "9745f7173ef14ee4155722d1cbf13304339fd00d900b759c6f9d58579b5765fb",
                "5573c8ede34936c29cdfdfe743f7f5fdfbd4f54ba0705259e62f39917065cb9b",
                "82a02ecbb6623b4274dfcab82b336dc017a27136e08521091e443e62582e8f05",
                "507ccae5ed9b340363a0e6d765af148be9cb1c8766ccc922f83e4ae681658308",
                "a7a4aec28e7162e1e9ef33dfa30f0bc0526e6cf4b11a576f6c5de58593898330",
                "bb6267664bd833fd9fc82582853ab144fece26b7a8a5bf328f8a059445b59add",
                "ea6d7ac1ee77fbacee58fc717b990c4fcccf1b19af43103c090f601677fd8836",
                "457743861de496c429912558a106b810b0507975a49773228aa788df40730d41",
                "7688029288efc9e9a0011c960a6ed9e5466581abf3e3a6c26ee317461add619a",
                "b1ae7f15836cb2286cdd4e2c37bf9bb7da0a2846d06867a429f654b2e7f383c9",
                "9b74f89fa3f93e71ff2c241f32945d877281a6a50a6bf94adac002980aafe5ab",
                "b3a92b5b255019bdaf754875633c2de9fec2ab03e6b8ce669d07cb5b18804638",
                "b5c0b915312b9bdaedd2b86aa2d0f8feffc73a2d37668fd9010179261e25e263",
                "c9d52c5cb1e557b92c84c52e7c4bfbce859408bedffc8a5560fd6e35e10b8800",
                "c555bc5fc3bc096df0a0c9532f07640bfb76bfe4fc1ace214b8b228a1297a4c2",
                "f9dbfafc3af3400954975da24eb325e326960a25b87fffe23eef3e7ed2fb610e"
            ];
            byte[][] hashes = strHashes.Select(s => Tools.HexStringToBytes(s)).ToArray();

            MerkleTree tree = new MerkleTree(strHashes.Length);
            byte[] bits = Enumerable.Repeat((byte)1, 31).ToArray();
            tree.PopulateTree(bits, hashes);

            string want = "597c4bafe3832b17cbbabe56f878f4fc2ad0f6a402cee7fa851a9cb205f87ed1";
            string root = Tools.BytesToHexString(tree.Root());
            AssertEqual(want, root);
        }

        public static void test_merkle_tree_populate_2()
        {
            string[] strHashes = [
                "42f6f52f17620653dcc909e58bb352e0bd4bd1381e2955d19c00959a22122b2e",
                "94c3af34b9667bf787e1c6a0a009201589755d01d02fe2877cc69b929d2418d4",
                "959428d7c48113cb9149d0566bde3d46e98cf028053c522b8fa8f735241aa953",
                "a9f27b99d5d108dede755710d4a1ffa2c74af70b4ca71726fa57d68454e609a2",
                "62af110031e29de1efcad103b3ad4bec7bdcf6cb9c9f4afdd586981795516577"
            ];
            byte[][] hashes = strHashes.Select(s => Tools.HexStringToBytes(s)).ToArray();

            MerkleTree tree = new MerkleTree(strHashes.Length);
            byte[] bits = Enumerable.Repeat((byte)1, 11).ToArray();
            tree.PopulateTree(bits, hashes);

            string want = "a8e8bd023169b81bc56854137a135b97ef47a6a7237f4c6e037baed16285a5ab";
            string root = Tools.BytesToHexString(tree.Root());
            AssertEqual(want, root);
        }
    }
}
