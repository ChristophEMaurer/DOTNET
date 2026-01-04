using System.Numerics;

namespace BitcoinLib
{
    /// <summary>
    /// C:\cmaurer\cmaurer\privat\develop\crypto\bitcoin-master\src\primitives\transaction.h
    /// UnserializeTransaction
    /// 
    ///Basic transaction serialization format:
    ///- int32_t nVersion
    ///- std::vector<CTxIn> vin     -> first byte must be != 0
    ///- std::vector<CTxOut> vout
    ///- uint32_t nLockTime
    ///
    ///Extended transaction serialization format:
    ///- int32_t nVersion
    ///- unsigned char witnessMarker = 0x00     -> if this byte is 0 then this is the witness marker and flags/CTxWitness must exist. If this byte is not 0, then it belongs to the vin data.
    ///- unsigned char flags(!= 0)
    ///- std::vector<CTxIn> vin
    ///- std::vector<CTxOut> vout
    ///- if (flags & 1):
    ///  - CTxWitness wit;
    ///- uint32_t nLockTime
    ///

    /// </summary>
    public class Tx : NetworkMessage
    {
        public static string Command = "tx";

        public UInt32 _version;

        /// <summary>
        /// must be 0x01
        /// </summary>
        public byte _witnessMarker;

        /// <summary>
        /// must be != 0
        /// </summary>
        public byte _flags;

        public List<TxIn> _txIns;
        public List<TxOut> _txOuts;
        public UInt32 _locktime;
        public bool _testnet = false;

        public bool _isSegWit;

        public byte[] _hash_prevouts;
        public byte[] _hash_sequence;
        public byte[] _hash_outputs;

        public Tx(UInt32 version, List<TxIn> txIns, List<TxOut> txOuts, UInt32 locktime, bool isSegWit)
            : this(version, 0, 0, txIns, txOuts, locktime, isSegWit)
        {
        }

        public Tx(UInt32 version, byte witnessMarker, byte flags, List<TxIn> txIns, List<TxOut> txOuts, UInt32 locktime, bool isSegWit) :
            base(Command)
        {
            _version = version;
            _witnessMarker = witnessMarker;
            _flags = flags;
            _txIns = txIns;
            _txOuts = txOuts;
            _locktime = locktime;
            _isSegWit = isSegWit;
        }

        public override string ToString()
        {
            string text;
            string txins = "";
            int count = 0;
            foreach (TxIn txin in _txIns)
            {
                txins += "vin[" + count++ + "]: " + txin.ToString() + Environment.NewLine;
            }
            string txouts = "";
            count = 0;
            foreach (TxOut txout in _txOuts)
            {
                txouts += "vout[" + count++ + "]: " + txout.ToString() + Environment.NewLine;
            }

            if (_flags == 1)
            {
                string witnesses = "";
                count = 0;
                foreach (TxIn txin in _txIns)
                {
                    witnesses += "witness[" + count++ + "]: " + txin._witness.ToString() + Environment.NewLine;
                }
                text = string.Format("tx: {0}\nversion: {1}\nwitness_marker: {2} flags: {3}\ntx_ins:\n{4}tx_outs:\n{5}witnesses:\n{6}locktime= {7}",
                    Id(), _version, _witnessMarker, _flags, txins, txouts, witnesses, _locktime);
            }
            else
            {
                text = string.Format("tx: {0}\nversion: {1}\ntx_ins:\n{2}tx_outs:\n{3}locktime={4}",
                    Id(), _version, txins, txouts, _locktime);
            }

            return text;
        }

        public static Tx Parse(string data)
        {
            Tx tx = Tx.Parse(new BinaryReader(new MemoryStream(Tools.HexStringToBytes(data))));

            return tx;
        }

