// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/AppendStringBuffer.java/
/*
 * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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

/* @test
 * @bug 6206780
 * @summary Test StringBuilder.append(StringBuffer);
 * @key randomness
 */

using NUnit.Framework;
using System;

namespace J2N.Text
{
    public abstract partial class StringBuilderTestBase
    {
        private static Randomizer generator = new Randomizer();

        // J2N TODO: Evaluate the best way to convert this test to use SynchronizedTextBuffer instead of StringBuffer.
        // We currently have no APIs on TextBuilder that append a SynchronizedTextBuffer without explicit external locking.
        // It seems like having an Append(SynchronizedTextBuffer?) overload might be appropriate here, but if we go that route,
        // we also need to cover other common APIs such as Equals(), CompareTo(), Insert(), Replace(), Repeat, etc. It may ultimately
        // be more sensible to support that entire path through ICharSequence overloads (SynchronizedTextBuilder.AsCharSequence()),
        // revert to having ICharSequence implemented dirctly on SynchronizedTextBuilder, or perhaps creating a new interface that
        // represents a type that must be synchronized. Since implicit conversion to ReadOnlySpan<char> on SynchronizedTextBuilder
        // is impractical because nothing we ever do with it will correctly synchronize the type, it seems like implementing
        // ICharSequence directly would solve that problem in a way that doesn't add extra APIs onto MutableTextBuffer (except perhaps
        // Equals, which doesn't curently support ICharSequence).

        [Test]
        public void Test_Append_StringBuffer()
        {
            for (int i=0; i<1000; i++)
            {
                StringBuffer sb1 = generateTestBuffer(10, 100);
                StringBuffer sb2 = generateTestBuffer(10, 100);
                StringBuffer sb3 = generateTestBuffer(10, 100);
                String s1 = sb1.ToString();
                String s2 = sb2.ToString();
                String s3 = sb3.ToString();

                String concatResult = s1 + s2 + s3;

                TextBuilder test = StringBuilderFactory();
                test.Append(sb1);
                test.Append(sb2);
                test.Append(sb3);

                assertEquals("StringBuffer.append failure", concatResult, test.ToString());
            }
        }

        private static int getRandomIndex(int constraint1, int constraint2)
        {
            int range = constraint2 - constraint1;
            int x = generator.Next(range);
            return constraint1 + x;
        }

        private static StringBuffer generateTestBuffer(int min, int max)
        {
            StringBuffer aNewStringBuffer = new StringBuffer(120);
            int aNewLength = getRandomIndex(min, max);
            for (int y = 0; y < aNewLength; y++)
            {
                int achar = generator.Next(30) + 30;
                char test = (char)(achar);
                aNewStringBuffer.Append(test);
            }
            return aNewStringBuffer;
        }
    }
}
