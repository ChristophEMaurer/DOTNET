
namespace BitcoinLib
{
    /// <summary>
    /// OPitem is either an opcode stored in an int
    /// or
    /// an element stored in a byte array, the first byte is at index 0.
    /// Left to right in a hex string <=> index 0 to last index in the byte array
    /// </summary>
    public class OpItem
    {
        private const int OpCodeInvalid = -1;

        /// <summary>
        /// Element stored in a byte array, _opcode is OpCodeInvalid
        /// </summary>
        public byte[] _element = null;

        /// <summary>
        /// Opcode stored in an int, _element is null
        /// </summary>
        public int _opCode = OpCodeInvalid;

        /// <summary>
        /// Create a deep copy
        /// </summary>
        /// <param name="other"></param>
        public OpItem(OpItem other)
        {
            if (other._element != null)
            {
                _element = (byte[])other._element.Clone();
            }
            _opCode = other._opCode;
        }

        /// <summary>
        /// Create an OpItem containing the specified element passed in the byte array
        /// </summary>
        /// <param name="element"></param>
        public OpItem(byte[] element)
        {
            _element = element;
            _opCode = OpCodeInvalid;
        }

        /// <summary>
        /// Create an OpItem with an opcode
        /// </summary>
        /// <param name="opCode"></param>
        public OpItem(byte opCode)
        {
            _opCode = opCode;
            _element = null;
        }

        public override string ToString()
        {
            string text = "";

            if (IsOpcode())
            {
                byte n = (byte)_opCode;
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
                text = name;
            }
            else
            {
                string name = Tools.BytesToHexString(_element);
                text = "Element: " + name;
            }

            return text;
        }

        /// <summary>
        /// Return true if this is an opcode and not an element
        /// If the opcode is a valid one, then it is an opcode
        /// </summary>
        /// <returns></returns>
        public bool IsOpcode()
        {
            return _opCode != OpCodeInvalid;
        }

        /// <summary>
        /// If the opcode is invalid, then we have an element
        /// </summary>
        /// <returns></returns>
        public bool IsElement()
        {
            return _opCode == OpCodeInvalid;
        }

        /// <summary>
        /// return the byte at the spedified position. Crashes if this is an opcode
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte this[int i]
        {
            get => _element[i];
            set => _element[i] = value;
        }

        /// <summary>
        /// return the length of the element, crashes if this is an opcode
        /// </summary>
        public int Length
        {
            get { return _element.Length; }
        }

        public override bool Equals(object obj)
        {
            return this == (OpItem)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Compares two elements of length 1, this crashes for an opcode
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(OpItem a, int b)
        {
            bool success = false;

            if ((a.Length == 1) && (a[0] == b))
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Compares two elements of length 1, this crashes for an opcode
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(OpItem a, int b)
        {
            bool success = true;

            if ((a.Length == 1) && (a[0] == b))
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Compare two eleemnts, crashes for opcodes.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if both _elements have same length and content.</returns>
        public static bool operator ==(OpItem a, OpItem b)
        {
            bool equal = a._element.SequenceEqual(b._element);

            return equal;
        }

        /// <summary>
        /// Compare two elements, crashes for opcodes
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if the length or content is different</returns>
        public static bool operator !=(OpItem a, OpItem b)
        {
            bool not_equal = !a._element.SequenceEqual(b._element);

            return not_equal;
        }

        /// <summary>
        /// Return the last byte of the byte array byte[].
        /// The _element array must have at least one byte or the code will crash
        /// </summary>
        /// <returns></returns>
        public byte Pop()
        {
            byte ret = _element[_element.Length - 1];

            byte[] newElement = new byte[_element.Length - 1];
            Array.Copy(_element, newElement, newElement.Length);
            _element = newElement;

            return ret;
        }

        public static bool operator <(int a, OpItem b)
        {
            bool success = false;

            if (b.Length == 1 && a < b[0])
            {
                success = true;
            }

            return success;
        }

        public static bool operator >(int a, OpItem b)
        {
            bool success = false;

            if (b.Length == 1 && a > b[0])
            {
                success = true;
            }

            return success;
        }

        public static bool operator <(OpItem a, int b)
        {
            bool success = false;

            if (a.Length == 1 && a[0] < b)
            {
                success = true;
            }

            return success;
        }

        public static bool operator >(OpItem a, int b)
        {
            bool success = false;

            if (a.Length == 1 && a[0] > b)
            {
                success = true;
            }

            return success;
        }

        public static bool operator >(OpItem a, OpItem b)
        {
            bool success = false;

            if (a.Length == 1 && b.Length == 1 && a[0] > b[0])
            {
                success = true;
            }

            return success;
        }

        public static bool operator <(OpItem a, OpItem b)
        {
            bool success = false;

            if (a.Length == 1 && b.Length == 1 && a[0] < b[0])
            {
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Check if this is OP_0 which is an empty byte array (an array of length 0)
        /// </summary>
        /// <returns></returns>
        public bool IsZero()
        {
            return (_element != null) && (_element.Length == 0);
        }

        /// <summary>
        /// Returns true if this is an opcode and not an element
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        public bool IsOpcode(byte opCode)
        {
            return _opCode == opCode;
        }
    }
}
