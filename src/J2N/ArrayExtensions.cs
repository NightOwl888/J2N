using System;

namespace J2N
{
    /// <summary>
    /// Extensions to <see cref="System.Array"/> types.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Assigns the specified value to each element of the specified array.
        /// </summary>
        /// <typeparam name="T">The type array element.</typeparam>
        /// <param name="array">The array to be filled.</param>
        /// <param name="value">The value to be set to all of the elements of the array.</param>
        public static void Fill<T>(this T[] array, T value)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }

        /// <summary>
        /// Assigns the specified value to each element of the specified array.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length, not an exclusive end index as would be the case in Java. To translate from Java,
        /// use <c>endIndex - beginIndex</c> to resolve the value for <paramref name="length"/>.
        /// </summary>
        /// <typeparam name="T">The type array element.</typeparam>
        /// <param name="array">The array to be filled.</param>
        /// <param name="startIndex">The index of the first element (inclusive) to be filled with the specified <paramref name="value"/>.</param>
        /// <param name="length">The number of elements to set to the specified <paramref name="value"/>.</param>
        /// <param name="value">The value to be set to the specified elements of the array.</param>
        /// 
        public static void Fill<T>(T[] array, int startIndex, int length, T value)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex > array.Length - length)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(startIndex)} ({startIndex}) + {nameof(length)} ({length}) > {nameof(array.Length)} ({array.Length}).");

            int end = startIndex + length;
            for (int i = startIndex; i < end; i++)
                array[i] = value;
        }
    }
}
