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
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, sbyte, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<sbyte, SByteFormatter>(3, value, format, provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, byte, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<byte, ByteFormatter>(4, value, format, provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, short, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<short, Int16Formatter>(4, value, format, provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, int, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<int, Int32Formatter>(6, value, format, provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, long, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<long, Int64Formatter>(10, value, format, provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, float, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
            => AppendNumberCore<float, SingleFormatter>(6, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, double, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
            => AppendNumberCore<double, DoubleFormatter>(14, value, format, provider);

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, decimal, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        // J2N TODO: Since BigDecimal in Java doesn't use the same default format as this, we will need to change the default before this can be made public
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(decimal value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendInternal(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ushort, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<ushort, UInt16Formatter>(4, value, format, provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, uint, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<uint, UInt32Formatter>(6, value, format, provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified numeric type to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, ulong, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<ulong, UInt64Formatter>(10, value, format, provider);
#endif

        // J2N: Helper method for supported types so we don't need to duplicate all of this business logic
        // on every number type.

        private void AppendNumberCore<T, TFormatter>(
            int ensureAdditionalCapacityBeyondPos, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TFormatter : struct, INumberFormatter<T>
        {
            provider ??= NumberFormatInfo.InvariantInfo; // For JDK-style formatting

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
                // Check if the valueCount will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the MutableTextBuffer.
                int newLength = m_Chars.Length * 2;
                if (newLength > m_MaxCapacity)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                // J2N: This effectively doubles the buffer
                Grow(m_Chars.Length + 1); // rare
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

            provider ??= NumberFormatInfo.InvariantInfo; // For JDK-style formatting
            int charsWritten;
            while (!value.TryFormat(m_Chars.AsSpan(m_Position), out charsWritten, format, provider))
            {
                int length = m_Chars.Length;
                int additionalCapacity = length - m_Position == length ? m_Chars.Length + 1 : m_Chars.Length; // Ensure we request enough to cause a re-grow

                // Check if the valueCount will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the MutableTextBuffer.
                int newLength = m_Position + additionalCapacity;
                if (newLength > m_MaxCapacity)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                Grow(additionalCapacity);
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
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, sbyte, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<sbyte, SByteFormatter>(index, value, format, provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, byte, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<byte, ByteFormatter>(index, value, format, provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, short, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<short, Int16Formatter>(index, value, format, provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, int, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<int, Int32Formatter>(index, value, format, provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, long, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<long, Int64Formatter>(index, value, format, provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, float, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
            => InsertNumberCore<float, SingleFormatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, double, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
            => InsertNumberCore<double, DoubleFormatter>(index, value, format, provider);

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, decimal, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        // J2N TODO: Since BigDecimal in Java doesn't use the same default format as this, we will need to change the default before this can be made public
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, decimal value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertInternal(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ushort, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<ushort, UInt16Formatter>(index, value, format, provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, uint, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<uint, UInt32Formatter>(index, value, format, provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified numeric type to this instance
        /// at the specified position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, ulong, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<ulong, UInt64Formatter>(index, value, format, provider);
#endif

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

            provider ??= NumberFormatInfo.InvariantInfo; // For JDK-style formatting
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

            provider ??= NumberFormatInfo.InvariantInfo; // For JDK-style formatting

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
