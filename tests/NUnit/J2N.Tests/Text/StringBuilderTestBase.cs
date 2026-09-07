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
        #region TextBuilder Helper Methods

        /// <summary>
        /// Creates an instance of an <see cref="TextBuilder"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="TextBuilder"/> that can be used for testing.</returns>
        protected abstract TextBuilder StringBuilderFactory();

        /// <summary>
        /// Creates an instance of an <see cref="TextBuilder"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="TextBuilder"/> that can be used for testing.</returns>
        protected abstract TextBuilder StringBuilderFactory(int capacity);

        /// <summary>
        /// Creates an instance of an <see cref="TextBuilder"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="TextBuilder"/> that can be used for testing.</returns>
        protected abstract TextBuilder StringBuilderFactory(string? value);

        /// <summary>
        /// Creates an instance of an <see cref="TextBuilder"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="TextBuilder"/> that can be used for testing.</returns>
        protected abstract TextBuilder StringBuilderFactory(ReadOnlySpan<char> value);

        /// <summary>
        /// Creates an instance of an <see cref="TextBuilder"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="TextBuilder"/> that can be used for testing.</returns>
        protected abstract TextBuilder StringBuilderFactory(StringBuilder? value);

        /// <summary>
        /// Creates an instance of an <see cref="TextBuilder"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="TextBuilder"/> that can be used for testing.</returns>
        protected abstract TextBuilder StringBuilderFactory(ICharSequence? value);

        #endregion TextBuilder Helper Methods
    }
}
