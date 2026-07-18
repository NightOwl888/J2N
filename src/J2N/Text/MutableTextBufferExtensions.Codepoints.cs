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
        /// Appends the string representation of the <paramref name="codePoint"/>
        /// argument to this sequence.
        /// <para>
        /// The argument is appended to the contents of this sequence.
        /// The length of this sequence increases by <see cref="Character.CharCount(int)"/>.
        /// </para>
        /// <para>
        /// The overall effect is exactly as if the argument were
        /// converted to a <see cref="char"/> array by the method
        /// <see cref="Character.ToChars(int)"/> and the character in that array
        /// were then appended to this <see cref="MutableTextBuffer"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="codePoint">A Unicode code point.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentException"><paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendCodePoint<TBuilder>(this TBuilder text, int codePoint)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendCodePointInternal(codePoint);
            return text;
        }

        /// <summary>
        /// Insert the string representation of the <paramref name="codePoint"/>
        /// argument to this sequence at <paramref name="index"/>.
        /// <para>
        /// The argument is inserted into to the contents of this sequence.
        /// The length of this sequence increases by <see cref="Character.CharCount(int)"/>.
        /// </para>
        /// <para>
        /// The overall effect is exactly as if the argument were
        /// converted to a <see cref="char"/> array by the method
        /// <see cref="Character.ToChars(int)"/> and the character in that array
        /// were then inserted into this <see cref="MutableTextBuffer"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="codePoint">A Unicode code point.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater
        /// than the length of this instance.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder InsertCodePoint<TBuilder>(this TBuilder text, int index, int codePoint)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertCodePointInternal(index, codePoint);
            return text;
        }
    }
}
