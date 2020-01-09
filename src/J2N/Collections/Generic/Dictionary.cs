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

    /// <summary>
    /// Represents a collection of keys and values.
    /// <para/>
    /// <see cref="Dictionary{TKey, TValue}"/> adds the following features to <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>:
    /// <list type="bullet">
    ///     <item><description>
    ///         If <typeparamref name="TKey"/> is <see cref="Nullable{T}"/> or a reference type, the key can be
    ///         <c>null</c> without throwing an exception.
    ///     </description></item>
    ///     <item><description>
    ///         Overrides the <see cref="Equals(object)"/> and <see cref="GetHashCode()"/> methods to compare collections
    ///         using structural equality by default. Also, <see cref="IStructuralEquatable"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Overrides the <see cref="ToString()"/> method to list the contents of the set
    ///         by default. Also, <see cref="IFormatProvider"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Uses <see cref="EqualityComparer{T}.Default"/> by default, which provides some specialized equality comparisons
    ///         for specific types to match the behavior of Java.
    ///     </description></item>
    /// </list>
    /// <para/>
    /// Usage Note: This class is intended to be a direct replacement for <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> in order
    /// to provide default structural equality and formatting behavior similar to Java. Note that the <see cref="ToString()"/>
    /// method uses the current culture by default to behave like other components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>, IStructuralEquatable, IStructuralFormattable
#if FEATURE_SERIALIZABLE
        , System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
#endif
    {
        private static readonly bool TKeyIsNullable = typeof(TKey).IsNullableType();

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private readonly IConcreteDictionary<TKey, TValue> dictionary;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private bool hasNullKey;
        private KeyValuePair<TKey, TValue> nullEntry;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private int version;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private KeyCollection keys;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private ValueCollection values;

#if FEATURE_SERIALIZABLE

        private System.Runtime.Serialization.SerializationInfo/*?*/ siInfo; //A temporary variable which we need during deserialization.

        // names for serialization
        private const string ComparerName = "Comparer"; // Do not rename (binary serialization)
        private const string CountName = "Count"; // Do not rename (binary serialization) - used to allocate during deserialzation, not actually a field
        private const string KeyValuePairsName = "KeyValuePairs"; // Do not rename (binary serialization)
        private const string VersionName = "Version"; // Do not rename (binary serialization)
#endif

        public Dictionary() : this(0, null) { }

        public Dictionary(int capacity) : this(capacity, null) { }

        public Dictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) { }

        public Dictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            dictionary = new ConcreteDictionary(capacity, comparer ?? EqualityComparer<TKey>.Default);
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
            : this(collection, null)
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class with serialized data.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object containing
        /// the information required to serialize the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure containing
        /// the source and destination of the serialized stream associated with the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute an object transmitted over a stream.
        /// For more information, see
        /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/serialization/xml-and-soap-serialization">XML and SOAP Serialization</a>.
        /// </remarks>
        protected Dictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            siInfo = info;
            int capacity = info.GetInt32(CountName);
            var comparer = (IEqualityComparer<TKey>)siInfo.GetValue(ComparerName, typeof(IEqualityComparer<TKey>));
            this.dictionary = new ConcreteDictionary(capacity, comparer);
        }

#endif

#region SCG.Dictionary<TKey, TValue> Members

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine equality of keys
        /// for the dictionary.
        /// </summary>
        /// <remarks>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine
        /// whether keys are equal. You can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter; if you do not
        /// specify one, J2N's default generic equality comparer <see cref="EqualityComparer{T}.Default"/> is used.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
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
            return ContainsValue(value, EqualityComparer<TValue>.Default);
        }

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains a specific value
        /// as determined by the provided <paramref name="valueComparer"/>.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="Dictionary{TKey, TValue}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <param name="valueComparer">The <see cref="IEqualityComparer{TValue}"/> to use
        /// to test each value for equality.</param>
        /// <returns><c>true</c> if the <see cref="Dictionary{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method performs a linear search; therefore, the average execution time
        /// is proportional to <see cref="Count"/>. That is, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue(TValue value, IEqualityComparer<TValue> valueComparer) // Overload added so end user can override J2N's equality comparer
        {
            if (TKeyIsNullable && hasNullKey && valueComparer.Equals(nullEntry.Value, value))
                return true;

            foreach (var val in dictionary.Values)
            {
                if (valueComparer.Equals(val, value))
                    return true;
            }
            return false;
        }

