// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.CodeGeneration;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Extensions to <see cref="MutableTextBuffer"/>.
    /// </summary>
    public static partial class MutableTextBufferExtensions
    {
        #region Append/Insert bool

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance
        /// in lowercase.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The Boolean value to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// This matches the behavior of Java's StringBuilder. To match the behavior
        /// of .NET, call <see cref="Insert{TBuilder}(TBuilder, int, bool, BooleanFormat)"/> and specify <see cref="BooleanFormat.TitleCase"/>.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="bool" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, bool value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance
        /// in the specified format.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The Boolean value to append.</param>
        /// <param name="format">
        /// The format to use. Specify <see cref="BooleanFormat.Lowercase"/> to match Java.
        /// Specify <see cref="BooleanFormat.TitleCase"/> to match .NET.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        /// <seealso cref="bool" />
        /// <seealso cref="BooleanFormat" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, bool value, BooleanFormat format)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified Boolean value to this instance
        /// in lowercase at the specifed character position.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This matches the behavior of Java's StringBuilder. To match the behavior
        /// of .NET, call <see cref="Insert{TBuilder}(TBuilder, int, bool, BooleanFormat)"/> and specify <see cref="BooleanFormat.TitleCase"/>.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// </remarks>
        /// <seealso cref="bool" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, bool value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified Boolean value to this instance
        /// in the specified format at the specified position.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="format">
        /// The format to use. Specify <see cref="BooleanFormat.Lowercase"/> to match Java.
        /// Specify <see cref="BooleanFormat.TitleCase"/> to match .NET.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.</remarks>
        /// <seealso cref="bool" />
        /// <seealso cref="BooleanFormat" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, bool value, BooleanFormat format)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format);
            return text;
        }

        #endregion Append/Insert bool

        #region AppendLine

        /// <summary>Appends the default line terminator to the end of the current <see cref="MutableTextBuffer"/> object.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLine<TBuilder>(this TBuilder text)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLineInternal();
            return text;
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of the
        /// current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="string" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLine<TBuilder>(this TBuilder text, string? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLineInternal(value);
            return text;
        }

        /// <summary>
        /// Appends a copy of the specified sequence of characters followed by the default line terminator to the end of the
        /// current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="ReadOnlySpan{Char}" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLine<TBuilder>(this TBuilder text, ReadOnlySpan<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLineInternal(value);
            return text;
        }

        #endregion

        #region AsCharSequence

        /// <summary>
        /// Convenience method to wrap a string in a <see cref="MutableTextBufferCharSequence"/>
        /// so a <see cref="MutableTextBuffer"/> can be used as <see cref="ICharSequence"/>.
        /// </summary>
        [CodeGenerationIgnore]
        public static ICharSequence AsCharSequence(this MutableTextBuffer? text)
        {
            return new MutableTextBufferCharSequence(text);
        }

        #endregion AsCharSequence

        #region AsMemory

        /// <summary>
        /// Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <returns>The read-only character memory representation of the string, or <c>default</c> if
        /// <paramref name="text"/> is <see langword="null"/>.</returns>
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned memory is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the memory contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned memory provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of memory usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned memory is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the memory contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned memory provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of memory usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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
        /// if <paramref name="text"/> is <see langword="null"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>, <paramref name="length"/>,
        /// or <paramref name="start"/> + <paramref name="length"/> is not in the range of <paramref name="text"/>.</exception>
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned memory is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the memory contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned memory provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of memory usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less
        /// than 0 or greater than <c>text.Length</c>.</exception>
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned memory is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the memory contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned memory provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of memory usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="range"/>'s start or end index is not within the bounds of the string.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="range"/>'s start index is greater than its end index.
        /// </exception>
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned memory is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the memory contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned memory provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of memory usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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

        #region AsSpan

        /// <summary>
        /// Creates a new readonly span over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <returns>The read-only span representation of the string.</returns>
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned span is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the span contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned span provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of span usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned span is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the span contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned span provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of span usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned span is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the span contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned span provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of span usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less
        /// than 0 or greater than <c>text.Length</c>.</exception>
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned span is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the span contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned span provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of span usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="range"/>'s start or end index is not within the bounds of the string.
        /// -or-
        /// <paramref name="range"/>'s start index is greater than its end index.
        /// </exception>
        /// <remarks>
        /// Returns <see langword="default"/> when <paramref name="text"/> is <see langword="null"/>.
        /// <para/>
        /// The returned span is only valid while the underlying <see cref="MutableTextBuffer"/> remains unchanged.
        /// Concurrent mutation invalidates the span contents.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned span provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref = "SynchronizedTextBuilder.SyncRoot" /> property for the
        /// entire duration of span usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
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

        #region Clear

        /// <summary>Removes all characters from the current <see cref="MutableTextBuffer"/> instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// <see cref="Clear"/> is a convenience method that is equivalent to setting
        /// the <see cref="MutableTextBuffer.Length"/> property of the current instance to 0 (zero).
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Clear<TBuilder>(this TBuilder text)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ClearInternal();
            return text;
        }

        #endregion Clear

        #region InsertFromSelf

