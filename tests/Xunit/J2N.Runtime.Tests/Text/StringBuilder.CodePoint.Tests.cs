using J2N.TestUtilities.Xunit;
using System;
using Xunit;

namespace J2N.Text.Tests
{
    public abstract partial class StringBuilder_Tests
    {
        [Theory]
        [InlineData("", 0x0041, "A")]
        [InlineData("abc", 0x0041, "abcA")]
        [InlineData("", 0x1F600, "\U0001F600")]
        [InlineData("abc", 0x1F600, "abc\U0001F600")]
        public void AppendCodePoint(string value, int codePoint, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            builder.AppendCodePoint(codePoint);

            Assert.Equal(expected, builder.ToString());
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
    }
}
