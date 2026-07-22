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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace J2N.Text
{
#pragma warning disable CS3019 // CLS compliance checking will not be performed because it is not visible from outside this assembly
    internal static partial class MutableTextBufferExtensions
    {
        /// <summary>
        /// Appends the string representation of a specified 8-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="sbyte" />
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified 8-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="byte" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified 16-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="short" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified 32-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="int" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified 64-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="long" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified single-precision floating-point number to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture using the "J" format, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="float" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified double-precision floating-point number to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture using the "J" format, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="double" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified decimal to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="decimal" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder] // J2N TODO: We either need to complete this or make it internal because the JDK format is not implemented yet.
        internal static TBuilder Append<TBuilder>(this TBuilder text, decimal value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified 16-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="ushort" />
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified 32-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="uint" />
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified 64-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="ulong" />
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified 8-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="sbyte" />
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified 8-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="byte" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified 16-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="short" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified 32-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="int" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified 64-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="long" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified single-precision floating-point number to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture using the "J" format, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="float" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified double-precision floating-point number to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture using the "J" format, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="double" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified decimal to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="decimal" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder] // J2N TODO: We either need to complete this or make it internal because the JDK format is not implemented yet.
        internal static TBuilder Insert<TBuilder>(this TBuilder text, int index, decimal value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified 16-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="ushort" />
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified 32-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="uint" />
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified 64-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If no format provider is explicitly specified, this method uses the default formatting behavior
        /// determined by the <see cref="MutableTextBuffer.UseInvariantDefaults"/> setting.
        /// </remarks>
        /// <seealso cref="ulong" />
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format, provider);
            return text;
        }
    }
}
