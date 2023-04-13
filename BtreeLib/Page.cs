using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtreeLib
{
    internal class Page
    {
        public int KeyCount;
        public bool IsLeaf;
        public int[] _keys;
        public Page[] _child { get; private set; }
        public Page _parent;

        public Page(bool isLeaf, int t)
        {
            KeyCount = 0;
            IsLeaf = isLeaf;
            _keys = new int[2 * t - 1];
            _child = new Page[2 * t];
           
        }


        public int this[int index] { get => _keys[index]; set => _keys[index] = value; }

    }
   
}
