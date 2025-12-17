using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib
{
    public class ValueErrorException : Exception
    {
        public ValueErrorException(string text) : base(text)
        {
        }
    }
}
