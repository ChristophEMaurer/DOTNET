using System;
using System.IO;
using System.Numerics;
using System.Globalization;
using System.Security.Cryptography;

namespace BitcoinLib
{
    public class S256Point : Point
    {
        public S256Point(BigInteger x, BigInteger y)
            : this(new S256Field(x), new S256Field(y))
        {
        }

        public S256Point(Point p) : base(p)
        {
        }

        public override string ToString()
        {
            string text;

            if (_isInfinity)
            {
                text = string.Format("S256Point(infinity)");
            }
            else
            {
                text = string.Format("S256Point({0},{1})",
                    Tools.BigIntegerToHexString(_x._num),
                    Tools.BigIntegerToHexString(_y._num)
                    );
            }

            return text;
        }

        /// <summary>
        /// A Point in the S256 field with modulo
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public S256Point(S256Field x, S256Field y)
            : base(x, y, new S256Field(S256Field.A), new S256Field(S256Field.B))
        {
        }

        public byte[] Hash160(bool compressed)
        {
            byte[] sec = Sec(compressed);
            byte[] hash = Tools.Hash160(sec);

            return hash;
        }

        /// <summary>
        /// SEC = Standards for Efficient Cryptography
        /// Serialize this point: return the point as a byte array, this can be written to disk or something else
        /// </summary>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public byte[] Sec(bool compressed = true)
        {
            byte[] data;
            byte[] sig = new byte[1];
            byte[] x = Tools.ToBytes(_x._num, 32, "big");

            if (compressed)
            {
                //
                // p 74 compressed SEC format: 
                // P = (Px, Py): Px can be calculated fomr Py
                //
                // marker is 0x02 for odd Py or 0x03 for even Py
                //
                // [0x02] [32 bytes big-endian Px]  if Py is odd
                // [0x03] [32 bytes big-endian Px]  if Py is even
                //
                if (_y._num % 2 == 0)
                {
                    // y is even
                    sig[0] = 0x02;
                    data = ArrayHelpers.ConcatArrays(sig, x);
                }
                else
                {
                    // y is odd
                    sig[0] = 0x03;
                    data = ArrayHelpers.ConcatArrays(sig, x);
                }
            }
            else
            {
                //
                // p 74 uncompressed SEC format: 
                // P = (Px, Py)
                // marker: 0x04
                //
                // [0x04] [32 bytes big-endian Px] [32 bytes big-endian Py]
                //
                byte[] y = Tools.ToBytes(_y._num, 32, "big");

                sig[0] = 0x04;
                data = ArrayHelpers.ConcatArrays(sig, x);
                data = ArrayHelpers.ConcatArrays(data, y);
            }

            return data;
        }

        public static S256Point Parse(string data)
        {
            return S256Point.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(data))));
        }

        public static S256Point Parse(byte[] data)
        {
            return S256Point.Parse(new BinaryReader(new MemoryStream(data)));
        }

        public static S256Point Parse(BinaryReader input)
        {
            byte prefix = input.ReadByte();
            byte[] data;
            S256Point point;

            if (prefix == 4)
            {
                //
                // P = (Px, Py)
                // [0x04] [32 bytes big-endian Px] [32 bytes big-endian Py]
                //
                data = input.ReadBytes(32);
                BigInteger x = Tools.BigIntegerFromBytes(data, "big");
                data = input.ReadBytes(32);
                BigInteger y = Tools.BigIntegerFromBytes(data, "big");

                point = new S256Point(x, y);
            }
            else
            {
                //
                // P = (Px, Py): Px can be calculated fomr Py
                // [0x02] [32 bytes big-endian Px]  if Py is even
                // [0x03] [32 bytes big-endian Px]  if Py is odd
                //

                bool is_even = prefix == 2;
                data = input.ReadBytes(32);
                BigInteger temp = Tools.BigIntegerFromBytes(data, "big");
                S256Field x = new S256Field(temp);
                S256Field alpha = x.OperatorPow(3);
                alpha = alpha + new S256Field(S256Field.B);
                S256Field beta = alpha.Sqrt();

                S256Field even_beta;
                S256Field odd_beta;
                if (beta._num % 2 == 0)
                {
                    even_beta = beta;
                    odd_beta = new S256Field(S256Field.P - beta._num);
                }
                else
                {
                    even_beta = new S256Field(S256Field.P - beta._num);
                    odd_beta = beta;
                }
                if (is_even)
                {
                    point = new S256Point(x, even_beta);
                }
                else
                {
                    point = new S256Point(x, odd_beta);
                }
            }
            return point;
        }

        public string Address(bool compressed, bool testnet)
        {
            byte[] h160 = Hash160(compressed);

            return Base58Encoding.H160To_P2PKH_Address(h160, testnet);
        }

        /// <summary>
        /// The instance of this function is the public key
        /// </summary>
        /// <param name="z"></param>
        /// <param name="sig"></param>
        /// <returns></returns>
        public bool Verify(BigInteger z, Signature sig)
        {
            BigInteger s_inv = BigInteger.ModPow(sig._s, S256Field.N - 2, S256Field.N);

            BigInteger u = BigInteger.ModPow(z * s_inv, 1, S256Field.N);
            BigInteger v = BigInteger.ModPow(sig._r * s_inv, 1, S256Field.N);

            Point R1 = (u * S256Field.G);
            Point R2 = (v * this);
            Point R = R1 + R2;

            bool success = R._x._num == sig._r;

            return success;
        }

        public override Point OperatorMult(BigInteger coefficient)
        {
            BigInteger coef = FieldElement.Mod(coefficient, S256Field.N);

            return base.OperatorMult(coef);
        }

        public static S256Point operator *(BigInteger n, S256Point a)
        {
            Point point = a.OperatorMult(n);
            S256Point s256_point = new S256Point(point);

            return s256_point;
        }
    }
}
