#region Copyright 2014 by matarillo, Licensed under the The MIT License (MIT)
/*
The MIT License (MIT)

Copyright (c) 2014 matarillo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using J2N.Collections.ObjectModel;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace J2N.Collections.Generic
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Represents a collection of key/value pairs that are sorted based on insertion order.
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
    /// Usage Note: This class is designed to be a direct replacement for Java's LinkedHashMap, except that
    /// it doesn't contain a constructor overload with an order parameter to turn it into an LRU cache.
    /// <para/>
    /// Note that the <see cref="ToString()"/> method uses the current culture by default to behave like other
    /// components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [SuppressMessage("Style", "IDE0034:Simplify 'default' expression", Justification = "Following Microsoft's code styles")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public class LinkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IStructuralEquatable, IStructuralFormattable
#if FEATURE_SERIALIZABLE
            , System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
#endif
    {
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> dictionary;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private readonly LinkedList<KeyValuePair<TKey, TValue>> list;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private KeyCollection? keys;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private ValueCollection? values;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private int version;

#if FEATURE_SERIALIZABLE
        private System.Runtime.Serialization.SerializationInfo? siInfo; //A temporary variable which we need during deserialization.

        // names for serialization
        private const string EqualityComparerName = "EqualityComparer"; // Do not rename (binary serialization)
        private const string CountName = "Count"; // Do not rename (binary serialization) - used to allocate during deserialzation, not actually a field
        private const string KeyValuePairsName = "KeyValuePairs"; // Do not rename (binary serialization)
        private const string VersionName = "Version"; // Do not rename (binary serialization)
#endif

        #region Constructors

        public LinkedDictionary()
            : this(0, (IEqualityComparer<TKey>?)null)
        {
        }

        public LinkedDictionary(IEqualityComparer<TKey>? comparer)
            : this(0, comparer)
        {
        }

        public LinkedDictionary(int capacity)
            : this(capacity, (IEqualityComparer<TKey>?)null)
        {
        }

        public LinkedDictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, SR.ArgumentOutOfRange_NeedNonNegNum);

            dictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity, comparer ?? EqualityComparer<TKey>.Default);
            list = new LinkedList<KeyValuePair<TKey, TValue>>();
        }

        public LinkedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }

        public LinkedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer)
            : this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }

        public LinkedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(collection, null)
        { }

        public LinkedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
            : this(collection is ICollection<KeyValuePair<TKey, TValue>> col ? col.Count : 0, comparer)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var pair in collection)
                Add(pair.Key, pair.Value);
        }

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedDictionary{TKey, TValue}"/> class with serialized data.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object containing
        /// the information required to serialize the <see cref="LinkedDictionary{TKey, TValue}"/>.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure containing
        /// the source and destination of the serialized stream associated with the <see cref="LinkedDictionary{TKey, TValue}"/>.</param>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute an object transmitted over a stream.
        /// For more information, see
        /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/serialization/xml-and-soap-serialization">XML and SOAP Serialization</a>.
        /// </remarks>
        protected LinkedDictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            siInfo = info;
            int capacity = info.GetInt32(CountName);
            var comparer = (IEqualityComparer<TKey>?)siInfo.GetValue(EqualityComparerName, typeof(IEqualityComparer<TKey>));
            dictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity, comparer);
            list = new LinkedList<KeyValuePair<TKey, TValue>>();
        }

#endif

        #endregion

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="LinkedDictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="LinkedDictionary{TKey, TValue}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlyDictionary{TKey, TValue}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="LinkedDictionary{TKey, TValue}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlyDictionary<TKey, TValue> AsReadOnly()
            => new ReadOnlyDictionary<TKey, TValue>(this);

        #endregion AsReadOnly

        #region SCG.Dictionary<TKey, TValue> Members

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine equality of keys
        /// for the dictionary.
        /// </summary>
        /// <remarks>
        /// <see cref="LinkedDictionary{TKey, TValue}"/> requires an equality implementation to determine
        /// whether keys are equal. You can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter; if you do not
        /// specify one, J2N's default generic equality comparer <see cref="EqualityComparer{T}.Default"/> is used.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public IEqualityComparer<TKey> EqualityComparer => dictionary.EqualityComparer;

        /// <summary>
        /// Determines whether the <see cref="LinkedDictionary{TKey, TValue}"/> contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if the <see cref="LinkedDictionary{TKey, TValue}"/> contains an element
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
            return ContainsValue(value, null);
        }

        /// <summary>
        /// Determines whether the <see cref="LinkedDictionary{TKey, TValue}"/> contains a specific value
        /// as determined by the provided <paramref name="valueComparer"/>.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <param name="valueComparer">The <see cref="IEqualityComparer{TValue}"/> to use
        /// to test each value for equality.</param>
        /// <returns><c>true</c> if the <see cref="LinkedDictionary{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method performs a linear search; therefore, the average execution time
        /// is proportional to <see cref="Count"/>. That is, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue(TValue value, IEqualityComparer<TValue>? valueComparer) // Overload added so end user can override J2N's equality comparer
        {
            valueComparer ??= EqualityComparer<TValue>.Default;

            foreach (var pair in dictionary)
            {
                if (valueComparer.Equals(pair.Value.Value.Value, value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="LinkedDictionary{TKey, TValue}"/> to the specified array
        /// of <see cref="KeyValuePair{TKey, TValue}"/> structures, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array of <see cref="KeyValuePair{TKey, TValue}"/> structures
        /// that is the destination of the elements copied from the current <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentException">The number of elements in the source array is greater
        /// than the available space from <paramref name="index"/> to the end of the destination array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <remarks>This method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - index < Count)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            foreach (var item in list)
                array[index++] = item;
        }


#if FEATURE_DICTIONARY_ENSURECAPACITY

        /// <summary>
        /// Ensures that the dictionary can hold up to a specified number of entries without any
        /// further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">The number of entries.</param>
        /// <returns>The current capacity of the <see cref="LinkedDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), SR.ArgumentOutOfRange_NeedNonNegNum);

            version++;
            return dictionary.EnsureCapacity(capacity);
        }
#endif
#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and returns the data needed to
        /// serialize the <see cref="LinkedDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains the
        /// information required to serialize the <see cref="LinkedDictionary{TKey, TValue}"/>.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure that contains
        /// the source and destination of the serialized stream associated with the <see cref="LinkedDictionary{TKey, TValue}"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <c>null</c>.</exception>
        /// <remarks>This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        [System.Security.SecurityCritical]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            // Customized serialization for LinkedDictionary
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            int count = Count;
            info.AddValue(CountName, count);
            info.AddValue(EqualityComparerName, EqualityComparer, typeof(IEqualityComparer<TKey>));
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
        /// <see cref="LinkedDictionary{TKey, TValue}"/> instance is invalid.</exception>
        /// <remarks>This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        public virtual void OnDeserialization(object? sender)
        {
            if (siInfo == null)
            {
                return; //Somebody had a dependency on this Dictionary and fixed us up before the ObjectManager got to it.
            }

            int count = siInfo.GetInt32(CountName);
            if (count > 0)
            {
                KeyValuePair<TKey, TValue>[]? array = (KeyValuePair<TKey, TValue>[]?)
                    siInfo.GetValue(KeyValuePairsName, typeof(KeyValuePair<TKey, TValue>[]));

                if (array == null)
                {
                    throw new System.Runtime.Serialization.SerializationException(SR.Serialization_MissingKeys);
                }

                for (int i = 0; i < array.Length; i++)
                {
                    var item = array[i];
                    DoAdd(item.Key, item.Value);
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
            version++;
            dictionary.TrimExcess(capacity);
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
            version++;
            dictionary.TrimExcess();
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
        // J2N: Explicitly defined to undercut the extension method in CollectionExtensions that doesn't allow
        // null keys.
        public bool TryAdd([AllowNull] TKey key, [AllowNull] TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                DoAdd(key, value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// If the element exists, the associated <paramref name="value"/> is output after it is removed.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if key is not found in the <see cref="LinkedDictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// If the <see cref="LinkedDictionary{TKey, TValue}"/> does not contain an element with the specified key, the
        /// <see cref="LinkedDictionary{TKey, TValue}"/> remains unchanged. No exception is thrown.
        /// </remarks>
        // J2N: This is an extension method on IDictionary<TKey, TValue>, but only for .NET Standard 2.1+.
        // It is redefined here to ensure we have it in prior platforms.
        public bool Remove([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (dictionary.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
            {
                value = node.Value.Value;
                DoRemove(node);
                return true;
            }

            value = default;
            return false;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> for the
        /// <see cref="LinkedDictionary{TKey, TValue}"/>.</returns>
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
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IDictionary<TKey, TValue> Members

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The keys in the <see cref="LinkedDictionary{TKey, TValue}.KeyCollection"/> are sorted according
        /// the order in which they were inserted, and are in the same order as the associated values in
        /// the <see cref="LinkedDictionary{TKey, TValue}.ValueCollection"/> returned by the <see cref="Values"/> property.
        /// <para/>
        /// The returned <see cref="LinkedDictionary{TKey, TValue}.KeyCollection"/> is not a static copy; instead,
        /// the <see cref="LinkedDictionary{TKey, TValue}.KeyCollection"/> refers back to the keys in the original
        /// <see cref="LinkedDictionary{TKey, TValue}"/>. Therefore, changes to the <see cref="LinkedDictionary{TKey, TValue}"/>
        /// continue to be reflected in the <see cref="LinkedDictionary{TKey, TValue}.KeyCollection"/>.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public ICollection<TKey> Keys
        {
            get
            {
                if (keys == null) keys = new KeyCollection(this);
                return keys;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The values in the <see cref="LinkedDictionary{TKey, TValue}.ValueCollection"/> are sorted according to
        /// the order in which they were inserted, and are in the same order as the associated keys in the
        /// <see cref="LinkedDictionary{TKey, TValue}.KeyCollection"/> returned by the <see cref="Keys"/> property.
        /// <para/>
        /// The returned <see cref="LinkedDictionary{TKey, TValue}.ValueCollection"/> is not a static copy;
        /// instead, the <see cref="LinkedDictionary{TKey, TValue}.ValueCollection"/> refers back to the
        /// values in the original <see cref="LinkedDictionary{TKey, TValue}"/>. Therefore, changes to
        /// the <see cref="LinkedDictionary{TKey, TValue}"/> continue to be reflected in the
        /// <see cref="LinkedDictionary{TKey, TValue}.ValueCollection"/>.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public ICollection<TValue> Values
        {
            get
            {
                if (values == null) values = new ValueCollection(this);
                return values;
            }
        }

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
        /// that does not exist in the <see cref="LinkedDictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c>. However, if the specified key already exists in
        /// the <see cref="LinkedDictionary{TKey, TValue}"/>, setting the <see cref="this[TKey]"/> property overwrites
        /// the old value. In contrast, the <see cref="Add(TKey, TValue)"/> method does not modify existing elements.
        /// <para/>
        /// Both keys and values can be <c>null</c> if either <see cref="Nullable{T}"/> or a reference type.
        /// <para/>
        /// The C# language uses the <see cref="this"/> keyword to define the indexers instead of implementing the
        /// <c>Item[TKey]</c> property. Visual Basic implements <c>Item[TKey]</c> as a default property, which provides
        /// the same indexing functionality.
        /// <para/>
        /// Getting the value of this property is an O(log <c>n</c>) operation; setting the property is also
        /// an O(log <c>n</c>) operation.
        /// </remarks>
        public TValue this[[AllowNull] TKey key]
        {
            get
            {
                if (!dictionary.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
                {
                    throw new KeyNotFoundException(J2N.SR.Format(SR.Arg_KeyNotFoundWithKey, key));
                }

                if (node is null)
                    return default(TValue)!;

                return node.Value.Value;
            }
            set
            {
                if (!dictionary.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
                {
                    DoAdd(key, value);
                    return;
                }
                DoSet(node, key, value);
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists
        /// in the <see cref="LinkedDictionary{TKey, TValue}"/>.</exception>
        /// <remarks>
        /// You can also use the <see cref="this[TKey]"/> property to add new elements by setting the value of
        /// a key that does not exist in the <see cref="LinkedDictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c> (in Visual Basic, <c>myCollection("myNonexistantKey") = myValue</c>).
        /// However, if the specified key already exists in the <see cref="LinkedDictionary{TKey, TValue}"/>, setting
        /// the <see cref="this[TKey]"/> property overwrites the old value. In contrast, the <see cref="Add(TKey, TValue)"/>
        /// method throws an exception if an element with the specified key already exists.
        /// <para/>
        /// Both keys and values can be <c>null</c> if the corresponding <typeparamref name="TKey"/> or
        /// <typeparamref name="TValue"/> is <see cref="Nullable{T}"/> or a reference type.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void Add([AllowNull] TKey key, [AllowNull] TValue value)
        {
            if (dictionary.ContainsKey(key))
                throw new ArgumentException(J2N.SR.Format(SR.Argument_AddingDuplicate, key));
            DoAdd(key, value);
        }

        /// <summary>
        /// Determines whether the <see cref="LinkedDictionary{TKey, TValue}"/> contains an
        /// element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="LinkedDictionary{TKey, TValue}"/>. The key can be <c>null</c></param>
        /// <returns><c>true</c> if the <see cref="LinkedDictionary{TKey, TValue}"/> contains an element
        /// with the specified key; otherwise, <c>false</c>.</returns>
        /// <remarks>This method is an O(log <c>n</c>) operation.</remarks>
        public bool ContainsKey([AllowNull] TKey key)
            => dictionary.ContainsKey(key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns><c>true</c> if the <see cref="LinkedDictionary{TKey, TValue}"/> contains an element with the
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
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            if (dictionary.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
            {
                value = node.Value.Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if key is not found in the <see cref="LinkedDictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// If the <see cref="LinkedDictionary{TKey, TValue}"/> does not contain an element with the specified key, the
        /// <see cref="LinkedDictionary{TKey, TValue}"/> remains unchanged. No exception is thrown.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation.
        /// </remarks>
        public bool Remove([AllowNull] TKey key)
        {
            if (!dictionary.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
            {
                return false;
            }
            DoRemove(node);
            return true;
        }

        #endregion

        #region Private Helper Methods

        private void DoAdd([AllowNull] TKey key, [AllowNull] TValue value)
        {
            version++;
            var pair = new KeyValuePair<TKey, TValue>(key!, value!);
            var node = new LinkedListNode<KeyValuePair<TKey, TValue>>(pair);
            dictionary.Add(key, node);
            list.AddLast(node);
        }

        private void DoSet(LinkedListNode<KeyValuePair<TKey, TValue>> node, [AllowNull] TKey key, [AllowNull] TValue value)
        {
            version++;
            var pair = new KeyValuePair<TKey, TValue>(key!, value!);
            var newNode = new LinkedListNode<KeyValuePair<TKey, TValue>>(pair);
            dictionary[key] = newNode;
            list.AddAfter(node, newNode);
            list.Remove(node);
        }

        private void DoRemove(LinkedListNode<KeyValuePair<TKey, TValue>> node)
        {
            version++;
            dictionary.Remove(node.Value.Key);
            list.Remove(node);
        }

        private static bool IsCompatibleKey(object? key)
        {
            if (key is null)
                return typeof(TKey).IsNullableType();

            return (key is TKey);
        }

        private bool TryGetNode([AllowNull] TKey key, [AllowNull] TValue value, [MaybeNullWhen(false)] out LinkedListNode<KeyValuePair<TKey, TValue>> node)
        {
            if (dictionary.TryGetValue(key, out node) && EqualityComparer<TValue>.Default.Equals(value!, node.Value.Value))
                return true;
            node = null;
            return false;
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        /// Removes all elements from the <see cref="LinkedDictionary{TKey, TValue}"/>.
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
            version++;
            dictionary.Clear();
            list.Clear();
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => dictionary.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetNode(item.Key, item.Value, out _);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!TryGetNode(item.Key, item.Value, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
            {
                return false;
            }
            DoRemove(node);
            return true;
        }

        #endregion

        #region IDictionary Members

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        ICollection IDictionary.Keys => (ICollection)Keys;

        ICollection IDictionary.Values => (ICollection)Values;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((IDictionary)dictionary).SyncRoot;

        object? IDictionary.this[object? key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    if (TryGetValue((TKey)key!, out TValue? value))
                    {
                        return value;
                    }
                }

                return null;
            }
            set
            {
                // J2N: Only throw if the generic closing type is not nullable
                if (key is null && !typeof(TKey).IsNullableType())
                    throw new ArgumentNullException(nameof(key));
                if (value is null && !typeof(TValue).IsNullableType())
                    throw new ArgumentNullException(nameof(value));

                try
                {
                    TKey tempKey = (TKey)key!;
                    try
                    {
                        this[tempKey] = (TValue)value!;
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

        void IDictionary.Add(object? key, object? value)
        {
            // J2N: Only throw if the generic closing type is not nullable
            if (key is null && !typeof(TKey).IsNullableType())
                throw new ArgumentNullException(nameof(key));
            if (value is null && !typeof(TValue).IsNullableType())
                throw new ArgumentNullException(nameof(value));

            try
            {
                TKey tempKey = (TKey)key!;

                try
                {
                    Add(tempKey, (TValue)value!);
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

        bool IDictionary.Contains(object? key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key!);
            }
            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.DictEntry);
        }

        void IDictionary.Remove(object? key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key!);
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
                foreach (var item in this)
                    dictEntryArray[index++] = new DictionaryEntry(item.Key!, item.Value);
            }
            else
            {
                if (!(array is object[] objects))
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType);
                }
                try
                {
                    foreach (var item in this)
                        objects[index++] = item;
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType);
                }
            }
        }

        #endregion

#if FEATURE_IREADONLYCOLLECTIONS
        #region IReadOnlyDictionary<TKey, TValue> Members

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        #endregion IReadOnlyDictionary<TKey, TValue> Members
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
        public override bool Equals(object? obj)
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
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
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

        #region Nested Structure: Enumerator

        /// <summary>
        /// Enumerates the elements of a <see cref="LinkedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
        /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
        /// <para/>
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// <para/>
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
        /// <see cref="Enumerator.Current"/> property is undefined. Therefore, you must call the
        /// <see cref="Enumerator.MoveNext()"/> method to advance the enumerator to the first element
        /// of the collection before reading the value of <see cref="Enumerator.Current"/>.
        /// <para/>
        /// The <see cref="Enumerator.Current"/> property returns the same object until
        /// <see cref="Enumerator.MoveNext()"/> is called. <see cref="Enumerator.MoveNext()"/>
        /// sets <see cref="Enumerator.Current"/> to the next element.
        /// <para/>
        /// If <see cref="Enumerator.MoveNext()"/> passes the end of the collection, the enumerator is
        /// positioned after the last element in the collection and <see cref="Enumerator.MoveNext()"/>
        /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="Enumerator.MoveNext()"/>
        /// also return <c>false</c>. If the last call to <see cref="Enumerator.MoveNext()"/> returned <c>false</c>,
        /// <see cref="Enumerator.Current"/> is undefined. You cannot set <see cref="Enumerator.Current"/>
        /// to the first element of the collection again; you must create a new enumerator object instead.
        /// <para/>
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
        /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
        /// to <see cref="Enumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
        /// <see cref="InvalidOperationException"/>.
        /// <para/>
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
        /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
        /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
        /// reading and writing, you must implement your own synchronization.
        /// <para/>
        /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
        /// </remarks>
        internal struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly LinkedDictionary<TKey, TValue> dictionary;
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
            private readonly int _getEnumeratorRetType;  // What should Enumerator.Current return?
            private bool notStartedOrEnded;
            [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Following Microsoft's code style")]
            private int version;

            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;

            internal Enumerator(LinkedDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
                enumerator = dictionary.list.GetEnumerator();
                _getEnumeratorRetType = getEnumeratorRetType;
                notStartedOrEnded = true;
                version = dictionary.version;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="SortedDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
            /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            /// <remarks>
            /// After an enumerator is created, the enumerator is positioned before the first element in the collection,
            /// and the first call to the <see cref="MoveNext()"/> method advances the enumerator to the first element
            /// of the collection.
            /// <para/>
            /// If MoveNext passes the end of the collection, the enumerator is positioned after the last element in the
            /// collection and <see cref="MoveNext()"/> returns <c>false</c>. When the enumerator is at this position,
            /// subsequent calls to <see cref="MoveNext()"/> also return <c>false</c>.
            /// <para/>
            /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the
            /// collection, such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated
            /// and the next call to <see cref="MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
            /// <see cref="InvalidOperationException"/>.
            /// </remarks>
            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                if (enumerator.MoveNext())
                {
                    notStartedOrEnded = false;
                    return true;
                }
                notStartedOrEnded = true;
                return false;
            }

            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            public void Dispose()
            {
                enumerator.Dispose();
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <remarks>
            /// <see cref="Current"/> is undefined under any of the following conditions:
            /// <list type="bullet">
            ///     <item><description>
            ///         The enumerator is positioned before the first element of the collection. That happens after an
            ///         enumerator is created or after the <see cref="IEnumerator.Reset()"/> method is called. The <see cref="MoveNext()"/>
            ///         method must be called to advance the enumerator to the first element of the collection before reading the value of
            ///         the <see cref="Current"/> property.
            ///     </description></item>
            ///     <item><description>
            ///         The last call to <see cref="MoveNext()"/> returned <c>false</c>, which indicates the end of the collection and that the
            ///         enumerator is positioned after the last element of the collection.
            ///     </description></item>
            ///     <item><description>
            ///         The enumerator is invalidated due to changes made in the collection, such as adding, modifying, or deleting elements.
            ///     </description></item>
            /// </list>
            /// <para/>
            /// <see cref="Current"/> does not move the position of the enumerator, and consecutive calls to <see cref="Current"/> return
            /// the same object until either <see cref="MoveNext()"/> or <see cref="IEnumerator.Reset()"/> is called.
            /// </remarks>
            public KeyValuePair<TKey, TValue> Current => enumerator.Current;

            void IEnumerator.Reset()
            {
                notStartedOrEnded = true;
                enumerator.Reset();
            }

            object IEnumerator.Current
            {
                get
                {
                    if (notStartedOrEnded)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    if (_getEnumeratorRetType == DictEntry)
                    {
                        return new DictionaryEntry(Current.Key!, Current.Value);
                    }
                    else
                    {
                        return new KeyValuePair<TKey, TValue>(Current.Key, Current.Value);
                    }
                }
            }

            object? IDictionaryEnumerator.Key
            {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
                get
#pragma warning restore CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
                {
                    if (notStartedOrEnded)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return Current.Key;
                }
            }

            object? IDictionaryEnumerator.Value
            {
                get
                {
                    if (notStartedOrEnded)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return Current.Value;
                }
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (notStartedOrEnded)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return new DictionaryEntry(Current.Key!, Current.Value);
                }
            }
        }

        #endregion

        #region Nested Class: KeyCollection

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        [DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        internal sealed class KeyCollection : ICollection<TKey>, ICollection
        {
            private readonly LinkedDictionary<TKey, TValue> dictionary;

            public KeyCollection(LinkedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public int Count => dictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            int ICollection.Count => dictionary.Count;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                return dictionary.dictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (array.Length - index < Count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                foreach (var element in dictionary.list)
                    array[index++] = element.Key;
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return new Enumerator(dictionary.list.GetEnumerator());
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
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

                if (array is TKey[] keys)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    if (!(array is object?[]))
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }
                    try
                    {
                        object?[] objects = (object?[])array;
                        foreach (var item in this)
                            objects[index++] = item;
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region Nested Structure: Enumerator

#if FEATURE_SERIALIZABLE
            [Serializable]
#endif
            internal struct Enumerator : IEnumerator<TKey>, IEnumerator
            {
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
                private bool notStartedOrEnded;

                public Enumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
                {
                    this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
                    notStartedOrEnded = true;
                }

                public TKey Current => enumerator.Current.Key;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (notStartedOrEnded)
                        {
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                        }

                        return enumerator.Current.Key;
                    }
                }

                public void Dispose()
                {
                    enumerator.Dispose();
                }

                public bool MoveNext()
                {
                    if (enumerator.MoveNext())
                    {
                        notStartedOrEnded = false;
                        return true;
                    }
                    notStartedOrEnded = true;
                    return false;
                }

                void IEnumerator.Reset()
                {
                    enumerator.Reset();
                }
            }

            #endregion
        }

        #endregion

        #region Nested Class: ValueCollection

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        [DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        internal sealed class ValueCollection : ICollection<TValue>, ICollection
        {
            private readonly LinkedDictionary<TKey, TValue> dictionary;

            public ValueCollection(LinkedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public int Count => dictionary.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                foreach (var pair in dictionary.dictionary)
                {
                    if (EqualityComparer<TValue>.Default.Equals(pair.Value.Value.Value, item))
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Copies the <see cref="ValueCollection"/> elements to an existing one-dimensional
            /// array, starting at the specified array index.
            /// </summary>
            /// <param name="array">The one-dimensional array that is the destination of the elements copied from
            /// the <see cref="ValueCollection"/>. The array must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
            /// <exception cref="ArgumentException">The number of elements in the source <see cref="ValueCollection"/>
            /// is greater than the available space from <paramref name="index"/> to the end of the destination
            /// <paramref name="array"/>.</exception>
            /// <remarks>
            /// The elements are copied to the array in the same order in which the enumerator iterates through the
            /// <see cref="ValueCollection"/>.
            /// <para/>
            /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
            /// </remarks>
            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (array.Length - index < Count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                foreach (var element in dictionary.list)
                    array[index++] = element.Value;
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
                    if (!(array is object?[]))
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }
                    try
                    {
                        object?[] objects = (object?[])array;
                        foreach (var entry in this)
                            objects[index++] = entry;
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }
                }
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new Enumerator(dictionary.list.GetEnumerator());
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region Nested Structure: Enumerator

#if FEATURE_SERIALIZABLE
            [Serializable]
#endif
            internal struct Enumerator : IEnumerator<TValue>, IEnumerator
            {
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
                private bool notStartedOrEnded;

                public Enumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
                {
                    this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
                    notStartedOrEnded = true;
                }

                public TValue Current => enumerator.Current.Value;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (notStartedOrEnded)
                        {
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                        }

                        return enumerator.Current.Value;
                    }
                }

                public void Dispose()
                {
                    enumerator.Dispose();
                }

                public bool MoveNext()
                {
                    if (enumerator.MoveNext())
                    {
                        notStartedOrEnded = false;
                        return true;
                    }
                    notStartedOrEnded = true;
                    return false;
                }

                void IEnumerator.Reset()
                {
                    enumerator.Reset();
                }
            }

            #endregion
        }

        #endregion
    }
}
