// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/v9.0.5/src/libraries/System.Collections/src/System/Collections/Generic/OrderedDictionary.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using J2N.Collections.ObjectModel;
using J2N.Runtime.CompilerServices;
using J2N.Text;

#if FEATURE_EXCEPTION_STATIC_GUARDCLAUSES
using static System.ArgumentNullException;
using static System.ArgumentOutOfRangeException;
#else
using static J2N.Collections.StaticThrowHelper;
#endif

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Represents a collection of key/value pairs that are sorted based on insertion order.
    /// <see cref="OrderedDictionary{TKey, TValue}"/> adds the following features to <c>System.Collections.Generic.OrderedDictionary&lt;TKey, TValue&gt;</c>
    /// (in addition to making it available on older platforms):
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
    /// <remarks>
    /// Operations on the collection have algorithmic complexities that are similar to that of the <see cref="List{T}"/>
    /// class, except with lookups by key similar in complexity to that of <see cref="Dictionary{TKey, TValue}"/>.
    /// </remarks>
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public class OrderedDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IDictionary,
        IList<KeyValuePair<TKey, TValue>>,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyList<KeyValuePair<TKey, TValue>>,
#endif
        IList,
        IStructuralEquatable,
        IStructuralFormattable
    {
        /// <summary>The comparer used by the collection. May be null if the default comparer is used.</summary>
        private IEqualityComparer<TKey>? _comparer;
        /// <summary>Indexes into <see cref="_entries"/> for the start of chains; indices are 1-based.</summary>
        private int[]? _buckets;
        /// <summary>Ordered entries in the dictionary.</summary>
        /// <remarks>
        /// Unlike <see cref="Dictionary{TKey, TValue}"/>, removed entries are actually removed rather than left as holes
        /// that can be filled in by subsequent additions. This is done to retain ordering.
        /// </remarks>
        private Entry[]? _entries;
        /// <summary>The number of items in the collection.</summary>
        private int _count;
        /// <summary>Version number used to invalidate an enumerator.</summary>
        private int _version;
        /// <summary>Multiplier used on 64-bit to enable faster % operations.</summary>
        private ulong _fastModMultiplier;

        /// <summary>Lazily-initialized wrapper collection that serves up only the keys, in order.</summary>
        private KeyCollection? _keys;
        /// <summary>Lazily-initialized wrapper collection that serves up only the values, in order.</summary>
        private ValueCollection? _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class that is empty,
        /// has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public OrderedDictionary() : this(0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class that is empty,
        /// has the specified initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="OrderedDictionary{TKey, TValue}"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0.</exception>
        public OrderedDictionary(int capacity) : this(capacity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class that is empty,
        /// has the default initial capacity, and uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys,
        /// or null to use the default <see cref="EqualityComparer{TKey}"/> for the type of the key.
        /// </param>
        public OrderedDictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class that is empty,
        /// has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="OrderedDictionary{TKey, TValue}"/> can contain.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys,
        /// or null to use the default <see cref="EqualityComparer{TKey}"/> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0.</exception>
        public OrderedDictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            ThrowIfNegative(capacity);

            if (capacity > 0)
            {
                EnsureBucketsAndEntriesInitialized(capacity);
            }

            // Initialize the comparer:
            // - Strings: Special-case EqualityComparer<string>.Default, StringComparer.Ordinal, and
            //   StringComparer.OrdinalIgnoreCase. We start with a non-randomized comparer for improved throughput,
            //   falling back to a randomized comparer if the hash buckets become sufficiently unbalanced to cause
            //   more collisions than a preset threshold.
            // - Other reference types: we always want to store a comparer instance, either the one provided,
            //   or if one wasn't provided, the default (accessing EqualityComparer<TKey>.Default
            //   with shared generics on every dictionary access can add measurable overhead).
            // - Value types: if no comparer is provided, or if the default is provided, we'd prefer to use
            //   EqualityComparer<TKey>.Default.Equals on every use, enabling the JIT to
            //   devirtualize and possibly inline the operation.
            if (!typeof(TKey).IsValueType)
            {
                _comparer = comparer ?? EqualityComparer<TKey>.Default;

                if (typeof(TKey) == typeof(string) &&
                    // ReSharper disable once ConvertTypeCheckPatternToNullCheck
                    NonRandomizedStringEqualityComparer.GetStringComparer(_comparer) is IEqualityComparer<string> stringComparer)
                {
                    _comparer = (IEqualityComparer<TKey>)stringComparer;
                }
            }
            else if (comparer is not null && // first check for null to avoid forcing default comparer instantiation unnecessarily
                     !ReferenceEquals(comparer, EqualityComparer<TKey>.Default)) // J2N: use ReferenceEquals to be explicit
            {
                _comparer = comparer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class that contains elements copied from
        /// the specified <see cref="IDictionary{TKey, TValue}"/> and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new <see cref="OrderedDictionary{TKey, TValue}"/>.
        /// The initial order of the elements in the new collection is the order the elements are enumerated from the supplied dictionary.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
        public OrderedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class that contains elements copied from
        /// the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new <see cref="OrderedDictionary{TKey, TValue}"/>.
        /// The initial order of the elements in the new collection is the order the elements are enumerated from the supplied dictionary.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys,
        /// or null to use the default <see cref="EqualityComparer{TKey}"/> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
        public OrderedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) :
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract - J2N: so that the throw happens in the ctor body instead
            this(dictionary?.Count ?? 0, comparer)
        {
            if (dictionary is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);

            AddRange(dictionary!); // [!]: thrown if null above
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IEnumerable{T}"/> and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="OrderedDictionary{TKey, TValue}"/>.
        /// The initial order of the elements in the new collection is the order the elements are enumerated from the supplied collection.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IEnumerable{T}"/> and uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="OrderedDictionary{TKey, TValue}"/>.
        /// The initial order of the elements in the new collection is the order the elements are enumerated from the supplied collection.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}"/> implementation to use when comparing keys,
        /// or null to use the default <see cref="EqualityComparer{TKey}"/> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) :
            this((collection as ICollection<KeyValuePair<TKey, TValue>>)?.Count ?? 0, comparer)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (collection is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);

            AddRange(collection);
        }

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="Dictionary{TKey, TValue}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlyDictionary{TKey, TValue}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="Dictionary{TKey, TValue}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlyDictionary<TKey, TValue> AsReadOnly()
            => new ReadOnlyDictionary<TKey, TValue>(this);

        #endregion AsReadOnly

        /// <summary>Initializes the <see cref="_buckets"/>/<see cref="_entries"/>.</summary>
        /// <param name="capacity"></param>
        [MemberNotNull(nameof(_buckets))]
        [MemberNotNull(nameof(_entries))]
        private void EnsureBucketsAndEntriesInitialized(int capacity)
        {
            Resize(HashHelpers.GetPrime(capacity));
        }

        /// <summary>Gets the total number of key/value pairs the internal data structure can hold without resizing.</summary>
        public int Capacity => _entries?.Length ?? 0;

        /// <summary>Gets the <see cref="IEqualityComparer{TKey}"/> that is used to determine equality of keys for the dictionary.</summary>
        public IEqualityComparer<TKey> EqualityComparer
        {
            get
            {
                IEqualityComparer<TKey>? comparer = _comparer;

                // If the key is a string, we may have substituted a non-randomized comparer during construction.
                // If we did, fish out and return the actual comparer that had been provided.
                if (typeof(TKey) == typeof(string) &&
                    (comparer as NonRandomizedStringEqualityComparer)?.GetUnderlyingEqualityComparer() is IEqualityComparer<TKey> ec)
                {
                    return ec;
                }

                // Otherwise, return whatever comparer we have, or the default if none was provided.
                return comparer ?? EqualityComparer<TKey>.Default;
            }
        }

        /// <summary>Gets the number of key/value pairs contained in the <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        public int Count => _count;

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        /// <inheritdoc/>
        bool IDictionary.IsReadOnly => false;

        /// <inheritdoc/>
        bool IList.IsReadOnly => false;

        /// <inheritdoc/>
        bool IDictionary.IsFixedSize => false;

        /// <inheritdoc/>
        bool IList.IsFixedSize => false;

        /// <summary>Gets a collection containing the keys in the <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        public KeyCollection Keys => _keys ??= new(this);

#if FEATURE_IREADONLYCOLLECTIONS
        /// <inheritdoc/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
#endif

        /// <inheritdoc/>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        /// <inheritdoc/>
        ICollection IDictionary.Keys => Keys;

        /// <summary>Gets a collection containing the values in the <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        public ValueCollection Values => _values ??= new(this);

#if FEATURE_IREADONLYCOLLECTIONS
        /// <inheritdoc/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
#endif

        /// <inheritdoc/>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        /// <inheritdoc/>
        ICollection IDictionary.Values => Values;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => this;

        /// <inheritdoc/>
        object? IList.this[int index]
        {
            get => GetAt(index);
            set
            {
                ThrowIfNull(value);

                if (value is not KeyValuePair<TKey, TValue> tpair)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(KeyValuePair<TKey, TValue>));
                    return;
                }

                SetAt(index, tpair.Key, tpair.Value);
            }
        }

        /// <inheritdoc/>
        object? IDictionary.this[object? key]
        {
            get
            {
                // J2N: allow null keys
                //ThrowIfNull(key, ExceptionArgument.key);

                if (IsCompatibleKey(key) && TryGetValue((TKey?)key, out var value)) // J2N: allow null keys
                {
                    return value;
                }

                return null;
            }
            set
            {
                // J2N: allow null keys

                TKey tkey = default!;
                if (key is not null)
                {
                    if (key is not TKey temp)
                    {
                        ThrowHelper.ThrowWrongKeyTypeArgumentException(value, typeof(TKey));
                        return;
                    }

                    tkey = temp;
                }
                else if (default(TKey) is not null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                }

                TValue tvalue = default!;
                if (value is not null)
                {
                    if (value is not TValue temp)
                    {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                        return;
                    }

                    tvalue = temp;
                }
                else if (default(TValue) is not null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
                }

                this[tkey] = tvalue;
            }
        }

        /// <inheritdoc/>
        KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
        {
            get => GetAt(index);
            set => SetAt(index, value.Key, value.Value);
        }

