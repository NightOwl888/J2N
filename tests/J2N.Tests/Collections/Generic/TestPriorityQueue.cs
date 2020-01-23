// This class was sourced from the Apache Harmony project
// https://svn.apache.org/repos/asf/harmony/enhanced/java/trunk/

using J2N.Util;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace J2N.Collections.Generic
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    public class TestPriorityQueue : TestCase
    {

        /// <summary>
        /// @tests java.util.PriorityQueue#iterator()
        /// </summary>
        [Test]
        public void Test_Iterator()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            int[] array = { 2, 45, 7, -12, 9 };
            for (int i = 0; i < array.Length; i++)
            {
                integerQueue.Enqueue(array[i]);
            }
            List<Integer> iterResult = new List<Integer>();
            using (IEnumerator<Integer> iter = integerQueue.GetEnumerator())
            {
                assertNotNull(iter);

                while (iter.MoveNext())
                {
                    iterResult.Add(iter.Current);
                }
            }
            var resultArray = iterResult.ToArray();
            Array.Sort(array);
            Array.Sort(resultArray);
            assertTrue(ArraysEquals(array, resultArray));
        }

        [Test]
        public void Test_Iterator_Empty()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            IEnumerator<Integer> iter;
            using (iter = integerQueue.GetEnumerator())
            {
                assertFalse(iter.MoveNext());
            }
            // J2N: Remove not supported in .NET
            //using (iter = integerQueue.GetEnumerator())
            //{
            //    try
            //    {
            //        iter.remove();
            //        fail("should throw IllegalStateException");
            //    }
            //    catch (IllegalStateException e)
            //    {
            //        // expected
            //    }
            //}
        }

        [Test]
        public void Test_Iterator_Outofbound()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            integerQueue.Enqueue(0);
            IEnumerator<Integer> iter;

            using (iter = integerQueue.GetEnumerator())
            {
                iter.MoveNext();
                assertFalse(iter.MoveNext());
            }
            // J2N: Remove not supported in .NET
            //using (iter = integerQueue.GetEnumerator())
            //{
            //    iter.MoveNext();
            //    iter.remove();
            //    try
            //    {
            //        iter.next();
            //        fail("should throw NoSuchElementException");
            //    }
            //    catch (NoSuchElementException e)
            //    {
            //        // expected
            //    }
            //}
        }

        // Iterator Remove methods omitted...

        [Test]
        public void Test_Size()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            assertEquals(0, integerQueue.Count);
            int[] array = { 2, 45, 7, -12, 9 };
            for (int i = 0; i < array.Length; i++)
            {
                integerQueue.Enqueue(array[i]);
            }
            assertEquals(array.Length, integerQueue.Count);
        }

        [Test]
        public void Test_Constructor()
        {
            PriorityQueue<object> queue = new PriorityQueue<object>();
            assertNotNull(queue);
            assertEquals(0, queue.Count);
            assertNull(queue.Comparer);
        }

        [Test]
        public void Test_ConstructorI()
        {
            PriorityQueue<object> queue = new PriorityQueue<object>(100);
            assertNotNull(queue);
            assertEquals(0, queue.Count);
            assertNull(queue.Comparer);
        }

        [Test]
        public void Test_ConstructorILComparer()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>(100,
                    (IComparer<object>)null);
            assertNotNull(queue);
            assertEquals(0, queue.Count);
            assertNull(queue.Comparer);

            MockComparer<Object> comparator = new MockComparer<Object>();
            queue = new PriorityQueue<Object>(100, comparator);
            assertNotNull(queue);
            assertEquals(0, queue.Count);
            assertEquals(comparator, queue.Comparer);
        }

        [Test]
        public void Test_ConstructorILComparer_illegalCapacity()
        {
            try
            {
                new PriorityQueue<Object>(0, new MockComparer<Object>());
                fail("should throw ArgumentException");
            }
#pragma warning disable 168
            catch (ArgumentException e)
#pragma warning restore 168
            {
                // expected
            }

            try
            {
                new PriorityQueue<Object>(-1, new MockComparer<Object>());
                fail("should throw ArgumentException");
            }
#pragma warning disable 168
            catch (ArgumentException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_ConstructorILComparer_cast()
        {
            MockComparerCast<Integer> objectComparator = new MockComparerCast<Integer>();
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>(100,
                    objectComparator);
            assertNotNull(integerQueue);
            assertEquals(0, integerQueue.Count);
            assertEquals(objectComparator, integerQueue.Comparer);
            Integer[] array = { 2, 45, 7, -12, 9 };
            integerQueue.AddRange(array);
            assertEquals(array.Length, integerQueue.Count);
            // just test here no cast exception raises.
        }

        [Test]
        public void Test_ConstructorLCollection()
        {
            int[] array = { 2, 45, 7, -12, 9 };
            List<Integer> list = CreateIntegerList(array);
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>(list);
            assertEquals(array.Length, integerQueue.Count);
            assertNull(integerQueue.Comparer);
            Array.Sort(array);
            for (int i = 0; i < array.Length; i++)
            {
                assertTrue(integerQueue.TryDequeue(out Integer result));
                assertEquals(array[i], result);
            }
        }

        [Test]
        public void Test_ConstructorLColleciton_null()
        {
            List<Object> list = new List<Object>();
            list.Add(new float?(11));
            list.Add(null);
            list.Add(new Integer(10));
            try
            {
                new PriorityQueue<Object>(list);
                fail("should throw ArgumentNullException");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_ConstructorLColleciton_non_comparable()
        {
            List<Object> list = new List<Object>();
            list.Add(new float?(11));
            list.Add(new Integer(10));
            try
            {
                new PriorityQueue<Object>(list);
                fail("should throw InvalidOperationException");
            }
#pragma warning disable 168
            catch (InvalidOperationException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_ConstructorLColleciton_from_priorityqueue()
        {
            String[] array = { "AAAAA", "AA", "AAAA", "AAAAAAAA" };
            PriorityQueue<String> queue = new PriorityQueue<String>(4,
                    new MockComparerStringByLength());
            for (int i = 0; i < array.Length; i++)
            {
                queue.Enqueue(array[i]);
            }
            ICollection<string> c = queue;
            PriorityQueue<String> constructedQueue = new PriorityQueue<String>(c);
            assertEquals(queue.Comparer, constructedQueue.Comparer);
            while (queue.Count > 0)
            {
                assertEquals(queue.Dequeue(), constructedQueue.Dequeue());
            }
            assertEquals(0, constructedQueue.Count);
        }

        [Test]
        public void Test_ConstructorLCollection_from_sortedset()
        {
            int[] array = { 3, 5, 79, -17, 5 };
            SortedSet<Integer> treeSet = new SortedSet<Integer>(new MockComparer<Integer>());
            for (int i = 0; i < array.Length; i++)
            {
                treeSet.Add(array[i]);
            }
            ICollection<Integer> c = treeSet;
            PriorityQueue<Integer> queue = new PriorityQueue<Integer>(c);
            assertEquals(treeSet.Comparer, queue.Comparer);
            IEnumerator<Integer> iter = treeSet.GetEnumerator();
            while (iter.MoveNext())
            {
                assertEquals(iter.Current, queue.Dequeue());
            }
            assertEquals(0, queue.Count);
        }

        [Test]
        public void Test_ConstructorLPriorityQueue()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            int[] array = { 2, 45, 7, -12, 9 };
            for (int i = 0; i < array.Length; i++)
            {
                integerQueue.Enqueue(array[i]);
            }
            // Can't cast int > object in .NET
            PriorityQueue<Integer> objectQueue = new PriorityQueue<Integer>(
                    integerQueue);
            assertEquals(integerQueue.Count, objectQueue.Count);
            assertEquals(integerQueue.Comparer, objectQueue.Comparer);
            Array.Sort(array);
            for (int i = 0; i < array.Length; i++)
            {
                assertEquals(array[i], objectQueue.Dequeue());
            }
        }

        [Test]
        public void Test_ConstructorLPriorityQueue_null()
        {
            try
            {
                new PriorityQueue<Integer>((PriorityQueue<Integer>)null);
                fail("should throw ArgumentNullException");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_ConstructorLSortedSet()
        {
            int[] array = { 3, 5, 79, -17, 5 };
            SortedSet<Integer> treeSet = new SortedSet<Integer>();
            for (int i = 0; i < array.Length; i++)
            {
                treeSet.Add(array[i]);
            }
            PriorityQueue<Integer> queue = new PriorityQueue<Integer>(treeSet);
            var iter = treeSet.GetEnumerator();
            while (iter.MoveNext())
            {
                assertEquals(iter.Current, queue.Dequeue());
            }
        }

        [Test]
        public void Test_ConstructorLSortedSet_null()
        {
            try
            {
                new PriorityQueue<Integer>((SortedSet<Integer>)null);
                fail("should throw ArgumentNullException");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_OfferLObject()
        {
            PriorityQueue<String> queue = new PriorityQueue<String>(10,
                    new MockComparerStringByLength());
            String[] array = { "AAAAA", "AA", "AAAA", "AAAAAAAA" };
            for (int i = 0; i < array.Length; i++)
            {
                queue.Enqueue(array[i]);
            }
            String[] sortedArray = { "AA", "AAAA", "AAAAA", "AAAAAAAA" };
            for (int i = 0; i < sortedArray.Length; i++)
            {
                assertEquals(sortedArray[i], queue.Dequeue());
            }
            assertEquals(0, queue.Count);
            assertFalse(queue.TryDequeue(out string _));
        }

        [Test]
        public void Test_OfferLObject_null()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            try
            {
                queue.Enqueue(null);
                fail("should throw ArgumentNullException");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_Offer_LObject_non_Comparable()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            queue.Enqueue(new Integer(10));
            try
            {
                queue.Enqueue(new float?(1.3f));
                fail("should throw InvalidOperationException");
            }
#pragma warning disable 168
            catch (InvalidOperationException e)
#pragma warning restore 168
            {
                // expected
            }

            queue = new PriorityQueue<Object>();
            queue.Enqueue(new int?(10));
            try
            {
                queue.Enqueue(new Object());
                fail("should throw InvalidOperationException");
            }
#pragma warning disable 168
            catch (InvalidOperationException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_Poll()
        {
            PriorityQueue<String> stringQueue = new PriorityQueue<String>();
            String[] array = { "MYTESTSTRING", "AAAAA", "BCDEF", "ksTRD", "AAAAA" };
            for (int i = 0; i < array.Length; i++)
            {
                stringQueue.Enqueue(array[i]);
            }
            Array.Sort(array);
            for (int i = 0; i < array.Length; i++)
            {
                assertEquals(array[i], stringQueue.Dequeue());
            }
            assertEquals(0, stringQueue.Count);
            //assertNull(stringQueue.Dequeue());
            assertFalse(stringQueue.TryDequeue(out string _));
        }

        [Test]
        public void Test_Poll_empty()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            assertEquals(0, queue.Count);
            //assertNull(queue.Poll());
            assertFalse(queue.TryDequeue(out object _));
        }

        [Test]
        public void Test_Peek()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            int[] array = { 2, 45, 7, -12, 9 };
            for (int i = 0; i < array.Length; i++)
            {
                integerQueue.Add(array[i]);
            }
            Array.Sort(array);
            assertEquals(new Integer(array[0]), integerQueue.Peek());
            assertEquals(new Integer(array[0]), integerQueue.Peek());
        }

        [Test]
        public void Test_Peek_empty()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            assertEquals(0, queue.Count);
            //assertNull(queue.Peek());
            //assertNull(queue.Peek());
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }

        [Test]
        public void Test_TryPeek()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            int[] array = { 2, 45, 7, -12, 9 };
            for (int i = 0; i < array.Length; i++)
            {
                integerQueue.Add(array[i]);
            }
            Array.Sort(array);
            assertTrue(integerQueue.TryPeek(out Integer value1));
            assertEquals(array[0], value1);
            assertTrue(integerQueue.TryPeek(out Integer value2));
            assertEquals(array[0], value2);
        }

        [Test]
        public void Test_TryPeek_empty()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            assertEquals(0, queue.Count);
            assertFalse(queue.TryPeek(out object _));
            assertFalse(queue.TryPeek(out object _));
        }

        [Test]
        public void Test_Clear()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            int[] array = { 2, 45, 7, -12, 9 };
            for (int i = 0; i < array.Length; i++)
            {
                integerQueue.Enqueue(array[i]);
            }
            integerQueue.Clear();
            assertTrue(!integerQueue.Any());
        }


        #region Non-Generic Tests

        [Test]
        public void Test_Add_int()
        {
            PriorityQueue<int> integerQueue = new PriorityQueue<int>();
            int[] array = { 2, 45, 7, -12, 9, 0 };
            for (int i = 0; i < array.Length; i++)
            {
                integerQueue.Add(array[i]);
            }
            Array.Sort(array);
            assertEquals(array.Length, integerQueue.Count);
            for (int i = 0; i < array.Length; i++)
            {
                assertEquals(array[i], integerQueue.Dequeue());
            }
            assertEquals(0, integerQueue.Count);
        }

        [Test]
        public void Test_Remove_int()
        {
            int[] array = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 16, 39, 0 };
            List<Integer> list = CreateIntegerList(array);
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>(list);
            assertTrue(integerQueue.Remove(16));
            int[] newArray = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 39, 0 };
            Array.Sort(newArray, Comparer<int>.Default);
            for (int i = 0; i < newArray.Length; i++)
            {
                assertEquals(newArray[i], integerQueue.Dequeue());
            }
            assertEquals(0, integerQueue.Count);
        }

        [Test]
        public void Test_Remove_int_using_comparator()
        {
            PriorityQueue<int> queue = new PriorityQueue<int>(10,
                    Comparer<int>.Default);
            int[] array = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 16, 39, 0 };
            for (int i = 0; i < array.Length; i++)
            {
                queue.Enqueue(array[i]);
            }
            assertFalse(queue.Contains(75));
            assertTrue(queue.Contains(0));
            assertTrue(queue.Contains(1118));
            assertTrue(queue.Remove(0));
            assertTrue(queue.Remove(1118));
        }

        [Test]
        public void Test_Remove_LObject_not_exists()
        {
            int[] array = { 2, -12, 9, 23, 17, 1118, 0 };
            PriorityQueue<int> integerQueue = new PriorityQueue<int>(array);
            assertFalse(integerQueue.Remove(111));
            assertTrue(integerQueue.Remove(0));

            // Since 0 is the default value that is stored in the free space at the end of the queue, we need
            // to ensure removing it again isn't possible
            assertFalse(integerQueue.Remove(0));
            //assertFalse(integerQueue.Remove(""));
        }


        #endregion




        [Test]
        public void Test_Add_LObject()
        {
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>();
            int[] array = { 2, 45, 7, -12, 9 };
            for (int i = 0; i < array.Length; i++)
            {
                integerQueue.Add(array[i]);
            }
            Array.Sort(array);
            assertEquals(array.Length, integerQueue.Count);
            for (int i = 0; i < array.Length; i++)
            {
                assertEquals(array[i], integerQueue.Dequeue());
            }
            assertEquals(0, integerQueue.Count);
        }

        [Test]
        public void Test_Add_LObject_null()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            try
            {
                queue.Add(null);
                fail("should throw ArgumentNullException");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_Add_LObject_non_Comparable()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            queue.Add(new int?(10));
            try
            {
                queue.Add(new float?(1.3f));
                fail("should throw InvalidOperationException");
            }
#pragma warning disable 168
            catch (InvalidOperationException e)
#pragma warning restore 168
            {
                // expected
            }

            queue = new PriorityQueue<Object>();
            queue.Add(new int?(10));
            try
            {
                queue.Add(new Object());
                fail("should throw InvalidOperationException");
            }
#pragma warning disable 168
            catch (InvalidOperationException e)
#pragma warning restore 168
            {
                // expected
            }
        }

        [Test]
        public void Test_Remove_LObject()
        {
            int[] array = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 16, 39 };
            List<Integer> list = CreateIntegerList(array);
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>(list);
            assertTrue(integerQueue.Remove(16));
            int[] newArray = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 39 };
            Array.Sort(newArray, Comparer<int>.Default);
            for (int i = 0; i < newArray.Length; i++)
            {
                assertEquals(newArray[i], integerQueue.Dequeue());
            }
            assertEquals(0, integerQueue.Count);
        }

        [Test]
        public void Test_Remove_LObject_using_comparator()
        {
            PriorityQueue<String> queue = new PriorityQueue<String>(10,
                    new MockComparerStringByLength());
            String[] array = { "AAAAA", "AA", "AAAA", "AAAAAAAA" };
            for (int i = 0; i < array.Length; i++)
            {
                queue.Enqueue(array[i]);
            }
            assertFalse(queue.Contains("BB"));
            assertTrue(queue.Remove("AA"));
        }

        [Test]
        public void Test_Remove_int_not_exists()
        {
            int[] array = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 16, 39 };
            List<Integer> list = CreateIntegerList(array);
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>(list);
            assertFalse(integerQueue.Remove(111));
            assertFalse(integerQueue.Remove(null));
            //assertFalse(integerQueue.Remove(""));
        }

        [Test]
        public void Test_Remove_LObject_null()
        {
            int[] array = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 16, 39 };
            List<Integer> list = CreateIntegerList(array);
            PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>(list);
            assertFalse(integerQueue.Remove(null));
        }

        [Test]
        public void Test_Remove_LObject_not_Compatible()
        {
            // J2N: Cannot remove a float from an integer queue - won't compile
            //int[] array = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 16, 39 };
            //List<Integer> list = CreateIntegerList(array);
            //PriorityQueue<Integer> integerQueue = new PriorityQueue<Integer>(list);
            //assertFalse(integerQueue.Remove(new float?(1.3F)));

            // although argument element type is not compatible with those in queue,
            // but comparator supports it.
            MockComparer<Object> comparator = new MockComparer<Object>();
            PriorityQueue<object> integerQueue1 = new PriorityQueue<object>(100,
                    comparator);
            integerQueue1.Enqueue(1);
            assertFalse(integerQueue1.Remove(new float?(1.3F)));

            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            Object o = new Object();
            queue.Enqueue(o);
            assertTrue(queue.Remove(o));
        }

        [Test]
        public void Test_Comparer()
        {
            PriorityQueue<Object> queue = new PriorityQueue<Object>();
            assertNull(queue.Comparer);

            MockComparer<Object> comparator = new MockComparer<Object>();
            queue = new PriorityQueue<Object>(100, comparator);
            assertEquals(comparator, queue.Comparer);
        }

