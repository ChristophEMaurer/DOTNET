using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Globalization;

namespace BitcoinLib.Test
{
    public class EccTest : UnitTest
    {
        public static void test_on_curve()
        {
            ConsoleOutWriteLine("Check if point is on curve");

            BigInteger prime = 223;
            FieldElement a = new FieldElement(0, prime);
            FieldElement b = new FieldElement(7, prime);

            object[] data =
            {
                true, 192, 105, true, 17, 56, true, 1, 193,
                false, 200, 119, false, 42, 99, 
            };

            for (int i = 0; i < data.Length;)
            {
                bool isOnCurve = (bool)data[i++];
                FieldElement x1 = new FieldElement((int)data[i++], prime);
                FieldElement y1 = new FieldElement((int)data[i++], prime);

                bool result = Point.PointIsOnCurve(x1, y1, a, b);
                AssertTrue(result == isOnCurve);
            }
        }

        public static void test_chapter_3_p60()
        {
            ConsoleOutWriteLine("Checking that the generator point G is on the curve y^2 = x^3 + 7");

            BigInteger gx = BigInteger.Parse("079be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", NumberStyles.AllowHexSpecifier);
            BigInteger gy = BigInteger.Parse("0483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", NumberStyles.AllowHexSpecifier);

            BigInteger n1 = BigInteger.ModPow(gy, 2, S256Field.P);
            BigInteger n2 = BigInteger.Pow(gx, 3) + 7;
            n2 = BigInteger.ModPow(n2, 1, S256Field.P);
            Console.WriteLine("{0}=={1}", n1, n2);
            AssertTrue(n1 == n2);
        }

        public static void test_chapter_3_p60_b()
        {
            ConsoleOutWriteLine("Checking that n * G is infinity");

            BigInteger gx = BigInteger.Parse("079be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", NumberStyles.AllowHexSpecifier);
            BigInteger gy = BigInteger.Parse("0483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", NumberStyles.AllowHexSpecifier);

            Point G = new Point(
                new FieldElement(gx, S256Field.P),
                new FieldElement(gy, S256Field.P),
                new FieldElement(0, S256Field.P),
                new FieldElement(7, S256Field.P)
                );
            Point result = S256Field.N * G;

            Console.WriteLine(string.Format("n*G\n={0}\n*{1}\n={2}",
                S256Field.N,
                G,
                result));
        }

        public static void test_chapter_3_p61()
        {
            Point result = S256Field.N * S256Field.G;

            Console.WriteLine(result);
        }

        public static void test_add()
        {
            ConsoleOutWriteLine("Chapter 3 ex 3: testing FieldElement.Add()");
            BigInteger prime = 223;
            FieldElement a = new FieldElement(0, prime);
            FieldElement b = new FieldElement(7, prime);

            int[] additions =
            {
                192, 105, 17, 56, 170, 142,
                47, 71, 117, 141, 60, 139,
                143, 98, 76, 66, 47, 71,
            };

            for (int i = 0; i < additions.Length;)
            {
                FieldElement x1 = new FieldElement(additions[i++], prime);
                FieldElement y1 = new FieldElement(additions[i++], prime);
                Point p1 = new Point(x1, y1, a, b);
                FieldElement x2 = new FieldElement(additions[i++], prime);
                FieldElement y2 = new FieldElement(additions[i++], prime);
                Point p2 = new Point(x2, y2, a, b);
                FieldElement x3 = new FieldElement(additions[i++], prime);
                FieldElement y3 = new FieldElement(additions[i++], prime);
                Point p3 = new Point(x3, y3, a, b);

                AssertEqual(p1 + p2, p3);
            }
        }

        public static void test_rmul()
        {
            BigInteger prime = 223;
            BigInteger a = 0;
            BigInteger b = 7;

            int[] mults =
            {
            2, 192, 105, 49, 71,
            2, 143, 98, 64, 168,
            2, 47, 71, 36, 111,
            4, 47, 71, 194, 51,
            8, 47, 71, 116, 55,
            21, 47, 71, -1, -1,
            };

            for (int i = 0; i < mults.Length;)
            {
                int s = mults[i++];
                int x1_raw = mults[i++];
                int y1_raw = mults[i++];
                int x2_raw = mults[i++];
                int y2_raw = mults[i++];

                Point p1 = new Point(
                    new FieldElement(x1_raw, prime),
                    new FieldElement(y1_raw, prime),
                    new FieldElement(a, prime),
                    new FieldElement(b, prime));

                Point p2;
                if (x2_raw == -1)
                {
                    p2 = new Point(
                        null,
                        null,
                        new FieldElement(a, prime),
                        new FieldElement(b, prime));
                }
                else
                {
                    p2 = new Point(
                        new FieldElement(x2_raw, prime),
                        new FieldElement(y2_raw, prime),
                        new FieldElement(a, prime),
                        new FieldElement(b, prime));
                }
                AssertEqual(s * p1, p2);
            }
        }

        public static void test_chapter_3_ex_4()
        {
            ConsoleOutWriteLine("Chapter 3 ex 4: some Point stuff");

            BigInteger prime = 223;
            FieldElement a = new FieldElement(0, prime);
            FieldElement b = new FieldElement(7, prime);

            FieldElement x = new FieldElement(192, prime);
            FieldElement y = new FieldElement(105, prime);
            Point p = new Point(x, y, a, b);
            Console.WriteLine(p + p);

            x = new FieldElement(143, prime);
            y = new FieldElement(98, prime);
            p = new Point(x, y, a, b);
            Console.WriteLine(p + p);

            x = new FieldElement(47, prime);
            y = new FieldElement(71, prime);
            p = new Point(x, y, a, b);
            Console.WriteLine(p + p);
            Console.WriteLine(p + p + p + p);
            Console.WriteLine(p + p + p + p + p + p + p + p);
            Console.WriteLine(p + p + p + p + p + p + p + p + p + p + p + p + p + p + p + p + p + p + p + p + p);
        }

        public static void test_chapter_3_ex_5()
        {
            BigInteger prime = 223;
            FieldElement a = new FieldElement(0, prime);
            FieldElement b = new FieldElement(7, prime);

            FieldElement x = new FieldElement(15, prime);
            FieldElement y = new FieldElement(86, prime);
            Point p = new Point(x, y, a, b);
            Point inf = new Point(null, null, a, b);
            Point product = p;
            int count = 1;
            while (product != inf)
            {
                product += p;
                count++;
            }
            Console.WriteLine(count);
        }
    }
}
