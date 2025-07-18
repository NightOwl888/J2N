﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public partial class LinkedDictionary_IDictionary_NonGeneric_Tests : IDictionary_NonGeneric_Tests
    {
        protected override IDictionary NonGenericIDictionaryFactory()
        {
            return new LinkedDictionary<string, string>();
        }

        protected override bool NullAllowed => true;

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
        protected override object CreateTValue(int seed) => CreateTKey(seed);

        protected override Type ICollection_NonGeneric_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

        #region IDictionary tests

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_NullValueWhenDefaultValueIsNonNull()
        {
            IDictionary dictionary = new LinkedDictionary<string, int>();
            Assert.Throws<ArgumentNullException>(() => dictionary[GetNewKey(dictionary)] = null);
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new LinkedDictionary<string, string>();
                AssertExtensions.Throws<ArgumentException>("key", () => dictionary[23] = CreateTValue(12345));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_ValueOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new LinkedDictionary<string, string>();
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
                IDictionary dictionary = new LinkedDictionary<string, string>();
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
                IDictionary dictionary = new LinkedDictionary<string, string>();
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
                IDictionary dictionary = new LinkedDictionary<string, int>();
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
                IDictionary dictionary = new LinkedDictionary<string, int>();
                Assert.False(dictionary.Contains(1));
            }
        }

        [Fact]
        public void Clear_OnEmptyCollection_DoesNotInvalidateEnumerator()
        {
            if (ModifyEnumeratorAllowed.HasFlag(ModifyOperation.Clear))
            {
                IDictionary dictionary = new LinkedDictionary<string, string>();
                IEnumerator valuesEnum = dictionary.GetEnumerator();

                dictionary.Clear();
                Assert.Empty(dictionary);
                Assert.False(valuesEnum.MoveNext());
            }
        }

        #endregion

        #region ICollection tests

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfIncorrectKeyValuePairType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            KeyValuePair<string, int>[] array = new KeyValuePair<string, int>[count * 3 / 2];
            AssertExtensions.Throws<ArgumentException>(null, () => collection.CopyTo(array, 0));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfCorrectKeyValuePairType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (object obj in collection)
                Assert.Equal(array[i++], obj);
        }

        #endregion
    }

    public class LinkedDictionary_Tests
    {
        [Fact]
        public void CopyConstructorExceptions()
        {
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new LinkedDictionary<int, int>((IDictionary<int, int>)null!));
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new LinkedDictionary<int, int>((IDictionary<int, int>)null!, null));
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new LinkedDictionary<int, int>((IDictionary<int, int>)null!, J2N.Collections.Generic.EqualityComparer<int>.Default));

            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LinkedDictionary<int, int>(new NegativeCountDictionary<int, int>()));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LinkedDictionary<int, int>(new NegativeCountDictionary<int, int>(), null));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LinkedDictionary<int, int>(new NegativeCountDictionary<int, int>(), J2N.Collections.Generic.EqualityComparer<int>.Default));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(101)]
        public void ICollection_NonGeneric_CopyTo_NonContiguousDictionary(int count)
        {
            ICollection collection = (ICollection)CreateDictionary(count, k => k.ToString());
            KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (object obj in collection)
                Assert.Equal(array[i++], obj);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(101)]
        public void ICollection_Generic_CopyTo_NonContiguousDictionary(int count)
        {
            ICollection<KeyValuePair<string, string>> collection = CreateDictionary(count, k => k.ToString());
            KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (KeyValuePair<string, string> obj in collection)
                Assert.Equal(array[i++], obj);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(101)]
        public void IDictionary_Generic_CopyTo_NonContiguousDictionary(int count)
        {
            IDictionary<string, string> collection = CreateDictionary(count, k => k.ToString());
            KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (KeyValuePair<string, string> obj in collection)
                Assert.Equal(array[i++], obj);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(101)]
        public void CopyTo_NonContiguousDictionary(int count)
        {
            LinkedDictionary<string, string> collection = (LinkedDictionary<string, string>)CreateDictionary(count, k => k.ToString());
            string[] array = new string[count];
            collection.Keys.CopyTo(array, 0);
            int i = 0;
            foreach (KeyValuePair<string, string> obj in collection)
                Assert.Equal(array[i++], obj.Key);

            collection.Values.CopyTo(array, 0);
            i = 0;
            foreach (KeyValuePair<string, string> obj in collection)
                Assert.Equal(array[i++], obj.Key);
        }

        [Fact]
        public void Remove_NonExistentEntries_DoesNotPreventEnumeration()
        {
            const string SubKey = "-sub-key";
            var dictionary = new LinkedDictionary<string, string>();
            dictionary.Add("a", "b");
            dictionary.Add("c", "d");
            foreach (string key in dictionary.Keys)
            {
                if (dictionary.Remove(key + SubKey))
                    break;
            }

            dictionary.Add("c" + SubKey, "d");
            foreach (string key in dictionary.Keys)
            {
                if (dictionary.Remove(key + SubKey))
                    break;
            }
        }

        [Fact]
        public void TryAdd_ItemAlreadyExists_DoesNotInvalidateEnumerator()
        {
            var dictionary = new LinkedDictionary<string, string>();
            dictionary.Add("a", "b");

            IEnumerator valuesEnum = dictionary.GetEnumerator();
            Assert.False(dictionary.TryAdd("a", "c"));

            Assert.True(valuesEnum.MoveNext());
        }

        [Theory]
        [MemberData(nameof(CopyConstructorInt32Data))]
        public void CopyConstructorInt32(int size, Func<int, int> keyValueSelector, Func<IDictionary<int, int>, IDictionary<int, int>> dictionarySelector)
        {
            TestCopyConstructor(size, keyValueSelector, dictionarySelector);
        }

        public static IEnumerable<object[]> CopyConstructorInt32Data
        {
            get { return GetCopyConstructorData(i => i); }
        }

        [Theory]
        [MemberData(nameof(CopyConstructorStringData))]
        public void CopyConstructorString(int size, Func<int, string> keyValueSelector, Func<IDictionary<string, string>, IDictionary<string, string>> dictionarySelector)
        {
            TestCopyConstructor(size, keyValueSelector, dictionarySelector);
        }

        public static IEnumerable<object[]> CopyConstructorStringData
        {
            get { return GetCopyConstructorData(i => i.ToString()); }
        }

        private static void TestCopyConstructor<T>(int size, Func<int, T> keyValueSelector, Func<IDictionary<T, T>, IDictionary<T, T>> dictionarySelector)
        {
            IDictionary<T, T> expected = CreateDictionary(size, keyValueSelector);
            IDictionary<T, T> input = dictionarySelector(CreateDictionary(size, keyValueSelector));

            Assert.Equal(expected, new LinkedDictionary<T, T>(input));
        }

        [Theory]
        [MemberData(nameof(CopyConstructorInt32ComparerData))]
        public void CopyConstructorInt32Comparer(int size, Func<int, int> keyValueSelector, Func<IDictionary<int, int>, IDictionary<int, int>> dictionarySelector, IEqualityComparer<int> comparer)
        {
            TestCopyConstructor(size, keyValueSelector, dictionarySelector, comparer);
        }

        public static IEnumerable<object[]> CopyConstructorInt32ComparerData
        {
            get
            {
                var comparers = new IEqualityComparer<int>[]
                {
                    null!,
                    J2N.Collections.Generic.EqualityComparer<int>.Default
                };

                return GetCopyConstructorData(i => i, comparers);
            }
        }

        [Theory]
        [MemberData(nameof(CopyConstructorStringComparerData))]
        public void CopyConstructorStringComparer(int size, Func<int, string> keyValueSelector, Func<IDictionary<string, string>, IDictionary<string, string>> dictionarySelector, IEqualityComparer<string> comparer)
        {
            TestCopyConstructor(size, keyValueSelector, dictionarySelector, comparer);
        }

        [Fact]
        public void CantAcceptDuplicateKeysFromSourceDictionary()
        {
            LinkedDictionary<string, int> source = new LinkedDictionary<string, int> { { "a", 1 }, { "A", 1 } };
            AssertExtensions.Throws<ArgumentException>(null, () => new LinkedDictionary<string, int>(source, StringComparer.OrdinalIgnoreCase));
        }

        public static IEnumerable<object[]> CopyConstructorStringComparerData
        {
            get
            {
                var comparers = new IEqualityComparer<string>[]
                {
                    null!,
                    J2N.Collections.Generic.EqualityComparer<string>.Default,
                    StringComparer.Ordinal,
                    StringComparer.OrdinalIgnoreCase
                };

                return GetCopyConstructorData(i => i.ToString(), comparers);
            }
        }

        private static void TestCopyConstructor<T>(int size, Func<int, T> keyValueSelector, Func<IDictionary<T, T>, IDictionary<T, T>> dictionarySelector, IEqualityComparer<T> comparer)
        {
            IDictionary<T, T> expected = CreateDictionary(size, keyValueSelector, comparer);
            IDictionary<T, T> input = dictionarySelector(CreateDictionary(size, keyValueSelector, comparer));

            Assert.Equal(expected, new LinkedDictionary<T, T>(input, comparer));
        }

        private static IEnumerable<object[]> GetCopyConstructorData<T>(Func<int, T> keyValueSelector, IEqualityComparer<T>[]? comparers = null)
        {
            var dictionarySelectors = new Func<IDictionary<T, T>, IDictionary<T, T>>[]
            {
                d => d,
                d => new DictionarySubclass<T, T>(d),
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8714 // Nullability of type argument doesn't match 'notnull' constraint.
                d => new ReadOnlyDictionary<T, T>(d)
#pragma warning restore CS8714 // Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary suppression
            };

            var sizes = new int[] { 0, 1, 2, 3 };

            foreach (Func<IDictionary<T, T>, IDictionary<T, T>> dictionarySelector in dictionarySelectors)
            {
                foreach (int size in sizes)
                {
                    if (comparers != null)
                    {
                        foreach (IEqualityComparer<T> comparer in comparers)
                        {
                            yield return new object[] { size, keyValueSelector, dictionarySelector, comparer };
                        }
                    }
                    else
                    {
                        yield return new object[] { size, keyValueSelector, dictionarySelector };
                    }
                }
            }
        }

        private static IDictionary<T, T> CreateDictionary<T>(int size, Func<int, T> keyValueSelector, IEqualityComparer<T>? comparer = null)
        {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8714 // Nullability of type argument doesn't match 'notnull' constraint.
            SCG.Dictionary<T, T> temp = Enumerable.Range(0, size + 1).ToDictionary(keyValueSelector, keyValueSelector, comparer);
#pragma warning restore CS8714 // Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary suppression
            LinkedDictionary<T, T> dict = new LinkedDictionary<T, T>(temp, comparer);
            // Remove first item to reduce Count to size and alter the contiguity of the dictionary
            dict.Remove(keyValueSelector(0));
            return dict;
        }

