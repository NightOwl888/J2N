using J2N.CodeGeneration;
using System;

namespace J2N.Text
{
    public partial class MutableTextBuffer
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
        /// <param name="codePoint">A Unicode code point.</param>
        /// <returns>This <see cref="MutableTextBuffer"/>, for chaining.</returns>
        /// <exception cref="ArgumentException"><paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        [CodeGenerationReturnsSelf]
        public void AppendCodePoint(int codePoint) // Coverage for the JDK
        {
            int count = Character.ToChars(codePoint, out char high, out char low);

            int pos = m_Position;
            if (pos > m_Chars.Length - count)
            {
                // Check if the count will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the StringBuilder.
                int newLength = pos + count;
                if (newLength > m_MaxCapacity || newLength < count)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.codePoint, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                Grow(count);
            }

            m_Chars[pos++] = high;
            if (count == 2)
                m_Chars[pos++] = low;
            m_Position += count;
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
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="codePoint">A Unicode code point.</param>
        /// <returns>This <see cref="MutableTextBuffer"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater
        /// than the length of this instance.</exception>
        /// <exception cref="ArgumentException"><paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        [CodeGenerationReturnsSelf]
        public void InsertCodePoint(int index, int codePoint)
        {
            if ((uint)index > Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

            int count = Character.ToChars(codePoint, out char high, out char low);

            // Check if the count will put us over m_MaxCapacity.
            // Doing the check here prevents corruption of the StringBuilder.
            int newLength = m_Position + count;
            if (newLength > m_MaxCapacity || newLength < count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.codePoint, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
            }

            MakeRoom(index, count);

            m_Chars[index] = high;
            if (count == 2)
                m_Chars[index + 1] = low;
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
