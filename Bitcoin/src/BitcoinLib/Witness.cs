using System;

namespace BitcoinLib
{
    /// <summary>
    /// Witness is the same as byte[][]
    /// TODO: remove this Witness class and replace it by using byte[][]
    /// </summary>
    public class Witness
    {
        public byte[][] _stackitems;

        public Witness(UInt64 stackitems)
        {
            _stackitems = new byte[stackitems][];
        }

        public override string ToString()
        {
            string elements = "stackelements: " + _stackitems.Length;

            if (_stackitems.Length > 0)
            {
                elements += "\n";
            }

            for (int i = 0; i < _stackitems.Length; i++)
            {
                byte[] stackitem = _stackitems[i];
                elements += String.Format("  data[{0}]: {1} bytes: {2}",
                    i, stackitem.Length, Tools.BytesToHexString(stackitem));

                if (i < _stackitems.Length - 1)
                {
                elements += "\n";
                }
            }

            return elements;
        }

        public int Length
        {
            get { return _stackitems.Length; }
        }

        public byte[] this[UInt64 i]
        {
            get => _stackitems[i];
            set => _stackitems[i] = value;
        }
    }
}
