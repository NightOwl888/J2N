using J2N.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#nullable enable

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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static ReadOnlyList<T> AsReadOnly<T>(this IList<T> collection)
        {
            return new ReadOnlyList<T>(collection);
        }

        /// <summary>
        /// Performs a binary search for the specified element in the specified
        /// sorted list. The list needs to be already sorted in natural sorting
        /// order. Searching in an unsorted list has an undefined result. It's also
        /// undefined which element is found if there are multiple occurrences of the
        /// same element.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The sorted list to search.</param>
        /// <param name="item">The element to find.</param>
        /// <returns>The non-negative index of the element, or a negative index which
        /// is the <c>-index - 1</c> where the element would be inserted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is <c>null</c>.</exception>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int BinarySearch<T>(this IList<T> list, T item)
            => BinarySearch(list, item, null);

        /// <summary>
        /// Performs a binary search for the specified element in the specified
        /// sorted list using the specified comparer. The list needs to be already
        /// sorted according to the <paramref name="comparer"/> passed. Searching in an unsorted list
        /// has an undefined result. It's also undefined which element is found if
        /// there are multiple occurrences of the same element.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The sorted <see cref="IList{T}"/> to search.</param>
        /// <param name="item">The element to find.</param>
        /// <param name="comparer">The comparer. If the comparer is <c>null</c> then the
        /// search uses the objects' natural ordering.</param>
        /// <returns>the non-negative index of the element, or a negative index which
        /// is the <c>-index - 1</c> where the element would be inserted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is <c>null</c>.</exception>
        public static int BinarySearch<T>(this IList<T> list, T item, IComparer<T>? comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (comparer == null)
            {
                comparer = J2N.Collections.Generic.Comparer<T>.Default;
            }

            if (list.Count == 0)
                return -1;

            int low = 0, mid = list.Count, high = mid - 1, result = -1;
            while (low <= high)
            {
                mid = (low + high) >> 1;
                if ((result = -comparer.Compare(list[mid], item)) > 0)
                    low = mid + 1;
                else if (result == 0)
                    return mid;
                else
                    high = mid - 1;
            }
            return -mid - (result < 0 ? 1 : 2);
        }

        /// <summary>
        /// Performs a binary search for the specified element in the specified
        /// sorted list using the specified comparer. The list needs to be already
        /// sorted according to the <paramref name="comparer"/> passed. Searching in an unsorted list
        /// has an undefined result. It's also undefined which element is found if
        /// there are multiple occurrences of the same element.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The sorted <see cref="IList{T}"/> to search.</param>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The element to find.</param>
        /// <param name="comparer">The comparer. If the comparer is <c>null</c> then the
        /// search uses the objects' natural ordering.</param>
        /// <returns>the non-negative index of the element, or a negative index which
        /// is the <c>-index - 1</c> where the element would be inserted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is <c>null</c>.</exception>
        public static int BinarySearch<T>(this IList<T> list, int index, int count, T item, IComparer<T>? comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (list.Count - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (comparer == null)
            {
                comparer = J2N.Collections.Generic.Comparer<T>.Default;
            }

            int lo = index;
            int hi = index + count - 1;
            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);
                int order = comparer.Compare(list[i], item);

                if (order == 0) return i;
                if (order < 0)
                {
                    lo = i + 1;
                }
                else
                {
                    hi = i - 1;
                }
            }

            return ~lo;
        }

        /// <summary>
        /// Moves every element of the list to a random new position in the <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> to shuffle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is <c>null</c>.</exception>
        /// <seealso cref="Shuffle{T}(IList{T}, System.Random)"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
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
