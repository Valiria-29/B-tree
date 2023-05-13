using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BtreeLib;

namespace Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Btree<int> btree = new Btree<int>(3);
            btree.Add(1);
            btree.Add(2);
            btree.Add(3);
           
            
        }
    }
}
