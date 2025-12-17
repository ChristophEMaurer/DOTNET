using System;

namespace BitcoinLib
{
    public class Witness
    {
        public ByteArray[] _stackitems;

        public Witness(int stackitems)
        {
            _stackitems = new ByteArray[stackitems];
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
                ByteArray stackitem = _stackitems[i];
                elements += String.Format("  data[{0}]: {1} bytes: {2}",
                    i, stackitem._bytes.Length, Tools.BytesToHexString(stackitem._bytes));

                if (i < _stackitems.Length - 1)
                {
                elements += "\n";
                }
            }

            return elements;
        }

        public ByteArray this[int i]
        {
            get => _stackitems[i];
            set => _stackitems[i] = value;
        }
    }
}
