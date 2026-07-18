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

namespace J2N.Text
{
    internal static partial class MutableTextBufferExtensions
    {
        /// <summary>
        /// Deletes a sequence of characters specified by <paramref name="startIndex"/> and <paramref name="count"/>.
        /// Shifts any remaining characters to the left.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// <para/>
        /// This method differs from <see cref="Remove{TBuilder}(TBuilder, int, int)"/> in that it will automatically
        /// adjust the <paramref name="count"/> if <c><paramref name="startIndex"/> + <paramref name="count"/> > <see cref="MutableTextBuffer.Length"/></c>
        /// to <c><see cref="MutableTextBuffer.Length"/> - <paramref name="startIndex"/>.</c>, provided it is not bounded by <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of characters to delete.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Delete<TBuilder>(this TBuilder text, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.DeleteInternal(startIndex, count);
            return text;
        }

        /// <summary>Removes the character at the specified index from this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The zero-based position in this instance of the character to remove.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or
        /// greater than or equal to the length of this instance.
        /// </exception>
        /// <remarks>
        /// The current method removes the specified character from the current instance. The characters at
        /// (<paramref name="index"/> + 1) are moved to <paramref name="index"/>, and
        /// the string value of the current instance is shortened by 1. The capacity of the
        /// current instance is unaffected.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder RemoveAt<TBuilder>(this TBuilder text, int index)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.RemoveAtInternal(index);
            return text;
        }

        /// <summary>Removes the specified range of characters from this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="startIndex">The zero-based position in this instance where removal begins.</param>
        /// <param name="length">The number of characters to remove.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="startIndex"/> or <paramref name="length"/> is less than zero,
        /// or <paramref name="startIndex"/> + <paramref name="length"/> is greater than the length of this instance.
        /// </exception>
        /// <remarks>
        /// The current method removes the specified range of characters from the current instance. The characters at
        /// (<paramref name="startIndex"/> + <paramref name="length"/>) are moved to <paramref name="startIndex"/>, and
        /// the string value of the current instance is shortened by <paramref name="length"/>. The capacity of the
        /// current instance is unaffected.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Remove<TBuilder>(this TBuilder text, int startIndex, int length)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.RemoveInternal(startIndex, length);
            return text;
        }
    }
}
