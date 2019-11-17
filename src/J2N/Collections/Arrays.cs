using System;
using System.Collections.Generic;
using System.Globalization;

namespace J2N.Collections
{
    /// <summary>
    /// <see cref="Arrays"/> contains static methods which operate on arrays.
    /// This class is a supplement to <see cref="System.Array"/> to support
    /// additional methods that are available in Java but not in .NET.
    /// </summary>
    public static class Arrays
    {
        /// <summary>
        /// The same implementation of Equals from Java's AbstractList
        /// (the default implementation for all lists)
        /// <para/>
        /// This algorithm depends on the order of the items in the list. 
        /// It is recursive and will determine equality based on the values of
        /// all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        public static bool Equals<T>(IList<T> listA, IList<T> listB)
        {
            return CollectionUtil.Equals(listA, listB);
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractCollection
        /// (the default implementation for all sets and lists)
        /// </summary>
        public static string ToString<T>(T[] array)
        {
            return CollectionUtil.ToString((IList<T>)array);
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractCollection
        /// (the default implementation for all sets and lists), plus the ability
        /// to specify culture for formatting of nested numbers and dates. Note that
        /// this overload will change the culture of the current thread.
        /// </summary>
        public static string ToString<T>(T array, CultureInfo culture)
        {
            return CollectionUtil.ToString((IList<T>)array, culture);
        }

        /// <summary>
        /// Assigns the specified value to each element of the specified array.
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="a">the array to be filled</param>
        /// <param name="val">the value to be stored in all elements of the array</param>
        public static void Fill<T>(T[] a, T val)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = val;
            }
        }

        /// <summary>
        /// Assigns the specified value to each element of the specified array.
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam>
        /// <param name="a">the array to be filled</param>
        /// <param name="fromIndex">the index of the first element (inclusive) to be filled with the specified value</param>
        /// <param name="toIndex">he index of the last element (exclusive) to be filled with the specified value</param>
        /// <param name="val">the value to be stored in all elements of the array</param>
        public static void Fill<T>(T[] a, int fromIndex, int toIndex, T val)
        {
            if (fromIndex > toIndex)
                throw new ArgumentException(
                    "fromIndex(" + fromIndex + ") > toIndex(" + toIndex + ")");
            if (fromIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(fromIndex));
            if (toIndex > a.Length)
                throw new ArgumentOutOfRangeException(nameof(toIndex));

            for (int i = fromIndex; i < toIndex; i++)
                a[i] = val;
        }

        //public static T[][] NewRectangularArray<T>(int size1, int size2)
        //{
        //    T[][] array;
        //    if (size1 > -1)
        //    {
        //        array = new T[size1][];
        //        if (size2 > -1)
        //        {
        //            for (int array1 = 0; array1 < size1; array1++)
        //            {
        //                array[array1] = new T[size2];
        //            }
        //        }
        //    }
        //    else
        //        array = null;

        //    return array;
        //}
    }
}
