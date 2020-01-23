// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace J2N.Collections.ObjectModel.Tests
{
    /// <summary>
    /// Since <see cref="ReadOnlyList{T}"/> is just a wrapper base class around an <see cref="IList{T}"/>,
    /// we just verify that the underlying list is what we expect, validate that the calls which
    /// we expect are forwarded to the underlying list, and verify that the exceptions we expect
    /// are thrown.
    /// </summary>
    public class ReadOnlySetTests : CollectionTestBase
    {
        private static readonly ReadOnlySet<int> s_empty = new ReadOnlySet<int>(new HashSet<int>());
        private static readonly HashSet<int> s_intSet = new HashSet<int>(s_intArray);

        [Fact]
        public static void Ctor_NullList_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("set", () => new ReadOnlySet<int>(null));
        }

        [Fact]
        public static void Ctor_IList()
        {
            var collection = new TestCollection<int>(s_intSet);
            Assert.Same(s_intSet, collection.GetItems());
        }

        [Fact]
        public static void Count()
        {
            var collection = new ReadOnlySet<int>(s_intSet);
            Assert.Equal(s_intSet.Count, collection.Count);
            Assert.Equal(0, s_empty.Count);
        }

        [Fact]
        public static void IsReadOnly_ReturnsTrue()
        {
            var collection = new ReadOnlySet<int>(s_intSet);
            Assert.True(((ISet<int>)collection).IsReadOnly);
            Assert.True(((ICollection<int>)s_empty).IsReadOnly);
        }

        [Fact]
        public static void Contains()
        {
            var collection = new ReadOnlySet<int>(s_intSet);
            for (int i = 0; i < s_intArray.Length; i++)
            {
                Assert.True(collection.Contains(s_intArray[i]));
            }

            for (int i = 0; i < s_excludedFromIntArray.Length; i++)
            {
                Assert.False(collection.Contains(s_excludedFromIntArray[i]));
            }
        }

        [Fact]
        public static void CopyTo()
        {
            var collection = new ReadOnlySet<int>(s_intSet);
            const int targetIndex = 3;
            int[] intArray = new int[s_intArray.Length + targetIndex];

            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(intArray, -1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(intArray, s_intArray.Length - 1));

            collection.CopyTo(intArray, targetIndex);
            for (int i = targetIndex; i < intArray.Length; i++)
            {
                Assert.True(collection.Contains(intArray[i]));
            }
        }

        [Fact]
        public static void MembersForwardedToUnderlyingIList()
        {
            var expectedApiCalls =
                ISetApi.Count |
                ISetApi.Contains |
                ISetApi.CopyTo |
                ISetApi.GetEnumeratorGeneric |
                ISetApi.IsProperSubsetOf |
                ISetApi.IsProperSupersetOf |
                ISetApi.IsSubsetOf |
                ISetApi.IsSupersetOf |
                ISetApi.Overlaps |
                ISetApi.SetEquals |
                ISetApi.GetEnumerator;

            var set = new CallTrackingISet<int>(expectedApiCalls);
            var collection = new ReadOnlySet<int>(set);

            int count = collection.Count;
            bool readOnly = ((ICollection<int>)collection).IsReadOnly;
            collection.Contains(default);
            collection.CopyTo(s_intArray, 0);
            collection.GetEnumerator();
            collection.IsProperSubsetOf(s_intArray);
            collection.IsProperSupersetOf(s_intArray);
            collection.IsSubsetOf(s_intArray);
            collection.IsSupersetOf(s_intArray);
            collection.Overlaps(s_intArray);
            collection.SetEquals(s_intArray);
            ((IEnumerable)collection).GetEnumerator();

            set.AssertAllMembersCalled();
        }

        [Fact]
        public void ModifyingCollection_ThrowsNotSupportedException()
        {
            var collection = (ISet<int>)new ReadOnlySet<int>(s_intSet);

            Assert.Throws<NotSupportedException>(() => collection.Add(0));
            Assert.Throws<NotSupportedException>(() => collection.Clear());
            Assert.Throws<NotSupportedException>(() => collection.ExceptWith(s_intArray));
            Assert.Throws<NotSupportedException>(() => collection.IntersectWith(s_intArray));
            Assert.Throws<NotSupportedException>(() => collection.Remove(0));
            Assert.Throws<NotSupportedException>(() => collection.SymmetricExceptWith(s_intArray));
            Assert.Throws<NotSupportedException>(() => collection.UnionWith(s_intArray));
        }

        private class TestCollection<T> : ReadOnlySet<T>
        {
            public TestCollection(ISet<T> items) : base(items)
            {
            }

            public ISet<T> GetItems() => Items;
        }
    }
}
