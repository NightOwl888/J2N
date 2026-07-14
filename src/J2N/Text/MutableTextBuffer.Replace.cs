// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.CodeGeneration;
using J2N.Collections.Generic;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace J2N.Text
{
    public partial class MutableTextBuffer
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
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(string oldValue, string? newValue, int startIndex, int count)
        {
            if (oldValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.oldValue);
            ReplaceInternal(oldValue.AsSpan(), newValue.AsSpan(), startIndex, count);
        }

        /// <summary>
        /// Replaces all instances of one read-only character span with another in a substring of this builder.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, ReadOnlySpan{char}, ReadOnlySpan{char}, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void ReplaceInternal(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)startIndex > (uint)currentLength)
            {
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            }
            if (count < 0 || startIndex > currentLength - count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            }
            if (oldValue.Length == 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_EmptySpan, ExceptionArgument.oldValue);
            }

            var replacements = new ValueListBuilder<int>(stackalloc int[128]); // A list of replacement positions in a chunk to apply
            try
            {
                // Starting point.
                int indexInChunk = startIndex;
                while (count > 0)
                {
                    //Debug.Assert(chunk != null, "chunk was null in replace");

                    // While the remaining search space is at least as large as the old value being replaced,
                    // find all occurrences of it contained entirely within the chunk. We stop searching
                    // once we're within oldValue.Length from the end of the chunk (or count limit), at which point
                    // we need to consider a value that bridges between two chunks.
                    ReadOnlySpan<char> remainingChunk = m_Chars.AsSpan(indexInChunk, Math.Min(m_Position - indexInChunk, count));
                    while (oldValue.Length <= remainingChunk.Length)
                    {
                        // Find the next match.
                        int foundPos = remainingChunk.IndexOf(oldValue);
                        if (foundPos >= 0)
                        {
                            // We found one.  Add it as a location for the replacement.
                            indexInChunk += foundPos;
                            replacements.Append(indexInChunk);

                            // Move ahead to the next location.
                            remainingChunk = remainingChunk.Slice(foundPos + oldValue.Length);
                            indexInChunk += oldValue.Length;
                            count -= foundPos + oldValue.Length;

                            // If after accounting for moving past the match our count has
                            // gone to 0, break out to stop searching.
                            Debug.Assert(count >= 0, "count should never go negative");
                            if (count == 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            // No match found. Reposition to one character beyond the last starting
                            // location searched, which will be oldValue.Length - 1 from the end.
                            // Then break out so that we can start the cross-chunk matching from that location.
                            int move = remainingChunk.Length - (oldValue.Length - 1);
                            indexInChunk += move;
                            count -= move;
                            break;
                        }
                    }

                    Debug.Assert(oldValue.Length > Math.Min(count, m_Position - indexInChunk),
                        $"oldValue.Length = {oldValue.Length}, m_Position - indexInChunk = {m_Position - indexInChunk}, count == {count}");

                    // Now do the more complicated cross-chunk matching.
                    while (indexInChunk < m_Position && count > 0)
                    {
                        if (StartsWith(indexInChunk, count, oldValue))
                        {
                            replacements.Append(indexInChunk);
                            indexInChunk += oldValue.Length;
                            count -= oldValue.Length;
                        }
                        else
                        {
                            indexInChunk++;
                            --count;
                        }
                    }

                    // We've either fully explored the chunk or we've reached our count limit.
                    Debug.Assert(indexInChunk >= m_Position || count == 0,
                        $"indexInChunk = {indexInChunk}, m_Position == {m_Position}, count == {count}");

                    // Replacing mutates the blocks, so we need to convert to a logical index and back afterwards.
                    int index = indexInChunk; // + chunk.m_ChunkOffset;

                    // Apply any replacements we accumulated.
                    if (replacements.Length != 0)
                    {
                        // Perform all replacements, and adjust the logical index if the new and old values
                        // have different lengths, such that the replacements would have impacted it.
                        ReplaceAll(replacements.AsSpan(), oldValue.Length, newValue);
                        index += (newValue.Length - oldValue.Length) * replacements.Length;
                        replacements.Length = 0;
                    }

                    //chunk = FindChunkForIndex(index);
                    //indexInChunk = index - chunk.m_ChunkOffset;
                    //Debug.Assert(chunk != null || count == 0, "Chunks ended prematurely!");

                    indexInChunk = index - m_Position;
                }
            }
            finally
            {
                replacements.Dispose();
            }

            //AssertInvariants();
        }

        /// <summary>
        /// Replaces all occurrences of a specified char in this instance with a specified char.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Replace{TBuilder}(TBuilder, char, char)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
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

        private void ReplaceAll(ReadOnlySpan<int> replacements, int removeCount, ReadOnlySpan<char> value) // Based on ReplaceAllInChunk()
        {
            Debug.Assert(!replacements.IsEmpty);

            // calculate the total amount of extra space or space needed for all the replacements.
            long longDelta = (value.Length - removeCount) * (long)replacements.Length;
            int delta = (int)longDelta;
            if (delta != longDelta)
            {
                throw new OutOfMemoryException();
            }

            int targetIndex = replacements[0];

            // Make the room needed for all the new characters if needed.
            if (delta > 0)
            {
                MakeRoom(targetIndex, delta);
            }

            char[] chars = m_Chars;
            // We made certain that characters after the insertion point are not moved,
            int i = 0;
            while (true)
            {
                // Copy in the new string for the ith replacement
                ReplaceInPlace(ref targetIndex, ref MemoryMarshal.GetReference(value), value.Length);
                int gapStart = replacements[i] + removeCount;
                i++;
                if ((uint)i >= replacements.Length)
                {
                    break;
                }

                int gapEnd = replacements[i];
                Debug.Assert(gapStart < chars.Length, "gap starts at end of buffer.  Should not happen");
                Debug.Assert(gapStart <= gapEnd, "negative gap size");
                Debug.Assert(gapEnd <= m_Position, "gap too big");
                if (delta != 0)     // can skip the sliding of gaps if source an target string are the same size.
                {
                    // Copy the gap data between the current replacement and the next replacement
                    ReplaceInPlace(ref targetIndex, ref chars[gapStart], gapEnd - gapStart);
                }
                else
                {
                    targetIndex += gapEnd - gapStart;
                    Debug.Assert(targetIndex <= m_Position, "gap not in chunk");
                }
            }

            // Remove extra space if necessary.
            if (delta < 0)
            {
                RemoveCore(targetIndex, -delta);
            }
        }

        private bool StartsWith(int index, int count, ReadOnlySpan<char> value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (count == 0)
                {
                    return false;
                }

                if (value[i] != m_Chars[index])
                {
                    return false;
                }

                index++;
                --count;
            }

            return true;
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
            Debug.Assert(startIndex <= int.MaxValue - sourceLength);

            // Fast path: Exact self-replacement (no-op)
            if (sourceOffset == startIndex && sourceLength == replacedLength)
            {
                return;
            }

            // Common case: Source entirely before the replacement region
            if ((uint)sourceOffset + (uint)sourceLength <= startIndex)
            {
                ReadOnlySpan<char> source = m_Chars.AsSpan(sourceOffset, sourceLength);

                int delta = sourceLength - replacedLength;

                if (delta > 0)
                {
                    MakeRoom(end, delta);
                }
                else if (delta < 0)
                {
                    RemoveCore(startIndex + sourceLength, -delta);
                }

                if (sourceLength > 0)
                {
                    source.CopyTo(m_Chars.AsSpan(startIndex));
                }
                return;
            }

            char[]? arrayToReturn = null;
            try
            {
                Span<char> temp = sourceLength <= CharStackBufferSize
                    ? stackalloc char[sourceLength]
                    : (arrayToReturn = allocator.Allocate(sourceLength)).AsSpan(0, sourceLength);

                m_Chars.AsSpan(sourceOffset, sourceLength)
                    .CopyTo(temp);

                int delta = sourceLength - replacedLength;

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
                    allocator.Return(arrayToReturn);
            }
        }

        #endregion Replace (JDK overloads)
    }
}
