using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    using SR = J2N.Resources.Strings;

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>, IStructuralEquatable, IStructuralFormattable
#if FEATURE_SERIALIZABLE
        , System.Runtime.Serialization.ISerializable
#endif
    {
        private static readonly bool TKeyIsNullable = typeof(TKey).IsNullableType();

        private readonly IConcreteDictionary<TKey, TValue> dictionary;

        public Dictionary() : this(0, null) { }

        public Dictionary(int capacity) : this(capacity, null) { }

        public Dictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) { }

        public Dictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            comparer = comparer ?? EqualityComparer<TKey>.Default;

            if (TKeyIsNullable)
                dictionary = new NullableKeyDictionary(capacity, comparer);
            else
                dictionary = new ConcreteDictionary(capacity, comparer);
        }

        public Dictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }

        public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }

        public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(collection is ICollection<KeyValuePair<TKey, TValue>> col ? col.Count : 0, null)
        { }

        public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this(collection is ICollection<KeyValuePair<TKey, TValue>> col ? col.Count : 0, comparer)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var pair in collection)
                Add(pair.Key, pair.Value);
        }

#if FEATURE_SERIALIZABLE
        protected Dictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (TKeyIsNullable)
                dictionary = new NullableKeyDictionary(info, context);
            else
                dictionary = new ConcreteDictionary(info, context);
        }

