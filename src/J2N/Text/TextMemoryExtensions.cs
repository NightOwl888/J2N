// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Text
{
    /// <summary>
    /// Extensions to <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static partial class TextMemoryExtensions
    {
        #region AsSpan

        /// <summary>
        /// Creates a new readonly span over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <returns>The read-only span representation of the string.</returns>
        /// <remarks>Returns <c>default</c> when <paramref name="text"/> is <c>null</c>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer? text)
        {
            if (text is null)
                return default;

#if FEATURE_MEMORYMARSHAL_CREATEREADONLYSPAN && FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
            // J2N: Careful - need to create a local copy because it could move.
            char[] chars = text.m_Chars;
            return MemoryMarshal.CreateReadOnlySpan<char>(ref MemoryMarshal.GetArrayDataReference(chars), text.Length);
#else
            return new ReadOnlySpan<char>(text.m_Chars, 0, text.Length);
#endif
        }

        /// <summary>
        /// Creates a new read-only span over a portion of the target string from
        /// a specified position to the end of the string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <returns>The read-only span representation of the string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="start"/> is less than 0 or greater than <c>text.Length</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer? text, int start)
        {
            if (text == null)
            {
                if (start != 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                return default;
            }

            if ((uint)start > (uint)text.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

#if FEATURE_MEMORYMARSHAL_CREATEREADONLYSPAN && FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
            // J2N: Careful - need to create a local copy because it could move.
            char[] chars = text.m_Chars;
            return MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars),
                (nint)(uint)start /* force zero-extension */), text.Length - start);
#else
            return new ReadOnlySpan<char>(text.m_Chars, start, text.Length - start);
#endif
        }

        /// <summary>
        /// Creates a new read-only span over a portion of the target string from a
        /// specified position for a specified number of characters.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The desired length for the slice.</param>
        /// <returns>The read-only span representation of the string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="start"/>, <paramref name="length"/>, or
        /// <paramref name="start"/> + <paramref name="length"/> is not
        /// in the range of <paramref name="text"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer? text, int start, int length)
        {
            if (text == null)
            {
                if (start != 0 || length != 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                return default;
            }

            if (IntPtr.Size == 8) // 64-bit process
            {
                // See comment in Span<T>.Slice for how this works.
                if ((ulong)(uint)start + (ulong)(uint)length > (ulong)(uint)text.Length)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
            }
            else
            {
                if ((uint)start > (uint)text.Length || (uint)length > (uint)(text.Length - start))
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
            }

#if FEATURE_MEMORYMARSHAL_CREATEREADONLYSPAN && FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
            // J2N: Careful - need to create a local copy because it could move.
            char[] chars = text.m_Chars;
            return MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars),
                (nint)(uint)start /* force zero-extension */), length);
#else
            return new ReadOnlySpan<char>(text.m_Chars, start, length);
#endif
        }

#if FEATURE_INDEX_RANGE

        /// <summary>
        /// Creates a new read-only span over a portion of the
        /// target string from a specified position to the end of the string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="startIndex">The index at which to begin this slice.</param>
        /// <returns>The read-only span representation of the string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less
        /// than 0 or greater than <c>text.Length</c>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer? text, Index startIndex)
        {
            if (text is null)
            {
                if (!startIndex.Equals(Index.Start))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex);
                }

                return default;
            }

            int actualIndex = startIndex.GetOffset(text.Length);
            if ((uint)actualIndex > (uint)text.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex);
            }

#if FEATURE_MEMORYMARSHAL_CREATEREADONLYSPAN && FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
            // J2N: Careful - need to create a local copy because it could move.
            char[] chars = text.m_Chars;
            return MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars),
                (nint)(uint)actualIndex /* force zero-extension */), text.Length - actualIndex);
#else
            return new ReadOnlySpan<char>(text.m_Chars, actualIndex, text.Length - actualIndex);
#endif
        }

        /// <summary>
        /// Creates a new read-only span over a portion of a target string
        /// using the range start and end indexes.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="range">The range that has start and end indexes to use for slicing the string.</param>
        /// <returns>The read-only span representation of the string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="range"/>'s start or end index is not within the bounds of the string.
        /// -or-
        /// <paramref name="range"/>'s start index is greater than its end index.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer? text, Range range)
        {
            if (text is null)
            {
                Index startIndex = range.Start;
                Index endIndex = range.End;

                if (!startIndex.Equals(Index.Start) || !endIndex.Equals(Index.Start))
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
                }

                return default;
            }

            (int start, int length) = range.GetOffsetAndLength(text.Length);

