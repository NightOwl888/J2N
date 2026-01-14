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
    public class TestDictionary : TestCase
    {
#if FEATURE_SERIALIZABLE
        /// <summary>
        /// Tests that Dictionary instances serialized with J2N 2.1.0 can be deserialized
        /// correctly. This ensures backward compatibility with binary serialization.
        /// </summary>
        [Test]
        public void TestDeserializeLegacy_String_Int32_OrdinalComparer()
        {
            Dictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("dictionary-string-int32-ordinal-v2.1.0.bin")!)
            {
                dict = (Dictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { "five", "four", "one", "three", "two" };
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEquivalent(expectedKeys, actualKeys);

            // Verify the values are correct
            var expectedValues = new[] { 5, 4, 1, 3, 2 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEquivalent(expectedValues, actualValues);

            var expectedComparer = StringComparer.Ordinal;
            var actualComparer = dict.EqualityComparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_Int32_OrdinalIgnoreCaseComparer()
        {
            Dictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("dictionary-string-int32-ordinalignorecase-v2.1.0.bin")!)
            {
                dict = (Dictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { "five", "four", "one", "three", "two" };
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEquivalent(expectedKeys, actualKeys);

            // Verify the values are correct
            var expectedValues = new[] { 5, 4, 1, 3, 2 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEquivalent(expectedValues, actualValues);

            var expectedComparer = StringComparer.OrdinalIgnoreCase;
            var actualComparer = dict.EqualityComparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_Int32_InvariantComparer()
        {
            Dictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("dictionary-string-int32-invariant-v2.1.0.bin")!)
            {
                dict = (Dictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { "five", "four", "one", "three", "two" };
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEquivalent(expectedKeys, actualKeys);

            // Verify the values are correct
            var expectedValues = new[] { 5, 4, 1, 3, 2 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEquivalent(expectedValues, actualValues);

            var expectedComparer = StringComparer.InvariantCulture;
            var actualComparer = dict.EqualityComparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_Int32_CustomComparer()
        {
            Dictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("dictionary-string-int32-customcomparer-v2.1.0.bin")!)
            {
                dict = (Dictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { "five", "four", "one", "three", "two" };
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEquivalent(expectedKeys, actualKeys);

            // Verify the values are correct
            var expectedValues = new[] { 5, 4, 1, 3, 2 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEquivalent(expectedValues, actualValues);

            var expectedComparerType = typeof(CustomStringComparer);
            var actualComparerType = dict.EqualityComparer.GetType();
            assertEquals(expectedComparerType, actualComparerType);
        }

        [Test]
        public void TestDeserializeLegacy_String_Int32_DefaultComparer_Empty()
        {
            Dictionary<string, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("dictionary-string-int32-defaultcomparer-empty-v2.1.0.bin")!)
            {
                dict = (Dictionary<string, int>)formatter.Deserialize(stream);
            }

            assertEquals(0, dict.Count);

            var expectedComparer = EqualityComparer<string>.Default; // J2N default comparer (ordinal)
            var actualComparer = dict.EqualityComparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_Int32_Int32_CustomComparer()
        {
            Dictionary<int, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("dictionary-int32-int32-customcomparer-v2.1.0.bin")!)
            {
                dict = (Dictionary<int, int>)formatter.Deserialize(stream);
            }

            assertEquals(5, dict.Count);

            var expectedKeys = new[] { 1, 2, 3, 4, 5 };
            var actualKeys = dict.Keys.ToArray();
            CollectionAssert.AreEquivalent(expectedKeys, actualKeys);

            // Verify the values are correct
            var expectedValues = new[] { 1, 2, 3, 4, 5 };
            var actualValues = dict.Values.ToArray();
            CollectionAssert.AreEquivalent(expectedValues, actualValues);

            var expectedComparerType = typeof(CustomInt32Comparer);
            var actualComparerType = dict.EqualityComparer.GetType();
            assertEquals(expectedComparerType, actualComparerType);
        }

        [Test]
        public void TestDeserializeLegacy_Int32_Int32_DefaultComparer_Empty()
        {
            Dictionary<int, int> dict;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("dictionary-int32-int32-defaultcomparer-empty-v2.1.0.bin")!)
            {
                dict = (Dictionary<int, int>)formatter.Deserialize(stream);
            }

            assertEquals(0, dict.Count);

            var expectedComparer = EqualityComparer<int>.Default; // J2N default comparer
            var actualComparer = dict.EqualityComparer;
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

                Dictionary<string, int> source = new Dictionary<string, int>(StringComparer.CurrentCulture)
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

                Dictionary<string, int> roundTripped;

                using (var ms = new MemoryStream(blob))
                {
                    roundTripped = (Dictionary<string, int>)formatter.Deserialize(ms);
                }

                // This is the key assertion
                // The comparer must NOT be tied to cultureB
                Assert.That(roundTripped.EqualityComparer, Is.Not.EqualTo(StringComparer.CurrentCulture),
                    "Comparer incorrectly rebound to the current culture after deserialization.");

                // Verify behavior matches cultureA
                bool compareResult = roundTripped.EqualityComparer.Equals("I", "ı");

                Assert.That(compareResult, Is.EqualTo(
                    StringComparer.Create(cultureA, ignoreCase: false).Equals("I", "ı")),
                    "Comparer did not preserve original culture semantics.");

                // Extra sanity: value consistency
                KeyValuePair<string, int>[] result = new KeyValuePair<string, int>[roundTripped.Count];
                roundTripped.CopyTo(result, 0);

                CollectionAssert.AreEquivalent(
                    source,
                    result
                );
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }


        // IMPORTANT: These are serialized with the J2N.Tests assembly name. So, they must always be available at that location.
        [Serializable]
        public class CustomStringComparer : IEqualityComparer<string>
        {
            public bool Equals(string? x, string? y) => StringComparer.Ordinal.Equals(x, y);

            public int GetHashCode([DisallowNull] string obj) => StringComparer.Ordinal.GetHashCode(obj);
        }

        [Serializable]
        public class CustomInt32Comparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y) => x == y;

            public int GetHashCode([DisallowNull] int obj) => obj.GetHashCode();
        }
#endif
    }
}
