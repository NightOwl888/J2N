// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/BuilderForwarding.java/
/*
 * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

/**
 * @test
 * @bug 6206780
 * @summary  Test forwarding of methods to super in StringBuilder
 * @author Jim Gish <jim.gish@oracle.com>
 */

using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace J2N.Text
{
    public abstract partial class OpenStringBuilderTestBase
    {
        private const string A_STRING_BUFFER_VAL = "aStringBuffer";
        private const string A_STRING_BUILDER_VAL = "aStringBuilder";
        private const string A_STRING_VAL = "aString";
        private const string NON_EMPTY_VAL = "NonEmpty";

        [Test]
        public void Test_appendCharSequence()
        {
            // three different flavors of CharSequence
            ICharSequence aString = A_STRING_VAL.AsCharSequence();
            ICharSequence aStringBuilder = OpenStringBuilderFactory(A_STRING_BUILDER_VAL);
            ICharSequence aStringBuffer = new StringBuffer(A_STRING_BUFFER_VAL);

            assertEquals( /*actual*/ OpenStringBuilderFactory().Append(aString).ToString(), /*expected*/ A_STRING_VAL);
            assertEquals(OpenStringBuilderFactory().Append(aStringBuilder).ToString(), A_STRING_BUILDER_VAL);
            assertEquals(OpenStringBuilderFactory().Append(aStringBuffer).ToString(), A_STRING_BUFFER_VAL);

            assertEquals( /*actual*/ OpenStringBuilderFactory(NON_EMPTY_VAL).Append(aString).ToString(), NON_EMPTY_VAL + A_STRING_VAL);
            assertEquals(OpenStringBuilderFactory(NON_EMPTY_VAL).Append(aStringBuilder).ToString(), NON_EMPTY_VAL + A_STRING_BUILDER_VAL);
            assertEquals(OpenStringBuilderFactory(NON_EMPTY_VAL).Append(aStringBuffer).ToString(), NON_EMPTY_VAL + A_STRING_BUFFER_VAL);
        }

        [Test]
        public void Test_indexOfString()
        {
            MutableTextBuffer sb = OpenStringBuilderFactory();
            Assert.Throws<ArgumentNullException>(() => sb.IndexOf(null));

            sb = OpenStringBuilderFactory("xyz");
            assertEquals(sb.IndexOf("y"), 1);
            assertEquals(sb.IndexOf("not found"), -1);
        }

        [Test]
        public void Test_indexOfStringint()
        {
            MutableTextBuffer sb = OpenStringBuilderFactory();
            Assert.Throws<ArgumentNullException>(() => sb.IndexOf(null, 1));

            sb = OpenStringBuilderFactory("xyyz");
            assertEquals(sb.IndexOf("y", 0), 1);
            assertEquals(sb.IndexOf("y", 1), 1);
            assertEquals(sb.IndexOf("y", 2), 2);
            assertEquals(sb.IndexOf("not found"), -1);
        }

        [Test]
        public void Test_indexOfStringIntNull()
        {
            MutableTextBuffer sb = OpenStringBuilderFactory();

            Assert.Throws<ArgumentNullException>(() => sb.IndexOf(null, 1));
        }

        [Test]
        public void Test_indexOfStringNull()
        {
            MutableTextBuffer sb = OpenStringBuilderFactory();

            Assert.Throws<ArgumentNullException>(() => sb.IndexOf(null));
        }

        [Test]
        public void Test_insertintboolean()
        {
            bool b = true;
            MutableTextBuffer sb = OpenStringBuilderFactory("012345");
            assertEquals(sb.Insert(2, b, BooleanFormat.Lowercase).ToString(), "01true2345");
        }

        [Test]
        public void Test_insertintchar()
        {
            char c = 'C';
            MutableTextBuffer sb = OpenStringBuilderFactory("012345");
            assertEquals(sb.Insert(2, c).ToString(), "01C2345");
        }

        [Test]
        public void Test_insertintCharSequence()
        {
            const string initString = "012345";
            // three different flavors of CharSequence
            ICharSequence aString = A_STRING_VAL.AsCharSequence();
            ICharSequence aStringBuilder = OpenStringBuilderFactory(A_STRING_BUILDER_VAL);
            ICharSequence aStringBuffer = new StringBuffer(A_STRING_BUFFER_VAL);

            assertEquals(OpenStringBuilderFactory(initString).Insert(2, aString).ToString(), "01" + A_STRING_VAL + "2345");

            assertEquals(OpenStringBuilderFactory(initString).Insert(2, aStringBuilder).ToString(), "01" + A_STRING_BUILDER_VAL + "2345");

            assertEquals(OpenStringBuilderFactory(initString).Insert(2, aStringBuffer).ToString(), "01" + A_STRING_BUFFER_VAL + "2345");

            Assert.Throws<ArgumentOutOfRangeException>(() => OpenStringBuilderFactory(initString).Insert(7, aString));
        }

        [Test]
        public void Test_insertintdouble()
        {
            double d = 99d;
            MutableTextBuffer sb = OpenStringBuilderFactory("012345");
            assertEquals(sb.Insert(2, d).ToString(), "0199.02345");
        }

        [Test]
        public void Test_insertintfloat()
        {
            float f = 99.0f;
            MutableTextBuffer sb = OpenStringBuilderFactory("012345");
            assertEquals(sb.Insert(2, f).ToString(), "0199.02345");
        }

        [Test]
        public void Test_insertintint()
        {
            int i = 99;
            MutableTextBuffer sb = OpenStringBuilderFactory("012345");
            assertEquals(sb.Insert(2, i).ToString(), "01992345");
        }

        [Test]
        public void Test_insertintlong()
        {
            long l = 99;
            MutableTextBuffer sb = OpenStringBuilderFactory("012345");
            assertEquals(sb.Insert(2, l).ToString(), "01992345");
        }

        [Test]
        public void Test_insertintObject()
        {
            MutableTextBuffer sb = OpenStringBuilderFactory("012345");
            List<string> ls = new List<string>();
            ls.Add("A"); ls.Add("B");
            string lsString = ls.ToString();
            assertEquals(sb.Insert(2, ls).ToString(), "01" + lsString + "2345");

            Assert.Throws<ArgumentOutOfRangeException>(() => sb.Insert(sb.Length + 1, ls));
        }

        [Test]
        public void Test_lastIndexOfString()
        {
            string xyz = "xyz";
            string xyz3 = "xyzxyzxyz";
            MutableTextBuffer sb = OpenStringBuilderFactory(xyz3);
            int pos = sb.LastIndexOf("xyz");
            assertEquals(pos, 2 * xyz.Length);
        }

        [Test]
        public void Test_lastIndexOfStringint()
        {
            MutableTextBuffer sb = OpenStringBuilderFactory("xyzxyzxyz");
            int pos = sb.LastIndexOf("xyz", 5);
            assertEquals(pos, 3);
            pos = sb.LastIndexOf("xyz", 6);
            assertEquals(pos, 6);
        }
    }
}
