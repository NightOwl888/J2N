using NUnit.Framework;
using System;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// J2N-specific tests for EnsureCapacity behavior with SubLists.
    /// These tests verify that EnsureCapacity doesn't cause co-modification exceptions
    /// between Lists and SubLists in either direction.
    /// </summary>
    [TestFixture]
    public class TestList_EnsureCapacity
    {
        [Test]
        public void Test_List_EnsureCapacity_DoesNot_Invalidate_SubList()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var subList = list.GetView(2, 5); // Elements 3, 4, 5, 6, 7

            // Act - EnsureCapacity on parent list
            list.EnsureCapacity(100);

            // Assert - SubList operations should still work
            Assert.DoesNotThrow(() => subList.Add(11));
            Assert.AreEqual(6, subList.Count);
            Assert.AreEqual(11, list.Count);

            Assert.DoesNotThrow(() => subList[0] = 30);
            Assert.AreEqual(30, list[2]);

            Assert.DoesNotThrow(() => subList.RemoveAt(0));
            Assert.AreEqual(5, subList.Count);
            Assert.AreEqual(10, list.Count);
        }

        [Test]
        public void Test_SubList_EnsureCapacity_DoesNot_Invalidate_Parent_List()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var subList = list.GetView(2, 5); // Elements 3, 4, 5, 6, 7

            // Act - EnsureCapacity on sublist
            subList.EnsureCapacity(50);

            // Assert - Parent list operations should still work
            Assert.DoesNotThrow(() => list.Add(11));
            Assert.AreEqual(11, list.Count);

            Assert.DoesNotThrow(() => list[0] = 10);
            Assert.AreEqual(10, list[0]);

            Assert.DoesNotThrow(() => list.RemoveAt(0));
            Assert.AreEqual(10, list.Count);
        }

        [Test]
        public void Test_List_EnsureCapacity_DoesNot_Invalidate_SubList_Enumerator()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var subList = list.GetView(2, 5);
            var enumerator = subList.GetEnumerator();

            // Act - EnsureCapacity on parent list
            list.EnsureCapacity(100);

            // Assert - Enumerator should still work
            Assert.DoesNotThrow(() => enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);
        }

        [Test]
        public void Test_SubList_EnsureCapacity_DoesNot_Invalidate_Parent_Enumerator()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var subList = list.GetView(2, 5);
            var enumerator = list.GetEnumerator();

            // Act - EnsureCapacity on sublist
            subList.EnsureCapacity(50);

            // Assert - Enumerator should still work
            Assert.DoesNotThrow(() => enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);
        }

        [Test]
        public void Test_Nested_SubList_EnsureCapacity_DoesNot_Invalidate_Enumerators()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var subList1 = list.GetView(1, 8); // Elements 2-9
            var subList2 = subList1.GetView(1, 6); // Elements 3-8 of original

            var listEnum = list.GetEnumerator();
            var subList1Enum = subList1.GetEnumerator();
            var subList2Enum = subList2.GetEnumerator();

            // Move all enumerators to first position
            listEnum.MoveNext();
            subList1Enum.MoveNext();
            subList2Enum.MoveNext();

            // Act - EnsureCapacity at various levels
            list.EnsureCapacity(100);
            Assert.DoesNotThrow(() => subList2Enum.MoveNext());

            subList1.EnsureCapacity(50);
            Assert.DoesNotThrow(() => subList2Enum.MoveNext());

            subList2.EnsureCapacity(30);
            Assert.DoesNotThrow(() => listEnum.MoveNext());
            Assert.DoesNotThrow(() => subList1Enum.MoveNext());

            // Assert - All enumerators should still be valid
            Assert.AreEqual(2, listEnum.Current);
            Assert.AreEqual(3, subList1Enum.Current);
            Assert.AreEqual(5, subList2Enum.Current);
        }

        [Test]
        public void Test_Multiple_SubLists_With_EnsureCapacity()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var subList1 = list.GetView(0, 5); // Elements 1-5
            var subList2 = list.GetView(5, 5); // Elements 6-10

            var subList1Enum = subList1.GetEnumerator();
            var subList2Enum = subList2.GetEnumerator();

            subList1Enum.MoveNext();
            subList2Enum.MoveNext();

            // Act - EnsureCapacity on parent
            list.EnsureCapacity(100);

            // Assert - Both sublist enumerators should still work
            Assert.DoesNotThrow(() => subList1Enum.MoveNext());
            Assert.DoesNotThrow(() => subList2Enum.MoveNext());

            Assert.AreEqual(2, subList1Enum.Current);
            Assert.AreEqual(7, subList2Enum.Current);

            // Act - EnsureCapacity on one sublist
            subList1.EnsureCapacity(50);

            // Assert - Other sublist enumerator should still work
            Assert.DoesNotThrow(() => subList2Enum.MoveNext());
            Assert.AreEqual(8, subList2Enum.Current);
        }

        [Test]
        public void Test_TrimExcess_DoesNot_Invalidate_SubList()
        {
            // Arrange
            var list = new List<int>(100) { 1, 2, 3, 4, 5 };
            var subList = list.GetView(1, 3);

            // Act - TrimExcess on parent list
            list.TrimExcess();

            // Assert - SubList operations should still work
            Assert.DoesNotThrow(() => subList.Add(6));
            Assert.AreEqual(4, subList.Count);
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void Test_SubList_TrimExcess_DoesNot_Invalidate_Parent()
        {
            // Arrange
            var list = new List<int>(100) { 1, 2, 3, 4, 5 };
            var subList = list.GetView(1, 3);

            // Act - TrimExcess on sublist
            subList.TrimExcess();

            // Assert - Parent list operations should still work
            Assert.DoesNotThrow(() => list.Add(6));
            Assert.AreEqual(6, list.Count);
        }

        [Test]
        public void Test_EnsureCapacity_With_Concurrent_Iteration()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var subList = list.GetView(1, 3);

            var listEnumerator = list.GetEnumerator();
            var subListEnumerator = subList.GetEnumerator();

            // Move both enumerators to first position
            listEnumerator.MoveNext();
            subListEnumerator.MoveNext();

            // Act - Various capacity operations
            list.EnsureCapacity(50);
            subList.EnsureCapacity(30);
            list.TrimExcess();
            subList.TrimExcess();

            // Assert - Both enumerators should still be valid
            Assert.AreEqual(1, listEnumerator.Current);
            Assert.AreEqual(2, subListEnumerator.Current);

            Assert.DoesNotThrow(() => listEnumerator.MoveNext());
            Assert.DoesNotThrow(() => subListEnumerator.MoveNext());

            Assert.AreEqual(2, listEnumerator.Current);
            Assert.AreEqual(3, subListEnumerator.Current);
        }

        [Test]
        public void Test_Structural_Modification_Still_Throws_After_EnsureCapacity()
        {
            // This test verifies that actual structural modifications still throw
            // even after EnsureCapacity has been called

            var list = new List<int> { 1, 2, 3, 4, 5 };
            var subList = list.GetView(1, 3);
            var subListEnum = subList.GetEnumerator();

            // EnsureCapacity should not affect version
            list.EnsureCapacity(50);

            // Enumerator should still work after EnsureCapacity
            Assert.DoesNotThrow(() => subListEnum.MoveNext());

            // But structural modification should invalidate the enumerator
            list.Add(6);
            Assert.Throws<InvalidOperationException>(() => subListEnum.MoveNext());

            // Reset for second test
            list = new List<int> { 1, 2, 3, 4, 5 };
            subList = list.GetView(1, 3);
            var listEnum = list.GetEnumerator();

            // EnsureCapacity on sublist should not affect version
            subList.EnsureCapacity(30);

            // Enumerator should still work after EnsureCapacity
            Assert.DoesNotThrow(() => listEnum.MoveNext());

            // But structural modification should invalidate the enumerator
            subList.Add(6);
            Assert.Throws<InvalidOperationException>(() => listEnum.MoveNext());
        }

        [Test]
        public void Test_Add_To_SubList_After_Parent_EnsureCapacity()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var subList = list.GetView(1, 3); // Elements 2, 3, 4

            // Act - EnsureCapacity on parent, then add to sublist
            list.EnsureCapacity(100);
            subList.Add(10);
            subList.Add(20);

            // Assert
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual(5, subList.Count);
            Assert.AreEqual(new List<int> { 1, 2, 3, 4, 10, 20, 5 }, list);
            Assert.AreEqual(new List<int> { 2, 3, 4, 10, 20 }, subList);
        }

        [Test]
        public void Test_Add_To_Parent_After_SubList_EnsureCapacity()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var subList = list.GetView(1, 3); // Elements 2, 3, 4

            // Act - EnsureCapacity on sublist, then add to parent
            subList.EnsureCapacity(50);
            list.Add(10);
            list.Add(20);

            // Assert - parent list
            Assert.AreEqual(7, list.Count);
            var listArray = list.ToArray();
            Assert.AreEqual(new int[] { 1, 2, 3, 4, 5, 10, 20 }, listArray);

            // SubList remains valid after parent structural modifications
            Assert.AreEqual(3, subList.Count);
        }

        [Test]
        public void Test_Mixed_Operations_After_EnsureCapacity()
        {
            // This test verifies that after EnsureCapacity, the first structural
            // modification works but then invalidates other views

            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            var subList1 = list.GetView(1, 3); // Elements 2, 3, 4
            var subList2 = list.GetView(4, 3); // Elements 5, 6, 7

            // Act - EnsureCapacity doesn't invalidate views
            list.EnsureCapacity(50);
            Assert.DoesNotThrow(() => subList1.Count.ToString());
            Assert.DoesNotThrow(() => subList2.Count.ToString());

            // First structural modification works
            subList1.Add(100); // Adds after element 4
            Assert.AreEqual(new List<int> { 1, 2, 3, 4, 100, 5, 6, 7, 8 }, list);
            Assert.AreEqual(4, subList1.Count);

            // But now subList2 is invalid due to subList1's modification
            Assert.Throws<InvalidOperationException>(() => subList2.Add(200));
        }

        [Test]
        public void Test_Insert_After_EnsureCapacity()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var subList = list.GetView(1, 3);

            // Act - EnsureCapacity doesn't invalidate
            list.EnsureCapacity(100);
            Assert.DoesNotThrow(() => subList.Count.ToString());

            // Insert in sublist works
            subList.Insert(1, 99); // Insert at position 1 of sublist
            Assert.AreEqual(new List<int> { 1, 2, 99, 3, 4, 5 }, list);
            Assert.AreEqual(4, subList.Count);

            // But now parent list operations on the sublist would throw
            // We can still modify the parent directly though
            list.Insert(0, 88);
            Assert.AreEqual(new List<int> { 88, 1, 2, 99, 3, 4, 5 }, list);

            // SubList remains valid after parent structural modifications
            Assert.AreEqual(4, subList.Count);
        }

        [Test]
        public void Test_AddRange_After_EnsureCapacity()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var subList = list.GetView(2, 2); // Elements 3, 4

            // Act - EnsureCapacity doesn't invalidate
            list.EnsureCapacity(100);
            Assert.DoesNotThrow(() => subList.Count.ToString());

            // AddRange to sublist works
            subList.AddRange(new[] { 10, 20, 30 });
            Assert.AreEqual(5, subList.Count);
            Assert.AreEqual(new List<int> { 1, 2, 3, 4, 10, 20, 30, 5 }, list);

            // Parent can still be modified directly
            list.AddRange(new[] { 40, 50 });
            Assert.AreEqual(new List<int> { 1, 2, 3, 4, 10, 20, 30, 5, 40, 50 }, list);

            // SubList remains valid after parent structural modifications
            Assert.AreEqual(5, subList.Count);
        }

        [Test]
        public void Test_Nested_SubList_Operations_After_EnsureCapacity()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var subList1 = list.GetView(2, 6); // Elements 3-8
            var subList2 = subList1.GetView(1, 4); // Elements 4-7 of original

            // Act - Multiple EnsureCapacity calls followed by adds
            list.EnsureCapacity(100);
            subList2.Add(100);
            Assert.AreEqual(5, subList2.Count);
            Assert.AreEqual(7, subList1.Count);
            Assert.AreEqual(11, list.Count);

            subList1.EnsureCapacity(50);
            subList1.Add(200);
            Assert.AreEqual(5, subList2.Count); // unchanged
            Assert.AreEqual(8, subList1.Count);
            Assert.AreEqual(12, list.Count);
            
            // Editing sublist1 breaks sublist2
            Assert.Throws<InvalidOperationException>(() => subList2.EnsureCapacity(25));
            list.Add(300);
            Assert.AreEqual(5, subList2.Count); // unchanged
            Assert.AreEqual(8, subList1.Count); // unchanged
            Assert.AreEqual(13, list.Count);

            // Assert final list state
            var expected = new List<int> { 1, 2, 3, 4, 5, 6, 7, 100, 8, 200, 9, 10, 300 };
            Assert.AreEqual(expected, list);
        }

        [Test]
        public void Test_Multiple_SubLists_Add_After_EnsureCapacity()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            var subList1 = list.GetView(0, 4); // Elements 1-4
            var subList2 = list.GetView(4, 4); // Elements 5-8

            // Act - EnsureCapacity doesn't invalidate views
            list.EnsureCapacity(100);
            Assert.DoesNotThrow(() => subList1.Count.ToString());
            Assert.DoesNotThrow(() => subList2.Count.ToString());

            // First add works
            subList1.Add(10);
            Assert.AreEqual(5, subList1.Count);
            Assert.AreEqual(new List<int> { 1, 2, 3, 4, 10, 5, 6, 7, 8 }, list);

            // But now subList2 is invalid due to subList1's modification
            Assert.Throws<InvalidOperationException>(() => subList2.Add(20));
        }

        [Test]
        public void Test_Large_Capacity_Then_Many_Adds()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };
            var subList = list.GetView(1, 2);

            // Act - Large capacity increase doesn't invalidate
            list.EnsureCapacity(1000);
            Assert.DoesNotThrow(() => subList.Count.ToString());

            // Add many items to sublist
            for (int i = 0; i < 100; i++)
            {
                subList.Add(100 + i);
            }

            Assert.AreEqual(102, subList.Count);
            Assert.AreEqual(2, subList[0]);
            Assert.AreEqual(3, subList[1]);
            Assert.AreEqual(100, subList[2]);
            Assert.AreEqual(199, subList[101]);
            Assert.AreEqual(103, list.Count);

            // After sublist modification, we can still modify parent directly
            for (int i = 0; i < 50; i++)
            {
                list.Add(1000 + i);
            }

            Assert.AreEqual(153, list.Count);

            // But sublist is now invalid
            Assert.Throws<InvalidOperationException>(() => subList[0].ToString());
        }

        [Test]
        public void Test_TrimExcess_Then_Add_Operations()
        {
            // Arrange
            var list = new List<int>(100) { 1, 2, 3, 4, 5 };
            var subList = list.GetView(1, 3);

            // Act - TrimExcess doesn't invalidate
            list.TrimExcess();
            Assert.DoesNotThrow(() => subList.Count.ToString());

            // Add to sublist works
            subList.Add(10);
            Assert.AreEqual(4, subList.Count);
            Assert.AreEqual(new List<int> { 1, 2, 3, 4, 10, 5 }, list);

            // Add to parent still works directly
            list.Add(20);
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual(new List<int> { 1, 2, 3, 4, 10, 5, 20 }, list);

            // SubList remains valid after parent structural modifications
            Assert.AreEqual(4, subList.Count);
        }
    }
}