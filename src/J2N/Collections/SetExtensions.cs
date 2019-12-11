using System;
using System.Collections;
using System.Collections.Generic;

namespace J2N.Collections
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
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return new UnmodifiableSet<T>(set);
        }

        #region Nested Type: UnmodifiableSet<T>

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class UnmodifiableSet<T> : ISet<T>, IEquatable<ISet<T>>
        {
            internal readonly ISet<T> set; // internal for testing
            public UnmodifiableSet(ISet<T> set)
            {
                this.set = set ?? throw new ArgumentNullException(nameof(set));
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

            public bool Equals(ISet<T> other)
            {
                if (other == null)
                    return false;

                return CollectionUtil.Equals(set, other);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ISet<T>))
                    return false;

                return CollectionUtil.Equals(set, obj as ISet<T>);
            }

            public override int GetHashCode()
            {
                return CollectionUtil.GetHashCode(set);
            }

            public override string ToString()
            {
                return CollectionUtil.ToString(this);
            }
        }

        #endregion
    }
}
