using J2N.Text;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
#nullable enable

namespace J2N.Collections
{
    /// <summary>
    /// <see cref="Arrays"/> contains static methods which operate on arrays.
    /// This class is a supplement to <see cref="System.Array"/> to support
    /// additional methods that are available in Java but not in .NET.
    /// </summary>
    // J2N: This class currently only exists as a proxy for testing
    // behaviors of equality comparers and to serve as examples until
    // documentation can be written.
    // 
    // We should probably make any remaining Arrays functionality into
    // extension methods of single-dimensional arrays (in ArrayExtensions).
    internal static class Arrays
    {
        /// <summary>
        /// Returns <c>true</c> if the two given arrays are deeply equal to one another.
        /// Unlike the method <see cref="Equals{T}(T[], T[])"/>, this method
        /// is appropriate for use for nested arrays of arbitrary depth.
        /// <para/>
        /// Two array references are considered deeply equal if they are both <c>null</c>
        /// or if they refer to arrays that have the same length and the elements at
        /// each index in the two arrays are equal.
        /// </summary>
        /// <typeparam name="T">The type of array.</typeparam>
        /// <param name="arrayA">The array to be compared.</param>
        /// <param name="arrayB">The array to be compared with.</param>
        /// <returns><c>true</c> if both arrays are <c>null</c> or if the arrays have the
        /// same length and the elements at each index in the two arrays are
        /// equal according to <see cref="IEqualityComparer{T}.Equals(T, T)"/> of
        /// <see cref="EqualityComparer{T}.Default"/>; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool DeepEquals<T>(T[]? arrayA, T[]? arrayB)
        {
            return StructuralEqualityComparer.Default.Equals(arrayA, arrayB);
        }

        /// <summary>
        /// Compares the entire members of one array with another array.
        /// </summary>
        /// <typeparam name="T">The type of array.</typeparam>
        /// <param name="arrayA">The array to be compared.</param>
        /// <param name="arrayB">The array to be compared with.</param>
        /// <returns>Returns <c>true</c> if the two specified arrays of <typeparamref name="T"/> are equal
        /// to one another. The two arrays are considered equal if both arrays
        /// contain the same number of elements, and all corresponding pairs of
        /// elements in the two arrays are equal. Two objects <c>e1</c> and <c>e2</c> are
        /// considered equal if (<c>e1</c>==null ? <c>e2</c>==null : <c>e1</c>.Equals(<c>e2</c>)). In other
        /// words, the two arrays are equal if they contain the same elements in
        /// the same order. Also, two array references are considered equal if
        /// both are <c>null</c>.
        /// <para/>
        /// Note that if the type of <typeparamref name="T"/> is a <see cref="IDictionary{TKey, TValue}"/>,
        /// <see cref="IList{T}"/>, or <see cref="ISet{T}"/>, its values and any nested collection values
        /// will be compared for equality as well.
        /// </returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool Equals<T>(T[]? arrayA, T[]? arrayB)
        {
            return ArrayEqualityComparer<T>.OneDimensional.Equals(arrayA!, arrayB!);
        }

        /// <summary>
        /// Returns a hash code based on the "deep contents" of the given array. If
        /// the array contains other arrays as its elements, the hash code is based
        /// on their contents not their identities. So it is not acceptable to invoke
        /// this method on an array that contains itself as an element, either
        /// directly or indirectly.
        /// <para/>
        /// For any two arrays <c>a</c> and <c>b</c>, if
        /// <c>Arrays.DeepEquals(a, b)</c> returns <c>true</c>, it
        /// means that the return value of <c>Arrays.GetDeepHashCode(a)</c> equals
        /// <c>Arrays.GetDeepHashCode(b)</c>.
        /// <para/>
        /// The computation of the value returned by this method is similar to that
        /// of the value returned by <see cref="CollectionUtil.GetHashCode{T}(IList{T})"/> invoked on a
        /// <see cref="IList{T}"/> containing a sequence of instances representing the
        /// elements of array in the same order. The difference is: If an element <c>e</c>
        /// of array is itself an array, its hash code is computed by calling
        /// <c>Arrays.GetHashCode(e)</c> if <c>e</c> is an array of a
        /// value type, or by calling <c>Arrays.GetDeepHashCode(e)</c> recursively if <c>e</c> is
        /// an array of reference type. The value returned by this method is the
        /// same value as the method <c>StructuralComparison.StructuralEqualityComparer.Default.GetHashCode(array)</c>.
        /// If the array is <c>null</c>, the return value is <c>0</c> (zero).
        /// </summary>
        /// <typeparam name="T">The type of array.</typeparam>
        /// <param name="array">The array whose hash code to compute.</param>
        /// <returns>The deep hash code for <paramref name="array"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static int GetDeepHashCode<T>(T[]? array)
        {
            return StructuralEqualityComparer.Default.GetHashCode(array);
        }

