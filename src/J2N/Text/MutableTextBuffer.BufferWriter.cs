using J2N.CodeGeneration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace J2N.Text
{
    internal partial class MutableTextBuffer
    {
        // Implementation of IBufferWriter<char>

        /// <summary>
        /// Returns a <see cref="Span{Char}"/> to write to that is at least the requested size
        /// (specified by <paramref name="sizeHint"/>).
        /// </summary>
        /// <param name="sizeHint">The minimum length of the returned <see cref="Span{Char}"/>.
        /// If 0, a non-empty buffer is returned.</param>
        /// <returns>A <see cref="Span{Char}"/> of at least the size <paramref name="sizeHint"/>.
        /// If <paramref name="sizeHint"/> is 0, returns a non-empty buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sizeHint"/> is less than zero.</exception>
        /// <remarks>
        /// The capacity is adjusted as needed.
        /// <para/>
        /// This method never returns <see cref="Span{Char}.Empty"/>.
        /// <para/>
        /// The returned <see cref="Span{Char}"/> allows writing characters directly to the buffer of
        /// <see cref="MutableTextBuffer"/>. This can be used for more complex and low-level business
        /// logic to be applied when either the number of characters is unknown or the number of separate
        /// operations would be prohibitively costly.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned span provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref="SynchronizedTextBuilder.SyncRoot"/> for the duration of the
        /// span usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
        public Span<char> GetSpan(int sizeHint = 0)
        {
            if (sizeHint < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(sizeHint, ExceptionArgument.sizeHint);
            }

            // Return a minimum of DefaultCapacity.
            if (sizeHint < DefaultCapacity)
            {
                sizeHint = DefaultCapacity;
            }

            EnsureCapacityForWriting(sizeHint);

            int position = m_Position;

            if (!clearExposedBuffers)
            {
                // Return the whole remaining buffer, uncleared
                return m_Chars.AsSpan(position);
            }

            // Return the larger of DefaultCapacity or sizeHint, cleared
            return GetClearedWritableSpan(position, sizeHint);
        }

        /// <summary>
        /// Returns a <see cref="Memory{Char}"/> to write to that is at least the length
        /// specified by <paramref name="sizeHint"/>.
        /// </summary>
        /// <param name="sizeHint">The minimum requested length of the <see cref="Memory{Char}"/>.
        /// If 0, a non-empty buffer is returned.</param>
        /// <returns>A <see cref="Memory{Char}"/> whose length is at least <paramref name="sizeHint"/>.
        /// If <paramref name="sizeHint"/> is not provided or is equal to 0, some non-empty buffer is returned.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sizeHint"/> is less than zero.</exception>
        /// <remarks>
        /// The capacity is adjusted as needed.
        /// <para/>
        /// This method never returns <see cref="Memory{Char}.Empty"/>.
        /// <para/>
        /// The returned <see cref="Memory{Char}"/> allows writing characters directly to the buffer of
        /// <see cref="MutableTextBuffer"/>. This can be used for more complex and low-level business
        /// logic to be applied when either the number of characters is unknown or the number of separate
        /// operations would be prohibitively costly.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned memory provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref="SynchronizedTextBuilder.SyncRoot"/> for the duration of the
        /// span usage if concurrent mutation is possible.
        /// </synchronizationNote>
        [CodeGenerationSkipSynchronization]
        public Memory<char> GetMemory(int sizeHint = 0)
        {
            if (sizeHint < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(sizeHint, ExceptionArgument.sizeHint);
            }

            // Return a minimum of DefaultCapacity.
            if (sizeHint < DefaultCapacity)
            {
                sizeHint = DefaultCapacity;
            }

            EnsureCapacityForWriting(sizeHint);

            int position = m_Position;

            if (!clearExposedBuffers)
            {
                // Return the whole remaining buffer, uncleared
                return m_Chars.AsMemory(position);
            }

            // Return the larger of DefaultCapacity or sizeHint, cleared
            m_Chars.AsSpan(position, sizeHint).Clear();
            return m_Chars.AsMemory(position, sizeHint);
        }

        /// <summary>
        /// Notifies the <see cref="MutableTextBuffer"/> that <paramref name="count"/> items were
        /// written to the output <see cref="Span{Char}"/> of a prior call to <see cref="GetSpan(int)"/>
        /// or <see cref="Memory{Char}"/> of a prior call to <see cref="GetMemory(int)"/>.
        /// </summary>
        /// <param name="count">The number of items written.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero.</exception>
        /// <exception cref="InvalidOperationException">The method call attempts to advance past the remaining <see cref="Capacity"/>
        /// beyond <see cref="Length"/>.</exception>
        /// <remarks>
        /// You must request a new buffer after calling <see cref="Advance(int)"/> to continue writing more data
        /// and cannot write to a previously acquired buffer.
        /// <para/>
        /// Calling <see cref="Advance(int)"/> is effictively the same operation as adding <paramref name="count"/>
        /// to the existing <see cref="Length"/>.
        /// </remarks>
        /// <synchronizationNote>
        /// This method is intended to be used in conjunction with either <see cref="GetSpan(int)"/> or <see cref="GetMemory(int)"/>.
        /// If concurrent mutation is possible, this method should be synchronized externally by the caller with either of those two
        /// methods using <see cref="SynchronizedTextBuilder.SyncRoot"/> for the duration of the memory usage.
        /// </synchronizationNote>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationSkipSynchronization]
        public void Advance(int count)
        {
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (count > (m_Chars.Length - m_Position))
            {
                ThrowInvalidOperationException_AdvancedTooFar(m_Chars.Length);
            }

            m_Position += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacityForWriting(int sizeHint)
        {
            if (sizeHint > (m_Chars.Length - m_Position))
            {
                Grow(sizeHint);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<char> GetClearedWritableSpan(int start, int length)
        {
            Span<char> span = m_Chars.AsSpan(start, length);
            span.Fill('\0');
            return span;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [DoesNotReturn]
        private static void ThrowInvalidOperationException_AdvancedTooFar(int capacity)
        {
            throw new InvalidOperationException(SR.Format(SR.BufferWriterAdvancedTooFar, capacity));
        }
    }
}
