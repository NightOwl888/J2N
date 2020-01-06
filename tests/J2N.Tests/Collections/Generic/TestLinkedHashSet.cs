using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace J2N.Collections.Generic
{
    public class TestLinkedHashSet : TestCase
    {
        LinkedHashSet<object> hs;

        static object[] objArray = LoadObjArray();

        private static object[] LoadObjArray()
        {
            var objArray = new object[1000];
            for (int i = 0; i < objArray.Length; i++)
                objArray[i] = new int?(i);
            return objArray;
        }

        /**
         * @tests java.util.LinkedHashSet#LinkedHashSet()
         */
        [Test]
        public void Test_Constructor()
        {
            // Test for method java.util.LinkedHashSet()
            LinkedHashSet<object> hs2 = new LinkedHashSet<object>();
            assertEquals("Created incorrect LinkedHashSet", 0, hs2.Count);
        }

#if FEATURE_HASHSET_CAPACITY

        /**
         * @tests java.util.LinkedHashSet#LinkedHashSet(int)
         */
        [Test]
        public void Test_ConstructorI()
        {
            // Test for method java.util.LinkedHashSet(int)
            LinkedHashSet<object> hs2 = new LinkedHashSet<object>(5);
            assertEquals("Created incorrect LinkedHashSet", 0, hs2.Count);
            try
            {
                new LinkedHashSet<object>(-1);
            }
            catch (ArgumentException e)
            {
                return;
            }
            fail(
                    "Failed to throw IllegalArgumentException for capacity < 0");
        }

#endif

        ///**
        // * @tests java.util.LinkedHashSet#LinkedHashSet(int, float)
        // */[Test]
        //public void Test_ConstructorIF()
        //{
        //    // Test for method java.util.LinkedHashSet(int, float)
        //    LinkedHashSet<object> hs2 = new LinkedHashSet<object>(5/*, (float)0.5*/);
        //    assertEquals("Created incorrect LinkedHashSet", 0, hs2.size());
        //    try
        //    {
        //        new LinkedHashSet<object>(0, 0);
        //    }
        //    catch (ArgumentException e)
        //    {
        //        return;
        //    }
        //    fail(
        //            "Failed to throw IllegalArgumentException for initial load factor <= 0");
        //}

        /**
         * @tests java.util.LinkedHashSet#LinkedHashSet(java.util.Collection)
         */
        [Test]
        public void Test_ConstructorLjava_util_Collection()
        {
            // Test for method java.util.LinkedHashSet(java.util.Collection)
            LinkedHashSet<object> hs2 = new LinkedHashSet<object>(objArray);
            for (int counter = 0; counter < objArray.Length; counter++)
                assertTrue("LinkedHashSet does not contain correct elements", hs
                        .Contains(objArray[counter]));
            assertTrue("LinkedHashSet created from collection incorrect size", hs2
                    .Count == objArray.Length);
        }

        /**
         * @tests java.util.LinkedHashSet#add(java.lang.Object)
         */
        [Test]
        public void Test_addLjava_lang_Object()
        {
            // Test for method boolean java.util.LinkedHashSet.add(java.lang.Object)
            int size = hs.Count;
            hs.Add(new int?(8));
            assertTrue("Added element already contained by set", hs.Count == size);
            hs.Add(new int?(-9));
            assertTrue("Failed to increment set size after add",
                    hs.Count == size + 1);
            assertTrue("Failed to add element to set", hs.Contains(new int?(-9)));
        }

        /**
         * @tests java.util.LinkedHashSet#clear()
         */
        [Test]
        public void Test_clear()
        {
            // Test for method void java.util.LinkedHashSet.clear()
            var orgSet = new LinkedHashSet<object>(hs, EqualityComparer<object>.Default);
            hs.Clear();
            var i = orgSet.GetEnumerator();
            assertEquals("Returned non-zero size after clear", 0, hs.Count);
            while (i.MoveNext())
                assertTrue("Failed to clear set", !hs.Contains(i.Current));
        }

        /**
         * @tests java.util.LinkedHashSet#clone()
         */
        [Test]
        public void Test_clone()
        {
            // Test for method java.lang.Object java.util.LinkedHashSet.clone()
            var hs2 = new LinkedHashSet<object>(hs, EqualityComparer<object>.Default);
            assertTrue("clone returned an equivalent LinkedHashSet", hs != hs2);
            assertTrue("clone did not return an equal LinkedHashSet", hs
                    .Equals(hs2));
        }

        /**
         * @tests java.util.LinkedHashSet#contains(java.lang.Object)
         */
        [Test]
        public void Test_containsLjava_lang_Object()
        {
            // Test for method boolean
            // java.util.LinkedHashSet.contains(java.lang.Object)
            assertTrue("Returned false for valid object", hs.Contains(objArray[90]));
            assertTrue("Returned true for invalid Object", !hs
                    .Contains(new Object()));

            LinkedHashSet<object> s = new LinkedHashSet<object>();
            s.Add(null);
            assertTrue("Cannot handle null", s.Contains(null));
        }

        /**
         * @tests java.util.LinkedHashSet#isEmpty()
         */
        [Test]
        public void Test_isEmpty()
        {
            // Test for method boolean java.util.LinkedHashSet.isEmpty()
            assertTrue("Empty set returned false", !(new LinkedHashSet<object>().Any()));
            assertTrue("Non-empty set returned true", hs.Any());
        }

        /**
         * @tests java.util.LinkedHashSet#iterator()
         */
        [Test]
        public void Test_iterator()
        {
            // Test for method java.util.Iterator java.util.LinkedHashSet.iterator()
            var i = hs.GetEnumerator();
            int x = 0;
            int j;
            for (j = 0; i.MoveNext(); j++)
            {
                Object oo = i.Current;
                if (oo != null)
                {
                    int? ii = (int?)oo;
                    assertTrue("Incorrect element found", ii.Value == j);
                }
                else
                {
                    assertTrue("Cannot find null", hs.Contains(oo));
                }
                ++x;
            }
            assertTrue("Returned iteration of incorrect size", hs.Count == x);

            LinkedHashSet<object> s = new LinkedHashSet<object>();
            s.Add(null);
            assertNull("Cannot handle null", s.First());
        }

        /**
         * @tests java.util.LinkedHashSet#remove(java.lang.Object)
         */
        [Test]
        public void Test_removeLjava_lang_Object()
        {
            // Test for method boolean
            // java.util.LinkedHashSet.remove(java.lang.Object)
            int size = hs.Count;
            hs.Remove(new int?(98));
            assertTrue("Failed to remove element", !hs.Contains(new int?(98)));
            assertTrue("Failed to decrement set size", hs.Count == size - 1);

            var s = new LinkedHashSet<object>();
            s.Add(null);
            assertTrue("Cannot handle null", s.Remove(null));
        }

        /**
         * @tests java.util.LinkedHashSet#size()
         */
        [Test]
        public void Test_size()
        {
            // Test for method int java.util.LinkedHashSet.size()
            assertTrue("Returned incorrect size", hs.Count == (objArray.Length + 1));
            hs.Clear();
            assertEquals("Cleared set returned non-zero size", 0, hs.Count);
        }

        [Test]
        public void Test_InsertionOrder()
        {
            var set = new LinkedHashSet<string>(StringComparer.Ordinal);
            set.Add("one");
            set.Add("two");
            set.Add("three");

            var i = set.GetEnumerator();
            assertTrue(i.MoveNext());
            assertEquals("one", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("two", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("three", i.Current);
            assertFalse(i.MoveNext());

            set.UnionWith(new string[] { "four", "five", "six" });

            i = set.GetEnumerator();
            assertTrue(i.MoveNext());
            assertEquals("one", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("two", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("three", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("four", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("five", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("six", i.Current);
            assertFalse(i.MoveNext());

            set.ExceptWith(new string[] { "two", "four" });

            i = set.GetEnumerator();
            assertTrue(i.MoveNext());
            assertEquals("one", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("three", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("five", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("six", i.Current);
            assertFalse(i.MoveNext());

            set.UnionWith(new string[] { "two", "four" }); // Make sure these get added to the end

            i = set.GetEnumerator();
            assertTrue(i.MoveNext());
            assertEquals("one", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("three", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("five", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("six", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("two", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("four", i.Current);
            assertFalse(i.MoveNext());

            set.SymmetricExceptWith(new string[] { "seven", "nine", "three", "five", "eight" });

            i = set.GetEnumerator();
            assertTrue(i.MoveNext());
            assertEquals("one", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("six", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("two", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("four", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("seven", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("nine", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("eight", i.Current);
            assertFalse(i.MoveNext());

            set.Remove("two");

            i = set.GetEnumerator();
            assertTrue(i.MoveNext());
            assertEquals("one", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("six", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("four", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("seven", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("nine", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("eight", i.Current);
            assertFalse(i.MoveNext());

            set.Add("two");

            i = set.GetEnumerator();
            assertTrue(i.MoveNext());
            assertEquals("one", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("six", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("four", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("seven", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("nine", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("eight", i.Current);
            assertTrue(i.MoveNext());
            assertEquals("two", i.Current);
            assertFalse(i.MoveNext());
        }

        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            hs = new LinkedHashSet<object>(EqualityComparer<object>.Default);
            for (int i = 0; i < objArray.Length; i++)
                hs.Add(objArray[i]);
            hs.Add(null);
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        public override void TearDown()
        {
        }


        public class JDK7Basic : TestCase
        {
            static Random rnd = new Random(666);

            [Test]
            public void TestBasic()
            {
                int numItr = 500;
                int setSize = 500;

                for (int i = 0; i < numItr; i++)
                {
                    ISet<int> s1 = new LinkedHashSet<int>();
                    AddRandoms(s1, setSize);

                    ISet<int> s2 = new LinkedHashSet<int>();
                    AddRandoms(s2, setSize);

                    ISet<int> intersection = clone(s1);
                    intersection.IntersectWith(s2);
                    ISet<int> diff1 = clone(s1); diff1.ExceptWith(s2);
                    ISet<int> diff2 = clone(s2); diff2.ExceptWith(s1);
                    ISet<int> union = clone(s1); union.UnionWith(s2);

                    //if (diff1.ExceptWith(diff2))
                    //    throw new Exception("Set algebra identity 2 failed");
                    //if (diff1.ExceptWith(intersection))
                    //    throw new Exception("Set algebra identity 3 failed");
                    //if (diff2.ExceptWith(diff1))
                    //    throw new Exception("Set algebra identity 4 failed");
                    //if (diff2.ExceptWith(intersection))
                    //    throw new Exception("Set algebra identity 5 failed");
                    //if (intersection.ExceptWith(diff1))
                    //    throw new Exception("Set algebra identity 6 failed");
                    //if (intersection.ExceptWith(diff1))
                    //    throw new Exception("Set algebra identity 7 failed");

                    int expected = diff1.Count;
                    diff1.ExceptWith(diff2);
                    assertEquals("Set algebra identity 2 failed", expected, diff1.Count);

                    expected = diff1.Count;
                    diff1.ExceptWith(intersection);
                    assertEquals("Set algebra identity 3 failed", expected, diff1.Count);

                    expected = diff2.Count;
                    diff2.ExceptWith(diff1);
                    assertEquals("Set algebra identity 4 failed", expected, diff2.Count);

                    expected = diff2.Count;
                    diff2.ExceptWith(intersection);
                    assertEquals("Set algebra identity 5 failed", expected, diff2.Count);

                    expected = intersection.Count;
                    intersection.ExceptWith(diff1);
                    assertEquals("Set algebra identity 6 failed", expected, intersection.Count);

                    expected = intersection.Count;
                    intersection.ExceptWith(diff1);
                    assertEquals("Set algebra identity 7 failed", expected, intersection.Count);

                    intersection.UnionWith(diff1); intersection.UnionWith(diff2);

                    assertTrue("Set algebra identity 1 failed", intersection.Equals(union)); // .NET - test structural equality (this could also be done with SetEquals())

                    assertEquals("Incorrect hashCode computation.", new LinkedHashSet<int>(union).GetHashCode(), union.GetHashCode());

                    var e = union.GetEnumerator();
                    while (e.MoveNext())
                        assertTrue("Couldn't remove element from copy.", intersection.Remove(e.Current));

                    assertFalse("Copy nonempty after deleting all elements.", intersection.Any());

                    e = union.GetEnumerator();
                    while (e.MoveNext())
                    {
                        int o = e.Current;
                        assertTrue("Set doesn't contain one of its elements.", union.Contains(o));

                        // Not possible in .NET
                        //e.Remove();
                        //    if (union.contains(o))
                        //        throw new Exception("Set contains element after deletion.");
                    }
                    // Since we couldn't do the above op, we need to Clear() manually
                    union.Clear();

                    assertFalse("Set nonempty after deleting all elements.", union.Any());

                    s1.Clear();
                    assertFalse("Set nonempty after clear.", s1.Any());

                }
                Console.Out.WriteLine("Success.");
            }

            static ISet<int> clone(ISet<int> s)
            {
                ISet<int> clone;
                int method = rnd.Next(2);
                clone = //(method==0 ?  (ISet<int>) ((LinkedHashSet<int>)s).clone() :
#if FEATURE_SERIALIZABLE
                    (method == 0 ?
#endif
                    new LinkedHashSet<int>(s, EqualityComparer<int>.Default)
#if FEATURE_SERIALIZABLE
                    : Clone(s))
#endif
                    ;

                if (!s.Equals(clone))
                    throw new Exception("Set not equal to copy: " + method);
                if (!s.IsSupersetOf(clone))
                    throw new Exception("Set does not contain copy.");
                if (!clone.IsSupersetOf(s))
                    throw new Exception("Copy does not contain set.");
                return clone;
            }

            static void AddRandoms(ISet<int> s, int n)
            {
                for (int i = 0; i < n; i++)
                {
                    int r = rnd.Next() % n;
                    int e = (r < 0 ? -r : r);

                    int preSize = s.Count;
                    bool prePresent = s.Contains(e);
                    bool added = s.Add(e);
                    if (!s.Contains(e))
                        throw new Exception("Element not present after addition.");
                    if (added == prePresent)
                        throw new Exception("added == alreadyPresent");
                    int postSize = s.Count;
                    if (added && preSize == postSize)
                        throw new Exception("Add returned true, but size didn't change.");
                    if (!added && preSize != postSize)
                        throw new Exception("Add returned false, but size changed.");
                }
            }
        }

    }

}