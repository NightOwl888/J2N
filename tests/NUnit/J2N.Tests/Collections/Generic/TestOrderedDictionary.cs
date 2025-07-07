// Based on: https://github.com/apache/harmony/blob/trunk/classlib/modules/luni/src/test/api/common/org/apache/harmony/luni/tests/java/util/LinkedHashMapTest.java

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Integer = J2N.Numerics.Int32;

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

    public partial class TestOrderedDictionary : TestCase
    {
        OrderedDictionary<object, object> hm;

        private const int hmSize = 1000;

        private static object[] objArray = LoadObjArray();

        private static object[] objArray2 = LoadObjArray2();

        private static object[] LoadObjArray()
        {
            var objArray = new object[hmSize];
            for (int i = 0; i < objArray.Length; i++)
                objArray[i] = new Integer(i);
            return objArray;
        }

        private static object[] LoadObjArray2()
        {
            var objArray2 = new object[hmSize];
            for (int i = 0; i < objArray.Length; i++)
                objArray2[i] = i.ToString(CultureInfo.InvariantCulture);
            return objArray2;
        }


        //static final class CacheMap extends LinkedHashMap
        //{

        //        protected boolean removeEldestEntry(Map.Entry e)
        //{
        //    return size() > 5;
        //}
        //	}

        //    private static class MockMapNull extends AbstractMap
        //{
        //    @Override
        //        public Set entrySet()
        //{
        //    return null;
        //}

        //@Override
        //        public int size()
        //{
        //    return 10;
        //}
        //    }

        /**
         * @tests java.util.LinkedHashMap#LinkedHashMap()
         */
        [Test]
        public void Test_Constructor()
        {
            // Test for method java.util.LinkedHashMap()
            new Support_MapTest2(new OrderedDictionary<string, string>()).RunTest();

            OrderedDictionary<string, string> hm2 = new();
            assertEquals("Created incorrect LinkedHashMap", 0, hm2.Count);
        }

        /**
         * @tests java.util.LinkedHashMap#LinkedHashMap(int)
         */
        [Test]
        public void Test_ConstructorI()
        {
            // Test for method java.util.LinkedHashMap(int)
            //LinkedHashMap hm2 = new LinkedHashMap(5);
            OrderedDictionary<object, object> hm2 = new(5);
            assertEquals("Created incorrect LinkedHashMap", 0, hm2.Count);
            try
            {
                new OrderedDictionary<object, object>(-1);
                fail("Failed to throw IllegalArgumentException for initial capacity < 0");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            

            IDictionary empty = new OrderedDictionary<object, object>(0);
            assertNull("Empty LinkedHashMap access", empty["nothing"]);
            empty["something"] = "here";
            assertTrue("cannot get element", (string)empty["something"] == "here");
        }

        ///**
        // * @tests java.util.LinkedHashMap#LinkedHashMap(int, float)
        // */
        //public void test_ConstructorIF()
        //{
        //    // Test for method java.util.LinkedHashMap(int, float)
        //    LinkedHashMap hm2 = new LinkedHashMap(5, (float)0.5);
        //    assertEquals("Created incorrect LinkedHashMap", 0, hm2.size());
        //    try
        //    {
        //        new LinkedHashMap(0, 0);
        //    }
        //    catch (IllegalArgumentException e)
        //    {
        //        return;
        //    }
        //    fail(
        //            "Failed to throw IllegalArgumentException for initial load factor <= 0");
        //    LinkedHashMap empty = new LinkedHashMap(0, 0.75f);
        //    assertNull("Empty hashtable access", empty.get("nothing"));
        //    empty.put("something", "here");
        //    assertTrue("cannot get element", empty.get("something") == "here");
        //}

        /**
         * @tests java.util.LinkedHashMap#LinkedHashMap(java.util.Map)
         */
        [Test]
        public void Test_ConstructorLjava_util_Map()
        {
            // Test for method java.util.LinkedHashMap(java.util.Map)
            var myMap = new SortedDictionary<object, object>();
            for (int counter = 0; counter < hmSize; counter++)
                myMap[objArray2[counter]] = objArray[counter];
            //LinkedHashMap hm2 = new LinkedHashMap(myMap);
            OrderedDictionary<object, object> hm2 = new(myMap);
            for (int counter = 0; counter < hmSize; counter++)
                assertTrue("Failed to construct correct LinkedHashMap", hm
                        [objArray2[counter]] == hm2[objArray2[counter]]);
        }

        /**
         * @tests java.util.LinkedHashMap#get(java.lang.Object)
         */
        [Test]
        public void Test_getLjava_lang_Object()
        {
            // Test for method java.lang.Object
            // java.util.LinkedHashMap.get(java.lang.Object)
            assertNull("Get returned non-null for non existent key",
                    ((IDictionary)hm)["T"]);
            hm["T"] = "HELLO";
            assertEquals("Get returned incorecct value for existing key", "HELLO", hm["T"]);

            OrderedDictionary<object, object> m = new();
            m[null] = "test";
            assertEquals("Failed with null key", "test", m[null]);
            assertNull("Failed with missing key matching null hash", ((IDictionary)m)[new int?(0)]);
        }

        /**
         * @tests java.util.LinkedHashMap#put(java.lang.Object, java.lang.Object)
         */
        [Test]
        public void Test_putLjava_lang_ObjectLjava_lang_Object()
        {
            // Test for method java.lang.Object
            // java.util.LinkedHashMap.put(java.lang.Object, java.lang.Object)
            hm["KEY"] = "VALUE";
            assertEquals("Failed to install key/value pair",
                    "VALUE", hm["KEY"]);

            OrderedDictionary<object, object> m = new();
            m[new short?((short)0)] = "short";
            m[null] = "test";
            m[new int?(0)] = "int";
            assertEquals("Failed adding to bucket containing null", "short", m[new short?((short)0)]);
            assertEquals("Failed adding to bucket containing null2", "int", m[new int?(0)]);
        }

        /**
         * @tests java.util.LinkedHashMap#putAll(java.util.Map)
         */
        [Test]
        public void Test_putAllLjava_util_Map()
        {
            // Test for method void java.util.LinkedHashMap.putAll(java.util.Map)
            OrderedDictionary<object, object> hm2 = new(hm);
            //LinkedHashMap hm2 = new LinkedHashMap();
            //hm2.putAll(hm);
            for (int i = 0; i < 1000; i++)
                assertTrue("Failed to clear all elements", hm2[
                        i.ToString(CultureInfo.InvariantCulture)].Equals(new Integer(i)));
        }

        // J2N TODO: Can this be converted?

        //        /**
        //         * @tests java.util.LinkedHashMap#putAll(java.util.Map)
        //         */
        //        [Test]
        //        public void Test_putAll_Ljava_util_Map_Null()
        //{
        //            OrderedDictionary<object, object> linkedHashMap = new();
        //            //LinkedHashMap linkedHashMap = new LinkedHashMap();
        //            try
        //    {
        //        linkedHashMap.putAll(new MockMapNull());
        //        fail("Should throw NullPointerException");
        //    }
        //    catch (NullPointerException e)
        //    {
        //        // expected.
        //    }

        //    try
        //    {
        //        linkedHashMap = new LinkedHashMap(new MockMapNull());
        //        fail("Should throw NullPointerException");
        //    }
        //    catch (NullPointerException e)
        //    {
        //        // expected.
        //    }
        //}

        /**
         * @tests java.util.LinkedHashMap#entrySet()
         */
        [Test]
        public void Test_entrySet()
        {
            // Test for method java.util.Set java.util.LinkedHashMap.entrySet()
            OrderedDictionary<object, object> s = hm;
            using var i = s.GetEnumerator();
            assertTrue("Returned set of incorrect size", hm.Count == s.Count);
            while (i.MoveNext())
            {
                KeyValuePair<object, object> m = i.Current;
                assertTrue("Returned incorrect entry set", hm.ContainsKey(m.Key)
                        && hm.ContainsValue(m.Value));
            }
        }

        /**
         * @tests java.util.LinkedHashMap#keySet()
         */
        [Test]
        [Ignore("J2N TODO: Enable when OrderedDictionary supports deleting while iterating")]
        public void Test_keySet()
        {
            // Test for method java.util.Set java.util.LinkedHashMap.keySet()
            ICollection<object> s = hm.Keys;
            assertTrue("Returned set of incorrect size()", s.Count == hm.Count);
            for (int i = 0; i < objArray.Length; i++)
                assertTrue("Returned set does not contain all keys", s
                        .Contains(objArray[i].ToString()));

            OrderedDictionary<object, object> m = new();
            m[null] = "test";
            assertTrue("Failed with null key", m.Keys.Contains(null));
            assertNull("Failed with null key", m.Keys.FirstOrDefault());

            IDictionary<int, string> map = new OrderedDictionary<int, string>(101);
            map[1] = "1";
            map[102] = "102";
            map[203] = "203";
            var it = map.Keys.GetEnumerator();
            assertTrue(it.MoveNext());
            int remove1 = it.Current;
            assertTrue(it.MoveNext());
            //it.hasNext();
            //it.Remove();
            map.Remove(remove1);
            it.MoveNext();
            int remove2 = it.Current;
            map.Remove(remove2);
            List<int> list = new() { 1, 102, 203 };

            //ArrayList list = new ArrayList(Arrays.asList(new Integer[] {
            //            new Integer(1), new Integer(102), new Integer(203) }));
            list.Remove(remove1);
            list.Remove(remove2);
            assertTrue(it.MoveNext());
            assertTrue("Wrong result", it.Current.Equals(list[0]));
            assertEquals("Wrong size", 1, map.Count);
            assertTrue("Wrong contents", map.Keys.First().Equals(
                    list[0]));

            IDictionary<int, string> map2 = new OrderedDictionary<int, string>(101);
            map2[1] = "1";
            map2[4] = "4";
            var it2 = map2.Keys.GetEnumerator();
            assertTrue(it2.MoveNext());
            int remove3 = it2.Current;
            int next;
            if (remove3 == 1)
                next = 4;
            else
                next = 1;
            assertTrue(it2.MoveNext());
            //it2.remove();
            map2.Remove(remove3);
            assertTrue(it2.MoveNext());
            assertTrue("Wrong result 2", it2.Current.Equals(next));
            assertEquals("Wrong size 2", 1, map2.Count);
            assertTrue("Wrong contents 2", map2.Keys.First().Equals(
                    next));
        }

        /**
         * @tests java.util.LinkedHashMap#values()
         */
        [Test]
        public void Test_values()
        {
            // Test for method java.util.Collection java.util.LinkedHashMap.values()
            ICollection<object> c = hm.Values;
            assertTrue("Returned collection of incorrect size()", c.Count == hm
                    .Count);
            for (int i = 0; i < objArray.Length; i++)
                assertTrue("Returned collection does not contain all keys", c
                        .Contains(objArray[i]));

            OrderedDictionary<object, Integer> myLinkedHashMap = new();
            //LinkedHashMap myLinkedHashMap = new LinkedHashMap();
            for (int i = 0; i < 100; i++)
                myLinkedHashMap[objArray2[i]] = (Integer)objArray[i];
            ICollection<Integer> values = myLinkedHashMap.Values;
            new Support_UnmodifiableCollectionTest(
                    "Test Returned Collection From LinkedHashMap.values()", values)
                    .RunTest();

            // J2N: The OrderedDictionary ValueCollection is not editable. The workaround is to call Remove() on the collection itself.
            Assert.Throws<NotSupportedException>(() => values.Remove(new Integer(0)));
            assertTrue(
                    "Removing from the values collection should remove from the original map",
                    myLinkedHashMap.ContainsValue(new Integer(0)));

        }

        /**
         * @tests java.util.LinkedHashMap#remove(java.lang.Object)
         */
        [Test]
        public void Test_removeLjava_lang_Object()
        {
            // Test for method java.lang.Object
            // java.util.LinkedHashMap.remove(java.lang.Object)
            int size = hm.Count;
            Integer y = new Integer(9);
            hm.Remove(y.ToString(CultureInfo.InvariantCulture), out object xObj);
            Integer x = (Integer)xObj;
            assertTrue("Remove returned incorrect value", x.Equals(new Integer(9)));
            assertFalse("Failed to remove given key", hm.TryGetValue(new Integer(9), out _));
            assertTrue("Failed to decrement size", hm.Count == (size - 1));
            assertFalse("Remove of non-existent key returned false", hm
                    .Remove("LCLCLC"));

            OrderedDictionary<int?, string> m = new();
            //LinkedHashMap m = new LinkedHashMap();
            m[null] = "test";
            assertFalse("Failed with same hash as null",
                    m.Remove(new int?(0)));
            assertTrue("Failed with null key", m.Remove(null, out string value));
            assertEquals("test", value);
            //assertEquals("Failed with null key", "test", m.Remove(null));
        }

        /**
         * @tests java.util.LinkedHashMap#clear()
         */
        [Test]
        public void Test_clear()
        {
            // Test for method void java.util.LinkedHashMap.clear()
            hm.Clear();
            assertEquals("Clear failed to reset size", 0, hm.Count);
            for (int i = 0; i < hmSize; i++)
                assertFalse("Failed to clear all elements",
                        hm.ContainsKey(objArray2[i]));

        }

        //        /**
        //         * @tests java.util.LinkedHashMap#clone()
        //         */
        //        [Test]
        //        public void Test_clone()
        //{
        //    // Test for method java.lang.Object java.util.LinkedHashMap.clone()
        //    LinkedHashMap hm2 = (LinkedHashMap)hm.clone();
        //    assertTrue("Clone answered equivalent LinkedHashMap", hm2 != hm);
        //    for (int counter = 0; counter < hmSize; counter++)
        //        assertTrue("Clone answered unequal LinkedHashMap", hm
        //                .get(objArray2[counter]) == hm2.get(objArray2[counter]));

        //    LinkedHashMap map = new LinkedHashMap();
        //    map.put("key", "value");
        //    // get the keySet() and values() on the original Map
        //    Set keys = map.keySet();
        //    Collection values = map.values();
        //    assertEquals("values() does not work",
        //            "value", values.iterator().next());
        //    assertEquals("keySet() does not work",
        //            "key", keys.iterator().next());
        //    AbstractMap map2 = (AbstractMap)map.clone();
        //    map2.put("key", "value2");
        //    Collection values2 = map2.values();
        //    assertTrue("values() is identical", values2 != values);

        //    // values() and keySet() on the cloned() map should be different
        //    assertEquals("values() was not cloned",
        //            "value2", values2.iterator().next());
        //    map2.clear();
        //    map2.put("key2", "value3");
        //    Set key2 = map2.keySet();
        //    assertTrue("keySet() is identical", key2 != keys);
        //    assertEquals("keySet() was not cloned",
        //            "key2", key2.iterator().next());
        //}

        //        // regresion test for HARMONY-4603
        //        [Test]
        //        public void test_clone_Mock()
        //{
        //    LinkedHashMap hashMap = new MockMap();
        //    String value = "value a";
        //    hashMap.put("key", value);
        //    MockMap cloneMap = (MockMap)hashMap.clone();
        //    assertEquals(value, cloneMap.get("key"));
        //    assertEquals(hashMap, cloneMap);
        //    assertEquals(1, cloneMap.num);

        //    hashMap.put("key", "value b");
        //    assertFalse(hashMap.equals(cloneMap));
        //}

        //class MockMap extends LinkedHashMap
        //{
        //        int num;

        //        public Object put(Object k, Object v)
        //{
        //    num++;
        //    return super.put(k, v);
        //}

        //protected bool removeEldestEntry(Map.Entry e)
        //{
        //    return num > 1;
        //}
        //    } 

        /**
         * @tests java.util.LinkedHashMap#containsKey(java.lang.Object)
         */
        [Test]
        public void Test_containsKeyLjava_lang_Object()
        {
            // Test for method boolean
            // java.util.LinkedHashMap.containsKey(java.lang.Object)
            assertTrue("Returned false for valid key", hm.ContainsKey(
                    876.ToString(CultureInfo.InvariantCulture)));
            assertTrue("Returned true for invalid key", !hm.ContainsKey("KKDKDKD"));

            OrderedDictionary<int?, string> m = new();
            m[null] = "test";
            assertTrue("Failed with null key", m.ContainsKey(null));
            assertTrue("Failed with missing key matching null hash", !m
                    .ContainsKey(new int?(0)));
        }

        /**
         * @tests java.util.LinkedHashMap#containsValue(java.lang.Object)
         */
        [Test]
        public void Test_containsValueLjava_lang_Object()
        {
            // Test for method boolean
            // java.util.LinkedHashMap.containsValue(java.lang.Object)
            assertTrue("Returned false for valid value", hm
                    .ContainsValue(new Integer(875)));
            assertTrue("Returned true for invalid valie", !hm
                    .ContainsValue(new Integer(-9)));
        }

        //        /**
        //         * @tests java.util.LinkedHashMap#isEmpty()
        //         */
        //        [Test]
        //        public void Test_isEmpty()
        //{
        //    // Test for method boolean java.util.LinkedHashMap.isEmpty()
        //    assertTrue("Returned false for new map", new LinkedHashMap().isEmpty());
        //    assertTrue("Returned true for non-empty", !hm.isEmpty());
        //}

        /**
         * @tests java.util.LinkedHashMap#size()
         */
        [Test]
        public void Test_size()
        {
            // Test for method int java.util.LinkedHashMap.size()
            assertTrue("Returned incorrect size",
                    hm.Count == (objArray.Length + 2));
        }

        /**
         * @tests java.util.LinkedHashMap#entrySet()
         */
        [Test]
        public void Test_ordered_entrySet()
        {
            int i;
            int sz = 100;
            //LinkedHashMap lhm = new LinkedHashMap();
            OrderedDictionary<Integer, string> lhm = new();
            for (i = 0; i < sz; i++)
            {
                Integer ii = new Integer(i);
                lhm[ii] = ii.ToString(CultureInfo.InvariantCulture);
            }

            //Set s1 = lhm.entrySet();
            //Iterator it1 = s1.iterator();
            var it1 = lhm.GetEnumerator();
            assertTrue("Returned set of incorrect size 1", lhm.Count == lhm.Count());
            for (i = 0; it1.MoveNext(); i++)
            {
                KeyValuePair<Integer, string> m = it1.Current;
                Integer jj = m.Key;
                assertTrue("Returned incorrect entry set 1", jj.ToInt32() == i);
            }

            // J2N: OrderedDictionary doesn't track last access order
            //////LinkedHashMap lruhm = new LinkedHashMap(200, .75f, true);
            ////OrderedDictionary<Integer, string> lruhm = new(200);
            ////for (i = 0; i < sz; i++)
            ////{
            ////    Integer ii = new Integer(i);
            ////    lruhm[ii] = ii.ToString(CultureInfo.InvariantCulture);
            ////}

            //////Set s3 = lruhm.entrySet();
            //////Iterator it3 = s3.iterator();
            ////var it3 = lruhm.GetEnumerator();
            ////assertTrue("Returned set of incorrect size 2", lruhm.Count == lruhm
            ////        .Count());
            ////for (i = 0; i < sz && it3.MoveNext(); i++)
            ////{
            ////    KeyValuePair<Integer, string> m = it3.Current;
            ////    Integer jj = m.Key;
            ////    assertTrue("Returned incorrect entry set 2", jj.ToInt32() == i);
            ////}

            /////* fetch the even numbered entries to affect traversal order */
            ////int p = 0;
            ////for (i = 0; i < sz; i += 2)
            ////{
            ////    string ii = (string)lruhm[new Integer(i)];
            ////    p = p + Integer.Parse(ii, CultureInfo.InvariantCulture);
            ////}
            ////assertEquals("invalid sum of even numbers", 2450, p);

            //////Set s2 = lruhm.entrySet();
            //////Iterator it2 = s2.iterator();
            ////var it2 = lruhm.GetEnumerator();
            ////assertTrue("Returned set of incorrect size 3", lruhm.Count == lruhm
            ////        .Count());
            ////for (i = 1; i < sz && it2.MoveNext(); i += 2)
            ////{
            ////    KeyValuePair<Integer, string> m = it2.Current;
            ////    Integer jj = m.Key;
            ////    assertTrue("Returned incorrect entry set 3", jj.ToInt32() == i);
            ////}
            ////for (i = 0; i < sz && it2.MoveNext(); i += 2)
            ////{
            ////    KeyValuePair<Integer, string> m = it2.Current;
            ////    Integer jj = m.Key;
            ////    assertTrue("Returned incorrect entry set 4", jj.ToInt32() == i);
            ////}
            ////assertTrue("Entries left to iterate on", !it2.MoveNext());
        }

        /**
         * @tests java.util.LinkedHashMap#keySet()
         */
        [Test]
        public void Test_ordered_keySet()
        {
            int i;
            int sz = 100;
            //LinkedHashMap lhm = new LinkedHashMap();
            OrderedDictionary<Integer, string> lhm = new();
            for (i = 0; i < sz; i++)
            {
                Integer ii = new Integer(i);
                lhm[ii] = ii.ToString(CultureInfo.InvariantCulture);
            }

            OrderedDictionary<Integer, string>.KeyCollection s1 = lhm.Keys;
            OrderedDictionary<Integer, string>.KeyCollection.Enumerator it1 = s1.GetEnumerator();
            assertTrue("Returned set of incorrect size", lhm.Count == s1.Count);
            for (i = 0; it1.MoveNext(); i++)
            {
                Integer jj = it1.Current;
                assertTrue("Returned incorrect entry set", jj.ToInt32() == i);
            }

            // J2N: OrderedDictionary doesn't track last access order
            //////LinkedHashMap lruhm = new LinkedHashMap(200, .75f, true);
            ////OrderedDictionary<Integer, string> lruhm = new(200);
            ////for (i = 0; i < sz; i++)
            ////{
            ////    Integer ii = new Integer(i);
            ////    lruhm[ii] = ii.ToString(CultureInfo.InvariantCulture);
            ////}

            ////OrderedDictionary<Integer, string>.KeyCollection s3 = lruhm.Keys;
            ////OrderedDictionary<Integer, string>.KeyCollection.Enumerator it3 = s3.GetEnumerator();
            ////assertTrue("Returned set of incorrect size", lruhm.Count == s3.Count);
            ////for (i = 0; i < sz && it3.MoveNext(); i++)
            ////{
            ////    Integer jj = it3.Current;
            ////    assertTrue("Returned incorrect entry set", jj.ToInt32() == i);
            ////}

            /////* fetch the even numbered entries to affect traversal order */
            ////int p = 0;
            ////for (i = 0; i < sz; i += 2)
            ////{
            ////    String ii = lruhm[new Integer(i)];
            ////    p = p + Integer.Parse(ii, CultureInfo.InvariantCulture);
            ////}
            ////assertEquals("invalid sum of even numbers", 2450, p);

            ////OrderedDictionary<Integer, string>.KeyCollection s2 = lruhm.Keys;
            ////OrderedDictionary<Integer, string>.KeyCollection.Enumerator it2 = s2.GetEnumerator();
            ////assertTrue("Returned set of incorrect size", lruhm.Count == s2.Count);
            ////for (i = 1; i < sz && it2.MoveNext(); i += 2)
            ////{
            ////    Integer jj = it2.Current;
            ////    assertTrue("Returned incorrect entry set", jj.ToInt32() == i);
            ////}
            ////for (i = 0; i < sz && it2.MoveNext(); i += 2)
            ////{
            ////    Integer jj = it2.Current;
            ////    assertTrue("Returned incorrect entry set", jj.ToInt32() == i);
            ////}
            ////assertTrue("Entries left to iterate on", !it2.MoveNext());
        }

        /**
         * @tests java.util.LinkedHashMap#values()
         */
        [Test]
        public void Test_ordered_values()
        {
            int i;
            int sz = 100;
            //LinkedHashMap lhm = new LinkedHashMap();
            OrderedDictionary<Integer, Integer> lhm = new();
            for (i = 0; i < sz; i++)
            {
                Integer ii = new Integer(i);
                lhm[ii] = new Integer(i * 2);
            }

            OrderedDictionary<Integer, Integer>.ValueCollection s1 = lhm.Values;
            OrderedDictionary<Integer, Integer>.ValueCollection.Enumerator it1 = s1.GetEnumerator();
            assertTrue("Returned set of incorrect size 1", lhm.Count == s1.Count);
            for (i = 0; it1.MoveNext(); i++)
            {
                Integer jj = it1.Current;
                assertTrue("Returned incorrect entry set 1", jj.ToInt32() == i * 2);
            }

            // J2N: OrderedDictionary doesn't track last access order
            //////LinkedHashMap lruhm = new LinkedHashMap(200, .75f, true);
            ////OrderedDictionary<Integer, Integer> lruhm = new(200);
            ////for (i = 0; i < sz; i++)
            ////{
            ////    Integer ii = new Integer(i);
            ////    lruhm[ii] = new Integer(i * 2);
            ////}

            ////OrderedDictionary<Integer, Integer>.ValueCollection s3 = lruhm.Values;
            ////OrderedDictionary<Integer, Integer>.ValueCollection.Enumerator it3 = s3.GetEnumerator();
            ////assertTrue("Returned set of incorrect size", lruhm.Count == s3.Count);
            ////for (i = 0; i < sz && it3.MoveNext(); i++)
            ////{
            ////    Integer jj = it3.Current;
            ////    assertTrue("Returned incorrect entry set", jj.ToInt32() == i * 2);
            ////}

            ////// fetch the even numbered entries to affect traversal order
            ////int p = 0;
            ////for (i = 0; i < sz; i += 2)
            ////{
            ////    Integer ii = (Integer)lruhm[new Integer(i)];
            ////    p = p + ii.ToInt32();
            ////}
            ////assertTrue("invalid sum of even numbers", p == 2450 * 2);

            ////OrderedDictionary<Integer, Integer>.ValueCollection s2 = lruhm.Values;
            ////OrderedDictionary<Integer, Integer>.ValueCollection.Enumerator it2 = s2.GetEnumerator();
            ////assertTrue("Returned set of incorrect size", lruhm.Count == s2.Count);
            ////for (i = 1; i < sz && it2.MoveNext(); i += 2)
            ////{
            ////    Integer jj = it2.Current;
            ////    assertTrue("Returned incorrect entry set", jj.ToInt32() == i * 2);
            ////}
            ////for (i = 0; i < sz && it2.MoveNext(); i += 2)
            ////{
            ////    Integer jj = it2.Current;
            ////    assertTrue("Returned incorrect entry set", jj.ToInt32() == i * 2);
            ////}
            ////assertTrue("Entries left to iterate on", !it2.MoveNext());
        }

        //        /**
        //         * @tests java.util.LinkedHashMap#removeEldestEntry(java.util.Map$Entry)
        //         */
        //        [Test]
        //        public void test_remove_eldest()
        //{
        //    int i;
        //    int sz = 10;
        //    CacheMap lhm = new CacheMap();
        //    for (i = 0; i < sz; i++)
        //    {
        //        Integer ii = new Integer(i);
        //        lhm.put(ii, new Integer(i * 2));
        //    }

        //    Collection s1 = lhm.values();
        //    Iterator it1 = s1.iterator();
        //    assertTrue("Returned set of incorrect size 1", lhm.size() == s1.size());
        //    for (i = 5; it1.hasNext(); i++)
        //    {
        //        Integer jj = (Integer)it1.next();
        //        assertTrue("Returned incorrect entry set 1", jj.intValue() == i * 2);
        //    }
        //    assertTrue("Entries left in map", !it1.hasNext());
        //}

        //        [Test]
        //        public void test_getInterfaces()
        //{
        //    Class <?> [] interfaces = HashMap.class.getInterfaces();
        //assertEquals(3, interfaces.length);

        //List < Class <?>> interfaceList = Arrays.asList(interfaces);
        //assertTrue(interfaceList.contains(Map.class));
        //assertTrue(interfaceList.contains(Cloneable.class));
        //assertTrue(interfaceList.contains(Serializable.class));

        //interfaces = LinkedHashMap.class.getInterfaces();
        //assertEquals(1, interfaces.length);

        //interfaceList = Arrays.asList(interfaces);
        //assertTrue(interfaceList.contains(Map.class));
        //    }

        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            base.SetUp();
            hm = new OrderedDictionary<object, object>();
            for (int i = 0; i < objArray.Length; i++)
                hm[objArray2[i]] = objArray[i];
            hm["test"] = null;
            hm[null] = "test";
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
