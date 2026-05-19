// Source: https://github.com/dotnet/runtime/blob/v9.0.1/src/libraries/System.Memory/tests/Memory/AsMemory.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Xunit;

namespace J2N.Text.Tests
{
    public static class AsMemory
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(3, 0)]
        [InlineData(3, 1)]
        [InlineData(3, 2)]
        [InlineData(3, 3)]
        [InlineData(10, 0)]
        [InlineData(10, 3)]
        [InlineData(10, 10)]
        public static void MutableTextBufferAsMemoryWithStart(int length, int start)
        {
            MutableTextBuffer a = new MutableTextBuffer().Initialize(length);
            a.Append('\0', length);
            ReadOnlyMemory<char> m = a.AsMemory(start);
            Assert.Equal(length - start, m.Length);
            if (start != length)
            {
                a[start] = (char)42;
                Assert.Equal(42, m.Span[0]);
            }
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(3, 0, 3)]
        [InlineData(3, 1, 2)]
        [InlineData(3, 2, 1)]
        [InlineData(3, 3, 0)]
        [InlineData(10, 0, 5)]
        [InlineData(10, 3, 2)]
        public static void MutableTextBufferAsMemoryWithStartAndLength(int length, int start, int subLength)
        {
            MutableTextBuffer a = new MutableTextBuffer().Initialize(length);
            a.Append('\0', length);

            ReadOnlyMemory<char> m = a.AsMemory(start, subLength);
            Assert.Equal(subLength, m.Length);
            if (subLength != 0)
            {
                a[start] = (char)42;
                Assert.Equal(42, m.Span[0]);
            }
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(0, 1)]
        [InlineData(5, 6)]
        public static void MutableTextBufferAsMemoryWithStartNegative(int length, int start)
        {
            MutableTextBuffer a = new MutableTextBuffer().Initialize(length);
            Assert.Throws<ArgumentOutOfRangeException>(() => a.AsMemory(start));
        }

        [Theory]
        [InlineData(0, -1, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(0, 0, -1)]
        [InlineData(0, 0, 1)]
        [InlineData(5, 6, 0)]
        [InlineData(5, 3, 3)]
        public static void MutableTextBufferAsMemoryWithStartAndLengthNegative(int length, int start, int subLength)
        {
            MutableTextBuffer a = new MutableTextBuffer().Initialize(length);
            Assert.Throws<ArgumentOutOfRangeException>(() => a.AsMemory(start, subLength));
        }
    }
}
