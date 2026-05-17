using J2N.Runtime.InteropServices;
using J2N.Text;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N
{
    /// <summary>
    /// Extensions to <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static partial class MemoryExtensions
    {
        #region AsSpan

        /// <summary>
        /// Creates a new readonly span over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <returns>The read-only span representation of the string.</returns>
        /// <remarks>Returns <c>default</c> when <paramref name="text"/> is <c>null</c>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer text)
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
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer text, int start)
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
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer text, int start, int length)
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
                if ((uint) start > (uint) text.Length || (uint) length > (uint)(text.Length - start))
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
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer text, Index startIndex)
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
        public static ReadOnlySpan<char> AsSpan(this MutableTextBuffer text, Range range)
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
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer text)
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
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer text, int start)
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
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer text, int start, int length)
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
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer text, Index startIndex)
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
        public static ReadOnlyMemory<char> AsMemory(this MutableTextBuffer text, Range range)
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

        #region IndexOf

        /// <summary>
        /// Searches for the specified sequence and returns the index of its first occurrence.
        /// <para/>
        /// This method simply cascades the call to <see cref="System.MemoryExtensions.IndexOf{T}(ReadOnlySpan{T}, T)"/>.
        /// Its purpose is to avoid the overhead of casting to an <see cref="int"/> and back to <see cref="char"/> when
        /// calling <see cref="IndexOf(ReadOnlySpan{char}, int)"/> with a <see cref="char"/>. If this method did not exist,
        /// the compiler would always choose <see cref="IndexOf(ReadOnlySpan{char}, int)"/> instead of
        /// <see cref="System.MemoryExtensions.IndexOf{T}(ReadOnlySpan{T}, T)"/> when the type of <c>T</c> is <see cref="char"/>.
        /// </summary>
        /// <param name="text">The span to search.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>The index of the occurrence of the value in the span. If not found, returns -1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(this ReadOnlySpan<char> text, char value)
            => System.MemoryExtensions.IndexOf(text, value);

        /// <summary>
        /// Searches for the specified sequence and returns the index of its first occurrence.
        /// <para/>
        /// This method simply cascades the call to <see cref="System.MemoryExtensions.IndexOf{T}(Span{T}, T)"/>.
        /// Its purpose is to avoid the overhead of casting to an <see cref="int"/> and back to <see cref="char"/> when
        /// calling <see cref="IndexOf(Span{char}, int)"/> with a <see cref="char"/>. If this method did not exist,
        /// the compiler would always choose <see cref="IndexOf(Span{char}, int)"/> instead of
        /// <see cref="System.MemoryExtensions.IndexOf{T}(Span{T}, T)"/> when the type of <c>T</c> is <see cref="char"/>.
        /// </summary>
        /// <param name="text">The span to search.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>The index of the occurrence of the value in the span. If not found, returns -1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(this Span<char> text, char value)
            => System.MemoryExtensions.IndexOf(text, value);

        /// <summary>
        /// Returns the index within this string of the first occurrence of
        /// the specified character. If a character with value
        /// <paramref name="codePoint"/> occurs in the character sequence represented by
        /// this <see cref="ReadOnlySpan{Char}"/> object, then the index (in Unicode
        /// code units) of the first such occurrence is returned. For
        /// values of <paramref name="codePoint"/> in the range from 0 to 0xFFFF
        /// (inclusive), this is the smallest value <i>k</i> such that:
        /// <code>
        ///     this[(<i>k</i>] == <paramref name="codePoint"/>
        /// </code>
        /// is true. For other values of <paramref name="codePoint"/>, it is the
        /// smallest value <i>k</i> such that:
        /// <code>
        ///     this.CodePointAt(<i>k</i>) == <paramref name="codePoint"/>
        /// </code>
        /// is true. In either case, if no such character occurs in this
        /// string, then <c>-1</c> is returned.
        /// </summary>
        /// <param name="text">This <see cref="ReadOnlySpan{Char}"/>.</param>
        /// <param name="codePoint">A character (Unicode code point).</param>
        /// <returns>The index of the first occurrence of the character in the
        /// character sequence represented by this object, or
        /// <c>-1</c> if the character does not occur.
        /// </returns>
        public static int IndexOf(this ReadOnlySpan<char> text, int codePoint) // KEEP IN SYNC WITH StringExtensions.IndexOf()
        {
            if (codePoint < Character.MinSupplementaryCodePoint)
            {
                // handle most cases here (ch is a BMP code point or a
                // negative value (invalid code point))
                if (codePoint >= Character.MinCodePoint)
                    return System.MemoryExtensions.IndexOf(text, (char)codePoint);

                return -1;
            }
            else
            {
                unsafe
                {
                    fixed (char* textPtr = &MemoryMarshal.GetReference(text))
                    {
                        return IndexOfSupplementary(textPtr, text.Length, codePoint);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the index within this string of the first occurrence of
        /// the specified character. If a character with value
        /// <paramref name="codePoint"/> occurs in the character sequence represented by
        /// this <see cref="Span{Char}"/> object, then the index (in Unicode
        /// code units) of the first such occurrence is returned. For
        /// values of <paramref name="codePoint"/> in the range from 0 to 0xFFFF
        /// (inclusive), this is the smallest value <i>k</i> such that:
        /// <code>
        ///     this[(<i>k</i>] == <paramref name="codePoint"/>
        /// </code>
        /// is true. For other values of <paramref name="codePoint"/>, it is the
        /// smallest value <i>k</i> such that:
        /// <code>
        ///     this.CodePointAt(<i>k</i>) == <paramref name="codePoint"/>
        /// </code>
        /// is true. In either case, if no such character occurs in this
        /// string, then <c>-1</c> is returned.
        /// </summary>
        /// <param name="text">This <see cref="Span{Char}"/>.</param>
        /// <param name="codePoint">A character (Unicode code point).</param>
        /// <returns>The index of the first occurrence of the character in the
        /// character sequence represented by this object, or
        /// <c>-1</c> if the character does not occur.
        /// </returns>
        public static int IndexOf(this Span<char> text, int codePoint) // KEEP IN SYNC WITH StringExtensions.IndexOf()
        {
            if (codePoint < Character.MinSupplementaryCodePoint)
            {
                // handle most cases here (ch is a BMP code point or a
                // negative value (invalid code point))
                if (codePoint >= Character.MinCodePoint)
                    return System.MemoryExtensions.IndexOf(text, (char)codePoint);

                return -1;
            }
            else
            {
                unsafe
                {
                    fixed (char* textPtr = &MemoryMarshal.GetReference(text))
                    {
                        return IndexOfSupplementary(textPtr, text.Length, codePoint);
                    }
                }
            }
        }

        /// <summary>
        /// Handles (rare) calls of indexOf with a supplementary character.
        /// </summary>
        private unsafe static int IndexOfSupplementary(char* text, int textLength, int codePoint) // KEEP IN SYNC WITH StringExtensions.IndexOfSupplementary()
        {
            if (Character.IsValidCodePoint(codePoint))
            {
                Character.ToChars(codePoint, out char hi, out char lo); // J2N: Eliminated array allocation
                int max = textLength - 1;
                for (int i = 0; i < max; i++)
                {
                    if (text[i] == hi && text[i + 1] == lo)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        #endregion IndexOf

        #region LastIndexOf

        /// <summary>
        /// Searches for the specified value and returns the index of its last occurrence.
        /// <para/>
        /// This method simply cascades the call to <see cref="System.MemoryExtensions.LastIndexOf{T}(ReadOnlySpan{T}, T)"/>.
        /// Its purpose is to avoid the overhead of casting to an <see cref="int"/> and back to <see cref="char"/> when
        /// calling <see cref="LastIndexOf(ReadOnlySpan{char}, int)"/> with a <see cref="char"/>. If this method did not exist,
        /// the compiler would always choose <see cref="LastIndexOf(ReadOnlySpan{char}, int)"/> instead of
        /// <see cref="System.MemoryExtensions.LastIndexOf{T}(ReadOnlySpan{T}, T)"/> when the type of <c>T</c> is <see cref="char"/>.
        /// </summary>
        /// <param name="text">The span to search.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>The index of the last occurrence of the value in the span. If not found, returns -1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOf(this ReadOnlySpan<char> text, char value)
            => System.MemoryExtensions.LastIndexOf(text, value);

        /// <summary>
        /// Searches for the specified value and returns the index of its last occurrence.
        /// <para/>
        /// This method simply cascades the call to <see cref="System.MemoryExtensions.LastIndexOf{T}(Span{T}, T)"/>.
        /// Its purpose is to avoid the overhead of casting to an <see cref="int"/> and back to <see cref="char"/> when
        /// calling <see cref="LastIndexOf(Span{char}, int)"/> with a <see cref="char"/>. If this method did not exist,
        /// the compiler would always choose <see cref="LastIndexOf(Span{char}, int)"/> instead of
        /// <see cref="System.MemoryExtensions.LastIndexOf{T}(Span{T}, T)"/> when the type of <c>T</c> is <see cref="char"/>.
        /// </summary>
        /// <param name="text">The span to search.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>The index of the last occurrence of the value in the span. If not found, returns -1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOf(this Span<char> text, char value)
            => System.MemoryExtensions.LastIndexOf(text, value);

        /// <summary>
        /// Returns the index within this string of the last occurrence of
        /// the specified character. For values of <paramref name="codePoint"/> in the
        /// range from 0 to 0xFFFF (inclusive), the index (in Unicode code
        /// units) returned is the largest value <i>k</i> such that:
        /// <code>
        ///     this[<i>k</i>] == <paramref name="codePoint"/>
        /// </code>
        /// is <c>true</c>. For other values of <paramref name="codePoint"/>, it is the
        /// largest value in <i>k</i> such that:
        /// <code>
        ///     this.CodePointAt(<i>k</i>) = <paramref name="codePoint"/>
        /// </code>
        /// is <c>true</c>. In either case, if no such character occurs in this
        /// string, then <c>-1</c> is returned.  The
        /// <paramref name="text"/> is searched backwards starting at the last character.
        /// </summary>
        /// <param name="text">This <see cref="ReadOnlySpan{Char}"/>.</param>
        /// <param name="codePoint">A character (Unicode code point).</param>
        /// <returns>
        /// The index of the last occurrence of the character in the
        /// character sequence represented by this object, or
        /// <c>-1</c> if the character does not occur.
        /// </returns>
        public static int LastIndexOf(this ReadOnlySpan<char> text, int codePoint) // KEEP IN SYNC WITH StringExtensions.LastIndexOf()
        {
            if (codePoint < Character.MinSupplementaryCodePoint)
            {
                // handle most cases here (ch is a BMP code point or a
                // negative value (invalid code point))
                if (codePoint >= Character.MinCodePoint)
                    return System.MemoryExtensions.LastIndexOf(text, (char)codePoint);

                return -1;
            }
            else
            {
                unsafe
                {
                    fixed (char* textPtr = &MemoryMarshal.GetReference(text))
                    {
                        return LastIndexOfSupplementary(textPtr, text.Length, codePoint);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the index within this string of the last occurrence of
        /// the specified character. For values of <paramref name="codePoint"/> in the
        /// range from 0 to 0xFFFF (inclusive), the index (in Unicode code
        /// units) returned is the largest value <i>k</i> such that:
        /// <code>
        ///     this[<i>k</i>] == <paramref name="codePoint"/>
        /// </code>
        /// is <c>true</c>. For other values of <paramref name="codePoint"/>, it is the
        /// largest value in <i>k</i> such that:
        /// <code>
        ///     this.CodePointAt(<i>k</i>) = <paramref name="codePoint"/>
        /// </code>
        /// is <c>true</c>. In either case, if no such character occurs in this
        /// string, then <c>-1</c> is returned.  The
        /// <paramref name="text"/> is searched backwards starting at the last character.
        /// </summary>
        /// <param name="text">This <see cref="Span{Char}"/>.</param>
        /// <param name="codePoint">A character (Unicode code point).</param>
        /// <returns>
        /// The index of the last occurrence of the character in the
        /// character sequence represented by this object, or
        /// <c>-1</c> if the character does not occur.
        /// </returns>
        public static int LastIndexOf(this Span<char> text, int codePoint) // KEEP IN SYNC WITH StringExtensions.LastIndexOf()
        {
            if (codePoint < Character.MinSupplementaryCodePoint)
            {
                // handle most cases here (ch is a BMP code point or a
                // negative value (invalid code point))
                if (codePoint >= Character.MinCodePoint)
                    return System.MemoryExtensions.LastIndexOf(text, (char)codePoint);

                return -1;
            }
            else
            {
                unsafe
                {
                    fixed (char* textPtr = &MemoryMarshal.GetReference(text))
                    {
                        return LastIndexOfSupplementary(textPtr, text.Length, codePoint);
                    }
                }
            }
        }

        /// <summary>
        /// Handles (rare) calls of lastIndexOf with a supplementary character.
        /// </summary>
        private unsafe static int LastIndexOfSupplementary(char* text, int textLength, int codePoint) // KEEP IN SYNC WITH StringExtensions.LastIndexOfSupplementary()
        {
            if (Character.IsValidCodePoint(codePoint))
            {
                Character.ToChars(codePoint, out char hi, out char lo); // J2N: Eliminated array allocation
                for (int i = textLength - 2; i >= 0; i--)
                {
                    if (text[i] == hi && text[i + 1] == lo)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        #endregion LastIndexOf

        #region Replace

        /// <summary>
        /// Returns a new string in which all occurrences of a specified Unicode character in this instance
        /// are replaced with another specified Unicode character. The operation is done in place.
        /// </summary>
        /// <param name="text">This <see cref="Span{T}"/>.</param>
        /// <param name="oldChar">The Unicode character to be replaced.</param>
        /// <param name="newChar">The Unicode character to replace all occurrences of <paramref name="oldChar"/>.</param>
        internal static void Replace(this Span<char> text, char oldChar, char newChar) // J2N TODO: Should we make a generic overload to patch System.MemoryExtensions publicly?
        {
#if FEATURE_MEMORYEXTENSIONS_REPLACE_T_T
            System.MemoryExtensions.Replace(text, oldChar, newChar);
#else
            int length = text.Length;
            for (int i = 0; i < length; i++)
            {
                if (text[i] == oldChar)
                    text[i] = newChar;
            }
#endif
        }

        #endregion Replace

        #region ReverseText

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
            fixed (char* textPtr = &MemoryMarshal.GetReference(text))
            {
                ReverseText(textPtr, count);
            }
        }

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

        #endregion ReverseText

        #region TryGetReference

        /// <summary>
        /// Sets the supplied <paramref name="reference"/> to the underlying <see cref="string"/> or <see cref="T:char[]"/>
        /// of this <see cref="ReadOnlyMemory{Char}"/>. This allows use of <see cref="ReadOnlyMemory{Char}"/> as a field
        /// of a struct or class without having the underlying <see cref="string"/> or <see cref="T:char[]"/> go out of scope.
        /// </summary>
        /// <param name="text">This <see cref="ReadOnlyMemory{Char}"/>.</param>
        /// <param name="reference">When this method returns successfully, the refernce will be set by ref to the
        /// underlying <see cref="string"/> or <see cref="T:char[]"/> of <paramref name="text"/>.</param>
        /// <returns><c>true</c> if the underlying reference could be retrieved; otherwise, <c>false</c>.
        /// Note that if the underlying memory is not a <see cref="string"/> or <see cref="T:char[]"/>,
        /// this method will always return <c>false</c>.</returns>
        internal static bool TryGetReference(this ReadOnlyMemory<char> text, [MaybeNullWhen(false)] ref object? reference)
        {
            if (MemoryMarshal.TryGetString(text, out string? stringValue, out _, out _) && stringValue is not null)
            {
                reference = stringValue;
                return true;
            }
            else if (MemoryMarshal.TryGetArray(text, out ArraySegment<char> arraySegment) && arraySegment.Array is not null)
            {
                reference = arraySegment.Array;
                return true;
            }
            return false;
        }

        #endregion
    }
}