#if FEATURE_SERIALIZABLE
        [Test]
        public void Test_Serialization()
        {
            int[] array = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 16, 39 };
            List<Integer> list = CreateIntegerList(array);
            PriorityQueue<Integer> srcIntegerQueue = new PriorityQueue<Integer>(
                list);
            //PriorityQueue<int> destIntegerQueue = (PriorityQueue<int>)SerializationTester
            //        .getDeserilizedObject(srcIntegerQueue);
            PriorityQueue<Integer> destIntegerQueue = Clone(srcIntegerQueue);
            Array.Sort(array);
            for (int i = 0; i < array.Length; i++)
            {
                assertEquals(array[i], destIntegerQueue.Dequeue());
            }
            assertEquals(0, destIntegerQueue.Count);
        }

        // J2N: This type of casting is not allowed in .NET
        //[Test]
        //public void Test_Serialization_casting()
        //{
        //    int[] array = { 2, 45, 7, -12, 9, 23, 17, 1118, 10, 16, 39 };
        //    List<int> list = Arrays.AsList(array);
        //    PriorityQueue<int> srcIntegerQueue = new PriorityQueue<int>(
        //        list);
        //    PriorityQueue<String> destStringQueue = (PriorityQueue<String>)GetDeserializedObject<object>(srcIntegerQueue);
        //    // will not incur class cast exception.
        //    Object o = destStringQueue.Peek();
        //    Array.Sort(array);
        //    int I = (int)o;
        //    assertEquals(array[0], I);
        //}

