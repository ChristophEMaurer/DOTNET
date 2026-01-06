using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BitcoinLib
{
    public static class Bech32
    {
        const string CHARSET = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";

        public static string Encode(string hrp, int witVer, byte[] prog)
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
            int[] checksum = CreateChecksum(hrp, data);

            // Payload + Checksumme → Bech32-String
            string payload = string.Concat(data.Select(b => CHARSET[b]));
            string check = string.Concat(checksum.Select(b => CHARSET[b]));
            return hrp + "1" + payload + check;
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

        static int[] CreateChecksum(string hrp, int[] data)
        {
            int[] values = HrpExpand(hrp).Concat(data).Concat(new int[6]).ToArray();
            int mod = Polymod(values) ^ 1;
            int[] ret = new int[6];
            for (int i = 0; i < 6; i++) ret[i] = (mod >> (5 * (5 - i))) & 31;
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