#if FEATURE_SERIALIZABLE
        [Fact]
        public void ComparerSerialization()
        {
            // Strings switch between randomized and non-randomized comparers,
            // however this should never be observable externally.
            TestComparerSerialization(J2N.Collections.Generic.EqualityComparer<string>.Default, "System.OrdinalComparer");
            // OrdinalCaseSensitiveComparer is internal and (de)serializes as OrdinalComparer
            TestComparerSerialization(StringComparer.Ordinal, "System.OrdinalComparer");
            // OrdinalIgnoreCaseComparer is internal and (de)serializes as OrdinalComparer
            TestComparerSerialization(StringComparer.OrdinalIgnoreCase, "System.OrdinalComparer");
            TestComparerSerialization(StringComparer.CurrentCulture);
            TestComparerSerialization(StringComparer.CurrentCultureIgnoreCase);
            TestComparerSerialization(StringComparer.InvariantCulture);
            TestComparerSerialization(StringComparer.InvariantCultureIgnoreCase);

            // Check other types while here, IEquatable valuetype, nullable valuetype, and non IEquatable object
            TestComparerSerialization(J2N.Collections.Generic.EqualityComparer<int>.Default);
            TestComparerSerialization(J2N.Collections.Generic.EqualityComparer<int?>.Default);
            TestComparerSerialization(J2N.Collections.Generic.EqualityComparer<object>.Default);
        }

        private static void TestComparerSerialization<T>(IEqualityComparer<T> equalityComparer, string? internalTypeName = null)
        {
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var s = new MemoryStream();

            var dict = new LinkedDictionary<T, T>(equalityComparer);

            Assert.Same(equalityComparer, dict.EqualityComparer);

            bf.Serialize(s, dict);
            s.Position = 0;
            dict = (LinkedDictionary<T, T>)bf.Deserialize(s);

            if (internalTypeName == null)
            {
                Assert.IsType(equalityComparer.GetType(), dict.EqualityComparer);
            }
            else
            {
                Assert.Equal(internalTypeName, dict.EqualityComparer.GetType().ToString());
            }

            Assert.True(equalityComparer.Equals(dict.EqualityComparer));
        }
