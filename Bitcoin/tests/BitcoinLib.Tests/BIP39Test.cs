using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib.BIP39;

namespace BitcoinLib.Test.BIP39
{
    public class BIP39Test : UnitTest
    {
        public static void test_bip39_mnemonic()
        {
            ConsoleOutWriteLine("testing implementation of entropy <-> mnemonic -> seed");

            string[]data =
            {
                // samples are from "Mastering Bitcoin, Andreas Antonopoulos, p. 103
                "0c1e24e5917779d297e14d45f14e1a1a", 
                    "army van defense carry jealous true garbage claim echo media make crunch", 
                    "", "0111", "5b56c417303faa3fcba7e57400e120a0ca83ec5a4fc9ffba757fbe63fbd77a89a1a3be4c67196f57c39a88b76373733891bfaba16ed27a813ceed498804c0570",
                "0c1e24e5917779d297e14d45f14e1a1a", 
                    "army van defense carry jealous true garbage claim echo media make crunch", 
                    "SuperDuperSecret", "0111", "3b5df16df2157104cfdd22830162a5e170c0161653e3afe6c88defeefb0818c793dbb28ab3ab091897d0715861dc8a18358f80b79d49acf64142ae57037d1d54",
                "2041546864449caff939d32d574753fe684d3c947c3346713dd8423e74abcf8c", 
                    "cake apple borrow silk endorse fitness top denial coil riot stay wolf luggage oxygen faint major edit measure invite love trap field dilemma oblige", 
                    "", "11000001", "3269bce2674acbd188d4f120072b13b088a0ecf87c6e4cae41657a0bb78f5315b33b3a04356e53d062e55f1e0deaa082df8d487381379df848a6ad7e98798404",
            };

            for (int i = 0; i < data.Length; i++)
            {
                string entropy = data[i++];
                string mnemonic = data[i++];
                string passphrase = data[i++];
                string checksum = data[i++];
                string seed = data[i];

                Console.WriteLine("- Entropy=" + entropy + ", checksum=" + checksum + ", mnemonic=" + mnemonic + ", passphrase=" + passphrase + ", seed=" + seed);
                string calculatedEntropy, calculatedChecksum;
                bool success = BitcoinLib.BIP39.BIP39.MnemonicToEntropy(mnemonic, out calculatedEntropy, out calculatedChecksum);
                AssertTrue(success);
                AssertEqual(entropy, calculatedEntropy);
                AssertEqual(checksum, calculatedChecksum);

                byte[] arCalculatedSeed = BitcoinLib.BIP39.BIP39.MnemonicToSeed(mnemonic, passphrase);
                string strCalculatedSeed = Tools.BytesToHexString(arCalculatedSeed);
                AssertEqual(strCalculatedSeed, seed);
            }
        }
    }
}
