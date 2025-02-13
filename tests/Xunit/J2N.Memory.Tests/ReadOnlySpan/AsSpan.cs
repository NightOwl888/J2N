// Source: https://github.com/dotnet/runtime/blob/v9.0.1/src/libraries/System.Memory/tests/ReadOnlySpan/AsSpan.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using Xunit;
using J2N.TestUtilities.Xunit;
using J2N.Text;

namespace J2N.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void OpenStringBuilderAsSpanNullary()
        {
            OpenStringBuilder s = new OpenStringBuilder("Hello");
            ReadOnlySpan<char> span = s.AsSpan();
            char[] expected = s.ToCharArray();
            span.Validate(expected);
        }

        [Fact]
        public static void StringAsSpanEmptyString()
        {
            OpenStringBuilder s = new OpenStringBuilder();
            ReadOnlySpan<char> span = s.AsSpan();
            span.ValidateNonNullEmpty();
        }

        [Fact]
        public static void StringAsSpanNullChecked()
        {
#pragma warning disable CA2265 // Do not compare Span<T> to null or default
            OpenStringBuilder s = null;
            ReadOnlySpan<char> span = s.AsSpan();
            span.Validate();
            Assert.True(span == default);

            span = s.AsSpan(0);
            span.Validate();
            Assert.True(span == default);

            span = s.AsSpan(0, 0);
            span.Validate();
            Assert.True(span == default);
#pragma warning restore CA2265 // Do not compare Span<T> to null or default
        }

        [Fact]
        public static void StringAsSpanNullNonZeroStartAndLength()
        {
            OpenStringBuilder str = null;

            Assert.Throws<ArgumentOutOfRangeException>(() => str.AsSpan(1).DontBox());
            Assert.Throws<ArgumentOutOfRangeException>(() => str.AsSpan(-1).DontBox());

            Assert.Throws<ArgumentOutOfRangeException>(() => str.AsSpan(0, 1).DontBox());
            Assert.Throws<ArgumentOutOfRangeException>(() => str.AsSpan(1, 0).DontBox());
            Assert.Throws<ArgumentOutOfRangeException>(() => str.AsSpan(1, 1).DontBox());
            Assert.Throws<ArgumentOutOfRangeException>(() => str.AsSpan(-1, -1).DontBox());

#if FEATURE_INDEX_RANGE
            Assert.Throws<ArgumentOutOfRangeException>(() => str.AsSpan(new Index(1)).DontBox());
            Assert.Throws<ArgumentOutOfRangeException>(() => str.AsSpan(new Index(0, fromEnd: true)).DontBox());

            Assert.Throws<ArgumentNullException>(() => str.AsSpan(0..1).DontBox());
            Assert.Throws<ArgumentNullException>(() => str.AsSpan(new Range(new Index(0), new Index(0, fromEnd: true))).DontBox());
            Assert.Throws<ArgumentNullException>(() => str.AsSpan(new Range(new Index(0, fromEnd: true), new Index(0))).DontBox());
            Assert.Throws<ArgumentNullException>(() => str.AsSpan(new Range(new Index(0, fromEnd: true), new Index(0, fromEnd: true))).DontBox());
#endif
        }

        [Theory]
        [MemberData(nameof(TestHelpers.StringSliceTestData), MemberType = typeof(TestHelpers))]
        public static void AsSpan_StartAndLength(string textStr, int start, int length)
        {
            OpenStringBuilder text = new OpenStringBuilder(textStr);

            if (start == -1)
            {
                Validate(text, 0, text.Length, text.AsSpan());
                Validate(text, 0, text.Length, text.AsSpan(0));
#if FEATURE_INDEX_RANGE
                Validate(text, 0, text.Length, text.AsSpan(0..^0));
#endif
            }
            else if (length == -1)
            {
                Validate(text, start, text.Length - start, text.AsSpan(start));
#if FEATURE_INDEX_RANGE
                Validate(text, start, text.Length - start, text.AsSpan(start..));
#endif
            }
            else
            {
                Validate(text, start, length, text.AsSpan(start, length));
#if FEATURE_INDEX_RANGE
                Validate(text, start, length, text.AsSpan(start..(start + length)));
#endif
            }


            static unsafe void Validate(OpenStringBuilder text, int start, int length, ReadOnlySpan<char> span)
            {
                Assert.Equal(length, span.Length);
                fixed (char* pText = text.m_Chars)
                {
                    // Unsafe.AsPointer is safe here since it's pinned (since text and span should be the same string)
                    char* expected = pText + start;
                    void* actual = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
                    Assert.Equal((IntPtr)expected, (IntPtr)actual);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestHelpers.StringSlice2ArgTestOutOfRangeData), MemberType = typeof(TestHelpers))]
        public static unsafe void AsSpan_2Arg_OutOfRange(string textStr, int start)
        {
            OpenStringBuilder text = new OpenStringBuilder(textStr);

            AssertExtensions.Throws<ArgumentOutOfRangeException>("start", () => text.AsSpan(start).DontBox());
#if FEATURE_INDEX_RANGE
            if (start >= 0)
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => text.AsSpan(new Index(start)).DontBox());
            }
#endif
        }


        [Theory]
        [MemberData(nameof(TestHelpers.StringSlice3ArgTestOutOfRangeData), MemberType = typeof(TestHelpers))]
        public static unsafe void AsSpan_3Arg_OutOfRange(string textStr, int start, int length)
        {
            OpenStringBuilder text = new OpenStringBuilder(textStr);

            AssertExtensions.Throws<ArgumentOutOfRangeException>("start", () => text.AsSpan(start, length).DontBox());
#if FEATURE_INDEX_RANGE
            if (start >= 0 && length >= 0 && start + length >= 0)
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => text.AsSpan(start..(start + length)).DontBox());
            }
#endif
        }
    }
}
