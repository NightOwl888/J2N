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

using System;
using System.Text;
#nullable enable

namespace J2N.Text
{
    public class TestOpenStringBuilder : OpenStringBuilderTestBase
    {
        protected override OpenStringBuilder OpenStringBuilderFactory()
            => new OpenStringBuilder() { UseInvariantDefaults = true };

        protected override OpenStringBuilder OpenStringBuilderFactory(int capacity)
            => new OpenStringBuilder(capacity) { UseInvariantDefaults = true };

        protected override OpenStringBuilder OpenStringBuilderFactory(string? value)
            => new OpenStringBuilder(value) { UseInvariantDefaults = true };

        protected override OpenStringBuilder OpenStringBuilderFactory(ReadOnlySpan<char> value)
            => new OpenStringBuilder(value) { UseInvariantDefaults = true };

        protected override OpenStringBuilder OpenStringBuilderFactory(StringBuilder? value)
            => new OpenStringBuilder(value) { UseInvariantDefaults = true };

        protected override OpenStringBuilder OpenStringBuilderFactory(ICharSequence? value)
            => new OpenStringBuilder(value) { UseInvariantDefaults = true };
    }
}
