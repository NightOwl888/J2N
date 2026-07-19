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

using J2N.TestUtilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace J2N.Text.Tests
{
    public abstract partial class StringBuilder_Tests
    {
        // ------------------------------
        // AppendLower()
        // ------------------------------

        public static IEnumerable<object[]> Append_LowerCase_TestData()
        {
            // ASCII
            yield return new object[] { "Foo", "HELLO", "Foohello", CultureInfo.InvariantCulture };
            yield return new object[] { "Foo", "WORLD", "Fooworld", new CultureInfo("en-US") };

            // Turkish (special dotless i behavior)
            yield return new object[] { "Hello", "I", "Helloı", new CultureInfo("tr-TR") };
            yield return new object[] { "Hello", "İ", "Helloi", new CultureInfo("tr-TR") }; // with combining dot

            // German (ß)
            yield return new object[] { "Hello", "SS", "Helloss", new CultureInfo("de-DE") };
            yield return new object[] { "Hello", "STRAẞE", "Hellostraße", new CultureInfo("de-DE") };
            yield return new object[] { "Hello", "FUßBALL", "Hellofußball", new CultureInfo("de-DE") };

            // Greek final sigma
            yield return new object[] { "Hello", "Σ", "Helloσ", new CultureInfo("el-GR") };
        }

        [Theory]
        [MemberData(nameof(Append_LowerCase_TestData))]
        public void AppendLower_CharSpan(string original, string input, string expected, CultureInfo culture)
        {
            var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory(original, 15);
            sb.AppendLower(input.AsSpan(), culture);
            Assert.Equal(expected, sb.ToString());
        }

        [Fact]
        public void AppendLower_CharSpan_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.AppendLower("I".AsSpan(), culture: null);
            Assert.Equal("ı", sb.ToString());
        }

        [Fact]
        public void AppendLower_CharSpan_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendLower("I".AsSpan(), culture: null);
            Assert.Equal("i", sb.ToString());
        }

        [Fact]
        public void AppendLower_CharSpan_ExplicitCultureOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendLower("I".AsSpan(), new CultureInfo("tr-TR"));
            Assert.Equal("ı", sb.ToString());
        }

        [Theory]
        [MemberData(nameof(Append_LowerCase_TestData))]
        public void AppendLower_String(string original, string input, string expected, CultureInfo culture)
        {
            var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory(original, 15);
            sb.AppendLower(input, culture);
            Assert.Equal(expected, sb.ToString());
        }

        [Fact]
        public void AppendLower_String_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.AppendLower("I", culture: null);
            Assert.Equal("ı", sb.ToString());
        }

        [Fact]
        public void AppendLower_String_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendLower("I", culture: null);
            Assert.Equal("i", sb.ToString());
        }

        [Fact]
        public void AppendLower_String_ExplicitCultureOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendLower("I", new CultureInfo("tr-TR"));
            Assert.Equal("ı", sb.ToString());
        }

        // ------------------------------
        // AppendUpper()
        // ------------------------------

        public static IEnumerable<object[]> Append_UpperCase_TestData()
        {
            // ASCII
            yield return new object[] { "Foo", "hello", "FooHELLO", CultureInfo.InvariantCulture };
            yield return new object[] { "Foo", "world", "FooWORLD", new CultureInfo("en-US") };

            // Turkish (special dotted i)
            yield return new object[] { "Hello", "i", "Helloİ", new CultureInfo("tr-TR") };
            yield return new object[] { "Hello", "ı", "HelloI", new CultureInfo("tr-TR") };

            // German sharp S expands (length change)
            yield return new object[] { "Hello", "longinputstringthatdoesnotfit", "HelloLONGINPUTSTRINGTHATDOESNOTFIT", new CultureInfo("de-DE") };

            // Greek
            yield return new object[] { "Hello", "ὀδυσσεύς", "HelloὈΔΥΣΣΕΎΣ", new CultureInfo("el-GR") };
        }

        [Theory]
        [MemberData(nameof(Append_UpperCase_TestData))]
        public void AppendUpper_CharSpan(string original, string input, string expected, CultureInfo culture)
        {
            var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory(original, 15);
            sb.AppendUpper(input.AsSpan(), culture);
            Assert.Equal(expected, sb.ToString());
        }

        [Fact]
        public void AppendUpper_CharSpan_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.AppendUpper("i".AsSpan(), culture: null);
            Assert.Equal("İ", sb.ToString());
        }

        [Fact]
        public void AppendUpper_CharSpan_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendUpper("i".AsSpan(), culture: null);
            Assert.Equal("I", sb.ToString());
        }

        [Fact]
        public void AppendUpper_CharSpan_ExplicitCultureOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendUpper("i".AsSpan(), new CultureInfo("tr-TR"));
            Assert.Equal("İ", sb.ToString());
        }

        [Theory]
        [MemberData(nameof(Append_UpperCase_TestData))]
        public void AppendUpper_String(string original, string input, string expected, CultureInfo culture)
        {
            var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory(original, 15);
            sb.AppendUpper(input, culture);
            Assert.Equal(expected, sb.ToString());
        }

        [Fact]
        public void AppendUpper_String_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.AppendUpper("i", culture: null);
            Assert.Equal("İ", sb.ToString());
        }

        [Fact]
        public void AppendUpper_String_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("tr-TR");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendUpper("i", culture: null);
            Assert.Equal("I", sb.ToString());
        }

        [Fact]
        public void AppendUpper_String_ExplicitCultureOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("", 16, new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendUpper("i", new CultureInfo("tr-TR"));
            Assert.Equal("İ", sb.ToString());
        }

        // ------------------------------
        // AppendLowerInvariant()
        // ------------------------------

        public static IEnumerable<object[]> AppendInvariant_LowerCase_TestData()
        {
            yield return new object[] { "Foo", "HELLO", "Foohello" };
            yield return new object[] { "Foo", "Straße", "Foostraße" }; // stays same length (ß not expanded in lower)
            yield return new object[] { "Foo", "İ", "Fooİ" }; // combining dot preserved
        }

        [Theory]
        [MemberData(nameof(AppendInvariant_LowerCase_TestData))]
        public void AppendLowerInvariant_CharSpan(string original, string input, string expected)
        {
            var sb = MutableTextBufferFactory(original, 15);
            sb.AppendLowerInvariant(input.AsSpan());
            Assert.Equal(expected, sb.ToString());
        }

        [Theory]
        [MemberData(nameof(AppendInvariant_LowerCase_TestData))]
        public void AppendLowerInvariant_String(string original, string input, string expected)
        {
            var sb = MutableTextBufferFactory(original, 15);
            sb.AppendLowerInvariant(input);
            Assert.Equal(expected, sb.ToString());
        }

        // ------------------------------
        // AppendUpperInvariant()
        // ------------------------------

        public static IEnumerable<object[]> AppendInvariant_UpperCase_TestData()
        {
            yield return new object[] { "Foo", "hello", "FooHELLO" };
            yield return new object[] { "Foo", "fußball", "FooFUßBALL" };
            yield return new object[] { "Foo", "i", "FooI" };
        }

        [Theory]
        [MemberData(nameof(AppendInvariant_UpperCase_TestData))]
        public void AppendUpperInvariant_CharSpan(string original, string input, string expected)
        {
            var sb = MutableTextBufferFactory(original, 15);
            sb.AppendUpperInvariant(input.AsSpan());
            Assert.Equal(expected, sb.ToString());
        }

        [Theory]
        [MemberData(nameof(AppendInvariant_UpperCase_TestData))]
        public void AppendUpperInvariant_String(string original, string input, string expected)
        {
            var sb = MutableTextBufferFactory(original, 15);
            sb.AppendUpperInvariant(input);
            Assert.Equal(expected, sb.ToString());
        }
    }
}
