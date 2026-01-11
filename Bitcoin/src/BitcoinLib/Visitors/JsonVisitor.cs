using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitcoinLib.Visitors
{
    // check this output with 
    // https://blockstream.info/api/block/000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f

    public class JsonVisitor : IBitcoinVisitor
    {
        public string _result;

        public void Visit(Block block)
        {
            _result = JsonSerializer.Serialize(block, new JsonSerializerOptions { WriteIndented = true });
        }
        public void Visit(BlockHeader blockHeader)
        {
            _result = JsonSerializer.Serialize(blockHeader, new JsonSerializerOptions { WriteIndented = true });
        }
        public void Visit(Tx tx)
        {
            _result = JsonSerializer.Serialize(tx, new JsonSerializerOptions { WriteIndented = true });
        }
        public void Visit(Script script)
        {
            _result = JsonSerializer.Serialize(script, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}

