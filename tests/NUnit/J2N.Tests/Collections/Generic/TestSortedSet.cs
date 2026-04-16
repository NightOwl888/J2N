// Partly based on: https://github.com/sestoft/C5/blob/master/C5.Tests/Trees/RedBlackTreeSetTests.cs#L857-L966
using J2N.Util;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using SCG = System.Collections.Generic;
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

        private void LoadForGetView()
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
            var view = tree!.GetView(6, 14);

            int res;
            Assert.IsTrue(view.TryGetPredecessor(9, out res) && res == 8);
            Assert.IsTrue(view.TryGetPredecessor(10, out res) && res == 8);

            // The bottom (relative to view)
            Assert.IsTrue(view.TryGetPredecessor(7, out res) && res == 6);

            // The top (relative to view)
            Assert.IsTrue(view.TryGetPredecessor(15, out res) && res == 14);
        }

        [Test]
        public void TestTryGetPredecessor_View_LowerExclusive_AtLowerBound()
        {
            loadup();

            // View is (6, 14] or (6, 14) depending on implementation
            var view = tree!.GetView(6, fromInclusive: false, 14, toInclusive: true);

            // 6 is excluded — predecessor should NOT exist
            bool found = view.TryGetPredecessor(6, out int res);

            Assert.IsFalse(found);
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TestTryGetPredecessor_View_LowerExclusive_AboveLowerBound()
        {
            loadup();

            var view = tree!.GetView(6, fromInclusive: false, 14, toInclusive: true);

            // 7 → predecessor would be 6, but 6 is excluded
            bool found = view.TryGetPredecessor(7, out int res);

            Assert.IsFalse(found);
            Assert.AreEqual(0, res);
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
            var view = tree!.GetView(6, 14);

            int res;
            Assert.IsTrue(view.TryGetSuccessor(9, out res) && res == 10);
            Assert.IsTrue(view.TryGetSuccessor(10, out res) && res == 12);

            // The bottom (relative to view)
            Assert.IsTrue(view.TryGetSuccessor(5, out res) && res == 6);

            // The top (relative to view)
            Assert.IsTrue(view.TryGetSuccessor(13, out res) && res == 14);
        }

        [Test]
        public void TestTryGetSuccessor_View_UpperExclusive_AtUpperBound()
        {
            loadup();

            var view = tree!.GetView(6, fromInclusive: true, 14, toInclusive: false);

            // 14 is excluded — successor should NOT exist
            bool found = view.TryGetSuccessor(14, out int res);

            Assert.IsFalse(found);
            Assert.AreEqual(0, res);
        }

        [Test]
        public void TestTryGetSuccessor_View_UpperExclusive_BelowUpperBound()
        {
            loadup();

            var view = tree!.GetView(6, fromInclusive: true, 14, toInclusive: false);

            // 13 → successor would be 14, but 14 is excluded
            bool found = view.TryGetSuccessor(13, out int res);

            Assert.IsFalse(found);
            Assert.AreEqual(0, res);
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
            var view = set.GetView(4, 7);

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
            var view = tree!.GetView(6, 14);

            Assert.IsTrue(view.TryGetFloor(9, out int res) && res == 8);
            Assert.IsTrue(view.TryGetFloor(10, out res) && res == 10);

            // The bottom
            Assert.IsTrue(view.TryGetFloor(6, out res) && res == 6);

            // The top
            Assert.IsTrue(view.TryGetFloor(15, out res) && res == 14);
            Assert.IsTrue(view.TryGetFloor(14, out res) && res == 14);
        }

        [Test]
        public void TestTryGetFloor_View_LowerExclusive()
        {
            loadup();

            var view = tree!.GetView(6, fromInclusive: false, 14, toInclusive: true);

            // Floor of 6 would be 6, but 6 is excluded
            bool found = view.TryGetFloor(6, out int res);

            Assert.IsFalse(found);
            Assert.AreEqual(0, res);
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
            var view = tree!.GetView(6, 14);

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
        public void TestTryGetCeiling_View_UpperExclusive()
        {
            loadup();

            var view = tree!.GetView(6, fromInclusive: true, 14, toInclusive: false);

            // Ceiling of 14 would be 14, but 14 is excluded
            bool found = view.TryGetCeiling(14, out int res);

            Assert.IsFalse(found);
            Assert.AreEqual(0, res);
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
            var view = tree!.GetView(6, 14);

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
        //    var range = set.GetView("B", false, "G", false);
        //    var count = range.Count;

        //}

        /**
         * @tests java.util.TreeSet#subSet(java.lang.Object, java.lang.Object)
         */
        [Test]
        public void Test_subSetLjava_lang_ObjectLjava_lang_Object()
        {
            LoadForGetView();

            // Test for method java.util.SortedSet
            // java.util.TreeSet.subSet(java.lang.Object, java.lang.Object)
            int startPos = objArray.Length / 4;
            int endPos = 3 * objArray.Length / 4;
            SortedSet<int> aSubSet = tree!.GetView(objArray[startPos], fromInclusive: true, objArray[endPos], toInclusive: false);
            assertTrue("Subset has wrong number of elements",
                    aSubSet.Count == (endPos - startPos));
            for (int counter = startPos; counter < endPos; counter++)
                assertTrue("Subset does not contain all the elements it should",
                        aSubSet.Contains(objArray[counter]));

            int result;
            try
            {
                tree.GetView(objArray[3], fromInclusive: true, objArray[0], toInclusive: false);
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

                // This is the key assertion
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


        [Test] // J2N: Regression test for BCL bug on Min (out of date)
        public void Test_Min_ViewReflectsUnderlyingMutation()
        {
            SortedSet<int> set = new() { 1, 2, 3, 4, 5 };

            SortedSet<int> view = set.GetView(2, 4);

            // Sanity check
            Assert.AreEqual(2, view.Min);

            // Mutate underlying set in a way that affects the view
            set.Remove(2);

            // BUG: Without VersionCheck() in MinInternal, this returns 2
            Assert.AreEqual(3, view.Min);
        }

        [Test] // J2N: Regression test for BCL bug on Max (out of date)
        public void Test_Max_ViewReflectsUnderlyingMutation()
        {
            SortedSet<string> set = new() { "1", "2", "3", "4", "5" };

            SortedSet<string> view = set.GetView("2", "4");

            // Mutate underlying set *outside* the view's range
            set.Clear();

            // BUG: Without VersionCheck() in MaxInternal, returns "4"
            Assert.AreEqual(null, view.Max);
        }

        #region Loading and Comparing

        [Test] // J2N: Regression for ToDistinctArray using EqualityComparer<T>.Default instead of comparer
        public void Test_Constructor_SortedCollection_CustomComparer_Deduplicates()
        {
            var comparer = new FirstCharComparer();

            // Sorted, but contains "duplicates" per comparer (same first char)
            SortedCollection<string> src = new(comparer)
            {
                "a1",
                "a2",
                "b1",
                "b2"
            };

            SortedSet<string> set = new(src, comparer);

            // Expected (correct behavior): deduplicated by comparer: one "a", one "b"
            // Actual (bug): all 4 elements remain because EqualityComparer<T>.Default is used
            CollectionAssert.AreEqual(new[] { "a1", "b1" }, set);
        }

        private sealed class FirstCharComparer : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                // only first char matters (forces "duplicates" for "a1"/"a2" and "b1"/"b2")
                // that disobey the rules of EqualityComparer<T>.Default, which considers all chars
                return x![0].CompareTo(y![0]);
            }

            public override bool Equals(object? obj)
            {
                return typeof(FirstCharComparer) == obj?.GetType();
            }

            public override int GetHashCode()
            {
                return typeof(FirstCharComparer).GetHashCode();
            }
        }

        [Test]
        public void Test_Constructor_BclSortedSet_WithSameComparer()
        {
            SCG.SortedSet<int> src = new() { 1, 2, 3 };
            SortedSet<int> target = new(src, Comparer<int>.Default);

            CollectionAssert.AreEqual(src, target);
        }

        [Test]
        public void Test_Constructor_BclSortedDictionaryKeys_WithMatchingComparer()
        {
            SCG.SortedDictionary<int, string> dict = new(Comparer<int>.Default)
            {
                [1] = "a",
                [2] = "b",
                [3] = "c"
            };

            SortedSet<int> target = new(dict.Keys, Comparer<int>.Default);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, target);
        }

        [Test]
        public void Test_Constructor_BclSortedDictionaryValues_Deduplicates()
        {
            SCG.SortedDictionary<int, int> dict = new()
            {
                [1] = 10,
                [2] = 10,
                [3] = 20
            };

            SortedSet<int> target = new(dict.Values, Comparer<int>.Default);

            CollectionAssert.AreEqual(new[] { 10, 20 }, target);
        }

        [Test]
        public void Test_Constructor_BclSortedDictionaryValues_RejectsNonDefaultComparer()
        {
            SCG.SortedDictionary<int, int> dict = new()
            {
                [1] = 10,
                [2] = 20
            };

            var reverse = SCG.Comparer<int>.Create((a, b) => b.CompareTo(a));

            SortedSet<int> set = new(dict.Values, reverse);

            CollectionAssert.AreEqual(new[] { 20, 10 }, set); // falls back to slow path
        }

        [Test]
        public void Test_Constructor_BclSortedDictionaryValuesWithDuplicates_UsesDistinctPath()
        {
            SCG.SortedDictionary<int, int> src = new()
            {
                [1] = 10,
                [2] = 10,
                [3] = 10
            };

            SortedSet<int> set = new(src.Values, Comparer<int>.Default);

            CollectionAssert.AreEqual(new[] { 10 }, set);
        }


        [Test]
        public void Test_Constructor_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> src = new() { 1, 2, 3 };
            SortedSet<int> target = new(src, Comparer<int>.Default);

            CollectionAssert.AreEqual(src, target);
        }

        [Test]
        public void Test_Constructor_J2NSortedDictionaryKeys_WithMatchingComparer()
        {
            SortedDictionary<int, string> dict = new(Comparer<int>.Default)
            {
                [1] = "a",
                [2] = "b",
                [3] = "c"
            };

            SortedSet<int> target = new(dict.Keys, Comparer<int>.Default);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, target);
        }

        [Test]
        public void Test_Constructor_J2NSortedDictionaryValues_Deduplicates()
        {
            SortedDictionary<int, int> dict = new()
            {
                [1] = 10,
                [2] = 10,
                [3] = 20
            };

            SortedSet<int> target = new(dict.Values, Comparer<int>.Default);

            CollectionAssert.AreEqual(new[] { 10, 20 }, target);
        }

        [Test]
        public void Test_Constructor_J2NSortedDictionaryValues_RejectsNonDefaultComparer()
        {
            SortedDictionary<int, int> dict = new()
            {
                [1] = 10,
                [2] = 20
            };

            var reverse = SCG.Comparer<int>.Create((a, b) => b.CompareTo(a));

            SortedSet<int> set = new(dict.Values, reverse);

            CollectionAssert.AreEqual(new[] { 20, 10 }, set); // falls back to slow path
        }

        [Test]
        public void Test_Constructor_J2NSortedDictionaryValuesWithDuplicates_UsesDistinctPath()
        {
            SortedDictionary<int, int> src = new()
            {
                [1] = 10,
                [2] = 10,
                [3] = 10
            };

            SortedSet<int> set = new(src.Values, Comparer<int>.Default);

            CollectionAssert.AreEqual(new[] { 10 }, set);
        }


        [Test]
        public void Test_Constructor_UnsortedEnumerable_FallsBackToSortAndDedup()
        {
            var src = new[] { 3, 1, 2, 2, 1 };
            SortedSet<int> set = new(src, Comparer<int>.Default);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, set);
        }


        [Test] // J2N: Regression for failure due to hard coding EqualityComparer<T>.Default
        public void Test_UnionWith_SortedNonDistinct_OrdinalIgnoreCase_Deduplicates()
        {
            SortedSet<string> left = new(StringComparer.OrdinalIgnoreCase)
            {
                "a"
            };

            SortedCollection<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A", "a"
            };

            left.UnionWith(right);

            Assert.That(left.SetEquals(new[] { "a" }));
        }

        [Test]
        public void Test_UnionWith_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 3 };
            SortedSet<int> right = new() { 2, 3 };

            left.UnionWith(right);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, left);
        }

        [Test]
        public void Test_UnionWith_BclSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 3 };
            SCG.SortedSet<int> right = new() { 2, 3 };

            left.UnionWith(right);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, left);
        }

        [Test]
        public void Test_UnionWith_J2NSortedDictionaryKeys()
        {
            SortedSet<int> left = new() { 1, 3 };

            SortedDictionary<int, string> dict = new()
            {
                [3] = "c",
                [2] = "b",
            };

            left.UnionWith(dict.Keys);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, left);
        }

        [Test]
        public void Test_UnionWith_J2NSortedDictionaryKeys_ThisAsView()
        {
            SortedSet<int> left = new() { 1, 3, 5, 7, 9 };
            SortedSet<int> leftView = left.GetView(1, 4);

            SortedDictionary<int, string> dict = new()
            {
                [3] = "c",
                [2] = "b",
            };

            leftView.UnionWith(dict.Keys);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, leftView);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 5, 7, 9 }, left);
        }

        [Test]
        public void Test_UnionWith_J2NSortedDictionaryValues_Deduplicates()
        {
            SortedSet<int> left = new() { 5 };

            SortedDictionary<int, int> dict = new()
            {
                [1] = 10,
                [2] = 10,
                [3] = 20
            };

            left.UnionWith(dict.Values);

            CollectionAssert.AreEqual(new[] { 5, 10, 20 }, left);
        }

        [Test]
        public void Test_UnionWith_SortedNonDistinct_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 4 };

            SortedDictionary<int, int> dict = new()
            {
                [1] = 2,
                [2] = 2,
                [3] = 3,
                [4] = 3
            };

            left.UnionWith(dict.Values);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, left);
        }

        [Test]
        public void Test_UnionWith_J2NSortedDictionaryValues_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 4 };

            SortedDictionary<int, int> dict = new()
            {
                [1] = 100,
                [2] = 1,
                [3] = 50,
                [4] = 2
            };

            left.UnionWith(dict.Values);

            CollectionAssert.AreEqual(new[] { 1, 2, 4, 50, 100 }, left);
        }

        [Test]
        public void Test_UnionWith_SortedNonDistinct_UsesOptimizedMergePath()
        {
            SortedSet<int> left = new() { 4, 1 };

            SortedCollection<int> nonDistinct = new()
            {
                1, 1, 1, 2, 2, 2, 3, 3,
            };

            left.UnionWith(nonDistinct);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, left);
        }


        [Test] // J2N: Regression for failure due to hard coding EqualityComparer<T>.Default
        public void Test_IntersectWith_SortedNonDistinct_OrdinalIgnoreCase_Deduplicates()
        {
            SortedSet<string> left = new(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b"
            };

            SortedCollection<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A", "a", "A", "a", "B", "b"
            };

            left.IntersectWith(right);

            SortedCollection<string> expected = new(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b"
            };

            Assert.That(left.SetEquals(expected));
        }

        [Test]
        public void Test_IntersectWith_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 3 };
            SortedSet<int> right = new() { 2, 3 };

            left.IntersectWith(right);

            CollectionAssert.AreEqual(new[] { 3 }, left);
        }

        [Test]
        public void Test_IntersectWith_BclSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 3 };
            SCG.SortedSet<int> right = new() { 2, 3 };

            left.IntersectWith(right);

            CollectionAssert.AreEqual(new[] { 3 }, left);
        }

        [Test]
        public void Test_IntersectWith_J2NSortedDictionaryKeys()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            SortedDictionary<int, string> dict = new()
            {
                [3] = "c",
                [4] = "d"
            };

            left.IntersectWith(dict.Keys);

            CollectionAssert.AreEqual(new[] { 3 }, left);
        }

        [Test]
        public void Test_IntersectWith_J2NSortedDictionaryValues_Deduplicates()
        {
            SortedSet<int> left = new() { 10, 20, 30 };

            SortedDictionary<int, int> dict = new()
            {
                [1] = 10,
                [2] = 10,
                [3] = 20,
                [4] = 20,
                [5] = 40
            };

            left.IntersectWith(dict.Values); // Fallback path

            CollectionAssert.AreEqual(new[] { 10, 20 }, left);
        }

        [Test]
        public void Test_IntersectWith_SortedNonDistinct_UsesOptimizedMergePath()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            SortedCollection<int> nonDistinct = new()
            {
                1, 1, 2, 2, 3, 3
            };

            left.IntersectWith(nonDistinct);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, left);
        }

        [Test]
        public void Test_IntersectWith_UnsortedEnumerable_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            int[] other = { 4, 3, 3, 5 };

            left.IntersectWith(other);

            CollectionAssert.AreEqual(new[] { 3, 4 }, left);
        }

        [Test]
        public void Test_IntersectWith_EmptyOther_ClearsSet()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            left.IntersectWith(Array.Empty<int>());

            CollectionAssert.IsEmpty(left);
        }

        [Test]
        public void Test_IntersectWith_Self_NoChange()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            left.IntersectWith(left);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, left);
        }

        [Test]
        public void Test_IntersectWith_TreeSubSet_InBounds_DistinctSorted()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedSet<int> other = new() { 1, 3, 5, 7 };

            subset.IntersectWith(other);

            CollectionAssert.AreEqual(new[] { 3, 5 }, subset);
            CollectionAssert.AreEqual(new[] { 1, 3, 5, 6 }, root);
        }

        [Test]
        public void Test_IntersectWith_TreeSubSet_InBounds_SortedNonDistinct()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedCollection<int> nonDistinct = new()
            {
                2, 2, 3, 3, 4, 4
            };

            subset.IntersectWith(nonDistinct);

            CollectionAssert.AreEqual(new[] { 2, 3, 4 }, subset);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 6 }, root);
        }



        [Test]
        public void Test_ExceptWith_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };
            SortedSet<int> right = new() { 2, 4 };

            left.ExceptWith(right);

            CollectionAssert.AreEqual(new[] { 1, 3 }, left);
        }

        [Test]
        public void Test_ExceptWith_BclSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };
            SCG.SortedSet<int> right = new() { 2, 4 };

            left.ExceptWith(right);

            CollectionAssert.AreEqual(new[] { 1, 3 }, left);
        }

        [Test]
        public void Test_ExceptWith_J2NSortedDictionaryKeys()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            SortedDictionary<int, string> dict = new()
            {
                [2] = "b",
                [4] = "d"
            };

            left.ExceptWith(dict.Keys);

            CollectionAssert.AreEqual(new[] { 1, 3 }, left);
        }

        [Test]
        public void Test_ExceptWith_IDistinctSortedCollection()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4, 5 };

            DistinctSortedCollection<int> other = new()
            {
                2, 4
            };

            left.ExceptWith(other);

            CollectionAssert.AreEqual(new[] { 1, 3, 5 }, left);
        }

        [Test]
        public void Test_ExceptWith_SortedNonDistinct_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            SortedCollection<int> nonDistinct = new()
            {
                2, 2, 4, 4
            };

            left.ExceptWith(nonDistinct);

            CollectionAssert.AreEqual(new[] { 1, 3 }, left);
        }

        [Test]
        public void Test_ExceptWith_UnsortedEnumerable_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            int[] other = { 4, 2, 2, 10 };

            left.ExceptWith(other);

            CollectionAssert.AreEqual(new[] { 1, 3 }, left);
        }

        [Test]
        public void Test_ExceptWith_EmptyOther_NoChange()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            left.ExceptWith(Array.Empty<int>());

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, left);
        }

        [Test]
        public void Test_ExceptWith_Self_Clears()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            left.ExceptWith(left);

            CollectionAssert.IsEmpty(left);
        }

        [Test]
        public void Test_ExceptWith_TreeSubSet_InBounds()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedSet<int> other = new() { 3, 5 };

            subset.ExceptWith(other);

            CollectionAssert.AreEqual(new[] { 2, 4 }, subset);
            CollectionAssert.AreEqual(new[] { 1, 2, 4, 6 }, root);
        }

        [Test]
        public void Test_ExceptWith_TreeSubSet_SortedNonDistinct()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedCollection<int> nonDistinct = new()
            {
                2, 2, 4, 4
            };

            subset.ExceptWith(nonDistinct);

            CollectionAssert.AreEqual(new[] { 3, 5 }, subset);
            CollectionAssert.AreEqual(new[] { 1, 3, 5, 6 }, root);
        }



        [Test]
        public void Test_SymmetricExceptWith_IDistinctSortedCollection_SameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4, 5 };

            DistinctSortedCollection<int> other = new()
            {
                2, 4, 6
            };

            left.SymmetricExceptWith(other);

            CollectionAssert.AreEqual(new[] { 1, 3, 5, 6 }, left);
        }

        [Test]
        public void Test_SymmetricExceptWith_SortedNonDistinct_SameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            SortedCollection<int> other = new()
            {
                2, 2, 3, 3, 5
            };

            left.SymmetricExceptWith(other);

            CollectionAssert.AreEqual(new[] { 1, 4, 5 }, left);
        }

        [Test]
        public void Test_SymmetricExceptWith_BclSortedSet_SameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };
            SCG.SortedSet<int> other = new() { 2, 5 };

            left.SymmetricExceptWith(other);

            CollectionAssert.AreEqual(new[] { 1, 3, 4, 5 }, left);
        }

        [Test]
        public void Test_SymmetricExceptWith_UnsortedEnumerable_Fallback()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            int[] other = { 4, 2, 2, 10 };

            left.SymmetricExceptWith(other);

            CollectionAssert.AreEqual(new[] { 1, 3, 10 }, left);
        }

        [Test]
        public void Test_SymmetricExceptWith_SortedDifferentComparer_Fallback()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a", "b", "c"
            };

            SortedSet<string> other = new(StringComparer.OrdinalIgnoreCase)
            {
                "B", "c", "d"
            };

            left.SymmetricExceptWith(other);

            CollectionAssert.AreEqual(new[] { "B", "a", "b", "d" }, left);
        }

        [Test]
        public void Test_SymmetricExceptWith_EmptyOther_NoChange()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            left.SymmetricExceptWith(Array.Empty<int>());

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, left);
        }

        [Test]
        public void Test_SymmetricExceptWith_Self_Clears()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            left.SymmetricExceptWith(left);

            CollectionAssert.IsEmpty(left);
        }

        [Test]
        public void Test_SymmetricExceptWith_TreeSubSet()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedSet<int> other = new() { 3, 6 };

            subset.SymmetricExceptWith(other);

            CollectionAssert.AreEqual(new[] { 2, 4, 5 }, subset);
            CollectionAssert.AreEqual(new[] { 1, 2, 4, 5, 6 }, root);
        }





        [Test] // Regression for BCL issue (GetViewBetween Throws)
        public void Test_IsSubsetOf_DoesNotThrow_WhenOtherSubsetBoundsDoNotOverlap()
        {
            var root = new SortedSet<int> { 10, 20, 30, 40, 50 };
            var thisSubset = root.GetView(20, 40);

            var otherRoot = new SortedSet<int> { 1, 2, 3, 4, 5 };
            var otherSubset = otherRoot.GetView(2, 4);

            Assert.DoesNotThrow(() =>
            {
                bool result = thisSubset.IsSubsetOf(otherSubset);
                Assert.False(result);
            });
        }

        [Test] // Regression for BCL issue (GetViewBetween Throws)
        public void Test_IsSubsetOf_SubsetOfSubset_WithOutOfRangeMinMax()
        {
            var root = new SortedSet<int> { 1, 2, 3, 4, 5, 6 };
            var subset1 = root.GetView(2, 5);   // [2..5]
            var subset2 = subset1.GetView(3, 4); // [3..4]

            var otherRoot = new SortedSet<int> { 0, 1, 2, 3 };
            var otherSubset = otherRoot.GetView(0, 2); // [0..2]

            bool result = subset2.IsSubsetOf(otherSubset);

            Assert.False(result);
        }

        [Test] // Regression for BCL issue (GetViewBetween Throws)
        public void Test_IsSubsetOf_SameComparer_NoOverlap_NoException()
        {
            var a = new SortedSet<int> { 5, 6, 7 };
            var b = new SortedSet<int> { 8, 9, 10, 11 };

            bool result = a.IsSubsetOf(b);

            Assert.False(result);
        }



        [Test]
        public void Test_IsSubsetOf_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 2, 4 };
            SortedSet<int> right = new() { 1, 2, 3, 4 };

            Assert.True(left.IsSubsetOf(right));
        }

        [Test]
        public void Test_IsSubsetOf_BclSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 2, 4 };
            SCG.SortedSet<int> right = new() { 1, 2, 3, 4 };

            Assert.True(left.IsSubsetOf(right));
        }

        [Test]
        public void Test_IsSubsetOf_J2NSortedDictionaryKeys()
        {
            SortedSet<int> left = new() { 2, 4 };

            SortedDictionary<int, string> dict = new()
            {
                [1] = "a",
                [2] = "b",
                [3] = "c",
                [4] = "d"
            };

            Assert.True(left.IsSubsetOf(dict.Keys));
        }

        [Test]
        public void Test_IsSubsetOf_IDistinctSortedCollection()
        {
            SortedSet<int> left = new() { 2, 4 };

            DistinctSortedCollection<int> other = new()
            {
                1, 2, 3, 4
            };

            Assert.True(left.IsSubsetOf(other));
        }

        [Test]
        public void Test_IsSubsetOf_SortedNonDistinct_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 2, 4 };

            SortedCollection<int> nonDistinct = new()
            {
                1, 2, 2, 3, 4, 4
            };

            Assert.True(left.IsSubsetOf(nonDistinct));
        }

        [Test]
        public void Test_IsSubsetOf_UnsortedEnumerable_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 2, 4 };

            int[] other = { 4, 2, 2, 10 };

            Assert.True(left.IsSubsetOf(other));
        }

        [Test]
        public void Test_IsSubsetOf_EmptyLeft_ReturnsTrue()
        {
            SortedSet<int> left = new();
            int[] other = { 1, 2, 3 };

            Assert.True(left.IsSubsetOf(other));
        }

        [Test]
        public void Test_IsSubsetOf_LeftLargerThanOther_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SortedSet<int> right = new() { 1, 2 };

            Assert.False(left.IsSubsetOf(right));
        }

        [Test]
        public void Test_IsSubsetOf_Self_ReturnsTrue()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            Assert.True(left.IsSubsetOf(left));
        }

        [Test]
        public void Test_IsSubsetOf_TreeSubSet_InBounds()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedSet<int> other = new() { 2, 3, 4, 5 };

            Assert.True(subset.IsSubsetOf(other));
        }

        [Test]
        public void Test_IsSubsetOf_TreeSubSet_SortedNonDistinct()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedCollection<int> nonDistinct = new()
            {
                2, 2, 3, 4, 4, 5
            };

            Assert.True(subset.IsSubsetOf(nonDistinct));
        }

        [Test]
        public void Test_IsSubsetOf_HashSet_StringComparerOrdinal()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a", "b"
            };

            HashSet<string> right = new(StringComparer.Ordinal)
            {
                "a", "b", "c"
            };

            Assert.True(left.IsSubsetOf(right));
        }

        [Test]
        public void Test_IsSubsetOf_HashSet_StringComparerMismatch_FallsBack()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a"
            };

            HashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A"
            };

            Assert.False(left.IsSubsetOf(right));
        }

        [Test]
        public void Test_IsSubsetOf_OrderedHashSet_StringComparer()
        {
            SortedSet<string> left = new(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b"
            };

            OrderedHashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A", "B", "C"
            };

            Assert.True(left.IsSubsetOf(right));
        }

        [Test]
        public void Test_IsSubsetOf_BclHashSet_StringComparer()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a", "b"
            };

            SCG.HashSet<string> right = new(StringComparer.Ordinal)
            {
                "a", "b", "c"
            };

            Assert.True(left.IsSubsetOf(right));
        }

        [Test]
        public void Test_IsSubsetOf_HashSet_NonString_NoComparerOptimization()
        {
            SortedSet<int> left = new()
            {
                1, 2
            };

            HashSet<int> right = new()
            {
                1, 2, 3
            };

            Assert.True(left.IsSubsetOf(right)); // fallback path
        }



        [Test]
        public void Test_IsProperSubsetOf_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 2, 4 };
            SortedSet<int> right = new() { 1, 2, 3, 4 };

            Assert.True(left.IsProperSubsetOf(right));
        }

        [Test]
        public void Test_IsProperSubsetOf_BclSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 2, 4 };
            SCG.SortedSet<int> right = new() { 1, 2, 3, 4 };

            Assert.True(left.IsProperSubsetOf(right));
        }

        [Test]
        public void Test_IsProperSubsetOf_J2NSortedDictionaryKeys()
        {
            SortedSet<int> left = new() { 2, 4 };

            SortedDictionary<int, string> dict = new()
            {
                [1] = "a",
                [2] = "b",
                [3] = "c",
                [4] = "d"
            };

            Assert.True(left.IsProperSubsetOf(dict.Keys));
        }

        [Test]
        public void Test_IsProperSubsetOf_IDistinctSortedCollection()
        {
            SortedSet<int> left = new() { 2, 4 };

            DistinctSortedCollection<int> other = new()
            {
                1, 2, 3, 4
            };

            Assert.True(left.IsProperSubsetOf(other));
        }

        [Test]
        public void Test_IsProperSubsetOf_SortedNonDistinct_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 2, 4 };

            SortedCollection<int> nonDistinct = new()
            {
                1, 2, 2, 3, 4, 4
            };

            Assert.True(left.IsProperSubsetOf(nonDistinct));
        }

        [Test] // J2N: Regression for not ensuring Count >= other.Count
        public void Test_IsProperSubsetOf_SortedNonDistinct_SameDistinctElements_ReturnsFalse()
        {
            SortedSet<int> left = new() { 2, 4 };

            SortedCollection<int> nonDistinct = new()
            {
                2, 2, 4, 4
            };

            Assert.False(left.IsProperSubsetOf(nonDistinct));
        }

        [Test]
        public void Test_IsProperSubsetOf_UnsortedEnumerable_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 2, 4 };

            int[] other = { 4, 2, 2, 10 };

            Assert.True(left.IsProperSubsetOf(other));
        }

        [Test]
        public void Test_IsProperSubsetOf_EmptyLeft_ReturnsTrue()
        {
            SortedSet<int> left = new();
            SortedSet<int> right = new() { 1 };

            Assert.True(left.IsProperSubsetOf(right));
        }

        [Test]
        public void Test_IsProperSubsetOf_EqualSets_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SortedSet<int> right = new() { 1, 2, 3 };

            Assert.False(left.IsProperSubsetOf(right));
        }

        [Test]
        public void Test_IsProperSubsetOf_Self_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            Assert.False(left.IsProperSubsetOf(left));
        }

        [Test]
        public void Test_IsProperSubsetOf_TreeSubSet_InBounds_ReturnsTrue()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedSet<int> other = new() { 1, 2, 3, 4, 5, 6 };

            Assert.True(subset.IsProperSubsetOf(other));
        }

        [Test]
        public void Test_IsProperSubsetOf_TreeSubSet_SortedNonDistinct_ReturnsTrue()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5, 6 };
            SortedSet<int> subset = root.GetView(2, 5);

            SortedCollection<int> nonDistinct = new()
            {
                1, 2, 2, 3, 4, 4, 5, 6
            };

            Assert.True(subset.IsProperSubsetOf(nonDistinct));
        }

        [Test]
        public void Test_IsProperSubsetOf_HashSet_StringComparerOrdinalIgnoreCase_ProperSubSet_ReturnsTrue()
        {
            SortedSet<string> left = new(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b"
            };

            HashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A", "B", "C"
            };

            Assert.True(left.IsProperSubsetOf(right));
        }

        [Test]
        public void IsProperSubsetOf_HashSet_SameStringComparer_EqualSets_ReturnsFalse()
        {
            SortedSet<string> set = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b", "c"
            };

            HashSet<string> other = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "A", "B", "C"
            };

            Assert.IsFalse(set.IsProperSubsetOf(other));
        }

        [Test]
        public void Test_IsProperSubsetOf_OrderedHashSet_StringComparerOrdinalIgnoreCase_ProperSubSet_ReturnsTrue()
        {
            SortedSet<string> left = new(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b"
            };

            OrderedHashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A", "B", "C"
            };

            Assert.True(left.IsProperSubsetOf(right));
        }

        [Test]
        public void IsProperSubsetOf_OrderedHashSet_SameStringComparer_EqualSets_ReturnsFalse()
        {
            SortedSet<string> set = new(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b", "c"
            };

            OrderedHashSet<string> other = new(StringComparer.OrdinalIgnoreCase)
            {
                "A", "B", "C"
            };

            Assert.IsFalse(set.IsProperSubsetOf(other));
        }

        [Test]
        public void IsProperSubsetOf_BclSortedSet_SameStringComparer_ProperSubset_ReturnsTrue()
        {
            StringComparer comparer = StringComparer.Ordinal;

            SortedSet<string> set = new SortedSet<string>(comparer)
            {
                "a", "b"
            };

            SCG.SortedSet<string> other = new(comparer)
            {
                "a", "b", "c"
            };

            Assert.IsTrue(set.IsProperSubsetOf(other));
        }

        [Test]
        public void IsProperSubsetOf_BclSortedSet_View_SameStringComparer_UsesFallback()
        {
            StringComparer comparer = StringComparer.Ordinal;

            SCG.SortedSet<string> root = new(comparer)
            {
                "a", "b", "c", "d"
            };

            SCG.SortedSet<string> view = root.GetViewBetween("b", "c");

            SortedSet<string> set = new SortedSet<string>(comparer)
            {
                "b"
            };

            Assert.IsTrue(set.IsProperSubsetOf(view));
        }




        [Test]
        public void Test_IsSupersetOf_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };
            SortedSet<int> right = new() { 2, 4 };

            Assert.True(left.IsSupersetOf(right));
        }

        [Test]
        public void Test_IsSupersetOf_BclSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };
            SCG.SortedSet<int> right = new() { 2, 4 };

            Assert.True(left.IsSupersetOf(right));
        }

        [Test]
        public void Test_IsSupersetOf_J2NSortedDictionaryKeys()
        {
            SortedDictionary<int, string> dict = new()
            {
                [1] = "a",
                [2] = "b",
                [3] = "c",
                [4] = "d"
            };

            SortedSet<int> set = new() { 1, 2, 3, 4 };

            Assert.True(set.IsSupersetOf(dict.Keys));
        }

        [Test]
        public void Test_IsSupersetOf_IDistinctSortedCollection()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            DistinctSortedCollection<int> right = new()
            {
                2, 4
            };

            Assert.True(left.IsSupersetOf(right));
        }

        [Test]
        public void Test_IsSupersetOf_SortedNonDistinct_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            SortedCollection<int> nonDistinct = new()
            {
                2, 2, 4, 4
            };

            Assert.True(left.IsSupersetOf(nonDistinct));
        }

        [Test]
        public void Test_IsSupersetOf_UnsortedEnumerable_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            int[] other = { 4, 2, 2 };

            Assert.True(left.IsSupersetOf(other));
        }

        [Test]
        public void Test_IsSupersetOf_EmptyOther_ReturnsTrue()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            int[] other = Array.Empty<int>();

            Assert.True(left.IsSupersetOf(other));
        }

        [Test]
        public void Test_IsSupersetOf_OtherLargerThanLeft_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2 };
            SortedSet<int> right = new() { 1, 2, 3 };

            Assert.False(left.IsSupersetOf(right));
        }

        [Test]
        public void Test_IsSupersetOf_Self_ReturnsTrue()
        {
            SortedSet<int> set = new() { 1, 2, 3 };

            Assert.True(set.IsSupersetOf(set));
        }

        [Test]
        public void Test_IsSupersetOf_TreeSubSet_FallsBackCorrectly()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5 };
            SortedSet<int> subset = root.GetView(2, 4);

            SortedSet<int> set = new() { 1, 2, 3, 4, 5 };

            Assert.True(set.IsSupersetOf(subset));
        }

        [Test]
        public void Test_IsSupersetOf_HashSet_StringComparerOrdinal()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a", "b", "c"
            };

            HashSet<string> right = new(StringComparer.Ordinal)
            {
                "a", "b"
            };

            Assert.True(left.IsSupersetOf(right));
        }

        [Test]
        public void Test_IsSupersetOf_HashSet_StringComparerMismatch_FallsBack()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a"
            };

            HashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A"
            };

            Assert.False(left.IsSupersetOf(right));
        }

        [Test]
        public void Test_IsSupersetOf_OrderedHashSet_StringComparer()
        {
            SortedSet<string> left = new(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b", "c"
            };

            OrderedHashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A", "B"
            };

            Assert.True(left.IsSupersetOf(right));
        }

        [Test]
        public void Test_IsSupersetOf_BclHashSet_StringComparer()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a", "b", "c"
            };

            SCG.HashSet<string> right = new(StringComparer.Ordinal)
            {
                "a", "b"
            };

            Assert.True(left.IsSupersetOf(right));
        }


        [Test]
        public void Test_IsProperSupersetOf_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };
            SortedSet<int> right = new() { 2, 4 };

            Assert.True(left.IsProperSupersetOf(right));
        }

        [Test]
        public void Test_IsProperSupersetOf_BclSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };
            SCG.SortedSet<int> right = new() { 2, 4 };

            Assert.True(left.IsProperSupersetOf(right));
        }

        [Test]
        public void Test_IsProperSupersetOf_J2NSortedDictionaryKeys()
        {
            SortedDictionary<int, string> dict = new()
            {
                [2] = "b",
                [4] = "d"
            };

            SortedSet<int> set = new() { 1, 2, 3, 4 };

            Assert.True(set.IsProperSupersetOf(dict.Keys));
        }

        [Test]
        public void Test_IsProperSupersetOf_IDistinctSortedCollection()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            DistinctSortedCollection<int> right = new()
            {
                2, 4
            };

            Assert.True(left.IsProperSupersetOf(right));
        }

        [Test]
        public void Test_IsProperSupersetOf_SortedNonDistinct_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            SortedCollection<int> nonDistinct = new()
            {
                2, 2, 4, 4
            };

            Assert.True(left.IsProperSupersetOf(nonDistinct));
        }

        [Test] // J2N: Regression for not ensuring other.Count >= Count
        public void Test_IsProperSupersetOf_SortedNonDistinct_SameDistinctElements_ReturnsFalse()
        {
            SortedSet<int> left = new() { 2, 4 };

            SortedCollection<int> nonDistinct = new()
            {
                2, 2, 4, 4
            };

            Assert.False(left.IsProperSupersetOf(nonDistinct));
        }

        [Test]
        public void Test_IsProperSupersetOf_UnsortedEnumerable_FallsBackCorrectly()
        {
            SortedSet<int> left = new() { 1, 2, 3, 4 };

            int[] other = { 4, 2 };

            Assert.True(left.IsProperSupersetOf(other));
        }

        [Test]
        public void Test_IsProperSupersetOf_EqualSets_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SortedSet<int> right = new() { 1, 2, 3 };

            Assert.False(left.IsProperSupersetOf(right));
        }

        [Test]
        public void Test_IsProperSupersetOf_Self_ReturnsFalse()
        {
            SortedSet<int> set = new() { 1, 2, 3 };

            Assert.False(set.IsProperSupersetOf(set));
        }

        [Test]
        public void Test_IsProperSupersetOf_TreeSubSet_FallsBackCorrectly()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5 };
            SortedSet<int> subset = root.GetView(2, 4);

            SortedSet<int> set = new() { 1, 2, 3, 4, 5 };

            Assert.True(set.IsProperSupersetOf(subset));
        }

        [Test]
        public void Test_IsProperSupersetOf_HashSet_StringComparerOrdinalIgnoreCase()
        {
            SortedSet<string> left = new(StringComparer.OrdinalIgnoreCase)
            {
                "a", "b", "c"
            };

            HashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A", "B"
            };

            Assert.True(left.IsProperSupersetOf(right));
        }

        [Test]
        public void Test_IsProperSupersetOf_BclSortedSet_View_SameComparer_UsesFallback()
        {
            StringComparer comparer = StringComparer.Ordinal;

            SCG.SortedSet<string> root = new(comparer)
            {
                "a", "b", "c", "d"
            };

            SCG.SortedSet<string> view = root.GetViewBetween("b", "c");

            SortedSet<string> set = new SortedSet<string>(comparer)
            {
                "a", "b", "c", "d"
            };

            Assert.True(set.IsProperSupersetOf(view));
        }


        [Test]
        public void Test_SetEquals_J2NSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SortedSet<int> right = new() { 1, 2, 3 };

            Assert.True(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_BclSortedSet_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SCG.SortedSet<int> right = new() { 1, 2, 3 };

            Assert.True(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_IDistinctSortedCollection()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            DistinctSortedCollection<int> right = new()
            {
                1, 2, 3
            };

            Assert.True(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_SortedNonDistinct_WithSameComparer()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            SortedCollection<int> right = new()
            {
                1, 1, 2, 2, 3, 3
            };

            Assert.True(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_SortedNonDistinct_ExtraElement_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };

            SortedCollection<int> right = new()
            {
                1, 2, 3, 4, 4
            };

            Assert.False(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_HashSet_StringComparerOrdinal()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a", "b", "c"
            };

            HashSet<string> right = new(StringComparer.Ordinal)
            {
                "c", "b", "a"
            };

            Assert.True(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_HashSet_StringComparerMismatch_ReturnsFalse()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a"
            };

            HashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A"
            };

            Assert.False(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_OrderedHashSet_StringComparerOrdinal()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a", "b", "c"
            };

            OrderedHashSet<string> right = new(StringComparer.Ordinal)
            {
                "c", "b", "a"
            };

            Assert.True(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_OrderedHashSet_StringComparerMismatch_ReturnsFalse()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a"
            };

            OrderedHashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A"
            };

            Assert.False(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_BclHashSet_StringComparerOrdinal()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a", "b", "c"
            };

            SCG.HashSet<string> right = new(StringComparer.Ordinal)
            {
                "c", "b", "a"
            };

            Assert.True(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_BclHashSet_StringComparerMismatch_ReturnsFalse()
        {
            SortedSet<string> left = new(StringComparer.Ordinal)
            {
                "a"
            };

            SCG.HashSet<string> right = new(StringComparer.OrdinalIgnoreCase)
            {
                "A"
            };

            Assert.False(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_Self_ReturnsTrue()
        {
            SortedSet<int> set = new() { 1, 2, 3 };

            Assert.True(set.SetEquals(set));
        }

        [Test]
        public void Test_SetEquals_BothEmpty_ReturnsTrue()
        {
            SortedSet<int> left = new();
            SortedSet<int> right = new();

            Assert.True(left.SetEquals(right));
        }

        [Test]
        public void Test_SetEquals_EmptyAndNonEmpty_ReturnsFalse()
        {
            SortedSet<int> left = new();
            int[] right = { 1 };

            Assert.False(left.SetEquals(right));
        }



        [Test]
        public void Test_Overlaps_J2NSortedSet_WithOverlap_ReturnsTrue()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SortedSet<int> right = new() { 3, 4, 5 };

            Assert.True(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_J2NSortedSet_NoOverlap_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SortedSet<int> right = new() { 4, 5, 6 };

            Assert.False(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_BclSortedSet_WithOverlap_ReturnsTrue()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SCG.SortedSet<int> right = new() { 3, 4 };

            Assert.True(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_BclSortedSet_NoOverlap_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SCG.SortedSet<int> right = new() { 4, 5 };

            Assert.False(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_SortedDictionaryKeys_WithOverlap_ReturnsTrue()
        {
            SortedDictionary<int, string> dict = new()
            {
                [1] = "a",
                [2] = "b",
                [3] = "c"
            };

            SortedSet<int> set = new() { 3, 4, 5 };

            Assert.True(set.Overlaps(dict.Keys));
        }

        [Test]
        public void Test_Overlaps_SortedDictionaryKeys_NoOverlap_ReturnsFalse()
        {
            SortedDictionary<int, string> dict = new()
            {
                [4] = "a",
                [5] = "b"
            };

            SortedSet<int> set = new() { 1, 2, 3 };

            Assert.False(set.Overlaps(dict.Keys));
        }


        [Test]
        public void Test_Overlaps_SortedDictionary_WithOverlap_ReturnsTrue()
        {
            SortedDictionary<int, string> dict = new()
            {
                [1] = "a",
                [2] = "b"
            };

            // J2N: KeyValuePair doesn't implement IComparable<T> so we have to shoehorn the SortedDictionary<TKey, TValue> comparer
            // into this set for them to match. This is just proof that Overlaps() can be called inside of SortedDictionary<TKey, TValue>
            // which may be used as a future optimization.
            SortedSet<KeyValuePair<int, string>> set = new(new SortedDictionary<int, string>.KeyValuePairComparer(dict.Comparer))
            {
                new(2, "b"),
                new(3, "c")
            };

            Assert.True(set.Overlaps(dict));
        }

        [Test]
        public void Test_Overlaps_SortedDictionary_NoOverlap_ReturnsFalse()
        {
            SortedDictionary<int, string> dict = new()
            {
                [1] = "a"
            };

            // J2N: KeyValuePair doesn't implement IComparable<T> so we have to shoehorn the SortedDictionary<TKey, TValue> comparer
            // into this set for them to match. This is just proof that Overlaps() can be called inside of SortedDictionary<TKey, TValue>
            // which may be used as a future optimization.
            SortedSet<KeyValuePair<int, string>> set = new(new SortedDictionary<int, string>.KeyValuePairComparer(dict.Comparer))
            {
                new(2, "b")
            };

            Assert.False(set.Overlaps(dict));
        }

        [Test]
        public void Test_Overlaps_ViewAsThis_WithOverlap_ReturnsTrue()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5 };
            SortedSet<int> view = root.GetView(2, 4);

            SortedSet<int> other = new() { 4, 6 };

            Assert.True(view.Overlaps(other));
        }

        [Test]
        public void Test_Overlaps_ViewAsThis_NoOverlap_ReturnsFalse()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5 };
            SortedSet<int> view = root.GetView(2, 3);

            SortedSet<int> other = new() { 4, 5 };

            Assert.False(view.Overlaps(other));
        }

        [Test]
        public void Test_Overlaps_ViewAsOther_WithOverlap_ReturnsTrue()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5 };
            SortedSet<int> view = root.GetView(3, 5);

            SortedSet<int> set = new() { 2, 3 };

            Assert.True(set.Overlaps(view));
        }

        [Test]
        public void Test_Overlaps_ViewAsOther_NoOverlap_ReturnsFalse()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5 };
            SortedSet<int> view = root.GetView(4, 5);

            SortedSet<int> set = new() { 1, 2 };

            Assert.False(set.Overlaps(view));
        }

        [Test]
        public void Test_Overlaps_BothViews_WithOverlap_ReturnsTrue()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5 };

            SortedSet<int> left = root.GetView(2, 4);
            SortedSet<int> right = root.GetView(4, 5);

            Assert.True(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_BothViews_NoOverlap_ReturnsFalse()
        {
            SortedSet<int> root = new() { 1, 2, 3, 4, 5 };

            SortedSet<int> left = root.GetView(1, 2);
            SortedSet<int> right = root.GetView(4, 5);

            Assert.False(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_HashSet_WithOverlap_ReturnsTrue()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            HashSet<int> right = new() { 3, 4 };

            Assert.True(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_HashSet_NoOverlap_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            HashSet<int> right = new() { 4, 5 };

            Assert.False(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_OrderedHashSet_WithOverlap_ReturnsTrue()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            OrderedHashSet<int> right = new() { 3, 4 };

            Assert.True(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_OrderedHashSet_NoOverlap_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            OrderedHashSet<int> right = new() { 4, 5 };

            Assert.False(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_BclHashSet_WithOverlap_ReturnsTrue()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SCG.HashSet<int> right = new() { 3, 4 };

            Assert.True(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_BclHashSet_NoOverlap_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2, 3 };
            SCG.HashSet<int> right = new() { 4, 5 };

            Assert.False(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_EmptyThis_ReturnsFalse()
        {
            SortedSet<int> left = new();
            SortedSet<int> right = new() { 1 };

            Assert.False(left.Overlaps(right));
        }

        [Test]
        public void Test_Overlaps_EmptyOther_ReturnsFalse()
        {
            SortedSet<int> left = new() { 1, 2 };
            int[] other = Array.Empty<int>();

            Assert.False(left.Overlaps(other));
        }

        [Test]
        public void Test_Overlaps_Self_ReturnsTrue()
        {
            SortedSet<int> set = new() { 1, 2, 3 };

            Assert.True(set.Overlaps(set));
        }


        #endregion Loading and Comparing

        [Test]
        public void Test_headSet_descendingSet()
        {
            SortedSet<int> set = new() { 1, 2, 3, 4, 5 };

            SortedSet<int> ascending = set.GetViewBefore(4, true);
            SortedSet<int> descending = ascending.GetViewDescending();

            // Different iteration order
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, ascending.ToArray());
            CollectionAssert.AreEqual(new int[] { 4, 3, 2, 1 }, descending.ToArray());
        }

        [Test]
        public void Test_descendingSet_headSet()
        {
            SortedSet<int> set = new() { 1, 2, 3, 4, 5 };

            SortedSet<int> ascending = set.GetViewBefore(4, true);
            SortedSet<int> descending = set.GetViewDescending().GetViewBefore(4, true);

            // Different iteration order
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, ascending.ToArray());
            CollectionAssert.AreEqual(new int[] { 5, 4 }, descending.ToArray());
        }

        [Test]
        public void Test_tailSet_descendingSet()
        {
            SortedSet<int> set = new() { 1, 2, 3, 4, 5 };

            SortedSet<int> ascending = set.GetViewAfter(4, true);
            SortedSet<int> descending = ascending.GetViewDescending();

            // Different iteration order
            CollectionAssert.AreEqual(new int[] { 4, 5 }, ascending.ToArray());
            CollectionAssert.AreEqual(new int[] { 5, 4 }, descending.ToArray());
        }

        [Test]
        public void Test_descendingSet_tailSet()
        {
            SortedSet<int> set = new() { 1, 2, 3, 4, 5 };

            SortedSet<int> ascending = set.GetViewAfter(4, true);
            SortedSet<int> descending = set.GetViewDescending().GetViewAfter(4, true);

            // Different iteration order
            CollectionAssert.AreEqual(new int[] { 4, 5 }, ascending.ToArray());
            CollectionAssert.AreEqual(new int[] { 4, 3, 2, 1 }, descending.ToArray());
        }

        // Edge cases
        [Test]
        public void Test_GetView_GetViewDescending_GetViewBefore_MatchesSubset()
        {
            SortedSet<int> set = new SortedSet<int> { 1, 2, 3, 4, 5 };

            SortedSet<int> result = set.GetView(2, true, 5, true)
                            .GetViewDescending()
                            .GetViewBefore(4, true);

            CollectionAssert.AreEqual(new[] { 5, 4 }, result.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewBefore_Exclusive_MatchesSubset()
        {
            SortedSet<int> set = new SortedSet<int> { 1, 2, 3, 4, 5 };

            SortedSet<int> result = set.GetViewDescending().GetViewBefore(3, false);

            CollectionAssert.AreEqual(new[] { 5, 4 }, result.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewDescending_MatchesSet()
        {
            SortedSet<int> set = new SortedSet<int> { 1, 2, 3, 4, 5 };

            SortedSet<int> result = set.GetViewDescending().GetViewDescending();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, result.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewAfter_OutOfRange_Lower_Empty()
        {
            SortedSet<int> set = new SortedSet<int> { 1, 2, 3, 4, 5 };

            SortedSet<int> descending = set.GetViewDescending();

            SortedSet<int> tail = descending.GetViewAfter(0, true);

            CollectionAssert.AreEqual(Arrays.Empty<int>(), tail.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewBefore_OutOfRange_Lower_Unchanged()
        {
            SortedSet<int> set = new SortedSet<int> { 1, 2, 3, 4, 5 };

            SortedSet<int> descending = set.GetViewDescending();

            SortedSet<int> head = descending.GetViewBefore(0, true);

            CollectionAssert.AreEqual(new[] { 5, 4, 3, 2, 1 }, head.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewAfter_GetViewBefore_OutOfRange_Higher_Throws()
        {
            SortedSet<int> set = new SortedSet<int> { 1, 2, 3, 4, 5 };

            SortedSet<int> descending = set.GetViewDescending();

            SortedSet<int> tail = descending.GetViewAfter(3, true);

            Assert.Throws<ArgumentOutOfRangeException>(() => tail.GetViewBefore(4, true));
        }

        [Test]
        public void Test_GetView_RangeOutsideOfBaseSet_Empty()
        {
            SortedSet<string> set = new SortedSet<string>(StringComparer.Ordinal) { "1", "2", "3" };

            SortedSet<string> view = set.GetView("4", "9");
            Assert.AreEqual(0, view.Count);

            var lookup = set.GetSpanAlternateLookup<char>();
            SortedSet<string> lookupView = lookup.GetView("4".AsSpan(), "9".AsSpan());
            Assert.AreEqual(0, lookupView.Count);
        }

        [Test]
        public void Test_GetView_Exclusive_Exclusive_SameValue_Empty()
        {
            SortedSet<string> set = new SortedSet<string>(StringComparer.Ordinal) { "1", "2", "3", "4", "5" };

            SortedSet<string> view = set.GetView("3", false, "3", false);
            Assert.AreEqual(0, view.Count);

            var lookup = set.GetSpanAlternateLookup<char>();
            SortedSet<string> lookupView = lookup.GetView("3".AsSpan(), false, "3".AsSpan(), false);
            Assert.AreEqual(0, lookupView.Count);
        }

        [Test]
        public void Test_GetView_GetView_Exclusive_Exclusive_SameValue_Empty()
        {
            SortedSet<string> set = new SortedSet<string>(StringComparer.Ordinal) { "1", "2", "3", "4", "5" };

            SortedSet<string> view1 = set.GetView("2", "4");

            SortedSet<string> view2 = view1.GetView("3", false, "3", false);
            Assert.AreEqual(0, view2.Count);

            var lookup = view1.GetSpanAlternateLookup<char>();
            SortedSet<string> lookupView = lookup.GetView("3".AsSpan(), false, "3".AsSpan(), false);
            Assert.AreEqual(0, lookupView.Count);
        }


        // More cases

        //------------------------------------------------------------
        // 1. HEADSET (GetViewBefore) - Ascending vs Descending
        // ------------------------------------------------------------

        [Test]
        public void Test_headSet_ascending_validRange()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            SortedSet<int> view = set.GetViewBefore(5, false);

            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4 }, view.ToArray());

            SortedSet<int> descending = set.GetViewDescending().GetViewBefore(5, false);
            SortedSet<int> inverse = set.GetViewAfter(5);

            CollectionAssert.AreEqual(new[] { 9, 8, 7, 6 }, descending.ToArray());
            CollectionAssert.AreEqual(new[] { 5, 6, 7, 8, 9 }, inverse.ToArray());
        }

        [Test]
        public void Test_headSet_descending_validRange()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> view = desc.GetViewBefore(5, false);

            // In descending view, "head" means values BEFORE 5 in descending order
            CollectionAssert.AreEqual(new[] { 9, 8, 7, 6 }, view.ToArray());
        }

        [Test]
        public void Test_headSet_descending_inclusive()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> view = desc.GetViewBefore(5, true);

            CollectionAssert.AreEqual(new[] { 9, 8, 7, 6, 5 }, view.ToArray());
        }

        // ------------------------------------------------------------
        // 2. TAILSET (GetViewAfter) - Ascending vs Descending
        // ------------------------------------------------------------

        [Test]
        public void Test_tailSet_ascending_validRange()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            SortedSet<int> view = set.GetViewAfter(5);

            CollectionAssert.AreEqual(new[] { 5, 6, 7, 8, 9 }, view.ToArray());
        }

        [Test]
        public void Test_tailSet_descending_validRange()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> view = desc.GetViewAfter(5);

            // In descending view, "tail" means <= 5 in descending order
            CollectionAssert.AreEqual(new[] { 5, 4, 3, 2, 1, 0 }, view.ToArray());
        }

        [Test]
        public void Test_tailSet_descending_inclusive()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> view = desc.GetViewAfter(5, true);

            CollectionAssert.AreEqual(new[] { 5, 4, 3, 2, 1, 0 }, view.ToArray());
        }

        // ------------------------------------------------------------
        // 3. CRITICAL: Exception behavior (nested views)
        // ------------------------------------------------------------

        [Test]
        public void Test_headSet_ascending_outOfRange_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            SortedSet<int> view = set.GetViewBefore(5, false);

            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(7, false));
        }

        [Test]
        public void Test_headSet_descending_outOfRange_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> view = desc.GetViewBefore(5, false);

            // KEY TEST: which side is "out of range"?
            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(3, false));
        }

        [Test]
        public void Test_tailset_ascending_outOfRange_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> view = set.GetViewAfter(5, false);
            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(3, false));
        }

        [Test]
        public void Test_tailSet_descending_outOfRange_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();
            SortedSet<int> view = desc.GetViewAfter(5, false);
            // KEY TEST
            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(7, false)); // <-- critical for direction semantics
        }

        // ------------------------------------------------------------
        // 4. CROSS-DIRECTION sanity checks
        // ------------------------------------------------------------

        [Test]
        public void Test_descending_headSet_then_tailSet_behavior()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> head = desc.GetViewBefore(5, false); // [9..6]
            SortedSet<int> tail = head.GetViewAfter(7);

            CollectionAssert.AreEqual(new[] { 7, 6 }, tail.ToArray());
        }

        [Test]
        public void Test_descending_tailSet_then_headSet_behavior()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> tail = desc.GetViewAfter(5); // [5..0]
            SortedSet<int> head = tail.GetViewBefore(3, false);

            CollectionAssert.AreEqual(new[] { 5, 4 }, head.ToArray());
        }

        [Test]
        public void Test_subSet_chaining_three_levels()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            SortedSet<int> a = set.GetView(2, true, 9, false);   // [2..8]
            SortedSet<int> b = a.GetView(3, true, 7, false);     // [3..6]
            SortedSet<int> c = b.GetView(4, true, 6, false);     // [4..5]

            CollectionAssert.AreEqual(new[] { 4, 5 }, c.ToArray());
        }

        [Test]
        public void Test_descending_multi_chain()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> a = desc.GetViewBefore(7, false);    // [9..8]
            SortedSet<int> b = a.GetViewAfter(8);               // [8]

            CollectionAssert.AreEqual(new[] { 8 }, b.ToArray());
        }

        [Test]
        public void Test_mixed_subset_head_tail()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            SortedSet<int> sub = set.GetView(2, true, 8, false);   // [2..7]
            SortedSet<int> head = sub.GetViewBefore(6, false);     // [2..5]
            SortedSet<int> tail = head.GetViewAfter(4);            // [4..5]

            CollectionAssert.AreEqual(new[] { 4, 5 }, tail.ToArray());
        }

        [Test]
        public void Test_descending_subset_then_head()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SortedSet<int> desc = set.GetViewDescending();

            SortedSet<int> sub = desc.GetView(8, true, 3, false);  // [8..4]
            SortedSet<int> head = sub.GetViewBefore(6, false);    // [8..7]

            CollectionAssert.AreEqual(new[] { 8, 7 }, head.ToArray());
        }

        [Test]
        public void Test_chained_out_of_range_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            SortedSet<int> sub = set.GetView(2, true, 6, false);

            Assert.Throws<ArgumentOutOfRangeException>(() => sub.GetViewBefore(7, false));
        }

        // subSet bounds errors
        [Test]
        public void Test_subSet_lowerViolatesUpperBound_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 1, 2, 3, 4, 5 };

            SortedSet<int> view = set.GetView(2, true, 4, true);

            // lower = 5: violates UPPER bound (4)
            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetView(5, true, 6, true));
        }

        [Test]
        public void Test_subSet_upperViolatesLowerBound_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 1, 2, 3, 4, 5 };

            SortedSet<int> view = set.GetView(2, true, 4, true);

            // lower = 5: violates UPPER bound (4)
            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetView(0, true, 1, true));
        }

        [Test]
        public void Test_subSet_bothEndpointsOutsideRange_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 1, 2, 3, 4, 5 };

            SortedSet<int> view = set.GetView(2, true, 4, true);

            // lower = 5: violates UPPER bound (4)
            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetView(0, true, 6, true));
        }

        [Test]
        public void test_descendingSubSet_oppositeBoundViolation_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 1, 2, 3, 4, 5 };

            SortedSet<int> view = set.GetView(2, true, 4, true).GetViewDescending();

            // lower = 5: violates UPPER bound (4)
            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetView(5, true, 3, true));
        }

        [Test]
        public void Test_tailSet_exceedsUpperBound_throws()
        {
            SortedSet<int> set = new SortedSet<int>() { 1, 2, 3, 4, 5 };

            SortedSet<int> view = set.GetView(2, true, 4, true);

            // lower = 5: violates UPPER bound (4)
            Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(5, true));
        }

        [Test]
        public void Test_GetView_Exclusive_Exclusive_OutOfRangeBy1_DoesNotThrow()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 20; i++)
                set.Add(i.ToString("D2"));

            SortedSet<string> v1 = set.GetView("01", false, "18", false);
            SortedSet<string> v2 = v1.GetView("03", false, "15", false);
            SortedSet<string> v3 = v2.GetView("05", false, "10", false);

            SortedSet<string> end = v3.GetView("05", false, "10", false);

            // Special case that does not throw because both bounds are exclusive
            CollectionAssert.AreEqual(new[] { "06", "07", "08", "09" }, end.ToArray());

            //Assert.Throws<ArgumentOutOfRangeException>(() => v3.GetView("05", false, "10", false));
        }

        /// <summary>
        /// Represents a sorted collection that may contain duplicates. Note this is just a mock and
        /// the data provided to the constructor must already be sorted according to the provided comparer.
        /// The <see cref="ISortedCollection{T}"/> interface is guaranteed only by implementation, not by
        /// interface contract.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class SortedCollection<T> : ISortedCollection<T>
        {
            private readonly IComparer<T> comparer;
            private readonly List<T> list = new List<T>();

            public SortedCollection() : this(null)
            {
            }

            public SortedCollection(IComparer<T>? comparer)
            {
                this.comparer = comparer ?? Comparer<T>.Default;
            }

            public IComparer<T> Comparer => comparer;

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public void Add(T item) => list.Add(item);

            public void Clear() => list.Clear();

            public bool Contains(T item) => list.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

            public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

            public bool Remove(T item) => list.Remove(item);

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();
        }

        /// <summary>
        /// Represents a sorted collection that contains only distinct elements. Note this is just a mock and
        /// the data provided to the constructor must already be sorted according to the provided comparer.
        /// The <see cref="IDistinctSortedCollection{T}"/> interface is guaranteed only by implementation, not by
        /// interface contract.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class DistinctSortedCollection<T> : IDistinctSortedCollection<T>
        {
            private readonly IComparer<T> comparer;
            private readonly List<T> list = new List<T>();

            public DistinctSortedCollection() : this(null)
            {
            }

            public DistinctSortedCollection(IComparer<T>? comparer)
            {
                this.comparer = comparer ?? Comparer<T>.Default;
            }

            public IComparer<T> Comparer => comparer;

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public void Add(T item) => list.Add(item);

            public void Clear() => list.Clear();

            public bool Contains(T item) => list.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

            public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

            public bool Remove(T item) => list.Remove(item);

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();
        }
    }
}
