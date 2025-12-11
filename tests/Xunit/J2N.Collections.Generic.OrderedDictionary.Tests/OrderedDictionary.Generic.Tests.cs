// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using J2N.TestUtilities.Xunit;
using Xunit;
using JCG = J2N.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the Dictionary class.
    /// </summary>
    public abstract class OrderedDictionary_Generic_Tests<TKey, TValue> : IDictionary_Generic_Tests<TKey, TValue>
    {
        #region IDictionary<TKey, TValue> Helper Methods

        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;
        protected override bool DefaultValueAllowed => true;
        protected override bool DefaultValueWhenNotAllowed_Throws => true;
        protected override ModifyOperation ModifyEnumeratorThrows => ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Remove | ModifyOperation.Clear;
        protected override ModifyOperation ModifyEnumeratorAllowed => ModifyOperation.Overwrite;

        protected override IDictionary<TKey, TValue> GenericIDictionaryFactory() => new JCG.OrderedDictionary<TKey, TValue>();

        #endregion

        #region Constructors

        [Fact]
        public void OrderedDictionary_Generic_Constructor()
        {
            JCG.OrderedDictionary<TKey, TValue> instance;
            IEqualityComparer<TKey> comparer = GetKeyIEqualityComparer();

            instance = new JCG.OrderedDictionary<TKey, TValue>();
            Assert.Empty(instance);
            Assert.Empty(instance.Keys);
            Assert.Empty(instance.Values);
            Assert.Same(JCG.EqualityComparer<TKey>.Default, instance.EqualityComparer);
            Assert.Equal(0, instance.Capacity);

            instance = new JCG.OrderedDictionary<TKey, TValue>(42);
            Assert.Empty(instance);
            Assert.Empty(instance.Keys);
            Assert.Empty(instance.Values);
            Assert.Same(JCG.EqualityComparer<TKey>.Default, instance.EqualityComparer);
            Assert.InRange(instance.Capacity, 42, int.MaxValue);

            instance = new JCG.OrderedDictionary<TKey, TValue>(comparer);
            Assert.Empty(instance);
            Assert.Empty(instance.Keys);
            Assert.Empty(instance.Values);
            Assert.Same(comparer, instance.EqualityComparer);
            Assert.Equal(0, instance.Capacity);

            instance = new JCG.OrderedDictionary<TKey, TValue>(42, comparer);
            Assert.Empty(instance);
            Assert.Empty(instance.Keys);
            Assert.Empty(instance.Values);
            Assert.Same(comparer, instance.EqualityComparer);
            Assert.InRange(instance.Capacity, 42, int.MaxValue);

            IEqualityComparer<TKey> customComparer = EqualityComparerHelper<TKey>.Create(comparer.Equals!, comparer.GetHashCode!);
            instance = new JCG.OrderedDictionary<TKey, TValue>(42, customComparer);
            Assert.Empty(instance);
            Assert.Empty(instance.Keys);
            Assert.Empty(instance.Values);
            Assert.Same(customComparer, instance.EqualityComparer);
            Assert.InRange(instance.Capacity, 42, int.MaxValue);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_Constructor_IDictionary(int count)
        {
            IDictionary<TKey, TValue> source = GenericIDictionaryFactory(count);
            IEqualityComparer<TKey> comparer = GetKeyIEqualityComparer();
            JCG.OrderedDictionary<TKey, TValue> copied;

            copied = new JCG.OrderedDictionary<TKey, TValue>(source);
            Assert.Equal(source, copied);
            Assert.Same(comparer, JCG.EqualityComparer<TKey>.Default);

            copied = new JCG.OrderedDictionary<TKey, TValue>(source, comparer);
            Assert.Equal(source, copied);
            Assert.Same(comparer, copied.EqualityComparer);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_Constructor_IEnumerable(int count)
        {
            IEnumerable<KeyValuePair<TKey, TValue>> initial = GenericIDictionaryFactory(count);

            foreach (IEnumerable<KeyValuePair<TKey, TValue>> source in new[] { initial, initial.ToArray(), initial.Where(i => true) })
            {
                IEqualityComparer<TKey> comparer = GetKeyIEqualityComparer();
                JCG.OrderedDictionary<TKey, TValue> copied;

                copied = new JCG.OrderedDictionary<TKey, TValue>(source);
                Assert.Equal(source, copied);
                Assert.Same(comparer, JCG.EqualityComparer<TKey>.Default);
                Assert.InRange(copied.Capacity, copied.Count, int.MaxValue);

                copied = new JCG.OrderedDictionary<TKey, TValue>(source, comparer);
                Assert.Equal(source, copied);
                Assert.Same(comparer, copied.EqualityComparer);
                Assert.InRange(copied.Capacity, copied.Count, int.MaxValue);
            }
        }

        [Fact]
        public void OrderedDictionary_Generic_Constructor_NullIDictionary_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new JCG.OrderedDictionary<TKey, TValue>((IDictionary<TKey, TValue>)null!));
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new JCG.OrderedDictionary<TKey, TValue>((IDictionary<TKey, TValue>)null!, null));
            AssertExtensions.Throws<ArgumentNullException>("dictionary", () => new JCG.OrderedDictionary<TKey, TValue>((IDictionary<TKey, TValue>)null!, JCG.EqualityComparer<TKey>.Default));

            AssertExtensions.Throws<ArgumentNullException>("collection", () => new JCG.OrderedDictionary<TKey, TValue>((IEnumerable<KeyValuePair<TKey, TValue>>)null!));
            AssertExtensions.Throws<ArgumentNullException>("collection", () => new JCG.OrderedDictionary<TKey, TValue>((IEnumerable<KeyValuePair<TKey, TValue>>)null!, null));
            AssertExtensions.Throws<ArgumentNullException>("collection", () => new JCG.OrderedDictionary<TKey, TValue>((IEnumerable<KeyValuePair<TKey, TValue>>)null!, JCG.EqualityComparer<TKey>.Default));
        }

        [Fact]
        public void OrderedDictionary_Generic_Constructor_AllKeysEqualComparer()
        {
            var dictionary = new JCG.OrderedDictionary<TKey, TValue>(EqualityComparerHelper<TKey>.Create((x, y) => true, x => 1));
            Assert.Equal(0, dictionary.Count);

            Assert.True(dictionary.TryAdd(CreateTKey(0), CreateTValue(0)));
            Assert.Equal(1, dictionary.Count);

            Assert.False(dictionary.TryAdd(CreateTKey(1), CreateTValue(0)));
            Assert.Equal(1, dictionary.Count);

            dictionary.Remove(CreateTKey(2));
            Assert.Equal(0, dictionary.Count);
        }

        #endregion

        #region TryAdd
        [Fact]
        public void TryAdd_NullKeyDoesNotThrow() // J2N: allow null keys, was TryAdd_NullKeyThrows
        {
            if (default(TKey) is not null)
            {
                return;
            }

            var dictionary = new JCG.OrderedDictionary<TKey, TValue>();
            int index;

            // J2N: was: AssertExtensions.Throws<ArgumentNullException>("key", () => dictionary.TryAdd(default(TKey), CreateTValue(0)));
            // J2N: was: AssertExtensions.Throws<ArgumentNullException>("key", () => dictionary.TryAdd(default(TKey), CreateTValue(0), out index));

            Assert.True(dictionary.TryAdd(default, CreateTValue(0)));
            Assert.True(dictionary.TryAdd(CreateTKey(0), default!));
            Assert.Equal(2, dictionary.Count);

            Assert.False(dictionary.TryAdd(default, CreateTValue(0), out index));
            Assert.Equal(0, index);
            Assert.True(dictionary.TryAdd(CreateTKey(1), default, out index));
            Assert.Equal(2, index);
            Assert.Equal(3, dictionary.Count);
        }

        [Fact]
        public void TryAdd_AppendsItemToEndOfDictionary()
        {
            var dictionary = new JCG.OrderedDictionary<TKey, TValue>();
            AddToCollection(dictionary, 10);
            foreach (var entry in dictionary)
            {
                Assert.False(dictionary.TryAdd(entry.Key, entry.Value));
            }

            TKey newKey;
            int i = 0;
            do
            {
                newKey = CreateTKey(i);
            }
            while (dictionary.ContainsKey(newKey));

            Assert.True(dictionary.TryAdd(newKey, CreateTValue(42)));
            Assert.Equal(dictionary.Count - 1, dictionary.IndexOf(newKey));
        }

        [Fact]
        public void TryAdd_ItemAlreadyExists_DoesNotInvalidateEnumerator()
        {
            TKey key1 = CreateTKey(1);

            var dictionary = new JCG.OrderedDictionary<TKey, TValue>() { [key1] = CreateTValue(2) };

            IEnumerator valuesEnum = dictionary.GetEnumerator();
            Assert.False(dictionary.TryAdd(key1, CreateTValue(3)));

            Assert.True(valuesEnum.MoveNext());
        }
        #endregion

        #region ContainsValue

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_ContainsValue_NotPresent(int count)
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            TValue notPresent = CreateTValue(seed++);
            while (dictionary.Values.Contains(notPresent))
            {
                notPresent = CreateTValue(seed++);
            }

            Assert.False(dictionary.ContainsValue(notPresent));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_ContainsValue_Present(int count)
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            KeyValuePair<TKey, TValue> notPresent = CreateT(seed++);
            while (dictionary.Contains(notPresent))
            {
                notPresent = CreateT(seed++);
            }

            dictionary.Add(notPresent.Key, notPresent.Value);
            Assert.True(dictionary.ContainsValue(notPresent.Value));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_ContainsValue_DefaultValueNotPresent(int count)
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.False(dictionary.ContainsValue(default(TValue)));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_ContainsValue_DefaultValuePresent(int count)
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            TKey notPresent = CreateTKey(seed++);
            while (dictionary.ContainsKey(notPresent))
            {
                notPresent = CreateTKey(seed++);
            }

            dictionary.Add(notPresent, default(TValue)!);
            Assert.True(dictionary.ContainsValue(default(TValue)));
        }

        #endregion

        #region GetAt / SetAt

        [Fact]
        public void OrderedDictionary_Generic_SetAt_GetAt_InvalidInputs()
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory();

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.GetAt(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.GetAt(0));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.SetAt(-1, CreateTValue(0)));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.SetAt(0, CreateTValue(0)));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.SetAt(-1, CreateTKey(0), CreateTValue(0)));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.SetAt(0, CreateTKey(0), CreateTValue(0)));

            dictionary.Add(CreateTKey(0), CreateTValue(0));

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.GetAt(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.GetAt(1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.SetAt(-1, CreateTValue(0)));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.SetAt(1, CreateTValue(0)));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.SetAt(-1, CreateTKey(0), CreateTValue(0)));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => dictionary.SetAt(1, CreateTKey(0), CreateTValue(0)));

            if (default(TKey) is null)
            {
                // J2N: modified to allow for null keys
                // was: AssertExtensions.Throws<ArgumentNullException>("key", () => dictionary.SetAt(0, default, CreateTValue(0)));
                dictionary.SetAt(0, default, CreateTValue(0));
            }

            dictionary.Add(CreateTKey(1), CreateTValue(1));

            TKey firstKey = dictionary.GetAt(0).Key;
            dictionary.SetAt(0, firstKey, CreateTValue(0));
            dictionary.SetAt(0, CreateTKey(2), CreateTValue(0));
            dictionary.SetAt(0, firstKey, CreateTValue(0));

            // J2N: null key throws a different ArgumentException without "key" as the parameter name
            AssertExtensions.Throws<ArgumentException>(/*"key",*/ () =>
                dictionary.SetAt(1, firstKey, CreateTValue(0)));
        }

        [Theory]
        [MemberData(nameof(ValidPositiveCollectionSizes))]
        public void OrderedDictionary_Generic_SetAt_GetAt_Roundtrip(int count)
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            KeyValuePair<TKey, TValue> pair;

            for (int i = 0; i < dictionary.Count; i++)
            {
                pair = dictionary.GetAt(i);
                Assert.Equal(pair, ((IList<KeyValuePair<TKey, TValue>>)dictionary)[i]);

                dictionary.SetAt(i, CreateTValue(i + 500));
                pair = dictionary.GetAt(i);
                Assert.Equal(pair, ((IList<KeyValuePair<TKey, TValue>>)dictionary)[i]);

                dictionary.SetAt(i, CreateTKey(i + 1000), CreateTValue(i + 1000));
                pair = dictionary.GetAt(i);
                Assert.Equal(pair, ((IList<KeyValuePair<TKey, TValue>>)dictionary)[i]);
            }
        }

        [Fact]
        public void OrderedDictionary_SetAt_KeyValuePairSubsequentlyAvailable()
        {
            TKey key0 = CreateTKey(0), key1 = CreateTKey(1);
            TValue value0 = CreateTValue(0), value1 = CreateTValue(1);

            var dict = new JCG.OrderedDictionary<TKey, TValue>
            {
                [key0] = value0,
            };

            dict.SetAt(index: 0, key1, value1);

            Assert.Equal(1, dict.Count);
            Assert.Equal([new(key1, value1)], dict);
            Assert.False(dict.ContainsKey(key0));
            Assert.True(dict.ContainsKey(key1));
        }

        #endregion

        #region Remove(..., out TValue)

        [Theory]
        [MemberData(nameof(ValidPositiveCollectionSizes))]
        public void OrderedDictionary_Generic_Remove(int count)
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);

            KeyValuePair<TKey, TValue> pair = default;
            while (dictionary.Count > 0)
            {
                pair = dictionary.GetAt(0);
                Assert.True(dictionary.Remove(pair.Key, out TValue? value));
                Assert.Equal(pair.Value, value);
            }

            Assert.False(dictionary.Remove(pair.Key, out _));
        }

        #endregion

        #region TrimExcess

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_TrimExcess(int count)
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);

            int dictCount = dictionary.Count;
            dictionary.TrimExcess();
            Assert.Equal(dictCount, dictionary.Count);
            Assert.InRange(dictionary.Capacity, dictCount, int.MaxValue);

            if (count > 0)
            {
                int oldCapacity = dictionary.Capacity;
                int newCapacity = dictionary.EnsureCapacity(count * 10);
                Assert.Equal(newCapacity, dictionary.Capacity);
                Assert.InRange(newCapacity, oldCapacity + 1, int.MaxValue);
                dictionary.TrimExcess(dictCount);
                Assert.Equal(oldCapacity, dictionary.Capacity);
            }
        }

        #endregion

        public static IEnumerable<object[]> Dictionary_GetAlternateLookup_OperationsMatchUnderlyingDictionary_MemberData()
        {
            yield return new object[] { JCG.EqualityComparer<string>.Default };
            yield return new object[] { StringComparer.Ordinal };
            yield return new object[] { StringComparer.OrdinalIgnoreCase };
            yield return new object[] { StringComparer.InvariantCulture };
            yield return new object[] { StringComparer.InvariantCultureIgnoreCase };
            yield return new object[] { StringComparer.CurrentCulture };
            yield return new object[] { StringComparer.CurrentCultureIgnoreCase };
        }

        #region GetAlternateComparer

