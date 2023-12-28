using System;

namespace J2N.Memory
{
    /// <summary>
    /// Extensions to System.Memory types.
    /// </summary>
    public static class MemoryExtensions
    {
        #region Reverse

#if FEATURE_SPAN
        /// <summary>
        /// Causes this character sequence to be replaced by the reverse of
        /// the sequence. If there are any surrogate pairs included in the
        /// sequence, these are treated as single characters for the
        /// reverse operation. Thus, the order of the high-low surrogates
        /// is never reversed.
        /// <para/>
        /// This operation is done in-place.
        /// <para/>
        /// Let <c>n</c> be the character length of this character sequence
        /// (not the length in <see cref="char"/> values) just prior to
        /// execution of the <see cref="ReverseText(Span{char})"/> method. Then the
        /// character at index <c>k</c> in the new character sequence is
        /// equal to the character at index <c>n-k-1</c> in the old
        /// character sequence.
        /// <para/>
        /// Note that the reverse operation may result in producing
        /// surrogate pairs that were unpaired low-surrogates and
        /// high-surrogates before the operation. For example, reversing
        /// "&#92;uDC00&#92;uD800" produces "&#92;uD800&#92;uDC00" which is
        /// a valid surrogate pair.
        /// <para/>
        /// Usage Note: This is the same operation as
        /// <see cref="J2N.Text.StringBuilderExtensions.Reverse(System.Text.StringBuilder)"/>
        /// (derived from Java's StringBuilder.reverse() method) but is more
        /// efficient because it doesn't allocate a new <see cref="System.Text.StringBuilder"/>
        /// instance.
        /// </summary>
        /// <param name="text">this <see cref="Span{Char}"/></param>
        /// <seealso cref="J2N.Text.StringBuilderExtensions.Reverse(System.Text.StringBuilder)"/>
        /// <seealso cref="J2N.Text.StringExtensions.ReverseText(string)"/>
        public unsafe static void ReverseText(this Span<char> text)
        {
            int count = text.Length;
            if (count == 0) return;
            fixed (char* textPtr = text)
            {
                ReverseText(textPtr, count);
            }
        }

#endif

        internal unsafe static void ReverseText(char* text, int count)
        {
            if (count == 0) return;
            int start = 0;
            int end = count - 1;
            char startHigh = text[0];
            char endLow = text[end];
            bool allowStartSurrogate = true, allowEndSurrogate = true;
            while (start < end)
            {
                char startLow = text[start + 1];
                char endHigh = text[end - 1];
                bool surrogateAtStart = allowStartSurrogate && startLow >= 0xdc00
                        && startLow <= 0xdfff && startHigh >= 0xd800
                        && startHigh <= 0xdbff;
                if (surrogateAtStart && (count < 3))
                {
                    return;
                }
                bool surrogateAtEnd = allowEndSurrogate && endHigh >= 0xd800
                        && endHigh <= 0xdbff && endLow >= 0xdc00
                        && endLow <= 0xdfff;
                allowStartSurrogate = allowEndSurrogate = true;
                if (surrogateAtStart == surrogateAtEnd)
                {
                    if (surrogateAtStart)
                    {
                        // both surrogates
                        text[end] = startLow;
                        text[end - 1] = startHigh;
                        text[start] = endHigh;
                        text[start + 1] = endLow;
                        startHigh = text[start + 2];
                        endLow = text[end - 2];
                        start++;
                        end--;
                    }
                    else
                    {
                        // neither surrogates
                        text[end] = startHigh;
                        text[start] = endLow;
                        startHigh = startLow;
                        endLow = endHigh;
                    }
                }
                else
                {
                    if (surrogateAtStart)
                    {
                        // surrogate only at the front
                        text[end] = startLow;
                        text[start] = endLow;
                        endLow = endHigh;
                        allowStartSurrogate = false;
                    }
                    else
                    {
                        // surrogate only at the end
                        text[end] = startHigh;
                        text[start] = endHigh;
                        startHigh = startLow;
                        allowEndSurrogate = false;
                    }
                }
                start++;
                end--;
            }
            if ((count & 1) == 1 && (!allowStartSurrogate || !allowEndSurrogate))
            {
                text[end] = allowStartSurrogate ? endLow : startHigh;
            }
        }

        #endregion
    }
}
