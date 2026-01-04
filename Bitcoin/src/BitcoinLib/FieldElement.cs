using System.Numerics;

namespace BitcoinLib
{
    /*
     * A field is a finite number of integers such as F9 = { 1,2,3,4,5,6,7,8,9 }
     * A FieldElement is one of these numbers. _prime is the order of the field and as addition etc.
     * are defined with modulo _prime, the definition of a FieldElement must contain the number and the order
     * so that the result can be wrapped around into the field.
     * A FieldElement is just a normal number, but it also contains the max value/order of its field
     */ 
    public class FieldElement
    {
        public static BigInteger NO_MODULO = -1;
        public BigInteger _num;

        /// <summary>
        /// If _prime is -1, then we do normal arithmetic without mod.
        /// This means we do not have a field. The prime is the order of the field.
        /// The order is also the number of values in the field.
        /// </summary>
        public BigInteger _prime;

        public FieldElement(BigInteger num, BigInteger prime)
        {
            if ((prime != NO_MODULO) && (num >= prime || num < 0))
            {
                string text = string.Format("Num {0} not in field range 0 to {1}", num, prime - 1);
                throw new ValueErrorException(text);
            }
            _num = num;
            _prime = prime;
        }

        /// <summary>
        /// FieldElement_9(5) : value 5 in a field with prime/order 9
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string text = string.Format("FieldElement_{0}({1})", _prime, _num);
            return text;
        }

        public override bool Equals(object obj)
        {
            return this == (FieldElement) obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool OperatorEqual(FieldElement b)
        {
            if (b is null)
            {
                return false;
            }

            bool success = ((_num == b._num) && (_prime == b._prime));
            return success;
        }

        public static bool operator ==(FieldElement a, FieldElement b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            if (a is null)
            {
                return false;
            }
            if (b is null)
            {
                return false;
            }
            return a.OperatorEqual(b);
        }
        public static bool operator !=(FieldElement a, FieldElement b)
        {
            return !(a == b);
        }

        public virtual FieldElement OperatorPlus(FieldElement b)
        {
            if (_prime != b._prime)
            {
                string text = string.Format("Cannot add two number in different fields {0}-{1}", _prime, b._prime);
                throw new TypeErrorException(text);
            }

            BigInteger num;
            if (_prime == NO_MODULO)
            {
                num = _num + b._num;
            }
            else
            {
                num = FieldElement.Mod(_num + b._num, _prime);
            }

            FieldElement c = new FieldElement(num, _prime);
            return c;
        }

        public static FieldElement operator +(FieldElement a, FieldElement b)
        {
            return a.OperatorPlus(b);
        }

        public FieldElement OperatorMinus(FieldElement b)
        {
            if (_prime != b._prime)
            {
                string text = string.Format("Cannot add two number in different fields {0}-{1}", _prime, b._prime);
                throw new TypeErrorException(text);
            }
            BigInteger num;
            if (_prime == NO_MODULO)
            {
                num = _num - b._num;
            }
            else
            {
                num = FieldElement.Mod(_num - b._num, _prime);
            }

            FieldElement c = new FieldElement(num, _prime);
            return c;

        }

        public static FieldElement operator -(FieldElement a, FieldElement b)
        {
            return a.OperatorMinus(b);
        }

        public FieldElement OperatorMult(FieldElement b)
        {
            if (_prime != b._prime)
            {
                string text = string.Format("Cannot add two number in different fields {0}-{1}", _prime, b._prime);
                throw new TypeErrorException(text);
            }
            BigInteger num;
            if (_prime == NO_MODULO)
            {
                num = _num * b._num;
            }
            else
            {
                num = FieldElement.Mod(_num * b._num, _prime);
            }
            FieldElement c = new FieldElement(num, _prime);
            return c;
        }

        public static FieldElement operator *(FieldElement a, FieldElement b)
        {
            return a.OperatorMult(b);
        }

        public static FieldElement operator *(BigInteger n, FieldElement b)
        {
            BigInteger num;
            if (b._prime == NO_MODULO)
            {
                num = n * b._num;
            }
            else
            {
                num = FieldElement.Mod(n * b._num, b._prime);
            }
            FieldElement c = new FieldElement(num, b._prime);
            return c;
        }

        public FieldElement OperatorDiv(FieldElement b)
        {
            if (_prime != b._prime)
            {
                string text = string.Format("Cannot add two number in different fields {0}-{1}", _prime, b._prime);
                throw new TypeErrorException(text);
            }

            BigInteger num;
            if (b._prime == NO_MODULO)
            {
                num = _num / b._num;
            }
            else
            {
                //
                // 1/n == pow(n, p-2, p)
                //                   |          mod                           |
                //                   |        pow                |
                // num = (self.num * pow(other.num, self.prime - 2, self.prime)) % self.prime

                //BigInteger pow = Tools.Pow(b._num, _prime - 2);
                //BigInteger mod = FieldElement.Mod(pow, _prime);
                BigInteger mod = BigInteger.ModPow(b._num, _prime - 2, _prime);

                num = _num * mod;
                num = FieldElement.Mod(num, _prime);
            }

            FieldElement c = new FieldElement(num, _prime);
            return c;
        }

        public static FieldElement operator /(FieldElement a, FieldElement b)
        {
            return a.OperatorDiv(b);
        }

        public static BigInteger Mod(BigInteger value, BigInteger modulo)
        {
            //
            // keyword=modulo-normalization
            //
            // -3 % 5                -3     wrong
            // (-3 % 5 + 5) % 5       2     correct
            // 

            BigInteger num = ((value % modulo) + modulo) % modulo;
            return num;
        }

        public FieldElement OperatorPow(BigInteger exponent)
        {
            BigInteger num;

            if (_prime == NO_MODULO)
            {
                num = Tools.Pow(_num, exponent);
            }
            else
            {
                int n = (int)FieldElement.Mod(exponent, _prime - 1);
                num = BigInteger.ModPow(_num, n, _prime);
            }

            FieldElement c = new FieldElement(num, _prime);
            return c;
        }
        public static FieldElement Pow(FieldElement a, BigInteger exponent)
        {
            return a.OperatorPow(exponent);
        }
    }
}
