using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Test
{
    public class UnitTest
    {
        public delegate void TestCase();

        public static void Run(TestCase testcase)
        {
            testcase();
        }

        public static void AssertTrue(bool b)
        {
            Debug.Assert(b);
        }
        public static void AssertFalse(bool b)
        {
            Debug.Assert(!b);
        }

        public static void AssertIsNull(object o)
        {
            Debug.Assert(o is null);
        }

        public static void AssertEqual(object a, object b, bool print = false)
        {
            // dont use a == b , it will not call the child classes operator == and a and b are always not equal
            bool success;
            success = (a == b);
            success = (a.Equals(b));
            Debug.Assert(success);

            if (print)
            {
                Console.WriteLine(b);
            }
        }


        public static void AssertEqual(UInt32 a, UInt32 b)
        {
            // dont use a == b , it will not call the child classes operator == and a and b are always not equal
            bool success = (a == b);
            Debug.Assert(success);
        }

        public static void AssertEqual(BigInteger a, BigInteger b)
        {
            bool success = (a == b);
            Debug.Assert(success);
        }

        public static void AssertEqual(byte[] b1, byte[] b2)
        {
            bool success = true;

            if (b1.Length != b2.Length)
            {
                success = false;
            }
            else
            {
                for (int i = 0; i < b1.Length; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        success = false;
                        break;
                    }
                }
            }

            Debug.Assert(success);
        }

        public static void AssertEqual(List<byte> b1, byte[] b2)
        {
            bool success = true;

            if (b1.Count != b2.Length)
            {
                success = false;
            }
            else
            {
                for (int i = 0; i < b1.Count; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        success = false;
                        break;
                    }
                }
            }

            Debug.Assert(success);
        }

        public static void AssertException(Type type, Action action)
        {
            bool success = false;

            try
            {
                action();
            }
            catch (Exception e)
            {
                if (type == e.GetType())
                {
                    success = true;
                }
            }

            Debug.Assert(success);
        }

        public static void ConsoleOutWriteHeader(string text)
        {
            Tools.ConsoleOutWriteHeader(text);
        }
        public static void ConsoleOutWriteLine(string text)
        {
            Console.Out.WriteLine(text);
        }
        public static void ConsoleOutWrite(string text)
        {
            Console.Out.Write(text);
        }
    }
}