#if FEATURE_IALTERNATEEQUALITYCOMPARER

        [Fact]
        public void GetAlternateLookup_FailsWhenIncompatible()
        {
            var dictionary = new JCG.OrderedDictionary<string, string>(StringComparer.Ordinal);

            dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
            Assert.True(dictionary.TryGetAlternateLookup<ReadOnlySpan<char>>(out _));

            Assert.Throws<InvalidOperationException>(() => dictionary.GetAlternateLookup<ReadOnlySpan<byte>>());
            Assert.Throws<InvalidOperationException>(() => dictionary.GetAlternateLookup<string>());
            Assert.Throws<InvalidOperationException>(() => dictionary.GetAlternateLookup<int>());

            Assert.False(dictionary.TryGetAlternateLookup<ReadOnlySpan<byte>>(out _));
            Assert.False(dictionary.TryGetAlternateLookup<string>(out _));
            Assert.False(dictionary.TryGetAlternateLookup<int>(out _));
        }

        [Theory]
        [MemberData(nameof(Dictionary_GetAlternateLookup_OperationsMatchUnderlyingDictionary_MemberData))]
        public void Dictionary_GetAlternateLookup_OperationsMatchUnderlyingDictionary(IEqualityComparer<string> comparer)
        {
            // Test with a variety of comparers to ensure that the alternate lookup is consistent with the underlying dictionary
            JCG.OrderedDictionary<string, int> dictionary = new(comparer);
            JCG.OrderedDictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> lookup = dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
            Assert.Same(dictionary, lookup.Dictionary);
            Assert.Same(lookup.Dictionary, lookup.Dictionary);

            string? actualKey;
            int value;

            // Add to the dictionary and validate that the lookup reflects the changes
            dictionary["123"] = 123;
            Assert.True(lookup.ContainsKey("123".AsSpan()));
            Assert.True(lookup.TryGetValue("123".AsSpan(), out value));
            Assert.Equal(123, value);
            Assert.Equal(123, lookup["123".AsSpan()]);
            Assert.False(lookup.TryAdd("123".AsSpan(), 321));
            Assert.True(lookup.Remove("123".AsSpan()));
            Assert.False(dictionary.ContainsKey("123"));
            Assert.Throws<KeyNotFoundException>(() => lookup["123".AsSpan()]);

            // Add via the lookup and validate that the dictionary reflects the changes
            Assert.True(lookup.TryAdd("123".AsSpan(), 123));
            Assert.True(dictionary.ContainsKey("123"));
            lookup.TryGetValue("123".AsSpan(), out value);
            Assert.Equal(123, value);
            Assert.False(lookup.Remove("321".AsSpan(), out actualKey, out value));
            Assert.Null(actualKey);
            Assert.Equal(0, value);
            Assert.True(lookup.Remove("123".AsSpan(), out actualKey, out value));
            Assert.Equal("123", actualKey);
            Assert.Equal(123, value);

            // Ensure that case-sensitivity of the comparer is respected
            lookup["a".AsSpan()] = 42;
            if (dictionary.EqualityComparer.Equals(EqualityComparer<string>.Default) ||
                dictionary.EqualityComparer.Equals(StringComparer.Ordinal) ||
                dictionary.EqualityComparer.Equals(StringComparer.InvariantCulture) ||
                dictionary.EqualityComparer.Equals(StringComparer.CurrentCulture))
            {
                Assert.True(lookup.TryGetValue("a".AsSpan(), out actualKey, out value));
                Assert.Equal("a", actualKey);
                Assert.Equal(42, value);
                Assert.True(lookup.TryAdd("A".AsSpan(), 42));
                Assert.True(lookup.Remove("a".AsSpan()));
                Assert.False(lookup.Remove("a".AsSpan()));
                Assert.True(lookup.Remove("A".AsSpan()));
            }
            else
            {
                Assert.True(lookup.TryGetValue("A".AsSpan(), out actualKey, out value));
                Assert.Equal("a", actualKey);
                Assert.Equal(42, value);
                Assert.False(lookup.TryAdd("A".AsSpan(), 42));
                Assert.True(lookup.Remove("A".AsSpan()));
                Assert.False(lookup.Remove("a".AsSpan()));
                Assert.False(lookup.Remove("A".AsSpan()));
            }

            // Validate overwrites
            lookup["a".AsSpan()] = 42;
            Assert.Equal(42, dictionary["a"]);
            lookup["a".AsSpan()] = 43;
            Assert.True(lookup.Remove("a".AsSpan(), out actualKey, out value));
            Assert.Equal("a", actualKey);
            Assert.Equal(43, value);

            // Test adding multiple entries via the lookup
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i, dictionary.Count);
                Assert.True(lookup.TryAdd(i.ToString().AsSpan(), i));
                Assert.False(lookup.TryAdd(i.ToString().AsSpan(), i));
            }

            Assert.Equal(10, dictionary.Count);

            // Test that the lookup and the dictionary agree on what's in and not in
            for (int i = -1; i <= 10; i++)
            {
                Assert.Equal(dictionary.TryGetValue(i.ToString(), out int dv), lookup.TryGetValue(i.ToString().AsSpan(), out int lv));
                Assert.Equal(dv, lv);
            }

            // Test removing multiple entries via the lookup
            for (int i = 9; i >= 0; i--)
            {
                Assert.True(lookup.Remove(i.ToString().AsSpan(), out actualKey, out value));
                Assert.Equal(i.ToString(), actualKey);
                Assert.Equal(i, value);
                Assert.False(lookup.Remove(i.ToString().AsSpan(), out actualKey, out value));
                Assert.Null(actualKey);
                Assert.Equal(0, value);
                Assert.Equal(i, dictionary.Count);
            }
        }

        [Theory] // J2N specific
        [MemberData(nameof(Dictionary_GetAlternateLookup_OperationsMatchUnderlyingDictionary_MemberData))]
        public void Dictionary_GetAlternateLookup_NullableType_OperationsMatchUnderlyingDictionary(IEqualityComparer<string> comparer)
        {
            // Test with a variety of comparers to ensure that the alternate lookup is consistent with the underlying dictionary
            JCG.OrderedDictionary<string, int> dictionary = new(new AlternateStringComparer(comparer));
            JCG.OrderedDictionary<string, int>.AlternateLookup<string> lookup = dictionary.GetAlternateLookup<string>();
            Assert.Same(dictionary, lookup.Dictionary);
            Assert.Same(lookup.Dictionary, lookup.Dictionary);

            // Test the behavior of null vs "" in the dictionary and lookup
            // including using null for the lookup key
            Assert.True(dictionary.TryAdd(null, 1));
            Assert.True(dictionary.TryAdd(string.Empty, 2));
            Assert.Equal(2, dictionary.Count);
            Assert.True(dictionary.Contains(new(null!, 1)));
            Assert.True(dictionary.Contains(new("", 2)));
            Assert.True(dictionary.ContainsKey(null));
            Assert.True(dictionary.ContainsKey(""));
            Assert.False(lookup.TryAdd(null, 1));
            Assert.False(lookup.TryAdd("", 2));
            Assert.True(lookup.ContainsKey(null));
            Assert.True(lookup.ContainsKey(""));
            Assert.True(lookup.TryGetValue(null, out string? actualKey, out int value));
            Assert.Null(actualKey);
            Assert.Equal(1, value);
            Assert.True(lookup.TryGetValue("", out actualKey, out value));
            Assert.Equal("", actualKey);
            Assert.Equal(2, value);
            Assert.True(lookup.Remove(""));
            Assert.True(lookup.Remove(null));
            Assert.True(lookup.TryAdd(null, 1));
            Assert.True(dictionary.Remove(null));
            Assert.Equal(0, dictionary.Count);
            Assert.False(lookup.Remove(""));
            Assert.False(dictionary.Remove(""));
            Assert.False(lookup.Remove(null));
            Assert.False(dictionary.Remove(null));
            Assert.Equal(0, dictionary.Count);

            lookup[null] = 3;
            lookup.TryGetValue(null, out value);
            Assert.Equal(3, value);
            Assert.True(lookup.Remove(null));
            Assert.Equal(0, dictionary.Count);

            dictionary[null] = 3;
            dictionary.TryGetValue(null, out value);
            Assert.Equal(3, value);
            Assert.True(dictionary.Remove(null));
            Assert.Equal(0, dictionary.Count);
        }

