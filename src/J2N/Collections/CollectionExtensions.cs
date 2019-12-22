using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace J2N.Collections
{
    /// <summary>
    /// Extensions to the <see cref="ICollection{T}"/> interface.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns a wrapper on the specified collection which throws a
        /// <see cref="NotSupportedException"/> whenever an attempt is made to
        /// modify the collection.
        /// <para/>
        /// Usage Note: By default, the returned collection uses <see cref="StringFormatter.CurrentCulture"/>
        /// in the <see cref="Object.ToString()"/> method. To exactly match Java, call the
        /// <see cref="ToUnmodifiableCollection{T}(ICollection{T}, IFormatProvider)"/> overload and pass
        /// <see cref="StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// The structural equality method that is used depends on the collection that is passed in:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the collection implements <see cref="IStructuralEquatable"/>, it is assumed
        ///         that all nested types (such as collections) also implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="StructuralEqualityComparer.Default"/> comparison is used.
        ///     </description></item>
        ///     <item><description>
        ///         If the collection does not implement <see cref="IStructuralEquatable"/>, it is assumed
        ///         that no nested types (such as collections) implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="StructuralEqualityComparer.Aggressive"/> comparison is used.
        ///     </description></item>
        /// </list>
        /// While aggressive structural comparision patches the built-in .NET collections so the will structually
        /// compare each other, it involves some reflection and dynamic conversion that makes it slower than
        /// if types implement <see cref="IStructuralEquatable"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collection">The collection to wrap in an unmodifiable collection.</param>
        /// <returns>An unmodifiable collection.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="collection"/> is <c>null</c>.</exception>
        /// <seealso cref="DictionaryExtensions.ToUnmodifiableDictionary{TKey, TValue}(IDictionary{TKey, TValue})"/>
        /// <seealso cref="ListExtensions.ToUnmodifiableList{T}(IList{T})"/>
        /// <seealso cref="SetExtensions.ToUnmodifiableSet{T}(ISet{T})"/>
        public static ICollection<T> ToUnmodifiableCollection<T>(this ICollection<T> collection)
        {
            return ToUnmodifiableCollection(collection, StringFormatter.CurrentCulture);
        }

        /// <summary>
        /// Returns a wrapper on the specified collection which throws a
        /// <see cref="NotSupportedException"/> whenever an attempt is made to
        /// modify the collection.
        /// <para/>
        /// Usage Note: To exactly match Java behavior in <see cref="Object.ToString()"/> of the nested collection,
        /// pass <see cref="StringFormatter.InvariantCulture"/> in the <paramref name="toStringFormatProvider"/> argument.
        /// <para/>
        /// The structural equality method that is used depends on the collection that is passed in:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the collection implements <see cref="IStructuralEquatable"/>, it is assumed
        ///         that all nested types (such as collections) also implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="StructuralEqualityComparer.Default"/> comparison is used.
        ///     </description></item>
        ///     <item><description>
        ///         If the collection does not implement <see cref="IStructuralEquatable"/>, it is assumed
        ///         that no nested types (such as collections) implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="StructuralEqualityComparer.Aggressive"/> comparison is used.
        ///     </description></item>
        /// </list>
        /// While aggressive structural comparision patches the built-in .NET collections so the will structually
        /// compare each other, it involves some reflection and dynamic conversion that makes it slower than
        /// if types implement <see cref="IStructuralEquatable"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collection">The collection to wrap in an unmodifiable collection.</param>
        /// <param name="toStringFormatProvider">An object that provides formatting rules for the default <see cref="object.ToString()"/> method of the collection.</param>
        /// <returns>An unmodifiable collection.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="collection"/> or <paramref name="toStringFormatProvider"/> is <c>null</c>.</exception>
        /// <seealso cref="DictionaryExtensions.ToUnmodifiableDictionary{TKey, TValue}(IDictionary{TKey, TValue}, IFormatProvider)"/>
        /// <seealso cref="ListExtensions.ToUnmodifiableList{T}(IList{T}, IFormatProvider)"/>
        /// <seealso cref="SetExtensions.ToUnmodifiableSet{T}(ISet{T}, IFormatProvider)"/>
        public static ICollection<T> ToUnmodifiableCollection<T>(this ICollection<T> collection, IFormatProvider toStringFormatProvider)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (toStringFormatProvider == null)
                throw new ArgumentNullException(nameof(toStringFormatProvider));

            // We assume that if the passed in collection is IStructuralEquatable, the end user took care of ensuring that
            // all nested collections also implement IStructuralEquatable. Otherwise, we assume that none of them are,
            // and use aggressive comparing.
            var comparer = collection is IStructuralEquatable ? 
                StructuralEqualityComparer.Default : 
                StructuralEqualityComparer.Aggressive;

            return new UnmodifiableCollection<T>(collection, comparer, toStringFormatProvider);
        }

        #region Nested Type: UnmodifiableCollection

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class UnmodifiableCollection<T> : ICollection<T>, IStructuralEquatable
        {
            internal readonly ICollection<T> collection; // internal for testing
            private readonly StructuralEqualityComparer structuralEqualityComparer;
            private readonly IFormatProvider toStringFormatProvider;

            public UnmodifiableCollection(ICollection<T> collection, StructuralEqualityComparer structuralEqualityComparer, IFormatProvider toStringFormatProvider)
            {
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
                this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
                this.toStringFormatProvider = toStringFormatProvider ?? throw new ArgumentNullException(nameof(toStringFormatProvider));
            }

            public int Count => collection.Count;

            public bool IsReadOnly => true;

            public void Add(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public bool Contains(T item) => collection.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

            public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

            public bool Remove(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public bool Equals(object other, IEqualityComparer comparer)
            {
                if (collection is IStructuralEquatable se)
                    return se.Equals(other, comparer);
                return structuralEqualityComparer.Equals(collection, other);
            }

            public int GetHashCode(IEqualityComparer comparer)
            {
                if (collection is IStructuralEquatable se)
                    return se.GetHashCode(comparer);
                return structuralEqualityComparer.GetHashCode(collection);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                return structuralEqualityComparer.Equals(collection, obj);
            }

            public override int GetHashCode()
            {
                return structuralEqualityComparer.GetHashCode(collection);
            }

            public override string ToString()
            {
                return string.Format(toStringFormatProvider, "{0}", collection);
            }
        }

        #endregion
    }
}
