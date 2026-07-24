// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/Capacity.java/
/*
 * Copyright (c) 2016, Oracle and/or its affiliates. All rights reserved.
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

/*
 * @test
 * @bug 8149330
 * @summary Basic set of tests of capacity management
 * @run testng Capacity
 */

using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace J2N.Text
{
    public abstract partial class StringBuilderTestBase
    {
        protected const int DEFAULT_CAPACITY = 16;

        protected static int newCapacity(int oldCapacity,
                int desiredCapacity)
        {
            return Math.Max(oldCapacity * 2 + 2, desiredCapacity);
        }

        protected static int nextNewCapacity(int oldCapacity)
        {
            return newCapacity(oldCapacity, oldCapacity + 1);
        }

        [TestCaseSource(nameof(singleChar))]
        public virtual void Test_defaultCapacity(char ch)
        {
            TextBuilder sb = StringBuilderFactory();
            assertEquals(sb.Capacity, DEFAULT_CAPACITY);
            for (int i = 0; i < DEFAULT_CAPACITY; i++)
            {
                sb.Append(ch);
                assertEquals(sb.Capacity, DEFAULT_CAPACITY);
            }
            sb.Append(ch);
            assertEquals(sb.Capacity, nextNewCapacity(DEFAULT_CAPACITY));
        }

        [TestCaseSource(nameof(charCapacity))]
        public virtual void Test_explicitCapacity(char ch, int initCapacity)
        {
            TextBuilder sb = StringBuilderFactory(initCapacity);
            assertEquals(sb.Capacity, initCapacity);
            for (int i = 0; i < initCapacity; i++)
            {
                sb.Append(ch);
                assertEquals(sb.Capacity, initCapacity);
            }
            sb.Append(ch);
            assertEquals(sb.Capacity, nextNewCapacity(initCapacity));
        }

        [TestCaseSource(nameof(singleChar))]
        public virtual void Test_sbFromString(char ch)
        {
            string s = "string " + ch;
            int expectedCapacity = s.Length + DEFAULT_CAPACITY;
            TextBuilder sb = StringBuilderFactory(s);
            assertEquals(sb.Capacity, expectedCapacity);
            for (int i = 0; i < DEFAULT_CAPACITY; i++)
            {
                sb.Append(ch);
                assertEquals(sb.Capacity, expectedCapacity);
            }
            sb.Append(ch);
            assertEquals(sb.Capacity, nextNewCapacity(expectedCapacity));
        }

        [TestCaseSource(nameof(singleChar))]
        public virtual void Test_sbFromReadOnlySpan(char ch)
        {
            string s = "string " + ch;
            ReadOnlySpan<char> span = s.AsSpan();
            int expectedCapacity = span.Length + DEFAULT_CAPACITY;
            TextBuilder sb = StringBuilderFactory(span);
            assertEquals(sb.Capacity, expectedCapacity);
            for (int i = 0; i < DEFAULT_CAPACITY; i++)
            {
                sb.Append(ch);
                assertEquals(sb.Capacity, expectedCapacity);
            }
            sb.Append(ch);
            assertEquals(sb.Capacity, nextNewCapacity(expectedCapacity));
        }

        [TestCaseSource(nameof(singleChar))]
        public virtual void Test_sbFromCharSeq(char ch)
        {
            ICharSequence cs = new MyCharSeq(("char seq " + ch).AsCharSequence());
            int expectedCapacity = cs.Length + DEFAULT_CAPACITY;
            TextBuilder sb = StringBuilderFactory(cs);
            assertEquals(sb.Capacity, expectedCapacity);
            for (int i = 0; i < DEFAULT_CAPACITY; i++)
            {
                sb.Append(ch);
                assertEquals(sb.Capacity, expectedCapacity);
            }
            sb.Append(ch);
            assertEquals(sb.Capacity, nextNewCapacity(expectedCapacity));
        }

        [TestCaseSource(nameof(charCapacity))]
        public virtual void Test_ensureCapacity(char ch, int cap)
        {
            TextBuilder sb = StringBuilderFactory(0);
            assertEquals(sb.Capacity, 0); 
            sb.EnsureCapacity(cap); // only has effect if cap > 0
            int newCap = (cap == 0) ? 0 : newCapacity(0, cap);
            assertEquals(sb.Capacity, newCap);
            sb.EnsureCapacity(newCap + 1);
            assertEquals(sb.Capacity, nextNewCapacity(newCap));
            sb.Append(ch);
            assertEquals(sb.Capacity, nextNewCapacity(newCap));
        }

        [TestCaseSource(nameof(negativeCapacity))]
        public void Test_negativeInitialCapacity(int negCap)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => StringBuilderFactory(negCap));
        }

        [TestCaseSource(nameof(negativeCapacity))]
        public void Test_ensureNegativeCapacity(int negCap)
        {
            TextBuilder sb = StringBuilderFactory();

            // J2N: Throwing in this case to match the BCL. Ignoring a negative value seems pointless and even dangerous.
            Assert.Throws<ArgumentOutOfRangeException>(() => sb.EnsureCapacity(negCap));

            //sb.EnsureCapacity(negCap);
            //assertEquals(sb.Capacity, DEFAULT_CAPACITY);
        }

        [TestCaseSource(nameof(charCapacity))]
        public void Test_trimToSize(char ch, int cap)
        {
            TextBuilder sb = StringBuilderFactory(cap);
            int halfOfCap = cap / 2;
            for (int i = 0; i < halfOfCap; i++)
            {
                sb.Append(ch);
            }
            sb.TrimExcess();
            // according to the spec, capacity doesn't have to
            // become exactly the size
            assertTrue(sb.Capacity >= halfOfCap);
        }

        public static IEnumerable<object[]> singleChar()
        {
            yield return ['J'];
            yield return ['\u042b'];
        }

        public static IEnumerable<object[]> charCapacity()
        {
            yield return ['J', 0];
            yield return ['J', 1];
            yield return ['J', 15];
            yield return ['J', DEFAULT_CAPACITY];
            yield return ['J', 1024];
            yield return ['\u042b', 0];
            yield return ['\u042b', 1];
            yield return ['\u042b', 15];
            yield return ['\u042b', DEFAULT_CAPACITY];
            yield return ['\u042b', 1024];
        }

        public static IEnumerable<object[]> negativeCapacity()
        {
            yield return [-1];
            yield return [int.MinValue];
        }

        protected sealed class MyCharSeq : ICharSequence
        {
            private ICharSequence s;
            public MyCharSeq(ICharSequence s) { this.s = s; }

            public char this[int index] => s[index];

            public bool HasValue => s != null && s.HasValue;

            public int Length => s.Length;

            public ICharSequence Subsequence(int startIndex, int length)
                => s.Subsequence(startIndex, length);

            public override string ToString() => s.ToString();
        }
    }
}
