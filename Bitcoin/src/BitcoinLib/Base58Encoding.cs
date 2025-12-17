using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace BitcoinLib
{
    // Implements https://en.bitcoin.it/wiki/Base58Check_encoding
    public static class Base58Encoding
    {
        //
        // Base58 is lower case + upper case + 0..9 without 0/O/l/I
        // 10 + 26 + 26 - 4 = 62 - 4 = 58
        //
        private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private const int CheckSumSizeInBytes = 4;

        /// <summary>
        /// Calculate the hash256 of the data and append the first lower 4 bytes of the checksum to the data
        /// </summary>
        /// <param name="data">data is </param>
        /// <returns></returns>
        private static byte[] AddCheckSum(byte[] data)
        {
#if CONTRACT
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<byte[]>().Length == data.Length + CheckSumSizeInBytes);
#endif
            byte[] checkSum = GetCheckSum(data);
            byte[] dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);
            return dataWithCheckSum;
        }

        /// <summary>
        /// removes the last 4 bytes which belong to the checksum, calculates the hash256 on the remaining data
        /// and checks if the hash256 ends with the supplied 4 bytes
        /// </summary>
        /// <param name="data">data is  [0x00/0x6F] [hash160] [4 bytes checksum]</param>
        /// <returns>Returns null if the checksum is invalid, otherwise returns the  [0x00/0x6F] [hash160] with the checksum removed</returns>
        private static byte[] VerifyAndRemoveCheckSum(byte[] data)
        {
#if CONTRACT

            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<byte[]>() == null || Contract.Result<byte[]>().Length + CheckSumSizeInBytes == data.Length);