#endif

        private class MockComparer<E> : IComparer<E>
        {
            public int Compare(E object1, E object2)
            {
                int hashcode1 = object1.GetHashCode();
                int hashcode2 = object2.GetHashCode();
                if (hashcode1 > hashcode2)
                {
                    return 1;
                }
                else if (hashcode1 == hashcode2)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }

        private class MockComparerStringByLength : IComparer<string>
        {
            public int Compare(string object1, string object2)
            {
                int length1 = object1.Length;
                int length2 = object2.Length;
                if (length1 > length2)
                {
                    return 1;
                }
                else if (length1 == length2)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }

        private class MockComparerCast<E> : IComparer<E>
        {
            public int Compare(E object1, E object2)
            {
                return 0;
            }
        }

        private List<Integer> CreateIntegerList(IEnumerable<int> collection)
        {
            List<Integer> list = new List<Integer>();
            foreach (var value in collection)
            {
                list.Add(new Integer(value));
            }
            return list;
        }

        internal bool ArraysEquals(int[] expected, Integer[] actual)
        {
            int[] actual2 = new int[actual.Length];
            for (int i = 0; i < actual.Length; i++)
            {
                actual2[i] = actual[i].Value;
            }
            return Arrays.Equals(expected, actual2);
        }

        #region AbstractQueue Tests

        private PriorityQueue<Object> queue;

        private class MockPriorityQueue<E> : PriorityQueue<E>
        {

            internal const int CAPACITY = 10;

            private int size = 0;

            private Object[] elements = new Object[CAPACITY];

            public override IEnumerator<E> GetEnumerator()
            {
                return new Enumerator<E>(this);
            }


            public override int Count => size;


            public override bool Enqueue(E item)
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item));

                if (size >= CAPACITY)
                {
                    return false;
                }

                elements[size++] = item;
                return true;
            }

            public override E Dequeue()
            {
                if (size == 0)
                {
                    throw new InvalidOperationException();
                }
                E e = (E)elements[0];
                for (int i = 0; i < size - 1; i++)
                {
                    elements[i] = elements[i + 1];
                }
                size--;
                return e;
            }

            public override bool TryDequeue(out E result)
            {
                if (size == 0)
                {
                    result = default;
                    return false;
                }
                E e = (E)elements[0];
                for (int i = 0; i < size - 1; i++)
                {
                    elements[i] = elements[i + 1];
                }
                size--;
                result = e;
                return true;
            }

            public override E Peek()
            {
                if (size == 0)
                {
                    throw new InvalidOperationException();
                }
                return (E)elements[0];
            }

            public override bool TryPeek(out E result)
            {
                if (size == 0)
                {
                    result = default;
                    return false;
                }
                result = (E)elements[0];
                return true;
            }

            public override bool Contains(E item)
            {
                return IndexOf(item) != -1;
            }

            private int IndexOf(E o)
            {
                for (int i = 0; i < size; i++)
                    if (o.Equals(elements[i]))
                        return i;
                return -1;
            }

            public override void Clear()
            {
                base.Clear();
                elements.Fill(default);
                size = 0;
            }


            private struct Enumerator<T1> : IEnumerator<T1> where T1 : E
            {
                private int currentIndex;
                private T1 current;
                private readonly MockPriorityQueue<T1> queue;
                public Enumerator(MockPriorityQueue<T1> queue)
                {
                    this.queue = queue ?? throw new ArgumentNullException(nameof(queue));
                    currentIndex = -1;
                    current = default;
                }


                public T1 Current => current;

                object IEnumerator.Current => current;

                public void Dispose()
                {
                    current = default;
                }

                private bool HasNext => queue.size > 0 && currentIndex < queue.size;

                public bool MoveNext()
                {
                    if (HasNext)
                    {
                        currentIndex++;
                        current = (T1)queue.elements[currentIndex];
                        return true;
                    }
                    current = default;
                    return false;
                }

                public void Reset()
                {
                    throw new NotSupportedException();
                }
            }


        }



        /**
         * @tests java.util.AbstractQueue.add(E)
         */
        [Test]
        public void Test_addLE_null()
        {
            try
            {
                queue.Add(null);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        /**
         * @tests java.util.AbstractQueue.add(E)
         */
        [Test]
        public void Test_addLE_Full()
        {
            Object o = new Object();

            for (int i = 0; i < MockPriorityQueue<object>.CAPACITY; i++)
            {
                queue.Add(o);
            }

            try
            {
                queue.Add(o);
                fail("should throw IllegalStateException");
            }
            catch (InvalidOperationException e)
            {
                //expected
            }
        }

        /**
         * @tests java.util.AbstractQueue#add(E)
         */
        [Test]
        public void Test_addLE()
        {
            Object o = new Object();
            const int LAST_INDEX = 4;
            for (int i = 0; i < LAST_INDEX; i++)
            {
                queue.Add(o);
            }
            Integer I = new Integer(123456);
            queue.Add(I);
            assertTrue(queue.Contains(I));
            using (var iter = queue.GetEnumerator())
            {
                for (int i = 0; i < LAST_INDEX; i++)
                {
                    iter.MoveNext();
                }
                assertTrue(iter.MoveNext());
                assertTrue(I == iter.Current);
            }
        }

        /**
         * @tests java.util.AbstractQueue#addAll(E)
         */
        [Test]
        public void Test_addAllLE_null()
        {
            try
            {
                queue.AddRange(null);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        /**
         * @tests java.util.AbstractQueue#addAll(E)
         */
        [Test]
        public void Test_addAllLE_with_null()
        {
            IList<object> list = new object[] { "MYTESTSTRING", null, new float?(123.456f) };
            try
            {
                queue.AddRange(list);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        /**
         * @tests java.util.AbstractQueue#addAll(E)
         */
        [Test]
        public void Test_addAllLE_full()
        {
            IList<object> list = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            try
            {
                queue.AddRange(list);
                fail("should throw InvalidOperationException");
            }
            catch (InvalidOperationException e)
            {
                // expected
            }
        }

        /**
         * @tests java.util.AbstractQueue#addAll(E)
         */
        [Test]
        public void Test_addAllLE_empty()
        {
            // Regression test for HARMONY-1178
            IList<object> list = new List<Object>(0);
            assertFalse("Non modification to queue should return false", queue.AddRange(list));
        }

        /**
         * @tests java.util.AbstractQueue#addAll(E)
         */
        [Test]
        public void Test_addAllLE_this()
        {
            try
            {
                queue.AddRange(queue);
                fail("should throw ArgumentException ");
            }
            catch (ArgumentException e)
            {
                // expected
            }
        }

        /**
         * @tests java.util.AbstractQueue#clear()
         */
        [Test]
        public void Test_clear_empty()
        {
            queue.Clear();
            assertTrue(queue.Count == 0);
            assertFalse(queue.TryPeek(out object _));
            //assertNull(queue.peek());
        }

        /**
         * @tests java.util.AbstractQueue#clear()
         */
        [Test]
        public void Test_clear()
        {
            IList<object> list = new object[] { 123.456, "MYTESTSTRING", new Object(), 'c' };
            queue.AddRange(list);
            queue.Clear();
            assertTrue(queue.Count == 0);
            assertFalse(queue.TryPeek(out object _));
            //assertNull(queue.peek());
        }

        /**
         * @tests java.util.AbstractQueue#AbstractQueue()
         */
        [Test]
        public void Test_Constructor_MockPriorityQueue()
        {
            MockPriorityQueue<object> queue = new MockPriorityQueue<object>();
            assertNotNull(queue);
        }

        /**
         * @tests java.util.AbstractQueue#remove()
         */
        [Test]
        public void Test_remove_null()
        {
            try
            {
                queue.Dequeue();
                fail("should throw InvalidOperationException");
            }
            catch (InvalidOperationException e)
            {
                // expected
            }

        }

        /**
         * @tests java.util.AbstractQueue#remove()
         */
        [Test]
        public void Test_remove()
        {
            char c = 'a';
            queue.Add(c);
            c = 'b';
            queue.Add(c);
            assertEquals('a', queue.Dequeue());
            assertEquals('b', queue.Dequeue());
            try
            {
                queue.Dequeue();
                fail("should throw InvalidOperationException");
            }
            catch (InvalidOperationException e)
            {
                // expected
            }
        }

        /**
         * @tests java.util.AbstractQueue#element()
         */
        [Test]
        public void Test_element_empty()
        {
            try
            {
                queue.Peek();
                fail("should throw InvalidOperationException");
            }
            catch (InvalidOperationException e)
            {
                // expected
            }
        }

        /**
         * @tests java.util.AbstractQueue#element()
         */
        [Test]
        public void Test_element()
        {
            String s = "MYTESTSTRING_ONE";
            queue.Add(s);
            s = "MYTESTSTRING_TWO";
            queue.Add(s);
            assertEquals("MYTESTSTRING_ONE", queue.Peek());
            // still the first element
            assertEquals("MYTESTSTRING_ONE", queue.Peek());
        }

        public override void SetUp()
        {
            base.SetUp();
            queue = new MockPriorityQueue<Object>();
        }

        public override void TearDown()
        {
            base.TearDown();
            queue = null;
        }

        #endregion
    }
}
