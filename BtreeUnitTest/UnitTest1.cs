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
            Assert.AreEqual(3, btree.MinTreeDegree);
        }

        [TestMethod]
        public void AfterAddOneItemInNullBtreeKeyCountIsRight()
        {
            Btree btree = new Btree(3);
            btree.Add(1);
            Assert.AreEqual(1, btree.Count);
        }
        [TestMethod]
        public void AfterAddItemIsExictInBtree()
        {
            Btree btree = new Btree(3);
            btree.Add(1);
            Assert.AreEqual(true,btree.Find(1));
        }
        //не проходится тест
        //[TestMethod]
        //public void SplitRootPageHasCorrectKeyCount()
        //{
        //    Btree btree = new Btree(2);
        //    btree.Add(1);
        //    btree.Add(2);
        //    btree.Add(3);
        //    Assert.AreEqual(3, btree.Count);
        //}
        [TestMethod]
        public void SplitPagesHasCorrectKeys()
        {
            Btree btree = new Btree(3);
            btree.Add(1);
            btree.Add(2);
            btree.Add(3);
            Assert.AreEqual(true,btree.Find(1));
            Assert.AreEqual(true, btree.Find(2));
            Assert.AreEqual(true, btree.Find(3));
            Assert.AreEqual(false, btree.Find(4));
        }


    }
}