#endif

        #region SCG.Dictionary<TKey, TValue> Members

        public IEqualityComparer<TKey> Comparer => dictionary.Comparer;

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="Dictionary{TKey, TValue}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if the <see cref="Dictionary{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method determines equality using J2N's default equality comparer
        /// <see cref="EqualityComparer{T}.Default"/> for <typeparamref name="TValue"/>,
        /// the type of values in the dictionary.
        /// <para/>
        /// This method performs a linear search; therefore, the average execution time
        /// is proportional to <see cref="Count"/>. That is, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue(TValue value)
        {
            // NOTE: We do this check here to override the .NET default equality comparer
            // with J2N's version
            foreach (var item in dictionary.Values)
            {
                if (EqualityComparer<TValue>.Default.Equals(value, item))
                    return true;
            }
            return false;
        }

#if FEATURE_DICTIONARY_ENSURECAPACITY
        public int EnsureCapacity(int capacity)
            => dictionary.EnsureCapacity(capacity);
#endif
#if FEATURE_SERIALIZABLE
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            => dictionary.GetObjectData(info, context);

        public virtual void OnDeserialization(object sender)
            => dictionary.OnDeserialization(sender);
#endif

#if FEATURE_DICTIONARY_TRIMEXCESS

        /// <summary>
        /// Sets the capacity of this dictionary to hold up a specified number of entries
        /// without any further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less
        /// than <see cref="Dictionary{TKey, TValue}"/>.</exception>
        /// <remarks>
        /// This method can be used to minimize the memory overhead once it is known that no
        /// new elements will be added.
        /// </remarks>
        public void TrimExcess(int capacity)
            => dictionary.TrimExcess(capacity);

        /// <summary>
        /// Sets the capacity of this dictionary to what it would be if it had been originally
        /// initialized with all its entries.
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize memory overhead once it is known that no new
        /// elements will be added to the dictionary. To allocate a minimum size storage array,
        /// execute the following statements:
        /// <code>
        /// dictionary.Clear();
        /// dictionary.TrimExcess();
        /// </code>
        /// </remarks>
        public void TrimExcess()
            => dictionary.TrimExcess();
#endif

        // J2N: Explicitly defined to undercut the extension method in CollectionExtensions that doesn't allow
        // null keys.
        public bool TryAdd(TKey key, TValue value)
        {
            if (dictionary is NullableKeyDictionary nullable)
                return nullable.TryAdd(key, value);

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }

        #endregion SCG.Dictionary<TKey, TValue> Members

        #region IDictionary<TKey, TValue> Members

        public ICollection<TKey> Keys => dictionary.Keys;

        public ICollection<TValue> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly => dictionary.IsReadOnly;

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public void Add(TKey key, TValue value)
            => dictionary.Add(key, value);

        public bool ContainsKey(TKey key)
            => dictionary.ContainsKey(key);

        public bool Remove(TKey key)
            => dictionary.Remove(key);

        public bool TryGetValue(TKey key, out TValue value)
            => dictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item)
            => dictionary.Add(item);

        public void Clear()
            => dictionary.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => dictionary.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => dictionary.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item)
            => dictionary.Remove(item);

        // J2N: Explicitly defined to undercut the extension method in CollectionExtensions that doesn't allow
        // null keys.
        public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (dictionary is NullableKeyDictionary nullable)
                return nullable.Remove(key, out value);

            if (dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }

            value = default!;
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion IDictionary<TKey, TValue> Members

        #region IDictionary Members

        public bool IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

        ICollection IDictionary.Keys => ((IDictionary)dictionary).Keys;

        ICollection IDictionary.Values => ((IDictionary)dictionary).Values;

        public bool IsSynchronized => ((IDictionary)dictionary).IsSynchronized;

        public object SyncRoot => ((IDictionary)dictionary).SyncRoot;

        public object this[object key]
        {
            get => ((IDictionary)dictionary)[key];
            set => ((IDictionary)dictionary)[key] = value;
        }

        public void Add(object key, object value)
            => ((IDictionary)dictionary).Add(key, value);

        public bool Contains(object key)
            => ((IDictionary)dictionary).Contains(key);

        IDictionaryEnumerator IDictionary.GetEnumerator()
            => ((IDictionary)dictionary).GetEnumerator();

        public void Remove(object key)
            => ((IDictionary)dictionary).Remove(key);

        public void CopyTo(Array array, int index)
            => ((IDictionary)dictionary).CopyTo(array, index);

        #endregion IDictionary Members

        #region IReadOnlyDictionary<TKey, TValue> Members

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ((IReadOnlyDictionary<TKey, TValue>)dictionary).Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => ((IReadOnlyDictionary<TKey, TValue>)dictionary).Values;

        #endregion IReadOnlyDictionary<TKey, TValue> Members

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
        public virtual bool Equals(object other, IEqualityComparer comparer)
            => DictionaryEqualityComparer<TKey, TValue>.Equals(this, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current dictionary using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current dictionary.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
            => DictionaryEqualityComparer<TKey, TValue>.GetHashCode(this, comparer);

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current dictionary
        /// using rules similar to those in the JDK's AbstactMap class. Two dictionaries are considered
        /// equal when they both contain the same mapppings (in any order).
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="IDictionary{TKey, TValue}"/>
        /// and it contains the same elements; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object obj)
            => Equals(obj, DictionaryEqualityComparer<TKey, TValue>.Default);

        /// <summary>
        /// Gets the hash code for the current list. The hash code is calculated 
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <seealso cref="GetHashCode(IEqualityComparer)"/>
        public override int GetHashCode()
            => GetHashCode(DictionaryEqualityComparer<TKey, TValue>.Default);

        #endregion Structural Equality

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
        public virtual string ToString(string format, IFormatProvider formatProvider)
            => string.Format(formatProvider, format, this);

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
            => ToString("{0}", StringFormatter.CurrentCulture);


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
            => ToString(format, StringFormatter.CurrentCulture);

        #endregion ToString

        #region Nested Type: NullableKeyDictionary<TKey, TValue>

        /// <summary>
        /// A <see cref="IConcreteDictionary{TKey, TValue}"/> implementation that supports null keys.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class NullableKeyDictionary : IConcreteDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
#if FEATURE_SERIALIZABLE
            , System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
#endif
        {
            private readonly IConcreteDictionary<TKey, TValue> dictionary;

            private bool hasNullKey;
            private KeyValuePair<TKey, TValue> nullEntry;

#if FEATURE_SERIALIZABLE
            [NonSerialized]
#endif
            private KeyCollection keys;
#if FEATURE_SERIALIZABLE
            [NonSerialized]
#endif
            private ValueCollection values;

            public NullableKeyDictionary(int capacity, IEqualityComparer<TKey> comparer)
            {
                dictionary = new ConcreteDictionary(capacity, comparer ?? EqualityComparer<TKey>.Default);
            }

#if FEATURE_SERIALIZABLE
            public NullableKeyDictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                dictionary = new DictionaryWrapper(info, context);
            }

            private class DictionaryWrapper : ConcreteDictionary
            {
                public DictionaryWrapper(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
                    : base(info, context)
                { }
            }
#endif

            public TValue this[TKey key]
            {
                get
                {
                    if (key is null)
                    {
                        if (!hasNullKey)
                            throw new KeyNotFoundException(J2N.SR.Format(SR.Arg_KeyNotFoundWithKey, "null"));
                        return nullEntry.Value;
                    }
                    return dictionary[key];
                }
                set
                {
                    if (key is null) // This class only gets instantiated when the type is nullable
                    {
                        hasNullKey = true;
                        nullEntry = new KeyValuePair<TKey, TValue>(default, value);
                    }
                    else
                    {
                        dictionary[key] = value;
                    }
                }
            }

            public IEqualityComparer<TKey> Comparer => dictionary.Comparer;

            public ICollection<TKey> Keys
            {
                get
                {
                    if (keys == null) keys = new KeyCollection(this, dictionary.Keys);
                    return keys;
                }
            }

            public ICollection<TValue> Values
            {
                get
                {
                    if (values == null) values = new ValueCollection(this, dictionary.Values);
                    return values;
                }
            }

            public int Count => dictionary.Count + (hasNullKey ? 1 : 0);

            public bool IsReadOnly => false;

            public bool IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

            ICollection IDictionary.Keys => (ICollection)Keys;

            ICollection IDictionary.Values => (ICollection)Values;

            public bool IsSynchronized => ((ICollection)dictionary).IsSynchronized;

            public object SyncRoot => ((ICollection)dictionary).SyncRoot;

            IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

            IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

            public object this[object key]
            {
                get
                {
                    if (key is null)
                    {
                        if (hasNullKey)
                            return nullEntry.Value;
                    }
                    if (key is TKey)
                        return this[(TKey)key];
                    return null;
                }
                set
                {
                    try
                    {
                        TKey tempKey = (TKey)key;

                        try
                        {
                            this[tempKey] = (TValue)value;
                        }
                        catch (InvalidCastException)
                        {
                            throw new ArgumentException(J2N.SR.Format(SR.Arg_WrongType, value, typeof(TValue)), nameof(value));
                        }
                    }
                    catch (InvalidCastException)
                    {
                        throw new ArgumentException(J2N.SR.Format(SR.Arg_WrongType, key, typeof(TKey)), nameof(key));
                    }
                }
            }

            public void Add(TKey key, TValue value)
            {
                if (key is null)
                {
                    if (hasNullKey)
                        throw new ArgumentException(J2N.SR.Format(SR.Argument_AddingDuplicate, "null"));

                    hasNullKey = true;
                    nullEntry = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    dictionary.Add(key, value);
                }
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                if (item.Key is null)
                {
                    if (hasNullKey)
                        throw new ArgumentException(J2N.SR.Format(SR.Argument_AddingDuplicate, "null"));

                    hasNullKey = true;
                    nullEntry = item;
                }
                else
                {
                    dictionary.Add(item);
                }
            }

            public void Clear()
            {
                hasNullKey = false;
                nullEntry = default;
                dictionary.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                if (item.Key is null)
                    return hasNullKey && EqualityComparer<TValue>.Default.Equals(item.Value, nullEntry.Value);
                return dictionary.Contains(item);
            }

            public bool ContainsKey(TKey key)
            {
                if (key is null)
                    return hasNullKey;
                return dictionary.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (array.Length - index < Count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                if (hasNullKey)
                    array[index++] = nullEntry;
                foreach (var item in dictionary)
                    array[index++] = item;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                // Only use our enumerator if we have a null value, otherwise, use the original
                if (hasNullKey)
                    return new Enumerator(this, Enumerator.KeyValuePair);
                return dictionary.GetEnumerator();
            }

            public bool Remove(TKey key)
            {
                if (key is null)
                {
                    if (!hasNullKey)
                        return false;

                    hasNullKey = false;
                    nullEntry = default;
                    return true;
                }
                return dictionary.Remove(key);
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                if (item.Key is null)
                {
                    if (!hasNullKey)
                        return false;

                    hasNullKey = false;
                    nullEntry = default;
                    return true;
                }
                return dictionary.Remove(item);
            }

            // J2N: Explicitly defined to undercut the extension method in CollectionExtensions that doesn't allow
            // null keys.
            public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
            {
                if (key is null)
                {
                    if (!hasNullKey)
                    {
                        value = default!;
                        return false;
                    }

                    value = nullEntry.Value;
                    return true;
                }

                if (dictionary.TryGetValue(key, out value))
                {
                    dictionary.Remove(key);
                    return true;
                }

                value = default!;
                return false;
            }

#if FEATURE_DICTIONARY_ENSURECAPACITY
            public int EnsureCapacity(int capacity)
                => dictionary.EnsureCapacity(capacity);
#endif

#if FEATURE_SERIALIZABLE
            public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
                => dictionary.GetObjectData(info, context);

            public void OnDeserialization(object sender)
                => dictionary.OnDeserialization(sender);
#endif

#if FEATURE_DICTIONARY_TRIMEXCESS
            public void TrimExcess(int capacity)
                => dictionary.TrimExcess(capacity);

            public void TrimExcess()
                => dictionary.TrimExcess();
#endif
            // J2N: Explicitly defined to undercut the extension method in CollectionExtensions that doesn't allow
            // null keys.
            public bool TryAdd(TKey key, TValue value)
            {
                if (key is null)
                {
                    if (hasNullKey)
                        return false;
                    nullEntry = new KeyValuePair<TKey, TValue>(key, value);
                    return (hasNullKey = true);
                }

                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, value);
                    return true;
                }

                return false;
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                if (key is null)
                {
                    if (hasNullKey)
                    {
                        value = nullEntry.Value;
                        return true;
                    }

                    value = default;
                    return false;
                }
                return dictionary.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            public void Add(object key, object value)
            {
                try
                {
                    TKey tempKey = (TKey)key;

                    try
                    {
                        Add(tempKey, (TValue)value);
                    }
                    catch (InvalidCastException)
                    {
                        throw new ArgumentException(J2N.SR.Format(SR.Arg_WrongType, value, typeof(TValue)), nameof(value));
                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(J2N.SR.Format(SR.Arg_WrongType, key, typeof(TKey)), nameof(key));
                }
            }

            public bool Contains(object key)
            {
                if (key is TKey)
                    return ContainsKey((TKey)key);
                return false;
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                // Only use our enumerator if we have a null value, otherwise, use the original
                if (hasNullKey)
                    return new Enumerator(this, Enumerator.DictEntry);
                return ((IDictionary)dictionary).GetEnumerator();
            }

            public void Remove(object key)
            {
                if (key is TKey)
                    Remove((TKey)key);
            }

            public void CopyTo(Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
                if (array.GetLowerBound(0) != 0)
                    throw new ArgumentException(SR.Arg_NonZeroLowerBound);
                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (array.Length - index < Count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                if (array is KeyValuePair<TKey, TValue>[] pairs)
                {
                    CopyTo(pairs, index);
                }
                else if (array is DictionaryEntry[] dictEntryArray)
                {
                    // Null check not needed because we are enumerating this
                    foreach (var entry in this)
                        dictEntryArray[index++] = new DictionaryEntry(entry.Key, entry.Value);
                }
                else
                {
                    if (!(array is object[] objects))
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }

                    try
                    {
                        // Null check not needed because we are enumerating this
                        foreach (var entry in this)
                            objects[index++] = entry;
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }
                }
            }

            #region Nested Class: KeyCollection

            /// <summary>
            /// Represents the collection of keys in a <see cref="NullableKeyDictionary"/>. This class cannot be inherited.
            /// </summary>
#if FEATURE_SERIALIZABLE
            [Serializable]
#endif
            private sealed class KeyCollection : ICollection<TKey>, ICollection
            {
                private readonly NullableKeyDictionary nullableKeyDictionary;
                private readonly ICollection<TKey> collection;
                public KeyCollection(NullableKeyDictionary nullableKeyDictionary, ICollection<TKey> collection)
                {
                    this.nullableKeyDictionary = nullableKeyDictionary ?? throw new ArgumentNullException(nameof(nullableKeyDictionary));
                    this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
                }

                /// <summary>
                ///  Gets the number of elements contained in the <see cref="KeyCollection"/>.
                /// </summary>
                public int Count => nullableKeyDictionary.Count;

                public bool IsReadOnly => true;

                public bool IsSynchronized => nullableKeyDictionary.IsSynchronized;

                public object SyncRoot => nullableKeyDictionary.SyncRoot;

                public void Add(TKey item)
                    => throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);

                public void Clear()
                    => throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);

                public bool Contains(TKey item)
                {
                    if (item is null)
                        return nullableKeyDictionary.hasNullKey;
                    return collection.Contains(item);
                }

                public void CopyTo(TKey[] array, int index)
                {
                    if (array == null)
                        throw new ArgumentNullException(nameof(array));
                    if (index < 0 || index > array.Length)
                        throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                    if (array.Length - index < Count)
                        throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                    // Null check not needed because we are enumerating this
                    foreach (var item in this)
                        array[index++] = item;
                }

                public IEnumerator<TKey> GetEnumerator()
                {
                    // Only use our enumerator if we have a null value, otherwise, use the original
                    if (nullableKeyDictionary.hasNullKey)
                        return new Enumerator(collection);
                    return collection.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                public bool Remove(TKey item)
                    => throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);

                public void CopyTo(Array array, int index)
                {
                    if (array == null)
                        throw new ArgumentNullException(nameof(array));
                    if (array.Rank != 1)
                        throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
                    if (array.GetLowerBound(0) != 0)
                        throw new ArgumentException(SR.Arg_NonZeroLowerBound);
                    if (index < 0 || index > array.Length)
                        throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                    if (array.Length - index < Count)
                        throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                    if (array is TKey[] keys)
                    {
                        CopyTo(keys, index);
                    }
                    else
                    {
                        if (!(array is object[] objects))
                        {
                            throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                        }
                        try
                        {
                            // Null check not needed because we are enumerating this
                            foreach (var item in this)
                                objects[index++] = item;
                        }
                        catch (ArrayTypeMismatchException)
                        {
                            throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                        }
                    }
                }

                #region Nested Structure: Enumerator

                /// <summary>
                /// An enumerator that contains a null key to swap in when there is one.
                /// </summary>
#if FEATURE_SERIALIZABLE
                [Serializable]
#endif
                private struct Enumerator : IEnumerator<TKey>, IEnumerator
                {
                    private readonly IEnumerator<TKey> enumerator;
                    private TKey current;
                    private bool nullKeySeen;
                    public Enumerator(ICollection<TKey> keyCollection)
                    {
                        this.enumerator = keyCollection.GetEnumerator();
                        current = default;
                        nullKeySeen = false;
                    }

                    public TKey Current => current;

                    object IEnumerator.Current => Current;

                    public void Dispose()
                        => enumerator.Dispose();

                    public bool MoveNext()
                    {
                        if (!nullKeySeen)
                        {
                            nullKeySeen = true;
                            current = default;
                            return true;
                        }

                        if (enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            return true;
                        }
                        current = default;
                        return false;
                    }

                    public void Reset()
                    {
                        nullKeySeen = false;
                        enumerator.Reset();
                    }
                }

                #endregion
            }

            #endregion

            #region Nested Class: ValueCollection

            /// <summary>
            /// Represents the collection of values in a <see cref="NullableKeyDictionary"/>. This class cannot be inherited.
            /// </summary>
#if FEATURE_SERIALIZABLE
            [Serializable]
#endif
            private sealed class ValueCollection : ICollection<TValue>, ICollection
            {
                private readonly NullableKeyDictionary nullableKeyDictionary;
                private readonly ICollection<TValue> collection;

                public ValueCollection(NullableKeyDictionary nullableKeyDictionary, ICollection<TValue> collection)
                {
                    this.nullableKeyDictionary = nullableKeyDictionary ?? throw new ArgumentNullException(nameof(nullableKeyDictionary));
                    this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
                }

                public int Count => nullableKeyDictionary.Count;

                public bool IsSynchronized => nullableKeyDictionary.IsSynchronized;

                public object SyncRoot => nullableKeyDictionary.SyncRoot;

                public bool IsReadOnly => true;

                public void Add(TValue item)
                    => throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);

                public void Clear()
                    => throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);

                public bool Contains(TValue item)
                {
                    if (nullableKeyDictionary.hasNullKey)
                    {
                        if (EqualityComparer<TValue>.Default.Equals(nullableKeyDictionary.nullEntry.Value, item))
                            return true;
                    }
                    foreach (var value in collection)
                    {
                        if (EqualityComparer<TValue>.Default.Equals(value, item))
                            return true;
                    }
                    return false;
                }

                public void CopyTo(Array array, int index)
                {
                    if (array == null)
                        throw new ArgumentNullException(nameof(array));
                    if (array.Rank != 1)
                        throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
                    if (array.GetLowerBound(0) != 0)
                        throw new ArgumentException(SR.Arg_NonZeroLowerBound);
                    if (index < 0 || index > array.Length)
                        throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                    if (array.Length - index < Count)
                        throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                    if (array is TValue[] values)
                    {
                        CopyTo(values, index);
                    }
                    else
                    {
                        if (!(array is object[] objects))
                        {
                            throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                        }

                        try
                        {
                            // Null check not needed because we are enumerating this
                            foreach (var entry in this)
                                objects[index++] = entry;
                        }
                        catch (ArrayTypeMismatchException)
                        {
                            throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                        }
                    }
                }

                public void CopyTo(TValue[] array, int arrayIndex)
                {
                    // Null check not needed because we are enumerating this
                    foreach (var value in this)
                        array[arrayIndex++] = value;
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

                public bool Remove(TValue item)
                    => throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);

                public IEnumerator<TValue> GetEnumerator()
                {
                    // Only use our enumerator if we have a null value, otherwise, use the original
                    if (nullableKeyDictionary.hasNullKey)
                        return new Enumerator(nullableKeyDictionary, collection);
                    return collection.GetEnumerator();
                }

                #region Nested Structure: Enumerator

                /// <summary>
                /// An enumerator that contains a null key to swap in when there is one.
                /// </summary>
                // NOTE: Xamarin.iOS only has partial generics support. One issue it has
                // is that it cannot cope with nested structs that implement generic interfaces
                // unless the struct is also made generic. This is why we have named the
                // type ref TValue1 instead of simply using TValue of the parent class.
