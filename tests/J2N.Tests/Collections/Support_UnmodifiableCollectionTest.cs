using System;
using System.Collections.Generic;
using System.Linq;

namespace J2N.Collections
{
    public class Support_UnmodifiableCollectionTest : TestCase
    {
        ICollection<object> col;

        // must be a collection containing the Integers 0 to 99 (which will iterate
        // in order)

        public Support_UnmodifiableCollectionTest(String p1)
        //: base(p1)
        {
        }

        public Support_UnmodifiableCollectionTest(String p1, ICollection<object> c)
        //: base(p1)
        {
            col = c;
        }

        public void RunTest()
        {

            // contains
            assertTrue("UnmodifiableCollectionTest - should contain 0", col
                    .Contains(new int?(0)));
            assertTrue("UnmodifiableCollectionTest - should contain 50", col
                    .Contains(new int?(50)));
            assertTrue("UnmodifiableCollectionTest - should not contain 100", !col
                    .Contains(new int?(100)));

            // containsAll
            HashSet<object> hs = new HashSet<object>();
            hs.Add(new int?(0));
            hs.Add(new int?(25));
            hs.Add(new int?(99));
            assertTrue(
                    "UnmodifiableCollectionTest - should contain set of 0, 25, and 99",
                    col.Intersect(hs).Count() == hs.Count); // Contains all
            hs.Add(new int?(100));
            assertTrue(
                    "UnmodifiableCollectionTest - should not contain set of 0, 25, 99 and 100",
                    col.Intersect(hs).Count() != hs.Count); // Doesn't contain all

            // isEmpty
            assertTrue("UnmodifiableCollectionTest - should not be empty", col.Any());

            // iterator
            IEnumerator<object> it = col.GetEnumerator();
            SortedSet<object> ss = new SortedSet<object>();
            while (it.MoveNext())
            {
                ss.Add(it.Current);
            }
            it = ss.GetEnumerator();
            for (int counter = 0; it.MoveNext(); counter++)
            {
                int nextValue = ((int?)it.Current).Value;
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
                        objArray[counter] == it.Current);
            }

            // toArray (Object[])
            var intArray = new object[100];
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
