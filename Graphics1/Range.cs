using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics1
{
    public class Range
    {
        public int start;
        public int center;
        public int end;

        public Range(int _start, int _end)
        {
            start = _start;
            end = _end;
            center = (start + end) / 2;
        }
    }
}
