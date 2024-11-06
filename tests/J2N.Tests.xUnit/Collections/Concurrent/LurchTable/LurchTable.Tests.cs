﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using J2N.Collections.Tests;
using System;
using System.Collections;
using SCG = System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;
using J2N.TestUtilities.Xunit;

namespace J2N.Collections.Concurrent.Tests
{
    public partial class LurchTable_IDictionary_NonGeneric_Tests : IDictionary_NonGeneric_Tests
    {
        protected override IDictionary NonGenericIDictionaryFactory()
        {
            return new LurchTable<string, string>();
        }

        protected override ModifyOperation ModifyEnumeratorThrows => ModifyOperation.None;

        protected override ModifyOperation ModifyEnumeratorAllowed => ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Remove | ModifyOperation.Clear;

        protected override bool NullAllowed => true;

        protected override bool IDictionary_NonGeneric_Keys_Values_ParentDictionaryModifiedInvalidates => false;

        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;


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
            IDictionary dictionary = new LurchTable<string, int>();
            Assert.Throws<ArgumentNullException>(() => dictionary[GetNewKey(dictionary)] = null);
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new LurchTable<string, string>();
                AssertExtensions.Throws<ArgumentException>("key", () => dictionary[23] = CreateTValue(12345));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_ValueOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new LurchTable<string, string>();
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
                IDictionary dictionary = new LurchTable<string, string>();
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
                IDictionary dictionary = new LurchTable<string, string>();
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
                IDictionary dictionary = new LurchTable<string, int>();
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
                IDictionary dictionary = new LurchTable<string, int>();
                Assert.False(dictionary.Contains(1));
            }
        }

        [Fact]
        public void Clear_OnEmptyCollection_DoesNotInvalidateEnumerator()
        {
            if (ModifyEnumeratorAllowed.HasFlag(ModifyOperation.Clear))
            {
                IDictionary dictionary = new LurchTable<string, string>();
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
            SCG.KeyValuePair<string, int>[] array = new SCG.KeyValuePair<string, int>[count * 3 / 2];
            AssertExtensions.Throws<ArgumentException>(null, () => collection.CopyTo(array, 0));
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

    public class LurchTable_Tests
    {
        [Fact]
        public void CopyConstructorExceptions()
        {
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new LurchTable<int, int>((SCG.IDictionary<int, int>)null));
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new LurchTable<int, int>((SCG.IDictionary<int, int>)null, null));
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new LurchTable<int, int>((SCG.IDictionary<int, int>)null, EqualityComparer<int>.Default));

            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LurchTable<int, int>(new NegativeCountDictionary<int, int>()));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LurchTable<int, int>(new NegativeCountDictionary<int, int>(), null));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LurchTable<int, int>(new NegativeCountDictionary<int, int>(), EqualityComparer<int>.Default));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(101)]
        public void ICollection_NonGeneric_CopyTo_NonContiguousDictionary(int count)
        {
            ICollection collection = (ICollection)CreateDictionary(count, k => k.ToString());
            SCG.KeyValuePair<string, string>[] array = new SCG.KeyValuePair<string, string>[count];
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
            SCG.ICollection<SCG.KeyValuePair<string, string>> collection = CreateDictionary(count, k => k.ToString());
            SCG.KeyValuePair<string, string>[] array = new SCG.KeyValuePair<string, string>[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (SCG.KeyValuePair<string, string> obj in collection)
                Assert.Equal(array[i++], obj);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(101)]
        public void IDictionary_Generic_CopyTo_NonContiguousDictionary(int count)
        {
            SCG.IDictionary<string, string> collection = CreateDictionary(count, k => k.ToString());
            SCG.KeyValuePair<string, string>[] array = new SCG.KeyValuePair<string, string>[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (SCG.KeyValuePair<string, string> obj in collection)
                Assert.Equal(array[i++], obj);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(101)]
        public void CopyTo_NonContiguousDictionary(int count)
        {
            LurchTable<string, string> collection = (LurchTable<string, string>)CreateDictionary(count, k => k.ToString());
            string[] array = new string[count];
            collection.Keys.CopyTo(array, 0);
            int i = 0;
            foreach (SCG.KeyValuePair<string, string> obj in collection)
                Assert.Equal(array[i++], obj.Key);

            collection.Values.CopyTo(array, 0);
            i = 0;
            foreach (SCG.KeyValuePair<string, string> obj in collection)
                Assert.Equal(array[i++], obj.Key);
        }

        [Fact]
        public void Remove_NonExistentEntries_DoesNotPreventEnumeration()
        {
            const string SubKey = "-sub-key";
            var dictionary = new Dictionary<string, string>();
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
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("a", "b");

            IEnumerator valuesEnum = dictionary.GetEnumerator();
            Assert.False(dictionary.TryAdd("a", "c"));

            Assert.True(valuesEnum.MoveNext());
        }

        [Theory]
        [MemberData(nameof(CopyConstructorInt32Data))]
        public void CopyConstructorInt32(int size, Func<int, int> keyValueSelector, Func<SCG.IDictionary<int, int>, SCG.IDictionary<int, int>> dictionarySelector)
        {
            TestCopyConstructor(size, keyValueSelector, dictionarySelector);
        }

        public static SCG.IEnumerable<object[]> CopyConstructorInt32Data
        {
            get { return GetCopyConstructorData(i => i); }
        }

        [Theory]
        [MemberData(nameof(CopyConstructorStringData))]
        public void CopyConstructorString(int size, Func<int, string> keyValueSelector, Func<SCG.IDictionary<string, string>, SCG.IDictionary<string, string>> dictionarySelector)
        {
            TestCopyConstructor(size, keyValueSelector, dictionarySelector);
        }

        public static SCG.IEnumerable<object[]> CopyConstructorStringData
        {
            get { return GetCopyConstructorData(i => i.ToString()); }
        }

        private static void TestCopyConstructor<T>(int size, Func<int, T> keyValueSelector, Func<SCG.IDictionary<T, T>, SCG.IDictionary<T, T>> dictionarySelector)
        {
            SCG.IDictionary<T, T> expected = CreateDictionary(size, keyValueSelector);
            SCG.IDictionary<T, T> input = dictionarySelector(CreateDictionary(size, keyValueSelector));

            Assert.Equal(expected, new Dictionary<T, T>(input));
        }

        [Theory]
        [MemberData(nameof(CopyConstructorInt32ComparerData))]
        public void CopyConstructorInt32Comparer(int size, Func<int, int> keyValueSelector, Func<SCG.IDictionary<int, int>, SCG.IDictionary<int, int>> dictionarySelector, SCG.IEqualityComparer<int> comparer)
        {
            TestCopyConstructor(size, keyValueSelector, dictionarySelector, comparer);
        }

        public static SCG.IEnumerable<object[]> CopyConstructorInt32ComparerData
        {
            get
            {
                var comparers = new SCG.IEqualityComparer<int>[]
                {
                    null,
                    EqualityComparer<int>.Default
                };

                return GetCopyConstructorData(i => i, comparers);
            }
        }

        [Theory]
        [MemberData(nameof(CopyConstructorStringComparerData))]
        public void CopyConstructorStringComparer(int size, Func<int, string> keyValueSelector, Func<SCG.IDictionary<string, string>, SCG.IDictionary<string, string>> dictionarySelector, SCG.IEqualityComparer<string> comparer)
        {
            TestCopyConstructor(size, keyValueSelector, dictionarySelector, comparer);
        }

        [Fact]
        public void CantAcceptDuplicateKeysFromSourceDictionary()
        {
            Dictionary<string, int> source = new Dictionary<string, int> { { "a", 1 }, { "A", 1 } };
            AssertExtensions.Throws<ArgumentException>(null, () => new LurchTable<string, int>(source, StringComparer.OrdinalIgnoreCase));
        }

        public static SCG.IEnumerable<object[]> CopyConstructorStringComparerData
        {
            get
            {
                var comparers = new SCG.IEqualityComparer<string>[]
                {
                    null,
                    EqualityComparer<string>.Default,
                    StringComparer.Ordinal,
                    StringComparer.OrdinalIgnoreCase
                };

                return GetCopyConstructorData(i => i.ToString(), comparers);
            }
        }

        private static void TestCopyConstructor<T>(int size, Func<int, T> keyValueSelector, Func<SCG.IDictionary<T, T>, SCG.IDictionary<T, T>> dictionarySelector, SCG.IEqualityComparer<T> comparer)
        {
            SCG.IDictionary<T, T> expected = CreateDictionary(size, keyValueSelector, comparer);
            SCG.IDictionary<T, T> input = dictionarySelector(CreateDictionary(size, keyValueSelector, comparer));

            Assert.Equal(expected, new Dictionary<T, T>(input, comparer));
        }

        private static SCG.IEnumerable<object[]> GetCopyConstructorData<T>(Func<int, T> keyValueSelector, SCG.IEqualityComparer<T>[] comparers = null)
        {
            var dictionarySelectors = new Func<SCG.IDictionary<T, T>, SCG.IDictionary<T, T>>[]
            {
                d => d,
                d => new LurchTableSubclass<T, T>(d),
                d => new ReadOnlyDictionary<T, T>(d)
            };

            var sizes = new int[] { 0, 1, 2, 3 };

            foreach (Func<SCG.IDictionary<T, T>, SCG.IDictionary<T, T>> dictionarySelector in dictionarySelectors)
            {
                foreach (int size in sizes)
                {
                    if (comparers != null)
                    {
                        foreach (SCG.IEqualityComparer<T> comparer in comparers)
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

        private static SCG.IDictionary<T, T> CreateDictionary<T>(int size, Func<int, T> keyValueSelector, SCG.IEqualityComparer<T> comparer = null)
        {
            SCG.Dictionary<T, T> temp = Enumerable.Range(0, size + 1).ToDictionary(keyValueSelector, keyValueSelector, comparer);
            LurchTable<T, T> dict = new LurchTable<T, T>(temp, comparer);
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

        private static void TestComparerSerialization<T>(SCG.IEqualityComparer<T> equalityComparer, string internalTypeName = null)
        {
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var s = new MemoryStream();

            var dict = new Dictionary<T, T>(equalityComparer);

            Assert.Same(equalityComparer, dict.EqualityComparer);

            bf.Serialize(s, dict);
            s.Position = 0;
            dict = (Dictionary<T, T>)bf.Deserialize(s);

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

        private sealed class LurchTableSubclass<TKey, TValue> : LurchTable<TKey, TValue>
        {
            public LurchTableSubclass(SCG.IDictionary<TKey, TValue> dictionary)
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
        private sealed class NegativeCountDictionary<TKey, TValue> : SCG.IDictionary<TKey, TValue>
        {
            public int Count { get { return -1; } }

            public TValue this[TKey key] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
            public bool IsReadOnly { get { throw new NotImplementedException(); } }
            public SCG.ICollection<TKey> Keys { get { throw new NotImplementedException(); } }
            public SCG.ICollection<TValue> Values { get { throw new NotImplementedException(); } }
            public void Add(SCG.KeyValuePair<TKey, TValue> item) { throw new NotImplementedException(); }
            public void Add(TKey key, TValue value) { throw new NotImplementedException(); }
            public void Clear() { throw new NotImplementedException(); }
            public bool Contains(SCG.KeyValuePair<TKey, TValue> item) { throw new NotImplementedException(); }
            public bool ContainsKey(TKey key) { throw new NotImplementedException(); }
            public void CopyTo(SCG.KeyValuePair<TKey, TValue>[] array, int arrayIndex) { throw new NotImplementedException(); }
            public SCG.IEnumerator<SCG.KeyValuePair<TKey, TValue>> GetEnumerator() { throw new NotImplementedException(); }
            public bool Remove(SCG.KeyValuePair<TKey, TValue> item) { throw new NotImplementedException(); }
            public bool Remove(TKey key) { throw new NotImplementedException(); }
            public bool TryGetValue(TKey key, out TValue value) { throw new NotImplementedException(); }
            IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
        }
    }
}
