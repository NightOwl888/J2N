// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.CodeGeneration;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    internal partial class MutableTextBuffer
    {
        #region Append char repeating

        /// <summary>
        /// Appends a specified number of copies of the string representation of a Unicode character to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, char, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(char value, int repeatCount)
        {
            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            if (repeatCount == 0)
            {
                return;
            }

            AppendCore(value, repeatCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendCore(char value, int repeatCount)
        {
            char[] chars = m_Chars;
            int pos = m_Position;

            // Try to fit the whole repeatCount in the current chunk
            // Use the same check as Span<T>.Slice for 64-bit so it can be folded
            // Since repeatCount can't be negative, there's no risk for it to overflow on 32 bit
            if (((nuint)(uint)pos + (nuint)(uint)repeatCount) <= (nuint)(uint)chars.Length)
            {
                chars.AsSpan(pos, repeatCount).Fill(value);
                m_Position += repeatCount;
            }
            else
            {
                AppendWithExpansion(value, repeatCount);
            }

            //AssertInvariants();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendWithExpansion(char value, int repeatCount)
        {
            Debug.Assert(repeatCount > 0, "Invalid length; should have been validated by caller.");

            // Check if the repeatCount will put us over m_MaxCapacity
            if ((uint)(repeatCount + Length) > (uint)m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(repeatCount, ExceptionArgument.repeatCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
            }

            Grow(repeatCount);
            m_Chars.AsSpan(m_Position, repeatCount).Fill(value);
            m_Position += repeatCount;
        }

        #endregion Append char repeating

        #region AppendCodePoint repeating

        /// <summary>
        /// Appends the string representation of the <paramref name="codePoint"/>.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendCodePoint{TBuilder}(TBuilder, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendCodePointInternal(int codePoint, int repeatCount) // Coverage for the JDK
        {
            uint value = (uint)codePoint;
            if (!UnicodeUtility.IsValidCodePoint(value))
                ThrowHelper.ThrowArgumentOutOfRange_InvalidCodePoint(codePoint);
            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            if (repeatCount == 0)
                return;

            if (UnicodeUtility.IsBmpCodePoint(value))
            {
                AppendCore((char)value, repeatCount);
                return;
            }

            AppendSupplementaryRepeated(value, repeatCount);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendSupplementaryRepeated(uint scalar, int repeatCount)
        {
            Debug.Assert(UnicodeUtility.GetUtf16SequenceLength(scalar) == 2);

            int pos = m_Position;
            // Ensure we don't append more chars than we can hold, and we don't
            // have any integer overflow in our new length.
            long appendingChars = (long)repeatCount * 2;
            if (pos > m_Chars.Length - appendingChars)
            {
                // Check if the count will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the StringBuilder.
                long newLength = pos + appendingChars;
                if (newLength > m_MaxCapacity || newLength < appendingChars)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        repeatCount,
                        ExceptionArgument.repeatCount,
                        ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                Grow((int)appendingChars);
            }

            int destinationLength = (int)appendingChars;
            Span<char> destination = m_Chars.AsSpan(pos, destinationLength);

            UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(
                scalar,
                out destination[0],
                out destination[1]);

            int copied = 2;

            while (copied < destinationLength)
            {
                int copyLength = Math.Min(copied, destinationLength - copied);

                destination.Slice(0, copyLength)
                    .CopyTo(destination.Slice(copied));

                copied += copyLength;
            }

            m_Position = pos + destinationLength;
        }

        #endregion

        #region Insert string repeating

        /// <summary>
        /// Inserts one or more copies of a specified string into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, string?, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, string? value, int repeatCount) => InsertInternal(index, value.AsSpan(), repeatCount);

        #endregion Insert string repeating

        #region Insert ReadOnlySpan<char> repeating

        /// <summary>
        /// Inserts one or more copies of a specified span into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ReadOnlySpan{char}, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ReadOnlySpan<char> value, int repeatCount) // J2N: Made public to match ValueStringBuilder API
        {
            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            }

            if (value.IsEmpty || repeatCount == 0)
            {
                return;
            }

            // Ensure we don't insert more chars than we can hold, and we don't
            // have any integer overflow in our new length.
            long insertingChars = (long)value.Length * repeatCount;
            if (insertingChars > MaxCapacity - m_Position)
            {
                throw new OutOfMemoryException();
            }
            Debug.Assert(insertingChars + m_Position < int.MaxValue);

            int destinationLength = (int)insertingChars;
            if (destinationLength == 0)
                return;

            if (m_Chars.AsSpan().Overlaps(value))
            {
                InsertOverlappingRepeated(index, value, destinationLength, repeatCount);
                return;
            }

            InsertRepeated(index, value, destinationLength);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InsertOverlappingRepeated(int index, ReadOnlySpan<char> value, int destinationLength, int repeatCount)
        {
            char[]? buffer = null;
            try
            {
                int valueLength = value.Length;
                Span<char> temp = valueLength <= CharStackBufferSize
                    ? stackalloc char[valueLength]
                    : (buffer = ArrayPool<char>.Shared.Rent(valueLength)).AsSpan(0, valueLength);

                value.CopyTo(temp);
                InsertRepeated(index, temp, destinationLength);
            }
            finally
            {
                if (buffer is not null)
                    ArrayPool<char>.Shared.Return(buffer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertRepeated(int index, ReadOnlySpan<char> value, int destinationLength)
        {
            MakeRoom(index, destinationLength);

            Span<char> destination =
                m_Chars.AsSpan(index, destinationLength);

            // We only copy from the source once. The remainder of the copies
            // are from destination to destination. This allows for more opportunities
            // for the BCL to optimize the copy.
            value.CopyTo(destination);

            int copied = value.Length;

            while (copied < destinationLength)
            {
                int remaining = destinationLength - copied;
                int copyLength = copied < remaining ? copied : remaining;

                destination.Slice(0, copyLength)
                    .CopyTo(destination.Slice(copied));

                copied += copyLength;
            }
        }

        #endregion Insert ReadOnlySpan<char> repeating

        #region Insert StringBuilder repeating

        /// <summary>
        /// Inserts one or more copies of a specified string builder into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, StringBuilder?, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, StringBuilder? value, int repeatCount)
        {
            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            }

            if (value is null || repeatCount == 0)
            {
                return;
            }

            // Ensure we don't insert more chars than we can hold, and we don't
            // have any integer overflow in our new length.
            long insertingChars = (long)value.Length * repeatCount;
            if (insertingChars > MaxCapacity - m_Position)
            {
                throw new OutOfMemoryException();
            }
            Debug.Assert(insertingChars + m_Position < int.MaxValue);

            int destinationLength = (int)insertingChars;

            if (destinationLength == 0)
                return;

            MakeRoom(index, destinationLength);

            int copied = value.Length;

            // We only copy from the source once. The remainder of the copies
            // are from destination to destination. This allows for more opportunities
            // for the BCL to optimize the copy.
            value.CopyTo(0, m_Chars, index, copied);

            Span<char> destination =
                m_Chars.AsSpan(index, destinationLength);

            while (copied < destinationLength)
            {
                int remaining = destinationLength - copied;
                int copyLength = copied < remaining ? copied : remaining;

                destination.Slice(0, copyLength)
                    .CopyTo(destination.Slice(copied));

                copied += copyLength;
            }
        }

        #endregion Insert StringBuilder repeating

        #region Insert ICharSequence repeating

        /// <summary>
        /// Inserts one or more copies of a specified character sequence into this instance at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="J2N.MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ICharSequence, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ICharSequence? value, int repeatCount)
        {
            if (value is ISpannable<char> spannable)
            {
                InsertInternal(index, spannable.AsSpan(), repeatCount);
                return;
            }

            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            }

            if (value is null || value.Length == 0 || repeatCount == 0)
            {
                return;
            }

            // Ensure we don't insert more chars than we can hold, and we don't
            // have any integer overflow in our new length.
            long insertingChars = (long)value.Length * repeatCount;
            if (insertingChars > MaxCapacity - m_Position)
            {
                throw new OutOfMemoryException();
            }
            Debug.Assert(insertingChars + m_Position < int.MaxValue);

            int destinationLength = (int)insertingChars;
            if (destinationLength == 0)
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

                InsertRepeated(index, temp, destinationLength);
            }
            finally
            {
                if (arrayToReturn is not null)
                    allocator.Return(arrayToReturn);
            }
        }

        #endregion Insert ICharSequence repeating
    }
}