        /// <summary>
        /// Returns a hash code based on the contents of the given array. For any two
        /// <typeparamref name="T"/> arrays <c>a</c> and <c>b</c>, if
        /// <c>Arrays.Equals(b)</c> returns <c>true</c>, it means
        /// that the return value of <c>Arrays.GetHashCode(a)</c> equals <c>Arrays.GetHashCode(b)</c>.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="array">The array whose hash code to compute.</param>
        /// <returns>The hash code for <paramref name="array"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static int GetHashCode<T>(T[]? array)
        {
            return ArrayEqualityComparer<T>.OneDimensional.GetHashCode(array!); // J2N TODO: array can be null here, but need to override the constraint
        }

        // J2N TODO: DeepToString()

        /// <summary>
        /// Creates a <see cref="string"/> representation of the array passed.
        /// The result is surrounded by brackets <c>"[]"</c>, each
        /// element is converted to a <see cref="string"/> via the
        /// <see cref="StringFormatter.InvariantCulture"/> and separated by <c>", "</c>. If
        /// the array is <c>null</c>, then <c>"null"</c> is returned.
        /// </summary>
        /// <typeparam name="T">The type of array element.</typeparam>
        /// <param name="array">The array to convert.</param>
        public static string ToString<T>(T[]? array)
            => ToString(array, StringFormatter.CurrentCulture);

        /// <summary>
        /// Creates a <see cref="string"/> representation of the array passed.
        /// The result is surrounded by brackets <c>"[]"</c>, each
        /// element is converted to a <see cref="string"/> via the
        /// <see cref="StringFormatter.InvariantCulture"/> and separated by <c>", "</c>. If
        /// the array is <c>null</c>, then <c>"null"</c> is returned.
        /// </summary>
        /// <typeparam name="T">The type of array element.</typeparam>
        /// <param name="array">The array to convert.</param>
        /// <param name="provider">The format provider. If <c>null</c>, will use <see cref="StringFormatter.CurrentCulture"/></param>
        public static string ToString<T>(T[]? array, IFormatProvider? provider)
        {
            if (array == null)
                return "null"; //$NON-NLS-1$
            if (array.Length == 0)
                return "[]"; //$NON-NLS-1$

            provider ??= StringFormatter.CurrentCulture;
            StringBuilder sb = new StringBuilder(2 + array.Length * 4);
            sb.Append('[');
            sb.AppendFormat(provider, "{0}", array[0]);
            for (int i = 1; i < array.Length; i++)
            {
                sb.Append(", "); //$NON-NLS-1$
                sb.AppendFormat(provider, "{0}", array[i]);
            }
            sb.Append(']');
            return sb.ToString();
        }

        /// <summary>
        /// Creates a <see cref="string"/> representation of the array passed.
        /// The result is surrounded by brackets <c>"[]"</c>, each
        /// element is converted to a <see cref="string"/> via the
        /// <see cref="StringFormatter.InvariantCulture"/> and separated by <c>", "</c>. If
        /// the array is <c>null</c>, then <c>"null"</c> is returned.
        /// </summary>
        /// <param name="array">The array to convert.</param>
        public static string ToString(Array? array)
            => ToString(array, StringFormatter.CurrentCulture);

