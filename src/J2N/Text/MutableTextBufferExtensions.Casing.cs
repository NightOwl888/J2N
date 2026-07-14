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
using System.Globalization;

namespace J2N.Text
{
    public static partial class MutableTextBufferExtensions
    {
        /// <summary>
        /// Appends the upper case string representation of a specified string
        /// to this instance using the casing rules from the specified culture.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string to append.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="culture"/> is <see langword="null"/>, <see cref="CultureInfo.CurrentCulture"/> will be used.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendUpper<TBuilder>(this TBuilder text, string? value, CultureInfo? culture)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendUpperInternal(value, culture);
            return text;
        }

        /// <summary>
        /// Appends the upper case string representation of a specified read-only character
        /// span to this instance using the casing rules from the specified culture.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The read-only character span to append.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="culture"/> is <see langword="null"/>, <see cref="CultureInfo.CurrentCulture"/> will be used.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendUpper<TBuilder>(this TBuilder text, ReadOnlySpan<char> value, CultureInfo? culture)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendUpperInternal(value, culture);
            return text;
        }

        /// <summary>
        /// Appends the upper case string representation of a specified string
        /// to this instance using the casing rules from the invariant culture.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendUpperInvariant<TBuilder>(this TBuilder text, string? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendUpperInvariantInternal(value);
            return text;
        }

        /// <summary>
        /// Appends the upper case string representation of a specified read-only character
        /// span to this instance using the casing rules from the invariant culture.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The read-only character span to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendUpperInvariant<TBuilder>(this TBuilder text, ReadOnlySpan<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendUpperInvariantInternal(value);
            return text;
        }

        /// <summary>
        /// Appends the lower case string representation of a specified string
        /// to this instance using the casing rules from the specified culture.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string to append.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="culture"/> is <see langword="null"/>, <see cref="CultureInfo.CurrentCulture"/> will be used.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLower<TBuilder>(this TBuilder text, string? value, CultureInfo? culture)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLowerInternal(value, culture);
            return text;
        }

        /// <summary>
        /// Appends the lower case string representation of a specified read-only character
        /// span to this instance using the casing rules from the specified culture.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The read-only character span to append.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLower<TBuilder>(this TBuilder text, ReadOnlySpan<char> value, CultureInfo? culture)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLowerInternal(value, culture);
            return text;
        }

        /// <summary>
        /// Appends the lower case string representation of a specified string
        /// to this instance using the casing rules from the invariant culture.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLowerInvariant<TBuilder>(this TBuilder text, string? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLowerInvariantInternal(value);
            return text;
        }

        /// <summary>
        /// Appends the lower case string representation of a specified read-only character
        /// span to this instance using the casing rules from the invariant culture.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The read-only character span to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLowerInvariant<TBuilder>(this TBuilder text, ReadOnlySpan<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLowerInvariantInternal(value);
            return text;
        }
    }
}
