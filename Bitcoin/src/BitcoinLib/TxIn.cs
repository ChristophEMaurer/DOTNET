using System;
using System.Collections.Generic;
using System.IO;

namespace BitcoinLib
{
    //
    //  http://mainnet.programmingbitcoin.com/tx/ee51510d7bbabe28052038d1deb10c03ec74f06a79e21913c6fcf48d56217c87.hex
    //

    public class TxIn
    {
        public byte[] _prev_tx;
        public UInt32 _prev_index;
        public Script _script_sig;
        public UInt32 _sequence;

        //
        // _witness is populated and serialized by Tx
        //
        public Witness _witness;

        public TxIn(byte[] prev_tx, UInt32 prev_index, Script script, UInt32 sequence)
        {
            _prev_tx = prev_tx;
            _prev_index = prev_index;
            _script_sig = script;
            _sequence = sequence;
        }

        public TxIn(byte[] prev_tx, UInt32 prev_index)
        {
            _prev_tx = prev_tx;
            _prev_index = prev_index;
            _script_sig = new Script();
            _sequence = 0xffffffff;
        }

        public override string ToString()
        {
            string data = string.Format("{0}:{1}",
                Tools.BytesToHexString(_prev_tx),
                _prev_index);

            return data;
        }

        public static TxIn Parse(BinaryReader input)
        {
            byte[] prev_tx = input.ReadBytes(32);
            Tools.Reverse(prev_tx);

            UInt32 prev_index = Tools.ReadInt32LittleEndian(input);

            Script script = Script.Parse(input);

            UInt32 sequence = Tools.ReadInt32LittleEndian(input);

            return new TxIn(prev_tx, prev_index, script, sequence);
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
            // serialize prev_tx, little endian
            for (int i = _prev_tx.Length - 1; i >= 0; i--)
            {
                data.Add(_prev_tx[i]);
            }
            // serialize prev_index, 4 bytes, little endian
            Tools.UIntToLittleEndian(_prev_index, data, 4);
            _script_sig.serialize(data);
            Tools.UIntToLittleEndian(_sequence, data, 4);
        }

        public Tx FetchPreviousTx(bool testnet = false)
        {
            string prev_tx = Tools.BytesToHexString(_prev_tx);

            Tx tx = TxFetcher.Fetch(prev_tx, testnet);

            return tx;
        }

        public UInt64 Value(bool testnet = false)
        {
            Tx tx = FetchPreviousTx(testnet);

            UInt64 amount = tx._txOuts[(int)_prev_index]._amount;

            return amount;
        }

        public Script GetPreviousScriptPubKey(bool testnet)
        {
            Tx tx = FetchPreviousTx(testnet);

            Script script = tx._txOuts[(int)_prev_index]._script_pubkey;

            return script;
        }
    }
}
