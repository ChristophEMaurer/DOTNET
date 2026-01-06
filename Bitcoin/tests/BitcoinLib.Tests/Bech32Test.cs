using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinLib.BIP39;

namespace BitcoinLib.Test
{
    public class Bech32Test : UnitTest
    {
        public static void test_bech32()
        {
            object[] data = 
            {
                "cafebabecafebabecafebabecafebabecafebabecafebabecafebabecafebabe", 0, "bc1qetlt40k2l6atajh7h2lv4l46hm90aw47etlt40k2l6atajh7h2lq0z93a2", "tb1qetlt40k2l6atajh7h2lv4l46hm90aw47etlt40k2l6atajh7h2lqc2n789",
                "cafebabecafebabecafebabecafebabecafebabe", 0, "bc1qetlt40k2l6atajh7h2lv4l46hm90aw476h9vq9", "tb1qetlt40k2l6atajh7h2lv4l46hm90aw47s37lmk",
                "001486715a3bb24ddfb80e50d70f60192fc8fa00", 0, "bc1qqq2gvu268weymhacpegdwrmqryhu37sq7cut25", "tb1qqq2gvu268weymhacpegdwrmqryhu37sq578c38",
                "751e76e8199196d454941c45d1b3a323f1433bd6", 0, "bc1qw508d6qejxtdg4y5r3zarvary0c5xw7kv8f3t4", "tb1qw508d6qejxtdg4y5r3zarvary0c5xw7kxpjzsx",
                "43ce35bcb9715e03221b31e26bc796abf74c8d46", 0, "bc1qg08rt09ew90qxgsmx83xh3uk40m5er2x5ne2ak", "tb1qg08rt09ew90qxgsmx83xh3uk40m5er2x74zex9",
                "1cc46e47714a22ebe94155030f1cd6c6716679313482b80ed22c3e193fda220e", 0, "bc1qrnzxu3m3fg3wh62p25ps78xkceckv7f3xjptsrkj9slpj076yg8q80yudy", "tb1qrnzxu3m3fg3wh62p25ps78xkceckv7f3xjptsrkj9slpj076yg8qs8jnht",
                "7d4596998de43e79cb661da4dc812220e5d9b865ab80c661936ba988d7e950b0", 0, "bc1q04zedxvdusl8njmxrkjdeqfzyrjanwr94wqvvcvndw5c34lf2zcq2gt2h7", "tb1q04zedxvdusl8njmxrkjdeqfzyrjanwr94wqvvcvndw5c34lf2zcqaqa9d3",
            };

            for (int i = 0; i < data.Length;)
            {
                byte[] bytes = Tools.HexStringToBytes((string)data[i++]);
                int version = (int) data[i++];
                string mainnet = (string) data[i++];
                string testnet = (string) data[i++];

                string actualMainnet = Bech32.Encode("bc", version, bytes);
                string actualTestnet = Bech32.Encode("tb", version, bytes);

                AssertEqual(actualMainnet, mainnet);
                AssertEqual(actualTestnet, testnet);
            }
        }
    }
}


