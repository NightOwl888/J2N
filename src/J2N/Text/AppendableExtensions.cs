using System;
using System.Buffers;

namespace J2N.Text
{
    /// <summary>
    /// Extensions to the <see cref="IAppendable"/> interface.
    /// </summary>
    public static class AppendableExtensions
    {
        private const int CharPoolBufferSize = 512;

        /// <summary>
        /// Appends the string representation of a specified read-only character span to this <paramref name="appendable"/>.
        /// </summary>
        /// <param name="appendable">This <see cref="IAppendable"/>.</param>
        /// <param name="value">The read-only character span to append.</param>
        /// <returns>A reference to this <see cref="IAppendable"/> instance after the append operation is completed.</returns>
        /// <remarks>
        /// If the <paramref name="appendable"/> supports <see cref="ISpanAppendable"/>, the optimized
        /// <see cref="ISpanAppendable.Append(ReadOnlySpan{char})"/> method is called on the <paramref name="appendable"/> implementation.
        /// If not, a <see cref="T:char[]"/> is obtained from the shared array pool and used to provide the chars via the
        /// <see cref="IAppendable.Append(char[], int, int)"/> method. For larger strings, the data is appended in chunks
        /// up to 512 characters long.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="appendable"/> is <c>null</c>.</exception>
        public static T Append<T>(this T appendable, ReadOnlySpan<char> value) where T : IAppendable
        {
            if (appendable is null)
                throw new ArgumentNullException(nameof(appendable));

            if (appendable is ISpanAppendable spanAppendable)
            {
                spanAppendable.Append(value);
                return appendable;
            }

            return AppendSlow(appendable, value);

            static T AppendSlow(T appendable, ReadOnlySpan<char> value)
            {

                int startIndex = 0;
                int remainingCount = value.Length;

                char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
                try
                {
                    while (remainingCount > 0)
                    {
                        // Determine the chunk size for the current iteration
                        int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                        // Copy the chunk to the buffer
                        value.Slice(startIndex, chunkLength).CopyTo(buffer);

                        appendable.Append(buffer, 0, chunkLength);

                        startIndex += chunkLength;
                        remainingCount -= chunkLength;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
                return appendable;
            }
        }
    }
}
