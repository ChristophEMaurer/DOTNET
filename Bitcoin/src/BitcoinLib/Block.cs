using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib
{
    public class Block
    {
        public const int TWO_WEEKS = 14 * 24 * 60 * 60; // 1.209.600 seconds <==> 2 weeks

        // MAX_TARGET = 0xFFFF * 256^(0x1D - 3)
        public static readonly BigInteger MAX_TARGET = new BigInteger(0xFFFF) * BigInteger.Pow(256, 0x1D - 3);

        public UInt32 _version;         // LE bytes are reversed when read into this UInt32
        public byte[] _prevBlockHash;   // LE bytes are reversed when read into this byte array
        public byte[] _merkleRoot;      // LE bytes are reversed when read into this byte array
        public UInt32 _timestamp;       // LE bytes are reversed when read into this UInt32
        public UInt32 _bits;            // LE bytes are reversed when read into this UInt32
        public UInt32 _nonce;           // LE bytes are reversed when read into this UInt32

        public Block(UInt32 version, byte[] prev_block, byte[] merkle_root, UInt32 timestamp, UInt32 bits, UInt32 nonce)
        {
            _version = version;
            _prevBlockHash = (byte[])prev_block.Clone();
            _merkleRoot = (byte[])merkle_root.Clone();
            _timestamp = timestamp;
            _bits = bits;
            _nonce = nonce;
        }

        public static Block Parse(byte[] raw)
        {
            return Block.Parse(new BinaryReader(new MemoryStream(raw)));
        }

        public static Block Parse(BinaryReader input)
        {
            UInt32 version = Tools.ReadUInt32LittleEndian(input);

            byte[] prevBlockHash = new byte[32];
            Tools.ReadBytes(input, prevBlockHash, 32);
            Tools.Reverse(prevBlockHash);

            byte[] merkleRoot = new byte[32];
            Tools.ReadBytes(input, merkleRoot, 32);
            Tools.Reverse(merkleRoot);

            UInt32 timestamp = Tools.ReadUInt32LittleEndian(input);
            UInt32 bits = Tools.ReadUInt32LittleEndian(input);
            UInt32 nonce = Tools.ReadUInt32LittleEndian(input);

            Block block = new Block(version, prevBlockHash, merkleRoot, timestamp, bits, nonce);

            return block;
        }

        public byte[] serialize()
        {
            List<byte> data = new List<byte>();
            serialize(data);
            return data.ToArray();
        }

        public void serialize(List<byte> data)
        {
            Tools.UIntToLittleEndian(_version, data, 4);
            Tools.SerializeLittleEndian(data, _prevBlockHash, 32);
            Tools.SerializeLittleEndian(data, _merkleRoot, 32);
            Tools.UIntToLittleEndian(_timestamp, data, 4);
            Tools.UIntToLittleEndian(_bits, data, 4);
            Tools.UIntToLittleEndian(_nonce, data, 4);
        }

        public byte[] Hash()
        {
            byte[] raw = serialize();
            byte[] hash = Tools.Hash256(raw);
            Tools.Reverse(hash);

            return hash;
        }

        public bool Bip9()
        {
            byte b = (byte)(_version >> 29);
            bool success = (b == 0x1);

            return success;
        }

        public bool Bip91()
        {
            byte b = (byte)((_version >> 4) & 1);
            bool success = (b == 0x1);

            return success;
        }
        public bool Bip141()
        {
            byte b = (byte)((_version >> 1) & 1);
            bool success = (b == 0x1);

            return success;
        }

        public BigInteger BitsToTarget()
        {
            return Block.BitsToTarget(_bits);
        }

        /// <summary>
        /// Returns the target based on the bits: target = coefficient * 256 ^ (exponent - 3)
        /// bits = e93c0118 in serialized bytes
        /// as little endian: 18013ce9
        /// 
        /// exponent = first byte       = 18
        /// coefficient = last 3 bytes  = 01 3c e9
        /// 
        /// 0x007FFFFF
        /// 00       7F       FF       FF
        /// 00000000 01111111 11111111 11111111
        ///          |
        ///        bit 24 is the sign, it must always be 0, so we ignore it
        /// </summary>
        /// <returns></returns>
        public static BigInteger BitsToTarget(UInt32 bits)
        {
            byte exponent = (byte)(bits >> 24);
            uint coefficient = bits & 0x007FFFFF; // 23 Bits für Mantisse

            BigInteger target = coefficient * BigInteger.Pow(2, 8 * (exponent - 3));

            return target;
        }

        /// <summary>
        /// This was provided by ChatGPT.
        /// This is the reverse of BitsToTarget: target = coefficient * 256 ^ (exponent - 3)
        /// </summary>
        /// <param name="target"></param>
        /// <returns>the bits in little endian as UInt32. To serialize, the bytes must be reversed</returns>
        public static UInt32 TargetToBits(BigInteger target)
        {
            /*
target_to_bits()
target= 11310011646688221264481072334894212966701871319455105024
raw_bytes= 0000000000000000007615000000000000000000000000000000000000000000
raw_bytes stripped= 7615000000000000000000000000000000000000000000
first bit is NOT 1
exponent= 23 decimal = 0x17
coefficient= 761500
bits= reversed(17761500)
bits= reversed(761500) + 17 -> 00157617 -> this is the way it is serialized
             */
            // Ziel in Big-Endian Bytes
            // bytes will be much bigger than necessary: something like 23 bytes
            byte[] bytes = target.ToByteArray(isUnsigned: true, isBigEndian: true);

            int exponent;
            UInt32 coefficient;

            if (bytes[0] > 0x7F)
            {
                exponent = bytes.Length + 1;
                coefficient = (UInt32)(bytes[0] << 16 | bytes[1] << 8 | bytes[2]);
            }
            else
            {
                exponent = bytes.Length;
                coefficient = (UInt32)(bytes[0] << 16);
                if (bytes.Length > 1)
                    coefficient |= (UInt32)(bytes[1] << 8);
                if (bytes.Length > 2)
                    coefficient |= bytes[2];
            }

            UInt32 bits = ((UInt32)exponent << 24) | coefficient;

            //UInt32 swapped = BinaryPrimitives.ReverseEndianness(bits);

            //
            return bits;
        }

        public BigInteger Target()
        {
            return BitsToTarget();
        }

        /// <summary>
        /// Returns the difficulty based on the bits
        /// </summary>
        /// <returns></returns>
        public double DifficultyAsDouble()
        {
            //# note difficulty is (target of lowest difficulty) / (self's target)
            //# lowest difficulty has bits that equal 0xffff001d

            BigInteger lowest = new BigInteger(0xffff) * BigInteger.Pow(256, 0x1d - 3);

            double difficulty = (double) lowest / (double) BitsToTarget();

            return difficulty;
        }

        /// <summary>
        /// Returns the difficulty based on the bits
        /// </summary>
        /// <returns></returns>
        public BigInteger DifficultyAsBigInteger()
        {
            //# note difficulty is (target of lowest difficulty) / (self's target)
            //# lowest difficulty has bits that equal 0xffff001d

            BigInteger lowest = new BigInteger(0xffff) * BigInteger.Pow(256, 0x1d - 3);

            BigInteger difficulty = lowest / BitsToTarget();

            return difficulty;
        }

        /// <summary>
        /// Check proof of work: the hash of the block must be lower than the target
        /// </summary>
        /// <returns></returns>
        public bool CheckPow()
        {
            bool success;

            byte[] hash = Tools.Hash256(serialize());

            // interpret this hash as a little-endian number
            BigInteger proof = new BigInteger(hash, isUnsigned: true, isBigEndian: false);

            // return whether this integer is less than the target
            success = proof < Target();

            return success;
        }

        /// <summary>
        /// Provided by ChatGPT
        /// </summary>
        /// <param name="previousBits"></param>
        /// <param name="timeDifferential"></param>
        /// <returns>The new Bits in little endian in UInt32. To serialize, the bytes must be reversed</returns>
        public static UInt32 CalculateNewBits(UInt32 previousBits, int timeDifferential)
        {
            // Begrenzen auf [1/4 .. 4] * TWO_WEEKS
            if (timeDifferential > TWO_WEEKS * 4)
                timeDifferential = TWO_WEEKS * 4;

            if (timeDifferential < TWO_WEEKS / 4)
                timeDifferential = TWO_WEEKS / 4;

            // previous target
            BigInteger prevTarget = BitsToTarget(previousBits);

            BigInteger newTarget = prevTarget * timeDifferential / TWO_WEEKS;

            // Cap auf MAX_TARGET
            if (newTarget > MAX_TARGET)
                newTarget = MAX_TARGET;

            // Zurück ins Compact-Format
            UInt32 newBits = TargetToBits(newTarget);

            return newBits;
        }
    }
}