#if FEATURE_DICTIONARY_ENSURECAPACITY

        /// <summary>
        /// Ensures that the dictionary can hold up to a specified number of entries without any
        /// further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">The number of entries.</param>
        /// <returns>The current capacity of the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public int EnsureCapacity(int capacity)
        {
            int result = dictionary.EnsureCapacity(capacity);
            version++;
            return result;
        }
#endif
#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and returns the data needed to
        /// serialize the <see cref="Dictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains the
        /// information required to serialize the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure that contains
        /// the source and destination of the serialized stream associated with the <see cref="Dictionary{TKey, TValue}"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <c>null</c>.</exception>
        /// <remarks>This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        [System.Security.SecurityCritical]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            // Customized serialization for LinkedList.
            // We need to do this because it will give us flexibility to change the design
            // without changing the serialized info.
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            int count = Count;
            info.AddValue(CountName, count);
            info.AddValue(ComparerName, Comparer, typeof(IEqualityComparer<TKey>));
            info.AddValue(VersionName, version);

            if (count > 0)
            {
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[count];
                CopyTo(array, 0);
                info.AddValue(KeyValuePairsName, array, typeof(KeyValuePair<TKey, TValue>));
            }
        }

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and raises the deserialization
        /// event when the deserialization is complete.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> object associated with the current
        /// <see cref="Dictionary{TKey, TValue}"/> instance is invalid.</exception>
        /// <remarks>This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        public virtual void OnDeserialization(object sender)
        {
            if (siInfo == null)
            {
                return; //Somebody had a dependency on this Dictionary and fixed us up before the ObjectManager got to it.
            }

            int count = siInfo.GetInt32(CountName);
            if (count > 0)
            {
                KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])
                    siInfo.GetValue(KeyValuePairsName, typeof(KeyValuePair<TKey, TValue>[]));

                if (array == null)
                {
                    throw new System.Runtime.Serialization.SerializationException(SR.Serialization_MissingKeys);
                }

                for (int i = 0; i < array.Length; i++)
                {
                    Add(array[i]);
                }
            }
            // Overwrite the version with the original after all of the items were added
            version = siInfo.GetInt32(VersionName);
            if (Count != count)
                throw new System.Runtime.Serialization.SerializationException(SR.Serialization_MismatchedCount);

            siInfo = null;
        }
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
        {
            dictionary.TrimExcess(capacity);
            version++;
        }

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
        {
            dictionary.TrimExcess();
            version++;
        }

#endif

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add. It can be <c>null</c>.</param>
        /// <param name="value">The value of the element to add. It can be <c>null</c>.</param>
        /// <returns><c>true</c> if the key/value pair was added to the dictionary successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>Unlike the <see cref="Add(TKey, TValue)"/> method, this method doesn't throw an exception
        /// if the element with the given key exists in the dictionary. Unlike the Dictionary indexer, <see cref="TryAdd(TKey, TValue)"/>
        /// doesn't override the element if the element with the given key exists in the dictionary. If the key already exists,
        /// <see cref="TryAdd(TKey, TValue)"/> does nothing and returns <c>false</c>.</remarks>
        // J2N: This is an extension method on IDictionary<TKey, TValue>, but only for .NET Standard 2.1+.
        // It is redefined here to ensure we have it in prior platforms.
        public bool TryAdd(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                Add(key, value);
                return true;
            }

            return false;
        }

#endregion SCG.Dictionary<TKey, TValue> Members