#if FEATURE_MEMORYMARSHAL_CREATEREADONLYSPAN && FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
            // J2N: Careful - need to create a local copy because it could move.
            char[] chars = text.m_Chars;
            return MemoryMarshal.CreateReadOnlySpan<char>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars),
                (nint)(uint)start /* force zero-extension */), length);
#else
            return new ReadOnlySpan<char>(text.m_Chars, start, length);
#endif
        }
#endif
        #endregion AsSpan

        #region AsMemory

        /// <summary>
        /// Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <returns>The read-only character memory representation of the string, or <c>default</c> if
        /// <paramref name="text"/> is <c>null</c>.</returns>
        /// <remarks>Returns default when <paramref name="text"/> is <c>null</c>.</remarks>
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer? text)
        {
            if (text is null)
                return default;

            return new ReadOnlyMemory<char>(text.m_Chars, 0, text.Length);
        }

        /// <summary>
        /// Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <returns>Returns default when <paramref name="text"/> is null.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="start"/> is not in range of <paramref name="text"/>
        /// (<paramref name="start"/> is &lt;0 or &gt;<c>text.Length</c>).
        /// </exception>
        /// <remarks>Returns default when <paramref name="text"/> is <c>null</c>.</remarks>
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer? text, int start)
        {
            if (text == null)
            {
                if (start != 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                return default;
            }

            if ((uint)start > (uint)text.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            return new ReadOnlyMemory<char>(text.m_Chars, start, text.Length - start);
        }

        /// <summary>
        /// Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The desired length for the slice (exclusive).</param>
        /// <returns>The read-only character memory representation of the string, or <c>default</c>
        /// if <paramref name="text"/> is <c>null</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>, <paramref name="length"/>,
        /// or <paramref name="start"/> + <paramref name="length"/> is not in the range of <paramref name="text"/>.</exception>
        /// <remarks>Returns <c>default</c> when <paramref name="text"/> is <c>null</c>.</remarks>
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer? text, int start, int length)
        {
            if (text == null)
            {
                if (start != 0 || length != 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                return default;
            }

            if (IntPtr.Size == 8) // 64-bit process
            {
                // See comment in Span<T>.Slice for how this works.
                if ((ulong)(uint)start + (ulong)(uint)length > (ulong)(uint)text.Length)
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
            }
            else
            {
                if ((uint)start > (uint)text.Length || (uint)length > (uint)(text.Length - start))
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
            }

            return new ReadOnlyMemory<char>(text.m_Chars, start, length);
        }

#if FEATURE_INDEX_RANGE
        /// <summary>
        /// Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="startIndex">The index at which to begin this slice.</param>
        /// <returns>The read-only character memory representation of the string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less
        /// than 0 or greater than <c>text.Length</c>.</exception>
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer? text, Index startIndex)
        {
            if (text == null)
            {
                if (!startIndex.Equals(Index.Start))
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

                return default;
            }

            int actualIndex = startIndex.GetOffset(text.Length);
            if ((uint)actualIndex > (uint)text.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException();

            return new ReadOnlyMemory<char>(text.m_Chars, actualIndex, text.Length - actualIndex);
        }

        /// <summary>
        /// Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="range">The range used to indicate the start and length of the sliced string.</param>
        /// <returns>The read-only character memory representation of the string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="range"/>'s start or end index is not within the bounds of the string.
        /// -or-
        /// <paramref name="range"/>'s start index is greater than its end index.
        /// </exception>
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer? text, Range range)
        {
            if (text == null)
            {
                Index startIndex = range.Start;
                Index endIndex = range.End;

                if (!startIndex.Equals(Index.Start) || !endIndex.Equals(Index.Start))
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

                return default;
            }

            (int start, int length) = range.GetOffsetAndLength(text.Length);
            return new ReadOnlyMemory<char>(text.m_Chars, start, length);
        }
#endif
        #endregion AsMemory
    }
}
