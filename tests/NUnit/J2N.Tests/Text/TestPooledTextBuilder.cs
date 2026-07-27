#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
#nullable enable

namespace J2N.Text
{
    public class TestPooledTextBuilder : StringBuilderTestBase
    {
        private readonly List<IDisposable> toDisposeAfterTest = new();

        protected override TextBuilder StringBuilderFactory()
        {
            var builder = new PooledTextBuilder() { UseInvariantDefaults = true };
            toDisposeAfterTest.Add(builder);
            return builder;
        }

        protected override TextBuilder StringBuilderFactory(int capacity)
        {
            var builder = new PooledTextBuilder(capacity) { UseInvariantDefaults = true };
            toDisposeAfterTest.Add(builder);
            return builder;
        }

        protected override TextBuilder StringBuilderFactory(string? value)
        {
            var builder = new PooledTextBuilder(value) { UseInvariantDefaults = true };
            toDisposeAfterTest.Add(builder);
            return builder;
        }

        protected override TextBuilder StringBuilderFactory(ReadOnlySpan<char> value)
        {
            var builder = new PooledTextBuilder(value) { UseInvariantDefaults = true };
            toDisposeAfterTest.Add(builder);
            return builder;
        }

        protected override TextBuilder StringBuilderFactory(StringBuilder? value)
        {
            var builder = new PooledTextBuilder(value) { UseInvariantDefaults = true };
            toDisposeAfterTest.Add(builder);
            return builder;
        }

        protected override TextBuilder StringBuilderFactory(ICharSequence? value)
        {
            var builder = new PooledTextBuilder(value) { UseInvariantDefaults = true };
            toDisposeAfterTest.Add(builder);
            return builder;
        }

        [TearDown]
        public override void TearDown()
        {
            foreach (IDisposable disposable in toDisposeAfterTest)
                disposable.Dispose();

            toDisposeAfterTest.Clear();

            base.TearDown();
        }


        // We skip or override several capacity-specific tests because the array pool dictates the
        // capacity behavior. The best we can do is to ensure the capacity is at least as large as requested.

        [TestCaseSource(nameof(singleChar))]
        public override void Test_defaultCapacity(char ch)
        {
            TextBuilder sb = StringBuilderFactory();
            Assert.GreaterOrEqual(sb.Capacity, DEFAULT_CAPACITY);
            for (int i = 0; i < DEFAULT_CAPACITY; i++)
            {
                sb.Append(ch);
                Assert.GreaterOrEqual(sb.Capacity, DEFAULT_CAPACITY);
            }
            sb.Append(ch);
            Assert.GreaterOrEqual(sb.Capacity, nextNewCapacity(DEFAULT_CAPACITY));
        }

        [TestCaseSource(nameof(charCapacity))]
        public override void Test_explicitCapacity(char ch, int initCapacity)
        {
            TextBuilder sb = StringBuilderFactory(initCapacity);
            Assert.GreaterOrEqual(sb.Capacity, initCapacity);

            // J2N: actual capacity may be larger than the JDK, so the test was
            // adjusted to ensure that a grow does occur when we run out of capacity.

            int initialCapacity = sb.Capacity;

            while (sb.Length < initialCapacity)
            {
                sb.Append(ch);
                Assert.AreEqual(initialCapacity, sb.Capacity);
            }
            sb.Append(ch);
            Assert.Greater(sb.Capacity, initialCapacity);
        }

        [TestCaseSource(nameof(singleChar))]
        public override void Test_sbFromString(char ch)
        {
            string s = "string " + ch;
            int expectedCapacity = s.Length + DEFAULT_CAPACITY;
            TextBuilder sb = StringBuilderFactory(s);
            Assert.GreaterOrEqual(sb.Capacity, expectedCapacity);

            // J2N: actual capacity may be larger than the JDK, so the test was
            // adjusted to ensure that a grow does occur when we run out of capacity.

            int initialCapacity = sb.Capacity;

            while (sb.Length < initialCapacity)
            {
                sb.Append(ch);
                Assert.AreEqual(initialCapacity, sb.Capacity);
            }
            sb.Append(ch);
            Assert.Greater(sb.Capacity, initialCapacity);
        }

        [TestCaseSource(nameof(singleChar))]
        public override void Test_sbFromReadOnlySpan(char ch)
        {
            string s = "string " + ch;
            ReadOnlySpan<char> span = s.AsSpan();
            int expectedCapacity = span.Length + DEFAULT_CAPACITY;
            TextBuilder sb = StringBuilderFactory(span);
            Assert.GreaterOrEqual(sb.Capacity, expectedCapacity);

            // J2N: actual capacity may be larger than the JDK, so the test was
            // adjusted to ensure that a grow does occur when we run out of capacity.

            int initialCapacity = sb.Capacity;

            while (sb.Length < initialCapacity)
            {
                sb.Append(ch);
                Assert.AreEqual(initialCapacity, sb.Capacity);
            }
            sb.Append(ch);
            Assert.Greater(sb.Capacity, initialCapacity);
        }