#if FEATURE_INDEX_RANGE
        /// <summary>Inserts a copy of the specified range of characters from this buffer at the specified index.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The index at which the copied range will be inserted.</param>
        /// <param name="range">The range of characters to copy and insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="range"/> extends beyond the bounds
        /// of the buffer.
        /// </exception>
        /// <remarks>
        /// This operation supports overlapping source and destination ranges.
        /// <para/>
        /// <paramref name="index"/> refers to the original buffer before insertion takes place.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder InsertFromSelf<TBuilder>(this TBuilder text, int index, Range range)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertFromSelfInternal(index, range);
            return text;
        }

#endif

        /// <summary>
        /// Inserts a copy of a range of characters from this buffer
        /// at the specified index, expanding the <see cref="MutableTextBuffer.Length"/> by
        /// <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The index at which the copied range will be inserted.</param>
        /// <param name="startIndex">The starting index of the source range to copy.</param>
        /// <param name="count">The number of characters to copy.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/>, or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> or <paramref name="startIndex"/> is greater
        /// than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater
        /// than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        /// <remarks>
        /// This operation supports overlapping source and destination ranges.
        /// <para/>
        /// <paramref name="index"/> refers to the original buffer before insertion
        /// takes place.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder InsertFromSelf<TBuilder>(this TBuilder text, int index, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertFromSelfInternal(index, startIndex, count);
            return text;
        }

        #endregion InsertFromSelf

        #region Reverse

        /// <summary>
        /// Causes this character sequence to be replaced by the reverse of
        /// the sequence. If there are any surrogate pairs included in the
        /// sequence, these are treated as single characters for the
        /// reverse operation. Thus, the order of the high-low surrogates
        /// is never reversed.
        /// <para/>
        /// IMPORTANT: This operation is done in-place. Although a <see cref="MutableTextBuffer"/>
        /// is returned, it is the SAME instance as the one that is passed in.
        /// <para/>
        /// Let <c>n</c> be the character length of this character sequence
        /// (not the length in <see cref="char"/> values) just prior to
        /// execution of the <see cref="Reverse{TBuilder}(TBuilder)"/> method. Then the
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
        /// Usage Note: This is the same operation as Java's StringBuilder.reverse()
        /// method. However, J2N also provides <see cref="J2N.Text.StringExtensions.ReverseText(string)"/>
        /// and <see cref="J2N.MemoryExtensions.ReverseText(Span{char})"/> which
        /// don't require a <see cref="MutableTextBuffer"/> instance.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="StringExtensions.ReverseText(string)" />
        /// <seealso cref="MemoryExtensions.ReverseText(Span{char})" />
        /// <seealso cref="StringBuilderExtensions.Reverse(StringBuilder)" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Reverse<TBuilder>(this TBuilder text)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReverseInternal();
            return text;
        }

        #endregion
    }
}
