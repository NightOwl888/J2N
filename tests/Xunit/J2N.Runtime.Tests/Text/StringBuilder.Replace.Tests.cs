// Source: https://github.com/dotnet/runtime/blob/v9.0.0/src/libraries/System.Runtime/tests/System.Runtime.Tests/System/Text/StringBuilderReplaceTests.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.TestUtilities.Xunit;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace J2N.Text.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness (compliance with the BCL) of an <see cref="MutableTextBuffer"/> implementation
    /// including subclasses.
    /// </summary>
    /// <remarks>
    /// J2N: This class does not map exactly to the upstream code. It was refactored to be an abstract class that can be used for testing
    /// multiple implementations of <see cref="MutableTextBuffer"/>. Each implementation is presumed to have the same behavior, but
    /// may have different internal implementations. For example, one implementation may use a buffer that is allocated on the heap,
    /// while another may use a buffer that is allocated from an array pool. The tests in this class are designed to ensure that all
    /// implementations behave correctly and consistently with the BCL.
    /// </remarks>
    public abstract partial class StringBuilder_Tests
    {
        public static IEnumerable<object[]> Replace_TestData()
        {
            yield return new object[] { "", "a", "!", 0, 0, "" };
            yield return new object[] { "aaaabbbbccccdddd", "a", "!", 0, 16, "!!!!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "a", "!", 2, 3, "aa!!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "a", "!", 4, 1, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aab", "!", 2, 2, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aab", "!", 2, 3, "aa!bbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aa", "!", 0, 16, "!!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aa", "$!", 0, 16, "$!$!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aa", "$!$", 0, 16, "$!$$!$bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaa", "!", 0, 16, "!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaa", "$!", 0, 16, "$!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "a", "", 0, 16, "bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "b", null, 0, 16, "aaaaccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaabbbbccccdddd", "", 0, 16, "" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaabbbbccccdddd", "", 16, 0, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaabbbbccccdddde", "", 0, 16, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", "a", "b", 0, 16, "bbbbbbbbbbbbbbbb" };
        }

        [Theory]
        [MemberData(nameof(Replace_TestData))]
        public void Replace_String(string value, string oldValue, string newValue, int startIndex, int count, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && count == value.Length)
            {
                // Use Replace(string, string)
                builder = MutableTextBufferFactory(value);
                builder.Replace(oldValue, newValue);
                Assert.Equal(expected, builder.ToString());
            }

            // Use Replace(string, string, int, int)
            builder = MutableTextBufferFactory(value);
            builder.Replace(oldValue, newValue, startIndex, count);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_TestData))]
        public void Replace_CharSpan(string value, string oldValue, string newValue, int startIndex, int count, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && count == value.Length)
            {
                // Use Replace(ReadOnlySpan<char>, ReadOnlySpan<char>)
                builder = MutableTextBufferFactory(value);
                builder.Replace(oldValue.AsSpan(), newValue.AsSpan());
                Assert.Equal(expected, builder.ToString());
            }

            // Use Replace(ReadOnlySpan<char>, ReadOnlySpan<char>, int, int)
            builder = MutableTextBufferFactory(value);
            builder.Replace(oldValue.AsSpan(), newValue.AsSpan(), startIndex, count);
            Assert.Equal(expected, builder.ToString());
        }

        // J2N: Multiple chunks not supported
        //[Fact]
        //public void Replace_StringBuilderWithMultipleChunks()
        //{
        //    MutableTextBuffer builder = OpenStringBuilderTests.StringBuilderWithMultipleChunks();
        //    Replace(builder, "a", "b", builder.Length - 10, 10);
        //    Assert.Equal(new string('a', builder.Length - 10) + new string('b', 10), builder.ToString());
        //}

        [Fact]
        public void Replace_String_Large()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace("a", "b", builder.Length - 10, 10);
            Assert.Equal(new string('a', builder.Length - 10) + new string('b', 10), builder.ToString());
        }

        [Fact]
        public void Replace_CharSpan_Large()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace("a".AsSpan(), "b".AsSpan(), builder.Length - 10, 10);
            Assert.Equal(new string('a', builder.Length - 10) + new string('b', 10), builder.ToString());
        }

        // J2N: Multiple chunks not supported
        //[Fact]
        //public void Replace_StringBuilderWithMultipleChunks_WholeString()
        //{
        //    MutableTextBuffer builder = OpenStringBuilderTests.StringBuilderWithMultipleChunks();
        //    Replace(builder, builder.ToString(), "");
        //    Assert.Same(string.Empty, builder.ToString());
        //}

        [Fact]
        public void Replace_String_WholeString()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace(builder.ToString(), "");
            Assert.Same(string.Empty, builder.ToString());
        }

        [Fact]
        public void Replace_CharSpan_WholeString()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace(builder.AsSpan(), "".AsSpan());
            Assert.Same(string.Empty, builder.ToString());
        }

        // J2N: Multiple chunks not supported
        //[Fact]
        //public void Replace_StringBuilderWithMultipleChunks_LongString()
        //{
        //    MutableTextBuffer builder = OpenStringBuilderTests.StringBuilderWithMultipleChunks();
        //    Replace(builder, builder.ToString() + "b", "");
        //    Assert.Equal(OpenStringBuilderTests.s_chunkSplitSource, builder.ToString());
        //}

        [Fact]
        public void Replace_String_LongString()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace(builder.ToString() + "b", "");
            Assert.Equal(s_chunkSplitSource, builder.ToString());
        }

        [Fact]
        public void Replace_CharSpan_LongString()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            string findString = builder.ToString() + "b";
            builder.Replace(findString.AsSpan(), "".AsSpan());
            Assert.Equal(s_chunkSplitSource, builder.ToString());
        }

        [Fact]
        public void Replace_String_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentNullException>("oldValue", () => builder.Replace(null, "")); // Old value is null
            AssertExtensions.Throws<ArgumentNullException>("oldValue", () => builder.Replace(null, "a", 0, 0)); // Old value is null

            AssertExtensions.Throws<ArgumentException>("oldValue", () => builder.Replace("", "a")); // Old value is empty
            AssertExtensions.Throws<ArgumentException>("oldValue", () => builder.Replace("", "a", 0, 0)); // Old value is empty

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Replace("o", "oo")); // New length > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Replace("o", "oo", 0, 5)); // New length > builder.MaxCapacity

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace("a", "b", -1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a", "b", 0, -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace("a", "b", 6, 0)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a", "b", 5, 1)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a", "b", 4, 2)); // Count + start index > builder.Length
        }

        [Fact]
        public void Replace_CharSpan_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentException>("oldValue", () => builder.Replace("".AsSpan(), "a".AsSpan())); // Old value is empty
            AssertExtensions.Throws<ArgumentException>("oldValue", () => builder.Replace("".AsSpan(), "a".AsSpan(), 0, 0)); // Old value is empty

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Replace("o".AsSpan(), "oo".AsSpan())); // New length > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Replace("o".AsSpan(), "oo".AsSpan(), 0, 5)); // New length > builder.MaxCapacity

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace("a".AsSpan(), "b".AsSpan(), -1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a".AsSpan(), "b".AsSpan(), 0, -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace("a".AsSpan(), "b".AsSpan(), 6, 0)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a".AsSpan(), "b".AsSpan(), 5, 1)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a".AsSpan(), "b".AsSpan(), 4, 2)); // Count + start index > builder.Length
        }

        public static IEnumerable<object[]> Replace_Int32_Int32_TestData()
        {
            yield return new object[] { "", 0, 0, "abc", "abc" };
            yield return new object[] { "abcdef", 0, 0, "X", "Xabcdef" };
            yield return new object[] { "abcdef", 6, 0, "X", "abcdefX" };
            yield return new object[] { "abcdef", 2, 0, "XYZ", "abXYZcdef" };

            yield return new object[] { "abcdef", 0, 2, "XY", "XYcdef" };
            yield return new object[] { "abcdef", 2, 2, "XY", "abXYef" };
            yield return new object[] { "abcdef", 4, 2, "XY", "abcdXY" };

            yield return new object[] { "abcdef", 2, 2, "WXYZ", "abWXYZef" };
            yield return new object[] { "abcdef", 2, 3, "Q", "abQf" };
            yield return new object[] { "abcdef", 2, 3, "", "abf" };

            // Count extends past end (Harmony/JDK semantics)
            yield return new object[] { "abcdef", 4, 100, "XYZ", "abcdXYZ" };
            yield return new object[] { "abcdef", 6, 100, "XYZ", "abcdefXYZ" };

            yield return new object[] { "0123456789", 3, 4, "abcdef", "012abcdef789" };
            yield return new object[] { "The quick brown fox", 4, 5, "slow", "The slow brown fox" };
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_TestData))]
        public void Replace_Int32_Int32_String(string value, int startIndex, int count, string newValue, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            builder.Replace(startIndex, count, newValue);

            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_TestData))]
        public void Replace_Int32_Int32_CharSpan(string value, int startIndex, int count, string newValue, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            builder.Replace(startIndex, count, newValue.AsSpan());

            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_TestData))]
        public void Replace_Int32_Int32_StringBuilder(string value, int startIndex, int count, string newValue, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            builder.Replace(startIndex, count, new StringBuilder(newValue));

            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_TestData))]
        public void Replace_Int32_Int32_ICharSequence(string value, int startIndex, int count, string newValue, string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ICharSequence sequence = new SpannableCharSequence(newValue.AsMemory());
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ICharSequence sequence = new CopyableCharSequence(newValue.AsMemory());
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ICharSequence sequence = new SpanCopyableCharSequence(newValue.AsMemory());
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ICharSequence sequence = new SimpleCharSequence(newValue.AsMemory());
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }
        }

        public static IEnumerable<object[]> Replace_Int32_Int32_Invalid_TestData()
        {
            // value, capacity, maxCapacity, startIndex, count, newValue,
            // expectedExceptionType, expectedParamName

            // null replacement
            yield return new object[] { "Hello", 5, 100, 0, 0, null, typeof(ArgumentNullException), "newValue" };

            // invalid start index
            yield return new object[] { "Hello", 5, 100, -1, 0, "X", typeof(ArgumentOutOfRangeException), "startIndex" };
            yield return new object[] { "Hello", 5, 100, 6, 0, "X", typeof(ArgumentOutOfRangeException), "startIndex" };

            // invalid count
            yield return new object[] { "Hello", 5, 100, 0, -1, "X", typeof(ArgumentOutOfRangeException), "count" };

            // replacement would exceed MaxCapacity
            yield return new object[] { "Hello", 5, 5, 4, 1, "ABCDE", typeof(ArgumentOutOfRangeException), "requiredLength" };
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Invalid_TestData))]
        public void Replace_Int32_Int32_String_Invalid(
            string value,
            int capacity,
            int maxCapacity,
            int startIndex,
            int count,
            string newValue,
            Type exceptionType,
            string paramName)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(capacity, maxCapacity);
            builder.Append(value);

            AssertExtensions.Throws(exceptionType, paramName,
                () => builder.Replace(startIndex, count, newValue));
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Invalid_TestData))]
        public void Replace_Int32_Int32_CharSpan_Invalid(
            string value,
            int capacity,
            int maxCapacity,
            int startIndex,
            int count,
            string newValue,
            Type exceptionType,
            string paramName)
        {
            // Span overload cannot receive null.
            if (newValue is null)
                return;

            MutableTextBuffer builder = MutableTextBufferFactory(capacity, maxCapacity);
            builder.Append(value);

            AssertExtensions.Throws(exceptionType, paramName,
                () => builder.Replace(startIndex, count, newValue.AsSpan()));
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Invalid_TestData))]
        public void Replace_Int32_Int32_StringBuilder_Invalid(
            string value,
            int capacity,
            int maxCapacity,
            int startIndex,
            int count,
            string newValue,
            Type exceptionType,
            string paramName)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(capacity, maxCapacity);
            builder.Append(value);

            StringBuilder replacement = newValue is null ? null : new StringBuilder(newValue);

            AssertExtensions.Throws(exceptionType, paramName,
                () => builder.Replace(startIndex, count, replacement));
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Invalid_TestData))]
        public void Replace_Int32_Int32_ICharSequence_Invalid(
            string value,
            int capacity,
            int maxCapacity,
            int startIndex,
            int count,
            string newValue,
            Type exceptionType,
            string paramName)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(capacity, maxCapacity);
            builder.Append(value);

            ICharSequence replacement = newValue is null ? null : new SimpleCharSequence(newValue.AsMemory());

            AssertExtensions.Throws(exceptionType, paramName,
                () => builder.Replace(startIndex, count, replacement));
        }

        public static IEnumerable<object[]> Replace_Int32_Int32_Overlapping_TestData()
        {
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 3, 0, 5, "abcdefghijabcdenopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 3, 10, 5, "klmnodefghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 3, 10, 3, "klmdefghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 3, 0, 8, "abcdefghijabcdefghnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 3, 15, 8, "pqrstuvwdefghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 8, 0, 2, "abcdefghijabstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 8, 20, 2, "uvijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 10, 5, 5, "fghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 3, 2, 0, 10, "abcabcdefghijfghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 10, 5, 10, "abcdefghijfghijklmnouvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 10, 10, 10, "abcdeklmnopqrstpqrstuvwxyz" };

            // Whole-buffer replacement (fast no-op)
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 26, 0, 26, "abcdefghijklmnopqrstuvwxyz" };

            // Empty spans (Overlaps() == false)
            yield return new object[] { "abcdef", 2, 3, 1, 0, "abf" };
            yield return new object[] { "abcdef", 0, 6, 2, 0, "" };
            yield return new object[] { "abcdef", 4, 2, 0, 0, "abcd" };
            yield return new object[] { "abcdef", 3, 0, 1, 0, "abcdef" };
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Overlapping_TestData))]
        public void Replace_Int32_Int32_CharSpan_Overlapping(
            string value,
            int startIndex,
            int count,
            int sourceIndex,
            int sourceLength,
            string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            ReadOnlySpan<char> source = builder.AsSpan(sourceIndex, sourceLength);

            builder.Replace(startIndex, count, source);

            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Overlapping_TestData))]
        public void Replace_Int32_Int32_ICharSequence_Overlapping(
            string value,
            int startIndex,
            int count,
            int sourceIndex,
            int sourceLength,
            string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpannableCharSequence(memory);
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new CopyableCharSequence(memory);
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpanCopyableCharSequence(memory);
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SimpleCharSequence(memory);
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }
        }

        [Fact]
        public void Replace_Int32_Int32_ReadOnlySpan_Overlapping_SelfReplacement_NoOp()
        {
            MutableTextBuffer builder = MutableTextBufferFactory("abcdefghijklmnopqrstuvwxyz");

            builder.Replace(5, 10, builder.AsSpan(5, 10));

            Assert.Equal("abcdefghijklmnopqrstuvwxyz", builder.ToString());
        }

        [Fact]
        public void Replace_Int32_Int32_ICharSequence_Overlapping_SelfReplacement_NoOp()
        {
            MutableTextBuffer builder = MutableTextBufferFactory("abcdefghijklmnopqrstuvwxyz");

            builder.Replace(
                5,
                10,
                new SpannableCharSequence(builder.AsMemory(5, 10)));

            Assert.Equal("abcdefghijklmnopqrstuvwxyz", builder.ToString());
        }
    }
}
