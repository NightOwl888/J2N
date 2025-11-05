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
    /// Represents a collection of unique elements that are sorted based on insertion order.
    /// <see cref="OrderedHashSet{T}"/> adds the following features to <c>System.Collections.Generic.HashSet&lt;T&gt;</c>
    /// <list type="bullet">
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
    /// Usage Note: This class is designed to be a direct replacement for Java's LinkedHashSet.
    /// <para/>
    /// Note that the <see cref="ToString()"/> method uses the current culture by default to behave like other
    /// components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the set.</typeparam>
    /// <remarks>
    /// Operations on the collection have algorithmic complexities that are similar to that of the <see cref="List{T}"/>
    /// class, except with element lookups similar in complexity to that of <see cref="HashSet{T}"/>.
    /// <para/>
    /// <b>Java to .NET Method Mapping</b>
    /// <para/>
    /// The following table shows how common Java <see cref="ISet{T}"/> operations map to .NET <see cref="ISet{T}"/> operations:
    /// <list type="table">
    ///   <listheader>
    ///     <term>Java Operation</term>
    ///     <description>.NET Operation</description>
    ///   </listheader>
    ///   <item>
    ///     <term><c>set1.containsAll(set2)</c></term>
    ///     <description><see cref="IsSupersetOf(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>set1.containsAll(set2) &amp;&amp; !set1.equals(set2)</c></term>
    ///     <description><see cref="IsProperSupersetOf(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>set2.containsAll(set1)</c></term>
    ///     <description><see cref="IsSubsetOf(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>set2.containsAll(set1) &amp;&amp; !set2.equals(set1)</c></term>
    ///     <description><see cref="IsProperSubsetOf(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>Collections.disjoint(set1, set2)</c></term>
    ///     <description><c>!<see cref="Overlaps(IEnumerable{T})"/></c></description>
    ///   </item>
    ///   <item>
    ///     <term><c>!Collections.disjoint(set1, set2)</c></term>
    ///     <description><see cref="Overlaps(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>EnumSet.complementOf(enumSet)</c></term>
    ///     <description><see cref="SymmetricExceptWith(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>removeAll(other)</c></term>
    ///     <description><see cref="ExceptWith(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>retainAll(other)</c></term>
    ///     <description><see cref="IntersectWith(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>addAll(other)</c></term>
    ///     <description><see cref="UnionWith(IEnumerable{T})"/></description>
    ///   </item>
    ///   <item>
    ///     <term><c>equals(other)</c></term>
    ///     <description><see cref="SetEquals(IEnumerable{T})"/> or <see cref="Equals(object)"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class OrderedHashSet<T> : ICollection<T>, ISet<T>,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyCollection<T>,
#endif
#if FEATURE_READONLYSET
        IReadOnlySet<T>,
#endif
        IList<T>,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyList<T>,
