using J2N.Collections.ObjectModel;
using System;
using System.Collections.Generic;

namespace J2N.Collections.Generic.Extensions
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Extensions to the <see cref="IList{T}"/> interface.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Returns a read-only <see cref="IList{T}"/> wrapper for the current collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection to make read-only.</param>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="IList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// To prevent any modifications to the <see cref="IList{T}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlyList{T}"/> object does not expose methods that modify the collection. However, if
        /// changes are made to the underlying <see cref="IList{T}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public static ReadOnlyList<T> AsReadOnly<T>(this IList<T> collection)
        {
            return new ReadOnlyList<T>(collection);
        }

        /// <summary>
        /// Moves every element of the list to a random new position in the <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> to shuffle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is <c>null</c>.</exception>
        /// <seealso cref="Shuffle{T}(IList{T}, System.Random)"/>
        public static void Shuffle<T>(this IList<T> list)
        {
            Shuffle(list, new Random());
        }

        /// <summary>
        /// Moves every element of the list to a random new position in the list
        /// using the specified random number generator.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> to shuffle.</param>
        /// <param name="random">The random number generator.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> or <paramref name="random"/> is <c>null</c>.</exception>
        /// <seealso cref="Shuffle{T}(IList{T})"/>
        // Method found here http://stackoverflow.com/a/2301091
        // This shuffles the list in place without using LINQ, which is fast and efficient.
        public static void Shuffle<T>(this IList<T> list, System.Random random)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            for (int i = list.Count - 1; i > 0; i--)
            {
                int index = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[index];
                list[index] = temp;
            }
        }

        /// <summary>
        /// Swaps the elements of <paramref name="list"/> at indices <paramref name="index1"/>
        /// and <paramref name="index2"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The list to manipulate.</param>
        /// <param name="index1">Position of the first element to swap with the element in
        /// <paramref name="index2"/>.</param>
        /// <param name="index2">Position of the other element.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index1"/> or <paramref name="index2"/> is greater than <c><paramref name="list"/>.Count</c>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index1"/> or <paramref name="index2"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is <c>null</c>.</exception>
        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            //Contract.Requires<ArgumentNullException>(list != null);
            //Contract.Requires<ArgumentOutOfRangeException>(index1 >= 0 && index1 < list.Count);
            //Contract.Requires<ArgumentOutOfRangeException>(index2 >= 0 && index2 < list.Count);

            if (list == null)
                throw new ArgumentNullException(nameof(list));
            int size = list.Count;
            if (index1 < 0 || index1 > size)
                throw new ArgumentOutOfRangeException(nameof(index1));
            if (index2 < 0 || index2 > size)
                throw new ArgumentOutOfRangeException(nameof(index2));

            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }
    }
}