        public static Tx Parse(byte[] raw)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(raw));
            return Parse(reader);
        }

        public static Tx Parse(BinaryReader input)
        {
            bool isSegWit = false;
            UInt32 version = Tools.ReadUInt32LittleEndian(input);

            // if the next byte is 0, then we have all the witness data.
            // if the next byte is not 0, then there is no witness data and it belongs to the vin data.
            // witness_marker
            // flags
            byte witness_marker = (byte)input.PeekChar();
            byte flags = 0;
            if (witness_marker == 0)
            {
                isSegWit = true;
                witness_marker = input.ReadByte();
                flags = input.ReadByte();
                if (flags != 1)
                {
                    throw new Exception("witness_marker flag is not 1: " + flags);
                }
            }

            UInt64 numInputs = Tools.ReadVarInt(input);
            List<TxIn> txins = new List<TxIn>();
            for (UInt64 i = 0; i < numInputs; i++)
            {
                txins.Add(TxIn.Parse(input));
            }

            UInt64 numOutputs = Tools.ReadVarInt(input);
            List<TxOut> txouts = new List<TxOut>();
            for (UInt64 i = 0; i < numOutputs; i++)
            {
                txouts.Add(TxOut.Parse(input));
            }

            if ((flags & 0x01) != 0)
            {
                //
                // witnesses
                // https://bitcoin.stackexchange.com/questions/77180/what-is-the-witness-and-what-data-does-it-contain
                // https://github.com/bitcoin/bips/blob/master/bip-0141.mediawiki#transaction-id
                // https://bitcoincore.org/en/segwit_wallet_dev/
                //
                // for each TxIn we have:
                //
                // [witness_item_count]     varint
                //      [item_1_length]     varint
                //      [item_1_bytes]      bytes
                //      [item_2_length]     varint
                //      [item_2_bytes]      bytes
                //      ...
                //
                for (int i = 0; i < txins.Count; i++)
                {
                    //
                    // each input must have a witness. The witness can be 0. 
                    // A witness consists of n items, each item is a byte[]
                    //
                    UInt64 numStackitems = Tools.ReadVarInt(input);
                    byte[][] witness = new byte[numStackitems][];
                    txins[i]._witness = witness;
                    for (UInt64 j = 0; j < numStackitems; j++)
                    {
                        UInt64 numBytes = Tools.ReadVarInt(input);
                        // if numBytes is 0, then we do new byte[0]
                        byte[] bytes = new byte[numBytes];
                        input.Read(bytes, 0, (int) numBytes);
                        txins[i]._witness[j] = bytes;
                    }
                }
            }

            UInt32 locktime = Tools.ReadUInt32LittleEndian(input);

            Tx tx = new Tx(version, witness_marker, flags, txins, txouts, locktime, isSegWit);

            return tx;
        }

        /// <summary>
        /// Serializes the Tx: this may or may not include segwit data
        /// </summary>
        /// <returns></returns>
        public byte[] serialize()
        {
            List<byte> data = new List<byte>();
            serialize(data);
            byte[] bytes = data.ToArray();

            return bytes;
        }

        /// <summary>
        /// Serializes this Tx without SegWit data, this is used for creating the hash
        /// </summary>
        /// <returns></returns>
        public byte[] serialize_legacy()
        {
            List<byte> data = new List<byte>();
            serialize_legacy(data);
            byte[] bytes = data.ToArray();

            return bytes;
        }

        /// <summary>
        /// Serializes the Tx: this may or may not include segwit data
        /// </summary>
        /// <param name="data"></param>
        public void serialize(List<byte> data)
        {
            if (_isSegWit)
            {
                serialize_segwit(data);
            }
            else
            {
                serialize_legacy(data);
            }
        }

        public void serialize_legacy(List<byte> data)
        {
            Tools.UIntToLittleEndian(_version, data, 4);

            Tools.EncodeVarInt(data, (UInt64)_txIns.Count);
            foreach (TxIn txin in _txIns)
            {
                txin.serialize(data);
            }

            Tools.EncodeVarInt(data, (UInt64)_txOuts.Count);
            foreach (TxOut txout in _txOuts)
            {
                txout.serialize(data);
            }

            Tools.UIntToLittleEndian(_locktime, data, 4);
        }

        public void serialize_segwit(List<byte> data)
        {
            Tools.UIntToLittleEndian(_version, data, 4);

            // add witness marker and flags
            data.Add(_witnessMarker);
            data.Add(_flags);

            Tools.EncodeVarInt(data, (UInt64)_txIns.Count);
            foreach (TxIn txin in _txIns)
            {
                txin.serialize(data);
            }

            Tools.EncodeVarInt(data, (UInt64)_txOuts.Count);
            foreach (TxOut txout in _txOuts)
            {
                txout.serialize(data);
            }

            // add witness data from TxIns
            foreach (TxIn txin in _txIns)
            {
                Tools.EncodeVarInt(data, (UInt64)txin._witness.Length);
                for (int i = 0; i < txin._witness.Length; i++)
                {
                    //
                    // 0
                    // or
                    // [len varint] [data]
                    //
                    if (txin._witness[i].Length == 0)
                    {
                        // we did new byte[0] in parse()
                        data.Add(0);
                    }
                    else
                    {
                        Tools.EncodeVarInt(data, (UInt64)txin._witness[i].Length);
                        data.AddRange(txin._witness[i]);
                    }
                }
            }

            Tools.UIntToLittleEndian(_locktime, data, 4);
        }

        public string Id()
        {
            byte[] data = Hash();

            string text = Tools.BytesToHexString(data);

            return text;
        }

        public byte[] Hash()
        {
            byte[] raw = serialize_legacy();
            byte[] data = Tools.Hash256(raw);

            string s1 = Tools.BytesToHexString(raw);
            string s2 = Tools.BytesToHexString(data);

            Tools.Reverse(data);

            return data;
        }

        public UInt64 Fee()
        {
            UInt64 input_sum = 0;
            UInt64 output_sum = 0;

            foreach (TxIn txin in _txIns)
            {
                input_sum += txin.Value(_testnet);
            }

            foreach (TxOut txout in _txOuts)
            {
                output_sum += txout._amount;
            }

            UInt64 fee = input_sum - output_sum;

            return fee;
        }

        /// <summary>
        /// Calculates the hash of the this transaction for a specified input, returning the z.
        /// z is the hash of the message - in our case, the input.
        /// The ScriptSig of all inputs are replaced by 0x00 except for the specified input
        /// where the ScriptSig is replaced by the previous ScriptPubKey.
        /// Then, the hash is calculated from this modified transaction.
        /// The hash value z of the modified transaction is valid only for the specified input 
        /// and is used to evaluate the ScriptPubKey + ScriptSig
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <returns>Returns the hash of the transaction - z - for the specified input</returns>
        public BigInteger SigHash(int inputIndex, Script redeemScript)
        {
            byte[] bModifiedTransaction = Tools.ToBytes(_version, 4, "little");
            
            //
            // inputs: replace all ScriptSig by a 0x00 except for the input at the specified input index,
            // where the ScriptSig is replaced by the previous ScriptPubKey
            //
            byte[] bLen = Tools.EncodeVarIntToBytes(_txIns.Count);

            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bLen);

            for (int i = 0; i < _txIns.Count; i++)
            {
                Script script;

                if (i == inputIndex)
                {
                    if (redeemScript != null)
                    {
                        script = new Script(redeemScript);
                    }
                    else
                    {
                        script = _txIns[i].GetPreviousScriptPubKey(_testnet);
                    }
                }
                else
                {
                    script = new Script();
                }
                TxIn txInModified = new TxIn(_txIns[i]._prev_tx, _txIns[i]._prev_index, script, _txIns[i]._sequence);
                byte[] bTxInModified = txInModified.serialize();
                bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bTxInModified);
            }

            //
            // outputs: unchanged
            // 
            bLen = Tools.EncodeVarIntToBytes(_txOuts.Count);
            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bLen);
            for (int i = 0; i < _txOuts.Count; i++)
            {
                byte[] bTxOut = _txOuts[i].serialize();
                bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bTxOut);
            }

            byte[] bytes = Tools.ToBytes(_locktime, 4, "little");
            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bytes);

            bytes = Tools.ToBytes(Signature.SIGHASH_ALL, 4, "little");
            bModifiedTransaction = ArrayHelpers.ConcatArrays(bModifiedTransaction, bytes);

            bytes = Tools.Hash256(bModifiedTransaction);

            BigInteger z = Tools.BigIntegerFromBytes(bytes, "big");

            return z;
        }

        public bool VerifyInput(int inputIndex) // TODO: add segwit stuff from chapter 13
        {
            TxIn txIn = _txIns[inputIndex];
            Script script_pubkey = txIn.GetPreviousScriptPubKey(_testnet);
            Script redeemScript = null;
            BigInteger z;
            byte[][] witness = null;
            Script witnessScript = null;

            if (script_pubkey.Is_P2SH_ScriptPubkey())
            {
                OpItem cmd = txIn._script_sig._cmds.Peek();
                byte[] bLen = Tools.EncodeVarIntToBytes(cmd.Length);
                OpItem itemLen = new OpItem(bLen);
                byte[] raw_redeem = ArrayHelpers.ConcatArrays(itemLen._element, cmd._element);

                redeemScript = Script.Parse(raw_redeem);

                if (redeemScript.Is_P2WPKH_ScriptPubkey())
                {
                    z = sig_hash_bip143(inputIndex, redeemScript, null);
                    witness = txIn._witness;
                }
                else if (redeemScript.Is_P2WSH_ScriptPubkey())
                {
                    byte[] bData = txIn._witness[txIn._witness.Length - 1];
                    byte[] lenData = Tools.EncodeVarIntToBytes(bData.Length);
                    byte[] raw_witness = ArrayHelpers.ConcatArrays(lenData, bData);
                    Script tempScript = Script.Parse(raw_witness);
                    z = sig_hash_bip143(inputIndex, null, tempScript);
                    witness = txIn._witness;
                }
                else
                {
                    z = SigHash(inputIndex, redeemScript);
                    witness = null;
                }
            }
            else
            {
                // ScriptPubkey might be a p2wpkh or p2wsh
                // no redeem script
                if (script_pubkey.Is_P2WPKH_ScriptPubkey())
                {
                    z = sig_hash_bip143(inputIndex, null, null);
                    witness = txIn._witness;
                }
                else if (script_pubkey.Is_P2WSH_ScriptPubkey())
                {
                    byte[] bData = txIn._witness[txIn._witness.Length - 1];
                    byte[] lenData = Tools.EncodeVarIntToBytes(bData.Length);
                    byte[] raw_witness = ArrayHelpers.ConcatArrays(lenData, bData);
                    Script tempScript = Script.Parse(raw_witness);
                    z = sig_hash_bip143(inputIndex, null, tempScript);
                    witness = txIn._witness;
                }
                else
                {
                    z = SigHash(inputIndex, null);
                    witness = null;
                }
            }

            Script combinedScript = txIn._script_sig.Add(script_pubkey);

            if (witness != null)
            {
                witnessScript = new Script(witness);
            }

            if (Tools.LOGGING > 2)
            {
                Console.WriteLine($"Tx.VerifyInput({inputIndex}) z= {z.ToString()}");
            }

            bool success = combinedScript.Evaluate(z, witnessScript);

            return success;
        }

        public bool Verify()
        {
            if (Fee() < 0)
            {
                return false;
            }

            for (int i = 0; i < _txIns.Count; i++)
            {
                if (!VerifyInput(i))
                { 
                    return false; 
                }
            }

            return true;
        }

        /// <summary>
        /// ptpkh Pay-to-public-key-hash
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public bool SignInputPtpkh(int inputIndex, PrivateKey privateKey)
        {
            return SignInputPtpk(inputIndex, privateKey);
        }

        /// <summary>
        /// ptpk Pay-to-public-key-hash
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public bool SignInputPtpk(int inputIndex, PrivateKey privateKey)
        {
            BigInteger z = SigHash(inputIndex, null);
            byte[] bDer = privateKey.Sign(z).Der();
            byte[] bSighashAll = Tools.ToBytes(Signature.SIGHASH_ALL, 1, "little");

            byte[] bSignature = ArrayHelpers.ConcatArrays(bDer, bSighashAll);
            byte[] bSec = privateKey._point.Sec();
            _txIns[inputIndex]._script_sig = new Script(bSignature, bSec);

            bool success = VerifyInput(inputIndex);
            return success;
        }

        public bool IsCoinbase()
        {
            if (_txIns.Count != 1)
            {
                return false; 
            }
            TxIn txIn = _txIns[0];
            if (!txIn._prev_tx.SequenceEqual(new byte[32]))
            { 
                return false; 
            }
            if (txIn._prev_index != 0xffffffff)
            {
                return false;
            }

            return true;
        }

        public int CoinbaseHeight()
        {
            if (!IsCoinbase())
            {
                return 0;
            }
            OpItem item = _txIns[0]._script_sig._cmds[0];

            int height = (int) Op.DecodeNum(item._element);

            return height;
        }

        public byte[] HashPrevOuts()
        {
            if (_hash_prevouts == null)
            {
                List<byte> lst_all_prevouts = new List<byte>();

                foreach (TxIn tx_in in _txIns)
                {
                    // previous hash
                    var copy = (byte[])tx_in._prev_tx.Clone(); // Original bleibt unverändert
                    Array.Reverse(copy);               // nur die Kopie wird gedreht
                    lst_all_prevouts.AddRange(copy);

                    // previous index
                    Tools.UIntToLittleEndian(tx_in._prev_index, lst_all_prevouts, 4);
                }

                byte[] all_prevouts = lst_all_prevouts.ToArray();

                _hash_prevouts = Tools.Hash256(all_prevouts);
            }

            return _hash_prevouts;
        }

        public byte[] hash_sequence()
        {
            if (_hash_sequence== null)
            {
                List<byte> lst_all_sequence = new List<byte>();

                foreach (TxIn tx_in in _txIns)
                {
                    // add sequence
                    Tools.UIntToLittleEndian(tx_in._sequence, lst_all_sequence, 4);
                }

                byte[] all_sequence = lst_all_sequence.ToArray();

                _hash_sequence = Tools.Hash256(all_sequence);
            }

            return _hash_sequence;
        }

        
        public byte[] HashOutputs()
        {
            if (_hash_outputs == null)
            {
                List<byte> lst_all_outputs = new List<byte>();

                foreach (TxOut tx_out in _txOuts)
                {
                    tx_out.serialize(lst_all_outputs);
                }

                byte[] all_outputs = lst_all_outputs.ToArray();

                _hash_outputs = Tools.Hash256(all_outputs);
            }

            return _hash_outputs;
        }

        public BigInteger sig_hash_bip143(int inputIndex, Script redeemScript, Script witnessScript)
        {
            TxIn txIn = _txIns[inputIndex];

            // per BIP143 spec
            List<byte> s = new List<byte>();

            Tools.UIntToLittleEndian(_version, s, 4);
            s.AddRange(HashPrevOuts());
            s.AddRange(hash_sequence());

            // previous hash and index
            //List<byte> temp = new List<byte>();
            var copy = (byte[])txIn._prev_tx.Clone();
            Array.Reverse(copy);
            s.AddRange(copy);
            Tools.UIntToLittleEndian(txIn._prev_index, s, 4);

            byte[] script_code;

            if (witnessScript != null)
            {
                script_code = witnessScript.serialize();
            }
            else if (redeemScript != null)
            {
                script_code = Script.Create_P2PKH_Script(redeemScript._cmds[1]._element).serialize();
            }
            else
            {
                script_code = Script.Create_P2PKH_Script(txIn.GetPreviousScriptPubKey(_testnet)._cmds[1]._element).serialize();
            }
            s.AddRange(script_code);
            Tools.UIntToLittleEndian(txIn.Value(), s, 8);
            Tools.UIntToLittleEndian(txIn._sequence, s, 4);
            s.AddRange(HashOutputs());
            Tools.UIntToLittleEndian(_locktime, s, 4);
            Tools.UIntToLittleEndian(Signature.SIGHASH_ALL, s, 4);

            byte[] b = s.ToArray();
            byte[] hash = Tools.Hash256(b);

            BigInteger z = Tools.BigIntegerFromBytes(hash, "big");

            if (Tools.LOGGING > 1)
            {
                Console.WriteLine("sig_hash_bip143() z = " + z.ToString());
            }

            return z;
        }
    }
}
