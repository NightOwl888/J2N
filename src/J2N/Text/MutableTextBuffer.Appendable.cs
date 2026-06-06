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
        /// if <paramref name="charSequence"/> is <c>null</c>. Instead, no operation is performed.
        /// </summary>
        /// <param name="charSequence">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        /// <seealso cref="ICharSequence"/>
        [CodeGenerationReturnsSelf]
        public void Append(ICharSequence? charSequence) // J2N: Parameter named charSequence so it can be specified explicitly to differentiate from object
        {
            if (charSequence is null || !charSequence.HasValue)
                return;

            if (charSequence is StringCharSequence str)
            {
                Append(str.Value);
                return;
            }
            else if (charSequence is CharArrayCharSequence chars)
            {
                Append(chars.Value);
                return;
            }
            else if (charSequence is StringBuilderCharSequence sb)
            {
                Append(sb.Value);
                return;
            }
            else if (charSequence is MutableTextBuffer osb)
            {
                Append(osb);
                return;
            }
            else if (charSequence is StringBuffer buffer)
            {
                lock (buffer.SyncRoot)
                {
                    Append(buffer.builder);
                    return;
                }
            }

            int count = charSequence.Length;
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

            for (int i = 0; i < count; i++)
            {
                m_Chars[pos++] = charSequence[i];
            }
            m_Position += count;
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <param name="charSequence">The UTF-16-encoded code unit to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="charSequence"/>.</param>
        /// <param name="count">The number of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="charSequence"/> is <c>null</c>, and
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
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <seealso cref="ICharSequence"/>
        [CodeGenerationReturnsSelf]
        public void Append(ICharSequence? charSequence, int startIndex, int count)
        {
            if (charSequence is StringCharSequence str)
            {
                Append(str.Value, startIndex, count);
                return;
            }
            else if (charSequence is CharArrayCharSequence chars)
            {
                Append(chars.Value, startIndex, count);
                return;
            }
            else if (charSequence is StringBuilderCharSequence sb)
            {
                Append(sb.Value, startIndex, count);
                return;
            }
            else if (charSequence is MutableTextBuffer osb)
            {
                Append(osb, startIndex, count);
                return;
            }
            else if (charSequence is StringBuffer buffer)
            {
                lock (buffer.SyncRoot)
                {
                    Append(buffer.builder, startIndex, count);
                    return;
                }
            }

            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (charSequence == null || !charSequence.HasValue)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.charSequence);
            }

            if (count != 0)
            {
                if (startIndex > charSequence.Length - count)
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
                        ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                    }

                    Grow(count);
                }

                for (int i = 0; i < count; i++)
                {
                    m_Chars[pos++] = charSequence[i + startIndex];
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
        /// <param name="charSequence">The character sequence to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, ICharSequence? charSequence)
        {
            if (charSequence is null || !charSequence.HasValue || charSequence.Length == 0)
                return;

            if (charSequence is StringCharSequence stringCharSequence)
            {
                Insert(index, stringCharSequence?.Value);
                return;
            }
            else if (charSequence is CharArrayCharSequence chars)
            {
                Insert(index, chars.Value);
                return;
            }
            else if (charSequence is StringBuilderCharSequence sbCharSequence)
            {
                Insert(index, sbCharSequence.Value);
                return;
            }
            else if (charSequence is MutableTextBuffer osb)
            {
                Insert(index, osb.AsSpan());
                return;
            }
            else if (charSequence is StringBuffer sBuffer)
            {
                lock (sBuffer.SyncRoot)
                {
                    Insert(index, sBuffer.builder);
                    return;
                }
            }

            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            int count = charSequence.Length;
            MakeRoom(index, count);

            for (int i = 0; i < count; i++)
            {
                m_Chars[index++] = charSequence[i];
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="count"/>.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="charSequence"/>.</param>
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
        /// <paramref name="startIndex"/> plus <paramref name="count"/> is not a position within <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, ICharSequence? charSequence, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (charSequence == null || !charSequence.HasValue)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (charSequence is StringCharSequence stringCharSequence)
            {
                Insert(index, stringCharSequence.Value.AsSpan(startIndex, count));
                return;
            }
            else if (charSequence is CharArrayCharSequence chars)
            {
                Insert(index, chars.Value, startIndex, count);
                return;
            }
            else if (charSequence is StringBuilderCharSequence sbCharSequence)
            {
                Insert(index, sbCharSequence.Value!.ToString(startIndex, count));
                return;
            }
            else if (charSequence is MutableTextBuffer osb)
            {
                Insert(index, osb.AsSpan(startIndex, count));
                return;
            }
            else if (charSequence is StringBuffer sBuffer)
            {
                lock (sBuffer.SyncRoot)
                {
                    Insert(index, sBuffer.builder.ToString(startIndex, count));
                    return;
                }
            }

            if (startIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            }

            if (startIndex > charSequence.Length - count)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            if (count > 0)
            {
                MakeRoom(index, count);
                for (int i = 0; i < count; i++)
                {
                    m_Chars[index++] = charSequence[i + startIndex];
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