        /// <summary>
        /// Creates a <see cref="string"/> representation of the array passed.
        /// The result is surrounded by brackets <c>"[]"</c>, each
        /// element is converted to a <see cref="string"/> via the
        /// <see cref="StringFormatter.InvariantCulture"/> and separated by <c>", "</c>. If
        /// the array is <c>null</c>, then <c>"null"</c> is returned.
        /// </summary>
        /// <param name="array">The array to convert.</param>
        /// <param name="provider">The format provider. If <c>null</c>, will use <see cref="StringFormatter.CurrentCulture"/></param>
        public static string ToString(Array? array, IFormatProvider? provider)
        {
            if (array == null)
                return "null"; //$NON-NLS-1$
            if (array.Length == 0)
                return "[]"; //$NON-NLS-1$

            provider ??= StringFormatter.CurrentCulture;
            StringBuilder sb = new StringBuilder(2 + array.Length * 4);
            sb.Append('[');
            sb.AppendFormat(provider, "{0}", array.GetValue(0));
            for (int i = 1; i < array.Length; i++)
            {
                sb.Append(", "); //$NON-NLS-1$
                sb.AppendFormat(provider, "{0}", array.GetValue(i));
            }
            sb.Append(']');
            return sb.ToString();
        }

        ///// <summary>
        ///// This is the same implementation of ToString from Java's AbstractCollection
        ///// (the default implementation for all sets and lists), plus the ability
        ///// to specify culture for formatting of nested numbers and dates. Note that
        ///// this overload will change the culture of the current thread.
        ///// </summary>
        //public static string ToString<T>(T[] array, IFormatProvider provider)
        //{
        //    //return CollectionUtil.ToString((IList<T>)array, culture);
        //}

        ///// <summary>
        ///// Assigns the specified value to each element of the specified array.
        ///// </summary>
        ///// <typeparam name="T">The type of the array</typeparam>
        ///// <param name="a">The array to be filled</param>
        ///// <param name="val">The value to be stored in all elements of the array</param>
        //public static void Fill<T>(T[] a, T val)
        //{
        //    ArrayExtensions.Fill(a, val);
        //}

        ///// <summary>
        ///// Assigns the specified value to each element of the specified array.
        ///// </summary>
        ///// <typeparam name="T">The type of the array</typeparam>
        ///// <param name="a">The array to be filled</param>
        ///// <param name="fromIndex">The index of the first element (inclusive) to be filled with the specified value</param>
        ///// <param name="toIndex">The index of the last element (exclusive) to be filled with the specified value</param>
        ///// <param name="val">The value to be stored in all elements of the array</param>
        //public static void Fill<T>(T[] a, int fromIndex, int toIndex, T val)
        //{
        //    if (fromIndex > toIndex)
        //        throw new ArgumentException(
        //            "fromIndex(" + fromIndex + ") > toIndex(" + toIndex + ")");
        //    if (fromIndex < 0)
        //        throw new ArgumentOutOfRangeException(nameof(fromIndex));
        //    if (toIndex > a.Length)
        //        throw new ArgumentOutOfRangeException(nameof(toIndex));

        //    for (int i = fromIndex; i < toIndex; i++)
        //        a[i] = val;
        //}

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


        /// <summary>
        /// Returns an empty array.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <returns>An empty array.</returns>
        // J2N: Since Array.Empty<T>() doesn't exist in all supported platforms, we
        // have this wrapper method to add support.
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static T[] Empty<T>()
        {
#if FEATURE_ARRAYEMPTY
            return Array.Empty<T>();
#else
            return EmptyArrayHolder<T>.Empty;
#endif
        }

        private static class EmptyArrayHolder<T>
        {
#pragma warning disable CA1825 // Avoid zero-length array allocations.
            public static readonly T[] Empty = new T[0];
#pragma warning restore CA1825 // Avoid zero-length array allocations.
        }
    }
}
