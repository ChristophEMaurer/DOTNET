using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinLib.Visitors
{
    public interface IBitcoinVisitor
    {
        public void Visit(Block block);
        public void Visit(BlockHeader blockHeader);
        public void Visit(Tx tx);
        public void Visit(Script script);
    }
}
