using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Test
{
    public class PublicKeyTest
    {
        public static void test_decode()
        {
            string pubkey_hex = "0496b538e853519c726a2c91e61ec11600ae1390813a627c66fb8be7947be63c52da7589379515d4e0a604f8141781e62294721166bf621e73a82cbf2342c858eeac";
            string actual = new PublicKey().DecodePublicKey(pubkey_hex);
            Console.WriteLine($"pubkey_hex {pubkey_hex}");
            Console.WriteLine($"decoded: {actual}");

            pubkey_hex = "0357a4f368868a8a6d572991e484e664810ff14c05c0fa023275251151fe0e53d1";
            actual = new PublicKey().DecodePublicKey(pubkey_hex);
            Console.WriteLine($"pubkey_hex {pubkey_hex}");
            Console.WriteLine($"decoded: {actual}");

            pubkey_hex = "02933ec2d2b111b92737ec12f1c5d20f3233a0ad21cd8b36d0bca7a0cfa5cb8701";
            actual = new PublicKey().DecodePublicKey(pubkey_hex);
            Console.WriteLine($"pubkey_hex {pubkey_hex}");
            Console.WriteLine($"decoded: {actual}");

            pubkey_hex = "1BoatSLRHtKNngkdXEeobR76b53LETtpyT";
            actual = new PublicKey().DecodePublicKey(pubkey_hex);
            Console.WriteLine($"pubkey_hex {pubkey_hex}");
            Console.WriteLine($"decoded: {actual}");
            
            pubkey_hex = "bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kygt080";
            actual = new PublicKey().DecodePublicKey(pubkey_hex);
            Console.WriteLine($"pubkey_hex {pubkey_hex}");
            Console.WriteLine($"decoded: {actual}");

            pubkey_hex = "bc1qrp33g0q5c5txsp9arysrx4k6zdkfs4nce4xj0gdcccefvpysxf3q0sl5k7";
            actual = new PublicKey().DecodePublicKey(pubkey_hex);
            Console.WriteLine($"pubkey_hex {pubkey_hex}");
            Console.WriteLine($"decoded: {actual}");
        }
    }
}
