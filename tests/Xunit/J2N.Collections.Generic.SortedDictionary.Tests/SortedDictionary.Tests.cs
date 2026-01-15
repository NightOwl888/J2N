// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using J2N.TestUtilities;
using J2N.TestUtilities.Xunit;
using System;
using System.Collections;
using System.IO;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class SortedDictionary_IDictionary_NonGeneric_Tests : IDictionary_NonGeneric_Tests
    {
        #region IDictionary Helper Methods
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;

        protected override bool NullAllowed => true; // J2N allows null keys

        protected override IDictionary NonGenericIDictionaryFactory()
        {
            return new SortedDictionary<string, string>();
        }

        /// <summary>
        /// Creates an object that is dependent on the seed given. The object may be either
        /// a value type or a reference type, chosen based on the value of the seed.
        /// </summary>
        protected override object CreateTKey(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Creates an object that is dependent on the seed given. The object may be either
        /// a value type or a reference type, chosen based on the value of the seed.
        /// </summary>
        protected override object CreateTValue(int seed)
        {
            return CreateTKey(seed);
        }

        #endregion

        #region IDictionary tests

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_NullValueWhenDefaultValueIsNonNull()
        {
            IDictionary dictionary = new SortedDictionary<string, int>();
            Assert.Throws<ArgumentNullException>(() => dictionary[GetNewKey(dictionary)] = null);
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, string>();
                AssertExtensions.Throws<ArgumentException>("key", () => dictionary[23] = CreateTValue(12345));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_ValueOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, string>();
                object missingKey = GetNewKey(dictionary);
                AssertExtensions.Throws<ArgumentException>("value", () => dictionary[missingKey] = 324);
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Add_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, string>();
                object missingKey = 23;
                AssertExtensions.Throws<ArgumentException>("key", () => dictionary.Add(missingKey, CreateTValue(12345)));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Add_ValueOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, string>();
                object missingKey = GetNewKey(dictionary);
                AssertExtensions.Throws<ArgumentException>("value", () => dictionary.Add(missingKey, 324));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Add_NullValueWhenDefaultTValueIsNonNull()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, int>();
                object missingKey = GetNewKey(dictionary);
                Assert.Throws<ArgumentNullException>(() => dictionary.Add(missingKey, null));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Contains_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, int>();
                Assert.False(dictionary.Contains(1));
            }
        }

        [Fact]
        public void CantAcceptDuplicateKeysFromSourceDictionary()
        {
            Dictionary<string, int> source = new Dictionary<string, int> { { "a", 1 }, { "A", 1 } };
            AssertExtensions.Throws<ArgumentException>(null, () => new SortedDictionary<string, int>(source, StringComparer.OrdinalIgnoreCase));
        }

        #endregion

        #region ICollection tests

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfIncorrectKeyValuePairType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            SCG.KeyValuePair<string, int>[] array = new SCG.KeyValuePair<string, int>[count * 3 / 2];
            AssertExtensions.Throws<ArgumentException>("array", null, () => collection.CopyTo(array, 0));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfCorrectKeyValuePairType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            SCG.KeyValuePair<string, string>[] array = new SCG.KeyValuePair<string, string>[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (object obj in collection)
                Assert.Equal(array[i++], obj);
        }

        #endregion
    }

    public class SortedDictionary_Tests
    {
        [Fact]
        public void TryAdd_AddsItem()
        {
            var dictionary = new SortedDictionary<string, string>();
            Assert.True(dictionary.TryAdd("a", "b"));

            Assert.True(dictionary.ContainsKey("a"));
        }

        [Fact]
        public void TryAdd_ItemAlreadyExists_DoesNotAddItem()
        {
            var dictionary = new SortedDictionary<string, string>();
            dictionary.Add("a", "b");

            Assert.False(dictionary.TryAdd("a", "c"));
            Assert.True(dictionary.TryGetValue("a", out string value));
            Assert.Equal("b", value);
        }

#if FEATURE_SERIALIZABLE
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsBinaryFormatterSupported))]
        public void ComparerSerialization()
        {
            // J2N: We don't care about the internal type names as long as we get back the BCL comparer type.
            // Our deserialization works differently than the BCL so we can implement both J2N and BCL alternate
            // comparer interfaces. We never rely on the internal BCL types for alternate lookup.

            // Strings switch between randomized and non-randomized comparers,
            // however this should never be observable externally.
            TestComparerSerialization(J2N.Collections.Generic.Comparer<string>.Default /*, "System.OrdinalComparer"*/);
            // OrdinalCaseSensitiveComparer is internal and (de)serializes as OrdinalComparer
            TestComparerSerialization(StringComparer.Ordinal /*, "System.OrdinalComparer"*/);
            // OrdinalIgnoreCaseComparer is internal and (de)serializes as OrdinalComparer
            TestComparerSerialization(StringComparer.OrdinalIgnoreCase /*, "System.OrdinalComparer"*/);
            TestComparerSerialization(StringComparer.CurrentCulture);
            TestComparerSerialization(StringComparer.CurrentCultureIgnoreCase);
            TestComparerSerialization(StringComparer.InvariantCulture);
            TestComparerSerialization(StringComparer.InvariantCultureIgnoreCase);

            // Check other types while here, IEquatable valuetype, nullable valuetype, and non IEquatable object
            TestComparerSerialization(J2N.Collections.Generic.Comparer<int>.Default);
            TestComparerSerialization(J2N.Collections.Generic.Comparer<int?>.Default);
            TestComparerSerialization(J2N.Collections.Generic.Comparer<object>.Default);
        }

        private static void TestComparerSerialization<T>(SCG.IComparer<T> comparer, string internalTypeName = null)
        {
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var s = new MemoryStream();

            var dict = new SortedDictionary<T, T>(comparer);

            Assert.Same(comparer, dict.Comparer);

            bf.Serialize(s, dict);
            s.Position = 0;
            dict = (SortedDictionary<T, T>)bf.Deserialize(s);

            // J2N: this assertion fails on .NET <= 8, due to different internal implementations. Skipping it for now.
            //if (equalityComparer.Equals(EqualityComparer<string>.Default))
            //{
            //    // EqualityComparer<string>.Default is mapped to StringEqualityComparer, but serialized as GenericEqualityComparer<string>
            //    Assert.Equal("System.Collections.Generic.GenericEqualityComparer`1[System.String]", dict.EqualityComparer.GetType().ToString());
            //    return;
            //}

            if (internalTypeName == null)
            {
                Assert.IsType(comparer.GetType(), dict.Comparer);
            }
            else
            {
                Assert.Equal(internalTypeName, dict.Comparer.GetType().ToString());
            }

            Assert.True(comparer.Equals(dict.Comparer));
        }
#endif
    }
}
