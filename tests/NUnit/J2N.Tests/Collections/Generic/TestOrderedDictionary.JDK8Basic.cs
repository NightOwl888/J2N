// Source: https://github.com/openjdk/jdk/blob/jdk8-b120/jdk/test/java/util/LinkedHashMap/Basic.java

/*
 * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */

using J2N.Collections.Generic.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Integer = J2N.Numerics.Int32;

namespace J2N.Collections.Generic
{
    public partial class TestOrderedDictionary
    {
        public class JDK8Basic
        {
            private static Random rnd = new Random(666);
            private static object nil = new Integer(0);

            [Test]
            public void Test()
            {
                int numItr = 500;
                int mapSize = 500;

                // Linked List test
                for (int i = 0; i < numItr; i++)
                {
                    //Map m = new LinkedHashMap();
                    IDictionary<object, object> m = new OrderedDictionary<object, object>();
                    object head = nil;

                    for (int j = 0; j < mapSize; j++)
                    {
                        object newHead;
                        do
                        {
                            newHead = new Integer(rnd.Next());
                        } while (m.ContainsKey(newHead));
                        m[newHead] = head;
                        head = newHead;
                    }
                    if (m.Count != mapSize)
                        throw new Exception("Size not as expected.");

                    if (new Dictionary<object, object>(m).GetHashCode() != m.GetHashCode())
                        throw new Exception("Incorrect hashCode computation.");

                    OrderedDictionary<object, object> m2 = new OrderedDictionary<object, object>(m);
                    // J2N: Values collection is read-only
                    //m2.values().removeAll(m.keySet());)
                    //if (m2.Count != 1 || !m2.Values.Contains(nil))
                    //    throw new Exception("Collection views test failed.");

                    {
                        int j = 0;
                        while (head != nil)
                        {
                            if (!m.ContainsKey(head))
                                throw new Exception("Linked list doesn't contain a link.");
                            object newHead = m[head];
                            if (newHead == null)
                                throw new Exception("Could not retrieve a link.");
                            m.Remove(head);
                            head = newHead;
                            j++;
                        }
                        if (m.Count > 0)
                            throw new Exception("Map nonempty after removing all links.");
                        if (j != mapSize)
                            throw new Exception("Linked list size not as expected.");
                    }
                }

                {
                    OrderedDictionary<object, object> m = new OrderedDictionary<object, object>();
                    for (int i = 0; i < mapSize; i++)
                    {
                        // J2N TODO: Do we need an optimized Put() method that returns the previous value that it replaces?
                        //if (m.put(new Integer(i), new Integer(2 * i)) != null)
                        //    throw new Exception("put returns non-null value erroenously.");
                        var key = new Integer(i);
                        var value = new Integer(2 * i);
                        Assert.IsFalse(m.TryGetValue(key, out object previous) && previous is not null, "OrderdDictionary contains value erroneously.");
                        m[key] = value;
                    }
                    for (int i = 0; i < 2 * mapSize; i++)
                        if (m.ContainsValue(new Integer(i)) != (i % 2 == 0))
                            throw new Exception("contains value " + i);
                    //if (m.put(nil, nil) == null)
                    //    throw new Exception("put returns a null value erroenously.");
                    if (!m.TryGetValue(nil, out object previous2) && previous2 is not null)
                        throw new Exception("OrderdDictionary contains value erroneously.");
                    m[nil] = nil;

                    OrderedDictionary<object, object> m2 = new(m);
                    if (!m.Equals(m2))
                        throw new Exception("Clone not equal to original. (1)");
                    if (!m2.Equals(m))
                        throw new Exception("Clone not equal to original. (2)");
                    ICollection<KeyValuePair<object, object>> s = m, s2 = m2;
                    if (!s.Equals(s2))
                        throw new Exception("Clone not equal to original. (3)");
                    if (!s2.Equals(s))
                        throw new Exception("Clone not equal to original. (4)");
                    //if (!s.containsAll(s2))
                    if (!ContainsAll<object, object>(m, s2))
                        throw new Exception("Original doesn't contain clone!");
                    //if (!s2.containsAll(s))
                    if (!ContainsAll<object, object>(m2, s))
                        throw new Exception("Clone doesn't contain original!");

                    // J2N: OrderedDictionary doesn't support serialization by design
                    //m2 = serClone(m);
                    //if (!m.equals(m2))
                    //    throw new Exception("Serialize Clone not equal to original. (1)");
                    //if (!m2.equals(m))
                    //    throw new Exception("Serialize Clone not equal to original. (2)");
                    //s = m.entrySet(); s2 = m2.entrySet();
                    //if (!s.equals(s2))
                    //    throw new Exception("Serialize Clone not equal to original. (3)");
                    //if (!s2.equals(s))
                    //    throw new Exception("Serialize Clone not equal to original. (4)");
                    //if (!s.containsAll(s2))
                    //    throw new Exception("Original doesn't contain Serialize clone!");
                    //if (!s2.containsAll(s))
                    //    throw new Exception("Serialize Clone doesn't contain original!");

                    //s2.removeAll(s);
                    //if (m2.Count > 0)
                    //    throw new Exception("entrySet().removeAll failed.");

                    //m2.putAll(m);
                    m2.Clear();
                    if (m2.Count > 0)
                        throw new Exception("clear failed.");

                    // J2N TODO: Enable after OrderedDictionary is fixed to support deleting while iterating forward

                    //using var it = m.GetEnumerator();
                    //while (it.MoveNext())
                    //{
                    //    //it.next();
                    //    //it.remove();
                    //    var current = it.Current;
                    //    m.Remove(current.Key);
                    //}
                    //if (m2.Count > 0)
                    //    throw new Exception("Iterator.remove() failed");

                    // Test ordering properties with insert order
                    m = new();
                    IList<object> l = new List<object>(mapSize);
                    for (int i = 0; i < mapSize; i++)
                    {
                        Integer x = new Integer(i);
                        m[x] = x;
                        l.Add(x);
                    }
                    if (!new List<object>(m.Keys).Equals(l))
                        throw new Exception("Insertion order not preserved.");
                    for (int i = mapSize - 1; i >= 0; i--)
                    {
                        Integer x = (Integer)l[i];
                        if (!m[x].Equals(x))
                            throw new Exception("Wrong value: " + i + ", " + m[x] + ", " + x);
                    }
                    if (!new List<object>(m.Keys).Equals(l))
                        throw new Exception("Insertion order not preserved after read.");

                    for (int i = mapSize - 1; i >= 0; i--)
                    {
                        Integer x = (Integer)l[i];
                        m[x] = x;
                    }
                    if (!new List<object>(m.Keys).Equals(l))
                        throw new Exception("Insert order not preserved after reinsert.");

                    //m2 = (Map)((OrderedDictionary)m).clone();
                    m2 = new(m); // J2N: "clone" using the copy constructor
                    if (!m.Equals(m2))
                        throw new Exception("Insert-order Map != clone.");

                    IList<object> l2 = new List<object>(l);
                    l2.Shuffle();
                    for (int i = 0; i < mapSize; i++)
                    {
                        Integer x = (Integer)l2[i];
                        if (!m2[x].Equals(x))
                            throw new Exception("Clone: Wrong val: " + i + ", " + m[x] + ", " + x);
                    }
                    if (!new List<object>(m2.Keys).Equals(l))
                        throw new Exception("Clone: altered by read.");

                    // J2N: OrderedDictionary doesn't support access order
                    //// Test ordering properties with access order
                    //m = new LinkedHashMap(1000, .75f, true);
                    m = new OrderedDictionary<object, object>(1000);
                    for (int i = 0; i < mapSize; i++)
                    {
                        Integer x = new Integer(i);
                        m[x] = x;
                    }
                    if (!new List<object>(m.Keys).Equals(l))
                        throw new Exception("Insertion order not properly preserved.");

                    for (int i = 0; i < mapSize; i++)
                    {
                        Integer x = (Integer)l2[i];
                        if (!m[x].Equals(x))
                            throw new Exception("Wrong value: " + i + ", " + m[x] + ", " + x);
                    }
                    // J2N: OrderedDictionary doesn't support access order
                    //if (!new List<object>(m.Keys).Equals(l2))
                    //    throw new Exception("Insert order not properly altered by read.");

                    for (int i = 0; i < mapSize; i++)
                    {
                        Integer x = new Integer(i);
                        m[x]= x;
                    }
                    if (!new List<object>(m.Keys).Equals(l))
                        throw new Exception("Insertion order not altered by reinsert.");

                    //m2 = (Map)((LinkedHashMap)m).clone();
                    m2 = new(m); // J2N: Clone using constructor
                    if (!m.Equals(m2))
                        throw new Exception("Access-order Map != clone.");
                    for (int i = 0; i < mapSize; i++)
                    {
                        Integer x = (Integer)l[i];
                        if (!m2[x].Equals(x))
                            throw new Exception("Clone: Wrong val: " + i + ", " + m[x] + ", " + x);
                    }
                    if (!new List<object>(m2.Keys).Equals(l))
                        throw new Exception("Clone: order not properly altered by read.");

                    Console.WriteLine("Success.");
                }
            }

            private static bool ContainsAll<TKey, TValue>(IDictionary<TKey, TValue> dict,
                IEnumerable<KeyValuePair<TKey, TValue>> entries)
            {
                foreach (var entry in entries)
                {
                    if (!dict.TryGetValue(entry.Key, out var value))
                        return false;
                    if (!EqualityComparer<TValue>.Default.Equals(value, entry.Value))
                        return false;
                }
                return true;
            }
        }
    }
}
