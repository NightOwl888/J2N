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

using J2N.Buffers;
using J2N.TestUtilities.Xunit;
using System;
using System.Text;
using Xunit;

namespace J2N.Text.Tests
{
    public abstract partial class StringBuilder_Tests
    {
        private static readonly IArrayAllocator<char> ClearedAllocator = new MockArrayAllocator(value: '\0');
        private static readonly IArrayAllocator<char> NonClearingAllocator = new MockArrayAllocator(value: '\uFFFF');

        [Fact]
        public void GetSpan_DataAppendedCorrectly()
        {
            var expected = new StringBuilder();
            var actual = MutableTextBufferFactory();

            for (int i = 1; i <= 1000; i++)
            {
                string s = i.ToString();

                expected.Append(s);

                Span<char> span = actual.GetSpan(s.Length);
                Assert.Equal(expected.Length - s.Length, actual.Length);

                s.AsSpan().CopyTo(span);

                actual.Advance(s.Length);
            }

            Assert.Equal(expected.Length, actual.Length);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void GetMemory_DataAppendedCorrectly()
        {
            var expected = new StringBuilder();
            var actual = MutableTextBufferFactory();

            for (int i = 1; i <= 1000; i++)
            {
                string s = i.ToString();

                expected.Append(s);

                Memory<char> memory = actual.GetMemory(s.Length);

                s.AsSpan().CopyTo(memory.Span);

                actual.Advance(s.Length);
            }

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void GetSpan_DoesNotChangeLengthUntilAdvance()
        {
            var builder = MutableTextBufferFactory();

            builder.Append("Hello");

            int length = builder.Length;

            Span<char> span = builder.GetSpan(5);

            Assert.Equal(length, builder.Length);

            "World".AsSpan().CopyTo(span);

            Assert.Equal(length, builder.Length);

            builder.Advance(5);

            Assert.Equal(length + 5, builder.Length);
            Assert.Equal("HelloWorld", builder.ToString());
        }

        [Fact]
        public void Advance_CanAdvanceLessThanRequested()
        {
            var builder = MutableTextBufferFactory();

            Span<char> span = builder.GetSpan(32);

            "Hello".AsSpan().CopyTo(span);

            builder.Advance(5);

            Assert.Equal("Hello", builder.ToString());
        }

        [Fact]
        public void GetSpan_SizeHintZero_ReturnsNonEmptySpan()
        {
            var builder = MutableTextBufferFactory();

            Span<char> span = builder.GetSpan();

            Assert.False(span.IsEmpty);
        }

        [Fact]
        public void GetSpan_ClearExposedBuffers_ReturnsClearedBuffer()
        {
            var builder = MutableTextBufferFactory("abcdefghijklmnopqrstuvwxyz", new MutableTextBufferTestOptions { ClearExposedBuffers = true });
            builder.Length = 20;
            Span<char> span = builder.GetSpan(5);
            Assert.True(span.Length >= 5);
            for (int i = 0; i < span.Length; i++)
            {
                Assert.Equal('\0', span[i]);
            }
        }

        [Fact]
        public void GetSpan_DontClearExposedBuffers_ReturnsUnclearedBuffer()
        {
            var builder = MutableTextBufferFactory("abcdefghijklmnopqrstuvwxyz", new MutableTextBufferTestOptions { ClearExposedBuffers = false });
            builder.Length = 20;
            Span<char> span = builder.GetSpan(5);
            Assert.True(span.Length >= 5);
            Assert.True(span.StartsWith("uvwxy"));
        }

        [Fact]
        public void GetSpan_BoundedByMaxCapacity_DoesNotExpandBeyondMaxCapacity()
        {
            var builder = MutableTextBufferFactory(4, 4);
            AssertExtensions.GreaterThanOrEqualTo(builder.GetSpan(4).Length, 4);
            AssertExtensions.GreaterThanOrEqualTo(builder.GetSpan(0).Length, 4);
        }

        [Fact]
        public void GetSpan_SizeHintLessThanDefaultCapacity_ReturnsRequestedSize()
        {
            var builder = MutableTextBufferFactory(
                32,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true });

            Span<char> span = builder.GetSpan(4);

            Assert.Equal(4, span.Length);
        }

        [Fact]
        public void GetSpan_SizeHintZero_WithSufficientCapacity_ReturnsDefaultCapacity()
        {
            var builder = MutableTextBufferFactory(
                32,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true });

            Span<char> span = builder.GetSpan(0);

            Assert.Equal(16, span.Length);
        }

