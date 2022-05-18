#region Copyright 2012-2014 by Roger Knapp, Licensed under the Apache License, Version 2.0
/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion
using J2N.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;


namespace J2N.Collections.Concurrent
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Defines if and how items added to a LurchTable are linked together, this defines
    /// the value returned from Peek/Dequeue as the oldest entry of the specified operation.
    /// </summary>
    public enum LurchTableOrder
    {
        /// <summary> No linking </summary>
        None,
        /// <summary> Linked in insertion order </summary>
        Insertion,
        /// <summary> Linked by most recently inserted or updated </summary>
        Modified,
        /// <summary> Linked by most recently inserted, updated, or fetched </summary>
        Access,
    }

    /// <summary>
    /// LurchTable stands for "Least Used Recently Concurrent Hash Table" and has definite
    /// similarities to both the .NET 4 ConcurrentDictionary as well as Java's LinkedHashMap.
    /// This gives you a thread-safe dictionary/hashtable that stores element ordering by
    /// insertion, updates, or access.  In addition it can be configured to use a 'hard-limit'
    /// count of items that will automatically 'pop' the oldest item in the collection.
    /// <para/>
    /// Usage Note: Passing <see cref="LurchTableOrder.Access"/> to the constructor is similar
    /// to passing <c>true</c> to the <c>accessOrder</c> parameter of Java's LinkedHashMap. It allows
    /// <see cref="LurchTable{TKey, TValue}"/> to be used as an LRU cache. However, unlike
    /// LinkedHashMap, this implementation is thread-safe.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    [SuppressMessage("Style", "IDE0034:Simplify 'default' expression", Justification = "Following Microsoft's code styles")]
    [SuppressMessage("Style", "IDE0019:Use pattern matching", Justification = "Using Microsoft's code styles")]
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
#pragma warning disable IDE0079 // Remove unnecessary supppression
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    public class LurchTable<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IDisposable
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary supppression
    {
        /// <summary> Method signature for the ItemUpdated event </summary>
        public delegate void ItemUpdatedMethod(KeyValuePair<TKey, TValue> previous, KeyValuePair<TKey, TValue> next);

        /// <summary> Event raised after an item is removed from the collection </summary>
        public event Action<KeyValuePair<TKey, TValue>>? ItemRemoved;
        /// <summary> Event raised after an item is updated in the collection </summary>
        public event ItemUpdatedMethod? ItemUpdated;
        /// <summary> Event raised after an item is added to the collection </summary>
        public event Action<KeyValuePair<TKey, TValue>>? ItemAdded;

        private const int OverAlloc = 128;
        private const int FreeSlots = 32;

        private readonly IEqualityComparer<TKey> _comparer;
        private readonly int _hsize, _lsize;
        private int _limit; // J2N: Changed to read-write
        private readonly int _allocSize, _shift, _shiftMask;
        private readonly LurchTableOrder _ordering;
        private readonly object[] _locks;
        private readonly int[] _buckets;
        private readonly FreeList[] _free;

        private Entry[][] _entries;
        private int _used, _count;
        private int _allocNext, _freeVersion;

        private object? _syncRoot;

        #region Constructors

        /// <summary>Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/>.</summary>
        public LurchTable()
            : this(0, LurchTableOrder.None, int.MaxValue, null) { }

        /// <summary>Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> that can store up to
        /// <paramref name="capacity"/> items efficiently.</summary>
        /// <param name="capacity">The initial allowable number of items before allocation of more memory.</param>
        public LurchTable(int capacity)
            : this(capacity, LurchTableOrder.None, int.MaxValue, null) { }

        /// <summary>Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> that can store up to
        /// <paramref name="capacity"/> items efficiently.</summary>
        /// <param name="capacity">The initial allowable number of items before allocation of more memory.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        public LurchTable(int capacity, LurchTableOrder ordering)
            : this(capacity, ordering, int.MaxValue, null) { }

        /// <summary>Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> that can store up to
        /// <paramref name="capacity"/> items efficiently with the specified <paramref name="comparer"/>.</summary>
        /// <param name="capacity">The initial allowable number of items before allocation of more memory.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="comparer">The element hash generator for keys, or <c>null</c> to use <see cref="J2N.Collections.Generic.EqualityComparer{TKey}.Default"/>.</param>
        public LurchTable(int capacity, LurchTableOrder ordering, IEqualityComparer<TKey> comparer)
            : this(capacity, ordering, int.MaxValue, comparer) { }

        /// <summary>Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> with the specified <paramref name="comparer"/>.</summary>
        /// <param name="comparer">The element hash generator for keys, or <c>null</c> to use <see cref="J2N.Collections.Generic.EqualityComparer{TKey}.Default"/>.</param>
        public LurchTable(IEqualityComparer<TKey> comparer)
            : this(0, LurchTableOrder.None, int.MaxValue, comparer) { }

        /// <summary>Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> that can store up to
        /// <paramref name="capacity"/> items efficiently with the specified <paramref name="comparer"/>.</summary>
        /// <param name="capacity">The initial allowable number of items before allocation of more memory.</param>
        /// <param name="comparer">The element hash generator for keys, or <c>null</c> to use <see cref="J2N.Collections.Generic.EqualityComparer{TKey}.Default"/>.</param>
        public LurchTable(int capacity, IEqualityComparer<TKey>? comparer)
            : this(capacity, LurchTableOrder.None, int.MaxValue, comparer) { }

        /// <summary>Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> that orders items by
        /// <paramref name="ordering"/> and removes items once the specified <paramref name="limit"/> is reached.</summary>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="limit">The maximum allowable number of items, or <see cref="int.MaxValue"/> for unlimited.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="limit"/> is less than 1.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="ordering"/> is <see cref="LurchTableOrder.None"/> and <paramref name="limit"/> is less than <see cref="int.MaxValue"/>.
        /// </exception>
        public LurchTable(LurchTableOrder ordering, int limit)
            : this(limit, ordering, limit, null) { }

        /// <summary>Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> that orders items by
        /// <paramref name="ordering"/> and removes items once the specified <paramref name="limit"/> is reached.</summary>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="limit">The maximum allowable number of items, or <see cref="int.MaxValue"/> for unlimited.</param>
        /// <param name="comparer">The element hash generator for keys, or <c>null</c> to use <see cref="J2N.Collections.Generic.EqualityComparer{TKey}.Default"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="limit"/> is less than 1.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="ordering"/> is <see cref="LurchTableOrder.None"/> and <paramref name="limit"/> is less than <see cref="int.MaxValue"/>.
        /// </exception>
        public LurchTable(LurchTableOrder ordering, int limit, IEqualityComparer<TKey>? comparer)
            : this(limit, ordering, limit, comparer) { }

        /// <summary>
        /// Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> that can store up to
        /// <paramref name="capacity"/> items efficiently, orders items by
        /// <paramref name="ordering"/> and removes items once the specified <paramref name="limit"/> is reached.
        /// </summary>
        /// <param name="capacity">The initial allowable number of items before allocation of more memory.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="limit">The maximum allowable number of items, or <see cref="int.MaxValue"/> for unlimited.</param>
        /// <param name="comparer">The element hash generator for keys, or <c>null</c> to use <see cref="J2N.Collections.Generic.EqualityComparer{TKey}.Default"/></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="limit"/> is less than 1.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="ordering"/> is <see cref="LurchTableOrder.None"/> and <paramref name="limit"/> is less than <see cref="int.MaxValue"/>.
        /// </exception>
        public LurchTable(int capacity, LurchTableOrder ordering, int limit, IEqualityComparer<TKey>? comparer)
            : this(capacity, ordering, limit, capacity >> 1, capacity >> 4, capacity >> 8, comparer) { }

        /// <summary>
        /// Initializes a new instance of <see cref="LurchTable{TKey, TValue}"/> that orders items by
        /// <paramref name="ordering"/> and removes items once the specified <paramref name="limit"/> is reached.
        /// </summary>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="limit">The maximum allowable number of items, or <see cref="int.MaxValue"/> for unlimited.</param>
        /// <param name="hashSize">The number of hash buckets to use for the collection, usually 1/2 estimated capacity.</param>
        /// <param name="allocSize">The number of entries to allocate at a time, usually 1/16 estimated capacity.</param>
        /// <param name="lockSize">The number of concurrency locks to preallocate, usually 1/256 estimated capacity.</param>
        /// <param name="comparer">The element hash generator for keys, or <c>null</c> to use <see cref="J2N.Collections.Generic.EqualityComparer{TKey}.Default"/></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="limit"/> is less than 1.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="ordering"/> is <see cref="LurchTableOrder.None"/> and <paramref name="limit"/> is less than <see cref="int.MaxValue"/>.
        /// </exception>
        // J2N: Original constructor still used by tests. This constructor didn't provide a straightforward way to put a guard clause on
        // capacity to ensure it is non-negative.
        internal LurchTable(LurchTableOrder ordering, int limit, int hashSize, int allocSize, int lockSize, IEqualityComparer<TKey>? comparer)
            : this(hashSize << 1, ordering, limit, hashSize, allocSize, lockSize, comparer) { }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private LurchTable(int capacity, LurchTableOrder ordering, int limit, int hashSize, int allocSize, int lockSize, IEqualityComparer<TKey>? comparer)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), SR.ArgumentOutOfRange_NeedLimitAtLeast1);
            if (ordering == LurchTableOrder.None && limit < int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(limit), SR.LurchTable_NeedLimitIntMaxValue);
            if (hashSize < 0)
                throw new ArgumentOutOfRangeException(nameof(hashSize), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (allocSize < 0)
                throw new ArgumentOutOfRangeException(nameof(allocSize), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (lockSize < 0)
                throw new ArgumentOutOfRangeException(nameof(lockSize), SR.ArgumentOutOfRange_NeedNonNegNum);

            _limit = limit;
            _comparer = comparer ?? J2N.Collections.Generic.EqualityComparer<TKey>.Default;
            _ordering = ordering;

            allocSize = (int)Math.Min((long)allocSize + OverAlloc, 0x3fffffff);
            //last power of 2 that is less than allocSize
            for (_shift = 7; _shift < 24 && (1 << (_shift + 1)) < allocSize; _shift++) { }
            _allocSize = 1 << _shift;
            _shiftMask = _allocSize - 1;

            _hsize = HashHelpers.GetPrime(Math.Max(127, hashSize));
            _buckets = new int[_hsize];

            _lsize = HashHelpers.GetPrime(lockSize);
            _locks = new object[_lsize];
            for (int i = 0; i < _lsize; i++)
                _locks[i] = new object();

            _free = new FreeList[FreeSlots];
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements
        /// copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the default equality comparer
        /// for the key type.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the
        /// new <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the default equality
        /// comparer; likewise, every key in the source <paramref name="dictionary"/> must also be unique according to the default
        /// equality comparer.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the
        /// elements in <paramref name="dictionary"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// This constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>. If type
        /// <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in <paramref name="dictionary"/>.
        /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary supppression
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public LurchTable(IDictionary<TKey, TValue> dictionary) : this(dictionary, LurchTableOrder.None, null) { }
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary supppression

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements
        /// copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the default equality comparer
        /// for the key type.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the
        /// new <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the default equality
        /// comparer; likewise, every key in the source <paramref name="dictionary"/> must also be unique according to the default
        /// equality comparer.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the
        /// elements in <paramref name="dictionary"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// This constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>. If type
        /// <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in <paramref name="dictionary"/>.
        /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary supppression
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public LurchTable(IDictionary<TKey, TValue> dictionary, LurchTableOrder ordering) : this(dictionary, ordering, null) { }
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary supppression

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        /// <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c>
        /// to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// likewise, every key in the source <paramref name="dictionary"/> must also be unique according to the specified comparer.
        /// <para/>
        /// NOTE: For example, duplicate keys can occur if <paramref name="comparer"/> is one of the case-insensitive string
        /// comparers provided by the <see cref="StringComparer"/> class and <paramref name="dictionary"/> does not use a
        /// case-insensitive comparer key.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the elements in
        /// <paramref name="dictionary"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is <c>null</c>, this constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="dictionary"/>.
        /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary supppression
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public LurchTable(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) : this(dictionary, LurchTableOrder.None, comparer) { }
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary supppression

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        /// <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c>
        /// to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// likewise, every key in the source <paramref name="dictionary"/> must also be unique according to the specified comparer.
        /// <para/>
        /// NOTE: For example, duplicate keys can occur if <paramref name="comparer"/> is one of the case-insensitive string
        /// comparers provided by the <see cref="StringComparer"/> class and <paramref name="dictionary"/> does not use a
        /// case-insensitive comparer key.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the elements in
        /// <paramref name="dictionary"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is <c>null</c>, this constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="dictionary"/>.
        /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary supppression
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public LurchTable(IDictionary<TKey, TValue> dictionary, LurchTableOrder ordering, IEqualityComparer<TKey>? comparer)
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary supppression
            : this(dictionary, ordering, int.MaxValue, comparer) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        /// <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="limit">The maximum allowable number of items, or <see cref="int.MaxValue"/> for unlimited.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c>
        /// to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// likewise, every key in the source <paramref name="dictionary"/> must also be unique according to the specified comparer.
        /// <para/>
        /// NOTE: For example, duplicate keys can occur if <paramref name="comparer"/> is one of the case-insensitive string
        /// comparers provided by the <see cref="StringComparer"/> class and <paramref name="dictionary"/> does not use a
        /// case-insensitive comparer key.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the elements in
        /// <paramref name="dictionary"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is <c>null</c>, this constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="dictionary"/>.
        /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary supppression
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public LurchTable(IDictionary<TKey, TValue> dictionary, LurchTableOrder ordering, int limit, IEqualityComparer<TKey>? comparer)
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary supppression
            : this(dictionary is null ? 0 : dictionary.Count, ordering, limit, comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            CopyConstructorAddRange(dictionary);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements
        /// copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the default equality comparer
        /// for the key type.
        /// </summary>
        /// <param name="collection">The <see cref="T:IEnumerable{KeyValuePair{TKey, TValue}}"/> whose elements are copied to the
        /// new <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Every key in a <see cref="LurchTable{TKey, TValue}"/> must be unique according to the default equality
        /// comparer; likewise, every key in the source <paramref name="collection"/> must also be unique according to the default
        /// equality comparer.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the
        /// elements in <paramref name="collection"/>.
        /// <para/>
        /// <see cref="LurchTable{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// This constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>. If type
        /// <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in <paramref name="collection"/>.
        /// </remarks>
        public LurchTable(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements
        /// copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the default equality comparer
        /// for the key type.
        /// </summary>
        /// <param name="collection">The <see cref="T:IEnumerable{KeyValuePair{TKey, TValue}}"/> whose elements are copied to the
        /// new <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Every key in a <see cref="LurchTable{TKey, TValue}"/> must be unique according to the default equality
        /// comparer; likewise, every key in the source <paramref name="collection"/> must also be unique according to the default
        /// equality comparer.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the
        /// elements in <paramref name="collection"/>.
        /// <para/>
        /// <see cref="LurchTable{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// This constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>. If type
        /// <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in <paramref name="collection"/>.
        /// </remarks>
        public LurchTable(IEnumerable<KeyValuePair<TKey, TValue>> collection, LurchTableOrder ordering) : this(collection, ordering, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="T:IEnumerable{KeyValuePair{TKey, TValue}}"/> whose elements are copied to the new
        /// <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c>
        /// to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// likewise, every key in the source <paramref name="collection"/> must also be unique according to the specified comparer.
        /// <para/>
        /// NOTE: For example, duplicate keys can occur if <paramref name="comparer"/> is one of the case-insensitive string
        /// comparers provided by the <see cref="StringComparer"/> class and <paramref name="collection"/> does not use a
        /// case-insensitive comparer key.
        /// <para/>
        /// The initial capacity of the new <see cref="LurchTable{TKey, TValue}"/> is large enough to contain all the elements in
        /// <paramref name="collection"/>.
        /// <para/>
        /// <see cref="LurchTable{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is <c>null</c>, this constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="collection"/>.
        /// </remarks>
        public LurchTable(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) : this(collection, LurchTableOrder.None, comparer) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="T:IEnumerable{KeyValuePair{TKey, TValue}}"/> whose elements are copied to the new
        /// <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c>
        /// to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// likewise, every key in the source <paramref name="collection"/> must also be unique according to the specified comparer.
        /// <para/>
        /// NOTE: For example, duplicate keys can occur if <paramref name="comparer"/> is one of the case-insensitive string
        /// comparers provided by the <see cref="StringComparer"/> class and <paramref name="collection"/> does not use a
        /// case-insensitive comparer key.
        /// <para/>
        /// The initial capacity of the new <see cref="LurchTable{TKey, TValue}"/> is large enough to contain all the elements in
        /// <paramref name="collection"/>.
        /// <para/>
        /// <see cref="LurchTable{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is <c>null</c>, this constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="collection"/>.
        /// </remarks>
        public LurchTable(IEnumerable<KeyValuePair<TKey, TValue>> collection, LurchTableOrder ordering, IEqualityComparer<TKey>? comparer)
            : this(collection, ordering, int.MaxValue, comparer) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LurchTable{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <paramref name="ordering"/>,
        /// <paramref name="limit"/> and <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="T:IEnumerable{KeyValuePair{TKey, TValue}}"/> whose elements are copied to the new
        /// <see cref="LurchTable{TKey, TValue}"/>.</param>
        /// <param name="ordering">The type of linking for the items.</param>
        /// <param name="limit">The maximum allowable number of items, or <see cref="int.MaxValue"/> for unlimited.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c>
        /// to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// likewise, every key in the source <paramref name="collection"/> must also be unique according to the specified comparer.
        /// <para/>
        /// NOTE: For example, duplicate keys can occur if <paramref name="comparer"/> is one of the case-insensitive string
        /// comparers provided by the <see cref="StringComparer"/> class and <paramref name="collection"/> does not use a
        /// case-insensitive comparer key.
        /// <para/>
        /// The initial capacity of the new <see cref="LurchTable{TKey, TValue}"/> is large enough to contain all the elements in
        /// <paramref name="collection"/>.
        /// <para/>
        /// <see cref="LurchTable{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is <c>null</c>, this constructor uses the default generic equality comparer, <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="collection"/>.
        /// </remarks>
        public LurchTable(IEnumerable<KeyValuePair<TKey, TValue>> collection, LurchTableOrder ordering, int limit, IEqualityComparer<TKey>? comparer)
            : this(collection is ICollection<KeyValuePair<TKey, TValue>> col ? col.Count : 0, ordering, limit, comparer)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            CopyConstructorAddRange(collection);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Clears references to all objects and invalidates the collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clears references to all objects and invalidates the collection.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _entries = null!;
                _used = _count = 0;
            }
        }

        #endregion

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="LurchTable{TKey,TValue}"/> is empty.
        /// </summary>
        /// <value><c>true</c> if the <see cref="LurchTable{TKey,TValue}"/> is empty; otherwise,
        /// <c>false</c>.</value>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Retrieves the <see cref="LurchTableOrder"/> ordering enumeration this instance was created with.
        /// </summary>
        public LurchTableOrder Ordering => _ordering;

        /// <summary>
        /// Retrives the key comparer being used by this instance.
        /// </summary>
        public IEqualityComparer<TKey> EqualityComparer => _comparer;

        /// <summary>
        /// Gets or sets the record limit allowed in this instance.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 1.
        /// <para/>
        /// -or-
        /// <para/>
        /// <see cref="Ordering"/> is <see cref="LurchTableOrder.None"/> and <paramref name="value"/> is less than <see cref="int.MaxValue"/>.
        /// </exception>
        public int Limit
        {
            get => _limit;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_NeedLimitAtLeast1);
                if (_ordering == LurchTableOrder.None && value < int.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), SR.LurchTable_NeedLimitIntMaxValue);

                Interlocked.Exchange(ref _limit, value);
            }
        }

        /// <summary>
        /// WARNING: not thread-safe, reinitializes all internal structures.  Use Clear() for a thread-safe
        /// delete all.  If you have externally provided exclusive access using <see cref="ICollection.SyncRoot"/>
        /// this method may be used to more efficiently clear the collection.
        /// </summary>
        /// <exception cref="LurchTableCorruptionException">If the data in the table is corrupted due to unsynchronized updates.</exception>
        public void Initialize()
        {
            lock (SyncRoot)
            {
                _freeVersion = _allocNext = 0;
                _count = 0;
                _used = 1;

                Array.Clear(_buckets, 0, _hsize);
                _entries = new[] { new Entry[_allocSize] };
                for (int slot = 0; slot < FreeSlots; slot++)
                {
                    var index = Interlocked.CompareExchange(ref _used, _used + 1, _used);
                    if (index != slot + 1)
                        throw new LurchTableCorruptionException();

                    _free[slot].Tail = index;
                    _free[slot].Head = index;
                }

                if (_count != 0 || _used != FreeSlots + 1)
                    throw new LurchTableCorruptionException();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="LurchTable{TKey, TValue}"/> contains an element with
        /// the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="LurchTable{TKey, TValue}"/>. The value can be <c>null</c>.</param>
        /// <returns><c>true</c> if the <see cref="LurchTable{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method determines equality using J2N's default equality comparer <see cref="J2N.Collections.Generic.EqualityComparer{T}.Default"/>
        /// for the value type <typeparamref name="TValue"/>.
        /// <para/>
        /// This method performs a linear search; therefore, the average execution time is proportional to the <see cref="Count"/> property.
        /// That is, this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue(TValue value)
        {
            // NOTE: We do this check here to override the .NET default equality comparer
            // with J2N's version
            return ContainsValue(value, J2N.Collections.Generic.EqualityComparer<TValue>.Default);
        }

        /// <summary>
        /// Determines whether the <see cref="LurchTable{TKey, TValue}"/> contains a specific value
        /// as determined by the provided <paramref name="valueComparer"/>.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="LurchTable{TKey, TValue}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <param name="valueComparer">The <see cref="IEqualityComparer{TValue}"/> to use
        /// to test each value for equality.</param>
        /// <returns><c>true</c> if the <see cref="LurchTable{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method performs a linear search; therefore, the average execution time
        /// is proportional to <see cref="Count"/>. That is, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue(TValue value, IEqualityComparer<TValue> valueComparer) // Overload added so end user can override J2N's equality comparer
        {
            if (value is null)
            {
                return InternalContainsValue((otherValue) =>
                {
                    if (otherValue is null)
                        return true;
                    return false;
                });
            }
            else
            {
                return InternalContainsValue((otherValue) =>
                {
                    if (valueComparer.Equals(otherValue, value))
                        return true;
                    return false;
                });
            }
        }

        #region IDictionary Members

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        ICollection IDictionary.Keys => (ICollection)Keys;

        ICollection IDictionary.Values => (ICollection)Values;

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="System.Collections.ICollection"/> is
        /// synchronized with the <see cref="SyncRoot"/>.
        /// </summary>
        /// <value><c>true</c> if access to the <see cref="System.Collections.ICollection"/> is synchronized
        /// (thread safe); otherwise, <c>false</c>. For <see
        /// cref="J2N.Collections.Concurrent.LurchTable{TKey,TValue}"/>, this property always
        /// returns <c>false</c>.</value>
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => SyncRoot;

        private object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<object?>(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        object? IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    if (TryGetValue((TKey)key, out TValue value))
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
                    TKey tempKey = (TKey)key;
                    try
                    {
                        this[tempKey!] = (TValue)value!;
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
                TKey tempKey = (TKey)key;

                try
                {
                    Add(tempKey!, (TValue)value!);
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
            return new Enumerator(this, Enumerator.DictEntry);
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

        private static bool IsCompatibleKey(object key)
        {
            if (key is null)
                return typeof(TKey).IsNullableType();

            return (key is TKey);
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Dispose()"/> has already been called.</exception>
        public void Clear()
        {
            if (_entries == null) throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));
            foreach (var item in this)
                Remove(item.Key);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <exception cref="ObjectDisposedException"><see cref="Dispose()"/> has already been called.</exception>
        public bool ContainsKey(TKey key)
        {
            if (_entries == null) throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));
            return TryGetValue(key, out TValue _);
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
        /// that does not exist in the <see cref="LurchTable{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c>. However, if the specified key already exists in
        /// the <see cref="LurchTable{TKey, TValue}"/>, setting the <see cref="this[TKey]"/> property overwrites
        /// the old value. In contrast, the <see cref="Add(TKey, TValue)"/> method does not modify existing elements.
        /// <para/>
        /// Both keys and values can be <c>null</c> if either <see cref="Nullable{T}"/> or a reference type.
        /// <para/>
        /// The C# language uses the <see cref="this"/> keyword to define the indexers instead of implementing the
        /// <c>Item[TKey]</c> property. Visual Basic implements <c>Item[TKey]</c> as a default property, which provides
        /// the same indexing functionality.
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out TValue value))
                    throw new KeyNotFoundException(J2N.SR.Format(SR.Arg_KeyNotFoundWithKey, key));
                return value;
            }
            set
            {
                var info = new AddInfo<TKey, TValue> { Value = value, CanUpdate = true };
                Insert(key, ref info);
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>
        /// contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue([AllowNull, MaybeNull] TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            int hash = GetHash(key);
            return InternalGetValue(hash, key, out value);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists
        /// in the <see cref="Dictionary{TKey, TValue}"/>.</exception>
        public void Add([AllowNull] TKey key, [AllowNull] TValue value)
        {
            var info = new AddInfo<TKey, TValue> { Value = value! };
            if (InsertResult.Inserted != Insert(key, ref info))
                throw new ArgumentException(J2N.SR.Format(SR.Argument_AddingDuplicate, key));
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if the element is successfully removed; otherwise, <c>false</c>.  This method also returns <c>false</c>
        /// if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public bool Remove(TKey key)
        {
            var del = new DelInfo<TKey, TValue>();
            return Delete(key, ref del);
        }

        #endregion

        #region IDictionaryEx<TKey,TValue> Members

        /// <summary>
        /// Adds a key/value pair to the  <see cref="T:System.Collections.Generic.IDictionary`2"/> if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value to be added, if the key does not already exist.</param>
        /// <returns>The value for the key. This will be either the existing value for the key if the key is already
        /// in the dictionary, or the new value if the key was not in the dictionary.</returns>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            var info = new AddInfo<TKey, TValue> { Value = value, CanUpdate = false };
            if (InsertResult.Exists == Insert(key, ref info))
                return info.Value;
            return value;
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <returns><c>true</c> if the value was successfully added; otherwise, <c>false</c>.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            var info = new AddInfo<TKey, TValue> { Value = value, CanUpdate = false };
            return InsertResult.Inserted == Insert(key, ref info);
        }

        /// <summary>
        /// Updates an element with the provided key to the value if it exists.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to update.</param>
        /// <param name="value">The new value for the key if found.</param>
        /// <returns>Returns <c>true</c> if the key provided was found and updated to the value; otherwise, <c>false</c>.</returns>
        public bool TryUpdate(TKey key, TValue value)
        {
            var info = new UpdateInfo<TKey, TValue> { Value = value };
            return InsertResult.Updated == Insert(key, ref info);
        }

        /// <summary>
        /// Updates an element with the provided key to the value if it exists.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to update.</param>
        /// <param name="value">The new value for the key if found.</param>
        /// <param name="comparisonValue">The value that is compared to the value of the element with key.</param>
        /// <returns>Returns <c>true</c> if the key provided was found and updated to the value.</returns>
        public bool TryUpdate(TKey key, TValue value, TValue comparisonValue)
        {
            var info = new UpdateInfo<TKey, TValue>(comparisonValue) { Value = value };
            return InsertResult.Updated == Insert(key, ref info);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value that was removed.</param>
        /// <returns>
        /// <c>true</c> if the element is successfully removed; otherwise, <c>false</c>. This method also returns
        /// <c>false</c> if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            var info = new DelInfo<TKey, TValue>();
            if (Delete(key, ref info))
            {
                value = info.Value;
                return true;
            }
            value = default;
            return false;
        }

        #endregion

        #region IConcurrentDictionary<TKey,TValue> Members

        /// <summary>
        /// Adds a key/value pair to the  <see cref="T:System.Collections.Generic.IDictionary`2"/> if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to get or add.</param>
        /// <param name="fnCreate">Constructs a new value for the key.</param>
        /// <returns>The value for the key. This will be either the existing value for the key if the key is already
        /// in the dictionary, or the new value if the key was not in the dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fnCreate"/> is <c>null</c>.</exception>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> fnCreate)
        {
            if (fnCreate == null)
                throw new ArgumentNullException(nameof(fnCreate));

            var info = new Add2Info<TKey, TValue> { Create = fnCreate };
            Insert(key, ref info);
            return info.Value;
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="T:System.Collections.Generic.IDictionary`2"/> if the key does not already exist, 
        /// or updates a key/value pair if the key already exists.
        /// </summary>
        /// <param name="key">The key of the element to add or update.</param>
        /// <param name="addValue">The value to add if a value doesn't already exist.</param>
        /// <param name="fnUpdate">The delegate to call to update the value with if it already exists.</param>
        public TValue AddOrUpdate(TKey key, TValue addValue, KeyValueUpdate<TKey, TValue> fnUpdate)
        {
            if (fnUpdate == null)
                throw new ArgumentNullException(nameof(fnUpdate));

            var info = new Add2Info<TKey, TValue>(addValue) { Update = fnUpdate };
            Insert(key, ref info);
            return info.Value;
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="T:System.Collections.Generic.IDictionary`2"/> if the key does not already exist, 
        /// or updates a key/value pair if the key already exists.
        /// </summary>
        /// <param name="key">The key of the element to add or update.</param>
        /// <param name="fnCreate">The delegate to call to add if a value doesn't already exist.</param>
        /// <param name="fnUpdate">The delegate to call to update the value with if it already exists.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fnCreate"/> is <c>null</c>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="fnUpdate"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// Adds or modifies an element with the provided key and value.  If the key does not exist in the collection,
        /// the factory method <paramref name="fnCreate"/> will be called to produce the new value, if the key exists, the converter method
        /// <paramref name="fnUpdate"/> will be called to create an updated value.
        /// </remarks>
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> fnCreate, KeyValueUpdate<TKey, TValue> fnUpdate)
        {
            if (fnCreate == null)
                throw new ArgumentNullException(nameof(fnCreate));
            if (fnUpdate == null)
                throw new ArgumentNullException(nameof(fnUpdate));

            var info = new Add2Info<TKey, TValue> { Create = fnCreate, Update = fnUpdate };
            Insert(key, ref info);
            return info.Value;
        }

        // J2N: Removed, since we don't want to expose the IRemoveValue interface publicly.
        // If required, we can add this back in at some point, but it is more likely we will
        // make additional delegate methods like ConcurrentDictionary has instead.
        ///// <summary>
        ///// Add, update, or fetch a key/value pair from the dictionary via an implementation of the
        ///// <see cref="T:CSharpTest.Net.Collections.ICreateOrUpdateValue`2"/> interface.
        ///// </summary>
        ///// <param name="key">The key of the element to add or update.</param>
        ///// <param name="createOrUpdateValue">An implementation of <see cref="ICreateOrUpdateValue{TKey, TValue}"/>
        ///// to use to create or update the value associated with <paramref name="key"/>.</param>
        ///// <exception cref="ArgumentNullException"><paramref name="createOrUpdateValue"/> is <c>null</c>.</exception>
        //public bool AddOrUpdate<T>(TKey key, ref T createOrUpdateValue) where T : ICreateOrUpdateValue<TKey, TValue>
        //{
        //    if (createOrUpdateValue == null)
        //        throw new ArgumentNullException(nameof(createOrUpdateValue));

        //    var result = Insert(key, ref createOrUpdateValue);
        //    return result == InsertResult.Inserted || result == InsertResult.Updated;
        //}

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>
        /// by calling the provided factory method to construct the value if the key is not already present in the collection.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="fnCreate">The delegate to call to add if a value doesn't already exist.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fnCreate"/> is <c>null</c>.</exception>
        public bool TryAdd(TKey key, Func<TKey, TValue> fnCreate)
        {
            if (fnCreate == null)
                throw new ArgumentNullException(nameof(fnCreate));

            var info = new Add2Info<TKey, TValue> { Create = fnCreate };
            return InsertResult.Inserted == Insert(key, ref info);
        }

        /// <summary>
        /// Modify the value associated with the result of the provided update method
        /// as an atomic operation, Allows for reading/writing a single record within
        /// the syncronization lock.
        /// </summary>
        /// <param name="key">The key of the element to update.</param>
        /// <param name="fnUpdate">The delegate to call to update the value with if it exists.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fnUpdate"/> is <c>null</c>.</exception>
        public bool TryUpdate(TKey key, KeyValueUpdate<TKey, TValue> fnUpdate)
        {
            if (fnUpdate == null)
                throw new ArgumentNullException(nameof(fnUpdate));

            var info = new Add2Info<TKey, TValue> { Update = fnUpdate };
            return InsertResult.Updated == Insert(key, ref info);
        }

        /// <summary>Removes a key and value from the dictionary.</summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey,TValue}"/> representing the key and value to remove.</param>
        /// <returns>
        /// <c>true</c> if the key and value represented by <paramref name="item"/> are successfully
        /// found and removed; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Both the specifed key and value must match the entry in the dictionary for it to be removed.
        /// The key is compared using the dictionary's comparer (or the default comparer for <typeparamref name="TKey"/>
        /// if no comparer was provided to the dictionary when it was constructed).  The value is compared using the
        /// default comparer for <typeparamref name="TValue"/>.
        /// </remarks>
        public bool TryRemove(KeyValuePair<TKey, TValue> item)
        {
            var info = new DelInfo<TKey, TValue>(item.Value);
            return Delete(item.Key, ref info);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>
        /// if the <paramref name="fnCondition"/> predicate is <c>null</c> or returns <c>true</c>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="fnCondition">The predicate to use to determine whether to remove the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fnCondition"/> is <c>null</c>.</exception>
        public bool TryRemove(TKey key, KeyValuePredicate<TKey, TValue> fnCondition)
        {
            if (fnCondition == null)
                throw new ArgumentNullException(nameof(fnCondition));

            var info = new DelInfo<TKey, TValue> { Condition = fnCondition };
            return Delete(key, ref info);
        }

        // J2N: Removed, since we don't want to expose the IRemoveValue interface publicly.
        // If required, we can add this back in at some point, but it is more likely we will
        // make additional delegate methods like ConcurrentDictionary has instead.
        ///// <summary>
        ///// Conditionally removes a key/value pair from the dictionary via an implementation of the
        ///// <see cref="T:CSharpTest.Net.Collections.IRemoveValue`2"/> interface.
        ///// </summary>
        ///// <param name="key">The key of the element to remove.</param>
        ///// <param name="removeValue">An implementation of <see cref="IRemoveValue{TKey, TValue}"/> to use to remove the value.</param>
        ///// <exception cref="ArgumentNullException"><paramref name="removeValue"/> is <c>null</c>.</exception>
        //public bool TryRemove<T>(TKey key, ref T removeValue) where T : IRemoveValue<TKey, TValue>
        //{
        //    if (removeValue == null)
        //        throw new ArgumentNullException(nameof(removeValue));

        //    return Delete(key, ref removeValue);
        //}

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (TryGetValue(item.Key, out TValue test))
                return J2N.Collections.Generic.EqualityComparer<TValue>.Default.Equals(item.Value, test);
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="LurchTable{TKey, TValue}"/> to the specified array
        /// of <see cref="KeyValuePair{TKey, TValue}"/> structures, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array of <see cref="KeyValuePair{TKey, TValue}"/> structures
        /// that is the destination of the elements copied from the current <see cref="LurchTable{TKey, TValue}"/>.
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

            foreach (var item in this)
                array[index++] = item;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var del = new DelInfo<TKey, TValue>(item.Value);
            return Delete(item.Key, ref del);
        }

        #endregion

        #region IEnumerator<KeyValuePair<TKey, TValue>>

        private bool MoveNext(ref EnumState state)
        {
            if (_entries == null) throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));

            if (state.Current > 0)
                state.Current = state.Next;

            if (state.Current > 0)
            {
                state.Next = _entries[state.Current >> _shift][state.Current & _shiftMask].Link;
                return true;
            }

            state.Unlock();
            while (++state.Bucket < _hsize)
            {
                if (_buckets[state.Bucket] == 0)
                    continue;

                state.Lock(_locks[state.Bucket % _lsize]);
                try
                {
                    // Now that we have a lock, check this again
                    if (_buckets[state.Bucket] == 0)
                        continue;

                    state.Current = _buckets[state.Bucket];
                    if (state.Current > 0)
                    {
                        state.Next = _entries[state.Current >> _shift][state.Current & _shiftMask].Link;
                        return true;
                    }
                }
                finally
                {
                    state.Unlock();
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LurchTable{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> for the
        /// <see cref="LurchTable{TKey, TValue}"/>.</returns>
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
        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "not an expected scenario")]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA1815 and CA1034 don't fire on all target frameworks")]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly LurchTable<TKey, TValue> _owner;
            private EnumState _state;
            private readonly int _getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int KeyValuePair = 1;
            internal const int DictEntry = 2;

            internal Enumerator(LurchTable<TKey, TValue> owner, int getEnumeratorRetType)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _state = new EnumState();
                _state.Init();
                _getEnumeratorRetType = getEnumeratorRetType;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _state.Unlock();
            }

            object IEnumerator.Current
            {
                get
                {
                    int index = _state.Current;
                    if (index <= 0)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

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

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    int index = _state.Current;
                    if (index < 0)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    if (_owner._entries == null)
                        throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));

                    return new KeyValuePair<TKey, TValue>
                        (
                            _owner._entries[index >> _owner._shift][index & _owner._shiftMask].Key,
                            _owner._entries[index >> _owner._shift][index & _owner._shiftMask].Value
                        );
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="LurchTable{TKey, TValue}"/>.
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
            /// </remarks>
            public bool MoveNext()
            {
                return _owner.MoveNext(ref _state);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                _state.Unlock();
                _state.Init();
            }

            object? IDictionaryEnumerator.Key
            {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
                get
#pragma warning restore CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
                {
                    int index = _state.Current;
                    if (index <= 0)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                    return Current.Key;
                }
            }

            object? IDictionaryEnumerator.Value
            {
                get
                {
                    int index = _state.Current;
                    if (index <= 0)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                    return Current.Value;
                }
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    int index = _state.Current;
                    if (index <= 0)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                    return new DictionaryEntry(Current.Key!, Current.Value);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Nested Class: KeyCollection

        /// <summary>
        /// Represents the collection of keys in a <see cref="LurchTable{TKey, TValue}"/>.
        /// This class cannot be inherited.
        /// </summary>
        /// <remarks>
        /// The <see cref="LurchTable{TKey, TValue}.Keys"/> property returns an instance
        /// of this type, containing all the keys in that <see cref="SortedDictionary{TKey, TValue}"/>.
        /// The order of the keys in the <see cref="LurchTable{TKey, TValue}.KeyCollection"/> is the same as the
        /// order of elements in the <see cref="LurchTable{TKey, TValue}"/>, the same as the order
        /// of the associated values in the <see cref="LurchTable{TKey, TValue}.ValueCollection"/> returned
        /// by the <see cref="LurchTable{TKey, TValue}.Values"/> property.
        /// <para/>
        /// The <see cref="LurchTable{TKey, TValue}.KeyCollection"/> is not a static copy; instead,
        /// the <see cref="LurchTable{TKey, TValue}.KeyCollection"/> refers back to the keys in the
        /// original <see cref="LurchTable{TKey, TValue}"/>. Therefore, changes to the
        /// <see cref="LurchTable{TKey, TValue}"/> continue to be reflected in the
        /// <see cref="LurchTable{TKey, TValue}.KeyCollection"/>.
        /// </remarks>
        [DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
        public class KeyCollection : ICollection<TKey>, ICollection
#if FEATURE_IREADONLYCOLLECTIONS
            , IReadOnlyCollection<TKey>
#endif
        {
            private readonly LurchTable<TKey, TValue> _owner;

            internal KeyCollection(LurchTable<TKey, TValue> owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            #region ICollection<TKey> Members

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
            /// </summary>
            public bool Contains(TKey item)
            {
                return _owner.ContainsKey(item);
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
                    throw new ArgumentNullException(nameof(array));
                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (array.Length - index < Count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                foreach (var item in _owner)
                    array[index++] = item.Key;
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));
                if (array.GetLowerBound(0) != 0)
                    throw new ArgumentException(SR.Arg_NonZeroLowerBound, nameof(array));
                if (index < 0 || index > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (array.Length - index < _owner.Count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

                TKey[]? keys = array as TKey[];
                if (keys != null)
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

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)_owner).SyncRoot;

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            public int Count => _owner.Count;

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(_owner);
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
            public struct Enumerator : IEnumerator<TKey>, IEnumerator
            {
                private readonly LurchTable<TKey, TValue> _owner;
                private EnumState _state;

                internal Enumerator(LurchTable<TKey, TValue> owner)
                {
                    _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                    _state = new EnumState();
                    _state.Init();
                }

                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                    _state.Unlock();
                }

                object? IEnumerator.Current
                {
                    get
                    {
                        int index = _state.Current;
                        if (index <= 0)
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                        if (_owner._entries == null)
                            throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));
                        return _owner._entries[index >> _owner._shift][index & _owner._shiftMask].Key;
                    }
                }

                /// <summary>
                /// Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                public TKey Current
                {
                    get
                    {
                        int index = _state.Current;
                        if (index < 0)
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                        if (_owner._entries == null)
                            throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));
                        return _owner._entries[index >> _owner._shift][index & _owner._shiftMask].Key;
                    }
                }

                /// <summary>
                /// Advances the enumerator to the next element of the <see cref="KeyCollection"/>.
                /// </summary>
                /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
                /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
                /// <remarks>
                /// After an enumerator is created, the enumerator is positioned before the first element in the collection,
                /// and the first call to the <see cref="MoveNext()"/> method advances the enumerator to the first element
                /// of the collection.
                /// <para/>
                /// If MoveNext passes the end of the collection, the enumerator is positioned after the last element in the
                /// collection and <see cref="MoveNext()"/> returns <c>false</c>. When the enumerator is at this position,
                /// subsequent calls to <see cref="MoveNext()"/> also return <c>false</c>.
                /// </remarks>
                public bool MoveNext()
                {
                    return _owner.MoveNext(ref _state);
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                public void Reset()
                {
                    _state.Unlock();
                    _state.Init();
                }
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new Enumerator(_owner);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_owner);
            }

            bool ICollection<TKey>.IsReadOnly => true;

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
            }

            #endregion
        }

        private KeyCollection? _keyCollection;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the
        /// keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        public KeyCollection Keys => _keyCollection ??= new KeyCollection(this);