#endif
        IList,
        IStructuralEquatable,
        IStructuralFormattable
    {
        /// <summary>Cutoff point for stackallocs. This corresponds to the number of ints.</summary>
        private const int StackAllocThreshold = 100;
        /// <summary>Shrink threshold for TrimExcess; if collection is larger than this multiple of its size, it will be shrunk.</summary>
        private const int ShrinkThreshold = 3;

        /// <summary>The comparer used by the collection. May be null if the default comparer is used.</summary>
        private IEqualityComparer<T>? _comparer;
        /// <summary>Indexes into <see cref="_entries"/> for the start of chains; indices are 1-based.</summary>
        private int[]? _buckets;
        /// <summary>Ordered entries in the set.</summary>
        private Entry[]? _entries;
        /// <summary>The number of items in the collection.</summary>
        private int _count;
        /// <summary>Version number used to invalidate an enumerator.</summary>
        private int _version;
        /// <summary>Multiplier used on 64-bit to enable faster % operations.</summary>
        private ulong _fastModMultiplier;
        /// <summary>J2N: Cached hash code value.</summary>
        private int _cachedHashCode;
        /// <summary>J2N: Version number when hash code was cached (-1 indicates uncached).</summary>
        private int _hashCodeVersion = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class that is empty,
        /// has the default initial capacity, and uses the default equality comparer.
        /// </summary>
        public OrderedHashSet() : this(0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class that is empty,
        /// has the specified initial capacity, and uses the default equality comparer.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="OrderedHashSet{T}"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0.</exception>
        public OrderedHashSet(int capacity) : this(capacity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class that is empty,
        /// has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing entries,
        /// or null to use the default <see cref="EqualityComparer{T}"/> for the type of the entry.
        /// </param>
        public OrderedHashSet(IEqualityComparer<T>? comparer) : this(0, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class that is empty,
        /// has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="OrderedHashSet{T}"/> can contain.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing entries,
        /// or null to use the default <see cref="EqualityComparer{T}"/> for the type of the entry.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">capacity is less than 0.</exception>
        public OrderedHashSet(int capacity, IEqualityComparer<T>? comparer)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            }

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
            //   with shared generics on every access can add measurable overhead).
            // - Value types: if no comparer is provided, or if the default is provided, we'd prefer to use
            //   EqualityComparer<TKey>.Default.Equals on every use, enabling the JIT to
            //   devirtualize and possibly inline the operation.
            if (!typeof(T).IsValueType)
            {
                _comparer = comparer ?? EqualityComparer<T>.Default;

                if (typeof(T) == typeof(string) &&
                    // ReSharper disable once ConvertTypeCheckPatternToNullCheck
                    NonRandomizedStringEqualityComparer.GetStringComparer(_comparer) is IEqualityComparer<string> stringComparer)
                {
                    _comparer = (IEqualityComparer<T>)stringComparer;
                }
            }
            else if (comparer is not null && // first check for null to avoid forcing default comparer instantiation unnecessarily
                     !ReferenceEquals(comparer, EqualityComparer<T>.Default)) // J2N: use ReferenceEquals to be explicit
            {
                _comparer = comparer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class that contains elements copied from
        /// the specified <see cref="ISet{T}"/> and uses the default equality comparer for the entry type.
        /// </summary>
        /// <param name="set">
        /// The <see cref="ISet{T}"/> whose elements are copied to the new <see cref="OrderedHashSet{T}"/>.
        /// The initial order of the elements in the new collection is the order the elements are enumerated from the supplied set.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> is null.</exception>
        public OrderedHashSet(ISet<T> set) : this(set, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class that contains elements copied from
        /// the specified <see cref="ISet{T}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="set">
        /// The <see cref="ISet{T}"/> whose elements are copied to the new <see cref="OrderedHashSet{T}"/>.
        /// The initial order of the elements in the new collection is the order the elements are enumerated from the supplied set.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys,
        /// or null to use the default <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> is null.</exception>
        public OrderedHashSet(ISet<T> set, IEqualityComparer<T>? comparer) :
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract - J2N: so that the throw happens in the ctor body instead
            this(set?.Count ?? 0, comparer)
        {
            if (set is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.set);

            AddRange(set!); // [!]: thrown if null above
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class that contains elements copied
        /// from the specified <see cref="IEnumerable{T}"/> and uses the default equality comparer for the entry type.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="OrderedHashSet{T}"/>.
        /// The initial order of the elements in the new collection is the order the elements are enumerated from the supplied collection.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public OrderedHashSet(IEnumerable<T> collection) : this(collection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedHashSet{T}"/> class that contains elements copied
        /// from the specified <see cref="IEnumerable{T}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="OrderedHashSet{T}"/>.
        /// The initial order of the elements in the new collection is the order the elements are enumerated from the supplied collection.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys,
        /// or null to use the default <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public OrderedHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) :
            this((collection as ICollection<T>)?.Count ?? 0, comparer)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (collection is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);

            if (collection is OrderedHashSet<T> otherAsOrderedHashSet && EffectiveEqualityComparersAreEqual(this, otherAsOrderedHashSet))
            {
                ConstructFrom(otherAsOrderedHashSet);
            }
            else
            {
                // To avoid excess resizes, first set size based on collection's count. The collection may
                // contain duplicates, so call TrimExcess if resulting OrderedHashSet is larger than the threshold.
                AddRange(collection);

                if (_count > 0 && _entries!.Length / _count > ShrinkThreshold)
                {
                    TrimExcess();
                }
            }
        }

        /// <summary>Initializes the OrderedHashSet from another OrderedHashSet with the same element type and equality comparer.</summary>
        private void ConstructFrom(OrderedHashSet<T> source)
        {
            Debug.Assert(EffectiveEqualityComparersAreEqual(this, source), "must use identical effective comparers.");

            if (source.Count == 0)
            {
                // As well as short-circuiting on the rest of the work done,
                // this avoids errors from trying to access source._buckets
                // or source._entries when they aren't initialized.
                return;
            }

            int capacity = source._buckets!.Length;
            int threshold = HashHelpers.ExpandPrime(source.Count + 1);

            if (threshold >= capacity)
            {
                _buckets = (int[])source._buckets.Clone();
                _entries = (Entry[])source._entries!.Clone();
                _count = source._count;
                _fastModMultiplier = source._fastModMultiplier;
            }
            else
            {
                EnsureBucketsAndEntriesInitialized(source.Count);

                Entry[]? entries = source._entries;
                for (int i = 0; i < source._count; i++)
                {
                    ref Entry entry = ref entries![i];
                    TryInsert(index: -1, entry.Value, InsertionBehavior.OverwriteExisting, out _);
                }
            }

            Debug.Assert(Count == source.Count);
        }

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlySet{T}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current set.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="OrderedHashSet{T}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlySet{T}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="OrderedHashSet{T}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlySet<T> AsReadOnly()
            => new ReadOnlySet<T>(this);

        #endregion AsReadOnly

        /// <summary>Initializes the <see cref="_buckets"/>/<see cref="_entries"/>.</summary>
        /// <param name="capacity"></param>
        [MemberNotNull(nameof(_buckets))]
        [MemberNotNull(nameof(_entries))]
        private void EnsureBucketsAndEntriesInitialized(int capacity)
        {
            Resize(HashHelpers.GetPrime(capacity));
        }

        /// <summary>Gets the total number of elements the internal data structure can hold without resizing.</summary>
        public int Capacity => _entries?.Length ?? 0;

        /// <summary>Gets the <see cref="IEqualityComparer{T}"/> that is used to determine equality of elements for the set.</summary>
        public IEqualityComparer<T> EqualityComparer
        {
            get
            {
                IEqualityComparer<T>? comparer = _comparer;

                // If the key is a string, we may have substituted a non-randomized comparer during construction.
                // If we did, fish out and return the actual comparer that had been provided.
                if (typeof(T) == typeof(string) &&
                    (comparer as NonRandomizedStringEqualityComparer)?.GetUnderlyingEqualityComparer() is IEqualityComparer<T> ec)
                {
                    return ec;
                }

                // Otherwise, return whatever comparer we have, or the default if none was provided.
                return comparer ?? EqualityComparer<T>.Default;
            }
        }

        /// <summary>Gets the effective equality comparer used for operations.</summary>
        internal IEqualityComparer<T> EffectiveComparer => _comparer ?? EqualityComparer<T>.Default;

        /// <summary>Gets the number of elements contained in the <see cref="OrderedHashSet{T}"/>.</summary>
        public int Count => _count;

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        /// <inheritdoc/>
        bool IList.IsReadOnly => false;

        /// <inheritdoc/>
        bool IList.IsFixedSize => false;

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

                if (value is not T t)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                    return;
                }

                SetAt(index, t);
            }
        }

        /// <inheritdoc/>
        T IList<T>.this[int index]
        {
            get => GetAt(index);
            set => SetAt(index, value);
        }

#if FEATURE_IREADONLYCOLLECTIONS
        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index] => GetAt(index);
#endif

        // Contains changes from an unreleased future version (at time of writing) of .NET:
        // https://github.com/dotnet/runtime/blob/251ef76584bd6568439b5cbb3eb19bd13e42b93e/src/libraries/System.Collections/src/System/Collections/Generic/OrderedDictionary.cs#L385-L478
        /// <summary>Insert the element at the specified index.</summary>
        /// <param name="index">The index at which to insert the element, or -1 to append.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="behavior">
        /// The behavior controlling insertion behavior with respect to value duplication:
        /// - None: Immediately ends the operation, returning false, if the value already exists, e.g. TryAdd(value)
        /// - OverwriteExisting: If the key already exists, overwrites its value with the specified value, e.g. Add(value)
        /// - ThrowOnExisting: If the key already exists, throws an exception
        /// </param>
        /// <param name="valueIndex">The index of the added or existing element. This is always a valid index into the set.</param>
        /// <returns>true if the collection was updated; otherwise, false.</returns>
        private bool TryInsert(int index, [AllowNull] T value, InsertionBehavior behavior, out int valueIndex)
        {
            // Search for the value in the set.
            uint hashCode = 0, collisionCount = 0;
            int i = IndexOf(value, ref hashCode, ref collisionCount);

            // Handle the case where the value already exists, based on the requested behavior.
            if (i >= 0)
            {
                valueIndex = i;
                Debug.Assert(0 <= valueIndex && valueIndex < _count);

                Debug.Assert(_entries is not null);

                switch (behavior)
                {
                    case InsertionBehavior.OverwriteExisting:
                        Debug.Assert(index < 0, "Expected index to be unspecified when overwriting an existing key.");
                        _entries![i].Value = value;
                        return true;

                    case InsertionBehavior.ThrowOnExisting:
                        ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(value);
                        break;

                    default:
                        Debug.Assert(behavior is InsertionBehavior.None, $"Unknown behavior: {behavior}");
                        Debug.Assert(index < 0, "Expected index to be unspecified when ignoring a duplicate key.");
                        return false;
                }
            }

            // The value doesn't exist. If a non-negative index was provided, that is the desired index at which to insert,
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
            entry.Value = value; // allow null values
            PushEntryIntoBucket(ref entry, index);
            _count++;
            _version++;

            RehashIfNecessary(collisionCount, entries);

            valueIndex = index;
            Debug.Assert(0 <= valueIndex && valueIndex < _count);

            return true;
        }

        /// <summary>Adds the specified element to the set.</summary>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public bool Add([AllowNull] T value)
        {
            return TryInsert(index: -1, value, InsertionBehavior.None, out _);
        }

        /// <summary>Adds the specified value to the set if the value doesn't already exist.</summary>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <param name="index">The index of the added or existing <paramref name="value"/>. This is always a valid index into the set.</param>
        /// <returns>true if the key didn't exist and the value was added to the set; otherwise, false.</returns>
        public bool TryAdd([AllowNull] T value, out int index)
        {
            return TryInsert(index: -1, value, InsertionBehavior.None, out index);
        }

        /// <summary>Adds each element of the enumerable to the set.</summary>
        private void AddRange(IEnumerable<T> collection)
        {
            Debug.Assert(collection is not null);

            if (collection is T[] array)
            {
                foreach (T t in array)
                {
                    Add(t);
                }
            }
            else
            {
                foreach (T t in collection!) // [!]: asserted above
                {
                    Add(t);
                }
            }
        }

        /// <summary>Removes all values from the <see cref="OrderedHashSet{T}"/>.</summary>
        public void Clear()
        {
            if (_buckets is not null && _count != 0)
            {
                Debug.Assert(_entries is not null);

#if FEATURE_ARRAY_CLEAR_ARRAY
                Array.Clear(_buckets);
#else
                Array.Clear(_buckets, 0, _buckets.Length);
#endif
#if FEATURE_ARRAY_CLEAR_ARRAY
                Array.Clear(_entries!);
#else
                Array.Clear(_entries!, 0, _count);
#endif
                _count = 0;
                _version++;
            }
        }

        /// <summary>Determines whether the <see cref="OrderedHashSet{T}"/> contains the specified value.</summary>
        /// <param name="value">The value to locate in the <see cref="OrderedHashSet{T}"/>.</param>
        /// <returns>true if the <see cref="OrderedHashSet{T}"/> contains the specified value; otherwise, false.</returns>
        public bool Contains([AllowNull] T value) => IndexOf(value) >= 0;

        /// <summary>Gets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/>.</exception>
        public T GetAt(int index)
        {
            if ((uint)index >= (uint)_count)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange();
            }

            Debug.Assert(_entries is not null, "count must be positive, which means we must have entries");

            ref Entry e = ref _entries![index]; // [!]: asserted above
            return e.Value;
        }

        /// <summary>Determines the index of a specific value in the <see cref="OrderedHashSet{T}"/>.</summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>The index of <paramref name="value"/> if found; otherwise, -1.</returns>
        public int IndexOf([AllowNull] T value)
        {
            uint _ = 0;
            return IndexOf(value, ref _, ref _);
        }

        private int IndexOf([AllowNull] T value, ref uint outHashCode, ref uint outCollisionCount)
        {
            uint hashCode;
            uint collisionCount = 0;
            IEqualityComparer<T>? comparer = _comparer;

            if (_buckets is null)
            {
                hashCode = value is null ? 0 : (uint)(comparer?.GetHashCode(value) ?? value.GetHashCode());
                collisionCount = 0;
                goto ReturnNotFound;
            }

            int i = -1;
            ref Entry entry = ref UnsafeHelpers.NullRef<Entry>();

            Entry[]? entries = _entries;
            Debug.Assert(entries is not null, "expected entries to be is not null");

            if (typeof(T).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                comparer is null)
            {
                // ValueType: Devirtualize with EqualityComparer<T>.Default intrinsic

                hashCode = value is null ? 0 : (uint)value.GetHashCode();
                i = GetBucket(hashCode) - 1; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.

                // J2N: optimize for null case below to avoid comparer call
                if (value is not null)
                {
                    do
                    {
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length) // [!]: asserted above
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.HashCode == hashCode && EqualityComparer<T>.Default.Equals(entry.Value!, value!))
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
                        if (entry.HashCode == hashCode && entry.Value is null)
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
                hashCode = value is null ? 0 : (uint)comparer!.GetHashCode(value); // [!]: asserted above
                i = GetBucket(hashCode) - 1; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.

                // J2N: optimize for null case below to avoid comparer call
                if (value is not null)
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
                            comparer!.Equals(entry.Value, value!)) // [!]: asserted above, allow null keys
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
                        if (entry.HashCode == hashCode && entry.Value is null)
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
        /// <param name="value">The value to insert.</param>
        /// <exception cref="ArgumentException">An element with the same value already exists in the <see cref="OrderedHashSet{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than <see cref="Count"/>.</exception>
        public void Insert(int index, [AllowNull] T value)
        {
            if ((uint)index > (uint)_count)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange();
            }

            TryInsert(index, value, InsertionBehavior.ThrowOnExisting, out _);
        }

        /// <summary>Removes the element with the specified value from the <see cref="OrderedHashSet{T}"/>.</summary>
        /// <param name="value">The value of the element to remove.</param>
        /// <returns></returns>
        public bool Remove([AllowNull] T value)
        {
            // Find the value.
            int index = IndexOf(value);
            if (index >= 0)
            {
                // It exists. Remove it.
                Debug.Assert(_entries is not null);

                RemoveAt(index);

                return true;
            }

            return false;
        }

        /// <summary>Removes the element at the specified index.</summary>
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

            --_count;
            if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
            {
                entries![_count] = default;
            }
            _version++;
        }

        /// <summary>Sets the value at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <param name="value">The value to store at the specified index.</param>
        public void SetAt(int index, [AllowNull] T value)
        {
            if ((uint)index >= (uint)_count)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange();
            }

            Debug.Assert(_entries is not null);

            _entries![index].Value = value; // [!]: asserted above
        }

        /// <summary>Removes all elements that match the condition defined by the specified predicate from the set.</summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements that were removed from the set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        public int RemoveWhere(Predicate<T> match)
        {
            ThrowIfNull(match);

            // Empty sets may have _entries == null
            if (_entries is null)
            {
                return 0;
            }

            int numRemoved = 0;
            for (int i = _count - 1; i >= 0; i--)
            {
                // Cache value in case delegate removes it
                T? value = _entries![i].Value;
                if (match(value!))
                {
                    // Check again that remove actually removed it
                    if (Remove(value))
                    {
                        numRemoved++;
                    }
                }
            }

            return numRemoved;
        }

        /// <summary>Ensures that the set can hold up to <paramref name="capacity"/> entries without resizing.</summary>
        /// <param name="capacity">The desired minimum capacity of the set. The actual capacity provided may be larger.</param>
        /// <returns>The new capacity of the set.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is negative.</exception>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            }

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
            }

            return Capacity;
        }

        /// <summary>Sets the capacity of this set to what it would be if it had been originally initialized with all its entries.</summary>
        public void TrimExcess() => TrimExcess(_count);

        /// <summary>Sets the capacity of this set to hold up a specified number of entries without resizing.</summary>
        /// <param name="capacity">The desired capacity to which to shrink the set.</param>
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

        /// <summary>
        /// Searches the set for a given value and returns the equal value it finds, if any.
        /// </summary>
        /// <param name="equalValue">The value to search for.</param>
        /// <param name="actualValue">The value from the set that the search found, or the
        /// default value of <typeparamref name="T"/> when the search yielded no match.</param>
        /// <returns>A value indicating whether the search was successful.</returns>
        /// <remarks>
        /// This can be useful when you want to reuse a previously stored reference instead of
        /// a newly constructed one (so that more sharing of references can occur) or to look up
        /// a value that has more complete data than the value you currently have, although their
        /// comparer functions indicate they are equal.
        /// </remarks>
        public bool TryGetValue([AllowNull] T equalValue, [MaybeNullWhen(false)] out T actualValue)
        {
            // Find the element.
            int index = IndexOf(equalValue);
            if (index >= 0)
            {
                // It exists. Return its value.
                Debug.Assert(_entries is not null);
                actualValue = _entries![index].Value!; // [!]: asserted above
                return true;
            }

            actualValue = default;
            return false;
        }

        /// <summary>
        /// Modifies the current <see cref="OrderedHashSet{T}"/> object to contain all elements that are present
        /// in itself, the specified collection, or both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="OrderedHashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in the
        /// <paramref name="other"/> parameter.
        /// </remarks>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            foreach (T item in other)
            {
                TryInsert(index: -1, item, InsertionBehavior.None, out _);
            }
        }

        /// <summary>
        /// Modifies the current <see cref="HashSet{T}"/> object to contain only elements that are
        /// present in that object and in the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// If the collection represented by the other parameter is a <see cref="HashSet{T}"/> collection with
        /// the same equality comparer as the current <see cref="HashSet{T}"/> object, this method is an O(<c>n</c>) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c>
        /// is the number of elements in <paramref name="other"/>.
        /// </remarks>
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // intersection of anything with empty set is empty set, so return if count is 0
            if (_count == 0)
            {
                return;
            }

            // set intersecting with itself is the same set
            if (other == this)
            {
                return;
            }

            // if other is empty, intersection is empty set; remove all elements and we're done
            // can only figure this out if implements ICollection<T>. (IEnumerable<T> has no count)
            ICollection<T>? otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null)
            {
                if (otherAsCollection.Count == 0)
                {
                    Clear();
                    return;
                }

                // faster if other is a hashset using same equality comparer; so check
                // that other is a hashset using the same equality comparer.
                if (otherAsCollection is ISet<T> otherAsSet && AreEqualityComparersEqual(this, otherAsSet))
                {
                    IntersectWithHashSetWithSameEC(otherAsSet);
                    return;
                }
            }

            IntersectWithEnumerable(other);
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <param name="other">The collection of items to remove from the <see cref="HashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="ExceptWith(IEnumerable{T})"/> method is the equivalent of mathematical set subtraction.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in the <paramref name="other"/> parameter.
        /// </remarks>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // this is already the empty set; return
            if (_count == 0)
            {
                return;
            }

            // special case if other is this; a set minus itself is the empty set
            if (other == this)
            {
                Clear();
                return;
            }

            // remove every element in other from this
            foreach (T element in other)
            {
                Remove(element);
            }
        }

        /// <summary>
        /// Modifies the current <see cref="HashSet{T}"/> object to contain only elements that are present either
        /// in that object or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// If the other parameter is a <see cref="HashSet{T}"/> collection with the same equality comparer as
        /// the current <see cref="HashSet{T}"/> object, this method is an O(<c>n</c>) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where n is the number of elements in other and
        /// <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // if set is empty, then symmetric difference is other
            if (_count == 0)
            {
                UnionWith(other);
                return;
            }

            // special case this; the symmetric difference of a set with itself is the empty set
            if (other == this)
            {
                Clear();
                return;
            }

            // If other is a HashSet, it has unique elements according to its equality comparer,
            // but if they're using different equality comparers, then assumption of uniqueness
            // will fail. So first check if other is a hashset using the same equality comparer;
            // symmetric except is a lot faster and avoids bit array allocations if we can assume
            // uniqueness
            if (other is ISet<T> otherAsSet && AreEqualityComparersEqual(this, otherAsSet))
            {
                SymmetricExceptWithUniqueHashSet(otherAsSet);
            }
            else
            {
                SymmetricExceptWithEnumerable(other);
            }
        }

        /// <summary>
        /// Determines whether a <see cref="HashSet{T}"/> object is a subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="HashSet{T}"/> object is a subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a subset of any other collection, including an empty set; therefore, this method returns
        /// <c>true</c> if the collection represented by the current <see cref="HashSet{T}"/> object is empty,
        /// even if the <paramref name="other"/> parameter is an empty set.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is greater than the number of
        /// elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="HashSet{T}"/> collection with the same
        /// equality comparer as the current <see cref="HashSet{T}"/> object, this method is an O(<c>n</c>) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c>
        /// is the number of elements in other.
        /// </remarks>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // The empty set is a subset of any set
            if (_count == 0)
            {
                return true;
            }

            // Set is always a subset of itself
            if (other == this)
            {
                return true;
            }

            ICollection<T>? otherAsCollection = other as ICollection<T>;
            // faster if other has unique elements according to this equality comparer; so check
            // that other is a hashset using the same equality comparer.
            if (otherAsCollection != null && otherAsCollection is ISet<T> otherAsSet && AreEqualityComparersEqual(this, otherAsSet))
            {
                // if this has more elements then it can't be a subset
                if (_count > otherAsCollection.Count)
                {
                    return false;
                }

                // already checked that we're using same equality comparer. simply check that
                // each element in this is contained in other.
                return IsSubsetOfHashSetWithSameEC(otherAsSet);
            }
            else
            {
                ElementCount result = CheckUniqueAndUnfoundElements(other, false);
                return (result.uniqueCount == _count && result.unfoundCount >= 0);
            }
        }

        /// <summary>
        /// Determines whether a <see cref="HashSet{T}"/> object is a proper subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="HashSet{T}"/> object is a proper subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a proper subset of any other collection. Therefore, this method returns <c>true</c> if the
        /// collection represented by the current <see cref="HashSet{T}"/> object is empty unless the other
        /// parameter is also an empty set.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is greater than or equal to the number of
        /// elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="HashSet{T}"/> collection with the same equality
        /// comparer as the current <see cref="HashSet{T}"/> object, then this method is an O(n) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c> is the
        /// number of elements in other.
        /// </remarks>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // no set is a proper subset of itself.
            if (other == this)
            {
                return false;
            }

            ICollection<T>? otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null)
            {
                // no set is a proper subset of an empty set
                if (otherAsCollection.Count == 0)
                {
                    return false;
                }

                // the empty set is a proper subset of anything but the empty set
                if (_count == 0)
                {
                    return otherAsCollection.Count > 0;
                }
                // faster if other is a hashset (and we're using same equality comparer)
                if (otherAsCollection is ISet<T> otherAsSet && AreEqualityComparersEqual(this, otherAsSet))
                {
                    if (_count >= otherAsCollection.Count)
                    {
                        return false;
                    }
                    // this has strictly less than number of items in other, so the following
                    // check suffices for proper subset.
                    return IsSubsetOfHashSetWithSameEC(otherAsSet);
                }
            }

            ElementCount result = CheckUniqueAndUnfoundElements(other, false);
            return (result.uniqueCount == _count && result.unfoundCount > 0);
        }

        /// <summary>
        /// Determines whether a <see cref="HashSet{T}"/> object is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="HashSet{T}"/> object is a superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// All collections, including the empty set, are supersets of the empty set. Therefore, this method returns
        /// <c>true</c> if the collection represented by the other parameter is empty, even if the current
        /// <see cref="HashSet{T}"/> object is empty.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is less than the number of elements
        /// in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="HashSet{T}"/> collection with the same
        /// equality comparer as the current <see cref="HashSet{T}"/> object, this method is an O(<c>n</c>) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other
        /// and <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // a set is always a superset of itself
            if (other == this)
            {
                return true;
            }

            // try to fall out early based on counts
            ICollection<T>? otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null)
            {
                // if other is the empty set then this is a superset
                if (otherAsCollection.Count == 0)
                {
                    return true;
                }

                // try to compare based on counts alone if other is a hashset with
                // same equality comparer
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    if (otherAsCollection.Count > _count)
                    {
                        return false;
                    }
                }
            }

            return ContainsAllElements(other);
        }

        /// <summary>
        /// Determines whether a <see cref="HashSet{T}"/> object is a proper superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="HashSet{T}"/> object is a proper superset of other; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a proper superset of any other collection. Therefore, this method returns <c>true</c> if the
        /// collection represented by the other parameter is empty unless the current <see cref="HashSet{T}"/> collection is also empty.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is less than or equal to the number of elements in other.
        /// <para/>
        /// If the collection represented by other is a <see cref="HashSet{T}"/> collection with the same equality
        /// comparer as the current <see cref="HashSet{T}"/> object, this method is an O(<c>n</c>) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other and <c>m</c>
        /// is <see cref="Count"/>.
        /// </remarks>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // the empty set isn't a proper superset of any set.
            if (_count == 0)
            {
                return false;
            }

            // a set is never a strict superset of itself
            if (other == this)
            {
                return false;
            }

            ICollection<T>? otherAsCollection = other as ICollection<T>;
            if (otherAsCollection != null)
            {
                // if other is the empty set then this is a superset
                if (otherAsCollection.Count == 0)
                {
                    // note that this has at least one element, based on above check
                    return true;
                }
                // faster if other is a hashset with the same equality comparer
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    if (otherAsCollection.Count >= _count)
                    {
                        return false;
                    }
                    // now perform element check
                    return ContainsAllElements(otherAsCollection);
                }
            }
            // couldn't fall out in the above cases; do it the long way
            ElementCount result = CheckUniqueAndUnfoundElements(other, true);
            return (result.uniqueCount < _count && result.unfoundCount == 0);
        }

        /// <summary>
        /// Determines whether the current <see cref="HashSet{T}"/> object and a specified collection
        /// share common elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="HashSet{T}"/> object and <paramref name="other"/> share
        /// at least one common element; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in other.
        /// </remarks>
        public bool Overlaps(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (_count == 0)
            {
                return false;
            }

            // set overlaps itself
            if (other == this)
            {
                return true;
            }

            foreach (T element in other)
            {
                if (Contains(element))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether a <see cref="HashSet{T}"/> object and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="HashSet{T}"/> object is equal to <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="SetEquals(IEnumerable{T})"/> method ignores duplicate entries and the order of elements in the
        /// <paramref name="other"/> parameter.
        /// <para/>
        /// If the collection represented by other is a <see cref="HashSet{T}"/> collection with the same equality
        /// comparer as the current <see cref="HashSet{T}"/> object, this method is an O(<c>n</c>) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other and
        /// <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // a set is equal to itself
            if (other == this)
            {
                return true;
            }

            ICollection<T>? otherAsCollection = other as ICollection<T>;
            // faster if other is a hashset and we're using same equality comparer
            if (otherAsCollection != null && AreEqualityComparersEqual(this, otherAsCollection))
            {
                // attempt to return early: since both contain unique elements, if they have
                // different counts, then they can't be equal
                if (_count != otherAsCollection.Count)
                {
                    return false;
                }

                // already confirmed that the sets have the same number of distinct elements, so if
                // one is a superset of the other then they must be equal
                return ContainsAllElements(otherAsCollection);
            }
            else
            {
                if (otherAsCollection != null)
                {
                    // if this count is 0 but other contains at least one element, they can't be equal
                    if (_count == 0 && otherAsCollection.Count > 0)
                    {
                        return false;
                    }
                }
                ElementCount result = CheckUniqueAndUnfoundElements(other, true);
                return (result.uniqueCount == _count && result.unfoundCount == 0);
            }
        }

        /// <summary>
        /// Copies the elements of a <see cref="OrderedHashSet{T}"/> object to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="OrderedHashSet{T}"/> object.
        /// The array must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0, _count);
        }

        /// <summary>
        /// Copies the elements of a <see cref="OrderedHashSet{T}"/> object to an array,
        /// starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="OrderedHashSet{T}"/> object.
        /// The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="arrayIndex"/> is greater than the length of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex, _count);
        }

        /// <summary>
        /// Copies the specified number of elements of a <see cref="OrderedHashSet{T}"/>
        /// object to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="OrderedHashSet{T}"/> object.
        /// The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy to <paramref name="array"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="arrayIndex"/> is greater than the length of the destination <paramref name="array"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is greater than the available space from the <paramref name="arrayIndex"/>
        /// to the end of the destination <paramref name="array"/>.
        /// </exception>
        /// <remarks>
        /// Calling this method is an O(<c>n</c>) operation, where <c>n</c> is <paramref name="count"/>.
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);

            // check array index valid index into array
            if (arrayIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(arrayIndex, ExceptionArgument.arrayIndex);

            // also throw if count less than 0
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            // will array, starting at arrayIndex, be able to hold elements? Note: not
            // checking arrayIndex >= array.Length (consistency with list of allowing
            // count of 0; subsequent check takes care of the rest)
            if (arrayIndex > array.Length || count > array.Length - arrayIndex)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

            int numCopied = 0;
            Entry[]? entries = _entries;
            for (int i = 0; i < _count && numCopied < count; i++)
            {
                Debug.Assert(entries is not null);
                array[arrayIndex + numCopied] = entries![i].Value!; // 1st [!]: asserted above, 2nd [!]: allow null values
                numCopied++;
            }
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
            // This is only ever done for strings, so we can optimize it all away for value types.
            if (!typeof(T).IsValueType &&
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
            Debug.Assert(!forceNewHashCodes || !typeof(T).IsValueType, "Value types never rehash.");
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
            if (!typeof(T).IsValueType && forceNewHashCodes)
            {
                // Store the original randomized comparer instead of the non-randomized one.
                Debug.Assert(_comparer is NonRandomizedStringEqualityComparer);
                IEqualityComparer<T> comparer = _comparer = (IEqualityComparer<T>)((NonRandomizedStringEqualityComparer)_comparer!).GetUnderlyingEqualityComparer(); // [!]: asserted above
                Debug.Assert(_comparer is not null);
                Debug.Assert(_comparer is not NonRandomizedStringEqualityComparer);

                // Update all of the entries' hash codes based on the new comparer.
                for (int i = 0; i < count; i++)
                {
                    newEntries[i].HashCode = (uint)comparer.GetHashCode(newEntries[i].Value!);
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

        /// <summary>Returns an enumerator that iterates through the <see cref="OrderedHashSet{T}"/>.</summary>
        /// <returns>A <see cref="OrderedHashSet{T}.Enumerator"/> structure for the <see cref="OrderedHashSet{T}"/>.</returns>
        public Enumerator GetEnumerator() => new(this);

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            Count == 0 ? EnumerableHelpers.GetEmptyEnumerator<T>() :
            GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        /// <inheritdoc/>
        int IList<T>.IndexOf(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                Debug.Assert(_entries is not null);
                if (EqualityComparer<T>.Default.Equals(item, _entries![index].Value!)) // [!]: asserted above
                {
                    return index;
                }
            }

            return -1;
        }

        /// <inheritdoc/>
        void IList<T>.Insert(int index, T item) => Insert(index, item);

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) => Add(item);

        /// <inheritdoc/>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
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
                array[arrayIndex++] = entry.Value;
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

            if (array is T[] tarray)
            {
                ((ICollection<T>)this).CopyTo(tarray, index);
            }
            else
            {
                try
                {
                    object?[]? objects = array as object?[];
                    if (objects is null)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType(ExceptionArgument.array);
                    }

                    foreach (T t in this)
                    {
                        objects[index++] = t;
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
            if (value is not T t)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                return Count - 1;
            }

            Add(t);
            return Count - 1;
        }

        /// <inheritdoc/>
        bool IList.Contains(object? value) =>
            value is T t &&
            Contains(t);

        /// <inheritdoc/>
        int IList.IndexOf(object? value)
        {
            if (value is T t)
            {
                return ((IList<T>)this).IndexOf(t);
            }

            return -1;
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object? value)
        {
            if (value is not T t)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                return;
            }

            Insert(index, t);
        }

        /// <inheritdoc/>
        void IList.Remove(object? value)
        {
            if (value is T t)
            {
                ((ICollection<T>)this).Remove(t);
            }
        }

        #region Nested Structure: ElementCount

        /// <summary>Used for set checking operations (using enumerables) that rely on counting.</summary>
        private struct ElementCount
        {
            /// <summary>The count of unique elements found in the other collection.</summary>
            internal int uniqueCount;
            /// <summary>The count of elements in the other collection not found in this set.</summary>
            internal int unfoundCount;
        }

        #endregion Nested Structure: ElementCount

        #region Nested Structure: Entry

        /// <summary>Represents an element in the set.</summary>
        private struct Entry
        {
            /// <summary>The index of the next entry in the chain, or -1 if this is the last entry in the chain.</summary>
            public int Next;
            /// <summary>Cached hash code of <see cref="Value"/>.</summary>
            public uint HashCode;
            /// <summary>The value.</summary>
            [AllowNull, MaybeNull]
            public T Value;
        }

        #endregion Nested Structure: Entry

        #region Nested Structure: Enumerator

        /// <summary>Enumerates the elements of a <see cref="OrderedHashSet{T}"/>.</summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>
        {
            /// <summary>The set being enumerated.</summary>
            private readonly OrderedHashSet<T> _set;
            /// <summary>A snapshot of the set's version when enumeration began.</summary>
            private readonly int _version;
            /// <summary>The current index.</summary>
            private int _index;

            /// <summary>Initialize the enumerator.</summary>
            internal Enumerator(OrderedHashSet<T> set)
            {
                _set = set;
                _version = _set._version;
            }

            /// <inheritdoc/>
            [AllowNull]
            public T Current { get; private set; }

            /// <inheritdoc/>
            [AllowNull]
            readonly object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _set._count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }
                    return Current!; // [!]: allow null values
                }
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                OrderedHashSet<T> set = _set;

                if (_version != set._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                if (_index < set._count)
                {
                    Debug.Assert(set._entries is not null);
                    ref Entry entry = ref set._entries![_index]; // [!]: asserted above
                    Current = entry.Value;
                    _index++;
                    return true;
                }

                _index = set._count + 1;
                Current = default;
                return false;
            }

            /// <inheritdoc/>
            void IEnumerator.Reset()
            {
                if (_version != _set._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                _index = 0;
                Current = default;
            }

            /// <inheritdoc/>
            readonly void IDisposable.Dispose() { }
        }

        #endregion Nested Structure: Enumerator

        #region Helper methods

        /// <summary>
        /// Implementation Notes:
        /// If other is a set and is using same equality comparer, then checking subset is
        /// faster. Simply check that each element in this is in other.
        ///
        /// Note: if other doesn't use same equality comparer, then Contains check is invalid,
        /// which is why callers must take care of this.
        ///
        /// If callers are concerned about whether this is a proper subset, they take care of that.
        /// </summary>
        internal bool IsSubsetOfHashSetWithSameEC(ISet<T> other)
        {
            foreach (T item in this)
            {
                if (!other.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// If other is a set that uses same equality comparer, intersect is much faster
        /// because we can use other's Contains
        /// </summary>
        private void IntersectWithHashSetWithSameEC(ISet<T> other)
        {
            Entry[]? entries = _entries;
            int count = _count;
            for (int i = count - 1; i >= 0; i--)
            {
                ref Entry entry = ref entries![i];
                T item = entry.Value;
                if (!other.Contains(item))
                {
                    Remove(item);
                }
            }
        }

        /// <summary>
        /// Iterate over other. If contained in this, mark an element in bit array corresponding to
        /// its position in entries. If anything is unmarked (in bit array), remove it.
        ///
        /// This attempts to allocate on the stack, if below StackAllocThreshold.
        /// </summary>
        private unsafe void IntersectWithEnumerable(IEnumerable<T> other)
        {
            Debug.Assert(_buckets != null, "_buckets shouldn't be null; callers should check first");

            // Keep track of current count; don't want to move past the end of our entries array
            // (could happen if another thread is modifying the collection).
            int originalCount = _count;
            int intArrayLength = BitHelper.ToIntArrayLength(originalCount);

            Span<int> span = stackalloc int[StackAllocThreshold];
            BitHelper bitHelper = intArrayLength <= StackAllocThreshold ?
                new BitHelper(span.Slice(0, intArrayLength), clear: true) :
                new BitHelper(new int[intArrayLength], clear: false);

            // Mark if contains: find index of in entries array and mark corresponding element in bit array.
            foreach (T item in other)
            {
                int index = IndexOf(item);
                if (index >= 0)
                {
                    bitHelper.MarkBit(index);
                }
            }

            // If anything unmarked, remove it. Perf can be optimized here if BitHelper had a
            // FindFirstUnmarked method.
            for (int i = originalCount - 1; i >= 0; i--)
            {
                ref Entry entry = ref _entries![i];
                if (!bitHelper.IsMarked(i))
                {
                    Remove(entry.Value);
                }
            }
        }

        /// <summary>
        /// If other is a set, we can assume it doesn't have duplicate elements, so use this
        /// technique: if can't remove, then it wasn't present in this set, so add.
        ///
        /// As with other methods, callers take care of ensuring that other is a set using the
        /// same equality comparer.
        /// </summary>
        private void SymmetricExceptWithUniqueHashSet(ISet<T> other)
        {
            foreach (T item in other)
            {
                if (!Remove(item))
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Implementation notes:
        ///
        /// Used for symmetric except when other isn't a set. This is more tedious because
        /// other may contain duplicates. Set technique could fail in these situations:
        /// 1. Other has a duplicate that's not in this: Set technique would add then remove it.
        /// 2. Other has a duplicate that's in this: Set technique would remove then add it back.
        /// In general, its presence would be toggled each time it appears in other.
        ///
        /// This technique uses bit marking to indicate whether to add/remove the item. If already
        /// present in collection, it will get marked for deletion. If added from other, it will
        /// get marked as something not to remove.
        /// </summary>
        private unsafe void SymmetricExceptWithEnumerable(IEnumerable<T> other)
        {
            int originalCount = _count;
            int intArrayLength = BitHelper.ToIntArrayLength(originalCount);

            Span<int> itemsToRemoveSpan = stackalloc int[StackAllocThreshold / 2];
            BitHelper itemsToRemove = intArrayLength <= StackAllocThreshold / 2 ?
                new BitHelper(itemsToRemoveSpan.Slice(0, intArrayLength), clear: true) :
                new BitHelper(new int[intArrayLength], clear: false);

            Span<int> itemsAddedFromOtherSpan = stackalloc int[StackAllocThreshold / 2];
            BitHelper itemsAddedFromOther = intArrayLength <= StackAllocThreshold / 2 ?
                new BitHelper(itemsAddedFromOtherSpan.Slice(0, intArrayLength), clear: true) :
                new BitHelper(new int[intArrayLength], clear: false);

            foreach (T item in other)
            {
                if (Add(item))
                {
                    // wasn't already present in collection; flag it as something not to remove
                    // *NOTE* if location is out of range, we should ignore. BitHelper will
                    // detect that it's out of bounds and not try to mark it. But it's
                    // expected that location could be out of bounds because adding the item
                    // will increase _count as soon as all the free spots are filled.
                    int location = IndexOf(item);
                    if (location >= 0)
                    {
                        itemsAddedFromOther.MarkBit(location);
                    }
                }
                else
                {
                    // already there...if not added from other, mark for remove.
                    // *NOTE* Even though BitHelper will check that location is in range, we want
                    // to check here. There's no point in checking items beyond originalCount
                    // because they could not have been in the original collection
                    int location = IndexOf(item);
                    if (location < originalCount && !itemsAddedFromOther.IsMarked(location))
                    {
                        itemsToRemove.MarkBit(location);
                    }
                }
            }

            // if anything marked, remove it
            for (int i = originalCount - 1; i >= 0; i--)
            {
                if (itemsToRemove.IsMarked(i))
                {
                    Remove(_entries![i].Value);
                }
            }
        }

        /// <summary>
        /// Determines counts that can be used to determine equality, subset, and superset. This
        /// is only used when other is an IEnumerable and not a set. If other is a set
        /// these properties can be checked faster without use of marking because we can assume
        /// other has no duplicates.
        ///
        /// The following count checks are performed by callers:
        /// 1. Equals: checks if unfoundCount = 0 and uniqueFoundCount = _count; i.e. everything
        /// in other is in this and everything in this is in other
        /// 2. Subset: checks if unfoundCount >= 0 and uniqueFoundCount = _count; i.e. other may
        /// have elements not in this and everything in this is in other
        /// 3. Proper subset: checks if unfoundCount > 0 and uniqueFoundCount = _count; i.e
        /// other must have at least one element not in this and everything in this is in other
        /// 4. Proper superset: checks if unfound count = 0 and uniqueFoundCount strictly less
        /// than _count; i.e. everything in other was in this and this had at least one element
        /// not contained in other.
        ///
        /// An earlier implementation used delegates to perform these checks rather than returning
        /// an ElementCount struct; however this was changed due to the perf overhead of delegates.
        /// </summary>
        private ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
        {
            ElementCount result;

            // Need special case in case this has no elements.
            if (_count == 0)
            {
                int numElementsInOther = 0;
                foreach (T item in other)
                {
                    numElementsInOther++;
                    break; // break right away, all we want to know is whether other has 0 or 1 elements
                }

                result.uniqueCount = 0;
                result.unfoundCount = numElementsInOther;
                return result;
            }

            Debug.Assert((_buckets != null) && (_count > 0), "_buckets was null but count greater than 0");

            int originalCount = _count;
            int intArrayLength = BitHelper.ToIntArrayLength(originalCount);

            Span<int> span = stackalloc int[StackAllocThreshold];
            BitHelper bitHelper = intArrayLength <= StackAllocThreshold ?
                new BitHelper(span.Slice(0, intArrayLength), clear: true) :
                new BitHelper(new int[intArrayLength], clear: false);

            int unfoundCount = 0; // count of items in other not found in this
            int uniqueFoundCount = 0; // count of unique items in other found in this

            foreach (T item in other)
            {
                int index = IndexOf(item);
                if (index >= 0)
                {
                    if (!bitHelper.IsMarked(index))
                    {
                        // Item hasn't been seen yet.
                        bitHelper.MarkBit(index);
                        uniqueFoundCount++;
                    }
                }
                else
                {
                    unfoundCount++;
                    if (returnIfUnfound)
                    {
                        break;
                    }
                }
            }

            result.uniqueCount = uniqueFoundCount;
            result.unfoundCount = unfoundCount;
            return result;
        }

        /// <summary>
        /// Checks whether all elements of <paramref name="other"/> are contained in this set.
        /// </summary>
        private bool ContainsAllElements(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                if (!Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AreEqualityComparersEqual(OrderedHashSet<T> set1, IEnumerable<T> set2)
            => EqualityComparerHelper.AreSetEqualityComparersEqual(set1.EqualityComparer, set2);

        /// <summary>
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        private static bool AreEqualityComparersEqual(OrderedHashSet<T> set1, OrderedHashSet<T> set2)
            => set1.EqualityComparer.Equals(set2.EqualityComparer);

        /// <summary>
        /// Checks if effective equality comparers are equal. This is used for algorithms that
        /// require that both collections use identical hashing implementations for their entries.
        /// </summary>
        internal static bool EffectiveEqualityComparersAreEqual(OrderedHashSet<T> set1, OrderedHashSet<T> set2)
            => set1.EffectiveComparer.Equals(set2.EffectiveComparer);

        #endregion Helper methods

        #region Structural Equality

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current set
        /// using rules provided by the specified <paramref name="comparer"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to determine
        /// whether the current object and <paramref name="other"/> are structurally equal.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is structurally equal to the current set;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public virtual bool Equals(object? other, IEqualityComparer comparer)
            => SetEqualityComparer<T>.Equals(this, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current set using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current set.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
        {
            // J2N: Fast path for default comparer - use cached value if valid
            if (comparer is SetEqualityComparer<T> setComparer &&
                Equals(setComparer, SetEqualityComparer<T>.Default))
            {
                // Check if cached hash code is still valid
                if (_hashCodeVersion == _version)
                    return _cachedHashCode;

                // J2N: Calculate hash code locally instead of delegating
                int hashCode = 0;
                foreach (T element in this)
                {
                    hashCode += element?.GetHashCode() ?? 0;
                }

                // Cache the result
                _cachedHashCode = hashCode;
                _hashCodeVersion = _version;
                return hashCode;
            }

            // J2N: Fall back to SetEqualityComparer for special cases:
            // - Nested array types (using structural equality)
            // - "Aggressive" mode for nested BCL collection types
            return SetEqualityComparer<T>.GetHashCode(this, comparer);
        }

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current set
        /// using rules similar to those in the JDK's AbstractSet class. Two sets are considered
        /// equal when they both contain the same objects (in any order).
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="ISet{T}"/>
        /// and it contains the same elements; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
        {
            // J2N: Fast path for same-type comparison - if obj is ISet<T> with same equality comparer,
            // use hash-based lookups instead of the slower SetEqualityComparer (O(n) vs O(n²))
            if (obj is ISet<T> other && AreEqualityComparersEqual(this, other))
            {
                if (_count != other.Count)
                    return false;
                return ContainsAllElements(other);
            }

            // J2N: Fall back to SetEqualityComparer for special cases:
            // - Nested array types (using structural equality)
            // - "Aggressive" mode for nested BCL collection types
            // - Cross-type set comparisons
            return Equals(obj, SetEqualityComparer<T>.Default);
        }

        /// <summary>
        /// Gets the hash code for the current set. The hash code is calculated
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <seealso cref="GetHashCode(IEqualityComparer)"/>
        public override int GetHashCode()
            => GetHashCode(SetEqualityComparer<T>.Default);

        #endregion Structural Equality

        #region ToString

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
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
        /// Returns a string that represents the current set using
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
        /// Returns a string that represents the current set using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by curly
        /// brackets ("{}"). Keys and values are separated by '=',
        /// KeyValuePairs are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
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
    }
}