#endif

        #endregion

        #region GetSpanAlternateComparer

        [Fact]
        public void GetSpanAlternateLookup_FailsWhenIncompatible()
        {
            var dictionary = new JCG.OrderedDictionary<string, string>(StringComparer.Ordinal);

            dictionary.GetSpanAlternateLookup<char>();
            Assert.True(dictionary.TryGetSpanAlternateLookup<char>(out _));

            Assert.Throws<InvalidOperationException>(() => dictionary.GetSpanAlternateLookup<byte>());
            Assert.Throws<InvalidOperationException>(() => dictionary.GetSpanAlternateLookup<string>());
            Assert.Throws<InvalidOperationException>(() => dictionary.GetSpanAlternateLookup<int>());

            Assert.False(dictionary.TryGetSpanAlternateLookup<byte>(out _));
            Assert.False(dictionary.TryGetSpanAlternateLookup<string>(out _));
            Assert.False(dictionary.TryGetSpanAlternateLookup<int>(out _));
        }

        [Theory]
        [MemberData(nameof(Dictionary_GetAlternateLookup_OperationsMatchUnderlyingDictionary_MemberData))]
        public void Dictionary_GetSpanAlternateLookup_OperationsMatchUnderlyingDictionary(IEqualityComparer<string> comparer)
        {
            // Test with a variety of comparers to ensure that the alternate lookup is consistent with the underlying dictionary
            JCG.OrderedDictionary<string, int> dictionary = new(comparer);
            JCG.OrderedDictionary<string, int>.SpanAlternateLookup<char> lookup = dictionary.GetSpanAlternateLookup<char>();
            Assert.Same(dictionary, lookup.Dictionary);
            Assert.Same(lookup.Dictionary, lookup.Dictionary);

            string? actualKey;
            int value;

            // Add to the dictionary and validate that the lookup reflects the changes
            dictionary["123"] = 123;
            Assert.True(lookup.ContainsKey("123".AsSpan()));
            Assert.True(lookup.TryGetValue("123".AsSpan(), out value));
            Assert.Equal(123, value);
            Assert.Equal(123, lookup["123".AsSpan()]);
            Assert.False(lookup.TryAdd("123".AsSpan(), 321));
            Assert.True(lookup.Remove("123".AsSpan()));
            Assert.False(dictionary.ContainsKey("123"));
            Assert.Throws<KeyNotFoundException>(() => lookup["123".AsSpan()]);

            // Add via the lookup and validate that the dictionary reflects the changes
            Assert.True(lookup.TryAdd("123".AsSpan(), 123));
            Assert.True(dictionary.ContainsKey("123"));
            lookup.TryGetValue("123".AsSpan(), out value);
            Assert.Equal(123, value);
            Assert.False(lookup.Remove("321".AsSpan(), out actualKey, out value));
            Assert.Null(actualKey);
            Assert.Equal(0, value);
            Assert.True(lookup.Remove("123".AsSpan(), out actualKey, out value));
            Assert.Equal("123", actualKey);
            Assert.Equal(123, value);

            // Ensure that case-sensitivity of the comparer is respected
            lookup["a".AsSpan()] = 42;
            if (dictionary.EqualityComparer.Equals(EqualityComparer<string>.Default) ||
                dictionary.EqualityComparer.Equals(StringComparer.Ordinal) ||
                dictionary.EqualityComparer.Equals(StringComparer.InvariantCulture) ||
                dictionary.EqualityComparer.Equals(StringComparer.CurrentCulture))
            {
                Assert.True(lookup.TryGetValue("a".AsSpan(), out actualKey, out value));
                Assert.Equal("a", actualKey);
                Assert.Equal(42, value);
                Assert.True(lookup.TryAdd("A".AsSpan(), 42));
                Assert.True(lookup.Remove("a".AsSpan()));
                Assert.False(lookup.Remove("a".AsSpan()));
                Assert.True(lookup.Remove("A".AsSpan()));
            }
            else
            {
                Assert.True(lookup.TryGetValue("A".AsSpan(), out actualKey, out value));
                Assert.Equal("a", actualKey);
                Assert.Equal(42, value);
                Assert.False(lookup.TryAdd("A".AsSpan(), 42));
                Assert.True(lookup.Remove("A".AsSpan()));
                Assert.False(lookup.Remove("a".AsSpan()));
                Assert.False(lookup.Remove("A".AsSpan()));
            }

            // Validate overwrites
            lookup["a".AsSpan()] = 42;
            Assert.Equal(42, dictionary["a"]);
            lookup["a".AsSpan()] = 43;
            Assert.True(lookup.Remove("a".AsSpan(), out actualKey, out value));
            Assert.Equal("a", actualKey);
            Assert.Equal(43, value);

            // Test adding multiple entries via the lookup
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i, dictionary.Count);
                Assert.True(lookup.TryAdd(i.ToString().AsSpan(), i));
                Assert.False(lookup.TryAdd(i.ToString().AsSpan(), i));
            }

            Assert.Equal(10, dictionary.Count);

            // Test that the lookup and the dictionary agree on what's in and not in
            for (int i = -1; i <= 10; i++)
            {
                Assert.Equal(dictionary.TryGetValue(i.ToString(), out int dv), lookup.TryGetValue(i.ToString().AsSpan(), out int lv));
                Assert.Equal(dv, lv);
            }

            // Test removing multiple entries via the lookup
            for (int i = 9; i >= 0; i--)
            {
                Assert.True(lookup.Remove(i.ToString().AsSpan(), out actualKey, out value));
                Assert.Equal(i.ToString(), actualKey);
                Assert.Equal(i, value);
                Assert.False(lookup.Remove(i.ToString().AsSpan(), out actualKey, out value));
                Assert.Null(actualKey);
                Assert.Equal(0, value);
                Assert.Equal(i, dictionary.Count);
            }
        }

        #endregion

        #region EnsureCapacity

        [Fact]
        public void OrderedDictionary_Generic_EnsureCapacity()
        {
            JCG.OrderedDictionary<TKey, TValue> dictionary = (JCG.OrderedDictionary<TKey, TValue>)GenericIDictionaryFactory();
            Assert.Equal(0, dictionary.Capacity);

            dictionary.EnsureCapacity(1);
            Assert.InRange(dictionary.Capacity, 1, int.MaxValue);

            for (int i = 0; i < 30; i++)
            {
                dictionary.TryAdd(CreateTKey(i), CreateTValue(i));
            }
            int count = dictionary.Count;
            Assert.InRange(count, 1, 30);
            Assert.InRange(dictionary.Capacity, dictionary.Count, int.MaxValue);
            Assert.Equal(dictionary.Capacity, dictionary.EnsureCapacity(dictionary.Capacity));
            Assert.Equal(dictionary.Capacity, dictionary.EnsureCapacity(dictionary.Capacity - 1));
            Assert.Equal(dictionary.Capacity, dictionary.EnsureCapacity(0));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(() => dictionary.EnsureCapacity(-1));

            int oldCapacity = dictionary.Capacity;
            int newCapacity = dictionary.EnsureCapacity(oldCapacity * 2);
            Assert.Equal(newCapacity, dictionary.Capacity);
            Assert.InRange(newCapacity, oldCapacity * 2, int.MaxValue);

            for (int i = 0; i < 30; i++)
            {
                Assert.True(dictionary.ContainsKey(CreateTKey(i)));
            }
            Assert.Equal(count, dictionary.Count);
        }

        #endregion

#if FEATURE_IREADONLYCOLLECTIONS
        #region IReadOnlyDictionary<TKey, TValue>.Keys

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void IReadOnlyDictionary_Generic_Keys_ContainsAllCorrectKeys(int count)
        {
            IDictionary<TKey, TValue> dictionary = GenericIDictionaryFactory(count);
            IEnumerable<TKey> expected = dictionary.Select((pair) => pair.Key);
            IEnumerable<TKey> keys = ((IReadOnlyDictionary<TKey, TValue>)dictionary).Keys;
            Assert.True(expected.SequenceEqual(keys));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void IReadOnlyDictionary_Generic_Values_ContainsAllCorrectValues(int count)
        {
            IDictionary<TKey, TValue> dictionary = GenericIDictionaryFactory(count);
            IEnumerable<TValue> expected = dictionary.Select((pair) => pair.Value);
            IEnumerable<TValue> values = ((IReadOnlyDictionary<TKey, TValue>)dictionary).Values;
            Assert.True(expected.SequenceEqual(values));
        }

        #endregion
#endif
    }
}
