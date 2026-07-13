using J2N.CodeGeneration;
using System;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        #region Append ICharSequence

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified sequence to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ICharSequence?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(ICharSequence? value)
        {
            // This not only makes it faster, it will call our other overload to handle inserting into self
            if (value is ISpannable<char> spannable)
            {
                AppendInternal(spannable.AsSpan());
                return;
            }

            if (value is null || !value.HasValue)
                return;

            int count = value.Length;
            int pos = m_Position;
            if ((uint)pos + (uint)count > (uint)m_Chars.Length)
            {
                // Check if the count will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the StringBuilder.
                int newLength = pos + count;
                if (newLength > m_MaxCapacity || newLength < count)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                Grow(count);
            }

            if (value is ISpanCopyable<char> spanCopyable)
            {
                spanCopyable.CopyTo(0, m_Chars.AsSpan(pos), count);
            }
            else if (value is ICopyable<char> copyable)
            {
                copyable.CopyTo(0, m_Chars, pos, count);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    m_Chars[pos++] = value[i];
                }
            }
            m_Position += count;
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ICharSequence?, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(ICharSequence? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null || !value.HasValue)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }
            if (count > value.Length - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(count, ExceptionArgument.count);
            }

            if (count != 0)
            {
                if (startIndex > value.Length - count)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
                }

                int pos = m_Position;
                if ((uint)pos + (uint)count > (uint)m_Chars.Length)
                {
                    // Check if the count will put us over m_MaxCapacity.
                    // Doing the check here prevents corruption of the StringBuilder.
                    int newLength = pos + count;
                    if (newLength > m_MaxCapacity || newLength < count)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                    }

                    Grow(count);
                }

                // This not only makes it faster, it will call our other overload to handle inserting into self
                if (value is ISpannable<char> spannable)
                {
                    AppendInternal(spannable.AsSpan(startIndex, count));
                    return;
                }
                else if (value is ISpanCopyable<char> spanCopyable)
                {
                    spanCopyable.CopyTo(startIndex, m_Chars.AsSpan(pos), count);
                }
                else if (value is ICopyable<char> copyable)
                {
                    copyable.CopyTo(startIndex, m_Chars, pos, count);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        m_Chars[pos++] = value[i + startIndex];
                    }
                }
                m_Position += count;
            }
        }

        #endregion Append ICharSequence 

        #region Insert ICharSequence

        /// <summary>
        /// Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ICharSequence?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ICharSequence? value)
        {
            // This not only makes it faster, it will call our other overload to handle inserting into self
            if (value is ISpannable<char> spannable)
            {
                InsertInternal(index, spannable.AsSpan());
                return;
            }

            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value is null || !value.HasValue || value.Length == 0)
                return;

            int count = value.Length;
            MakeRoom(index, count);

            if (value is ISpanCopyable<char> spanCopyable)
            {
                spanCopyable.CopyTo(0, m_Chars.AsSpan(index), count);
            }
            else if (value is ICopyable<char> copyable)
            {
                copyable.CopyTo(0, m_Chars, index, count);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    m_Chars[index++] = value[i];
                }
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ICharSequence?, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ICharSequence? value, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value == null || !value.HasValue)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (startIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            }

            if (startIndex > value.Length - count)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            if (count > 0)
            {
                // This not only makes it faster, it will call our other overload to handle inserting into self
                if (value is ISpannable<char> spannable)
                {
                    InsertInternal(index, spannable.AsSpan(startIndex, count));
                    return;
                }

                MakeRoom(index, count);

                if (value is ISpanCopyable<char> spanCopyable)
                {
                    spanCopyable.CopyTo(startIndex, m_Chars.AsSpan(index), count);
                }
                else if (value is ICopyable<char> copyable)
                {
                    copyable.CopyTo(startIndex, m_Chars, index, count);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        m_Chars[index++] = value[i + startIndex];
                    }
                }
            }
        }

        #endregion Insert ICharSequence
    }
}