#if FEATURE_SERIALIZABLE
                [Serializable]
#endif
                private struct Enumerator : IEnumerator<TValue>, IEnumerator
                {
                    private readonly NullableKeyDictionary nullableKeyDictionary;
                    private readonly IEnumerator<TValue> enumerator;
                    private TValue current;
                    private bool nullValueSeen;
                    public Enumerator(NullableKeyDictionary nullableKeyDictionary, ICollection<TValue> valueCollection)
                    {
                        this.nullableKeyDictionary = nullableKeyDictionary ?? throw new ArgumentNullException(nameof(nullableKeyDictionary));
                        this.enumerator = valueCollection.GetEnumerator();
                        current = default;
                        nullValueSeen = false;
                    }

                    public TValue Current => current;

                    object IEnumerator.Current => current;

                    public void Dispose()
                        => enumerator.Dispose();

                    public bool MoveNext()
                    {
                        if (!nullValueSeen)
                        {
                            nullValueSeen = true;
                            current = nullableKeyDictionary.nullEntry.Value;
                            return true;
                        }

                        if (enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            return true;
                        }
                        current = default;
                        return false;
                    }

                    public void Reset()
                    {
                        nullValueSeen = false;
                        enumerator.Reset();
                    }
                }

                #endregion
            }

            #endregion

            #region Nested Structure: Enumerator

            /// <summary>
            /// Enumerates the elemensts of a <see cref="NullableKeyDictionary"/>.
            /// </summary>
            private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
            {
                private readonly NullableKeyDictionary nullableKeyDictionary;
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
                private int index;
                private bool nullKeySeen;
                private KeyValuePair<TKey, TValue> current;
                private readonly int getEnumeratorRetType;  // What should Enumerator.Current return?

                internal const int DictEntry = 1;
                internal const int KeyValuePair = 2;

                public Enumerator(NullableKeyDictionary nullableKeyDictionary, int getEnumeratorRetType)
                {
                    this.nullableKeyDictionary = nullableKeyDictionary ?? throw new ArgumentException(nameof(nullableKeyDictionary));
                    this.enumerator = nullableKeyDictionary.dictionary.GetEnumerator();
                    this.getEnumeratorRetType = getEnumeratorRetType;
                    index = 0;
                    nullKeySeen = false;
                    current = default;
                }

                public KeyValuePair<TKey, TValue> Current => current;

                object IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == nullableKeyDictionary.Count + 1))
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                        if (getEnumeratorRetType == DictEntry)
                            return new DictionaryEntry(current.Key, current.Value);
                        else
                            return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                    }
                }

                public void Dispose() => enumerator.Dispose();

                public bool MoveNext()
                {
                    if (!nullKeySeen)
                    {
                        index++;
                        nullKeySeen = true;
                        current = nullableKeyDictionary.nullEntry;
                        return true;
                    }
                    if (enumerator.MoveNext())
                    {
                        index++;
                        current = enumerator.Current;
                        return true;
                    }
                    index = nullableKeyDictionary.Count + 1;
                    current = default;
                    return false;
                }

                public void Reset()
                {
                    index = 0;
                    nullKeySeen = false;
                    enumerator.Reset();
                }

                #region IDictionaryEnumerator Members

                DictionaryEntry IDictionaryEnumerator.Entry
                {
                    get
                    {
                        if (index == 0 || (index == nullableKeyDictionary.Count + 1))
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                        return new DictionaryEntry(current.Key, current.Value);
                    }
                }

                object IDictionaryEnumerator.Key
                {
                    get
                    {
                        if (index == 0 || (index == nullableKeyDictionary.Count + 1))
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                        return current.Key;
                    }
                }

                object IDictionaryEnumerator.Value
                {
                    get
                    {
                        if (index == 0 || (index == nullableKeyDictionary.Count + 1))
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                        return current.Value;
                    }
                }

                #endregion
            }

            #endregion

        }

        #endregion

        #region Nested Type: ConcreteDictionary<TKey, TValue>

        /// <summary>
        /// An adapter class for <see cref="SCG.Dictionary{TKey, TValue}"/> to implement <see cref="IConcreteDictionary{TKey, TValue}"/>,
        /// which is an interface that is used to share all of the members between <see cref="SCG.Dictionary{TKey, TValue}"/>
        /// and <see cref="NullableKeyDictionary"/>.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class ConcreteDictionary : SCG.Dictionary<TKey, TValue>, IConcreteDictionary<TKey, TValue>
        {
            public ConcreteDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer ?? EqualityComparer<TKey>.Default) { }

