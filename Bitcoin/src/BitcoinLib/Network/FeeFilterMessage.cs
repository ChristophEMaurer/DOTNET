
namespace BitcoinLib.Network
{
    public class FeeFilterMessage : NetworkMessage
    {
        public static string Command = "feefilter";

        /// <summary>
        /// 8 byte fee LE
        /// </summary>
        public UInt64 _fee;

        public FeeFilterMessage(UInt64 fee) :
            base(Command)
        {
            _fee = fee;
        }

        public static FeeFilterMessage Parse(byte[] raw)
        {
            return FeeFilterMessage.Parse(new BinaryReader(new MemoryStream(raw)));
        }

        public static FeeFilterMessage Parse(BinaryReader data)
        {
            UInt64 fee = Tools.ReadUInt64LittleEndian(data);

            FeeFilterMessage msg = new FeeFilterMessage(fee);

            return msg;
        }
        
        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            Tools.UIntToLittleEndian(_fee, data, 8);
         
            return data.ToArray();
        }
    }
}

