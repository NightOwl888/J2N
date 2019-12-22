using J2N.Collections.Generic;
using J2N.Text;
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
        /// <para/>
        /// Usage Note: By default, the returned collection uses <see cref="StringFormatter.CurrentCulture"/>
        /// in the <see cref="Object.ToString()"/> method. To exactly match Java, call the
        /// <see cref="ToUnmodifiableDictionary{TKey, TValue}(IDictionary{TKey, TValue}, IFormatProvider)"/> overload and pass
        /// <see cref="StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// The structural equality method that is used depends on the collection that is passed in:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the collection implements <see cref="IStructuralEquatable"/>, it is assumed
        ///         that all nested types (such as collections) also implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="DictionaryEqualityComparer{TKey, TValue}.Default"/> comparison is used.
        ///     </description></item>
        ///     <item><description>
        ///         If the collection does not implement <see cref="IStructuralEquatable"/>, it is assumed
        ///         that no nested types (such as collections) implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="DictionaryEqualityComparer{TKey, TValue}.Aggressive"/> comparison is used.
        ///     </description></item>
        /// </list>
        /// While aggressive structural comparision patches the built-in .NET collections so the will structually
        /// compare each other, it involves some reflection and dynamic conversion that makes it slower than
        /// if types implement <see cref="IStructuralEquatable"/>.
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
            return ToUnmodifiableDictionary(dictionary, StringFormatter.CurrentCulture);
        }

        /// <summary>
        /// Returns a wrapper on the specified <paramref name="dictionary"/> which throws a
        /// <see cref="NotSupportedException"/> whenever an attempt is made to modify the dictionary.
        /// <para/>
        /// Usage Note: To exactly match Java behavior in <see cref="Object.ToString()"/> of the nested collection,
        /// pass <see cref="StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// The structural equality method that is used depends on the collection that is passed in:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the collection implements <see cref="IStructuralEquatable"/>, it is assumed
        ///         that all nested types (such as collections) also implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="DictionaryEqualityComparer{TKey, TValue}.Default"/> comparison is used.
        ///     </description></item>
        ///     <item><description>
        ///         If the collection does not implement <see cref="IStructuralEquatable"/>, it is assumed
        ///         that no nested types (such as collections) implement <see cref="IStructuralEquatable"/>,
        ///         and <see cref="DictionaryEqualityComparer{TKey, TValue}.Aggressive"/> comparison is used.
        ///     </description></item>
        /// </list>
        /// While aggressive structural comparision patches the built-in .NET collections so the will structually
        /// compare each other, it involves some reflection and dynamic conversion that makes it slower than
        /// if types implement <see cref="IStructuralEquatable"/>.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary to wrap in an unmodifiable dictionary.</param>
        /// <param name="toStringFormatProvider">An object that provides formatting rules for the default <see cref="object.ToString()"/> method of the collection.</param>
        /// <returns>An unmodifiable <see cref="IDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="dictionary"/> or <paramref name="toStringFormatProvider"/> is <c>null</c>.</exception>
        /// <seealso cref="CollectionExtensions.ToUnmodifiableCollection{T}(ICollection{T}, IFormatProvider)"/>
        /// <seealso cref="ListExtensions.ToUnmodifiableList{T}(IList{T}, IFormatProvider)"/>
        /// <seealso cref="SetExtensions.ToUnmodifiableSet{T}(ISet{T}, IFormatProvider)"/>
        public static IDictionary<TKey, TValue> ToUnmodifiableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IFormatProvider toStringFormatProvider)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (toStringFormatProvider == null)
                throw new ArgumentNullException(nameof(toStringFormatProvider));

            // We assume that if the passed in collection is IStructuralEquatable, the end user took care of ensuring that
            // all nested collections also implement IStructuralEquatable. Otherwise, we assume that none of them are,
            // and use aggressive comparing.
            var comparer = dictionary is IStructuralEquatable ? 
                DictionaryEqualityComparer<TKey, TValue>.Default : 
                DictionaryEqualityComparer<TKey, TValue>.Aggressive;

            return new UnmodifiableDictionary<TKey, TValue>(dictionary, comparer, StringFormatter.CurrentCulture);
        }

        #region Nested Type: UnmodifiableDictionary<TKey, TValue>

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class UnmodifiableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IStructuralEquatable
        {
            internal readonly IDictionary<TKey, TValue> dictionary; // internal for testing
            private readonly DictionaryEqualityComparer<TKey, TValue> structuralEqualityComparer;
            private readonly IFormatProvider toStringFormatProvider;

            public UnmodifiableDictionary(IDictionary<TKey, TValue> dictionary, DictionaryEqualityComparer<TKey, TValue> structuralEqualityComparer, IFormatProvider toStringFormatProvider)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
                this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
                this.toStringFormatProvider = toStringFormatProvider ?? throw new ArgumentNullException(nameof(toStringFormatProvider));
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


            public bool Equals(object other, IEqualityComparer comparer)
            {
                if (dictionary is IStructuralEquatable se)
                    return se.Equals(other, comparer);
                if (other is IDictionary<TKey, TValue> otherDictionary)
                    return structuralEqualityComparer.Equals(dictionary, otherDictionary);
                return false;
            }

            public int GetHashCode(IEqualityComparer comparer)
            {
                if (dictionary is IStructuralEquatable se)
                    return se.GetHashCode(comparer);
                return structuralEqualityComparer.GetHashCode(dictionary);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj is IDictionary<TKey, TValue> other)
                    return structuralEqualityComparer.Equals(dictionary, other);
                return false;
            }

            public override int GetHashCode()
            {
                return structuralEqualityComparer.GetHashCode(dictionary);
            }

            public override string ToString()
            {
                return string.Format(toStringFormatProvider, "{0}", dictionary);
            }
        }

        #endregion UnmodifiableDictionary<TKey, TValue>
    }
}
