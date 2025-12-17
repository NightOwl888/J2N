// Partly based on: https://github.com/sestoft/C5/blob/master/C5.Tests/Trees/RedBlackTreeSetTests.cs#L857-L966
using J2N.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace J2N.Collections.Generic
{
    public class TestSortedSet : TestCase
    {
        private int[] objArray = new int[1000];
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

        private void LoadForGetViewBetween()
        {
            for (int i = 0; i < objArray.Length; i++)
            {
                int x = new Integer(i);
                objArray[i] = x;
                tree.Add(x);
            }
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
        public void TestTryGetPredecessor_View()
        {
            loadup();
            var view = tree.GetViewBetween(6, 14);

            int res;
            Assert.IsTrue(view.TryGetPredecessor(9, out res) && res == 8);
            Assert.IsTrue(view.TryGetPredecessor(10, out res) && res == 8);

            // The bottom (relative to view)
            Assert.IsTrue(view.TryGetPredecessor(7, out res) && res == 6);

            // The top (relative to view)
            Assert.IsTrue(view.TryGetPredecessor(15, out res) && res == 14);
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
        public void TestTryGetSuccessor_View()
        {
            loadup();
            var view = tree.GetViewBetween(6, 14);

            int res;
            Assert.IsTrue(view.TryGetSuccessor(9, out res) && res == 10);
            Assert.IsTrue(view.TryGetSuccessor(10, out res) && res == 12);

            // The bottom (relative to view)
            Assert.IsTrue(view.TryGetSuccessor(5, out res) && res == 6);

            // The top (relative to view)
            Assert.IsTrue(view.TryGetSuccessor(13, out res) && res == 14);
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

        [Test] // Regression for https://github.com/NightOwl888/J2N/issues/175
        public void TestTryGetSuccessor_View_TooHigh()
        {
            var set = new SortedSet<int>();
            for (int i = 0; i <= 10; i++)
                set.Add(i);

            // View contains [4..7]
            var view = set.GetViewBetween(4, 7);

            // Ask for successor of the maximum element in the view
            bool found = view.TryGetSuccessor(7, out int successor);

            Assert.IsFalse(found);
        }


        [Test]
        public void TestTryGetFloor() // weak predecessor in C5, floor in JDK
        {
            loadup();
            Assert.IsTrue(tree.TryGetFloor(7, out int res) && res == 6);
            Assert.IsTrue(tree.TryGetFloor(8, out res) && res == 8);

            //The bottom
            Assert.IsTrue(tree.TryGetFloor(1, out res) && res == 0);
            Assert.IsTrue(tree.TryGetFloor(0, out res) && res == 0);

            //The top
            Assert.IsTrue(tree.TryGetFloor(39, out res) && res == 38);
            Assert.IsTrue(tree.TryGetFloor(38, out res) && res == 38);
        }

        [Test]
        public void TestTryGetFloor_View()
        {
            loadup();
            var view = tree.GetViewBetween(6, 14);

            Assert.IsTrue(view.TryGetFloor(9, out int res) && res == 8);
            Assert.IsTrue(view.TryGetFloor(10, out res) && res == 10);

            // The bottom
            Assert.IsTrue(view.TryGetFloor(6, out res) && res == 6);

            // The top
            Assert.IsTrue(view.TryGetFloor(15, out res) && res == 14);
            Assert.IsTrue(view.TryGetFloor(14, out res) && res == 14);
        }


        [Test]
        public void TestTryGetFloor_TooLow()
        {
            Assert.IsFalse(tree.TryGetFloor(-1, out int res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TestTryGetCeiling() // weak successor in C5, floor in JDK
        {
            loadup();
            Assert.IsTrue(tree.TryGetCeiling(6, out int res) && res == 6);
            Assert.IsTrue(tree.TryGetCeiling(7, out res) && res == 8);

            //The bottom
            Assert.IsTrue(tree.TryGetCeiling(-1, out res) && res == 0);
            Assert.IsTrue(tree.TryGetCeiling(0, out res) && res == 0);

            //The top
            Assert.IsTrue(tree.TryGetCeiling(37, out res) && res == 38);
            Assert.IsTrue(tree.TryGetCeiling(38, out res) && res == 38);
        }

        [Test]
        public void TestTryGetCeiling_View()
        {
            loadup();
            var view = tree.GetViewBetween(6, 14);

            Assert.IsTrue(view.TryGetCeiling(8, out int res) && res == 8);
            Assert.IsTrue(view.TryGetCeiling(9, out res) && res == 10);

            // The bottom
            Assert.IsTrue(view.TryGetCeiling(5, out res) && res == 6);
            Assert.IsTrue(view.TryGetCeiling(6, out res) && res == 6);

            // The top
            Assert.IsTrue(view.TryGetCeiling(13, out res) && res == 14);
            Assert.IsTrue(view.TryGetCeiling(14, out res) && res == 14);
        }



        [Test]
        public void TryGetCeiling_TooHigh()
        {
            Assert.IsFalse(tree.TryGetCeiling(39, out int res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TryGetCeiling_View_TooHigh()
        {
            loadup();
            var view = tree.GetViewBetween(6, 14);

            Assert.IsFalse(view.TryGetCeiling(15, out int res));
            Assert.AreEqual(0, res);
        }


        public class MockComparer : IComparer<int>
        {
            public int Compare([AllowNull] int a, [AllowNull] int b)
            {
                return a > b ? 1 : a < b ? -1 : 0;
            }
        }


        //[Test]
        //public void TestRange()
        //{
        //    var set = new SortedSet<string>(System.StringComparer.Ordinal) { "H", "G", "F", "E", "D", "C", "B", "A" };
        //    var range = set.GetViewBetween("B", false, "G", false);
        //    var count = range.Count;

        //}

        /**
         * @tests java.util.TreeSet#subSet(java.lang.Object, java.lang.Object)
         */
        [Test]
        public void Test_subSetLjava_lang_ObjectLjava_lang_Object()
        {
            LoadForGetViewBetween();

            // Test for method java.util.SortedSet
            // java.util.TreeSet.subSet(java.lang.Object, java.lang.Object)
            int startPos = objArray.Length / 4;
            int endPos = 3 * objArray.Length / 4;
            SortedSet<int> aSubSet = tree.GetViewBetween(objArray[startPos], lowerValueInclusive: true, objArray[endPos], upperValueInclusive: false);
            assertTrue("Subset has wrong number of elements",
                    aSubSet.Count == (endPos - startPos));
            for (int counter = startPos; counter < endPos; counter++)
                assertTrue("Subset does not contain all the elements it should",
                        aSubSet.Contains(objArray[counter]));

            int result;
            try
            {
                tree.GetViewBetween(objArray[3], lowerValueInclusive: true, objArray[0], upperValueInclusive: false);
                result = 0;
            }
            catch (ArgumentException e)
            {
                result = 1;
            }
            assertEquals("end less than start should throw", 1, result);
        }
    }
}
