using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace J2N.Collections.Generic.Extensions
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Extensions to the <see cref="IList{T}"/> interface.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Moves every element of the list to a random new position in the <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> to shuffle.</param>
        /// <exception cref="NotSupportedException">If <paramref name="list"/> is read-only.</exception>
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
        /// <exception cref="NotSupportedException">If <paramref name="list"/> is read-only.</exception>
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
            if (list.IsReadOnly)
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);

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
        /// <exception cref="NotSupportedException">If <paramref name="list"/> is read-only.</exception>
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
            if (list.IsReadOnly)
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);

            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }

        // J2N TODO: API - if we make this public, it will match .NET's API.
        // It would also make sense to add an AsReadOnly() extension method to
        // ICollection<T>, IDictionary<TKey, TValue> and ISet<T>. This is a breaking change,
        // though, so need to work this in at the appropriate time.

        // Downside: Since the AsReadOnly() method is a concrete method of SCG.List<T> and there
        // is also an AsReadOnly() method for array types, this
        // extension method won't work for those cases. Need to consider the right approach.
        // Perhaps we should make an overload that can be used in those cases, such as AsReadOnly(bool structurallyComparable)?
        // Whatever we decide, the ToList() and ToArray() methods from LINQ also need a similar solution.
        internal static UnmodifiableList<T> AsReadOnly<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            // We assume that if the passed in collection is IStructuralEquatable, the end user took care of ensuring that
            // all nested collections also implement IStructuralEquatable. Otherwise, we assume that none of them are,
            // and use aggressive comparing.
            var comparer = list is IStructuralEquatable ?
                ListEqualityComparer<T>.Default :
                ListEqualityComparer<T>.Aggressive;

            return new UnmodifiableList<T>(list, comparer, StringFormatter.CurrentCulture);
        }


        /// <summary>
        /// Returns a wrapper on the specified list which throws an
        /// <see cref="NotSupportedException"/> whenever an attempt is made to
        /// modify the list.
        /// <para/>
        /// Usage Note: By default, the returned collection uses <see cref="StringFormatter.CurrentCulture"/>
        /// in the <see cref="Object.ToString()"/> method. To exactly match Java, call the
        /// <see cref="ToUnmodifiableList{T}(IList{T}, IFormatProvider)"/> overload and pass
        /// <see cref="StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// The structural equality method that is used depends on the collection that is passed in:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the collection implements <see cref="IStructuralEquatable"/>, it is assumed
        ///         that all nested types (such as collections) also implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="ListEqualityComparer{T}.Default"/> comparison is used.
        ///     </description></item>
        ///     <item><description>
        ///         If the collection does not implement <see cref="IStructuralEquatable"/>, it is assumed
        ///         that no nested types (such as collections) implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="ListEqualityComparer{T}.Aggressive"/> comparison is used.
        ///     </description></item>
        /// </list>
        /// While aggressive structural comparision patches the built-in .NET collections so the will structually
        /// compare each other, it involves some reflection and dynamic conversion that makes it slower than
        /// if types implement <see cref="IStructuralEquatable"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The list to wrap in an unmodifiable list.</param>
        /// <returns>An unmodifiable <see cref="IList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is <c>null</c>.</exception>
        /// <seealso cref="CollectionExtensions.ToUnmodifiableCollection{T}(ICollection{T})"/>
        /// <seealso cref="DictionaryExtensions.ToUnmodifiableDictionary{TKey, TValue}(IDictionary{TKey, TValue})"/>
        /// <seealso cref="SetExtensions.ToUnmodifiableSet{T}(ISet{T})"/>
        public static IList<T> ToUnmodifiableList<T>(this IList<T> list)
        {
            return ToUnmodifiableList(list, StringFormatter.CurrentCulture);
        }

        /// <summary>
        /// Returns a wrapper on the specified list which throws an
        /// <see cref="NotSupportedException"/> whenever an attempt is made to
        /// modify the list.
        /// <para/>
        /// Usage Note: To exactly match Java behavior in <see cref="Object.ToString()"/> of the nested collection,
        /// pass <see cref="StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// The structural equality method that is used depends on the collection that is passed in:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the collection implements <see cref="IStructuralEquatable"/>, it is assumed
        ///         that all nested types (such as collections) also implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="ListEqualityComparer{T}.Default"/> comparison is used.
        ///     </description></item>
        ///     <item><description>
        ///         If the collection does not implement <see cref="IStructuralEquatable"/>, it is assumed
        ///         that no nested types (such as collections) implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="ListEqualityComparer{T}.Aggressive"/> comparison is used.
        ///     </description></item>
        /// </list>
        /// While aggressive structural comparision patches the built-in .NET collections so the will structually
        /// compare each other, it involves some reflection and dynamic conversion that makes it slower than
        /// if types implement <see cref="IStructuralEquatable"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The list to wrap in an unmodifiable list.</param>
        /// <param name="toStringFormatProvider">An object that provides formatting rules for the default <see cref="object.ToString()"/> method of the collection.</param>
        /// <returns>An unmodifiable <see cref="IList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> or <paramref name="toStringFormatProvider"/> is <c>null</c>.</exception>
        /// <seealso cref="CollectionExtensions.ToUnmodifiableCollection{T}(ICollection{T}, IFormatProvider)"/>
        /// <seealso cref="DictionaryExtensions.ToUnmodifiableDictionary{TKey, TValue}(IDictionary{TKey, TValue}, IFormatProvider)"/>
        /// <seealso cref="SetExtensions.ToUnmodifiableSet{T}(ISet{T}, IFormatProvider)"/>
        public static IList<T> ToUnmodifiableList<T>(this IList<T> list, IFormatProvider toStringFormatProvider)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (toStringFormatProvider == null)
                throw new ArgumentNullException(nameof(toStringFormatProvider));

            // We assume that if the passed in collection is IStructuralEquatable, the end user took care of ensuring that
            // all nested collections also implement IStructuralEquatable. Otherwise, we assume that none of them are,
            // and use aggressive comparing.
            var comparer = list is IStructuralEquatable ?
                ListEqualityComparer<T>.Default :
                ListEqualityComparer<T>.Aggressive;

            return new UnmodifiableList<T>(list, comparer, toStringFormatProvider);
        }

        #region Nested Type: UnmodifiableList<T>

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class UnmodifiableList<T> : ReadOnlyCollection<T>, IStructuralEquatable, IStructuralFormattable
        {
            private readonly ListEqualityComparer<T> structuralEqualityComparer;
            private readonly IFormatProvider toStringFormatProvider;

            public UnmodifiableList(IList<T> list, ListEqualityComparer<T> structuralEqualityComparer, IFormatProvider toStringFormatProvider)
                : base(list)
            {
                this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
                this.toStringFormatProvider = toStringFormatProvider ?? throw new ArgumentNullException(nameof(toStringFormatProvider));
            }

            internal IList<T> List => base.Items; // for testing

            public bool Equals(object other, IEqualityComparer comparer)
            {
                if (Items is IStructuralEquatable se)
                    return se.Equals(other, comparer);
                if (other is IList<T> otherList)
                    return structuralEqualityComparer.Equals(Items, otherList);
                return false;
            }

            public int GetHashCode(IEqualityComparer comparer)
            {
                if (Items is IStructuralEquatable se)
                    return se.GetHashCode(comparer);
                return structuralEqualityComparer.GetHashCode(Items);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj is IList<T> other)
                    return structuralEqualityComparer.Equals(Items, other);
                return false;
            }

            public override int GetHashCode()
            {
                return structuralEqualityComparer.GetHashCode(Items);
            }

            public override string ToString()
            {
                return ToString("{0}", toStringFormatProvider);
            }

            public string ToString(string format, IFormatProvider provider)
            {
                if (Items is IStructuralFormattable formattable)
                    return formattable.ToString(format, provider);
                return CollectionUtil.ToString(provider, format, Items);
            }
        }

        #endregion UnmodifiableList<T>
    }
}
