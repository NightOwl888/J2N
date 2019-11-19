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
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return new UnmodifiableCollection<T>(collection);
        }

        #region Nested Type: UnmodifiableCollection

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        private class UnmodifiableCollection<T> : ICollection<T>, IEquatable<ICollection<T>>
        {
            private readonly ICollection<T> collection;

            public UnmodifiableCollection(ICollection<T> collection)
            {
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
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

            public bool Equals(ICollection<T> other)
            {
                if (other == null)
                    return false;

                return CollectionUtil.Equals(collection, other);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                return CollectionUtil.Equals(collection, obj);
            }

            public override int GetHashCode()
            {
                return CollectionUtil.GetHashCode(collection);
            }

            public override string ToString()
            {
                return CollectionUtil.ToString(this);
            }
        }

        #endregion
    }
}
