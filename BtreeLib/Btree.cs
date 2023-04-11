using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtreeLib
{
    public class Btree 
    {
        public int t;
        private Page _root;
        public int Count { get; private set; }

        public Btree(int t)
        {
            this.t = t;
            _root = new Page(true, t);
            Count = 0;
        }

        public bool Find(int findKey)
        {
            if (_root==null)
            {
                throw new Exception(); //искллючение что дерево пустое
            }
            var currentPage = _root;
            int i = 0;
            while ( i !=currentPage.KeyCount) 
            {
                if (findKey > currentPage[i])
                {
                    i++;
                }
                if (findKey == currentPage[i])
                {
                    return true;
                }
                if (currentPage.IsLeaf)
                {
                    return false;
                }
                else
                {
                    currentPage = currentPage._child[i];
                    i = 0;
                }
            }
            return false;
        }

        
    }
}
