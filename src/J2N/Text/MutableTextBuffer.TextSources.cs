// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.CodeGeneration;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        #region Append char

        /// <summary>
        /// Appends the string representation of a specified <see cref="char"/> to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, char)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(char value)
        {
            int pos = m_Position;
            if ((uint)pos < (uint)m_Chars.Length)
            {
                m_Chars[pos] = value;
                m_Position = pos + 1;
            }
            else
            {
                AppendWithExpansion(value);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendWithExpansion(char value)
        {
            Grow(1);
            m_Chars[m_Position] = value;
            m_Position++;
        }

        #endregion Append char

        #region Append char[]

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified array to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, char[])"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(char[]? value)
        {
            if (value is not null)
            {
#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
                Append(ref MemoryMarshal.GetArrayDataReference(value), value.Length);
#else
                Append(ref MemoryMarshal.GetReference(value), value.Length);
#endif
            }
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, char[], int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(char[]? value, int startIndex, int charCount)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (charCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);

            if (value == null)
            {
                if (startIndex == 0 && charCount == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }
            if (charCount > value.Length - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(charCount, ExceptionArgument.charCount);
            }

            if (charCount != 0)
            {
                Append(ref value[startIndex], charCount);
            }
        }

        #endregion Append char[]

        #region Append char*

        /// <summary>
        /// Appends an array of Unicode characters starting at a specified address to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, char*, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal unsafe void AppendInternal(char* value, int valueCount)
        {
            // We don't check null value as this case will throw null reference exception anyway
            if (valueCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(valueCount, ExceptionArgument.valueCount);

            if (valueCount == 0)
                return;

            if (Overlaps(value, valueCount, out int sourceOffset))
            {
                AppendOverlapping(sourceOffset, valueCount);
                return;
            }

            Append(ref *value, valueCount);
        }

        #endregion Append char*

        #region Append string

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, string?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(string? value)
        {
            if (value is not null)
            {
                Append(ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
            }
        }

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, string?, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(string? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (count != 0)
            {
                if (startIndex > value.Length - count)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
                }

                Append(ref MemoryMarshal.GetReference(value.AsSpan(startIndex)), count);
            }
        }

        #endregion Append string

        #region Append ReadOnlySpan<char>

        /// <summary>
        /// Appends the string representation of a specified read-only character span to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ReadOnlySpan{char})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
                return;

            if (m_Chars.AsSpan().Overlaps(value, out int sourceOffset))
            {
                AppendOverlapping(sourceOffset, value.Length);
                return;
            }

            Append(ref MemoryMarshal.GetReference(value), value.Length);
        }

        #endregion Append ReadOnlySpan<char>

        #region Append ReadOnlyMemory<char>

        /// <summary>
        /// Appends the string representation of a specified read-only character memory to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ReadOnlyMemory{char})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(ReadOnlyMemory<char> value) => AppendInternal(value.Span);

        #endregion Append ReadOnlyMemory<char>

        #region Append StringBuilder

        /// <summary>
        /// Appends a copy of a specified string builder to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, StringBuilder?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(StringBuilder? value)
        {
            if (value != null && value.Length != 0)
            {
                AppendCore(value, 0, value.Length);
            }
        }

        /// <summary>
        /// Appends a copy of a specified substring of a string builder to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, StringBuilder?, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(StringBuilder? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (count == 0)
            {
                return;
            }

            if (count > value.Length - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            AppendCore(value, startIndex, count);
        }

        private void AppendCore(StringBuilder value, int startIndex, int count)
        {
            uint newLength = (uint)Length + (uint)count;

            if (newLength > (uint)m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.Capacity, ExceptionResource.ArgumentOutOfRange_Capacity);
            }

            int pos = m_Position;
            if (pos > m_Chars.Length - count)
            {
                Grow(count);
            }

            value.CopyTo(startIndex, m_Chars, m_Position, count);
            m_Position += count;
        }

        #endregion Append StringBuilder

        #region Append ICharSequence

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified sequence to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="J2N.MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ICharSequence?)"/>.
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
            char[]? arrayToReturn = null;
            try
            {
                // If the source doesn't implement ISpannable<char>, we have no way to test
                // whether the implementation overlaps our memory. So, the only safe approach
                // is to always take a snapshot prior to moving any memory.

                Span<char> temp = count <= CharStackBufferSize
                    ? stackalloc char[count]
                    : (arrayToReturn = allocator.Allocate(count)).AsSpan(0, count);

                if (value is ISpanCopyable<char> spanCopyable)
                {
                    spanCopyable.CopyTo(0, temp, count);
                }
                else if (arrayToReturn is not null && value is ICopyable<char> copyable)
                {
                    copyable.CopyTo(0, arrayToReturn, 0, count);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        temp[i] = value[i];
                    }
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

                temp.CopyTo(m_Chars.AsSpan(pos));
            }
            finally
            {
                if (arrayToReturn is not null)
                    allocator.Return(arrayToReturn);
            }

            m_Position += count;
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="J2N.MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ICharSequence?, int, int)"/>.
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

                // This not only makes it faster, it will call our other overload to handle inserting into self
                if (value is ISpannable<char> spannable)
                {
                    AppendInternal(spannable.AsSpan(startIndex, count));
                    return;
                }

                char[]? arrayToReturn = null;
                try
                {
                    // If the source doesn't implement ISpannable<char>, we have no way to test
                    // whether the implementation overlaps our memory. So, the only safe approach
                    // is to always take a snapshot prior to moving any memory.

                    Span<char> temp = count <= CharStackBufferSize
                        ? stackalloc char[count]
                        : (arrayToReturn = allocator.Allocate(count)).AsSpan(0, count);

                    if (value is ISpanCopyable<char> spanCopyable)
                    {
                        spanCopyable.CopyTo(startIndex, temp, count);
                    }
                    else if (arrayToReturn is not null && value is ICopyable<char> copyable)
                    {
                        copyable.CopyTo(startIndex, arrayToReturn, 0, count);
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            temp[i] = value[startIndex + i];
                        }
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

                    temp.CopyTo(m_Chars.AsSpan(pos));
                }
                finally
                {
                    if (arrayToReturn is not null)
                        allocator.Return(arrayToReturn);
                }

                m_Position += count;
            }
        }

        #endregion Append ICharSequence 


        #region Insert char

        /// <summary>
        /// Inserts the string representation of a specified Unicode character into this instance
        /// at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, char)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, char value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            Insert(index, ref value, 1);
        }

        #endregion Insert char

        #region Insert char[]

        /// <summary>
        /// Inserts the string representation of a specified array of Unicode characters into this instance
        /// at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, char[])"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, char[]? value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value != null)
            {
#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
                Insert(index, ref MemoryMarshal.GetArrayDataReference(value), value.Length);
#else
                Insert(index, ref MemoryMarshal.GetReference(value), value.Length);
#endif
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance
        /// at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, char[], int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, char[]? value, int startIndex, int charCount)
        {
            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value == null)
            {
                if (startIndex == 0 && charCount == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (startIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            }

            if (charCount < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);
            }

            if (startIndex > value.Length - charCount)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            if (charCount > 0)
            {
                Insert(index, ref value[startIndex], charCount);
            }
        }

        #endregion Insert char[]

        #region Insert char*

        /// <summary>
        /// Inserts an array of Unicode characters starting at a specified address into this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, char*, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal unsafe void InsertInternal(int index, char* value, int valueCount)
        {
            // We don't check null value as this case will throw null reference exception anyway
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }
            if (valueCount < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(valueCount, ExceptionArgument.valueCount);
            }
            // Check if the valueCount will put us over m_MaxCapacity.
            // Doing the check here prevents corruption of the MutableTextBuffer.
            int newLength = m_Position + valueCount;
            if (newLength > m_MaxCapacity || newLength < valueCount)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(valueCount, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
            }

            if (Overlaps(value, valueCount, out int sourceOffset))
            {
                InsertOverlapping(index, sourceOffset, valueCount);
                return;
            }

            Insert(index, ref *value, valueCount);
        }

        #endregion Insert char*

        #region Insert string

        /// <summary>
        /// Inserts a string into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, string?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, string? value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value != null)
            {
                Insert(index, ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
            }
        }

        /// <summary>
        /// Inserts the specified substring into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, string?, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, string? value, int startIndex, int count) // J2N: Added to cover the JDK better (rather than ICharSequence only)
        {
            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value == null)
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
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.charCount);
            }

            if (startIndex > value.Length - count)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            if (count > 0)
            {
                Insert(index, ref MemoryMarshal.GetReference(value.AsSpan(startIndex)), count);
            }
        }

        #endregion Insert string

        #region Insert ReadOnlySpan<char>

        /// <summary>
        /// Inserts the sequence of characters into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ReadOnlySpan{char})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ReadOnlySpan<char> value) // J2N NOTE: Weird that upstream they made an overload of ReadOnlyMemory<char> for Append, but not Insert.
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value.Length != 0)
            {
                // There is a slight danger that value is actually a slice of m_Chars.
                if (m_Chars.AsSpan().Overlaps(value, out int sourceOffset))
                {
                    InsertOverlapping(index, sourceOffset, value.Length);
                    return;
                }

                Insert(index, ref MemoryMarshal.GetReference(value), value.Length);
            }
        }

        #endregion Insert ReadOnlySpan<char>

        #region Insert StringBuilder

        /// <summary>
        /// Inserts a string builder into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, StringBuilder?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, StringBuilder? value)
        {
            if (value is null)
                return;

            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            int count = value.Length;
            if (count > 0)
            {
                MakeRoom(index, count);

                value.CopyTo(0, m_Chars, index, count);
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance
        /// at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, StringBuilder?, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, StringBuilder? value, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value == null)
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
                MakeRoom(index, count);
                value.CopyTo(startIndex, m_Chars, index, count);
            }
        }

        #endregion Insert StringBuilder

        #region Insert ICharSequence

        /// <summary>
        /// Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="J2N.MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ICharSequence?)"/>.
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
            char[]? arrayToReturn = null;
            try
            {
                // If the source doesn't implement ISpannable<char>, we have no way to test
                // whether the implementation overlaps our memory. So, the only safe approach
                // is to always take a snapshot prior to moving any memory.

                Span<char> temp = count <= CharStackBufferSize
                    ? stackalloc char[count]
                    : (arrayToReturn = allocator.Allocate(count)).AsSpan(0, count);

                if (value is ISpanCopyable<char> spanCopyable)
                {
                    spanCopyable.CopyTo(0, temp, count);
                }
                else if (arrayToReturn is not null && value is ICopyable<char> copyable)
                {
                    copyable.CopyTo(0, arrayToReturn, 0, count);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        temp[i] = value[i];
                    }
                }

                MakeRoom(index, count);
                temp.CopyTo(m_Chars.AsSpan(index));
            }
            finally
            {
                if (arrayToReturn is not null)
                    allocator.Return(arrayToReturn);
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="J2N.MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ICharSequence?, int, int)"/>.
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

                char[]? arrayToReturn = null;
                try
                {
                    // If the source doesn't implement ISpannable<char>, we have no way to test
                    // whether the implementation overlaps our memory. So, the only safe approach
                    // is to always take a snapshot prior to moving any memory.

                    Span<char> temp = count <= CharStackBufferSize
                        ? stackalloc char[count]
                        : (arrayToReturn = allocator.Allocate(count)).AsSpan(0, count);

                    if (value is ISpanCopyable<char> spanCopyable)
                    {
                        spanCopyable.CopyTo(startIndex, temp, count);
                    }
                    else if (arrayToReturn is not null && value is ICopyable<char> copyable)
                    {
                        copyable.CopyTo(startIndex, arrayToReturn, 0, count);
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            temp[i] = value[startIndex + i];
                        }
                    }

                    MakeRoom(index, count);
                    temp.CopyTo(m_Chars.AsSpan(index));
                }
                finally
                {
                    if (arrayToReturn is not null)
                        allocator.Return(arrayToReturn);
                }
            }
        }

        #endregion Insert ICharSequence


        /// <summary>
        /// Determines whether <paramref name="value"/> points into the current backing
        /// array and, if so, returns its character offset.
        /// </summary>
        /// <param name="value">The source pointer.</param>
        /// <param name="valueCount">The number of characters that will be read.</param>
        /// <param name="sourceOffset">
        /// Receives the offset into <see cref="m_Chars"/> if this method returns
        /// <see langword="true"/>; otherwise -1.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the entire source range lies within
        /// <see cref="m_Chars"/>; otherwise <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe bool Overlaps(char* value, int valueCount, out int sourceOffset)
        {
            Debug.Assert(valueCount >= 0, "Invalid length; should have been validated by caller.");

            fixed (char* buffer = m_Chars)
            {
                nuint offset = (nuint)(value - buffer);

                if (offset <= (nuint)(m_Chars.Length - valueCount))
                {
                    sourceOffset = (int)offset;
                    return true;
                }
            }

            sourceOffset = -1;
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendOverlapping(int sourceOffset, int valueCount)
        {
            // J2N TODO: Add Debug.Asserts here for invariants

            // If the append fits in the existing array, Memmove already
            // supports overlap perfectly.
            if ((uint)m_Position + (uint)valueCount <= (uint)m_Chars.Length)
            {
                Append(ref m_Chars[sourceOffset], valueCount);
                return;
            }

            // If not, we snapshot the source so the operation is safe
            AppendWithSnapshot(sourceOffset, valueCount);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void AppendWithSnapshot(int sourceOffset, int valueCount)
            {
                char[]? buffer = null;
                try
                {
                    Span<char> temp = valueCount <= CharStackBufferSize
                        ? stackalloc char[valueCount]
                        : (buffer = ArrayPool<char>.Shared.Rent(valueCount)).AsSpan(0, valueCount);

                    m_Chars.AsSpan(sourceOffset, valueCount).CopyTo(temp);
                    Append(ref MemoryMarshal.GetReference(temp), temp.Length);
                }
                finally
                {
                    if (buffer is not null)
                        ArrayPool<char>.Shared.Return(buffer);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InsertOverlapping(int index, int sourceOffset, int count)
        {
            // J2N TODO: Add Debug.Asserts here for invariants

            bool entirelyWithinLiveBuffer =
                (uint)sourceOffset <= (uint)m_Position &&
                (uint)sourceOffset + (uint)count <= (uint)m_Position;

            if (entirelyWithinLiveBuffer)
            {
                InsertFromSelfInternal(index, sourceOffset, count);
                return;
            }

            // Snapshot the value to a temporary buffer and then insert
            InsertWithSnapshot(index, sourceOffset, count);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void InsertWithSnapshot(int index, int sourceOffset, int count)
            {
                char[]? buffer = null;
                try
                {
                    Span<char> temp = count <= CharStackBufferSize
                        ? stackalloc char[count]
                        : (buffer = ArrayPool<char>.Shared.Rent(count)).AsSpan(0, count);

                    m_Chars.AsSpan(sourceOffset, count).CopyTo(temp);
                    Insert(index, ref MemoryMarshal.GetReference(temp), count);
                }
                finally
                {
                    if (buffer is not null)
                        ArrayPool<char>.Shared.Return(buffer);
                }
            }
        }
    }
}
