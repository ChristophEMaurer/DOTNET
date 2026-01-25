using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Storage
{
    public struct BlockWriteResult
    {
        public UInt32 FileNumber;
        public UInt32 Offset;
        public UInt32 Length;
    }
}
