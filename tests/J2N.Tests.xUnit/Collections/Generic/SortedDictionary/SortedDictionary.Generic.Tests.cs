﻿// Licensed to the .NET Foundation under one or more agreements.
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
    }
}
