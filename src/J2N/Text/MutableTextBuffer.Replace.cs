// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.CodeGeneration;
using J2N.Collections.Generic;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace J2N.Text
{
    internal partial class MutableTextBuffer
    {
        #region Replace (BCL overloads)

        /// <summary>
        /// Replaces all occurrences of a specified string in this instance with another specified string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, string, string?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(string oldValue, string? newValue) => ReplaceInternal(oldValue, newValue, 0, Length);

        /// <summary>
        /// Replaces all occurrences of a specified span in this instance with another specified span.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, ReadOnlySpan{char}, ReadOnlySpan{char})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue) => ReplaceInternal(oldValue, newValue, 0, Length);

        /// <summary>
        /// Replaces, within a substring of this instance, all occurrences of a specified string with another specified string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, string, string?, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(string oldValue, string? newValue, int startIndex, int count)
        {
            if (oldValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.oldValue);

            int currentLength = Length;
            if ((uint)startIndex > (uint)currentLength)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0 || startIndex > currentLength - count)
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            if (oldValue == string.Empty)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_EmptySpan, ExceptionArgument.oldValue);

            ReplaceCore(oldValue, newValue, startIndex, count);
        }

        /// <summary>
        /// Replaces all instances of one read-only character span with another in a substring of this builder.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, ReadOnlySpan{char}, ReadOnlySpan{char}, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)startIndex > (uint)currentLength)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0 || startIndex > currentLength - count)
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            if (oldValue.IsEmpty)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_EmptySpan, ExceptionArgument.oldValue);

            ReplaceCore(oldValue, newValue, startIndex, count);
        }

        private void ReplaceCore(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
        {
            Debug.Assert((uint)startIndex <= (uint)Length);
            Debug.Assert(count >= 0);
            Debug.Assert(startIndex <= Length - count);
            Debug.Assert(!oldValue.IsEmpty);

            var replacements = new ValueListBuilder<int>(stackalloc int[128]);

            try
            {
                int searchStart = startIndex;
                int searchEnd = startIndex + count; // Safe: validated above that startIndex + count <= currentLength.

                while (searchStart <= searchEnd - oldValue.Length)
                {
                    int found = m_Chars.AsSpan(searchStart, searchEnd - searchStart).IndexOf(oldValue);

                    if (found < 0)
                        break;

                    found += searchStart;
                    replacements.Append(found);
                    searchStart = found + oldValue.Length;
                }

                if (replacements.Length == 0)
                    return;

                int deltaPerMatch = newValue.Length - oldValue.Length;

                long longDelta = (long)deltaPerMatch * replacements.Length;
                int delta = (int)longDelta;

                if (delta != longDelta)
                    throw new OutOfMemoryException();

                if ((uint)delta + (uint)m_Position <= (uint)m_Chars.Length)
                {
                    if (oldValue.Overlaps(m_Chars) || newValue.Overlaps(m_Chars))
                    {
                        ReplaceAllOverlapping(ref replacements, delta, oldValue, newValue, startIndex, count);
                        return;
                    }

                    ReplaceAllCore(ref replacements, delta, oldValue, newValue, startIndex, count);
                }
                else
                {
                    ReplaceAllWithExpansion(ref replacements, delta, oldValue, newValue, startIndex, count);
                }
            }
            finally
            {
                replacements.Dispose();
            }
        }

        private void ReplaceAllCore(scoped ref ValueListBuilder<int> replacements, int delta, scoped ReadOnlySpan<char> oldValue, scoped ReadOnlySpan<char> newValue, int startIndex, int count)
        {
            Debug.Assert((uint)(m_Position + delta) <= (uint)m_Chars.Length);

            int originalLength = m_Position;
            int finalLength = originalLength + delta;

            if (delta < 0)
            {
                ReplaceAllLeftToRight(m_Chars, m_Chars, ref replacements, oldValue, newValue, originalLength, startIndex, count);
                m_Position = finalLength;
                return;
            }

            Span<char> chars = m_Chars;

            // Safe: validated by argument checking.
            int rangeEnd = startIndex + count;

            int sourceEnd = originalLength;
            int destinationEnd = finalLength;

            //
            // Copy suffix.
            //
            int suffixLength = originalLength - rangeEnd;

            if (suffixLength > 0 && delta != 0)
            {
                sourceEnd -= suffixLength;
                destinationEnd -= suffixLength;

                if (sourceEnd != destinationEnd)
                {
                    chars.Slice(sourceEnd, suffixLength)
                         .CopyTo(chars.Slice(destinationEnd));
                }
            }

            //
            // Replay replacements from right to left.
            //
            for (int i = replacements.Length - 1; i >= 0; i--)
            {
                int match = replacements[i];
                // Safe: every match satisfies
                // match + oldValue.Length <= rangeEnd.
                int sourceAfterMatch = match + oldValue.Length;

                //
                // Copy text between this match and the next one (or the suffix).
                //
                int betweenLength = sourceEnd - sourceAfterMatch;

                if (betweenLength > 0)
                {
                    int destinationAfterCopy = destinationEnd - betweenLength;

                    if (delta != 0)
                    {
                        chars.Slice(sourceAfterMatch, betweenLength)
                             .CopyTo(chars.Slice(destinationAfterCopy));
                    }

                    destinationEnd = destinationAfterCopy;
                }

                //
                // Write replacement.
                //
                destinationEnd -= newValue.Length;

                newValue.CopyTo(chars.Slice(destinationEnd));

                //
                // Continue processing the text preceding this match.
                //
                sourceEnd = match;
            }

            //
            // Copy prefix.
            //
            int prefixLength = sourceEnd - startIndex;

            if (prefixLength > 0)
            {
                int destinationPrefix = destinationEnd - prefixLength;

                if (delta != 0)
                {
                    chars.Slice(startIndex, prefixLength)
                         .CopyTo(chars.Slice(destinationPrefix));
                }

                destinationEnd = destinationPrefix;
            }

            Debug.Assert(destinationEnd == startIndex);

            m_Position = finalLength;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ReplaceAllOverlapping(ref ValueListBuilder<int> replacements, int delta, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
        {
            bool oldOverlaps = oldValue.Overlaps(m_Chars);
            bool newOverlaps = newValue.Overlaps(m_Chars);

            Debug.Assert(oldOverlaps || newOverlaps);

            if (oldOverlaps)
            {
                if (newOverlaps)
                {
                    ReplaceWithSnapshots(ref replacements, delta, oldValue, newValue, startIndex, count);
                }
                else
                {
                    ReplaceWithSnapshotOfOld(ref replacements, delta, oldValue, newValue, startIndex, count);
                }
            }
            else
            {
                ReplaceWithSnapshotOfNew(ref replacements, delta, oldValue, newValue, startIndex, count);
            }

            void ReplaceWithSnapshotOfOld(ref ValueListBuilder<int> replacements, int delta, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
            {
                char[]? buffer = null;

                try
                {
                    Span<char> temp = oldValue.Length <= CharStackBufferSize
                        ? stackalloc char[oldValue.Length]
                        : (buffer = LocalArrayPool.Instance.Rent(oldValue.Length)).AsSpan(0, oldValue.Length);

                    oldValue.CopyTo(temp);

                    ReplaceAllCore(ref replacements, delta, temp, newValue, startIndex, count);
                }
                finally
                {
                    if (buffer is not null)
                        LocalArrayPool.Instance.Return(buffer);
                }
            }

            void ReplaceWithSnapshotOfNew(ref ValueListBuilder<int> replacements, int delta, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
            {
                char[]? buffer = null;

                try
                {
                    Span<char> temp = newValue.Length <= CharStackBufferSize
                        ? stackalloc char[newValue.Length]
                        : (buffer = LocalArrayPool.Instance.Rent(newValue.Length)).AsSpan(0, newValue.Length);

                    newValue.CopyTo(temp);

                    ReplaceAllCore(ref replacements, delta, oldValue, temp, startIndex, count);
                }
                finally
                {
                    if (buffer is not null)
                        LocalArrayPool.Instance.Return(buffer);
                }
            }

            void ReplaceWithSnapshots(ref ValueListBuilder<int> replacements, int delta, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
            {
                char[]? oldBuffer = null;
                char[]? newBuffer = null;

                try
                {
                    Span<char> oldTemp = oldValue.Length <= CharStackBufferSize
                        ? stackalloc char[oldValue.Length]
                        : (oldBuffer = LocalArrayPool.Instance.Rent(oldValue.Length)).AsSpan(0, oldValue.Length);

                    Span<char> newTemp = newValue.Length <= CharStackBufferSize
                        ? stackalloc char[newValue.Length]
                        : (newBuffer = LocalArrayPool.Instance.Rent(newValue.Length)).AsSpan(0, newValue.Length);

                    oldValue.CopyTo(oldTemp);
                    newValue.CopyTo(newTemp);

                    ReplaceAllCore(ref replacements, delta, oldTemp, newTemp, startIndex, count);
                }
                finally
                {
                    if (oldBuffer is not null)
                        LocalArrayPool.Instance.Return(oldBuffer);

                    if (newBuffer is not null)
                        LocalArrayPool.Instance.Return(newBuffer);
                }
            }
        }

        private void ReplaceAllWithExpansion(ref ValueListBuilder<int> replacements, int delta, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
        {
            Debug.Assert((uint)delta + (uint)m_Position > (uint)m_Chars.Length);

            int originalLength = m_Position;
            int finalLength = originalLength + delta;

            if ((uint)finalLength > (uint)m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.requiredLength, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
            }

            char[] oldArray = m_Chars;
            char[] newArray = allocator.Allocate(CalculateNewArrayLength(delta));

            ReadOnlySpan<char> source = oldArray;
            Span<char> destination = newArray;

            //
            // Copy prefix before replacement range.
            //
            if (startIndex > 0)
            {
                source.Slice(0, startIndex)
                    .CopyTo(destination);
            }

            ReplaceAllLeftToRight(source, destination, ref replacements, oldValue, newValue, originalLength, startIndex, count);

            m_Chars = newArray;
            m_Position = finalLength;
            allocator.Return(oldArray);
        }

        private static void ReplaceAllLeftToRight(scoped ReadOnlySpan<char> source, scoped Span<char> destination,
            scoped ref ValueListBuilder<int> replacements, scoped ReadOnlySpan<char> oldValue, scoped ReadOnlySpan<char> newValue,
            int originalLength, int startIndex, int count)
        {
            Debug.Assert(startIndex >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert((uint)startIndex + (uint)count <= (uint)originalLength);

            int sourceIndex = startIndex;
            int destinationIndex = startIndex;

            //
            // Replay replacements from left to right.
            //
            for (int i = 0; i < replacements.Length; i++)
            {
                int match = replacements[i];

                //
                // Copy text before this match.
                //
                int copyLength = match - sourceIndex;

                if (copyLength > 0)
                {
                    source.Slice(sourceIndex, copyLength)
                          .CopyTo(destination.Slice(destinationIndex));

                    sourceIndex += copyLength;
                    destinationIndex += copyLength;
                }

                //
                // Write replacement.
                //
                newValue.CopyTo(destination.Slice(destinationIndex));
                destinationIndex += newValue.Length;

                //
                // Skip matched text.
                //
                sourceIndex += oldValue.Length;
            }

            //
            // Copy the remaining text in the replacement range.
            //
            int rangeEnd = startIndex + count;

            if (sourceIndex < rangeEnd)
            {
                int remaining = rangeEnd - sourceIndex;

                source.Slice(sourceIndex, remaining)
                      .CopyTo(destination.Slice(destinationIndex));

                destinationIndex += remaining;
            }

            //
            // Copy suffix.
            //
            if (rangeEnd < originalLength)
            {
                source.Slice(rangeEnd, originalLength - rangeEnd)
                      .CopyTo(destination.Slice(destinationIndex));
            }
        }

        /// <summary>
        /// Replaces all occurrences of a specified char in this instance with a specified char.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, char, char)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(char oldChar, char newChar)
        {
            ReplaceInternal(oldChar, newChar, 0, Length);
        }

        /// <summary>
        /// Replaces, within a substring of this instance, all occurrences of a specified character with another specified character.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, char, char, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(char oldChar, char newChar, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)startIndex > (uint)currentLength)
            {
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            }

            if (count < 0 || startIndex > currentLength - count)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(count, ExceptionArgument.count);
            }

            Span<char> span = m_Chars.AsSpan(startIndex, count);
            span.Replace(oldChar, newChar);

            //AssertInvariants();
        }

        #endregion Replace (BCL overloads)

        #region Replace (JDK overloads)

        /// <summary>
        /// Replaces the specified substring in this builder with the specified string.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, int, int, string)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(int startIndex, int count, string newValue)
        {
            if (newValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.newValue);
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            ReplaceCore(startIndex, count, newValue);
        }

        /// <summary>
        /// Replaces the specified substring in this builder with the specified span.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, int, int, ReadOnlySpan{char})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(int startIndex, int count, ReadOnlySpan<char> newValue)
        {
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            ReplaceCore(startIndex, count, newValue);
        }

        internal void ReplaceCore(int startIndex, int count, ReadOnlySpan<char> newValue)
        {
            Debug.Assert(startIndex >= 0 && startIndex <= m_Position);
            Debug.Assert(count >= 0);

            // Clamp to end of buffer (Harmony/JDK behavior)
            int end = count > m_Position - startIndex
                ? m_Position
                : startIndex + count; // Overflow not possible here

            int replacedLength = end - startIndex;

            // NOTE: If newValue is Empty, the Overlaps() check always fails.
            if (m_Chars.AsSpan().Overlaps(newValue, out int sourceOffset))
            {
                ReplaceCoreOverlapping(startIndex, count, sourceOffset, newValue.Length, replacedLength, end);
                return;
            }

            int delta = newValue.Length - replacedLength;
            if (delta > 0)
            {
                // Need more space.
                //
                // Insert the additional space immediately after the replaced region.
                // This preserves the replacement area while shifting only the tail.
                MakeRoom(end, delta);
            }
            else if (delta < 0)
            {
                // Need less space.
                //
                // Remove only the excess characters after the replacement area.
                RemoveCore(startIndex + newValue.Length, -delta);
            }

            // Overwrite the replacement area.
            if (!newValue.IsEmpty)
            {
                newValue.CopyTo(m_Chars.AsSpan(startIndex));
            }
        }

        private void ReplaceCoreOverlapping(int startIndex, int count, int sourceOffset, int sourceLength, int replacedLength, int end)
        {
            Debug.Assert(sourceLength > 0);
            Debug.Assert(startIndex <= int.MaxValue - sourceLength);

            // Fast path: Exact self-replacement (no-op)
            if (sourceOffset == startIndex && sourceLength == replacedLength)
            {
                return;
            }

            int delta = sourceLength - replacedLength;

            // Common case: Source entirely before the replacement region
            if ((uint)sourceOffset + (uint)sourceLength <= startIndex)
            {
                if (delta > 0)
                {
                    MakeRoom(end, delta);
                }
                else if (delta < 0)
                {
                    RemoveCore(startIndex + sourceLength, -delta);
                }

                // NOTE: We derive a new span here because MakeRoom() could replace our backing buffer.
                // We rely on the fact that MakeRoom() will copy the head chars to the same location as the
                // original buffer.
                m_Chars.AsSpan(sourceOffset, sourceLength).CopyTo(m_Chars.AsSpan(startIndex));
                return;
            }

            char[]? arrayToReturn = null;
            try
            {
                Span<char> temp = sourceLength <= CharStackBufferSize
                    ? stackalloc char[sourceLength]
                    : (arrayToReturn = LocalArrayPool.Instance.Rent(sourceLength)).AsSpan(0, sourceLength);

                m_Chars.AsSpan(sourceOffset, sourceLength)
                    .CopyTo(temp);

                if (delta > 0)
                {
                    MakeRoom(end, delta);
                }
                else if (delta < 0)
                {
                    RemoveCore(startIndex + sourceLength, -delta);
                }

                temp.CopyTo(m_Chars.AsSpan(startIndex));
            }
            finally
            {
                if (arrayToReturn is not null)
                    LocalArrayPool.Instance.Return(arrayToReturn);
            }
        }

        /// <summary>
        /// Replaces the specified substring in this builder with the specified span.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, int, int, StringBuilder)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(int startIndex, int count, StringBuilder newValue)
        {
            if (newValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.newValue);
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);


            // Clamp to end of buffer (Harmony/JDK behavior)
            int end = count > m_Position - startIndex
                ? m_Position
                : startIndex + count; // Overflow not possible here

            int replacedLength = end - startIndex;
            int delta = newValue.Length - replacedLength;
            if (delta > 0)
            {
                // Need more space.
                //
                // Insert the additional space immediately after the replaced region.
                // This preserves the replacement area while shifting only the tail.
                MakeRoom(end, delta);
            }
            else if (delta < 0)
            {
                // Need less space.
                //
                // Remove only the excess characters after the replacement area.
                RemoveCore(startIndex + newValue.Length, -delta);
            }

            // Overwrite the replacement area.
            if (newValue.Length > 0)
            {
                newValue.CopyTo(0, m_Chars, startIndex, newValue.Length);
            }
        }

        /// <summary>
        /// Replaces the specified substring in this builder with the specified span.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="J2N.MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, int, int, ICharSequence)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(int startIndex, int count, ICharSequence newValue)
        {
            if (newValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.newValue);
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (newValue is ISpannable<char> spannable)
            {
                ReplaceCore(startIndex, count, spannable.AsSpan());
                return;
            }

            // Clamp to end of buffer (Harmony/JDK behavior)
            int end = count > m_Position - startIndex
                ? m_Position
                : startIndex + count; // Overflow not possible here

            int replacedLength = end - startIndex;
            int newValueLength = newValue.Length;

            char[]? arrayToReturn = null;
            try
            {
                // If the source doesn't implement ISpannable<char>, we have no way to test
                // whether the implmentation overlaps our memory. So, the only safe approach
                // is to always take a snapshot prior to moving any memory.

                Span<char> temp = newValueLength <= CharStackBufferSize
                    ? stackalloc char[newValueLength]
                    : (arrayToReturn = LocalArrayPool.Instance.Rent(newValueLength)).AsSpan(0, newValueLength);

                if (newValue is ISpanCopyable<char> spanCopyable)
                {
                    spanCopyable.CopyTo(0, temp, newValueLength);
                }
                else if (arrayToReturn is not null && newValue is ICopyable<char> copyable)
                {
                    copyable.CopyTo(0, arrayToReturn, 0, newValueLength);
                }
                else
                {
                    for (int i = 0; i < newValueLength; i++)
                    {
                        temp[i] = newValue[i];
                    }
                }

                int delta = newValueLength - replacedLength;

                if (delta > 0)
                {
                    MakeRoom(end, delta);
                }
                else if (delta < 0)
                {
                    RemoveCore(startIndex + newValueLength, -delta);
                }

                if (newValueLength > 0)
                {
                    temp.CopyTo(m_Chars.AsSpan(startIndex));
                }
            }
            finally
            {
                if (arrayToReturn is not null)
                    LocalArrayPool.Instance.Return(arrayToReturn);
            }
        }

        #endregion Replace (JDK overloads)
    }
}
