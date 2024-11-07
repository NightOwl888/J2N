using System;
using System.Collections.Generic;
using System.Linq;
using Integer = J2N.Numerics.Int32;

namespace J2N.Collections
{
    public class Support_UnmodifiableMapTest : TestCase
    {
        readonly IDictionary<string, Integer> map;

        // must be a map containing the string keys "0"-"99" paired with the Integer
        // values Integer(0) to Integer(99)

        public Support_UnmodifiableMapTest(String p1)
        //: base(p1)
        {
        }

        public Support_UnmodifiableMapTest(String p1, IDictionary<string, Integer> m)
        //: base(p1)
        {
            map = m;
        }

        public void RunTest()
        {
            // containsKey
            assertTrue("UnmodifiableMapTest - Should contain the key \"0\"", map
                    .ContainsKey("0"));
            assertTrue("UnmodifiableMapTest - Should contain the key \"50\"", map
                    .ContainsKey("50"));
            assertTrue("UnmodifiableMapTest - Should not contain the key \"100\"",
                    !map.ContainsKey("100"));

            // containsValue
            assertTrue("UnmodifiableMapTest - Should contain the value 0", map
                    .Values.Contains(new int?(0)));
            assertTrue("UnmodifiableMapTest - Should contain the value 50", map
                    .Values.Contains(new int?(50)));
            assertTrue("UnmodifiableMapTest - Should not contain value 100", !map
                    .Values.Contains(new int?(100)));

            // entrySet
            //Set <?> entrySet = map.entrySet();
            var entrySetIterator = map.GetEnumerator();
            int myCounter = 0;
            while (entrySetIterator.MoveNext())
            {
                var me = entrySetIterator.Current;
                assertTrue("UnmodifiableMapTest - Incorrect Map.Entry returned",
                        map[me.Key].Equals(me.Value));
                myCounter++;
            }
            assertEquals("UnmodifiableMapTest - Incorrect number of map entries returned",
                    100, myCounter);

            // get
            assertTrue("UnmodifiableMapTest - getting \"0\" didn't return 0",
                    (int?)map["0"] == 0);
            assertTrue("UnmodifiableMapTest - getting \"50\" didn't return 0",
                    (int?)map["0"] == 0);
            assertNull("UnmodifiableMapTest - getting \"100\" didn't return null",
                    map.TryGetValue("100", out Integer value) ? value : null);

            // isEmpty
            assertTrue(
                    "UnmodifiableMapTest - should have returned false to isEmpty",
                    map.Count != 0);

            // keySet
            var keySet = map.Keys;
            t_KeySet(keySet);

            // size
            assertTrue("Size should return 100, returned: " + map.Count, map
                    .Count == 100);

            // values
            new Support_UnmodifiableCollectionTest("Unmod--from map test", map
                    .Values);

        }

        void t_KeySet(ICollection<string> keySet)
        {
            // keySet should be a set of the strings "0" to "99"

            // contains
            assertTrue("UnmodifiableMapTest - keySetTest - should contain \"0\"",
                    keySet.Contains("0"));
            assertTrue("UnmodifiableMapTest - keySetTest - should contain \"50\"",
                    keySet.Contains("50"));
            assertTrue(
                    "UnmodifiableMapTest - keySetTest - should not contain \"100\"",
                    !keySet.Contains("100"));

            // containsAll
            HashSet<String> hs = new HashSet<String>();
            hs.Add("0");
            hs.Add("25");
            hs.Add("99");
            assertTrue(
                    "UnmodifiableMapTest - keySetTest - should contain set of \"0\", \"25\", and \"99\"",
                    keySet.Intersect(hs).Count() == hs.Count); // Contains all
            hs.Add("100");
            assertTrue(
                    "UnmodifiableMapTest - keySetTest - should not contain set of \"0\", \"25\", \"99\" and \"100\"",
                    keySet.Intersect(hs).Count() != hs.Count); // Doesn't contain all

            // isEmpty
            assertTrue("UnmodifiableMapTest - keySetTest - should not be empty",
                    keySet.Count != 0);

            // iterator
            var it = keySet.GetEnumerator();
            while (it.MoveNext())
            {
                assertTrue(
                        "UnmodifiableMapTest - keySetTest - Iterator returned wrong values",
                        keySet.Contains(it.Current));
            }

            // size
            assertTrue(
                    "UnmodifiableMapTest - keySetTest - returned wrong size.  Wanted 100, got: "
                            + keySet.Count, keySet.Count == 100);

            // toArray
            string[] objArray;
            objArray = keySet.ToArray();
            it = keySet.GetEnumerator(); // J2N: Bug in Harmony, this needs to be reset to run
            for (int counter = 0; it.MoveNext(); counter++)
            {
                assertTrue(
                        "UnmodifiableMapTest - keySetTest - toArray returned incorrect array",
                        objArray[counter] == it.Current);
            }

            // toArray (Object[])
            objArray = new string[100];
            keySet.CopyTo(objArray, 0);
            it = keySet.GetEnumerator(); // J2N: Bug in Harmony, this needs to be reset to run
            for (int counter = 0; it.MoveNext(); counter++)
            {
                assertTrue(
                        "UnmodifiableMapTest - keySetTest - CopyTo(Object[], int) filled array incorrectly",
                        objArray[counter] == it.Current);
            }
        }
    }
}
