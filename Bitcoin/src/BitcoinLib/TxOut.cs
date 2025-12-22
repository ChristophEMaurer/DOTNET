using System;
using System.Collections.Generic;
using System.IO;

namespace BitcoinLib
{
    public class TxOut
    {
        public UInt64 _amount;
        public Script _script_pubkey;

        public TxOut(UInt64 amount, Script script)
        {
            _amount = amount;
            _script_pubkey = script;
        }

        public override string ToString()
        {
            string text = string.Format("{0}: {1}", 
                _amount, _script_pubkey.ToString());

            return text;
        }

        public static TxOut Parse(BinaryReader input)
        {
            UInt64 amount = Tools.ReadUInt64LittleEndian(input);
            Script script = Script.Parse(input);

            TxOut txout = new TxOut(amount, script);

            return txout;
        }

        public byte[] serialize()
        {
            List<byte> data = new List<byte>();
            serialize(data);
            byte[] bytes = data.ToArray();

            return bytes;
        }
        public void serialize(List<byte> data)
        {
            Tools.UIntToLittleEndian(_amount, data, 8);
            _script_pubkey.serialize(data);
        }
    }
}

