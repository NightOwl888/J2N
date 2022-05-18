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

using J2N.Collections.Generic;
using J2N.Collections.Generic.Extensions;
using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integer = J2N.Numerics.Int32;
using SCG = System.Collections.Generic;

namespace J2N.Collections
{
    public class Support_ListTest : TestCase
    {
        readonly SCG.IList<Integer> list; // must contain the Integers 0 to 99 in order

        public Support_ListTest(String p1)
        //    : base(p1)
        {
        }

        public Support_ListTest(String p1, SCG.IList<Integer> l)
        //    : base(p1)
        {
            list = l;
        }

        public void RunTest()
        {
            int hashCode = 1;
            for (int counter = 0; counter < 100; counter++)
            {
                Object elem;
                elem = list[counter];
                hashCode = 31 * hashCode + elem.GetHashCode();
                assertTrue("ListTest - get failed", elem
                        .Equals(new Integer(counter)));
            }
            assertTrue("ListTest - hashCode failed", hashCode == list.GetHashCode());

            list.Insert(50, new Integer(1000));
            assertTrue("ListTest - a) add with index failed--did not insert", list
                    [50].Equals(new Integer(1000)));
            assertTrue(
                    "ListTest - b) add with index failed--did not move following elements",
                    list[51].Equals(new Integer(50)));
            assertTrue(
                    "ListTest - c) add with index failed--affected previous elements",
                    list[49].Equals(new Integer(49)));

            list[50] = new Integer(2000);
            assertTrue("ListTest - a) set failed--did not set", list[50]
                    .Equals(new Integer(2000)));
            assertTrue("ListTest - b) set failed--affected following elements",
                    list[51].Equals(new Integer(50)));
            assertTrue("ListTest - c) set failed--affected previous elements", list
                    [49].Equals(new Integer(49)));

            list.RemoveAt(50);
            assertTrue("ListTest - a) remove with index failed--did not remove",
                    list[50].Equals(new Integer(50)));
            assertTrue(
                    "ListTest - b) remove with index failed--did not move following elements",
                    list[51].Equals(new Integer(51)));
            assertTrue(
                    "ListTest - c) remove with index failed--affected previous elements",
                    list[49].Equals(new Integer(49)));

            List<Integer> myList = new List<Integer>();
            myList.Add(new Integer(500));
            myList.Add(new Integer(501));
            myList.Add(new Integer(502));

            if (list is List<Integer> j2nList)
            {
                j2nList.InsertRange(50, myList);
            }
            else if (list is SubList<Integer> j2nWrapperSubList)
            {
                //j2nWrapperSubList.InsertRange(50, myList);
                for (int i = 0; i < myList.Count; i++)
                    j2nWrapperSubList.Insert(i + 50, myList[i]);
            }
            else if (list is SCG.List<Integer> scgList)
            {
                scgList.InsertRange(50, myList);
            }
            else
                throw new ArgumentException($"List type not supported in this test: {list.GetType()}");

            assertTrue("ListTest - a) addAll with index failed--did not insert",
                    list[50].Equals(new Integer(500)));
            assertTrue("ListTest - b) addAll with index failed--did not insert",
                    list[51].Equals(new Integer(501)));
            assertTrue("ListTest - c) addAll with index failed--did not insert",
                    list[52].Equals(new Integer(502)));
            assertTrue(
                    "ListTest - d) addAll with index failed--did not move following elements",
                    list[53].Equals(new Integer(50)));
            assertTrue(
                    "ListTest - e) addAll with index failed--affected previous elements",
                    list[49].Equals(new Integer(49)));

            SCG.IList<Integer> mySubList = list.GetView(50, 53 - 50);
            assertEquals(3, mySubList.Count);
            assertTrue(
                    "ListTest - a) sublist Failed--does not contain correct elements",
                    mySubList[0].Equals(new Integer(500)));
            assertTrue(
                    "ListTest - b) sublist Failed--does not contain correct elements",
                    mySubList[1].Equals(new Integer(501)));
            assertTrue(
                    "ListTest - c) sublist Failed--does not contain correct elements",
                    mySubList[2].Equals(new Integer(502)));

            t_listIterator(mySubList);

            mySubList.Clear();
            assertEquals("ListTest - Clearing the sublist did not remove the appropriate elements from the original list",
                    100, list.Count);

            t_listIterator(list);
            using (var li = list.GetEnumerator())
            {
                for (int counter = 0; li.MoveNext(); counter++)
                {
                    Object elem;
                    elem = li.Current;
                    assertTrue("ListTest - listIterator failed", elem
                            .Equals(new Integer(counter)));
                }
            }

            //new Support_CollectionTest("", list).RunTest(); // J2N TODO:

        }

