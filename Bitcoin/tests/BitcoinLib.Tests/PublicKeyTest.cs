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
            
            pubkey_hex = "bc1qetlt40k2l6atajh7h2lv4l46hm90aw476h9vq9";
            actual = new PublicKey().DecodePublicKey(pubkey_hex);
            Console.WriteLine($"pubkey_hex {pubkey_hex}");
            Console.WriteLine($"decoded: {actual}");

            pubkey_hex = "bc1qhysluleppcz8m3wwtj7a6uwaxru7rss3knqnh3aa7lryhqg0fs7sz4vpf2";
            actual = new PublicKey().DecodePublicKey(pubkey_hex);
            Console.WriteLine($"pubkey_hex {pubkey_hex}");
            Console.WriteLine($"decoded: {actual}");
        }
    }
}
