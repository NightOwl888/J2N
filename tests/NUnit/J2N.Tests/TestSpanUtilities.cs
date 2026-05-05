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

using J2N.Globalization;
using NUnit.Framework;
using System;

namespace J2N
{
    public class TestSpanUtilities : TestCase
    {
        // ============================
        // DATA SOURCES
        // ============================

        // --- IndexOf (2-arg) ---
        private static object[] IndexOf_2ArgCases =
        {
            // fixture, value, expected

            // Basic (Apache Harmony parity)
            new object[] { "0123456789", "0", 0 },
            new object[] { "0123456789", "012", 0 },
            new object[] { "0123456789", "02", -1 },
            new object[] { "0123456789", "89", 8 },

            // Empty value
            new object[] { "0123456789", "", 0 },
            new object[] { "", "foo", -1 },
            new object[] { "", "", 0 },

            // Overlapping
            new object[] { "ababa", "aba", 0 },
        };

        // --- IndexOf (3-arg) ---
        private static object[] IndexOf_3ArgCases =
        {
            // fixture, value, startIndex, expected

            new object[] { "0123456789", "89", 5, 8 },
            new object[] { "0123456789", "0", 5, -1 },

            // Empty value
            new object[] { "0123456789", "", 5, 5 },

            // Clamping (JDK-style)
            new object[] { "0123456789", "0", -5, 0 },
            new object[] { "0123456789", "0", 100, -1 },
            new object[] { "0123456789", "", -5, 0 },
            new object[] { "0123456789", "", 100, 10 },

            // Empty span
            new object[] { "", "", 0, 0 },
            new object[] { "", "", -5, 0 },
            new object[] { "", "", 100, 0 },
            new object[] { "", "a", 0, -1 },

            // Overlapping
            new object[] { "ababa", "aba", 0, 0 },
            new object[] { "ababa", "aba", 1, 2 },
        };

        // --- LastIndexOf (2-arg) ---
        private static object[] LastIndexOf_2ArgCases =
        {
            // Basic (Apache Harmony parity)
            new object[] { "0123456789", "0", 0 },
            new object[] { "0123456789", "012", 0 },
            new object[] { "0123456789", "02", -1 },
            new object[] { "0123456789", "89", 8 },

            // Empty value
            new object[] { "0123456789", "", 10 },
            new object[] { "", "", 0 },
            new object[] { "", "a", -1 },

            // Overlapping
            new object[] { "ababa", "aba", 2 },
        };

        // --- LastIndexOf (3-arg) ---
        private static object[] LastIndexOf_3ArgCases =
        {
            // Basic (Apache Harmony parity)
            new object[] { "0123456789", "0", 10, 0 },
            new object[] { "0123456789", "012", 10, 0 },
            new object[] { "0123456789", "02", 10, -1 },
            new object[] { "0123456789", "89", 10, 8 },

            // startIndex behavior
            new object[] { "0123456789", "0", 5, 0 },
            new object[] { "0123456789", "89", 5, -1 },

            // Empty value
            new object[] { "0123456789", "", 5, 5 },

            // Clamping
            new object[] { "0123456789", "0", -1, -1 },
            new object[] { "0123456789", "0", 100, 0 },
            new object[] { "0123456789", "", -1, -1 },
            new object[] { "0123456789", "", 100, 10 },

            // Empty span
            new object[] { "", "", 0, 0 },
            new object[] { "", "", -5, -1 },
            new object[] { "", "", 100, 0 },
            new object[] { "", "a", 0, -1 },

            // Overlapping
            new object[] { "ababa", "aba", 10, 2 },
            new object[] { "ababa", "aba", 1, 0 },
        };

        // --- StringComparison ---
        private static object[] ComparisonCases =
        {
            new object[] { "AbCd", "ab", StringComparison.OrdinalIgnoreCase, 0 },
            new object[] { "AbCd", "CD", StringComparison.OrdinalIgnoreCase, 2 },
            new object[] { "AbCd", "ab", StringComparison.Ordinal, -1 },

            // Culture-sensitive
            // Turkish dotted/dotless I
            new object[] { "I", "ı", StringComparison.CurrentCultureIgnoreCase, 0 }, // in tr-TR
            new object[] { "straße", "STRASSE", StringComparison.OrdinalIgnoreCase, -1 },
        };

        // ============================
        // INDEXOF TESTS
        // ============================

        [TestCaseSource(nameof(IndexOf_2ArgCases))]
        public void Test_IndexOf_String(string fixture, string value, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value));

        [TestCaseSource(nameof(IndexOf_2ArgCases))]
        public void Test_IndexOf_ReadOnlySpan(string fixture, string value, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value.AsSpan()));

        [TestCaseSource(nameof(IndexOf_3ArgCases))]
        public void Test_IndexOf_String_Int32(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value, startIndex));

        [TestCaseSource(nameof(IndexOf_3ArgCases))]
        public void Test_IndexOf_ReadOnlySpan_Int32(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value.AsSpan(), startIndex));

        [TestCaseSource(nameof(ComparisonCases))]
        public void Test_IndexOf_String_StringComparison(string fixture, string value, StringComparison cmp, int expected)
        {
            using var context = new CultureContext("tr-TR");
            Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value, cmp));
        }

        [TestCaseSource(nameof(ComparisonCases))]
        public void Test_IndexOf_ReadOnlySpan_StringComparison(string fixture, string value, StringComparison cmp, int expected)
        {
            using var context = new CultureContext("tr-TR");
            Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value.AsSpan(), cmp)); 
        }

        // ============================
        // LASTINDEXOF TESTS
        // ============================

        [TestCaseSource(nameof(LastIndexOf_2ArgCases))]
        public void Test_LastIndexOf_String(string fixture, string value, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value));

        [TestCaseSource(nameof(LastIndexOf_2ArgCases))]
        public void Test_LastIndexOf_ReadOnlySpan(string fixture, string value, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value.AsSpan()));

        [TestCaseSource(nameof(LastIndexOf_3ArgCases))]
        public void Test_LastIndexOf_String_Int32(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value, startIndex));

        [TestCaseSource(nameof(LastIndexOf_3ArgCases))]
        public void Test_LastIndexOf_ReadOnlySpan_Int32(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value.AsSpan(), startIndex));

        [TestCaseSource(nameof(ComparisonCases))]
        public void Test_LastIndexOf_String_StringComparison(string fixture, string value, StringComparison cmp, int expected)
        {
            using var context = new CultureContext("tr-TR");
            Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value, cmp));
        }

        [TestCaseSource(nameof(ComparisonCases))]
        public void Test_LastIndexOf_ReadOnlySpan_StringComparison(string fixture, string value, StringComparison cmp, int expected)
        {
            using var context = new CultureContext("tr-TR");
            Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value.AsSpan(), cmp));
        }
        // ============================
        // ERROR TESTS (SEPARATE)
        // ============================

        [Test]
        public void Test_IndexOf_String_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.IndexOf("abc".AsSpan(), (string)null));

        [Test]
        public void Test_IndexOf_String_Int32_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.IndexOf("abc".AsSpan(), (string)null, 0));

        [Test]
        public void Test_LastIndexOf_String_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.LastIndexOf("abc".AsSpan(), (string)null));

        [Test]
        public void Test_LastIndexOf_String_Int32_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.LastIndexOf("abc".AsSpan(), (string)null, 0));

        [Test]
        public void Test_IndexOf_InvalidComparison()
            => Assert.Throws<ArgumentOutOfRangeException>(() =>
                SpanUtilities.IndexOf("abc".AsSpan(), "a", (StringComparison)999));
    }
}
