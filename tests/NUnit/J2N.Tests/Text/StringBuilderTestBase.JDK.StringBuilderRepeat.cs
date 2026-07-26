// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/StringBuilderRepeat.java/
/*
 * Copyright (c) 2023, 2024, Oracle and/or its affiliates. All rights reserved.
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
#nullable enable

namespace J2N.Text
{
    public abstract partial class StringBuilderTestBase
    {
        private sealed class MyChars : ICharSequence
        {
            private static readonly char[] DATA = ['a', 'b', 'c'];

            public int Length => DATA.Length;

            public bool HasValue => true;

            public char this[int index] => DATA[index];

            public ICharSequence Subsequence(int startIndex, int length)
            {
                return new StringCharSequence(new string(DATA, startIndex, length));
            }

            public override string ToString()
            {
                return new string(DATA);
            }
        }

        [Test]
        public void Test_repeat_sanity()
        {
            MyChars MYCHARS = new();

            TextBuilder sb = StringBuilderFactory();

            // prime the StringBuilder
            sb.Append("repeat");

            // single character Latin1
            sb.Append('1', 0);
            sb.Append('2', 1);
            sb.Append('3', 5);

            // single string Latin1 (optimized)
            sb.Insert(sb.Length, "1", 0);
            sb.Insert(sb.Length, "2", 1);
            sb.Insert(sb.Length, "3", 5);

            // multi string Latin1
            sb.Insert(sb.Length, "-1", 0);
            sb.Insert(sb.Length, "-2", 1);
            sb.Insert(sb.Length, "-3", 5);

            // single character UTF16
            sb.Append('\u2460', 0);
            sb.Append('\u2461', 1);
            sb.Append('\u2462', 5);

            // single string UTF16 (optimized)
            sb.Insert(sb.Length, "\u2460", 0);
            sb.Insert(sb.Length, "\u2461", 1);
            sb.Insert(sb.Length, "\u2462", 5);

            // multi string UTF16
            sb.Insert(sb.Length, "-\u2460", 0);
            sb.Insert(sb.Length, "-\u2461", 1);
            sb.Insert(sb.Length, "-\u2462", 5);

            // CharSequence
            sb.Insert(sb.Length, MYCHARS, 3);

            // null
            sb.Insert(sb.Length, (string?)null, 0); // J2N: These should be no-op in J2N to match the BCL (keeping this in place and repeating with strings below to confirm)
            sb.Insert(sb.Length, (string?)null, 1);
            sb.Insert(sb.Length, (string?)null, 5);

            sb.Insert(sb.Length, "null", 0); // J2N: Added these to make the test pass - need to pass a real string to print the word "null"
            sb.Insert(sb.Length, "null", 1);
            sb.Insert(sb.Length, "null", 5);

            sb.Insert(sb.Length, (ICharSequence?)null, 0); // J2N: These should be no-op in J2N to match the BCL (keeping this in place and repeating with strings below to confirm)
            sb.Insert(sb.Length, (ICharSequence?)null, 1);
            sb.Insert(sb.Length, (ICharSequence?)null, 5);

            sb.Insert(sb.Length, "null", 0); // J2N: Added these to make the test pass - need to pass a real string to print the word "null"
            sb.Insert(sb.Length, "null", 1);
            sb.Insert(sb.Length, "null", 5);

            string expected =
                "repeat233333233333-2-3-3-3-3-3\u2461\u2462\u2462\u2462\u2462\u2462\u2461\u2462\u2462\u2462\u2462\u2462-\u2461-\u2462-\u2462-\u2462-\u2462-\u2462abcabcabc" +
                "nullnullnullnullnullnullnullnullnullnullnullnull";

            Assert.That(sb.ToString(), Is.EqualTo(expected));

            // Codepoints

            sb.Length = 0;

            sb.AppendCodePoint(0, 0);
            sb.AppendCodePoint(0, 1);
            sb.AppendCodePoint(0, 5);

            sb.AppendCodePoint((int)' ', 0);
            sb.AppendCodePoint((int)' ', 1);
            sb.AppendCodePoint((int)' ', 5);

            sb.AppendCodePoint(0x2460, 0);
            sb.AppendCodePoint(0x2461, 1);
            sb.AppendCodePoint(0x2462, 5);

            sb.AppendCodePoint(0x10FFFF, 0);
            sb.AppendCodePoint(0x10FFFF, 1);
            sb.AppendCodePoint(0x10FFFF, 5);

            expected =
                "\u0000\u0000\u0000\u0000\u0000\u0000\u0020\u0020\u0020\u0020\u0020\u0020\u2461\u2462\u2462\u2462\u2462\u2462\udbff\udfff\udbff\udfff\udbff\udfff\udbff\udfff\udbff\udfff\udbff\udfff";

            Assert.That(sb.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void Test_repeat_exceptions()
        {
            MyChars MYCHARS = new();

            TextBuilder sb = StringBuilderFactory();

            AssertExtensions.ThrowsAny<OutOfMemoryException, ArgumentOutOfRangeException>(() =>
            {
                sb.Append(' ', int.MaxValue);
            });

            AssertExtensions.ThrowsAny<OutOfMemoryException, ArgumentOutOfRangeException>(() =>
            {
                sb.Insert(sb.Length, "    ", int.MaxValue);
            });

            AssertExtensions.ThrowsAny<OutOfMemoryException, ArgumentOutOfRangeException>(() =>
            {
                sb.Insert(sb.Length, MYCHARS, int.MaxValue);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                sb.Append(' ', -1);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                sb.Insert(sb.Length, "abc", -1);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                sb.Insert(sb.Length, MYCHARS, -1);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                sb.AppendCodePoint(Character.MaxCodePoint + 1, 1); // J2N: Changed 2nd parameter to be valid so we get the right exception
            });

            Assert.Throws<ArgumentException>(() =>
            {
                sb.AppendCodePoint(Character.MinCodePoint - 1, 1); // J2N: Changed 2nd parameter to be valid so we get the right exception
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                sb.AppendCodePoint(0, -1); // J2N: Changed 1st parameter to be valid so we get the right exception
            });
        }
    }
}
