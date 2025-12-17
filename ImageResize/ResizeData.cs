using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResize
{
    public class ResizeData
    {
        public string _device;
        public int _x;
        public int _y;
        public string _fileName;

        public ResizeData(string device, int x, int y)
        {
            _device = device;
            _x = x;
            _y = y;
        }

        public ResizeData(string device, int x, int y, string fileName)
        {
            _device = device;
            _x = x;
            _y = y;
            _fileName = fileName;
        }
    }
}
