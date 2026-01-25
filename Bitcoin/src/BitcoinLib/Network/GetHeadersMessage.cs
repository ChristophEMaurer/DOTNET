using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitcoinLib.Network
{
    /*
    h
    h-1
    h-2
    h-3
    h-4
    h-5
    h-6
    h-7
    h-8
    h-9
    h-10
    h-12
    h-16
    h-24
    h-40
    h-72
    genesis


version 70015 year 2017+
Aktuelle stabile Version (Bitcoin Core 0.15+):
• Unterstützt getheaders
• Unterstützt sendheaders
• Bloom Filters (BIP 37) teilweise deprecated
• Kompatibel mit Compact Blocks (BIP 152)
• Viele kleinere P2P-Fixes
    */
    /// <summary>
    /// this does not use bloom filter
    /// Man übergibt eine Liste von Block-Hashes aus der eigenen besten Chain, 
    /// beginnend beim neuesten Block und dann immer weiter zurück, 
    /// mit exponentiell größer werdenden Abständen.
    /// Ab einem Punkt wird der Abstand jedes Mal verdoppelt.
    /// Maximal 101 Locator Hashes
    /// Reihenfolge: neu → alt
    /// [700000, 699999, 699998, ..., 699990, 699988, 699984, 699976, ..., genesis]
    /// </summary>
    public class GetHeadersMessage : NetworkMessage
    {
        public static string Command = "getheaders";

        private static int HashSize = 32;

        /// <summary>
        /// protocol version, little-endian
        /// </summary>
        public UInt32 _version;

        /// <summary>
        /// number of hashes in _block_locator_hashes, varint
        /// </summary>
        public UInt64 _hashCount;

        /// <summary>
        /// little endian, _hashCount * 32 bytes
        /// stored here in big-endian format, will be reversed on serialization
        /// </summary>
        public byte[] _block_locator_hashes;

        /// <summary>
        /// little endian, 32 bytes
        /// </summary>
        public byte[] _hash_stop;

        public GetHeadersMessage(UInt32 version, UInt64 hashCount,
            byte[] block_locator_hashes,
            byte[] hash_stop = null)
            : base(Command)
        {
            _version = version;
            _hashCount = hashCount;
            _block_locator_hashes = block_locator_hashes;

            if (hashCount > 101)
            {
                throw new ArgumentException($"hashCount must be <= 101, but is {hashCount}");
            }

            if (block_locator_hashes.Length != ((UInt32)hashCount * 32))
            {
                throw new ArgumentException($"block_locator_hashes length must be hashCount * 32 bytes, but hashCount={hashCount}, length is {block_locator_hashes.Length}");
            }

            if (hash_stop == null)
            {
                // create 32 null bytes.
                _hash_stop = new byte[32];
            }
            else
            {
                if (hash_stop.Length != 32)
                {
                    throw new ArgumentException($"hash_stop must be 32 bytes long, but length is {hash_stop.Length}");
                }
                _hash_stop = hash_stop;
            }
        }

        public GetHeadersMessage(byte[] block_locator_hashes) :
            this(VersionMessage.CurrentVersion, (UInt64)(block_locator_hashes.Length / HashSize), block_locator_hashes, null)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            Tools.UIntToLittleEndian(_version, data, 4);
            Tools.EncodeVarInt(data, _hashCount);

            // data = Byte-Array mit N Hashes à 32 Bytes
            int count = _block_locator_hashes.Length / HashSize;

            byte[] leData = new byte[_block_locator_hashes.Length];

            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                // Kopiere 32 Bytes vom originalen Hash in umgekehrter Reihenfolge
                for (int j = 0; j < HashSize; j++)
                {
                    leData[offset + j] = _block_locator_hashes[offset + (HashSize - 1 - j)];
                }
                offset += HashSize;
            }

            data.AddRange(leData);

            byte[] temp = Tools.ReverseCopy(_hash_stop);
            data.AddRange(temp);

            return data.ToArray();
        }
    }
}
