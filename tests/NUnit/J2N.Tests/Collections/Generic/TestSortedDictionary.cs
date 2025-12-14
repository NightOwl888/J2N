// Based on: https://github.com/sestoft/C5/blob/master/C5.Tests/Trees/Dictionary.cs#L72-L126
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if FEATURE_SERIALIZABLE
using System.Runtime.Serialization.Formatters.Binary;
#endif
#nullable enable

namespace J2N.Collections.Generic
{
    public class TestSortedDictionary : TestCase
    {
        private SortedDictionary<string, string> dict;


        public override void SetUp()
        {
            base.SetUp();
            dict = new SortedDictionary<string, string>(StringComparer.Ordinal);
        }

        public override void TearDown()
        {
            dict = null;
            base.TearDown();
        }

        [Test]
        public void TestTryGetPredecessor_KeyValuePair()
        {
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
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetPredecessor("B", out _, out string value));
            Assert.AreEqual("1", value);
            Assert.IsTrue(dict.TryGetPredecessor("C", out _, out value));
            Assert.AreEqual("1", value);

            Assert.IsFalse(dict.TryGetPredecessor("A", out string key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetSuccessor()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetSuccessor("B", out _, out string value));
            Assert.AreEqual("2", value);
            Assert.IsTrue(dict.TryGetSuccessor("C", out _, out value));
            Assert.AreEqual("3", value);

            Assert.IsFalse(dict.TryGetSuccessor("E", out string key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetFloor()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetFloor("B", out _, out string value));
            Assert.AreEqual("1", value);
            Assert.IsTrue(dict.TryGetFloor("C", out _, out value));
            Assert.AreEqual("2", value);

            Assert.IsFalse(dict.TryGetFloor("@", out string key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetCeiling()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetCeiling("B", out _, out string value));
            Assert.AreEqual("2", value);
            Assert.IsTrue(dict.TryGetCeiling("C", out _, out value));
            Assert.AreEqual("2", value);

            Assert.IsFalse(dict.TryGetCeiling("F", out string key, out value));
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
