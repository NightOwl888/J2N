#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.CodeGeneration;
using J2N.Numerics;
using J2N.Numerics.Formatters;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace J2N.Text
{
    internal partial class MutableTextBuffer
    {
        #region Append Number

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, sbyte, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<sbyte, SByteFormatter>(3, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, byte, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<byte, ByteFormatter>(4, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, short, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<short, Int16Formatter>(4, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, int, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<int, Int32Formatter>(6, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, long, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<long, Int64Formatter>(10, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, float, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<float, SingleFormatter>(6, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, double, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<double, DoubleFormatter>(14, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, decimal, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        // J2N TODO: Since BigDecimal in Java doesn't use the same default format as this, we will need to change the default before this can be made public
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(decimal value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<decimal, DecimalFormatter>(14, value, format, provider);
        //=> AppendInternal(value.ToString(Number.ConvertFormatToString(format), provider ?? DefaultNumberFormatInfo));

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ushort, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<ushort, UInt16Formatter>(4, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, uint, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<uint, UInt32Formatter>(6, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ulong, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => AppendNumberCore<ulong, UInt64Formatter>(10, value, format, provider);

        // J2N: Helper method for supported types so we don't need to duplicate all of this business logic
        // on every number type.

        private void AppendNumberCore<T, TFormatter>(
            int ensureAdditionalCapacityBeyondPos, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TFormatter : struct, INumberFormatter<T>
        {
            provider ??= DefaultNumberFormatInfo; // Set by UseInvariantDefaults

            if ((uint)m_Position + (uint)ensureAdditionalCapacityBeyondPos > (uint)m_Chars.Length)
            {
                // Check if the valueCount will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the MutableTextBuffer.
                int newLength = m_Position + ensureAdditionalCapacityBeyondPos;
                if (newLength > m_MaxCapacity)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                Grow(ensureAdditionalCapacityBeyondPos);
            }

            int charsWritten;
            while (!default(TFormatter).TryFormat(value, format, provider, m_Chars.AsSpan(m_Position), out charsWritten))
            {
                GrowForRetry(throwOnOverflow: true);
            }

            m_Position += charsWritten;
        }

        private void AppendSpanFormattable<T>(T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            where T : ISpanFormattable
#else
            where T : Number
#endif
        {
            Debug.Assert(typeof(T).Assembly.Equals(typeof(object).Assembly) || typeof(T).Assembly.Equals(typeof(Number).Assembly), "Implementation trusts the results of TryFormat because T is expected to be something known");

            provider ??= DefaultNumberFormatInfo; // Set by UseInvariantDefaults
            int charsWritten;
            while (!value.TryFormat(m_Chars.AsSpan(m_Position), out charsWritten, format, provider))
            {
                GrowForRetry(throwOnOverflow: true);
            }

            m_Position += charsWritten;
        }

        #endregion Append Number

        #region Insert Number

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, sbyte, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<sbyte, SByteFormatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, byte, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<byte, ByteFormatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, short, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<short, Int16Formatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, int, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<int, Int32Formatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, long, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<long, Int64Formatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, float, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<float, SingleFormatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, double, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<double, DoubleFormatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, decimal, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        // J2N TODO: Since BigDecimal in Java doesn't use the same default format as this, we will need to change the default before this can be made public
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, decimal value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<decimal, DecimalFormatter>(index, value, format, provider);
        //=> InsertInternal(index, value.ToString(Number.ConvertFormatToString(format), provider ?? DefaultNumberFormatInfo), 1);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ushort, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<ushort, UInt16Formatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, uint, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<uint, UInt32Formatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ulong, ReadOnlySpan{char}, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => InsertNumberCore<ulong, UInt64Formatter>(index, value, format, provider);

        // J2N: Helper method for supported types so we don't need to duplicate all of this business logic
        // on every number type.
        private void InsertNumberCore<T, TFormatter>(
            int index, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TFormatter : struct, INumberFormatter<T>
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            provider ??= DefaultNumberFormatInfo; // Set by UseInvariantDefaults
            char[]? arrayToReturnToPool = null;
            Span<char> buffer = stackalloc char[CharStackBufferSize];
            int charsWritten = 0;
            try
            {
                while (!default(TFormatter).TryFormat(value, format, provider, buffer, out charsWritten))
                {
                    // Check if the valueCount will put us over m_MaxCapacity.
                    // Doing the check here prevents corruption of the MutableTextBuffer.
                    int newLength = buffer.Length * 2;
                    if (newLength > m_MaxCapacity)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                    }
                    buffer = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(newLength);
                }

                // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
                // we want exceeding the maximum capacity to throw an OutOfMemoryException.
                InsertInternal(index, buffer.Slice(0, charsWritten), 1);
            }
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
        }

        private void InsertSpanFormattable<T>(int index, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            where T : ISpanFormattable
#else
            where T : Number
#endif
        {
            Debug.Assert(typeof(T).Assembly.Equals(typeof(object).Assembly) || typeof(T).Assembly.Equals(typeof(Number).Assembly), "Implementation trusts the results of TryFormat because T is expected to be something known");

            provider ??= DefaultNumberFormatInfo; // Set by UseInvariantDefaults

            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            char[]? arrayToReturnToPool = null;
            Span<char> buffer = stackalloc char[CharStackBufferSize];
            int charsWritten = 0;
            try
            {
                while (!value.TryFormat(buffer, out charsWritten, format, provider))
                {
                    // Check if the valueCount will put us over m_MaxCapacity.
                    // Doing the check here prevents corruption of the MutableTextBuffer.
                    int newLength = buffer.Length * 2;
                    if (newLength > m_MaxCapacity)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                    }

                    buffer = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(newLength);
                }

                // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
                // we want exceeding the maximum capacity to throw an OutOfMemoryException.
                InsertInternal(index, buffer.Slice(0, charsWritten), 1);
            }
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
        }

        #endregion Insert Number
    }
}
