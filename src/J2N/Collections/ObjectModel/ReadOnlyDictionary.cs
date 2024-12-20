// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace J2N.Collections.ObjectModel
{
    /// <summary>
    /// Provides the base class for a generic read-only collection of key/value pairs that is structurally equatable.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    // NOTE: We cannot subclass System.Collection.ObjectModel.ReadOnlyDictionary<TKey, TValue> because there is a nullable constraint on the key and a check for it in code as well
    [SuppressMessage("Style", "IDE0019:Use pattern matching", Justification = "Following Microsoft's code style")]
    [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Following Microsoft's code style")]
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IStructuralEquatable, IStructuralFormattable
    {
        private static readonly bool TKeyIsValueTypeOrStringOrStructuralEquatable = typeof(TKey).IsValueType || typeof(IStructuralEquatable).IsAssignableFrom(typeof(TKey)) || typeof(string).Equals(typeof(TKey));
        private static readonly bool TValueIsValueTypeOrStringOrStructuralEquatable = typeof(TValue).IsValueType || typeof(IStructuralEquatable).IsAssignableFrom(typeof(TValue)) || typeof(string).Equals(typeof(TValue));

        internal readonly IDictionary<TKey, TValue> dictionary; // Internal for testing
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
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
           : this(dictionary,
                 TKeyIsValueTypeOrStringOrStructuralEquatable && TValueIsValueTypeOrStringOrStructuralEquatable ?
                    DictionaryEqualityComparer<TKey, TValue>.Default :
                    DictionaryEqualityComparer<TKey, TValue>.Aggressive,
                 StringFormatter.CurrentCulture)
        {
        }

        internal ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary, DictionaryEqualityComparer<TKey, TValue> structuralEqualityComparer, IFormatProvider toStringFormatProvider)
        {
            if (dictionary is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
            if (structuralEqualityComparer is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.structuralEqualityComparer);
            if (toStringFormatProvider is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.toStringFormatProvider);
            this.dictionary = dictionary;
            this.structuralEqualityComparer = structuralEqualityComparer;
            this.toStringFormatProvider = toStringFormatProvider;
        }

        /// <summary>
        /// Gets the dictionary that is wrapped by this <see cref="ReadOnlyDictionary{TKey, TValue}"/> object.
        /// </summary>
        protected internal IDictionary<TKey, TValue> Dictionary => dictionary;

        /// <summary>
        /// Gets a key collection that contains the keys of the dictionary.
        /// </summary>
        public KeyCollection Keys
        {
            get
            {
                Contract.Ensures(Contract.Result<KeyCollection>() != null);
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
                Contract.Ensures(Contract.Result<ValueCollection>() != null);
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

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        /// Retrieves the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value will be retrieved.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the object that implements <see cref="ReadOnlyDictionary{TKey, TValue}"/>
        /// contains an element with the specified key; otherwise, <c>false</c>.</returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8767 // Nullability of reference types in type of parameter 'value' of 'bool ReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' doesn't match implicitly implemented member 'bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' (possibly because of nullability attributes).
        public bool TryGetValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter 'value' of 'bool ReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' doesn't match implicitly implemented member 'bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
            => dictionary.TryGetValue(key!, out value!);

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        /// <summary>
        /// Gets the element that has the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The element that has the specified key.</returns>
        public TValue this[[AllowNull] TKey key] => dictionary[key];

        void IDictionary<TKey, TValue>.Add([AllowNull] TKey key, [AllowNull] TValue value)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        bool IDictionary<TKey, TValue>.Remove([AllowNull] TKey key)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        TValue IDictionary<TKey, TValue>.this[[AllowNull] TKey key]
        {
            get => dictionary[key];
            set => throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

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
                return default(TKey) == null;

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
                    // Fall out early if the dictionary is empty.
                    if (dictionary.Count == 0)
                        return null;

                    // J2N: If the wrapped dictionary supports IDictionary, cascade the call.
                    // We don't expect a KeyNotFoundException to occur when using IDictionary,
                    // instead, we should return null.
                    if (dictionary is IDictionary dict)
                        return dict[key];

                    // J2N: Patch broken behavior in .NET - IDictionary should return
                    // null if not found.
                    var tKey = (TKey)key;
                    if (dictionary.ContainsKey(tKey))
                        return this[tKey];
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
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (array.Rank != 1)
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException(SR.Arg_NonZeroLowerBound);
            if ((uint)index > (uint)array.Length)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
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
                    object[]? objects = array as object[];
                    if (objects == null)
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
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

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
            private readonly IDictionary<TKey, TValue> dictionary;
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

            public DictionaryEnumerator(IDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                enumerator = this.dictionary.GetEnumerator();
            }

            public DictionaryEntry Entry
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8604 // Possible null reference argument.
                => new DictionaryEntry(enumerator.Current.Key, enumerator.Current.Value);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore IDE0079 // Remove unnecessary suppression

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8613, CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member.
            public object? Key => enumerator.Current.Key;
#pragma warning restore CS8613, CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member.
#pragma warning restore IDE0079 // Remove unnecessary suppression

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
                if (array is null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
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
                    ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
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
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }

                    //
                    // We can't cast array of value type to object[], so we don't support 
                    // widening of primitive types here.
                    //
                    object?[]? objects = array as object?[];
                    if (objects == null)
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
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
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }
                }
            }

            #endregion Helper method for our KeyCollection and ValueCollection
        }
    }
}
