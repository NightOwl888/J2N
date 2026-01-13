// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using System;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;


namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the Dictionary class.
    /// </summary>
    public abstract class SortedDictionary_Generic_Tests<TKey, TValue> : IDictionary_Generic_Tests<TKey, TValue>
    {
        #region IDictionary<TKey, TValue> Helper Methods
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;
        protected override bool DefaultValueWhenNotAllowed_Throws { get { return false; } }

        protected override SCG.IDictionary<TKey, TValue> GenericIDictionaryFactory()
        {
            return new SortedDictionary<TKey, TValue>();
        }

        #endregion

        #region Constructors

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_Constructor_IComparer(int count)
        {
            SCG.IComparer<TKey> comparer = GetKeyIComparer();
            SCG.IDictionary<TKey, TValue> source = GenericIDictionaryFactory(count);
            SortedDictionary<TKey, TValue> copied = new SortedDictionary<TKey, TValue>(source, comparer);
            Assert.Equal(source, copied);
            Assert.Equal(comparer, copied.Comparer);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_Constructor_IDictionary(int count)
        {
            SCG.IDictionary<TKey, TValue> source = GenericIDictionaryFactory(count);
            SCG.IDictionary<TKey, TValue> copied = new SortedDictionary<TKey, TValue>(source);
            Assert.Equal(source, copied);
        }

        [Fact]
        public void SortedDictionary_Generic_Constructor_NullIDictionary_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedDictionary<TKey, TValue>((SCG.IDictionary<TKey, TValue>)null));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_Constructor_IDictionary_IComparer(int count)
        {
            SCG.IComparer<TKey> comparer = GetKeyIComparer();
            SCG.IDictionary<TKey, TValue> source = GenericIDictionaryFactory(count);
            SortedDictionary<TKey, TValue> sourceSorted = new SortedDictionary<TKey, TValue>(source, comparer);
            Assert.Equal(source, sourceSorted);
            Assert.Equal(comparer, sourceSorted.Comparer);
            // Test copying a sorted dictionary.
            SortedDictionary<TKey, TValue> copied = new SortedDictionary<TKey, TValue>(sourceSorted, comparer);
            Assert.Equal(sourceSorted, copied);
            Assert.Equal(comparer, copied.Comparer);
            // Test copying a sorted dictionary with a different comparer.
            SCG.IComparer<TKey> reverseComparer = SCG.Comparer<TKey>.Create((key1, key2) => -comparer.Compare(key1, key2));
            SortedDictionary<TKey, TValue> copiedReverse = new SortedDictionary<TKey, TValue>(sourceSorted, reverseComparer);
            Assert.Equal(sourceSorted, copiedReverse);
            Assert.Equal(reverseComparer, copiedReverse.Comparer);
        }

        #endregion

        #region ContainsValue

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_ContainsValue_NotPresent(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            TValue notPresent = CreateTValue(seed++);
            while (dictionary.Values.Contains(notPresent))
                notPresent = CreateTValue(seed++);
            Assert.False(dictionary.ContainsValue(notPresent));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_ContainsValue_Present(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            SCG.KeyValuePair<TKey, TValue> notPresent = CreateT(seed++);
            while (dictionary.Contains(notPresent))
                notPresent = CreateT(seed++);
            dictionary.Add(notPresent.Key, notPresent.Value);
            Assert.True(dictionary.ContainsValue(notPresent.Value));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_ContainsValue_DefaultValueNotPresent(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.False(dictionary.ContainsValue(default(TValue)));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_ContainsValue_DefaultValuePresent(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            TKey notPresent = CreateTKey(seed++);
            while (dictionary.ContainsKey(notPresent))
                notPresent = CreateTKey(seed++);
            dictionary.Add(notPresent, default(TValue));
            Assert.True(dictionary.ContainsValue(default(TValue)));
        }

        #endregion

        #region Ordering

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_DictionaryIsProperlySortedAccordingToComparer(int setLength)
        {
            SortedDictionary<TKey, TValue> set = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(setLength);
            J2N.Collections.Generic.List<SCG.KeyValuePair<TKey, TValue>> expected = set.ToList();
            expected.Sort(GetIComparer());
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in set)
                Assert.Equal(expected[expectedIndex++], value);
        }

        #endregion

#if FEATURE_IREADONLYCOLLECTIONS
        #region IReadOnlyDictionary<TKey, TValue>.Keys

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void IReadOnlyDictionary_Generic_Keys_ContainsAllCorrectKeys(int count)
        {
            SCG.IDictionary<TKey, TValue> dictionary = GenericIDictionaryFactory(count);
            SCG.IEnumerable<TKey> expected = dictionary.Select((pair) => pair.Key);
            SCG.IEnumerable<TKey> keys = ((SCG.IReadOnlyDictionary<TKey, TValue>)dictionary).Keys;
            Assert.True(expected.SequenceEqual(keys));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void IReadOnlyDictionary_Generic_Values_ContainsAllCorrectValues(int count)
        {
            SCG.IDictionary<TKey, TValue> dictionary = GenericIDictionaryFactory(count);
            SCG.IEnumerable<TValue> expected = dictionary.Select((pair) => pair.Value);
            SCG.IEnumerable<TValue> values = ((SCG.IReadOnlyDictionary<TKey, TValue>)dictionary).Values;
            Assert.True(expected.SequenceEqual(values));
        }

        #endregion
#endif

        #region Remove(TKey)

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_RemoveKey_ValidKeyNotContainedInDictionary(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TValue value;
            TKey missingKey = GetNewKey(dictionary);

            Assert.False(dictionary.Remove(missingKey, out value));
            Assert.Equal(count, dictionary.Count);
            Assert.Equal(default(TValue), value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_RemoveKey_ValidKeyContainedInDictionary(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey missingKey = GetNewKey(dictionary);
            TValue outValue;
            TValue inValue = CreateTValue(count);

            dictionary.Add(missingKey, inValue);
            Assert.True(dictionary.Remove(missingKey, out outValue));
            Assert.Equal(count, dictionary.Count);
            Assert.Equal(inValue, outValue);
            Assert.False(dictionary.TryGetValue(missingKey, out outValue));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_RemoveKey_DefaultKeyNotContainedInDictionary(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TValue outValue;

            if (DefaultValueAllowed)
            {
                TKey missingKey = default(TKey);
                while (dictionary.ContainsKey(missingKey))
                    dictionary.Remove(missingKey);
                Assert.False(dictionary.Remove(missingKey, out outValue));
                Assert.Equal(default(TValue), outValue);
            }
            else
            {
                TValue initValue = CreateTValue(count);
                outValue = initValue;
                Assert.Throws<ArgumentNullException>(() => dictionary.Remove(default(TKey), out outValue));
                Assert.Equal(initValue, outValue);
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_RemoveKey_DefaultKeyContainedInDictionary(int count)
        {
            if (DefaultValueAllowed)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)(GenericIDictionaryFactory(count));
                TKey missingKey = default(TKey);
                TValue value;

                dictionary.TryAdd(missingKey, default(TValue));
                Assert.True(dictionary.Remove(missingKey, out value));
            }
        }

        #endregion

        public static SCG.IEnumerable<object[]> SortedDictionary_GetAlternateLookup_OperationsMatchUnderlyingDictionary_MemberData()
        {
            yield return new object[] { EqualityComparer<string>.Default };
            yield return new object[] { StringComparer.Ordinal };
            yield return new object[] { StringComparer.OrdinalIgnoreCase };
            yield return new object[] { StringComparer.InvariantCulture };
            yield return new object[] { StringComparer.InvariantCultureIgnoreCase };
            yield return new object[] { StringComparer.CurrentCulture };
            yield return new object[] { StringComparer.CurrentCultureIgnoreCase };
        }

        #region GetSpanAlternateComparer

        [Fact]
        public void GetSpanAlternateLookup_FailsWhenIncompatible()
        {
            var dictionary = new SortedDictionary<string, string>(StringComparer.Ordinal);

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
        [MemberData(nameof(SortedDictionary_GetAlternateLookup_OperationsMatchUnderlyingDictionary_MemberData))]
        public void SortedDictionary_GetSpanAlternateLookup_OperationsMatchUnderlyingDictionary(SCG.IComparer<string> comparer)
        {
            // Test with a variety of comparers to ensure that the alternate lookup is consistent with the underlying dictionary
            SortedDictionary<string, int> dictionary = new(comparer);
            SortedDictionary<string, int>.SpanAlternateLookup<char> lookup = dictionary.GetSpanAlternateLookup<char>();
            Assert.Same(dictionary, lookup.Dictionary);
            Assert.Same(lookup.Dictionary, lookup.Dictionary);

            string actualKey;
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
            Assert.Throws<SCG.KeyNotFoundException>(() => lookup["123".AsSpan()]);

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
            if (dictionary.Comparer.Equals(Comparer<string>.Default) ||
                dictionary.Comparer.Equals(StringComparer.Ordinal) ||
                dictionary.Comparer.Equals(StringComparer.InvariantCulture) ||
                dictionary.Comparer.Equals(StringComparer.CurrentCulture))
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

            // Add some sequential items again
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(i, dictionary.Count);
                Assert.True(lookup.TryAdd(i.ToString().AsSpan(), i));
            }

            // Test TryGetPredecessor, TryGetSuccessor,
            // TryGetFlor, TryGetCeiling
            for (int i = 0; i < 5; i++)
            {
                string item = i.ToString();
                Assert.Equal(dictionary.TryGetPredecessor(item, out string predecessorKey, out int predecessorValue),
                    lookup.TryGetPredecessor(item.AsSpan(), out string spanPredecessorKey, out int spanPredecessorValue));
                Assert.Equal(predecessorKey, spanPredecessorKey);
                Assert.Equal(predecessorValue, spanPredecessorValue);

                Assert.Equal(dictionary.TryGetSuccessor(item, out string successorKey, out int successorValue),
                    lookup.TryGetSuccessor(item.AsSpan(), out string spanSuccessorKey, out int spanSuccessorValue));
                Assert.Equal(successorKey, spanSuccessorKey);
                Assert.Equal(successorValue, spanSuccessorValue);

                Assert.Equal(dictionary.TryGetFloor(item, out string floorKey, out int floorValue),
                    lookup.TryGetFloor(item.AsSpan(), out string spanFloorKey, out int spanFloorValue));
                Assert.Equal(floorKey, spanFloorKey);
                Assert.Equal(floorValue, spanFloorValue);

                Assert.Equal(dictionary.TryGetCeiling(item, out string ceilingKey, out int ceilingValue),
                    lookup.TryGetCeiling(item.AsSpan(), out string spanCeilingKey, out int spanCeilingValue));
                Assert.Equal(ceilingKey, spanCeilingKey);
                Assert.Equal(ceilingValue, spanCeilingValue);
            }
        }

        #endregion
    }
}
