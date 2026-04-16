// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;
using static J2N.Collections.Tests.NavigableCollectionHelper;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the Dictionary class.
    /// </summary>
    public abstract class SortedDictionary_Generic_Tests<TKey, TValue> : INavigableDictionary_Generic_Tests<TKey, TValue>
    {
        #region IDictionary<TKey, TValue> Helper Methods

        // J2N: Added virtual properties to control inclusivity of bounds in GetView tests

        private bool? _isDesending;
        protected bool IsDescending => _isDesending ??= IsReverseKeyIComparer(GetKeyIComparer());

        protected virtual bool LowerBoundInclusive => true;
        protected virtual bool UpperBoundInclusive => true;

        protected virtual bool FirstKeyInclusive => IsDescending ? UpperBoundInclusive : LowerBoundInclusive;
        protected virtual bool LastKeyInclusive => IsDescending ? LowerBoundInclusive : UpperBoundInclusive;

        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;
        protected override bool DefaultValueWhenNotAllowed_Throws { get { return false; } }

        protected override SCG.IDictionary<TKey, TValue> GenericIDictionaryFactory()
        {
            return new SortedDictionary<TKey, TValue>();
        }

        protected static bool IsReverseKeyIComparer(SCG.IComparer<TKey> comparer)
        {
            return comparer is ReverseComparer<TKey>;
        }

        private SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> GetForwardIComparer()
        {
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer();

            if (comparer is KVPComparer kvpComparer && kvpComparer.KeyComparer is ReverseComparer<TKey> reverse)
                return new KVPComparer(reverse.InnerComparer, kvpComparer.KeyEqualityComparer);

            return comparer;
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
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(setLength);
            List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
            expected.Sort(GetIComparer());
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in dictionary)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_ReverseDictionaryIsProperlySortedAccordingToComparer(int setLength)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(setLength);
            List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in dictionary.Reverse())
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewDescending_IsProperlySortedAccordingToComparer(int setLength)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(setLength);
            SortedDictionary<TKey, TValue> descendingDictionary = dictionary.GetViewDescending();
            List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in descendingDictionary)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewDescending_GetViewDescending_IsProperlySortedAccordingToComparer(int setLength)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(setLength);
            SortedDictionary<TKey, TValue> doubleDescendingDictionary = dictionary.GetViewDescending().GetViewDescending();
            List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
            expected.Sort(GetIComparer());
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in doubleDescendingDictionary)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewDescending_HasComparerWithReversedBehavior(int setLength)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(setLength);
            SortedDictionary<TKey, TValue> descendingDictionary = dictionary.GetViewDescending();
            SortedDictionary<TKey, TValue> doubleDescendingDictionary = dictionary.GetViewDescending().GetViewDescending();
            SCG.IComparer<TKey> originalComparer = dictionary.Comparer;
            SCG.IComparer<TKey> descendingComparer = descendingDictionary.Comparer;
            SCG.IComparer<TKey> doubleDescendingComparer = doubleDescendingDictionary.Comparer;

            List<TKey> keys = dictionary.Keys.ToList();
            int limit = Math.Min(keys.Count, 10);

            for (int i = 0; i < limit; i++)
            {
                for (int j = 0; j < limit; j++)
                {
                    TKey a = keys[i];
                    TKey b = keys[j];

                    int original = originalComparer.Compare(a, b);

                    int reverse = descendingComparer.Compare(a, b);
                    int reverseSwapped = descendingComparer.Compare(b, a);

                    int forward = doubleDescendingComparer.Compare(a, b);
                    int forwardSwapped = doubleDescendingComparer.Compare(b, a);

                    // Core invariant: reversed ordering
                    Assert.Equal(Math.Sign(original), -Math.Sign(reverse));

                    // Symmetry invariant
                    Assert.Equal(Math.Sign(original), Math.Sign(reverseSwapped));

                    // Core invariant: double-reversed ordering (forward)
                    Assert.Equal(Math.Sign(original), Math.Sign(forward));

                    // Symmetry invariant (double-reversed)
                    Assert.Equal(Math.Sign(original), -Math.Sign(forwardSwapped));
                }
            }
        }

        [Fact]
        public void SortedDictionary_Generic_TestSubSetEnumerator()
        {
            SortedDictionary<int, int> sortedSet = new SortedDictionary<int, int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.ContainsKey(i))
                    sortedSet.Add(i, i);
            }
            SortedDictionary<int, int> mySubSet = sortedSet.GetView(45, 90);

            Assert.Equal(46, mySubSet.Count); //"not all elements were encountered"

            SCG.IEnumerable<SCG.KeyValuePair<int, int>> en = mySubSet.Reverse();
            SortedDictionary<int, int> descending = mySubSet.GetViewDescending();

            // J2N: Added asserts for descending set comparison
            using var descendingEnumerator = descending.GetEnumerator();
            foreach (SCG.KeyValuePair<int, int> element in en)
            {
                Assert.True(descendingEnumerator.MoveNext());
                Assert.Equal(element, descendingEnumerator.Current);
            }
            Assert.False(descendingEnumerator.MoveNext());
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
        public virtual void SortedDictionary_Generic_RemoveKey_DefaultKeyNotContainedInDictionary(int count)
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

        #region GetSpanAlternateLookup

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
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void SortedDictionary_GetSpanAlternateLookup_OperationsMatchUnderlyingDictionary(int mode)
        {
            // Test with a variety of comparers to ensure that the alternate lookup is consistent with the underlying dictionary
            SortedDictionary<string, int> dictionary = new(mode switch
            {
                0 => StringComparer.Ordinal,
                1 => StringComparer.OrdinalIgnoreCase,
                2 => StringComparer.InvariantCulture,
                3 => StringComparer.InvariantCultureIgnoreCase,
                4 => StringComparer.CurrentCulture,
                5 => StringComparer.CurrentCultureIgnoreCase,
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            });

            AssertSpanLookupMatchesRootDictionary(dictionary);
            dictionary.Clear();
            AssertSpanLookupMatchesRootDictionary(dictionary.GetViewDescending());
        }

        [Fact]
        public void SortedDictionary_GetSpanAlternateLookup_GetView_MatchesDictionary()
        {
            var dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                dictionary.Add(i.ToString(), i);

            var lookup = dictionary.GetSpanAlternateLookup<char>();

            AssertLookupMatchesDictionary(dictionary, lookup, "3", "6");

            // Descending dictionary/view
            var descendingDictionary = dictionary.GetViewDescending();
            var descendingLookup = descendingDictionary.GetSpanAlternateLookup<char>();

            AssertLookupMatchesDictionary(descendingDictionary, descendingLookup, "6", "3");

            static void AssertLookupMatchesDictionary(SortedDictionary<string, int> dictionary, SortedDictionary<string, int>.SpanAlternateLookup<char> lookup, string from, string to)
            {
                // Inclusive
                var dictionaryView = dictionary.GetView(from, to);
                var lookupView = lookup.GetView(from.AsSpan(), to.AsSpan());

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());

                dictionaryView = dictionary.GetView(from, true, to, true);
                lookupView = lookup.GetView(from.AsSpan(), true, to.AsSpan(), true);

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());

                // Exclusive
                dictionaryView = dictionary.GetView(from, false, to, false);
                lookupView = lookup.GetView(from.AsSpan(), false, to.AsSpan(), false);

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_GetSpanAlternateLookup_GetView_LowerValueGreaterThanUpperValue_ThrowsArgumentException_MatchingDictionary(int count)
        {
            if (count >= 2)
            {
                SCG.IComparer<string> comparer = StringComparer.Ordinal;
                var dictionary = new SortedDictionary<string, int>(comparer);
                for (int i = 0; i < count; i++)
                    dictionary.Add(i.ToString(), i);

                Assert_LowerValueGreaterThanUpperValue_ThrowsArgumentException_MatchingDictionary(dictionary, comparer, count);
                Assert_LowerValueGreaterThanUpperValue_ThrowsArgumentException_MatchingDictionary(dictionary.GetViewDescending(), ReverseComparer<string>.Create(comparer), count);
            }

            static void Assert_LowerValueGreaterThanUpperValue_ThrowsArgumentException_MatchingDictionary(SortedDictionary<string, int> dictionary, SCG.IComparer<string> comparer, int count)
            {
                string firstElement = dictionary.ElementAt(0).Key;
                string lastElement = dictionary.ElementAt(count - 1).Key;
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    var lookup = dictionary.GetSpanAlternateLookup<char>();

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                        () => dictionary.GetView(lastElement, firstElement),
                        () => lookup.GetView(lastElement.AsSpan(), firstElement.AsSpan()));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                        () => dictionary.GetView(lastElement, fromInclusive: true, firstElement, toInclusive: true),
                        () => lookup.GetView(lastElement.AsSpan(), fromInclusive: true, firstElement.AsSpan(), toInclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                        () => dictionary.GetView(lastElement, fromInclusive: true, firstElement, toInclusive: false),
                        () => lookup.GetView(lastElement.AsSpan(), fromInclusive: true, firstElement.AsSpan(), toInclusive: false));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                        () => dictionary.GetView(lastElement, fromInclusive: false, firstElement, toInclusive: true),
                        () => lookup.GetView(lastElement.AsSpan(), fromInclusive: false, firstElement.AsSpan(), toInclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                        () => dictionary.GetView(lastElement, fromInclusive: false, firstElement, toInclusive: false),
                        () => lookup.GetView(lastElement.AsSpan(), fromInclusive: false, firstElement.AsSpan(), toInclusive: false));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_GetSpanAlternateLookup_GetView_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(int count)
        {
            if (count >= 3)
            {
                SCG.IComparer<string> comparer = StringComparer.Ordinal;
                var dictionary = new SortedDictionary<string, int>(comparer);
                for (int i = 0; i < count; i++)
                    dictionary.Add(i.ToString(), i);

                Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(dictionary, comparer, count);
                Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(dictionary.GetViewDescending(), ReverseComparer<string>.Create(comparer), count);
            }

            static void Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(SortedDictionary<string, int> dictionary, SCG.IComparer<string> comparer, int count)
            {
                string firstElement = dictionary.ElementAt(0).Key;
                string middleElement = dictionary.ElementAt(count / 2).Key;
                string lastElement = dictionary.ElementAt(count - 1).Key;
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    SortedDictionary<string, int> view = dictionary.GetView(firstElement, middleElement);
                    var lookup = view.GetSpanAlternateLookup<char>();

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, lastElement),
                        () => lookup.GetView(middleElement.AsSpan(), lastElement.AsSpan()));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, fromInclusive: true, lastElement, toInclusive: true),
                        () => lookup.GetView(middleElement.AsSpan(), fromInclusive: true, lastElement.AsSpan(), toInclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, fromInclusive: true, lastElement, toInclusive: false),
                        () => lookup.GetView(middleElement.AsSpan(), fromInclusive: true, lastElement.AsSpan(), toInclusive: false));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, fromInclusive: false, lastElement, toInclusive: true),
                        () => lookup.GetView(middleElement.AsSpan(), fromInclusive: false, lastElement.AsSpan(), toInclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, fromInclusive: false, lastElement, toInclusive: false),
                        () => lookup.GetView(middleElement.AsSpan(), fromInclusive: false, lastElement.AsSpan(), toInclusive: false));
                }
            }
        }

        [Fact]
        public void SortedDictionary_GetSpanAlternateLookup_GetViewBefore_MatchesDictionary()
        {
            var dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                dictionary.Add(i.ToString(), i);

            var lookup = dictionary.GetSpanAlternateLookup<char>();

            AssertLookupMatchesDictionary(dictionary, lookup);

            // Descending dictionary/view
            var descendingDictionary = dictionary.GetViewDescending();
            var descendingLookup = descendingDictionary.GetSpanAlternateLookup<char>();

            AssertLookupMatchesDictionary(descendingDictionary, descendingLookup);

            static void AssertLookupMatchesDictionary(SortedDictionary<string, int> dictionary, SortedDictionary<string, int>.SpanAlternateLookup<char> lookup)
            {
                // Inclusive
                var dictionaryView = dictionary.GetViewBefore("6");
                var lookupView = lookup.GetViewBefore("6".AsSpan());

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());

                dictionaryView = dictionary.GetViewBefore("6", true);
                lookupView = lookup.GetViewBefore("6".AsSpan(), true);

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());

                // Exclusive
                dictionaryView = dictionary.GetViewBefore("6", false);
                lookupView = lookup.GetViewBefore("6".AsSpan(), false);

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_GetSpanAlternateLookup_GetViewBefore_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                SCG.IComparer<string> comparer = StringComparer.Ordinal;
                var dictionary = new SortedDictionary<string, int>(comparer);
                for (int i = 0; i < count; i++)
                    dictionary.Add(i.ToString(), i);

                Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(dictionary, comparer, count);
                Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(dictionary.GetViewDescending(), ReverseComparer<string>.Create(comparer), count);
            }

            static void Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(SortedDictionary<string, int> dictionary, SCG.IComparer<string> comparer, int count)
            {
                string firstElement = dictionary.ElementAt(0).Key;
                string middleElement = dictionary.ElementAt(count / 2).Key;
                string lastElement = dictionary.ElementAt(count - 1).Key;
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    // J2N: this was confirmed to match JDK behavior
                    SortedDictionary<string, int> view = dictionary.GetView(firstElement, middleElement);
                    var lookup = view.GetSpanAlternateLookup<char>();

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetViewBefore(lastElement),
                        () => lookup.GetViewBefore(lastElement.AsSpan()));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetViewBefore(lastElement, inclusive: true),
                        () => lookup.GetViewBefore(lastElement.AsSpan(), inclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetViewBefore(lastElement, inclusive: false),
                        () => lookup.GetViewBefore(lastElement.AsSpan(), inclusive: false));

                    Assert.NotNull(lookup.GetViewBefore(middleElement.AsSpan(), inclusive: true));
                    Assert.NotNull(lookup.GetViewBefore(middleElement.AsSpan(), inclusive: false));
                }
            }
        }

        [Fact]
        public void SortedDictionary_GetSpanAlternateLookup_GetViewAfter_MatchesDictionary()
        {
            var dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                dictionary.Add(i.ToString(), i);

            var lookup = dictionary.GetSpanAlternateLookup<char>();

            AssertLookupMatchesDictionary(dictionary, lookup);

            // Descending dictionary/view
            var descendingDictionary = dictionary.GetViewDescending();
            var descendingLookup = descendingDictionary.GetSpanAlternateLookup<char>();

            AssertLookupMatchesDictionary(descendingDictionary, descendingLookup);

            static void AssertLookupMatchesDictionary(SortedDictionary<string, int> dictionary, SortedDictionary<string, int>.SpanAlternateLookup<char> lookup)
            {
                // Inclusive
                var dictionaryView = dictionary.GetViewAfter("3");
                var lookupView = lookup.GetViewAfter("3".AsSpan());

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());

                dictionaryView = dictionary.GetViewAfter("3", true);
                lookupView = lookup.GetViewAfter("3".AsSpan(), true);

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());

                // Exclusive
                dictionaryView = dictionary.GetViewAfter("3", false);
                lookupView = lookup.GetViewAfter("3".AsSpan(), false);

                Assert.Equal(dictionaryView.ToArray(), lookupView.ToArray());
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_GetSpanAlternateLookup_GetViewAfter_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                SCG.IComparer<string> comparer = StringComparer.Ordinal;
                var dictionary = new SortedDictionary<string, int>(comparer);
                for (int i = 0; i < count; i++)
                    dictionary.Add(i.ToString(), i);

                Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(dictionary, comparer, count);
                Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(dictionary.GetViewDescending(), ReverseComparer<string>.Create(comparer), count);
            }

            static void Asssert_SubsquentCallOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingDictionary(SortedDictionary<string, int> dictionary, SCG.IComparer<string> comparer, int count)
            {
                string firstElement = dictionary.ElementAt(0).Key;
                string middleElement = dictionary.ElementAt(count / 2).Key;
                string lastElement = dictionary.ElementAt(count - 1).Key;
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    // J2N: this was confirmed to match JDK behavior
                    SortedDictionary<string, int> view = dictionary.GetViewAfter(middleElement);
                    var lookup = view.GetSpanAlternateLookup<char>();

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetViewAfter(firstElement),
                        () => lookup.GetViewAfter(firstElement.AsSpan()));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetViewAfter(firstElement, inclusive: true),
                        () => lookup.GetViewAfter(firstElement.AsSpan(), inclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetViewAfter(firstElement, inclusive: false),
                        () => lookup.GetViewAfter(firstElement.AsSpan(), inclusive: false));

                    Assert.NotNull(lookup.GetViewAfter(middleElement.AsSpan(), inclusive: true));
                    Assert.NotNull(lookup.GetViewAfter(middleElement.AsSpan(), inclusive: false));
                }
            }
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SortedDictionary_GetSpanAlternateLookup_WorksOnView(
           bool fromInclusive,
           bool toInclusive)
        {
            var dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                dictionary.Add(i.ToString("D2"), i);

            // View: [02,07]
            var view = dictionary.GetView("02", fromInclusive, "07", toInclusive);

            int minInclusive = fromInclusive ? 2 : 3;
            int maxInclusive = toInclusive ? 7 : 6;

            AssertSpanLookupMatchesView(view, minInclusive, maxInclusive);

            dictionary.Clear();
            for (int i = 0; i < 10; i++)
                dictionary.Add(i.ToString("D2"), i);

            AssertSpanLookupMatchesView(view.GetViewDescending(), minInclusive, maxInclusive);

            int actualLower = fromInclusive ? 1 : 2;
            int actualUpper = toInclusive ? 8 : 7;

            // Special case - if both bounds are exclusive, the "closed range" rule takes effect.
            // In this case, we adjust the upper up 1 because [02,07] would not throw.
            if (!fromInclusive && !toInclusive)
            {
                actualUpper = 8;
            }

            AssertSpanLookupRejectsOutOfRangeValues(
                view,
                actualLower,
                actualUpper,
                fromInclusive,
                toInclusive);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SortedDictionary_GetSpanAlternateLookup_WorksOnNestedView(
            bool fromInclusive,
            bool toInclusive)
        {
            var dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                dictionary.Add(i.ToString("D2"), i);

            var view1 = dictionary.GetView("02", fromInclusive, "08", toInclusive);
            var view2 = view1.GetView("03", fromInclusive, "06", toInclusive);

            int minInclusive = fromInclusive ? 3 : 4;
            int maxInclusive = toInclusive ? 6 : 5;

            AssertSpanLookupMatchesView(view2, minInclusive, maxInclusive);

            dictionary.Clear();
            for (int i = 0; i < 10; i++)
                dictionary.Add(i.ToString("D2"), i);

            AssertSpanLookupMatchesView(view2.GetViewDescending(), minInclusive, maxInclusive);

            int lowerReject = fromInclusive ? 2 : 3;
            int upperReject = toInclusive ? 7 : 6;

            // Special case - if both bounds are exclusive, the "closed range" rule takes effect.
            // In this case, we adjust the upper up 1 because [03,06] would not throw.
            if (!fromInclusive && !toInclusive)
            {
                upperReject = 7;
            }

            AssertSpanLookupRejectsOutOfRangeValues(
                view2,
                lowerReject,
                upperReject,
                fromInclusive,
                toInclusive);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SortedDictionary_GetSpanAlternateLookup_WorksOnDeeplyNestedViews(
            bool fromInclusive,
            bool toInclusive)
        {
            var dictionary = new SortedDictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < 20; i++)
                dictionary.Add(i.ToString("D2"), i);

            var v1 = dictionary.GetView("01", fromInclusive, "18", toInclusive);
            var v2 = v1.GetView("03", fromInclusive, "15", toInclusive);
            var v3 = v2.GetView("05", fromInclusive, "10", toInclusive);

            int minInclusive = fromInclusive ? 5 : 6;
            int maxInclusive = toInclusive ? 10 : 9;

            AssertSpanLookupMatchesView(v3, minInclusive, maxInclusive);

            dictionary.Clear();
            for (int i = 0; i < 20; i++)
                dictionary.Add(i.ToString("D2"), i);

            AssertSpanLookupMatchesView(v3.GetViewDescending(), minInclusive, maxInclusive);

            int lowerReject = fromInclusive ? 4 : 5;
            int upperReject = toInclusive ? 11 : 10;

            // Special case - if both bounds are exclusive, the "closed range" rule takes effect.
            // In this case, we adjust the upper up 1 because [05,10] would not throw.
            if (!fromInclusive && !toInclusive)
            {
                upperReject = 11;
            }

            AssertSpanLookupRejectsOutOfRangeValues(
                v3,
                lowerReject,
                upperReject,
                fromInclusive,
                toInclusive);
        }

        private static void AssertSpanLookupMatchesRootDictionary(SortedDictionary<string, int> dictionary)
        {
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
                dictionary.Comparer.Equals(StringComparer.CurrentCulture) ||
                dictionary.Comparer.Equals(ReverseComparer<string>.Create(StringComparer.Ordinal)) ||
                dictionary.Comparer.Equals(ReverseComparer<string>.Create(StringComparer.InvariantCulture)) ||
                dictionary.Comparer.Equals(ReverseComparer<string>.Create(StringComparer.CurrentCulture)))
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

        private static void AssertSpanLookupMatchesView(SortedDictionary<string, int> dictionary, int minInclusive, int maxInclusive)
        {
            var lookup = dictionary.GetSpanAlternateLookup<char>();
            Assert.Same(dictionary, lookup.Dictionary);

            string actualKey;
            int value;

            // in-range add/remove
            for (int i = minInclusive; i <= maxInclusive; i++)
            {
                string s = i.ToString("D2");

                Assert.False(lookup.TryAdd(s.AsSpan(), i));
                Assert.False(dictionary.TryAdd(s, i));

                Assert.True(lookup.ContainsKey(s.AsSpan()));
                Assert.True(dictionary.ContainsKey(s));

                lookup.TryGetValue(s.AsSpan(), out actualKey, out value);
                Assert.Equal(s, actualKey);
                Assert.Equal(i, value);

                dictionary.TryGetValue(s, out value);
                Assert.Equal(i, value);
            }

            int spanValue;

            // predecessor / successor within range
            for (int i = minInclusive; i <= maxInclusive; i++)
            {
                string s = i.ToString("D2");

                Assert.Equal(
                    dictionary.TryGetPredecessor(s, out string predecessor, out value),
                    lookup.TryGetPredecessor(s.AsSpan(), out string spanPredecessor, out spanValue));
                Assert.Equal(predecessor, spanPredecessor);
                Assert.Equal(value, spanValue);

                Assert.Equal(
                    dictionary.TryGetSuccessor(s, out string successor, out value),
                    lookup.TryGetSuccessor(s.AsSpan(), out var spanSuccessor, out spanValue));
                Assert.Equal(successor, spanSuccessor);
                Assert.Equal(value, spanValue);

                Assert.Equal(
                    dictionary.TryGetPredecessor(s, out string floor, out value),
                    lookup.TryGetPredecessor(s.AsSpan(), out string spanFloor, out spanValue));
                Assert.Equal(floor, spanFloor);
                Assert.Equal(value, spanValue);

                Assert.Equal(
                    dictionary.TryGetSuccessor(s, out string ceiling, out value),
                    lookup.TryGetSuccessor(s.AsSpan(), out string spanCeiling, out spanValue));
                Assert.Equal(ceiling, spanCeiling);
                Assert.Equal(value, spanValue);
            }

            // in-range remove
            for (int i = maxInclusive; i >= minInclusive; i--)
            {
                string s = i.ToString("D2");
                Assert.True(lookup.Remove(s.AsSpan()));
                Assert.False(lookup.Remove(s.AsSpan()));

                Assert.False(dictionary.Remove(s));
            }

            Assert.Equal(0, dictionary.Count);
        }

        private static void AssertSpanLookupRejectsOutOfRangeValues(SortedDictionary<string, int> dictionary, int below, int above, bool fromInclusive, bool toInclusive)
        {
            var lookup = dictionary.GetSpanAlternateLookup<char>();

            string low = below.ToString("D2");
            string high = above.ToString("D2");

            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Add(low, below));
            Assert.False(dictionary.TryAdd(low, below));
            Assert.False(lookup.TryAdd(low.AsSpan(), below));

            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Add(high, above));
            Assert.False(dictionary.TryAdd(high, above));
            Assert.False(lookup.TryAdd(high.AsSpan(), above));

            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.GetView(low, high));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetView(low.AsSpan(), high.AsSpan()));
            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.GetView(low, fromInclusive, high, toInclusive));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetView(low.AsSpan(), fromInclusive, high.AsSpan(), toInclusive));
        }

        #endregion GetSpanAlternateLookup

        #region First and Last

        // J2N: Added TryGetFirst and TryGetLast methods to replace Min and Max
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_FirstAndLast(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SortedDictionary<TKey, TValue> descendingDictionary = dictionary.GetViewDescending();
            if (count > 0)
            {
                List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
                expected.Sort(GetIComparer());

                AssertFirstLastMatch(expected, dictionary, count);

                expected.Reverse();

                AssertFirstLastMatch(expected, descendingDictionary, count);
            }
            else
            {
                Assert.False(dictionary.TryGetFirst(out TKey key, out TValue value));
                Assert.Equal(default(TKey), key);
                Assert.Equal(default(TValue), value);

                Assert.False(dictionary.TryGetLast(out key, out value));
                Assert.Equal(default(TKey), key);
                Assert.Equal(default(TValue), value);
            }

            static void AssertFirstLastMatch(List<SCG.KeyValuePair<TKey, TValue>> expected, SortedDictionary<TKey, TValue> dictionary, int count)
            {
                Assert.True(dictionary.TryGetFirst(out TKey key, out TValue value));
                Assert.Equal(expected[0].Key, key);
                Assert.Equal(expected[0].Value, value);

                Assert.True(dictionary.TryGetLast(out key, out value));
                Assert.Equal(expected[count - 1].Key, key);
                Assert.Equal(expected[count - 1].Value, value);
            }
        }

        #endregion First and Last

        #region GetView

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, lastElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Inclusive_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, true, lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Inclusive_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, true, lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Exclusive_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, false, lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, false, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Exclusive_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, false, lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, false, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, lastElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Inclusive_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, true, lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Inclusive_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, true, lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Exclusive_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, false, lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, false, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Exclusive_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, false, lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, false, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int count)
        {
            if (count >= 2)
            {
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer();
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    const string fromArgumentName = "fromKey";
                    const string toArgumentName = "toKey";
                    string lowerArgumentName = IsDescending ? toArgumentName : fromArgumentName;
                    string upperArgumentName = IsDescending ? fromArgumentName : toArgumentName;

                    ArgumentException exception = AssertExtensions.Throws<ArgumentException>(() => dictionary.GetView(lastElement.Key, firstElement.Key));
                    Assert.Equal(lowerArgumentName, exception.ParamName);
                    Assert.Contains(upperArgumentName, exception.Message);
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Inclusive_Inclusive_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int count)
        {
            if (count >= 2)
            {
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer();
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    const string fromArgumentName = "fromKey";
                    const string toArgumentName = "toKey";
                    string lowerArgumentName = IsDescending ? toArgumentName : fromArgumentName;
                    string upperArgumentName = IsDescending ? fromArgumentName : toArgumentName;

                    ArgumentException exception = AssertExtensions.Throws<ArgumentException>(() => dictionary.GetView(lastElement.Key, true, firstElement.Key, true));
                    Assert.Equal(lowerArgumentName, exception.ParamName);
                    Assert.Contains(upperArgumentName, exception.Message);
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer();
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> middleElement = dictionary.ElementAt(count / 2);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, middleElement.Key);
                    Assert.Throws<ArgumentOutOfRangeException>("toKey", () => view.GetView(middleElement.Key, lastElement.Key));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Inclusive_Inclusive_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer();
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> middleElement = dictionary.ElementAt(count / 2);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    SortedDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, middleElement.Key);
                    Assert.Throws<ArgumentOutOfRangeException>("toKey", () => view.GetView(middleElement.Key, true, lastElement.Key, true));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetView_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.Equal(count, dictionary.Count);

            SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
            SCG.KeyValuePair<TKey, TValue> secondElement = dictionary.ElementAt(1);
            SCG.KeyValuePair<TKey, TValue> nextToLastElement = dictionary.ElementAt(count - 2);
            SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);

            SCG.KeyValuePair<TKey, TValue>[] items = dictionary.ToArray();
            for (int i = 1; i < count - 1; i++)
            {
                dictionary.Remove(items[i].Key);
            }
            Assert.Equal(2, dictionary.Count);

            SortedDictionary<TKey, TValue> view = dictionary.GetView(secondElement.Key, nextToLastElement.Key);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out TKey key, out TValue value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);

            Assert.False(view.TryGetLast(out key, out value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);
        }

        #endregion GetView

        #region GetViewBefore

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewBefore_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewBefore_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewBefore_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewBefore_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewBefore_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewBefore_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewBefore_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer();
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> middleElement = dictionary.ElementAt(count / 2);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if (comparer.Compare(middleElement, lastElement) < 0)
                {
                    // J2N: this was confirmed to match JDK behavior
                    SortedDictionary<TKey, TValue> view = dictionary.GetViewBefore(middleElement.Key);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement.Key));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement.Key, inclusive: true));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement.Key, inclusive: false));
                    Assert.NotNull(view.GetViewBefore(middleElement.Key, inclusive: true));
                    Assert.NotNull(view.GetViewBefore(middleElement.Key, inclusive: false));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewBefore_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.Equal(count, dictionary.Count);

            SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
            SCG.KeyValuePair<TKey, TValue> secondElement = dictionary.ElementAt(1);
            SCG.KeyValuePair<TKey, TValue> nextToLastElement = dictionary.ElementAt(count - 2);
            SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);

            SCG.KeyValuePair<TKey, TValue>[] items = dictionary.ToArray();
            for (int i = 0; i < count - 1; i++)
            {
                dictionary.Remove(items[i].Key);
            }
            Assert.Equal(1, dictionary.Count);

            SortedDictionary<TKey, TValue> view = dictionary.GetViewBefore(nextToLastElement.Key);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out TKey key, out TValue value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);

            Assert.False(view.TryGetLast(out key, out value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);
        }

        #endregion GetViewBefore

        #region GetViewAfter

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewAfter_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewAfter_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewAfter_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewAfter_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewAfter_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewAfter_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                SortedDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewAfter_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer();
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> middleElement = dictionary.ElementAt(count / 2);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    // J2N: this was confirmed to match JDK behavior
                    SortedDictionary<TKey, TValue> view = dictionary.GetViewAfter(middleElement.Key);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement.Key));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement.Key, inclusive: true));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement.Key, inclusive: false));
                    Assert.NotNull(view.GetViewAfter(middleElement.Key, inclusive: true));
                    Assert.NotNull(view.GetViewAfter(middleElement.Key, inclusive: false));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_GetViewAfter_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.Equal(count, dictionary.Count);

            SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
            SCG.KeyValuePair<TKey, TValue> secondElement = dictionary.ElementAt(1);
            SCG.KeyValuePair<TKey, TValue> nextToLastElement = dictionary.ElementAt(count - 2);
            SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);

            SCG.KeyValuePair<TKey, TValue>[] items = dictionary.ToArray();
            for (int i = 1; i < count; i++)
            {
                dictionary.Remove(items[i].Key);
            }
            Assert.Equal(1, dictionary.Count);

            SortedDictionary<TKey, TValue> view = dictionary.GetViewAfter(secondElement.Key);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out TKey key, out TValue value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);

            Assert.False(view.TryGetLast(out key, out value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);
        }

        #endregion GetViewAfter

        #region CopyTo

        // J2N: Added to test descending dictionary CopyTo method
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_CopyTo_GetViewDescending_WithIndex_PreservesReverseOrder(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SortedDictionary<TKey, TValue> descendingDictionary = dictionary.GetViewDescending();
            List<SCG.KeyValuePair<TKey, TValue>> expected = descendingDictionary.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            SCG.KeyValuePair<TKey, TValue>[] actual = new SCG.KeyValuePair<TKey, TValue>[count];
            descendingDictionary.CopyTo(actual, 0);
            Assert.Equal(expected, actual);
        }

        #endregion CopyTo

        #region TryGetPredecessor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_TryGetPredecessor(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer() ?? Comparer<SCG.KeyValuePair<TKey, TValue>>.Default;

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                bool foundExpected = TryGetPredecessorExpected(dictionary, kvp, out var expectedKvp, comparer);
                bool foundActual = dictionary.TryGetPredecessor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }

            // Descending view
            SortedDictionary<TKey, TValue> desc = dictionary.GetViewDescending();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in desc)
            {
                bool foundExpected = TryGetPredecessorExpected(desc, kvp, out var expectedKvp, ReverseComparer<SCG.KeyValuePair<TKey, TValue>>.Create(comparer));
                bool foundActual = desc.TryGetPredecessor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }
        }

        #endregion TryGetPredecessor

        #region TryGetSuccessor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_TryGetSuccessor(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer() ?? Comparer<SCG.KeyValuePair<TKey, TValue>>.Default;

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                bool foundExpected = TryGetSuccessorExpected(dictionary, kvp, out var expectedKvp, comparer);
                bool foundActual = dictionary.TryGetSuccessor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }

            // Descending view
            SortedDictionary<TKey, TValue> desc = dictionary.GetViewDescending();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in desc)
            {
                bool foundExpected = TryGetSuccessorExpected(desc, kvp, out var expectedKvp, ReverseComparer<SCG.KeyValuePair<TKey, TValue>>.Create(comparer));
                bool foundActual = desc.TryGetSuccessor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }
        }

        #endregion TryGetSuccessor

        #region TryGetFloor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_TryGetFloor(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer() ?? Comparer<SCG.KeyValuePair<TKey, TValue>>.Default;

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                bool foundExpected = TryGetFloorExpected(dictionary, kvp, out var expectedKvp, comparer);
                bool foundActual = dictionary.TryGetFloor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }

            SortedDictionary<TKey, TValue> desc = dictionary.GetViewDescending();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in desc)
            {
                bool foundExpected = TryGetFloorExpected(desc, kvp, out var expectedKvp, ReverseComparer<SCG.KeyValuePair<TKey, TValue>>.Create(comparer));
                bool foundActual = desc.TryGetFloor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }
        }

        #endregion TryGetFloor

        #region TryGetCeiling

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_TryGetCeiling(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparer() ?? Comparer<SCG.KeyValuePair<TKey, TValue>>.Default;

            foreach (SCG.KeyValuePair<TKey, TValue> value in dictionary)
            {
                bool foundExpected = TryGetCeilingExpected(dictionary, value, out var expectedKvp, comparer);
                bool foundActual = dictionary.TryGetCeiling(value.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }

            SortedDictionary<TKey, TValue> desc = dictionary.GetViewDescending();

            foreach (SCG.KeyValuePair<TKey, TValue> value in desc)
            {
                bool foundExpected = TryGetCeilingExpected(desc, value, out var expectedKvp, ReverseComparer<SCG.KeyValuePair<TKey, TValue>>.Create(comparer));
                bool foundActual = desc.TryGetCeiling(value.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }
        }

        #endregion TryGetCeiling
    }
}
