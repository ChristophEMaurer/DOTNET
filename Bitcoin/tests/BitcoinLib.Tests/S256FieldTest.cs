using System;
using System.Numerics;
using System.Globalization;

namespace BitcoinLib.Test
{
    public class S256FieldTest : UnitTest
    {
        public static void test_plus()
        {
            S256Field x = new S256Field(5);
            S256Field y = new S256Field(9);

            S256Field z = x + y;

            AssertTrue(z._prime == S256Field.P);
            AssertTrue(z._num == 14);
        }
    }
}