        [Fact]
        public void GetSpan_SizeHintZero_WithLessThanDefaultCapacityRemaining_ReturnsRemainingCapacity()
        {
            var builder = MutableTextBufferFactory(
                26,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true, Allocator = ClearedAllocator });

            builder.Append("abcdefghijklmnopqrstuvwxyz");
            builder.Length = 16;

            Span<char> span = builder.GetSpan(0);

            Assert.Equal(10, span.Length);

            for (int i = 0; i < span.Length; i++)
            {
                Assert.Equal('\0', span[i]);
            }
        }

        [Fact]
        public void GetSpan_SizeHintZero_WithNoCapacity_GrowsToDefaultCapacity()
        {
            var builder = MutableTextBufferFactory(
                0,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true });

            Span<char> span = builder.GetSpan(0);

            Assert.Equal(16, span.Length);
        }

        [Fact]
        public void GetSpan_ClearExposedBuffers_ClearsOnlyRequestedSize()
        {
            var builder = MutableTextBufferFactory(
                32,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true, Allocator = ClearedAllocator });

            Span<char> initial = builder.GetSpan(10);
            "abcdefghij".AsSpan().CopyTo(initial);

            Span<char> result = builder.GetSpan(4);

            Assert.Equal(4, result.Length);
            Assert.Equal('\0', result[0]);
            Assert.Equal('\0', result[1]);
            Assert.Equal('\0', result[2]);
            Assert.Equal('\0', result[3]);

            // The portion beyond sizeHint was not unnecessarily cleared.
            Assert.Equal('e', initial[4]);
            Assert.Equal('f', initial[5]);
            Assert.Equal('g', initial[6]);
            Assert.Equal('h', initial[7]);
            Assert.Equal('i', initial[8]);
            Assert.Equal('j', initial[9]);
        }

        [Fact]
        public void GetSpan_GrowsWithClearedAllocator_ReturnsEntireClearedBuffer()
        {
            var builder = MutableTextBufferFactory(
                4,
                new MutableTextBufferTestOptions
                {
                    ClearExposedBuffers = true,
                    Allocator = ClearedAllocator
                });

            Span<char> span = builder.GetSpan(20);

            Assert.True(span.Length >= 20);
            Assert.Equal(builder.Capacity - builder.Length, span.Length);

            for (int i = 0; i < span.Length; i++)
            {
                Assert.Equal('\0', span[i]);
            }
        }

        [Fact]
        public void GetSpan_GrowsWithNonClearingAllocator_ReturnsOnlyRequestedSize()
        {
            var builder = MutableTextBufferFactory(
                4,
                new MutableTextBufferTestOptions
                {
                    ClearExposedBuffers = true,
                    Allocator = NonClearingAllocator,
                });

            Span<char> span = builder.GetSpan(20);

            Assert.Equal(20, span.Length);

            for (int i = 0; i < span.Length; i++)
            {
                Assert.Equal('\0', span[i]);
            }
        }

        [Fact]
        public void GetSpan_GrowsWithoutClearing_ReturnsEntireRemainingBuffer()
        {
            var builder = MutableTextBufferFactory(
                4,
                new MutableTextBufferTestOptions
                {
                    ClearExposedBuffers = false,
                    Allocator = NonClearingAllocator,
                });

            Span<char> span = builder.GetSpan(20);

            Assert.True(span.Length >= 20);
            Assert.Equal(builder.Capacity - builder.Length, span.Length);

            for (int i = 0; i < span.Length; i++)
            {
                Assert.Equal('\uFFFF', span[i]);
            }
        }

        [Fact]
        public void GetMemory_SizeHintZero_ReturnsNonEmptySpan()
        {
            var builder = MutableTextBufferFactory();

            Memory<char> memory = builder.GetMemory();

            Assert.False(memory.IsEmpty);
        }

        [Fact]
        public void GetMemory_ClearExposedBuffers_ReturnsClearedBuffer()
        {
            var builder = MutableTextBufferFactory("abcdefghijklmnopqrstuvwxyz", new MutableTextBufferTestOptions { ClearExposedBuffers = true });
            builder.Length = 20;
            Memory<char> memory = builder.GetMemory(5);
            Assert.True(memory.Length >= 5);
            Span<char> span = memory.Span;
            for (int i = 0; i < memory.Length; i++)
            {
                Assert.Equal('\0', span[i]);
            }
        }

        [Fact]
        public void GetMemory_DontClearExposedBuffers_ReturnsUnclearedBuffer()
        {
            var builder = MutableTextBufferFactory("abcdefghijklmnopqrstuvwxyz", new MutableTextBufferTestOptions { ClearExposedBuffers = false });
            builder.Length = 20;
            Memory<char> memory = builder.GetMemory(5);
            Assert.True(memory.Length >= 5);
            Assert.True(memory.Span.StartsWith("uvwxy"));
        }

        [Fact]
        public void GetMemory_BoundedByMaxCapacity_DoesNotExpandBeyondMaxCapacity()
        {
            var builder = MutableTextBufferFactory(4, 4);

            AssertExtensions.GreaterThanOrEqualTo(builder.GetMemory(4).Length, 4);
            AssertExtensions.GreaterThanOrEqualTo(builder.GetMemory(0).Length, 4);
        }


        [Fact]
        public void GetMemory_SizeHintLessThanDefaultCapacity_ReturnsRequestedSize()
        {
            var builder = MutableTextBufferFactory(
                32,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true });

            Memory<char> memory = builder.GetMemory(4);

            Assert.Equal(4, memory.Length);
        }

        [Fact]
        public void GetMemory_SizeHintZero_WithSufficientCapacity_ReturnsDefaultCapacity()
        {
            var builder = MutableTextBufferFactory(
                32,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true });

            Memory<char> memory = builder.GetMemory(0);

            Assert.Equal(16, memory.Length);
        }

        [Fact]
        public void GetMemory_SizeHintZero_WithLessThanDefaultCapacityRemaining_ReturnsRemainingCapacity()
        {
            var builder = MutableTextBufferFactory(
                26,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true, Allocator = ClearedAllocator });

            builder.Append("abcdefghijklmnopqrstuvwxyz");
            builder.Length = 16;

            Memory<char> memory = builder.GetMemory(0);

            Assert.Equal(10, memory.Length);

            for (int i = 0; i < memory.Length; i++)
            {
                Assert.Equal('\0', memory.Span[i]);
            }
        }

        [Fact]
        public void GetMemory_SizeHintZero_WithNoCapacity_GrowsToDefaultCapacity()
        {
            var builder = MutableTextBufferFactory(
                0,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true });

            Memory<char> memory = builder.GetMemory(0);

            Assert.Equal(16, memory.Length);
        }

        [Fact]
        public void GetMemory_ClearExposedBuffers_ClearsOnlyRequestedSize()
        {
            var builder = MutableTextBufferFactory(
                32,
                new MutableTextBufferTestOptions { ClearExposedBuffers = true, Allocator = ClearedAllocator });

            Memory<char> initial = builder.GetMemory(10);
            "abcdefghij".AsSpan().CopyTo(initial.Span);

            Memory<char> result = builder.GetMemory(4);

            Assert.Equal(4, result.Length);
            Assert.Equal('\0', result.Span[0]);
            Assert.Equal('\0', result.Span[1]);
            Assert.Equal('\0', result.Span[2]);
            Assert.Equal('\0', result.Span[3]);

            // The portion beyond sizeHint was not unnecessarily cleared.
            Assert.Equal('e', initial.Span[4]);
            Assert.Equal('f', initial.Span[5]);
            Assert.Equal('g', initial.Span[6]);
            Assert.Equal('h', initial.Span[7]);
            Assert.Equal('i', initial.Span[8]);
            Assert.Equal('j', initial.Span[9]);
        }

        [Fact]
        public void GetMemory_GrowsWithClearedAllocator_ReturnsEntireClearedBuffer()
        {
            var builder = MutableTextBufferFactory(
                4,
                new MutableTextBufferTestOptions
                {
                    ClearExposedBuffers = true,
                    Allocator = ClearedAllocator
                });

            Memory<char> memory = builder.GetMemory(20);

            Assert.True(memory.Length >= 20);
            Assert.Equal(builder.Capacity - builder.Length, memory.Length);

            for (int i = 0; i < memory.Length; i++)
            {
                Assert.Equal('\0', memory.Span[i]);
            }
        }

        [Fact]
        public void GetMemory_GrowsWithNonClearingAllocator_ReturnsOnlyRequestedSize()
        {
            var builder = MutableTextBufferFactory(
                4,
                new MutableTextBufferTestOptions
                {
                    ClearExposedBuffers = true,
                    Allocator = NonClearingAllocator,
                });

            Memory<char> memory = builder.GetMemory(20);

            Assert.Equal(20, memory.Length);

            for (int i = 0; i < memory.Length; i++)
            {
                Assert.Equal('\0', memory.Span[i]);
            }
        }

        [Fact]
        public void GetMemory_GrowsWithoutClearing_ReturnsEntireRemainingBuffer()
        {
            var builder = MutableTextBufferFactory(
                4,
                new MutableTextBufferTestOptions
                {
                    ClearExposedBuffers = false,
                    Allocator = NonClearingAllocator,
                });

            Memory<char> memory = builder.GetMemory(20);

            Assert.True(memory.Length >= 20);
            Assert.Equal(builder.Capacity - builder.Length, memory.Length);

            for (int i = 0; i < memory.Length; i++)
            {
                Assert.Equal('\uFFFF', memory.Span[i]);
            }
        }




        [Fact]
        public void GetSpan_Invalid()
        {
            var builder = MutableTextBufferFactory();

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "sizeHint",
                () => builder.GetSpan(-1));
        }

        [Fact]
        public void GetMemory_Invalid()
        {
            var builder = MutableTextBufferFactory();

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "sizeHint",
                () => builder.GetMemory(-1));
        }

        [Fact]
        public void Advance_Negative_Throws()
        {
            var builder = MutableTextBufferFactory();

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "count",
                () => builder.Advance(-1));
        }

        [Fact]
        public void Advance_PastCapacity_Throws()
        {
            var builder = MutableTextBufferFactory();

            builder.GetSpan(10);

            Assert.Throws<InvalidOperationException>(
                () => builder.Advance(int.MaxValue));
        }

        [Fact]
        public void Advance_ExactlyRequested_Succeeds()
        {
            var builder = MutableTextBufferFactory();

            Span<char> span = builder.GetSpan(5);

            "Hello".AsSpan().CopyTo(span);

            builder.Advance(5);

            Assert.Equal("Hello", builder.ToString());
        }

        [Fact]
        public void GetSpan_MultipleWrites_WorkCorrectly()
        {
            var builder = MutableTextBufferFactory();

            Span<char> span = builder.GetSpan(5);
            "Hello".AsSpan().CopyTo(span);
            builder.Advance(5);

            span = builder.GetSpan(1);
            span[0] = ' ';
            builder.Advance(1);

            span = builder.GetSpan(5);
            "World".AsSpan().CopyTo(span);
            builder.Advance(5);

            Assert.Equal("Hello World", builder.ToString());
        }

        [Fact]
        public void GetSpan_DoesNotModifyExistingContents()
        {
            var builder = MutableTextBufferFactory();

            builder.Append("Hello");

            Span<char> span = builder.GetSpan(5);

            Assert.Equal("Hello", builder.ToString());

            "World".AsSpan().CopyTo(span);

            Assert.Equal("Hello", builder.ToString());

            builder.Advance(5);

            Assert.Equal("HelloWorld", builder.ToString());
        }

        private sealed class MockArrayAllocator : IArrayAllocator<char>
        {
            private readonly bool guaranteesClearedArrays;
            private readonly char value;

            public MockArrayAllocator(char value)
            {
                guaranteesClearedArrays = value == '\0';
                this.value = value;
            }

            public bool GuaranteesClearedArrays => guaranteesClearedArrays;

            public char[] Allocate(int minimumLength)
            {
                char[] array = new char[minimumLength];
                ArrayExtensions.Fill(array, value);
                return array;
            }

            public void Return(char[] array)
            {
                // Intentionally empty
            }
        }


        //private sealed class MockArrayAllocator<T> : IArrayAllocator<T>
        //{
        //    private readonly IArrayAllocator<T> innerAllocator;
        //    private readonly bool guaranteesClearedArrays;

        //    public MockArrayAllocator(IArrayAllocator<T> innerAllocator, bool guaranteesClearedArrays)
        //    {
        //        this.innerAllocator = innerAllocator;
        //        this.guaranteesClearedArrays = guaranteesClearedArrays;
        //    }

        //    public bool GuaranteesClearedArrays => guaranteesClearedArrays;

        //    public T[] Allocate(int minimumLength)
        //    {
        //        return innerAllocator.Allocate(minimumLength);
        //    }

        //    public void Return(T[] array)
        //    {
        //        innerAllocator.Return(array);
        //    }
        //}
    }
}
