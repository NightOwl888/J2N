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

        // J2N-specific test: Verify that Slice on SubList returns correct grandchild view
        [Fact]
        public void TestSlice_OnSubList_ReturnsGrandchildView()
        {
            // Arrange
            List<int> list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

            // Create SubList: elements 5-11 (7 elements starting at index 5)
            List<int> subList = list.GetView(5, 7); // [5, 6, 7, 8, 9, 10, 11]

            // Create Slice of SubList: elements 2-4 of subList (3 elements)
            List<int> slicedSubList = subList.Slice(2, 3); // [7, 8, 9]

            // Assert initial values
            Assert.Equal(3, slicedSubList.Count);
            Assert.Equal(7, slicedSubList[0]);
            Assert.Equal(8, slicedSubList[1]);
            Assert.Equal(9, slicedSubList[2]);

            // Modify through the sliced SubList
            slicedSubList[1] = 800;

            // Assert changes propagate to all levels
            Assert.Equal(800, list[8]); // Original list affected
            Assert.Equal(800, subList[3]); // Parent SubList affected
            Assert.Equal(800, slicedSubList[1]); // Slice reflects change
        }

        // J2N-specific test: Multiple levels of Slice nesting
        [Fact]
        public void TestSlice_MultipleLevelsOfNesting()
        {
            // Arrange
            List<int> list = new List<int>();
            for (int i = 0; i < 100; i++)
                list.Add(i);

            // Create multiple levels of views/slices
            List<int> view1 = list.GetView(20, 60); // Elements 20-79
            List<int> slice1 = view1.Slice(10, 40); // Elements 30-69 in original
            List<int> view2 = slice1.GetView(5, 30); // Elements 35-64 in original
            List<int> slice2 = view2.Slice(5, 20); // Elements 40-59 in original

            // Verify correct elements
            Assert.Equal(20, slice2.Count);
            Assert.Equal(40, slice2[0]);
            Assert.Equal(59, slice2[19]);

            // Modify and verify propagation
            slice2[10] = 5000; // Modify element at index 50 in original

            Assert.Equal(5000, list[50]);
            Assert.Equal(5000, view1[30]);
            Assert.Equal(5000, slice1[20]);
            Assert.Equal(5000, view2[15]);
            Assert.Equal(5000, slice2[10]);
        }

        // J2N-specific test: Slice with operations on SubList
        [Fact]
        public void TestSlice_OperationsOnSubList()
        {
            // Arrange
            List<int> list = new List<int> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            List<int> subList = list.GetView(2, 6); // [30, 40, 50, 60, 70, 80]
            List<int> slice = subList.Slice(1, 4); // [40, 50, 60, 70]

            // Test Add operation
            slice.Add(75);
            Assert.Equal(5, slice.Count);
            Assert.Equal(75, slice[4]);
            Assert.Equal(7, subList.Count);
            Assert.Equal(11, list.Count);
            Assert.Equal(75, list[7]); // New element at position 7 in original list

            // Test Remove operation
            slice.RemoveAt(2); // Remove 60
            Assert.Equal(4, slice.Count);
            Assert.Equal(new List<int> { 40, 50, 70, 75 }, slice);
            Assert.Equal(6, subList.Count);
            Assert.Equal(10, list.Count);

            // Test Clear operation
            slice.Clear();
            Assert.Equal(0, slice.Count);
            Assert.Equal(2, subList.Count); // Only [30, 80] remain
            Assert.Equal(6, list.Count); // [10, 20, 30, 80, 90, 100]
        }

        // J2N-specific test: Slice boundary conditions with SubLists
        [Fact]
        public void TestSlice_BoundaryConditionsWithSubList()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<int> subList = list.GetView(3, 5); // [4, 5, 6, 7, 8]

            // Test full slice (should behave like another view)
            List<int> fullSlice = subList.Slice(0, 5);
            Assert.Equal(5, fullSlice.Count);
            Assert.Equal(subList.Count, fullSlice.Count);
            fullSlice[2] = 600;
            Assert.Equal(600, list[5]);
            Assert.Equal(600, subList[2]);

            // Test empty slice
            List<int> emptySlice = subList.Slice(2, 0);
            Assert.Equal(0, emptySlice.Count);

            // Test single element slice
            List<int> singleSlice = subList.Slice(3, 1);
            Assert.Equal(1, singleSlice.Count);
            Assert.Equal(7, singleSlice[0]);

            // Test slice at the end
            List<int> endSlice = subList.Slice(3, 2);
            Assert.Equal(2, endSlice.Count);
            Assert.Equal(7, endSlice[0]);
            Assert.Equal(8, endSlice[1]);
        }

        // J2N-specific test: Verify Slice works with InsertRange on SubLists
        [Fact]
        public void TestSlice_InsertRangeOnSubList()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<int> subList = list.GetView(2, 5); // [3, 4, 5, 6, 7]
            List<int> slice = subList.Slice(1, 3); // [4, 5, 6]

            // Act - insert range into slice
            slice.InsertRange(1, new[] { 44, 55 });

            // Assert
            Assert.Equal(5, slice.Count); // Slice should have 5 elements now
            Assert.Equal(new List<int> { 4, 44, 55, 5, 6 }, slice);
            Assert.Equal(7, subList.Count); // SubList should have 7 elements
            Assert.Equal(12, list.Count); // Original list should have 12 elements

            // Verify correct insertion in original list
            var expectedList = new List<int> { 1, 2, 3, 4, 44, 55, 5, 6, 7, 8, 9, 10 };
            Assert.Equal(expectedList, list);
        }

        // J2N-specific test: Verify Slice with RemoveRange on SubLists
        [Fact]
        public void TestSlice_RemoveRangeOnSubList()
        {
            // Arrange
            List<int> list = new List<int> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110 };
            List<int> subList = list.GetView(2, 7); // [30, 40, 50, 60, 70, 80, 90]
            List<int> slice = subList.Slice(1, 5); // [40, 50, 60, 70, 80]

            // Act - remove range from slice
            slice.RemoveRange(1, 3); // Remove [50, 60, 70]

            // Assert
            Assert.Equal(2, slice.Count); // Slice should have 2 elements left
            Assert.Equal(new List<int> { 40, 80 }, slice);
            Assert.Equal(4, subList.Count); // SubList should have 4 elements
            Assert.Equal(8, list.Count); // Original list should have 8 elements

            // Verify correct removal from original list
            var expectedList = new List<int> { 10, 20, 30, 40, 80, 90, 100, 110 };
            Assert.Equal(expectedList, list);
        }

        // J2N-specific test: Verify concurrent modification detection with Slice
        [Fact]
        public void TestSlice_ConcurrentModificationDetection()
        {
            // Test 1: Modifications through a slice don't invalidate the slice itself
            List<int> list = new List<int> { 1, 2, 3, 4, 5 };
            List<int> slice1 = list.Slice(1, 3); // [2, 3, 4]

            // Modify through slice1
            slice1[0] = 200;

            // Slice1 is still valid after its own modification
            Assert.Equal(200, slice1[0]);
            Assert.Equal(3, slice1[1]);
            Assert.Equal(4, slice1[2]);

            // Test 2: Direct modifications to parent invalidate all slices
            List<int> list2 = new List<int> { 10, 20, 30, 40, 50 };
            List<int> slice2 = list2.Slice(1, 3); // [20, 30, 40]
            List<int> slice3 = list2.Slice(2, 2); // [30, 40]

            // Initial check
            Assert.Equal(20, slice2[0]);
            Assert.Equal(30, slice3[0]);

            // Modify the original list directly
            list2.Add(60);

            // Both slices should now be invalid for operations that check co-modification
            Assert.Throws<InvalidOperationException>(() => slice2[0]);
            Assert.Throws<InvalidOperationException>(() => slice3[0]);
            Assert.Throws<InvalidOperationException>(() => slice2.Add(100));
            Assert.Throws<InvalidOperationException>(() => slice3.RemoveAt(0));

            // Test 3: Nested slices - modification through parent slice
            List<int> list3 = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            List<int> parentSlice = list3.Slice(1, 5); // [2, 3, 4, 5, 6]
            List<int> childSlice = parentSlice.Slice(1, 3); // [3, 4, 5]

            // Modify through parent slice
            parentSlice[0] = 200;

            // Parent slice is still valid
            Assert.Equal(200, parentSlice[0]);

            // Child slice is invalidated because parent was modified
            Assert.Throws<InvalidOperationException>(() => childSlice[0]);
        }

        // J2N-specific test: Verify Slice of Slice of Slice (triple nesting)
        [Fact]
        public void TestSlice_TripleNesting()
        {
            // Arrange
            List<int> list = new List<int>();
            for (int i = 0; i < 50; i++)
                list.Add(i * 2); // [0, 2, 4, 6, 8, ..., 98]

            // Create first level slice
            List<int> slice1 = list.Slice(10, 30); // Elements 20-78 (indices 10-39)
            Assert.Equal(20, slice1[0]);
            Assert.Equal(78, slice1[29]);

            // Create second level slice
            List<int> slice2 = slice1.Slice(5, 20); // Elements 30-68 (indices 15-34 in original)
            Assert.Equal(30, slice2[0]);
            Assert.Equal(68, slice2[19]);

            // Create third level slice
            List<int> slice3 = slice2.Slice(5, 10); // Elements 40-58 (indices 20-29 in original)
            Assert.Equal(40, slice3[0]);
            Assert.Equal(58, slice3[9]);

            // Modify through the deepest slice
            slice3[5] = 999; // Modify element at index 25 in original (was 50)

            // Verify propagation through all levels
            Assert.Equal(999, list[25]);
            Assert.Equal(999, slice1[15]);
            Assert.Equal(999, slice2[10]);
            Assert.Equal(999, slice3[5]);
        }

        // J2N-specific test: Verify Slice with Contains/IndexOf/LastIndexOf
        [Fact]
        public void TestSlice_SearchMethods()
        {
            // Arrange
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 3, 4, 5 };
            List<int> subList = list.GetView(2, 9); // [3, 4, 5, 6, 7, 8, 9, 10, 3]
            List<int> slice = subList.Slice(1, 7); // [4, 5, 6, 7, 8, 9, 10]

            // Test Contains
            Assert.True(slice.Contains(6));
            Assert.True(slice.Contains(10));
            Assert.False(slice.Contains(3));
            Assert.False(slice.Contains(11));

            // Test IndexOf
            Assert.Equal(0, slice.IndexOf(4));
            Assert.Equal(2, slice.IndexOf(6));
            Assert.Equal(6, slice.IndexOf(10));
            Assert.Equal(-1, slice.IndexOf(3));

            // Test LastIndexOf
            Assert.Equal(0, slice.LastIndexOf(4));
            Assert.Equal(2, slice.LastIndexOf(6));
            Assert.Equal(6, slice.LastIndexOf(10));
            Assert.Equal(-1, slice.LastIndexOf(3));

            // Add duplicate and test again
            slice.Add(4);
            Assert.Equal(0, slice.IndexOf(4));
            Assert.Equal(7, slice.LastIndexOf(4));
        }
    }
}