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
        public bool IsLeaf { get; private set; }
        public int[] _keys;
        public Page[] _child { get; private set; }
        public Page _parent;

        public Page(bool IsLeaf, int t)
        {
            KeyCount = 0;
            IsLeaf = true;
            _keys = new int[2 * t - 1];
            _child = new Page[2 * t];
        }


        public int this[int index] { get => _keys[index]; set => _keys[index] = value; }

    }
    //internal class Page
    //{
    //    public int KeyCount { get; private set; }
    //    public bool IsLeaf { get; private set; }
    //    public int[] _keys;
    //    public Page Left;
    //    public Page Right;
    //    public Page Parent;

    //    public Page(bool IsLeaf, int t)
    //    {
    //        KeyCount = 0;
    //        IsLeaf = true;
    //        _keys = new int[2 * t - 1];
    //        _child = new Page[2 * t];
    //        _parent = new Page[2 * t - 1];
    //    }


    //    public int this[int index] { get => _keys[index]; set => _keys[index] = value; }

    //}
}
