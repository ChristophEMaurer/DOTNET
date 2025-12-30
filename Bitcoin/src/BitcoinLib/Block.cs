using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitcoinLib
{
    public class Block
    {
        public static byte[] GENESIS_BLOCK = Tools.HexStringToBytes("0100000000000000000000000000000000000000000000000000000000000000000000003ba3edfd7a7b12b27ac72c3e67768f617fc81bc3888a51323a9fb8aa4b1e5e4a29ab5f49ffff001d1dac2b7c");
        public static byte[] TESTNET_GENESIS_BLOCK = Tools.HexStringToBytes("0100000000000000000000000000000000000000000000000000000000000000000000003ba3edfd7a7b12b27ac72c3e67768f617fc81bc3888a51323a9fb8aa4b1e5e4adae5494dffff001d1aa4ae18");
        public static UInt32 LOWEST_BITS = 0x1d00ffff; // 0xffff001d reversed

        public UInt32 _blockSize;
        public BlockHeader _blockHeader;
        public UInt64 _transactionCount;
        public List<Tx> _transactions;

        public Block(UInt32 blockSize, BlockHeader blockHeader, UInt64 transactionCount, List<Tx> transactions)
        {
            _blockSize = blockSize;
            _blockHeader = blockHeader;
            _transactionCount = transactionCount;
            _transactions = transactions;
        }

        public static Block Parse(byte[] rawData)
        {
            List<Tx> transactions = new List<Tx>();

            BinaryReader reader = new BinaryReader(new MemoryStream(rawData));

            UInt32 blockSize = (UInt32)Tools.ReadVarInt(reader);
            BlockHeader blockHeader = BlockHeader.Parse(rawData);
            UInt64 transactionCount = Tools.ReadVarInt(reader);

            for (UInt64 i = 0; i < transactionCount; i++)
            {
                // Parse each transaction
                Tx transaction = Tx.Parse(reader);
                transactions.Add(transaction);
            }

            Block block = new Block(blockSize, blockHeader, transactionCount, transactions);
            
            return block;
        }


    }
}
