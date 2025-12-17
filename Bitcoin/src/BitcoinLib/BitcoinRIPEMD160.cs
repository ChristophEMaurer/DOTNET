using System;

namespace BitcoinLib.Crypto
{
    public class RIPEMD160
    {
        private const int BlockSize = 64; // 64 Bytes pro Block
        private readonly byte[] _buffer = new byte[BlockSize];
        private int _bufferOffset;
        private long _count;

        private uint[] _h = new uint[5];
        private uint[] _x = new uint[16];

        public RIPEMD160()
        {
            Reset();
        }

        public void Reset()
        {
            _count = 0;
            _bufferOffset = 0;
            _h[0] = 0x67452301;
            _h[1] = 0xEFCDAB89;
            _h[2] = 0x98BADCFE;
            _h[3] = 0x10325476;
            _h[4] = 0xC3D2E1F0;
        }

        public void BlockUpdate(byte[] input, int inOff, int length)
        {
            for (int i = 0; i < length; i++)
            {
                _buffer[_bufferOffset++] = input[inOff + i];
                _count++;
                if (_bufferOffset == BlockSize)
                {
                    ProcessBlock(_buffer, 0);
                    _bufferOffset = 0;
                }
            }
        }

        public int DoFinal(byte[] output, int outOff)
        {
            long bitLength = _count * 8;

            // Padding: 0x80, Nullen bis 56 Bytes, dann Länge
            _buffer[_bufferOffset++] = 0x80;
            if (_bufferOffset > 56)
            {
                while (_bufferOffset < 64) _buffer[_bufferOffset++] = 0;
                ProcessBlock(_buffer, 0);
                _bufferOffset = 0;
            }
            while (_bufferOffset < 56) _buffer[_bufferOffset++] = 0;

            for (int i = 0; i < 8; i++)
                _buffer[_bufferOffset++] = (byte)(bitLength >> (8 * i) & 0xff);

            ProcessBlock(_buffer, 0);

            for (int i = 0; i < 5; i++)
            {
                output[outOff + i * 4] = (byte)(_h[i] & 0xff);
                output[outOff + i * 4 + 1] = (byte)((_h[i] >> 8) & 0xff);
                output[outOff + i * 4 + 2] = (byte)((_h[i] >> 16) & 0xff);
                output[outOff + i * 4 + 3] = (byte)((_h[i] >> 24) & 0xff);
            }

            Reset();
            return 20;
        }

        public int GetDigestSize() => 20;

        private void ProcessBlock(byte[] block, int off)
        {
            // Konvertiere 64 Bytes in 16 32-Bit-Wörter (Little-Endian)
            for (int i = 0; i < 16; i++)
            {
                _x[i] = (uint)(block[off + i * 4] |
                               (block[off + i * 4 + 1] << 8) |
                               (block[off + i * 4 + 2] << 16) |
                               (block[off + i * 4 + 3] << 24));
            }

            uint a = _h[0], b = _h[1], c = _h[2], d = _h[3], e = _h[4];
            uint aa = a, bb = b, cc = c, dd = d, ee = e;

            int[] r = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,
                          7,4,13,1,10,6,15,3,12,0,9,5,2,14,11,8,
                          3,10,14,4,9,15,8,1,2,7,0,6,13,11,5,12,
                          1,9,11,10,0,8,12,4,13,3,7,15,14,5,6,2,
                          4,0,5,9,7,12,2,10,14,1,3,8,11,6,15,13 };

            int[] rp = { 5,14,7,0,9,2,11,4,13,6,15,8,1,10,3,12,
                         6,11,3,7,0,13,5,10,14,15,8,12,4,9,1,2,
                         15,5,1,3,7,14,6,9,11,8,12,2,10,0,4,13,
                         8,6,4,1,3,11,15,0,5,12,2,13,9,7,10,14,
                         12,15,10,4,1,5,8,7,6,2,13,14,0,3,9,11 };

            int[] s = { 11,14,15,12,5,8,7,9,11,13,14,15,6,7,9,8,
                          7,6,8,13,11,9,7,15,7,12,15,9,11,7,13,12,
                          11,13,6,7,14,9,13,15,14,8,13,6,5,12,7,5,
                          11,12,14,15,14,15,9,8,9,14,5,6,8,6,5,12,
                          9,15,5,11,6,8,13,12,5,12,13,14,11,8,5,6 };

            int[] sp = { 8,9,9,11,13,15,15,5,7,7,8,11,14,14,12,6,
                         9,13,15,7,12,8,9,11,7,7,12,7,6,15,13,11,
                         9,7,15,11,8,6,6,14,12,13,5,14,13,13,7,5,
                         15,5,8,11,14,14,6,14,6,9,12,9,12,5,15,8,
                         8,5,12,9,12,5,14,6,8,13,6,5,15,13,11,11 };

            uint[] k = { 0x00000000, 0x5A827999, 0x6ED9EBA1, 0x8F1BBCDC, 0xA953FD4E };
            uint[] kp = { 0x50A28BE6, 0x5C4DD124, 0x6D703EF3, 0x7A6D76E9, 0x00000000 };

            for (int j = 0; j < 80; j++)
            {
                uint t = RotateLeft(a + F(j, b, c, d) + _x[r[j]] + k[j / 16], s[j]) + e;
                a = e; e = d; d = RotateLeft(c, 10); c = b; b = t;

                t = RotateLeft(aa + F(79 - j, bb, cc, dd) + _x[rp[j]] + kp[j / 16], sp[j]) + ee;
                aa = ee; ee = dd; dd = RotateLeft(cc, 10); cc = bb; bb = t;
            }

            uint tTemp = _h[1] + c + dd;
            _h[1] = _h[2] + d + ee;
            _h[2] = _h[3] + e + aa;
            _h[3] = _h[4] + a + bb;
            _h[4] = _h[0] + b + cc;
            _h[0] = tTemp;
        }

        private static uint RotateLeft(uint x, int n) => (x << n) | (x >> (32 - n));

        private static uint F(int j, uint x, uint y, uint z)
        {
            if (j <= 15) return x ^ y ^ z;
            if (j <= 31) return (x & y) | (~x & z);
            if (j <= 47) return (x | ~y) ^ z;
            if (j <= 63) return (x & z) | (y & ~z);
            return x ^ (y | ~z);
        }
    }
}
