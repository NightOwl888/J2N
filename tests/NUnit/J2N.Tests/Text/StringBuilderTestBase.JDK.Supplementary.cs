// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/Supplementary.java/
/*
 * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

using NUnit.Framework;
using System;
using System.Globalization;

namespace J2N.Text
{
    /**
     *
     * @test
     * @bug 4533872 4915683 4985217 5017280
     * @summary Unit tests for supplementary character support (JSR-204)
     */

    public abstract partial class OpenStringBuilderTestBase
    {
        /* Text strings which are used as input data.
         * The comment above each text string means the index of each 16-bit char
         * for convenience.
         */
        static readonly string[] input =
        {
            /*                               111     1     111111     22222
               0123     4     5678     9     012     3     456789     01234 */
            "abc\uD800\uDC00def\uD800\uD800ab\uD800\uDC00cdefa\uDC00bcdef",
            /*                          1     1111     1111     1     222
               0     12345     6789     0     1234     5678     9     012     */
            "\uD800defg\uD800hij\uD800\uDC00klm\uDC00nop\uDC00\uD800rt\uDC00",
            /*                          11     1     1111     1     112     222
               0     12345     6     78901     2     3456     7     890     123     */
            "\uDC00abcd\uDBFF\uDFFFefgh\uD800\uDC009ik\uDC00\uDC00lm\uDC00no\uD800",
            /*                                    111     111111     1 22     2
               0     1     2345     678     9     012     345678     9 01     2     */
            "\uD800\uDC00!#$\uD800%&\uD800\uDC00;+\uDC00<>;=^\uDC00\\@\uD800\uDC00",

            // includes an undefined supplementary character in Unicode 4.0.0
            /*                                    1     11     1     1111     1
               0     1     2345     6     789     0     12     3     4567     8     */
            "\uDB40\uDE00abc\uDE01\uDB40de\uDB40\uDE02f\uDB40\uDE03ghi\uDB40\uDE02",
        };


        /* Expected results for:
         *     test1(): for codePointAt()
         *
         * Each character in each array is the golden data for each text string
         * in the above input data. For example, the first data in each array is
         * for the first input string.
         */
        static readonly int[][] golden1 =
        {
            new[] { 'a',    0xD800, 0xDC00,  0x10000, 0xE0200 }, // codePointAt(0)
            new[] { 0xD800, 0x10000, 'g',    0xDC00,  0xE0202 }, // codePointAt(9)
            new[] { 'f',    0xDC00,  0xD800, 0xDC00,  0xDE02 },  // codePointAt(length-1)
        };

        /*
         * Test for codePointAt(int index) method
         */
        [Test]
        public void Test_test1()
        {
            for (int i = 0; i < input.Length; i++)
            {
                MutableTextBuffer sb = OpenStringBuilderFactory(input[i]);

                /*
                 * Normal case
                 */
                testCodePoint(At, sb, 0, golden1[0][i]);
                testCodePoint(At, sb, 9, golden1[1][i]);
                testCodePoint(At, sb, sb.Length - 1, golden1[2][i]);

                /*
                 * Abnormal case - verify that an exception is thrown.
                 */
                testCodePoint(At, sb, -1);
                testCodePoint(At, sb, sb.Length);
            }
        }


        /* Expected results for:
         *     test2(): for codePointBefore()
         *
         * Each character in each array is the golden data for each text string
         * in the above input data. For example, the first data in each array is
         * for the first input string.
         */
        static readonly int[][] golden2 =
        {
            new[] { 'a',    0xD800, 0xDC00,  0xD800,  0xDB40 },  // codePointBefore(1)
            new[] { 0xD800, 'l',    0x10000, 0xDC00,  0xDB40 },  // codePointBefore(13)
            new[] { 'f',    0xDC00, 0xD800,  0x10000, 0xE0202 }, // codePointBefore(length)
        };

        /*
         * Test for codePointBefore(int index) method
         */
        [Test]
        public void Test_test2()
        {
            for (int i = 0; i < input.Length; i++)
            {
                MutableTextBuffer sb = OpenStringBuilderFactory(input[i]);

                /*
                 * Normal case
                 */
                testCodePoint(Before, sb, 1, golden2[0][i]);
                testCodePoint(Before, sb, 13, golden2[1][i]);
                testCodePoint(Before, sb, sb.Length, golden2[2][i]);

                /*
                 * Abnormal case - verify that an exception is thrown.
                 */
                testCodePoint(Before, sb, 0);
                testCodePoint(Before, sb, sb.Length + 1);
            }
        }


        /* Expected results for:
         *     test3(): for reverse()
         *
         * Unlike golden1 and golden2, each array is the golden data for each text
         * string in the above input data. For example, the first array is for
         * the first input string.
         */
        static readonly string[] golden3 =
        {
            "fedcb\uDC00afedc\uD800\uDC00ba\uD800\uD800fed\uD800\uDC00cba",
            "\uDC00tr\uD800\uDC00pon\uDC00mlk\uD800\uDC00jih\uD800gfed\uD800",
            "\uD800on\uDC00ml\uDC00\uDC00ki9\uD800\uDC00hgfe\uDBFF\uDFFFdcba\uDC00",
            "\uD800\uDC00@\\\uDC00^=;><\uDC00+;\uD800\uDC00&%\uD800$#!\uD800\uDC00",

            // includes an undefined supplementary character in Unicode 4.0.0
            "\uDB40\uDE02ihg\uDB40\uDE03f\uDB40\uDE02ed\uDB40\uDE01cba\uDB40\uDE00",
        };

        // Additional input data & expected result for test3()
        static readonly string[][] testdata1 =
        {
            new[] { "a\uD800\uDC00", "\uD800\uDC00a" },
            new[] { "a\uDC00\uD800", "\uD800\uDC00a" },
            new[] { "\uD800\uDC00a", "a\uD800\uDC00" },
            new[] { "\uDC00\uD800a", "a\uD800\uDC00" },
            new[] { "\uDC00\uD800\uD801", "\uD801\uD800\uDC00" },
            new[] { "\uDC00\uD800\uDC01", "\uD800\uDC01\uDC00" },
            new[] { "\uD801\uD800\uDC00", "\uD800\uDC00\uD801" },
            new[] { "\uD800\uDC01\uDC00", "\uDC00\uD800\uDC01" },
            new[] { "\uD800\uDC00\uDC01\uD801", "\uD801\uDC01\uD800\uDC00" },
        };

        /*
         * Test for reverse() method
         */
        [Test]
        public void Test_test3()
        {
            for (int i = 0; i < input.Length; i++)
            {
                MutableTextBuffer sb = OpenStringBuilderFactory(input[i]).Reverse();

                check(!golden3[i].Equals(sb.ToString(), StringComparison.Ordinal),
                     "reverse() for <" + toHexString(input[i]) + ">",
                     sb, golden3[i]);
            }

            for (int i = 0; i < testdata1.Length; i++)
            {
                MutableTextBuffer sb = OpenStringBuilderFactory(testdata1[i][0]).Reverse();

                check(!testdata1[i][1].Equals(sb.ToString(), StringComparison.Ordinal),
                     "reverse() for <" + toHexString(testdata1[i][0]) + ">",
                     sb, testdata1[i][1]);
            }
        }

        /**
         * Test for appendCodePoint() method
         */
        [Test]
        public void Test_test4()
        {
            for (int i = 0; i < input.Length; i++)
            {
                string s = input[i];
                MutableTextBuffer sb = OpenStringBuilderFactory();
                int c;
                for (int j = 0; j < s.Length; j += Character.CharCount(c))
                {
                    c = s.CodePointAt(j);
                    MutableTextBuffer rsb = sb.AppendCodePoint(c);
                    check(sb != rsb, "appendCodePoint returned a wrong object");
                    int sbc = sb.CodePointAt(j);
                    check(sbc != c, "appendCodePoint(" + j + ") != c", sbc, c);
                }
                check(!s.Equals(sb.ToString()),
                      "appendCodePoint() produced a wrong result with input[" + i + "]");
            }

            // test exception
            testAppendCodePoint(-1, typeof(ArgumentException));
            testAppendCodePoint(0x10FFFF + 1, typeof(ArgumentException));
        }

        /**
         * Test codePointCount(int, int)
         *
         * This test case assumes that
         * Character.codePointCount(CharSequence, int, int) works
         * correctly.
         */
        [Test]
        public void Test_test5()
        {
            for (int i = 0; i < input.Length; i++)
            {
                string s = input[i];
                MutableTextBuffer sb = OpenStringBuilderFactory(s);
                int length = sb.Length;

                for (int j = 0; j <= length; j++)
                {
                    int result = sb.CodePointCount(j, length - j); // J2N: Corrected 2nd argument
                    int expected = Character.CodePointCount(sb.AsSpan(j, length - j)); // J2N: Corrected 2nd argument

                    check(result != expected,
                          "codePointCount(input[" + i + "], " + j + ", " + length + ")",
                          result, expected);
                }

                for (int j = length; j >= 0; j--)
                {
                    int result = sb.CodePointCount(0, j);
                    int expected = Character.CodePointCount(sb.AsSpan(0, j)); //CountCodePoints(sb.AsSpan(0, j));

                    check(result != expected,
                          "codePointCount(input[" + i + "], 0, " + j + ")",
                          result, expected);
                }

                // test exceptions
                testCodePointCount(null!, 0, 0, typeof(NullReferenceException)); // J2N: Not sure what the point of this is - the only way to change this is to use extension methods.
                testCodePointCount(sb, -1, length, typeof(ArgumentOutOfRangeException));
                testCodePointCount(sb, 0, length + 1, typeof(ArgumentOutOfRangeException));
                testCodePointCount(sb, length, length - 1, typeof(ArgumentOutOfRangeException));
            }
        }

        /**
         * Test offsetByCodePoints(int, int)
         *
         * This test case assumes that
         * Character.codePointCount(CharSequence, int, int) works
         * correctly.
         */
        [Test]
        public void Test_test6()
        {
            for (int i = 0; i < input.Length; i++)
            {
                string s = input[i];
                MutableTextBuffer sb = OpenStringBuilderFactory(s);
                int length = s.Length;

                for (int j = 0; j <= length; j++)
                {
                    int nCodePoints = Character.CodePointCount(sb.AsSpan(j, length - j)); // J2N: Corrected 2nd argument

                    int result = sb.OffsetByCodePoints(j, nCodePoints);

                    check(result != length,
                          "offsetByCodePoints(input[" + i + "], " + j + ", " + nCodePoints + ")",
                          result, length);

                    result = sb.OffsetByCodePoints(length, -nCodePoints);

                    int expected = j;

                    if (j > 0 && j < length)
                    {
                        int cp = sb.CodePointBefore(j + 1);
                        if (Character.IsSupplementaryCodePoint(cp))
                        {
                            expected--;
                        }
                    }

                    check(result != expected,
                          "offsetByCodePoints(input[" + i + "], " + j + ", " + (-nCodePoints) + ")",
                          result, expected);
                }

                for (int j = length; j >= 0; j--)
                {
                    int nCodePoints = Character.CodePointCount(sb.AsSpan(0, j));

                    int result = sb.OffsetByCodePoints(0, nCodePoints);

                    int expected = j;

                    if (j > 0 && j < length)
                    {
                        int cp = sb.CodePointAt(j - 1);
                        if (Character.IsSupplementaryCodePoint(cp))
                        {
                            expected++;
                        }
                    }

                    check(result != expected,
                          "offsetByCodePoints(input[" + i + "], 0, " + nCodePoints + ")",
                          result, expected);

                    result = sb.OffsetByCodePoints(j, -nCodePoints);

                    check(result != 0,
                          "offsetBycodePoints(input[" + i + "], " + j + ", " + (-nCodePoints) + ")",
                          result, 0);
                }

                // test exceptions
                testOffsetByCodePoints(null!, 0, 0, typeof(NullReferenceException));
                testOffsetByCodePoints(sb, -1, length, typeof(ArgumentOutOfRangeException));
                testOffsetByCodePoints(sb, 0, length + 1, typeof(ArgumentOutOfRangeException));
                testOffsetByCodePoints(sb, 1, -2, typeof(ArgumentOutOfRangeException));
                testOffsetByCodePoints(sb, length, length - 1, typeof(ArgumentOutOfRangeException));
                testOffsetByCodePoints(sb, length, -(length + 1), typeof(ArgumentOutOfRangeException));
            }
        }

        [Test]
        public void Test_testDontReadOutOfBoundsTrailingSurrogate()
        {
            MutableTextBuffer sb = OpenStringBuilderFactory();

            int suppl = Character.MinSupplementaryCodePoint;
            sb.AppendCodePoint(suppl);

            check(sb.CodePointAt(0) != suppl,
                  "codePointAt(0)", sb.CodePointAt(0), suppl);

            check(sb.Length != 2, "sb.Length()");

            sb.Length = 1;

            check(sb.Length != 1, "sb.Length()");

            ReadOnlySpan<char> codePoints = Character.ToChars(suppl, stackalloc char[2]);

            check(sb.CodePointAt(0) != codePoints[0], // J2N TODO: Missing APIs Character.GetHighSurrogate() and Character.GetLowSurrogate()
                  "codePointAt(0)",
                  sb.CodePointAt(0),
                  codePoints[0]);
        }

        static readonly bool At = true, Before = false;

        static void testCodePoint(bool isAt, MutableTextBuffer sb, int index, int expected)
        {
            int c = isAt ? sb.CodePointAt(index) : sb.CodePointBefore(index);

            check(c != expected,
                  "codePoint" + (isAt ? "At" : "Before") + "(" + index + ") for <"
                  + sb + ">", c, expected);
        }

        static void testCodePoint(bool isAt, MutableTextBuffer sb, int index)
        {
            if (isAt)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => sb.CodePointAt(index));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => sb.CodePointBefore(index));
            }
        }

        void testAppendCodePoint(int codePoint, Type expectedException)
        {
            Assert.Throws(expectedException, () =>
            {
                OpenStringBuilderFactory().AppendCodePoint(codePoint);
            });
        }

        static void testCodePointCount(MutableTextBuffer sb, int beginIndex, int endIndex,
            Type expectedException)
        {
            Assert.Throws(expectedException, () =>
            {
                sb.CodePointCount(beginIndex, endIndex - beginIndex); // J2N: Corrected 2nd argument
            });
        }

        static void testOffsetByCodePoints(MutableTextBuffer sb, int index, int offset,
            Type expectedException)
        {
            Assert.Throws(expectedException, () =>
            {
                sb.OffsetByCodePoints(index, offset);
            });
        }

        static void check(bool err, string msg)
        {
            if (err)
            {
                Assert.Fail("Error: " + msg);
            }
        }

        static void check(bool err, string s, int got, int expected)
        {
            if (err)
            {
                Assert.Fail("Error: " + s
                    + " returned an unexpected value. got "
                    + toHexString(got)
                    + ", expected "
                    + toHexString(expected));
            }
        }

        static void check(bool err, string s, MutableTextBuffer got, string expected)
        {
            if (err)
            {
                Assert.Fail("Error: " + s
                    + " returned an unexpected value. got <"
                    + toHexString(got.ToString())
                    + ">, expected <"
                    + toHexString(expected)
                    + ">");
            }
        }

        private static string toHexString(int c)
        {
            return c.ToHexString();
        }

        private static string toHexString(string s)
        {
            using ValueStringBuilder sb = new(stackalloc char[64]);

            foreach (char c in s)
            {
                sb.Append(" 0x");

                if (c < 0x10) sb.Append('0');
                if (c < 0x100) sb.Append('0');
                if (c < 0x1000) sb.Append('0');

                sb.Append(((int)c).ToString("x", CultureInfo.InvariantCulture));
            }

            sb.Append(' ');

            return sb.ToString();
        }
    }
}
