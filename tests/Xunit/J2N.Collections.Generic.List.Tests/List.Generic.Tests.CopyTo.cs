// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using J2N.Collections.Generic;
using J2N.Collections.Generic.Extensions;
using J2N.TestUtilities.Xunit;
using Xunit;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public abstract partial class List_Generic_Tests<T> : IList_Generic_Tests<T>
    {
        [Fact]
        public void CopyTo_InvalidArgs_Throws()
        {
            AssertExtensions.Throws<ArgumentNullException>("list", () => ListExtensions.CopyTo(null!, Span<int>.Empty));
            AssertExtensions.Throws<ArgumentNullException>("list", () => ListExtensions.CopyTo(null!, new Span<int>(new int[1])));

            var list = new List<int>() { 1, 2, 3 };
            Assert.Throws<ArgumentException>(() => ListExtensions.CopyTo(list, (Span<int>)new int[2]));
        }

        // J2N specific test for SubLists
        [Fact]
        public void CopyTo_SubList_ItemsCopiedCorrectly()
        {
            List<int> list, sublist;
            Span<int> destination;

            list = new List<int>();
            sublist = list.GetView(0, 0);
            destination = Span<int>.Empty;
            sublist.CopyTo(destination);

            list = new List<int>() { 1, 2, 3 };
            sublist = list.GetView(0, 2);
            destination = new int[3];
            sublist.CopyTo(destination);
            Assert.Equal(new[] { 1, 2, 0 }, destination.ToArray());

            list = new List<int>() { 1, 2, 3 };
            sublist = list.GetView(1, 2);
            destination = new int[4];
            // ReSharper disable once ReplaceSliceWithRangeIndexer
            sublist.CopyTo(destination.Slice(1));
            Assert.Equal(new[] { 0, 2, 3, 0 }, destination.ToArray());
        }

        [Fact]
        public void CopyTo_ItemsCopiedCorrectly()
        {
            List<int> list;
            Span<int> destination;

            list = new List<int>();
            destination = Span<int>.Empty;
            list.CopyTo(destination);

            list = new List<int>() { 1, 2, 3 };
            destination = new int[3];
            list.CopyTo(destination);
            Assert.Equal(new[] { 1, 2, 3 }, destination.ToArray());

            list = new List<int>() { 1, 2, 3 };
            destination = new int[4];
            list.CopyTo(destination);
            Assert.Equal(new[] { 1, 2, 3, 0 }, destination.ToArray());
        }
    }
}
