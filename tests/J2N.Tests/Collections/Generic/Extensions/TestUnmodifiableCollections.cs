using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Integer = J2N.Numerics.Int32;
using Long = J2N.Numerics.Int64;

namespace J2N.Collections.Generic.Extensions
{
    public class TestUnmodifiableCollections : TestCase
    {
        private IList<Integer> ll;

        private ISet<Integer> s;

        private IDictionary<string, Integer> hm;

        private static readonly Integer[] objArray = LoadObjectArray();

        private static Integer[] LoadObjectArray()
        {
            var objArray = new Integer[1000];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray[i] = new Integer(i);
            }
            return objArray;
        }

        /**
        * @tests java.util.Collections#unmodifiableCollection(java.util.Collection)
        */
        [Test]
        public void Test_unmodifiableCollectionLjava_util_Collection()
        {
            // Test for method java.util.Collection
            // java.util.Collections.unmodifiableCollection(java.util.Collection)
            bool exception = false;
            var c = (IList<Integer>)ll.AsReadOnly();
            assertTrue("Returned collection is of incorrect size", c.Count == ll.Count);
            var i = ll.GetEnumerator();
            while (i.MoveNext())
                assertTrue("Returned list missing elements", c.Contains(i.Current));
            try
            {
                c.Add(new Integer(0));
            }
            catch (NotSupportedException e)
            {
                exception = true;
                // Correct
            }
            if (!exception)
            {
                fail("Allowed modification of collection");
            }

            try
            {
                c.Remove(new Integer(0));
                fail("Allowed modification of collection");
            }
            catch (NotSupportedException e)
            {
                // Correct
            }

            var myCollection = new List<Integer>();
            myCollection.Add(new Integer(20));
            myCollection.Add(null);
            c = myCollection.AsReadOnly();
            assertTrue("Collection should contain null", c.Contains(null));
            assertTrue("Collection should contain Integer(20)", c
                    .Contains(new Integer(20)));

            myCollection = new List<Integer>();
            for (int counter = 0; counter < 100; counter++)
            {
                myCollection.Add(objArray[counter]);
            }
            new Support_UnmodifiableCollectionTest("", (myCollection).AsReadOnly()).RunTest();

#if FEATURE_SERIALIZABLE
            // Serialization
            var col = myCollection.AsReadOnly();
            var clone = Clone(col);

            assertNotSame(col, clone);
            assertNotSame(col.List, clone.List);
            assertEquals(col.Count, clone.Count);
            
            using (var it1 = col.GetEnumerator())
            using (var it2 = col.GetEnumerator())
            {
                while (it1.MoveNext() && it2.MoveNext())
                    assertTrue(
                        "UnmodifiableCollectionTest - Deserialized clone returned incorrect values",
                        it1.Current == it2.Current);
            }
#endif
        }


