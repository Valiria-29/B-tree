using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtreeLib
{
    public class Page <T> 
    {
        public int KeyCount;
        public bool IsLeaf;
        public T[] _keys;
        public Page<T>[] _child { get; private set; }
        public Page<T> _parent;

        public Page(bool isLeaf, int t)
        {
            KeyCount = 0;
            IsLeaf = isLeaf;
            _keys = new T [2 * t - 1];
            _child = new Page<T>[2 * t];
           
        }


        public T this[int index] { get => _keys[index]; set => _keys[index] = value; }

    }
   
}
