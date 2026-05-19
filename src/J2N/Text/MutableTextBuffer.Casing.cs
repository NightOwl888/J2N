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

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        /// <summary>
        /// Appends the upper case string representation of a specified string
        /// to this instance using the casing rules from the specified culture.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <remarks>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="culture"/> is <c>null</c>, <see cref="CultureInfo.CurrentCulture"/> will be used.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public MutableTextBuffer AppendUpper(string? value, CultureInfo? culture)
            => AppendUpper(value.AsSpan(), culture);

        /// <summary>
        /// Appends the upper case string representation of a specified read-only character
        /// span to this instance using the casing rules from the specified culture.
        /// </summary>
        /// <param name="value">The read-only character span to append.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <remarks>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="culture"/> is <c>null</c>, <see cref="CultureInfo.CurrentCulture"/> will be used.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public MutableTextBuffer AppendUpper(ReadOnlySpan<char> value, CultureInfo? culture)
        {
            culture ??= CultureInfo.CurrentCulture;

            int valueLength = value.Length;
            if (valueLength == 0)
                return this;

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
            return this;
        }

        /// <summary>
        /// Appends the lower case string representation of a specified string
        /// to this instance using the casing rules from the specified culture.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <remarks>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="culture"/> is <c>null</c>, <see cref="CultureInfo.CurrentCulture"/> will be used.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public MutableTextBuffer AppendLower(string? value, CultureInfo? culture)
            => AppendLower(value.AsSpan(), culture);

        /// <summary>
        /// Appends the lower case string representation of a specified read-only character
        /// span to this instance using the casing rules from the specified culture.
        /// </summary>
        /// <param name="value">The read-only character span to append.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationReturnsSelf]
        public MutableTextBuffer AppendLower(ReadOnlySpan<char> value, CultureInfo? culture)
        {
            culture ??= CultureInfo.CurrentCulture;

            int valueLength = value.Length;
            if (valueLength == 0)
                return this;

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
            return this;
        }

        /// <summary>
        /// Appends the upper case string representation of a specified string
        /// to this instance using the casing rules from the invariant culture.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationReturnsSelf]
        public MutableTextBuffer AppendUpperInvariant(string? value)
            => AppendUpperInvariant(value.AsSpan());

        /// <summary>
        /// Appends the upper case string representation of a specified read-only character
        /// span to this instance using the casing rules from the invariant culture.
        /// </summary>
        /// <param name="value">The read-only character span to append.</param>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationReturnsSelf]
        public MutableTextBuffer AppendUpperInvariant(ReadOnlySpan<char> value)
        {
            int valueLength = value.Length;
            if (valueLength == 0)
                return this;

            int pos = m_Position;
            if (pos > m_Chars.Length - valueLength)
            {
                Grow(valueLength);
            }

            int length = value.ToUpperInvariant(m_Chars.AsSpan(m_Position));
            Debug.Assert(length >= 0, "The invariant culture should never require expanding the buffer to more characters than the original value");
            m_Position += length;
            return this;
        }

        /// <summary>
        /// Appends the lower case string representation of a specified string
        /// to this instance using the casing rules from the invariant culture.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationReturnsSelf]
        public MutableTextBuffer AppendLowerInvariant(string? value)
            => AppendLowerInvariant(value.AsSpan());

        /// <summary>
        /// Appends the lower case string representation of a specified read-only character
        /// span to this instance using the casing rules from the invariant culture.
        /// </summary>
        /// <param name="value">The read-only character span to append.</param>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationReturnsSelf]
        public MutableTextBuffer AppendLowerInvariant(ReadOnlySpan<char> value)
        {
            int valueLength = value.Length;
            if (valueLength == 0)
                return this;

            int pos = m_Position;
            if (pos > m_Chars.Length - valueLength)
            {
                Grow(valueLength);
            }

            int length = value.ToLowerInvariant(m_Chars.AsSpan(m_Position));
            Debug.Assert(length >= 0, "The invariant culture should never require expanding the buffer to more characters than the original value");
            m_Position += length;
            return this;
        }
    }
}
