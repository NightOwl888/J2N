﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public abstract partial class List_Generic_Tests<T> : IList_Generic_Tests<T>
    {
        public static SCG.IEnumerable<object[]> ValidCollectionSizes_GreaterThanOne()
        {
            yield return new object[] { 2 };
            yield return new object[] { 20 };
        }

        #region Sort

        [Theory]
        [MemberData(nameof(ValidCollectionSizes_GreaterThanOne))]
        public void Sort_WithoutDuplicates(int count)
        {
            List<T> list = GenericListFactory(count);
            SCG.IComparer<T> comparer = Comparer<T>.Default;
            list.Sort();
            Assert.All(Enumerable.Range(0, count - 2), i =>
            {
                Assert.True(comparer.Compare(list[i], list[i + 1]) < 0);
            });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes_GreaterThanOne))]
        public void Sort_WithDuplicates(int count)
        {
            List<T> list = GenericListFactory(count);
            list.Add(list[0]);
            SCG.IComparer<T> comparer = Comparer<T>.Default;
            list.Sort();
            Assert.All(Enumerable.Range(0, count - 2), i =>
            {
                Assert.True(comparer.Compare(list[i], list[i + 1]) <= 0);
            });
        }

        #endregion

        #region Sort(IComparer)

        [Theory]
        [MemberData(nameof(ValidCollectionSizes_GreaterThanOne))]
        public void Sort_IComparer_WithoutDuplicates(int count)
        {
            List<T> list = GenericListFactory(count);
            SCG.IComparer<T> comparer = GetIComparer();
            list.Sort(comparer);
            Assert.All(Enumerable.Range(0, count - 2), i =>
            {
                Assert.True(comparer.Compare(list[i], list[i + 1]) < 0);
            });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes_GreaterThanOne))]
        public void Sort_IComparer_WithDuplicates(int count)
        {
            List<T> list = GenericListFactory(count);
            list.Add(list[0]);
            SCG.IComparer<T> comparer = GetIComparer();
            list.Sort(comparer);
            Assert.All(Enumerable.Range(0, count - 2), i =>
            {
                Assert.True(comparer.Compare(list[i], list[i + 1]) <= 0);
            });
        }

        #endregion

        #region Sort(Comparison)

        [Theory]
        [MemberData(nameof(ValidCollectionSizes_GreaterThanOne))]
        public void Sort_Comparison_WithoutDuplicates(int count)
        {
            List<T> list = GenericListFactory(count);
            SCG.IComparer<T> iComparer = GetIComparer();
            Comparison<T> comparer = ((T first, T second) => { return iComparer.Compare(first, second); });
            list.Sort(comparer);
            Assert.All(Enumerable.Range(0, count - 2), i =>
            {
                Assert.True(iComparer.Compare(list[i], list[i + 1]) < 0);
            });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes_GreaterThanOne))]
        public void Sort_Comparison_WithDuplicates(int count)
        {
            List<T> list = GenericListFactory(count);
            list.Add(list[0]);
            SCG.IComparer<T> iComparer = GetIComparer();
            Comparison<T> comparer = ((T first, T second) => { return iComparer.Compare(first, second); });
            list.Sort(comparer);
            Assert.All(Enumerable.Range(0, count - 2), i =>
            {
                Assert.True(iComparer.Compare(list[i], list[i + 1]) <= 0);
            });
        }

        #endregion

        #region Sort(int, int, IComparer<T>)

        [Theory]
        [MemberData(nameof(ValidCollectionSizes_GreaterThanOne))]
        public void Sort_intintIComparer_WithoutDuplicates(int count)
        {
            List<T> unsortedList = GenericListFactory(count);
            SCG.IComparer<T> comparer = GetIComparer();
            for (int startIndex = 0; startIndex < count - 2; startIndex++)
                for (int sortCount = 1; sortCount < count - startIndex; sortCount++)
                {
                    List<T> list = new List<T>(unsortedList);
                    list.Sort(startIndex, sortCount + 1, comparer);
                    for (int i = startIndex; i < sortCount; i++)
                        Assert.InRange(comparer.Compare(list[i], list[i + 1]), int.MinValue, 0);
                }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes_GreaterThanOne))]
        public void Sort_intintIComparer_WithDuplicates(int count)
        {
            List<T> unsortedList = GenericListFactory(count);
            SCG.IComparer<T> comparer = GetIComparer();
            unsortedList.Add(unsortedList[0]);
            for (int startIndex = 0; startIndex < count - 2; startIndex++)
                for (int sortCount = 2; sortCount < count - startIndex; sortCount++)
                {
                    List<T> list = new List<T>(unsortedList);
                    list.Sort(startIndex, sortCount + 1, comparer);
                    for (int i = startIndex; i < sortCount; i++)
                        Assert.InRange(comparer.Compare(list[i], list[i + 1]), int.MinValue, 1);
                }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Sort_intintIComparer_NegativeRange_ThrowsArgumentOutOfRangeException(int count)
        {
            List<T> list = GenericListFactory(count);
            Tuple<int, int>[] InvalidParameters = new Tuple<int, int>[]
            {
                Tuple.Create(-1,-1),
                Tuple.Create(-1, 0),
                Tuple.Create(-1, 1),
                Tuple.Create(-1, 2),
                Tuple.Create(-2, 0),
                Tuple.Create(int.MinValue, 0),
                Tuple.Create(0 ,-1),
                Tuple.Create(0 ,-2),
                Tuple.Create(0 , int.MinValue),
                Tuple.Create(1 ,-1),
                Tuple.Create(2 ,-1),
            };

            Assert.All(InvalidParameters, invalidSet =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => list.Sort(invalidSet.Item1, invalidSet.Item2, GetIComparer()));
            });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Sort_intintIComparer_InvalidRange_ThrowsArgumentException(int count)
        {
            List<T> list = GenericListFactory(count);
            Tuple<int, int>[] InvalidParameters = new Tuple<int, int>[]
            {
                Tuple.Create(count, 1),
                Tuple.Create(count + 1, 0),
                Tuple.Create(int.MaxValue, 0),
            };

            Assert.All(InvalidParameters, invalidSet =>
            {
                AssertExtensions.Throws<ArgumentException>(null, () => list.Sort(invalidSet.Item1, invalidSet.Item2, GetIComparer()));
            });
        }

        #endregion
    }
}
