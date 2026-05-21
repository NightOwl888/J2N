// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/CompactStringBuilder.java/
/*
 * Copyright (c) 2015, 2025, Oracle and/or its affiliates. All rights reserved.
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

using J2N.Collections;
using NUnit.Framework;
using System;

namespace J2N.Text
{
    public abstract partial class StringBuilderTestBase
    {
        /*
        * Tests for "A"
        */
        [Test]
        public void TestCompactStringBuilderForLatinA()
        {
            const string ORIGIN = "A";
            /*
             * Because right now ASCII is the default encoding parameter for source
             * code in JDK build environment, so we escape them. same as below.
             */
            check(StringBuilderFactory(ORIGIN).Append(new char[] { '\uFF21' }),
                    "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Append(new StringBuffer("\uFF21")),
                    "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Append("\uFF21"), "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Append(new char[] { 'a', 'b', 'c' }),
                    "Aabc");
            check(StringBuilderFactory(ORIGIN).Append(new char[] { 'a', 'b', 'c' }, 1, 2),
                    "Abc");
            check(StringBuilderFactory(ORIGIN).Append(new StringBuffer("\uFF21")),
                    "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Delete(0, 1), "");
            check(StringBuilderFactory(ORIGIN).Delete(0, 0), "A");
            check(StringBuilderFactory(ORIGIN).RemoveAt(0), "");
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("A", 0), 0);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uFF21", 0), -1);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("", 0), 0);
            assertEquals(StringBuilderFactory(ORIGIN).Insert(1, "\uD801\uDC00")
                    .IndexOf("A", 0), 0);
            assertEquals(StringBuilderFactory(ORIGIN).Insert(0, "\uD801\uDC00")
                    .IndexOf("A", 0), 2);
            check(StringBuilderFactory(ORIGIN).Insert(0, new char[] { }), "A");
            check(StringBuilderFactory(ORIGIN).Insert(1, new char[] { '\uFF21' }),
                    "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Insert(0, new char[] { '\uFF21' }),
                    "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Insert(0, new StringBuffer("\uFF21")),
                    "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Insert(1, new StringBuffer("\uFF21")),
                    "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Insert(0, ""), "A");
            check(StringBuilderFactory(ORIGIN).Insert(0, "\uFF21"), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Insert(1, "\uFF21"), "A\uFF21");
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("A"), 0);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uFF21"), -1);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf(""), 1);
            check(StringBuilderFactory(ORIGIN).Replace(0, 0, "\uFF21"), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Replace(0, 1, "\uFF21"), "\uFF21");
            checkSetCharAt(StringBuilderFactory(ORIGIN), 0, '\uFF21', "\uFF21");
            checkSetLength(StringBuilderFactory(ORIGIN), 0, "");
            checkSetLength(StringBuilderFactory(ORIGIN), 1, "A");
            check(StringBuilderFactory(ORIGIN).AsSpan(0), "A");
            check(StringBuilderFactory(ORIGIN).AsSpan(1), "");
        }

        /*
         * Tests for "\uFF21"
         */
        [Test]
        public void TestCompactStringBuilderForNonLatinA()
        {
            const string ORIGIN = "\uFF21";
            check(StringBuilderFactory(ORIGIN).Append(new char[] { 'A' }), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Append(new StringBuffer("A")), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Append("A"), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Append(new StringBuffer("A")), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Delete(0, 1), "");
            check(StringBuilderFactory(ORIGIN).Delete(0, 0), "\uFF21");
            check(StringBuilderFactory(ORIGIN).RemoveAt(0), "");
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("A", 0), -1);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uFF21", 0), 0);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("", 0), 0);
            check(StringBuilderFactory(ORIGIN).Insert(0, new char[] { }), "\uFF21");
            check(StringBuilderFactory(ORIGIN).Insert(1, new char[] { 'A' }), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Insert(0, new char[] { 'A' }), "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Insert(0, new StringBuffer("A")),
                    "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Insert(1, new StringBuffer("A")),
                    "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Insert(0, ""), "\uFF21");
            check(StringBuilderFactory(ORIGIN).Insert(0, "A"), "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Insert(1, "A"), "\uFF21A");
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("A"), -1);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uFF21"), 0);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf(""), 1);
            check(StringBuilderFactory(ORIGIN).Replace(0, 0, "A"), "A\uFF21");
            check(StringBuilderFactory(ORIGIN).Replace(0, 1, "A"), "A");
            checkSetCharAt(StringBuilderFactory(ORIGIN), 0, 'A', "A");
            checkSetLength(StringBuilderFactory(ORIGIN), 0, "");
            checkSetLength(StringBuilderFactory(ORIGIN), 1, "\uFF21");
            check(StringBuilderFactory(ORIGIN).AsSpan(0), "\uFF21");
            check(StringBuilderFactory(ORIGIN).AsSpan(1), "");
        }

        /*
         * Tests for "\uFF21A"
         */
        [Test]
        public void TestCompactStringBuilderForMixedA1()
        {
            const string ORIGIN = "\uFF21A";
            check(StringBuilderFactory(ORIGIN).Delete(0, 1), "A");
            check(StringBuilderFactory(ORIGIN).Delete(1, 2 - 1), "\uFF21");  // J2N: Corrected 2nd argument
            check(StringBuilderFactory(ORIGIN).RemoveAt(1), "\uFF21");
            check(StringBuilderFactory(ORIGIN).RemoveAt(0), "A");
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("A", 0), 1);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uFF21", 0), 0);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("", 0), 0);
            check(StringBuilderFactory(ORIGIN).Insert(1, new char[] { 'A' }),
                    "\uFF21AA");
            check(StringBuilderFactory(ORIGIN).Insert(0, new char[] { '\uFF21' }),
                    "\uFF21\uFF21A");
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("A"), 1);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uFF21"), 0);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf(""), 2);
            check(StringBuilderFactory(ORIGIN).Replace(0, 0, "A"), "A\uFF21A");
            check(StringBuilderFactory(ORIGIN).Replace(0, 1, "A"), "AA");
            checkSetCharAt(StringBuilderFactory(ORIGIN), 0, 'A', "AA");
            checkSetLength(StringBuilderFactory(ORIGIN), 0, "");
            checkSetLength(StringBuilderFactory(ORIGIN), 1, "\uFF21");
            check(StringBuilderFactory(ORIGIN).AsSpan(0), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).AsSpan(1), "A");
        }

        /*
         * Tests for "A\uFF21"
         */
        [Test]
        public void TestCompactStringBuilderForMixedA2()
        {
            const string ORIGIN = "A\uFF21";
            check(StringBuilderFactory(ORIGIN).Replace(1, 2 - 1, "A"), "AA");  // J2N: Corrected 2nd argument
            checkSetLength(StringBuilderFactory(ORIGIN), 1, "A");
            check(StringBuilderFactory(ORIGIN).AsSpan(0), "A\uFF21");
            check(StringBuilderFactory(ORIGIN).AsSpan(1), "\uFF21");
            check(StringBuilderFactory(ORIGIN).AsSpan(0, 1), "A");
        }

        /*
         * Tests for "\uFF21A\uFF21A\uFF21A\uFF21A\uFF21A"
         */
        [Test]
        public void TestCompactStringBuilderForDuplicatedMixedA1()
        {
            const string ORIGIN = "\uFF21A\uFF21A\uFF21A\uFF21A\uFF21A";
            checkSetLength(StringBuilderFactory(ORIGIN), 1, "\uFF21");
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("A", 5), 5);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uFF21", 5), 6);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("A"), 9);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uFF21"), 8);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf(""), 10);
            check(StringBuilderFactory(ORIGIN).AsSpan(9), "A");
            check(StringBuilderFactory(ORIGIN).AsSpan(8), "\uFF21A");
        }

        /*
         * Tests for "A\uFF21A\uFF21A\uFF21A\uFF21A\uFF21"
         */
        [Test]
        public void TestCompactStringBuilderForDuplicatedMixedA2()
        {
            const string ORIGIN = "A\uFF21A\uFF21A\uFF21A\uFF21A\uFF21";
            checkSetLength(StringBuilderFactory(ORIGIN), 1, "A");
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("A", 5), 6);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uFF21", 5), 5);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("A"), 8);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uFF21"), 9);
            check(StringBuilderFactory(ORIGIN).AsSpan(9), "\uFF21");
            check(StringBuilderFactory(ORIGIN).AsSpan(8), "A\uFF21");
        }

        /*
         * Tests for "\uD801\uDC00\uD801\uDC01"
         */
        [Test]
        public void TestCompactStringForSupplementaryCodePoint()
        {
            const string ORIGIN = "\uD801\uDC00\uD801\uDC01";
            check(StringBuilderFactory(ORIGIN).Append("A"), "\uD801\uDC00\uD801\uDC01A");
            check(StringBuilderFactory(ORIGIN).Append("\uFF21"),
                    "\uD801\uDC00\uD801\uDC01\uFF21");
            check(StringBuilderFactory(ORIGIN).AppendCodePoint('A'),
                    "\uD801\uDC00\uD801\uDC01A");
            check(StringBuilderFactory(ORIGIN).AppendCodePoint('\uFF21'),
                    "\uD801\uDC00\uD801\uDC01\uFF21");
            assertEquals(StringBuilderFactory(ORIGIN)[0], '\uD801');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointAt(0),
                    Character.CodePointAt(ORIGIN, 0));
            assertEquals(StringBuilderFactory(ORIGIN).CodePointAt(1),
                    Character.CodePointAt(ORIGIN, 1));
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(2),
                    Character.CodePointAt(ORIGIN, 0));
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(1, 3 - 1), 2); // J2N: Corrected 2nd argument
            check(StringBuilderFactory(ORIGIN).Delete(0, 2), "\uD801\uDC01");
            check(StringBuilderFactory(ORIGIN).Delete(0, 3), "\uDC01");
            check(StringBuilderFactory(ORIGIN).RemoveAt(1), "\uD801\uD801\uDC01");
            checkGetChars(StringBuilderFactory(ORIGIN), 0, 3, new char[] { '\uD801',
                '\uDC00', '\uD801' });
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uD801\uDC01"), 2);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uDC01"), 3);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uFF21"), -1);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("A"), -1);
            check(StringBuilderFactory(ORIGIN).Insert(0, "\uFF21"),
                    "\uFF21\uD801\uDC00\uD801\uDC01");
            check(StringBuilderFactory(ORIGIN).Insert(1, "\uFF21"),
                    "\uD801\uFF21\uDC00\uD801\uDC01");
            check(StringBuilderFactory(ORIGIN).Insert(1, "A"),
                    "\uD801A\uDC00\uD801\uDC01");
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uDC00\uD801"), 1);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uD801"), 2);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uFF21"), -1);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("A"), -1);
            assertEquals(StringBuilderFactory(ORIGIN).Length, 4);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(1, 1), 2);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(0, 1), 2);
            check(StringBuilderFactory(ORIGIN).Replace(0, 2, "A"), "A\uD801\uDC01");
            check(StringBuilderFactory(ORIGIN).Replace(0, 3, "A"), "A\uDC01");
            check(StringBuilderFactory(ORIGIN).Replace(0, 2, "\uFF21"),
                    "\uFF21\uD801\uDC01");
            check(StringBuilderFactory(ORIGIN).Replace(0, 3, "\uFF21"), "\uFF21\uDC01");
            check(StringBuilderFactory(ORIGIN).Reverse(), "\uD801\uDC01\uD801\uDC00");
            checkSetCharAt(StringBuilderFactory(ORIGIN), 1, '\uDC01',
                    "\uD801\uDC01\uD801\uDC01");
            checkSetCharAt(StringBuilderFactory(ORIGIN), 1, 'A', "\uD801A\uD801\uDC01");
            checkSetLength(StringBuilderFactory(ORIGIN), 2, "\uD801\uDC00");
            checkSetLength(StringBuilderFactory(ORIGIN), 3, "\uD801\uDC00\uD801");
            check(StringBuilderFactory(ORIGIN).AsSpan(1, 3 - 1), "\uDC00\uD801");     // J2N: Corrected 2nd argument
        }

        /*
         * Tests for "A\uD801\uDC00\uFF21"
         */
        [Test]
        public void TestCompactStringForSupplementaryCodePointMixed1()
        {
            const string ORIGIN = "A\uD801\uDC00\uFF21";
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(3),
                    Character.CodePointAt(ORIGIN, 1));
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(2), '\uD801');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(1), 'A');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(0, 3), 2);
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(0, 4), 3);
            check(StringBuilderFactory(ORIGIN).Delete(0, 1), "\uD801\uDC00\uFF21");
            check(StringBuilderFactory(ORIGIN).Delete(0, 1).Delete(2, 3 - 2),          // J2N: Corrected 2nd argument
                    "\uD801\uDC00");
            check(StringBuilderFactory(ORIGIN).RemoveAt(3).RemoveAt(0),
                    "\uD801\uDC00");
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("\uFF21"), 3);
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("A"), 0);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("\uFF21"), 3);
            assertEquals(StringBuilderFactory(ORIGIN).LastIndexOf("A"), 0);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(0, 1), 1);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(1, 1), 3);
            check(StringBuilderFactory(ORIGIN).Replace(1, 3 - 1, "A"), "AA\uFF21");    // J2N: Corrected 2nd argument
            check(StringBuilderFactory(ORIGIN).Replace(1, 4 - 1, "A"), "AA");          // J2N: Corrected 2nd argument
            check(StringBuilderFactory(ORIGIN).Replace(1, 4 - 1, ""), "A");            // J2N: Corrected 2nd argument
            check(StringBuilderFactory(ORIGIN).Reverse(), "\uFF21\uD801\uDC00A");
            checkSetLength(StringBuilderFactory(ORIGIN), 1, "A");
            check(StringBuilderFactory(ORIGIN).AsSpan(0, 1), "A");
        }

        /*
         * Tests for "\uD801\uDC00\uFF21A"
         */
        [Test]
        public void TestCompactStringForSupplementaryCodePointMixed2()
        {
            const string ORIGIN = "\uD801\uDC00\uFF21A";
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(3),
                    Character.CodePointAt(ORIGIN, 2));
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(2),
                    Character.CodePointAt(ORIGIN, 0));
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(1), '\uD801');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(0, 3), 2);
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(0, 4), 3);
            check(StringBuilderFactory(ORIGIN).Delete(0, 2), "\uFF21A");
            check(StringBuilderFactory(ORIGIN).Delete(0, 3), "A");
            check(StringBuilderFactory(ORIGIN).RemoveAt(0).RemoveAt(0)
                    .RemoveAt(0), "A");
            assertEquals(StringBuilderFactory(ORIGIN).IndexOf("A"), 3);
            assertEquals(StringBuilderFactory(ORIGIN).Delete(0, 3).IndexOf("A"), 0);
            assertEquals(StringBuilderFactory(ORIGIN).Replace(0, 3, "B").IndexOf("A"),
                    1);
            assertEquals(StringBuilderFactory(ORIGIN).AsSpan(3, 4 - 3).IndexOf("A"), 0);   // J2N: Corrected 2nd argument
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(1, 1), 2);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(0, 1), 2);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(2, 1), 3);
            check(StringBuilderFactory(ORIGIN).Replace(0, 3, "B"), "BA");
            check(StringBuilderFactory(ORIGIN).Reverse(), "A\uFF21\uD801\uDC00");
        }

        /*
         * Tests for "\uD801A\uDC00\uFF21"
         */
        [Test]
        public void TestCompactStringForSupplementaryCodePointMixed3()
        {
            const string ORIGIN = "\uD801A\uDC00\uFF21";
            assertEquals(StringBuilderFactory(ORIGIN).CodePointAt(1), 'A');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointAt(3), '\uFF21');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(1), '\uD801');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(2), 'A');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(3), '\uDC00');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(0, 3), 3);
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(1, 3 - 1), 2);   // J2N: Corrected 2nd argument
            assertEquals(StringBuilderFactory(ORIGIN).Delete(0, 1).Delete(1, 3 - 1)
                    .IndexOf("A"), 0);
            assertEquals(
                    StringBuilderFactory(ORIGIN).Replace(0, 1, "B").Replace(2, 4 - 2, "C") // J2N: Corrected 2nd argument
                            .IndexOf("A"), 1);
            assertEquals(StringBuilderFactory(ORIGIN).AsSpan(1, 4 - 1).Slice(0, 1)    // J2N: Corrected 2nd argument
                    .IndexOf("A"), 0);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(0, 1), 1);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(1, 1), 2);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(2, 1), 3);
            check(StringBuilderFactory(ORIGIN).Reverse(), "\uFF21\uDC00A\uD801");
        }

        /*
         * Tests for "A\uDC01\uFF21\uD801"
         */
        [Test]
        public void TestCompactStringForSupplementaryCodePointMixed4()
        {
            const string ORIGIN = "A\uDC01\uFF21\uD801";
            assertEquals(StringBuilderFactory(ORIGIN).CodePointAt(1), '\uDC01');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointAt(3), '\uD801');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(1), 'A');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(2), '\uDC01');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointBefore(3), '\uFF21');
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(0, 3), 3);
            assertEquals(StringBuilderFactory(ORIGIN).CodePointCount(1, 3 - 1), 2);        // J2N: Corrected 2nd argument
            assertEquals(StringBuilderFactory(ORIGIN).Delete(1, 4 - 1).IndexOf("A"), 0);   // J2N: Corrected 2nd argument
            assertEquals(StringBuilderFactory(ORIGIN).Replace(1, 4 - 1, "B").IndexOf("A"), // J2N: Corrected 2nd argument
                    0);
            assertEquals(StringBuilderFactory(ORIGIN).AsSpan(0, 1).IndexOf("A"), 0);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(0, 1), 1);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(1, 1), 2);
            assertEquals(StringBuilderFactory(ORIGIN).OffsetByCodePoints(2, 1), 3);
            check(StringBuilderFactory(ORIGIN).Reverse(), "\uD801\uFF21\uDC01A");
        }

        /*
         * Tests for maybeLatin1 attribute
         */
        [Test]
        public void TestCompactStringForMaybeLatin1()
        {
            TextBuilder sb = StringBuilderFactory("A\uDC01");

            sb.Length = 0;      // maybeLatin1 become true
            check(sb, "");
            check(StringBuilderFactory(sb).Append('A'), "A");
            check(StringBuilderFactory().Append(sb), "");

            sb = StringBuilderFactory("A\uDC01");
            sb[1] = 'B';   // maybeLatin1 become true
            check(sb, "AB");
            check(StringBuilderFactory(sb).Append('A'), "ABA");
            check(StringBuilderFactory().Append(sb), "AB");

            sb = StringBuilderFactory("A\uDC01");
            sb.RemoveAt(1);   // maybeLatin1 become true
            check(sb, "A");
            check(StringBuilderFactory(sb).Append('B'), "AB");
            check(StringBuilderFactory().Append(sb), "A");

            sb = StringBuilderFactory("A\uDC01\uFF21\uD801");
            sb.Delete(1, 4 - 1); // J2N: Corrected 2nd argument
            check(sb, "A");      // maybeLatin1 become true
            check(StringBuilderFactory(sb).Append('B'), "AB");
            check(StringBuilderFactory().Append(sb), "A");

            sb = StringBuilderFactory("A\uDC01\uFF21\uD801");
            sb.Replace(1, 4 - 1, "B"); // J2N: Corrected 2nd argument
            check(sb, "AB");      // maybeLatin1 become true
            check(StringBuilderFactory(sb).Append('A'), "ABA");
            check(StringBuilderFactory().Append(sb), "AB");
        }

        // Test cases to force expanding the capacity during replace.
        // Start with a known capacity and a known initial value that almost fills it.
        // Use both latin1 and utf16 initial values and replacement values.
        // Iterate through cases of SB.replace start and end values and various lengths of replacement.
        // The results are checked by composing the new string using concatenation of the segments.
        [Test]
        public void TestGrowingCapacityReplace()
        {
            const int INIT_CAPACITY = 8;
            string[] INITIAL_CHARS = ["A", "\u0100"];
            string[] REPLACEMENT_CHARS = ["B", "\u0101"];
            foreach (string INITIAL in INITIAL_CHARS)
            {
                foreach (string REPLACEMENT in REPLACEMENT_CHARS)
                {
                    for (int initLen = INIT_CAPACITY - 1; initLen < INIT_CAPACITY + 2; initLen++)
                    {
                        string orig = Repeat(INITIAL, initLen);
                        for (int start = INIT_CAPACITY - 4; start < orig.Length; start++)
                        {
                            for (int end = start; end < orig.Length; end++)
                            {
                                for (int insLen = 0; insLen < 2; insLen++)
                                {
                                    string repl = Repeat(REPLACEMENT, insLen);
                                    var sb = StringBuilderFactory(INIT_CAPACITY)
                                            .Append(orig);
                                    int capBefore = sb.Capacity;
                                    sb.Replace(start, end - start, repl); // J2N: Corrected 2nd argument
                                    int capAfter = sb.Capacity;
                                    string expected = genReplacementString(orig, start, end, repl);
                                    try
                                    {
                                        check(sb, expected);
                                    }
                                    catch (Exception)
                                    {
                                        TestContext.WriteLine($"repl: \"{repl}\", actual: {sb}, expected: {expected}");
                                        TestContext.WriteLine(
                                            $"    insLen: {insLen}, gap: {end - start}, beforeLen: {orig.Length}, " +
                                            $"afterLen: {sb.Length}, capBefore: {capBefore}, capAfter: {capAfter}");
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string Repeat(string orig, int repeatCount)
        {
            using ValueStringBuilder sb = new(stackalloc char[64]);
            while (repeatCount > 0)
            {
                sb.Append(orig);
                repeatCount--;
            }
            return sb.ToString();
        }

        // Construct the replacement string using string concat of the segments
        private static string genReplacementString(string orig, int start, int end, string repl)
        {
            return orig.Substring(0, start) +
                    repl +
                    orig.Substring(end, orig.Length - end); // J2N: Corrected 2nd argument
        }

        private void checkGetChars(TextBuilder sb, int srcBegin, int srcEnd,
                char[] expected)
        {
            char[] dst = new char[srcEnd - srcBegin];
            sb.CopyTo(srcBegin, dst, 0, srcEnd - srcBegin); // J2N: Corrected 4th argument
            assertTrue(Arrays.Equals(dst, expected));
        }

        private void checkSetCharAt(TextBuilder sb, int index, char ch,
                string expected)
        {
            sb[index] = ch;
            check(sb, expected);
        }

        private void checkSetLength(TextBuilder sb, int newLength, string expected)
        {
            sb.Length = newLength;
            check(sb, expected);
        }

        private void check(TextBuilder sb, string expected)
        {
            check(sb.AsSpan(), expected);
        }

        private void check(ReadOnlySpan<char> str, string expected)
        {
            assertTrue($"Get ({escapeNonASCIIs(str)}) but expect ({escapeNonASCIIs(expected)}), ",
                str.Equals(expected, StringComparison.Ordinal));
        }

        /*
         * Escape non-ASCII characters since not all systems support them.
         */
        private string escapeNonASCIIs(ReadOnlySpan<char> str)
        {
            using ValueStringBuilder sb = new(stackalloc char[64]);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c > 0x7F)
                {
                    sb.Append("\\u");
                    sb.Append(((int)c).ToHexString());
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