#endif

        private sealed class DictionarySubclass<TKey, TValue> : J2N.Collections.Generic.Dictionary<TKey, TValue>
        {
            public DictionarySubclass(IDictionary<TKey, TValue> dictionary)
            {
                foreach (var pair in dictionary)
                {
                    Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// An incorrectly implemented dictionary that returns -1 from Count.
        /// </summary>
        private sealed class NegativeCountDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            public int Count { get { return -1; } }

            public TValue this[TKey key] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
            public bool IsReadOnly { get { throw new NotImplementedException(); } }
            public ICollection<TKey> Keys { get { throw new NotImplementedException(); } }
            public ICollection<TValue> Values { get { throw new NotImplementedException(); } }
            public void Add(KeyValuePair<TKey, TValue> item) { throw new NotImplementedException(); }
            public void Add(TKey key, TValue value) { throw new NotImplementedException(); }
            public void Clear() { throw new NotImplementedException(); }
            public bool Contains(KeyValuePair<TKey, TValue> item) { throw new NotImplementedException(); }
            public bool ContainsKey(TKey key) { throw new NotImplementedException(); }
            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { throw new NotImplementedException(); }
            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { throw new NotImplementedException(); }
            public bool Remove(KeyValuePair<TKey, TValue> item) { throw new NotImplementedException(); }
            public bool Remove(TKey key) { throw new NotImplementedException(); }
            public bool TryGetValue(TKey key, out TValue value) { throw new NotImplementedException(); }
            IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
        }
    }
}
