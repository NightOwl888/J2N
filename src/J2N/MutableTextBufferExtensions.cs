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
using J2N.Numerics;
using J2N.Text;
using System;
using System.Runtime.CompilerServices;

namespace J2N
{
    /// <summary>
    /// Extensions to <see cref="MutableTextBuffer"/>.
    /// </summary>
    // NOTE: These methods are not in the J2N.Text namespace because we want to lower priority of object? and ICharSequence?
    // overloads in favor of the more specific overloads in J2N.Text.MutableTextBufferExtensions, such as ReadOnlySpan<char>.
    internal static class MutableTextBufferExtensions
    {
        #region Append object

        /// <summary>
        /// Appends the string representation of a specified object to this instance using the specified format
        /// and culture-specific format information.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
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
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="J2N.Text.MutableTextBufferExtensions.Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="J2N.Text.MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="object"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        // J2N TODO: Mark public once we have fixed the ability to convert arbitrary objects to localized strings defaulting to Java formats.
        internal static TBuilder Append<TBuilder>(this TBuilder text, object? value, string? format = null, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            text.AppendInternal(value, format, provider);
            return text;
        }

        #endregion Append object

        #region Append ICharSequence

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified sequence to this instance.
        /// <para/>
        /// NOTE: Unlike the Java implementation, this method does not add the word <c>"null"</c> to the <see cref="MutableTextBuffer"/>
        /// if <paramref name="value"/> is <see langword="null"/>. Instead, no operation is performed.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <seealso cref="ICharSequence" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ICharSequence? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>Appends the string representation of a specified subarray of Unicode characters to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The sequence of characters to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="count">The number of characters to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>, and
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <seealso cref="ICharSequence" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ICharSequence? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, count);
            return text;
        }

        #endregion Append ICharSequence 


        #region Insert object

        /// <summary>
        /// Inserts the string representation of an object into this instance at the specified character position.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The object to insert, or <see langword="null"/>.</param>
        /// <param name="format">A standard or custom format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// </exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// <paramref name="format"/> and <paramref name="provider"/> are only applied if the object implements <see cref="ISpanFormattable"/>,
        /// <see cref="IFormattable"/>, <c>IStructuralFormattable</c>, or subclasses <see cref="Number"/>.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="value"/> is <see langword="null"/>, the <see cref="MutableTextBuffer"/> is not changed.
        /// </remarks>
        /// <seealso cref="object"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        // J2N TODO: Mark public once we have fixed the ability to convert arbitrary objects to localized strings defaulting to Java formats.
        internal static TBuilder Insert<TBuilder>(this TBuilder text, int index, object? value, string? format = null, IFormatProvider? provider = null)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            text.InsertInternal(index, value, format, provider);
            return text;
        }

        #endregion Insert object

        #region Insert ICharSequence

        /// <summary>Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character sequence to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ICharSequence? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character sequence to insert.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> is not a position within <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ICharSequence? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, startIndex, count);
            return text;
        }

        #endregion Insert ICharSequence

        #region Insert ICharSequence repeated

        /// <summary>Inserts one or more copies of a specified sequence of characters into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The sequence of characters to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="repeatCount"/> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of <paramref name="value"/>
        /// times <paramref name="repeatCount"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// This <see cref="MutableTextBuffer"/> object is not changed if the length of <paramref name="value"/> is zero or
        /// <paramref name="repeatCount"/> is zero.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ICharSequence? value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, repeatCount);
            return text;
        }

        #endregion Insert ICharSequence repeated


        #region Replace ICharSequence

        /// <summary>
        /// Replaces the specified substring in this builder with the specified
        /// sequence of characters, <paramref name="newValue"/>. The substring begins at the specified
        /// <paramref name="startIndex"/> and ends at
        /// <c><paramref name="startIndex"/> + <paramref name="count"/></c> or
        /// to the end of the sequence if no such character exists. First the
        /// characters in the substring are removed and then the specified
        /// <paramref name="newValue"/> is inserted at <paramref name="startIndex"/>.
        /// This <see cref="MutableTextBuffer"/> will be lengthened to accommodate the
        /// specified <paramref name="newValue"/> if necessary.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="startIndex">The inclusive begin index in this builder.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="newValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than or equal to <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, int startIndex, int count, ICharSequence newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(startIndex, count, newValue);
            return text;
        }

        #endregion Replace ICharSequence
    }
}
