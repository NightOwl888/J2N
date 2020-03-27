using J2N.Collections.Generic;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#if FEATURE_CONTRACTBLOCKS
using System.Diagnostics.Contracts;
#endif
using System.Reflection;
#nullable enable

namespace J2N.Collections.ObjectModel
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Provides the base class for a generic read-only collection of key/value pairs that is structurally equatable.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    // NOTE: We cannot subclass System.Collection.ObjectModel.ReadOnlyDictionary<TKey, TValue> because there is a nullable constraint on the key and a check for it in code as well
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IStructuralEquatable, IStructuralFormattable
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    {
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
        private static readonly bool TKeyIsValueTypeOrStringOrStructuralEquatable = typeof(TKey).GetTypeInfo().IsValueType || typeof(IStructuralEquatable).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()) || typeof(string).Equals(typeof(TKey));
        private static readonly bool TValueIsValueTypeOrStringOrStructuralEquatable = typeof(TValue).GetTypeInfo().IsValueType || typeof(IStructuralEquatable).GetTypeInfo().IsAssignableFrom(typeof(TValue).GetTypeInfo()) || typeof(string).Equals(typeof(TValue));
#else
        private static readonly bool TKeyIsValueTypeOrStringOrStructuralEquatable = typeof(TKey).IsValueType || typeof(IStructuralEquatable).IsAssignableFrom(typeof(TKey)) || typeof(string).Equals(typeof(TKey));
        private static readonly bool TValueIsValueTypeOrStringOrStructuralEquatable = typeof(TValue).IsValueType || typeof(IStructuralEquatable).IsAssignableFrom(typeof(TValue)) || typeof(string).Equals(typeof(TValue));
#endif
        private static readonly bool TKeyIsNullable = typeof(TKey).IsNullableType();

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        internal readonly IDictionary<TKey, TValue> dictionary; // Internal for testing
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        private readonly DictionaryEqualityComparer<TKey, TValue> structuralEqualityComparer;
        private readonly IFormatProvider toStringFormatProvider;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private object? syncRoot;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private KeyCollection? keys;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private ValueCollection? values;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> class that is a wrapper around the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
           : this(dictionary,
                 TKeyIsValueTypeOrStringOrStructuralEquatable && TValueIsValueTypeOrStringOrStructuralEquatable ? 
                    DictionaryEqualityComparer<TKey, TValue>.Default :
                    DictionaryEqualityComparer<TKey, TValue>.Aggressive,
                 StringFormatter.CurrentCulture)
        {
        }

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        internal ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary, DictionaryEqualityComparer<TKey, TValue> structuralEqualityComparer, IFormatProvider toStringFormatProvider)
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
            this.toStringFormatProvider = toStringFormatProvider ?? throw new ArgumentNullException(nameof(toStringFormatProvider));
        }

        /// <summary>
        /// Gets the dictionary that is wrapped by this <see cref="ReadOnlyDictionary{TKey, TValue}"/> object.
        /// </summary>
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        protected internal IDictionary<TKey, TValue> Dictionary => dictionary;
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        /// <summary>
        /// Gets a key collection that contains the keys of the dictionary.
        /// </summary>
        public KeyCollection Keys
        {
            get
            {
#if FEATURE_CONTRACTBLOCKS
                Contract.Ensures(Contract.Result<KeyCollection>() != null);
#endif
                if (keys == null)
                {
                    keys = new KeyCollection(dictionary.Keys);
                }
                return keys;
            }
        }

        /// <summary>
        /// Gets a collection that contains the values in the dictionary.
        /// </summary>
        public ValueCollection Values
        {
            get
            {
#if FEATURE_CONTRACTBLOCKS
                Contract.Ensures(Contract.Result<ValueCollection>() != null);
#endif
                if (values == null)
                {
                    values = new ValueCollection(dictionary.Values);
                }
                return values;
            }
        }

        #region IDictionary<TKey, TValue> Members

        /// <summary>
        /// Determines whether the dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns><c>true</c> if the dictionary contains an element that has the specified key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        /// <summary>
        /// Retrieves the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value will be retrieved.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the object that implements <see cref="ReadOnlyDictionary{TKey, TValue}"/>
        /// contains an element with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        /// <summary>
        /// Gets the element that has the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The element that has the specified key.</returns>
        public TValue this[TKey key] => dictionary[key];

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => dictionary[key];
            set => throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        /// Gets the number of items in the dictionary.
        /// </summary>
        public int Count => dictionary.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IDictionary Members

        private static bool IsCompatibleKey(object? key)
        {
            if (key is null)
                return TKeyIsNullable;

            return key is TKey;
        }

        void IDictionary.Add(object? key, object? value)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void IDictionary.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        bool IDictionary.Contains(object key)
        {
            return IsCompatibleKey(key) && ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            if (dictionary is IDictionary d)
            {
                return d.GetEnumerator();
            }
            return new DictionaryEnumerator(dictionary);
        }

        bool IDictionary.IsFixedSize => true;

        bool IDictionary.IsReadOnly => true;

        ICollection IDictionary.Keys => Keys;

        void IDictionary.Remove(object key)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        ICollection IDictionary.Values => Values;

        object? IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    return this[(TKey)key];
                }
                return null;
            }
            set
            {
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException(SR.Arg_NonZeroLowerBound);
            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - index < Count)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            if (array is KeyValuePair<TKey, TValue>[] pairs)
            {
                dictionary.CopyTo(pairs, index);
            }
            else
            {
                if (array is DictionaryEntry[] dictEntryArray)
                {
                    foreach (var item in dictionary)
                    {
                        dictEntryArray[index++] = new DictionaryEntry(item.Key!, item.Value);
                    }
                }
                else
                {
                    if (!(array is object[] objects))
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType);
                    }

                    try
                    {
                        foreach (var item in dictionary)
                        {
                            objects[index++] = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType);
                    }
                }
            }
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                {
                    if (dictionary is ICollection c)
                    {
                        syncRoot = c.SyncRoot;
                    }
                    else
                    {
                        System.Threading.Interlocked.CompareExchange<object?>(ref syncRoot, new object(), null);
                    }
                }
                return syncRoot;
            }
        }

        #endregion