        [TestCaseSource(nameof(singleChar))]
        public override void Test_sbFromCharSeq(char ch)
        {
            ICharSequence cs = new MyCharSeq(("char seq " + ch).AsCharSequence());
            int expectedCapacity = cs.Length + DEFAULT_CAPACITY;
            TextBuilder sb = StringBuilderFactory(cs);
            Assert.GreaterOrEqual(sb.Capacity, expectedCapacity);

            // J2N: actual capacity may be larger than the JDK, so the test was
            // adjusted to ensure that a grow does occur when we run out of capacity.

            int initialCapacity = sb.Capacity;

            while (sb.Length < initialCapacity)
            {
                sb.Append(ch);
                Assert.AreEqual(initialCapacity, sb.Capacity);
            }
            sb.Append(ch);
            Assert.Greater(sb.Capacity, initialCapacity);
        }

        [TestCaseSource(nameof(charCapacity))]
        public override void Test_ensureCapacity(char ch, int cap)
        {
            TextBuilder sb = StringBuilderFactory(0);
            assertEquals(sb.Capacity, 0);
            sb.EnsureCapacity(cap); // only has effect if cap > 0
            int newCap = (cap == 0) ? 0 : newCapacity(0, cap);
            Assert.GreaterOrEqual(sb.Capacity, newCap);
            newCap = sb.Capacity; // J2N: We need to set newCap to the threshold to get it to grow
            sb.EnsureCapacity(newCap + 1);
            Assert.GreaterOrEqual(sb.Capacity, nextNewCapacity(newCap));
            sb.Append(ch);
            Assert.GreaterOrEqual(sb.Capacity, nextNewCapacity(newCap));
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder()
         */
        [Test]
        public override void Test_Constructor()
        {
            TextBuilder sb = StringBuilderFactory();
            assertNotNull(sb);
            Assert.LessOrEqual(16, sb.Capacity);
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder(int)
         */
        [Test]
        public override void Test_ConstructorI()
        {
            TextBuilder sb = StringBuilderFactory(24);
            assertNotNull(sb);
            Assert.LessOrEqual(24, sb.Capacity);

            try
            {
                new StringBuilder(-1);
                fail("no exception");
            }
            catch (ArgumentOutOfRangeException) // NegativeArraySizeException
            {
                // Expected
            }

            assertNotNull(StringBuilderFactory(0));
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder(CharSequence)
         */
        //@SuppressWarnings("cast")
        [Test]
        public override void Test_ConstructorLjava_lang_CharSequence()
        {
            TextBuilder sb = StringBuilderFactory("fixture".AsCharSequence());
            assertEquals("fixture", sb.ToString());
            Assert.LessOrEqual("fixture".Length + 16, sb.Capacity);

            sb = StringBuilderFactory((ICharSequence)new StringBuffer("fixture"));
            assertEquals("fixture", sb.ToString());
            Assert.LessOrEqual("fixture".Length + 16, sb.Capacity);

            // J2N: Changed behavior to match .NET string overload to allow null
            sb = StringBuilderFactory((ICharSequence?)null);
            assertEquals("", sb.ToString());
            //try
            //{
            //    OpenStringBuilderFactory(ICharSequence)null);
            //    fail("no NPE");
            //}
            //catch (ArgumentNullException) // J2N: Using ArgumentNullException instead of NullReferenceException
            //{
            //    // Expected
            //}
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder(String)
         */
        [Test]
        public override void Test_ConstructorLjava_lang_String()
        {
            TextBuilder sb = StringBuilderFactory("fixture");
            assertEquals("fixture", sb.ToString());
            Assert.LessOrEqual("fixture".Length + 16, sb.Capacity);

            // J2N: Changed behavior to match .NET string overload to allow null
            sb = StringBuilderFactory((string?)null);
            assertEquals("", sb.ToString());
            //try
            //{
            //    OpenStringBuilderFactory(string)null);
            //    fail("no NPE");
            //}
            //catch (ArgumentNullException) // NullPointerException
            //{
            //}
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder(ReadOnlySpan<char>)
         */
        [Test]
        public override void Test_ConstructorLjava_lang_ReadOnlySpan()
        {
            TextBuilder sb = StringBuilderFactory("fixture".AsSpan());
            assertEquals("fixture", sb.ToString());
            Assert.LessOrEqual("fixture".Length + 16, sb.Capacity);

            // J2N: Changed behavior to match .NET string overload to allow null
            sb = StringBuilderFactory(ReadOnlySpan<char>.Empty);
            assertEquals("", sb.ToString());
            assertTrue(sb.AsSpan().IsEmpty);
            //try
            //{
            //    OpenStringBuilderFactory(string)null);
            //    fail("no NPE");
            //}
            //catch (ArgumentNullException) // NullPointerException
            //{
            //}
        }

        /**
         * @tests java.lang.StringBuilder.EnsureCapacity(int)'
         */
        [Test]
        public override void Test_ensureCapacityI()
        {
            TextBuilder sb = StringBuilderFactory(5);
            Assert.LessOrEqual(5, sb.Capacity);
            sb.EnsureCapacity(10);
            Assert.LessOrEqual(12, sb.Capacity);
            sb.EnsureCapacity(26);
            Assert.LessOrEqual(26, sb.Capacity);
            sb.EnsureCapacity(55);
            Assert.LessOrEqual(55, sb.Capacity);
        }
    }
}
