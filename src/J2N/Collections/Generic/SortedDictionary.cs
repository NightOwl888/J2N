// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if FEATURE_SERIALIZABLE
using System.Runtime.Serialization;
#endif
using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Represents a collection of key/value pairs that are sorted on the key.
    /// <para/>
    /// <see cref="SortedDictionary{TKey, TValue}"/> differs from <see cref="System.Collections.Generic.SortedDictionary{TKey, TValue}"/>
    /// in the following ways:
    /// <list type="bullet">
    ///     <item><description>
    ///         If <typeparamref name="TKey"/> is <see cref="Nullable{T}"/> or a reference type, the key can be
    ///         <c>null</c> without throwing an exception.
    ///     </description></item>
    ///     <item><description>
    ///         The <see cref="Equals(object)"/> and <see cref="GetHashCode()"/> methods are implemented to compare dictionaries
    ///         using structural equality by default. Also, <see cref="IStructuralEquatable"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         The <see cref="ToString()"/> method is overridden to list the contents of the dictionary
    ///         in the current culture by default. Also, <see cref="IFormattable"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Uses <see cref="Comparer{T}.Default"/> by default, which provides some specialized equality comparisons
    ///         for specific types to match the behavior of Java.
    ///     </description></item>
    /// </list>
    /// <para/>
    /// Usage Note: This class is intended to be a direct replacement for <see cref="System.Collections.Generic.SortedDictionary{TKey, TValue}"/> in order
    /// to provide default structural equality and formatting behavior similar to Java. Note that the <see cref="ToString()"/>
    /// method uses the current culture by default to behave like other components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("Count = {Count}")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>, IStructuralEquatable, IStructuralFormattable
    {
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private KeyCollection/*?*/ _keys;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private ValueCollection/*?*/ _values;

        private readonly TreeSet<KeyValuePair<TKey, TValue>> _set; // Do not rename (binary serialization)

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey, TValue}"/> class that is
        /// empty and uses J2N's default <see cref="IComparer{T}"/> implementation for the key type.
        /// </summary>
        /// <remarks>
        /// Every key in a <see cref="SortedDictionary{TKey, TValue}"/> must be unique according to the default comparer.
        /// <para/>
        /// <see cref="SortedDictionary{TKey, TValue}"/> requires a comparer implementation to perform key comparisons.
        /// This constructor uses J2N's default generic equality comparer <see cref="Comparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IComparable{T}"/> generic interface, the
        /// default comparer uses that implementation. Alternatively, you can specify an implementation of the
        /// <see cref="IComparer{T}"/> generic interface by using a constructor that accepts a comparer parameter.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public SortedDictionary() : this((IComparer<TKey>/*?*/)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey, TValue}"/> class that contains
        /// elements copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses J2N's default
        /// <see cref="IComparer{T}"/> implementation for the key type.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied
        /// to the new <see cref="SortedDictionary{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Every key in a <see cref="SortedDictionary{TKey, TValue}"/> must be unique according to the default
        /// comparer; therefore, every key in the source dictionary must also be unique according to the default comparer.
        /// <para/>
        /// <see cref="SortedDictionary{TKey, TValue}"/> requires a comparer implementation to perform key comparisons.
        /// This constructor uses J2N's default generic equality comparer, <see cref="Comparer{T}.Default"/>. If type <typeparamref name="TKey"/>
        /// implements the <see cref="IComparable{T}"/> generic interface, the default comparer uses that implementation
        /// (except for some types that have been overridden to match Java's default behavior).
        /// Alternatively, you can specify an implementation of the <see cref="IComparer{T}"/> generic interface by using
        /// a constructor that accepts a comparer parameter.
        /// <para/>
        /// This constructor is an O(<c>n</c> log <c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="dictionary"/>.
        /// </remarks>
        public SortedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey, TValue}"/> class that contains
        /// elements copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified
        /// <see cref="IComparer{T}"/> implementation to compare keys.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied
        /// to the new <see cref="SortedDictionary{TKey, TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing
        /// keys, or <c>null</c> to use J2N's default <see cref="Comparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Every key in a <see cref="SortedDictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// therefore, every key in the source dictionary must also be unique according to the specified comparer.
        /// <para/>
        /// <see cref="SortedDictionary{TKey, TValue}"/> requires a comparer implementation to perform key comparisons.
        /// If <paramref name="comparer"/> is <c>null</c>, this constructor uses J2N's default generic equality comparer,
        /// <see cref="Comparer{T}.Default"/>. If type <typeparamref name="TKey"/> implements the <see cref="IComparable{T}"/>
        /// generic interface, the default comparer uses that implementation (except for some types that have been
        /// overridden to match Java's default behavior).
        /// <para/>
        /// This constructor is an O(<c>n</c> log <c>n</c>) operation, where <c>n</c> is the number of elements in dictionary.
        /// </remarks>
        public SortedDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey>/*?*/ comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            _set = new TreeSet<KeyValuePair<TKey, TValue>>(new KeyValuePairComparer(comparer));

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                _set.Add(pair);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey, TValue}"/> class that
        /// is empty and uses the specified <see cref="IComparer{T}"/> implementation to compare keys.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing
        /// keys, or <c>null</c> to use J2N's default <see cref="Comparer{T}"/> for the type of the key.</param>
        /// <remarks>
        /// Every key in a <see cref="SortedDictionary{TKey, TValue}"/> must be unique according to the specified comparer.
        /// <para/>
        /// <see cref="SortedDictionary{TKey, TValue}"/> requires a comparer implementation to perform key comparisons.
        /// If comparer is null, this constructor uses J2N's default generic equality comparer, <see cref="Comparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IComparable{T}"/> generic interface, the
        /// default comparer uses that implementation (except for some types that have been overridden to match Java's
        /// default behavior).
        /// </remarks>
        public SortedDictionary(IComparer<TKey>/*?*/ comparer)
        {
            _set = new TreeSet<KeyValuePair<TKey, TValue>>(new KeyValuePairComparer(comparer));
        }

        #endregion

        #region SCG.SortedDictionary<TKey, TValue> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            _set.Add(keyValuePair);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TreeSet<KeyValuePair<TKey, TValue>>.Node/*?*/ node = _set.FindNode(keyValuePair);
            if (node == null)
            {
                return false;
            }

            if (keyValuePair.Value == null)
            {
                return node.Item.Value == null;
            }
            else
            {
                return EqualityComparer<TValue>.Default.Equals(node.Item.Value, keyValuePair.Value);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TreeSet<KeyValuePair<TKey, TValue>>.Node/*?*/ node = _set.FindNode(keyValuePair);
            if (node == null)
            {
                return false;
            }

            if (EqualityComparer<TValue>.Default.Equals(node.Item.Value, keyValuePair.Value))
            {
                _set.Remove(keyValuePair);
                return true;
            }
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
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
        /// that does not exist in the <see cref="SortedDictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c>. However, if the specified key already exists in
        /// the <see cref="SortedDictionary{TKey, TValue}"/>, setting the <see cref="this[TKey]"/> property overwrites
        /// the old value. In contrast, the <see cref="Add(TKey, TValue)"/> method does not modify existing elements.
        /// <para/>
        /// Unlike the <see cref="System.Collections.Generic.SortedDictionary{TKey, TValue}"/>, both keys and values can
        /// be <c>null</c> a key can be null if either <see cref="Nullable{T}"/> or a reference type.
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
                //if (key == null) // J2N: Making key nullable
                //{
                //    throw new ArgumentNullException(nameof(key));
                //}

                TreeSet<KeyValuePair<TKey, TValue>>.Node node = _set.FindNode(new KeyValuePair<TKey, TValue>(key, default));
                if (node == null)
                {
                    throw new KeyNotFoundException(J2N.SR.Format(SR.Arg_KeyNotFoundWithKey, string.Format(StringFormatter.CurrentCulture, "{0}", key)));
                }

                return node.Item.Value;
            }
            set
            {
                //if (key == null) // J2N: Making key nullable
                //{
                //    throw new ArgumentNullException(nameof(key));
                //}

                TreeSet<KeyValuePair<TKey, TValue>>.Node node = _set.FindNode(new KeyValuePair<TKey, TValue>(key, default));
                if (node == null)
                {
                    _set.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
                else
                {
                    node.Item = new KeyValuePair<TKey, TValue>(node.Item.Key, value);
                    _set.UpdateVersion();
                }
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => _set.Count;

        /// <summary>
        /// Gets the <see cref="IComparer{T}"/> used to order the elements of the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="SortedDictionary{TKey, TValue}"/> requires a comparer implementation to perform key comparisons.
        /// You can specify an implementation of the <see cref="IComparer{T}"/> generic interface by using a constructor
        /// that accepts a comparer parameter. If you do not, J2N's default generic equality comparer, <see cref="Comparer{T}.Default"/>,
        /// is used. If type <typeparamref name="TKey"/> implements the <see cref="IComparable{T}"/> generic interface,
        /// the default comparer uses that implementation (except for some types that have been overridden to match Java's
        /// default behavior).
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public IComparer<TKey> Comparer
            => ((KeyValuePairComparer)_set.Comparer).keyComparer;

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The keys in the <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> are sorted according
        /// to the <see cref="Comparer"/> property and are in the same order as the associated values in
        /// the <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> returned by the <see cref="Values"/> property.
        /// <para/>
        /// The returned <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> is not a static copy; instead,
        /// the <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> refers back to the keys in the original
        /// <see cref="SortedDictionary{TKey, TValue}"/>. Therefore, changes to the <see cref="SortedDictionary{TKey, TValue}"/>
        /// continue to be reflected in the <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public KeyCollection Keys
        {
            get
            {
                if (_keys == null) _keys = new KeyCollection(this);
                return _keys;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        /// Gets a collection containing the values in the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The values in the <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> are sorted according to
        /// the <see cref="Comparer"/> property, and are in the same order as the associated keys in the
        /// <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> returned by the <see cref="Keys"/> property.
        /// <para/>
        /// The returned <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> is not a static copy;
        /// instead, the <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> refers back to the
        /// values in the original <see cref="SortedDictionary{TKey, TValue}"/>. Therefore, changes to
        /// the <see cref="SortedDictionary{TKey, TValue}"/> continue to be reflected in the
        /// <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/>.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public ValueCollection Values
        {
            get
            {
                if (_values == null) _values = new ValueCollection(this);
                return _values;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists
        /// in the <see cref="SortedDictionary{TKey, TValue}"/>.</exception>
        /// <remarks>
        /// You can also use the <see cref="this[TKey]"/> property to add new elements by setting the value of
        /// a key that does not exist in the <see cref="SortedDictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c> (in Visual Basic, <c>myCollection("myNonexistantKey") = myValue</c>).
        /// However, if the specified key already exists in the <see cref="SortedDictionary{TKey, TValue}"/>, setting
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
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}
            _set.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Removes all elements from the <see cref="SortedDictionary{TKey, TValue}"/>.
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
            _set.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="SortedDictionary{TKey, TValue}"/> contains an
        /// element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="SortedDictionary{TKey, TValue}"/>. The key can be <c>null</c></param>
        /// <returns><c>true</c> if the <see cref="SortedDictionary{TKey, TValue}"/> contains an element
        /// with the specified key; otherwise, <c>false</c>.</returns>
        /// <remarks>This method is an O(log <c>n</c>) operation.</remarks>
        public bool ContainsKey(TKey key)
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}

            return _set.Contains(new KeyValuePair<TKey, TValue>(key, default));
        }

        /// <summary>
        /// Determines whether the <see cref="SortedDictionary{TKey, TValue}"/> contains an element with
        /// the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="SortedDictionary{TKey, TValue}"/>. The value can be <c>null</c>.</param>
        /// <returns><c>true</c> if the <see cref="SortedDictionary{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method determines equality using J2N's default equality comparer <see cref="EqualityComparer{T}.Default"/>
        /// for the value type <typeparamref name="TValue"/>.
        /// <para/>
        /// This method performs a linear search; therefore, the average execution time is proportional to the <see cref="Count"/> property.
        /// That is, this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue(TValue value)
        {
            bool found = false;
            if (value == null)
            {
                _set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node)
                {
                    if (node.Item.Value == null)
                    {
                        found = true;
                        return false;  // stop the walk
                    }
                    return true;
                });
            }
            else
            {
                IEqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;
                _set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node)
                {
                    if (valueComparer.Equals(node.Item.Value, value))
                    {
                        found = true;
                        return false;  // stop the walk
                    }
                    return true;
                });
            }
            return found;
        }

        /// <summary>
        /// Copies the elements of the <see cref="SortedDictionary{TKey, TValue}"/> to the specified array
        /// of <see cref="KeyValuePair{TKey, TValue}"/> structures, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array of <see cref="KeyValuePair{TKey, TValue}"/> structures
        /// that is the destination of the elements copied from the current <see cref="SortedDictionary{TKey, TValue}"/>.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            _set.CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="SortedDictionary{TKey, TValue}.Enumerator"/> for the
        /// <see cref="SortedDictionary{TKey, TValue}"/>.</returns>
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
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if key is not found in the <see cref="SortedDictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// If the <see cref="SortedDictionary{TKey, TValue}"/> does not contain an element with the specified key, the
        /// <see cref="SortedDictionary{TKey, TValue}"/> remains unchanged. No exception is thrown.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation.
        /// </remarks>
        public bool Remove(TKey key)
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}

            return _set.Remove(new KeyValuePair<TKey, TValue>(key, default));
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, /*[MaybeNullWhen(false)]*/ out TValue value)
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}

            TreeSet<KeyValuePair<TKey, TValue>>.Node node = _set.FindNode(new KeyValuePair<TKey, TValue>(key, default));
            if (node == null)
            {
                value = default;
                return false;
            }
            value = node.Item.Value;
            return true;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_set).CopyTo(array, index);
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get { return (ICollection)Keys; }
        }

        ICollection IDictionary.Values
        {
            get { return (ICollection)Values; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    TValue value;
                    if (TryGetValue((TKey)key, out value))
                    {
                        return value;
                    }
                }

                return null;
            }
            set
            {
                //if (key == null) // J2N: Making key nullable
                //{
                //    throw new ArgumentNullException(nameof(key));
                //}

                // J2N: Only throw if the generic closing type is not nullable
                if (key == null && !typeof(TKey).IsNullableType())
                    throw new ArgumentNullException(nameof(key));

                if (value == null && !typeof(TValue).IsNullableType())
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
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}

            // J2N: Only throw if the generic closing type is not nullable
            if (key == null && !typeof(TKey).IsNullableType())
                throw new ArgumentNullException(nameof(key));

            if (value == null && !typeof(TValue).IsNullableType())
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

        private static bool IsCompatibleKey(object key)
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}
            if (key == null && !(default(TKey) == null))
                return false;

            return (key is TKey);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.DictEntry);
        }

        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_set).SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        #endregion

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

        #region Nested Structure: Enumerator

        /// <summary>
        /// Enumerates the elements of a <see cref="SortedDictionary{TKey, TValue}"/>.
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
        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not an expected scenario")]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private TreeSet<KeyValuePair<TKey, TValue>>.Enumerator _treeEnum;
            private readonly int _getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;

            internal Enumerator(SortedDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                _treeEnum = dictionary._set.GetEnumerator();
                _getEnumeratorRetType = getEnumeratorRetType;
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
                return _treeEnum.MoveNext();
            }

            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            public void Dispose()
            {
                _treeEnum.Dispose();
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
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return _treeEnum.Current;
                }
            }

            internal bool NotStartedOrEnded
            {
                get
                {
                    return _treeEnum.NotStartedOrEnded;
                }
            }

            internal void Reset()
            {
                _treeEnum.Reset();
            }


            void IEnumerator.Reset()
            {
                _treeEnum.Reset();
            }

            object IEnumerator.Current
            {
                get
                {
                    if (NotStartedOrEnded)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    if (_getEnumeratorRetType == DictEntry)
                    {
                        return new DictionaryEntry(Current.Key, Current.Value);
                    }
                    else
                    {
                        return new KeyValuePair<TKey, TValue>(Current.Key, Current.Value);
                    }
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (NotStartedOrEnded)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return Current.Key;
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (NotStartedOrEnded)
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
                    if (NotStartedOrEnded)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return new DictionaryEntry(Current.Key, Current.Value);
                }
            }
        }

        #endregion

        #region Nested Class: KeyCollection

        /// <summary>
        /// Represents the collection of keys in a <see cref="SortedDictionary{TKey, TValue}"/>.
        /// This class cannot be inherited.
        /// </summary>
        /// <remarks>
        /// The <see cref="SortedDictionary{TKey, TValue}.Keys"/> property returns an instance
        /// of this type, containing all the keys in that <see cref="SortedDictionary{TKey, TValue}"/>.
        /// The order of the keys in the <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> is the same as the
        /// order of elements in the <see cref="SortedDictionary{TKey, TValue}"/>, the same as the order
        /// of the associated values in the <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> returned
        /// by the <see cref="SortedDictionary{TKey, TValue}.Values"/> property.
        /// <para/>
        /// The <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> is not a static copy; instead,
        /// the <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> refers back to the keys in the
        /// original <see cref="SortedDictionary{TKey, TValue}"/>. Therefore, changes to the
        /// <see cref="SortedDictionary{TKey, TValue}"/> continue to be reflected in the
        /// <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/>.
        /// </remarks>
        [DebuggerDisplay("Count = {Count}")]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
        public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
        {
            private readonly SortedDictionary<TKey, TValue> _dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection"/> class that reflects the keys
            /// in the specified <see cref="SortedDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="dictionary">The <see cref="SortedDictionary{TKey, TValue}"/> whose keys are reflected in the new
            /// <see cref="KeyCollection"/>.</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
            /// <remarks>
            /// The <see cref="KeyCollection"/> is not a static copy; instead, the
            /// <see cref="KeyCollection"/> refers back to the keys in the original <see cref="SortedDictionary{TKey, TValue}"/>.
            /// Therefore, changes to the <see cref="SortedDictionary{TKey, TValue}"/> continue to be reflected in
            /// the <see cref="KeyCollection"/>.
            /// <para/>
            /// This constructor is an O(1) operation.
            /// </remarks>
            public KeyCollection(SortedDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException(nameof(dictionary));
                }
                _dictionary = dictionary;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="KeyCollection"/>.
            /// </summary>
            /// <returns>A <see cref="Enumerator"/> structure for the <see cref="KeyCollection"/>.</returns>
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
            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            /// <summary>
            /// Copies the <see cref="KeyCollection"/> elements to an existing one-dimensional array,
            /// starting at the specified array index.
            /// </summary>
            /// <param name="array">The one-dimensional array that is the destination of the elements
            /// copied from the <see cref="KeyCollection"/>. The array must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
            /// <exception cref="ArgumentException">
            /// The number of elements in the source <see cref="KeyCollection"/> is greater than the available
            /// space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.
            /// </exception>
            /// <remarks>
            /// The elements are copied to the array in the same order in which the enumerator iterates through
            /// the <see cref="KeyCollection"/>.
            /// <para/>
            /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
            /// </remarks>
            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                }

                if (array.Length - index < Count)
                {
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
                }

                _dictionary._set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) { array[index++] = node.Item.Key; return true; });
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException(SR.Arg_NonZeroLowerBound, nameof(array));
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                }

                if (array.Length - index < _dictionary.Count)
                {
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
                }

                TKey[] keys = array as TKey[];
                if (keys != null)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    try
                    {
                        object[] objects = (object[])array;
                        _dictionary._set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) { objects[index++] = node.Item.Key; return true; });
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }
                }
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="KeyCollection"/>.
            /// </summary>
            /// <remarks>Getting the value of this property is an O(1) operation.</remarks>
            public int Count
            {
                get { return _dictionary.Count; }
            }

            bool ICollection<TKey>.IsReadOnly
            {
                get { return true; }
            }

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
                return _dictionary.ContainsKey(item);
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)_dictionary).SyncRoot; }
            }

            /// <summary>
            /// Enumerates the elements of a <see cref="KeyCollection"/>.
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
            [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not an expected scenario")]
            [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
            public struct Enumerator : IEnumerator<TKey>, IEnumerator
            {
                private SortedDictionary<TKey, TValue>.Enumerator _dictEnum;

                internal Enumerator(SortedDictionary<TKey, TValue> dictionary)
                {
                    _dictEnum = dictionary.GetEnumerator();
                }

                /// <summary>
                /// Releases all resources used by the <see cref="Enumerator"/>.
                /// </summary>
                public void Dispose()
                {
                    _dictEnum.Dispose();
                }

                /// <summary>
                /// Advances the enumerator to the next element of the <see cref="KeyCollection"/>.
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
                    return _dictEnum.MoveNext();
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
                public TKey Current => _dictEnum.Current.Key;

                object IEnumerator.Current
                {
                    get
                    {
                        if (_dictEnum.NotStartedOrEnded)
                        {
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                        }

                        return Current;
                    }
                }

                void IEnumerator.Reset()
                {
                    _dictEnum.Reset();
                }
            }
        }

        #endregion

        #region Nested Class: ValueCollection

        /// <summary>
        /// Represents the collection of values in a <see cref="SortedDictionary{TKey, TValue}"/>. This class cannot be inherited.
        /// </summary>
        /// <remarks>
        /// The <see cref="SortedDictionary{TKey, TValue}.Values"/> property returns an instance
        /// of this type, containing all the values in that <see cref="SortedDictionary{TKey, TValue}"/>.
        /// The order of the values in the <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> is the same as the
        /// order of elements in the <see cref="SortedDictionary{TKey, TValue}"/>, the same as the order
        /// of the associated values in the <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> returned
        /// by the <see cref="SortedDictionary{TKey, TValue}.Keys"/> property.
        /// <para/>
        /// The <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> is not a static copy; instead,
        /// the <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> refers back to the keys in the
        /// original <see cref="SortedDictionary{TKey, TValue}"/>. Therefore, changes to the
        /// <see cref="SortedDictionary{TKey, TValue}"/> continue to be reflected in the
        /// <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/>.
        /// </remarks>
        [DebuggerDisplay("Count = {Count}")]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
        public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
        {
            private readonly SortedDictionary<TKey, TValue> _dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueCollection"/> class that reflects the values in
            /// the specified <see cref="SortedDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="dictionary">The <see cref="SortedDictionary{TKey, TValue}"/> whose valeus are reflected
            /// in the new <see cref="ValueCollection"/>.</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
            /// <remarks>
            /// The <see cref="ValueCollection"/> is not a static copy; instead, the <see cref="ValueCollection"/>
            /// refers back to the values in the original <see cref="SortedDictionary{TKey, TValue}"/>. Therefore,
            /// changes to the <see cref="SortedDictionary{TKey, TValue}"/> continue to be reflected in the
            /// <see cref="ValueCollection"/>.
            /// </remarks>
            public ValueCollection(SortedDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException(nameof(dictionary));
                }
                _dictionary = dictionary;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="SortedDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <returns>A <see cref="Enumerator"/> structure for the <see cref="SortedDictionary{TKey, TValue}"/>.</returns>
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
            /// <para/>
            /// This method is an O(1) operation.
            /// </remarks>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_dictionary);
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
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                }

                if (array.Length - index < Count)
                {
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
                }

                _dictionary._set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) { array[index++] = node.Item.Value; return true; });
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException(SR.Arg_NonZeroLowerBound, nameof(array));
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                }

                if (array.Length - index < _dictionary.Count)
                {
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
                }

                TValue[] values = array as TValue[];
                if (values != null)
                {
                    CopyTo(values, index);
                }
                else
                {
                    try
                    {
                        object[] objects = (object[])array;
                        _dictionary._set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node) { objects[index++] = node.Item.Value; return true; });
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                    }
                }
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="ValueCollection"/>.
            /// </summary>
            /// <remarks>
            /// Retrieving the value of this property is an O(1) operation.
            /// </remarks>
            public int Count => _dictionary.Count;

            bool ICollection<TValue>.IsReadOnly => true;

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
                return _dictionary.ContainsValue(item);
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)_dictionary).SyncRoot; }
            }

            #region Nested Structure: Enumerator

            /// <summary>
            /// Enumerates the elements of a <see cref="ValueCollection"/>.
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
            [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not an expected scenario")]
            [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
            public struct Enumerator : IEnumerator<TValue>, IEnumerator
            {
                private SortedDictionary<TKey, TValue>.Enumerator _dictEnum;

                internal Enumerator(SortedDictionary<TKey, TValue> dictionary)
                {
                    _dictEnum = dictionary.GetEnumerator();
                }

                /// <summary>
                /// Releases all resources used by the <see cref="Enumerator"/>.
                /// </summary>
                public void Dispose()
                {
                    _dictEnum.Dispose();
                }

                /// <summary>
                /// Advances the enumerator to the next element of the <see cref="ValueCollection"/>.
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
                    return _dictEnum.MoveNext();
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
                public TValue Current => _dictEnum.Current.Value;

                object IEnumerator.Current
                {
                    get
                    {
                        if (_dictEnum.NotStartedOrEnded)
                        {
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                        }

                        return Current;
                    }
                }

                void IEnumerator.Reset()
                {
                    _dictEnum.Reset();
                }
            }

            #endregion
        }

        #endregion

        #region Nested Class: KeyValuePairComparer

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal sealed class KeyValuePairComparer : SCG.Comparer<KeyValuePair<TKey, TValue>>
        {
            internal IComparer<TKey> keyComparer; // Do not rename (binary serialization)

            public KeyValuePairComparer(IComparer<TKey> keyComparer)
            {
                if (keyComparer == null)
                {
                    this.keyComparer = Comparer<TKey>.Default;
                }
                else
                {
                    this.keyComparer = keyComparer;
                }
            }

            public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return keyComparer.Compare(x.Key, y.Key);
            }
        }

        #endregion
    }

    /// <summary>
    /// This class is intended as a helper for backwards compatibility with existing SortedDictionaries.
    /// TreeSet has been converted into SortedSet{T}, which will be exposed publicly. SortedDictionaries
    /// have the problem where they have already been serialized to disk as having a backing class named
    /// TreeSet. To ensure that we can read back anything that has already been written to disk, we need to
    /// make sure that we have a class named TreeSet that does everything the way it used to.
    ///
    /// The only thing that makes it different from SortedSet is that it throws on duplicates
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    internal sealed class TreeSet<T> : SortedSet<T>
    {
        public TreeSet()
            : base()
        { }

        public TreeSet(IComparer<T> comparer) : base(comparer) { }

#if FEATURE_SERIALIZABLE
        private TreeSet(SerializationInfo siInfo, StreamingContext context) : base(siInfo, context) { }
#endif

        internal override bool AddIfNotPresent(T item)
        {
            bool ret = base.AddIfNotPresent(item);
            if (!ret)
            {
                throw new ArgumentException(J2N.SR.Format(SR.Argument_AddingDuplicate, item));
            }
            return ret;
        }
    }
}
