#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Collections.Generic.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using Integer = J2N.Numerics.Int32;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic
{
    public class TestList : TestCase
    {
#if FEATURE_SERIALIZABLE
        //[Test]
        //public void TestSerialize1()
        //{
        //    var intList = new List<int> { 1, 2, 3, 4, 5 };
        //    var stringList = new List<string> { "one", "two", "three", "four", "five" };

        //    var formatter = new BinaryFormatter();

        //    using (Stream stream = File.Open(@"F:\legacy-list-int.bin", FileMode.OpenOrCreate))
        //        formatter.Serialize(stream, intList);

        //    using (Stream stream = File.Open(@"F:\legacy-list-string.bin", FileMode.OpenOrCreate))
        //        formatter.Serialize(stream, stringList);
        //}

        [Test]
        public void TestDeserializeLegacy()
        {
            List<int> intList;
            List<string> stringList;

            var formatter = new BinaryFormatter();

            //using (Stream stream = File.Open(@"F:\legacy-list-int.bin", FileMode.Open))
            //    intList = (List<int>)formatter.Deserialize(stream);
            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("legacy-list-int.bin"))
                intList = (List<int>)formatter.Deserialize(stream);

            assertEquals(5, intList.Count);
            assertEquals(5, intList._version);

            //using (Stream stream = File.Open(@"F:\legacy-list-string.bin", FileMode.Open))
            //    stringList = (List<string>)formatter.Deserialize(stream);
            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("legacy-list-string.bin"))
                stringList = (List<string>)formatter.Deserialize(stream);

            assertEquals(5, stringList.Count);
            assertEquals(5, stringList._version);
        }
