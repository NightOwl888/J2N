using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N
{
    /// <summary>
    /// Contract that specifies this type is capable of copying its contents to a
    /// <see cref="Span{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of span element.</typeparam>
    internal interface ISpanCopyable<T>
    {
        /// <summary>
        /// Gets the length of this instance.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Copies the characters from a specified segment of this instance to a destination span.
        /// </summary>
        /// <param name="sourceIndex">The starting position in this instance where elements will
        /// be copied from. The index is zero-based.</param>
        /// <param name="destination">The writable span where elements will be copied.</param>
        /// <param name="count">The number of elements to be copied.</param>
        void CopyTo(int sourceIndex, Span<char> destination, int count);
    }
}
