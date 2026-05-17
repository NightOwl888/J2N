
// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/CharAt.java/
/*
 * Copyright (c) 2021, Oracle and/or its affiliates. All rights reserved.
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

/*
 * @test
 * @bug 8271732
 * @summary Basic test that charAt throws IIOBE as expected for out of bounds indexes.
 * @run testng CharAt
 */

using J2N.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace J2N.Text
{
    public abstract partial class OpenStringBuilderTestBase
    {
        /**
         * StringBuilder/-Buffer.charAt throws:
         * IndexOutOfBoundsException - if index is negative or greater than or equal to length().
         * the test inputs, expected to throw IndexOutOfBoundsException.
         */
        [Test]
        public void Test_charAtIIOBE()
        {
            MutableTextBuffer sb = OpenStringBuilderFactory("test");
            StringBuffer sbuf = new StringBuffer("test");

            MutableTextBuffer sbUtf16 = OpenStringBuilderFactory("\uFF34est"); // Fullwidth Latin Capital Letter T
            StringBuffer sbufUtf16 = new StringBuffer("\uFF34est");

            List<Integer> outOfBoundsIndices = new() { int.MinValue, -2, -1, 4, 5, int.MaxValue };

            foreach (int index in outOfBoundsIndices)
            {
                Assert.Throws<IndexOutOfRangeException>(() => _ = sb[index]);
                Assert.Throws<IndexOutOfRangeException>(() => _ = sbUtf16[index]);
                Assert.Throws<IndexOutOfRangeException>(() => _ = sbuf[index]);
                Assert.Throws<IndexOutOfRangeException>(() => _ = sbufUtf16[index]);
            }
        }
    }
}
