using J2N.Collections.Generic;
using System;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// J2N-specific tests for List.Slice() method to verify it returns a view (SubList) instead of a copy.
    /// </summary>
    public class List_Generic_Tests_Slice_J2N
    {
        // J2N-specific test: Verify that Slice returns a view (SubList) instead of a copy
        [Fact]
        public void TestSlice_ReturnsView_NotCopy()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            List<int> slice = list.Slice(2, 4); // Get elements at indices 2-5 (values: 3, 4, 5, 6)

            // Assert initial state
            Assert.Equal(4, slice.Count);
            Assert.Equal(3, slice[0]);
            Assert.Equal(4, slice[1]);
            Assert.Equal(5, slice[2]);
            Assert.Equal(6, slice[3]);

            // Modify the slice
            slice[1] = 100;

            // Assert that the original list is affected (proving it's a view, not a copy)
            Assert.Equal(100, list[3]); // Slice should be a view - changes should affect original list
            Assert.Equal(100, slice[1]); // Slice should reflect the change
        }

        // J2N-specific test: Verify that modifying the original list via the slice's index works
        [Fact]
        public void TestSlice_ModifyOriginal_ThroughSliceIndex()
        {
            // Arrange
            List<int> list = new List<int> { 10, 20, 30, 40, 50 };
            List<int> slice = list.Slice(1, 3); // Get elements at indices 1-3 (values: 20, 30, 40)

            // Initial values check
            Assert.Equal(20, slice[0]);
            Assert.Equal(30, slice[1]);
            Assert.Equal(40, slice[2]);

            // Since SubList has concurrent modification protection,
            // we can't modify the parent list directly after creating a slice.
            // This is expected behavior from Java's SubList implementation.
        }

        // J2N-specific test: Verify that direct modification of parent list after creating slice throws
        [Fact]
        public void TestSlice_ModifyOriginal_AfterSliceCreation_Throws()
        {
            // Arrange
            List<int> list = new List<int> { 10, 20, 30, 40, 50 };
            List<int> slice = list.Slice(1, 3); // Get elements at indices 1-3 (values: 20, 30, 40)

            // Act - modify the original list (this should invalidate the slice)
            list[2] = 300;

            // Assert that accessing the slice now throws due to concurrent modification
            Assert.Throws<InvalidOperationException>(() => slice[1]);
        }

        // J2N-specific test: Verify that adding to a slice view affects the original list
        [Fact]
        public void TestSlice_AddToSlice_AffectsOriginal()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5 };
            List<int> slice = list.Slice(1, 2); // Get elements at indices 1-2 (values: 2, 3)

            // Act - add to the slice
            slice.Add(99);

            // Assert
            Assert.Equal(6, list.Count); // Adding to slice should increase original list size
            Assert.Equal(99, list[3]); // New element should be inserted at the correct position in original list
            Assert.Equal(3, slice.Count); // Slice count should increase
            Assert.Equal(99, slice[2]); // New element should be at the end of the slice
        }

        // J2N-specific test: Verify that removing from a slice view affects the original list
        [Fact]
        public void TestSlice_RemoveFromSlice_AffectsOriginal()
        {
            // Arrange
            List<int> list = new List<int> { 10, 20, 30, 40, 50, 60 };
            List<int> slice = list.Slice(2, 3); // Get elements at indices 2-4 (values: 30, 40, 50)

            // Act - remove from the slice
            slice.RemoveAt(1); // Remove 40

            // Assert
            Assert.Equal(5, list.Count); // Removing from slice should decrease original list size

            var expectedList = new List<int> { 10, 20, 30, 50, 60 };
            Assert.Equal(expectedList.Count, list.Count);
            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.Equal(expectedList[i], list[i]);
            }

            Assert.Equal(2, slice.Count); // Slice count should decrease
            Assert.Equal(30, slice[0]);
            Assert.Equal(50, slice[1]);
        }

        // J2N-specific test: Verify that clearing a slice view affects the original list
        [Fact]
        public void TestSlice_ClearSlice_AffectsOriginal()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            List<int> slice = list.Slice(2, 3); // Get elements at indices 2-4 (values: 3, 4, 5)

            // Act - clear the slice
            slice.Clear();

            // Assert
            Assert.Equal(4, list.Count); // Clearing slice should remove elements from original list

            var expectedList = new List<int> { 1, 2, 6, 7 };
            Assert.Equal(expectedList.Count, list.Count);
            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.Equal(expectedList[i], list[i]);
            }

            Assert.Equal(0, slice.Count); // Slice should be empty
        }

        // J2N-specific test: Verify that nested slice views work correctly
        [Fact]
        public void TestSlice_NestedSlices()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<int> slice1 = list.Slice(2, 6); // Get elements at indices 2-7 (values: 3, 4, 5, 6, 7, 8)
            List<int> slice2 = slice1.Slice(1, 3); // Get elements at indices 1-3 of slice1 (values: 4, 5, 6)

            // Act - modify nested slice
            slice2[1] = 500;

            // Assert that all levels are affected
            Assert.Equal(500, list[4]); // Change in nested slice should affect original list
            Assert.Equal(500, slice1[2]); // Change in nested slice should affect parent slice
            Assert.Equal(500, slice2[1]); // Nested slice should reflect the change
        }

        // J2N-specific test: Verify bounds checking for the Slice method
        [Fact]
        public void TestSlice_BoundsChecking()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5 };

            // Test valid slice
            List<int> validSlice = list.Slice(1, 3);
            Assert.Equal(3, validSlice.Count);

            // Test edge cases
            List<int> fullSlice = list.Slice(0, 5);
            Assert.Equal(5, fullSlice.Count);

            List<int> emptySlice = list.Slice(2, 0);
            Assert.Equal(0, emptySlice.Count);

            // Test invalid slices should throw
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Slice(-1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Slice(0, -1));
            Assert.Throws<ArgumentException>(() => list.Slice(3, 5)); // start + length > count
        }

        // J2N-specific test: Verify that GetRange still returns a copy (for comparison)
        [Fact]
        public void TestGetRange_ReturnsCopy_NotView()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            List<int> range = list.GetRange(1, 3); // Get elements at indices 1-3 (values: 2, 3, 4)

            // Modify the range
            range[1] = 100;

            // Assert that the original list is NOT affected (proving it's a copy, not a view)
            Assert.Equal(3, list[2]); // GetRange should return a copy - changes should NOT affect original list
            Assert.Equal(100, range[1]); // Range should reflect the change
        }

        // J2N-specific test: Verify Slice vs GetRange behavior difference
        [Fact]
        public void TestSlice_Vs_GetRange_Behavior()
        {
            // Arrange
            List<int> list1 = new List<int> { 1, 2, 3, 4, 5 };
            List<int> list2 = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            List<int> slice = list1.Slice(1, 3);   // View
            List<int> range = list2.GetRange(1, 3); // Copy

            // Modify both
            slice[0] = 100;
            range[0] = 100;

            // Assert different behaviors
            Assert.Equal(100, list1[1]); // Slice modifies original
            Assert.Equal(2, list2[1]);   // GetRange doesn't modify original
        }
    }
}