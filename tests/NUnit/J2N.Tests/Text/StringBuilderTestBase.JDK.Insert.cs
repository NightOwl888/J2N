// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/Insert.java/
/*
 * Copyright (c) 2003, 2020, Oracle and/or its affiliates. All rights reserved.
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

namespace J2N.Text
{
    public abstract partial class StringBuilderTestBase
    {
        [Test]
        public void Test_insertFalse()
        {
            // Caused an infinite loop before 4914802
            MutableTextBuffer sb = OpenStringBuilderFactory();
            assertEquals("false", sb.Insert(0, false, BooleanFormat.Lowercase).ToString());
        }

        [Test]
        public void Test_insertOffset()
        {
            // 8254082 made the String variant cause an AIOOBE, fixed in 8257511
            assertEquals("efabc", OpenStringBuilderFactory("abc").Insert(0, "def", 1, 3 - 1).ToString()); // J2N: Corrected 2nd parameter
            assertEquals("efabc", OpenStringBuilderFactory("abc".AsCharSequence()).Insert(0, "def".AsCharSequence(), 1, 3 - 1).ToString()); // J2N: Corrected 2nd parameter
            assertEquals("efabc", OpenStringBuilderFactory("abc").Insert(0, OpenStringBuilderFactory("def"), 1, 3 - 1).ToString()); // J2N: Corrected 4th parameter
            assertEquals("efabc", OpenStringBuilderFactory("abc".AsCharSequence()).Insert(0, OpenStringBuilderFactory("def".AsCharSequence()), 1, 3 - 1).ToString()); // J2N: Corrected 4th parameter
            // insert(I[CII) and insert(ILjava/lang/CharSequence;II) are inconsistently specified
            assertEquals("efabc", OpenStringBuilderFactory("abc").Insert(0, new char[] { 'd', 'e', 'f' }, 1, 2).ToString()); // J2N: Checked 4th parameter
            assertEquals("efabc", OpenStringBuilderFactory("abc".AsCharSequence()).Insert(0, new char[] { 'd', 'e', 'f' }.AsCharSequence(), 1, 2).ToString()); // J2N: Checked 4th parameter
        }
    }
}
