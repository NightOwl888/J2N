using J2N.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace J2N.Collections
{
    public class TestUnmodifiableCollections : TestCase
    {
        private IList<object> ll;

        private ISet<object> s;

        private IDictionary<object, object> hm;

        private static object[] objArray = LoadObjectArray();

        private static object[] LoadObjectArray()
        {
            var objArray = new object[1000];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray[i] = new int?(i);
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
            var c = ll.ToUnmodifiableCollection();
            assertTrue("Returned collection is of incorrect size", c.Count == ll.Count);
            var i = ll.GetEnumerator();
            while (i.MoveNext())
                assertTrue("Returned list missing elements", c.Contains(i.Current));
            try
            {
                c.Add(new object());
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
                c.Remove(new object());
                fail("Allowed modification of collection");
            }
            catch (NotSupportedException e)
            {
                // Correct
            }

            var myCollection = new List<object>();
            myCollection.Add(new int?(20));
            myCollection.Add(null);
            c = myCollection.ToUnmodifiableCollection();
            assertTrue("Collection should contain null", c.Contains(null));
            assertTrue("Collection should contain Integer(20)", c
                    .Contains(new int?(20)));

            myCollection = new List<object>();
            for (int counter = 0; counter < 100; counter++)
            {
                myCollection.Add(objArray[counter]);
            }
            new Support_UnmodifiableCollectionTest("", (myCollection).ToUnmodifiableCollection()).RunTest();

#if FEATURE_SERIALIZABLE
            // Serialization
            var col = (CollectionExtensions.UnmodifiableCollection<object>)myCollection.ToUnmodifiableCollection();
            var clone = Clone(col);

            assertNotSame(col, clone);
            assertNotSame(col.collection, clone.collection);
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
            var c = ll.ToUnmodifiableList();
            // Ensure a NPE is thrown if the list is NULL
            try
            {
                ((IList<object>)null).ToUnmodifiableList();
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
                c.Add(new Object());
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
                c.Remove(new Object());
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
            c = smallList.ToUnmodifiableList();
            assertNull("First element should be null", c[0]);
            assertTrue("List should contain null", c.Contains(null));
            //assertTrue(
            //        "T1. Returned List should implement Random Access interface",
            //        c instanceof RandomAccess);

            smallList = new List<object>();
            for (int counter = 0; counter < 100; counter++)
            {
                smallList.Add(objArray[counter]);
            }
            var myList = smallList.ToUnmodifiableList();
            assertTrue("List should not contain null", !myList.Contains(null));
            //assertTrue(
            //        "T2. Returned List should implement Random Access interface",
            //        myList instanceof RandomAccess);

            assertTrue("get failed on unmodifiable list", myList[50].Equals(
                    new int?(50)));
            var listIterator = myList.GetEnumerator();
            for (int counter = 0; listIterator.MoveNext(); counter++)
            {
                assertTrue("List has wrong elements", ((int?)listIterator
                        .Current) == counter);
            }
            new Support_UnmodifiableCollectionTest("", smallList).RunTest();

#if FEATURE_SERIALIZABLE
            // Serialization
            var col = (ListExtensions.UnmodifiableList<object>)smallList.ToUnmodifiableList();
            var clone = Clone(col);

            assertNotSame(col, clone);
            assertNotSame(col.list, clone.list);
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
            var c = hm.ToUnmodifiableDictionary();
            assertTrue("Returned map is of incorrect size", c.Count == hm.Count);
            var i = hm.Keys.GetEnumerator();
            while (i.MoveNext())
            {
                Object x = i.Current;
                assertTrue("Returned map missing elements", c[x].Equals(
                        hm[x]));
            }
            try
            {
                c[new Object()] = "";
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
                c.Remove(new Object());
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

            var smallMap = new Dictionary<object, object>(); // J2N TODO: Need HashDictionary so we can support null keys here
            //smallMap[null] = new long?(30); // J2N TODO: Need HashDictionary so we can support null keys here
            smallMap[new long?(25)] = null;
            var unmodMap = smallMap.ToUnmodifiableDictionary();

            assertNull("Trying to use a null value in map failed", unmodMap[new long?(25)]);
            //assertTrue("Trying to use a null key in map failed", unmodMap[null] // J2N TODO: Need HashDictionary so we can support null keys here
            //        .Equals(new long?(30)));

            smallMap = new Dictionary<object, object>();
            for (int counter = 0; counter < 100; counter++)
            {
                smallMap[objArray[counter].ToString()] = objArray[counter];
            }
            unmodMap = smallMap.ToUnmodifiableDictionary();
            new Support_UnmodifiableMapTest("", unmodMap).RunTest();

#if FEATURE_SERIALIZABLE
            // Serialization
            var col = (DictionaryExtensions.UnmodifiableDictionary<object, object>)unmodMap;
            var clone = Clone(col);

            assertNotSame(col, clone);
            assertNotSame(col.dictionary, clone.dictionary);
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
            var c = s.ToUnmodifiableSet();
            assertTrue("Returned set is of incorrect size", c.Count == s.Count);
            var i = ll.GetEnumerator();
            while (i.MoveNext())
                assertTrue("Returned set missing elements", c.Contains(i.Current));
            try
            {
                c.Add(new Object());
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
                c.Remove(new Object());
                fail("Allowed modification of set");
            }
            catch (NotSupportedException e)
            {
                // Correct
            }

            var mySet = new HashSet<object>().ToUnmodifiableSet();
            assertTrue("Should not contain null", !mySet.Contains(null));
            mySet = (new HashSet<object> { null }).ToUnmodifiableSet();
            assertTrue("Should contain null", mySet.Contains(null));

            mySet = new SortedSet<object>();
            for (int counter = 0; counter < 100; counter++)
            {
                mySet.Add(objArray[counter]);
            }
            new Support_UnmodifiableCollectionTest("", (mySet).ToUnmodifiableSet()).RunTest();

#if FEATURE_SERIALIZABLE
            // Serialization
            var col = (SetExtensions.UnmodifiableSet<object>)mySet.ToUnmodifiableSet();
            var clone = Clone(col);

            assertNotSame(col, clone);
            assertNotSame(col.set, clone.set);
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
            var uc = al.ToUnmodifiableCollection();
            assertEquals("[a, b]", uc.ToString());
            var m = new Dictionary<string, string>
            {
                ["one"] = "1",
                ["two"] = "2"
            };
            var um = m.ToUnmodifiableDictionary();
            assertEquals("{one=1, two=2}", um.ToString());
        }


        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            ll = new List<object>();
            //myll = new List<object>();
            s = new HashSet<object>();
            //mys = new HashSet<object>();
            //reversedLinkedList = new LinkedList(); // to be sorted in reverse order
            //myReversedLinkedList = new LinkedList(); // to be sorted in reverse
            // order
            hm = new Dictionary<object, object>();
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