#endif

        [Test]
        public void TestSubList_SelfInsert()
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 10);

            subList.AddRange(subList);

            var expectedList = new List<int> {
                // Head of parent items
                1, 2,

                // SubList items
                3, 4, 5, 6, 7, 8, 9, 10, 11, 12,

                // Inserted items
                3, 4, 5, 6, 7, 8, 9, 10, 11, 12,

                // Tail of parent items
                13, 14, 15
            };

            assertEquals(expectedList, list);
        }

        [Test]
        public void TestSubList_Grandchild_SelfInsert()
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 12);
            List<int> grandchildList = subList.GetView(1, 10);

            grandchildList.AddRange(grandchildList);

            var expectedList = new List<int> {
                // Head of grandparent items
                1, 2,

                // Head of parent items
                3,

                // SubList items
                4, 5, 6, 7, 8, 9, 10, 11, 12, 13,

                // Inserted items
                4, 5, 6, 7, 8, 9, 10, 11, 12, 13,

                // Tail of parent items
                14,

                // Tail of grandparent items
                15
            };

            assertEquals(expectedList, list);
        }

        //[Test]
        //public void TestSubList()
        //{
        //    var intList = new List<int> { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        //    var subList = intList.GetView(2, 6);

        //    intList.Sort();

        //    using (var enumerator = subList.GetEnumerator())
        //    {
        //        while (enumerator.MoveNext())
        //        {
        //            var current = enumerator.Current;
        //        }
        //    }
        //}

        private static List<int> InitilizeList()
        {
            return new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        }

        private static List<int> InitilizeList(int capacity)
        {
            return new List<int>(capacity) { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        }

        // See: https://stackoverflow.com/a/8002466
        public class TupleList<T1, T2> : List<Tuple<T1, T2>>
        {
            public TupleList() { }

            public TupleList(IEnumerable<Tuple<T1, T2>> collection)
                : base(collection) { }

            public void Add(T1 item, T2 item2)
            {
                Add(new Tuple<T1, T2>(item, item2));
            }

            public TupleList<T1, T2> ConcatWith(IEnumerable<Tuple<T1, T2>> other)
            {
                return new TupleList<T1, T2>(this.Concat(other));
            }
        }

        internal static class SentinelArray
        {
            private class Dummy { }

            public static T[] CreateFor<T>(int length)
            {
                // Make an array of a completely incompatible type.
                // Actual use will throw InvalidCastException.
                var dummy = new Dummy[length];

                // Bypass type system – treat Dummy[] reference as T[]
                return Unsafe.As<Dummy[], T[]>(ref dummy);
            }
        }

        private static readonly TupleList<string, Action<List<int>>> ListEditActions = new()
        {
            {"Add",                                         (list) => list.Add(123)},
            {"AddRange",                                    (list) => list.AddRange(new int[] { 44, 45, 46, 47 })},
            {"this (setter)",                               (list) => list[1] = 5},
            {"Clear",                                       (list) => list.Clear()},
            {"Insert",                                      (list) => list.Insert(1, 999)},
            {"InsertRange",                                 (list) => list.InsertRange(3, new int[] { 44, 45, 46, 47 })},
            {"Remove",                                      (list) => list.Remove(6)},
            {"RemoveAll",                                   (list) => list.RemoveAll((value) => value == 4 || value == 6)},
            {"RemoveAt",                                    (list) => list.RemoveAt(3)},
            {"RemoveRange",                                 (list) => list.RemoveRange(4, 2)},
            {"Reverse",                                     (list) => list.Reverse()},
            {"Reverse(int, int)",                           (list) => list.Reverse(4, 2)},
            {"Sort()",                                      (list) => list.Sort()},
            {"Sort(IComparer<T>)",                          (list) => list.Sort(Comparer<int>.Default)},
            {"Sort(int, int, IComparer<T>)",                (list) => list.Sort(4, 2, Comparer<int>.Default)},
            {"Sort(Comparison)",                            (list) => list.Sort((x, y) => x - y)},
            {"Shuffle",                                     (list) => list.Shuffle()},
            {"Swap",                                        (list) => list.Swap(4, 6)},
        };

        private static readonly int[] ListTestBuffer = new int[15];
        private static readonly int[] ListTestSpanBuffer = new int[15];

        private static readonly TupleList<string, Action<List<int>>> ListQueryActions = new()
        {
            { "this (getter)",                              (list) => _ = list[0] },
            { "AsReadOnly",                                 (list) => list.AsReadOnly()},
            { "BinarySearch(int)",                          (list) => list.BinarySearch(6)},
            { "BinarySearch(int, IComparer<T>)",            (list) => list.BinarySearch(6, Comparer<int>.Default)},
            { "BinarySearch(int, int, int, IComparer<T>)",  (list) => list.BinarySearch(0, list.Count, 6, Comparer<int>.Default)},
            { "Contains",                                   (list) => list.Contains(6)},
            { "ConvertAll",                                 (list) => list.ConvertAll(x => x.ToString())},
            { "CopyTo(T[])",                                (list) => list.CopyTo(ListTestBuffer)},
            { "CopyTo(T[], int)",                           (list) => list.CopyTo(ListTestBuffer, 0)},
            { "CopyTo(int, T[], int, int)",                 (list) => list.CopyTo(0, ListTestBuffer, 0, 1)},
            { "CopyTo(Span<T>)",                            (list) => list.CopyTo(ListTestSpanBuffer.AsSpan(0, list.Count))},
            { "Equals(object)",                             (list) => list.Equals(list)},
            { "Equals(object, IEqualityComparer<T>)",       (list) => list.Equals(list, ListEqualityComparer<int>.Default)},
            { "Exists",                                     (list) => list.Exists((value) => value == 6)},
            { "Find",                                       (list) => list.Find((value) => value == 6)},
            { "FindAll",                                    (list) => list.FindAll((value) => value == 6)},
            { "FindIndex(Predicate<T>)",                    (list) => list.FindIndex((value) => value == 6)},
            { "FindIndex(int, Predicate<T>)",               (list) => list.FindIndex(0, (value) => value == 6)},
            { "FindIndex(int, int, Predicate<T>)",          (list) => list.FindIndex(0, list.Count, (value) => value == 6)},
            { "FindLast(Predicate<T>)",                     (list) => list.FindLast((value) => value == 6)},
            { "FindLastIndex(Predicate<T>)",                (list) => list.FindLastIndex((value) => value == 6)},
            { "FindLastIndex(int, Predicate<T>)",           (list) => list.FindLastIndex(list.Count - 1, (value) => value == 6)},
            { "FindLastIndex(int, int, Predicate<T>)",      (list) => list.FindLastIndex(list.Count - 1, list.Count, (value) => value == 6)},
            { "ForEach",                                    (list) => list.ForEach((value) => Console.WriteLine(value))},
            { "GetEnumerator",                              (list) => list.GetEnumerator()},
            { "GetHashCode",                                (list) => list.GetHashCode()},
            { "GetHashCode(IEqualityComparer)",             (list) => list.GetHashCode(ListEqualityComparer<int>.Default)},
            { "GetRange",                                   (list) => list.GetRange(0, list.Count)},
            { "GetView",                                    (list) => list.GetView(2, 2)},
            { "IndexOf(int)",                               (list) => list.IndexOf(6)},
            { "IndexOf(int, int)",                          (list) => list.IndexOf(6, 1)},
            { "IndexOf(int, int, int)",                     (list) => list.IndexOf(6, 1, 5)},
            { "LastIndexOf(int)",                           (list) => list.LastIndexOf(6)},
            { "LastIndexOf(int, int)",                      (list) => list.LastIndexOf(6, list.Count - 1)},
            { "LastIndexOf(int, int, int)",                 (list) => list.LastIndexOf(6, list.Count - 1, 5)},
            { "Slice",                                      (list) => list.Slice(1, 2)},
            { "ToArray",                                    (list) => list.ToArray()},
            { "ToString()",                                 (list) => list.ToString()},
            { "ToString(string)",                           (list) => list.ToString("J")},
            { "ToString(IFormatProvider)",                  (list) => list.ToString(J2N.Text.StringFormatter.InvariantCulture)},
            { "ToString(string, IFormatProvider)",          (list) => list.ToString("J", J2N.Text.StringFormatter.InvariantCulture)},
            { "TrueForAll",                                 (list) => list.TrueForAll((value) => value == 6)},
        };

        private static readonly TupleList<string, Action<List<int>>> ListCapacityChangeActions = new()
        {
            { "Capacity (setter)",                          (list) => list.Capacity = 100 },
            { "EnsureCapacity",                             (list) => list.EnsureCapacity(100) },
            { "TrimExcess",                                 (list) => list.TrimExcess() },
        };

        private static readonly TupleList<string, Action<List<int>>> ListArrayAccessActions =
            ListEditActions
                .ConcatWith(ListQueryActions)
                .ConcatWith(ListCapacityChangeActions)
                .ConcatWith(new TupleList<string, Action<List<int>>>
                {
                    // This one is an oddball because calling it will never throw on an invalid sublist
                    { "Capacity (getter)",                  (list) => _ = list.Capacity },
                });


        public static IEnumerable<TestCaseData> SubList_CoModification_Data
        {
            get
            {
                foreach (var edit in ListEditActions)
                {
                    foreach (var action in ListQueryActions.Union(ListEditActions))
                    {
                        yield return new TestCaseData($"edit: '{edit.Item1}', action: '{action.Item1}'", edit.Item2, action.Item2);
                    }
                }
            }
        }

        public static IEnumerable<TestCaseData> SubList_Enumerator_CoModification_Data
        {
            get
            {
                foreach (var edit in ListEditActions)
                {
                    yield return new TestCaseData($"edit: '{edit.Item1}'", edit.Item2);
                }
            }
        }

        public static IEnumerable<TestCaseData> SubList_ArrayAccess_Data
        {
            get
            {
                foreach (var action in ListArrayAccessActions)
                {
                    yield return new TestCaseData($"action: '{action.Item1}'", action.Item2);
                }
            }
        }

        /// <summary>
        /// Verifies changing the array through setting Capacity, calling EnsureCapacity(int)
        /// or calling TrimExcess() on an ancestor doesn't illegally access a subList array
        /// without setting its _items field to the original list's _items.
        /// </summary>
        [TestCaseSource(nameof(SubList_ArrayAccess_Data))]
        public void TestSubList_ArrayAccess_Integrity(string title, Action<List<int>> test)
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 10);

            // Inject an array with an invalid type into the sublist to make sure it always
            // replaces it (or avoids it) instead of using it.
            subList._items = SentinelArray.CreateFor<int>(subList._items.Length);
            list.Capacity = 100;
            Assert.DoesNotThrow(() => test(subList), $"Executing test on SubList after set Capacity. {title}");

            // Reset
            list = InitilizeList();
            subList = list.GetView(2, 10);

            // Inject an array with an invalid type into the sublist to make sure it always
            // replaces it (or avoids it) instead of using it.
            subList._items = SentinelArray.CreateFor<int>(subList._items.Length);
            list.EnsureCapacity(100);
            Assert.DoesNotThrow(() => test(subList), $"Executing test on SubList after EnsureCapacity(). {title}");

            // Reset
            list = InitilizeList(100);
            subList = list.GetView(2, 10);

            // Inject an array with an invalid type into the sublist to make sure it always
            // replaces it (or avoids it) instead of using it.
            subList._items = SentinelArray.CreateFor<int>(subList._items.Length);
            list.TrimExcess();
            Assert.DoesNotThrow(() => test(subList), $"Executing test on SubList after TrimExcess(). {title}");
        }

        /// <summary>
        /// Verifies any modification to the List's array through the parent API will
        /// invalidate a sublist.
        /// </summary>
        [TestCaseSource(nameof(SubList_CoModification_Data))]
        public void TestSubList_CoModification_Version(string combination, Action<List<int>> edit, Action<List<int>> test)
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 10);

            // Apply both actions on the sublist
            Assert.DoesNotThrow(() => edit(subList), combination);
            // Clear() will actually zero this out, and some actions require there to be items
            if (subList.Count == 0)
                subList.AddRange(InitilizeList());
            Assert.DoesNotThrow(() => test(subList), "Executing test on SubList. " + combination);

            // Reset
            list = InitilizeList();
            subList = list.GetView(2, 10);

            // Apply the edit to the list and the action to the sublist.
            // This should always throw an exception.
            Assert.DoesNotThrow(() => edit(list), combination);
            Assert.Throws<InvalidOperationException>(() => test(subList), "Executing test on PARENT. " + combination);
        }

        /// <summary>
        /// Verifies any modification to the List's array through the parent API will
        /// invalidate a sublist.
        /// </summary>
        [TestCaseSource(nameof(SubList_CoModification_Data))]
        public void TestSubList_Grandchild_CoModification_Version(string combination, Action<List<int>> edit, Action<List<int>> test)
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 12);
            List<int> grandchildList = subList.GetView(1, 10);

            // Apply both actions on the grandchildList
            Assert.DoesNotThrow(() => edit(grandchildList), combination);
            // Clear() will actually zero this out, and some actions require there to be items
            if (grandchildList.Count == 0)
                grandchildList.AddRange(InitilizeList());
            Assert.DoesNotThrow(() => test(grandchildList), "Executing test on GRANDCHILD. " + combination);

            // Reset
            list = InitilizeList();
            subList = list.GetView(2, 12);
            grandchildList = subList.GetView(1, 10);

            // Apply the edit to the list and the action to the sublist.
            // This should always throw an exception.
            Assert.DoesNotThrow(() => edit(list), combination);
            Assert.Throws<InvalidOperationException>(() => test(subList), "Executing test on CHILD. " + combination);

            // Reset
            list = InitilizeList();
            subList = list.GetView(2, 12);
            grandchildList = subList.GetView(1, 10);

            // Apply the edit to the list and the action to the sublist.
            // This should always throw an exception.
            Assert.DoesNotThrow(() => edit(list), combination);
            Assert.Throws<InvalidOperationException>(() => test(grandchildList), "Executing test on PARENT. " + combination);
        }

        /// <summary>
        /// Verifies any modification to the List's array through the parent API will
        /// invalidate a sublist's enumerator
        /// </summary>
        [TestCaseSource(nameof(SubList_Enumerator_CoModification_Data))]
        public void TestSubList_Enumerator_CoModification_Version(string combination, Action<List<int>> edit)
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 10);

            using (var enumerator = subList.GetEnumerator())
            {
                Assert.DoesNotThrow(() => enumerator.MoveNext());
                Assert.DoesNotThrow(() => enumerator.Reset());

                // Apply the edit to the list and check enumerator.
                // This should always throw an exception.
                Assert.DoesNotThrow(() => edit(list), combination);

                Assert.Throws<InvalidOperationException>(() => enumerator.Reset(), "Executing Reset() while edit applied on PARENT. " + combination);
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext(), "Executing MoveNext() while edit applied on PARENT. " + combination);
            }
        }

        /// <summary>
        /// Verifies any modification to the List's array through the parent API will
        /// invalidate a grandchild sublist's enumerator
        /// </summary>
        [TestCaseSource(nameof(SubList_Enumerator_CoModification_Data))]
        public void TestSubList_Grandchild_Enumerator_CoModification_Version(string combination, Action<List<int>> edit)
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 12);
            List<int> grandchildList = subList.GetView(1, 10);

            using (var enumerator = grandchildList.GetEnumerator())
            {
                Assert.DoesNotThrow(() => enumerator.MoveNext());
                Assert.DoesNotThrow(() => enumerator.Reset());

                // Apply the edit to the subList and check enumerator.
                // Enumeration should always throw an exception.
                Assert.DoesNotThrow(() => edit(subList), combination);

                Assert.Throws<InvalidOperationException>(() => enumerator.Reset(), "Executing Reset() while edit applied on PARENT. " + combination);
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext(), "Executing MoveNext() while edit applied on PARENT. " + combination);
            }

            // Reset
            list = InitilizeList();
            subList = list.GetView(2, 12);
            grandchildList = subList.GetView(1, 10);

            using (var enumerator = grandchildList.GetEnumerator())
            {
                Assert.DoesNotThrow(() => enumerator.MoveNext());
                Assert.DoesNotThrow(() => enumerator.Reset());

                // Apply the edit to the list and check enumerator.
                // Enumeration should always throw an exception.
                Assert.DoesNotThrow(() => edit(list), combination);

                Assert.Throws<InvalidOperationException>(() => enumerator.Reset(), "Executing Reset() while edit applied on GRANDPARENT. " + combination);
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext(), "Executing MoveNext() while edit applied on GRANDPARENT. " + combination);
            }
        }

        /// <summary>
        /// Verifies that removal of a SubList offset range modifies the parent as expected
        /// </summary>
        [Test]
        public void TestSubList_RemoveAll()
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 12);

            subList.RemoveAll((value) => value == 1 || value == 4 || value == 6 || value == 15); // 1 and 15 are outside of the view

            assertEquals(10, subList.Count);
            assertEquals(13, list.Count);

            var expectedValues = new int[] { 1, 2, 3, 5, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            for (int i = 0; i < expectedValues.Length; i++)
                assertEquals(expectedValues[i], list[i]);

            // Test again using enumerator
            int index = 0;
            foreach (var value in list)
            {
                assertEquals(expectedValues[index++], value);
            }
        }


        /// <summary>
        /// Verifies that removal of a grandchild SubList offset range modifies the parent as expected
        /// </summary>
        [Test]
        public void TestSubList_Grandchild_RemoveAll()
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 12);
            List<int> grandchildList = subList.GetView(1, 8);

            grandchildList.RemoveAll((value) => value == 1 || value == 3 || value == 5 || value == 7 || value == 15); // 1, 3, and 15 are outside of the view

            assertEquals(6, grandchildList.Count);
            assertEquals(10, subList.Count);
            assertEquals(13, list.Count);

            var expectedValues = new int[] { 1, 2, 3, 4, 6, 8, 9, 10, 11, 12, 13, 14, 15 };
            for (int i = 0; i < expectedValues.Length; i++)
                assertEquals(expectedValues[i], list[i]);

            // Test again using enumerator
            int index = 0;
            foreach (var value in list)
            {
                assertEquals(expectedValues[index++], value);
            }
        }

        /// <summary>
        /// Verifies that removal of a grandchild SubList offset range modifies the parent as expected
        /// </summary>
        [Test]
        public void TestSubList_Grandchild_IndexOf_T()
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 12);
            List<int> grandchildList = subList.GetView(1, 8);

            assertEquals(7, subList.IndexOf(10));
            assertEquals(6, grandchildList.IndexOf(10));
        }

        /// <summary>
        /// Verifies that removal of a grandchild SubList offset range modifies the parent as expected
        /// </summary>
        [Test]
        public void TestSubList_Grandchild_IndexOf_T_Int32()
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 12);
            List<int> grandchildList = subList.GetView(1, 8);

            assertEquals(7, subList.IndexOf(10, 5));
            assertEquals(6, grandchildList.IndexOf(10, 5));
        }

        /// <summary>
        /// Verifies that removal of a grandchild SubList offset range modifies the parent as expected
        /// </summary>
        [Test]
        public void TestSubList_Grandchild_IndexOf_T_Int32_Int32()
        {
            List<int> list = InitilizeList();
            List<int> subList = list.GetView(2, 12);
            List<int> grandchildList = subList.GetView(1, 8);

            assertEquals(7, subList.IndexOf(10, 5, subList.Count - 5));
            assertEquals(6, grandchildList.IndexOf(10, 5, grandchildList.Count - 5));

            Assert.Throws<ArgumentOutOfRangeException>(() => subList.IndexOf(10, 5, subList.Count));
            Assert.Throws<ArgumentOutOfRangeException>(() => grandchildList.IndexOf(10, 5, grandchildList.Count));
        }


        public class Harmony_ArrayListTest : TestCase
        {
            private List<Integer> alist;
            private List<object> olist;

            private static readonly Object[] objArray = LoadObjectArray();

            private static object[] LoadObjectArray()
            {
                var objArray = new Object[100];
                for (int i = 0; i < objArray.Length; i++)
                    objArray[i] = new Integer(i);
                return objArray;
            }

            /**
             * @tests java.util.ArrayList#ArrayList()
             */
            [Test]
            public void Test_Constructor()
            {
                // Test for method java.util.ArrayList()
                new Support_ListTest("", alist).RunTest();

                var subList = new List<Integer>();
                for (int i = -50; i < 150; i++)
                    subList.Add(new Integer(i));
                new Support_ListTest("", subList.GetView(50, 150 - 50)).RunTest();

                // Test for GetView() extension method over SCG.List<T>
                IList<Integer> subList2 = new SCG.List<Integer>();
                for (int i = -50; i < 150; i++)
                    subList2.Add(new Integer(i));
                new Support_ListTest("", subList2.GetView(50, 150 - 50)).RunTest();
            }

            /**
             * @tests java.util.ArrayList#ArrayList(int)
             */
            [Test]
            public void Test_ConstructorI()
            {
                // Test for method java.util.ArrayList(int)
                var al = new List<Integer>(5);
                assertEquals("Incorrect arrayList created", 0, al.Count);

                al = new List<Integer>(0);
                assertEquals("Incorrect arrayList created", 0, al.Count);

                try
                {
                    al = new List<Integer>(-1);
                    fail("Should throw IllegalArgumentException");
                }
                catch (ArgumentException e)
                {
                    // Excepted
                }
            }

            /**
             * @tests java.util.ArrayList#ArrayList(java.util.Collection)
             */
            [Test]
            public void Test_ConstructorLjava_util_Collection()
            {
                // Test for method java.util.ArrayList(java.util.Collection)
                List<Integer> al = new List<Integer>(objArray.Cast<Integer>());
                assertTrue("arrayList created from collection has incorrect size", al
                        .Count == objArray.Length);
                for (int counter = 0; counter < objArray.Length; counter++)
                    assertTrue(
                            "arrayList created from collection has incorrect elements",
                            al[counter] == objArray[counter]);

            }

            // J2N: This test relies on the Java implementation of HashSet that seems to handle concurrency
            // using iterator.remove() but .NET's HashSet doesn't work the same way
            //[Test]
            //public void TestConstructorWithConcurrentCollection()
            //{
            //    ICollection<String> collection = shrinksOnSize("A", "B", "C", "D");
            //    List<String> list = new List<String>(collection);
            //    assertFalse(list.Contains(null));
            //}

            /**
             * @tests java.util.ArrayList#add(int, java.lang.Object)
             */
            [Test]
            public void Test_addILjava_lang_Object()
            {
                // Test for method void java.util.ArrayList.Add(int, java.lang.Object)
                Object o;
                olist.Insert(50, o = new Object());
                assertTrue("Failed to add Object", olist[50] == o);
                assertTrue("Failed to fix up list after insert",
                        olist[51] == objArray[50]
                                && (olist[52] == objArray[51]));
                Object oldItem = olist[25];
                olist.Insert(25, null);
                assertNull("Should have returned null", olist[25]);
                assertTrue("Should have returned the old item from slot 25", olist
                        [26] == oldItem);

                olist.Insert(0, o = new Object());
                assertEquals("Failed to add Object", olist[0], o);
                assertEquals(olist[1], objArray[0]);
                assertEquals(olist[2], objArray[1]);

                oldItem = olist[0];
                olist.Insert(0, null);
                assertNull("Should have returned null", olist[0]);
                assertEquals("Should have returned the old item from slot 0", olist
                        [1], oldItem);

                try
                {
                    olist.Insert(-1, new Object());
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    olist.Insert(-1, null);
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    olist.Insert(olist.Count + 1, new Object());
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    olist.Insert(olist.Count + 1, null);
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }
            }

            /**
             * @tests java.util.ArrayList#add(int, java.lang.Object)
             */
            [Test]
            public void Test_addILjava_lang_Object_2()
            {
                Object o = new Object();
                int size = olist.Count;
                olist.Insert(size, o);
                assertEquals("Failed to add Object", olist[size], o);
                assertEquals(olist[size - 2], objArray[size - 2]);
                assertEquals(olist[size - 1], objArray[size - 1]);

                olist.RemoveAt(size);

                size = olist.Count;
                olist.Insert(size, null);
                assertNull("Should have returned null", olist[size]);
                assertEquals(olist[size - 2], objArray[size - 2]);
                assertEquals(olist[size - 1], objArray[size - 1]);
            }

            /**
             * @tests java.util.ArrayList#add(java.lang.Object)
             */
            [Test]
            public void Test_addLjava_lang_Object()
            {
                // Test for method boolean java.util.ArrayList.Add(java.lang.Object)
                Object o = new Object();
                olist.Add(o);
                assertTrue("Failed to add Object", olist[olist.Count - 1] == o);
                olist.Add(null);
                assertNull("Failed to add null", olist[olist.Count - 1]);
            }

            /**
             * @tests java.util.ArrayList#addAll(int, java.util.Collection)
             */
            [Test]
            public void Test_addAllILjava_util_Collection()
            {
                // Test for method boolean java.util.ArrayList.addAll(int,
                // java.util.Collection)
                olist.InsertRange(50, olist);
                assertEquals("Returned incorrect size after adding to existing list",
                        200, olist.Count);
                for (int i = 0; i < 50; i++)
                    assertTrue("Manipulated elements < index",
                            olist[i] == objArray[i]);
                for (int i = 0; i >= 50 && (i < 150); i++)
                    assertTrue("Failed to ad elements properly",
                            olist[i] == objArray[i - 50]);
                for (int i = 0; i >= 150 && (i < 200); i++)
                    assertTrue("Failed to ad elements properly",
                            olist[i] == objArray[i - 100]);
                List<object> listWithNulls = new List<object>();
                listWithNulls.Add(null);
                listWithNulls.Add(null);
                listWithNulls.Add("yoink");
                listWithNulls.Add("kazoo");
                listWithNulls.Add(null);
                olist.InsertRange(100, listWithNulls);
                assertTrue("Incorrect size: " + olist.Count, olist.Count == 205);
                assertNull("Item at slot 100 should be null", olist[100]);
                assertNull("Item at slot 101 should be null", olist[101]);
                assertEquals("Item at slot 102 should be 'yoink'", "yoink", olist
                        [102]);
                assertEquals("Item at slot 103 should be 'kazoo'", "kazoo", olist
                        [103]);
                assertNull("Item at slot 104 should be null", olist[104]);
                olist.InsertRange(205, listWithNulls);
                assertTrue("Incorrect size2: " + olist.Count, olist.Count == 210);
            }

            /**
             * @tests java.util.ArrayList#addAll(int, java.util.Collection)
             */
            [Test]
            public void Test_addAllILjava_util_Collection_2()
            {
                // Regression for HARMONY-467
                List<object> obj = new List<object>();
                try
                {
                    obj.InsertRange((int)-1, /*(ICollection<object>)null*/new List<object>()); // J2N: .NET checks for null first, so this will trigger the wrong exception
                    fail("IndexOutOfBoundsException expected");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                // Regression for HARMONY-5705
                String[] data = new String[] { "1", "2", "3", "4", "5", "6", "7", "8" };
                List<object> list1 = new List<object>();
                List<object> list2 = new List<object>();
                foreach (String d in data)
                {
                    list1.Add(d);
                    list2.Add(d);
                    list2.Add(d);
                }
                while (list1.Count > 0)
                    list1.RemoveAt(0);
                list1.AddRange(list2);
                assertTrue("The object list is not the same as original list", new HashSet<object>(list1)
                        .IsSupersetOf(list2)
                        && new HashSet<object>(list2).IsSupersetOf(list1));

                obj = new List<object>();
                for (int i = 0; i < 100; i++)
                {
                    if (list1.Count > 0)
                    {
                        foreach (var toRemove in list1)
                            obj.Remove(toRemove);
                        obj.AddRange(list1);
                    }
                }
                assertTrue("The object list is not the same as original list", new HashSet<object>(obj)
                        .IsSupersetOf(list1)
                        && new HashSet<object>(list1).IsSupersetOf(obj));

                // Regression for Harmony-5799
                list1 = new List<object>();
                list2 = new List<object>();
                int location = 2;

                String[] strings = { "0", "1", "2", "3", "4", "5", "6" };
                int[] integers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                for (int i = 0; i < 7; i++)
                {
                    list1.Add(strings[i]);
                }
                for (int i = 0; i < 10; i++)
                {
                    list2.Add(integers[i]);
                }
                list1.RemoveAt(location);
                list1.InsertRange(location, list2);

                // Inserted elements should be equal to integers array
                for (int i = 0; i < integers.Length; i++)
                {
                    assertEquals(integers[i], list1[location + i]);
                }
                // Elements after inserted location should
                // be equals to related elements in strings array
                for (int i = location + 1; i < strings.Length; i++)
                {
                    assertEquals(strings[i], list1[i + integers.Length - 1]);
                }
            }

            /**
             * @tests java.util.ArrayList#addAll(int, java.util.Collection)
             */
            [Test]
            public void Test_addAllILjava_util_Collection_3()
            {
                var obj = new List<object>();
                obj.InsertRange(0, obj);
                obj.InsertRange(obj.Count, obj);
                try
                {
                    obj.InsertRange(-1, obj);
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    obj.InsertRange(obj.Count + 1, obj);
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    obj.InsertRange(0, null);
                    fail("Should throw NullPointerException");
                }
                catch (ArgumentNullException e)
                {
                    // Excepted
                }

                try
                {
                    obj.InsertRange(obj.Count + 1, /*null*/ new List<object>()); // J2N: .NET checks for null first, so we are expecting ArgumentNullException here
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    obj.InsertRange((int)-1, (ICollection<object>)/*null*/ new List<object>()); // J2N: .NET checks for null first, so we are expecting ArgumentNullException here
                    fail("IndexOutOfBoundsException expected");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }
            }

            [Test]
            public void Test_addAllCollectionOfQextendsE()
            {
                // Regression for HARMONY-539
                // https://issues.apache.org/jira/browse/HARMONY-539
                List<String> alist = new List<String>();
                List<String> blist = new List<String>();
                alist.Add("a");
                alist.Add("b");
                blist.Add("c");
                blist.Add("d");
                blist.RemoveAt(0);
                blist.InsertRange(0, alist);
                assertEquals("a", blist[0]);
                assertEquals("b", blist[1]);
                assertEquals("d", blist[2]);
            }

            /**
             * @tests java.util.ArrayList#addAll(java.util.Collection)
             */
            [Test]
            public void Test_addAllLjava_util_Collection()
            {
                // Test for method boolean
                // java.util.ArrayList.addAll(java.util.Collection)
                List<object> l = new List<object>();
                l.AddRange(olist);
                for (int i = 0; i < olist.Count; i++)
                    assertTrue("Failed to add elements properly", l[i].Equals(
                            olist[i]));
                olist.AddRange(olist);
                assertEquals("Returned incorrect size after adding to existing list",
                        200, olist.Count);
                for (int i = 0; i < 100; i++)
                {
                    assertTrue("Added to list in incorrect order", olist[i].Equals(
                            l[i]));
                    assertTrue("Failed to add to existing list", olist[i + 100]
                            .Equals(l[i]));
                }
                ISet<object> setWithNulls = new HashSet<object>();
                setWithNulls.Add(null);
                setWithNulls.Add(null);
                setWithNulls.Add("yoink");
                setWithNulls.Add("kazoo");
                setWithNulls.Add(null);
                olist.InsertRange(100, setWithNulls);
                var it = setWithNulls.GetEnumerator();
                it.MoveNext();
                assertTrue("Item at slot 100 is wrong: " + olist[100], olist
                        [100] == it.Current);
                it.MoveNext();
                assertTrue("Item at slot 101 is wrong: " + olist[101], olist
                    [101] == it.Current);
                it.MoveNext();
                assertTrue("Item at slot 103 is wrong: " + olist[102], olist
                    [102] == it.Current);

                try
                {
                    olist.AddRange(null);
                    fail("Should throw NullPointerException");
                }
                catch (ArgumentNullException e)
                {
                    // Excepted
                }

                // Regression test for Harmony-3481
                List<Integer> originalList = new List<Integer>(12);
                for (int j = 0; j < 12; j++)
                {
                    originalList.Add(j);
                }

                originalList.RemoveAt(0);
                originalList.RemoveAt(0);

                List<Integer> additionalList = new List<Integer>(11);
                for (int j = 0; j < 11; j++)
                {
                    additionalList.Add(j);
                }
                originalList.AddRange(additionalList);
                //assertTrue();
                assertEquals(21, originalList.Count);

            }

            [Test]
            public void Test_ArrayList_addAll_scenario1()
            {
                List<int> arrayListA = new List<int>();
                arrayListA.Add(1);
                List<int> arrayListB = new List<int>();
                arrayListB.Add(1);
                arrayListA.InsertRange(1, arrayListB);
                int size = arrayListA.Count;
                assertEquals(2, size);
                for (int index = 0; index < size; index++)
                {
                    assertEquals(1, arrayListA[index]);
                }
            }

            [Test]
            public void Test_ArrayList_addAll_scenario2()
            {
                List<int> arrayList = new List<int>();
                arrayList.Add(1);
                arrayList.InsertRange(1, arrayList);
                int size = arrayList.Count;
                assertEquals(2, size);
                for (int index = 0; index < size; index++)
                {
                    assertEquals(1, arrayList[index]);
                }
            }

            // Regression test for HARMONY-5839
            [Test]
            public void TestaddAllHarmony5839()
            {
                var coll = new List<string>(new String[] { "1", "2" });
                List<string> list = new List<string>();
                list.Add("a");
                list.Insert(0, "b");
                list.Insert(0, "c");
                list.Insert(0, "d");
                list.Insert(0, "e");
                list.Insert(0, "f");
                list.Insert(0, "g");
                list.Insert(0, "h");
                list.Insert(0, "i");

                list.InsertRange(6, coll);

                assertEquals(11, list.Count);
                assertFalse(list.Contains(null));
            }

            /**
             * @tests java.util.ArrayList#clear()
             */
            [Test]
            public void Test_clear()
            {
                // Test for method void java.util.ArrayList.clear()
                olist.Clear();
                assertEquals("List did not clear", 0, olist.Count);
                olist.Add(null);
                olist.Add(null);
                olist.Add(null);
                olist.Add("bam");
                olist.Clear();
                assertEquals("List with nulls did not clear", 0, olist.Count);
                /*
                 * for (int i = 0; i < olist.Count; i++) assertNull("Failed to clear
                 * list", olist.get(i));
                 */

            }

            /**
             * @tests java.util.ArrayList#clone()
             */
            [Test]
            public void Test_clone()
            {
                // Test for method java.lang.Object java.util.ArrayList.clone()
                //ArrayList x = (ArrayList)(((ArrayList)(alist)).clone());
                var x = new List<Integer>(alist);
                assertTrue("Cloned list was inequal to original", x.Equals(alist));
                for (int i = 0; i < alist.Count; i++)
                    assertTrue("Cloned list contains incorrect elements",
                            alist[i] == x[i]);

                alist.Add(null);
                alist.Insert(25, null);
                //x = (ArrayList)(((ArrayList)(alist)).clone());
                x = new List<Integer>(alist);
                assertTrue("nulls test - Cloned list was inequal to original", x
                        .Equals(alist));
                for (int i = 0; i < alist.Count; i++)
                    assertTrue("nulls test - Cloned list contains incorrect elements",
                            alist[i] == x[i]);

            }

            /**
             * @tests java.util.ArrayList#contains(java.lang.Object)
             */
            [Test]
            public void Test_containsLjava_lang_Object()
            {
                // Test for method boolean
                // java.util.ArrayList.contains(java.lang.Object)
                assertTrue("Returned false for valid element", alist
                        .Contains(objArray[99]));
                assertTrue("Returned false for equal element", alist
                        .Contains(new Integer(8)));
                assertTrue("Returned true for invalid element", !alist
                        .Contains(new Object()));
                assertTrue("Returned true for null but should have returned false",
                        !alist.Contains(null));
                alist.Add(null);
                assertTrue("Returned false for null but should have returned true",
                        alist.Contains(null));
            }

            /**
             * @tests java.util.ArrayList#ensureCapacity(int)
             */
            [Test]
            public void Test_ensureCapacityI()
            {
                // Test for method void java.util.ArrayList.ensureCapacity(int)
                // TODO : There is no good way to test this as it only really impacts on
                // the private implementation.

                Object testObject = new Object();
                int capacity = 20;
                List<object> al = new List<object>(capacity);
                int i;
                for (i = 0; i < capacity / 2; i++)
                {
                    al.Insert(i, new Object());
                }
                al.Insert(i, testObject);
                int location = al.IndexOf(testObject);
                al.EnsureCapacity(capacity);
                assertTrue("EnsureCapacity moved objects around in array1.",
                        location == al.IndexOf(testObject));
                al.RemoveAt(0);
                al.EnsureCapacity(capacity);
                assertTrue("EnsureCapacity moved objects around in array2.",
                        --location == al.IndexOf(testObject));
                al.EnsureCapacity(capacity + 2);
                assertTrue("EnsureCapacity did not change location.", location == al
                        .IndexOf(testObject));

                List<String> list = new List<String>(1);
                list.Add("hello");
                list.EnsureCapacity(/*int.MinValue*/ 0); // J2N: int.MinValue throws ArgumentOutOfRangeException, but this is nonsensical, anyway.
            }

            /**
             * @tests java.util.ArrayList#get(int)
             */
            [Test]
            public void Test_getI()
            {
                // Test for method java.lang.Object java.util.ArrayList.get(int)
                assertTrue("Returned incorrect element", alist[22] == objArray[22]);
                try
                {
                    var _ = alist[8765];
                    fail("Failed to throw expected exception for index > size");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }
            }

            /**
             * @tests java.util.ArrayList#indexOf(java.lang.Object)
             */
            [Test]
            public void Test_indexOfLjava_lang_Object()
            {
                // Test for method int java.util.ArrayList.indexOf(java.lang.Object)
                assertEquals("Returned incorrect index", 87, olist
                    .IndexOf(objArray[87]));
                assertEquals("Returned index for invalid Object", -1, olist
                        .IndexOf(new Object()));
                olist.Insert(25, null);
                olist.Insert(50, null);
                assertTrue("Wrong indexOf for null.  Wanted 25 got: "
                        + olist.IndexOf(null), olist.IndexOf(null) == 25);
            }

            ///**
            // * @tests java.util.ArrayList#isEmpty()
            // */
            //public void Test_isEmpty()
            //{
            //    // Test for method boolean java.util.ArrayList.isEmpty()
            //    assertTrue("isEmpty returned false for new list", new ArrayList()
            //            .isEmpty());
            //    assertTrue("Returned true for existing list with elements", !alist
            //            .isEmpty());
            //}

            /**
             * @tests java.util.ArrayList#lastIndexOf(java.lang.Object)
             */
            [Test]
            public void Test_lastIndexOfLjava_lang_Object()
            {
                // Test for method int java.util.ArrayList.lastIndexOf(java.lang.Object)
                olist.Add(new Integer(99));
                assertEquals("Returned incorrect index", 100, olist
                        .LastIndexOf(objArray[99]));
                assertEquals("Returned index for invalid Object", -1, olist
                        .LastIndexOf(new Object()));
                olist.Insert(25, null);
                olist.Insert(50, null);
                assertTrue("Wrong lastIndexOf for null.  Wanted 50 got: "
                        + olist.LastIndexOf(null), olist.LastIndexOf(null) == 50);
            }

            /**
             * @tests {@link java.util.ArrayList#removeRange(int, int)}
             */
            [Test]
            public void Test_removeRange()
            {
                //MockArrayList mylist = new MockArrayList();
                var mylist = new List<object>(); // J2N: Since it is not possible to override Size anyway in .NET, we are testing the regular list, but if we make Count virtual at some point we should revisit this
                mylist.RemoveRange(0, 0);

                try
                {
                    mylist.RemoveRange(0, 1);
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentException e) // J2N: in .NET we throw ArgumentException when the offset + length is greater than the size of the collection
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                int[] data = { 1, 2, 3 };
                for (int i = 0; i < data.Length; i++)
                {
                    mylist.Insert(i, data[i]);
                }

                mylist.RemoveRange(0, 1);
                assertEquals(data[1], mylist[0]);
                assertEquals(data[2], mylist[1]);

                try
                {
                    mylist.RemoveRange(-1, 1);
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    mylist.RemoveRange(0, -1);
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                //try
                //{
                //    mylist.RemoveRange(1, 0);
                //    fail("Should throw IndexOutOfBoundsException");
                //}
                //catch (ArgumentOutOfRangeException e)
                //{
                //    // Expected
                //    assertNotNull(e.Message);
                //}

                // J2N: zero count is fine, it is simply a no-op in .NET
                Assert.DoesNotThrow(() => mylist.RemoveRange(1, 0));

                try
                {
                    mylist.RemoveRange(2, 1);
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentException e) // J2N: in .NET we throw ArgumentException when the offset + length is greater than the size of the collection
                {
                    // Expected
                    assertNotNull(e.Message);
                }
            }

            /**
             * @tests java.util.ArrayList#remove(int)
             */
            [Test]
            public void Test_removeI()
            {
                // Test for method java.lang.Object java.util.ArrayList.remove(int)
                olist.RemoveAt(10);
                assertEquals("Failed to remove element", -1, olist
                        .IndexOf(objArray[10]));
                try
                {
                    olist.RemoveAt(999);
                    fail("Failed to throw exception when index out of range");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                //ArrayList myList = (ArrayList)(((ArrayList)(olist)).clone());
                var myList = new List<object>(olist);
                olist.Insert(25, null);
                olist.Insert(50, null);
                olist.RemoveAt(50);
                olist.RemoveAt(25);
                assertTrue("Removing nulls did not work", olist.Equals(myList));

                List<string> list = new List<string>(new String[] { "a", "b", "c",
                "d", "e", "f", "g" });
                //assertTrue("Removed wrong element 1", list.RemoveAt(0) == "a");
                //assertTrue("Removed wrong element 2", list.RemoveAt(4) == "f");
                list.RemoveAt(0);
                list.RemoveAt(4);
                String[] result = new String[5];
                list.CopyTo(result);
                assertTrue("Removed wrong element 3", Arrays.Equals(result,
                        new String[] { "b", "c", "d", "e", "g" }));

                List<object> l = new List<object>(0);
                l.Add(new Object());
                l.Add(new Object());
                l.RemoveAt(0);
                l.RemoveAt(0);
                try
                {
                    l.RemoveAt(-1);
                    fail("-1 should cause exception");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }
                try
                {
                    l.RemoveAt(0);
                    fail("0 should case exception");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }
            }

            /**
             * @tests java.util.ArrayList#set(int, java.lang.Object)
             */
            [Test]
            public void Test_setILjava_lang_Object()
            {
                // Test for method java.lang.Object java.util.ArrayList.set(int,
                // java.lang.Object)
                Object obj;
                olist[65] = obj = new Object();
                assertTrue("Failed to set object", olist[65] == obj);
                olist[50] = null;
                assertNull("Setting to null did not work", olist[50]);
                assertTrue("Setting increased the list's size to: " + olist.Count,
                        olist.Count == 100);

                obj = new Object();
                olist[0] = obj;
                assertTrue("Failed to set object", olist[0] == obj);

                try
                {
                    olist[-1] = obj;
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    olist[olist.Count] = obj;
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    olist[-1] = null;
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }

                try
                {
                    olist[olist.Count] = null;
                    fail("Should throw IndexOutOfBoundsException");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Expected
                    assertNotNull(e.Message);
                }
            }

            /**
             * @tests java.util.ArrayList#size()
             */
            [Test]
            public void Test_size()
            {
                // Test for method int java.util.ArrayList.Count
                assertEquals("Returned incorrect size for exiting list", 100, alist
                        .Count);
                assertEquals("Returned incorrect size for new list", 0, new List<object>()
                        .Count);
            }

            /**
             * @tests java.util.AbstractCollection#toString()
             */
            [Test]
            public void Test_toString()
            {
                var l = new List<object>(1);
                l.Add(l);
                String result = l.ToString();
                assertTrue("should contain self ref", result.IndexOf("(this") > -1);
            }

            /**
             * @tests java.util.ArrayList#toArray()
             */
            [Test]
            public void Test_toArray()
            {
                // Test for method java.lang.Object [] java.util.ArrayList.toArray()
                alist[25] = null;
                alist[75] = null;
                Object[] obj = alist.ToArray();
                assertEquals("Returned array of incorrect size", objArray.Length,
                        obj.Length);

                for (int i = 0; i < obj.Length; i++)
                {
                    if ((i == 25) || (i == 75))
                        assertNull("Should be null at: " + i + " but instead got: "
                                + obj[i], obj[i]);
                    else
                        assertTrue("Returned incorrect array: " + i,
                                obj[i] == objArray[i]);
                }

            }

            /**
             * @tests java.util.ArrayList#toArray(java.lang.Object[])
             */
            [Test]
            public void Test_toArray_Ljava_lang_Object()
            {
                // Test for method java.lang.Object []
                // java.util.ArrayList.toArray(java.lang.Object [])
                alist[25] = null;
                alist[75] = null;
                Integer[] argArray = new Integer[100];
                Integer[] retArray = new Integer[100];
                //retArray = alist.ToArray(argArray);
                alist.CopyTo(argArray);
                retArray = alist.ToArray();
                //assertTrue("Returned different array than passed", retArray == argArray);
                argArray = new Integer[1000];
                //retArray = alist.ToArray(argArray);

                assertNull("Failed to set first extra element to null", argArray[alist
                        .Count]);
                for (int i = 0; i < 100; i++)
                {
                    if ((i == 25) || (i == 75))
                        assertNull("Should be null: " + i, retArray[i]);
                    else
                        assertTrue("Returned incorrect array: " + i,
                                retArray[i] == objArray[i]);
                }
            }

            /**
             * @tests java.util.ArrayList#trimToSize()
             */
            [Test]
            public void Test_trimToSize()
            {
                // Test for method void java.util.ArrayList.trimToSize()
                for (int i = 99; i > 24; i--)
                    alist.RemoveAt(i);
                ((List<Integer>)alist).TrimExcess();
                assertEquals("Returned incorrect size after trim", 25, alist.Count);
                for (int i = 0; i < alist.Count; i++)
                    assertTrue("Trimmed list contained incorrect elements", alist
                            [i] == objArray[i]);
                //var v = new List<string>();
                //v.Add("a");
                // J2N: .NET doesn't throw in this case, because TrimExcess (like EnsureCapacity) is not
                // considered to be a structural modification of the collection state.
                List<string> al = new List<string>(capacity: 16);
                al.Add("a");
                var it = al.GetEnumerator();
                al.TrimExcess();
                // Should not throw - capacity changes don't invalidate enumerators
                assertTrue(it.MoveNext());
            }

            /**
             * @test java.util.ArrayList#addAll(int, Collection)
             */
            [Test]
            public void Test_addAll()
            {
                List<string> list = new List<string>();
                list.Add("one");
                list.Add("two");
                assertEquals(2, list.Count);

                list.RemoveAt(0);
                assertEquals(1, list.Count);

                List<string> collection = new List<string>();
                collection.Add("1");
                collection.Add("2");
                collection.Add("3");
                assertEquals(3, collection.Count);

                list.InsertRange(0, collection);
                assertEquals(4, list.Count);

                list.RemoveAt(0);
                list.RemoveAt(0);
                assertEquals(2, list.Count);

                collection.Add("4");
                collection.Add("5");
                collection.Add("6");
                collection.Add("7");
                collection.Add("8");
                collection.Add("9");
                collection.Add("10");
                collection.Add("11");
                collection.Add("12");

                assertEquals(12, collection.Count);

                list.InsertRange(0, collection);
                assertEquals(14, list.Count);
            }

            // J2N: This test relies on the Java implementation of HashSet that seems to handle concurrency
            // using iterator.remove() but .NET's HashSet doesn't work the same way
            //[Test]
            //public void TestAddAllWithConcurrentCollection()
            //{
            //    List<string> list = new List<string>();
            //    list.AddRange(shrinksOnSize("A", "B", "C", "D"));
            //    assertFalse(list.Contains(null));
            //}

            // J2N: This test relies on the Java implementation of HashSet that seems to handle concurrency
            // using iterator.remove() but .NET's HashSet doesn't work the same way
            //[Test]
            //public void TestAddAllAtPositionWithConcurrentCollection()
            //{
            //    List<string> list = new List<string> { "A", "B", "C", "D" };

            //    list.InsertRange(3, shrinksOnSize("E", "F", "G", "H"));
            //    assertFalse(list.Contains(null));
            //}

            // J2N: Size is not public (and was added mainly to support SubList/GetView) so
            // we don't really need this functionality
            //[Test]
            //public void Test_override_size()
            //{
            //    var testlist = new MockArrayList();
            //    // though size is overriden, it should passed without exception
            //    testlist.Add("test_0");
            //    testlist.Add("test_1");
            //    testlist.Add("test_2");
            //    testlist.Insert(1, "test_3");
            //    var _ = testlist[1];
            //    testlist.RemoveAt(2);
            //    testlist[1] = "test_4";
            //}

            public class ArrayListExtend : List<object>
            {

                private int size = 0;

                public ArrayListExtend()
                    : base(10)
                {
                }

                internal override void DoAdd(object item)
                {
                    size++;
                    base.DoAdd(item);
                }

                internal override int Size => size;
            }

            public class MockArrayList : List<object>
            {
                internal override int Size => 0;
                //        public int size()
                //{
                //    return 0;
                //}

                internal override void DoRemoveRange(int index, int count)
                {
                    base.DoRemoveRange(index, count);
                }
                //        public void removeRange(int start, int end)
                //{
                //    super.removeRange(start, end);
                //}
            }

            [Test]
            public void Test_subclassing()
            {
                ArrayListExtend a = new ArrayListExtend();
                /*
                 * Regression test for subclasses that override size() (which used to
                 * cause an exception when growing 'a').
                 */
                for (int i = 0; i < 100; i++)
                {
                    a.Add(new Object());
                }
            }

            /**
             * Sets up the fixture, for example, open a network connection. This method
             * is called before a test is executed.
             */
            public override void SetUp()
            {
                base.SetUp();
                alist = new List<Integer>();
                for (int i = 0; i < objArray.Length; i++)
                    alist.Add((Integer)objArray[i]);
                olist = new List<object>();
                for (int i = 0; i < objArray.Length; i++)
                    olist.Add(objArray[i]);
            }

            //private class MockHashSet<T> : HashSet<T>
            //{
            //    private bool shrink = true;
            //    private readonly T[] firstItemBuffer = new T[1];
            //    public MockHashSet(IEnumerable<T> collection)
            //        : base(collection)
            //    { }

            //    internal override int Size
            //    {
            //        get
            //        {
            //            int result = base.Size;
            //            if (shrink)
            //            {
            //                if (result > 0)
            //                {
            //                    shrink = false;
            //                    this.CopyTo(firstItemBuffer, 0, 1);
            //                    Remove(firstItemBuffer[0]);
            //                    shrink = true;

            //                }
            //                //using (var i = GetEnumerator())
            //                //{
            //                //    i.next();
            //                //    i.remove();
            //                //}
            //            }
            //            return result;
            //        }
            //    }

            //    public T[] ToArray()
            //    {
            //        shrink = false;
            //        int size = Size;
            //        T[] buffer = new T[size];
            //        base.CopyTo(buffer);
            //        return buffer;
            //    }

            //}

            ///**
            // * Returns a collection that emulates another thread calling remove() each
            // * time the current thread calls size().
            // */
            //private ICollection<T> shrinksOnSize<T>(params T[] elements)
            //{
            //    return new MockHashSet<T>(elements);
            //}
        }

    }
}