        public void t_listIterator(SCG.IList<Integer> list)
        {
            //var li = list.listIterator(1);
            var li1 = list.Skip(1);
            assertTrue("listIterator(1)", li1.First() == list[1]);

            int orgSize = list.Count;
            var li = list.GetEnumerator();
            for (int i = 0; i <= orgSize; i++)
            {
                //if (i == 0)
                //{
                //    assertTrue("list iterator hasPrevious(): " + i, !li
                //            .hasPrevious());
                //}
                //else
                //{
                //    assertTrue("list iterator hasPrevious(): " + i, li
                //            .hasPrevious());
                //}
                if (i == list.Count)
                {
                    assertTrue("list iterator hasNext(): " + i, !li.MoveNext());
                }
                else
                {
                    assertTrue("list iterator hasNext(): " + i, li.MoveNext());
                }
                //assertTrue("list iterator nextIndex(): " + i, li.nextIndex() == i);
                //assertTrue("list iterator previousIndex(): " + i, li
                //        .previousIndex() == i - 1);
                //bool exception = false;
                //try
                //{
                //    assertTrue("list iterator next(): " + i, li.Current == list[i]);
                //}
                //catch (NoSuchElementException e)
                //{
                //    exception = true;
                //}
                //if (i == list.Count)
                //{
                //    assertTrue("list iterator next() exception: " + i, !li.MoveNext());
                //}
                //else
                //{
                //    assertTrue("list iterator next() exception: " + i, li.MoveNext());
                //}
            }

            //for (int i = orgSize - 1; i >= 0; i--)
            //{
            //    assertTrue("list iterator previous(): " + i, li.previous() == list
            //            .get(i));
            //    assertTrue("list iterator nextIndex()2: " + i, li.nextIndex() == i);
            //    assertTrue("list iterator previousIndex()2: " + i, li
            //            .previousIndex() == i - 1);
            //    if (i == 0)
            //    {
            //        assertTrue("list iterator hasPrevious()2: " + i, !li
            //                .hasPrevious());
            //    }
            //    else
            //    {
            //        assertTrue("list iterator hasPrevious()2: " + i, li
            //                .hasPrevious());
            //    }
            //    assertTrue("list iterator hasNext()2: " + i, li.hasNext());
            //}
            //boolean exception = false;
            //try
            //{
            //    li.previous();
            //}
            //catch (NoSuchElementException e)
            //{
            //    exception = true;
            //}
            //assertTrue("list iterator previous() exception", exception);

            Integer add1 = new Integer(600);
            Integer add2 = new Integer(601);
            list.Add(add1);
            assertTrue("list iterator add(), size()", list.Count == (orgSize + 1));
            // J2N: None of the below methods are supported in .NET, but we still
            // need to remove what we just added.
            list.Remove(add1);

            //assertEquals("list iterator add(), nextIndex()", 1, li.nextIndex());
            //assertEquals("list iterator add(), previousIndex()",
            //        0, li.previousIndex());
            //Object next = li.next();
            //assertTrue("list iterator add(), next(): " + next, next == list.get(1));
            //li.add(add2);
            //Object previous = li.previous();
            //assertTrue("list iterator add(), previous(): " + previous,
            //        previous == add2);
            //assertEquals("list iterator add(), nextIndex()2", 2, li.nextIndex());
            //assertEquals("list iterator add(), previousIndex()2",
            //        1, li.previousIndex());

            //li.remove();
            //assertTrue("list iterator remove(), size()",
            //        list.size() == (orgSize + 1));
            //assertEquals("list iterator remove(), nextIndex()", 2, li.nextIndex());
            //assertEquals("list iterator remove(), previousIndex()", 1, li
            //        .previousIndex());
            //assertTrue("list iterator previous()2", li.previous() == list.get(1));
            //assertTrue("list iterator previous()3", li.previous() == list.get(0));
            //assertTrue("list iterator next()2", li.next() == list.get(0));
            //li.remove();
            //assertTrue("list iterator hasPrevious()3", !li.hasPrevious());
            //assertTrue("list iterator hasNext()3", li.hasNext());
            //assertTrue("list iterator size()", list.size() == orgSize);
            //assertEquals("list iterator nextIndex()3", 0, li.nextIndex());
            //assertEquals("list iterator previousIndex()3", -1, li.previousIndex());
        }
    }
}
