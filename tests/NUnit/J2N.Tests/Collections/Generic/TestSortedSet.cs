// Partly based on: https://github.com/sestoft/C5/blob/master/C5.Tests/Trees/RedBlackTreeSetTests.cs#L857-L966
using J2N.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
#nullable enable

namespace J2N.Collections.Generic
{
    public class TestSortedSet : TestCase
    {
        private int[] objArray = new int[1000];
        private SortedSet<int>? tree;

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
                tree!.Add(2 * i);
        }

        private void LoadForGetViewBetween()
        {
            for (int i = 0; i < objArray.Length; i++)
            {
                int x = new Integer(i);
                objArray[i] = x;
                tree!.Add(x);
            }
        }

        [Test]
        public void TestTryGetPredecessor()
        {
            loadup();
            int res;
            Assert.IsTrue(tree!.TryGetPredecessor(7, out res) && res == 6);
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
            var view = tree!.GetViewBetween(6, 14);

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
            Assert.IsFalse(tree!.TryGetPredecessor(-2, out res));
            Assert.AreEqual(0, res);
            Assert.IsFalse(tree.TryGetPredecessor(0, out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TestTryGetSuccessor()
        {
            loadup();
            int res;
            Assert.IsTrue(tree!.TryGetSuccessor(7, out res) && res == 8);
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
            var view = tree!.GetViewBetween(6, 14);

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
            Assert.IsFalse(tree!.TryGetSuccessor(38, out res));
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
            Assert.IsTrue(tree!.TryGetFloor(7, out int res) && res == 6);
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
            var view = tree!.GetViewBetween(6, 14);

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
            Assert.IsFalse(tree!.TryGetFloor(-1, out int res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TestTryGetCeiling() // weak successor in C5, floor in JDK
        {
            loadup();
            Assert.IsTrue(tree!.TryGetCeiling(6, out int res) && res == 6);
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
            var view = tree!.GetViewBetween(6, 14);

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
            Assert.IsFalse(tree!.TryGetCeiling(39, out int res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TryGetCeiling_View_TooHigh()
        {
            loadup();
            var view = tree!.GetViewBetween(6, 14);

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
            SortedSet<int> aSubSet = tree!.GetViewBetween(objArray[startPos], lowerValueInclusive: true, objArray[endPos], upperValueInclusive: false);
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

#if FEATURE_SERIALIZABLE
        /// <summary>
        /// Tests that SortedSet instances serialized with J2N 2.1.0 can be deserialized
        /// correctly. This ensures backward compatibility with binary serialization.
        /// </summary>
        [Test]
        public void TestDeserializeLegacy_String_OrdinalComparer()
        {
            SortedSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sortedset-string-ordinal-v2.1.0.bin")!)
            {
                set = (SortedSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { "five", "four", "one", "three", "two" }; // Sorted lexographically
            var actualValues = set.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparer = StringComparer.Ordinal;
            var actualComparer = set.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_OrdinalIgnoreCaseComparer()
        {
            SortedSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sortedset-string-ordinalignorecase-v2.1.0.bin")!)
            {
                set = (SortedSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { "five", "four", "one", "three", "two" }; // Sorted lexographically
            var actualValues = set.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparer = StringComparer.OrdinalIgnoreCase;
            var actualComparer = set.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_InvariantComparer()
        {
            SortedSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sortedset-string-invariant-v2.1.0.bin")!)
            {
                set = (SortedSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { "five", "four", "one", "three", "two" }; // Sorted lexographically
            var actualValues = set.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparer = StringComparer.InvariantCulture;
            var actualComparer = set.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_CustomComparer()
        {
            SortedSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sortedset-string-customcomparer-v2.1.0.bin")!)
            {
                set = (SortedSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { "five", "four", "one", "three", "two" }; // Sorted lexographically
            var actualValues = set.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparerType = typeof(CustomStringComparer);
            var actualComparerType = set.Comparer.GetType();
            assertEquals(expectedComparerType, actualComparerType);
        }

        [Test]
        public void TestDeserializeLegacy_String_DefaultComparer_Empty()
        {
            SortedSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sortedset-string-defaultcomparer-empty-v2.1.0.bin")!)
            {
                set = (SortedSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(0, set.Count);

            var expectedComparer = Comparer<string>.Default; // J2N default comparer (ordinal)
            var actualComparer = set.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_Int32_CustomComparer()
        {
            SortedSet<int> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sortedset-int32-customcomparer-v2.1.0.bin")!)
            {
                set = (SortedSet<int>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { 1, 2, 3, 4, 5 };
            var actualValues = set.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparerType = typeof(CustomInt32Comparer);
            var actualComparerType = set.Comparer.GetType();
            assertEquals(expectedComparerType, actualComparerType);
        }

        [Test]
        public void TestDeserializeLegacy_Int32_DefaultComparer_Empty()
        {
            SortedSet<int> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sortedset-int32-defaultcomparer-empty-v2.1.0.bin")!)
            {
                set = (SortedSet<int>)formatter.Deserialize(stream);
            }

            assertEquals(0, set.Count);

            var expectedComparer = Comparer<int>.Default; // J2N default comparer
            var actualComparer = set.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestSerializeRoundTrip_String_CurrentCulture()
        {
            CultureInfo originalCulture = CultureInfo.CurrentCulture;

            try
            {
                // Culture A (creation + serialization)
                CultureInfo cultureA = CultureInfo.GetCultureInfo("tr-TR");
                CultureInfo cultureB = CultureInfo.GetCultureInfo("en-US");

                CultureInfo.CurrentCulture = cultureA;

                SortedSet<string> source = new SortedSet<string>(StringComparer.CurrentCulture)
                {
                    "I",
                    "ı", // dotless i
                    "i",
                    "İ"  // dotted I
                };

                byte[] blob;

#pragma warning disable SYSLIB0011 // BinaryFormatter obsolete
                var formatter = new BinaryFormatter();

                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, source);
                    blob = ms.ToArray();
                }
#pragma warning restore SYSLIB0011

                // Switch culture BEFORE deserialization
                CultureInfo.CurrentCulture = cultureB;

                SortedSet<string> roundTripped;

                using (var ms = new MemoryStream(blob))
                {
                    roundTripped = (SortedSet<string>)formatter.Deserialize(ms);
                }

                // 🔴 This is the key assertion
                // The comparer must NOT be tied to cultureB
                Assert.That(roundTripped.Comparer, Is.Not.EqualTo(StringComparer.CurrentCulture),
                    "Comparer incorrectly rebound to the current culture after deserialization.");

                // Verify behavior matches cultureA
                int compareResult = roundTripped.Comparer.Compare("I", "ı");

                Assert.That(compareResult, Is.EqualTo(
                    StringComparer.Create(cultureA, ignoreCase: false).Compare("I", "ı")),
                    "Comparer did not preserve original culture semantics.");

                // Extra sanity: ordering consistency
                string[] result = new string[roundTripped.Count];
                roundTripped.CopyTo(result);

                CollectionAssert.AreEqual(
                    source,
                    result,
                    "SortedSet order changed after round-trip serialization.");
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        // IMPORTANT: These are serialized with the J2N.Tests assembly name. So, they must always be available at that location.
        [Serializable]
        public class CustomStringComparer : IComparer<string>
        {
            public int Compare(string? x, string? y) => StringComparer.Ordinal.Compare(x, y);
        }

        [Serializable]
        public class CustomInt32Comparer : IComparer<int>
        {
            public int Compare(int x, int y) => x.CompareTo(y);
        }
#endif
    }
}
