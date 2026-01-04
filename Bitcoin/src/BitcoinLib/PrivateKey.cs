using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace BitcoinLib
{
    public class PrivateKey
    {
        /// <summary>
        /// The private key is a large natural number: 1..2**256
        /// </summary>
        public BigInteger _secret;

        /// <summary>
        /// The public key.
        /// This is a point on the bitcoin curve
        /// P = e*G
        /// P - public key
        /// e - private key
        /// G - known point on bitcoin curve
        /// </summary>
        public S256Point _point;

        public PrivateKey(BigInteger secret)
        {
            _secret = secret;
            _point = secret * S256Field.G;
        }

        public override string ToString()
        {
            string text = string.Format("{0:x}", _secret);
            return text;
        }

        public string hex()
        {
            return ToString();
        }

        /// <summary>
        /// Sign a message. The message is the hash of some real data.
        /// </summary>
        /// <param name="z">The message</param>
        /// <returns></returns>
        public Signature Sign(BigInteger z)
        {
            BigInteger k = DeterministicK(z);
            
            S256Point R = k * S256Field.G;
            BigInteger r = R._x._num;

            BigInteger k_inv = BigInteger.ModPow(k, S256Field.N - 2, S256Field.N);
            BigInteger s = (z + (r * _secret)) * k_inv;
            s = BigInteger.ModPow(s, 1, S256Field.N);
            if (s > S256Field.N / 2)
            {
                s = S256Field.N - s;
            }

            Signature signature = new Signature(r, s);

            return signature;
        }

        /// <summary>
        /// WIF = Wallet Import Format: this serializes the private key
        /// </summary>
        /// <param name="compressed"></param>
        /// <param name="testnet"></param>
        /// <returns></returns>
        public string Wif(bool compressed, bool testnet)
        {
            byte[] prefix = { testnet ? (byte) 0xef : (byte) 0x80 };
            byte[] suffix;

            if (compressed)
            {
                suffix = new byte[] { 0x01 };
            }
            else
            {
                suffix = new byte[] {};
            }

            byte[] secret_bytes = Tools.ToBytes(_secret, 32, "big");
            byte[] data = ArrayHelpers.ConcatArrays(prefix, secret_bytes, suffix);

            string wif = Base58Encoding.EncodeWithCheckSum(data);
            return wif;
        }


        /// <summary>
        /// TODO: implement DeterminicsticK
        /// https://tools.ietf.org/html/rfc6979
        /// This code was given to me by ChatGPT
        /// </summary>
        /// <param name="z">The message hash</param>
        /// <returns></returns>
        public BigInteger DeterministicK(BigInteger z)
        {
            // todo page 71 
            // implement RFC 6979
            //
            // search for hmac.cs from NBitcoin: "D:\Daten\Develop\crypto\NBitcoin-5.0.68\NBitcoin\BouncyCastle\crypto\macs\HMac.cs"
            //
            // z = z mod n (wie im Python-Code)
            z = (z % S256Field.N + S256Field.N) % S256Field.N;

            // 32-Byte big-endian
            byte[] secret32 = To32BytesBE(_secret);
            byte[] z32 = To32BytesBE(z);

            // RFC6979 Initialisierung
            byte[] V = Enumerable.Repeat((byte)0x01, 32).ToArray();
            byte[] K = Enumerable.Repeat((byte)0x00, 32).ToArray();

            // Step 1
            K = HmacSHA256(K, V.Concat(new byte[] { 0x00 }).Concat(secret32).Concat(z32).ToArray());
            V = HmacSHA256(K, V);

            // Step 2
            K = HmacSHA256(K, V.Concat(new byte[] { 0x01 }).Concat(secret32).Concat(z32).ToArray());
            V = HmacSHA256(K, V);

            // Loop wie im Python-Code
            while (true)
            {
                V = HmacSHA256(K, V);
                BigInteger candidate = FromBigEndian(V);

                if (candidate >= 1 && candidate < S256Field.N)
                    return candidate;

                // retry
                K = HmacSHA256(K, V.Concat(new byte[] { 0x00 }).ToArray());
                V = HmacSHA256(K, V);
            }
        }

        // Hilfsfunktionen
        private static byte[] HmacSHA256(byte[] key, byte[] data)
        {
            using (var h = new HMACSHA256(key))
                return h.ComputeHash(data);
        }

        private static byte[] To32BytesBE(BigInteger v)
        {
            byte[] raw = v.ToByteArray();              // little-endian + sign byte
            if (raw.Length > 33)
                throw new ValueErrorException("BigInteger too large");

            // make positive + big endian + 32 bytes
            byte[] be = raw.TakeWhile(b => true).Reverse().ToArray();
            if (be.Length > 32)
                be = be.Skip(be.Length - 32).ToArray();

            return Enumerable.Repeat((byte)0, 32 - be.Length).Concat(be).ToArray();
        }

        private static BigInteger FromBigEndian(byte[] be)
        {
            return new BigInteger(be.Reverse().Concat(new byte[] { 0 }).ToArray());
        }
    }
}
