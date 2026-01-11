using BitcoinLib.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitcoinLib
{
    /// <summary>
    /// A full Block message is sent from a full node to a client if the client requested data with getdata (without a bloom filter)
    /// A Block is the payload of a network message.
    /// 
    /// to get the full data of block n:
    /// https://blockstream.info/api/block-height/100000 -> blockhash
    /// https://blockstream.info/api/block/{blockhash}/raw
    /// </summary>
    public class Block
    {
        public static byte[] GENESIS_BLOCK_HASH = Tools.HexStringToBytes("000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f");
        public static byte[] GENESIS_BLOCK = Tools.HexStringToBytes("0100000000000000000000000000000000000000000000000000000000000000000000003ba3edfd7a7b12b27ac72c3e67768f617fc81bc3888a51323a9fb8aa4b1e5e4a29ab5f49ffff001d1dac2b7c0101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73ffffffff0100f2052a01000000434104678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5fac00000000");
        public static byte[] TESTNET_GENESIS_BLOCK = Tools.HexStringToBytes("0100000000000000000000000000000000000000000000000000000000000000000000003ba3edfd7a7b12b27ac72c3e67768f617fc81bc3888a51323a9fb8aa4b1e5e4adae5494dffff001d1aa4ae18");
        public static UInt32 LOWEST_BITS = 0x1d00ffff; // 0xffff001d reversed

        public BlockHeader _blockHeader;
        public BlockHeader blockHeader { get { return _blockHeader; } }
        public UInt64 _transactionCount;
        public List<Tx> _transactions;
        public List<Tx> transactions { get { return _transactions; } }


        public Block(BlockHeader blockHeader, UInt64 transactionCount, List<Tx> transactions)
        {
            _blockHeader = blockHeader;
            _transactionCount = transactionCount;
            _transactions = transactions;
        }

        public static Block Parse(string fileName)
        {
            return Parse(new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)));
        }

        public static Block Parse(byte[] raw)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(raw));
            return Parse(reader);
        }

        public static Block Parse(BinaryReader reader)
        {
            List<Tx> transactions = new List<Tx>();

            BlockHeader blockHeader = BlockHeader.Parse(reader);

            UInt64 transactionCount = Tools.ReadVarInt(reader);

            for (UInt64 i = 0; i < transactionCount; i++)
            {
                // Parse each transaction
                Tx tx = Tx.Parse(reader);
                transactions.Add(tx);
            }

            Block block = new Block(blockHeader, transactionCount, transactions);

            // this is for json serialization,we need the block instance
            foreach (Tx tx in transactions)
            {

                // for json serialization, we add block data into the tx
                tx._txStatus = new TxStatus();
                tx._txStatus.block_hash = blockHeader.HashAsString();
                // block_height: the block height is simply the sequence number of a block.
                // first block is 1, second block is 2 etc.
                // we can only know the height if we have all blocks, so we cannot calculate this
                // except for a coinbase Tx, this contains the height in the block
                if (tx.IsCoinbase())
                {
                    tx._txStatus.block_height = tx.CoinbaseHeight();
                }
                tx._txStatus.block_time = blockHeader._timestamp;
            }

            blockHeader.tx_count = transactionCount;
            blockHeader.weight = block.Weight();
            blockHeader.size = block.TotalSize();
            blockHeader.height = block.Height();

            return block;
        }

        public byte[] serialize_total()
        {
            List<byte> data = new List<byte>();
            serialize_total(data);
            return data.ToArray();
        }

        public void serialize_total(List<byte> data)
        {
            _blockHeader.serialize(data);

            Tools.EncodeVarInt(data, _transactionCount);
            foreach (Tx tx in _transactions)
            {
                // this will include segwit data, but only if they exist
                tx.serialize(data);
            }
        }

        public byte[] serialize_stripped()
        {
            List<byte> data = new List<byte>();
            serialize_stripped(data);
            return data.ToArray();
        }

        public void serialize_stripped(List<byte> data)
        {
            _blockHeader.serialize(data);

            Tools.EncodeVarInt(data, _transactionCount);
            foreach (Tx tx in _transactions)
            {
                tx.serialize_legacy(data);
            }
        }

        public void Accept(IBitcoinVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Calculate the size in bytes of the entire tx: this includes segwit bytes if they are present
        /// </summary>
        /// <returns></returns>
        public UInt64 TotalSize()
        {
            byte[] data = serialize_total();
            return (UInt64)data.Length;
        }

        /// <summary>
        /// X-Größe ohne SegWit-Daten (Inputs/Outputs Scriptsig + Scriptpubkey ohne Witness)
        /// </summary>
        /// <returns></returns>
        public UInt64 StrippedSize()
        {
            byte[] data = serialize_stripped();
            return (UInt64)data.Length;
        }

        /// <summary>
        /// Weight = 3 × stripped_size + total_size
        /// Stripped Size = Block ohne Witness
        /// Total Size = Block mit Witness
        /// </summary>
        /// <returns></returns>
        public UInt64 Weight()
        {
            UInt64 weight = StrippedSize() * 3 + TotalSize();

            return weight;
        }

        public int Height()
        {
            int height = _transactions[0].CoinbaseHeight();

            return height;
        }
        public static void PrintBlock(Block block)
        {
            JsonVisitor jsonVisitor = new JsonVisitor();
            block.Accept(jsonVisitor);
            Console.WriteLine("block:");
            Console.WriteLine(jsonVisitor._result);
        }
    }
}
