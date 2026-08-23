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

using System;

namespace J2N.Numerics.Formatters
{
    /// <summary>
    /// Defines a number formtter. This is similar to ISpanFormattable, except that it
    /// only has a single member. The purpose is to simplify the usage of number formatting
    /// by providing a common interface for formatting.
    /// </summary>
    /// <typeparam name="T">The type to format to a sequence of characters.</typeparam>
    internal interface INumberFormatter<T>
    {
        bool TryFormat(
            T value,
            ReadOnlySpan<char> format,
            IFormatProvider provider,
            Span<char> destination,
            out int charsWritten);
    }
}