        /**
         * @tests java.util.Collections#unmodifiableList(java.util.List)
         */
        [Test]
        public void Test_unmodifiableListLjava_util_List()
        {
            // Test for method java.util.List
            // java.util.Collections.unmodifiableList(java.util.List)

            // test with a Sequential Access List
            bool exception = false;
            var c = (IList<Integer>)ll.AsReadOnly();
            // Ensure a NPE is thrown if the list is NULL
            try
            {
                ((IList<Integer>)null).AsReadOnly();
                fail("Expected NullPointerException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
            }

            assertTrue("Returned list is of incorrect size", c.Count == ll.Count);
            //assertTrue(
            //        "Returned List should not implement Random Access interface",
            //        !(c instanceof RandomAccess));

            var i = ll.GetEnumerator();
            while (i.MoveNext())
                assertTrue("Returned list missing elements", c.Contains(i.Current));
            try
            {
                c.Add(new Integer(0));
            }
            catch (NotSupportedException e)
            {
                exception = true;
                // Correct
            }
            if (!exception)
            {
                fail("Allowed modification of list");
            }

            try
            {
                c.Remove(new Integer(0));
                fail("Allowed modification of list");
            }
            catch (NotSupportedException e)
            {
                // Correct
            }

            // test with a Random Access List
            var smallList = new List<object>();
            smallList.Add(null);
            smallList.Add("yoink");
            var c2 = smallList.AsReadOnly();
            assertNull("First element should be null", c2[0]);
            assertTrue("List should contain null", c2.Contains(null));
            //assertTrue(
            //        "T1. Returned List should implement Random Access interface",
            //        c2 instanceof RandomAccess);

            var smallList2 = new List<Integer>();
            for (int counter = 0; counter < 100; counter++)
            {
                smallList2.Add(objArray[counter]);
            }
            var myList = smallList2.AsReadOnly();
            assertTrue("List should not contain null", !myList.Contains(null));
            //assertTrue(
            //        "T2. Returned List should implement Random Access interface",
            //        myList instanceof RandomAccess);

            assertTrue("get failed on unmodifiable list", myList[50].Equals(
                    new Integer(50)));
            var listIterator = myList.GetEnumerator();
            for (int counter = 0; listIterator.MoveNext(); counter++)
            {
                assertTrue("List has wrong elements", listIterator
                        .Current == counter);
            }
            new Support_UnmodifiableCollectionTest("", smallList2).RunTest();

#if FEATURE_SERIALIZABLE
            // Serialization
            var col = smallList2.AsReadOnly();
            var clone = Clone(col);

            assertNotSame(col, clone);
            assertNotSame(col.List, clone.List);
            assertEquals(col.Count, clone.Count);

            using (var it1 = col.GetEnumerator())
            using (var it2 = col.GetEnumerator())
            {
                while (it1.MoveNext() && it2.MoveNext())
                    assertTrue(
                        "UnmodifiableListTest - Deserialized clone returned incorrect values",
                        it1.Current == it2.Current);
            }
#endif
        }

        /**
         * @tests java.util.Collections#unmodifiableMap(java.util.Map)
         */
        [Test]
        public void Test_unmodifiableMapLjava_util_Map()
        {
            // Test for method java.util.Map
            // java.util.Collections.unmodifiableMap(java.util.Map)
            bool exception = false;
            var c = (IDictionary<string, Integer>)hm.AsReadOnly();
            assertTrue("Returned map is of incorrect size", c.Count == hm.Count);
            var i = hm.Keys.GetEnumerator();
            while (i.MoveNext())
            {
                var x = i.Current;
                assertTrue("Returned map missing elements", c[x].Equals(
                        hm[x]));
            }
            try
            {
                c[string.Empty] = new Integer(0);
            }
            catch (NotSupportedException e)
            {
                exception = true;
                // Correct
            }
            assertTrue("Allowed modification of map", exception);

            exception = false;
            try
            {
                c.Remove(string.Empty);
            }
            catch (NotSupportedException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Allowed modification of map", exception);

            exception = false;
            var it = c.GetEnumerator();
            var entry = it.Current;
            //try
            //{
            //    entry.Value = ("modified"); // J2N: In .NET, Value property doesn't have a setter
            //}
            //catch (NotSupportedException e)
            //{
            //    // Correct
            //    exception = true;
            //}
            //assertTrue("Allowed modification of entry", exception);

            exception = false;
            object[] array = c.Cast<object>().ToArray();
            //try
            //{
            //    ((Map.Entry)array[0]).setValue("modified"); // J2N: In .NET, Value property doesn't have a setter
            //}
            //catch (NotSupportedException e)
            //{
            //    // Correct
            //    exception = true;
            //}
            //assertTrue("Allowed modification of array entry", exception);

            exception = false;
            var array2 = c.ToArray();
            //try
            //{
            //    array2[0].setValue("modified"); // J2N: In .NET, Value property doesn't have a setter
            //}
            //catch (NotSupportedException e)
            //{
            //    // Correct
            //    exception = true;
            //}
            //assertTrue("Allowed modification of array entry2", exception);

            var smallMap = new Dictionary<string, Integer>();
            smallMap[null] = new Integer(30);
            //smallMap[new long?(25)] = null;
            var unmodMap = smallMap.AsReadOnly();

            //assertNull("Trying to use a null value in map failed", unmodMap[new long?(25)]);
            assertTrue("Trying to use a null key in map failed", unmodMap[null]
                    .Equals(new Integer(30)));

            smallMap = new Dictionary<string, Integer>();
            for (int counter = 0; counter < 100; counter++)
            {
                smallMap[objArray[counter].ToString()] = objArray[counter];
            }
            unmodMap = smallMap.AsReadOnly();
            new Support_UnmodifiableMapTest("", unmodMap).RunTest();

#if FEATURE_SERIALIZABLE
            // Serialization
            var col = unmodMap;
            var clone = Clone(col);

            assertNotSame(col, clone);
            assertNotSame(col.Dictionary, clone.Dictionary);
            assertEquals(col.Count, clone.Count);

            using (var it1 = col.GetEnumerator())
            using (var it2 = col.GetEnumerator())
            {
                while (it1.MoveNext() && it2.MoveNext())
                    assertTrue(
                        "UnmodifiableDictionaryTest - Deserialized clone returned incorrect values",
                        it1.Current.Equals(it2.Current));
            }
#endif
        }

        /**
         * @tests java.util.Collections#unmodifiableSet(java.util.Set)
         */
        [Test]
        public void Test_unmodifiableSetLjava_util_Set()
        {
            // Test for method java.util.Set
            // java.util.Collections.unmodifiableSet(java.util.Set)
            bool exception = false;
            ISet<Integer> c = s.AsReadOnly();
            assertTrue("Returned set is of incorrect size", c.Count == s.Count);
            var i = ll.GetEnumerator();
            while (i.MoveNext())
                assertTrue("Returned set missing elements", c.Contains(i.Current));
            try
            {
                c.Add(new Integer(0));
            }
            catch (NotSupportedException e)
            {
                exception = true;
                // Correct
            }
            if (!exception)
            {
                fail("Allowed modification of set");
            }
            try
            {
                c.Remove(new Integer(0));
                fail("Allowed modification of set");
            }
            catch (NotSupportedException e)
            {
                // Correct
            }

            ISet<Integer> mySet = new HashSet<Integer>().AsReadOnly();
            assertTrue("Should not contain null", !mySet.Contains(null));
            mySet = (new HashSet<Integer> { null }).AsReadOnly();
            assertTrue("Should contain null", mySet.Contains(null));

            mySet = new SortedSet<Integer>();
            for (int counter = 0; counter < 100; counter++)
            {
                mySet.Add(objArray[counter]);
            }
            new Support_UnmodifiableCollectionTest("", (mySet).AsReadOnly()).RunTest();

#if FEATURE_SERIALIZABLE
            // Serialization
            var col = mySet.AsReadOnly();
            var clone = Clone(col);

            assertNotSame(col, clone);
            assertNotSame(col.Items, clone.Items);
            assertEquals(col.Count, clone.Count);

            assertTrue("UnmodifiableSetTest - Deserialized clone returned incorrect values", 
                col.SetEquals(clone));
#endif
        }


        /**
         * Test unmodifiable objects toString methods
         */
        [Test]
        public void Test_unmodifiable_toString_methods()
        {
            // Regression for HARMONY-552
            var al = new List<object>();
            al.Add("a");
            al.Add("b");
            var uc = al.AsReadOnly();
            assertEquals("[a, b]", uc.ToString());
            var m = new Dictionary<string, string>
            {
                ["one"] = "1",
                ["two"] = "2"
            };
            var um = m.AsReadOnly();
            assertEquals("{one=1, two=2}", um.ToString());
        }


        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            ll = new List<Integer>();
            //myll = new List<object>();
            s = new HashSet<Integer>();
            //mys = new HashSet<object>();
            //reversedLinkedList = new LinkedList(); // to be sorted in reverse order
            //myReversedLinkedList = new LinkedList(); // to be sorted in reverse
            // order
            hm = new Dictionary<string, Integer>();
            for (int i = 0; i < objArray.Length; i++)
            {
                ll.Add(objArray[i]);
                //myll.Add(myobjArray[i]);
                s.Add(objArray[i]);
                //mys.Add(myobjArray[i]);
                //reversedLinkedList.Add(objArray[objArray.Length - i - 1]);
                //myReversedLinkedList.Add(myobjArray[myobjArray.Length - i - 1]);
                hm[objArray[i].ToString()] = objArray[i];
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
