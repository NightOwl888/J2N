using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SCG = System.Collections.Generic;
#nullable enable

namespace J2N.Collections.Generic
{
    public class TestHashSet : TestCase
    {
#if FEATURE_SERIALIZABLE
        /// <summary>
        /// Tests that HashSet instances serialized with J2N 2.1.0 can be deserialized
        /// correctly. This ensures backward compatibility with binary serialization.
        /// </summary>
        [Test]
        public void TestDeserializeLegacy_String_OrdinalComparer()
        {
            HashSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("hashset-string-ordinal-v2.1.0.bin")!)
            {
                set = (HashSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct (ignoring order)
            var expected = new SCG.HashSet<string> { "five", "four", "one", "three", "two" };
            var actual = set;

            Assert.That(expected.SetEquals(actual), Is.True);
            Assert.That(actual.SetEquals(expected), Is.True);

            var expectedComparer = StringComparer.Ordinal;
            var actualComparer = set.EqualityComparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_OrdinalIgnoreCaseComparer()
        {
            HashSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("hashset-string-ordinalignorecase-v2.1.0.bin")!)
            {
                set = (HashSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct (ignoring order)
            var expected = new SCG.HashSet<string> { "five", "four", "one", "three", "two" };
            var actual = set;

            Assert.That(expected.SetEquals(actual), Is.True);
            Assert.That(actual.SetEquals(expected), Is.True);

            var expectedComparer = StringComparer.OrdinalIgnoreCase;
            var actualComparer = set.EqualityComparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_InvariantComparer()
        {
            HashSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("hashset-string-invariant-v2.1.0.bin")!)
            {
                set = (HashSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct (ignoring order)
            var expected = new SCG.HashSet<string> { "five", "four", "one", "three", "two" };
            var actual = set;

            Assert.That(expected.SetEquals(actual), Is.True);
            Assert.That(actual.SetEquals(expected), Is.True);

            var expectedComparer = StringComparer.InvariantCulture;
            var actualComparer = set.EqualityComparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_String_CustomComparer()
        {
            HashSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("hashset-string-customcomparer-v2.1.0.bin")!)
            {
                set = (HashSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct (ignoring order)
            var expected = new SCG.HashSet<string> { "five", "four", "one", "three", "two" };
            var actual = set;

            Assert.That(expected.SetEquals(actual), Is.True);
            Assert.That(actual.SetEquals(expected), Is.True);

            var expectedComparerType = typeof(CustomStringComparer);
            var actualComparerType = set.EqualityComparer.GetType();
            assertEquals(expectedComparerType, actualComparerType);
        }

        [Test]
        public void TestDeserializeLegacy_String_DefaultComparer_Empty()
        {
            HashSet<string> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("hashset-string-defaultcomparer-empty-v2.1.0.bin")!)
            {
                set = (HashSet<string>)formatter.Deserialize(stream);
            }

            assertEquals(0, set.Count);

            var expectedComparer = EqualityComparer<string>.Default; // J2N default comparer (ordinal)
            var actualComparer = set.EqualityComparer;
            assertEquals(expectedComparer, actualComparer);
        }

        [Test]
        public void TestDeserializeLegacy_Int32_CustomComparer()
        {
            HashSet<int> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("hashset-int32-customcomparer-v2.1.0.bin")!)
            {
                set = (HashSet<int>)formatter.Deserialize(stream);
            }

            assertEquals(5, set.Count);

            // Verify the values are correct (ignoring order)
            var expected = new SCG.HashSet<int> { 5, 4, 1, 3, 2 };
            var actual = set;

            Assert.That(expected.SetEquals(actual), Is.True);
            Assert.That(actual.SetEquals(expected), Is.True);

            var expectedComparerType = typeof(CustomInt32Comparer);
            var actualComparerType = set.EqualityComparer.GetType();
            assertEquals(expectedComparerType, actualComparerType);
        }

        [Test]
        public void TestDeserializeLegacy_Int32_DefaultComparer_Empty()
        {
            HashSet<int> set;
            var formatter = new BinaryFormatter();

            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("hashset-int32-defaultcomparer-empty-v2.1.0.bin")!)
            {
                set = (HashSet<int>)formatter.Deserialize(stream);
            }

            assertEquals(0, set.Count);

            var expectedComparer = EqualityComparer<int>.Default; // J2N default comparer
            var actualComparer = set.EqualityComparer;
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

                HashSet<string> source = new HashSet<string>(StringComparer.CurrentCulture)
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

                HashSet<string> roundTripped;

                using (var ms = new MemoryStream(blob))
                {
                    roundTripped = (HashSet<string>)formatter.Deserialize(ms);
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

                // Extra sanity: ordering consistency
                SCG.HashSet<string> result = new(roundTripped);

                Assert.That(source.SetEquals(result), Is.True);
                Assert.That(result.SetEquals(source), Is.True);
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
