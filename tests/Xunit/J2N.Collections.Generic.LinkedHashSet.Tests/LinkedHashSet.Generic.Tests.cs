﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the LinkedHashSet class.
    /// </summary>
    public abstract class LinkedHashSet_Generic_Tests<T> : ISet_Generic_Tests<T>
    {
        #region ISet<T> Helper Methods

        protected override bool ResetImplemented => true;

        protected override SCG.ISet<T> GenericISetFactory()
        {
            return new LinkedHashSet<T>();
        }

        #endregion

        #region Constructors

        private static SCG.IEnumerable<int> NonSquares(int limit)
        {
            for (int i = 0; i != limit; ++i)
            {
                int root = (int)Math.Sqrt(i);
                if (i != root * root)
                    yield return i;
            }
        }

        [Fact]
        public void LinkedHashSet_Generic_Constructor()
        {
            LinkedHashSet<T> set = new LinkedHashSet<T>();
            Assert.Empty(set);
        }

        [Fact]
        public void LinkedHashSet_Generic_Constructor_IEqualityComparer()
        {
            SCG.IEqualityComparer<T> comparer = GetIEqualityComparer();
            LinkedHashSet<T> set = new LinkedHashSet<T>(comparer);
            if (comparer == null)
                Assert.Equal(EqualityComparer<T>.Default, set.EqualityComparer);
            else
                Assert.Equal(comparer, set.EqualityComparer);
        }

        [Fact]
        public void LinkedHashSet_Generic_Constructor_NullIEqualityComparer()
        {
            SCG.IEqualityComparer<T> comparer = null;
            LinkedHashSet<T> set = new LinkedHashSet<T>(comparer);
            if (comparer == null)
                Assert.Equal(EqualityComparer<T>.Default, set.EqualityComparer);
            else
                Assert.Equal(comparer, set.EqualityComparer);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void LinkedHashSet_Generic_Constructor_IEnumerable(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            _ = setLength;
            _ = numberOfMatchingElements;
            SCG.IEnumerable<T> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
            LinkedHashSet<T> set = new LinkedHashSet<T>(enumerable);
            Assert.True(set.SetEquals(enumerable));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_Constructor_IEnumerable_WithManyDuplicates(int count)
        {
            SCG.IEnumerable<T> items = CreateEnumerable(EnumerableType.List, null, count, 0, 0);
            LinkedHashSet<T> hashSetFromDuplicates = new LinkedHashSet<T>(Enumerable.Range(0, 40).SelectMany(i => items).ToArray());
            LinkedHashSet<T> hashSetFromNoDuplicates = new LinkedHashSet<T>(items);
            Assert.True(hashSetFromNoDuplicates.SetEquals(hashSetFromDuplicates));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_Constructor_LinkedHashSet_SparselyFilled(int count)
        {
            LinkedHashSet<T> source = (LinkedHashSet<T>)CreateEnumerable(EnumerableType.LinkedHashSet, null, count, 0, 0);
            List<T> sourceElements = source.ToList();
            foreach (int i in NonSquares(count))
                source.Remove(sourceElements[i]);// Unevenly spaced survivors increases chance of catching any spacing-related bugs.


            LinkedHashSet<T> set = new LinkedHashSet<T>(source, GetIEqualityComparer());
            Assert.True(set.SetEquals(source));
        }

        [Fact]
        public void LinkedHashSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new LinkedHashSet<T>((SCG.IEnumerable<T>)null));
            Assert.Throws<ArgumentNullException>(() => new LinkedHashSet<T>((SCG.IEnumerable<T>)null, EqualityComparer<T>.Default));
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void LinkedHashSet_Generic_Constructor_IEnumerable_IEqualityComparer(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            _ = setLength;
            _ = numberOfMatchingElements;
            _ = numberOfDuplicateElements;
            SCG.IEnumerable<T> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
            LinkedHashSet<T> set = new LinkedHashSet<T>(enumerable, GetIEqualityComparer());
            Assert.True(set.SetEquals(enumerable));
        }

        #endregion

        #region RemoveWhere

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_RemoveWhere_AllElements(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            int removedCount = set.RemoveWhere((value) => { return true; });
            Assert.Equal(setLength, removedCount);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_RemoveWhere_NoElements(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            int removedCount = set.RemoveWhere((value) => { return false; });
            Assert.Equal(0, removedCount);
            Assert.Equal(setLength, set.Count);
        }

        [Fact]
        public void LinkedHashSet_Generic_RemoveWhere_NewObject() // Regression Dev10_624201
        {
            object[] array = new object[2];
            object obj = new object();
            LinkedHashSet<object> set = new LinkedHashSet<object>();

            set.Add(obj);
            set.Remove(obj);
            foreach (object o in set) { }
            set.CopyTo(array, 0, 2);
            set.RemoveWhere((element) => { return false; });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_RemoveWhere_NullMatchPredicate(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

        #endregion

        #region TrimExcess

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_TrimExcess_OnValidSetThatHasntBeenRemovedFrom(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            set.TrimExcess();
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_TrimExcess_Repeatedly(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            List<T> expected = set.ToList();
            set.TrimExcess();
            set.TrimExcess();
            set.TrimExcess();
            Assert.True(set.SetEquals(expected));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_TrimExcess_AfterRemovingOneElement(int setLength)
        {
            if (setLength > 0)
            {
                LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
                List<T> expected = set.ToList();
                T elementToRemove = set.ElementAt(0);

                set.TrimExcess();
                Assert.True(set.Remove(elementToRemove));
                expected.Remove(elementToRemove);
                set.TrimExcess();

                Assert.True(set.SetEquals(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_TrimExcess_AfterClearingAndAddingSomeElementsBack(int setLength)
        {
            if (setLength > 0)
            {
                LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
                set.TrimExcess();
                set.Clear();
                set.TrimExcess();
                Assert.Equal(0, set.Count);

                AddToCollection(set, setLength / 10);
                set.TrimExcess();
                Assert.Equal(setLength / 10, set.Count);
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_TrimExcess_AfterClearingAndAddingAllElementsBack(int setLength)
        {
            if (setLength > 0)
            {
                LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
                set.TrimExcess();
                set.Clear();
                set.TrimExcess();
                Assert.Equal(0, set.Count);

                AddToCollection(set, setLength);
                set.TrimExcess();
                Assert.Equal(setLength, set.Count);
            }
        }

        #endregion

        #region CopyTo

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException(int count)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(count);
            T[] arr = new T[count];
            Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(arr, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(arr, 0, int.MinValue));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_CopyTo_NoIndexDefaultsToZero(int count)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(count);
            T[] arr1 = new T[count];
            T[] arr2 = new T[count];
            set.CopyTo(arr1);
            set.CopyTo(arr2, 0);
            Assert.True(arr1.SequenceEqual(arr2));
        }

        #endregion

        #region CreateSetComparer

        [Fact]
        public void SetComparer_SetEqualsTests()
        {
            SCG.List<T> objects = new SCG.List<T>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new LinkedHashSet<LinkedHashSet<T>>()
            {
                new LinkedHashSet<T> { objects[0], objects[1], objects[2] },
                new LinkedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new LinkedHashSet<LinkedHashSet<T>>()
            {
                new LinkedHashSet<T> { objects[0], objects[1], objects[2] },
                new LinkedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new LinkedHashSet<LinkedHashSet<T>>(SetEqualityComparer<T>.Default)
            {
                new LinkedHashSet<T> { objects[0], objects[1], objects[2] },
                new LinkedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new LinkedHashSet<LinkedHashSet<T>>(SetEqualityComparer<T>.Default)
            {
                new LinkedHashSet<T> { objects[3], objects[4], objects[5] },
                new LinkedHashSet<T> { objects[0], objects[1], objects[2] }
            };

            Assert.True(noComparerSet.SetEquals(set)); // Our implementation is structurally equatable by default
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }

        [Fact]
        public void SetComparer_SequenceEqualTests()
        {
            SCG.List<T> objects = new SCG.List<T>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new LinkedHashSet<LinkedHashSet<T>>()
            {
                new LinkedHashSet<T> { objects[0], objects[1], objects[2] },
                new LinkedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new LinkedHashSet<LinkedHashSet<T>>()
            {
                new LinkedHashSet<T> { objects[0], objects[1], objects[2] },
                new LinkedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var comparerSet = new LinkedHashSet<LinkedHashSet<T>>(SetEqualityComparer<T>.Default)
            {
                new LinkedHashSet<T> { objects[0], objects[1], objects[2] },
                new LinkedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            Assert.True(noComparerSet.SequenceEqual(set)); // Unlike the .NET LinkedHashSet, ours is structurally equatable by default
            Assert.True(noComparerSet.SequenceEqual(set, SetEqualityComparer<T>.Default));
            Assert.True(comparerSet.SequenceEqual(set)); // Unlike the .NET LinkedHashSet, ours is structurally equatable by default
        }

        #endregion

        [Fact]
        public void CanBeCastedToISet()
        {
            LinkedHashSet<T> set = new LinkedHashSet<T>();
            SCG.ISet<T> iset = (set as SCG.ISet<T>);
            Assert.NotNull(iset);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_Constructor_int(int capacity)
        {
            LinkedHashSet<T> set = new LinkedHashSet<T>(capacity);
            Assert.Equal(0, set.Count);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_Constructor_int_AddUpToAndBeyondCapacity(int capacity)
        {
            LinkedHashSet<T> set = new LinkedHashSet<T>(capacity);

            AddToCollection(set, capacity);
            Assert.Equal(capacity, set.Count);

            AddToCollection(set, capacity + 1);
            Assert.Equal(capacity + 1, set.Count);
        }

        [Fact]
        public void LinkedHashSet_Generic_Constructor_Capacity_ToNextPrimeNumber()
        {
            // Highest pre-computed number + 1.
            const int Capacity = 7199370;
            var set = new LinkedHashSet<T>(Capacity);

            // Assert that the HashTable's capacity is set to the descendant prime number of the given one.
            const int NextPrime = 7199371;
            Assert.Equal(NextPrime, set.EnsureCapacity(0));
        }

        [Fact]
        public void LinkedHashSet_Generic_Constructor_int_Negative_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LinkedHashSet<T>(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LinkedHashSet<T>(int.MinValue));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_Constructor_int_IEqualityComparer(int capacity)
        {
            SCG.IEqualityComparer<T> comparer = GetIEqualityComparer();
            LinkedHashSet<T> set = new LinkedHashSet<T>(capacity, comparer);
            Assert.Equal(0, set.Count);
            if (comparer == null)
                Assert.Equal(EqualityComparer<T>.Default, set.EqualityComparer);
            else
                Assert.Equal(comparer, set.EqualityComparer);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedHashSet_Generic_Constructor_int_IEqualityComparer_AddUpToAndBeyondCapacity(int capacity)
        {
            SCG.IEqualityComparer<T> comparer = GetIEqualityComparer();
            LinkedHashSet<T> set = new LinkedHashSet<T>(capacity, comparer);

            AddToCollection(set, capacity);
            Assert.Equal(capacity, set.Count);

            AddToCollection(set, capacity + 1);
            Assert.Equal(capacity + 1, set.Count);
        }

        [Fact]
        public void LinkedHashSet_Generic_Constructor_int_IEqualityComparer_Negative_ThrowsArgumentOutOfRangeException()
        {
            SCG.IEqualityComparer<T> comparer = GetIEqualityComparer();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LinkedHashSet<T>(-1, comparer));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new LinkedHashSet<T>(int.MinValue, comparer));
        }

        #region TryGetValue

        [Fact]
        public void LinkedHashSet_Generic_TryGetValue_Contains()
        {
            T value = CreateT(1);
            LinkedHashSet<T> set = new LinkedHashSet<T> { value };
            T equalValue = CreateT(1);
            T actualValue;
            Assert.True(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(value, actualValue);
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                Assert.Same((object)value, (object)actualValue);
            }
        }

        [Fact]
        public void LinkedHashSet_Generic_TryGetValue_Contains_OverwriteOutputParam()
        {
            T value = CreateT(1);
            LinkedHashSet<T> set = new LinkedHashSet<T> { value };
            T equalValue = CreateT(1);
            T actualValue = CreateT(2);
            Assert.True(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(value, actualValue);
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                Assert.Same((object)value, (object)actualValue);
            }
        }

        [Fact]
        public void LinkedHashSet_Generic_TryGetValue_NotContains()
        {
            T value = CreateT(1);
            LinkedHashSet<T> set = new LinkedHashSet<T> { value };
            T equalValue = CreateT(2);
            T actualValue;
            Assert.False(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(default(T), actualValue);
        }

        [Fact]
        public void LinkedHashSet_Generic_TryGetValue_NotContains_OverwriteOutputParam()
        {
            T value = CreateT(1);
            LinkedHashSet<T> set = new LinkedHashSet<T> { value };
            T equalValue = CreateT(2);
            T actualValue = equalValue;
            Assert.False(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(default(T), actualValue);
        }

        #endregion

        #region EnsureCapacity

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void EnsureCapacity_Generic_RequestingLargerCapacity_DoesNotInvalidateEnumeration(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)(GenericISetFactory(setLength));
            var capacity = set.EnsureCapacity(0);
            IEnumerator valuesEnum = set.GetEnumerator();
            IEnumerator valuesListEnum = new SCG.List<T>(set).GetEnumerator();

            set.EnsureCapacity(capacity + 1); // Verify EnsureCapacity does not invalidate enumeration

            while (valuesEnum.MoveNext())
            {
                valuesListEnum.MoveNext();
                Assert.Equal(valuesListEnum.Current, valuesEnum.Current);
            }
        }

        [Fact]
        public void EnsureCapacity_Generic_NegativeCapacityRequested_Throws()
        {
            var set = new LinkedHashSet<T>();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => set.EnsureCapacity(-1));
        }

        [Fact]
        public void EnsureCapacity_Generic_HashsetNotInitialized_RequestedZero_ReturnsZero()
        {
            var set = new LinkedHashSet<T>();
            Assert.Equal(0, set.EnsureCapacity(0));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void EnsureCapacity_Generic_HashsetNotInitialized_RequestedNonZero_CapacityIsSetToAtLeastTheRequested(int requestedCapacity)
        {
            var set = new LinkedHashSet<T>();
            Assert.InRange(set.EnsureCapacity(requestedCapacity), requestedCapacity, int.MaxValue);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(7)]
        public void EnsureCapacity_Generic_RequestedCapacitySmallerThanCurrent_CapacityUnchanged(int currentCapacity)
        {
            LinkedHashSet<T> set;

            // assert capacity remains the same when ensuring a capacity smaller or equal than existing
            for (int i = 0; i <= currentCapacity; i++)
            {
                set = new LinkedHashSet<T>(currentCapacity);
                Assert.Equal(currentCapacity, set.EnsureCapacity(i));
            }
        }

        [Theory]
        [InlineData(7)]
        [InlineData(89)]
        public void EnsureCapacity_Generic_ExistingCapacityRequested_SameValueReturned(int capacity)
        {
            var set = new LinkedHashSet<T>(capacity);
            Assert.Equal(capacity, set.EnsureCapacity(capacity));

            set = (LinkedHashSet<T>)GenericISetFactory(capacity);
            Assert.Equal(capacity, set.EnsureCapacity(capacity));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void EnsureCapacity_Generic_EnsureCapacityCalledTwice_ReturnsSameValue(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            int capacity = set.EnsureCapacity(0);
            Assert.Equal(capacity, set.EnsureCapacity(0));

            set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            capacity = set.EnsureCapacity(setLength);
            Assert.Equal(capacity, set.EnsureCapacity(setLength));

            set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            capacity = set.EnsureCapacity(setLength + 1);
            Assert.Equal(capacity, set.EnsureCapacity(setLength + 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(8)]
        public void EnsureCapacity_Generic_HashsetNotEmpty_RequestedSmallerThanCount_ReturnsAtLeastSizeOfCount(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);
            Assert.InRange(set.EnsureCapacity(setLength - 1), setLength, int.MaxValue);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(20)]
        public void EnsureCapacity_Generic_HashsetNotEmpty_SetsToAtLeastTheRequested(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);

            // get current capacity
            int currentCapacity = set.EnsureCapacity(0);

            // assert we can update to a larger capacity
            int newCapacity = set.EnsureCapacity(currentCapacity * 2);
            Assert.InRange(newCapacity, currentCapacity * 2, int.MaxValue);
        }

        [Fact]
        public void EnsureCapacity_Generic_CapacityIsSetToPrimeNumberLargerOrEqualToRequested()
        {
            var set = new LinkedHashSet<T>();
            Assert.Equal(17, set.EnsureCapacity(17));

            set = new LinkedHashSet<T>();
            Assert.Equal(17, set.EnsureCapacity(15));

            set = new LinkedHashSet<T>();
            Assert.Equal(17, set.EnsureCapacity(13));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(10)]
        public void EnsureCapacity_Generic_GrowCapacityWithFreeList(int setLength)
        {
            LinkedHashSet<T> set = (LinkedHashSet<T>)GenericISetFactory(setLength);

            // Remove the first element to ensure we have a free list.
            Assert.True(set.Remove(set.ElementAt(0)));

            int currentCapacity = set.EnsureCapacity(0);
            Assert.True(currentCapacity > 0);

            int newCapacity = set.EnsureCapacity(currentCapacity + 1);
            Assert.True(newCapacity > currentCapacity);
        }

        #endregion

        #region Remove

        [Theory]
        [MemberData(nameof(ValidPositiveCollectionSizes))]
        public void Remove_NonDefaultComparer_ComparerUsed(int capacity)
        {
            var c = new TrackingEqualityComparer<T>();
            var set = new LinkedHashSet<T>(capacity, c);

            AddToCollection(set, capacity);
            T first = set.First();
            c.EqualsCalls = 0;
            c.GetHashCodeCalls = 0;

            Assert.Equal(capacity, set.Count);
            set.Remove(first);
            Assert.Equal(capacity - 1, set.Count);

            Assert.InRange(c.EqualsCalls, 1, int.MaxValue);
            Assert.InRange(c.GetHashCodeCalls, 1, int.MaxValue);
        }

        #endregion
    }
}
