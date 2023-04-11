using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtreeLib
{
    internal class Page 
    {
        public int KeyCount { get; private set; }
        public bool IsLeaf { get;private set; }
        private int[] _keys;
        public Page[] _child { get; private set; }

        public Page(bool IsLeaf, int t)
        {
            KeyCount = 0;
            IsLeaf = IsLeaf;
            _keys = new int[2 * t - 1];
            _child = new Page[2 * t];
        }
        

        public int this[int index] { get => _keys[index]; set => _keys[index] = value; }

    }
}
