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
    public abstract partial class StringBuilderTestBase : TestCase
    {
        #region MutableTextBuffer Helper Methods

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        protected abstract MutableTextBuffer OpenStringBuilderFactory();

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        protected abstract MutableTextBuffer OpenStringBuilderFactory(int capacity);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        protected abstract MutableTextBuffer OpenStringBuilderFactory(string? value);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        protected abstract MutableTextBuffer OpenStringBuilderFactory(ReadOnlySpan<char> value);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        protected abstract MutableTextBuffer OpenStringBuilderFactory(StringBuilder? value);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        protected abstract MutableTextBuffer OpenStringBuilderFactory(ICharSequence? value);

        #endregion MutableTextBuffer Helper Methods
    }
}
