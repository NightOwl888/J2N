using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace J2N.Text
{
    public partial class MutableTextBuffer
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
        /// <remarks>This method never returns <see cref="Span{Char}.Empty"/>.</remarks>
        public Span<char> GetSpan(int sizeHint = 0)
        {
            if (sizeHint < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(sizeHint, ExceptionArgument.sizeHint);
            }

            if (sizeHint == 0)
            {
                sizeHint = DefaultCapacity;
            }

            EnsureCapacityForWriting(sizeHint);

            int position = m_Position;

            if (!clearExposedBuffers)
            {
                return m_Chars.AsSpan(position);
            }

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
        /// <remarks>This method never returns <see cref="Memory{Char}.Empty"/>.</remarks>
        public Memory<char> GetMemory(int sizeHint = 0)
        {
            if (sizeHint < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(sizeHint, ExceptionArgument.sizeHint);
            }

            if (sizeHint == 0)
            {
                sizeHint = DefaultCapacity;
            }

            EnsureCapacityForWriting(sizeHint);

            int position = m_Position;

            if (!clearExposedBuffers)
            {
                return m_Chars.AsMemory(position);
            }

            m_Chars.AsSpan(position, sizeHint).Clear();
            return m_Chars.AsMemory(position, sizeHint);
        }

        /// <summary>
        /// Notifies the <see cref="MutableTextBuffer"/> that <paramref name="count"/> items were
        /// written to the output <see cref="Span{Char}"/> or <see cref="Memory{Char}"/>.
        /// </summary>
        /// <param name="count">The number of items written.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero.</exception>
        /// <exception cref="InvalidOperationException">The method call attempts to advance past the remaining <see cref="Capacity"/>
        /// beyond <see cref="Length"/>.</exception>
        /// <remarks>You must request a new buffer after calling <see cref="Advance(int)"/> to continue writing more data
        /// and cannot write to a previously acquired buffer.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [DoesNotReturn]
        private static void ThrowInvalidOperationException_AdvancedTooFar(int capacity)
        {
            throw new InvalidOperationException(SR.Format(SR.BufferWriterAdvancedTooFar, capacity));
        }
    }
}
