using System;

namespace J2N.Text
{
    public partial class OpenStringBuilder
    {
        /// <summary>
        /// Retrieves a sub-sequence from this instance.
        /// The sub-sequence starts at a specified character position and has a specified length.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the second parameter is a length,
        /// not an exclusive end index as it would be in Java.
        /// </summary>
        /// <param name="startIndex">
        /// The start index of the sub-sequence. It is inclusive, that
        /// is, the index of the first character that is included in the
        /// sub-sequence.
        /// </param>
        /// <param name="length">The number of characters to return in the sub-sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public ICharSequence Subsequence(int startIndex, int length)
        {
            // From Apache Harmony String class
            if (m_Chars is null || (startIndex == 0 && length == m_Position))
            {
                return new CharArrayCharSequence(m_Chars);
            }
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(startIndex));
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(length));
            if ((uint)startIndex + (uint)length > m_Position)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);

            char[] result = new char[length];
            m_Chars.AsSpan(startIndex, length).CopyTo(result);
            return new CharArrayCharSequence(result);
        }

        #region ICharSequence

        bool ICharSequence.HasValue => true;

        #endregion ICharSequence
    }
}
