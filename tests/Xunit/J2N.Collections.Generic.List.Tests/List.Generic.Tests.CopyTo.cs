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

        // J2N specific test for grandchild SubLists (SubList of SubList)
        [Fact]
        public void CopyTo_GrandchildSubList_ItemsCopiedCorrectly()
        {
            // Test the offset logic with nested SubLists
            List<int> list, sublist, grandchildSublist;
            Span<int> destination;

            // Setup: Create list with values 0-19
            list = new List<int>();
            for (int i = 0; i < 20; i++)
                list.Add(i);

            // Test 1: Basic grandchild SubList
            // Parent SubList: elements 5-14 (10 elements starting at index 5)
            sublist = list.GetView(5, 10);
            // Grandchild SubList: elements 2-7 of parent (6 elements), which is 7-12 in original list
            grandchildSublist = sublist.GetView(2, 6);
            destination = new int[6];
            grandchildSublist.CopyTo(destination);
            Assert.Equal(new[] { 7, 8, 9, 10, 11, 12 }, destination.ToArray());

            // Test 2: Empty grandchild SubList
            grandchildSublist = sublist.GetView(3, 0);
            destination = Span<int>.Empty;
            grandchildSublist.CopyTo(destination);

            // Test 3: Grandchild at the end of parent SubList
            grandchildSublist = sublist.GetView(7, 3); // Last 3 elements of parent (12, 13, 14)
            destination = new int[3];
            grandchildSublist.CopyTo(destination);
            Assert.Equal(new[] { 12, 13, 14 }, destination.ToArray());

            // Test 4: Grandchild at the beginning of parent SubList
            grandchildSublist = sublist.GetView(0, 3); // First 3 elements of parent (5, 6, 7)
            destination = new int[3];
            grandchildSublist.CopyTo(destination);
            Assert.Equal(new[] { 5, 6, 7 }, destination.ToArray());

            // Test 5: Multiple levels of nesting
            list = new List<int>();
            for (int i = 0; i < 30; i++)
                list.Add(i * 10); // 0, 10, 20, 30, ...

            sublist = list.GetView(10, 15); // Elements 100-240
            grandchildSublist = sublist.GetView(5, 8); // Elements 150-220
            var greatGrandchildSublist = grandchildSublist.GetView(2, 4); // Elements 170-200
            destination = new int[4];
            greatGrandchildSublist.CopyTo(destination);
            Assert.Equal(new[] { 170, 180, 190, 200 }, destination.ToArray());

            // Test 6: Partial copy to destination with offset
            list = new List<int>() { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            sublist = list.GetView(2, 6); // 30, 40, 50, 60, 70, 80
            grandchildSublist = sublist.GetView(1, 4); // 40, 50, 60, 70
            destination = new int[7];
            destination[0] = -1;
            destination[1] = -2;
            // ReSharper disable once ReplaceSliceWithRangeIndexer
            grandchildSublist.CopyTo(destination.Slice(2));
            Assert.Equal(new[] { -1, -2, 40, 50, 60, 70, 0 }, destination.ToArray());
        }

        // J2N specific test for grandchild SubLists with different types
        [Fact]
        public void CopyTo_GrandchildSubList_StringType()
        {
            List<string> list, sublist, grandchildSublist;
            Span<string> destination;

            // Create list with string values
            list = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

            // Parent SubList: elements "c" through "h" (6 elements starting at index 2)
            sublist = list.GetView(2, 6);

            // Grandchild SubList: elements 1-4 of parent (4 elements), which is "d", "e", "f", "g"
            grandchildSublist = sublist.GetView(1, 4);

            destination = new string[4];
            grandchildSublist.CopyTo(destination);
            Assert.Equal(new[] { "d", "e", "f", "g" }, destination.ToArray());

            // Test with null values
            list = new List<string>() { "a", null, "c", null, "e", null, "g", "h", null, "j" };
            sublist = list.GetView(1, 8); // null, "c", null, "e", null, "g", "h", null
            grandchildSublist = sublist.GetView(2, 4); // null, "e", null, "g"
            destination = new string[4];
            grandchildSublist.CopyTo(destination);
            Assert.Equal(new[] { null, "e", null, "g" }, destination.ToArray());
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
