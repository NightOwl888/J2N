﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//using Common.System;
using J2N.Collections.Generic;
using J2N.TestUtilities;
using J2N.TestUtilities.Xunit;
using System;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the LinkedDictionary class.
    /// </summary>
    public abstract class LinkedDictionary_Generic_Tests<TKey, TValue> : IDictionary_Generic_Tests<TKey, TValue>
    {

        #region IDictionary<TKey, TValue Helper Methods

        protected override SCG.IDictionary<TKey, TValue> GenericIDictionaryFactory()
        {
            return new J2N.Collections.Generic.LinkedDictionary<TKey, TValue>();
        }

        protected override Type ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

        #endregion

        #region Constructors

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_Constructor_IDictionary(int count)
        {
            SCG.IDictionary<TKey, TValue> source = GenericIDictionaryFactory(count);
            SCG.IDictionary<TKey, TValue> copied = new LinkedDictionary<TKey, TValue>(source);
            Assert.Equal(source, copied);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_Constructor_IDictionary_IEqualityComparer(int count)
        {
            SCG.IEqualityComparer<TKey> comparer = GetKeyIEqualityComparer();
            SCG.IDictionary<TKey, TValue> source = GenericIDictionaryFactory(count);
            LinkedDictionary<TKey, TValue> copied = new LinkedDictionary<TKey, TValue>(source, comparer);
            Assert.Equal(source, copied);
            Assert.Equal(comparer, copied.EqualityComparer);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_Constructor_IEqualityComparer(int count)
        {
            SCG.IEqualityComparer<TKey> comparer = GetKeyIEqualityComparer();
            SCG.IDictionary<TKey, TValue> source = GenericIDictionaryFactory(count);
            LinkedDictionary<TKey, TValue> copied = new LinkedDictionary<TKey, TValue>(source, comparer);
            Assert.Equal(source, copied);
            Assert.Equal(comparer, copied.EqualityComparer);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_Constructor_int(int count)
        {
            SCG.IDictionary<TKey, TValue> dictionary = new LinkedDictionary<TKey, TValue>(count);
            Assert.Equal(0, dictionary.Count);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_Constructor_int_IEqualityComparer(int count)
        {
            SCG.IEqualityComparer<TKey> comparer = GetKeyIEqualityComparer();
            LinkedDictionary<TKey, TValue> dictionary = new LinkedDictionary<TKey, TValue>(count, comparer);
            Assert.Equal(0, dictionary.Count);
            Assert.Equal(comparer, dictionary.EqualityComparer);
        }

        #endregion

        #region ContainsValue

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_ContainsValue_NotPresent(int count)
        {
            LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            TValue notPresent = CreateTValue(seed++);
            while (dictionary.Values.Contains(notPresent))
                notPresent = CreateTValue(seed++);
            Assert.False(dictionary.ContainsValue(notPresent));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_ContainsValue_Present(int count)
        {
            LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            SCG.KeyValuePair<TKey, TValue> notPresent = CreateT(seed++);
            while (dictionary.Contains(notPresent))
                notPresent = CreateT(seed++);
            dictionary.Add(notPresent.Key, notPresent.Value);
            Assert.True(dictionary.ContainsValue(notPresent.Value));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_ContainsValue_DefaultValueNotPresent(int count)
        {
            LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.False(dictionary.ContainsValue(default(TValue)!));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_ContainsValue_DefaultValuePresent(int count)
        {
            LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int seed = 4315;
            TKey notPresent = CreateTKey(seed++);
            while (dictionary.ContainsKey(notPresent))
                notPresent = CreateTKey(seed++);
            dictionary.Add(notPresent, default(TValue));
            Assert.True(dictionary.ContainsValue(default(TValue)!));
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
        public void LinkedDictionary_Generic_RemoveKey_ValidKeyNotContainedInDictionary(int count)
        {
            LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TValue? value;
            TKey missingKey = GetNewKey(dictionary);

            Assert.False(dictionary.Remove(missingKey, out value));
            Assert.Equal(count, dictionary.Count);
            Assert.Equal(default(TValue), value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_RemoveKey_ValidKeyContainedInDictionary(int count)
        {
            LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey missingKey = GetNewKey(dictionary);
            TValue? outValue;
            TValue inValue = CreateTValue(count);

            dictionary.Add(missingKey, inValue);
            Assert.True(dictionary.Remove(missingKey, out outValue));
            Assert.Equal(count, dictionary.Count);
            Assert.Equal(inValue, outValue);
            Assert.False(dictionary.TryGetValue(missingKey, out outValue));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_RemoveKey_DefaultKeyNotContainedInDictionary(int count)
        {
            LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TValue? outValue;

            if (DefaultValueAllowed)
            {
                TKey? missingKey = default(TKey);
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
        public void LinkedDictionary_Generic_RemoveKey_DefaultKeyContainedInDictionary(int count)
        {
            if (DefaultValueAllowed)
            {
                LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)(GenericIDictionaryFactory(count));
                TKey? missingKey = default(TKey);
                TValue? value;

                dictionary.TryAdd(missingKey, default(TValue));
                Assert.True(dictionary.Remove(missingKey, out value));
            }
        }

        [Fact]
        public void LinkedDictionary_Generic_Remove_RemoveFirstEnumerationContinues()
        {
            if (ModifyEnumeratorAllowed.HasFlag(ModifyOperation.Remove))
            {
                LinkedDictionary<TKey, TValue> dict = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(3);
                using (var enumerator = dict.GetEnumerator())
                {
                    enumerator.MoveNext();
                    TKey key = enumerator.Current.Key;
                    enumerator.MoveNext();
                    dict.Remove(key);
                    Assert.True(enumerator.MoveNext());
                    Assert.False(enumerator.MoveNext());
                }
            }
        }

        [Fact]
        public void LinkedDictionary_Generic_Remove_RemoveCurrentEnumerationContinues()
        {
            if (ModifyEnumeratorAllowed.HasFlag(ModifyOperation.Remove))
            {
                LinkedDictionary<TKey, TValue> dict = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(3);
                using (var enumerator = dict.GetEnumerator())
                {
                    enumerator.MoveNext();
                    enumerator.MoveNext();
                    dict.Remove(enumerator.Current.Key);
                    Assert.True(enumerator.MoveNext());
                    Assert.False(enumerator.MoveNext());
                }
            }
        }

        [Fact]
        public void LinkedDictionary_Generic_Remove_RemoveLastEnumerationFinishes()
        {
            if (ModifyEnumeratorAllowed.HasFlag(ModifyOperation.Remove))
            {
                LinkedDictionary<TKey, TValue> dict = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(3);
                TKey? key = default;
                using (var enumerator = dict.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        key = enumerator.Current.Key;
                    }
                }
                using (var enumerator = dict.GetEnumerator())
                {
                    enumerator.MoveNext();
                    enumerator.MoveNext();
                    dict.Remove(key);
                    Assert.False(enumerator.MoveNext());
                }
            }
        }

        #endregion

        #region EnsureCapacity

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void EnsureCapacity_Generic_RequestingLargerCapacity_DoesInvalidateEnumeration(int count)
        {
            var dictionary = (LinkedDictionary<TKey, TValue>)(GenericIDictionaryFactory(count));
            var capacity = dictionary.EnsureCapacity(0);
            var enumerator = dictionary.GetEnumerator();

            dictionary.EnsureCapacity(capacity + 1); // Verify EnsureCapacity does invalidate enumeration

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        public void EnsureCapacity_Generic_NegativeCapacityRequested_Throws()
        {
            var dictionary = new LinkedDictionary<TKey, TValue>();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => dictionary.EnsureCapacity(-1));
        }

        [Fact]
        public void EnsureCapacity_Generic_DictionaryNotInitialized_RequestedZero_ReturnsZero()
        {
            var dictionary = new LinkedDictionary<TKey, TValue>();
            Assert.Equal(0, dictionary.EnsureCapacity(0));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void EnsureCapacity_Generic_DictionaryNotInitialized_RequestedNonZero_CapacityIsSetToAtLeastTheRequested(int requestedCapacity)
        {
            var dictionary = new LinkedDictionary<TKey, TValue>();
            Assert.InRange(dictionary.EnsureCapacity(requestedCapacity), requestedCapacity, int.MaxValue);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(7)]
        public void EnsureCapacity_Generic_RequestedCapacitySmallerThanCurrent_CapacityUnchanged(int currentCapacity)
        {
            LinkedDictionary<TKey, TValue> dictionary;

            // assert capacity remains the same when ensuring a capacity smaller or equal than existing
            for (int i = 0; i <= currentCapacity; i++)
            {
                dictionary = new LinkedDictionary<TKey, TValue>(currentCapacity);
                Assert.Equal(currentCapacity, dictionary.EnsureCapacity(i));
            }
        }

        [Theory]
        [InlineData(7)]
        public void EnsureCapacity_Generic_ExistingCapacityRequested_SameValueReturned(int capacity)
        {
            var dictionary = new LinkedDictionary<TKey, TValue>(capacity);
            Assert.Equal(capacity, dictionary.EnsureCapacity(capacity));

            dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(capacity);
            Assert.Equal(capacity, dictionary.EnsureCapacity(capacity));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void EnsureCapacity_Generic_EnsureCapacityCalledTwice_ReturnsSameValue(int count)
        {
            var dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            int capacity = dictionary.EnsureCapacity(0);
            Assert.Equal(capacity, dictionary.EnsureCapacity(0));

            dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            capacity = dictionary.EnsureCapacity(count);
            Assert.Equal(capacity, dictionary.EnsureCapacity(count));

            dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            capacity = dictionary.EnsureCapacity(count + 1);
            Assert.Equal(capacity, dictionary.EnsureCapacity(count + 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(7)]
        public void EnsureCapacity_Generic_DictionaryNotEmpty_RequestedSmallerThanCount_ReturnsAtLeastSizeOfCount(int count)
        {
            var dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.InRange(dictionary.EnsureCapacity(count - 1), count, int.MaxValue);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(20)]
        public void EnsureCapacity_Generic_DictionaryNotEmpty_SetsToAtLeastTheRequested(int count)
        {
            var dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);

            // get current capacity
            int currentCapacity = dictionary.EnsureCapacity(0);

            // assert we can update to a larger capacity
            int newCapacity = dictionary.EnsureCapacity(currentCapacity * 2);
            Assert.InRange(newCapacity, currentCapacity * 2, int.MaxValue);
        }

        [Fact]
        public void EnsureCapacity_Generic_CapacityIsSetToPrimeNumberLargerOrEqualToRequested()
        {
            var dictionary = new LinkedDictionary<TKey, TValue>();
            Assert.Equal(17, dictionary.EnsureCapacity(17));

            dictionary = new LinkedDictionary<TKey, TValue>();
            Assert.Equal(17, dictionary.EnsureCapacity(15));

            dictionary = new LinkedDictionary<TKey, TValue>();
            Assert.Equal(17, dictionary.EnsureCapacity(13));
        }

        #endregion

        #region TrimExcess

        [Fact]
        public void TrimExcess_Generic_NegativeCapacity_Throw()
        {
            var dictionary = new LinkedDictionary<TKey, TValue>();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => dictionary.TrimExcess(-1));
        }

        [Theory]
        [InlineData(20)]
        [InlineData(23)]
        public void TrimExcess_Generic_CapacitySmallerThanCount_Throws(int suggestedCapacity)
        {
            var dictionary = new LinkedDictionary<TKey, TValue>();
            dictionary.Add(GetNewKey(dictionary), CreateTValue(0));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => dictionary.TrimExcess(0));

            dictionary = new LinkedDictionary<TKey, TValue>(suggestedCapacity);
            dictionary.Add(GetNewKey(dictionary), CreateTValue(0));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => dictionary.TrimExcess(0));
        }

        [Fact]
        public void TrimExcess_Generic_LargeInitialCapacity_TrimReducesSize()
        {
            var dictionary = new LinkedDictionary<TKey, TValue>(20);
            dictionary.TrimExcess(7);
            Assert.Equal(7, dictionary.EnsureCapacity(0));
        }

        [Theory]
        [InlineData(20)]
        [InlineData(23)]
        public void TrimExcess_Generic_TrimToLargerThanExistingCapacity_DoesNothing(int suggestedCapacity)
        {
            var dictionary = new LinkedDictionary<TKey, TValue>();
            int capacity = dictionary.EnsureCapacity(0);
            dictionary.TrimExcess(suggestedCapacity);
            Assert.Equal(capacity, dictionary.EnsureCapacity(0));

            dictionary = new LinkedDictionary<TKey, TValue>(suggestedCapacity / 2);
            capacity = dictionary.EnsureCapacity(0);
            dictionary.TrimExcess(suggestedCapacity);
            Assert.Equal(capacity, dictionary.EnsureCapacity(0));
        }

        [Fact]
        public void TrimExcess_Generic_DictionaryNotInitialized_CapacityRemainsAsMinPossible()
        {
            var dictionary = new LinkedDictionary<TKey, TValue>();
            Assert.Equal(0, dictionary.EnsureCapacity(0));
            dictionary.TrimExcess();
            Assert.Equal(0, dictionary.EnsureCapacity(0));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(85)]
        [InlineData(89)]
        public void TrimExcess_Generic_ClearThenTrimNonEmptyDictionary_SetsCapacityTo3(int count)
        {
            LinkedDictionary<TKey, TValue> dictionary = (LinkedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.Equal(count, dictionary.Count);
            // The smallest possible capacity size after clearing a dictionary is 3
            dictionary.Clear();
            dictionary.TrimExcess();
            Assert.Equal(3, dictionary.EnsureCapacity(0));
        }

        [Theory]
        [InlineData(85)]
        [InlineData(89)]
        public void TrimExcess_NoArguments_TrimsToAtLeastCount(int count)
        {
            var dictionary = new LinkedDictionary<int, int>(20);
            for (int i = 0; i < count; i++)
            {
                dictionary.Add(i, 0);
            }
            dictionary.TrimExcess();
            Assert.InRange(dictionary.EnsureCapacity(0), count, int.MaxValue);
        }

        [Theory]
        [InlineData(85)]
        [InlineData(89)]
        public void TrimExcess_WithArguments_OnDictionaryWithManyElementsRemoved_TrimsToAtLeastRequested(int finalCount)
        {
            const int InitToFinalRatio = 10;
            int initialCount = InitToFinalRatio * finalCount;
            var dictionary = new LinkedDictionary<int, int>(initialCount);
            Assert.InRange(dictionary.EnsureCapacity(0), initialCount, int.MaxValue);
            for (int i = 0; i < initialCount; i++)
            {
                dictionary.Add(i, 0);
            }
            for (int i = 0; i < initialCount - finalCount; i++)
            {
                dictionary.Remove(i);
            }
            for (int i = InitToFinalRatio; i > 0; i--)
            {
                dictionary.TrimExcess(i * finalCount);
                Assert.InRange(dictionary.EnsureCapacity(0), i * finalCount, int.MaxValue);
            }
        }

        [Theory]
        [InlineData(1000, 900, 5000, 85, 89)]
        [InlineData(1000, 400, 5000, 85, 89)]
        [InlineData(1000, 900, 500, 85, 89)]
        [InlineData(1000, 400, 500, 85, 89)]
        [InlineData(1000, 400, 500, 1, 3)]
        public void TrimExcess_NoArgument_TrimAfterEachBulkAddOrRemove_TrimsToAtLeastCount(int initialCount, int numRemove, int numAdd, int newCount, int newCapacity)
        {
            Random random = new Random(32);
            var dictionary = new LinkedDictionary<int, int>();
            dictionary.TrimExcess();
            Assert.InRange(dictionary.EnsureCapacity(0), dictionary.Count, int.MaxValue);

            var initialKeys = new int[initialCount];
            for (int i = 0; i < initialCount; i++)
            {
                initialKeys[i] = i;
            }
            random.Shuffle(initialKeys);
            foreach (var key in initialKeys)
            {
                dictionary.Add(key, 0);
            }
            dictionary.TrimExcess();
            Assert.InRange(dictionary.EnsureCapacity(0), dictionary.Count, int.MaxValue);

            random.Shuffle(initialKeys);
            for (int i = 0; i < numRemove; i++)
            {
                dictionary.Remove(initialKeys[i]);
            }
            dictionary.TrimExcess();
            Assert.InRange(dictionary.EnsureCapacity(0), dictionary.Count, int.MaxValue);

            var moreKeys = new int[numAdd];
            for (int i = 0; i < numAdd; i++)
            {
                moreKeys[i] = i + initialCount;
            }
            random.Shuffle(moreKeys);
            foreach (var key in moreKeys)
            {
                dictionary.Add(key, 0);
            }
            int currentCount = dictionary.Count;
            dictionary.TrimExcess();
            Assert.InRange(dictionary.EnsureCapacity(0), currentCount, int.MaxValue);

            int[] existingKeys = new int[currentCount];
            Array.Copy(initialKeys, numRemove, existingKeys, 0, initialCount - numRemove);
            Array.Copy(moreKeys, 0, existingKeys, initialCount - numRemove, numAdd);
            random.Shuffle(existingKeys);
            for (int i = 0; i < currentCount - newCount; i++)
            {
                dictionary.Remove(existingKeys[i]);
            }
            dictionary.TrimExcess();
            int finalCapacity = dictionary.EnsureCapacity(0);
            Assert.InRange(finalCapacity, newCount, initialCount);
            Assert.Equal(newCapacity, finalCapacity);
        }

        [Fact]
        public void TrimExcess_DictionaryHasElementsChainedWithSameHashcode_Success()
        {
            var dictionary = new LinkedDictionary<string, int>(7);
            for (int i = 0; i < 4; i++)
            {
                dictionary.Add(i.ToString(), 0);
            }
            var s_64bit = new string[] { "95e85f8e-67a3-4367-974f-dd24d8bb2ca2", "eb3d6fe9-de64-43a9-8f58-bddea727b1ca" };
            var s_32bit = new string[] { "25b1f130-7517-48e3-96b0-9da44e8bfe0e", "ba5a3625-bc38-4bf1-a707-a3cfe2158bae" };
            string[] chained = (Environment.Is64BitProcess ? s_64bit : s_32bit).ToArray();
            dictionary.Add(chained[0], 0);
            dictionary.Add(chained[1], 0);
            for (int i = 0; i < 4; i++)
            {
                dictionary.Remove(i.ToString());
            }
            dictionary.TrimExcess(3);
            Assert.Equal(2, dictionary.Count);
            int val;
            Assert.True(dictionary.TryGetValue(chained[0], out val));
            Assert.True(dictionary.TryGetValue(chained[1], out val));
        }

        [Fact]
        public void TrimExcess_Generic_DoesInvalidateEnumeration()
        {
            var dictionary = new LinkedDictionary<TKey, TValue>(20);
            var enumerator = dictionary.GetEnumerator();

            dictionary.TrimExcess(7); // Verify TrimExcess does invalidate enumeration

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        #endregion
    }
}
