#if FEATURE_SPAN
using System;

namespace J2N.Text
{
    /// <summary>
    /// Provides functionality to append the characters from <see cref="ReadOnlySpan{T}"/> to an
    /// <see cref="IAppendable"/> implementation.
    /// </summary>
    public interface ISpanAppendable : IAppendable
    {
        /// <summary>
        /// Appends the string representation of a specified read-only character span to this instance.
        /// <para/>
        /// <see cref="IAppendable"/> types that implement this interface will be called by the
        /// <see cref="AppendableExtensions.Append{T}(T, ReadOnlySpan{char})"/> to provide a better
        /// optimized implementation than the default, which uses a <see cref="T:char[]"/> buffer obtained
        /// from the shared array pool and calls the <see cref="IAppendable.Append(char[], int, int)"/>
        /// method to perform the append operation (in chunks).
        /// </summary>
        /// <param name="value">The read-only character span to append.</param>
        /// <returns>A reference to this instance after the append operation is completed.</returns>
        /// <seealso cref="AppendableExtensions"/>
        /// <seealso cref="AppendableExtensions.Append{T}(T, ReadOnlySpan{char})"/>
        ISpanAppendable Append(ReadOnlySpan<char> value);
    }
}
#endif