#if FEATURE_IREADONLYCOLLECTIONS
        /// <inheritdoc/>
        KeyValuePair<TKey, TValue> IReadOnlyList<KeyValuePair<TKey, TValue>>.this[int index] => GetAt(index);
#endif

        /// <summary>Gets or sets the value associated with the specified key.</summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the specified key.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="key"/> does not exist in the collection.</exception>
        /// <remarks>Setting the value of an existing key does not impact its order in the collection.</remarks>
        public TValue this[[AllowNull] TKey key]
        {
            [return: MaybeNull]
            get
            {
                if (!TryGetValue(key, out TValue? value))
                {
                    if (key is null)
                    {
                        ThrowHelper.ThrowKeyNotFoundException("(null)");
                        return default;
                    }

                    ThrowHelper.ThrowKeyNotFoundException(key);
                }

                return value;
            }
            set
            {
                //ThrowIfNull(key, ExceptionArgument.key);

                bool modified = TryInsert(index: -1, key, value, InsertionBehavior.OverwriteExisting, out _);
                Debug.Assert(modified);
            }
        }

        // Contains changes from an unreleased future version (at time of writing) of .NET:
        // https://github.com/dotnet/runtime/blob/251ef76584bd6568439b5cbb3eb19bd13e42b93e/src/libraries/System.Collections/src/System/Collections/Generic/OrderedDictionary.cs#L385-L478
        /// <summary>Insert the key/value pair at the specified index.</summary>
        /// <param name="index">The index at which to insert the pair, or -1 to append.</param>
        /// <param name="key">The key to insert.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="behavior">
        /// The behavior controlling insertion behavior with respect to key duplication:
        /// - None: Immediately ends the operation, returning false, if the key already exists, e.g. TryAdd(key, value)
        /// - OverwriteExisting: If the key already exists, overwrites its value with the specified value, e.g. this[key] = value
        /// - ThrowOnExisting: If the key already exists, throws an exception, e.g. Add(key, value)
        /// </param>
        /// <param name="keyIndex">The index of the added or existing key. This is always a valid index into the dictionary.</param>
        /// <returns>true if the collection was updated; otherwise, false.</returns>
        private bool TryInsert(int index, [AllowNull] TKey key, [AllowNull] TValue value, InsertionBehavior behavior, out int keyIndex)
        {
            // Search for the key in the dictionary.
            uint hashCode = 0, collisionCount = 0;
            int i = IndexOf(key, ref hashCode, ref collisionCount);

            // Handle the case where the key already exists, based on the requested behavior.
            if (i >= 0)
            {
                keyIndex = i;
                Debug.Assert(0 <= keyIndex && keyIndex < _count);

                Debug.Assert(_entries is not null);

                switch (behavior)
                {
                    case InsertionBehavior.OverwriteExisting:
                        Debug.Assert(index < 0, "Expected index to be unspecied when overwriting an existing key.");
                        _entries![i].Value = value;
                        return true;

                    case InsertionBehavior.ThrowOnExisting:
                        ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                        break;

                    default:
                        Debug.Assert(behavior is InsertionBehavior.None, $"Unknown behavior: {behavior}");
                        Debug.Assert(index < 0, "Expected index to be unspecied when ignoring a duplicate key.");
                        return false;
                }
            }

            // The key doesn't exist. If a non-negative index was provided, that is the desired index at which to insert,
            // which should have already been validated by the caller. If negative, we're appending.
            if (index < 0)
            {
                index = _count;
            }
            Debug.Assert(index <= _count);

            // Ensure the collection has been initialized.
            if (_buckets is null)
            {
                EnsureBucketsAndEntriesInitialized(0);
            }

            // As we just initialized the collection, _entries must be non-null.
            Entry[]? entries = _entries;
            Debug.Assert(entries is not null);

            // Grow capacity if necessary to accomodate the extra entry.
            if (entries!.Length == _count) // [!]: asserted above
            {
                Resize(HashHelpers.ExpandPrime(entries.Length));
                entries = _entries;
            }

            // The _entries array is ordered, so we need to insert the new entry at the specified index. That means
            // not only shifting up all elements at that index and higher, but also updating the buckets and chains
            // to record the newly updated indices.
            for (i = _count - 1; i >= index; --i)
            {
                entries![i + 1] = entries[i]; // [!]: asserted above
                UpdateBucketIndex(i, shiftAmount: 1);
            }

            // Store the new key/value pair.
            ref Entry entry = ref entries![index]; // [!]: asserted above
            entry.HashCode = hashCode;
            entry.Key = key; // allow null keys
            entry.Value = value; // allow null values
            PushEntryIntoBucket(ref entry, index);
            _count++;
            _version++;

            RehashIfNecessary(collisionCount, entries);

            keyIndex = index;
            Debug.Assert(0 <= keyIndex && keyIndex < _count);

            return true;
        }

        /// <summary>Adds the specified key and value to the dictionary.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="OrderedDictionary{TKey, TValue}"/>.</exception>
        public void Add([AllowNull] TKey key, [AllowNull] TValue value)
        {
            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            TryInsert(index: -1, key, value, InsertionBehavior.ThrowOnExisting, out _);
        }

        /// <summary>Adds the specified key and value to the dictionary if the key doesn't already exist.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <returns>true if the key didn't exist and the key and value were added to the dictionary; otherwise, false.</returns>
        public bool TryAdd([AllowNull] TKey key, [AllowNull] TValue value) => TryAdd(key, value, out _);

        // Currently unreleased in .NET, but will be available in a future version.
        // https://github.com/dotnet/runtime/blob/251ef76584bd6568439b5cbb3eb19bd13e42b93e/src/libraries/System.Collections/src/System/Collections/Generic/OrderedDictionary.cs#L499-L510
        /// <summary>Adds the specified key and value to the dictionary if the key doesn't already exist.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <param name="index">The index of the added or existing <paramref name="key"/>. This is always a valid index into the dictionary.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <returns>true if the key didn't exist and the key and value were added to the dictionary; otherwise, false.</returns>
        public bool TryAdd([AllowNull] TKey key, [AllowNull] TValue value, out int index)
        {
            // J2N: allow null keys
            // ThrowIfNull(key);

            return TryInsert(index: -1, key, value, InsertionBehavior.None, out index);
        }

        /// <summary>Adds each element of the enumerable to the dictionary.</summary>
        private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            Debug.Assert(collection is not null);

            if (collection is KeyValuePair<TKey, TValue>[] array)
            {
                foreach (KeyValuePair<TKey, TValue> pair in array)
                {
                    Add(pair.Key, pair.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<TKey, TValue> pair in collection!) // [!]: asserted above
                {
                    Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>Removes all keys and values from the <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        public void Clear()
        {
            if (_buckets is not null && _count != 0)
            {
                Debug.Assert(_entries is not null);

                Array.Clear(_buckets, 0, _buckets.Length);
                Array.Clear(_entries!, 0, _count); // [!]: asserted above
                _count = 0;
                _version++;
            }
        }

        /// <summary>Determines whether the <see cref="OrderedDictionary{TKey, TValue}"/> contains the specified key.</summary>
        /// <param name="key">The key to locate in the <see cref="OrderedDictionary{TKey, TValue}"/>.</param>
        /// <returns>true if the <see cref="OrderedDictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKey([AllowNull] TKey key) => IndexOf(key) >= 0;

        /// <summary>Determines whether the <see cref="OrderedDictionary{TKey, TValue}"/> contains a specific value.</summary>
        /// <param name="value">The value to locate in the <see cref="OrderedDictionary{TKey, TValue}"/>. The value can be null for reference types.</param>
        /// <returns>true if the <see cref="OrderedDictionary{TKey, TValue}"/> contains an element with the specified value; otherwise, false.</returns>
        public bool ContainsValue([AllowNull] TValue value)
        {
            int count = _count;

            Entry[]? entries = _entries;
            if (entries is null)
            {
                return false;
            }

            if (value is null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (entries[i].Value is null)
                    {
                        return true;
                    }
                }
            }
            if (typeof(TValue).IsValueType)
            {
                for (int i = 0; i < count; i++)
                {
                    if (EqualityComparer<TValue>.Default.Equals(value!, entries[i].Value!)) // [!]: allow null values
                    {
                        return true;
                    }
                }
            }
            else
            {
                IEqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (comparer.Equals(value!, entries[i].Value!)) // [!]: allow null values
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>Determines whether the <see cref="OrderedDictionary{TKey, TValue}"/> contains a specific value.</summary>
        /// <param name="value">The value to locate in the <see cref="OrderedDictionary{TKey, TValue}"/>. The value can be null for reference types.</param>
        /// <param name="valueComparer">The <see cref="IEqualityComparer{TValue}"/> to use
        /// to test each value for equality.</param>
        /// <returns>true if the <see cref="OrderedDictionary{TKey, TValue}"/> contains an element with the specified value; otherwise, false.</returns>
        public bool ContainsValue([AllowNull] TValue value, IEqualityComparer<TValue>? valueComparer)
        {
            valueComparer ??= EqualityComparer<TValue>.Default;

            int count = _count;

            Entry[]? entries = _entries;
            if (entries is null)
            {
                return false;
            }

            if (value is null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (entries[i].Value is null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (valueComparer.Equals(value!, entries[i].Value!)) // [!]: allow null values
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>Gets the key/value pair at the specified index.</summary>
        /// <param name="index">The zero-based index of the pair to get.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.</exception>
        public KeyValuePair<TKey, TValue> GetAt(int index)
        {
            if ((uint)index >= (uint)_count)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange();
            }

            Debug.Assert(_entries is not null, "count must be positive, which means we must have entries");

            ref Entry e = ref _entries![index]; // [!]: asserted above
            return new(e.Key, e.Value);
        }

        /// <summary>Determines the index of a specific key in the <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The index of <paramref name="key"/> if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public int IndexOf([AllowNull] TKey key)
        {
            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            uint _ = 0;
            return IndexOf(key, ref _, ref _);
        }

        private int IndexOf([AllowNull] TKey key, ref uint outHashCode, ref uint outCollisionCount)
        {
            // J2N: allow null keys
            //Debug.Assert(key is not null, "Key nullness should have been validated by caller.");

            uint hashCode;
            uint collisionCount = 0;
            IEqualityComparer<TKey>? comparer = _comparer;

            if (_buckets is null)
            {
                hashCode = key is null ? 0 : (uint)(comparer?.GetHashCode(key) ?? key.GetHashCode());
                collisionCount = 0;
                goto ReturnNotFound;
            }

            int i = -1;
            ref Entry entry = ref UnsafeHelpers.NullRef<Entry>();

            Entry[]? entries = _entries;
            Debug.Assert(entries is not null, "expected entries to be is not null");

            if (typeof(TKey).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                comparer is null)
            {
                // ValueType: Devirtualize with EqualityComparer<TKey>.Default intrinsic

                hashCode = key is null ? 0 : (uint)key.GetHashCode();
                i = GetBucket(hashCode) - 1; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.

                // J2N: optimize for null case below to avoid comparer call
                if (key is not null)
                {
                    do
                    {
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length) // [!]: asserted above
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.Key!, key!))
                        {
                            goto Return;
                        }

                        i = entry.Next;

                        collisionCount++;
                    } while (collisionCount <= (uint)entries.Length);
                }
                else
                {
                    do
                    {
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length) // [!]: asserted above
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.HashCode == hashCode && entry.Key is null)
                        {
                            goto Return;
                        }

                        i = entry.Next;

                        collisionCount++;
                    } while (collisionCount <= (uint)entries.Length);
                }

                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                goto ConcurrentOperation;
            }
            else
            {
                Debug.Assert(comparer is not null);
                hashCode = key is null ? 0 : (uint)comparer!.GetHashCode(key); // [!]: asserted above
                i = GetBucket(hashCode) - 1; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.

                // J2N: optimize for null case below to avoid comparer call
                if (key is not null)
                {
                    do
                    {
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length) // [!]: asserted above
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.HashCode == hashCode &&
                            comparer!.Equals(entry.Key, key!)) // [!]: asserted above, allow null keys
                        {
                            goto Return;
                        }

                        i = entry.Next;

                        collisionCount++;
                    } while (collisionCount <= (uint)entries.Length);
                }
                else
                {
                    do
                    {
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length) // [!]: asserted above
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.HashCode == hashCode && entry.Key is null)
                        {
                            goto Return;
                        }

                        i = entry.Next;

                        collisionCount++;
                    } while (collisionCount <= (uint)entries.Length);
                }

                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                goto ConcurrentOperation;
            }

        ReturnNotFound:
            i = -1;
            outCollisionCount = collisionCount;
            goto Return;

        ConcurrentOperation:
            // We examined more entries than are actually in the list, which means there's a cycle
            // that's caused by erroneous concurrent use.
            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();

        Return:
            outHashCode = hashCode;
            return i;
        }

        /// <summary>Inserts an item into the collection at the specified index.</summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="key">The key to insert.</param>
        /// <param name="value">The value to insert.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="OrderedDictionary{TKey, TValue}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than <see cref="Count"/>.</exception>
        public void Insert(int index, [AllowNull] TKey key, TValue value)
        {
            if ((uint)index > (uint)_count)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange();
            }

            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            TryInsert(index, key, value, InsertionBehavior.ThrowOnExisting, out _);
        }

        /// <summary>Removes the value with the specified key from the <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns></returns>
        public bool Remove([AllowNull] TKey key)
        {
            // The overload Remove(TKey key, out TValue value) is a copy of this method with one additional
            // statement to copy the value for entry being removed into the output parameter.
            // Code has been intentionally duplicated for performance reasons.

            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            // Find the key.
            int index = IndexOf(key);
            if (index >= 0)
            {
                // It exists. Remove it.
                Debug.Assert(_entries is not null);

                RemoveAt(index);

                return true;
            }

            return false;
        }

        /// <summary>Removes the value with the specified key from the <see cref="OrderedDictionary{TKey, TValue}"/> and copies the element to the value parameter.</summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The removed element.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
        public bool Remove([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            // Find the key.
            int index = IndexOf(key);
            if (index >= 0)
            {
                // It exists. Remove it.
                Debug.Assert(_entries is not null);

                value = _entries![index].Value!; // [!]: asserted above
                RemoveAt(index);

                return true;
            }

            value = default;
            return false;
        }

        /// <summary>Removes the key/value pair at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            int count = _count;
            if ((uint)index >= (uint)count)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange();
            }

            // Remove from the associated bucket chain the entry that lives at the specified index.
            RemoveEntryFromBucket(index);

            // Shift down all entries above this one, and fix up the bucket chains to reflect the new indices.
            Entry[]? entries = _entries;
            Debug.Assert(entries is not null);
            for (int i = index + 1; i < count; i++)
            {
                entries![i - 1] = entries[i]; // [!]: asserted above
                UpdateBucketIndex(i, shiftAmount: -1);
            }

            entries![--_count] = default; // [!]: asserted above
            _version++;
        }

        /// <summary>Sets the value for the key at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <param name="value">The value to store at the specified index.</param>
        public void SetAt(int index, [AllowNull] TValue value)
        {
            if ((uint)index >= (uint)_count)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange();
            }

            Debug.Assert(_entries is not null);

            _entries![index].Value = value; // [!]: asserted above
        }

        /// <summary>Sets the key/value pair at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <param name="key">The key to store at the specified index.</param>
        /// <param name="value">The value to store at the specified index.</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetAt(int index, [AllowNull] TKey key, [AllowNull] TValue value)
        {
            if ((uint)index >= (uint)_count)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange();
            }

            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            Debug.Assert(_entries is not null);
            ref Entry e = ref _entries![index]; // [!]: asserted above

            // If the key matches the one that's already in that slot, just update the value.
            if (key is null)
            {
                if (e.Key is null)
                {
                    e.Value = value;
                    return;
                }
            }
            else if (typeof(TKey).IsValueType && _comparer is null)
            {
                if (EqualityComparer<TKey>.Default.Equals(key!, e.Key)) // [!]: allow null keys
                {
                    e.Value = value;
                    return;
                }
            }
            else
            {
                Debug.Assert(_comparer is not null);
                if (_comparer!.Equals(key!, e.Key)) // [!]: asserted above, allow null keys
                {
                    e.Value = value;
                    return;
                }
            }

            // The key doesn't match that index. If it exists elsewhere in the collection, fail.
            uint hashCode = 0, collisionCount = 0;
            if (IndexOf(key, ref hashCode, ref collisionCount) >= 0)
            {
                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
            }

            // The key doesn't exist in the collection. Update the key and value, but also update
            // the bucket chains, as the new key may not hash to the same bucket as the old key
            // (we could check for this, but in a properly balanced dictionary the chances should
            // be low for a match, so it's not worth it).
            RemoveEntryFromBucket(index);
            e.HashCode = hashCode;
            e.Key = key;
            e.Value = value;
            PushEntryIntoBucket(ref e, index);

            _version++;

            RehashIfNecessary(collisionCount, _entries);
        }

        /// <summary>Ensures that the dictionary can hold up to <paramref name="capacity"/> entries without resizing.</summary>
        /// <param name="capacity">The desired minimum capacity of the dictionary. The actual capacity provided may be larger.</param>
        /// <returns>The new capacity of the dictionary.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        public int EnsureCapacity(int capacity)
        {
            ThrowIfNegative(capacity);

            if (Capacity < capacity)
            {
                if (_buckets is null)
                {
                    EnsureBucketsAndEntriesInitialized(capacity);
                }
                else
                {
                    Resize(HashHelpers.GetPrime(capacity));
                }

                _version++;
            }

            return Capacity;
        }

        /// <summary>Sets the capacity of this dictionary to what it would be if it had been originally initialized with all its entries.</summary>
        public void TrimExcess() => TrimExcess(_count);

        /// <summary>Sets the capacity of this dictionary to hold up a specified number of entries without resizing.</summary>
        /// <param name="capacity">The desired capacity to which to shrink the dictionary.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than <see cref="Count"/>.</exception>
        public void TrimExcess(int capacity)
        {
            ThrowIfLessThan(capacity, Count);

            int currentCapacity = _entries?.Length ?? 0;
            capacity = HashHelpers.GetPrime(capacity);
            if (capacity < currentCapacity)
            {
                Resize(capacity);
            }
        }

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter.
        /// </param>
        /// <returns>true if the <see cref="OrderedDictionary{TKey, TValue}"/> contains an element with the specified key; otherwise, false.</returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            // Find the key.
            int index = IndexOf(key);
            if (index >= 0)
            {
                // It exists. Return its value.
                Debug.Assert(_entries is not null);
                value = _entries![index].Value!; // [!]: asserted above
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>Pushes the entry into its bucket.</summary>
        /// <remarks>
        /// The bucket is a linked list by index into the <see cref="_entries"/> array.
        /// The new entry's <see cref="Entry.Next"/> is set to the bucket's current
        /// head, and then the new entry is made the new head.
        /// </remarks>
        private void PushEntryIntoBucket(ref Entry entry, int entryIndex)
        {
            ref int bucket = ref GetBucket(entry.HashCode);
            entry.Next = bucket - 1;
            bucket = entryIndex + 1;
        }

        /// <summary>Removes an entry from its bucket.</summary>
        private void RemoveEntryFromBucket(int entryIndex)
        {
            // We're only calling this method if there's an entry to be removed, in which case
            // entries must have been initialized.
            Entry[]? entries = _entries;
            Debug.Assert(entries is not null);

            // Get the entry to be removed and the associated bucket.
            Entry entry = entries![entryIndex]; // [!]: asserted above
            ref int bucket = ref GetBucket(entry.HashCode);

            if (bucket == entryIndex + 1)
            {
                // If the entry was at the head of its bucket list, to remove it from the list we
                // simply need to update the next entry in the list to be the new head.
                bucket = entry.Next + 1;
            }
            else
            {
                // The entry wasn't the head of the list. Walk the chain until we find the entry,
                // updating the previous entry's Next to point to this entry's Next.
                int i = bucket - 1;
                int collisionCount = 0;
                while (true)
                {
                    ref Entry e = ref entries[i];
                    if (e.Next == entryIndex)
                    {
                        e.Next = entry.Next;
                        return;
                    }

                    i = e.Next;

                    if (++collisionCount > entries.Length)
                    {
                        // We examined more entries than are actually in the list, which means there's a cycle
                        // that's caused by erroneous concurrent use.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the bucket chain containing the specified entry (by index) to shift indices
        /// by the specified amount.
        /// </summary>
        /// <param name="entryIndex">The index of the target entry.</param>
        /// <param name="shiftAmount">
        /// 1 if this is part of an insert and the values are being shifted one higher.
        /// -1 if this is part of a remove and the values are being shifted one lower.
        /// </param>
        private void UpdateBucketIndex(int entryIndex, int shiftAmount)
        {
            Debug.Assert(shiftAmount is 1 or -1);

            Entry[]? entries = _entries;
            Debug.Assert(entries is not null);

            Entry entry = entries![entryIndex]; // [!]: asserted above
            ref int bucket = ref GetBucket(entry.HashCode);

            if (bucket == entryIndex + 1)
            {
                // If the entry was at the head of its bucket list, the only thing that needs to be updated
                // is the bucket head value itself, since no other entries' Next will be referencing this node.
                bucket += shiftAmount;
            }
            else
            {
                // The entry wasn't the head of the list. Walk the chain until we find the entry, updating
                // the previous entry's Next that's pointing to the target entry.
                int i = bucket - 1;
                int collisionCount = 0;
                while (true)
                {
                    ref Entry e = ref entries[i];
                    if (e.Next == entryIndex)
                    {
                        e.Next += shiftAmount;
                        return;
                    }

                    i = e.Next;

                    if (++collisionCount > entries.Length)
                    {
                        // We examined more entries than are actually in the list, which means there's a cycle
                        // that's caused by erroneous concurrent use.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see whether the collision count that occurred during lookup warrants upgrading to a non-randomized comparer,
        /// and does so if necessary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Is no-op in certain targets")]
        private void RehashIfNecessary(uint collisionCount, Entry[] entries)
        {
            // If we exceeded the hash collision threshold and we're using a randomized comparer, rehash.
            // This is only ever done for string keys, so we can optimize it all away for value type keys.
            if (!typeof(TKey).IsValueType &&
                collisionCount > HashHelpers.HashCollisionThreshold &&
                _comparer is NonRandomizedStringEqualityComparer)
            {
                // Switch to a randomized comparer and rehash.
                Resize(entries.Length, forceNewHashCodes: true);
            }
        }

        /// <summary>Grow or shrink <see cref="_buckets"/> and <see cref="_entries"/> to the specified capacity.</summary>
        [MemberNotNull(nameof(_buckets))]
        [MemberNotNull(nameof(_entries))]
        private void Resize(int newSize, bool forceNewHashCodes = false)
        {
            Debug.Assert(!forceNewHashCodes || !typeof(TKey).IsValueType, "Value types never rehash.");
            Debug.Assert(newSize >= _count, "The requested size must accomodate all of the current elements.");

            // Create the new arrays. We allocate both prior to storing either; in case one of the allocation fails,
            // we want to avoid corrupting the data structure.
            int[] newBuckets = new int[newSize];
            Entry[] newEntries = new Entry[newSize];
            if (IntPtr.Size == 8)
            {
                // Any time the capacity changes, that impacts the divisor of modulo operations,
                // and we need to update our fast modulo multiplier.
                _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);
            }

            // Copy the existing entries to the new entries array.
            int count = _count;
            if (_entries is not null)
            {
                Array.Copy(_entries, newEntries, count);
            }

            // If we're being asked to upgrade to a non-randomized comparer due to too many collisions, do so.
            if (!typeof(TKey).IsValueType && forceNewHashCodes)
            {
                // Store the original randomized comparer instead of the non-randomized one.
                Debug.Assert(_comparer is NonRandomizedStringEqualityComparer);
                IEqualityComparer<TKey> comparer = _comparer = (IEqualityComparer<TKey>)((NonRandomizedStringEqualityComparer)_comparer!).GetUnderlyingEqualityComparer(); // [!]: asserted above
                Debug.Assert(_comparer is not null);
                Debug.Assert(_comparer is not NonRandomizedStringEqualityComparer);

                // Update all of the entries' hash codes based on the new comparer.
                for (int i = 0; i < count; i++)
                {
                    newEntries[i].HashCode = (uint)comparer.GetHashCode(newEntries[i].Key!);
                }
            }

            // Now publish the buckets array. It's necessary to do this prior to the below loop,
            // as PushEntryIntoBucket will be populating _buckets.
            _buckets = newBuckets;

            // Populate the buckets.
            for (int i = 0; i < count; i++)
            {
                PushEntryIntoBucket(ref newEntries[i], i);
            }

            _entries = newEntries;
        }

        /// <summary>Gets the bucket assigned to the specified hash code.</summary>
        /// <remarks>
        /// Buckets are 1-based. This is so that the default initialized value of 0
        /// maps to -1 and is usable as a sentinel.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int GetBucket(uint hashCode)
        {
            int[]? buckets = _buckets;
            Debug.Assert(buckets is not null);

            if (IntPtr.Size == 8)
            {
                return ref buckets![HashHelpers.FastMod(hashCode, (uint)buckets.Length, _fastModMultiplier)]; // [!]: asserted above
            }
            else
            {
                return ref buckets![(uint)hashCode % buckets.Length]; // [!]: asserted above
            }
        }

        /// <summary>Returns an enumerator that iterates through the <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        /// <returns>A <see cref="OrderedDictionary{TKey, TValue}.Enumerator"/> structure for the <see cref="OrderedDictionary{TKey, TValue}"/>.</returns>
        public Enumerator GetEnumerator() => new(this, useDictionaryEntry: false);

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<KeyValuePair<TKey, TValue>>() :
            GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();

        /// <inheritdoc/>
        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this, useDictionaryEntry: true);

        /// <inheritdoc/>
        int IList<KeyValuePair<TKey, TValue>>.IndexOf(KeyValuePair<TKey, TValue> item)
        {
            // J2N: allow null keys
            //ThrowIfNull(item.Key, ExceptionArgument.item);

            int index = IndexOf(item.Key);
            if (index >= 0)
            {
                Debug.Assert(_entries is not null);
                if (EqualityComparer<TValue>.Default.Equals(item.Value, _entries![index].Value!)) // [!]: asserted above
                {
                    return index;
                }
            }

            return -1;
        }

        /// <inheritdoc/>
        void IList<KeyValuePair<TKey, TValue>>.Insert(int index, KeyValuePair<TKey, TValue> item) => Insert(index, item.Key, item.Value);

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            // J2N: allow null keys
            //ThrowIfNull(item.Key, ExceptionArgument.item);

            return
                TryGetValue(item.Key, out TValue? value) &&
                EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ThrowIfNull(array);
            ThrowIfNegative(arrayIndex);

            if (array.Length - arrayIndex < _count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }

            for (int i = 0; i < _count; i++)
            {
                ref Entry entry = ref _entries![i];
                array[arrayIndex++] = new(entry.Key, entry.Value);
            }
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) =>
            TryGetValue(item.Key, out TValue? value) &&
            EqualityComparer<TValue>.Default.Equals(value, item.Value) &&
            Remove(item.Key);

        /// <inheritdoc/>
        void IDictionary.Add(object? key, object? value)
        {
            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            TKey tkey = default!;
            if (key is not null)
            {
                if (key is not TKey temp)
                {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                    return;
                }

                tkey = temp;
            }
            else if (default(TKey) is not null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }

            TValue tvalue = default!;
            if (value is not null)
            {
                if (value is not TValue temp)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    return;
                }

                tvalue = temp;
            }
            else if (default(TValue) is not null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            Add(tkey, tvalue);
        }

        /// <inheritdoc/>
        bool IDictionary.Contains(object? key)
        {
            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            return IsCompatibleKey(key) && ContainsKey((TKey?)key);
        }

        /// <inheritdoc/>
        void IDictionary.Remove(object? key)
        {
            // J2N: allow null keys
            //ThrowIfNull(key, ExceptionArgument.key);

            if (IsCompatibleKey(key)) // J2N: allow null keys
            {
                Remove((TKey?)key);
            }
        }

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            ThrowIfNull(array);

            if (array.GetLowerBound(0) != 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }

            ThrowIfNegative(index);

            if (array.Length - index < _count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }

            if (array is KeyValuePair<TKey, TValue>[] tarray)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)this).CopyTo(tarray, index);
            }
            else
            {
                try
                {
                    object[]? objects = array as object[];
                    if (objects is null)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType(ExceptionArgument.array);
                    }

                    foreach (KeyValuePair<TKey, TValue> pair in this)
                    {
                        objects[index++] = pair;
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType(ExceptionArgument.array);
                }
            }
        }

        /// <inheritdoc/>
        int IList.Add(object? value)
        {
            if (value is not KeyValuePair<TKey, TValue> pair)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(KeyValuePair<TKey, TValue>));
                return Count - 1;
            }

            Add(pair.Key, pair.Value);
            return Count - 1;
        }

        /// <inheritdoc/>
        bool IList.Contains(object? value) =>
            value is KeyValuePair<TKey, TValue> pair &&
            TryGetValue(pair.Key, out TValue? v) &&
            EqualityComparer<TValue>.Default.Equals(v, pair.Value);

        /// <inheritdoc/>
        int IList.IndexOf(object? value)
        {
            if (value is KeyValuePair<TKey, TValue> pair)
            {
                return ((IList<KeyValuePair<TKey, TValue>>)this).IndexOf(pair);
            }

            return -1;
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object? value)
        {
            if (value is not KeyValuePair<TKey, TValue> pair)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(KeyValuePair<TKey, TValue>));
                return;
            }

            Insert(index, pair.Key, pair.Value);
        }

        /// <inheritdoc/>
        void IList.Remove(object? value)
        {
            if (value is KeyValuePair<TKey, TValue> pair)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)this).Remove(pair);
            }
        }

        #region Nested Structure: Entry

        /// <summary>Represents a key/value pair in the dictionary.</summary>
        private struct Entry
        {
            /// <summary>The index of the next entry in the chain, or -1 if this is the last entry in the chain.</summary>
            public int Next;
            /// <summary>Cached hash code of <see cref="Key"/>.</summary>
            public uint HashCode;
            /// <summary>The key.</summary>
            [AllowNull, MaybeNull]
            public TKey Key;
            /// <summary>The value associated with <see cref="Key"/>.</summary>
            [AllowNull, MaybeNull]
            public TValue Value;
        }

        #endregion

        #region Nested Structure: Enumerator

        /// <summary>Enumerates the elements of a <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            /// <summary>The dictionary being enumerated.</summary>
            private readonly OrderedDictionary<TKey, TValue> _dictionary;
            /// <summary>A snapshot of the dictionary's version when enumeration began.</summary>
            private readonly int _version;
            /// <summary>Whether Current should be a DictionaryEntry.</summary>
            private readonly bool _useDictionaryEntry;
            /// <summary>The current index.</summary>
            private int _index;

            /// <summary>Initialize the enumerator.</summary>
            internal Enumerator(OrderedDictionary<TKey, TValue> dictionary, bool useDictionaryEntry)
            {
                _dictionary = dictionary;
                _version = _dictionary._version;
                _useDictionaryEntry = useDictionaryEntry;
            }

            /// <inheritdoc/>
            public KeyValuePair<TKey, TValue> Current { get; private set; }

            /// <inheritdoc/>
            readonly object IEnumerator.Current => _useDictionaryEntry ?
                new DictionaryEntry(Current.Key!, Current.Value) : // [!]: allow null keys
                Current;

            /// <inheritdoc/>
            readonly DictionaryEntry IDictionaryEnumerator.Entry => new(Current.Key!, Current.Value); // [!]: allow null keys

            /// <inheritdoc/>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
            readonly object? IDictionaryEnumerator.Key => Current.Key;
