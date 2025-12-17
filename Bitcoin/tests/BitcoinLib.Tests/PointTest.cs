using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Diagnostics;

namespace BitcoinLib.Test
{
    public class PointTest : UnitTest
    {
        public static void test_ne()
        {
            Point a = new Point(3, -7, 5, 7);
            Point b = new Point(18, 77, 5, 7);
            Debug.Assert(a != b);
            Debug.Assert(false == (a != a));
        }

        public static void test_on_curve()
        {
            AssertException(typeof(ValueErrorException), () => new Point(2, 4, 5, 7));

            Point p1 = new Point(3, 7, 5, 7);
            Point p2 = new Point(18, 77, 5, 7);
        }

        public static void test_add0()
        {
            Point a = new Point(5, 7);
            Point b = new Point(2, 5, 5, 7);
            Point c = new Point(2, -5, 5, 7);
            AssertEqual(b + c, a);
            AssertEqual(a + b, b);
            AssertEqual(b + a, b);
        }

        public static void test_add1()
        {
            Point a = new Point(3, 7, 5, 7);
            Point b = new Point(-1, -1, 5, 7);

            Point ab = a + b;

            AssertEqual(a + b, new Point(2, -5, 5, 7));
        }

        public static void test_add2()
        {
            Point a = new Point(-1, -1, 5, 7);
            Point aa = a + a;
            AssertEqual(a + a, new Point(18, 77, 5, 7));
        }

        public static void test_redux()
        {
            int prime = 223;
            FieldElement a = new FieldElement(0, prime);
            FieldElement b = new FieldElement(7, prime);
            FieldElement x = new FieldElement(47, prime);
            FieldElement y = new FieldElement(71, prime);
            Point p = new Point(x, y, a, b);
        }

        public static void test_all()
        {
            test_ne();
            test_on_curve();
            test_add0();
            test_add1();
            test_add2();
        }

        public static bool chapter_2_ex_1_tool(int x, int y)
        {
            return y * y == x * x * x + 5 * x + 7;
        }

        public static void chapter_2_ex_1()
        {
            Console.WriteLine("2,4:" + chapter_2_ex_1_tool(2, 4));
            Console.WriteLine("-1,-1:" + chapter_2_ex_1_tool(-1, -1));
            Console.WriteLine("18,77:" + chapter_2_ex_1_tool(18, 77));
            Console.WriteLine("5,7:" + chapter_2_ex_1_tool(5, 7));
        }

        public static void chapter_2_ex_4()
        {
            int x1 = 2;
            int y1 = 5;
            int x2 = -1;
            int Y2 = -1;

            int s = (Y2 - y1) / (x2 - x1);
            int x3 = s * s - x1 - x2;
            int y3 = s * (x1 - x3) - y1;

            Console.WriteLine(x3 + ", " + y3);
        }

        public static void chapter_2_ex_5()
        {
            int a = 5;
            int x1 = -1;
            int y1 = -1;

            int s = (3 * x1*x1+ a) / (2 * y1);
            int x3 = s * s - 2 * x1;
            int y3 = s * (x1 - x3) - y1;

            Console.WriteLine(x3 + ", " + y3);
        }
    }
}

