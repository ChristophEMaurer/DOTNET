using System;
using System.Collections.Generic;
using System.IO;

namespace BitcoinLib
{
    //
    //  http://mainnet.programmingbitcoin.com/tx/ee51510d7bbabe28052038d1deb10c03ec74f06a79e21913c6fcf48d56217c87.hex
    //

    /// <summary>
    /// One Tx has n TxIn and m TxOut.
    /// The difference between all TxIn and TxOut is the fee.
    /// </summary>
    public class TxIn
    {

        public byte[] _prev_tx;
        public string txid { get { return Tools.BytesToHexString(_prev_tx); } }

        public UInt32 _prev_index;
        public UInt32 prevout { get { return _prev_index; } }
        public UInt32 vout { get { return _prev_index; } }

        public Script _script_sig;
        public Script scriptsig { get { return _script_sig; } }

        public UInt32 _sequence;
        public UInt32 sequence { get { return _sequence; } }
        public bool is_coinbase { get { return IsCoinbase(); } }


        //
        // _witness is populated and serialized by Tx
        //
        public byte[][] _witness;

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

            UInt32 prev_index = Tools.ReadUInt32LittleEndian(input);

            Script script = Script.Parse(input);

            UInt32 sequence = Tools.ReadUInt32LittleEndian(input);

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
            UInt64 amount = 0;

            // if _prev_tx is all 0, we are the genesis block and there are no previous txs
            if (!_prev_tx.SequenceEqual(new byte[32]))
            {
                Tx tx = FetchPreviousTx(testnet);

                if (tx != null)
                {
                    // must check because FetchPreviousTx() fails often
                    amount = tx._txOuts[(int)_prev_index]._amount;
                }
            }

            return amount;
        }

        public Script GetPreviousScriptPubKey(bool testnet)
        {
            Tx tx = FetchPreviousTx(testnet);

            Script script = tx._txOuts[(int)_prev_index]._script_pubkey;

            return script;
        }

        public bool IsCoinbase()
        {
            if (!_prev_tx.SequenceEqual(new byte[32]))
            {
                return false;
            }
            if (_prev_index != 0xffffffff)
            {
                return false;
            }

            return true;
        }
    }
}
