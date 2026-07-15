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

using J2N.CodeGeneration;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        /// <summary>
        /// Appends the upper-case representation of a string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendUpper{TBuilder}(TBuilder, string?, CultureInfo?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendUpperInternal(string? value, CultureInfo? culture)
            => AppendUpperInternal(value.AsSpan(), culture);

        /// <summary>
        /// Appends the upper-case representation of a string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendUpper{TBuilder}(TBuilder, ReadOnlySpan{char}, CultureInfo?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendUpperInternal(ReadOnlySpan<char> value, CultureInfo? culture)
        {
            culture ??= CultureInfo.CurrentCulture;

            int valueLength = value.Length;
            if (valueLength == 0)
                return;

            int pos = m_Position;
            if (pos > m_Chars.Length - valueLength)
            {
                Grow(valueLength);
            }

            int length = value.ToUpper(m_Chars.AsSpan(m_Position), culture);
            while (length < 0) // rare
            {
                Grow(valueLength);
                length = value.ToUpper(m_Chars.AsSpan(m_Position), culture);
            }
            m_Position += length;
        }

        /// <summary>
        /// Appends the lower-case representation of a string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendLower{TBuilder}(TBuilder, string?, CultureInfo?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendLowerInternal(string? value, CultureInfo? culture)
            => AppendLowerInternal(value.AsSpan(), culture);

        /// <summary>
        /// Appends the lower-case representation of a string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendLower{TBuilder}(TBuilder, ReadOnlySpan{char}, CultureInfo?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendLowerInternal(ReadOnlySpan<char> value, CultureInfo? culture)
        {
            culture ??= CultureInfo.CurrentCulture;

            int valueLength = value.Length;
            if (valueLength == 0)
                return;

            int pos = m_Position;
            if (pos > m_Chars.Length - valueLength)
            {
                Grow(valueLength);
            }

            int length = value.ToLower(m_Chars.AsSpan(m_Position), culture);
            while (length < 0) // rare
            {
                Grow(valueLength);
                length = value.ToLower(m_Chars.AsSpan(m_Position), culture);
            }
            m_Position += length;
        }

        /// <summary>
        /// Appends the upper-case representation of a string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendUpperInvariant{TBuilder}(TBuilder, string?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendUpperInvariantInternal(string? value)
            => AppendUpperInvariantInternal(value.AsSpan());

        /// <summary>
        /// Appends the upper-case representation of a string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendUpperInvariant{TBuilder}(TBuilder, ReadOnlySpan{char})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendUpperInvariantInternal(ReadOnlySpan<char> value)
        {
            int valueLength = value.Length;
            if (valueLength == 0)
                return;

            int pos = m_Position;
            if (pos > m_Chars.Length - valueLength)
            {
                Grow(valueLength);
            }

            int length = value.ToUpperInvariant(m_Chars.AsSpan(m_Position));
            Debug.Assert(length >= 0, "The invariant culture should never require expanding the buffer to more characters than the original value");
            m_Position += length;
        }

        /// <summary>
        /// Appends the lower-case representation of a string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendLowerInvariant{TBuilder}(TBuilder, string?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendLowerInvariantInternal(string? value)
            => AppendLowerInvariantInternal(value.AsSpan());

        /// <summary>
        /// Appends the lower-case representation of a string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendLowerInvariant{TBuilder}(TBuilder, ReadOnlySpan{char})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendLowerInvariantInternal(ReadOnlySpan<char> value)
        {
            int valueLength = value.Length;
            if (valueLength == 0)
                return;

            int pos = m_Position;
            if (pos > m_Chars.Length - valueLength)
            {
                Grow(valueLength);
            }

            int length = value.ToLowerInvariant(m_Chars.AsSpan(m_Position));
            Debug.Assert(length >= 0, "The invariant culture should never require expanding the buffer to more characters than the original value");
            m_Position += length;
        }
    }
}
