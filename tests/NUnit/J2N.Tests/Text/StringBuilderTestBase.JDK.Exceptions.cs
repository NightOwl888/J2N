// Source: https://github.com/openjdk/jdk/blob/jdk-27%2B21/test/jdk/java/lang/StringBuilder/Exceptions.java/
/*
 * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 6248507
 * @summary Verify that exceptions are thrown as expected.
 */

using NUnit.Framework;
using System;

namespace J2N.Text
{
    /// <summary>
    /// Port of OpenJDK Exceptions.java test.
    /// Adapted to NUnit idioms and .NET exception behavior.
    /// </summary>
    public abstract partial class StringBuilderTestBase
    {
        private static void Pass(string scenario)
        {
            TestContext.WriteLine($"{scenario} -- OK");
        }

        private static void TryCatch<TException>(
            string scenario,
            string expectedMessage,
            TestDelegate code)
            where TException : Exception
        {
            TException ex = Assert.Throws<TException>(code);

            if (expectedMessage != null)
            {
                Assert.AreEqual(expectedMessage, ex.Message, scenario);
            }

            Pass(scenario);
        }

        private static void TryPass(string scenario, TestDelegate code)
        {
            Assert.DoesNotThrow(code, scenario);
            Pass(scenario);
        }

        [Test]
        public void Test_Exceptions()
        {
            TestContext.WriteLine("TextBuilder()");
            TryPass("  no args", () =>
            {
                _ = StringBuilderFactory();
            });

            TestContext.WriteLine("TextBuilder(int capacity)");
            TryPass("  1", () =>
            {
                _ = StringBuilderFactory(1);
            });

            TryCatch<ArgumentOutOfRangeException>(
                "  -1",
                null, // BCL/J2N messages vary by framework/runtime
                () =>
                {
                    _ = StringBuilderFactory(-1);
                });

            TestContext.WriteLine("TextBuilder(string value)");

            // J2N: We are allowing null to be a no-op to match the BCL
            //TryCatch<ArgumentNullException>(
            //    "  null",
            //    null,
            //    () =>
            //    {
            //        _ = OpenStringBuilderFactory((string)null);
            //    });

            TryPass("  null", () =>
            {
                _ = StringBuilderFactory((string)null);
            });

            TryPass("  foo", () =>
            {
                _ = StringBuilderFactory("foo");
            });

            TestContext.WriteLine("TextBuilder.Replace(int startIndex, int count, string newValue)");

            TryCatch<ArgumentOutOfRangeException>(
                "  -1, 2 - -1, \" \"",
                null,
                () =>
                {
                    var sb = StringBuilderFactory("hilbert");
                    sb.Replace(-1, 2 - -1, " "); // J2N: Corrected 2nd argument
                });

            TryCatch<ArgumentOutOfRangeException>(
                "  7, 8 - 7, \" \"",
                null,
                () =>
                {
                    var sb = StringBuilderFactory("banach");
                    sb.Replace(7, 8 - 7, " "); // J2N: Corrected 2nd argument
                });

            TryCatch<ArgumentOutOfRangeException>(
                "  2, 1 - 2, \" \"",
                null,
                () =>
                {
                    var sb = StringBuilderFactory("riemann");
                    sb.Replace(2, 1 - 2, " "); // J2N: Corrected 2nd argument
                });
        }
    }
}
