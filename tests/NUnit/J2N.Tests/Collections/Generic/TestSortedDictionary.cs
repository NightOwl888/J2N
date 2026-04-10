// Based on: https://github.com/sestoft/C5/blob/master/C5.Tests/Trees/Dictionary.cs#L72-L126
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
#if FEATURE_SERIALIZABLE
using System.Runtime.Serialization.Formatters.Binary;
#endif
using SCG = System.Collections.Generic;
#nullable enable

namespace J2N.Collections.Generic
{
    public class TestSortedDictionary : TestCase
    {
        [Test]
        public void TestTryGetPredecessor_KeyValuePair()
        {
            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);

            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetPredecessor("B", out KeyValuePair<string, string> res));
            Assert.AreEqual("1", res.Value);
            Assert.IsTrue(dict.TryGetPredecessor("C", out res));
            Assert.AreEqual("1", res.Value);

            Assert.IsFalse(dict.TryGetPredecessor("A", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);
        }

        [Test]
        public void TestTryGetSuccessor_KeyValuePair()
        {
            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);

            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetSuccessor("B", out KeyValuePair<string, string> res));
            Assert.AreEqual("2", res.Value);
            Assert.IsTrue(dict.TryGetSuccessor("C", out res));
            Assert.AreEqual("3", res.Value);

            Assert.IsFalse(dict.TryGetSuccessor("E", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);
        }

        [Test]
        public void TestTryGetPredecessor()
        {
            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);

            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetPredecessor("B", out _, out string? value));
            Assert.AreEqual("1", value);
            Assert.IsTrue(dict.TryGetPredecessor("C", out _, out value));
            Assert.AreEqual("1", value);

            Assert.IsFalse(dict.TryGetPredecessor("A", out string? key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetSuccessor()
        {
            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);

            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetSuccessor("B", out _, out string? value));
            Assert.AreEqual("2", value);
            Assert.IsTrue(dict.TryGetSuccessor("C", out _, out value));
            Assert.AreEqual("3", value);

            Assert.IsFalse(dict.TryGetSuccessor("E", out string? key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetFloor()
        {
            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);

            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetFloor("B", out _, out string? value));
            Assert.AreEqual("1", value);
            Assert.IsTrue(dict.TryGetFloor("C", out _, out value));
            Assert.AreEqual("2", value);

            Assert.IsFalse(dict.TryGetFloor("@", out string? key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetCeiling()
        {
            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);

            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetCeiling("B", out _, out string? value));
            Assert.AreEqual("2", value);
            Assert.IsTrue(dict.TryGetCeiling("C", out _, out value));
            Assert.AreEqual("2", value);

            Assert.IsFalse(dict.TryGetCeiling("F", out string? key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

#if FEATURE_SERIALIZABLE
        /// <summary>
        /// Tests that SortedDictionary instances serialized with J2N 2.1.0 can be deserialized
        /// correctly. This ensures backward compatibility with binary serialization.
        /// </summary>
        [Test]
        public void TestDeserializeLegacy_String_Int32_OrdinalComparer()
        {
            SortedDictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sorteddictionary-string-int32-ordinal-v2.1.0.bin")!)
            {
                dict = (SortedDictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { "five", "four", "one", "three", "two" }; // Sorted lexographically
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEqual(expectedKeys, actualKeys);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { 5, 4, 1, 3, 2 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparer = StringComparer.Ordinal;
            var actualComparer = dict.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_Int32_OrdinalIgnoreCaseComparer()
        {
            SortedDictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sorteddictionary-string-int32-ordinalignorecase-v2.1.0.bin")!)
            {
                dict = (SortedDictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { "five", "four", "one", "three", "two" }; // Sorted lexographically
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEqual(expectedKeys, actualKeys);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { 5, 4, 1, 3, 2 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparer = StringComparer.OrdinalIgnoreCase;
            var actualComparer = dict.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_Int32_InvariantComparer()
        {
            SortedDictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sorteddictionary-string-int32-invariant-v2.1.0.bin")!)
            {
                dict = (SortedDictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { "five", "four", "one", "three", "two" }; // Sorted lexographically
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEqual(expectedKeys, actualKeys);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { 5, 4, 1, 3, 2 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparer = StringComparer.InvariantCulture;
            var actualComparer = dict.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_Int32_CustomComparer()
        {
            SortedDictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sorteddictionary-string-int32-customcomparer-v2.1.0.bin")!)
            {
                dict = (SortedDictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { "five", "four", "one", "three", "two" }; // Sorted lexographically
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEqual(expectedKeys, actualKeys);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { 5, 4, 1, 3, 2 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparerType = typeof(CustomStringComparer);
            var actualComparerType = dict.Comparer.GetType();
            assertEquals(expectedComparerType, actualComparerType);
        }

        [Test]
        public void TestDeserializeLegacy_String_Int32_DefaultComparer_Empty()
        {
            SortedDictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sorteddictionary-string-int32-defaultcomparer-empty-v2.1.0.bin")!)
            {
                dict = (SortedDictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(0, dict.Count);

            var expectedComparer = Comparer<string>.Default; // J2N default comparer (ordinal)
            var actualComparer = dict.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_Int32_Int32_CustomComparer()
        {
            SortedDictionary<int, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sorteddictionary-int32-int32-customcomparer-v2.1.0.bin")!)
            {
                dict = (SortedDictionary<int, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { 1, 2, 3, 4, 5 };
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEqual(expectedKeys, actualKeys);

            // Verify the values are correct and in the expected order
            var expectedValues = new[] { 1, 2, 3, 4, 5 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEqual(expectedValues, actualValues);

            var expectedComparerType = typeof(CustomInt32Comparer);
            var actualComparerType = dict.Comparer.GetType();
            assertEquals(expectedComparerType, actualComparerType);
        }

        [Test]
        public void TestDeserializeLegacy_Int32_Int32_DefaultComparer_Empty()
        {
            SortedDictionary<int, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("sorteddictionary-int32-int32-defaultcomparer-empty-v2.1.0.bin")!)
            {
                dict = (SortedDictionary<int, int>)formatter.Deserialize(stream);
            }

            assertEquals(0, dict.Count);

            var expectedComparer = Comparer<int>.Default; // J2N default comparer
            var actualComparer = dict.Comparer;
            assertEquals(expectedComparer, actualComparer);
        }


        [Test]
        public void TestSerializeRoundTrip_String_Int32_CurrentCulture()
        {
            CultureInfo originalCulture = CultureInfo.CurrentCulture;

            try
            {
                // Culture A (creation + serialization)
                CultureInfo cultureA = CultureInfo.GetCultureInfo("tr-TR");
                CultureInfo cultureB = CultureInfo.GetCultureInfo("en-US");

                CultureInfo.CurrentCulture = cultureA;

                SortedDictionary<string,int> source = new SortedDictionary<string, int>(StringComparer.CurrentCulture)
                {
                    ["I"] = 1,
                    ["ı"] = 2, // dotless i
                    ["i"] = 3,
                    ["İ"] = 4  // dotted I
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

                SortedDictionary<string, int> roundTripped;

                using (var ms = new MemoryStream(blob))
                {
                    roundTripped = (SortedDictionary<string, int>)formatter.Deserialize(ms);
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
                KeyValuePair<string, int>[] result = new KeyValuePair<string, int>[roundTripped.Count];
                roundTripped.CopyTo(result, 0);

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

        [Test]
        public void Test_Constructor_BclSortedDictionaryKeys_WithMatchingComparer()
        {
            SCG.SortedDictionary<int, string> dict = new(Comparer<int>.Default)
            {
                [1] = "a",
                [2] = "b",
                [3] = "c"
            };

            SortedDictionary<int, string> target = new(dict, Comparer<int>.Default);

            CollectionAssert.AreEqual(dict, target);
        }

        [Test]
        public void Test_Values_AreNotSorted()
        {
            SortedDictionary<int, int> dict = new()
            {
                [1] = 100,
                [2] = 1,
                [3] = 50,
                [4] = 2
            };

            CollectionAssert.AreEqual(
                new[] { 100, 1, 50, 2 },
                dict.Values.ToArray());
        }


        [Test]
        public void Test_headMap_descendingMap()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> ascending = dictionary.GetViewBefore(4, true);
            SortedDictionary<int, int> descending = ascending.GetViewDescending();

            // Different iteration order
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, ascending.Keys.ToArray());
            CollectionAssert.AreEqual(new int[] { 4, 3, 2, 1 }, descending.Keys.ToArray());
        }

        [Test]
        public void Test_descendingMap_headMap()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> ascending = dictionary.GetViewBefore(4, true);
            SortedDictionary<int, int> descending = dictionary.GetViewDescending().GetViewBefore(4, true);

            // Different iteration order
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, ascending.Keys.ToArray());
            CollectionAssert.AreEqual(new int[] { 5, 4 }, descending.Keys.ToArray());
        }

        [Test]
        public void Test_tailMap_descendingMap()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> ascending = dictionary.GetViewAfter(4, true);
            SortedDictionary<int, int> descending = ascending.GetViewDescending();

            // Different iteration order
            CollectionAssert.AreEqual(new int[] { 4, 5 }, ascending.Keys.ToArray());
            CollectionAssert.AreEqual(new int[] { 5, 4 }, descending.Keys.ToArray());
        }

        [Test]
        public void Test_descendingMap_tailMap()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> ascending = dictionary.GetViewAfter(4, true);
            SortedDictionary<int, int> descending = dictionary.GetViewDescending().GetViewAfter(4, true);

            // Different iteration order
            CollectionAssert.AreEqual(new int[] { 4, 5 }, ascending.Keys.ToArray());
            CollectionAssert.AreEqual(new int[] { 4, 3, 2, 1 }, descending.Keys.ToArray());
        }

        // Edge cases
        [Test]
        public void Test_GetView_GetViewDescending_GetViewBefore_MatchesSubset()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> result = dictionary.GetView(2, true, 5, true)
                            .GetViewDescending()
                            .GetViewBefore(4, true);

            CollectionAssert.AreEqual(new[] { 5, 4 }, result.Keys.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewBefore_Exclusive_MatchesSubset()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> result = dictionary.GetViewDescending().GetViewBefore(3, false);

            CollectionAssert.AreEqual(new[] { 5, 4 }, result.Keys.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewDescending_MatchesSet()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> result = dictionary.GetViewDescending().GetViewDescending();

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, result.Keys.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewAfter_OutOfRange_Lower_Empty()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> descending = dictionary.GetViewDescending();

            SortedDictionary<int, int> tail = descending.GetViewAfter(0, true);

            CollectionAssert.AreEqual(Arrays.Empty<int>(), tail.Keys.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewBefore_OutOfRange_Lower_Unchanged()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> descending = dictionary.GetViewDescending();

            SortedDictionary<int, int> head = descending.GetViewBefore(0, true);

            CollectionAssert.AreEqual(new[] { 5, 4, 3, 2, 1 }, head.Keys.ToArray());
        }

        [Test]
        public void Test_GetViewDescending_GetViewAfter_GetViewBefore_OutOfRange_Higher_Throws()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 4,
                [5] = 5
            };

            SortedDictionary<int, int> descending = dictionary.GetViewDescending();

            SortedDictionary<int, int> tail = descending.GetViewAfter(3, true);

            Assert.Throws<ArgumentOutOfRangeException>(() => tail.GetViewBefore(4, true));
        }

        [Test]
        public void Test_GetView_RangeOutsideOfBaseSet_Empty()
        {
            SortedDictionary<string, int> dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal)
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3
            };

            SortedDictionary<string, int> view = dictionary.GetView("4", "9");
            Assert.AreEqual(0, view.Count);

            var lookup = dictionary.GetSpanAlternateLookup<char>();
            SortedDictionary<string, int> lookupView = lookup.GetView("4".AsSpan(), "9".AsSpan());
            Assert.AreEqual(0, lookupView.Count);
        }

        [Test]
        public void Test_GetView_Exclusive_Exclusive_SameValue_Empty()
        {
            SortedDictionary<string, int> dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal)
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
                ["4"] = 4,
                ["5"] = 5
            };

            SortedDictionary<string, int> view = dictionary.GetView("3", false, "3", false);
            Assert.AreEqual(0, view.Count);

            var lookup = dictionary.GetSpanAlternateLookup<char>();
            SortedDictionary<string, int> lookupView = lookup.GetView("3".AsSpan(), false, "3".AsSpan(), false);
            Assert.AreEqual(0, lookupView.Count);
        }

        [Test]
        public void Test_GetView_GetView_Exclusive_Exclusive_SameValue_Empty()
        {
            SortedDictionary<string, int> dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal)
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
                ["4"] = 4,
                ["5"] = 5
            };

            SortedDictionary<string, int> view1 = dictionary.GetView("2", "4");

            SortedDictionary<string, int> view2 = view1.GetView("3", false, "3", false);
            Assert.AreEqual(0, view2.Count);

            var lookup = view1.GetSpanAlternateLookup<char>();
            SortedDictionary<string, int> lookupView = lookup.GetView("3".AsSpan(), false, "3".AsSpan(), false);
            Assert.AreEqual(0, lookupView.Count);
        }
    }
}