#endif
            // data: [0x00/0x6F] [data] [4 bytes checksum]
            // result: [0x00/0x6F] [data]
            byte[] result = ArrayHelpers.SubArray(data, 0, data.Length - CheckSumSizeInBytes);

            // givenCheckSum: [4 bytes checksum]
            byte[] givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CheckSumSizeInBytes);

            // correctCheckSum: the calculated checksum from  [0x00/0x6F] [data]
            byte[] correctCheckSum = GetCheckSum(result);

            if (givenCheckSum.SequenceEqual(correctCheckSum))
                return result;
            else
                return null;
        }

        /// <summary>
        /// removes the last 4 bytes which belong to the checksum, calculates the hash256 on the remaining data
        /// and checks if the hash256 ends with the supplied 4 bytes
        /// </summary>
        /// <param name="data">data is  [0x00/0x6F] [hash160] [4 bytes checksum]</param>
        /// <returns>Returns null if the checksum is invalid, otherwise returns the [hash160] with the checksum and the network prefix removed</returns>
        public static byte[] VerifyAndRemoveNetworkprefixAndCheckSum(string base58Text)
        {
            // data: [0x00/0x6F] [hash160] [4 bytes checksum]
            byte[] data = Decode(base58Text);

            // result: [0x00/0x6F] [hash160]
            byte[] result = ArrayHelpers.SubArray(data, 0, data.Length - CheckSumSizeInBytes);

            // givenCheckSum: [4 bytes checksum]
            byte[] givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CheckSumSizeInBytes);

            // correctCheckSum: the calculated checksum from  [0x00/0x6F] [hash160]
            byte[] correctCheckSum = GetCheckSum(result);

            if (givenCheckSum.SequenceEqual(correctCheckSum))
            {
                byte[] bHash160 = ArrayHelpers.SubArray(data, 1, data.Length - CheckSumSizeInBytes - 1);
                return bHash160;
            }
            else
                return null;
        }

        /// <summary>
        /// Converts a byte array to a byse58 string. String - left to right matches byte array index 0,1,2,3,...
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encode(byte[] data)
        {
#if CONTRACT
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<string>() != null);
#endif

            // Decode byte[] to BigInteger
            BigInteger intData = 0;
            for (int i = 0; i < data.Length; i++)
            {
                intData = (intData << 8) + data[i];
            }

            // Encode BigInteger to Base58 string
            string result = "";
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Prepend `1` for each leading 0 byte
            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

        /// <summary>
        /// Calculates the checksum of data, appends the first 4 bytes of the checksum to data and returns all this as base58
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string EncodeWithCheckSum(byte[] data)
        {
#if CONTRACT
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<string>() != null);
#endif
            return Encode(AddCheckSum(data));
        }

        /// <summary>
        /// Decode a base58 text into a BigInteger and return that BigInteger as a byte array in big endian
        /// /// </summary>
        /// <param name="base58Text"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static byte[] Decode(string base58Text)
        {
#if CONTRACT
            Contract.Requires<ArgumentNullException>(base58Text != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);
#endif
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (int i = 0; i < base58Text.Length; i++)
            {
                int digit = Digits.IndexOf(base58Text[i]); //Slow
                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", base58Text[i], i));
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            int leadingZeroCount = base58Text.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                .Reverse()// to big endian
                .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();

            return result;
        }

        /// <summary>
        /// 
        /// Throws `FormatException` if s is not a valid Base58 string, or the checksum is invalid
        /// </summary>
        /// <param name="base58Text"></param>
        /// <exception cref="FormatException"></exception>
        /// <param name="data">data with network prefix and checksum postfix</param>
        /// <returns>Returns null if the checksum is invalid, otherwise returns the  [0x00/0x6F] [hash160] with the checksum removed</returns>
        public static byte[] DecodeWithCheckSum(string base58Text)
        {
#if CONTRACT
            Contract.Requires<ArgumentNullException>(base58Text != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);
#endif
            var dataWithCheckSum = Decode(base58Text);
            var dataWithoutCheckSum = VerifyAndRemoveCheckSum(dataWithCheckSum);
            if (dataWithoutCheckSum == null)
                throw new FormatException("Base58 checksum is invalid");

            return dataWithoutCheckSum;
        }

        /// <summary>
        /// Calculate the hash256 (=2 x SHA256) of the data and return the first lower 4 bytes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] GetCheckSum(byte[] data)
        {
#if CONTRACT
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Ensures(Contract.Result<byte[]>() != null);
#endif
            SHA256 sha256 = new SHA256Managed();
            byte[] hash1 = sha256.ComputeHash(data);
            byte[] hash2 = sha256.ComputeHash(hash1);

            var result = new byte[CheckSumSizeInBytes];
            Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

            return result;
        }

        /// <summary>
        /// Takes a byte sequence h160 and returns a p2pkh address string
        /// p2pkh has a prefix of 0x00 for mainnet, 0x6f for testnet
        /// test_chapter_8_ex_2
        /// </summary>
        /// <param name="h160"></param>
        /// <param name="testnet"></param>
        /// <returns></returns>
        public static string H160ToP2pkhAddress(byte[] h160, bool testnet = false)
        {
            byte[] data;

            if (testnet)
            {
                data = ArrayHelpers.ConcatArrays(new byte[] { 0x6f }, h160);
            }
            else
            {
                data = ArrayHelpers.ConcatArrays(new byte[] { 0x00 }, h160);
            }

            return Base58Encoding.EncodeWithCheckSum(data);
        }
        /// <summary>
        /// Takes a byte sequence h160 and returns a p2sh address string
        /// p2pkh has a prefix of 0x05 for mainnet, 0xc4 for testnet
        /// test_chapter_8_ex_3
        /// </summary>
        /// <param name="h160"></param>
        /// <param name="testnet"></param>
        /// <returns></returns>
        public static string H160ToP2shAddress(byte[] h160, bool testnet = false)
        {
            byte[] data;

            if (testnet)
            {
                data = ArrayHelpers.ConcatArrays(new byte[] { 0xc4 }, h160);
            }
            else
            {
                data = ArrayHelpers.ConcatArrays(new byte[] { 0x05 }, h160);
            }

            return Base58Encoding.EncodeWithCheckSum(data);
        }
    }
}

