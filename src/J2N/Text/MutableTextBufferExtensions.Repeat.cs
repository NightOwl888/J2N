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
    public static partial class MutableTextBufferExtensions
    {
        /// <summary>Appends a specified number of copies of the string representation of a Unicode character to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The character to append.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="repeatCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="OutOfMemoryException">Out of memory.</exception>
        /// <remarks>
        /// The <see cref="Append{TBuilder}(TBuilder, char, int)"/> method modifies the existing instance of this class;
        /// it does not return a new class instance. Because of this, you can call a method or property
        /// on the existing reference and you do not have to assign the return value to an
        /// <see cref="MutableTextBuffer"/> object, as the following example illustrates.
        /// <code>
        /// decimal value = 1346.19m;
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// sb.Append('*', 5).AppendFormat("{0:C2}", value).Append('*', 5);
        /// Console.WriteLine(sb);
        /// // The example displays the following output:
        /// //       *****$1,346.19*****
        /// </code>
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
        /// <seealso cref="char" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, char value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, repeatCount);
            return text;
        }

        /// <summary>Inserts one or more copies of a specified string into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The string to insert.</param>
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
        /// This <see cref="MutableTextBuffer"/> object is not changed if <paramref name="value"/> is <see langword="null"/>,
        /// <paramref name="value"/> is not <see langword="null"/> but its length is zero, or <paramref name="repeatCount"/> is zero.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, string? value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, repeatCount);
            return text;
        }

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
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ReadOnlySpan<char> value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, repeatCount);
            return text;
        }

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
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, StringBuilder? value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, repeatCount);
            return text;
        }

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
    }
}
