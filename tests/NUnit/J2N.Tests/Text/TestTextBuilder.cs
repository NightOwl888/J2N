#region Copyright 2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using NUnit.Framework;
using System;
using System.Text;
#nullable enable

namespace J2N.Text
{
    public class TestTextBuilder : StringBuilderTestBase
    {
        protected override TextBuilder StringBuilderFactory()
            => new TextBuilder() { UseInvariantDefaults = true };

        protected override TextBuilder StringBuilderFactory(int capacity)
            => new TextBuilder(capacity) { UseInvariantDefaults = true };

        protected override TextBuilder StringBuilderFactory(string? value)
            => new TextBuilder(value) { UseInvariantDefaults = true };

        protected override TextBuilder StringBuilderFactory(ReadOnlySpan<char> value)
            => new TextBuilder(value) { UseInvariantDefaults = true };

        protected override TextBuilder StringBuilderFactory(StringBuilder? value)
            => new TextBuilder(value) { UseInvariantDefaults = true };

        protected override TextBuilder StringBuilderFactory(ICharSequence? value)
            => new TextBuilder(value) { UseInvariantDefaults = true };

        // Regression for overflow check bug in StringExtensions
        [Test]
        public void TextBuilder_Delete_WhenCountOverflows_ShouldClampToEnd()
        {
            // Arrange
            var text = StringBuilderFactory("abcdef");

            // Act + Assert
            Assert.DoesNotThrow(() =>
            {
                text.Delete(1, int.MaxValue);
            });

            Assert.That(text.ToString(), Is.EqualTo("a"));
        }
    }
}
