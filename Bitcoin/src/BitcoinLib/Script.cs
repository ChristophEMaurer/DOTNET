using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace BitcoinLib
{
    /// <summary>
    /// Remember that the data of the script has to be parsed from left to right
    /// </summary>
    public class Script
    {
        public const byte OP_IF = 99;
        public const byte OP_NOTIF = 100;
        public const byte OP_TOALTSTACK = 107;
        public const byte OP_FROMALTSTACK = 108;
        public const byte OP_DUP = 118;
        public const byte OP_EQUAL = 135;
        public const byte OP_EQUALVERIFY = 136;
        public const byte OP_CHECKSIG = 172;
        public const byte OP_CHECKSIGVERIFY = 173;
        public const byte OP_HASH160 = 169;
        
        public static bool DebugDumpStacks = false;
        //
        // The stack contains 
        // - a byte -> this is an opcode
        // - a byte array (byte[] -> then this is an element, an argument used by an opcode.
        // object is byte[] or byte
        //
        public OpItems _cmds = null;

        /// <summary>
        /// Make a deep copy of another script
        /// </summary>
        /// <param name="other"></param>
        public Script(Script other) : this(other._cmds)
        {
        }

        /// <summary>
        /// create an empty Script. This is used when verifying a transaction: the ScriptSig must be replaced by a 0x00
        /// An empty Script serialzes to one 0x00 because it has length 0.
        /// </summary>
        public Script()
        {
            _cmds = new OpItems();
        }

        /// <summary>
        /// Initialize from existing array: you do not need the length for this
        /// </summary>
        /// <param name="cmds"></param>
        public Script(OpItems cmds)
        {
            _cmds = cmds;
        }

        /// <summary>
        /// Initialize from existing array: you do not need the length for this
        /// </summary>
        /// <param name="cmds"></param>
        public Script(List<OpItem> cmds)
        {
            _cmds = new OpItems();

            foreach (OpItem oi in cmds)
            {
                _cmds.Add(oi);
            }
        }

        /// <summary>
        /// Create a Script with one element which contains some bytes
        /// This is only for testing
        /// </summary>
        /// <param name="cmds"></param>
        public Script(byte[] data)
        {
            _cmds = new OpItems();
            OpItem opItem = new OpItem(data);
            _cmds.Add(opItem);
        }

        public Script(byte[] data1, byte[] data2)
        {
            _cmds = new OpItems();
            OpItem opItem = new OpItem(data1);
            _cmds.Add(opItem);
            opItem = new OpItem(data2);
            _cmds.Add(opItem);
        }

        public override string ToString()
        {
            string text = "";

            foreach (OpItem cmd in _cmds)
            {
                if (cmd.IsOpcode())
                {
                    byte n = (byte) cmd._opCode;
                    string name = "";
                    if (Op.OpCodeNames.TryGetValue(n, out name))
                    {
                        //
                        // name has the value from the array
                        //
                    }
                    else
                    {
                        //
                        // name is missing in array: put the int value
                        //
                        name = string.Format("OP_[{0}]", n);
                    }
                    text += name;
                }
                else
                {
                    byte[] raw = cmd._element;
                    string name = Tools.BytesToHexString(raw);
                    text += " " + name;
                }
                text += " ";
            }

            return text;
        }

        public void Dump()
        {
            Console.WriteLine(ToString());
        }


        /// <summary>
        /// Initialize from string data. We read from left to right, the bytes will in in the array starting at index 0.
        /// The string does NOT contain the lenth of the remaining bytes.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Script Parse(string input)
        {
            // prepend the length of the bytes so that we can use the Parse() method.
            if ((input.Length % 2) != 0)
            {
                throw new Exception("Length of input string '" + input + "' is not even");
            }

            UInt64 noBytes = (UInt64)input.Length / 2;
            string strLength = Tools.EncodeVarInt(noBytes);

            string inputWithLength = strLength + input;

            Script script = Script.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(inputWithLength))));
            return script;
        }

        /// <summary>
        /// The first byte is the total length of the data.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static Script Parse(byte[] raw)
        {
            return Script.Parse(new BinaryReader(new MemoryStream(raw)));
        }

        /// <summary>
        /// Read the input and parse them to create a stack of commands.
        /// The first byte is the total length of the data.
        /// The first byte at the left will end up at index 0, the last byte at the right will be at the top
        ///
        /// Parse("03010203") will create a List _cmds like so:
        /// _cmds[0] = 01
        /// _cmds[1] = 02
        /// _cmds[2] = 03
        /// if you parse this script, you must start at index 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Script Parse(BinaryReader input)
        {
            UInt64 length = Tools.ReadVarInt(input);
            OpItems cmds = new OpItems();

            UInt64 count = 0;
            while (count < length)
            {
                byte current = input.ReadByte();
                count++;
                byte current_byte = current;
                if (current_byte >= 1 && current_byte <= 75)
                {
                    //
                    // n bytes with 1 <= n <= 75
                    // the next n bytes are an element
                    uint n = current_byte;
                    byte[] bytes = input.ReadBytes((int)n);
                    count += n;
                    cmds.Add(new OpItem(bytes));
                }
                else if (current_byte == 76)
                {
                    //
                    // op_pushdata1: 76 <= n <= 255
                    // but you could also specify less than 76 bytes, dont know if this is legal
                    //
                    uint n = input.ReadByte();
                    if (n < 76)
                    {
                        string text = "Less than 76 bytes (" + n + ") specified with op_pushdata1, should use value 1 <= n <= 75";
                        throw new FormatException(text);
                    }
                    byte[] bytes = input.ReadBytes((int)n);
                    count += n + 1;
                    cmds.Add(new OpItem(bytes));
                }
                else if (current_byte == 77)
                {
                    //
                    // op_pushdata2: ee
                    // Anything over 520 is not allowed
                    //
                    uint n = Tools.ReadUInt16LittleEndian(input);
                    if (n <= 75)
                    {
                        string text = "Less than 76 bytes (" + n + ") specified with op_pushdata2, should use value 1 <= n <= 75";
                        throw new FormatException(text);
                    }
                    if (n < 256)
                    {
                        string text = "Value 76 <= n <= 255 (" + n + ") specified with op_pushdata2, should use op_pushdata1";
                        throw new FormatException(text);
                    }
                    byte[] bytes = input.ReadBytes((int)n);
                    count += n + 2;
                    cmds.Add(new OpItem(bytes));
                }
                else if (current_byte == 78)
                {
                    //
                    // op_pushdata4: the next 4 bytes tells us how many bytes to read 521 <= n <= 4GB - 1
                    // Anything over 520 it not allowed, dont know why this command even exists!
                    // You could still specify n <= 520 with this command
                    //
                    uint n = Tools.ReadUInt16LittleEndian(input);
                    byte[] bytes = input.ReadBytes((int) n);
                    if (n < 76)
                    {
                        string text = "Less than 76 bytes (" + n + ") specified with op_pushdata4, should use value 1 <= n <= 75";
                        throw new FormatException(text);
                    }
                    if (n < 256)
                    {
                        string text = "Value 76 <= n <= 255 (" + n + ") specified with op_pushdata4, should use op_pushdata1";
                        throw new FormatException(text);
                    }
                    if (n < 521)
                    {
                        string text = "Value 256 <= n <= 520 (" + n + ") specified with op_pushdata4, should use op_pushdata2";
                        throw new FormatException(text);
                    }
                    if (n >= 521)
                    {
                        string text = "Value 521 <= n (" + n + ") specified with op_pushdata4, values geater than 520 are not allowed";
                        throw new FormatException(text);
                    }
                    //
                    // We can never get here.
                    //
                    count += n + 4;
                    cmds.Add(new OpItem(bytes));
                }
                else
                {
                    //
                    // a byte with value n = 0 or 79 <= n: an opcode
                    //
                    byte op_code = current_byte;
                    cmds.Add(new OpItem(op_code));
                }
            }

            if (count != length)
            {
                throw new FormatException("parsing script failed");
            }

            return new Script(cmds);
        }

        public byte[] serialize()
        {
            List<byte> data = new List<byte>();
            serialize(data);

            return data.ToArray();
        }

        /// <summary>
        /// Serialized the length of the entire script plus the serial data itself.
        /// The length is the number of elements. One element is a byte or an array of bytes.
        /// </summary>
        /// <param name="data"></param>
        public void serialize(List<byte> data)
        {
            List<byte> raw = new List<byte>();

            raw_serialize(raw);
            UInt64 len = (UInt64)raw.Count;

            Tools.EncodeVarInt(data, len);
            foreach (byte b in raw)
            {
                data.Add(b);
            }
        }

        //
        // Serializes the Script data without the length
        //
        public void raw_serialize(List<byte> data)
        {
            foreach (OpItem cmd in _cmds)
            {
                if (cmd.IsOpcode())
                {
                    byte b = (byte)cmd._opCode;
                    data.Add(b);
                }
                else
                {
                    //
                    // byes contains the pure data
                    //
                    byte[] bytes = cmd._element;
                    uint len = (uint)bytes.Length;

                    //
                    // 1 Write the length of the bytes that will follow
                    //
                    if (len <= 75)
                    {
                        //
                        // OP_0 and 1 <= n <= 75
                        //
                        //
                        data.Add((byte)len);
                    }
                    // 0x100 = 256
                    else if ((len >= 76) && (len <= 255))
                    {
                        data.Add(76);
                        data.Add((byte)len);
                    }
                    else if ((len >= 256) && (len <= 520))
                    {
                        data.Add(77);
                        List<byte> lstLen = new List<byte>();
                        Tools.UIntToLittleEndian(len, lstLen, 1);
                        foreach (byte b in lstLen)
                        {
                            data.Add(b);
                        }
                    }

                    //
                    // 2 Write the bytes
                    //
                    foreach (byte b in bytes)
                    {
                        data.Add(b);
                    }
                }
            }
        }

        private void DumpStack(string text, OpItems stack, bool reverse)
        {
            Console.WriteLine(text);

            if (reverse)
            {
                for (int i = stack.Count - 1; i >= 0; i--)
                {
                    Console.WriteLine(string.Format("{0:000}: {1}", i, stack[i].ToString()));
                }
            }
            else
            {
                for (int i = 0; i < stack.Count; i++)
                {
                    Console.WriteLine(string.Format("{0:000}: {1}", i, stack[i].ToString()));
                }
            }

            if (stack.Count > 0)
            {
                Console.WriteLine();
            }

        }

        private void DumpStacks(OpItems cmds, OpItems stack, OpItems altstack)
        {
            ConsoleColor oldColor = Console.ForegroundColor;

            Tools.ConsoleColorBlue();
            Console.WriteLine("Dumping script stacks###################################################################");
            DumpStack("Script commands:", cmds, false);
            Tools.ConsoleColorYellow();
            DumpStack("Stack:", stack, true);
            DumpStack("AltStack:", altstack, true);

            Console.ForegroundColor = oldColor;
        }

        /// <summary>
        /// The data is in a List, but the leftmost bytes is at index 0
        /// </summary>
        /// <param name="z">the private key</param>
        /// <returns></returns>
        public bool Evaluate(BigInteger z) // TODO
        {
            OpItem cmd;

            //
            // create a copy as we may need to add to this list if we have a RedeemScript
            //
            OpItems cmds = new OpItems(_cmds);
            OpItems stack = new OpItems();
            OpItems altstack = new OpItems();

            while (cmds.Count > 0)
            {
                if (DebugDumpStacks)
                {
                    DumpStacks(cmds, stack, altstack);
                }

                cmd = cmds.Pop(0);
                if (cmd.IsOpcode())
                {
                    byte opCode = (byte)cmd._opCode;

                    if (opCode == OP_IF)
                    {
                        if (!Op.op_if(stack, cmds))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                    else if (opCode == OP_NOTIF)
                    {
                        if (!Op.op_notif(stack, cmds))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                    else if (opCode == OP_TOALTSTACK)
                    {
                        if (!Op.op_toaltstack(stack, altstack))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                    else if (opCode == OP_FROMALTSTACK)
                    {
                        if (!Op.op_fromaltstack(stack, altstack))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                    else if (opCode == OP_CHECKSIG)
                    {
                        if (!Op.op_checksig(stack, z))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                    else if (opCode == OP_CHECKSIGVERIFY)
                    {
                        if (!Op.op_checksigverify(stack, z))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                    else if (opCode == 174)
                    {
                        if (!Op.op_checkmultisig(stack, z))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                    else if (opCode == 175)
                    {
                        if (!Op.op_checkmultisigverify(stack, z))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                    else
                    {
                        Op.OpFunction opFunction = Op.OpCodeFunctions[opCode];
                        if (!opFunction(stack))
                        {
                            Tools.ConsoleOutWriteWarning("bad op: " + Op.OpCodeNames[opCode]);
                            return false;
                        }
                    }
                }
                else
                {
                    // add the element to the stack
                    stack.Add(cmd);

                    if (IsPtshScriptPubkey(cmds))
                    {
                        // Pay-to-Script Hash
                        byte[] scriptLen = Op.EncodeNum(cmd.Length);
                        byte[] redeemScript = ArrayHelpers.ConcatArrays(scriptLen, cmd._element);

                        cmds.Pop(0);
                        byte[] h160 = cmds.Pop(0)._element;
                        cmds.Pop(0);
                        if (!Op.op_hash160(stack))
                        {
                            return false;
                        }
                        stack.Add(new OpItem(h160));
                        if (!Op.op_equal(stack))
                        {
                            return false;
                        }
                        // final result should be a 1
                        if (! Op.op_verify(stack))
                        {
                            return false;
                        }
                        // hashes match! now add the RedeemScript
                        Script script = Script.Parse(redeemScript);
                        cmds.AddRange(script._cmds);
                    }
                }
            }
            
            if (stack.Count == 0)
            {
                //if the stack is empty at the end of processing all the commands we fail
                return false;
            }

            cmd = stack.Pop();
            if (!cmd.IsOpcode() && (cmd.Length == 0))
            {
                // if the stack's top element is an empty byte string (a 0), then we fail
                return false;
            }

            // any other result means that the script has validated.
            return true;
        }


        /// <summary>
        /// Returns whether this follows the
        /// OP_DUP OP_HASH160<20 byte hash> OP_EQUALVERIFY OP_CHECKSIG
        /// pattern
        /// </summary>
        /// <returns></returns>
        public bool IsPtpkhScriptPubkey()
        {
            return IsPtpkhScriptPubkey(_cmds);
        }

        /// <summary>
        /// Returns whether this follows the
        /// OP_DUP OP_HASH160<20 byte hash> OP_EQUALVERIFY OP_CHECKSIG
        /// pattern
        /// </summary>
        /// <param name="cmds">The script commands</param>
        /// <returns></returns>
        private bool IsPtpkhScriptPubkey(OpItems cmds)
        {
            //
            // there should be exactly 5 cmds
            // OP_DUP, OP_HASH160, 20-byte hash, OP_EQUALVERIFY, OP_CHECKSIG
            //
            if (cmds.Count == 5)
            {
                if (cmds[0].IsOpcode(Script.OP_DUP) 
                    && cmds[1].IsOpcode(OP_HASH160)
                    && cmds[2].IsElement() 
                    && (cmds[2].Length == 20) 
                    && cmds[3].IsOpcode(OP_EQUALVERIFY)
                    && cmds[4].IsOpcode(OP_CHECKSIG)
                    )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Is this script a ptsh pay-to-script-hash script?
        /// Returns whether this follows the OP_HASH160<20 byte hash> OP_EQUAL pattern.
        /// </summary>
        /// <returns></returns>
        public bool IsPtshScriptPubkey()
        {
            return IsPtshScriptPubkey(_cmds);
        }

        /// <summary>
        /// Is this script a ptsh pay-to-script-hash script?
        /// Returns whether this follows the OP_HASH160<20 byte hash> OP_EQUAL pattern.
        /// </summary>
        /// <param name="cmds"></param>
        /// <returns></returns>
        private bool IsPtshScriptPubkey(OpItems cmds)
        {
            //
            //  there should be exactly 3 cmds
            // OP_HASH160 (0xa9), 20-byte hash, OP_EQUAL (0x87)
            //
            //if (cmds.Count == 3)
            {
                if (cmds[0].IsOpcode(Script.OP_HASH160) && cmds[1].IsElement() && (cmds[1].Length == 20) && cmds[2].IsOpcode(OP_EQUAL))
                {
                    return true;
                }
            }

            return false;
        }


        //
        // return a new Script object created by combining this script with another one.
        //
        public Script Add(Script other)
        {
            object o = ArrayHelpers.Join(this._cmds, other._cmds);
            List<OpItem> total = (List <OpItem>) o;
            Script script = new Script(total);

            return script;
        }

        public static Script CreateP2pkhScript(byte[] hash160)
        {
            //
            // 0x76: OP_DUP
            // 0xa9: OP_HASH160
            // hash160
            // 0x88: OP_EQUALVERIFY
            // 0xac: OP_CHECKSIG
            //
            OpItem op_dup = new OpItem(0x76);
            OpItem op_hash160 = new OpItem(0xa9);
            OpItem data = new OpItem(hash160);
            OpItem op_equalverify = new OpItem(0x88);
            OpItem op_checksig = new OpItem(0xac);

            OpItems opItems = new OpItems();
            opItems.Add(op_dup);
            opItems.Add(op_hash160);
            opItems.Add(data);
            opItems.Add(op_equalverify);
            opItems.Add(op_checksig);

            Script script = new Script(opItems);

            return script;
        }
    }
}
