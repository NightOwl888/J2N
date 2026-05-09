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

            // Exact full-length
            new object[] { "abcde", "abcde", 0 },

            // Longer than source
            new object[] { "abc", "abcd", -1 },

            // Single-char
            new object[] { "abcde", "e", 4 },

            // Repeated overlap
            new object[] { "aaaaaa", "aaa", 0 },

            // Multiple occurrences
            new object[] { "abcabcabc", "abc", 0 },

            // Tail occurrence
            new object[] { "abcxxx", "xxx", 3 },

            // No partial-tail match
            new object[] { "abcxx", "xxx", -1 },

            // Unicode ordinal
            new object[] { "Straße", "ß", 4 },

            // Case-sensitive ordinal
            new object[] { "ABCDE", "abc", -1 },
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
            new object[] { "", "abc", 100, -1 },

            // Overlapping
            new object[] { "ababa", "aba", 0, 0 },
            new object[] { "ababa", "aba", 1, 2 },

            // Exact boundary match
            new object[] { "abcde", "de", 3, 3 },

            // Boundary miss
            new object[] { "abcde", "de", 4, -1 },

            // startIndex == Length
            new object[] { "abcde", "e", 5, -1 },
            new object[] { "abcde", "", 5, 5 },

            // value longer than searchable tail
            new object[] { "abcde", "cde", 3, -1 },

            // Full-length
            new object[] { "abcde", "abcde", 0, 0 },
            new object[] { "abcde", "abcde", 1, -1 },

            // Repeated overlap
            new object[] { "aaaaaa", "aaa", 1, 1 },
            new object[] { "aaaaaa", "aaa", 2, 2 },
            new object[] { "aaaaaa", "aaa", 3, 3 },
            new object[] { "aaaaaa", "aaa", 4, -1 },

            // Multiple occurrences
            new object[] { "abcabcabc", "abc", 1, 3 },
            new object[] { "abcabcabc", "abc", 4, 6 },

            // Tail search
            new object[] { "abcxxx", "xxx", 3, 3 },
            new object[] { "abcxxx", "xxx", 4, -1 },

            // Single-char
            new object[] { "abcde", "e", 4, 4 },

            // value longer than source
            new object[] { "abc", "abcd", 0, -1 },
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

            // Exact full-length
            new object[] { "abcde", "abcde", 0 },

            // Longer than source
            new object[] { "abc", "abcd", -1 },

            // Single-char
            new object[] { "abcde", "e", 4 },

            // Repeated overlap
            new object[] { "aaaaaa", "aaa", 3 },

            // Multiple occurrences
            new object[] { "abcabcabc", "abc", 6 },

            // Tail occurrence
            new object[] { "abcxxx", "xxx", 3 },

            // No partial-tail match
            new object[] { "abcxx", "xxx", -1 },

            // Unicode ordinal
            new object[] { "Straße", "ß", 4 },

            // Case-sensitive ordinal
            new object[] { "ABCDE", "abc", -1 },
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
            new object[] { "", "abc", 100, -1 },

            // Overlapping
            new object[] { "ababa", "aba", 10, 2 },
            new object[] { "ababa", "aba", 1, 0 },

            // Exact boundary match
            new object[] { "abcde", "ab", 0, 0 },

            // Boundary inclusion
            new object[] { "abcde", "de", 4, 3 },

            // Boundary exclusion
            new object[] { "abcde", "de", 2, -1 },

            // value longer than searchable window
            new object[] { "abcde", "cde", 1, -1 },

            // Full-length
            new object[] { "abcde", "abcde", 5, 0 },
            new object[] { "abcde", "abcde", 0, 0 },

            // Repeated overlap
            new object[] { "aaaaaa", "aaa", 5, 3 },
            new object[] { "aaaaaa", "aaa", 4, 3 },
            new object[] { "aaaaaa", "aaa", 3, 3 },
            new object[] { "aaaaaa", "aaa", 2, 2 },
            new object[] { "aaaaaa", "aaa", 1, 1 },
            new object[] { "aaaaaa", "aaa", 0, 0 },

            // Multiple occurrences
            new object[] { "abcabcabc", "abc", 8, 6 },
            new object[] { "abcabcabc", "abc", 5, 3 },
            new object[] { "abcabcabc", "abc", 2, 0 },

            // Tail window semantics
            new object[] { "abcxxx", "xxx", 2, -1 },
            new object[] { "abcxxx", "xxx", 3, 3 },
            new object[] { "abcxxx", "xxx", 4, 3 },
            new object[] { "abcxxx", "xxx", 5, 3 },

            // Single-char
            new object[] { "abcde", "a", 0, 0 },

            // value longer than source
            new object[] { "abc", "abcd", 100, -1 },
        };

        // --- StringComparison ---

        // --- IndexOf (3-arg) + StringComparison ---
        private static object[] IndexOf_3ArgComparisonCases =
        {
            // fixture, value, comparison, culture, expected

            // OrdinalIgnoreCase
            new object[] { "AbCd", "ab", StringComparison.OrdinalIgnoreCase, null, 0 },
            new object[] { "AbCd", "CD", StringComparison.OrdinalIgnoreCase, null, 2 },
            new object[] { "AaAaA", "aaa", StringComparison.OrdinalIgnoreCase, null, 0 },
            new object[] { "abc", "ABC", StringComparison.OrdinalIgnoreCase, null, 0 },

            // Ordinal
            new object[] { "AbCd", "ab", StringComparison.Ordinal, null, -1 },
            new object[] { "abcDEF", "DEF", StringComparison.Ordinal, null, 3 },
            new object[] { "abcDEF", "def", StringComparison.Ordinal, null, -1 },

            // Unicode ordinal behavior
            new object[] { "Straße", "ß", StringComparison.Ordinal, null, 4 },
            new object[] { "straße", "STRASSE", StringComparison.OrdinalIgnoreCase, null, -1 },

            // CurrentCultureIgnoreCase smoke test
            new object[] { "I", "ı", StringComparison.CurrentCultureIgnoreCase, "tr-TR", 0 },

            // Empty
            new object[] { "", "", StringComparison.Ordinal, null, 0 },

            // Full-length
            new object[] { "ABC", "abc", StringComparison.OrdinalIgnoreCase, null, 0 },
        };

        // --- IndexOf (4-arg) + StringComparison ---
        private static object[] IndexOf_4ArgComparisonCases =
        {
            // fixture, value, startIndex, comparison, culture, expected

            // OrdinalIgnoreCase
            new object[] { "AbCd", "cd", 0, StringComparison.OrdinalIgnoreCase, null, 2 },
            new object[] { "AaAaA", "aaa", 1, StringComparison.OrdinalIgnoreCase, null, 1 },
            new object[] { "AaAaA", "aaa", 3, StringComparison.OrdinalIgnoreCase, null, -1 },

            // Boundary behavior
            new object[] { "abcDEF", "DEF", 3, StringComparison.Ordinal, null, 3 },
            new object[] { "abcDEF", "DEF", 4, StringComparison.Ordinal, null, -1 },

            // Empty
            new object[] { "abc", "", 2, StringComparison.Ordinal, null, 2 },

            // CurrentCultureIgnoreCase smoke test
            new object[] { "I", "ı", 0, StringComparison.CurrentCultureIgnoreCase, "tr-TR", 0 },

            // Full-length
            new object[] { "ABC", "abc", 0, StringComparison.OrdinalIgnoreCase, null, 0 },
            new object[] { "ABC", "abc", 1, StringComparison.OrdinalIgnoreCase, null, -1 },
        };

        // --- LastIndexOf (3-arg) + StringComparison ---
        private static object[] LastIndexOf_3ArgComparisonCases =
        {
            // fixture, value, comparison, culture, expected

            // OrdinalIgnoreCase
            new object[] { "AbCd", "ab", StringComparison.OrdinalIgnoreCase, null, 0 },
            new object[] { "AbCd", "CD", StringComparison.OrdinalIgnoreCase, null, 2 },
            new object[] { "AaAaA", "aaa", StringComparison.OrdinalIgnoreCase, null, 2 },
            new object[] { "abc", "ABC", StringComparison.OrdinalIgnoreCase, null, 0 },

            // Ordinal
            new object[] { "AbCd", "ab", StringComparison.Ordinal, null, -1 },
            new object[] { "abcDEFabc", "DEF", StringComparison.Ordinal, null, 3 },
            new object[] { "abcDEF", "def", StringComparison.Ordinal, null, -1 },

            // Unicode ordinal behavior
            new object[] { "Straße", "ß", StringComparison.Ordinal, null, 4 },
            new object[] { "straßeSTRASSE", "STRASSE", StringComparison.OrdinalIgnoreCase, null, 6 },

            // CurrentCultureIgnoreCase smoke test
            new object[] { "I", "ı", StringComparison.CurrentCultureIgnoreCase, "tr-TR", 0 },

            // Empty
            new object[] { "", "", StringComparison.Ordinal, null, 0 },

            // Full-length
            new object[] { "ABC", "abc", StringComparison.OrdinalIgnoreCase, null, 0 },
        };

        // --- LastIndexOf (4-arg) + StringComparison ---
        private static object[] LastIndexOf_4ArgComparisonCases =
        {
            // fixture, value, startIndex, comparison, culture, expected

            // OrdinalIgnoreCase
            new object[] { "AaAaA", "aaa", 4, StringComparison.OrdinalIgnoreCase, null, 2 },
            new object[] { "AaAaA", "aaa", 2, StringComparison.OrdinalIgnoreCase, null, 2 },
            new object[] { "AaAaA", "aaa", 1, StringComparison.OrdinalIgnoreCase, null, 1 },

            // Boundary behavior
            new object[] { "abcDEF", "DEF", 5, StringComparison.Ordinal, null, 3 },
            new object[] { "abcDEF", "DEF", 2, StringComparison.Ordinal, null, -1 },

            // Empty
            new object[] { "abc", "", 2, StringComparison.Ordinal, null, 2 },

            // CurrentCultureIgnoreCase smoke test
            new object[] { "I", "ı", 0, StringComparison.CurrentCultureIgnoreCase, "tr-TR", 0 },

            // Full-length
            new object[] { "ABC", "abc", 3, StringComparison.OrdinalIgnoreCase, null, 0 },
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

        [TestCaseSource(nameof(IndexOf_2ArgCases))]
        public void Test_IndexOf_String_StringComparison_Ordinal(string fixture, string value, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value, StringComparison.Ordinal));

        [TestCaseSource(nameof(IndexOf_2ArgCases))]
        public void Test_IndexOf_ReadOnlySpan_StringComparison_Ordinal(string fixture, string value, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value.AsSpan(), StringComparison.Ordinal));

        [TestCaseSource(nameof(IndexOf_3ArgCases))]
        public void Test_IndexOf_String_Int32(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value, startIndex));

        [TestCaseSource(nameof(IndexOf_3ArgCases))]
        public void Test_IndexOf_ReadOnlySpan_Int32(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value.AsSpan(), startIndex));

        [TestCaseSource(nameof(IndexOf_3ArgCases))]
        public void Test_IndexOf_String_Int32_StringComparison_Ordinal(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value, startIndex, StringComparison.Ordinal));

        [TestCaseSource(nameof(IndexOf_3ArgCases))]
        public void Test_IndexOf_ReadOnlySpan_Int32_StringComparison_Ordinal(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value.AsSpan(), startIndex, StringComparison.Ordinal));

        [TestCaseSource(nameof(IndexOf_3ArgComparisonCases))]
        public void Test_IndexOf_String_StringComparison(string fixture, string value, StringComparison cmp, string culture, int expected)
        {
            using IDisposable context = culture is not null ? new CultureContext(culture) : new DummyDisposable();
            Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value, cmp));
        }

        [TestCaseSource(nameof(IndexOf_3ArgComparisonCases))]
        public void Test_IndexOf_ReadOnlySpan_StringComparison(string fixture, string value, StringComparison cmp, string culture, int expected)
        {
            using IDisposable context = culture is not null ? new CultureContext(culture) : new DummyDisposable();
            Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value.AsSpan(), cmp));
        }

        [TestCaseSource(nameof(IndexOf_4ArgComparisonCases))]
        public void Test_IndexOf_String_Int32_StringComparison(string fixture, string value, int startIndex, StringComparison cmp, string culture, int expected)
        {
            using IDisposable context = culture is not null ? new CultureContext(culture) : new DummyDisposable();
            Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value, startIndex, cmp));
        }

        [TestCaseSource(nameof(IndexOf_4ArgComparisonCases))]
        public void Test_IndexOf_ReadOnlySpan_StringComparison(string fixture, string value, int startIndex, StringComparison cmp, string culture, int expected)
        {
            using IDisposable context = culture is not null ? new CultureContext(culture) : new DummyDisposable();
            Assert.AreEqual(expected, SpanUtilities.IndexOf(fixture.AsSpan(), value.AsSpan(), startIndex, cmp));
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

        [TestCaseSource(nameof(LastIndexOf_2ArgCases))]
        public void Test_LastIndexOf_String_StringComparison_Ordinal(string fixture, string value, int expected)
           => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value, StringComparison.Ordinal));

        [TestCaseSource(nameof(LastIndexOf_2ArgCases))]
        public void Test_LastIndexOf_ReadOnlySpan_StringComparison_Ordinal(string fixture, string value, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value.AsSpan(), StringComparison.Ordinal));

        [TestCaseSource(nameof(LastIndexOf_3ArgCases))]
        public void Test_LastIndexOf_String_Int32(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value, startIndex));

        [TestCaseSource(nameof(LastIndexOf_3ArgCases))]
        public void Test_LastIndexOf_ReadOnlySpan_Int32(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value.AsSpan(), startIndex));

        [TestCaseSource(nameof(LastIndexOf_3ArgCases))]
        public void Test_LastIndexOf_String_Int32_StringComparison_Ordinal(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value, startIndex, StringComparison.Ordinal));

        [TestCaseSource(nameof(LastIndexOf_3ArgCases))]
        public void Test_LastIndexOf_ReadOnlySpan_Int32_StringComparison_Ordinal(string fixture, string value, int startIndex, int expected)
            => Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value.AsSpan(), startIndex, StringComparison.Ordinal));

        [TestCaseSource(nameof(LastIndexOf_3ArgComparisonCases))]
        public void Test_LastIndexOf_String_StringComparison(string fixture, string value, StringComparison cmp, string culture, int expected)
        {
            using IDisposable context = culture is not null ? new CultureContext(culture) : new DummyDisposable();
            Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value, cmp));
        }

        [TestCaseSource(nameof(LastIndexOf_3ArgComparisonCases))]
        public void Test_LastIndexOf_ReadOnlySpan_StringComparison(string fixture, string value, StringComparison cmp, string culture, int expected)
        {
            using IDisposable context = culture is not null ? new CultureContext(culture) : new DummyDisposable();
            Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value.AsSpan(), cmp));
        }

        [TestCaseSource(nameof(LastIndexOf_4ArgComparisonCases))]
        public void Test_LastIndexOf_String_Int32_StringComparison(string fixture, string value, int startIndex, StringComparison cmp, string culture, int expected)
        {
            using IDisposable context = culture is not null ? new CultureContext(culture) : new DummyDisposable();
            Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value, startIndex, cmp));
        }

        [TestCaseSource(nameof(LastIndexOf_4ArgComparisonCases))]
        public void Test_LastIndexOf_ReadOnlySpan_Int32_StringComparison(string fixture, string value, int startIndex, StringComparison cmp, string culture, int expected)
        {
            using IDisposable context = culture is not null ? new CultureContext(culture) : new DummyDisposable();
            Assert.AreEqual(expected, SpanUtilities.LastIndexOf(fixture.AsSpan(), value.AsSpan(), startIndex, cmp));
        }

        // ============================
        // ERROR TESTS
        // ============================

        [Test]
        public void Test_IndexOf_String_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.IndexOf("abc".AsSpan(), (string)null));

        [Test]
        public void Test_IndexOf_String_Int32_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.IndexOf("abc".AsSpan(), (string)null, 0));

        [Test]
        public void Test_IndexOf_String_StringComparison_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.IndexOf("abc".AsSpan(), (string)null, StringComparison.Ordinal));

        [Test]
        public void Test_IndexOf_String_Int32_StringComparison_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.IndexOf("abc".AsSpan(), (string)null, 0, StringComparison.Ordinal));

        [Test]
        public void Test_LastIndexOf_String_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.LastIndexOf("abc".AsSpan(), (string)null));

        [Test]
        public void Test_LastIndexOf_String_Int32_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.LastIndexOf("abc".AsSpan(), (string)null, 0));

        [Test]
        public void Test_LastIndexOf_String_StringComparison_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.LastIndexOf("abc".AsSpan(), (string)null, StringComparison.Ordinal));

        [Test]
        public void Test_LastIndexOf_String_Int32_StringComparison_Null()
            => Assert.Throws<ArgumentNullException>(() => SpanUtilities.LastIndexOf("abc".AsSpan(), (string)null, 0, StringComparison.Ordinal));

        [Test]
        public void Test_IndexOf_String_StringComparison_InvalidComparison()
            => Assert.Throws<ArgumentException>(() =>
                SpanUtilities.IndexOf("abc".AsSpan(), "a", (StringComparison)999));

        [Test]
        public void Test_IndexOf_ReadOnlySpan_StringComparison_InvalidComparison()
            => Assert.Throws<ArgumentException>(() =>
                SpanUtilities.IndexOf("abc".AsSpan(), "a".AsSpan(), (StringComparison)999));

        [Test]
        public void Test_IndexOf_String_Int32_StringComparison_InvalidComparison()
            => Assert.Throws<ArgumentException>(() =>
                SpanUtilities.IndexOf("abc".AsSpan(), "a", 0, (StringComparison)999));

        [Test]
        public void Test_IndexOf_ReadOnlySpan_Int32_StringComparison_InvalidComparison()
            => Assert.Throws<ArgumentException>(() =>
                SpanUtilities.IndexOf("abc".AsSpan(), "a".AsSpan(), 0, (StringComparison)999));

        [Test]
        public void Test_LastIndexOf_String_StringComparison_InvalidComparison()
            => Assert.Throws<ArgumentException>(() =>
                SpanUtilities.LastIndexOf("abc".AsSpan(), "a", (StringComparison)999));

        [Test]
        public void Test_LastIndexOf_ReadOnlySpan_StringComparison_InvalidComparison()
            => Assert.Throws<ArgumentException>(() =>
                SpanUtilities.LastIndexOf("abc".AsSpan(), "a".AsSpan(), (StringComparison)999));

        [Test]
        public void Test_LastIndexOf_String_Int32_StringComparison_InvalidComparison()
            => Assert.Throws<ArgumentException>(() =>
                SpanUtilities.LastIndexOf("abc".AsSpan(), "a", 0, (StringComparison)999));

        [Test]
        public void Test_LastIndexOf_ReadOnlySpan_Int32_StringComparison_InvalidComparison()
            => Assert.Throws<ArgumentException>(() =>
                SpanUtilities.LastIndexOf("abc".AsSpan(), "a".AsSpan(), 0, (StringComparison)999));

        private sealed class DummyDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}
