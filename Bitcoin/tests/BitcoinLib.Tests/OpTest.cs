using System;
using System.Diagnostics;
using System.Numerics;

namespace BitcoinLib.Test
{
    public class OpTest : UnitTest
    {
        public class OpTestCase
        {
            public Int64 x;
            public byte[] data;

            public OpTestCase(Int64 x, byte[] data)
            {
                this.x = x;
                this.data = data;
            }
        }

        public static void test_encode_decode_num()
        {
#if false
            -32767..- 128 and 128..32767: normal 2 - byte data push; (02 FF FF)..(02 80 80) and(02 80 00)..(02 FF 7F)
            - 8388607..- 32768 and 32768..8388607: normal 3 - byte data push; (03 FF FF FF)..(03 00 80 80) and(03 00 80 00)..(03 FF FF 7F)
            - 2147483647..- 8388608 and 8388608..2147483647: normal 4 - byte data push; (04 FF FF FF FF)..(04 00 00 80 80) and(04 00 00 80 00)..(04 FF FF FF 7F)
#endif
            OpTestCase[] testcases =
            {
                new OpTestCase(0,         new byte[]  {}),
                new OpTestCase(1,         new byte[]  {1}),
                new OpTestCase(127,       new byte[]  {0x7F}),
                new OpTestCase(128,       new byte[] { 0x80, 0x00 }),
                new OpTestCase(-128,      new byte[] { 0x80, 0x80 }),
                new OpTestCase(32767,     new byte[] { 0xFF, 0x7F }),
                new OpTestCase(-32767,    new byte[] { 0xFF, 0xFF }),
                new OpTestCase(32768,     new byte[] { 0x00, 0x80, 0x00 }),
                new OpTestCase(-32768,    new byte[] { 0x00, 0x80, 0x80 }),
                new OpTestCase(8388607,   new byte[] { 0xFF, 0xFF, 0x7F }),
                new OpTestCase(-8388607,  new byte[] { 0xFF, 0xFF, 0xFF }),
                new OpTestCase(8388608,   new byte[] { 0x00, 0x00, 0x80, 0x00 }),
                new OpTestCase(-8388608,  new byte[] { 0x00, 0x00, 0x80, 0x80 }),
                new OpTestCase(2147483647,    new byte[] { 0xFF, 0xFF, 0xFF, 0x7F }),
                new OpTestCase(-2147483647,   new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }),
            };

            foreach (OpTestCase testcase in testcases)
            {
                byte[] data = Op.EncodeNum(testcase.x);

                AssertTrue(data.Length == testcase.data.Length);
                if (data.Length == testcase.data.Length)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        Debug.Assert(data[i] == testcase.data[i]);
                    }
                }

                Int64 num = Op.DecodeNum(testcase.data);
                AssertTrue(num == testcase.x);
            }
        }

        public static void test_op_checkmultisig()
        {
            Console.WriteLine("Testing op_checkmultisig with 2 public keys and 2 signatures");

            BigInteger z = Tools.MakeBigInteger("0e71bfa115715d6fd33796948126f40a8cdd39f187e4afb03896795189fe1423c");
            byte[] sig1 = Tools.HexStringToBytes("3045022100dc92655fe37036f47756db8102e0d7d5e28b3beb83a8fef4f5dc0559bddfb94e02205a36d4e4e6c7fcd16658c50783e00c341609977aed3ad00937bf4ee942a8993701");
            byte[] sig2 = Tools.HexStringToBytes("3045022100da6bee3c93766232079a01639d07fa869598749729ae323eab8eef53577d611b02207bef15429dcadce2121ea07f233115c6f09034c0be68db99980b9a6c5e75402201");
            byte[] sec1 = Tools.HexStringToBytes("022626e955ea6ea6d98850c994f9107b036b1334f18ca8830bfff1295d21cfdb70");
            byte[] sec2 = Tools.HexStringToBytes("03b287eaf122eea69030a0e9feed096bed8045c8b98bec453e1ffac7fbdbd4bb71");

            // stack = [b'', sig1, sig2, b'\x02', sec1, sec2, b'\x02']
            OpItems stack = new OpItems();
            stack.Push(Op.EncodeNum(0));
            stack.Push(sig1);
            stack.Push(sig2);
            stack.Push(Op.EncodeNum(2));
            stack.Push(sec1);
            stack.Push(sec2);
            stack.Push(Op.EncodeNum(2));

            bool success = Op.op_checkmultisig(stack, z);
            AssertTrue(success);

            OpItem item = stack.Pop();
            Int64 num = Op.DecodeNum(item);
            AssertTrue(num == 1);
        }
    }
}

