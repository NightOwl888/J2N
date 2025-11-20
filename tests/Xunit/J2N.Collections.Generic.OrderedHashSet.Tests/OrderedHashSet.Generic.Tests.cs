// Licensed to the .NET Foundation under one or more agreements.
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
    /// Contains tests that ensure the correctness of the OrderedHashSet class.
    /// </summary>
    public abstract class OrderedHashSet_Generic_Tests<T> : ISet_Generic_Tests<T>
    {
        #region ISet<T> Helper Methods
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;

        protected override bool ResetImplemented => true;

        // J2N TODO: Make OrderedHashSet allow Remove/Clear while iterating forward, like HashSet<T> and Dictionary<TKey, TValue>
        protected override ModifyOperation ModifyEnumeratorThrows => base.ModifyEnumeratorAllowed /*& ~(ModifyOperation.Remove | ModifyOperation.Clear)*/;

        protected override ModifyOperation ModifyEnumeratorAllowed => ModifyOperation.Overwrite /*| ModifyOperation.Remove | ModifyOperation.Clear*/;

        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;

        protected override SCG.ISet<T> GenericISetFactory()
        {
            return new OrderedHashSet<T>();
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
        public void OrderedHashSet_Generic_Constructor()
        {
            OrderedHashSet<T> set = new OrderedHashSet<T>();
            Assert.Empty(set);
        }

        [Fact]
        public void OrderedHashSet_Generic_Constructor_IEqualityComparer()
        {
            SCG.IEqualityComparer<T> comparer = GetIEqualityComparer();
            OrderedHashSet<T> set = new OrderedHashSet<T>(comparer);
            if (comparer == null)
                Assert.Equal(EqualityComparer<T>.Default, set.EqualityComparer);
            else
                Assert.Equal(comparer, set.EqualityComparer);
        }

        [Fact]
        public void OrderedHashSet_Generic_Constructor_NullIEqualityComparer()
        {
            SCG.IEqualityComparer<T> comparer = null;
            OrderedHashSet<T> set = new OrderedHashSet<T>(comparer);
            if (comparer == null)
                Assert.Equal(EqualityComparer<T>.Default, set.EqualityComparer);
            else
                Assert.Equal(comparer, set.EqualityComparer);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void OrderedHashSet_Generic_Constructor_IEnumerable(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            _ = setLength;
            _ = numberOfMatchingElements;
            SCG.IEnumerable<T> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
            OrderedHashSet<T> set = new OrderedHashSet<T>(enumerable);
            Assert.True(set.SetEquals(enumerable));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_Constructor_IEnumerable_WithManyDuplicates(int count)
        {
            SCG.IEnumerable<T> items = CreateEnumerable(EnumerableType.List, null, count, 0, 0);
            OrderedHashSet<T> hashSetFromDuplicates = new OrderedHashSet<T>(Enumerable.Range(0, 40).SelectMany(i => items).ToArray());
            OrderedHashSet<T> hashSetFromNoDuplicates = new OrderedHashSet<T>(items);
            Assert.True(hashSetFromNoDuplicates.SetEquals(hashSetFromDuplicates));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_Constructor_OrderedHashSet_SparselyFilled(int count)
        {
            OrderedHashSet<T> source = (OrderedHashSet<T>)CreateEnumerable(EnumerableType.OrderedHashSet, null, count, 0, 0);
            List<T> sourceElements = source.ToList();
            foreach (int i in NonSquares(count))
                source.Remove(sourceElements[i]);// Unevenly spaced survivors increases chance of catching any spacing-related bugs.


            OrderedHashSet<T> set = new OrderedHashSet<T>(source, GetIEqualityComparer());
            Assert.True(set.SetEquals(source));
        }

        [Fact]
        public void OrderedHashSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderedHashSet<T>((SCG.IEnumerable<T>)null));
            Assert.Throws<ArgumentNullException>(() => new OrderedHashSet<T>((SCG.IEnumerable<T>)null, EqualityComparer<T>.Default));
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void OrderedHashSet_Generic_Constructor_IEnumerable_IEqualityComparer(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            _ = setLength;
            _ = numberOfMatchingElements;
            _ = numberOfDuplicateElements;
            SCG.IEnumerable<T> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
            OrderedHashSet<T> set = new OrderedHashSet<T>(enumerable, GetIEqualityComparer());
            Assert.True(set.SetEquals(enumerable));
        }

        #endregion

        #region RemoveWhere

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_RemoveWhere_AllElements(int setLength)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
            int removedCount = set.RemoveWhere((value) => { return true; });
            Assert.Equal(setLength, removedCount);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_RemoveWhere_NoElements(int setLength)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
            int removedCount = set.RemoveWhere((value) => { return false; });
            Assert.Equal(0, removedCount);
            Assert.Equal(setLength, set.Count);
        }

        [Fact]
        public void OrderedHashSet_Generic_RemoveWhere_NewObject() // Regression Dev10_624201
        {
            object[] array = new object[2];
            object obj = new object();
            OrderedHashSet<object> set = new OrderedHashSet<object>();

            set.Add(obj);
            set.Remove(obj);
            foreach (object o in set) { }
            set.CopyTo(array, 0, 2);
            set.RemoveWhere((element) => { return false; });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_RemoveWhere_NullMatchPredicate(int setLength)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
            Assert.Throws<ArgumentNullException>(() => set.RemoveWhere(null));
        }

        #endregion

        #region TrimExcess

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_TrimExcess_OnValidSetThatHasntBeenRemovedFrom(int setLength)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
            set.TrimExcess();
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_TrimExcess_Repeatedly(int setLength)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
            List<T> expected = set.ToList();
            set.TrimExcess();
            set.TrimExcess();
            set.TrimExcess();
            Assert.True(set.SetEquals(expected));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_TrimExcess_AfterRemovingOneElement(int setLength)
        {
            if (setLength > 0)
            {
                OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
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
        public void OrderedHashSet_Generic_TrimExcess_AfterClearingAndAddingSomeElementsBack(int setLength)
        {
            if (setLength > 0)
            {
                OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
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
        public void OrderedHashSet_Generic_TrimExcess_AfterClearingAndAddingAllElementsBack(int setLength)
        {
            if (setLength > 0)
            {
                OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
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
        public void OrderedHashSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException(int count)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(count);
            T[] arr = new T[count];
            Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(arr, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(arr, 0, int.MinValue));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_CopyTo_NoIndexDefaultsToZero(int count)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(count);
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

            var set = new OrderedHashSet<OrderedHashSet<T>>()
            {
                new OrderedHashSet<T> { objects[0], objects[1], objects[2] },
                new OrderedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new OrderedHashSet<OrderedHashSet<T>>()
            {
                new OrderedHashSet<T> { objects[0], objects[1], objects[2] },
                new OrderedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new OrderedHashSet<OrderedHashSet<T>>(SetEqualityComparer<T>.Default)
            {
                new OrderedHashSet<T> { objects[0], objects[1], objects[2] },
                new OrderedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new OrderedHashSet<OrderedHashSet<T>>(SetEqualityComparer<T>.Default)
            {
                new OrderedHashSet<T> { objects[3], objects[4], objects[5] },
                new OrderedHashSet<T> { objects[0], objects[1], objects[2] }
            };

            Assert.True(noComparerSet.SetEquals(set)); // Our implementation is structurally equatable by default
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }

        [Fact]
        public void SetComparer_SequenceEqualTests()
        {
            SCG.List<T> objects = new SCG.List<T>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new OrderedHashSet<OrderedHashSet<T>>()
            {
                new OrderedHashSet<T> { objects[0], objects[1], objects[2] },
                new OrderedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new OrderedHashSet<OrderedHashSet<T>>()
            {
                new OrderedHashSet<T> { objects[0], objects[1], objects[2] },
                new OrderedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            var comparerSet = new OrderedHashSet<OrderedHashSet<T>>(SetEqualityComparer<T>.Default)
            {
                new OrderedHashSet<T> { objects[0], objects[1], objects[2] },
                new OrderedHashSet<T> { objects[3], objects[4], objects[5] }
            };

            Assert.True(noComparerSet.SequenceEqual(set)); // Unlike the .NET OrderedHashSet, ours is structurally equatable by default
            Assert.True(noComparerSet.SequenceEqual(set, SetEqualityComparer<T>.Default));
            Assert.True(comparerSet.SequenceEqual(set)); // Unlike the .NET OrderedHashSet, ours is structurally equatable by default
        }

        #endregion

        [Fact]
        public void CanBeCastedToISet()
        {
            OrderedHashSet<T> set = new OrderedHashSet<T>();
            SCG.ISet<T> iset = (set as SCG.ISet<T>);
            Assert.NotNull(iset);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_Constructor_int(int capacity)
        {
            OrderedHashSet<T> set = new OrderedHashSet<T>(capacity);
            Assert.Equal(0, set.Count);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_Constructor_int_AddUpToAndBeyondCapacity(int capacity)
        {
            OrderedHashSet<T> set = new OrderedHashSet<T>(capacity);

            AddToCollection(set, capacity);
            Assert.Equal(capacity, set.Count);

            AddToCollection(set, capacity + 1);
            Assert.Equal(capacity + 1, set.Count);
        }

        [Fact]
        public void OrderedHashSet_Generic_Constructor_Capacity_ToNextPrimeNumber()
        {
            // Highest pre-computed number + 1.
            const int Capacity = 7199370;
            var set = new OrderedHashSet<T>(Capacity);

            // Assert that the HashTable's capacity is set to the descendant prime number of the given one.
            const int NextPrime = 7199371;
            Assert.Equal(NextPrime, set.EnsureCapacity(0));
        }

        [Fact]
        public void OrderedHashSet_Generic_Constructor_int_Negative_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new OrderedHashSet<T>(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new OrderedHashSet<T>(int.MinValue));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_Constructor_int_IEqualityComparer(int capacity)
        {
            SCG.IEqualityComparer<T> comparer = GetIEqualityComparer();
            OrderedHashSet<T> set = new OrderedHashSet<T>(capacity, comparer);
            Assert.Equal(0, set.Count);
            if (comparer == null)
                Assert.Equal(EqualityComparer<T>.Default, set.EqualityComparer);
            else
                Assert.Equal(comparer, set.EqualityComparer);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedHashSet_Generic_Constructor_int_IEqualityComparer_AddUpToAndBeyondCapacity(int capacity)
        {
            SCG.IEqualityComparer<T> comparer = GetIEqualityComparer();
            OrderedHashSet<T> set = new OrderedHashSet<T>(capacity, comparer);

            AddToCollection(set, capacity);
            Assert.Equal(capacity, set.Count);

            AddToCollection(set, capacity + 1);
            Assert.Equal(capacity + 1, set.Count);
        }

        [Fact]
        public void OrderedHashSet_Generic_Constructor_int_IEqualityComparer_Negative_ThrowsArgumentOutOfRangeException()
        {
            SCG.IEqualityComparer<T> comparer = GetIEqualityComparer();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new OrderedHashSet<T>(-1, comparer));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new OrderedHashSet<T>(int.MinValue, comparer));
        }

        #region TryGetValue

        [Fact]
        public void OrderedHashSet_Generic_TryGetValue_Contains()
        {
            T value = CreateT(1);
            OrderedHashSet<T> set = new OrderedHashSet<T> { value };
            T equalValue = CreateT(1);
            T actualValue;
            Assert.True(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(value, actualValue);
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
#pragma warning disable xUnit2005 // Do not use identity check on value type
                Assert.Same((object)value, (object)actualValue);
#pragma warning restore xUnit2005 // Do not use identity check on value type
            }
        }

        [Fact]
        public void OrderedHashSet_Generic_TryGetValue_Contains_OverwriteOutputParam()
        {
            T value = CreateT(1);
            OrderedHashSet<T> set = new OrderedHashSet<T> { value };
            T equalValue = CreateT(1);
            T actualValue = CreateT(2);
            Assert.True(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(value, actualValue);
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
#pragma warning disable xUnit2005 // Do not use identity check on value type
                Assert.Same((object)value, (object)actualValue);
#pragma warning restore xUnit2005 // Do not use identity check on value type
            }
        }

        [Fact]
        public void OrderedHashSet_Generic_TryGetValue_NotContains()
        {
            T value = CreateT(1);
            OrderedHashSet<T> set = new OrderedHashSet<T> { value };
            T equalValue = CreateT(2);
            T actualValue;
            Assert.False(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(default(T), actualValue);
        }

        [Fact]
        public void OrderedHashSet_Generic_TryGetValue_NotContains_OverwriteOutputParam()
        {
            T value = CreateT(1);
            OrderedHashSet<T> set = new OrderedHashSet<T> { value };
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
            OrderedHashSet<T> set = (OrderedHashSet<T>)(GenericISetFactory(setLength));
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
            var set = new OrderedHashSet<T>();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => set.EnsureCapacity(-1));
        }

        [Fact]
        public void EnsureCapacity_Generic_HashsetNotInitialized_RequestedZero_ReturnsZero()
        {
            var set = new OrderedHashSet<T>();
            Assert.Equal(0, set.EnsureCapacity(0));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void EnsureCapacity_Generic_HashsetNotInitialized_RequestedNonZero_CapacityIsSetToAtLeastTheRequested(int requestedCapacity)
        {
            var set = new OrderedHashSet<T>();
            Assert.InRange(set.EnsureCapacity(requestedCapacity), requestedCapacity, int.MaxValue);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(7)]
        public void EnsureCapacity_Generic_RequestedCapacitySmallerThanCurrent_CapacityUnchanged(int currentCapacity)
        {
            OrderedHashSet<T> set;

            // assert capacity remains the same when ensuring a capacity smaller or equal than existing
            for (int i = 0; i <= currentCapacity; i++)
            {
                set = new OrderedHashSet<T>(currentCapacity);
                Assert.Equal(currentCapacity, set.EnsureCapacity(i));
            }
        }

        [Theory]
        [InlineData(7)]
        [InlineData(89)]
        public void EnsureCapacity_Generic_ExistingCapacityRequested_SameValueReturned(int capacity)
        {
            var set = new OrderedHashSet<T>(capacity);
            Assert.Equal(capacity, set.EnsureCapacity(capacity));

            set = (OrderedHashSet<T>)GenericISetFactory(capacity);
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
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
            int capacity = set.EnsureCapacity(0);
            Assert.Equal(capacity, set.EnsureCapacity(0));

            set = (OrderedHashSet<T>)GenericISetFactory(setLength);
            capacity = set.EnsureCapacity(setLength);
            Assert.Equal(capacity, set.EnsureCapacity(setLength));

            set = (OrderedHashSet<T>)GenericISetFactory(setLength);
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
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);
            Assert.InRange(set.EnsureCapacity(setLength - 1), setLength, int.MaxValue);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(20)]
        public void EnsureCapacity_Generic_HashsetNotEmpty_SetsToAtLeastTheRequested(int setLength)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);

            // get current capacity
            int currentCapacity = set.EnsureCapacity(0);

            // assert we can update to a larger capacity
            int newCapacity = set.EnsureCapacity(currentCapacity * 2);
            Assert.InRange(newCapacity, currentCapacity * 2, int.MaxValue);
        }

        [Fact]
        public void EnsureCapacity_Generic_CapacityIsSetToPrimeNumberLargerOrEqualToRequested()
        {
            var set = new OrderedHashSet<T>();
            Assert.Equal(17, set.EnsureCapacity(17));

            set = new OrderedHashSet<T>();
            Assert.Equal(17, set.EnsureCapacity(15));

            set = new OrderedHashSet<T>();
            Assert.Equal(17, set.EnsureCapacity(13));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(10)]
        public void EnsureCapacity_Generic_GrowCapacityWithFreeList(int setLength)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(setLength);

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
            var set = new OrderedHashSet<T>(capacity, c);

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

        #region GetAlternateLookup

#if FEATURE_IALTERNATEEQUALITYCOMPARER
        [Fact]
        public void GetAlternateLookup_FailsWhenIncompatible()
        {
            var orderedSet = new OrderedHashSet<string>(StringComparer.Ordinal);

            orderedSet.GetAlternateLookup<ReadOnlySpan<char>>();
            Assert.True(orderedSet.TryGetAlternateLookup<ReadOnlySpan<char>>(out _));

            Assert.Throws<InvalidOperationException>(() => orderedSet.GetAlternateLookup<ReadOnlySpan<byte>>());
            Assert.Throws<InvalidOperationException>(() => orderedSet.GetAlternateLookup<string>());
            Assert.Throws<InvalidOperationException>(() => orderedSet.GetAlternateLookup<int>());

            Assert.False(orderedSet.TryGetAlternateLookup<ReadOnlySpan<byte>>(out _));
            Assert.False(orderedSet.TryGetAlternateLookup<string>(out _));
            Assert.False(orderedSet.TryGetAlternateLookup<int>(out _));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void OrderedHashSet_GetAlternateLookup_OperationsMatchUnderlyingSet(int mode)
        {
            // Test with a variety of comparers to ensure that the alternate lookup is consistent with the underlying set
            OrderedHashSet<string> set = new(mode switch
            {
                0 => StringComparer.Ordinal,
                1 => StringComparer.OrdinalIgnoreCase,
                2 => StringComparer.InvariantCulture,
                3 => StringComparer.InvariantCultureIgnoreCase,
                4 => StringComparer.CurrentCulture,
                5 => StringComparer.CurrentCultureIgnoreCase,
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            });
            OrderedHashSet<string>.AlternateLookup<ReadOnlySpan<char>> lookup = set.GetAlternateLookup<ReadOnlySpan<char>>();
            Assert.Same(set, lookup.Set);
            Assert.Same(lookup.Set, lookup.Set);

            // Add to the set and validate that the lookup reflects the changes
            Assert.True(set.Add("123"));
            Assert.True(lookup.Contains("123".AsSpan()));
            Assert.False(lookup.Add("123".AsSpan()));
            Assert.True(lookup.Remove("123".AsSpan()));
            Assert.False(set.Contains("123"));

            // Add via the lookup and validate that the set reflects the changes
            Assert.True(lookup.Add("123".AsSpan()));
            Assert.True(set.Contains("123"));
            lookup.TryGetValue("123".AsSpan(), out string value);
            Assert.Equal("123", value);
            Assert.False(lookup.Remove("321".AsSpan()));
            Assert.True(lookup.Remove("123".AsSpan()));

            // Ensure that case-sensitivity of the comparer is respected
            Assert.True(lookup.Add("a"));
            if (set.EqualityComparer.Equals(StringComparer.Ordinal) ||
                set.EqualityComparer.Equals(StringComparer.InvariantCulture) ||
                set.EqualityComparer.Equals(StringComparer.CurrentCulture))
            {
                Assert.True(lookup.Add("A".AsSpan()));
                Assert.True(lookup.Remove("a".AsSpan()));
                Assert.False(lookup.Remove("a".AsSpan()));
                Assert.True(lookup.Remove("A".AsSpan()));
            }
            else
            {
                Assert.False(lookup.Add("A".AsSpan()));
                Assert.True(lookup.Remove("A".AsSpan()));
                Assert.False(lookup.Remove("a".AsSpan()));
                Assert.False(lookup.Remove("A".AsSpan()));
            }

            // Test the behavior of null vs "" in the set and lookup
            Assert.True(set.Add(null));
            Assert.True(set.Add(string.Empty));
            Assert.True(set.Contains(null));
            Assert.True(set.Contains(""));
            Assert.True(lookup.Contains("".AsSpan()));
            Assert.True(lookup.Remove("".AsSpan()));
            Assert.Equal(1, set.Count);
            Assert.False(lookup.Remove("".AsSpan()));
            Assert.True(set.Remove(null));
            Assert.Equal(0, set.Count);

            // Test adding multiple entries via the lookup
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i, set.Count);
                Assert.True(lookup.Add(i.ToString().AsSpan()));
                Assert.False(lookup.Add(i.ToString().AsSpan()));
            }

            Assert.Equal(10, set.Count);

            // Test that the lookup and the set agree on what's in and not in
            for (int i = -1; i <= 10; i++)
            {
                Assert.Equal(set.TryGetValue(i.ToString(), out string dv), lookup.TryGetValue(i.ToString().AsSpan(), out string lv));
                Assert.Equal(dv, lv);
            }

            // Test removing multiple entries via the lookup
            for (int i = 9; i >= 0; i--)
            {
                Assert.True(lookup.Remove(i.ToString().AsSpan()));
                Assert.False(lookup.Remove(i.ToString().AsSpan()));
                Assert.Equal(i, set.Count);
            }
        }
#endif

        #endregion

        #region SetAt

        [Theory]
        [MemberData(nameof(ValidPositiveCollectionSizes))]
        public void OrderedHashSet_Generic_SetAt_InvalidInputs(int count)
        {
            OrderedHashSet<T> set = (OrderedHashSet<T>)GenericISetFactory(count);
            T value = CreateT(1);

            // Test index out of range
            Assert.Throws<ArgumentOutOfRangeException>(() => set.SetAt(-1, value));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.SetAt(count, value));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.SetAt(int.MaxValue, value));
        }

        [Fact]
        public void OrderedHashSet_Generic_SetAt_ValueUpdated()
        {
            OrderedHashSet<int> set = new OrderedHashSet<int> { 1, 2, 3, 4, 5 };
            int newValue = 99;

            set.SetAt(0, newValue);

            // The value at index 0 should be updated
            Assert.Equal(newValue, set.ElementAt(0));
        }

        [Fact]
        public void OrderedHashSet_Generic_SetAt_SameIndex_ValueUnchanged()
        {
            OrderedHashSet<int> set = new OrderedHashSet<int> { 1, 2, 3, 4, 5 };
            int originalValue = set.GetAt(2);
            int newValue = originalValue;

            set.SetAt(2, newValue);

            // The value at index 2 should be the same as before
            Assert.Equal(originalValue, set.ElementAt(2));
        }

        [Fact]
        public void OrderedHashSet_Generic_SetAt_ValueReplacedAndFindable()
        {
            // When a value at an index is replaced via SetAt, the new value
            // should be findable via Contains() and the old value should not be.

            OrderedHashSet<int> set = new OrderedHashSet<int>();

            // Add values 1-10
            for (int i = 1; i <= 10; i++)
            {
                set.Add(i);
            }

            Assert.Equal(10, set.Count);
            Assert.True(set.Contains(1)); // Value at index 0 initially

            // Replace value 1 (at index 0) with value 12
            set.SetAt(0, 12);

            // After SetAt, the entry array contains 12 at index 0
            Assert.Equal(12, set.ElementAt(0));

            // The new value should be findable via Contains()
            // (requires rehashing of buckets)
            Assert.True(set.Contains(12));

            // The old value should no longer be findable
            Assert.False(set.Contains(1));
        }

        [Fact]
        public void OrderedHashSet_Generic_SetAt_ReplacedValueFindableByLookup()
        {
            // After replacing a value via SetAt, the new value should be
            // findable and retrievable from the set.

            OrderedHashSet<int> set = new OrderedHashSet<int>();

            for (int i = 1; i <= 10; i++)
            {
                set.Add(i);
            }

            int oldValue = 1;
            int newValue = 12;

            // Get the index of the old value
            int indexOfOldValue = set.ToList().IndexOf(oldValue);

            // Replace old value with new value
            set.SetAt(indexOfOldValue, newValue);

            // The old value should no longer be findable
            Assert.False(set.Contains(oldValue));

            // The new value should be stored at the index
            Assert.Equal(newValue, set.ElementAt(indexOfOldValue));

            // The new value should be findable via Contains()
            Assert.True(set.Contains(newValue));
        }

        [Fact]
        public void OrderedHashSet_Generic_SetAt_ReplacedValueWithDifferentHash()
        {
            // When replacing a value with one that hashes to a different bucket,
            // the new value should still be findable.

            OrderedHashSet<int> set = new OrderedHashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // SetAt(1, 20) - replace value at index 1 (which is value 2) with value 20
            // 2 and 20 hash to different buckets
            set.SetAt(1, 20);

            // The new value is stored in the entries array
            Assert.Equal(20, set.ElementAt(1));

            // The new value should be findable via Contains()
            Assert.True(set.Contains(20));

            // The old value should no longer be findable
            Assert.False(set.Contains(2));
        }

        [Fact]
        public void OrderedHashSet_Generic_SetAt_MultipleReplacements_AllFindable()
        {
            // When multiple values are replaced via SetAt, all new values
            // should be findable and old values should not be.

            OrderedHashSet<int> set = new OrderedHashSet<int>();

            // Build initial set
            for (int i = 1; i <= 5; i++)
            {
                set.Add(i);
            }

            // Replace multiple values
            set.SetAt(0, 100); // Replace 1 with 100
            set.SetAt(1, 101); // Replace 2 with 101
            set.SetAt(2, 102); // Replace 3 with 102

            // The entry array is updated
            Assert.Equal(100, set.ElementAt(0));
            Assert.Equal(101, set.ElementAt(1));
            Assert.Equal(102, set.ElementAt(2));

            // All new values should be findable via Contains()
            Assert.True(set.Contains(100));
            Assert.True(set.Contains(101));
            Assert.True(set.Contains(102));

            // The old values should not be found
            Assert.False(set.Contains(1));
            Assert.False(set.Contains(2));
            Assert.False(set.Contains(3));

            // The unchanged values should still be findable
            Assert.True(set.Contains(4));
            Assert.True(set.Contains(5));
        }

        #endregion
    }
}
