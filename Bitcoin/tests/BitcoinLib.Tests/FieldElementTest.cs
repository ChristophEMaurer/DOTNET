using System;
using System.Diagnostics;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace BitcoinLib.Test
{
    public class FieldElementTest : UnitTest
    {
        public static void test_null0()
        {
            FieldElement a = null;
            FieldElement b = new FieldElement(5, 7);
            AssertFalse(a == b);

            a = new FieldElement(5, 7);
            b = null;
            AssertFalse(a == b);

            a = null;
            b = null;
            AssertTrue(a == b);

            a = new FieldElement(5, 7);
            b = new FieldElement(5, 7);
            AssertTrue(a == b);
        }

        public static void test_mod()
        {
            // test_mod
            BigInteger x = 2;
            BigInteger m = 19;
            Debug.Assert(FieldElement.Mod(x, m) == 2);
            x = -7;
            Debug.Assert(FieldElement.Mod(x, m) == 12);
        }

        public static void test_ne()
        {
            FieldElement a = new FieldElement(2, 31);
            FieldElement b = new FieldElement(2, 31);
            FieldElement c = new FieldElement(15, 31);
            FieldElement d = new FieldElement(15, 32);
            Debug.Assert(a == b);
            Debug.Assert(a != c);
            Debug.Assert(a != d);
            Debug.Assert(b != c);
            Debug.Assert(b != d);

            Debug.Assert(false == (a != b));
            Debug.Assert(false == (b == c));
            Debug.Assert(false == (c == d));
        }

        public static void test_add()
        {
            // test_add
            FieldElement a = new FieldElement(2, 31);
            FieldElement b = new FieldElement(15, 31);
            Debug.Assert(a + b == new FieldElement(17, 31));
            a = new FieldElement(17, 31);
            b = new FieldElement(21, 31);
            Debug.Assert(a + b == new FieldElement(7, 31));
        }
        public static void test_sub()
        {
            // test_sub
            FieldElement a = new FieldElement(29, 31);
            FieldElement b = new FieldElement(4, 31);
            Debug.Assert(a - b == new FieldElement(25, 31));
            a = new FieldElement(15, 31);
            b = new FieldElement(30, 31);
            Debug.Assert(a - b == new FieldElement(16, 31));
        }
        public static void test_mul()
        {
            // test_mul;
            FieldElement a = new FieldElement(24, 31);
            FieldElement b = new FieldElement(19, 31);
            Debug.Assert(a * b == new FieldElement(22, 31));
        }

        public static void test_rmul()
        {
            // test_rmul
            FieldElement a = new FieldElement(24, 31);
            BigInteger m = 2;
            Debug.Assert(m * a == a + a);

        }
        public static void test_pow()
        {
            // test_pow
            FieldElement a = new FieldElement(17, 31);
            Debug.Assert(FieldElement.Pow(a, 3) == new FieldElement(15, 31));
            a = new FieldElement(5, 31);
            FieldElement b = new FieldElement(18, 31);
            Debug.Assert(FieldElement.Pow(a, 5) * b == new FieldElement(16, 31));

        }
        public static void test_div()
        {
            // test_div
            FieldElement a = new FieldElement(3, 31);
            FieldElement b = new FieldElement(24, 31);

            FieldElement result = a / b;
            Debug.Assert(result == new FieldElement(4, 31));
            a = new FieldElement(17, 31);
            Debug.Assert(FieldElement.Pow(a, -3) == new FieldElement(29, 31));
            a = new FieldElement(4, 31);
            b = new FieldElement(11, 31);
            Debug.Assert(FieldElement.Pow(a, -4) * b == new FieldElement(13, 31));

        }
        public static void test_all()
        {
            test_mod();
            test_ne();
            test_add();
            test_sub();
            test_mul();
            test_rmul();
            test_pow();
            test_div();
        }

        public static void chapter_1_ex_2()
        {
            ConsoleOutWriteLine("testing FieldElement.Mod()");

            BigInteger prime = 57;

            AssertEqual(FieldElement.Mod(44 + 33, prime), new BigInteger(20), true);
            AssertEqual(FieldElement.Mod(9 - 29, prime), new BigInteger(37), true);
            AssertEqual(FieldElement.Mod(17 + 42 + 49, prime), new BigInteger(51), true);
            AssertEqual(FieldElement.Mod(52 - 30 - 38, prime), new BigInteger(41), true);
        }

        public static void chapter_1_ex_4()
        {
            ConsoleOutWriteLine("testing FieldElement.Mod()");

            BigInteger prime = 97;

            AssertEqual(FieldElement.Mod(95 * 45 * 31, prime), new BigInteger(23), true);
            AssertEqual(FieldElement.Mod(17 * 13 * 19 * 44, prime), new BigInteger(68), true);
            AssertEqual(FieldElement.Mod(BigInteger.Pow(12, 7) * BigInteger.Pow(77, 49), prime), new BigInteger(63), true);
        }


        public static void chapter_1_ex_5()
        {
            chapter_1_ex_5_param(19);
            chapter_1_ex_5_param(20);
            chapter_1_ex_5_param(26);
            chapter_1_ex_5_param(29);
        }

        public static void chapter_1_ex_5_param(BigInteger order)
        {
            int[] data = { 1, 3, 7, 10, 13, 14, 15, 18 };

            Console.WriteLine("Order = " + order);

            Console.WriteLine("unsorted");
            foreach (int i in data)
            {
                Console.Write("F" + order + " k=" + i + ": ");
                List<BigInteger> list = new List<BigInteger>();
                for (int j = 0; j < order; j++)
                {
                    BigInteger bi = FieldElement.Mod(i * j, order);
                    list.Add(bi);
                }
                foreach (BigInteger bi in list)
                {
                    Console.Write(bi + ", ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("sorted and duplicates removed");

            foreach (int i in data)
            {
                List<BigInteger> list = new List<BigInteger>();
                
                Console.Write("F" + order + " k=" + i + ": ");
                for (int j = 0; j < order; j++)
                {
                    BigInteger bi = FieldElement.Mod(i * j, order);
                    list.Add(bi);
                }
                list = list.Distinct().ToList();
                list.Sort();
                foreach (BigInteger bi in list)
                {
                    Console.Write(bi + ", ");
                }
                Console.WriteLine();
            }
        }

        public static void chapter_1_ex_7()
        {
            int[] data = { 7, 11, 17, 31 };

            foreach (int p in data)
            {
                Console.Write("[");
                for (int j = 1; j < p; j++)
                {
                    FieldElement result = FieldElement.Pow(new FieldElement(j, p), p - 1);
                    Console.Write(result._num + ",");
                }
                Console.WriteLine("]");
            }
        }
        public static void chapter_1_ex_8()
        {
            int prime = 31;

            FieldElement x = 3 * FieldElement.Pow(new FieldElement(24, prime), prime - 2);
            BigInteger bi = FieldElement.Mod(x._num, prime);
            Console.WriteLine(bi);

            x = FieldElement.Pow(new FieldElement(17, prime), -3);
            Console.WriteLine(x._num);

            x = FieldElement.Pow(new FieldElement(4, prime), -4);
            x = x.OperatorMult(new FieldElement(11, prime));
            Console.WriteLine(x._num);
        }



        public static void chapter_3_ex_1()
        {
            BigInteger prime = 223;
            FieldElement a = new FieldElement(0, prime);
            FieldElement b = new FieldElement(7, prime);

            int[] additions =
            {
                192, 105, 17, 56, 200, 119,
                1, 193, 42, 99,
            };

            for (int i = 0; i < additions.Length;)
            {
                FieldElement x1 = new FieldElement(additions[i++], prime);
                FieldElement y1 = new FieldElement(additions[i++], prime);

                Console.WriteLine("(" + x1._num + "," + y1._num + "): " + Point.PointIsOnCurve(x1, y1, a, b));
            }
        }

        public static void chapter_3_page_46()
        {
            ConsoleOutWriteLine("Chapter 3, page 46: line over a finite field");
            ConsoleOutWriteLine("A line in a finite field looks like random stuff, look at: y = 5x + 4");

            BigInteger prime = 19;

            FieldElement five = new FieldElement(5, prime);
            FieldElement four = new FieldElement(4, prime);

            for (int i = 0; i < prime; i++)
            {
                FieldElement x = new FieldElement(i, prime);
                FieldElement y = five * x + four;
                Console.WriteLine(string.Format("{0} * {1} + {2} = {3}",
                    five._num, x.ToString(), four._num, y.ToString()));
            }
        }
        public static void chapter_3_ex_2()
        {
            ConsoleOutWriteLine("testing Point and FieldElement");

            int prime = 223;
            FieldElement a = new FieldElement(0, prime);
            FieldElement b = new FieldElement(7, prime);

            Point p1 = new Point(new FieldElement(170, prime), new FieldElement(142, prime), a, b);
            Point p2 = new Point(new FieldElement(60, prime), new FieldElement(139, prime), a, b);
            Console.WriteLine(p1 + p2);

            p1 = new Point(new FieldElement(47, prime), new FieldElement(71, prime), a, b);
            p2 = new Point(new FieldElement(17, prime), new FieldElement(56, prime), a, b);
            Console.WriteLine(p1 + p2);

            p1 = new Point(new FieldElement(143, prime), new FieldElement(98, prime), a, b);
            p2 = new Point(new FieldElement(76, prime), new FieldElement(66, prime), a, b);
            Console.WriteLine(p1 + p2);
        }

        public static void chapter_3_page50()
        {
            BigInteger prime = 223;

            Point p = new Point(
                new FieldElement(47, prime),
                new FieldElement(71, prime),
                new FieldElement(0, prime),
                new FieldElement(7, prime));

            for (int i = 1; i < 25; i++)
            {
                Point result = i * p;
                Console.WriteLine(string.Format("{0} * {1} = {2}",
                    i, p.ToString(), result.ToString()));
            }
        }
    }
}

