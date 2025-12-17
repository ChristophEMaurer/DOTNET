namespace BitcoinLib
{
    /// <summary>
    /// Dont remember why we need a class instead of using byte[]
    /// </summary>
    public class ByteArray
    {
        public byte[] _bytes;

        public ByteArray(int len)
        {
            _bytes = new byte[len];
        }

        public byte this[int dataIndex]
        {
            get => _bytes[dataIndex];
            set => _bytes[dataIndex] = value;
        }
    }
}

