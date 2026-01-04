namespace BitcoinLib
{
    /// <summary>
    /// Dont remember why we need a class instead of using byte[]
    /// 03.01.2026: removed all usage of this class
    /// </summary>
    public class ByteArray
    {
        private byte[] _bytes;

        private ByteArray(int len)
        {
            _bytes = new byte[len];
        }

        private byte this[int dataIndex]
        {
            get => _bytes[dataIndex];
            set => _bytes[dataIndex] = value;
        }
    }
}