#pragma warning restore CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression

            /// <inheritdoc/>
            readonly object? IDictionaryEnumerator.Value => Current.Value;

            /// <inheritdoc/>
            public bool MoveNext()
            {
                OrderedDictionary<TKey, TValue> dictionary = _dictionary;

                if (_version != dictionary._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                if (_index < dictionary._count)
                {
                    Debug.Assert(dictionary._entries is not null);
                    ref Entry entry = ref dictionary._entries![_index]; // [!]: asserted above
                    Current = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
                    _index++;
                    return true;
                }

                Current = default;
                return false;
            }

            /// <inheritdoc/>
            void IEnumerator.Reset()
            {
                if (_version != _dictionary._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                _index = 0;
                Current = default;
            }

            /// <inheritdoc/>
            readonly void IDisposable.Dispose() { }
        }

        #endregion

        #region Nested Class: KeyCollection

        /// <summary>Represents the collection of keys in a <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : IList<TKey>,
#if FEATURE_IREADONLYCOLLECTIONS
            IReadOnlyList<TKey>,
#endif
            IList
        {
            /// <summary>The dictionary whose keys are being exposed.</summary>
            private readonly OrderedDictionary<TKey, TValue> _dictionary;

            /// <summary>Initialize the collection wrapper.</summary>
            internal KeyCollection(OrderedDictionary<TKey, TValue> dictionary) => _dictionary = dictionary;

            /// <inheritdoc/>
            public int Count => _dictionary.Count;

            /// <inheritdoc/>
            bool ICollection<TKey>.IsReadOnly => true;

            /// <inheritdoc/>
            bool IList.IsReadOnly => true;

            /// <inheritdoc/>
            bool IList.IsFixedSize => false;

            /// <inheritdoc/>
            bool ICollection.IsSynchronized => false;

            /// <inheritdoc/>
            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            /// <inheritdoc/>
            public bool Contains([AllowNull] TKey key) => _dictionary.ContainsKey(key);

            /// <inheritdoc/>
            bool IList.Contains(object? value) => IsCompatibleKey(value) && Contains((TKey?)value);

            /// <inheritdoc/>
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                ThrowIfNull(array);
                ThrowIfNegative(arrayIndex);

                // J2N: throw ArgumentOutOfRangeException for consistency
                if ((uint)arrayIndex > (uint)array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(arrayIndex);
                }

                OrderedDictionary<TKey, TValue> dictionary = _dictionary;
                int count = dictionary._count;

                if (array.Length - arrayIndex < count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall, ExceptionArgument.array);
                }

                Entry[]? entries = dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    Debug.Assert(entries is not null);
                    array[arrayIndex++] = entries![i].Key!; // [!]: asserted above
                }
            }

            /// <inheritdoc/>
            void ICollection.CopyTo(Array array, int index)
            {
                ThrowIfNull(array);

                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }

                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }

                ThrowIfNegative(index);

                // J2N: throw ArgumentOutOfRangeException for consistency
                if ((uint)index > (uint)array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                }

                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }

                if (array is TKey[] keys)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    try
                    {
                        if (array is not object?[] objects)
                        {
                            ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                            return;
                        }

                        foreach (TKey key in this)
                        {
                            objects[index++] = key!;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }
                }
            }

            /// <inheritdoc/>
            TKey IList<TKey>.this[int index]
            {
                get => _dictionary.GetAt(index).Key;
                set => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }

            /// <inheritdoc/>
            object? IList.this[int index]
            {
                get => _dictionary.GetAt(index).Key;
                set => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }

