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

using J2N.Numerics;
using J2N.Text;
using System;

namespace J2N
{
    /// <summary>
    /// Extensions to <see cref="PooledTextBuilder"/>.
    /// </summary>
    // NOTE: These methods are not in the J2N.Text namespace because we want to lower priority of object? and ICharSequence?
    // overloads in favor of the more specific overloads in J2N.Text.MutableTextBufferExtensions, such as ReadOnlySpan<char>.
    public static class PooledTextBuilderExtensions
    {
        /// <summary>
        /// Appends the string representation of a specified object to this instance using the specified format
        /// and culture-specific format information.
        /// </summary>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The object to append.</param>
        /// <param name="format">A standard or custom format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/></exception>
        /// <remarks>
        /// <paramref name="format"/> and <paramref name="provider"/> are only applied if the object implements <see cref="ISpanFormattable"/>,
        /// <see cref="IFormattable"/>, <c>IStructuralFormattable</c>, or subclasses <see cref="Number"/>.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="PooledTextBuilder"/> object by calling <see cref="PooledTextBuilder(int, int)"/>,
        /// both the length and the capacity of the <see cref="PooledTextBuilder"/> instance can grow beyond
        /// the value of its <see cref="PooledTextBuilder.MaxCapacity"/> property. This can occur particularly when you call the <see cref="J2N.Text.PooledTextBuilderExtensions.Append(PooledTextBuilder, string?)"/>
        /// and <see cref="J2N.Text.PooledTextBuilderExtensions.AppendFormat(PooledTextBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="object" />
        public static PooledTextBuilder Append(this PooledTextBuilder text, object? value, string? format = null, IFormatProvider? provider = null)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            text.buffer.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>Inserts the string representation of an object into this instance at the specified character position.</summary>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The object to insert, or <see langword="null"/>.</param>
        /// <param name="format">A standard or custom format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the current length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="PooledTextBuilder.MaxCapacity"/>.</exception>
        /// <remarks>
        /// <paramref name="format"/> and <paramref name="provider"/> are only applied if the object implements <see cref="ISpanFormattable"/>,
        /// <see cref="IFormattable"/>, <c>IStructuralFormattable</c>, or subclasses <see cref="Number"/>.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="value"/> is <see langword="null"/>, the <see cref="PooledTextBuilder"/> is not changed.
        /// </remarks>
        /// <seealso cref="object" />
        public static PooledTextBuilder Insert(this PooledTextBuilder text, int index, object? value, string? format = null, IFormatProvider? provider = null)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            text.buffer.InsertInternal(index, value, format, provider);
            return text;
        }
    }
}
