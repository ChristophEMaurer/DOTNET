using BitcoinLib.Crypto;
using System.Globalization;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BitcoinLib
{
    public class Tools
    {
        /// <summary>
        /// 0 - nothing
        /// 1 - some
        /// 2 - some more
        /// 3 - everything
        /// </summary>
        public static int LOGGING = 0;
        public static bool LOGGING_TIME = false;

        public static object? CallStaticMethod(string strClass, string strFunction, object parameter = null)
        {
            object? returnValue = null;
            bool success = false;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(strClass);
                if (type != null)
                {
                    Type[] paramTypes;
                    object?[] args;

                    if (parameter == null)
                    {
                        paramTypes = Type.EmptyTypes;
                        args = Array.Empty<object>();
                    }
                    else
                    {
                        paramTypes = new[] { parameter.GetType() };
                        args = new[] { parameter };
                    }

                    var method = type.GetMethod(strFunction, BindingFlags.Public | BindingFlags.Static, paramTypes);

                    if (method != null)
                    {
                        if (parameter != null)
                        {
                            Tools.ConsoleOutWriteHeader(string.Format("Tools::CallStaticMethod(): Calling function {0}::{1}({2})", strClass, strFunction, parameter));
                            DateTime start = DateTime.Now;
                            returnValue = method.Invoke(null, args);
                            success = true;
                            DateTime end = DateTime.Now;
                            TimeSpan duration = end - start;
                            if (LOGGING >0)
                            {
                                Console.WriteLine("Time taken: " + duration.ToString(@"hh\:mm\:ss\.fff"));
                            }
                            break;
                        }
                        else
                        {
                            Tools.ConsoleOutWriteHeader(string.Format("Tools::CallStaticMethod(): Calling function {0}::{1}()", strClass, strFunction));
                            DateTime start = DateTime.Now;
                            method.Invoke(null, null);
                            success = true;
                            DateTime end = DateTime.Now;
                            TimeSpan duration = end - start;
                            if (LOGGING_TIME)
                            {
                                Console.WriteLine("Time taken: " + duration.ToString(@"hh\:mm\:ss\.fff"));
                            }
                            break;
                        }
                    }
                }
            }

            if (!success)
            {
                throw new Exception(string.Format("Tools::CallStaticMethod(): Error: Function {0}::{1}({2}) not found!!!", strClass, strFunction, parameter));
            }

            return returnValue;
        }

        /// <summary>
        /// Call a function from a string
        /// </summary>
        /// <param name="strFullyQualifiedName">Full name such as BitcoinLib.OpTest</param>
        /// <returns></returns>
        /*public static object GetInstance(string strFullyQualifiedName)
        {
            Type t = Type.GetType(strFullyQualifiedName);
            return Activator.CreateInstance(t);
        }*/

        /// <summary>
        /// Creates a BigInteger from a hex string that must start with a zero 0 such as
        /// 0eff69ef2b1bd93a66ed5219add4fb51e11a840f404876325a1e8ffe0529a2c
        /// </summary>
        /// <param name="value">Must start with a zero 0</param>
        /// <returns></returns>
        public static BigInteger MakeBigInteger(string value)
        {
            BigInteger bi = BigInteger.Parse(value, NumberStyles.AllowHexSpecifier);
            return bi;
        }

        public static BigInteger MakeBigIntegerBase10(string value)
        {
            BigInteger bi = BigInteger.Parse(value);
            return bi;
        }

        public static UInt16 ReadUInt16LittleEndian(BinaryReader input)
        {
            UInt32 result = 0;

            UInt32 b1 = input.ReadByte();
            UInt32 b2 = input.ReadByte();

            result = (b2 << 8) | b1;

            return (UInt16) result;
        }

        public static UInt32 ReadUInt32LittleEndian(BinaryReader input)
        {
            UInt32 result = 0;

            UInt32 b1 = input.ReadByte();
            UInt32 b2 = input.ReadByte();
            UInt32 b3 = input.ReadByte();
            UInt32 b4 = input.ReadByte();

            result = (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;

            return result;
        }

        public static UInt32 ReadUInt32BigEndian(BinaryReader input)
        {
            UInt32 result = 0;

            UInt32 b1 = input.ReadByte();
            UInt32 b2 = input.ReadByte();
            UInt32 b3 = input.ReadByte();
            UInt32 b4 = input.ReadByte();

            result = (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;

            return result;
        }

        public static UInt64 ReadUInt64LittleEndian(BinaryReader input)
        {
            UInt64 result = 0;

            UInt64 b1 = input.ReadByte();
            UInt64 b2 = input.ReadByte();
            UInt64 b3 = input.ReadByte();
            UInt64 b4 = input.ReadByte();
            UInt64 b5 = input.ReadByte();
            UInt64 b6 = input.ReadByte();
            UInt64 b7 = input.ReadByte();
            UInt64 b8 = input.ReadByte();

            result = (b8 << 56) | (b7 << 48) | (b6 << 40) | (b5 << 32) | (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;

            return result;
        }

        public static byte[] EncodeVarIntToBytes(int value)
        {
            List<byte> data = new List<byte>();

            EncodeVarInt(data, (UInt64)value);

            byte[] bytes = data.ToArray();

            return bytes;
        }


        public static string EncodeVarInt(int value)
        {
            return EncodeVarInt((UInt64)value);
        }

        public static string EncodeVarInt(UInt64 value)
        {
            List<byte> data = new List<byte>();

            EncodeVarInt(data, value);

            string strData = "";

            foreach (byte b in data)
            {
                strData = strData + String.Format("{0:x2}", b);
            }

            return strData;
        }

        public static void EncodeVarInt(List<byte> data, int value)
        {
            EncodeVarInt(data, (UInt64)value);
        }

        public static void EncodeVarInt(List<byte> data, UInt64 value)
        {
            if (value < 0xfd)
            {
                data.Add((byte)value);
            }
            else if (value <= 0xFFFF)
            {
                data.Add(0xFD); // Prefix für 2-Byte Zahl
                byte[] b = BitConverter.GetBytes((ushort)value); // 2 Byte Little Endian
                if (!BitConverter.IsLittleEndian) Array.Reverse(b);
                data.AddRange(b);
            }
            else if (value <= 0xFFFFFFFF)
            {
                data.Add(0xFE); // Prefix für 4-Byte Zahl
                byte[] b = BitConverter.GetBytes((uint)value); // 4 Byte Little Endian
                if (!BitConverter.IsLittleEndian) Array.Reverse(b);
                data.AddRange(b);
            }
            else if (value <= 0xFFFFFFFFFFFFFFFF)
            {
                data.Add(0xFF); // Prefix für 8-Byte Zahl
                byte[] b = BitConverter.GetBytes(value); // 8 Byte Little Endian
                if (!BitConverter.IsLittleEndian) Array.Reverse(b);
                data.AddRange(b);
            }
            else
            {
                throw new Exception("Value is too large for varint: " + value);
            }
        }

        public static UInt64 ReadVarInt(BinaryReader input)
        {
            UInt64 result = 0;

            UInt64 b = input.ReadByte();

            if (b == 0xfd)
            {
                // 2 byte number
                UInt64 b1 = input.ReadByte();
                UInt64 b2 = input.ReadByte();
                result = (b2 << 8) | b1;
            }
            else if (b == 0xfe)
            {
                // 4 byte number
                UInt64 b1 = input.ReadByte();
                UInt64 b2 = input.ReadByte();
                UInt64 b3 = input.ReadByte();
                UInt64 b4 = input.ReadByte();
                result = (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;
            }
            else if (b == 0xff)
            {
                // 8 byte number
                UInt64 b1 = input.ReadByte();
                UInt64 b2 = input.ReadByte();
                UInt64 b3 = input.ReadByte();
                UInt64 b4 = input.ReadByte();
                UInt64 b5 = input.ReadByte();
                UInt64 b6 = input.ReadByte();
                UInt64 b7 = input.ReadByte();
                UInt64 b8 = input.ReadByte();
                result = (b8 << 56) | (b7 << 48) | (b6 << 40) | (b5 << 32) | (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;
            }
            else
            {
                result = b;
            }

            return result;
        }

        public static void ReadBytes(BinaryReader input, byte[] data, int length)
        {
            input.BaseStream.ReadExactly(data, 0, length);
        }

        /// <summary>
        /// Two rounds of SHA256. The hex string is converted to a byte array first.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Returns an array of 32 byte</returns>
        public static byte[] Hash256(string data)
        {
            byte[] bytes = Tools.HexStringToBytes(data);
            return Hash256(bytes);
        }

        /// <summary>
        /// Two rounds of SHA256
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Returns an array of 32 byte</returns>
        public static byte[] Hash256(byte[] data)
        {
            HashAlgorithm alg = System.Security.Cryptography.SHA256.Create();
            byte[] result1 = alg.ComputeHash(data);
            byte[] result2 = alg.ComputeHash(result1);

            return result2;
        }

        /// <summary>
        /// returns the first four bytes of the double SHA256 hash in big endian without reversing anything
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Hash256FirstFourBytes(byte[] data)
        {
            byte[] result1 = Hash256(data);
            byte[] result2 = result1.Take(4).ToArray();

            return result2;
        }
        /*
        /// <summary>
        /// 1 x RIPEMD160
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] RipeMD160_bouncyCastle(byte[] data)
        {
            // Digest erzeugen
            RipeMD160Digest digest = new RipeMD160Digest();

            // Daten einfügen
            digest.BlockUpdate(data, 0, data.Length);

            // Ergebnis auslesen
            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);

            return hash;
        }
        */


        public static byte[] RipeMD160(byte[] data)
        {
            // Digest erzeugen
            RIPEMD160 digest = new RIPEMD160();

            // Daten einfügen
            digest.BlockUpdate(data, 0, data.Length);

            // hash auslesen
            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);

            return hash;
        }


        /// <summary>
        /// 1 x -SHA256
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] SHA256(byte[] data)
        {
            HashAlgorithm alg = System.Security.Cryptography.SHA256.Create();
            byte[] result1 = alg.ComputeHash(data);

            return result1;
        }

        /// <summary>
        /// 1 x SHA1
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] SHA1(byte[] data)
        {
            HashAlgorithm alg = System.Security.Cryptography.SHA1.Create();
            byte[] result1 = alg.ComputeHash(data);

            return result1;
        }

        /// <summary>
        /// RIPEMD160( SHA256( data ) )
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Hash160(byte[] data)
        {
            HashAlgorithm alg1 = System.Security.Cryptography.SHA256.Create();
            byte[] result1 = alg1.ComputeHash(data);


            byte[] result2 = Tools.RipeMD160(result1);

            //HashAlgorithm alg2 = System.Security.Cryptography.RIPEMD160.Create();
            //byte[] result2 = alg2.ComputeHash(result1);

            return result2;
        }

        /// <summary>
        /// Convert a byte array to a hex string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lowerCase"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] data, bool lowerCase = true)
        {
            string text = BitConverter.ToString(data).Replace("-", "");

            if (lowerCase)
            {
                text = text.ToLower();
            }

            return text;
        }

        public static string ByteToBinaryString(byte data, bool prependZeroes = true)
        {
            string text = Convert.ToString(data, 2);

            if (prependZeroes)
            {
                while (text.Length < 8)
                {
                    text = "0" + text;
                }
                text = text.ToLower();
            }

            return text;
        }
        public static string Int32ToBinaryString(Int32[] data)
        {
            string text = "";

            for (int i = 0; i < data.Length; i++)
            {
                string binaryValue = Convert.ToString(data[i], 2);
                while (binaryValue.Length < 11)
                {
                    binaryValue = "0" + binaryValue;
                }
                text = text + binaryValue;
            }

            return text;
        }

        public static byte[] AsciiStringToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        public static byte[] HexStringToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string BigIntegerToHexString(BigInteger n)
        {
            string hex = string.Format("{0:x}", n);

            if (hex.StartsWith("0"))
            {
                hex = hex.Remove(0, 1);
            }
            hex = "0x" + hex;

            return hex;
        }


        /// <summary>
        /// value =  0xaaBBccDD
        /// data =   0  1  2  3
        ///          dd cc bb aa
        /// 01, 1    -> 01
        /// 01, 4    -> 01 00 00 00      /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="data"></param>
        /// <param name="len"></param>
        public static void UIntToLittleEndian(UInt64 value, List<byte> data, int len)
        {
            int count = 0;
            while (value > 0)
            {
                UInt64 b64 = value & 0xff;
                byte b = (byte)b64;

                data.Add(b);
                value >>= 8;
                count++;
            }
            while (count < len)
            {
                data.Add(0);
                count++;
            }
        }

        public static void UInt16ToBigEndian(UInt32 value, List<byte> data)
        {
            data.Add((byte)(value >> 8));
            data.Add((byte)value);
        }

        public static void UInt32ToBigEndian(UInt32 value, List<byte> data)
        {
            data.Add((byte)(value >> 24));
            data.Add((byte)(value >> 16));
            data.Add((byte)(value >> 8));
            data.Add((byte)value);
        }

        /// <summary>
        /// Reverse the bytes in the array. This changes the original!
        /// </summary>
        /// <param name="data"></param>
        public static void Reverse(byte[] data)
        {
            Array.Reverse(data);
        }

        /// <summary>
        /// This creates a reversed copy of the byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The reversed bytes</returns>
        public static byte[] ReverseCopy(byte[] data)
        {
            byte[] reversed = (byte[])data.Clone();
            Array.Reverse(reversed);

            return reversed;
        }



        public static BigInteger Pow(BigInteger x, BigInteger exp)
        {
            BigInteger result;

            if (exp == 0)
            {
                result = 1;
            }
            else
            {
                result = x;
                while (exp > 1)
                {
                    result = BigInteger.Multiply(result, x);
                    exp--;
                }
            }
            return result;
        }

        public static BigInteger GetUnsafeRandom()
        {
            Random random = new Random();
            return random.Next();
        }

        public static BigInteger GetUnsafeRandom(BigInteger max)
        {
            BigInteger rnd = GetUnsafeRandom();
            rnd = BigInteger.ModPow(rnd, 1, max);

            return rnd;
        }

        /// <summary>
        /// (00 00 00 01), 00 -> (01)
        /// (00 00 00), 00 -> ()
        /// (01 02 03), 00 -> (01 02 03)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Lstrip(byte[] data, byte value)
        {
            int start = 0;

            for (start = 0; start < data.Length; start++)
            {
                if (value != data[start])
                {
                    break;
                }
            }

            int len = data.Length - start;
            byte[] stripped = new byte[len];
            for (int dst = 0, src = start; src < data.Length; dst++, src++)
            {
                stripped[dst] = data[src];
            }
            return stripped;
        }

        public static byte[] ToBytes(BigInteger n, int length, string endianness)
        {
            // little, big
            bool big = endianness == "big";
            string text = "";

            while (n > 0)
            {
                byte b = (byte) (n & 0xff);
                string strByte = string.Format("{0,2:x2}", b);

                if (big)
                {
                    text = strByte + text;
                }
                else
                {
                    text = text + strByte;
                }
                if (text.Length > length * 2)
                {
                    throw new IndexOutOfRangeException("int too big to convert");
                }

                n >>= 8;
            }

            while (text.Length < length * 2)
            {
                if (big)
                {
                    text = "00" + text;
                }
                else
                {
                    text = text + "00";
                }
            }

            if (text.Length > length * 2)
            {
                throw new IndexOutOfRangeException("int too big to convert");
            }

            byte[] data = HexStringToBytes(text);

            return data;
        }

        public static BigInteger BigIntegerFromBytes(byte[] data, string endian)
        {
            BigInteger x = 0;

            if (endian == "big")
            {
                for (int i = 0; i < data.Length; i++)
                {
                    x <<= 8;
                    x += data[i];
                }
            }
            else
            {
                for (int i = data.Length - 1; i >= 0; i--)
                {
                    x <<= 8;
                    x += data[i];
                }
            }

            return x;
        }

        private static string EncodeBase58(byte[] data) // TODO delete this function
        {
            // https://gist.github.com/CodesInChaos/3175971
            throw new NotImplementedException("EncodeBase58");
        }

        public static void ConsoleColorBlue()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
        }
        public static void ConsoleColorYellow()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        public static void ConsoleOutWriteHeader(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }
        public static void ConsoleOutWriteWarning(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }

        public static void SerializeLittleEndian(List<byte> list, byte[] data, int length)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                list.Add(data[i]);
            }
        }

        public static void SerializeBigEndian(List<byte> list, byte[] data)
        {
            list.AddRange(data);
        }

        public static byte[] ReadExactly(NetworkStream reader, int count)
        {
            byte[] buffer = new byte[count];
            int offset = 0;

            while (offset < count)
            {
                int read = reader.Read(buffer, offset, count - offset);
                if (read == 0)
                    throw new EndOfStreamException("Stream closed early");

                offset += read;
            }

            return buffer;
        }

        public static void WriteLine(string text)
        {
            Console.Write(DateTime.Now.ToString("HH.mm.ss.fff"));
            Console.Write(":");
            Console.WriteLine(text);
        }
    }
}