#region IDictionary<TKey, TValue> Members

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The order of the keys in the <see cref="ICollection{TKey}"/> is unspecified, but it is the same order as the
        /// associated values in the <see cref="ICollection{TValue}"/> returned by the <see cref="Values"/> property.
        /// <para/>
        /// The returned <see cref="ICollection{TKey}"/> is not a static copy; instead,
        /// the <see cref="ICollection{TKey}"/> refers back to the keys in the original
        /// <see cref="Dictionary{TKey, TValue}"/>. Therefore, changes to the <see cref="Dictionary{TKey, TValue}"/>
        /// continue to be reflected in the <see cref="ICollection{TKey}"/>.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public ICollection<TKey> Keys
        {
            get
            {
                if (keys == null) keys = new KeyCollection(this, dictionary.Keys);
                return keys;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The order of the values in the <see cref="ICollection{TValue}"/> is unspecified, but it is the same order as the
        /// associated keys in the <see cref="ICollection{TKey}"/> returned by the <see cref="Keys"/> property.
        /// <para/>
        /// The returned <see cref="ICollection{TValue}"/> is not a static copy;
        /// instead, the <see cref="ICollection{TValue}"/> refers back to the
        /// values in the original <see cref="SortedDictionary{TKey, TValue}"/>. Therefore, changes to
        /// the <see cref="SortedDictionary{TKey, TValue}"/> continue to be reflected in the
        /// <see cref="ICollection{TValue}"/>.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public ICollection<TValue> Values
        {
            get
            {
                if (values == null) values = new ValueCollection(this, dictionary.Values);
                return values;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => dictionary.Count + (TKeyIsNullable && hasNullKey ? 1 : 0);

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        /// <summary>
        /// Gets or sets the value associated with the specified key. The key may be <c>null</c>.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key. If the specified key
        /// is not found, a get operation throws a <see cref="KeyNotFoundException"/>, and
        /// a set operation creates a new element with the specified key.</returns>
        /// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="key"/>
        /// does not exist in the collection.</exception>
        /// <remarks>
        /// This property provides the ability to access a specific element in the collection by using
        /// the following C# syntax: <c>myCollection[key]</c> (<c>myCollection(key)</c> in Visual Basic).
        /// <para/>
        /// You can also use the <see cref="this[TKey]"/> property to add new elements by setting the value of a key
        /// that does not exist in the <see cref="Dictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c>. However, if the specified key already exists in
        /// the <see cref="Dictionary{TKey, TValue}"/>, setting the <see cref="this[TKey]"/> property overwrites
        /// the old value. In contrast, the <see cref="Add(TKey, TValue)"/> method does not modify existing elements.
        /// <para/>
        /// Unlike the <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>, both keys and values can
        /// be <c>null</c> if either <see cref="Nullable{T}"/> or a reference type.
        /// <para/>
        /// The C# language uses the <see cref="this"/> keyword to define the indexers instead of implementing the
        /// <c>Item[TKey]</c> property. Visual Basic implements <c>Item[TKey]</c> as a default property, which provides
        /// the same indexing functionality.
        /// <para/>
        /// Getting the value of this property is an O(log <c>n</c>) operation; setting the property is also
        /// an O(log <c>n</c>) operation.
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                if (TKeyIsNullable && key is null)
                {
                    if (!hasNullKey)
                        throw new KeyNotFoundException(J2N.SR.Format(SR.Arg_KeyNotFoundWithKey, "null"));
                    return nullEntry.Value;
                }
                return dictionary[key];
            }
            set
            {
                if (TKeyIsNullable && key is null)
                {
                    hasNullKey = true;
                    nullEntry = new KeyValuePair<TKey, TValue>(default, value);
                }
                else
                {
                    dictionary[key] = value;
                }
                version++;
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists
        /// in the <see cref="Dictionary{TKey, TValue}"/>.</exception>
        /// <remarks>
        /// You can also use the <see cref="this[TKey]"/> property to add new elements by setting the value of
        /// a key that does not exist in the <see cref="Dictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c> (in Visual Basic, <c>myCollection("myNonexistantKey") = myValue</c>).
        /// However, if the specified key already exists in the <see cref="Dictionary{TKey, TValue}"/>, setting
        /// the <see cref="this[TKey]"/> property overwrites the old value. In contrast, the <see cref="Add(TKey, TValue)"/>
        /// method throws an exception if an element with the specified key already exists.
        /// <para/>
        /// Both keys and values can be <c>null</c> if the corresponding <typeparamref name="TKey"/> or
        /// <typeparamref name="TValue"/> is <see cref="Nullable{T}"/> or a reference type.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void Add(TKey key, TValue value)
        {
            if (TKeyIsNullable && key is null)
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
            version++;
        }

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains an
        /// element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="Dictionary{TKey, TValue}"/>. The key can be <c>null</c></param>
        /// <returns><c>true</c> if the <see cref="Dictionary{TKey, TValue}"/> contains an element
        /// with the specified key; otherwise, <c>false</c>.</returns>
        /// <remarks>This method is an O(log <c>n</c>) operation.</remarks>
        public bool ContainsKey(TKey key)
        {
            if (TKeyIsNullable && key is null)
                return hasNullKey;
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if key is not found in the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// If the <see cref="Dictionary{TKey, TValue}"/> does not contain an element with the specified key, the
        /// <see cref="Dictionary{TKey, TValue}"/> remains unchanged. No exception is thrown.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation.
        /// </remarks>
        public bool Remove(TKey key)
        {
            if (TKeyIsNullable && key is null)
            {
                if (!hasNullKey)
                    return false;

                hasNullKey = false;
                nullEntry = default;
                version++;
                return true;
            }
            else if (dictionary.Remove(key))
            {
                version++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns><c>true</c> if the <see cref="Dictionary{TKey, TValue}"/> contains an element with the
        /// specified key; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method combines the functionality of the <see cref="ContainsKey(TKey)"/> method
        /// and the <see cref="this[TKey]"/> property.
        /// <para/>
        /// If the key is not found, then the <paramref name="value"/> parameter gets the appropriate
        /// default value for the type <typeparamref name="TValue"/>; for example, 0 (zero) for
        /// integer types, <c>false</c> for Boolean types, and <c>null</c> for reference types.
        /// <para/>
        /// Use the <see cref="TryGetValue(TKey, out TValue)"/> method if your code frequently
        /// attempts to access keys that are not in the dictionary. Using this method is more
        /// efficient than catching the <see cref="KeyNotFoundException"/> thrown by the
        /// <see cref="this[TKey]"/> property.
        /// <para/>
        /// This method approaches an O(1) operation.
        /// </remarks>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (TKeyIsNullable && key is null)
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

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item);
        
        // declared locally so we can use during deserialization
        private void Add(KeyValuePair<TKey, TValue> item)
        {
            if (TKeyIsNullable && item.Key is null)
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
            version++;
        }

        /// <summary>
        /// Removes all elements from the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Count"/> property is set to 0, and references to other objects
        /// from elements of the collection are also released.
        /// <para/>
        /// This method is an O(1) operation, since the root of the internal data structures
        /// is simply released for garbage collection.
        /// </remarks>
        public void Clear()
        {
            dictionary.Clear();
            version++;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key is null)
                return hasNullKey && EqualityComparer<TValue>.Default.Equals(item.Value, nullEntry.Value);
            return dictionary.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="Dictionary{TKey, TValue}"/> to the specified array
        /// of <see cref="KeyValuePair{TKey, TValue}"/> structures, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array of <see cref="KeyValuePair{TKey, TValue}"/> structures
        /// that is the destination of the elements copied from the current <see cref="Dictionary{TKey, TValue}"/>.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - index < Count)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            if (TKeyIsNullable && hasNullKey)
                array[index++] = nullEntry;
            foreach (var item in dictionary)
                array[index++] = item;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (TKeyIsNullable && item.Key is null)
            {
                if (!hasNullKey || !EqualityComparer<TValue>.Default.Equals(item.Value, nullEntry.Value))
                    return false;

                hasNullKey = false;
                nullEntry = default;
                return true;
            }
            return dictionary.Remove(item);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="Dictionary{TKey, TValue}"/>.
        /// If the element exists, the associated <paramref name="value"/> is output after it is removed.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if key is not found in the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// If the <see cref="Dictionary{TKey, TValue}"/> does not contain an element with the specified key, the
        /// <see cref="Dictionary{TKey, TValue}"/> remains unchanged. No exception is thrown.
        /// </remarks>
        // J2N: This is an extension method on IDictionary<TKey, TValue>, but only for .NET Standard 2.1+.
        // It is redefined here to ensure we have it in prior platforms.
        public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (TKeyIsNullable && key is null)
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

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> for the
        /// <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// For purposes of enumeration, each item is a <see cref="KeyValuePair{TKey, TValue}"/> structure
        /// representing a value and its key.
        /// <para/>
        /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
        /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
        /// <para/>
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// <para/>
        /// The dictionary is maintained in a sorted order using an internal tree. Every new element is positioned at the correct sort position,
        /// and the tree is adjusted to maintain the sort order whenever an element is removed. While enumerating, the sort order is maintained.
        /// <para/>
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
        /// <see cref="IEnumerator{T}.Current"/> property is undefined. Therefore, you must call the
        /// <see cref="IEnumerator.MoveNext()"/> method to advance the enumerator to the first element
        /// of the collection before reading the value of <see cref="IEnumerator{T}.Current"/>.
        /// <para/>
        /// The <see cref="IEnumerator{T}.Current"/> property returns the same object until
        /// <see cref="IEnumerator.MoveNext()"/> is called. <see cref="IEnumerator.MoveNext()"/>
        /// sets <see cref="IEnumerator{T}.Current"/> to the next element.
        /// <para/>
        /// If <see cref="IEnumerator.MoveNext()"/> passes the end of the collection, the enumerator is
        /// positioned after the last element in the collection and <see cref="IEnumerator.MoveNext()"/>
        /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="IEnumerator.MoveNext()"/>
        /// also return <c>false</c>. If the last call to <see cref="IEnumerator.MoveNext()"/> returned <c>false</c>,
        /// <see cref="IEnumerator{T}.Current"/> is undefined. You cannot set <see cref="IEnumerator{T}.Current"/>
        /// to the first element of the collection again; you must create a new enumerator object instead.
        /// <para/>
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
        /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
        /// to <see cref="IEnumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
        /// <see cref="InvalidOperationException"/>.
        /// <para/>
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
        /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
        /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
        /// reading and writing, you must implement your own synchronization.
        /// <para/>
        /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            // Only use our enumerator if we have a null value, otherwise, use the original
            if (TKeyIsNullable && hasNullKey)
                return new Enumerator(this, Enumerator.KeyValuePair);
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

#endregion IDictionary<TKey, TValue> Members

#region IDictionary Members

        bool IDictionary.IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

        ICollection IDictionary.Keys => (ICollection)Keys;

        ICollection IDictionary.Values => (ICollection)Values;

        bool IDictionary.IsReadOnly => false;

        bool ICollection.IsSynchronized => ((ICollection)dictionary).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

        object IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    if (TKeyIsNullable && key is null)
                    {
                        if (hasNullKey)
                            return nullEntry.Value;
                    }
                    else if (TryGetValue((TKey)key, out TValue value))
                    {
                        return value;
                    }
                }
                return null;
            }
            set
            {
                // J2N: Only throw if the generic closing type is not nullable
                if (!TKeyIsNullable && key is null)
                    throw new ArgumentNullException(nameof(key));

                if (value is null && !typeof(TValue).IsNullableType())
                    throw new ArgumentNullException(nameof(value));

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

        void IDictionary.Add(object key, object value)
        {
            // J2N: Only throw if the generic closing type is not nullable
            if (!TKeyIsNullable && key is null)
                throw new ArgumentNullException(nameof(key));

            if (value is null && !typeof(TValue).IsNullableType())
                throw new ArgumentNullException(nameof(value));

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

        bool IDictionary.Contains(object key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key);
            }
            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            // Only use our enumerator if we have a null value, otherwise, use the original
            if (hasNullKey)
                return new Enumerator(this, Enumerator.DictEntry);
            return ((IDictionary)dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
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
                    throw new ArgumentException(SR.Argument_InvalidArrayType);
                }

                try
                {
                    // Null check not needed because we are enumerating this
                    foreach (var entry in this)
                        objects[index++] = entry;
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType);
                }
            }
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key is null)
                return TKeyIsNullable;

            return (key is TKey);
        }

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
            => CollectionUtil.ToString(formatProvider, format, this);

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

        #region Nested Class: KeyCollection

        /// <summary>
        /// Represents the collection of keys in a <see cref="Dictionary{TKey, TValue}"/>. This class cannot be inherited.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        private sealed class KeyCollection : ICollection<TKey>, ICollection
        {
            private readonly Dictionary<TKey, TValue> nullableKeyDictionary;
            private readonly ICollection<TKey> collection;
            public KeyCollection(Dictionary<TKey, TValue> nullableKeyDictionary, ICollection<TKey> collection)
            {
                this.nullableKeyDictionary = nullableKeyDictionary ?? throw new ArgumentNullException(nameof(nullableKeyDictionary));
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

            /// <summary>
            ///  Gets the number of elements contained in the <see cref="KeyCollection"/>.
            /// </summary>
            public int Count => nullableKeyDictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            public bool IsSynchronized => false;

            public object SyncRoot => ((ICollection)nullableKeyDictionary).SyncRoot;

            void ICollection<TKey>.Add(TKey item)
                => throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);

            void ICollection<TKey>.Clear()
                => throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);

            bool ICollection<TKey>.Contains(TKey item)
            {
                if (TKeyIsNullable && item is null)
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
                if (TKeyIsNullable && nullableKeyDictionary.hasNullKey)
                    return new Enumerator(nullableKeyDictionary, collection);
                return collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            bool ICollection<TKey>.Remove(TKey item)
                => throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);

            void ICollection.CopyTo(Array array, int index)
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
            internal struct Enumerator : IEnumerator<TKey>, IEnumerator
            {
                private readonly Dictionary<TKey, TValue> dictionary;
                private readonly IEnumerator<TKey> enumerator;
                private readonly int version;
                private TKey current;
                private bool nullKeySeen;
                public Enumerator(Dictionary<TKey, TValue> dictionary, ICollection<TKey> keyCollection)
                {
                    this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
                    this.enumerator = keyCollection.GetEnumerator();
                    this.version = dictionary.version;
                    current = default;
                    nullKeySeen = false;
                }

                public TKey Current => current;

                object IEnumerator.Current => Current;

                public void Dispose()
                    => enumerator.Dispose();

                public bool MoveNext()
                {
                    if (version != dictionary.version)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

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

                void IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                    nullKeySeen = false;
                    enumerator.Reset();
                }
            }

            #endregion
        }

        #endregion

        #region Nested Class: ValueCollection

        /// <summary>
        /// Represents the collection of values in a <see cref="Dictionary{TKey, TValue}"/>. This class cannot be inherited.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal sealed class ValueCollection : ICollection<TValue>, ICollection
        {
            private readonly Dictionary<TKey, TValue> nullableKeyDictionary;
            private readonly ICollection<TValue> collection;

            public ValueCollection(Dictionary<TKey, TValue> nullableKeyDictionary, ICollection<TValue> collection)
            {
                this.nullableKeyDictionary = nullableKeyDictionary ?? throw new ArgumentNullException(nameof(nullableKeyDictionary));
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

            public int Count => nullableKeyDictionary.Count;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)nullableKeyDictionary).SyncRoot;

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item)
                => throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);

            void ICollection<TValue>.Clear()
                => throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);

            bool ICollection<TValue>.Contains(TValue item)
            {
                if (TKeyIsNullable && nullableKeyDictionary.hasNullKey)
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

            void ICollection.CopyTo(Array array, int index)
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

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (array.Length - index < Count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                // Null check not needed because we are enumerating this
                foreach (var value in this)
                    array[index++] = value;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            bool ICollection<TValue>.Remove(TValue item)
                => throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);

            public IEnumerator<TValue> GetEnumerator()
            {
                // Only use our enumerator if we have a null value, otherwise, use the original
                if (TKeyIsNullable && nullableKeyDictionary.hasNullKey)
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
                private readonly Dictionary<TKey, TValue> dictionary;
                private readonly IEnumerator<TValue> enumerator;
                private readonly int version;
                private TValue current;
                private bool nullValueSeen;
                public Enumerator(Dictionary<TKey, TValue> dictionary, ICollection<TValue> valueCollection)
                {
                    this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
                    this.enumerator = valueCollection.GetEnumerator();
                    this.version = dictionary.version;
                    current = default;
                    nullValueSeen = false;
                }

                public TValue Current => current;

                object IEnumerator.Current => current;

                public void Dispose()
                    => enumerator.Dispose();

                public bool MoveNext()
                {
                    if (version != dictionary.version)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                    if (!nullValueSeen)
                    {
                        nullValueSeen = true;
                        current = dictionary.nullEntry.Value;
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

                void IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                    nullValueSeen = false;
                    enumerator.Reset();
                }
            }

            #endregion
        }

        #endregion

        #region Nested Structure: Enumerator

        /// <summary>
        /// Enumerates the elemensts of a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        internal struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly Dictionary<TKey, TValue> dictionary;
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
            private readonly int version;
            private int index;
            private bool nullKeySeen;
            private KeyValuePair<TKey, TValue> current;
            private readonly int getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int DictEntry = 1;
            internal const int KeyValuePair = 2;

            public Enumerator(Dictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary ?? throw new ArgumentException(nameof(dictionary));
                this.enumerator = dictionary.dictionary.GetEnumerator();
                this.getEnumeratorRetType = getEnumeratorRetType;
                version = dictionary.version;
                index = 0;
                nullKeySeen = false;
                current = default;
            }

            public KeyValuePair<TKey, TValue> Current => current;

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == dictionary.Count + 1))
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
                if (version != dictionary.version)
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                if (!nullKeySeen)
                {
                    index++;
                    nullKeySeen = true;
                    current = dictionary.nullEntry;
                    return true;
                }
                if (enumerator.MoveNext())
                {
                    index++;
                    current = enumerator.Current;
                    return true;
                }
                index = dictionary.Count + 1;
                current = default;
                return false;
            }

            public void Reset()
            {
                if (version != dictionary.version)
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                index = 0;
                nullKeySeen = false;
                enumerator.Reset();
            }

            #region IDictionaryEnumerator Members

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (index == 0 || (index == dictionary.Count + 1))
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                    return new DictionaryEntry(current.Key, current.Value);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (index == 0 || (index == dictionary.Count + 1))
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                    return current.Key;
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (index == 0 || (index == dictionary.Count + 1))
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                    return current.Value;
                }
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
        internal class ConcreteDictionary : SCG.Dictionary<TKey, TValue>, IConcreteDictionary<TKey, TValue>
        {
            public ConcreteDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer ?? EqualityComparer<TKey>.Default) { }
        }

        #endregion

    }

#region Interface: IConcreteDictionary<TKey, TValue>

    /// <summary>
    /// Interface to expose all of the members of the concrete <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> type,
    /// so we can duplicate them in other types without having to cast.
    /// </summary>
    internal interface IConcreteDictionary<TKey, TValue> : IDictionary<TKey, TValue>//, IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        IEqualityComparer<TKey> Comparer { get; }

        //bool ContainsValue(TValue value); // NOTE: We don't want to utilize the built-in method because
        // it uses the .NET default equality comparer, and we want to swap that.


#if FEATURE_DICTIONARY_ENSURECAPACITY
        int EnsureCapacity(int capacity);
#endif
#if FEATURE_DICTIONARY_TRIMEXCESS
        void TrimExcess(int capacity);

        void TrimExcess();
#endif

    }

#endregion

}
