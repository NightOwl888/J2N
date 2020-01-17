using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace J2N.Collections.Generic
{
    public class TestSortedSet : TestCase
    {
        private SortedSet<int> tree;

        public override void SetUp()
        {
            base.SetUp();
            tree = new SortedSet<int>(new MockComparer());
        }

        public override void TearDown()
        {
            tree = null;
            base.TearDown();
        }


        private void loadup()
        {
            for (int i = 0; i < 20; i++)
                tree.Add(2 * i);
        }

        [Test]
        public void TestTryGetPredecessor()
        {
            loadup();
            int res;
            Assert.IsTrue(tree.TryGetPredecessor(7, out res) && res == 6);
            Assert.IsTrue(tree.TryGetPredecessor(8, out res) && res == 6);

            //The bottom
            Assert.IsTrue(tree.TryGetPredecessor(1, out res) && res == 0);

            //The top
            Assert.IsTrue(tree.TryGetPredecessor(39, out res) && res == 38);
        }

        [Test]
        public void TestTryGetPredecessor_TooLow()
        {
            int res;
            Assert.IsFalse(tree.TryGetPredecessor(-2, out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(tree.TryGetPredecessor(0, out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TestTryGetSuccessor()
        {
            loadup();
            int res;
            Assert.IsTrue(tree.TryGetSuccessor(7, out res) && res == 8);
            Assert.IsTrue(tree.TryGetSuccessor(8, out res) && res == 10);

            //The bottom
            Assert.IsTrue(tree.TryGetSuccessor(0, out res) && res == 2);
            Assert.IsTrue(tree.TryGetSuccessor(-1, out res) && res == 0);

            //The top
            Assert.IsTrue(tree.TryGetSuccessor(37, out res) && res == 38);
        }

        [Test]
        public void TestTryGetSuccessor_TooHigh()
        {
            int res;
            Assert.IsFalse(tree.TryGetSuccessor(38, out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(tree.TryGetSuccessor(39, out res));
            Assert.AreEqual(0, res);
        }

        public class MockComparer : IComparer<int>
        {
            public int Compare([AllowNull] int a, [AllowNull] int b)
            {
                return a > b ? 1 : a < b ? -1 : 0;
            }
        }
    }
}
