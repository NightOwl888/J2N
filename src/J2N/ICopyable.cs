using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N
{
    /// <summary>
    /// Contract that specifies this type is capable of copying its contents to an array.
    /// </summary>
    /// <typeparam name="T">The type of array element.</typeparam>
    internal interface ICopyable<T>
    {
        /// <summary>
        /// Gets the length of this instance.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Copies the elements from a specified segment of this instance to a specified
        /// segment of a destination array.
        /// </summary>
        /// <param name="sourceIndex">The starting position in this instance where elements will be copied from.
        /// The index is zero-based.</param>
        /// <param name="destination">The array where elements will be copied.</param>
        /// <param name="destinationIndex">The starting position in <paramref name="destination"/> where eleements
        /// will be copied. The index is zero-based.</param>
        /// <param name="count">The number of elements to be copied.</param>
        void CopyTo(int sourceIndex, T[] destination, int destinationIndex, int count);
    }
}
