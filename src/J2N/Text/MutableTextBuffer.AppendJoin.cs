// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        #region AppendJoin

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder}(TBuilder, string?, object?[])"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal(string? separator, params object?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder}(TBuilder, string?, ReadOnlySpan{object?})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal(string? separator, params ReadOnlySpan<object?> values)
        {
            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder, T}(TBuilder, string?, IEnumerable{T})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal<T>(string? separator, IEnumerable<T> values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder}(TBuilder, string?, string?[])"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal(string? separator, params string?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder}(TBuilder, string?, ReadOnlySpan{string?})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal(string? separator, params ReadOnlySpan<string?> values)
        {
            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder}(TBuilder, char, object?[])"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal(char separator, params object?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            AppendJoinCore(ref separator, 1, values);
        }

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder}(TBuilder, char, ReadOnlySpan{object?})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal(char separator, params ReadOnlySpan<object?> values) =>
            AppendJoinCore(ref separator, 1, values);

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder, T}(TBuilder, char, IEnumerable{T})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal<T>(char separator, IEnumerable<T> values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            AppendJoinCore(ref separator, 1, values);
        }

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder}(TBuilder, char, string?[])"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal(char separator, params string?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            AppendJoinCore(ref separator, 1, values);
        }

        /// <summary>
        /// Concatenates the strings of the provided sequence, using the specified separator between each string,
        /// then appends the result to the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendJoin{TBuilder}(TBuilder, char, ReadOnlySpan{string?})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendJoinInternal(char separator, params ReadOnlySpan<string?> values) =>
            AppendJoinCore(ref separator, 1, values);

        private void AppendJoinCore<T>(ref char separator, int separatorLength, IEnumerable<T> values)
        {
            Debug.Assert(values != null);
            Debug.Assert(!Unsafe.IsNullRef(ref separator));
            Debug.Assert(separatorLength >= 0);

            using (IEnumerator<T> en = values!.GetEnumerator())
            {
                if (!en.MoveNext())
                {
                    return;
                }

                T value = en.Current;
                if (value != null)
                {
                    AppendInternal(value.ToString()); // J2N TODO: ISpanFormattable, IFormattable to override culture?
                }

                while (en.MoveNext())
                {
                    Append(ref separator, separatorLength);
                    value = en.Current;
                    if (value != null)
                    {
                        AppendInternal(value.ToString()); // J2N TODO: ISpanFormattable, IFormattable to override culture?
                    }
                }
            }
        }

        private void AppendJoinCore<T>(ref char separator, int separatorLength, ReadOnlySpan<T> values)
        {
            if (values.IsEmpty)
            {
                return;
            }

            if (values[0] != null)
            {
                AppendInternal(values[0]!.ToString()); // J2N TODO: ISpanFormattable, IFormattable to override culture?
            }

            for (int i = 1; i < values.Length; i++)
            {
                Append(ref separator, separatorLength);
                if (values[i] != null)
                {
                    AppendInternal(values[i]!.ToString()); // J2N TODO: ISpanFormattable, IFormattable to override culture?
                }
            }
        }

        #endregion AppendJoin
    }
}