#pragma warning disable IDE0079 // Remove unnecessary supppression
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

#if FEATURE_IREADONLYCOLLECTIONS
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
#endif
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary supppression

        #endregion

        #region Nested Class: ValueCollection

        /// <summary>
        /// Represents the collection of values in a <see cref="LurchTable{TKey, TValue}"/>. This class cannot be inherited.
        /// </summary>
        /// <remarks>
        /// The <see cref="LurchTable{TKey, TValue}.Values"/> property returns an instance
        /// of this type, containing all the values in that <see cref="LurchTable{TKey, TValue}"/>.
        /// The order of the values in the <see cref="LurchTable{TKey, TValue}.ValueCollection"/> is the same as the
        /// order of elements in the <see cref="LurchTable{TKey, TValue}"/>, the same as the order
        /// of the associated values in the <see cref="LurchTable{TKey, TValue}.KeyCollection"/> returned
        /// by the <see cref="LurchTable{TKey, TValue}.Keys"/> property.
        /// <para/>
        /// The <see cref="LurchTable{TKey, TValue}.ValueCollection"/> is not a static copy; instead,
        /// the <see cref="LurchTable{TKey, TValue}.ValueCollection"/> refers back to the keys in the
        /// original <see cref="LurchTable{TKey, TValue}"/>. Therefore, changes to the
        /// <see cref="LurchTable{TKey, TValue}"/> continue to be reflected in the
        /// <see cref="LurchTable{TKey, TValue}.ValueCollection"/>.
        /// </remarks>
        [DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Collection design requires this to be public")]
        public class ValueCollection : ICollection<TValue>, ICollection
#if FEATURE_IREADONLYCOLLECTIONS
            , IReadOnlyCollection<TValue>
#endif
        {
            private readonly LurchTable<TKey, TValue> _owner;

            internal ValueCollection(LurchTable<TKey, TValue> owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)_owner).SyncRoot;


            #region ICollection<TValue> Members

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
            /// </summary>
            public bool Contains(TValue value)
            {
                var comparer = J2N.Collections.Generic.EqualityComparer<TValue>.Default;
                foreach (var item in _owner)
                {
                    if (comparer.Equals(item.Value, value))
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

                foreach (var item in _owner)
                    array[index++] = item.Value;
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

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            public int Count => _owner.Count;

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(_owner);
            }

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
            [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA1815 and CA1034 don't fire on all target frameworks")]
            public struct Enumerator : IEnumerator<TValue>, IEnumerator
            {
                private readonly LurchTable<TKey, TValue> _owner;
                private EnumState _state;

                internal Enumerator(LurchTable<TKey, TValue> owner)
                {
                    _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                    _state = new EnumState();
                    _state.Init();
                }

                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                    _state.Unlock();
                }

                object? IEnumerator.Current
                {
                    get
                    {
                        int index = _state.Current;
                        if (index <= 0)
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                        if (_owner._entries == null)
                            throw new ObjectDisposedException(GetType().Name);
                        return _owner._entries[index >> _owner._shift][index & _owner._shiftMask].Value;
                    }
                }

                /// <summary>
                /// Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                public TValue Current
                {
                    get
                    {
                        int index = _state.Current;
                        if (index < 0)
                            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                        if (_owner._entries == null)
                            throw new ObjectDisposedException(GetType().Name);
                        return _owner._entries[index >> _owner._shift][index & _owner._shiftMask].Value;
                    }
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
                /// </remarks>
                public bool MoveNext()
                {
                    return _owner.MoveNext(ref _state);
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                public void Reset()
                {
                    _state.Unlock();
                    _state.Init();
                }
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(_owner);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_owner);
            }

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
            }

            #endregion
        }

        private ValueCollection? _valueCollection;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        public ValueCollection Values => _valueCollection ??= new ValueCollection(this);

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

#if FEATURE_IREADONLYCOLLECTIONS
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
#endif
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
#pragma warning restore IDE0079 // Remove unnecessary suppression

        #endregion

        #region Peek/Dequeue

        /// <summary>
        /// Retrieves the oldest entry in the collection based on the ordering supplied to the constructor.
        /// </summary>
        /// <returns>True if the out parameter value was set.</returns>
        /// <exception cref="InvalidOperationException">The table is unordered.</exception>
        public bool Peek(out KeyValuePair<TKey, TValue> value)
        {
            if (_ordering == LurchTableOrder.None)
                throw new InvalidOperationException();
            if (_entries == null)
                throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));

            while (true)
            {
                int index = Interlocked.CompareExchange(ref _entries[0][0].Prev, 0, 0);
                if (index == 0)
                {
                    value = default;
                    return false;
                }

                int hash = _entries[index >> _shift][index & _shiftMask].Hash;
                if (hash >= 0)
                {
                    int bucket = hash % _hsize;
                    lock (_locks[bucket % _lsize])
                    {
                        if (index == _entries[0][0].Prev &&
                            hash == _entries[index >> _shift][index & _shiftMask].Hash)
                        {
                            value = new KeyValuePair<TKey, TValue>(
                                _entries[index >> _shift][index & _shiftMask].Key,
                                _entries[index >> _shift][index & _shiftMask].Value
                            );
                            return true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the oldest entry in the collection based on the ordering supplied to the constructor.
        /// If an item is not available a busy-wait loop is used to wait for for an item.
        /// </summary>
        /// <returns>The Key/Value pair removed.</returns>
        /// <exception cref="InvalidOperationException">The table is unordered.</exception>
        public KeyValuePair<TKey, TValue> Dequeue()
        {
            if (_ordering == LurchTableOrder.None)
                throw new InvalidOperationException();
            if (_entries == null)
                throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));

            KeyValuePair<TKey, TValue> value;
            while (!TryDequeue(out value))
            {
                while (0 == Interlocked.CompareExchange(ref _entries[0][0].Prev, 0, 0))
                    Thread.Sleep(0);
            }
            return value;
        }

        /// <summary>
        /// Removes the oldest entry in the collection based on the ordering supplied to the constructor.
        /// </summary>
        /// <returns>False if no item was available</returns>
        /// <exception cref="InvalidOperationException">The table is unordered.</exception>
        public bool TryDequeue(out KeyValuePair<TKey, TValue> value)
        {
            return TryDequeue(null, out value);
        }


        /// <summary>
        /// Removes the oldest entry in the collection based on the ordering supplied to the constructor.
        /// </summary>
        /// <returns>False if no item was available</returns>
        /// <exception cref="InvalidOperationException">The table is unordered.</exception>
        public bool TryDequeue(Predicate<KeyValuePair<TKey, TValue>>? predicate, out KeyValuePair<TKey, TValue> value)
        {
            if (_ordering == LurchTableOrder.None)
                throw new InvalidOperationException();
            if (_entries == null)
                throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));

            while (true)
            {
                int index = Interlocked.CompareExchange(ref _entries[0][0].Prev, 0, 0);
                if (index == 0)
                {
                    value = default(KeyValuePair<TKey, TValue>);
                    return false;
                }

                int hash = _entries[index >> _shift][index & _shiftMask].Hash;
                if (hash >= 0)
                {
                    int bucket = hash % _hsize;
                    lock (_locks[bucket % _lsize])
                    {
                        if (index == _entries[0][0].Prev &&
                            hash == _entries[index >> _shift][index & _shiftMask].Hash)
                        {
                            if (predicate != null)
                            {
                                var item = new KeyValuePair<TKey, TValue>(
                                    _entries[index >> _shift][index & _shiftMask].Key,
                                    _entries[index >> _shift][index & _shiftMask].Value
                                );
                                if (!predicate(item))
                                {
                                    value = item;
                                    return false;
                                }
                            }

                            int next = _entries[index >> _shift][index & _shiftMask].Link;
                            bool removed = false;

                            if (_buckets[bucket] == index)
                            {
                                _buckets[bucket] = next;
                                removed = true;
                            }
                            else
                            {
                                int test = _buckets[bucket];
                                while (test != 0)
                                {
                                    int cmp = _entries[test >> _shift][test & _shiftMask].Link;
                                    if (cmp == index)
                                    {
                                        _entries[test >> _shift][test & _shiftMask].Link = next;
                                        removed = true;
                                        break;
                                    }
                                    test = cmp;
                                }
                            }
                            if (!removed)
                                throw new LurchTableCorruptionException();

                            value = new KeyValuePair<TKey, TValue>(
                                _entries[index >> _shift][index & _shiftMask].Key,
                                _entries[index >> _shift][index & _shiftMask].Value
                            );
                            Interlocked.Decrement(ref _count);
                            if (_ordering != LurchTableOrder.None)
                                InternalUnlink(index);
                            FreeSlot(ref index, Interlocked.Increment(ref _freeVersion));

                            ItemRemoved?.Invoke(value);

                            return true;
                        }
                    }
                }
            }
        }

        #endregion

        #region Internal Implementation

        private enum InsertResult { Inserted = 1, Updated = 2, Exists = 3, NotFound = 4 }

        private bool InternalGetValue(int hash, [AllowNull, MaybeNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_entries == null)
                throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));

            int bucket = hash % _hsize;
            lock (_locks[bucket % _lsize])
            {
                int index = _buckets[bucket];
                while (index != 0)
                {
                    if (hash == _entries[index >> _shift][index & _shiftMask].Hash &&
                        KeyEquals(key, _entries[index >> _shift][index & _shiftMask].Key))
                    {
                        value = _entries[index >> _shift][index & _shiftMask].Value;
                        if (hash == _entries[index >> _shift][index & _shiftMask].Hash)
                        {
                            if (_ordering == LurchTableOrder.Access)
                            {
                                InternalUnlink(index);
                                InternalLink(index);
                            }
                            return true;
                        }
                    }
                    index = _entries[index >> _shift][index & _shiftMask].Link;
                }

                value = default;
                return false;
            }
        }

        private bool InternalContainsValue(Predicate<TValue> matchPredicate)
        {
            if (_entries == null)
                throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));

            for (int bucket = 0; bucket < _buckets.Length; bucket++)
            {
                lock (_locks[bucket % _lsize])
                {
                    int index = _buckets[bucket];
                    while (index != 0)
                    {
                        if (matchPredicate(_entries[index >> _shift][index & _shiftMask].Value))
                        {
                            if (_ordering == LurchTableOrder.Access)
                            {
                                InternalUnlink(index);
                                InternalLink(index);
                            }
                            return true;
                        }
                        index = _entries[index >> _shift][index & _shiftMask].Link;
                    }
                }
            }
            return false;
        }

        private InsertResult Insert<T>([AllowNull] TKey key, [MaybeNull] ref T value) where T : ICreateOrUpdateValue<TKey, TValue>
        {
            if (_entries == null)
                throw new ObjectDisposedException(nameof(LurchTable<TKey, TValue>));

            int hash = GetHash(key);

            InsertResult result = InternalInsert(hash, key, out int added, ref value);

            if (added > _limit && _ordering != LurchTableOrder.None)
            {
                TryDequeue(out KeyValuePair<TKey, TValue> _);
            }
            return result;
        }

        private InsertResult InternalInsert<T>(int hash, [AllowNull] TKey key, out int added, ref T value) where T : ICreateOrUpdateValue<TKey, TValue>
        {
            int bucket = hash % _hsize;
            lock (_locks[bucket % _lsize])
            {
                TValue temp;
                int index = _buckets[bucket];
                while (index != 0)
                {
                    if (hash == _entries[index >> _shift][index & _shiftMask].Hash &&
                        KeyEquals(key, _entries[index >> _shift][index & _shiftMask].Key))
                    {
                        temp = _entries[index >> _shift][index & _shiftMask].Value;
                        var original = temp;
                        if (value.UpdateValue(key, ref temp))
                        {
                            _entries[index >> _shift][index & _shiftMask].Value = temp;

                            if (_ordering == LurchTableOrder.Modified || _ordering == LurchTableOrder.Access)
                            {
                                InternalUnlink(index);
                                InternalLink(index);
                            }

                            ItemUpdated?.Invoke(new KeyValuePair<TKey, TValue>(key!, original), new KeyValuePair<TKey, TValue>(key!, temp));

                            added = -1;
                            return InsertResult.Updated;
                        }

                        added = -1;
                        return InsertResult.Exists;
                    }
                    index = _entries[index >> _shift][index & _shiftMask].Link;
                }
                if (value.CreateValue(key, out temp))
                {
                    index = AllocSlot();
                    _entries[index >> _shift][index & _shiftMask].Hash = hash;
                    _entries[index >> _shift][index & _shiftMask].Key = key;
                    _entries[index >> _shift][index & _shiftMask].Value = temp;
                    _entries[index >> _shift][index & _shiftMask].Link = _buckets[bucket];
                    _buckets[bucket] = index;

                    added = Interlocked.Increment(ref _count);
                    if (_ordering != LurchTableOrder.None)
                        InternalLink(index);

                    ItemAdded?.Invoke(new KeyValuePair<TKey, TValue>(key!, temp));

                    return InsertResult.Inserted;
                }
            }

            added = -1;
            return InsertResult.NotFound;
        }

        // Not thread-safe - used by copy constructors.
        private void CopyConstructorAddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (var pair in collection)
            {
                int hash = GetHash(pair.Key);
                int bucket = hash % _hsize;
                int index = _buckets[bucket];

                // Ensure we didn't already add the key
                while (index != 0)
                {
                    if (hash == _entries[index >> _shift][index & _shiftMask].Hash &&
                        KeyEquals(pair.Key, _entries[index >> _shift][index & _shiftMask].Key))
                    {
                        throw new ArgumentException(J2N.SR.Format(SR.Argument_AddingDuplicate, pair.Key));
                    }
                    index = _entries[index >> _shift][index & _shiftMask].Link;
                }

                // Add the pair
                index = AllocSlot();
                _entries[index >> _shift][index & _shiftMask].Hash = hash;
                _entries[index >> _shift][index & _shiftMask].Key = pair.Key;
                _entries[index >> _shift][index & _shiftMask].Value = pair.Value;
                _entries[index >> _shift][index & _shiftMask].Link = _buckets[bucket];
                _buckets[bucket] = index;

                int added = ++_count;
                if (_ordering != LurchTableOrder.None)
                {
                    InternalLink(index);
                    if (added > _limit)
                        TryDequeue(out KeyValuePair<TKey, TValue> _);
                }
            }
        }

        private bool Delete<T>(TKey key, ref T value) where T : IRemoveValue<TKey, TValue>
        {
            if (_entries == null)
                throw new ObjectDisposedException(GetType().Name);

            int hash = GetHash(key);
            int bucket = hash % _hsize;
            lock (_locks[bucket % _lsize])
            {
                int prev = 0;
                int index = _buckets[bucket];
                while (index != 0)
                {
                    if (hash == _entries[index >> _shift][index & _shiftMask].Hash &&
                        KeyEquals(key, _entries[index >> _shift][index & _shiftMask].Key))
                    {
                        TValue temp = _entries[index >> _shift][index & _shiftMask].Value;

                        if (value.RemoveValue(key, temp))
                        {
                            int next = _entries[index >> _shift][index & _shiftMask].Link;
                            if (prev == 0)
                                _buckets[bucket] = next;
                            else
                                _entries[prev >> _shift][prev & _shiftMask].Link = next;

                            Interlocked.Decrement(ref _count);
                            if (_ordering != LurchTableOrder.None)
                                InternalUnlink(index);
                            FreeSlot(ref index, Interlocked.Increment(ref _freeVersion));

                            ItemRemoved?.Invoke(new KeyValuePair<TKey, TValue>(key, temp));

                            return true;
                        }
                        return false;
                    }

                    prev = index;
                    index = _entries[index >> _shift][index & _shiftMask].Link;
                }
            }
            return false;
        }

        private void InternalLink(int index)
        {
            Interlocked.Exchange(ref _entries[index >> _shift][index & _shiftMask].Prev, 0);
            Interlocked.Exchange(ref _entries[index >> _shift][index & _shiftMask].Next, ~0);
            int next = Interlocked.Exchange(ref _entries[0][0].Next, index);
            if (next < 0)
                throw new LurchTableCorruptionException();

            while (0 != Interlocked.CompareExchange(ref _entries[next >> _shift][next & _shiftMask].Prev, index, 0))
            { }

            Interlocked.Exchange(ref _entries[index >> _shift][index & _shiftMask].Next, next);
        }

        private void InternalUnlink(int index)
        {
            while (true)
            {
                int cmp;
                int prev = _entries[index >> _shift][index & _shiftMask].Prev;
                while (prev >= 0 && prev != (cmp = Interlocked.CompareExchange(
                            ref _entries[index >> _shift][index & _shiftMask].Prev, ~prev, prev)))
                    prev = cmp;
                if (prev < 0)
                    throw new LurchTableCorruptionException();

                int next = _entries[index >> _shift][index & _shiftMask].Next;
                while (next >= 0 && next != (cmp = Interlocked.CompareExchange(
                            ref _entries[index >> _shift][index & _shiftMask].Next, ~next, next)))
                    next = cmp;
                if (next < 0)
                    throw new LurchTableCorruptionException();

                if ((Interlocked.CompareExchange(
                        ref _entries[prev >> _shift][prev & _shiftMask].Next, next, index) == index))
                {
                    while (Interlocked.CompareExchange(
                               ref _entries[next >> _shift][next & _shiftMask].Prev, prev, index) != index)
                    { }
                    return;
                }

                //cancel the delete markers and retry
                if (~next != Interlocked.CompareExchange(
                        ref _entries[index >> _shift][index & _shiftMask].Next, next, ~next))
                    throw new LurchTableCorruptionException();
                if (~prev != Interlocked.CompareExchange(
                        ref _entries[index >> _shift][index & _shiftMask].Prev, prev, ~prev))
                    throw new LurchTableCorruptionException();
            }
        }

        // Release build inlining, so we need to ignore for testing.
        private int AllocSlot()
        {
            while (true)
            {
                int allocated = _entries.Length * _allocSize;
                var previous = _entries;

                while (_count + OverAlloc < allocated || _used < allocated)
                {
                    int next;
                    if (_count + FreeSlots < _used)
                    {
                        int freeSlotIndex = Interlocked.Increment(ref _allocNext);
                        int slot = (freeSlotIndex & int.MaxValue) % FreeSlots;
                        next = Interlocked.Exchange(ref _free[slot].Head, 0);
                        if (next != 0)
                        {
                            int nextFree = _entries[next >> _shift][next & _shiftMask].Link;
                            if (nextFree == 0)
                            {
                                Interlocked.Exchange(ref _free[slot].Head, next);
                            }
                            else
                            {
                                Interlocked.Exchange(ref _free[slot].Head, nextFree);
                                return next;
                            }
                        }
                    }

                    next = _used;
                    if (next < allocated)
                    {
                        int alloc = Interlocked.CompareExchange(ref _used, next + 1, next);
                        if (alloc == next)
                        {
                            return next;
                        }
                    }
                }

                lock (SyncRoot)
                {
                    //time to grow...
                    if (ReferenceEquals(_entries, previous))
                    {
                        Entry[][] arentries = new Entry[_entries.Length + 1][];
                        _entries.CopyTo(arentries, 0);
                        arentries[arentries.Length - 1] = new Entry[_allocSize];

                        Interlocked.CompareExchange(ref _entries, arentries, previous);
                    }
                }
            }
        }

        private void FreeSlot(ref int index, int ver)
        {
            _entries[index >> _shift][index & _shiftMask].Key = default!;
            _entries[index >> _shift][index & _shiftMask].Value = default!;
            Interlocked.Exchange(ref _entries[index >> _shift][index & _shiftMask].Link, 0);

            int slot = (ver & int.MaxValue) % FreeSlots;
            int prev = Interlocked.Exchange(ref _free[slot].Tail, index);

            if (prev <= 0 || 0 != Interlocked.CompareExchange(ref _entries[prev >> _shift][prev & _shiftMask].Link, index, 0))
            {
                throw new LurchTableCorruptionException();
            }
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        private int GetHash([AllowNull] TKey key)
        {
            return (key is null ? 0 : _comparer.GetHashCode(key)) & int.MaxValue;
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        private bool KeyEquals([AllowNull] TKey key1, [AllowNull] TKey key2)
        {
            if (key1 is null)
                return key2 is null;
            if (key2 is null)
                return false;

            return _comparer.Equals(key1, key2);
        }

        #endregion

        #region Nested Structures

        private struct FreeList
        {
            public int Head;
            public int Tail;
        }

        private struct Entry
        {
            public int Prev, Next; // insertion/access sequence ordering
            public int Link;
            public int Hash; // hash value of entry's Key
            [AllowNull] public TKey Key; // key of entry
            [AllowNull] public TValue Value; // value of entry
        }

        private struct EnumState
        {
            private object? _locked;
            public int Bucket, Current, Next;
            public void Init()
            {
                Bucket = -1;
                Current = 0;
                Next = 0;
                _locked = null;
            }

            public void Unlock()
            {
                if (_locked != null)
                {
                    Monitor.Exit(_locked);
                    _locked = null;
                }
            }

            public void Lock(object lck)
            {
                if (_locked != null)
                    Monitor.Exit(_locked);
                Monitor.Enter(_locked = lck);
            }
        }

        #endregion

        #region Nested Class: NullableKeyComparer

        private class NullableKeyComparer : IEqualityComparer<TKey>
        {
            private readonly IEqualityComparer<TKey> wrappedComparer;

            public NullableKeyComparer(IEqualityComparer<TKey> wrappedComparer)
            {
                this.wrappedComparer = wrappedComparer ?? throw new ArgumentNullException(nameof(wrappedComparer));
            }

            public bool Equals([AllowNull] TKey x, [AllowNull] TKey y)
            {
                if (x is null)
                    return y is null;
                if (y is null)
                    return false;

                return wrappedComparer.Equals(x, y);
            }

            public int GetHashCode([AllowNull] TKey obj)
            {
                return obj is null ? 0 : wrappedComparer.GetHashCode(obj);
            }
        }

        #endregion
    }

    #region LurchTable Support

    #region Structures

    // NOTE: These were originally nested within LurchTable, but
    // using nested generic structs without explicitly defining their parameters
    // fails on Xamarin.iOS because of incomplete generics support. Therefore,
    // we de-nested them and moved them here. See: LUCENENET-602

    internal struct DelInfo<TKey, TValue> : IRemoveValue<TKey, TValue>
    {
        [AllowNull] public TValue Value;
        private readonly bool _hasTestValue;
        [AllowNull] private readonly TValue _testValue;
        [AllowNull] public KeyValuePredicate<TKey, TValue> Condition;

        public DelInfo([AllowNull] TValue expected)
        {
            Value = default;
            _testValue = expected;
            _hasTestValue = true;
            Condition = null;
        }

        public bool RemoveValue([AllowNull] TKey key, [AllowNull] TValue value)
        {
            Value = value;

            if (_hasTestValue && !J2N.Collections.Generic.EqualityComparer<TValue>.Default.Equals(_testValue, value!))
                return false;
            if (Condition != null && !Condition(key, value))
                return false;

            return true;
        }
    }


    internal struct AddInfo<TKey, TValue> : ICreateOrUpdateValue<TKey, TValue>
    {
        public bool CanUpdate;
        [AllowNull] public TValue Value;
        public bool CreateValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            value = Value;
            return true;
        }

        public bool UpdateValue([AllowNull] TKey key, [AllowNull] ref TValue value)
        {
            if (!CanUpdate)
            {
                Value = value;
                return false;
            }

            value = Value;
            return true;
        }
    }

    internal struct Add2Info<TKey, TValue> : ICreateOrUpdateValue<TKey, TValue>
    {
        private readonly bool _hasAddValue;
        [AllowNull] private readonly TValue _addValue;
        [AllowNull] public TValue Value;
        [AllowNull] public Func<TKey, TValue> Create;
        [AllowNull] public KeyValueUpdate<TKey, TValue> Update;

        public Add2Info([AllowNull] TValue addValue) : this()
        {
            _hasAddValue = true;
            _addValue = addValue;
        }

        public bool CreateValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_hasAddValue)
            {
                value = Value = _addValue;
                return true;
            }
            if (Create != null)
            {
                value = Value = Create(key!);
                return true;
            }
            value = Value = default;
            return false;
        }

        public bool UpdateValue([AllowNull] TKey key, [AllowNull] ref TValue value)
        {
            if (Update == null)
            {
                Value = value;
                return false;
            }

            value = Value = Update(key, value);
            return true;
        }
    }

    internal struct UpdateInfo<TKey, TValue> : ICreateOrUpdateValue<TKey, TValue>
    {
        [AllowNull] public TValue Value;
        private readonly bool _hasTestValue;
        [AllowNull] private readonly TValue _testValue;

        public UpdateInfo([AllowNull] TValue expected)
        {
            Value = default;
            _testValue = expected;
            _hasTestValue = true;
        }

        bool ICreateValue<TKey, TValue>.CreateValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            value = default;
            return false;
        }
        public bool UpdateValue([AllowNull] TKey key, [AllowNull] ref TValue value)
        {
            if (_hasTestValue && !J2N.Collections.Generic.EqualityComparer<TValue>.Default.Equals(_testValue, value!))
                return false;

            value = Value;
            return true;
        }
    }

    #endregion

    #region Exceptions

    /// <summary>
    /// Exception class: LurchTableCorruptionException
    /// The LurchTable internal datastructure appears to be corrupted.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class LurchTableCorruptionException : Exception
    {
#if FEATURE_SERIALIZABLE
        /// <summary>
        /// Serialization constructor
        /// </summary>
        protected LurchTableCorruptionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
#endif
        /// <summary>
        /// Initializes a new instance of <see cref="LurchTableCorruptionException"/> with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public LurchTableCorruptionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="LurchTableCorruptionException"/> with the specified message
        /// and original exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The original exception.</param>
        public LurchTableCorruptionException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Used to create this exception from an hresult and message bypassing the message formatting
        /// </summary>
        internal static Exception Create(int hResult, string message)
        {
            return new LurchTableCorruptionException(null, hResult, message);
        }
        /// <summary>
        /// Constructs the exception from an hresult and message bypassing the message formatting
        /// </summary>
        protected LurchTableCorruptionException(Exception? innerException, int hResult, string message) : base(message, innerException)
        {
            base.HResult = hResult;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LurchTableCorruptionException"/> with the default message.
        /// </summary>
        public LurchTableCorruptionException() : this(null, -1, SR.LurchTable_CorruptedData) { }

        /// <summary>
        /// Initializes a new instance of <see cref="LurchTableCorruptionException"/> with the default message
        /// and original exception.
        /// </summary>
        public LurchTableCorruptionException(Exception? innerException) : this(innerException, -1, SR.LurchTable_CorruptedData) { }

        /// <summary>
        /// If <paramref name="condition"/> is <c>false</c>, throws <see cref="LurchTableCorruptionException"/> with the default message.
        /// </summary>
        public static void Assert(bool condition)
        {
            if (!condition) throw new LurchTableCorruptionException();
        }
    }

    #endregion // Exceptions

    #region Delegates

    /// <summary> Provides a delegate that performs an atomic update of a key/value pair </summary>
    public delegate TValue KeyValueUpdate<TKey, TValue>([AllowNull] TKey key, [AllowNull] TValue original);

    /// <summary> Provides a delegate that performs a test on key/value pair </summary>
    public delegate bool KeyValuePredicate<TKey, TValue>([AllowNull] TKey key, [AllowNull] TValue original);

    #endregion // Delegates

    #region Interfaces

    /// <summary>
    /// An interface to provide conditional or custom creation logic to a concurrent dictionary.
    /// </summary>
    internal interface ICreateValue<TKey, TValue>
    {
        /// <summary>
        /// Called when the key was not found within the dictionary to produce a new value that can be added.
        /// Return true to continue with the insertion, or false to prevent the key/value from being inserted.
        /// </summary>
        bool CreateValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value);
    }

    /// <summary>
    /// An interface to provide conditional or custom update logic to a concurrent dictionary.
    /// </summary>
    internal interface IUpdateValue<TKey, TValue>
    {
        /// <summary>
        /// Called when the key was found within the dictionary to produce a modified value to update the item
        /// to. Return true to continue with the update, or false to prevent the key/value from being updated.
        /// </summary>
        bool UpdateValue([AllowNull] TKey key, [MaybeNullWhen(false)] ref TValue value);
    }

    /// <summary>
    /// An interface to provide conditional or custom creation or update logic to a concurrent dictionary.
    /// </summary>
    /// <remarks>
    /// Generally implemented as a struct and passed by ref to save stack space and to retrieve the values
    /// that where inserted or updated.
    /// </remarks>
    internal interface ICreateOrUpdateValue<TKey, TValue> : ICreateValue<TKey, TValue>, IUpdateValue<TKey, TValue>
    {
    }

    /// <summary>
    /// An interface to provide conditional removal of an item from a concurrent dictionary.
    /// </summary>
    /// <remarks>
    /// Generally implemented as a struct and passed by ref to save stack space and to retrieve the values
    /// that where inserted or updated.
    /// </remarks>
    internal interface IRemoveValue<TKey, TValue>
    {
        /// <summary>
        /// Called when the dictionary is about to remove the key/value pair provided, return true to allow
        /// it's removal, or false to prevent it from being removed.
        /// </summary>
        bool RemoveValue(TKey key, TValue value);
    }

    #endregion // interfaces

    #endregion // LurchTable Support
}
