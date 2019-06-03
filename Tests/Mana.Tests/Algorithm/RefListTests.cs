using System;
using Mana.Utilities.Algorithm;
using NUnit.Framework;

namespace Tests.Algorithm
{
    [TestFixture]
    public class RefListTests
    {
        [Test]
        public void TestAdd()
        {
            var list = new RefList<int>();

            Assert.That(list.Capacity == 0);

            list.Add(101);

            Assert.Greater(list.Capacity, 1);
            Assert.AreEqual(list[0], 101);
            
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                int t = list[1];
            });
        }

        [Test]
        public void TestRemoveAt()
        {
            var list = new RefList<int>(12);
            
            list.Add(5);
            list.Add(6);
            list.Add(7);
            list.Add(8);
            list.Add(9);

            Assert.AreEqual(5, list.Length);
            
            Assert.AreEqual(5, list[0]);
            Assert.AreEqual(6, list[1]);
            Assert.AreEqual(7, list[2]);
            Assert.AreEqual(8, list[3]);
            Assert.AreEqual(9, list[4]);

            list.RemoveAt(2);

            Assert.AreEqual(4, list.Length);
            Assert.AreEqual(5, list[0]);
            Assert.AreEqual(6, list[1]);
            Assert.AreEqual(8, list[2]);
            Assert.AreEqual(9, list[3]);
            
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                int t = list[4];
            });
        }

        [Test]
        public void TestRemoveRange_Middle()
        {
            var list = new RefList<int>(5);

            for (int i = 0; i < 5; i++)
                list.Add(i);
            
            // 0
            // 1 (removed)
            // 2 (removed)
            // 3 
            // 4
            
            Assert.AreEqual(5, list.Length);
            
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);
            
            list.RemoveRange(1, 2);
            
            Assert.AreEqual(3, list.Length);
            
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(4, list[2]);
        }
        
        [Test]
        public void TestRemoveRange_Start()
        {
            var list = new RefList<int>(5);

            for (int i = 0; i < 5; i++)
                list.Add(i);
            
            // 0 (removed)
            // 1 (removed)
            // 2 
            // 3 
            // 4
            
            Assert.AreEqual(5, list.Length);
            
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);
            
            list.RemoveRange(0, 2);
            
            Assert.AreEqual(3, list.Length);
            
            Assert.AreEqual(2, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(4, list[2]);
        }
        
        [Test]
        public void TestRemoveRange_End()
        {
            var list = new RefList<int>(5);

            for (int i = 0; i < 5; i++)
                list.Add(i);
            
            // 0 
            // 1 
            // 2 
            // 3 (removed)
            // 4 (removed)
            
            Assert.AreEqual(5, list.Length);
            
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);
            
            list.RemoveRange(3, 2);
            
            Assert.AreEqual(3, list.Length);
            
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
        }
        
        [Test]
        public void TestRemoveRange_All()
        {
            var list = new RefList<int>(5);

            for (int i = 0; i < 5; i++)
                list.Add(i);
            
            // 0 (removed)
            // 1 (removed)
            // 2 (removed)
            // 3 (removed)
            // 4 (removed)
            
            Assert.AreEqual(5, list.Length);
            
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);
            
            list.RemoveRange(0, 5);
            
            Assert.AreEqual(0, list.Length);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                int t = list[0];
            });
        }

        [Test]
        public void TestRemoveRange_Throws()
        {
            var list = new RefList<int>(5);

            for (int i = 0; i < 5; i++)
                list.Add(i);

            void ThrowTest(int index, int length)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    list.RemoveRange(index, length);
                });
            }

            ThrowTest(0, 0);
            ThrowTest(1, 0);
            ThrowTest(-1, 0);
            ThrowTest(-1, 1);
            ThrowTest(0, 6);
            ThrowTest(5, 1);
        }
        
    }
}