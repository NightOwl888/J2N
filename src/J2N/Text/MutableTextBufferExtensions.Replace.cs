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
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    internal static partial class MutableTextBufferExtensions
    {
        /// <summary>Replaces all occurrences of a specified string in this instance with another specified string.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <see langword="null"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/> in the
        /// current instance. If <paramref name="newValue"/> is <see langword="null"/> or <see cref="string.Empty"/>, all occurrences of
        /// <paramref name="oldValue"/> are removed.
        /// </remarks>
        /// <seealso cref="Remove{TBuilder}(TBuilder, int, int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, string oldValue, string? newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldValue, newValue);
            return text;
        }

        /// <summary>Replaces all instances of one read-only character span with another in this builder.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldValue">The read-only character span to replace.</param>
        /// <param name="newValue">The read-only character span to replace <paramref name="oldValue"/> with.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/> in the
        /// current instance. If <paramref name="newValue"/> is empty, all occurrences of <paramref name="oldValue"/> are removed.
        /// </remarks>
        /// <seealso cref="Remove{TBuilder}(TBuilder, int, int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldValue, newValue);
            return text;
        }

        /// <summary>Replaces, within a substring of this instance, all occurrences of a specified string with another specified string.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <see langword="null"/>.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a character position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/>
        /// in the specified substring. If <paramref name="newValue"/> is <see langword="null"/> or <see cref="string.Empty"/>,
        /// all occurrences of <paramref name="oldValue"/> in the specified range are removed.
        /// </remarks>
        /// <seealso cref="Remove{TBuilder}(TBuilder, int, int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, string oldValue, string? newValue, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldValue, newValue, startIndex, count);
            return text;
        }

        /// <summary>Replaces all instances of one read-only character span with another in a substring of this builder.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldValue">The read-only character span to replace.</param>
        /// <param name="newValue">The read-only character span to replace <paramref name="oldValue"/> with.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a character position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/>
        /// in the specified substring. If <paramref name="newValue"/> is empty, all occurrences of <paramref name="oldValue"/>
        /// in the specified range are removed.
        /// </remarks>
        /// <seealso cref="Remove{TBuilder}(TBuilder, int, int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldValue, newValue, startIndex, count);
            return text;
        }

        /// <summary>Replaces all occurrences of a specified character in this instance with another specified character.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldChar">The character to replace.</param>
        /// <param name="newChar">The character that replaces <paramref name="oldChar"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of
        /// <paramref name="oldChar"/> in the current instance. The size of the current
        /// <see cref="MutableTextBuffer"/> instance is unchanged after the replacement.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, char oldChar, char newChar)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldChar, newChar);
            return text;
        }

        /// <summary>Replaces, within a substring of this instance, all occurrences of a specified character with another specified character.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldChar">The character to replace.</param>
        /// <param name="newChar">The character that replaces <paramref name="oldChar"/>.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a character position not within this instance.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of
        /// <paramref name="oldChar"/> in the current instance within the specified substring. The size of the current
        /// <see cref="MutableTextBuffer"/> instance is unchanged after the replacement.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, char oldChar, char newChar, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldChar, newChar, startIndex, count);
            return text;
        }

        /// <summary>
        /// Replaces the specified substring in this builder with the specified
        /// string, <paramref name="newValue"/>. The substring begins at the specified
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
        public static TBuilder Replace<TBuilder>(this TBuilder text, int startIndex, int count, string newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(startIndex, count, newValue);
            return text;
        }

        /// <summary>
        /// Replaces the specified substring in this builder with the specified
        /// character span, <paramref name="newValue"/>. The substring begins at the specified
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than or equal to <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        /// <remarks>This method allows <paramref name="newValue"/> to be this instance or a slice of this instance.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, int startIndex, int count, ReadOnlySpan<char> newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(startIndex, count, newValue);
            return text;
        }

        /// <summary>
        /// Replaces the specified substring in this builder with the specified
        /// string builder, <paramref name="newValue"/>. The substring begins at the specified
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
        public static TBuilder Replace<TBuilder>(this TBuilder text, int startIndex, int count, StringBuilder newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(startIndex, count, newValue);
            return text;
        }

        // J2N: Moved ICharSequence overload to the J2N namespace
    }
}
