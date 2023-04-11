using BtreeLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BtreeUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void BtreeCreateIsSuccess()
        {
            Btree btree = new Btree(3);
            Assert.IsNotNull(btree);
        }

        [TestMethod]
        public void CountZeroAfterCreate()
        {
            Btree btree = new Btree(3);
            Assert.AreEqual(0,btree.Count);
        }

        [TestMethod]
        public void BtreeDegreeIsRight()
        {
            Btree btree = new Btree(3);
            Assert.AreEqual(3, btree.t);
        }

        
    }
}