#if FEATURE_IREADONLYCOLLECTIONS
            /// <inheritdoc/>
            TKey IReadOnlyList<TKey>.this[int index] => _dictionary.GetAt(index).Key;
#endif

            /// <summary>Returns an enumerator that iterates through the <see cref="OrderedDictionary{TKey, TValue}.KeyCollection"/>.</summary>
            /// <returns>A <see cref="OrderedDictionary{TKey, TValue}.KeyCollection.Enumerator"/> for the <see cref="OrderedDictionary{TKey, TValue}.KeyCollection"/>.</returns>
            public Enumerator GetEnumerator() => new(_dictionary);

            /// <inheritdoc/>
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() =>
                Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<TKey>() :
                GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TKey>)this).GetEnumerator();

            /// <inheritdoc/>
            int IList<TKey>.IndexOf(TKey item) => _dictionary.IndexOf(item);

            /// <inheritdoc/>
            void ICollection<TKey>.Add(TKey item)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            /// <inheritdoc/>
            void ICollection<TKey>.Clear()
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            /// <inheritdoc/>
            void IList<TKey>.Insert(int index, TKey item)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            /// <inheritdoc/>
            bool ICollection<TKey>.Remove(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }

            /// <inheritdoc/>
            void IList<TKey>.RemoveAt(int index)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            /// <inheritdoc/>
            int IList.Add(object? value)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return Count - 1;
            }

            /// <inheritdoc/>
            void IList.Clear()
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            /// <inheritdoc/>
            int IList.IndexOf(object? value) => IsCompatibleKey(value) ? _dictionary.IndexOf((TKey?)value) : -1;

            /// <inheritdoc/>
            void IList.Insert(int index, object? value)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            /// <inheritdoc/>
            void IList.Remove(object? value)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            /// <inheritdoc/>
            void IList.RemoveAt(int index)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            /// <summary>Enumerates the elements of a <see cref="OrderedDictionary{TKey, TValue}.KeyCollection"/>.</summary>
            public struct Enumerator : IEnumerator<TKey>
            {
                /// <summary>The dictionary's enumerator.</summary>
                private OrderedDictionary<TKey, TValue>.Enumerator _enumerator;

                /// <summary>Initialize the enumerator.</summary>
                internal Enumerator(OrderedDictionary<TKey, TValue> dictionary) => _enumerator = dictionary.GetEnumerator();

                /// <inheritdoc/>
                public TKey Current => _enumerator.Current.Key;

                /// <inheritdoc/>
                object? IEnumerator.Current => Current;

                /// <inheritdoc/>
                public bool MoveNext() => _enumerator.MoveNext();

                /// <inheritdoc/>
                void IEnumerator.Reset() => EnumerableHelpers.Reset(in _enumerator);

                /// <inheritdoc/>
                readonly void IDisposable.Dispose() { }
            }
        }

        #endregion

        #region Nested Class: ValueCollection

        /// <summary>Represents the collection of values in a <see cref="OrderedDictionary{TKey, TValue}"/>.</summary>
        [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class ValueCollection : IList<TValue>,
#if FEATURE_IREADONLYCOLLECTIONS
            IReadOnlyList<TValue>,
#endif
            IList
        {
            /// <summary>The dictionary whose values are being exposed.</summary>
            private readonly OrderedDictionary<TKey, TValue> _dictionary;

            /// <summary>Initialize the collection wrapper.</summary>
            internal ValueCollection(OrderedDictionary<TKey, TValue> dictionary) => _dictionary = dictionary;

            /// <inheritdoc/>
            public int Count => _dictionary.Count;

            /// <inheritdoc/>
            bool ICollection<TValue>.IsReadOnly => true;

            /// <inheritdoc/>
            bool IList.IsReadOnly => true;

            /// <inheritdoc/>
            bool IList.IsFixedSize => false;

            /// <inheritdoc/>
            bool ICollection.IsSynchronized => false;

            /// <inheritdoc/>
            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            /// <inheritdoc/>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                ThrowIfNull(array);
                ThrowIfNegative(arrayIndex);

                OrderedDictionary<TKey, TValue> dictionary = _dictionary;
                int count = dictionary._count;

                if (array.Length - arrayIndex < count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall, ExceptionArgument.array);
                }

                Entry[]? entries = dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    Debug.Assert(entries is not null);
                    array[arrayIndex++] = entries![i].Value!; // [!]: asserted above
                }
            }

            /// <summary>Returns an enumerator that iterates through the <see cref="OrderedDictionary{TKey, TValue}.ValueCollection"/>.</summary>
            /// <returns>A <see cref="OrderedDictionary{TKey, TValue}.ValueCollection.Enumerator"/> for the <see cref="OrderedDictionary{TKey, TValue}.ValueCollection"/>.</returns>
            public Enumerator GetEnumerator() => new(_dictionary);

            /// <inheritdoc/>
            TValue IList<TValue>.this[int index]
            {
                get => _dictionary.GetAt(index).Value;
                set => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }

