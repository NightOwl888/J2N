using J2N.CodeGeneration;
using System;
using System.Text;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        #region Append ICharSequence

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified sequence to this instance.
        /// <para/>
        /// NOTE: Unlike the Java implementation, this method does not add the word <c>"null"</c> to the <see cref="MutableTextBuffer"/>
        /// if <paramref name="value"/> is <c>null</c>. Instead, no operation is performed.
        /// </summary>
        /// <param name="value">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        /// <seealso cref="ICharSequence"/>
        [CodeGenerationReturnsSelf]
        public void Append(ICharSequence? value)
        {
            // This not only makes it faster, it will call our other overload to handle inserting into self
            if (value is ISpannable<char> spannable)
            {
                Append(spannable.AsSpan());
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
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="count">The number of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <seealso cref="ICharSequence"/>
        [CodeGenerationReturnsSelf]
        public void Append(ICharSequence? value, int startIndex, int count)
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
                    Append(spannable.AsSpan(startIndex, count));
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
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character sequence to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, ICharSequence? value)
        {
            // This not only makes it faster, it will call our other overload to handle inserting into self
            if (value is ISpannable<char> spannable)
            {
                Insert(index, spannable.AsSpan());
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
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="count"/>.
        /// </summary>
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, ICharSequence? value, int startIndex, int count)
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
                    Insert(index, spannable.AsSpan(startIndex, count));
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

        #region ISpanAppendable Members

        ISpanAppendable ISpanAppendable.Append(ReadOnlySpan<char> value)
        {
            Append(value);
            return this;
        }

        #endregion ISpanAppendable Members

        #region IAppendable Members
        IAppendable IAppendable.Append(char value)
        {
            Append(value);
            return this;
        }

        IAppendable IAppendable.Append(string? value)
        {
            Append(value);
            return this;
        }

        IAppendable IAppendable.Append(string? value, int startIndex, int count)
        {
            Append(value, startIndex, count);
            return this;
        }

        IAppendable IAppendable.Append(StringBuilder? value)
        {
            Append(value);
            return this;
        }

        IAppendable IAppendable.Append(StringBuilder? value, int startIndex, int count)
        {
            Append(value, startIndex, count);
            return this;
        }

        IAppendable IAppendable.Append(char[]? value)
        {
            Append(value);
            return this;
        }

        IAppendable IAppendable.Append(char[]? value, int startIndex, int count)
        {
            Append(value, startIndex, count);
            return this;
        }

        IAppendable IAppendable.Append(ICharSequence? value)
        {
            Append(value);
            return this;
        }

        IAppendable IAppendable.Append(ICharSequence? value, int startIndex, int count)
        {
            Append(value, startIndex, count);
            return this;
        }

        #endregion IAppendable Members
    }
}
