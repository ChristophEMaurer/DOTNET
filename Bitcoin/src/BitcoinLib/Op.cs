using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace BitcoinLib
{
    public class Op
    {
        /*
        https://en.bitcoin.it/wiki/BIP_0062

        Push Operators
        Pushing an empty byte sequence must use OP_0.
        Pushing a 1-byte sequence of byte 0x01 through 0x10 must use OP_n.
        Pushing the byte 0x81 must use OP_1NEGATE.
        Pushing any other byte sequence up to 75 bytes must use the normal data push(opcode byte n, with n the number of bytes, followed n bytes of data being pushed).
        Pushing 76 to 255 bytes must use OP_PUSHDATA1.
        Pushing 256 to 520 bytes must use OP_PUSHDATA2.
        OP_PUSHDATA4 can never be used, as pushes over 520 bytes are not allowed, and those below can be done using other operators.
        Any other operation is not considered to be a push.

        Numbers
        The native data type of stack elements is byte arrays, but some operations interpret arguments as integers.
        The used encoding is little endian with an explicit sign bit (the highest bit of the last byte).
        The shortest encodings for numbers are (with the range boundaries encodings given in hex between ()).

        0: OP_0; (00)
        1..16: OP_1..OP_16; (51)..(60)
        -1: OP_1NEGATE; (79)
        -127..-2 and 17..127: normal 1-byte data push; (01 FF)..(01 82) and (01 11)..(01 7F)
        -32767..-128 and 128..32767: normal 2-byte data push; (02 FF FF)..(02 80 80) and (02 80 00)..(02 FF 7F)
        -8388607..-32768 and 32768..8388607: normal 3-byte data push; (03 FF FF FF)..(03 00 80 80) and (03 00 80 00)..(03 FF FF 7F)
        -2147483647..-8388608 and 8388608..2147483647: normal 4-byte data push; (04 FF FF FF FF)..(04 00 00 80 80) and (04 00 00 80 00)..(04 FF FF FF 7F)
        Any other numbers cannot be encoded.

            https://wiki.bitcoinsv.io/index.php/Opcodes_used_in_Bitcoin_Script
         */

        public delegate bool OpFunction(OpItems cmds);

        /// <summary>
        /// Convert a number to a variable number of bytes, the lower the number, the less bytes are used.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        static public byte[] EncodeNum(Int64 num)
        {
            List<byte> result = new List<byte>();

            if (num == 0)
            {
                // return an array of lenth 0
            }
            else
            {
                bool negative = num < 0;

                UInt64 abs_num = negative ? (UInt64)(-num) : (UInt64)num;

                while (abs_num > 0)
                {
                    UInt64 b64 = abs_num & 0xff;
                    byte b = (byte)b64;

                    result.Add(b);
                    abs_num >>= 8;
                }

                if ((result[result.Count - 1] & 0x80) != 0)
                {
                    if (negative)
                    {
                        result.Add(0x80);
                    }
                    else
                    {
                        result.Add(0x00);
                    }
                }
                else if (negative)
                {
                    result[result.Count - 1] |= 0x80;
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Convert the element of an @OpItem to a variable number of bytes, the lower the number, the less bytes are used.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Int64 DecodeNum(OpItem item)
        {
            return DecodeNum(item._element);
        }

        /// <summary>
        /// Convert a variable number of bytes into a number. The bytes are in little endian and have ot be reversed.
        /// 
        /// </summary>
        /// <param name="bytesLittleEndian"></param>
        /// <returns></returns>
        public static Int64 DecodeNum(byte[] bytesLittleEndian)
        {
            Int64 result = 0;

            if (bytesLittleEndian.Length == 0)
            {
                result = 0;
            }
            else
            {
                byte[] bytes = (byte[])bytesLittleEndian.Clone();
                Tools.Reverse(bytes);

                bool negative;

                if ((bytes[0] & 0x80) > 0)
                {
                    negative = true;
                    result = bytes[0] & 0x7F;
                }
                else
                {
                    negative = false;
                    result = bytes[0];
                }
                for (int i = 1; i < bytes.Length; i++)
                {
                    result <<= 8;
                    result += bytes[i];
                }
                if (negative)
                {
                    result = -result;
                }
            }

            return result;
        }

        public static bool op_0(OpItems cmds)
        {
            cmds.Push(EncodeNum(0));
            return true;
        }

        public static bool op_1(OpItems cmds)
        {
            cmds.Push(EncodeNum(1));
            return true;
        }

        public static bool op_1negate(OpItems cmds)
        {
            cmds.Push(EncodeNum(-1));
            return true;
        }

        public static bool op_2(OpItems cmds)
        {
            cmds.Push(EncodeNum(2));
            return true;
        }
        public static bool op_3(OpItems cmds)
        {
            cmds.Push(EncodeNum(3));
            return true;
        }
        public static bool op_4(OpItems cmds)
        {
            cmds.Push(EncodeNum(4));
            return true;
        }
        public static bool op_5(OpItems cmds)
        {
            cmds.Push(EncodeNum(5));
            return true;
        }
        public static bool op_6(OpItems cmds)
        {
            cmds.Push(EncodeNum(6));
            return true;
        }
        public static bool op_7(OpItems cmds)
        {
            cmds.Push(EncodeNum(7));
            return true;
        }
        public static bool op_8(OpItems cmds)
        {
            cmds.Push(EncodeNum(8));
            return true;
        }
        public static bool op_9(OpItems cmds)
        {
            cmds.Push(EncodeNum(9));
            return true;
        }
        public static bool op_10(OpItems cmds)
        {
            cmds.Push(EncodeNum(10));
            return true;
        }
        public static bool op_11(OpItems cmds)
        {
            cmds.Push(EncodeNum(11));
            return true;
        }
        public static bool op_12(OpItems cmds)
        {
            cmds.Push(EncodeNum(12));
            return true;
        }
        public static bool op_13(OpItems cmds)
        {
            cmds.Push(EncodeNum(13));
            return true;
        }
        public static bool op_14(OpItems cmds)
        {
            cmds.Push(EncodeNum(14));
            return true;
        }
        public static bool op_15(OpItems cmds)
        {
            cmds.Push(EncodeNum(15));
            return true;
        }

        public static bool op_16(OpItems cmds)
        {
            cmds.Push(EncodeNum(16));
            return true;
        }
        public static bool op_nop(OpItems cmds)
        {
            return true;
        }

        public static bool op_if(OpItems stack, OpItems items)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            OpItems true_items = new OpItems();
            OpItems false_items = new OpItems();
            OpItems current_items = true_items;
            bool found = false;
            int num_endifs_needed = 1;
            while (items.Count > 0)
            {
                OpItem item = (OpItem)items.Pop();
                if (item._opCode == 99 || item._opCode == 100)
                {
                    num_endifs_needed++;
                    current_items.Push(item);
                }
                else if ((num_endifs_needed == 1) && (item == 103))
                {
                    current_items = false_items;
                }
                else if (item == 104)
                {
                    if (num_endifs_needed == 1)
                    {
                        found = true;
                        break;
                    }
                    else
                    {
                        num_endifs_needed--;
                        current_items.Push(item);
                    }
                }
                else
                {
                    current_items.Push(item);
                }
            }

            if (!found)
            {
                return false;
            }

            OpItem element = (OpItem) stack.Pop();
            if (DecodeNum(element) == 0)
            {
                // items[:0] = false_items  // TODO ????
                Debug.Assert(false);
            }
            else
            {
                // items[:0] = true_items  // TODO ????
                Debug.Assert(false);
            }

            return true;
        }

        public static bool op_notif(OpItems stack, OpItems items)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            OpItems true_items = new OpItems();
            OpItems false_items = new OpItems();
            OpItems current_items = true_items;
            bool found = false;
            int num_endifs_needed = 1;
            while (items.Count > 0)
            {
                OpItem item = (OpItem)items.Pop();
                if (item._opCode == 99 || item._opCode == 100)
                {
                    num_endifs_needed++;
                    current_items.Push(item);
                }
                else if ((num_endifs_needed == 1) && (item == 103))
                {
                    current_items = false_items;
                }
                else if (item == 104)
                {
                    if (num_endifs_needed == 1)
                    {
                        found = true;
                        break;
                    }
                    else
                    {
                        num_endifs_needed--;
                        current_items.Push(item);
                    }
                }
                else
                {
                    current_items.Push(item);
                }
            }

            if (!found)
            {
                return false;
            }

            OpItem element = (OpItem) stack.Pop();
            if (DecodeNum(element) == 0)
            {
                // items[:0] = true_items
                // TODO ????
                Debug.Assert(false);
            } 
            else
            {
                Debug.Assert(false);
            }

            return true;
        }

        /// <summary>
        /// We remove the top element from the stack:
        /// Anything other than a 0 is success.
        /// A 0 means failure.
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public static bool op_verify(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            OpItem element = (OpItem)stack.Pop();
            if (DecodeNum(element) == 0)
            {
                return false;
            }

            return true;
        }

        public static bool op_return(OpItems stack)
        {
            return false;
        }

        public static bool op_toaltstack(OpItems stack, OpItems altstack)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            altstack.Push((OpItem)stack.Pop());

            return true;
        }

        public static bool op_fromaltstack(OpItems stack, OpItems altstack)
        {
            if (altstack.Count < 1)
            {
                return false;
            }

            stack.Push((OpItem)altstack.Pop());

            return true;
        }

        public static bool op_2drop(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            stack.Pop();
            stack.Pop();

            return true;
        }

        public static bool op_2dup(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }

            OpItem e1 = (OpItem) stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            stack.Push(e1);
            stack.Push(e2);
            stack.Push(e1);
            stack.Push(e2);

            return true;
        }

        public static bool op_3dup(OpItems stack)
        {
            if (stack.Count < 3)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            OpItem e3 = (OpItem)stack.Pop();
            stack.Push(e1);
            stack.Push(e2);
            stack.Push(e3);
            stack.Push(e1);
            stack.Push(e2);
            stack.Push(e3);

            return true;
        }

        public static bool op_2over(OpItems stack)
        {
            if (stack.Count < 4)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            OpItem e3 = (OpItem)stack.Pop();
            OpItem e4 = (OpItem)stack.Pop();
            stack.Push(e1);
            stack.Push(e2);
            stack.Push(e3);
            stack.Push(e4);
            stack.Push(e1);
            stack.Push(e2);

            return true;
        }

        public static bool op_2rot(OpItems stack)
        {
            if (stack.Count < 6)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            OpItem e3 = (OpItem)stack.Pop();
            OpItem e4 = (OpItem)stack.Pop();
            OpItem e5 = (OpItem)stack.Pop();
            OpItem e6 = (OpItem)stack.Pop();
            stack.Push(e3);
            stack.Push(e4);
            stack.Push(e5);
            stack.Push(e6);
            stack.Push(e1);
            stack.Push(e2);

            return true;
        }

        public static bool op_2swap(OpItems stack)
        {
            if (stack.Count < 4)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            OpItem e3 = (OpItem)stack.Pop();
            OpItem e4 = (OpItem)stack.Pop();
            stack.Push(e3);
            stack.Push(e4);
            stack.Push(e1);
            stack.Push(e2);

            return true;
        }

        public static bool op_ifdup(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Peek();
            if (e1 != 0)
            {
                stack.Push(e1);

            }

            return true;
        }

        public static bool op_depth(OpItems stack)
        {
            stack.Push(EncodeNum(stack.Count));

            return true;
        }

        public static bool op_drop(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            stack.Pop();

            return true;
        }

        public static bool op_dup(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            stack.Push((OpItem)stack.Peek());

            return true;
        }

        public static bool op_nip(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            stack.Push(e2);

            return true;
        }

        public static bool op_over(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            stack.Push(e1);
            stack.Push(e2);
            stack.Push(e1);

            return true;
        }

        // xn ... x2 x1 x0 <n>	xn ... x2 x1 x0 xn	The item n back in the stack is copied to the top.
        public static bool op_pick(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            OpItem element = (OpItem)stack.Pop();
            int n = (int)DecodeNum(element);
            if (stack.Count < n + 1)
            {
                return false;
            }

            OpItem item = (OpItem)stack[stack.Count - n];
            stack.Push(item);

            return true;
        }

        public static bool op_roll(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            OpItem element = (OpItem)stack.Pop();
            int n = (int)DecodeNum(element);
            if (stack.Count < n + 1)
            {
                return false;
            }

            OpItem item = (OpItem)stack.Pop(stack.Count - n);
            stack.Push(item);

            return true;
        }

        public static bool op_rot(OpItems stack)
        {
            if (stack.Count < 3)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            OpItem e3 = (OpItem)stack.Pop();
            stack.Push(e2);
            stack.Push(e3);
            stack.Push(e1);

            return true;
        }

        public static bool op_swap(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            stack.Push(e1);
            stack.Push(e2);

            return true;
        }

        public static bool op_tuck(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            stack.Push(e2);
            stack.Push(e1);
            stack.Push(e2);

            return true;
        }

        public static bool op_size(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            stack.Push(EncodeNum(((OpItem)stack.Peek()).Length));

            return true;
        }

        public static bool op_equal(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }

            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();

            if (e1 == e2)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_equalverify(OpItems stack)
        {
            bool success = op_equal(stack);

            success = op_verify(stack) && success;

            return success;
        }

        public static bool op_1add(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            Int64 n = DecodeNum((OpItem)stack.Pop());
            n++;
            stack.Push(EncodeNum(n));

            return true;
        }

        public static bool op_1sub(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            Int64 n = DecodeNum((OpItem)stack.Pop());
            n--;
            stack.Push(EncodeNum(n));

            return true;
        }

        public static bool op_negate(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            Int64 n = DecodeNum((OpItem)stack.Pop());
            stack.Push(EncodeNum(-n));

            return true;
        }

        public static bool op_abs(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            Int64 n = DecodeNum((OpItem)stack.Pop());
            if (n < 0)
            {
                stack.Push(EncodeNum(-n));
            }
            else
            {
                stack.Push(EncodeNum(n));
            }

            return true;
        }

        public static bool op_not(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            Int64 n = DecodeNum((OpItem)stack.Pop());
            if (n == 0)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_0notequal(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            Int64 n = DecodeNum((OpItem)stack.Pop());
            if (n == 0)
            {
                stack.Push(EncodeNum(0));
            }
            else
            {
                stack.Push(EncodeNum(1));
            }

            return true;
        }

        public static bool op_add(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());
            
            stack.Push(EncodeNum(n1 + n2));

            return true;
        }

        public static bool op_sub(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            stack.Push(EncodeNum(n2 - n1));

            return true;
        }

        public static bool op_mul(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            stack.Push(EncodeNum(n1 * n2));

            return true;
        }

        public static bool op_booland(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            if (n1 == 1 && n2 == 1)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_boolor(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            if (n1 != 0 || n2 != 0)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_numequal(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            if (n1 == n2)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_numequalverify(OpItems stack)
        {
            bool success = op_numequal(stack);
            success = op_verify(stack) && success;

            return success;
        }

        public static bool op_numnotequal(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            if (n1 != n2)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_lessthan(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            if (n2 < n1)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_greaterthan(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            if (n1 > n2)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_lessthanorequal(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            if (n2 <= n1)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_greaterthanorequal(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            Int64 n1 = DecodeNum((OpItem)stack.Pop());
            Int64 n2 = DecodeNum((OpItem)stack.Pop());

            if (n2 >= n1)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_min(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            Int64 n1 = DecodeNum(e1);
            Int64 n2 = DecodeNum(e2);

            if (n1 < n2)
            {
                stack.Push(e1);
            }
            else
            {
                stack.Push(e2);
            }

            return true;
        }

        public static bool op_max(OpItems stack)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            OpItem e1 = (OpItem)stack.Pop();
            OpItem e2 = (OpItem)stack.Pop();
            Int64 n1 = DecodeNum(e1);
            Int64 n2 = DecodeNum(e2);

            if (n1 > n2)
            {
                stack.Push(e1);
            }
            else
            {
                stack.Push(e2);
            }

            return true;
        }

        public static bool op_within(OpItems stack)
        {
            if (stack.Count < 3)
            {
                return false;
            }
            Int64 max = DecodeNum((OpItem)stack.Pop());
            Int64 min = DecodeNum((OpItem)stack.Pop());
            Int64 x = DecodeNum((OpItem)stack.Pop());

            if (min <= x && x < max)
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_ripemd160(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            byte[] x = Tools.RipeMD160(((OpItem)stack.Pop())._element);
            OpItem opx = new OpItem(x);
            stack.Push(opx);

            return true;
        }

        public static bool op_sha1(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            byte[] x = Tools.SHA1(((OpItem)stack.Pop())._element);
            OpItem opx = new OpItem(x);
            stack.Push(opx);

            return true;
        }

        public static bool op_sha256(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            byte[] x = Tools.SHA256(((OpItem)stack.Pop())._element);
            OpItem opx = new OpItem(x);
            stack.Push(opx);

            return true;
        }

        public static bool op_hash160(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            byte[] x = Tools.Hash160(((OpItem)stack.Pop())._element);
            OpItem opx = new OpItem(x);
            stack.Push(opx);

            return true;
        }

        public static bool op_hash256(OpItems stack)
        {
            if (stack.Count < 1)
            {
                return false;
            }
            byte[] x = Tools.Hash256(((OpItem)stack.Pop())._element);
            OpItem opx = new OpItem(x);
            stack.Push(opx);

            return true;
        }

        /// <summary>
        /// check that there are at least 2 elements on the stack
        /// the top element of the stack is the SEC pubkey
        /// the next element of the stack is the DER signature
        /// take off the last byte of the signature as that's the hash_type
        /// parse the serialized pubkey and signature into objects
        /// verify the signature using S256Point.verify()
        /// push an encoded 1 or 0 depending on whether the signature verified
        /// Script              Stack
        ///                     [pubkey]
        /// [OP_CHECKSIG]       [signature]
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static bool op_checksig(OpItems stack, BigInteger z)
        {
            if (stack.Count < 2)
            {
                return false;
            }
            
            OpItem sec_pubkey = (OpItem)stack.Pop();
            OpItem der_signature_original = (OpItem)stack.Pop();

            //
            // it pop()s all except the last byte which is the 1-Byte-SIGHASH
            // we must make a copy of the der_signature bytes, otherwise this will modify the cached transaction and that transaction will miss a byte.
            //
            OpItem der_signature = new OpItem(der_signature_original);
            der_signature.Pop();

            S256Point point = S256Point.Parse(new BinaryReader(new MemoryStream(sec_pubkey._element)));
            Signature sig = Signature.Parse(der_signature._element);

            if (point.Verify(z, sig))
            {
                stack.Push(EncodeNum(1));
            }
            else
            {
                stack.Push(EncodeNum(0));
            }

            return true;
        }

        public static bool op_checksigverify(OpItems stack, BigInteger z)
        {
            bool success = op_checksig(stack, z);
            success = op_verify(stack) && success;

            return success;
        }

        /// <summary>
        /// We have n public keys. the ScriptPubKey also says that m signature are required to unlock the output.
        /// The ScriptSig has to supply those m signatures, and they must be in the order of the public keys.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static bool op_checkmultisig(OpItems stack, BigInteger z)
        {
            if (stack.Count < 1)
            {
                return false;
            }

            // read public keys
            OpItem element = (OpItem)stack.Pop();
            Int64 n = DecodeNum(element);
            if (stack.Count < n + 1)
            {
                return false;
            }
            OpItems publicKeys = new OpItems();
            for (int i = 0; i < n; i++)
            {
                OpItem itemKey = stack.Pop();
                publicKeys.Add(itemKey);
            }

            // read signatures
            element = (OpItem)stack.Pop();
            Int64 m = DecodeNum(element);
            if (stack.Count < m + 1)
            {
                return false;
            }
            OpItems derSignatures = new OpItems();
            for (int i = 0; i < m; i++)
            {
                OpItem itemSig = stack.Pop();

                // this removes the signature flag at the end: Signature.SIGHASH_*
                itemSig.Pop();
                derSignatures.Add(itemSig);
            }

            // OP_CHECKMULTISIG bug: remove dummy OP_0
            OpItem item = stack.Pop(); // TODO: check that this is OP_0
            if (!item.IsOpZero())
            {
                return false;
            }

            Queue<S256Point> points = new Queue<S256Point>();
            Queue<Signature> signatures = new Queue<Signature>();

            for (int i = 0; i < publicKeys.Count; i++)
            {
                S256Point point = S256Point.Parse(publicKeys[i]._element);
                points.Enqueue(point);
            }
            for (int i = 0; i < derSignatures.Count; i++)
            {
                Signature signature = Signature.Parse(derSignatures[i]._element);
                signatures.Enqueue(signature);
            }

            //
            // for each signature, there must be a matching public key
            //
            while (signatures.Count > 0)
            {
                Signature signature = signatures.Dequeue();
                if (points.Count < 1)
                {
                    return false;
                }
                bool matched = false;
                while (points.Count > 0)
                {
                    S256Point point = points.Dequeue();
                    bool success = point.Verify(z, signature);
                    if (success)
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    return false;
                }
            }

            stack.Push(EncodeNum(1));

            return true;
        }

        public static bool op_checkmultisigverify(OpItems stack, BigInteger z)
        {
            bool success = op_checkmultisig(stack, z);
            success = op_verify(stack) && success;

            return success;
        }

        public static bool op_checklocktimeverify(OpItems stack, Int64 locktime, Int64 sequence)
        {
            if (sequence == 0xffffffff)
            {
                return false;
            }

            if (stack.Count < 1)
            {
                return false;
            }
            OpItem element = (OpItem) stack.Pop();
            Int64 numElement = DecodeNum(element);
            if (element < 0)
            {
                return false;
            }
            if (element < 500000000 && locktime > 500000000)
            {
                return false;
            }
            if (locktime < numElement)
            {
                return false;
            }

            return true;
        }

        public static bool op_checksequenceverify(OpItems stack, Int64 locktime, Int64 sequence)
        {
            if ((sequence & (1 << 31)) == (1 << 31))
            {
                return false;
            }
            if (stack.Count < 1)
            {
                return false;
            }
            Int64 numElement = DecodeNum((OpItem)stack.Pop());
            if (numElement < 0)
            {
                return false;
            }
            if ((numElement & ( 1 << 3)) == (1 << 31))
            {
                return false;
            }
            else if ((numElement & (1 << 22)) != (sequence & (1 << 22)))
            {
                return false;
            }
            else if ((numElement & 0xFFFF) > (sequence & 0xffff))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// See @Script for some op codes
        /// </summary>
        public static Dictionary<uint, OpFunction> OpCodeFunctions = new Dictionary<uint, OpFunction>()
        {
            //
            // https://en.bitcoin.it/wiki/Script
            //

            {0, new OpFunction(op_0) },
            {79, new OpFunction(op_1negate) },
            {81, new OpFunction(op_1) },
            {82, new OpFunction(op_2) },
            {83, new OpFunction(op_3) },
            {84, new OpFunction(op_4) },
            {85, new OpFunction(op_5) },
            {86, new OpFunction(op_6) },
            {87, new OpFunction(op_7) },
            {88, new OpFunction(op_8) },
            {89, new OpFunction(op_9) },
            {90, new OpFunction(op_10) },
            {91, new OpFunction(op_11) },
            {92, new OpFunction(op_12) },
            {93, new OpFunction(op_13) },
            {94, new OpFunction(op_14) },
            {95, new OpFunction(op_15) },
            {96, new OpFunction(op_16) },
            {97, new OpFunction(op_nop) },
            //{   99,  new OpFunction(op_if) },         // handled in script.cs
            //{  100,  new OpFunction(op_notif) },      // handled in script.cs
            {  105,  new OpFunction(op_verify) },
            {  106,  new OpFunction(op_return) },
            //{  107,  new OpFunction(op_toaltstack) },     // handled in script.cs
            //{  108,  new OpFunction(op_fromaltstack) },   // handled in script.cs
            {  109,  new OpFunction(op_2drop) },
            {  110,  new OpFunction(op_2dup) },
            {  111,  new OpFunction(op_3dup) },
            {  112,  new OpFunction(op_2over) },
            {  113,  new OpFunction(op_2rot) },
            {  114,  new OpFunction(op_2swap) },
            {  115,  new OpFunction(op_ifdup) },
            {  116,  new OpFunction(op_depth) },
            {  117,  new OpFunction(op_drop) },
            {  118,  new OpFunction(op_dup) },
            {  119,  new OpFunction(op_nip) },
            {  120,  new OpFunction(op_over) },
            {  121,  new OpFunction(op_pick) },
            {  122,  new OpFunction(op_roll) },
            {  123,  new OpFunction(op_rot) },
            {  124,  new OpFunction(op_swap) },
            {  125,  new OpFunction(op_tuck) },
            {  130,  new OpFunction(op_size) },
            {  135,  new OpFunction(op_equal) },
            {  136,  new OpFunction(op_equalverify) },
            {  139,  new OpFunction(op_1add) },
            {  140,  new OpFunction(op_1sub) },
            {  143,  new OpFunction(op_negate) },
            {  144,  new OpFunction(op_abs) },
            {  145,  new OpFunction(op_not) },
            {  146,  new OpFunction(op_0notequal) },
            {  147,  new OpFunction(op_add) },
            {  148,  new OpFunction(op_sub) },
            {  149,  new OpFunction(op_mul) },
            {  154,  new OpFunction(op_booland) },
            {  155,  new OpFunction(op_boolor) },
            {  156,  new OpFunction(op_numequal) },
            {   157,  new OpFunction(op_numequalverify) },
            {   158,  new OpFunction(op_numnotequal) },
            {   159,  new OpFunction(op_lessthan) },
            {   160,  new OpFunction(op_greaterthan) },
            {   161,  new OpFunction(op_lessthanorequal) },
            {   162,  new OpFunction(op_greaterthanorequal) },
            {   163,  new OpFunction(op_min) },
            {   164,  new OpFunction(op_max) },
            {   165,  new OpFunction(op_within) },
            {   166,  new OpFunction(op_ripemd160) },
            {   167,  new OpFunction(op_sha1) },
            {   168,  new OpFunction(op_sha256) },
            {   169,  new OpFunction(op_hash160) },
            {   170,  new OpFunction(op_hash256) },
            //{   172,  new OpFunction(op_checksig) },              // handled in script.cs
            //{   173,  new OpFunction(op_checksigverify) },        // handled in script.cs
            //{   174,  new OpFunction(op_checkmultisig) },         // handled in script.cs
            //{   175,  new OpFunction(op_checkmultisigverify) },   // handled in script.cs
            {   176,  new OpFunction(op_nop) },
            //{   177,  new OpFunction(op_checklocktimeverify) },   // TODO
            //{   178,  new OpFunction(op_checksequenceverify) },   // TODO
            {   179,  new OpFunction(op_nop) },
            {   180,  new OpFunction(op_nop) },
            {   181,  new OpFunction(op_nop) },
            {   182,  new OpFunction(op_nop) },
            {   183,  new OpFunction(op_nop) },
            {   184,  new OpFunction(op_nop) },
            {   185,  new OpFunction(op_nop) },
        };

        public static Dictionary<int, string> OpCodeNames = new Dictionary<int, string>()
            {
                { 0,"OP_0"},
                // 1 <= n <= 75
                { 76,"OP_PUSHDATA1"},
                { 77,"OP_PUSHDATA2"},
                { 78,"OP_PUSHDATA4"},
                { 79,"OP_1NEGATE"},
                { 81,"OP_1"},
                { 82,"OP_2"},
                { 83,"OP_3"},
                { 84,"OP_4"},
                { 85,"OP_5"},
                { 86,"OP_6"},
                { 87,"OP_7"},
                { 88,"OP_8"},
                { 89,"OP_9"},
                { 90,"OP_10"},
                { 91,"OP_11"},
                { 92,"OP_12"},
                { 93,"OP_13"},
                { 94,"OP_14"},
                { 95,"OP_15"},
                { 96,"OP_16"},
                { 97,"OP_NOP"},
                //{ 98,"OP_VER"},     // DISABLED
                { 99,"OP_IF"},
                { 100,"OP_NOTIF"},
                { 103,"OP_ELSE"},
                { 104,"OP_ENDIF"},
                { 105,"OP_VERIFY"},
                { 106,"OP_RETURN"},
                { 107,"OP_TOALTSTACK"},
                { 108,"OP_FROMALTSTACK"},
                { 109,"OP_2DROP"},
                { 110,"OP_2DUP"},
                { 111,"OP_3DUP"},
                { 112,"OP_2OVER"},
                { 113,"OP_2ROT"},
                { 114,"OP_2SWAP"},
                { 115,"OP_IFDUP"},
                { 116,"OP_DEPTH"},
                { 117,"OP_DROP"},
                { 118,"OP_DUP"},
                { 119,"OP_NIP"},
                { 120,"OP_OVER"},
                { 121,"OP_PICK"},
                { 122,"OP_ROLL"},
                { 123,"OP_ROT"},
                { 124,"OP_SWAP"},
                { 125,"OP_TUCK"},
                { 130,"OP_SIZE"},
                { 135,"OP_EQUAL"},
                { 136,"OP_EQUALVERIFY"},
                { 139,"OP_1ADD"},
                { 140,"OP_1SUB"},
                { 143,"OP_NEGATE"},
                { 144,"OP_ABS"},
                { 145,"OP_NOT"},
                { 146,"OP_0NOTEQUAL"},
                { 147,"OP_ADD"},
                { 148,"OP_SUB"},
                { 149,"OP_MUL"},
                { 154,"OP_BOOLAND"},
                { 155,"OP_BOOLOR"},
                { 156,"OP_NUMEQUAL"},
                { 157,"OP_NUMEQUALVERIFY"},
                { 158,"OP_NUMNOTEQUAL"},
                { 159,"OP_LESSTHAN"},
                { 160,"OP_GREATERTHAN"},
                { 161,"OP_LESSTHANOREQUAL"},
                { 162,"OP_GREATERTHANOREQUAL"},
                { 163,"OP_MIN"},
                { 164,"OP_MAX"},
                { 165,"OP_WITHIN"},
                { 166,"OP_RIPEMD160"},
                { 167,"OP_SHA1"},
                { 168,"OP_SHA256"},
                { 169,"OP_HASH160"},
                { 170,"OP_HASH256"},
                { 171,"OP_CODESEPARATOR"},
                { 172,"OP_CHECKSIG"},
                { 173,"OP_CHECKSIGVERIFY"},
                { 174,"OP_CHECKMULTISIG"},
                { 175,"OP_CHECKMULTISIGVERIFY"},
                { 176,"OP_NOP1"},
                { 177,"OP_CHECKLOCKTIMEVERIFY"},
                { 178,"OP_CHECKSEQUENCEVERIFY"},
                { 179,"OP_NOP4"},
                { 180,"OP_NOP5"},
                { 181,"OP_NOP6"},
                { 182,"OP_NOP7"},
                { 183,"OP_NOP8"},
                { 184,"OP_NOP9"},
                { 185,"OP_NOP10"}
            };
    }
}

