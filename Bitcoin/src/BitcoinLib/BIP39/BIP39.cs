using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.BIP39
{
    /// <summary>
    /// BIP39 
    /// </summary>
    public class BIP39
    {
        /// <summary>
        /// 2 ^11 = 2048
        /// 
        /// army van defense carry jealous true garbage claim echo media make crunch    12 words
        /// 
        /// 00001100000 11110001001 00111001011 00100010111 01110111100 11101001010     12 x 11 bit = 132 bits
        /// 01011111100 00101001101 01000101111 10001010011 10000110100 00110100111
        /// 
        /// checksum - last 4 bits: 0111
        ///
        /// entropy: all bits except the checksum grouped by bits of 8: 132 bits - 4 bit = 128 bits = 16 bytes
        /// 00001100 00011110 00100100 11100101     0c1e24e5
        /// 10010001 01110111 01111001 11010010     917779d2
        /// 10010111 11100001 01001101 01000101     97e14d45
        /// 11110001 01001110 00011010 00011010     f14e1a1a
        ///
        /// entropy = 0c1e24e5 917779d2 97e14d45 f14e1a1a = 0c1e24e5917779d297e14d45f14e1a1a
        /// </summary>
        /// <param name="mnemonics"></param>
        public static bool MnemonicToEntropy(string mnemonic, out string strEntropy, out string strRelevantChecksumBits)
        {
            // ENT= number of bits of entropy, CS=number of bits of checksum, ENT+CS= number of bits of entropy and checksum, MS=number of words in mnemonic sentence
            //               ENT  CS ENT+CS, MS
            int[] rules = {  128, 4, 132, 12,
                             160, 5, 165, 15,
                             192, 6, 198, 18,
                             224, 7, 231, 21,
                             256, 8, 264, 24
            };
            int[] allowed = { 12, 15, 18, 21, 24 };

            strEntropy = "";
            strRelevantChecksumBits = "";

            string normalizedMnemonic = mnemonic.Normalize(NormalizationForm.FormKD);
            String[] arMnemonics = normalizedMnemonic.Split(new string[] { " " }, StringSplitOptions.None);

            if (!allowed.Contains(arMnemonics.Length))
            {
                // invalid length of words
                return false;
            }
            Wordlist wordlist = new English();

            // data[i] contains the 11 bits of the word at index i
            Int32[] mnemonicData = new Int32[arMnemonics.Length];
            for (int i = 0; i < arMnemonics.Length; i++)
            {
                Int32 index;
                wordlist.WordExists(arMnemonics[i], out index);
                // index is 0..2028 and is 11 bits and is the 11-bit-value of the word
                mnemonicData[i] = index;
            }

            // create bit representation of values with max value 2^11 = 2048
            string strEntropyAndChecksum = Tools.Int32ToBinaryString(mnemonicData);

            // determine number of bits of checksum
            int numberOfchecksumBits = strEntropyAndChecksum.Length / 32;
            string strEntropyBits = strEntropyAndChecksum.Substring(0, strEntropyAndChecksum.Length - numberOfchecksumBits);
            string strEntropyChecksumBits = strEntropyAndChecksum.Substring(strEntropyAndChecksum.Length - numberOfchecksumBits);

            // split bits into groups of 8 to produce hex bytes
            int j = strEntropyBits.Length / 8;
            byte[] hexBytes = new byte[strEntropyBits.Length / 8];
            while (!String.IsNullOrEmpty(strEntropyBits))
            {
                string strByteBinary = strEntropyBits.Substring(strEntropyBits.Length - 8);
                strEntropyBits = strEntropyBits.Substring(0, strEntropyBits.Length - 8);

                hexBytes[--j] = Convert.ToByte(strByteBinary, 2);
            }
            strEntropy = Tools.BytesToHexString(hexBytes);

            byte[] arChecksum = Tools.SHA256(hexBytes);
            string strRelevantChecksumByte = Tools.ByteToBinaryString(arChecksum[0], true);
            strRelevantChecksumBits = strRelevantChecksumByte.Substring(0, numberOfchecksumBits);
            if (strEntropyChecksumBits != strRelevantChecksumBits)
            {
                // checksum does not match
                return false;
            }

            return true;
        }


        /// <summary>
        /// Cal
        /// </summary>
        /// <param name="mnemonic"></param>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public static byte[] MnemonicToSeed(string mnemonic, string passphrase = "")
        {
            // UTF-8 NFKD Normalisierung — in .NET: Normalize auf FormKD
            string normalizedMnemonic = mnemonic.Normalize(NormalizationForm.FormKD);
            string salt = ("mnemonic" + passphrase).Normalize(NormalizationForm.FormKD);

            var pbkdf2 = new Rfc2898DeriveBytes(
                password: normalizedMnemonic,
                salt: Encoding.UTF8.GetBytes(salt),
                iterations: 2048,
                hashAlgorithm: HashAlgorithmName.SHA512
            );

            return pbkdf2.GetBytes(64);  // 64 Bytes = 512 bits
        }
    }
}
