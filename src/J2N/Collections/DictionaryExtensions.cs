using System;
using System.Collections;
using System.Collections.Generic;

namespace J2N.Collections
{
    /// <summary>
    /// Extensions to the <see cref="IDictionary{TKey, TValue}"/> interface.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Returns a wrapper on the specified <paramref name="dictionary"/> which throws a
        /// <see cref="NotSupportedException"/> whenever an attempt is made to modify the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary to wrap in an unmodifiable dictionary.</param>
        /// <returns>An unmodifiable <see cref="IDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <seealso cref="CollectionExtensions.ToUnmodifiableCollection{T}(ICollection{T})"/>
        /// <seealso cref="ListExtensions.ToUnmodifiableList{T}(IList{T})"/>
        /// <seealso cref="SetExtensions.ToUnmodifiableSet{T}(ISet{T})"/>
        public static IDictionary<TKey, TValue> ToUnmodifiableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return new UnmodifiableDictionary<TKey, TValue>(dictionary);
        }

        #region Nested Type: UnmodifiableDictionary<TKey, TValue>

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        private class UnmodifiableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEquatable<IDictionary<TKey, TValue>>
        {
            private readonly IDictionary<TKey, TValue> dictionary;

            public UnmodifiableDictionary(IDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public void Add(TKey key, TValue value)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public bool ContainsKey(TKey key)
            {
                return dictionary.ContainsKey(key);
            }

            public ICollection<TKey> Keys => dictionary.Keys;

            public bool Remove(TKey key)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

            public ICollection<TValue> Values => dictionary.Values;

            public TValue this[TKey key]
            {
                get => dictionary[key];
                set => throw new NotSupportedException("Collection is read-only.");
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only."); ;
            }

            public bool Contains(KeyValuePair<TKey, TValue> item) => dictionary.Contains(item);

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => dictionary.CopyTo(array, arrayIndex);

            public int Count => dictionary.Count;

            public bool IsReadOnly => true;

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


            public bool Equals(IDictionary<TKey, TValue> other)
            {
                if (other == null)
                    return false;

                return CollectionUtil.Equals(dictionary, other);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is IDictionary<TKey, TValue>))
                    return false;

                return CollectionUtil.Equals(dictionary, obj as IDictionary<TKey, TValue>);
            }

            public override int GetHashCode()
            {
                return CollectionUtil.GetHashCode(dictionary);
            }

            public override string ToString()
            {
                return CollectionUtil.ToString(this);
            }
        }

        #endregion UnmodifiableDictionary<TKey, TValue>
    }
}
