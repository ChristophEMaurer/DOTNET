using System.Text;

using BitcoinLib;
using BitcoinLib.Test;

namespace BitcoinLib.Test;

public class MurmurHash3Test : UnitTest
{
    public static void test_murmur3_1()
    {
        object[] data =
        {
            "key", 293u, 2495785535u,
            "Hello World!", 420u, 1535517821u,
            "a$6ajXViSAfFw5pR2kkz3Q28YGrDx$jeaLJ5HFPe", 69u, 3131871211u,
            "!zhgt#HVY#tV%kPPZ$LXYEo@EqyKjqRJPzUb3*hhASWpdyZAF3!t$V96j9Eb9ivzMH2w4jvuyHaXRxd&YbHz*W8yZGJ#CXjXfqMzNGgf@YMfh*RdZpRXtPQ3mV$N9N!%", 23485u, 4240136436u,
        };

        for (int i = 0; i < data.Length;)
        {
            string input = (string)data[i++];
            uint seed = (uint)data[i++];
            uint expected = (uint)data[i++];

            ReadOnlySpan<byte> inputSpan = Encoding.UTF8.GetBytes(input).AsSpan();

            var actual = MurmurHash3.Hash32(ref inputSpan, (uint)seed);

            AssertEqual((uint)expected, actual);

            Console.WriteLine($"MurmurHash3.Hash32(input='{input}', seed={seed})={actual}");
        }
    }

    public static void test_murmur3_2()
    {
        object[] data =
        {
            "key", 293u, 2495785535u,
            "Hello World!", 420u, 1535517821u,
            "a$6ajXViSAfFw5pR2kkz3Q28YGrDx$jeaLJ5HFPe", 69u, 3131871211u,
            "!zhgt#HVY#tV%kPPZ$LXYEo@EqyKjqRJPzUb3*hhASWpdyZAF3!t$V96j9Eb9ivzMH2w4jvuyHaXRxd&YbHz*W8yZGJ#CXjXfqMzNGgf@YMfh*RdZpRXtPQ3mV$N9N!%", 23485u, 4240136436u,
        };

        for (int i = 0; i < data.Length;)
        {
            string input = (string)data[i++];
            uint seed = (uint)data[i++];
            uint expected = (uint)data[i++];

            byte[] bytes = Tools.AsciiStringToBytes(input);
            var actual = MurmurHash3.Hash32(bytes, (uint)seed);

            AssertEqual((uint)expected, actual);

            Console.WriteLine($"MurmurHash3.Hash32(input='{input}', seed={seed})={actual}");
        }
    }
}


