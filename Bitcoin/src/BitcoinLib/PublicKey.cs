using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib
{
    public class PublicKey
    {
        public string DecodePublicKey(string hex)
        {
            string result = "unknown address type";

            if ("04 03 02".Contains(hex.Substring(0, 2)))
            {
                byte[] bHex = Tools.HexStringToBytes(hex);
                S256Point point = S256Point.Parse(new BinaryReader(new MemoryStream(bHex)));
                result = "p2pk: " + point.ToString();
            }
            else if (hex.StartsWith("1"))
            {
                byte[] h160 = Base58Encoding.DecodeH160(hex);
                result = "p2pkh: " + Tools.BytesToHexString(h160);
            }
            else if (hex.StartsWith("3"))
            {
                byte[] h160 = Base58Encoding.DecodeH160(hex);
                result = "p2pkh: " + Tools.BytesToHexString(h160);
            }
            else if (hex.StartsWith("bc1") && hex.Length == 42)
            {
                // ptwpkh
                Bech32Decoded bech32 = Bech32.Decode(hex);
                result = "ptwpkh: ";
                result += Tools.CreateJsonObjectAsString(bech32);
            }
            else if (hex.StartsWith("bc1") && hex.Length == 62)
            {
                // ptwsh
                Bech32Decoded bech32 = Bech32.Decode(hex);
                result = "ptwsh: ";
                result += Tools.CreateJsonObjectAsString(bech32);
            }

            return result;
        }
    }
}
