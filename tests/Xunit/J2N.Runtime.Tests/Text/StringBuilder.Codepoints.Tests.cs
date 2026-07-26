using J2N.TestUtilities.Xunit;
using System;
using System.Collections.Generic;
using Xunit;

namespace J2N.Text.Tests
{
    public abstract partial class StringBuilder_Tests
    {

        public static IEnumerable<object[]> AppendCodePoint_TestData()
        {
            // BMP
            yield return new object[] { "", 0x0041, "A".ToCharArray() };
            yield return new object[] { "abc", 0x0041, "abcA".ToCharArray() };

            // Supplementary
            yield return new object[] { "", 0x1F600, "\uD83D\uDE00".ToCharArray() };
            yield return new object[] { "abc", 0x1F600, "abc\uD83D\uDE00".ToCharArray() };

            // Surrogates
            yield return new object[] { "", 0xD800, "\uD800".ToCharArray() };
            yield return new object[] { "abc", 0xD800, "abc\uD800".ToCharArray() };

            yield return new object[] { "", 0xDC00, "\uDC00".ToCharArray() };
            yield return new object[] { "abc", 0xDC00, "abc\uDC00".ToCharArray() };

            // Harmony Character.ToChars() tests
            yield return new object[] { "", 0x10000, "\uD800\uDC00".ToCharArray() };
            yield return new object[] { "", 0x10001, "\uD800\uDC01".ToCharArray() };
            yield return new object[] { "", 0x10401, "\uD801\uDC01".ToCharArray() };
            yield return new object[] { "", 0x10FFFF, "\uDBFF\uDFFF".ToCharArray() };
        }

        [Theory]
        [MemberData(nameof(AppendCodePoint_TestData))]
        public void AppendCodePoint(string value, int codePoint, char[] expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);
            builder.AppendCodePoint(codePoint);
            AssertExtensions.Equal(expected, builder.AsSpan().ToArray());
        }

        [Fact]
        public void AppendCodePoint_Invalid()
        {
            AssertExtensions.Throws<ArgumentNullException>(
                "text",
                () => J2N.Text.MutableTextBufferExtensions.AppendCodePoint((MutableTextBuffer)null!, 'A'));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                () => MutableTextBufferFactory().AppendCodePoint(Character.MinCodePoint - 1));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                () => MutableTextBufferFactory().AppendCodePoint(Character.MaxCodePoint + 1));
        }

        [Theory]
        [InlineData("", 0x0041, 1, "A")]
        [InlineData("", 0x0041, 2, "AA")]
        [InlineData("", 0x0041, 5, "AAAAA")]
        [InlineData("abc", 0x0041, 1, "abcA")]
        [InlineData("abc", 0x0041, 3, "abcAAA")]

        [InlineData("", 0x03A9, 1, "Ω")]
        [InlineData("abc", 0x03A9, 2, "abcΩΩ")]

        [InlineData("", 0x1F600, 1, "\U0001F600")]
        [InlineData("", 0x1F600, 2, "\U0001F600\U0001F600")]
        [InlineData("abc", 0x1F600, 1, "abc\U0001F600")]
        [InlineData("abc", 0x1F600, 3, "abc\U0001F600\U0001F600\U0001F600")]

        [InlineData("", 0x10FFFF, 1, "\U0010FFFF")]
        [InlineData("", 0x10FFFF, 2, "\U0010FFFF\U0010FFFF")]
        public void AppendCodePoint_RepeatCount(string value, int codePoint, int repeatCount, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            builder.AppendCodePoint(codePoint, repeatCount);

            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void AppendCodePoint_Int32_Int32_Invalid()
        {
            AssertExtensions.Throws<ArgumentNullException>(
                "text",
                () => J2N.Text.MutableTextBufferExtensions.AppendCodePoint((MutableTextBuffer)null!, 'A', 1));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "repeatCount",
                () => MutableTextBufferFactory().AppendCodePoint('A', -1));

            AssertExtensions.Throws<ArgumentException>(
                () => MutableTextBufferFactory().AppendCodePoint(-1, 1));

            AssertExtensions.Throws<ArgumentException>(
                () => MutableTextBufferFactory().AppendCodePoint(0x110000, 1));

            AssertExtensions.Throws<ArgumentException>(
                () => MutableTextBufferFactory().AppendCodePoint(Character.MinCodePoint - 1, 1));

            AssertExtensions.Throws<ArgumentException>(
                () => MutableTextBufferFactory().AppendCodePoint(Character.MaxCodePoint + 1, 1));
        }

        [Fact]
        public void AppendCodePoint_Int32_Int32_CapacityExceeded()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(0, 5);

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "repeatCount",
                () => builder.AppendCodePoint('A', 6));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "repeatCount",
                () => builder.AppendCodePoint(0x1F600, 3)); // 6 UTF-16 code units
        }

        public static IEnumerable<object[]> InsertCodePoint_TestData()
        {
            // BMP
            yield return new object[] { "", 0, 0x0041, "A".ToCharArray() };
            yield return new object[] { "abc", 0, 0x0041, "Aabc".ToCharArray() };
            yield return new object[] { "abc", 1, 0x0041, "aAbc".ToCharArray() };
            yield return new object[] { "abc", 3, 0x0041, "abcA".ToCharArray() };

            // Supplementary
            yield return new object[] { "", 0, 0x1F600, "\uD83D\uDE00".ToCharArray() };
            yield return new object[] { "abc", 0, 0x1F600, "\uD83D\uDE00abc".ToCharArray() };
            yield return new object[] { "abc", 1, 0x1F600, "a\uD83D\uDE00bc".ToCharArray() };
            yield return new object[] { "abc", 3, 0x1F600, "abc\uD83D\uDE00".ToCharArray() };

            // Surrogates
            yield return new object[] { "", 0, 0xD800, "\uD800".ToCharArray() };
            yield return new object[] { "abc", 1, 0xD800, "a\uD800bc".ToCharArray() };

            yield return new object[] { "", 0, 0xDC00, "\uDC00".ToCharArray() };
            yield return new object[] { "abc", 2, 0xDC00, "ab\uDC00c".ToCharArray() };

            // Harmony Character.ToChars() tests
            yield return new object[] { "", 0, 0x10000, "\uD800\uDC00".ToCharArray() };
            yield return new object[] { "", 0, 0x10001, "\uD800\uDC01".ToCharArray() };
            yield return new object[] { "", 0, 0x10401, "\uD801\uDC01".ToCharArray() };
            yield return new object[] { "", 0, 0x10FFFF, "\uDBFF\uDFFF".ToCharArray() };
        }

        [Theory]
        [MemberData(nameof(InsertCodePoint_TestData))]
        public void InsertCodePoint(string value, int index, int codePoint, char[] expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);
            builder.InsertCodePoint(index, codePoint);
            AssertExtensions.Equal(expected, builder.AsSpan().ToArray());
        }


        [Fact]
        public void InsertCodePoint_Invalid()
        {
            AssertExtensions.Throws<ArgumentNullException>(
                "text",
                () => J2N.Text.MutableTextBufferExtensions.InsertCodePoint((MutableTextBuffer)null!, 0, 'A'));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "index",
                () => MutableTextBufferFactory().InsertCodePoint(-1, 'A'));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                () => MutableTextBufferFactory().InsertCodePoint(0, Character.MinCodePoint - 1));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                () => MutableTextBufferFactory().InsertCodePoint(0, Character.MaxCodePoint + 1));
        }
    }
}