#if FEATURE_SERIALIZABLE
            public ConcreteDictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
                : base(info, context)
            { }
#endif
        }

#endregion

    }

#region Interface: IConcreteDictionary<TKey, TValue>

    /// <summary>
    /// Interface to expose all of the members of the concrete <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> type,
    /// so we can duplicate them in other types without having to cast.
    /// </summary>
    internal interface IConcreteDictionary<TKey, TValue> : IDictionary<TKey, TValue>//, IDictionary, IReadOnlyDictionary<TKey, TValue>
#if FEATURE_SERIALIZABLE
        , System.Runtime.Serialization.ISerializable
#endif
    {
        IEqualityComparer<TKey> Comparer { get; }

        //bool ContainsValue(TValue value); // NOTE: We don't want to utilize the built-in method because
        // it uses the .NET default equality comparer, and we want to swap that.


#if FEATURE_DICTIONARY_ENSURECAPACITY
            int EnsureCapacity(int capacity);
#endif
#if FEATURE_SERIALIZABLE
            void OnDeserialization(object sender);
#endif
#if FEATURE_DICTIONARY_TRIMEXCESS
            void TrimExcess(int capacity);

            void TrimExcess();
#endif
#if FEATURE_DICTIONARY_REMOVE_OUTVALUE
            bool Remove(TKey key, out TValue value);
#endif
#if FEATURE_DICTIONARY_TRYADD
            bool TryAdd(TKey key, TValue value);
#endif

    }

#endregion

}
