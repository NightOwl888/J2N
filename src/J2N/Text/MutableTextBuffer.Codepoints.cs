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
using System.Runtime.CompilerServices;

namespace J2N.Text
{
    internal partial class MutableTextBuffer
    {
        /// <summary>
        /// Appends the string representation of the <paramref name="codePoint"/>.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendCodePoint{TBuilder}(TBuilder, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendCodePointInternal(int codePoint) // Coverage for the JDK
        {
            uint value = (uint)codePoint;
            if (!UnicodeUtility.IsValidCodePoint(value))
                ThrowHelper.ThrowArgumentOutOfRange_InvalidCodePoint(codePoint);

            bool isBmp = UnicodeUtility.IsBmpCodePoint(value);
            int pos = m_Position;
            if (isBmp && (uint)pos < (uint)m_Chars.Length)
            {
                m_Chars[pos] = (char)value;
                m_Position = pos + 1;
            }
            else
            {
                AppendCodePointSlow(value, isBmp);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendCodePointSlow(uint value, bool isBmp)
        {
            Debug.Assert(UnicodeUtility.IsValidCodePoint(value));

            int count = isBmp ? 1 : 2;
            if ((uint)m_Position + (uint)count > (uint)m_Chars.Length)
            {
                Grow(count);
            }

            int pos = m_Position;

            if (count == 1)
            {
                m_Chars[pos] = (char)value;
            }
            else
            {
                UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(
                    value,
                    out m_Chars[pos],
                    out m_Chars[pos + 1]);
            }

            m_Position = pos + count;
        }

        /// <summary>
        /// Inserts the string representation of the <paramref name="codePoint"/>.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.InsertCodePoint{TBuilder}(TBuilder, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertCodePointInternal(int index, int codePoint) // Coverage for the JDK
        {
            uint value = (uint)codePoint;
            if (!UnicodeUtility.IsValidCodePoint(value))
                ThrowHelper.ThrowArgumentOutOfRange_InvalidCodePoint(codePoint);
            if ((uint)index > Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

            int count = UnicodeUtility.IsBmpCodePoint(value) ? 1 : 2;

            MakeRoom(index, count);

            if (count == 1)
            {
                m_Chars[index] = (char)value;
            }
            else
            {
                UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(
                    value,
                    out m_Chars[index],
                    out m_Chars[index + 1]);
            }
        }

        /// <summary>
        /// Returns the code point at <paramref name="index"/> in the specified sequence of
        /// character units. If the unit at <paramref name="index"/> is a high-surrogate unit,
        /// <c><paramref name="index"/> + 1</c> is less than the length of the sequence and the unit at
        /// <c><paramref name="index"/> + 1</c> is a low-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <paramref name="index"/> is returned.
        /// </summary>
        /// <param name="index">The position in this <see cref="MutableTextBuffer"/> from which to retrieve the code
        /// point.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value at <paramref name="index"/> in
        /// this <see cref="MutableTextBuffer"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than or equal to <see cref="Length"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        public int CodePointAt(int index) // Coverage for the JDK
            => new ReadOnlySpan<char>(m_Chars, 0, m_Position).CodePointAt(index);

        /// <summary>
        /// Returns the code point that precedes <paramref name="index"/> in the specified
        /// sequence of character units. If the unit at <c><paramref name="index"/> - 1</c> is a
        /// low-surrogate unit, <c><paramref name="index"/> - 2</c> is not negative and the unit at
        /// <c><paramref name="index"/> - 2</c> is a high-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <c><paramref name="index"/> - 1</c> is returned.
        /// </summary>
        /// <param name="index">The position in this <see cref="MutableTextBuffer"/> following the code
        /// point that should be returned.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value before <paramref name="index"/>
        /// in this <see cref="MutableTextBuffer"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than
        /// 1 or greater than <see cref="Length"/>.</exception>
        public int CodePointBefore(int index) // Coverage for the JDK
            => new ReadOnlySpan<char>(m_Chars, 0, m_Position).CodePointBefore(index);

        /// <summary>
        /// Returns the number of Unicode code points in the text range of the specified char sequence.
        /// The text range begins at the specified <paramref name="startIndex"/> and extends for the number
        /// of characters specified in <paramref name="length"/>. 
        /// Unpaired surrogates within the text range count as one code point each.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index. To convert from
        /// Java, use <c>endIndex - startIndex</c> to obtain the length.
        /// </summary>
        /// <param name="startIndex">The index to the first char of the text range.</param>
        /// <param name="length">The number of characters to consider in this <see cref="MutableTextBuffer"/>.</param>
        /// <returns>The number of Unicode code points in the specified text range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within
        /// this <see cref="MutableTextBuffer"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public int CodePointCount(int startIndex, int length) // Coverage for the JDK
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if ((uint)startIndex + (uint)length > m_Position)
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            return new ReadOnlySpan<char>(m_Chars, startIndex, length).CodePointCount();
        }

        /// <summary>
        /// Returns the index within the given char sequence that is offset from the given <paramref name="index"/> by
        /// <paramref name="codePointOffset"/> code points. Unpaired surrogates within the text range given by 
        /// <paramref name="index"/> and <paramref name="codePointOffset"/> count as one code point each.
        /// </summary>
        /// <param name="index">The index to be offset.</param>
        /// <param name="codePointOffset">The number of code points to look backwards or forwards; may
        /// be a negative or positive value.</param>
        /// <returns>The index within the char sequence, offset by <paramref name="codePointOffset"/> code points.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than <see cref="Length"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is positive and the subsequence starting with
        /// <paramref name="index"/> has fewer than <paramref name="codePointOffset"/> code points.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is negative and the subsequence before <paramref name="index"/>
        /// has fewer than the absolute value of <paramref name="codePointOffset"/> code points.
        /// </exception>
        public int OffsetByCodePoints(int index, int codePointOffset) // Coverage for the JDK
            => new ReadOnlySpan<char>(m_Chars, 0, m_Position).OffsetByCodePoints(index, codePointOffset);
    }
}
