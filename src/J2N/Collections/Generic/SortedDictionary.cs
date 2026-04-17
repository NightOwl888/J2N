// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.ObjectModel;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using SCG = System.Collections.Generic;

#if FEATURE_SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace J2N.Collections.Generic
{
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
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [SuppressMessage("Style", "IDE0019:Use pattern matching", Justification = "Using Microsoft's code styles")]
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, INavigableDictionary<TKey, TValue>, INavigableCollection<KeyValuePair<TKey, TValue>>, ICollectionView,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IStructuralEquatable, IStructuralFormattable
    {
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private KeyCollection? _keys;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private ValueCollection? _values;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private IComparer<TKey>? _reverseKeyComparer; // J2N: Cache reverse key comparer

        // J2N NOTE: In the BCL, this field was type TreeSet<KeyValuePair<TKey, TValue>>.
        // We have changed it to SortedSet<KeyValuePair<TKey, TValue>> to allow
        // views to function. Note that views are not serializable. The concrete type set here
        // is TreeSet<KeyValuePair<TKey, TValue>> for regular sets (which still round trip as TreeSet
        // during serialization), and for views it is SortedSet<KeyValuePair<TKey, TValue>.TreeSubSet
        // which does not support serialization by design (throws NotSupportedException). Any other types
        // are not currently set, so consideration must be given to how it will behave in terms of
        // serialization if another subclass of SortedSet<T> is allowed to be used here.

        private readonly SortedSet<KeyValuePair<TKey, TValue>> _set; // Do not rename (binary serialization)

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private object? _spanAdapterCache;


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
        public SortedDictionary() : this((IComparer<TKey>?)null)
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
        public SortedDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey>? comparer)
        {
            if (dictionary is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);

            var keyValuePairComparer = new KeyValuePairComparer(comparer);

            if (dictionary is SortedDictionary<TKey, TValue> sortedDictionary &&
                // J2N: Use RawComparer property to ensure we never get a ReverseComparer here
                sortedDictionary._set.RawComparer is KeyValuePairComparer kv &&
                // J2N: Use Comparer property to ensure we compare the *user* comparer for equality, not a wrapper
                kv.Comparer.Equals(keyValuePairComparer.Comparer))
            {
                _set = new TreeSet<KeyValuePair<TKey, TValue>>(sortedDictionary._set, keyValuePairComparer);
                return;
            }

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            // J2N: Added optimization for BCL SortedDictionary<TKey, TValue>
            if (dictionary is SCG.SortedDictionary<TKey, TValue> bclSortedDictionary)
            {
                _set = new TreeSet<KeyValuePair<TKey, TValue>>(new BclSortedDictionaryAdapter(bclSortedDictionary, keyValuePairComparer), keyValuePairComparer);
                return;
            }
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

            _set = new TreeSet<KeyValuePair<TKey, TValue>>(keyValuePairComparer);

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                // J2N: Throw exception here instead of TreeSet<T> so we can support TryAdd()
                if (!_set.Add(pair))
                {
                    ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException<TKey>(pair.Key);
                }
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
        public SortedDictionary(IComparer<TKey>? comparer)
        {
            _set = new TreeSet<KeyValuePair<TKey, TValue>>(new KeyValuePairComparer(comparer));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey, TValue}"/> class
        /// with the specified backing <paramref name="set"/>. This overload intended to be used
        /// to create views over a <see cref="SortedDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="set">The backing view set (an instance if <see cref="SortedSet{T}.TreeSubSet"/>).</param>
        /// <remarks>This constructor isn't intended to be used directly. Instead, it is exposed through
        /// <see cref="GetView(TKey, TKey)"/>.</remarks>
        internal SortedDictionary(SortedSet<KeyValuePair<TKey, TValue>> set)
        {
            _set = set;
        }

        #endregion

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="SortedDictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="SortedDictionary{TKey, TValue}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlyDictionary{TKey, TValue}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="SortedDictionary{TKey, TValue}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlyDictionary<TKey, TValue> AsReadOnly()
            => new ReadOnlyDictionary<TKey, TValue>(this);

        #endregion AsReadOnly

        #region SCG.SortedDictionary<TKey, TValue> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            // J2N: Throw exception here instead of TreeSet<T> so we can support TryAdd()
            if (!_set.Add(keyValuePair))
            {
                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException<TKey>(keyValuePair.Key);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            TreeSet<KeyValuePair<TKey, TValue>>.Node? node = _set.FindNode(keyValuePair);
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
            TreeSet<KeyValuePair<TKey, TValue>>.Node? node = _set.FindNode(keyValuePair);
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
        /// that does not exist in the <see cref="SortedDictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c>. However, if the specified key already exists in
        /// the <see cref="SortedDictionary{TKey, TValue}"/>, setting the <see cref="this[TKey]"/> property overwrites
        /// the old value. In contrast, the <see cref="Add(TKey, TValue)"/> method does not modify existing elements.
        /// <para/>
        /// Unlike the <see cref="System.Collections.Generic.SortedDictionary{TKey, TValue}"/>, both keys and values can
        /// be <c>null</c> if either <see cref="Nullable{T}"/> or a reference type.
        /// <para/>
        /// The C# language uses the <see cref="this"/> keyword to define the indexers instead of implementing the
        /// <c>Item[TKey]</c> property. Visual Basic implements <c>Item[TKey]</c> as a default property, which provides
        /// the same indexing functionality.
        /// <para/>
        /// Getting the value of this property is an O(log <c>n</c>) operation; setting the property is also
        /// an O(log <c>n</c>) operation.
        /// </remarks>
        public TValue this[[AllowNull]TKey key]
        {
            get
            {
                // J2N supports null keys

                TreeSet<KeyValuePair<TKey, TValue>>.Node? node = _set.FindNode(new KeyValuePair<TKey, TValue>(key, default!));
                if (node == null)
                {
                    if (key is null)
                    {
                        ThrowHelper.ThrowKeyNotFoundException("(null)");
                        return default;
                    }

                    ThrowHelper.ThrowKeyNotFoundException(key);
                    return default;
                }

                return node.Item.Value;
            }
            set
            {
                // J2N supports null keys

                TreeSet<KeyValuePair<TKey, TValue>>.Node? node = _set.FindNode(new KeyValuePair<TKey, TValue>(key, default!));
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
        {
            get
            {
                // Ensure we return the unwrapped comparer from the original set so we don't stack
                // reverse comparers. We use RawComparer here because it is slightly more efficient
                // than ComparerInternal and we know we will never be dealing with a string here
                // because it is always KeyValuePairComparer.
                var kv = (KeyValuePairComparer)_set.UnderlyingSet.RawComparer;
                // KeyValuePairComparer now will do the string comparer unwrapping for us.
                IComparer<TKey> cmp = kv.Comparer;

                if (_set.IsReversed)
                {
                    return _reverseKeyComparer ??= ReverseComparer<TKey>.Create(cmp);
                }

                return cmp;
            }
        }

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
        public ICollection<TKey> Keys => _keys ??= new KeyCollection(this);

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

#if FEATURE_IREADONLYCOLLECTIONS
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
#endif

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
        public ICollection<TValue> Values => _values ??= new ValueCollection(this);

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

#if FEATURE_IREADONLYCOLLECTIONS
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
#endif

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
        public void Add([AllowNull] TKey key, [AllowNull] TValue value)
        {
            // J2N supports null keys

            // J2N: Throw exception here instead of TreeSet<T> so we can support TryAdd()
            if (!_set.Add(new KeyValuePair<TKey, TValue>(key!, value!)))
            {
                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException<TKey>(key);
            }
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
        public bool ContainsKey([AllowNull] TKey key)
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}

            return _set.Contains(new KeyValuePair<TKey, TValue>(key!, default!));
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
            return ContainsValue(value, null);
        }

        /// <summary>
        /// Determines whether the <see cref="SortedDictionary{TKey, TValue}"/> contains a specific value
        /// as determined by the provided <paramref name="valueComparer"/>.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <param name="valueComparer">The <see cref="IEqualityComparer{TValue}"/> to use
        /// to test each value for equality.</param>
        /// <returns><c>true</c> if the <see cref="SortedDictionary{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method performs a linear search; therefore, the average execution time
        /// is proportional to <see cref="Count"/>. That is, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue([AllowNull] TValue value, IEqualityComparer<TValue>? valueComparer) // Overload added so end user can override J2N's equality comparer
        {
            bool found = false;
            if (value is null)
            {
                _set.InOrderTreeWalk(delegate (TreeSet<KeyValuePair<TKey, TValue>>.Node node)
                {
                    if (node.Item.Value is null)
                    {
                        found = true;
                        return false;  // stop the walk
                    }
                    return true;
                });
            }
            else
            {
                valueComparer ??= EqualityComparer<TValue>.Default;
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
        /// <exception cref="ArgumentException">The number of elements in the source array is greater
        /// than the available space from <paramref name="index"/> to the end of the destination array.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <remarks>This method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
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
        public Enumerator GetEnumerator() => new Enumerator(this, Enumerator.KeyValuePair);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<KeyValuePair<TKey, TValue>>() :
            GetEnumerator();

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
        public bool Remove([AllowNull] TKey key)
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}

            return _set.Remove(new KeyValuePair<TKey, TValue>(key!, default!));
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="SortedDictionary{TKey, TValue}"/>.
        /// If the element exists, the associated <paramref name="value"/> is output after it is removed.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if key is not found in the <see cref="SortedDictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// If the <see cref="SortedDictionary{TKey, TValue}"/> does not contain an element with the specified key, the
        /// <see cref="SortedDictionary{TKey, TValue}"/> remains unchanged. No exception is thrown.
        /// </remarks>
        // J2N: This is an extension method on IDictionary<TKey, TValue>, but only for .NET Standard 2.1+.
        // It is redefined here to ensure we have it in prior platforms.
        public bool Remove([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_set.DoRemove(new KeyValuePair<TKey, TValue>(key!, default!), out KeyValuePair<TKey, TValue> removed))
            {
                value = removed.Value;
                return true;
            }

            value = default!;
            return false;
        }

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
        public bool TryAdd([AllowNull] TKey key, [AllowNull] TValue value)
        {
            var kvp = new KeyValuePair<TKey, TValue>(key!, value!);
            if (_set is ICollectionView view && view.IsView && !_set.IsWithinRange(kvp))
                return false;

            return _set.Add(kvp);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns><c>true</c> if the <see cref="SortedDictionary{TKey, TValue}"/> contains an element with the
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
#pragma warning disable CS8767 // Nullability of reference types in type of parameter 'value' of 'bool Dictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' doesn't match implicitly implemented member 'bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' (possibly because of nullability attributes).
        public bool TryGetValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter 'value' of 'bool Dictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' doesn't match implicitly implemented member 'bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' (possibly because of nullability attributes).
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}

            TreeSet<KeyValuePair<TKey, TValue>>.Node? node = _set.FindNode(new KeyValuePair<TKey, TValue>(key!, default!));
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
                //if (key == null) // J2N: Making key nullable
                //{
                //    throw new ArgumentNullException(nameof(key));
                //}

                // J2N: Only throw if the generic closing type is not nullable
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TKey>(key, ExceptionArgument.key);
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);

                try
                {
                    TKey tempKey = (TKey)key!;
                    try
                    {
                        this[tempKey] = (TValue)value!;
                    }
                    catch (InvalidCastException)
                    {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    }
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                }
            }
        }

        void IDictionary.Add(object? key, object? value)
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}

            // J2N: Only throw if the generic closing type is not nullable
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TKey>(key, ExceptionArgument.key);
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);

            try
            {
                TKey tempKey = (TKey)key!;

                try
                {
                    Add(tempKey, (TValue)value!);
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                }
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
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

        private static bool IsCompatibleKey(object? key)
        {
            //if (key == null) // J2N: Making key nullable
            //{
            //    throw new ArgumentNullException(nameof(key));
            //}
            if (key is null)
                return default(TKey) == null;

            return (key is TKey);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this, Enumerator.DictEntry);

        void IDictionary.Remove(object? key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key!);
            }
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((ICollection)_set).SyncRoot;

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();

        #endregion

        #region Members for Alternate Lookup

        // This calls the correct layer to get the *outermost* comparer (a user comparer or a comparer wrapper around a BCL StringComparer)
        internal IComparer<TKey> RawComparer => ((KeyValuePairComparer)_set.RawComparer).keyComparer;

        // A simple one-instance cache to ensure that multiple SpanAlternateLookup calls with the same TAlternateKeySpan
        // don't incur runtime heap overhead. We need this because our comparer requires an adapter of type TAlternateKeySpan
        // which isn't known until the time of lookup, but our comparer is created during construction.
        private AlternateKeyValuePairComparer<TAlternateKeySpan> GetOrCreateAlternateComparer<TAlternateKeySpan>()
        {
            // Fast path (no fence)
            object? cache = _spanAdapterCache;
            if (cache is AlternateKeyValuePairComparer<TAlternateKeySpan> typed)
                return typed;

            // Create candidate
            var created = new AlternateKeyValuePairComparer<TAlternateKeySpan>((KeyValuePairComparer)_set.RawComparer);

            // Publish if still empty
            object? original = Interlocked.CompareExchange(ref _spanAdapterCache, created, null);

            // If we won the race, use ours; otherwise use existing
            return (original ?? created) as AlternateKeyValuePairComparer<TAlternateKeySpan> ?? created;
        }

        #endregion Members for Alternate Lookup

        #region SpanAlternateLookup

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="SortedDictionary{TKey, TValue}"/>
        /// using a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateKeySpan"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKeySpan">The alternate <see cref="ReadOnlySpan{T}"/> type of a key for performing lookups.</typeparam>
        /// <returns>The created lookup instance.</returns>
        /// <exception cref="InvalidOperationException">The dictionary's comparer is not compatible with <typeparamref name="TAlternateKeySpan"/>.</exception>
        /// <remarks>
        /// The dictionary must be using a comparer that implements <see cref="ISpanAlternateComparer{TAlternateKeySpan, TKey}"/> with
        /// a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateKeySpan"/> and <typeparamref name="TKey"/>.
        /// If it doesn't, an exception will be thrown.
        /// <para/>
        /// The following <see cref="StringComparer" /> options have built-in support when
        /// <typeparamref name="TAlternateKeySpan"/> is <see cref="char"/>.
        /// <list type="bullet">
        ///     <item><description><see cref="StringComparer.Ordinal"/></description></item>
        ///     <item><description><see cref="StringComparer.OrdinalIgnoreCase"/></description></item>
        ///     <item><description><see cref="StringComparer.InvariantCulture"/></description></item>
        ///     <item><description><see cref="StringComparer.InvariantCultureIgnoreCase"/></description></item>
        ///     <item><description><see cref="StringComparer.CurrentCulture"/></description></item>
        ///     <item><description><see cref="StringComparer.CurrentCultureIgnoreCase"/></description></item>
        /// </list>
        /// </remarks>
        public SpanAlternateLookup<TAlternateKeySpan> GetSpanAlternateLookup<TAlternateKeySpan>()
        {
            if (!SpanAlternateLookup<TAlternateKeySpan>.IsCompatibleKey(this))
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IncompatibleComparer);
            }

            return new SpanAlternateLookup<TAlternateKeySpan>(this);
        }

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="SortedDictionary{TKey, TValue}"/>
        /// using a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateKeySpan"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKeySpan">The alternate <see cref="ReadOnlySpan{T}"/> type of a key for performing lookups.</typeparam>
        /// <param name="lookup">The created lookup instance when the method returns <see langword="true"/>, or a default instance
        /// that should not be used if the method returns <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if a lookup could be created; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The dictionary must be using a comparer that implements <see cref="ISpanAlternateComparer{TAlternateKeySpan, TKey}"/> with
        /// a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateKeySpan"/> and <typeparamref name="TKey"/>.
        /// If it doesn't, the method will return <see langword="false"/>.
        /// <para/>
        /// The following <see cref="StringComparer" /> options have built-in support when
        /// <typeparamref name="TAlternateKeySpan"/> is <see cref="char"/>.
        /// <list type="bullet">
        ///     <item><description><see cref="StringComparer.Ordinal"/></description></item>
        ///     <item><description><see cref="StringComparer.OrdinalIgnoreCase"/></description></item>
        ///     <item><description><see cref="StringComparer.InvariantCulture"/></description></item>
        ///     <item><description><see cref="StringComparer.InvariantCultureIgnoreCase"/></description></item>
        ///     <item><description><see cref="StringComparer.CurrentCulture"/></description></item>
        ///     <item><description><see cref="StringComparer.CurrentCultureIgnoreCase"/></description></item>
        /// </list>
        /// </remarks>
        public bool TryGetSpanAlternateLookup<TAlternateKeySpan>(out SpanAlternateLookup<TAlternateKeySpan> lookup)
        {
            if (SpanAlternateLookup<TAlternateKeySpan>.IsCompatibleKey(this))
            {
                lookup = new SpanAlternateLookup<TAlternateKeySpan>(this);
                return true;
            }

            lookup = default;
            return false;
        }

        /// <summary>
        /// Provides a type that may be used to perform operations on a <see cref="SortedDictionary{TKey, TValue}"/>
        /// using a <see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TAlternateKeySpan"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKeySpan">The alternate <see cref="ReadOnlySpan{T}"/> type of a key for performing lookups.</typeparam>
        public readonly struct SpanAlternateLookup<TAlternateKeySpan>
        {
            private readonly SortedSet<KeyValuePair<TKey, TValue>> _set;
            private readonly SortedSet<KeyValuePair<TKey, TValue>>.SpanAlternateLookup<TAlternateKeySpan> _setLookup;
            private readonly AlternateKeyValuePairComparer<TAlternateKeySpan> _alternateComparer;

            /// <summary>Initialize the instance. The dictionary must have already been verified to have a compatible comparer.</summary>
            internal SpanAlternateLookup(SortedDictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(dictionary is not null);
                Debug.Assert(IsCompatibleKey(dictionary!)); // [!]: asserted above
                Dictionary = dictionary!; // [!]: asserted above
                _set = dictionary!._set; // [!]: asserted above
                _alternateComparer = dictionary!.GetOrCreateAlternateComparer<TAlternateKeySpan>(); // [!]: asserted above
                _setLookup = _set.GetSpanAlternateLookup(_alternateComparer);
            }

            /// <summary>Gets the <see cref="SortedDictionary{TKey, TValue}"/> against which this instance performs operations.</summary>
            public SortedDictionary<TKey, TValue> Dictionary { get; }

            /// <summary>Gets or sets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get or set.</param>
            /// <value>
            /// The value associated with the specified alternate key. If the specified alternate key is not found, a get operation throws
            /// a <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the specified key.
            /// </value>
            /// <exception cref="KeyNotFoundException">The property is retrieved and alternate key does not exist in the collection.</exception>
            public TValue this[ReadOnlySpan<TAlternateKeySpan> key]
            {
                get
                {
                    if (!_setLookup.TryGetValue(key, out KeyValuePair<TKey, TValue> pair))
                    {
                        ThrowHelper.ThrowKeyNotFoundException(GetAlternateComparer(Dictionary).Create(key));
                    }

                    return pair.Value;
                }
                set
                {
                    TreeSet<KeyValuePair<TKey, TValue>>.Node? node = _setLookup.FindNode(key);
                    if (node is null)
                    {
                        _set.Add(new KeyValuePair<TKey, TValue>(GetAlternateComparer(Dictionary).Create(key), value));
                    }
                    else
                    {
                        node.Item = new KeyValuePair<TKey, TValue>(node.Item.Key, value);
                        _set.UpdateVersion();
                    }
                }
            }

            /// <summary>Checks whether the dictionary has a comparer compatible with <typeparamref name="TAlternateKeySpan"/>.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static bool IsCompatibleKey(SortedDictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(dictionary is not null);
                return dictionary!.RawComparer is ISpanAlternateComparer<TAlternateKeySpan, TKey>; // [!]: asserted above
            }

            /// <summary>Gets the dictionary's alternate comparer. The dictionary must have already been verified as compatible.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static ISpanAlternateComparer<TAlternateKeySpan, TKey> GetAlternateComparer(SortedDictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(IsCompatibleKey(dictionary));
                return Unsafe.As<ISpanAlternateComparer<TAlternateKeySpan, TKey>>(dictionary.RawComparer)!;
            }

            /// <summary>Gets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get.</param>
            /// <param name="value">
            /// When this method returns, contains the value associated with the specified key, if the key is found;
            /// otherwise, the default value for the type of the value parameter.
            /// </param>
            /// <returns><see langword="true"/> if an entry was found; otherwise, <see langword="false"/>.</returns>
            public bool TryGetValue(ReadOnlySpan<TAlternateKeySpan> key, [MaybeNullWhen(false)] out TValue value)
            {
                if (_setLookup.TryGetValue(key, out KeyValuePair<TKey, TValue> pair))
                {
                    value = pair.Value;
                    return true;
                }

                value = default;
                return false;
            }

            /// <summary>Gets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get.</param>
            /// <param name="actualKey">
            /// When this method returns, contains the actual key associated with the alternate key, if the key is found;
            /// otherwise, the default value for the type of the key parameter.
            /// </param>
            /// <param name="value">
            /// When this method returns, contains the value associated with the specified key, if the key is found;
            /// otherwise, the default value for the type of the value parameter.
            /// </param>
            /// <returns><see langword="true"/> if an entry was found; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool TryGetValue(ReadOnlySpan<TAlternateKeySpan> key, [MaybeNullWhen(false)] out TKey actualKey, [MaybeNullWhen(false)] out TValue value)
            {
                if (_setLookup.TryGetValue(key, out KeyValuePair<TKey, TValue> pair))
                {
                    actualKey = pair.Key;
                    value = pair.Value;
                    return true;
                }

                actualKey = default;
                value = default;
                return false;
            }

            /// <summary>Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains the specified alternate key.</summary>
            /// <param name="key">The alternate key to check.</param>
            /// <returns><see langword="true"/> if the key is in the dictionary; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool ContainsKey(ReadOnlySpan<TAlternateKeySpan> key) =>
                _setLookup.Contains(key);

            /// <summary>Removes the value with the specified alternate key from the <see cref="Dictionary{TKey, TValue}"/>.</summary>
            /// <param name="key">The alternate key of the element to remove.</param>
            /// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool Remove(ReadOnlySpan<TAlternateKeySpan> key) =>
                _setLookup.Remove(key);

            /// <summary>
            /// Removes the value with the specified alternate key from the <see cref="Dictionary{TKey, TValue}"/>,
            /// and copies the element to the value parameter.
            /// </summary>
            /// <param name="key">The alternate key of the element to remove.</param>
            /// <param name="actualKey">The removed key.</param>
            /// <param name="value">The removed element.</param>
            /// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool Remove(ReadOnlySpan<TAlternateKeySpan> key, [MaybeNullWhen(false)] out TKey actualKey, [MaybeNullWhen(false)] out TValue value)
            {
                if (_setLookup.Remove(key, out KeyValuePair<TKey, TValue> removed))
                {
                    actualKey = removed.Key;
                    value = removed.Value;
                    return true;
                }

                actualKey = default;
                value = default;
                return false;
            }

            /// <summary>Attempts to add the specified key and value to the dictionary.</summary>
            /// <param name="key">The alternate key of the element to add.</param>
            /// <param name="value">The value of the element to add.</param>
            /// <returns><see langword="true"/> if the key/value pair was added to the dictionary successfully; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool TryAdd(ReadOnlySpan<TAlternateKeySpan> key, TValue value) =>
                _setLookup.TryAdd(key, value, _alternateComparer);

            /// <summary>
            /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
            /// is the predecessor of the specified <paramref name="key"/>.
            /// </summary>
            /// <param name="key">The key of the entry to get the predecessor of.</param>
            /// <param name="resultKey">Upon successful return, contains the key of the predecessor.</param>
            /// <param name="resultValue">Upon successful return, contains the value of the predecessor.</param>
            /// <returns><see langword="true"/> if a predecessor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
            /// <remarks>
            /// This method is a O(log <c>n</c>) operation.
            /// <para/>
            /// This is referred to as <c>strict predecessor</c> in order theory.
            /// <para/>
            /// Usage Note: This corresponds to the <c>lowerEntry()</c> method in the JDK.
            /// </remarks>
            public bool TryGetPredecessor(ReadOnlySpan<TAlternateKeySpan> key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
            {
                if (_setLookup.TryGetPredecessor(key, out KeyValuePair<TKey, TValue> result))
                {
                    resultKey = result.Key;
                    resultValue = result.Value;
                    return true;
                }
                resultKey = default;
                resultValue = default;
                return false;
            }

            /// <summary>
            /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
            /// is the successor of the specified <paramref name="key"/>.
            /// </summary>
            /// <param name="key">The key of the entry to get the successor of.</param>
            /// <param name="resultKey">Upon successful return, contains the key of the successor.</param>
            /// <param name="resultValue">Upon successful return, contains the value of the successor.</param>
            /// <returns><see langword="true"/> if a successor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
            /// <remarks>
            /// This method is a O(log <c>n</c>) operation.
            /// <para/>
            /// This is referred to as <c>strict successor</c> in order theory.
            /// <para/>
            /// Usage Note: This corresponds to the <c>higherEntry()</c> method in the JDK.
            /// </remarks>
            public bool TryGetSuccessor(ReadOnlySpan<TAlternateKeySpan> key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
            {
                if (_setLookup.TryGetSuccessor(key, out KeyValuePair<TKey, TValue> result))
                {
                    resultKey = result.Key;
                    resultValue = result.Value;
                    return true;
                }
                resultKey = default;
                resultValue = default;
                return false;
            }

            /// <summary>
            /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
            /// is the greatest element less than or equal to the specified <paramref name="key"/>.
            /// </summary>
            /// <param name="key">The key of the entry to get the floor of.</param>
            /// <param name="resultKey">Upon successful return, contains the key of the floor.</param>
            /// <param name="resultValue">Upon successful return, contains the value of the floor.</param>
            /// <returns><see langword="true"/> if a floor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
            /// <remarks>
            /// This method is a O(log <c>n</c>) operation.
            /// <para/>
            /// This is referred to as <c>weak predecessor</c> in order theory.
            /// <para/>
            /// Usage Note: This corresponds to the <c>floorEntry()</c> method in the JDK.
            /// </remarks>
            public bool TryGetFloor(ReadOnlySpan<TAlternateKeySpan> key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
            {
                if (_setLookup.TryGetFloor(key, out KeyValuePair<TKey, TValue> result))
                {
                    resultKey = result.Key;
                    resultValue = result.Value;
                    return true;
                }
                resultKey = default;
                resultValue = default;
                return false;
            }

            /// <summary>
            /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
            /// is the least element greater than or equal to the specified <paramref name="key"/>.
            /// </summary>
            /// <param name="key">The key of the entry to get the ceiling of.</param>
            /// <param name="resultKey">Upon successful return, contains the key of the ceiling.</param>
            /// <param name="resultValue">Upon successful return, contains the value of the ceiling.</param>
            /// <returns><see langword="true"/> if a ceiling to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
            /// <remarks>
            /// This method is a O(log <c>n</c>) operation.
            /// <para/>
            /// This is referred to as <b>weak successor</b> in order theory.
            /// <para/>
            /// Usage Note: This corresponds to the <c>ceilingEntry()</c> method in the JDK.
            /// </remarks>
            public bool TryGetCeiling(ReadOnlySpan<TAlternateKeySpan> key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
            {
                if (_setLookup.TryGetCeiling(key, out KeyValuePair<TKey, TValue> result))
                {
                    resultKey = result.Key;
                    resultValue = result.Value;
                    return true;
                }
                resultKey = default;
                resultValue = default;
                return false;
            }

            #region GetView

            /// <summary>
            /// Returns a view of a sub dictionary in a <see cref="SortedDictionary{TKey, TValue}"/>.
            /// <para/>
            /// Usage Note: In Java, the upper bound of TreeMap.subMap() is exclusive. To match the behavior, call
            /// <see cref="GetView(ReadOnlySpan{TAlternateKeySpan}, bool, ReadOnlySpan{TAlternateKeySpan}, bool)"/>,
            /// setting <c>fromInclusive</c> to <see langword="true"/> and <c>toInclusive</c> to <see langword="false"/>.
            /// </summary>
            /// <param name="fromKey">The first desired key in the view (lowest in ascending order, highest in descending order).</param>
            /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
            /// <returns>A sub dictionary view that contains only the values in the specified range.</returns>
            /// <exception cref="ArgumentException"><paramref name="fromKey"/> is after <paramref name="toKey"/>
            /// in the current view order according to the comparer.</exception>
            /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
            /// specified by <paramref name="fromKey"/> and <paramref name="toKey"/>.</exception>
            /// <remarks>
            /// This method returns a view of the range of elements that fall between <paramref name="fromKey"/> and
            /// <paramref name="toKey"/> (inclusive), as defined by the current view order and the comparer.
            /// This method does not copy elements from the <see cref="SortedDictionary{TKey, TValue}"/>, but provides a
            /// window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
            /// <para/>
            /// This corresponds to the <c>subMap()</c> method in the JDK.
            /// </remarks>
            public SortedDictionary<TKey, TValue> GetView(ReadOnlySpan<TAlternateKeySpan> fromKey, ReadOnlySpan<TAlternateKeySpan> toKey)
            {
                SortedSet<KeyValuePair<TKey, TValue>> viewSet = _setLookup.DoGetView(fromKey, fromInclusive: true, ExceptionArgument.fromKey, toKey, toInclusive: true, ExceptionArgument.toKey);
                return new SortedDictionary<TKey, TValue>(viewSet);
            }

            /// <summary>
            /// Returns a view of a sub dictionary in a <see cref="SortedDictionary{TKey, TValue}"/>.
            /// <para/>
            /// Usage Note: To match the behavior of the JDK, call this overload with <paramref name="fromInclusive"/>
            /// set to <see langword="true"/> and <paramref name="toInclusive"/> set to <see langword="false"/>.
            /// </summary>
            /// <param name="fromKey">The first key in the range for the view (lowest in ascending order, highest in descending order).</param>
            /// <param name="fromInclusive">If <see langword="true"/>, <paramref name="fromKey"/> will be included in the range;
            /// otherwise, it is an exclusive lower bound.</param>
            /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
            /// <param name="toInclusive">If <see langword="true"/>, <paramref name="toKey"/> will be included in the range;
            /// otherwise, it is an exclusive upper bound.</param>
            /// <returns>A sub dictionary view that contains only the values in the specified range.</returns>
            /// <exception cref="ArgumentException"><paramref name="fromKey"/> is after <paramref name="toKey"/>
            /// in the current view order according to the comparer.</exception>
            /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
            /// specified by <paramref name="fromKey"/> and <paramref name="toKey"/>.</exception>
            /// <remarks>
            /// This method returns a view of the range of elements that fall between <paramref name="fromKey"/> and
            /// <paramref name="toKey"/>, as defined by the current view order and comparer. Each bound may either be inclusive
            /// (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the values of <paramref name="fromInclusive"/>
            /// and <paramref name="toInclusive"/>. This method does not copy elements from the
            /// <see cref="SortedDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
            /// <para/>
            /// This corresponds to the <c>subMap()</c> method in the JDK.
            /// </remarks>
            public SortedDictionary<TKey, TValue> GetView(ReadOnlySpan<TAlternateKeySpan> fromKey, bool fromInclusive, ReadOnlySpan<TAlternateKeySpan> toKey, bool toInclusive)
            {
                SortedSet<KeyValuePair<TKey, TValue>> viewSet = _setLookup.DoGetView(fromKey, fromInclusive, ExceptionArgument.fromKey, toKey, toInclusive, ExceptionArgument.toKey);
                return new SortedDictionary<TKey, TValue>(viewSet);
            }

            #endregion GetView

            #region GetViewBefore

            /// <summary>
            /// Returns the view of a subset in a <see cref="SortedDictionary{TKey, TValue}"/> with no lower bound.
            /// <para/>
            /// Usage Note: To match the default behavior of the JDK, call the <see cref="GetViewBefore(ReadOnlySpan{TAlternateKeySpan}, bool)"/>
            /// overload with <c>inclusive</c> set to <see langword="false"/>.
            /// </summary>
            /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
            /// <returns>A subset view that contains only the values in the specified range.</returns>
            /// <remarks>
            /// This method returns a view of the range of elements that fall before <paramref name="toKey"/>
            /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
            /// <see cref="SortedDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
            /// <para/>
            /// This corresponds to the <c>headMap()</c> method in the JDK.
            /// </remarks>
            public SortedDictionary<TKey, TValue> GetViewBefore(ReadOnlySpan<TAlternateKeySpan> toKey)
            {
                SortedSet<KeyValuePair<TKey, TValue>> viewSet = _setLookup.DoGetViewBefore(toKey, inclusive: true, ExceptionArgument.toKey);
                return new SortedDictionary<TKey, TValue>(viewSet);
            }

            /// <summary>
            /// Returns the view of a subset in a <see cref="SortedDictionary{TKey, TValue}"/> with no lower bound.
            /// <para/>
            /// Usage Note: To match the default behavior of the JDK, call this overload with <paramref name="inclusive"/>
            /// set to <see langword="false"/>.
            /// </summary>
            /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
            /// <param name="inclusive">If <see langword="true"/>, <paramref name="toKey"/> will be included in the range;
            /// otherwise, it is an exclusive upper bound.</param>
            /// <returns>
            /// This method returns a view of the range of elements that fall before <paramref name="toKey"/>, as defined
            /// by the current view order and comparer. The upper bound may either be inclusive (<see langword="true"/>)
            /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>. This method
            /// does not copy elements from the <see cref="SortedDictionary{TKey, TValue}"/>, but provides a window into
            /// the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
            /// <para/>
            /// This corresponds to the <c>headMap()</c> method in the JDK.
            /// </returns>
            public SortedDictionary<TKey, TValue> GetViewBefore(ReadOnlySpan<TAlternateKeySpan> toKey, bool inclusive)
            {
                SortedSet<KeyValuePair<TKey, TValue>> viewSet = _setLookup.DoGetViewBefore(toKey, inclusive, ExceptionArgument.toKey);
                return new SortedDictionary<TKey, TValue>(viewSet);
            }

            #endregion GetViewBefore

            #region GetViewAfter

            /// <summary>
            /// Returns a view of a subset in a <see cref="SortedDictionary{TKey, TValue}"/> with no upper bound.
            /// </summary>
            /// <param name="fromKey">The first key in the range for the view (lowest in ascending order, highest in descending order).</param>
            /// <returns>A subset view that contains only the values in the specified range.</returns>
            /// <remarks>
            /// This method returns a view of the range of elements that fall after <paramref name="fromKey"/>
            /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
            /// <see cref="SortedDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
            /// <para/>
            /// This corresponds to the <c>tailMap()</c> method in the JDK.
            /// </remarks>
            public SortedDictionary<TKey, TValue> GetViewAfter(ReadOnlySpan<TAlternateKeySpan> fromKey)
            {
                SortedSet<KeyValuePair<TKey, TValue>> viewSet = _setLookup.DoGetViewAfter(fromKey, inclusive: true, ExceptionArgument.fromKey);
                return new SortedDictionary<TKey, TValue>(viewSet);
            }

            /// <summary>
            /// Returns a view of a subset in a <see cref="SortedDictionary{TKey, TValue}"/> with no upper bound.
            /// </summary>
            /// <param name="fromKey">The first key in the range for the view (lowest in ascending order, highest in descending order).</param>
            /// <param name="inclusive">If <see langword="true"/>, <paramref name="fromKey"/> will be included in the range;
            /// otherwise, it is an exclusive lower bound.</param>
            /// <returns>A subset view that contains only the values in the specified range.</returns>
            /// <remarks>
            /// This method returns a view of the range of elements that fall after <paramref name="fromKey"/>, as defined
            /// by the current view order and comparer. The lower bound may either be inclusive (<see langword="true"/>)
            /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>. This method
            /// does not copy elements from the <see cref="SortedDictionary{TKey, TValue}"/>, but provides a window into
            /// the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
            /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
            /// <para/>
            /// This corresponds to the <c>tailMap()</c> method in the JDK.
            /// </remarks>
            public SortedDictionary<TKey, TValue> GetViewAfter(ReadOnlySpan<TAlternateKeySpan> fromKey, bool inclusive)
            {
                SortedSet<KeyValuePair<TKey, TValue>> viewSet = _setLookup.DoGetViewAfter(fromKey, inclusive, ExceptionArgument.fromKey);
                return new SortedDictionary<TKey, TValue>(viewSet);
            }

            #endregion GetViewAfter
        }

        #endregion SpanAlternateLookup

        #region INavigableCollection<KeyValuePair<TKey, TValue>> members

        IComparer<KeyValuePair<TKey, TValue>> ISortedCollection<KeyValuePair<TKey, TValue>>.Comparer => _set.Comparer;

        bool INavigableCollection<KeyValuePair<TKey, TValue>>.TryGetFirst(out KeyValuePair<TKey, TValue> result) => _set.TryGetFirst(out result);

        bool INavigableCollection<KeyValuePair<TKey, TValue>>.TryGetLast(out KeyValuePair<TKey, TValue> result) => _set.TryGetLast(out result);

        bool INavigableCollection<KeyValuePair<TKey, TValue>>.RemoveFirst(out KeyValuePair<TKey, TValue> value) => _set.RemoveFirst(out value);

        bool INavigableCollection<KeyValuePair<TKey, TValue>>.RemoveLast(out KeyValuePair<TKey, TValue> value) => _set.RemoveLast(out value);

        INavigableCollection<KeyValuePair<TKey, TValue>> INavigableCollection<KeyValuePair<TKey, TValue>>.GetView(KeyValuePair<TKey, TValue> fromItem, KeyValuePair<TKey, TValue> toItem)
            => _set.GetView(fromItem, toItem);

        INavigableCollection<KeyValuePair<TKey, TValue>> INavigableCollection<KeyValuePair<TKey, TValue>>.GetView(KeyValuePair<TKey, TValue> fromItem, bool fromInclusive, KeyValuePair<TKey, TValue> toItem, bool toInclusive)
            => _set.GetView(fromItem, fromInclusive, toItem, toInclusive);

        INavigableCollection<KeyValuePair<TKey, TValue>> INavigableCollection<KeyValuePair<TKey, TValue>>.GetViewBefore(KeyValuePair<TKey, TValue> toItem)
            => _set.GetViewBefore(toItem);

        INavigableCollection<KeyValuePair<TKey, TValue>> INavigableCollection<KeyValuePair<TKey, TValue>>.GetViewBefore(KeyValuePair<TKey, TValue> toItem, bool inclusive)
            => _set.GetViewBefore(toItem, inclusive);

        INavigableCollection<KeyValuePair<TKey, TValue>> INavigableCollection<KeyValuePair<TKey, TValue>>.GetViewAfter(KeyValuePair<TKey, TValue> fromItem)
            => _set.GetViewAfter(fromItem);

        INavigableCollection<KeyValuePair<TKey, TValue>> INavigableCollection<KeyValuePair<TKey, TValue>>.GetViewAfter(KeyValuePair<TKey, TValue> fromItem, bool inclusive)
            => _set.GetViewAfter(fromItem, inclusive);

        INavigableCollection<KeyValuePair<TKey, TValue>> INavigableCollection<KeyValuePair<TKey, TValue>>.GetViewDescending()
            => _set.GetViewDescending();

        /// <summary>
        /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
        /// is the predecessor of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the predecessor of.</param>
        /// <param name="result">The <see cref="KeyValuePair{TKey, TValue}"/> representing the predecessor, if any.</param>
        /// <returns><see langword="true"/> if a predecessor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        [Obsolete("Use TryGetPredecessor(TKey, out TKey, out TValue) or INavigableCollection<KeyValuePair<TKey, TValue>>.TryGetPredecessor(TKey, out KeyValuePair<TKey, TValue>) instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool TryGetPredecessor(TKey key, out KeyValuePair<TKey, TValue> result)
        {
            return _set.TryGetPredecessor(new KeyValuePair<TKey, TValue>(key, default!), out result);
        }

        bool INavigableCollection<KeyValuePair<TKey, TValue>>.TryGetPredecessor(KeyValuePair<TKey, TValue> item, out KeyValuePair<TKey, TValue> result)
            => _set.TryGetPredecessor(item, out result);

        /// <summary>
        /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
        /// is the successor of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the successor of.</param>
        /// <param name="result">The <see cref="KeyValuePair{TKey, TValue}"/> representing the successor, if any.</param>
        /// <returns><see langword="true"/> if a successor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        [Obsolete("Use TryGetSuccessor(TKey, out TKey, out TValue) or INavigableCollection<KeyValuePair<TKey, TValue>>.TryGetSuccessor(TKey, out KeyValuePair<TKey, TValue>) instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool TryGetSuccessor(TKey key, out KeyValuePair<TKey, TValue> result)
        {
            return _set.TryGetSuccessor(new KeyValuePair<TKey, TValue>(key, default!), out result);
        }

        bool INavigableCollection<KeyValuePair<TKey, TValue>>.TryGetSuccessor(KeyValuePair<TKey, TValue> item, out KeyValuePair<TKey, TValue> result)
            => _set.TryGetSuccessor(item, out result);

        bool INavigableCollection<KeyValuePair<TKey, TValue>>.TryGetFloor(KeyValuePair<TKey, TValue> item, out KeyValuePair<TKey, TValue> result)
            => _set.TryGetFloor(item, out result);

        bool INavigableCollection<KeyValuePair<TKey, TValue>>.TryGetCeiling(KeyValuePair<TKey, TValue> item, out KeyValuePair<TKey, TValue> result)
            => _set.TryGetCeiling(item, out result);

        IEnumerable<KeyValuePair<TKey, TValue>> INavigableCollection<KeyValuePair<TKey, TValue>>.Reverse()
            => _set.Reverse();


        #endregion INavigableCollection<KeyValuePair<TKey, TValue>> members

        #region INavigableDictionary<TKey, TValue> members

        IComparer<TKey> INavigableDictionary<TKey, TValue>.Comparer => Comparer;

        INavigableCollection <TKey> INavigableDictionary<TKey, TValue>.Keys => (INavigableCollection<TKey>)Keys;

        /// <summary>
        /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
        /// is the first (lowest) value, as defined by the comparer.
        /// </summary>
        /// <param name="key">Upon successful return, contains the first (lowest) key in the collection.</param>
        /// <param name="value">Upon successful return, contains the value corresponding to the first (lowest) key in the collection.</param>
        /// <returns><see langword="true"/> if a first <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Usage Note: This corresponds to both the <c>firstKey()</c> and <c>firstEntry()</c> methods in the JDK.
        /// </remarks>
        public bool TryGetFirst([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_set.TryGetFirst(out KeyValuePair<TKey, TValue> result))
            {
                key = result.Key;
                value = result.Value;
                return true;
            }
            key = default;
            value = default;
            return false;
        }

        bool INavigableDictionary<TKey, TValue>.TryGetFirst([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
            => TryGetFirst(out key, out value);

        /// <summary>
        /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
        /// is the last (highest) value, as defined by the comparer.
        /// </summary>
        /// <param name="key">Upon successful return, contains the last (highest) key in the collection.</param>
        /// <param name="value">Upon successful return, contains the value corresponding to the last (highest) key in the collection.</param>
        /// <returns><see langword="true"/> if a last <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Usage Note: This corresponds to both the <c>lastKey()</c> and <c>lastEntry()</c> methods in the JDK.
        /// </remarks>
        public bool TryGetLast([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_set.TryGetLast(out KeyValuePair<TKey, TValue> result))
            {
                key = result.Key;
                value = result.Value;
                return true;
            }
            key = default;
            value = default;
            return false;
        }

        bool INavigableDictionary<TKey, TValue>.TryGetLast([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
            => TryGetLast(out key, out value);

        /// <summary>
        /// Removes the first (lowest) element in the <see cref="SortedDictionary{TKey, TValue}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="key">The key of the element before it is removed.</param>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><see langword="true"/>  if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>pollFirstEntry()</c> method in the JDK.
        /// </remarks>
        public bool RemoveFirst([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_set.RemoveFirst(out KeyValuePair<TKey, TValue> result))
            {
                key = result.Key;
                value = result.Value;
                return true;
            }
            key = default;
            value = default;
            return false;
        }

        bool INavigableDictionary<TKey, TValue>.RemoveFirst([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
            => RemoveFirst(out key, out value);

        /// <summary>
        /// Removes the last (highest) element in the <see cref="SortedDictionary{TKey, TValue}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="key">The key of the element before it is removed.</param>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><see langword="true"/>  if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>pollLastEntry()</c> method in the JDK.
        /// </remarks>
        public bool RemoveLast([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_set.RemoveLast(out KeyValuePair<TKey, TValue> result))
            {
                key = result.Key;
                value = result.Value;
                return true;
            }
            key = default;
            value = default;
            return false;
        }

        bool INavigableDictionary<TKey, TValue>.RemoveLast([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value)
            => RemoveLast(out key, out value);

        /// <summary>
        /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
        /// is the predecessor of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the predecessor of.</param>
        /// <param name="resultKey">Upon successful return, contains the key of the predecessor.</param>
        /// <param name="resultValue">Upon successful return, contains the value of the predecessor.</param>
        /// <returns><see langword="true"/> if a predecessor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>strict predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>lowerEntry()</c> method in the JDK.
        /// </remarks>
        public bool TryGetPredecessor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
        {
            if (_set.TryGetPredecessor(new KeyValuePair<TKey, TValue>(key!, default!), out KeyValuePair<TKey, TValue> result))
            {
                resultKey = result.Key;
                resultValue = result.Value;
                return true;
            }
            resultKey = default;
            resultValue = default;
            return false;
        }

        bool INavigableDictionary<TKey, TValue>.TryGetPredecessor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
            => TryGetPredecessor(key, out resultKey, out resultValue);

        /// <summary>
        /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
        /// is the successor of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the successor of.</param>
        /// <param name="resultKey">Upon successful return, contains the key of the successor.</param>
        /// <param name="resultValue">Upon successful return, contains the value of the successor.</param>
        /// <returns><see langword="true"/> if a successor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>strict successor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>higherEntry()</c> method in the JDK.
        /// </remarks>
        public bool TryGetSuccessor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
        {
            if (_set.TryGetSuccessor(new KeyValuePair<TKey, TValue>(key!, default!), out KeyValuePair<TKey, TValue> result))
            {
                resultKey = result.Key;
                resultValue = result.Value;
                return true;
            }
            resultKey = default;
            resultValue = default;
            return false;
        }

        bool INavigableDictionary<TKey, TValue>.TryGetSuccessor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
            => TryGetSuccessor(key, out resultKey, out resultValue);

        /// <summary>
        /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
        /// is the greatest element less than or equal to the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the floor of.</param>
        /// <param name="resultKey">Upon successful return, contains the key of the floor.</param>
        /// <param name="resultValue">Upon successful return, contains the value of the floor.</param>
        /// <returns><see langword="true"/> if a floor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>weak predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>floorEntry()</c> method in the JDK.
        /// </remarks>
        public bool TryGetFloor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
        {
            if (_set.TryGetFloor(new KeyValuePair<TKey, TValue>(key!, default!), out KeyValuePair<TKey, TValue> result))
            {
                resultKey = result.Key;
                resultValue = result.Value;
                return true;
            }
            resultKey = default;
            resultValue = default;
            return false;
        }

        bool INavigableDictionary<TKey, TValue>.TryGetFloor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
            => TryGetFloor(key, out resultKey, out resultValue);

        /// <summary>
        /// Gets the entry in the <see cref="SortedDictionary{TKey, TValue}"/> whose key
        /// is the least element greater than or equal to the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the ceiling of.</param>
        /// <param name="resultKey">Upon successful return, contains the key of the ceiling.</param>
        /// <param name="resultValue">Upon successful return, contains the value of the ceiling.</param>
        /// <returns><see langword="true"/> if a ceiling to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <b>weak successor</b> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>ceilingEntry()</c> method in the JDK.
        /// </remarks>
        public bool TryGetCeiling([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
        {
            if (_set.TryGetCeiling(new KeyValuePair<TKey, TValue>(key!, default!), out KeyValuePair<TKey, TValue> result))
            {
                resultKey = result.Key;
                resultValue = result.Value;
                return true;
            }
            resultKey = default;
            resultValue = default;
            return false;
        }

        bool INavigableDictionary<TKey, TValue>.TryGetCeiling([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue)
            => TryGetCeiling(key, out resultKey, out resultValue);

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> that iterates over the
        /// <see cref="SortedDictionary{TKey, TValue}"/> in reverse order.
        /// </summary>
        /// <returns>An enumerable that iterates over the <see cref="SortedDictionary{TKey, TValue}"/> in reverse order.</returns>
        /// <remarks>
        /// This corresponds roughly to the <c>descendingKeySet()</c> method in the JDK.
        /// </remarks>
        public IEnumerable<KeyValuePair<TKey, TValue>> Reverse()
            => _set.Reverse();

        IEnumerable<KeyValuePair<TKey, TValue>> INavigableDictionary<TKey, TValue>.Reverse()
            => Reverse();

        /// <summary>
        /// Returns a view of a sub dictionary in a <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// Usage Note: In Java, the upper bound of TreeMap.subMap() is exclusive. To match the behavior, call
        /// <see cref="GetView(TKey, bool, TKey, bool)"/>,
        /// setting <c>fromInclusive</c> to <see langword="true"/> and <c>toInclusive</c> to <see langword="false"/>.
        /// </summary>
        /// <param name="fromKey">The first desired key in the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
        /// <returns>A sub dictionary view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="fromKey"/> is after <paramref name="toKey"/>
        /// in the current view order according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="fromKey"/> and <paramref name="toKey"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="fromKey"/> and
        /// <paramref name="toKey"/> (inclusive), as defined by the current view order and the comparer.
        /// This method does not copy elements from the <see cref="SortedDictionary{TKey, TValue}"/>, but provides a
        /// window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>subMap()</c> method in the JDK.
        /// </remarks>
        public SortedDictionary<TKey, TValue> GetView([AllowNull] TKey fromKey, [AllowNull] TKey toKey)
        {
            SortedSet<KeyValuePair<TKey, TValue>> viewSet = _set.DoGetView(
                new KeyValuePair<TKey, TValue>(fromKey!, default!),
                fromInclusive: true,
                ExceptionArgument.fromKey,
                new KeyValuePair<TKey, TValue>(toKey!, default!),
                toInclusive: true,
                ExceptionArgument.toKey);

            return new SortedDictionary<TKey, TValue>(viewSet);
        }

        INavigableDictionary<TKey, TValue> INavigableDictionary<TKey, TValue>.GetView([AllowNull] TKey fromKey, [AllowNull] TKey toKey)
            => GetView(fromKey, toKey);

        /// <summary>
        /// Returns a view of a sub dictionary in a <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// Usage Note: To match the behavior of the JDK, call this overload with <paramref name="fromInclusive"/>
        /// set to <see langword="true"/> and <paramref name="toInclusive"/> set to <see langword="false"/>.
        /// </summary>
        /// <param name="fromKey">The first desired key in the range for the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="fromInclusive">If <see langword="true"/>, <paramref name="fromKey"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
        /// <param name="toInclusive">If <see langword="true"/>, <paramref name="toKey"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>A sub dictionary view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="fromKey"/> is after <paramref name="toKey"/>
        /// in the current view order according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="fromKey"/> and <paramref name="toKey"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="fromKey"/> and
        /// <paramref name="toKey"/>, as defined by the current view order and comparer. Each bound may either be inclusive
        /// (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the values of <paramref name="fromInclusive"/>
        /// and <paramref name="toInclusive"/>. This method does not copy elements from the
        /// <see cref="SortedDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>subMap()</c> method in the JDK.
        /// </remarks>
        public SortedDictionary<TKey, TValue> GetView([AllowNull] TKey fromKey, bool fromInclusive, [AllowNull] TKey toKey, bool toInclusive)
        {
            SortedSet<KeyValuePair<TKey, TValue>> viewSet = _set.DoGetView(
                new KeyValuePair<TKey, TValue>(fromKey!, default!),
                fromInclusive,
                ExceptionArgument.fromKey,
                new KeyValuePair<TKey, TValue>(toKey!, default!),
                toInclusive,
                ExceptionArgument.toKey);

            return new SortedDictionary<TKey, TValue>(viewSet);
        }

        INavigableDictionary<TKey, TValue> INavigableDictionary<TKey, TValue>.GetView([AllowNull] TKey fromKey, bool fromInclusive, [AllowNull] TKey toKey, bool toInclusive)
            => GetView(fromKey, fromInclusive, toKey, toInclusive);

        /// <summary>
        /// Returns the view of a subset in a <see cref="SortedDictionary{TKey, TValue}"/> with no lower bound.
        /// </summary>
        /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall before <paramref name="toKey"/>
        /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
        /// <see cref="SortedDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>headMap()</c> method in the JDK.
        /// </remarks>
        public SortedDictionary<TKey, TValue> GetViewBefore([AllowNull] TKey toKey)
        {
            SortedSet<KeyValuePair<TKey, TValue>> viewSet = _set.DoGetViewBefore(
                new KeyValuePair<TKey, TValue>(toKey!, default!),
                inclusive: true,
                ExceptionArgument.toKey);

            return new SortedDictionary<TKey, TValue>(viewSet);
        }

        INavigableDictionary<TKey, TValue> INavigableDictionary<TKey, TValue>.GetViewBefore([AllowNull] TKey toKey)
            => GetViewBefore(toKey);

        /// <summary>
        /// Returns the view of a subset in a <see cref="SortedDictionary{TKey, TValue}"/> with no lower bound.
        /// <para/>
        /// Usage Note: To match the default behavior of the JDK, call this overload with <paramref name="inclusive"/>
        /// set to <see langword="false"/>.
        /// </summary>
        /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
        /// <param name="inclusive">If <see langword="true"/>, <paramref name="toKey"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>
        /// This method returns a view of the range of elements that fall before <paramref name="toKey"/>, as defined
        /// by the current view order and comparer. The upper bound may either be inclusive (<see langword="true"/>) or
        /// exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>. 
        /// This method does not copy elements from the <see cref="SortedDictionary{TKey, TValue}"/>, but provides
        /// a window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>headMap()</c> method in the JDK.
        /// </returns>
        public SortedDictionary<TKey, TValue> GetViewBefore([AllowNull] TKey toKey, bool inclusive)
        {
            SortedSet<KeyValuePair<TKey, TValue>> viewSet = _set.DoGetViewBefore(
                new KeyValuePair<TKey, TValue>(toKey!, default!),
                inclusive,
                ExceptionArgument.toKey);

            return new SortedDictionary<TKey, TValue>(viewSet);
        }

        INavigableDictionary<TKey, TValue> INavigableDictionary<TKey, TValue>.GetViewBefore([AllowNull] TKey toKey, bool inclusive)
            => GetViewBefore(toKey, inclusive);

        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedDictionary{TKey, TValue}"/> with no upper bound.
        /// </summary>
        /// <param name="fromKey">The first desired key in the range for the view (lowest in ascending order, highest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="fromKey"/>
        /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
        /// <see cref="SortedDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>tailMap()</c> method in the JDK.
        /// </remarks>
        public SortedDictionary<TKey, TValue> GetViewAfter([AllowNull] TKey fromKey)
        {
            SortedSet<KeyValuePair<TKey, TValue>> viewSet = _set.DoGetViewAfter(
                new KeyValuePair<TKey, TValue>(fromKey!, default!),
                inclusive: true,
                ExceptionArgument.fromKey);

            return new SortedDictionary<TKey, TValue>(viewSet);
        }

        INavigableDictionary<TKey, TValue> INavigableDictionary<TKey, TValue>.GetViewAfter([AllowNull] TKey fromKey)
            => GetViewAfter(fromKey);

        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedDictionary{TKey, TValue}"/> with no upper bound.
        /// </summary>
        /// <param name="fromKey">The first desired key in the range for the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="inclusive">If <see langword="true"/>, <paramref name="fromKey"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="fromKey"/>, 
        /// as defined by the current view order and comparer. The lower bound may either be inclusive (<see langword="true"/>)
        /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>.
        /// This method does not copy elements from the <see cref="SortedDictionary{TKey, TValue}"/>, but
        /// provides a window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>tailMap()</c> method in the JDK.
        /// </remarks>
        public SortedDictionary<TKey, TValue> GetViewAfter([AllowNull] TKey fromKey, bool inclusive)
        {
            SortedSet<KeyValuePair<TKey, TValue>> viewSet = _set.DoGetViewAfter(
                new KeyValuePair<TKey, TValue>(fromKey!, default!),
                inclusive,
                ExceptionArgument.fromKey);

            return new SortedDictionary<TKey, TValue>(viewSet);
        }

        INavigableDictionary<TKey, TValue> INavigableDictionary<TKey, TValue>.GetViewAfter([AllowNull] TKey fromKey, bool inclusive)
            => GetViewAfter(fromKey, inclusive);

        /// <summary>
        /// Returns a reverse order view of the elements of the current <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A view that contains the values of the current <see cref="SortedDictionary{TKey, TValue}"/> in reverse order.</returns>
        /// <remarks>
        /// This method returns a reverse order view of the range of elements of this <see cref="SortedDictionary{TKey, TValue}"/>,
        /// as defined by the comparer. This method does not copy elements from the <see cref="SortedDictionary{TKey, TValue}"/>, but provides a
        /// window into the underlying <see cref="SortedDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>descendingMap()</c> method in the JDK.
        /// </remarks>
        public SortedDictionary<TKey, TValue> GetViewDescending()
        {
            SortedSet<KeyValuePair<TKey, TValue>> viewSet = _set.GetViewDescending();
            return new SortedDictionary<TKey, TValue>(viewSet);
        }

        INavigableDictionary<TKey, TValue> INavigableDictionary<TKey, TValue>.GetViewDescending()
            => GetViewDescending();

        #endregion INavigableDictionary<TKey, TValue> members

        #region ICollectionView Members

        bool ICollectionView.IsView => _set is ICollectionView view && view.IsView;

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
        /// using rules similar to those in the JDK's AbstractMap class. Two dictionaries are considered
        /// equal when they both contain the same mappings (in any order).
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
            private /*readonly*/ TreeSet<KeyValuePair<TKey, TValue>>.Enumerator _treeEnum;
            private /*readonly*/ int _getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;

            internal Enumerator(SortedDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                _treeEnum = dictionary._set.GetEnumeratorInternal(reverse: false);
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

            object? IEnumerator.Current
            {
                get
                {
                    if (NotStartedOrEnded)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
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
#pragma warning disable CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
                get
#pragma warning restore CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
                {
                    if (NotStartedOrEnded)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return Current.Key;
                }
            }

            object? IDictionaryEnumerator.Value
            {
                get
                {
                    if (NotStartedOrEnded)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
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
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return new DictionaryEntry(Current.Key!, Current.Value);
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
        [DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
        public sealed class KeyCollection : ICollection<TKey>, ICollection, INavigableCollection<TKey>, ICollectionView
#if FEATURE_IREADONLYCOLLECTIONS
            , IReadOnlyCollection<TKey>
#endif
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
                if (dictionary is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
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
            public Enumerator GetEnumerator() => new Enumerator(_dictionary);

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() =>
                Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<TKey>() :
                GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TKey>)this).GetEnumerator();

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
                if (array is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                if (index < 0)
                    ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
                if (array.Length - index < Count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

                // J2N: Ensure we always stay aligned with enumerator order
                foreach (var kvp  in _dictionary._set)
                {
                    array[index++] = kvp.Key;
                }
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                if (array.Rank != 1)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                if (array.GetLowerBound(0) != 0)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                if (index < 0)
                    ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
                if (array.Length - index < _dictionary.Count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

                if (array is TKey[] keys)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    try
                    {
                        object?[] objects = (object?[])array;

                        // J2N: Ensure we always stay aligned with enumerator order
                        foreach (var kvp in _dictionary._set)
                        {
                            objects[index++] = kvp.Key;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
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
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }

            void ICollection<TKey>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }

            /// <summary>
            /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
            /// <returns><c>true</c> if item is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
            public bool Contains([AllowNull] TKey item)
            {
                return _dictionary.ContainsKey(item);
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            #region INavigableCollection<T> members

            IComparer<TKey> ISortedCollection<TKey>.Comparer => _dictionary.Comparer;

            bool INavigableCollection<TKey>.TryGetFirst([MaybeNullWhen(false)] out TKey result)
            {
                if (_dictionary._set.TryGetFirst(out KeyValuePair<TKey, TValue> kvp))
                {
                    result = kvp.Key;
                    return true;
                }
                result = default;
                return false;
            }

            bool INavigableCollection<TKey>.TryGetLast([MaybeNullWhen(false)] out TKey result)
            {
                if (_dictionary._set.TryGetLast(out KeyValuePair<TKey, TValue> kvp))
                {
                    result = kvp.Key;
                    return true;
                }
                result = default;
                return false;
            }

            bool INavigableCollection<TKey>.RemoveFirst([MaybeNullWhen(false)] out TKey value)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                value = default;
                return false;
            }

            bool INavigableCollection<TKey>.RemoveLast([MaybeNullWhen(false)] out TKey value)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                value = default;
                return false;
            }

            bool INavigableCollection<TKey>.TryGetPredecessor([AllowNull] TKey item, [MaybeNullWhen(false)] out TKey result)
                => _dictionary.TryGetPredecessor(item, out result, out _);

            bool INavigableCollection<TKey>.TryGetSuccessor([AllowNull] TKey item, [MaybeNullWhen(false)] out TKey result)
                => _dictionary.TryGetSuccessor(item, out result, out _);

            bool INavigableCollection<TKey>.TryGetFloor([AllowNull] TKey item, [MaybeNullWhen(false)] out TKey result)
                => _dictionary.TryGetFloor(item, out result, out _);

            bool INavigableCollection<TKey>.TryGetCeiling([AllowNull] TKey item, [MaybeNullWhen(false)] out TKey result)
                => _dictionary.TryGetCeiling(item, out result, out _);

            IEnumerable<TKey> INavigableCollection<TKey>.Reverse()
            {
                var e = _dictionary._set.GetEnumeratorInternal(reverse: true);
                while (e.MoveNext())
                {
                    yield return e.Current.Key;
                }
            }

            INavigableCollection<TKey> INavigableCollection<TKey>.GetView([AllowNull] TKey fromItem, [AllowNull] TKey toItem)
            {
                // Note that if this is called on TreeSubSet, it overrides GetView() and properly
                // cascades the call to the underlying set.
                SortedDictionary<TKey, TValue> viewDictionary = _dictionary.GetView(fromItem, toItem);
                return new KeyCollection(viewDictionary);
            }

            INavigableCollection<TKey> INavigableCollection<TKey>.GetView([AllowNull] TKey fromItem, bool fromInclusive, [AllowNull] TKey toItem, bool toInclusive)
            {
                // Note that if this is called on TreeSubSet, it overrides GetView() and properly
                // cascades the call to the underlying set.
                SortedDictionary<TKey, TValue> viewDictionary = _dictionary.GetView(fromItem, fromInclusive, toItem, toInclusive);
                return new KeyCollection(viewDictionary);
            }

            INavigableCollection<TKey> INavigableCollection<TKey>.GetViewBefore([AllowNull] TKey toItem)
            {
                // Note that if this is called on TreeSubSet, it overrides GetViewBefore() and properly
                // cascades the call to the underlying set.
                SortedDictionary<TKey, TValue> viewDictionary = _dictionary.GetViewBefore(toItem);
                return new KeyCollection(viewDictionary);
            }

            INavigableCollection<TKey> INavigableCollection<TKey>.GetViewBefore([AllowNull] TKey toItem, bool inclusive)
            {
                // Note that if this is called on TreeSubSet, it overrides GetViewBefore() and properly
                // cascades the call to the underlying set.
                SortedDictionary<TKey, TValue> viewDictionary = _dictionary.GetViewBefore(toItem, inclusive);
                return new KeyCollection(viewDictionary);
            }

            INavigableCollection<TKey> INavigableCollection<TKey>.GetViewAfter([AllowNull] TKey fromItem)
            {
                // Note that if this is called on TreeSubSet, it overrides GetViewAfter() and properly
                // cascades the call to the underlying set.
                SortedDictionary<TKey, TValue> viewDictionary = _dictionary.GetViewAfter(fromItem);
                return new KeyCollection(viewDictionary);
            }

            INavigableCollection<TKey> INavigableCollection<TKey>.GetViewAfter([AllowNull] TKey fromItem, bool inclusive)
            {
                // Note that if this is called on TreeSubSet, it overrides GetViewAfter() and properly
                // cascades the call to the underlying set.
                SortedDictionary<TKey, TValue> viewDictionary = _dictionary.GetViewAfter(fromItem, inclusive);
                return new KeyCollection(viewDictionary);
            }

            INavigableCollection<TKey> INavigableCollection<TKey>.GetViewDescending()
            {
                // Note that if this is called on TreeSubSet, it overrides GetViewDescending() and properly
                // cascades the call to the underlying set.
                SortedDictionary<TKey, TValue> viewDictionary = _dictionary.GetViewDescending();
                return new KeyCollection(viewDictionary);
            }

            #endregion INavigableCollection<T> members

            #region ICollectionView Members

            bool ICollectionView.IsView => _dictionary._set is ICollectionView view && view.IsView;

            #endregion ICollectionView Members

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
                [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Following Microsoft's code style")]
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

                object? IEnumerator.Current
                {
                    get
                    {
                        if (_dictEnum.NotStartedOrEnded)
                        {
                            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
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
        [DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
        public sealed class ValueCollection : ICollection<TValue>, ICollection
#if FEATURE_IREADONLYCOLLECTIONS
            , IReadOnlyCollection<TValue>
#endif
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
                if (dictionary is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
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
            public Enumerator GetEnumerator() => new Enumerator(_dictionary);

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
                Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<TValue>() :
                GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TValue>)this).GetEnumerator();

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
                if (array is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                if (index < 0)
                    ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
                if (array.Length - index < Count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

                // J2N: Ensure we always stay aligned with enumerator order
                foreach (var kvp in _dictionary._set)
                {
                    array[index++] = kvp.Value;
                }
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array is null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }

                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }

                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }

                if (index < 0)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
                }

                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }

                if (array is TValue[] values)
                {
                    CopyTo(values, index);
                }
                else
                {
                    try
                    {
                        object?[] objects = (object?[])array;
                        // J2N: Ensure we always stay aligned with enumerator order
                        foreach (var kvp in _dictionary._set)
                        {
                            objects[index++] = kvp.Value;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
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
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }

            void ICollection<TValue>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                return _dictionary.ContainsValue(item);
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
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

                object? IEnumerator.Current
                {
                    get
                    {
                        if (_dictEnum.NotStartedOrEnded)
                        {
                            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
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

        // This class provides different comparer visibility:
        //
        // 1. keyComparer is the comparer used by SortedDictionary<TKey, TValue>, which may be wrapped in an IInternalStringComparer instance.
        // 2. The Comparer property provides access to the comparer that was provided by the user and unwraps it if necessary.
        // 3. SortedDictionary<TKey, TValue>.SpanAlternateLookup<TAlternateKeySpan> uses the keyComparer property to check whether ISpanAlternateComparer<TAlternateKeySpan, T> is implemented.
        //    Alternate lookup will fail without it.

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal sealed class KeyValuePairComparer : SCG.Comparer<KeyValuePair<TKey, TValue>> // J2N TODO: API - This is public in .NET, but I cannot find any docs for it.
#if FEATURE_SERIALIZABLE
            , ISerializable
#endif
        {

#if FEATURE_SERIALIZABLE
            private const string ComparerName = "keyComparer"; // Do not rename (binary serialization)

            // J2N NOTE: The constants in StringComparerMetadataSerializer are also used to round-trip any StringComparer metadata
#endif

            internal IComparer<TKey> keyComparer;

            public KeyValuePairComparer(IComparer<TKey>? keyComparer)
            {
                this.keyComparer = keyComparer ?? Comparer<TKey>.Default;

                // J2N: Special-case Comparer<string>.Default and StringComparer (all options).
                // We wrap these comparers to ensure that alternate string comparison is available.
                if (typeof(TKey) == typeof(string) &&
                    WrappedStringComparer.GetStringComparer(this.keyComparer) is IComparer<string> stringComparer)
                {
                    this.keyComparer = (IComparer<TKey>)stringComparer;
                }
            }

#if FEATURE_SERIALIZABLE
            private KeyValuePairComparer(SerializationInfo info, StreamingContext context)
            {
                keyComparer = (IComparer<TKey>)info.GetValue(ComparerName, typeof(IComparer<TKey>))!;

                // J2N:Try to wrap the comparer with WrappedStringComparer
                if (typeof(TKey) == typeof(string) && StringComparerMetadataSerializer.TryGetKnownStringComparer(keyComparer, info, out IComparer<string?>? stringComparer))
                {
                    keyComparer = (IComparer<TKey>)stringComparer;
                }
            }

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                IComparer<TKey> comparerToSerialize = keyComparer;

                if (typeof(TKey) == typeof(string) && comparerToSerialize is IInternalStringComparer internalComparer)
                {
                    comparerToSerialize = (IComparer<TKey>)internalComparer.GetUnderlyingComparer();
                }

                info.AddValue(ComparerName, comparerToSerialize, typeof(IComparer<TKey>));

                // J2N: Add metadata to the serialization blob so we can rehydrate the WrappedStringComparer properly
                if (typeof(TKey) == typeof(string) && StringComparerDescriptor.TryDescribe(comparerToSerialize, out StringComparerDescriptor descriptor))
                {
                    info.AddValue(ref descriptor);
                }
            }
#endif


            internal IComparer<TKey> Comparer
            {
                get
                {
                    Debug.Assert(keyComparer is not null, "The comparer should never be null.");
                    // J2N: We must unwrap the comparer before returning it to the user.
                    if (typeof(TKey) == typeof(string))
                    {
                        return (IComparer<TKey>)InternalStringComparer.GetUnderlyingComparer((IComparer<string?>)keyComparer!); // [!]: asserted above
                    }
                    else
                    {
                        return keyComparer!; // [!]: asserted above
                    }
                }
            }

            public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return keyComparer.Compare(x.Key, y.Key);
            }

            public override bool Equals(object? obj)
            {
                if (obj is KeyValuePairComparer other)
                {
                    // J2N: Get the underlying comparer for comparison, not a wrapper
                    IComparer<TKey> thisComparer = Comparer;
                    IComparer<TKey> otherComparer = other.Comparer;

                    // Commonly, both comparers will be the default comparer (and reference-equal). Avoid a virtual method call to Equals() in that case.
                    return thisComparer == otherComparer || thisComparer.Equals(otherComparer);
                }
                return false;
            }

            public override int GetHashCode()
            {
                // J2N: Get the underlying comparer for comparison, not a wrapper
                return Comparer.GetHashCode();
            }
        }

        #endregion

        #region Nested Class: AlternateKeyValuePairComparer<TAlternateKeySpan>

        /// <summary>
        /// An adapter to allow alternate lookup to cascade calls to the (already instantiated) comparer of <see cref="SortedSet{T}"/>
        /// and deferring the identification of <typeparamref name="TAlternateKeySpan"/> until alternate lookup is used.
        /// </summary>
        /// <typeparam name="TAlternateKeySpan">The type of <see cref="ReadOnlySpan{T}"/> for the alternate lookup comparer.</typeparam>
        internal sealed class AlternateKeyValuePairComparer<TAlternateKeySpan> : ISpanAlternateComparer<TAlternateKeySpan, KeyValuePair<TKey, TValue>>
        {
            private readonly KeyValuePairComparer comparer;

            public AlternateKeyValuePairComparer(KeyValuePairComparer comparer)
            {
                Debug.Assert(comparer is not null);
                this.comparer = comparer!; // [!] asserted above
            }

            // J2N: To use span alternate lookup from the underlying SortedSet<KeyValuePair<TKey, TValue>>, we
            // need this interface implemented so the checks pass when cascading from
            // SortedDictionary<TKey, TValue>.SpanAlternateLookup<TAlternateKeySpan> ->
            // SortedSet<KeyValuePair<TKey, TValue>>.SpanAlternateLookup<TAlternateSpan>
            int ISpanAlternateComparer<TAlternateKeySpan, KeyValuePair<TKey, TValue>>.Compare(ReadOnlySpan<TAlternateKeySpan> span, KeyValuePair<TKey, TValue> other) => Compare(span, other);

            public int Compare(ReadOnlySpan<TAlternateKeySpan> span, KeyValuePair<TKey, TValue> other)
            {
                if (comparer.keyComparer is ISpanAlternateComparer<TAlternateKeySpan, TKey> spanAlternateComparer)
                {
                    return spanAlternateComparer.Compare(span, other.Key);
                }

                // Should never get here - the above check should also be checked by the SpanAlternateComparer constructor
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IncompatibleComparer);
                return 0;
            }

            KeyValuePair<TKey, TValue> ISpanAlternateComparer<TAlternateKeySpan, KeyValuePair<TKey, TValue>>.Create(ReadOnlySpan<TAlternateKeySpan> span)
            {
                if (comparer.keyComparer is ISpanAlternateComparer<TAlternateKeySpan, TKey> spanAlternateComparer)
                {
                    return new KeyValuePair<TKey, TValue>(spanAlternateComparer.Create(span)!, default!);
                }

                // Should never get here - the above check should also be checked by the SpanAlternateComparer constructor
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IncompatibleComparer);
                return default;
            }

            // This overload is not part of the ISpanAlternateComparer<TAlternateKeySpan, KeyValuePair<TKey, TValue>>
            // contract, but is needed to allow the value to be passed to the KeyValuePair constructor.
            public KeyValuePair<TKey, TValue> Create(ReadOnlySpan<TAlternateKeySpan> span, TValue value)
            {
                if (comparer.keyComparer is ISpanAlternateComparer<TAlternateKeySpan, TKey> spanAlternateComparer)
                {
                    return new KeyValuePair<TKey, TValue>(spanAlternateComparer.Create(span)!, value);
                }

                // Should never get here - the above check should also be checked by the SpanAlternateComparer constructor
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IncompatibleComparer);
                return default;
            }
        }

        #endregion

        #region Nested Class: BclSortedDictionaryAdapter

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        private sealed class BclSortedDictionaryAdapter : IDistinctSortedCollection<KeyValuePair<TKey, TValue>>
        {
            private readonly SCG.SortedDictionary<TKey, TValue> dictionary;
            private readonly KeyValuePairComparer comparer;

            public BclSortedDictionaryAdapter(SCG.SortedDictionary<TKey, TValue> sortedDictionary, KeyValuePairComparer comparer)
            {
                Debug.Assert(sortedDictionary != null);
                Debug.Assert(comparer != null);
                dictionary = sortedDictionary!; // [!] asserted above
                this.comparer = comparer!; // [!] asserted above
            }

            public int Count => dictionary.Count;

            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;

            public IComparer<KeyValuePair<TKey, TValue>> Comparer => comparer;

            public void Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Add(item);

            public void Clear() => dictionary.Clear();

            public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();

            public bool Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item);

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)dictionary).GetEnumerator();
        }
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

        #endregion Nested Class: BclSortedDictionaryAdapter
    }

    /// <summary>
    /// This class is intended as a helper for backwards compatibility with existing SortedDictionaries.
    /// TreeSet has been converted into SortedSet{T}, which will be exposed publicly. SortedDictionaries
    /// have the problem where they have already been serialized to disk as having a backing class named
    /// TreeSet. To ensure that we can read back anything that has already been written to disk, we need to
    /// make sure that we have a class named TreeSet that does everything the way it used to.
    ///
    /// The only thing that makes it different from SortedSet is the type for serialization. Note that
    /// that the AddIfNotPresent() method is not overridden here so we can use the method from
    /// <see cref="SortedDictionary{TKey, TValue}.TryAdd(TKey, TValue)"/> without throwing exceptions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    internal sealed class TreeSet<T> : SortedSet<T> // J2N TODO: API - This is public in .NET, but I cannot find any docs for it.
    {
        public TreeSet()
            : base()
        { /* Intentionally blank */ }

        public TreeSet(IComparer<T> comparer) : base(comparer) { /* Intentionally blank */ }

        // J2N: Widened to allow any type of sorted collection as input
        internal TreeSet(ISortedCollection<T> collection, IComparer<T>? comparer) : base(collection, comparer) { /* Intentionally blank */ }

#if FEATURE_SERIALIZABLE
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        private TreeSet(SerializationInfo siInfo, StreamingContext context) : base(siInfo, context) { /* Intentionally blank */ }
#endif
    }
}
