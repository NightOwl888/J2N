// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using J2N.TestUtilities.Xunit;
using Xunit;
using JCG = J2N.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public abstract partial class List_Generic_Tests<T> : IList_Generic_Tests<T>
    {
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void EnsureCapacity_RequestingLargerCapacity_DoesInvalidateEnumeration(int count)
        {
            JCG.List<T> list = GenericListFactory(count);
            IEnumerator<T> copiedListEnumerator = new JCG.List<T>(list).GetEnumerator();
            IEnumerator<T> enumerator = list.GetEnumerator();
            var capacity = list.Capacity;

            list.EnsureCapacity(capacity + 1);

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        public void EnsureCapacity_NotInitialized_RequestedZero_ReturnsZero()
        {
            var list = new JCG.List<T>();
            Assert.Equal(0, list.EnsureCapacity(0));
            Assert.Equal(0, list.Capacity);
        }

        [Fact]
        public void EnsureCapacity_NegativeCapacityRequested_Throws()
        {
            var list = new JCG.List<T>();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => list.EnsureCapacity(-1));
        }

        const int MaxArraySize = 0X7FEFFFFF;

        [Theory]
        //[InlineData(5, MaxArraySize + 1)] // J2N NOTE: This does not throw OutOfMemoryException in all platforms, and is not really a useful test in practice.
        [InlineData(1, int.MaxValue)]
        //[SkipOnMono("mono forces no restrictions on array size.")]
        public void EnsureCapacity_LargeCapacity_Throws(int count, int requestCapacity)
        {
            JCG.List<T> list = GenericListFactory(count);
            Assert.Throws<OutOfMemoryException>(() => list.EnsureCapacity(requestCapacity));
        }

        [Theory]
        [InlineData(5)]
        public void EnsureCapacity_RequestedCapacitySmallerThanOrEqualToCurrent_CapacityUnchanged(int currentCapacity)
        {
            var list = new JCG.List<T>(currentCapacity);

            for (int requestCapacity = 0; requestCapacity <= currentCapacity; requestCapacity++)
            {
                Assert.Equal(currentCapacity, list.EnsureCapacity(requestCapacity));
                Assert.Equal(currentCapacity, list.Capacity);
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void EnsureCapacity_RequestedCapacitySmallerThanOrEqualToCount_CapacityUnchanged(int count)
        {
            JCG.List<T> list = GenericListFactory(count);
            var currentCapacity = list.Capacity;

            for (int requestCapacity = 0; requestCapacity <= count; requestCapacity++)
            {
                Assert.Equal(currentCapacity, list.EnsureCapacity(requestCapacity));
                Assert.Equal(currentCapacity, list.Capacity);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void EnsureCapacity_CapacityIsAtLeastTheRequested(int count)
        {
            JCG.List<T> list = GenericListFactory(count);

            int currentCapacity = list.Capacity;
            int requestCapacity = currentCapacity + 1;
            int newCapacity = list.EnsureCapacity(requestCapacity);
            Assert.InRange(newCapacity, requestCapacity, int.MaxValue);
        }

        // J2N NOTE: Per comment in DoSetCapacity, "we consider reallocating the array a "modification" to the list to ensure sublists use the same array reference"
        // [Theory]
        // [MemberData(nameof(ValidCollectionSizes))]
        // public void EnsureCapacity_RequestingLargerCapacity_DoesNotImpactListContent(int count)
        // {
        //     JCG.List<T> list = GenericListFactory(count);
        //     var copiedList = new JCG.List<T>(list);
        //
        //     list.EnsureCapacity(list.Capacity + 1);
        //     Assert.Equal(copiedList, list);
        // }
    }
}
