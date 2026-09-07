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

        public static IEnumerable<object[]> AppendCodePoint_Repeat_TestData()
        {
            // BMP
            yield return new object[] { "", 0x0041, 1, "A".ToCharArray() };
            yield return new object[] { "", 0x0041, 2, "AA".ToCharArray() };
            yield return new object[] { "", 0x0041, 5, "AAAAA".ToCharArray() };
            yield return new object[] { "abc", 0x0041, 3, "abcAAA".ToCharArray() };

            // Non-ASCII BMP
            yield return new object[] { "", 0x03A9, 1, "Ω".ToCharArray() };
            yield return new object[] { "abc", 0x03A9, 2, "abcΩΩ".ToCharArray() };

            // High surrogate (BMP branch)
            yield return new object[] { "", 0xD800, 3, new[] { '\uD800', '\uD800', '\uD800' } };

            // Low surrogate (BMP branch)
            yield return new object[] { "", 0xDC00, 2, new[] { '\uDC00', '\uDC00' } };

            // Harmony values
            yield return new object[] { "", 0x10000, 1, new[] { '\uD800', '\uDC00' } };
            yield return new object[] { "", 0x10001, 2, new[] { '\uD800','\uDC01', '\uD800','\uDC01' } };
            yield return new object[] { "", 0x10401, 2, new[] { '\uD801','\uDC01', '\uD801','\uDC01' } };
            yield return new object[] { "", 0x10FFFF, 2, new[] { '\uDBFF','\uDFFF', '\uDBFF','\uDFFF'} };

            // Emoji
            yield return new object[] { "", 0x1F600, 3, new[] { '\uD83D', '\uDE00', '\uD83D', '\uDE00', '\uD83D','\uDE00' } };
        }

        [Theory]
        [MemberData(nameof(AppendCodePoint_Repeat_TestData))]
        public void AppendCodePoint_RepeatCount(string value, int codePoint, int repeatCount, char[] expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);
            builder.AppendCodePoint(codePoint, repeatCount);
            AssertExtensions.Equal(expected, builder.AsSpan().ToArray());
        }

        [Theory]
        [InlineData(0x0041, 'A', '\0', 1, 101)]
        [InlineData(0x0041, 'A', '\0', 1, 127)]
        [InlineData(0x0041, 'A', '\0', 1, 256)]
        [InlineData(0x1F600, '\uD83D', '\uDE00', 2, 101)]
        [InlineData(0x1F600, '\uD83D', '\uDE00', 2, 127)]
        [InlineData(0x1F600, '\uD83D', '\uDE00', 2, 256)]
        public void AppendCodePoint_RepeatCount_Large(int codePoint, char first, char second, int charCount, int repeatCount)
        {
            MutableTextBuffer builder = MutableTextBufferFactory();

            builder.AppendCodePoint(codePoint, repeatCount);

            char[] expected = new char[repeatCount * charCount];

            int index = 0;
            for (int i = 0; i < repeatCount; i++)
            {
                expected[index++] = first;
                if (charCount == 2)
                    expected[index++] = second;
            }

            AssertExtensions.Equal(expected, builder.AsSpan().ToArray());
        }

        [Theory]
        [InlineData(0x0041, 'A', '\0', 1)]
        [InlineData(0x1F600, '\uD83D', '\uDE00', 2)]
        public void AppendCodePoint_RepeatCount_GrowsBuffer(int codePoint, char first, char second, int charCount)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(0, 512);
            const int RepeatCount = 100;
            builder.AppendCodePoint(codePoint, RepeatCount);
            Assert.Equal(RepeatCount * charCount, builder.Length);

            int index = 0;
            for (int i = 0; i < RepeatCount; i++)
            {
                Assert.Equal(first, builder[index++]);

                if (charCount == 2)
                    Assert.Equal(second, builder[index++]);
            }
        }

        [Fact]
        public void AppendCodePoint_RepeatCount_AppendsAfterExistingContent()
        {
            MutableTextBuffer builder = MutableTextBufferFactory("Hello");
            builder.AppendCodePoint(0x1F600, 100);
            Assert.Equal("Hello", builder.ToString(0, 5));
            for (int i = 0; i < 100; i++)
            {
                int index = 5 + i * 2;

                Assert.Equal('\uD83D', builder[index]);
                Assert.Equal('\uDE00', builder[index + 1]);
            }
        }

        [Fact]
        public void AppendCodePoint_RepeatCount_FillsCapacityExactly()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(0, 200);
            builder.AppendCodePoint(0x1F600, 100);
            Assert.Equal(200, builder.Length);
        }

        [Fact]
        public void AppendCodePoint_RepeatCount_Invalid()
        {
            AssertExtensions.Throws<ArgumentNullException>(
                "text",
                () => J2N.Text.MutableTextBufferExtensions.AppendCodePoint((MutableTextBuffer)null!, 'A', 1));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "repeatCount",
                () => MutableTextBufferFactory().AppendCodePoint('A', -1));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                () => MutableTextBufferFactory().AppendCodePoint(Character.MinCodePoint - 1, 1));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                () => MutableTextBufferFactory().AppendCodePoint(Character.MaxCodePoint + 1, 1));
        }

        [Fact]
        public void AppendCodePoint_RepeatCount_CapacityExceeded()
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
