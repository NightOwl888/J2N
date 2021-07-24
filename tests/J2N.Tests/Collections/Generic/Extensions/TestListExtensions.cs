using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic.Extensions
{
    public class TestListExtensions : TestCase
    {
        IList<object> ll;

        IList<IComparable<object>> myReversedLinkedList;

        static object[] objArray = LoadObjArray();
        static IComparable<object>[] myobjArray = LoadMyObjArray();

        private static object[] LoadObjArray()
        {
            var objArray = new object[1000];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray[i] = i;
            }
            return objArray;
        }

        private static IComparable<object>[] LoadMyObjArray()
        {
            var myobjArray = new IComparable<object>[1000];
            for (int i = 0; i < objArray.Length; i++)
            {
                myobjArray[i] = new MyInt(i);
            }
            return myobjArray;
        }

        

        public class ReversedMyIntComparator : IComparer, IComparer<object>
        {

            public int Compare(Object o1, Object o2)
            {
                return -((MyInt)o1).CompareTo((MyInt)o2);
            }

            new public static int Equals(Object o1, Object o2)
            {
                return ((MyInt)o1).CompareTo((MyInt)o2);
            }
        }

        internal class MyInt : IComparable<object>
        {
            internal int data;

            public MyInt(int value)
            {
                data = value;
            }

            public int CompareTo(object obj)
            {
                return data > ((MyInt)obj).data ? 1 : (data < ((MyInt)obj).data ? -1 : 0);
            }
        }

        /**
         * @tests java.util.Collections#binarySearch(java.util.List,
         *        java.lang.Object)
         */
        [Test]
        public void Test_binarySearchLjava_util_ListLjava_lang_Object()
        {
            // Test for method int
            // java.util.Collections.binarySearch(java.util.List, java.lang.Object)
            // assumes ll is sorted and has no duplicate keys
            int llSize = ll.Count;
            // Ensure a NPE is thrown if the list is NULL
            IList<IComparable<object>> list = null;
            try
            {
                list.BinarySearch(new MyInt(3));
                fail("Expected NullPointerException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
                //Expected
            }
            for (int counter = 0; counter < llSize; counter++)
            {
                assertEquals("Returned incorrect binary search item position", ll[counter], ll[ll.BinarySearch(ll[counter])]);
            }
        }

        /**
         * @tests java.util.Collections#binarySearch(java.util.List,
         *        java.lang.Object, java.util.Comparator)
         */
        [Test]
        public void Test_binarySearchLSystem_Collections_Generic_IListLSystem_ObjectLSystem_Collections_Generic_IComparer()
        {
            // Test for method int
            // java.util.Collections.binarySearch(java.util.List, java.lang.Object,
            // java.util.Comparator)
            // assumes reversedLinkedList is sorted in reversed order and has no
            // duplicate keys
            int rSize = myReversedLinkedList.Count;
            ReversedMyIntComparator comp = new ReversedMyIntComparator();
            // Ensure a NPE is thrown if the list is NULL
            IList<IComparable<object>> list = null;
            try
            {
                //Collections.binarySearch(null, new Object(), comp);
                list.BinarySearch(new MyInt(3), comp);
                fail("Expected NullPointerException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
                //Expected
            }
            for (int counter = 0; counter < rSize; counter++)
            {
                assertEquals(
                        "Returned incorrect binary search item position using custom comparator",
                        myReversedLinkedList[counter], myReversedLinkedList[myReversedLinkedList.BinarySearch(myReversedLinkedList[counter], comp)]);
            }
        }

        [Test]
        public void TestBinarySearch()
        {
            IList<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            assertEquals(6, list.BinarySearch(7));
            assertEquals(6, list.BinarySearch(7, null));
        }

        [Test]
        public void TestBinarySearchInRange()
        {
            IList<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            assertEquals(6, list.BinarySearch(3, 4, 7, null));
        }

        /**
        * @tests java.lang.System#arraycopy(java.lang.Object, int,
        *        java.lang.Object, int, int)
        */
        [Test]
        public void Test_CopyTo_List_Int32_List_Int32_Int32()
        {
            // Test for method void java.lang.System.arraycopy(java.lang.Object,
            // int, java.lang.Object, int, int)
            int[] a = new int[20];
            int[] b = new int[20];
            int i = 0;
            while (i < a.Length)
            {
                a[i] = i;
                ++i;
            }
            a.CopyTo(0, b, 0, a.Length);
            //System.arraycopy(a, 0, b, 0, a.length);
            for (i = 0; i < a.Length; i++)
                assertTrue("Copied elements incorrectly", a[i].Equals(b[i]));

            /* Non primitive array types don't need to be identical */
            String[] source1 = new String[] { "element1" };
            Object[] dest1 = new Object[1];
            //System.arraycopy(source1, 0, dest1, 0, dest1.length);
            source1.CopyTo(0, dest1, 0, dest1.Length);
            assertTrue("Invalid copy 1", dest1[0].Equals(source1[0]));

            char[][] source = new char[][] { new char[] { 'H', 'e', 'l', 'l', 'o' },
                new char[] { 'W', 'o', 'r', 'l', 'd' } };
            char[][] dest = new char[2][];
            //System.arraycopy(source, 0, dest, 0, dest.Length);
            source.CopyTo(0, dest, 0, dest.Length);
            assertTrue("Invalid copy 2", dest[0] == source[0]
                    && dest[1] == source[1]);

            var list = new List<int> { 4, 5, 6, 7, 8 };
            var list2 = new List<int> { 0, 0, 0, 0, 0 };

            list.CopyTo(0, list2, 0, 4);
            for (i = 0; i < 4; i++)
                assertEquals(list[i], list2[i]);
            assertTrue(list2[4] == 0);
        }

        [Test]
        public void Test_CopyTo_Exceptions()
        {
            var source = new List<int> { 1, 5, 3, 2 };
            var dest = new int[4];

            Assert.Throws<ArgumentNullException>(() => ((IList<int>)null).CopyTo(0, dest, 0, 0));
            Assert.Throws<ArgumentNullException>(() => source.CopyTo(0, null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyTo(-1, dest, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyTo(0, dest, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyTo(0, dest, 0, -1));
            Assert.Throws<ArgumentException>(() => source.CopyTo(1, dest, 0, 4));
            Assert.DoesNotThrow(() => source.CopyTo(1, dest, 0, 3));
            Assert.Throws<ArgumentException>(() => source.CopyTo(0, dest, 1, 4));
            Assert.DoesNotThrow(() => source.CopyTo(0, dest, 1, 3));
            Assert.DoesNotThrow(() => source.CopyTo(0, dest, 0, 4));
            Assert.Throws<ArgumentException>(() => source.CopyTo(0, dest, 0, 5));
            Assert.Throws<ArgumentException>(() => source.CopyTo(4, dest, 0, 1));
        }

        /**
        * @tests java.util.Collections#shuffle(java.util.List)
        */
        [Test]
        public void Test_shuffleLjava_util_List()
        {
            // Test for method void java.util.Collections.shuffle(java.util.List)
            // Assumes ll is sorted and has no duplicate keys and is large ( > 20
            // elements)

            // test shuffling a Sequential Access List
            try
            {
                ((IList<object>)null).Shuffle();
                fail("Expected ArgumentNullException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
                //Expected
            }
            var al = new List<object>();
            al.AddRange(ll);
            testShuffle(al, "Sequential Access", false);

            // test shuffling a Random Access List
            var ll2 = new List<object>();
            ll2.AddRange(ll);
            testShuffle(ll2, "Random Access", false);
        }

        // J2N: No "Random Access" in .NET, this test is redundant.
        //[Test]
        //public void TestShuffleRandomAccessWithSeededRandom()
        //{
        //    var list = new string[] { "A", "B", "C", "D", "E", "F", "G" }.ToList();
        //    list.Shuffle(new Random(0));
        //    assertTrue(new string[] {"B", "A", "D", "C", "G", "E", "F"}.SequenceEqual(list));
        //}

        [Test]
        public void TestShuffleWithSeededRandom()
        {
            var list = new string[] { "A", "B", "C", "D", "E", "F", "G" }.ToList();
            list.Shuffle(new Randomizer(0));
            assertTrue(new string[] { "B", "A", "D", "C", "G", "E", "F" }.SequenceEqual(list));
        }

        private void testShuffle(IList<object> list, string type, bool random)
        {
            bool sorted = true;
            bool allMatch = true;
            int index = 0;
            int size = list.Count;

            if (random)
                list.Shuffle();
            else
                list.Shuffle(new Random(200));

            for (int counter = 0; counter < size - 1; counter++)
            {
                if (((int)list[counter]).CompareTo((int)list[counter + 1]) > 0)
                {
                    sorted = false;
                }
            }
            assertFalse("Shuffling sorted " + type
                    + " list resulted in sorted list (should be unlikely)", sorted);
            for (int counter = 0; counter < 20; counter++)
            {
                index = 30031 * counter % (size + 1); // 30031 is a large prime
                if (list[index] != ll[index])
                    allMatch = false;
            }
            assertFalse("Too many element positions match in shuffled " + type
                    + " list", allMatch);
        }

        /**
         * @tests java.util.Collections#shuffle(java.util.List, java.util.Random)
         */
        [Test]
        public void Test_shuffleLjava_util_ListLjava_util_Random()
        {
            // Test for method void java.util.Collections.shuffle(java.util.List,
            // java.util.Random)
            // Assumes ll is sorted and has no duplicate keys and is large ( > 20
            // elements)

            // test shuffling a Sequential Access List
            try
            {
                ((IList<object>)null).Shuffle(new Random(200));
                fail("Expected ArgumentNullException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
                //Excepted
            }
            var al = new List<object>();
            al.AddRange(ll);
            testShuffle(al, "Sequential Access", true);

            // test shuffling a Random Access List
            var ll2 = new List<object>();
            ll2.AddRange(ll);
            testShuffle(ll2, "Random Access", true);

            var l = new List<object>();
            l.Add('a');
            l.Add('b');
            l.Add('c');
            l.Shuffle(new Randomizer(12345678921L));
            assertEquals("acb", l[0].ToString() + l[1] + l[2]);
        }

        [Test]
        public void TestShuffle_IsReadOnly()
        {
            var l = Enumerable.Repeat(false, 100).ToList().AsReadOnly();

            Assert.Throws<NotSupportedException>(() => l.Shuffle());
            Assert.Throws<NotSupportedException>(() => l.Shuffle(Random));
        }


        /**
         * @tests java.util.Collections#swap(java.util.List, int, int)
         */
        [Test]
        public void Test_swapLjava_util_ListII()
        {
            // Test for method swap(java.util.List, int, int)

            var smallList = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                smallList.Add(objArray[i]);
            }

            // test exception cases
            try
            {
                smallList.Swap(-1, 6);
                fail("Expected ArgumentOutOfRangeException for -1");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Expected
            }

            try
            {
                smallList.Swap(6, -1);
                fail("Expected ArgumentOutOfRangeException for -1");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Expected
            }

            try
            {
                smallList.Swap(6, 10); // J2N: We need to test against Count, not Count + 1, since that is the minimum error case
                fail("Expected ArgumentOutOfRangeException for 10");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Expected
            }

            try
            {
                smallList.Swap(10, 6); // J2N: We need to test against Count, not Count + 1, since that is the minimum error case
                fail("Expected ArgumentOutOfRangeException for 10");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Expected
            }

            // Ensure a NPE is thrown if the list is NULL
            try
            {
                ((IList<object>)null).Swap(1, 1);
                fail("Expected ArgumentNullException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
                //Expected
            }

            // test with valid parameters
            smallList.Swap(4, 7);
            assertEquals("Didn't Swap the element at position 4 ", new int?(7),
                    smallList[4]);
            assertEquals("Didn't Swap the element at position 7 ", new int?(4),
                    smallList[7]);

            // make sure other elements didn't get swapped by mistake
            for (int i = 0; i < 10; i++)
            {
                if (i != 4 && i != 7)
                    assertEquals("shouldn't have swapped the element at position "
                            + i, new int?(i), smallList[i]);
            }
        }

        [Test]
        public void TestSwap_JDK7()
        {
            int size = 100;

            var l = Enumerable.Repeat(false, 100).ToList();
            l[0] = true;
            for (int i = 0; i < size - 1; i++)
                l.Swap(i, i + 1);

            var l2 = Enumerable.Repeat(false, 100).ToList();
            l2[size - 1] = true;
            assertTrue(l.SequenceEqual(l2));
        }

        [Test]
        public void TestSwap_IsReadOnly()
        {
            var l = Enumerable.Repeat(false, 100).ToList().AsReadOnly();

            Assert.Throws<NotSupportedException>(() => l.Swap(0, 1));
        }

        [Test]
        public void TestRemoveAll_Predicate()
        {
            var toRemove = new HashSet<int> { 5, 10, 15, 20, 25, 30, 35, 40, 45 };
            var range = Enumerable.Range(1, 50);
            IList<int> actual = new SCG.List<int>(range);
            IList<int> expected = new SCG.List<int> { 1, 2, 3, 4, 6, 7, 8, 9, 11, 12, 13, 14, 16, 17, 18, 19, 21, 22, 23, 24, 26, 27, 28, 29, 31, 32, 33, 34, 36, 37, 38, 39, 41, 42, 43, 44, 46, 47, 48, 49, 50 };

            actual.RemoveAll((value) => toRemove.Contains(value));
            assertEquals(expected, actual);

            // Reset and use "unknown" implementation of IList<T>
            // This will test our slow path.
            actual = new MockList<int>(new SCG.List<int>(range));

            actual.RemoveAll((value) => toRemove.Contains(value));
            assertEquals(expected, actual);
        }

        [Test]
        public void TestRemoveAll_Int32_Int32_Predicate()
        {
            var toRemove = new HashSet<int> { 5, 10, 15, 20, 25, 30, 35, 40, 45 };
            var range = Enumerable.Range(1, 50);
            IList<int> actual = new SCG.List<int>(range);
            IList<int> expected = new SCG.List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23, 24, 26, 27, 28, 29, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            actual.RemoveAll(startIndex: 19, count: 14, (value) => toRemove.Contains(value));
            assertEquals(expected, actual);

            // Reset and use "unknown" implementation of IList<T>
            // This will test our slow path.
            actual = new MockList<int>(new SCG.List<int>(range));

            actual.RemoveAll(startIndex: 19, count: 14, (value) => toRemove.Contains(value));
            assertEquals(expected, actual);
        }

        // Just a hack so we can test an "unknown" implementation of IList<T>
        private class MockList<T> : IList<T>
        {
            private readonly IList<T> innerList;
            public MockList(IList<T> innerList)
            {
                this.innerList = innerList;
            }

            public T this[int index]
            {
                get => innerList[index];
                set => innerList[index] = value;
            }

            public int Count => innerList.Count;

            public bool IsReadOnly => innerList.IsReadOnly;

            public void Add(T item)
            {
                innerList.Add(item);
            }

            public void Clear()
            {
                innerList.Clear();
            }

            public bool Contains(T item)
            {
                return innerList.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                innerList.CopyTo(array, arrayIndex);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return innerList.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int IndexOf(T item)
            {
                return innerList .IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                innerList.Insert(index, item);
            }

            public bool Remove(T item)
            {
                return innerList.Remove(item);
            }

            public void RemoveAt(int index)
            {
                innerList.RemoveAt(index);
            }
        }


        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            base.SetUp();
            ll = new List<object>();
            //myll = new List<object>();
            //s = new HashSet<object>();
            //mys = new HashSet<object>();
            //reversedLinkedList = new LinkedList(); // to be sorted in reverse order
            myReversedLinkedList = new List<IComparable<object>>(); // to be sorted in reverse
            // order
            //hm = new Dictionary<object, object>();
            for (int i = 0; i < objArray.Length; i++)
            {
                ll.Add(objArray[i]);
                //myll.add(myobjArray[i]);
                //s.add(objArray[i]);
                //mys.add(myobjArray[i]);
                //reversedLinkedList.add(objArray[objArray.length - i - 1]);
                myReversedLinkedList.Add(myobjArray[myobjArray.Length - i - 1]);
                //hm.put(objArray[i].toString(), objArray[i]);
            }
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        public override void TearDown()
        {
        }
    }
}
