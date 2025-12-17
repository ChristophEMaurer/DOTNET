using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

namespace BitcoinLib
{
    /// <summary>
    /// See Programming bitcoin p81
    /// </summary>
    public class Signature
    {
        public const int SIGHASH_ALL = 1;
        public const int SIGHASH_NONE = 2;
        public const int SIGHASH_SINGLE = 3;


        public BigInteger _r;
        public BigInteger _s;

        public Signature(BigInteger r, BigInteger s)
        {
            _r = r;
            _s = s;
        }

        public override string ToString()
        {
            string text = string.Format("Signature(r={0:x}, s={1:x})", _r, _s);

            return text;
        }
        public static Signature Parse(byte[] input)
        {
            int index = 0;
            byte signature = input[index++];

            if (signature != 0x30)
            {
                throw new ValueErrorException(string.Format("Signature::Parse(): bad signature, read 0x{0:X} instead of 0x30", signature));
            }
            byte length = input[index++];

            // (r, s): r and s could be small, so the total length can be much smaller
            /*if (!allowAllLengthsForTesting)
            {
                if ((length != 0x45) && (length != 0x46))
                {
                    throw new Exception(string.Format("Signature::Parse(): bad length, read '{0}' instead of 0x45 or 0x46", length));
                }
            }*/
            if (length + 2 != input.Length)
            {
                throw new ValueErrorException(string.Format("Signature::Parse(): bad length, read 0x{0:X} but should be 0x{1:X}", length, input.Length - 2));
            }
            byte marker = input[index++];
            if (marker != 0x2)
            {
                throw new ValueErrorException(string.Format("Signature::Parse(): bad marker before r, read {0:X} instead of 0x2", marker));
            }
            byte rlength = input[index++];
            byte[] rbytes = ArrayHelpers.SubArray(input, index, rlength);
            BigInteger r = Tools.BigIntegerFromBytes(rbytes, "big");
            index += rlength;

            marker = input[index++];
            if (marker != 0x2)
            {
                throw new ValueErrorException(string.Format("Signature::Parse(): bad marker before s, read {0:X} instead of 0x2", marker));
            }

            byte slength = input[index++];
            byte[] sbytes = ArrayHelpers.SubArray(input, index, slength);
            BigInteger s = Tools.BigIntegerFromBytes(sbytes, "big");
            index += slength;

            return new Signature(r, s);
        }

        /// <summary>
        /// Serializes the Signature (r, s)
        /// </summary>
        /// <returns>The serialzed (r, s) as a byte array</returns>
        public byte[] Der()
        {
            byte[] result;
            byte[] der_r;
            byte[] der_s;

            byte[] rbin = Tools.ToBytes(_r, 32, "big");
            rbin = Tools.Lstrip(rbin, 0);
            if ((rbin[0] & 0x80) > 0)
            {
                byte[] prefix = { 2, (byte)(rbin.Length + 1), 0 };
                der_r = ArrayHelpers.ConcatArrays(prefix, rbin);
            }
            else
            {
                byte [] prefix = { 2, (byte) rbin.Length };
                der_r = ArrayHelpers.ConcatArrays(prefix, rbin);
            }

            byte[] sbin = Tools.ToBytes(_s, 32, "big");
            sbin = Tools.Lstrip(sbin, 0);
            if ((sbin[0] & 0x80) > 0)
            {
                byte[] prefix = { 2, (byte)(sbin.Length + 1), 0 };
                der_s = ArrayHelpers.ConcatArrays(prefix, sbin);
            }
            else
            {
                byte[] prefix = { 2, (byte)sbin.Length };
                der_s = ArrayHelpers.ConcatArrays(prefix, sbin);
            }

            byte[] prefix2 = { 0x30, (byte)(der_r.Length + der_s.Length) };

            result = ArrayHelpers.ConcatArrays(prefix2, der_r);
            result = ArrayHelpers.ConcatArrays(result, der_s);

            return result;
        }
    }
}
