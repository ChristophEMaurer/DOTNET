using System.Numerics;

namespace BitcoinLib
{
    public class Point
    {
        //
        // We are only looking at equations of type
        //  y**2 = a * x**3 + b
        //
        // Bitcoin uses:
        // y**2 = x**3 + 7      with a = 0, b = 7
        //
        // A Point is a point on the curve y**2 = a * x**3 + b
        //

        public bool _isInfinity;
        public FieldElement _x;
        public FieldElement _y;

        // why are _a and _b of type FieldElement and not just a BigInteger? TODO
        public FieldElement _a;
        public FieldElement _b;

        /// <summary>
        /// Copy contructor
        /// </summary>
        /// <param name="isInf"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Point(Point b)
        {
            _isInfinity = b._isInfinity;
            _x = b._x;
            _y = b._y;
            _a = b._a;
            _b = b._b;
        }

        public Point(BigInteger x, BigInteger y, BigInteger a, BigInteger b) :
            this(new FieldElement(x, FieldElement.NO_MODULO),
                new FieldElement(y, FieldElement.NO_MODULO),
                new FieldElement(a, FieldElement.NO_MODULO),
                new FieldElement(b, FieldElement.NO_MODULO))
        {
        }

        public Point(BigInteger a, BigInteger b) :
            this(null, null,
                new FieldElement(a, FieldElement.NO_MODULO),
                new FieldElement(b, FieldElement.NO_MODULO))
        {
        }

        public Point(FieldElement x, FieldElement y, FieldElement a, FieldElement b)
        {
            _isInfinity = ((x is null) && (y is null));
            _x = x;
            _y = y;
            _a = a;
            _b = b;

            if (_isInfinity)
            {
                return;
            }

            // y**2 == x**3 + a*x + b
            FieldElement y2 = FieldElement.Pow(_y, 2);
            FieldElement xx = FieldElement.Pow(_x, 3) + (a * _x) + b;
            if (y2 != xx)
            {
                throw new ValueErrorException(string.Format("({0}, {1}) is not on the curve", _x, _y));
            }
        }

        public override string ToString()
        {
            string text;

            if (_isInfinity)
            {
                text = string.Format("Point(infinity)_{0}_{1} FieldElement({2})",
                    _a._num,
                    _b._num,
                    _b._prime // or/same as a._prime
                    );
            }
            else
            {
                if (_x._prime == FieldElement.NO_MODULO)
                {
                    text = string.Format("Point({0},{1})_{2}_{3} NO_MODULO",
                        _x._num,
                        _y._num,
                        _a._num,
                        _b._num
                        );
                }
                else
                {
                    text = string.Format("Point({0},{1})_{2}_{3} FieldElement({4})",
                        _x._num,
                        _y._num,
                        _a._num,
                        _b._num,
                        _x._prime
                        );
                }
            }

            return text;
        }

        public override bool Equals(object obj)
        {
            return this == (Point)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual bool OperatorEqual(Point b)
        {
            bool success = ((_x == b._x)
                && (_y == b._y)
                && (_a == b._a) && (_b == b._b));

            return success;
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.OperatorEqual(b);
        }
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public virtual Point OperatorPlus(Point b)
        {
            if (_a != b._a || _b != b._b)
            {
                throw new TypeErrorException(string.Format("Points {0}, {1} are not on the same curve",
                    this, b));
            }

            if (_isInfinity)
            {
                return b;
            }
            if (b._isInfinity)
            {
                return this;
            }

            if (_x == b._x && _y != b._y)
            {
                return new Point(null, null, _a, this._b);
            }

            if (_x != b._x)
            {
                FieldElement s1 = (b._y - _y);
                FieldElement s2 = (b._x - _x);
                FieldElement s = s1 / s2;
                FieldElement x = FieldElement.Pow(s, 2) - _x - b._x;
                FieldElement y = s * (_x - x) - _y;
                return new Point(x, y, _a, _b);
            }

            if (this.OperatorEqual(b) && (_y == 0 * _x))
            {
                return new Point(null, null, _a, _b);
            }

            else //if (a == b)
            {
                FieldElement s = ((3 * FieldElement.Pow(_x, 2)) + _a) / (2 * _y);
                FieldElement x3 = FieldElement.Pow(s, 2) - (2 * _x);
                FieldElement y3 = s * (_x - x3) - _y;
                return new Point(x3, y3, _a, _b);
            }
        }

        public static Point operator +(Point a, Point b)
        {
            return a.OperatorPlus(b);
        }

        public virtual Point OperatorMult(BigInteger n)
        {
            Point current = this;
            Point result = new Point(null, null, _a, _b);
            while (n > 0)
            {
                if ((n & BigInteger.One) > 0)
                {
                    result += current;
                }
                current += current;
                n >>= 1;
            }

            return result;
        }

        /// <summary>
        /// n * a = a + ... + a: n times
        /// Faster: double the number for each set bit
        /// </summary>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Point operator *(BigInteger n, Point a)
        {
            return a.OperatorMult(n);
        }

        public static bool PointIsOnCurve(FieldElement x, FieldElement y, FieldElement a, FieldElement b)
        {
            return y * y == x * x * x + a * x + b;
        }
    }
}

