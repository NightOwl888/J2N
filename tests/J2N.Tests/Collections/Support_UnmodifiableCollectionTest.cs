using System;
using System.Collections.Generic;
using System.Linq;
using Integer = J2N.Numerics.Int32;

namespace J2N.Collections
{
    public class Support_UnmodifiableCollectionTest : TestCase
    {
        readonly ICollection<Integer> col;

        // must be a collection containing the Integers 0 to 99 (which will iterate
        // in order)

        public Support_UnmodifiableCollectionTest(String p1)
        //: base(p1)
        {
        }

        public Support_UnmodifiableCollectionTest(String p1, ICollection<Integer> c)
        //: base(p1)
        {
            col = c;
        }

        public void RunTest()
        {

            // contains
            assertTrue("UnmodifiableCollectionTest - should contain 0", col
                    .Contains(new Integer(0)));
            assertTrue("UnmodifiableCollectionTest - should contain 50", col
                    .Contains(new Integer(50)));
            assertTrue("UnmodifiableCollectionTest - should not contain 100", !col
                    .Contains(new Integer(100)));

            // containsAll
            HashSet<object> hs = new HashSet<object>();
            hs.Add(new Integer(0));
            hs.Add(new Integer(25));
            hs.Add(new Integer(99));
            assertTrue(
                    "UnmodifiableCollectionTest - should contain set of 0, 25, and 99",
                    col.Intersect(hs).Count() == hs.Count); // Contains all
            hs.Add(new Integer(100));
            assertTrue(
                    "UnmodifiableCollectionTest - should not contain set of 0, 25, 99 and 100",
                    col.Intersect(hs).Count() != hs.Count); // Doesn't contain all

            // isEmpty
            assertTrue("UnmodifiableCollectionTest - should not be empty", col.Count > 0);

            // iterator
            IEnumerator<Integer> it = col.GetEnumerator();
            SortedSet<Integer> ss = new SortedSet<Integer>();
            while (it.MoveNext())
            {
                ss.Add(it.Current);
            }
            it = ss.GetEnumerator();
            for (int counter = 0; it.MoveNext(); counter++)
            {
                int nextValue = it.Current;
                assertTrue(
                        "UnmodifiableCollectionTest - Iterator returned wrong value.  Wanted: "
                                + counter + " got: " + nextValue,
                        nextValue == counter);
            }

            // size
            assertTrue(
                    "UnmodifiableCollectionTest - returned wrong size.  Wanted 100, got: "
                            + col.Count, col.Count == 100);

            // toArray
            Object[] objArray;
            objArray = col.Cast<object>().ToArray();
            it = ss.GetEnumerator(); // J2N: Bug in Harmony, this needs to be reset to run
            for (int counter = 0; it.MoveNext(); counter++)
            {
                assertTrue(
                        "UnmodifiableCollectionTest - toArray returned incorrect array",
                        (Integer)objArray[counter] == it.Current);
            }

            // toArray (Object[])
            var intArray = new Integer[100];
            col.CopyTo(intArray, 0);
            it = ss.GetEnumerator(); // J2N: Bug in Harmony, this needs to be reset to run
            for (int counter = 0; it.MoveNext(); counter++)
            {
                assertTrue(
                        "UnmodifiableCollectionTest - CopyTo(object[], int) filled array incorrectly",
                        intArray[counter] == it.Current);
            }
        }
    }
}
