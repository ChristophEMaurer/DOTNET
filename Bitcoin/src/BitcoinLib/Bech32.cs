using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BitcoinLib
{
    public class Bech32Decoded
    {
        public string Hrp { get; set; }
        public int WitnessVersion { get; set; }
        public byte[] _witnessProgram;
        public string WitnessProgramm { get { return Tools.BytesToHexString(_witnessProgram); } }
        public bool IsBech32m { get; set; }
    }

    public static class Bech32
    {
        //
        // https://slowli.github.io/bech32-buffer/
        //

        const string CHARSET = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";

        private static byte CharToValue(char c)
        {
            int index = CHARSET.IndexOf(c);
            if (index < 0)
                throw new FormatException($"Ungültiges Bech32-Zeichen: '{c}'");
            return (byte)index;
        }

        /// <summary>
        /// Bech32 is used for SegWit addresses
        /// [HRP]1[DATA][CHECKSUM]
        /// [HRP]: bc mainnet | tb testnet
        /// </summary>
        /// <param name="hrp"></param>
        /// <param name="witVer"></param>
        /// <param name="prog"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Encode(string hrp, int witVer, byte[] prog, bool useBech32m)
        {
            if (witVer < 0 || witVer > 16) throw new ArgumentException("Invalid witness version");
            if (prog.Length < 2 || prog.Length > 40) throw new ArgumentException("Invalid witness program length");

            // Witness program von 8-Bit -> 5-Bit
            byte[] prog5 = ConvertBits(prog, 8, 5, true);

            // Daten: 5-Bit Witness-Version + prog5
            int[] data = new int[1 + prog5.Length];
            data[0] = witVer;
            for (int i = 0; i < prog5.Length; i++) data[i + 1] = prog5[i];

            // Checksumme berechnen
            int[] checksum = CreateChecksum(hrp, data, useBech32m);

            // Payload + Checksumme → Bech32-String
            string payload = string.Concat(data.Select(b => CHARSET[b]));
            string check = string.Concat(checksum.Select(b => CHARSET[b]));
            return hrp + "1" + payload + check;
        }

        public static Bech32Decoded Decode(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adresse leer");

            address = address.ToLowerInvariant();

            int sep = address.LastIndexOf('1');
            if (sep < 1 || sep + 7 > address.Length)
                throw new Exception("Ungültige Bech32-Adresse");

            string hrp = address.Substring(0, sep);
            string dataPart = address.Substring(sep + 1);

            // Bech32 charset → 5-bit Werte
            byte[] data = dataPart.Select(CharToValue).ToArray();

            // Checksum prüfen
            int polymod = Polymod(
    HrpExpand(hrp)
        .Concat(data.Select(b => (int)b))
        .ToArray()
);

            bool isBech32;
            bool isBech32m;

            if (polymod == 1)
            {
                isBech32 = true;
                isBech32m = false;
            }
            else if (polymod == 0x2bc830a3)
            {
                isBech32 = false;
                isBech32m = true;
            }
            else
            {
                throw new Exception("Ungültige Checksum");
            }

            // Payload ohne Checksum (letzte 6 Werte)
            byte[] payload = data.Take(data.Length - 6).ToArray();

            int witnessVersion = payload[0];
            if (witnessVersion > 16)
                throw new Exception("Ungültige Witness Version");

            // 5 Bit → 8 Bit
            byte[] witnessProgram =
                ConvertBits(payload.Skip(1).ToArray(), 5, 8, false);

            // Bitcoin-Regeln
            if (witnessProgram.Length < 2 || witnessProgram.Length > 40)
                throw new Exception("Ungültige Witness Program Länge");

            if (witnessVersion == 0 &&
                witnessProgram.Length != 20 &&
                witnessProgram.Length != 32)
                throw new Exception("Ungültiges SegWit v0 Programm");

            if (witnessVersion == 1 && witnessProgram.Length != 32)
                throw new Exception("Ungültiges Taproot Programm");

            // Bech32 vs Bech32m Konsistenz
            if (witnessVersion == 0 && !isBech32)
                throw new Exception("SegWit v0 muss Bech32 sein");

            if (witnessVersion > 0 && !isBech32m)
                throw new Exception("SegWit v1+ muss Bech32m sein");

            return new Bech32Decoded
            {
                Hrp = hrp,
                WitnessVersion = witnessVersion,
                _witnessProgram = witnessProgram,
                IsBech32m = isBech32m
            };
        }


        static byte[] ConvertBits(byte[] data, int fromBits, int toBits, bool pad)
        {
            int acc = 0, bits = 0;
            int maxv = (1 << toBits) - 1;
            var ret = new System.Collections.Generic.List<byte>();

            foreach (var value in data)
            {
                if (value < 0 || (value >> fromBits) != 0) throw new ArgumentException("Invalid value");

                acc = (acc << fromBits) | value;
                bits += fromBits;

                while (bits >= toBits)
                {
                    bits -= toBits;
                    ret.Add((byte)((acc >> bits) & maxv));
                }
            }

            if (pad && bits > 0)
                ret.Add((byte)((acc << (toBits - bits)) & maxv));
            else if (!pad && (bits >= fromBits || ((acc << (toBits - bits)) & maxv) != 0))
                throw new ArgumentException("Invalid padding");

            return ret.ToArray();
        }

        static int[] HrpExpand(string hrp)
        {
            return hrp.Select(c => c >> 5)
                      .Concat(new int[] { 0 })
                      .Concat(hrp.Select(c => c & 31))
                      .ToArray();
        }

        static int[] CreateChecksum(string hrp, int[] data, bool useBech32m)
        {
            // Bech32 vs Bech32m
            int xorConstant = useBech32m ? 0x2bc830a3 : 1;

            int[] values = HrpExpand(hrp).Concat(data).Concat(new int[6]).ToArray();
            int mod = Polymod(values) ^ xorConstant;
            int[] ret = new int[6];
            for (int i = 0; i < 6; i++) 
                ret[i] = (mod >> (5 * (5 - i))) & 31;
            return ret;
        }

        static int Polymod(int[] values)
        {
            int chk = 1;
            foreach (var v in values)
            {
                int top = chk >> 25;
                chk = ((chk & 0x1ffffff) << 5) ^ v;
                if ((top & 1) != 0) chk ^= 0x3b6a57b2;
                if ((top & 2) != 0) chk ^= 0x26508e6d;
                if ((top & 4) != 0) chk ^= 0x1ea119fa;
                if ((top & 8) != 0) chk ^= 0x3d4233dd;
                if ((top & 16) != 0) chk ^= 0x2a1462b3;
            }
            return chk;
        }
    }
}


