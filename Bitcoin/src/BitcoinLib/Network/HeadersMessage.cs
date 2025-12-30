using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Network
{
    public class HeadersMessage : NetworkMessage
    {
        public static string Command = "headers";

        public List<BlockHeader> _blockHeaders;

        public HeadersMessage(List<BlockHeader> blockHeaders) :
            base(HeadersMessage.Command)
        {
            _blockHeaders = blockHeaders;
        }

        public static HeadersMessage Parse(byte[] raw)
        {
            return Parse(new BinaryReader(new MemoryStream(raw)));
        }

        public static HeadersMessage Parse(BinaryReader input)
        {
            UInt64 numHeaders = Tools.ReadVarInt(input);
            List<BlockHeader> blockHeaders = new List<BlockHeader>();

            for (UInt64 i = 0; i < numHeaders; i++)
            {
                BlockHeader header = BlockHeader.Parse(input);
                blockHeaders.Add(header);

                // Read the transaction count (always 0 for headers message)
                UInt64 txCount = Tools.ReadVarInt(input);
                if (txCount != 0)
                {
                    throw new Exception("Invalid headers message: transaction count is not zero but: " + txCount);
                }
            }

            HeadersMessage message = new HeadersMessage(blockHeaders);

            return message;
        }

        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();
            
            Tools.EncodeVarInt(data, (UInt64) _blockHeaders.Count);
            for (int i = 0; i < _blockHeaders.Count; i++)
            {
                BlockHeader header = _blockHeaders[i];
                byte[] headerData = header.serialize();
                data.AddRange(headerData);
  
                // Transaction count (always 0 for headers message)
                Tools.EncodeVarInt(data, 0);
            }

            return data.ToArray();
        }
    }
}

