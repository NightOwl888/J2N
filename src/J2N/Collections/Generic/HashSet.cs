// https://github.com/dotnet/runtime/blob/v9.0.9/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/HashSet.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.ObjectModel;
using J2N.Runtime.CompilerServices;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SCG = System.Collections.Generic;

#if FEATURE_SERIALIZABLE
using System.ComponentModel;
#endif

#if FEATURE_EXCEPTION_STATIC_GUARDCLAUSES
using static System.ArgumentOutOfRangeException;
#else
using static J2N.Collections.StaticThrowHelper;
#endif

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Represents a set of values.
    /// <para/>
    /// <see cref="HashSet{T}"/> adds the following features to <see cref="System.Collections.Generic.HashSet{T}"/>:
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
    /// Usage Note: This class is intended to be a direct replacement for <see cref="System.Collections.Generic.HashSet{T}"/> in order
    /// to provide default structural equality and formatting behavior similar to Java. Note that the <see cref="ToString()"/>
    /// method uses the current culture by default to behave like other components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <remarks>
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
    [SuppressMessage("Style", "IDE0018:Inline variable declaration", Justification = "Following Microsoft's code style")]
    [SuppressMessage("Style", "IDE0019:Use pattern matching", Justification = "Following Microsoft's code style")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class HashSet<T> : ISet<T>, ICollection<T>,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyCollection<T>,
#endif
#if FEATURE_READONLYSET
        IReadOnlySet<T>,
#endif
        IStructuralEquatable, IStructuralFormattable
#if FEATURE_SERIALIZABLE
        , System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable
#endif
    {
        // This uses the same array-based implementation as Dictionary<TKey, TValue>.

#if FEATURE_SERIALIZABLE
        // constants for serialization
        private const string CapacityName = "Capacity"; // Do not rename (binary serialization)
        private const string ElementsName = "Elements"; // Do not rename (binary serialization)
        private const string EqualityComparerName = "EqualityComparer"; // Do not rename (binary serialization)
        private const string VersionName = "Version"; // Do not rename (binary serialization)
#endif

        /// <summary>Cutoff point for stackallocs. This corresponds to the number of ints.</summary>
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Following Microsoft's code style")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "IDE0051 doesn't fire on all target frameworks")]
        private const int StackAllocThreshold = 100;

        /// <summary>
        /// When constructing a hashset from an existing collection, it may contain duplicates,
        /// so this is used as the max acceptable excess ratio of capacity to count. Note that
        /// this is only used on the ctor and not to automatically shrink if the hashset has, e.g,
        /// a lot of adds followed by removes. Users must explicitly shrink by calling TrimExcess.
        /// This is set to 3 because capacity is acceptable as 2x rounded up to nearest prime.
        /// </summary>
        private const int ShrinkThreshold = 3;
        private const int StartOfFreeList = -3;

        private int[]? _buckets;
        private Entry[]? _entries;
        private ulong _fastModMultiplier;
        private int _count;
        private int _freeList;
        private int _freeCount;
        private int _version;
        private IEqualityComparer<T>? _comparer;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty
        /// and uses <see cref="EqualityComparer{T}.Default"/> for the set type.
        /// </summary>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="HashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public HashSet() : this((IEqualityComparer<T>?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty and uses the
        /// specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="HashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public HashSet(IEqualityComparer<T>? comparer)
        {
            // For reference types, we always want to store a comparer instance, either
            // the one provided, or if one wasn't provided, the default (accessing
            // EqualityComparer<TKey>.Default with shared generics on every dictionary
            // access can add measurable overhead).  For value types, if no comparer is
            // provided, or if the default is provided, we'd prefer to use
            // EqualityComparer<TKey>.Default.Equals on every use, enabling the JIT to
            // devirtualize and possibly inline the operation.
            if (!typeof(T).IsValueType)
            {
                _comparer = comparer ?? EqualityComparer<T>.Default;

                // Special-case EqualityComparer<string>.Default, StringComparer.Ordinal, and StringComparer.OrdinalIgnoreCase.
                // We use a non-randomized comparer for improved perf, falling back to a randomized comparer if the
                // hash buckets become unbalanced.
                if (typeof(T) == typeof(string) &&
                    NonRandomizedStringEqualityComparer.GetStringComparer(_comparer) is IEqualityComparer<string> stringComparer)
                {
                    _comparer = (IEqualityComparer<T>)stringComparer;
                }
            }
            else if (comparer is not null && // first check for null to avoid forcing default comparer instantiation unnecessarily
                     comparer != EqualityComparer<T>.Default)
            {
                _comparer = comparer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty, but has reserved
        /// space for <paramref name="capacity"/> items and uses <see cref="EqualityComparer{T}.Default"/> for the set type.
        /// </summary>
        /// <param name="capacity">The initial size of the <see cref="HashSet{T}"/>.</param>
        /// <remarks>
        /// Since resizes are relatively expensive (require rehashing), this attempts to minimize the need
        /// to resize by setting the initial capacity based on the value of the <paramref name="capacity"/>.
        /// </remarks>
        public HashSet(int capacity) : this(capacity, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that uses <see cref="EqualityComparer{T}.Default"/>
        /// for the set type, contains elements copied from the specified collection, and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="HashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// If <paramref name="collection"/> contains duplicates, the set will contain one of each unique element.No exception will
        /// be thrown. Therefore, the size of the resulting set is not identical to the size of <paramref name="collection"/>.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in the <paramref name="collection"/> parameter.
        /// </remarks>
        public HashSet(IEnumerable<T> collection) : this(collection, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that uses the specified equality comparer for the set type,
        /// contains elements copied from the specified collection, and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold. A <see cref="HashSet{T}"/>
        /// object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// If <paramref name="collection"/> contains duplicates, the set will contain one of each unique element. No exception will be thrown. Therefore,
        /// the size of the resulting set is not identical to the size of <paramref name="collection"/>.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in the <paramref name="collection"/> parameter.
        /// </remarks>
        public HashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
            : this(comparer)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }

            if (collection is HashSet<T> otherAsHashSet && EffectiveEqualityComparersAreEqual(this, otherAsHashSet))
            {
                ConstructFrom(otherAsHashSet);
            }
            else
            {
                // To avoid excess resizes, first set size based on collection's count. The collection may
                // contain duplicates, so call TrimExcess if resulting HashSet is larger than the threshold.
                if (collection is ICollection<T> coll)
                {
                    int count = coll.Count;
                    if (count > 0)
                    {
                        Initialize(count);
                    }
                }

                UnionWith(collection);

                if (_count > 0 && _entries!.Length / _count > ShrinkThreshold)
                {
                    TrimExcess();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that uses the specified equality comparer
        /// for the set type, and has sufficient capacity to accommodate <paramref name="capacity"/> elements.
        /// </summary>
        /// <param name="capacity">The initial size of the <see cref="HashSet{T}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <remarks>Since resizes are relatively expensive (require rehashing), this attempts to minimize the need to resize
        /// by setting the initial capacity based on the value of the <paramref name="capacity"/>.</remarks>
        public HashSet(int capacity, IEqualityComparer<T>? comparer)
            : this(comparer)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }

            if (capacity > 0)
            {
                Initialize(capacity);
            }
        }

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class with serialized data.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains
        /// the information required to serialize the <see cref="HashSet{T}"/> object.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure that contains
        /// the source and destination of the serialized stream associated with the <see cref="HashSet{T}"/> object.</param>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected HashSet(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            // We can't do anything with the keys and values until the entire graph has been
            // deserialized and we have a reasonable estimate that GetHashCode is not going to
            // fail.  For the time being, we'll just cache this.  The graph is not valid until
            // OnDeserialization has been called.
            HashHelpers.SerializationInfoTable.Add(this, info);
        }
#endif

        /// <summary>Initializes the HashSet from another HashSet with the same element type and equality comparer.</summary>
        private void ConstructFrom(HashSet<T> source)
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
                _freeList = source._freeList;
                _freeCount = source._freeCount;
                _count = source._count;
                _fastModMultiplier = source._fastModMultiplier;
            }
            else
            {
                Initialize(source.Count);

                Entry[]? entries = source._entries;
                for (int i = 0; i < source._count; i++)
                {
                    ref Entry entry = ref entries![i];
                    if (entry.Next >= -1)
                    {
                        AddIfNotPresent(entry.Value, out _);
                    }
                }
            }

            Debug.Assert(Count == source.Count);
        }

        #endregion Constructors

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlySet{T}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="HashSet{T}"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="HashSet{T}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlySet{T}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="HashSet{T}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlySet<T> AsReadOnly()
            => new ReadOnlySet<T>(this);

        #endregion AsReadOnly

        #region ICollection<T> methods

        /// <summary>
        /// Add item to this hashset. This is the explicit implementation of the <see cref="ICollection{T}"/>
        /// interface. The other Add method returns bool indicating whether item was added.
        /// </summary>
        /// <param name="item">item to add</param>
        void ICollection<T>.Add(T item) => AddIfNotPresent(item, out _);

        /// <summary>
        /// Removes all elements from a <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <remarks>
        /// <see cref="Count"/> is set to zero and references to other objects from elements of the
        /// collection are also released. The capacity remains unchanged until a call to <see cref="TrimExcess()"/> is made.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void Clear()
        {
            int count = _count;
            if (count > 0)
            {
                Debug.Assert(_buckets != null, "_buckets should be non-null");
                Debug.Assert(_entries != null, "_entries should be non-null");

#if FEATURE_ARRAY_CLEAR_ARRAY
                Array.Clear(_buckets);
#else
                Array.Clear(_buckets!, 0, _buckets!.Length);
#endif
                _count = 0;
                _freeList = -1;
                _freeCount = 0;
                Array.Clear(_entries!, 0, count);
            }
        }

        /// <summary>
        /// Determines whether a <see cref="HashSet{T}"/> object contains the specified element.
        /// </summary>
        /// <param name="item">The element to locate in the <see cref="HashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="HashSet{T}"/> object contains the specified element;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method is an O(1) operation.
        /// </remarks>
        public bool Contains(T item) => FindItemIndex(item) >= 0;

        /// <summary>Gets the index of the item in <see cref="_entries"/>, or -1 if it's not in the set.</summary>
        private int FindItemIndex(T item)
        {
            int[]? buckets = _buckets;
            if (buckets != null)
            {
                Entry[]? entries = _entries;
                Debug.Assert(entries != null, "Expected _entries to be initialized");

                uint collisionCount = 0;
                IEqualityComparer<T>? comparer = _comparer;

                if (typeof(T).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                    comparer == null)
                {
                    // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                    int hashCode = item!.GetHashCode();
                    int i = GetBucketRef(hashCode) - 1; // Value in _buckets is 1-based
                    while (i >= 0)
                    {
                        ref Entry entry = ref entries![i];
                        if (entry.HashCode == hashCode && EqualityComparer<T>.Default.Equals(entry.Value, item))
                        {
                            return i;
                        }
                        i = entry.Next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop, which means a concurrent update has happened.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
                else
                {
                    Debug.Assert(comparer is not null);
                    int hashCode = item != null ? comparer!.GetHashCode(item) : 0;
                    int i = GetBucketRef(hashCode) - 1; // Value in _buckets is 1-based
                    while (i >= 0)
                    {
                        ref Entry entry = ref entries![i];
                        if (entry.HashCode == hashCode && comparer!.Equals(entry.Value, item))
                        {
                            return i;
                        }
                        i = entry.Next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop, which means a concurrent update has happened.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>Gets a reference to the specified hashcode's bucket, containing an index into <see cref="_entries"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int GetBucketRef(int hashCode)
        {
            int[] buckets = _buckets!;
            if (IntPtr.Size == 8) // 64-bit process
            {
                return ref buckets[HashHelpers.FastMod((uint)hashCode, (uint)buckets.Length, _fastModMultiplier)];
            }
            else
            {
                return ref buckets[(uint)hashCode % (uint)buckets.Length];
            }
        }

        /// <summary>
        /// Removes the specified element from a <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns><c>true</c> if the element is successfully found and removed;
        /// otherwise, <c>false</c>. This method returns <c>false</c> if item is not
        /// found in the <see cref="HashSet{T}"/> object.</returns>
        /// <remarks>
        /// If the <see cref="HashSet{T}"/> object does not contain the specified
        /// element, the object remains unchanged. No exception is thrown.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public bool Remove(T item)
        {
            if (_buckets != null)
            {
                Entry[]? entries = _entries;
                Debug.Assert(entries != null, "entries should be non-null");

                uint collisionCount = 0;
                int last = -1;

                IEqualityComparer<T>? comparer = _comparer;
                Debug.Assert(typeof(T).IsValueType || comparer is not null);
                int hashCode =
                    typeof(T).IsValueType && comparer == null ? item!.GetHashCode() :
                    item is not null ? comparer!.GetHashCode(item) :
                    0;

                ref int bucket = ref GetBucketRef(hashCode);
                int i = bucket - 1; // Value in buckets is 1-based

                while (i >= 0)
                {
                    ref Entry entry = ref entries![i];

                    if (entry.HashCode == hashCode && (comparer?.Equals(entry.Value, item) ?? EqualityComparer<T>.Default.Equals(entry.Value, item)))
                    {
                        if (last < 0)
                        {
                            bucket = entry.Next + 1; // Value in buckets is 1-based
                        }
                        else
                        {
                            entries[last].Next = entry.Next;
                        }

                        Debug.Assert((StartOfFreeList - _freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                        entry.Next = StartOfFreeList - _freeList;

                        if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
                        {
                            entry.Value = default!;
                        }

                        _freeList = i;
                        _freeCount++;
                        return true;
                    }

                    last = i;
                    i = entry.Next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop; which means a concurrent update has happened.
                        // Break out of the loop and throw, rather than looping forever.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the number of elements that are contained in a set.
        /// </summary>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="HashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// The capacity is always greater than or equal to <see cref="Count"/>. If <see cref="Count"/> exceeds the
        /// capacity while adding elements, the capacity is set to the first prime number that is greater than
        /// double the previous capacity.
        /// <para/>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => _count - _freeCount;

        /// <summary>
        /// Gets the total numbers of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity => _entries?.Length ?? 0;

        /// <summary>
        /// Gets a value indicating whether a collection is read-only.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool ICollection<T>.IsReadOnly => false;

        #endregion ICollection<T> methods

        #region AlternateLookup

#if FEATURE_IALTERNATEEQUALITYCOMPARER
        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="HashSet{T}"/>
        /// using a <typeparamref name="TAlternate"/> instead of a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TAlternate">The alternate type of instance for performing lookups.</typeparam>
        /// <returns>The created lookup instance.</returns>
        /// <exception cref="InvalidOperationException">The set's comparer is not compatible with <typeparamref name="TAlternate"/>.</exception>
        /// <remarks>
        /// The set must be using a comparer that implements <see cref="IAlternateEqualityComparer{TAlternate, T}"/> with
        /// <typeparamref name="TAlternate"/> and <typeparamref name="T"/>. If it doesn't, an exception will be thrown.
        /// </remarks>
        public AlternateLookup<TAlternate> GetAlternateLookup<TAlternate>()
            where TAlternate : allows ref struct
        {
            if (!AlternateLookup<TAlternate>.IsCompatibleItem(this))
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IncompatibleComparer);
            }

            return new AlternateLookup<TAlternate>(this);
        }

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="HashSet{T}"/>
        /// using a <typeparamref name="TAlternate"/> instead of a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TAlternate">The alternate type of instance for performing lookups.</typeparam>
        /// <param name="lookup">The created lookup instance when the method returns true, or a default instance that should not be used if the method returns false.</param>
        /// <returns>true if a lookup could be created; otherwise, false.</returns>
        /// <remarks>
        /// The set must be using a comparer that implements <see cref="IAlternateEqualityComparer{TAlternate, T}"/> with
        /// <typeparamref name="TAlternate"/> and <typeparamref name="T"/>. If it doesn't, the method returns false.
        /// </remarks>
        public bool TryGetAlternateLookup<TAlternate>(out AlternateLookup<TAlternate> lookup)
            where TAlternate : allows ref struct
        {
            if (AlternateLookup<TAlternate>.IsCompatibleItem(this))
            {
                lookup = new AlternateLookup<TAlternate>(this);
                return true;
            }

            lookup = default;
            return false;
        }

        /// <summary>
        /// Provides a type that may be used to perform operations on a <see cref="HashSet{T}"/>
        /// using a <typeparamref name="TAlternate"/> instead of a <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TAlternate">The alternate type of instance for performing lookups.</typeparam>
        public struct AlternateLookup<TAlternate> where TAlternate : allows ref struct
        {
            /// <summary>Initialize the instance. The set must have already been verified to have a compatible comparer.</summary>
            internal AlternateLookup(HashSet<T> set)
            {
                Debug.Assert(set is not null);
                Debug.Assert(IsCompatibleItem(set));
                Set = set;
            }

            /// <summary>Gets the <see cref="HashSet{T}"/> against which this instance performs operations.</summary>
            public HashSet<T> Set { get; }

            /// <summary>Checks whether the set has a comparer compatible with <typeparamref name="TAlternate"/>.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static bool IsCompatibleItem(HashSet<T> set)
            {
                Debug.Assert(set is not null);
                return set._comparer is IAlternateEqualityComparer<TAlternate, T>;
            }

            /// <summary>Gets the set's alternate comparer. The set must have already been verified as compatible.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static IAlternateEqualityComparer<TAlternate, T> GetAlternateComparer(HashSet<T> set)
            {
                Debug.Assert(IsCompatibleItem(set));
                return Unsafe.As<IAlternateEqualityComparer<TAlternate, T>>(set._comparer)!;
            }

            /// <summary>Adds the specified element to a set.</summary>
            /// <param name="item">The element to add to the set.</param>
            /// <returns>true if the element is added to the set; false if the element is already present.</returns>
            public bool Add(TAlternate item)
            {
                HashSet<T> set = Set;
                IAlternateEqualityComparer<TAlternate, T> comparer = GetAlternateComparer(set);

                if (set._buckets == null)
                {
                    set.Initialize(0);
                }
                Debug.Assert(set._buckets != null);

                Entry[]? entries = set._entries;
                Debug.Assert(entries != null, "expected entries to be non-null");

                int hashCode;

                uint collisionCount = 0;
                ref int bucket = ref Unsafe.NullRef<int>();

                Debug.Assert(comparer is not null);
                hashCode = comparer.GetHashCode(item);
                bucket = ref set.GetBucketRef(hashCode);
                int i = bucket - 1; // Value in _buckets is 1-based
                while (i >= 0)
                {
                    ref Entry entry = ref entries[i];
                    if (entry.HashCode == hashCode && comparer.Equals(item, entry.Value))
                    {
                        return false;
                    }
                    i = entry.Next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop, which means a concurrent update has happened.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }

                // Invoke comparer.Map before allocating space in the collection in order to avoid corrupting
                // the collection if the operation fails.
                T mappedItem = comparer.Create(item);

                int index;
                if (set._freeCount > 0)
                {
                    index = set._freeList;
                    set._freeCount--;
                    Debug.Assert((StartOfFreeList - entries![set._freeList].Next) >= -1, "shouldn't overflow because `next` cannot underflow");
                    set._freeList = StartOfFreeList - entries[set._freeList].Next;
                }
                else
                {
                    int count = set._count;
                    if (count == entries.Length)
                    {
                        set.Resize();
                        bucket = ref set.GetBucketRef(hashCode);
                    }
                    index = count;
                    set._count = count + 1;
                    entries = set._entries;
                }

                {
                    ref Entry entry = ref entries![index];
                    entry.HashCode = hashCode;
                    entry.Next = bucket - 1; // Value in _buckets is 1-based
                    entry.Value = mappedItem;
                    bucket = index + 1;
                    set._version++;
                }

                // Value types never rehash
                if (!typeof(T).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold && comparer is NonRandomizedStringEqualityComparer)
                {
                    // If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
                    // i.e. EqualityComparer<string>.Default.
                    set.Resize(entries.Length, forceNewHashCodes: true);
                }

                return true;
            }

            /// <summary>Removes the specified element from a set.</summary>
            /// <param name="item">The element to remove.</param>
            /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
            public bool Remove(TAlternate item)
            {
                HashSet<T> set = Set;
                IAlternateEqualityComparer<TAlternate, T> comparer = GetAlternateComparer(set);

                if (set._buckets != null)
                {
                    Entry[]? entries = set._entries;
                    Debug.Assert(entries != null, "entries should be non-null");

                    uint collisionCount = 0;
                    int last = -1;

                    int hashCode = item is not null ? comparer!.GetHashCode(item) : 0;

                    ref int bucket = ref set.GetBucketRef(hashCode);
                    int i = bucket - 1; // Value in buckets is 1-based

                    while (i >= 0)
                    {
                        ref Entry entry = ref entries[i];

                        if (entry.HashCode == hashCode && comparer.Equals(item, entry.Value))
                        {
                            if (last < 0)
                            {
                                bucket = entry.Next + 1; // Value in buckets is 1-based
                            }
                            else
                            {
                                entries[last].Next = entry.Next;
                            }

                            Debug.Assert((StartOfFreeList - set._freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                            entry.Next = StartOfFreeList - set._freeList;

                            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                            {
                                entry.Value = default!;
                            }

                            set._freeList = i;
                            set._freeCount++;
                            return true;
                        }

                        last = i;
                        i = entry.Next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }

                return false;
            }

            /// <summary>Determines whether a set contains the specified element.</summary>
            /// <param name="item">The element to locate in the set.</param>
            /// <returns>true if the set contains the specified element; otherwise, false.</returns>
            public bool Contains(TAlternate item) => !Unsafe.IsNullRef(in FindValue(item));

            /// <summary>Searches the set for a given value and returns the equal value it finds, if any.</summary>
            /// <param name="equalValue">The value to search for.</param>
            /// <param name="actualValue">The value from the set that the search found, or the default value of <typeparamref name="T"/> when the search yielded no match.</param>
            /// <returns>A value indicating whether the search was successful.</returns>
            public bool TryGetValue(TAlternate equalValue, [MaybeNullWhen(false)] out T actualValue)
            {
                ref readonly T value = ref FindValue(equalValue);
                if (!Unsafe.IsNullRef(in value))
                {
                    actualValue = value;
                    return true;
                }

                actualValue = default!;
                return false;
            }

            /// <summary>Finds the item in the set and returns a reference to the found item, or a null reference if not found.</summary>
            internal ref readonly T FindValue(TAlternate item)
            {
                HashSet<T> set = Set;
                IAlternateEqualityComparer<TAlternate, T> comparer = GetAlternateComparer(set);

                ref Entry entry = ref Unsafe.NullRef<Entry>();
                if (set._buckets != null)
                {
                    Debug.Assert(set._entries != null, "expected entries to be != null");

                    int hashCode = comparer.GetHashCode(item);
                    int i = set.GetBucketRef(hashCode);
                    Entry[]? entries = set._entries;
                    uint collisionCount = 0;
                    i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                    do
                    {
                        // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                        // Test in if to drop range check for following array access
                        if ((uint)i >= (uint)entries.Length)
                        {
                            goto ReturnNotFound;
                        }

                        entry = ref entries[i];
                        if (entry.HashCode == hashCode && comparer.Equals(item, entry.Value))
                        {
                            goto ReturnFound;
                        }

                        i = entry.Next;

                        collisionCount++;
                    } while (collisionCount <= (uint)entries.Length);

                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    goto ConcurrentOperation;
                }

                goto ReturnNotFound;

            ConcurrentOperation:
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            ReturnFound:
                ref readonly T value = ref entry.Value;
            Return:
                return ref value;
            ReturnNotFound:
                value = ref Unsafe.NullRef<T>();
                goto Return;
            }
        }
#endif

        #endregion AlternateLookup

        #region IEnumerable methods

        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <returns>An <see cref="Enumerator"/> object (boxed as <see cref="IEnumerator{T}"/>) for the
        /// <see cref="HashSet{T}"/> object.</returns>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
        /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
        /// <para/>
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
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
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            Count == 0 ? SZGenericArrayEnumerator<T>.Empty :
            GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        #endregion IEnumerable methods

        #region ISerializable methods

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and returns the data
        /// needed to serialize a <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains
        /// the information required to serialize the <see cref="HashSet{T}"/> object.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure that
        /// contains the source and destination of the serialized stream associated with the <see cref="HashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Calling this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        /// <permission cref="System.Security.Permissions.SecurityPermissionAttribute">
        /// for providing serialization services. Security action: <see cref="System.Security.Permissions.SecurityAction.LinkDemand"/>.
        /// Associated enumeration: <see cref="System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter"/>
        /// </permission>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);
            }

            info.AddValue(VersionName, _version); // need to serialize version to avoid problems with serializing while enumerating
            info.AddValue(EqualityComparerName, EqualityComparer, typeof(IEqualityComparer<T>));
            info.AddValue(CapacityName, _buckets == null ? 0 : _buckets.Length);

            if (_buckets != null)
            {
                var array = new T[Count];
                CopyTo(array);
                info.AddValue(ElementsName, array, typeof(T[]));
            }
        }

#endif

        #endregion ISerializable methods

        #region IDeserializationCallback methods

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and raises the
        /// deserialization event when the deserialization is complete.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> object associated with the
        /// current <see cref="HashSet{T}"/> object is invalid.
        /// </exception>
        /// <remarks>
        /// Calling this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public virtual void OnDeserialization(object? sender)
        {
            HashHelpers.SerializationInfoTable.TryGetValue(this, out System.Runtime.Serialization.SerializationInfo? siInfo);

            if (siInfo == null)
            {
                // It might be necessary to call OnDeserialization from a container if the
                // container object also implements OnDeserialization. We can return immediately
                // if this function is called twice. Note we set _siInfo to null at the end of this method.
                return;
            }

            int capacity = siInfo.GetInt32(CapacityName);
            _comparer = (IEqualityComparer<T>)siInfo.GetValue(EqualityComparerName, typeof(IEqualityComparer<T>))!;
            _freeList = -1;
            _freeCount = 0;

            if (capacity != 0)
            {
                _buckets = new int[capacity];
                _entries = new Entry[capacity];
                if (IntPtr.Size == 8) // 64-bit process
                {
                    _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)capacity);
                }

                T[]? array = (T[]?)siInfo.GetValue(ElementsName, typeof(T[]));
                if (array == null)
                {
                    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingKeys);
                }

                // There are no resizes here because we already set capacity above.
                for (int i = 0; i < array.Length; i++)
                {
                    AddIfNotPresent(array[i], out _);
                }
            }
            else
            {
                _buckets = null;
            }

            _version = siInfo.GetInt32(VersionName);
            HashHelpers.SerializationInfoTable.Remove(this);
        }

#endif

        #endregion IDeserializationCallback methods

        #region HashSet methods

        /// <summary>
        /// Adds the specified element to a set.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns><c>true</c> if the element is added to the <see cref="HashSet{T}"/> object;
        /// <c>false</c> if the element is already present.</returns>
        /// <remarks>
        /// If <see cref="Count"/> already equals the capacity of the <see cref="HashSet{T}"/> object,
        /// the capacity is automatically adjusted to accommodate the new item.
        /// <para/>
        /// If <see cref="Count"/> is less than the capacity of the internal array, this method is an
        /// O(1) operation. If the <see cref="HashSet{T}"/> object must be resized, this method
        /// becomes an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool Add(T item) => AddIfNotPresent(item, out _);

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
        public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
        {
            if (_buckets != null)
            {
                int index = FindItemIndex(equalValue);
                if (index >= 0)
                {
                    actualValue = _entries![index].Value;
                    return true;
                }
            }

            actualValue = default;
            return false;
        }

        /// <summary>
        /// Modifies the current <see cref="HashSet{T}"/> object to contain all elements that are present
        /// in itself, the specified collection, or both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="HashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in the
        /// <paramref name="other"/> parameter.
        /// </remarks>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            foreach (T item in other)
            {
                AddIfNotPresent(item, out _);
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            // Intersection of anything with empty set is empty set, so return if count is 0.
            // Same if the set intersecting with itself is the same set.
            if (Count == 0 || other == this)
            {
                return;
            }

            // If other is known to be empty, intersection is empty set; remove all elements, and we're done.
            if (other is ICollection<T> otherAsCollection)
            {
                if (otherAsCollection.Count == 0)
                {
                    Clear();
                    return;
                }

                // Faster if other is a hashset using same equality comparer; so check
                // that other is a hashset using the same equality comparer.
                if (other is ISet<T> otherAsSet && EqualityComparersAreEqual(this, otherAsSet)) // J2N: Use ISet<T> to support other implementations
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            // This is already the empty set; return.
            if (Count == 0)
            {
                return;
            }

            // Special case if other is this; a set minus itself is the empty set.
            if (other == this)
            {
                Clear();
                return;
            }

            // Remove every element in other from this.
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            // If set is empty, then symmetric difference is other.
            if (Count == 0)
            {
                UnionWith(other);
                return;
            }

            // Special-case this; the symmetric difference of a set with itself is the empty set.
            if (other == this)
            {
                Clear();
                return;
            }

            // If other is a HashSet, it has unique elements according to its equality comparer,
            // but if they're using different equality comparers, then assumption of uniqueness
            // will fail. So first check if other is a hashset using the same equality comparer;
            // symmetric except is a lot faster and avoids bit array allocations if we can assume
            // uniqueness.
            if (other is ISet<T> otherAsSet && EqualityComparersAreEqual(this, otherAsSet)) // J2N: Use ISet<T> to support other implementations
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            // The empty set is a subset of any set, and a set is a subset of itself.
            // Set is always a subset of itself.
            if (Count == 0 || other == this)
            {
                return true;
            }

            if (other is ICollection<T> otherAsCollection)
            {
                // If this has more elements then it can't be a subset
                if (Count > otherAsCollection.Count)
                {
                    return false;
                }

                // Faster if other has unique elements according to this equality comparer; so check
                // that other is a hashset using the same equality comparer.
                if (other is ISet<T> otherAsSet && EqualityComparersAreEqual(this, otherAsSet)) // J2N: Use ISet<T> to support other implementations
                {
                    return IsSubsetOfHashSetWithSameEC(otherAsSet);
                }
            }

            ElementCount result = CheckUniqueAndUnfoundElements(other, returnIfUnfound: false);
            return result.uniqueCount == Count && result.unfoundCount >= 0;
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            // No set is a proper subset of itself.
            if (other == this)
            {
                return false;
            }

            if (other is ICollection<T> otherAsCollection)
            {
                // No set is a proper subset of a set with less or equal number of elements.
                if (otherAsCollection.Count <= Count)
                {
                    return false;
                }

                // The empty set is a proper subset of anything but the empty set.
                if (Count == 0)
                {
                    return true;
                }

                // Faster if other is a hashset (and we're using same equality comparer).
                if (other is ISet<T> otherAsSet && EqualityComparersAreEqual(this, otherAsSet)) // J2N: Use ISet<T> to support other implementations
                {
                    // This has strictly less than number of items in other, so the following
                    // check suffices for proper subset.
                    return IsSubsetOfHashSetWithSameEC(otherAsSet);
                }
            }

            ElementCount result = CheckUniqueAndUnfoundElements(other, returnIfUnfound: false);
            return result.uniqueCount == Count && result.unfoundCount > 0;
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            // A set is always a superset of itself.
            if (other == this)
            {
                return true;
            }

            // Try to fall out early based on counts.
            if (other is ICollection<T> otherAsCollection)
            {
                // If other is the empty set then this is a superset.
                if (otherAsCollection.Count == 0)
                {
                    return true;
                }

                // Try to compare based on counts alone if other is a hashset with same equality comparer.
                if (other is ISet<T> otherAsSet &&
                    EqualityComparersAreEqual(this, otherAsSet) &&
                    otherAsSet.Count > Count) // J2N: Use ISet<T> to support other implementations
                {
                    return false;
                }
            }

            foreach (T element in other)
            {
                if (!Contains(element))
                {
                    return false;
                }
            }

            return true;
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            // The empty set isn't a proper superset of any set, and a set is never a strict superset of itself.
            if (Count == 0 || other == this)
            {
                return false;
            }

            if (other is ICollection<T> otherAsCollection)
            {
                // If other is the empty set then this is a superset.
                if (otherAsCollection.Count == 0)
                {
                    // Note that this has at least one element, based on above check.
                    return true;
                }

                // Faster if other is a hashset with the same equality comparer
                // J2N: JCG.HashSet must be used here, as IsSubsetOfHashSetWithSameEC either
                // does not exist on the other types or is not public
                if (other is HashSet<T> otherAsSet && EqualityComparersAreEqual(this, otherAsSet))
                {
                    if (otherAsSet.Count >= Count)
                    {
                        return false;
                    }

                    // Now perform element check.
                    return otherAsSet.IsSubsetOfHashSetWithSameEC(this);
                }
            }

            // Couldn't fall out in the above cases; do it the long way
            ElementCount result = CheckUniqueAndUnfoundElements(other, returnIfUnfound: true);
            return result.uniqueCount < Count && result.unfoundCount == 0;
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            if (Count == 0)
            {
                return false;
            }

            // Set overlaps itself
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);
            }

            // A set is equal to itself.
            if (other == this)
            {
                return true;
            }

            if (other is ICollection<T> otherAsCollection)
            {
                // If this is empty, they are equal iff other is empty.
                if (Count == 0)
                {
                    return otherAsCollection.Count == 0;
                }

                // Faster if other is a hashset and we're using same equality comparer.
                if (other is ISet<T> otherAsSet && EqualityComparersAreEqual(this, otherAsSet)) // J2N: Use ISet<T> to support other implementations
                {
                    // Attempt to return early: since both contain unique elements, if they have
                    // different counts, then they can't be equal.
                    if (Count != otherAsSet.Count)
                    {
                        return false;
                    }

                    // Already confirmed that the sets have the same number of distinct elements, so if
                    // one is a subset of the other then they must be equal.
                    return IsSubsetOfHashSetWithSameEC(otherAsSet);
                }

                // Can't be equal if other set contains fewer elements than this.
                if (Count > otherAsCollection.Count)
                {
                    return false;
                }
            }

            ElementCount result = CheckUniqueAndUnfoundElements(other, returnIfUnfound: true);
            return result.uniqueCount == Count && result.unfoundCount == 0;
        }

        /// <summary>
        /// Copies the elements of a <see cref="HashSet{T}"/> object to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="HashSet{T}"/> object.
        /// The array must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void CopyTo(T[] array) => CopyTo(array, 0, Count);

        /// <summary>
        /// Copies the elements of a <see cref="HashSet{T}"/> collection to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="HashSet{T}"/> object.
        /// The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="arrayIndex"/> is greater than the length of the destination <paramref name="array"/>.
        /// </exception>
        /// <remarks>
        /// Calling this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex) => CopyTo(array, arrayIndex, Count);

        /// <summary>
        /// Copies the specified number of elements of a <see cref="HashSet{T}"/>
        /// object to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="HashSet{T}"/> object.
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
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            ThrowIfNegative(arrayIndex);
            ThrowIfNegative(count);

            // Will the array, starting at arrayIndex, be able to hold elements? Note: not
            // checking arrayIndex >= array.Length (consistency with list of allowing
            // count of 0; subsequent check takes care of the rest)
            if (arrayIndex > array.Length || count > array.Length - arrayIndex)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }

            Entry[]? entries = _entries;
            for (int i = 0; i < _count && count != 0; i++)
            {
                ref Entry entry = ref entries![i];
                if (entry.Next >= -1)
                {
                    array[arrayIndex++] = entry.Value;
                    count--;
                }
            }
        }

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified
        /// predicate from a <see cref="HashSet{T}"/> collection.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines
        /// the conditions of the elements to remove.</param>
        /// <returns>The number of elements that were removed from the
        /// <see cref="HashSet{T}"/> collection.</returns>
        public int RemoveWhere(Predicate<T> match)
        {
            if (match is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            Entry[]? entries = _entries;
            int numRemoved = 0;
            for (int i = 0; i < _count; i++)
            {
                ref Entry entry = ref entries![i];
                if (entry.Next >= -1)
                {
                    // Cache value in case delegate removes it
                    T value = entry.Value;
                    if (match(value))
                    {
                        // Check again that remove actually removed it.
                        if (Remove(value))
                        {
                            numRemoved++;
                        }
                    }
                }
            }

            return numRemoved;
        }

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> object that is used
        /// to determine equality for the values in the set.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public IEqualityComparer<T> EqualityComparer
        {
            get
            {
                if (typeof(T) == typeof(string))
                {
                    Debug.Assert(_comparer is not null, "The comparer should never be null for a reference type.");
                    return (IEqualityComparer<T>)InternalStringEqualityComparer.GetUnderlyingEqualityComparer((IEqualityComparer<string?>)_comparer!);
                }
                else
                {
                    return _comparer ?? EqualityComparer<T>.Default;
                }
            }
        }

        /// <summary>
        /// Similar to <see cref="EqualityComparer"/> but surfaces the actual comparer being used to hash entries.
        /// </summary>
        internal IEqualityComparer<T> EffectiveComparer => _comparer ?? EqualityComparer<T>.Default;

        /// <summary>
        /// Ensures that this hash set can hold the specified number of elements without growing.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            }

            int currentCapacity = _entries == null ? 0 : _entries.Length;
            if (currentCapacity >= capacity)
            {
                return currentCapacity;
            }

            if (_buckets == null)
            {
                return Initialize(capacity);
            }

            int newSize = HashHelpers.GetPrime(capacity);
            Resize(newSize, forceNewHashCodes: false);
            return newSize;
        }

        private void Resize() => Resize(HashHelpers.ExpandPrime(_count), forceNewHashCodes: false);

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            // Value types never rehash
            Debug.Assert(!forceNewHashCodes || !typeof(T).IsValueType);
            Debug.Assert(_entries != null, "_entries should be non-null");
            Debug.Assert(newSize >= _entries!.Length);

            var entries = new Entry[newSize];

            int count = _count;
            Array.Copy(_entries, entries, count);

            if (!typeof(T).IsValueType && forceNewHashCodes)
            {
                Debug.Assert(_comparer is NonRandomizedStringEqualityComparer);
                IEqualityComparer<T> comparer = _comparer = (IEqualityComparer<T>)((NonRandomizedStringEqualityComparer)_comparer!).GetRandomizedEqualityComparer();

                for (int i = 0; i < count; i++)
                {
                    ref Entry entry = ref entries[i];
                    if (entry.Next >= -1)
                    {
                        entry.HashCode = entry.Value != null ? comparer.GetHashCode(entry.Value) : 0;
                    }
                }
            }

            // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
            _buckets = new int[newSize];
            if (IntPtr.Size == 8) // 64-bit process
            {
                _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);
            }
            for (int i = 0; i < count; i++)
            {
                ref Entry entry = ref entries[i];
                if (entry.Next >= -1)
                {
                    ref int bucket = ref GetBucketRef(entry.HashCode);
                    entry.Next = bucket - 1; // Value in _buckets is 1-based
                    bucket = i + 1;
                }
            }

            _entries = entries;
        }

        /// <summary>
        /// Sets the capacity of a <see cref="HashSet{T}"/> object to the actual
        /// number of elements it contains, rounded up to a nearby, implementation-specific value.
        /// </summary>
        /// <remarks>
        /// You can use the <see cref="TrimExcess()"/> method to minimize a <see cref="HashSet{T}"/>
        /// object's memory overhead once it is known that no new elements will be added. To completely
        /// clear a <see cref="HashSet{T}"/> object and release all memory referenced by it,
        /// call this method after calling the <see cref="Clear()"/> method.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void TrimExcess() => TrimExcess(Count);

        /// <summary>
        /// Sets the capacity of a <see cref="HashSet{T}"/> object to the specified number of entries,
        /// rounded up to a nearby, implementation-specific value.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException">Passed capacity is lower than entries count.</exception>
        public void TrimExcess(int capacity)
        {
            ThrowIfLessThan(capacity, Count);

            int newSize = HashHelpers.GetPrime(capacity);
            Entry[]? oldEntries = _entries;
            int currentCapacity = oldEntries == null ? 0 : oldEntries.Length;
            if (newSize >= currentCapacity)
            {
                return;
            }

            int oldCount = _count;
            _version++;
            Initialize(newSize);
            Entry[]? entries = _entries;
            int count = 0;
            for (int i = 0; i < oldCount; i++)
            {
                int hashCode = oldEntries![i].HashCode; // At this point, we know we have entries.
                if (oldEntries[i].Next >= -1)
                {
                    ref Entry entry = ref entries![count];
                    entry = oldEntries[i];
                    ref int bucket = ref GetBucketRef(hashCode);
                    entry.Next = bucket - 1; // Value in _buckets is 1-based
                    bucket = count + 1;
                    count++;
                }
            }

            _count = count;
            _freeCount = 0;
        }

        #endregion HashSet methods

        #region Helper methods

        /// <summary>
        /// Returns an <see cref="IEqualityComparer"/> object that can be used
        /// for equality testing of a <see cref="HashSet{T}"/> object
        /// as well as any nested objects that implement <see cref="IStructuralEquatable"/>.
        /// <para/>
        /// Usage Note: This is exactly the same as <see cref="SetEqualityComparer{T}.Default"/>.
        /// It is included here to cover the <see cref="SCG.HashSet{T}"/> API.
        /// </summary>
        /// <returns>An <see cref="IEqualityComparer"/> object that can be used for deep
        /// equality testing of the <see cref="HashSet{T}"/> object.</returns>
        /// <remarks>
        /// The <see cref="IEqualityComparer"/> object checks for equality for multiple levels.
        /// Nested reference types that implement <see cref="IStructuralEquatable"/> are also compared.
        /// </remarks>
        public static IEqualityComparer<HashSet<T>> CreateSetComparer() => new HashSetEqualityComparer<T>();

        /// <summary>
        /// Initializes buckets and slots arrays. Uses suggested capacity by finding next prime
        /// greater than or equal to capacity.
        /// </summary>
        /// <param name="capacity"></param>
        private int Initialize(int capacity)
        {
            int size = HashHelpers.GetPrime(capacity);
            var buckets = new int[size];
            var entries = new Entry[size];

            // Assign member variables after both arrays are allocated to guard against corruption from OOM if second fails.
            _freeList = -1;
            _buckets = buckets;
            _entries = entries;
            if (IntPtr.Size == 8) // 64-bit process
            {
                _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size);
            }

            return size;
        }

        /// <summary>Adds the specified element to the set if it's not already contained.</summary>
        /// <param name="value">The element to add to the set.</param>
        /// <param name="location">The index into <see cref="_entries"/> of the element.</param>
        /// <returns>true if the element is added to the <see cref="HashSet{T}"/> object; false if the element is already present.</returns>
        private bool AddIfNotPresent(T value, out int location)
        {
            if (_buckets == null)
            {
                Initialize(0);
            }
            Debug.Assert(_buckets != null);

            Entry[]? entries = _entries;
            Debug.Assert(entries != null, "expected entries to be non-null");

            IEqualityComparer<T>? comparer = _comparer;
            int hashCode;

            uint collisionCount = 0;
            ref int bucket = ref UnsafeHelpers.NullRef<int>();

            if (typeof(T).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                comparer == null)
            {
                hashCode = value!.GetHashCode();
                bucket = ref GetBucketRef(hashCode);
                int i = bucket - 1; // Value in _buckets is 1-based

                // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                while (i >= 0)
                {
                    ref Entry entry = ref entries![i];
                    if (entry.HashCode == hashCode && EqualityComparer<T>.Default.Equals(entry.Value, value))
                    {
                        location = i;
                        return false;
                    }
                    i = entry.Next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop, which means a concurrent update has happened.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }
            else
            {
                Debug.Assert(comparer is not null);
                hashCode = value != null ? comparer!.GetHashCode(value) : 0;
                bucket = ref GetBucketRef(hashCode);
                int i = bucket - 1; // Value in _buckets is 1-based
                while (i >= 0)
                {
                    ref Entry entry = ref entries![i];
                    if (entry.HashCode == hashCode && comparer!.Equals(entry.Value, value))
                    {
                        location = i;
                        return false;
                    }
                    i = entry.Next;

                    collisionCount++;
                    if (collisionCount > (uint)entries.Length)
                    {
                        // The chain of entries forms a loop, which means a concurrent update has happened.
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }

            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                _freeCount--;
                Debug.Assert((StartOfFreeList - entries![_freeList].Next) >= -1, "shouldn't overflow because `next` cannot underflow");
                _freeList = StartOfFreeList - entries[_freeList].Next;
            }
            else
            {
                int count = _count;
                if (count == entries!.Length)
                {
                    Resize();
                    bucket = ref GetBucketRef(hashCode);
                }
                index = count;
                _count = count + 1;
                entries = _entries;
            }

            {
                ref Entry entry = ref entries![index];
                entry.HashCode = hashCode;
                entry.Next = bucket - 1; // Value in _buckets is 1-based
                entry.Value = value;
                bucket = index + 1;
                _version++;
                location = index;
            }

            // Value types never rehash
            if (!typeof(T).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold && comparer is NonRandomizedStringEqualityComparer)
            {
                // If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
                // i.e. EqualityComparer<string>.Default.
                Resize(entries.Length, forceNewHashCodes: true);
                location = FindItemIndex(value);
                Debug.Assert(location >= 0);
            }

            return true;
        }

        /// <summary>
        /// Implementation Notes:
        /// If other is a hashset and is using same equality comparer, then checking subset is
        /// faster. Simply check that each element in this is in other.
        ///
        /// Note: if other doesn't use same equality comparer, then Contains check is invalid,
        /// which is why callers must take are of this.
        ///
        /// If callers are concerned about whether this is a proper subset, they take care of that.
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        internal bool IsSubsetOfHashSetWithSameEC(ISet<T> other) // J2N: parameter was HashSet<T>
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
        /// If other is a hashset that uses same equality comparer, intersect is much faster
        /// because we can use other's Contains
        /// </summary>
        /// <param name="other"></param>
        private void IntersectWithHashSetWithSameEC(ISet<T> other) // J2N: parameter was HashSet<T>
        {
            Entry[]? entries = _entries;
            for (int i = 0; i < _count; i++)
            {
                ref Entry entry = ref entries![i];
                if (entry.Next >= -1)
                {
                    T item = entry.Value;
                    if (!other.Contains(item))
                    {
                        Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// Iterate over other. If contained in this, mark an element in bit array corresponding to
        /// its position in _slots. If anything is unmarked (in bit array), remove it.
        ///
        /// This attempts to allocate on the stack, if below StackAllocThreshold.
        /// </summary>
        /// <param name="other"></param>
        private unsafe void IntersectWithEnumerable(IEnumerable<T> other)
        {
            Debug.Assert(_buckets != null, "_buckets shouldn't be null; callers should check first");

            // Keep track of current last index; don't want to move past the end of our bit array
            // (could happen if another thread is modifying the collection).
            int originalCount = _count;
            int intArrayLength = BitHelper.ToIntArrayLength(originalCount);

            Span<int> span = stackalloc int[StackAllocThreshold];
            BitHelper bitHelper = intArrayLength <= StackAllocThreshold ?
                new BitHelper(span.Slice(0, intArrayLength), clear: true) :
                new BitHelper(new int[intArrayLength], clear: false);

            // Mark if contains: find index of in slots array and mark corresponding element in bit array.
            foreach (T item in other)
            {
                int index = FindItemIndex(item);
                if (index >= 0)
                {
                    bitHelper.MarkBit(index);
                }
            }

            // If anything unmarked, remove it. Perf can be optimized here if BitHelper had a
            // FindFirstUnmarked method.
            for (int i = 0; i < originalCount; i++)
            {
                ref Entry entry = ref _entries![i];
                if (entry.Next >= -1 && !bitHelper.IsMarked(i))
                {
                    Remove(entry.Value);
                }
            }
        }

        /// <summary>
        /// if other is a set, we can assume it doesn't have duplicate elements, so use this
        /// technique: if can't remove, then it wasn't present in this set, so add.
        ///
        /// As with other methods, callers take care of ensuring that other is a hashset using the
        /// same equality comparer.
        /// </summary>
        /// <param name="other"></param>
        private void SymmetricExceptWithUniqueHashSet(ISet<T> other) // J2N: parameter was HashSet<T>
        {
            foreach (T item in other)
            {
                if (!Remove(item))
                {
                    AddIfNotPresent(item, out _);
                }
            }
        }

        /// <summary>
        /// Implementation notes:
        ///
        /// Used for symmetric except when other isn't a HashSet. This is more tedious because
        /// other may contain duplicates. HashSet technique could fail in these situations:
        /// 1. Other has a duplicate that's not in this: HashSet technique would add then
        /// remove it.
        /// 2. Other has a duplicate that's in this: HashSet technique would remove then add it
        /// back.
        /// In general, its presence would be toggled each time it appears in other.
        ///
        /// This technique uses bit marking to indicate whether to add/remove the item. If already
        /// present in collection, it will get marked for deletion. If added from other, it will
        /// get marked as something not to remove.
        ///
        /// </summary>
        /// <param name="other"></param>
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
                int location;
                if (AddIfNotPresent(item, out location))
                {
                    // wasn't already present in collection; flag it as something not to remove
                    // *NOTE* if location is out of range, we should ignore. BitHelper will
                    // detect that it's out of bounds and not try to mark it. But it's
                    // expected that location could be out of bounds because adding the item
                    // will increase _lastIndex as soon as all the free spots are filled.
                    itemsAddedFromOther.MarkBit(location);
                }
                else
                {
                    // already there...if not added from other, mark for remove.
                    // *NOTE* Even though BitHelper will check that location is in range, we want
                    // to check here. There's no point in checking items beyond originalCount
                    // because they could not have been in the original collection
                    if (location < originalCount && !itemsAddedFromOther.IsMarked(location))
                    {
                        itemsToRemove.MarkBit(location);
                    }
                }
            }

            // if anything marked, remove it
            for (int i = 0; i < originalCount; i++)
            {
                if (itemsToRemove.IsMarked(i))
                {
                    Remove(_entries![i].Value);
                }
            }
        }

        /// <summary>
        /// Determines counts that can be used to determine equality, subset, and superset. This
        /// is only used when other is an IEnumerable and not a HashSet. If other is a HashSet
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
        /// <param name="other"></param>
        /// <param name="returnIfUnfound">Allows us to finish faster for equals and proper superset
        /// because unfoundCount must be 0.</param>
        /// <returns></returns>
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
                int index = FindItemIndex(item);
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
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        private static bool EqualityComparersAreEqual(HashSet<T> set1, IEnumerable<T> set2)
        {
            if (set2 is HashSet<T> hashSet)
                return set1.EqualityComparer.Equals(hashSet.EqualityComparer);
            else if (set2 is LinkedHashSet<T> linkedHashSet)
                return set1.EqualityComparer.Equals(linkedHashSet.EqualityComparer);
            else if (set2 is SCG.HashSet<T> scgHashSet)
                return set1.EqualityComparer.Equals(scgHashSet.Comparer);
            else if (set2 is Net5.HashSet<T> net5HashSet)
                return set1.EqualityComparer.Equals(net5HashSet.EqualityComparer);
            return false;
        }

        /// <summary>
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        internal static bool EqualityComparersAreEqual(HashSet<T> set1, HashSet<T> set2) => set1.EqualityComparer.Equals(set2.EqualityComparer);

        /// <summary>
        /// Checks if effective equality comparers are equal. This is used for algorithms that
        /// require that both collections use identical hashing implementations for their entries.
        /// </summary>
        internal static bool EffectiveEqualityComparersAreEqual(HashSet<T> set1, HashSet<T> set2) => set1.EffectiveComparer.Equals(set2.EffectiveComparer);

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
            => SetEqualityComparer<T>.GetHashCode(this, comparer);

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
            // J2N: Fast path for same-type comparison - if obj is HashSet<T> with same equality comparer,
            // use hash-based lookups instead of the slower SetEqualityComparer (O(n) vs O(n²))
            if (obj is HashSet<T> other && EqualityComparersAreEqual(this, other))
            {
                if (Count != other.Count)
                    return false;
                return other.IsSubsetOfHashSetWithSameEC(this);
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
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        public override string ToString()
            => ToString("{0}", StringFormatter.CurrentCulture);

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider? formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
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

        #region Nested Structures

        // used for set checking operations (using enumerables) that rely on counting
        internal struct ElementCount
        {
            internal int uniqueCount;
            internal int unfoundCount;
        }

        private struct Entry
        {
            public int HashCode;
            /// <summary>
            /// 0-based index of next entry in chain: -1 means end of chain
            /// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
            /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
            /// </summary>
            public int Next;
            public T Value;
        }

        /// <summary>
        /// An enumerator for the <see cref="HashSet{T}"/> class.
        /// </summary>
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly HashSet<T> _hashSet;
            private readonly int _version;
            private int _index;
            private T _current;

            internal Enumerator(HashSet<T> hashSet)
            {
                _hashSet = hashSet;
                _version = hashSet._version;
                _index = 0;
                _current = default!;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="HashSet{T}"/>.
            /// </summary>
            /// <returns>>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                if (_version != _hashSet._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is int.MaxValue
                while ((uint)_index < (uint)_hashSet._count)
                {
                    ref Entry entry = ref _hashSet._entries![_index++];
                    if (entry.Next >= -1)
                    {
                        _current = entry.Value;
                        return true;
                    }
                }

                _index = _hashSet._count + 1;
                _current = default!;
                return false;
            }

            /// <summary>
            /// Gets the element in the <see cref="HashSet{T}"/> at the current position of the enumerator.
            /// </summary>
            public T Current => _current;

            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            public void Dispose()
            {
            }

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || (_index == _hashSet._count + 1))
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return _current;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _hashSet._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                _index = 0;
                _current = default!;
            }
        }

        #endregion Nested Structures
    }
}