#if FEATURE_IREADONLYCOLLECTIONS
        #region IReadOnlyDictionary members
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        #endregion IReadOnlyDictionary members
#endif

        #region Structural Equality

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current dictionary
        /// using rules provided by the specified <paramref name="comparer"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to determine
        /// whether the current object and <paramref name="other"/> are structurally equal.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is structurally equal to the current dictionary;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public virtual bool Equals(object? other, IEqualityComparer comparer)
            => DictionaryEqualityComparer<TKey, TValue>.Equals(dictionary, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current dictionary using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current dictionary.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
            => DictionaryEqualityComparer<TKey, TValue>.GetHashCode(dictionary, comparer);

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current dictionary
        /// using rules similar to those in the JDK's AbstactMap class. Two dictionaries are considered
        /// equal when they both contain the same mapppings (in any order).
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="IDictionary{TKey, TValue}"/>
        /// and it contains the same elements; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
            => Equals(obj, structuralEqualityComparer);

        /// <summary>
        /// Gets the hash code for the current dictionary. The hash code is calculated 
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <seealso cref="GetHashCode(IEqualityComparer)"/>
        public override int GetHashCode()
            => GetHashCode(structuralEqualityComparer);

#endregion

#region ToString

        /// <summary>
        /// Returns a string that represents the current dictionary using the specified
        /// <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
            => CollectionUtil.ToString(formatProvider, format, dictionary);

        /// <summary>
        /// Returns a string that represents the current dictionary using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by curly
        /// brackets ("{}"). Keys and values are separated by '=',
        /// KeyValuePairs are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        public override string ToString()
            => ToString("{0}", toStringFormatProvider);


        /// <summary>
        /// Returns a string that represents the current dictionary using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current dictionary using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by curly
        /// brackets ("{}"). Keys and values are separated by '=',
        /// KeyValuePairs are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string format)
            => ToString(format, toStringFormatProvider);

        #endregion


        #region Nested Structure: DictionaryEnumerator

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        private struct DictionaryEnumerator : IDictionaryEnumerator
        {
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            private readonly IDictionary<TKey, TValue> dictionary;
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            private IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            public DictionaryEnumerator(IDictionary<TKey, TValue> dictionary)
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            {
                this.dictionary = dictionary;
                enumerator = this.dictionary.GetEnumerator();
            }

            public DictionaryEntry Entry
#pragma warning disable CS8604 // Possible null reference argument.
                => new DictionaryEntry(enumerator.Current.Key, enumerator.Current.Value);
#pragma warning restore CS8604 // Possible null reference argument.

#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
            public object? Key => enumerator.Current.Key;
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.

            public object? Value => enumerator.Current.Value;

            public object Current => Entry;

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }

        #endregion

        #region Nested Class: KeyCollection

        /// <summary>
        /// Represents a read-only collection of the keys of a <see cref="ReadOnlyDictionary{TKey, TValue}"/> object.
        /// </summary>
        [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
        [DebuggerDisplay("Count = {Count}")]
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        public sealed class KeyCollection : ICollection<TKey>, ICollection
#if FEATURE_IREADONLYCOLLECTIONS
            , IReadOnlyCollection<TKey>
#endif
        {
            private readonly ICollection<TKey> collection;
#if FEATURE_SERIALIZABLE
            [NonSerialized]
#endif
            private object? syncRoot;

            internal KeyCollection(ICollection<TKey> collection)
            {
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

#region ICollection<T> Members

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                return collection.Contains(item);
            }

            /// <summary>
            /// Copies the elements of the collection to an array, starting at a specific array index.
            /// </summary>
            /// <param name="array">The one-dimensional array that is the destination of the elements
            /// copied from the collection. The array must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
            /// <exception cref="ArgumentException">
            /// <paramref name="array"/> is multidimensional.
            /// <para/>
            /// -or-
            /// <para/>
            /// The number of elements in the source collection is greater than the available space from
            /// <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
            /// <para/>
            /// -or-
            /// <para/>
            /// Type <typeparamref name="TKey"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
            /// </exception>
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                collection.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets the number of elements in the collection.
            /// </summary>
            public int Count => collection.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
            }

            #endregion

            #region IEnumerable<T> Members

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            public IEnumerator<TKey> GetEnumerator()
            {
                return collection.GetEnumerator();
            }

#endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion

            #region ICollection Members

            void ICollection.CopyTo(Array array, int index)
            {
                ReadOnlyDictionaryHelpers.CopyToNonGenericICollectionHelper<TKey>(collection, array, index);
            }

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot
            {
                get
                {
                    if (syncRoot == null)
                    {
                        if (collection is ICollection c)
                        {
                            syncRoot = c.SyncRoot;
                        }
                        else
                        {
                            System.Threading.Interlocked.CompareExchange<object?>(ref syncRoot, new object(), null);
                        }
                    }
                    return syncRoot;
                }
            }

            #endregion
        }

        #endregion

        #region Nested Class: ValueCollection

        /// <summary>
        /// Represents a read-only collection of the values of a <see cref="ReadOnlyDictionary{TKey, TValue}"/> object.
        /// </summary>
        [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
        [DebuggerDisplay("Count = {Count}")]
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        public sealed class ValueCollection : ICollection<TValue>, ICollection
#if FEATURE_IREADONLYCOLLECTIONS
            , IReadOnlyCollection<TValue>
#endif
        {
            private readonly ICollection<TValue> collection;
#if FEATURE_SERIALIZABLE
            [NonSerialized]
#endif
            private object? syncRoot;

            internal ValueCollection(ICollection<TValue> collection)
            {
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

            #region ICollection<T> Members

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                return collection.Contains(item);
            }

            /// <summary>
            /// Copies the elements of the collection to an array, starting at a specific array index.
            /// </summary>
            /// <param name="array">The one-dimensional array that is the destination of the elements
            /// copied from the collection. The array must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
            /// <exception cref="ArgumentException">
            /// <paramref name="array"/> is multidimensional.
            /// <para/>
            /// -or-
            /// <para/>
            /// The number of elements in the source collection is greater than the available space from
            /// <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
            /// <para/>
            /// -or-
            /// <para/>
            /// Type <typeparamref name="TKey"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
            /// </exception>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                collection.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets the number of elements in the collection.
            /// </summary>
            public int Count => collection.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
            }

            #endregion

            #region IEnumerable<T> Members

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            public IEnumerator<TValue> GetEnumerator()
            {
                return collection.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion

            #region ICollection Members

            void ICollection.CopyTo(Array array, int index)
            {
                ReadOnlyDictionaryHelpers.CopyToNonGenericICollectionHelper<TValue>(collection, array, index);
            }

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot
            {
                get
                {
                    if (syncRoot == null)
                    {
                        if (collection is ICollection c)
                        {
                            syncRoot = c.SyncRoot;
                        }
                        else
                        {
                            System.Threading.Interlocked.CompareExchange<object?>(ref syncRoot, new object(), null);
                        }
                    }
                    return syncRoot;
                }
            }

            #endregion ICollection Members
        }

        #endregion

        internal static class ReadOnlyDictionaryHelpers
        {
            #region Helper method for our KeyCollection and ValueCollection

            // Abstracted away to avoid redundant implementations.
            internal static void CopyToNonGenericICollectionHelper<T>(ICollection<T> collection, Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException(SR.Arg_NonZeroLowerBound);
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
                }

                if (array.Length - index < collection.Count)
                {
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
                }

                // Easy out if the ICollection<T> implements the non-generic ICollection
                if (collection is ICollection nonGenericCollection)
                {
                    nonGenericCollection.CopyTo(array, index);
                    return;
                }

                if (array is T[] items)
                {
                    collection.CopyTo(items, index);
                }
                else
                {
                    //
                    // Catch the obvious case assignment will fail.
                    // We can found all possible problems by doing the check though.
                    // For example, if the element type of the Array is derived from T,
                    // we can't figure out if we can successfully copy the element beforehand.
                    //
                    Type targetType = array.GetType().GetElementType()!;
                    Type sourceType = typeof(T);
                    if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType);
                    }

                    //
                    // We can't cast array of value type to object[], so we don't support 
                    // widening of primitive types here.
                    //
                    if (!(array is object[] objects))
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType);
                    }

                    try
                    {
                        foreach (var item in collection)
                        {
                            objects[index++] = item!;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType);
                    }
                }
            }

            #endregion Helper method for our KeyCollection and ValueCollection
        }
    }
}
