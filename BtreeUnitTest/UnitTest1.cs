using BtreeLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BtreeUnitTest
{
    [TestClass]
    public class CreateBtreeTests
    {
        [TestMethod]
        public void BtreeCreateIsSuccess()
        {
            Btree<int> btree = new Btree<int>(3);
            Assert.IsNotNull(btree);
        }

        [TestMethod]
        public void CountZeroAfterCreate()
        {
            Btree<int> btree = new Btree<int>(3);
            Assert.AreEqual(0, btree.Count);
        }

        [TestMethod]
        public void BtreeDegreeIsRight()
        {
            Btree<int> btree = new Btree<int>(3);
            Assert.AreEqual(3, btree.MinTreeDegree);
        }
    }
    [TestClass]
    public class AddItemsInBtreeTests
    {
        [TestMethod]
        public void AfterAddOneItemInNullBtreeKeyCountIsRight()
        {
            Btree<int> btree = new Btree<int>(3);
            btree.Add(1);
            Assert.AreEqual(1, btree.Count);
        }
        [TestMethod]
        public void AfterAddItemIsExictInBtree()
        {
            Btree<int> btree = new Btree<int>(3);
            btree.Add(1);
            Assert.AreEqual(true, btree.Contains(1));
        }
    }
    [TestClass]
    public class SplitPageBtreeTests
    {
        [TestMethod]
        public void SplitRootPageHasCorrectKeyCount()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(2);
            btree.Add(3);
            Assert.AreEqual(3, btree.Count);
        }
        [TestMethod]
        public void SplitPagesHasCorrectKeys()
        {
            Btree<int> btree = new Btree<int>(3);
            btree.Add(1);
            btree.Add(2);
            btree.Add(3);
            Assert.AreEqual(true, btree.Contains(1));
            Assert.AreEqual(true, btree.Contains(2));
            Assert.AreEqual(true, btree.Contains(3));
            Assert.AreEqual(false, btree.Contains(4));
        }
        [TestMethod]
        public void AddManyKeyIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            for (int i = 1; i < 11; i++)
            {
                btree.Add(i);
            }
            Assert.AreEqual(btree.Count, 10);
        }
    }
    [TestClass]
    public class DeleteFromLeafTests
    {
        [TestMethod]
        public void DeleteFromLeftmostLeafIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(2);
            btree.Add(3);
            btree.Delete(1);
            Assert.AreEqual(false, btree.Contains(1));
            Assert.AreEqual(btree.Count, 2);
        }
        [TestMethod]
        public void DeleteFromRightmostLeafIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(2);
            btree.Add(3);
            btree.Delete(3);
            Assert.AreEqual(false, btree.Contains(3));
            Assert.AreEqual(btree.Count, 2);
        }
        [TestMethod]
        public void DeleteFromMiddleLeafIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(4);
            btree.Add(6);
            btree.Add(2);
            btree.Add(3);
            btree.Delete(3);
            Assert.AreEqual(false, btree.Contains(3));
            Assert.AreEqual(btree.Count, 4);
        }
        [TestMethod]
        public void BorrowFromRightNeighborIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(4);
            btree.Add(6);
            btree.Add(2);
            btree.Add(3);
            btree.Add(7);
            btree.Delete(3);
            Assert.AreEqual(false, btree.Contains(3));
            Assert.AreEqual(btree.Count, 5);
        }
        [TestMethod]
        public void BorrowFromLeftNeighborIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(3);
            btree.Add(5);
            btree.Add(2);
            btree.Delete(5);
            Assert.AreEqual(false, btree.Contains(5));
            Assert.AreEqual(btree.Count, 5);
        }
        [TestMethod]
        public void UniteWhithRightNeighborsIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(3);
            btree.Add(2);
            btree.Delete(3);
            Assert.AreEqual(false, btree.Contains(3));
            Assert.AreEqual(btree.Count, 4);
        }
        [TestMethod]
        public void UniteWhithLeftNeighborsIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(3);
            btree.Add(2);
            btree.Delete(9);
            Assert.AreEqual(false, btree.Contains(9));
            Assert.AreEqual(btree.Count, 4);
        }
        [TestMethod]
        public void SimpleDeleteIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(3);
            btree.Delete(3);
            Assert.AreEqual(false, btree.Contains(3));
            Assert.AreEqual(btree.Count, 3);
        }
        [TestMethod]
        public void HardDeleteIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(3);
            btree.Add(5);
            btree.Add(2);
            btree.Delete(9);
            btree.Delete(5);
            btree.Delete(1);
            btree.Delete(7);
            Assert.AreEqual(false, btree.Contains(5));
            Assert.AreEqual(false, btree.Contains(9));
            Assert.AreEqual(false, btree.Contains(1));
            Assert.AreEqual(false, btree.Contains(7));
            Assert.AreEqual(btree.Count, 2);
        }
        [TestMethod]
        public void Hard2DeleteIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(3);
            btree.Add(5);
            btree.Add(6);
            btree.Add(2);
            btree.Delete(5);
            btree.Delete(6);
            btree.Delete(9);
            Assert.AreEqual(false, btree.Contains(5));
            Assert.AreEqual(false, btree.Contains(6));
            Assert.AreEqual(false, btree.Contains(9));
            Assert.AreEqual(btree.Count, 4);
        }
        [ExpectedException(typeof(Exception))]
        public void DeleteFromRootToEmptyTree()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Delete(1);
            btree.Delete(7);
            Assert.ThrowsException<Exception>(()=>btree.Contains(1));
            Assert.ThrowsException<Exception>(() => btree.Contains(7));
        }
    }
    [TestClass]
    public class DeleteFromInternalNode
    {
        [TestMethod]
        public void FindSubstituteKeyInLeftSubTreeIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(2);
            btree.Add(3);
            btree.Add(11);
            btree.Add(12);
            btree.Add(10);
            btree.Delete(11);
            Assert.AreEqual(false, btree.Contains(11));
            Assert.AreEqual(btree.Count, 7);

        }
        [TestMethod]
        public void FindSubstituteKeyInRightSubTreeIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(2);
            btree.Add(3);
            btree.Add(11);
            btree.Add(12);
            btree.Add(13);
            btree.Delete(11);
            Assert.AreEqual(false, btree.Contains(11));
            Assert.AreEqual(btree.Count, 7);
        }
        [TestMethod]
        public void UnionTwoPageIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Add(2);
            btree.Add(3);
            btree.Add(11);
            btree.Add(12);
            btree.Delete(11);
            Assert.AreEqual(false, btree.Contains(11));
            Assert.AreEqual(btree.Count, 6);
        }
        [TestMethod]
        public void DeleteFromRootIsCorrect()
        {
            Btree<int> btree = new Btree<int>(2);
            btree.Add(1);
            btree.Add(7);
            btree.Add(9);
            btree.Delete(7);
            Assert.AreEqual(false, btree.Contains(7));
            Assert.AreEqual(btree.Count, 2);
        }
    }

}