#if FEATURE_IREADONLYCOLLECTIONS
            /// <inheritdoc/>
            TValue IReadOnlyList<TValue>.this[int index] => _dictionary.GetAt(index).Value;
#endif

            /// <inheritdoc/>
            object? IList.this[int index]
            {
                get => _dictionary.GetAt(index).Value;
                set => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }

            /// <inheritdoc/>
            bool ICollection<TValue>.Contains(TValue item) => _dictionary.ContainsValue(item);

            /// <inheritdoc/>
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
                Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<TValue>() :
                GetEnumerator();

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TValue>)this).GetEnumerator();

            /// <inheritdoc/>
            int IList<TValue>.IndexOf(TValue item)
            {
                Entry[]? entries = _dictionary._entries;
                if (entries is not null)
                {
                    int count = _dictionary._count;
                    if (item is not null)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (EqualityComparer<TValue>.Default.Equals(item, entries[i].Value!))
                            {
                                return i;
                            }
                        }
                    }
                    else // J2N: Factored out EqualityComparer for null check
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].Value is null)
                            {
                                return i;
                            }
                        }
                    }
                }

                return -1;
            }

            /// <inheritdoc/>
            void ICollection<TValue>.Add(TValue item)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            /// <inheritdoc/>
            void ICollection<TValue>.Clear()
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            /// <inheritdoc/>
            void IList<TValue>.Insert(int index, TValue item)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            /// <inheritdoc/>
            bool ICollection<TValue>.Remove(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
            }

            /// <inheritdoc/>
            void IList<TValue>.RemoveAt(int index)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            /// <inheritdoc/>
            int IList.Add(object? value)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return Count - 1;
            }

            /// <inheritdoc/>
            void IList.Clear()
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            /// <inheritdoc/>
            bool IList.Contains(object? value) =>
                value is null && default(TValue) is null ?
                    _dictionary.ContainsValue(default!) :
                    value is TValue tvalue && _dictionary.ContainsValue(tvalue);

            /// <inheritdoc/>
            int IList.IndexOf(object? value)
            {
                Entry[]? entries = _dictionary._entries;
                if (entries is not null)
                {
                    int count = _dictionary._count;

                    if (value is null && default(TValue) is null)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].Value is null)
                            {
                                return i;
                            }
                        }
                    }
                    else if (value is TValue tvalue)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (EqualityComparer<TValue>.Default.Equals(tvalue, entries[i].Value!))
                            {
                                return i;
                            }
                        }
                    }
                }

                return -1;
            }

            /// <inheritdoc/>
            void IList.Insert(int index, object? value)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            /// <inheritdoc/>
            void IList.Remove(object? value)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            /// <inheritdoc/>
            void IList.RemoveAt(int index)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            /// <inheritdoc/>
            void ICollection.CopyTo(Array array, int index)
            {
                ThrowIfNull(array);

                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }

                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }

                ThrowIfNegative(index);

                // J2N: throw ArgumentOutOfRangeException for consistency
                if ((uint)index > (uint)array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
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
                        if (array is not object?[] objects)
                        {
                            ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                            return;
                        }

                        foreach (TValue value in this)
                        {
                            objects[index++] = value!;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }
                }
            }

            /// <summary>Enumerates the elements of a <see cref="OrderedDictionary{TKey, TValue}.ValueCollection"/>.</summary>
            public struct Enumerator : IEnumerator<TValue>
            {
                /// <summary>The dictionary's enumerator.</summary>
                private OrderedDictionary<TKey, TValue>.Enumerator _enumerator;

                /// <summary>Initialize the enumerator.</summary>
                internal Enumerator(OrderedDictionary<TKey, TValue> dictionary) => _enumerator = dictionary.GetEnumerator();

                /// <inheritdoc/>
                public TValue Current => _enumerator.Current.Value;

                /// <inheritdoc/>
                object? IEnumerator.Current => Current;

                /// <inheritdoc/>
                public bool MoveNext() => _enumerator.MoveNext();

                /// <inheritdoc/>
                void IEnumerator.Reset() => EnumerableHelpers.Reset(in _enumerator);

                /// <inheritdoc/>
                readonly void IDisposable.Dispose() { }
            }
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
        /// Gets the hash code for the current dictionary. The hash code is calculated
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

        private static bool IsCompatibleKey(object? key)
        {
            if (key is null)
                return default(TKey) == null;

            return (key is TKey);
        }
    }
}
