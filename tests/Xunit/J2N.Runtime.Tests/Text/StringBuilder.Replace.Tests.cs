// Source: https://github.com/dotnet/runtime/blob/v9.0.0/src/libraries/System.Runtime/tests/System.Runtime.Tests/System/Text/StringBuilderReplaceTests.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.TestUtilities.Xunit;
using System;
using System.Collections.Generic;
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
        public void Replace_ReadOnlySpan(string value, string oldValue, string newValue, int startIndex, int count, string expected)
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
        public void Replace_ReadOnlySpan_Large()
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
        public void Replace_ReadOnlySpan_WholeString()
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
        public void Replace_ReadOnlySpan_LongString()
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
        public void Replace_ReadOnlySpan_Invalid()
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
    }
}
