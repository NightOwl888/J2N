using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace J2N.Collections.Generic.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="ISet{T}"/> interface.
    /// </summary>
    public static class SetExtensions
    {
        /// <summary>
        /// Returns a wrapper on the specified <paramref name="set"/> which throws a
        /// <see cref="NotSupportedException"/> whenever an attempt is made to
        /// modify the set.
        /// <para/>
        /// Usage Note: By default, the returned collection uses <see cref="StringFormatter.CurrentCulture"/>
        /// in the <see cref="Object.ToString()"/> method. To exactly match Java, call the
        /// <see cref="ToUnmodifiableSet{T}(ISet{T}, IFormatProvider)"/> overload and pass
        /// <see cref="StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// The structural equality method that is used depends on the collection that is passed in:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the collection implements <see cref="IStructuralEquatable"/>, it is assumed
        ///         that all nested types (such as collections) also implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="SetEqualityComparer{T}.Default"/> comparison is used.
        ///     </description></item>
        ///     <item><description>
        ///         If the collection does not implement <see cref="IStructuralEquatable"/>, it is assumed
        ///         that no nested types (such as collections) implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="SetEqualityComparer{T}.Aggressive"/> comparison is used.
        ///     </description></item>
        /// </list>
        /// While aggressive structural comparision patches the built-in .NET collections so the will structually
        /// compare each other, it involves some reflection and dynamic conversion that makes it slower than
        /// if types implement <see cref="IStructuralEquatable"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="set">The set to wrap in an unmodifiable set.</param>
        /// <returns>An unmodifiable <see cref="ISet{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="set"/> is <c>null</c>.</exception>
        /// <seealso cref="CollectionExtensions.ToUnmodifiableCollection{T}(ICollection{T})"/>
        /// <seealso cref="DictionaryExtensions.ToUnmodifiableDictionary{TKey, TValue}(IDictionary{TKey, TValue})"/>
        /// <seealso cref="ListExtensions.ToUnmodifiableList{T}(IList{T})"/>
        public static ISet<T> ToUnmodifiableSet<T>(this ISet<T> set)
        {
            return ToUnmodifiableSet(set, StringFormatter.CurrentCulture);
        }

        /// <summary>
        /// Returns a wrapper on the specified <paramref name="set"/> which throws a
        /// <see cref="NotSupportedException"/> whenever an attempt is made to
        /// modify the set.
        /// <para/>
        /// Usage Note: To exactly match Java behavior in <see cref="Object.ToString()"/> of the nested collection,
        /// pass <see cref="StringFormatter.InvariantCulture"/> in the <paramref name="toStringFormatProvider"/> argument.
        /// <para/>
        /// The structural equality method that is used depends on the collection that is passed in:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the collection implements <see cref="IStructuralEquatable"/>, it is assumed
        ///         that all nested types (such as collections) also implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="SetEqualityComparer{T}.Default"/> comparison is used.
        ///     </description></item>
        ///     <item><description>
        ///         If the collection does not implement <see cref="IStructuralEquatable"/>, it is assumed
        ///         that no nested types (such as collections) implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="SetEqualityComparer{T}.Aggressive"/> comparison is used.
        ///     </description></item>
        /// </list>
        /// While aggressive structural comparision patches the built-in .NET collections so the will structually
        /// compare each other, it involves some reflection and dynamic conversion that makes it slower than
        /// if types implement <see cref="IStructuralEquatable"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="set">The set to wrap in an unmodifiable set.</param>
        /// <param name="toStringFormatProvider">An object that provides formatting rules for the default <see cref="object.ToString()"/> method of the collection.</param>
        /// <returns>An unmodifiable <see cref="ISet{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="set"/> or <paramref name="toStringFormatProvider"/> is <c>null</c>.</exception>
        /// <seealso cref="CollectionExtensions.ToUnmodifiableCollection{T}(ICollection{T}, IFormatProvider)"/>
        /// <seealso cref="DictionaryExtensions.ToUnmodifiableDictionary{TKey, TValue}(IDictionary{TKey, TValue}, IFormatProvider)"/>
        /// <seealso cref="ListExtensions.ToUnmodifiableList{T}(IList{T}, IFormatProvider)"/>
        public static ISet<T> ToUnmodifiableSet<T>(this ISet<T> set, IFormatProvider toStringFormatProvider)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));
            if (toStringFormatProvider == null)
                throw new ArgumentNullException(nameof(toStringFormatProvider));

            // We assume that if the passed in collection is IStructuralEquatable, the end user took care of ensuring that
            // all nested collections also implement IStructuralEquatable. Otherwise, we assume that none of them are,
            // and use aggressive comparing.
            var comparer = set is IStructuralEquatable ?
                SetEqualityComparer<T>.Default :
                SetEqualityComparer<T>.Aggressive;

            return new UnmodifiableSet<T>(set, comparer, toStringFormatProvider);
        }

        #region Nested Type: UnmodifiableSet<T>

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class UnmodifiableSet<T> : ISet<T>, IStructuralEquatable
        {
            internal readonly ISet<T> set; // internal for testing
            private readonly SetEqualityComparer<T> structuralEqualityComparer;
            private readonly IFormatProvider toStringFormatProvider;
            public UnmodifiableSet(ISet<T> set, SetEqualityComparer<T> structuralEqualityComparer, IFormatProvider toStringFormatProvider)
            {
                this.set = set ?? throw new ArgumentNullException(nameof(set));
                this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
                this.toStringFormatProvider = toStringFormatProvider ?? throw new ArgumentNullException(nameof(toStringFormatProvider));
            }
            public int Count => set.Count;

            public bool IsReadOnly => true;

            public void Add(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public bool Contains(T item) => set.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => set.CopyTo(array, arrayIndex);

            public void ExceptWith(IEnumerable<T> other)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public IEnumerator<T> GetEnumerator() => set.GetEnumerator();

            public void IntersectWith(IEnumerable<T> other)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public bool IsProperSubsetOf(IEnumerable<T> other) => set.IsProperSubsetOf(other);

            public bool IsProperSupersetOf(IEnumerable<T> other) => set.IsProperSupersetOf(other);

            public bool IsSubsetOf(IEnumerable<T> other) => set.IsSubsetOf(other);

            public bool IsSupersetOf(IEnumerable<T> other) => set.IsSupersetOf(other);

            public bool Overlaps(IEnumerable<T> other) => set.Overlaps(other);

            public bool Remove(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public bool SetEquals(IEnumerable<T> other) => set.SetEquals(other);

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void UnionWith(IEnumerable<T> other)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            bool ISet<T>.Add(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public bool Equals(object other, IEqualityComparer comparer)
            {
                if (set is IStructuralEquatable se)
                    return se.Equals(other, comparer);
                if (other is ISet<T> otherSet)
                    return structuralEqualityComparer.Equals(set, otherSet);
                return false;
            }

            public int GetHashCode(IEqualityComparer comparer)
            {
                if (set is IStructuralEquatable se)
                    return se.GetHashCode(comparer);
                return structuralEqualityComparer.GetHashCode(set);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj is ISet<T> other)
                    return structuralEqualityComparer.Equals(set, other);
                return false;
            }

            public override int GetHashCode()
            {
                return structuralEqualityComparer.GetHashCode(set);
            }

            public override string ToString()
            {
                return string.Format(toStringFormatProvider, "{0}", set);
            }
        }

        #endregion
    }
}
