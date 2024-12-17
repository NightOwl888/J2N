// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.ObjectModel;
using J2N.Runtime.CompilerServices;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SCG = System.Collections.Generic;


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
        // store lower 31 bits of hash code
        private const int Lower31BitMask = 0x7FFFFFFF;
        // cutoff point, above which we won't do stackallocs. This corresponds to 100 integers.
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Following Microsoft's code style")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "IDE0051 doesn't fire on all target frameworks")]
        private const int StackAllocThreshold = 100;
        // when constructing a hashset from an existing collection, it may contain duplicates,
        // so this is used as the max acceptable excess ratio of capacity to count. Note that
        // this is only used on the ctor and not to automatically shrink if the hashset has, e.g,
        // a lot of adds followed by removes. Users must explicitly shrink by calling TrimExcess.
        // This is set to 3 because capacity is acceptable as 2x rounded up to nearest prime.
        private const int ShrinkThreshold = 3;

        private int[]? _buckets;
        private Slot[] _slots = default!; // TODO-NULLABLE: This should be Slot[]?, but the resulting annotations causes GenPartialFacadeSource to blow up: error : Unable to cast object of type 'Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax' to type 'Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax'
        private int _count;
        private int _lastIndex;
        private int _freeList;
        private IEqualityComparer<T>? _comparer;
        private int _version;

#if FEATURE_SERIALIZABLE
        private System.Runtime.Serialization.SerializationInfo? _siInfo; // temporary variable needed during deserialization

        // constants for serialization
        private const string CapacityName = "Capacity"; // Do not rename (binary serialization)
        private const string ElementsName = "Elements"; // Do not rename (binary serialization)
        private const string EqualityComparerName = "EqualityComparer"; // Do not rename (binary serialization)
        private const string VersionName = "Version"; // Do not rename (binary serialization)
#endif

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
            if (comparer == EqualityComparer<T>.Default)
            {
                comparer = null;
            }

            _comparer = comparer;
            _lastIndex = 0;
            _count = 0;
            _freeList = -1;
            _version = 0;
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
                throw new ArgumentNullException(nameof(collection));
            }

            var otherAsHashSet = collection as HashSet<T>;
            if (otherAsHashSet != null && AreEqualityComparersEqual(this, otherAsHashSet))
            {
                CopyFrom(otherAsHashSet);
            }
            else
            {
                // to avoid excess resizes, first set size based on collection's count. Collection
                // may contain duplicates, so call TrimExcess if resulting hashset is larger than
                // threshold
                ICollection<T>? coll = collection as ICollection<T>;
                int suggestedCapacity = coll == null ? 0 : coll.Count;
                Initialize(suggestedCapacity);

                UnionWith(collection);

                if (_count > 0 && _slots.Length / _count > ShrinkThreshold)
                {
                    TrimExcess();
                }
            }
        }

        // Initializes the HashSet from another HashSet with the same element type and
        // equality comparer.
        private void CopyFrom(HashSet<T> source)
        {
            int count = source._count;
            if (count == 0)
            {
                // As well as short-circuiting on the rest of the work done,
                // this avoids errors from trying to access otherAsHashSet._buckets
                // or otherAsHashSet._slots when they aren't initialized.
                return;
            }

            int capacity = source._buckets!.Length;
            int threshold = HashHelpers.ExpandPrime(count + 1);

            if (threshold >= capacity)
            {
                _buckets = (int[])source._buckets.Clone();
                _slots = (Slot[])source._slots.Clone();

                _lastIndex = source._lastIndex;
                _freeList = source._freeList;
            }
            else
            {
                int lastIndex = source._lastIndex;
                Slot[] slots = source._slots;
                Initialize(count);
                int index = 0;
                for (int i = 0; i < lastIndex; ++i)
                {
                    int hashCode = slots[i].hashCode;
                    if (hashCode >= 0)
                    {
                        AddValue(index, hashCode, slots[i].value);
                        ++index;
                    }
                }
                Debug.Assert(index == count);
                _lastIndex = index;
            }
            _count = count;
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
                throw new ArgumentOutOfRangeException(nameof(capacity));
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
            _siInfo = info;
        }
#endif

        #endregion

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
        void ICollection<T>.Add(T item)
        {
            AddIfNotPresent(item);
        }

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
            if (_lastIndex > 0)
            {
                Debug.Assert(_buckets != null, "_buckets was null but _lastIndex > 0");

                // clear the elements so that the gc can reclaim the references.
                // clear only up to _lastIndex for _slots
                Array.Clear(_slots, 0, _lastIndex);
                Array.Clear(_buckets, 0, _buckets!.Length);
                _lastIndex = 0;
                _count = 0;
                _freeList = -1;
            }
            _version++;
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
        public bool Contains(T item)
        {
            int[]? buckets = _buckets;

            if (buckets != null)
            {
                int collisionCount = 0;
                Slot[] slots = _slots;
                IEqualityComparer<T>? comparer = _comparer;

                if (comparer == null)
                {
                    int hashCode = item == null ? 0 : InternalGetHashCode(item.GetHashCode());

                    if (default(T)! != null) // TODO-NULLABLE: default(T) == null warning (https://github.com/dotnet/roslyn/issues/34757)
                    {
                        // see note at "HashSet" level describing why "- 1" appears in for loop
                        for (int i = buckets[hashCode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                        {
                            if (slots[i].hashCode == hashCode && EqualityComparer<T>.Default.Equals(slots[i].value, item))
                            {
                                return true;
                            }

                            if (collisionCount >= slots.Length)
                            {
                                // The chain of entries forms a loop, which means a concurrent update has happened.
                                throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                            }
                            collisionCount++;
                        }
                    }
                    else
                    {
                        // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
                        // https://github.com/dotnet/coreclr/issues/17273
                        // So cache in a local rather than get EqualityComparer per loop iteration
                        IEqualityComparer<T> defaultComparer = EqualityComparer<T>.Default;

                        // see note at "HashSet" level describing why "- 1" appears in for loop
                        for (int i = buckets[hashCode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                        {
                            if (slots[i].hashCode == hashCode && defaultComparer.Equals(slots[i].value, item))
                            {
                                return true;
                            }

                            if (collisionCount >= slots.Length)
                            {
                                // The chain of entries forms a loop, which means a concurrent update has happened.
                                throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                            }
                            collisionCount++;
                        }
                    }
                }
                else
                {
                    int hashCode = item == null ? 0 : InternalGetHashCode(comparer.GetHashCode(item));

                    // see note at "HashSet" level describing why "- 1" appears in for loop
                    for (int i = buckets[hashCode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                    {
                        if (slots[i].hashCode == hashCode && comparer.Equals(slots[i].value, item))
                        {
                            return true;
                        }

                        if (collisionCount >= slots.Length)
                        {
                            // The chain of entries forms a loop, which means a concurrent update has happened.
                            throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                        }
                        collisionCount++;
                    }
                }
            }

            // either _buckets is null or wasn't found
            return false;
        }

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
        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex, _count);
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
            int hashCode;
            int bucket;
            int last = -1;
            int collisionCount = 0;
            int i;
            Slot[] slots;
            IEqualityComparer<T>? comparer = _comparer;

            if (_buckets != null)
            {
                slots = _slots;

                if (comparer == null)
                {
                    hashCode = item == null ? 0 : InternalGetHashCode(item.GetHashCode());
                    bucket = hashCode % _buckets!.Length;

                    if (default(T)! != null) // TODO-NULLABLE: default(T) == null warning (https://github.com/dotnet/roslyn/issues/34757)
                    {
                        for (i = _buckets[bucket] - 1; i >= 0; last = i, i = slots[i].next)
                        {
                            if (slots[i].hashCode == hashCode && EqualityComparer<T>.Default.Equals(slots[i].value, item))
                            {
                                goto ReturnFound;
                            }

                            if (collisionCount >= slots.Length)
                            {
                                // The chain of entries forms a loop, which means a concurrent update has happened.
                                throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                            }
                            collisionCount++;
                        }
                    }
                    else
                    {
                        // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
                        // https://github.com/dotnet/coreclr/issues/17273
                        // So cache in a local rather than get EqualityComparer per loop iteration
                        IEqualityComparer<T> defaultComparer = EqualityComparer<T>.Default;

                        for (i = _buckets[bucket] - 1; i >= 0; last = i, i = slots[i].next)
                        {
                            if (slots[i].hashCode == hashCode && defaultComparer.Equals(slots[i].value, item))
                            {
                                goto ReturnFound;
                            }

                            if (collisionCount >= slots.Length)
                            {
                                // The chain of entries forms a loop, which means a concurrent update has happened.
                                throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                            }
                            collisionCount++;
                        }
                    }
                }
                else
                {
                    hashCode = item == null ? 0 : InternalGetHashCode(comparer.GetHashCode(item));
                    bucket = hashCode % _buckets!.Length;

                    for (i = _buckets[bucket] - 1; i >= 0; last = i, i = slots[i].next)
                    {
                        if (slots[i].hashCode == hashCode && comparer.Equals(slots[i].value, item))
                        {
                            goto ReturnFound;
                        }

                        if (collisionCount >= slots.Length)
                        {
                            // The chain of entries forms a loop, which means a concurrent update has happened.
                            throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                        }
                        collisionCount++;
                    }
                }
            }
            // either _buckets is null or wasn't found
            return false;

        ReturnFound:
            if (last < 0)
            {
                // first iteration; update buckets
                _buckets[bucket] = slots[i].next + 1;
            }
            else
            {
                // subsequent iterations; update 'next' pointers
                slots[last].next = slots[i].next;
            }
            slots[i].hashCode = -1;
            if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
            {
                slots[i].value = default!;
            }
            slots[i].next = _freeList;

            _count--;
            _version++;
            if (_count == 0)
            {
                _lastIndex = 0;
                _freeList = -1;
            }
            else
            {
                _freeList = i;
            }
            return true;
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
        public int Count => _count;

        /// <summary>
        /// Gets a value indicating whether a collection is read-only.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        bool ICollection<T>.IsReadOnly => false;

        #endregion

        #region IEnumerable methods

        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object for the <see cref="HashSet{T}"/> object.</returns>
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
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

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
        [System.Security.SecurityCritical]
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(VersionName, _version); // need to serialize version to avoid problems with serializing while enumerating
            info.AddValue(EqualityComparerName, _comparer ?? EqualityComparer<T>.Default, typeof(IEqualityComparer<T>));
            info.AddValue(CapacityName, _buckets == null ? 0 : _buckets.Length);

            if (_buckets != null)
            {
                T[] array = new T[_count];
                CopyTo(array);
                info.AddValue(ElementsName, array, typeof(T[]));
            }
        }

#endif

        #endregion

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
            if (_siInfo == null)
            {
                // It might be necessary to call OnDeserialization from a container if the
                // container object also implements OnDeserialization. We can return immediately
                // if this function is called twice. Note we set _siInfo to null at the end of this method.
                return;
            }

            int capacity = _siInfo.GetInt32(CapacityName);
            _comparer = (IEqualityComparer<T>)_siInfo.GetValue(EqualityComparerName, typeof(IEqualityComparer<T>))!;
            _freeList = -1;

            if (capacity != 0)
            {
                _buckets = new int[capacity];
                _slots = new Slot[capacity];

                T[]? array = (T[]?)_siInfo.GetValue(ElementsName, typeof(T[]));

                if (array == null)
                {
                    throw new System.Runtime.Serialization.SerializationException(SR.Serialization_MissingKeys);
                }

                // there are no resizes here because we already set capacity above
                for (int i = 0; i < array.Length; i++)
                {
                    AddIfNotPresent(array[i]);
                }
            }
            else
            {
                _buckets = null;
            }

            _version = _siInfo.GetInt32(VersionName);
            _siInfo = null;
        }

#endif

        #endregion

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
        public bool Add(T item)
        {
            return AddIfNotPresent(item);
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
        public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
        {
            if (_buckets != null)
            {
                int i = InternalIndexOf(equalValue);
                if (i >= 0)
                {
                    actualValue = _slots[i].value;
                    return true;
                }
            }
            actualValue = default!;
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
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (T item in other)
            {
                AddIfNotPresent(item);
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
        [System.Security.SecurityCritical]
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    IntersectWithHashSetWithSameEC(otherAsCollection);
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
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
        [System.Security.SecurityCritical]
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
            if (AreEqualityComparersEqual(this, other))
            {
                SymmetricExceptWithUniqueHashSet(other);
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
        [System.Security.SecurityCritical]
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
            if (otherAsCollection != null && AreEqualityComparersEqual(this, otherAsCollection))
            {
                // if this has more elements then it can't be a subset
                if (_count > otherAsCollection.Count)
                {
                    return false;
                }

                // already checked that we're using same equality comparer. simply check that
                // each element in this is contained in other.
                return IsSubsetOfHashSetWithSameEC(otherAsCollection);
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
        [System.Security.SecurityCritical]
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
                if (AreEqualityComparersEqual(this, otherAsCollection))
                {
                    if (_count >= otherAsCollection.Count)
                    {
                        return false;
                    }
                    // this has strictly less than number of items in other, so the following
                    // check suffices for proper subset.
                    return IsSubsetOfHashSetWithSameEC(otherAsCollection);
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
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
        [System.Security.SecurityCritical]
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
        [System.Security.SecurityCritical]
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
        /// Copies the elements of a <see cref="HashSet{T}"/> object to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="HashSet{T}"/> object.
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
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            // check array index valid index into array
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, SR.ArgumentOutOfRange_NeedNonNegNum);

            // also throw if count less than 0
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, SR.ArgumentOutOfRange_NeedNonNegNum);

            // will array, starting at arrayIndex, be able to hold elements? Note: not
            // checking arrayIndex >= array.Length (consistency with list of allowing
            // count of 0; subsequent check takes care of the rest)
            if (arrayIndex > array.Length || count > array.Length - arrayIndex)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            int numCopied = 0;
            for (int i = 0; i < _lastIndex && numCopied < count; i++)
            {
                if (_slots[i].hashCode >= 0)
                {
                    array[arrayIndex + numCopied] = _slots[i].value;
                    numCopied++;
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
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            int numRemoved = 0;
            for (int i = 0; i < _lastIndex; i++)
            {
                if (_slots[i].hashCode >= 0)
                {
                    // cache value in case delegate removes it
                    T value = _slots[i].value;
                    if (match(value))
                    {
                        // check again that remove actually removed it
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
        public IEqualityComparer<T> EqualityComparer => _comparer ?? EqualityComparer<T>.Default;

        /// <summary>
        /// Ensures that this hash set can hold the specified number of elements without growing.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            int currentCapacity = _slots == null ? 0 : _slots.Length;
            if (currentCapacity >= capacity)
                return currentCapacity;
            if (_buckets == null)
                return Initialize(capacity);

            int newSize = HashHelpers.GetPrime(capacity);
            SetCapacity(newSize);
            return newSize;
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
        public void TrimExcess()
        {
            Debug.Assert(_count >= 0, "_count is negative");

            if (_count == 0)
            {
                // if count is zero, clear references
                _buckets = null;
                _slots = null!;
                _version++;
            }
            else
            {
                Debug.Assert(_buckets != null, "_buckets was null but _count > 0");

                // similar to IncreaseCapacity but moves down elements in case add/remove/etc
                // caused fragmentation
                int newSize = HashHelpers.GetPrime(_count);
                Slot[] newSlots = new Slot[newSize];
                int[] newBuckets = new int[newSize];

                // move down slots and rehash at the same time. newIndex keeps track of current
                // position in newSlots array
                int newIndex = 0;
                for (int i = 0; i < _lastIndex; i++)
                {
                    if (_slots[i].hashCode >= 0)
                    {
                        newSlots[newIndex] = _slots[i];

                        // rehash
                        int bucket = newSlots[newIndex].hashCode % newSize;
                        newSlots[newIndex].next = newBuckets[bucket] - 1;
                        newBuckets[bucket] = newIndex + 1;

                        newIndex++;
                    }
                }

                Debug.Assert(newSlots.Length <= _slots.Length, "capacity increased after TrimExcess");

                _lastIndex = newIndex;
                _slots = newSlots;
                _buckets = newBuckets;
                _freeList = -1;
            }
        }

        #endregion

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
        public static IEqualityComparer<ISet<T>> CreateSetComparer()
        {
            return SetEqualityComparer<T>.Default;
        }

        /// <summary>
        /// Initializes buckets and slots arrays. Uses suggested capacity by finding next prime
        /// greater than or equal to capacity.
        /// </summary>
        /// <param name="capacity"></param>
        private int Initialize(int capacity)
        {
            Debug.Assert(_buckets == null, "Initialize was called but _buckets was non-null");

            int size = HashHelpers.GetPrime(capacity);

            _buckets = new int[size];
            _slots = new Slot[size];
            return size;
        }

        /// <summary>
        /// Expand to new capacity. New capacity is next prime greater than or equal to suggested
        /// size. This is called when the underlying array is filled. This performs no
        /// defragmentation, allowing faster execution; note that this is reasonable since
        /// AddIfNotPresent attempts to insert new elements in re-opened spots.
        /// </summary>
        private void IncreaseCapacity()
        {
            Debug.Assert(_buckets != null, "IncreaseCapacity called on a set with no elements");

            int newSize = HashHelpers.ExpandPrime(_count);
            if (newSize <= _count)
            {
                throw new ArgumentException(SR.Arg_HSCapacityOverflow);
            }

            // Able to increase capacity; copy elements to larger array and rehash
            SetCapacity(newSize);
        }

        /// <summary>
        /// Set the underlying buckets array to size newSize and rehash.  Note that newSize
        /// *must* be a prime.  It is very likely that you want to call IncreaseCapacity()
        /// instead of this method.
        /// </summary>
        private void SetCapacity(int newSize)
        {
            Debug.Assert(HashHelpers.IsPrime(newSize), "New size is not prime!");
            Debug.Assert(_buckets != null, "SetCapacity called on a set with no elements");

            Slot[] newSlots = new Slot[newSize];
            if (_slots != null)
            {
                Array.Copy(_slots, newSlots, _lastIndex);
            }

            int[] newBuckets = new int[newSize];
            for (int i = 0; i < _lastIndex; i++)
            {
                int hashCode = newSlots[i].hashCode;
                if (hashCode >= 0)
                {
                    int bucket = hashCode % newSize;
                    newSlots[i].next = newBuckets[bucket] - 1;
                    newBuckets[bucket] = i + 1;
                }
            }
            _slots = newSlots;
            _buckets = newBuckets;
        }

        /// <summary>
        /// Adds value to HashSet if not contained already
        /// Returns true if added and false if already present
        /// </summary>
        /// <param name="value">value to find</param>
        /// <returns></returns>
        private bool AddIfNotPresent(T value)
        {
            if (_buckets == null)
            {
                Initialize(0);
            }

            int hashCode;
            int bucket;
            int collisionCount = 0;
            Slot[] slots = _slots;

            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                hashCode = value == null ? 0 : InternalGetHashCode(value.GetHashCode());
                bucket = hashCode % _buckets!.Length;

                if (default(T)! != null) // TODO-NULLABLE: default(T) == null warning (https://github.com/dotnet/roslyn/issues/34757)
                {
                    for (int i = _buckets[bucket] - 1; i >= 0; i = slots[i].next)
                    {
                        if (slots[i].hashCode == hashCode && EqualityComparer<T>.Default.Equals(slots[i].value, value))
                        {
                            return false;
                        }

                        if (collisionCount >= slots.Length)
                        {
                            // The chain of entries forms a loop, which means a concurrent update has happened.
                            throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                        }
                        collisionCount++;
                    }
                }
                else
                {
                    // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
                    // https://github.com/dotnet/coreclr/issues/17273
                    // So cache in a local rather than get EqualityComparer per loop iteration
                    IEqualityComparer<T> defaultComparer = EqualityComparer<T>.Default;

                    for (int i = _buckets[bucket] - 1; i >= 0; i = slots[i].next)
                    {
                        if (slots[i].hashCode == hashCode && defaultComparer.Equals(slots[i].value, value))
                        {
                            return false;
                        }

                        if (collisionCount >= slots.Length)
                        {
                            // The chain of entries forms a loop, which means a concurrent update has happened.
                            throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                        }
                        collisionCount++;
                    }
                }
            }
            else
            {
                hashCode = value == null ? 0 : InternalGetHashCode(comparer.GetHashCode(value));
                bucket = hashCode % _buckets!.Length;

                for (int i = _buckets[bucket] - 1; i >= 0; i = slots[i].next)
                {
                    if (slots[i].hashCode == hashCode && comparer.Equals(slots[i].value, value))
                    {
                        return false;
                    }

                    if (collisionCount >= slots.Length)
                    {
                        // The chain of entries forms a loop, which means a concurrent update has happened.
                        throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                    }
                    collisionCount++;
                }
            }

            int index;
            if (_freeList >= 0)
            {
                index = _freeList;
                _freeList = slots[index].next;
            }
            else
            {
                if (_lastIndex == slots.Length)
                {
                    IncreaseCapacity();
                    // this will change during resize
                    slots = _slots;
                    bucket = hashCode % _buckets.Length;
                }
                index = _lastIndex;
                _lastIndex++;
            }
            slots[index].hashCode = hashCode;
            slots[index].value = value;
            slots[index].next = _buckets[bucket] - 1;
            _buckets[bucket] = index + 1;
            _count++;
            _version++;

            return true;
        }

        // Add value at known index with known hash code. Used only
        // when constructing from another HashSet.
        private void AddValue(int index, int hashCode, T value)
        {
            int bucket = hashCode % _buckets!.Length;

#if DEBUG
            IEqualityComparer<T> comparer = _comparer ?? EqualityComparer<T>.Default;
            Debug.Assert(InternalGetHashCode(value, comparer) == hashCode);
            for (int i = _buckets[bucket] - 1; i >= 0; i = _slots[i].next)
            {
                Debug.Assert(!comparer.Equals(_slots[i].value, value));
            }
#endif

            Debug.Assert(_freeList == -1);
            _slots[index].hashCode = hashCode;
            _slots[index].value = value;
            _slots[index].next = _buckets[bucket] - 1;
            _buckets[bucket] = index + 1;
        }

        /// <summary>
        /// Checks if this contains of other's elements. Iterates over other's elements and
        /// returns false as soon as it finds an element in other that's not in this.
        /// Used by SupersetOf, ProperSupersetOf, and SetEquals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool ContainsAllElements(IEnumerable<T> other)
        {
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
        private bool IsSubsetOfHashSetWithSameEC(ICollection<T> other)
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
        private void IntersectWithHashSetWithSameEC(ICollection<T> other)
        {
            for (int i = 0; i < _lastIndex; i++)
            {
                if (_slots[i].hashCode >= 0)
                {
                    T item = _slots[i].value;
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

            // keep track of current last index; don't want to move past the end of our bit array
            // (could happen if another thread is modifying the collection)
            int originalLastIndex = _lastIndex;
            int intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);

            Span<int> span = stackalloc int[StackAllocThreshold];
            BitHelper bitHelper = intArrayLength <= StackAllocThreshold ?
                new BitHelper(span.Slice(0, intArrayLength), clear: true) :
                new BitHelper(new int[intArrayLength], clear: false);

            // mark if contains: find index of in slots array and mark corresponding element in bit array
            foreach (T item in other)
            {
                int index = InternalIndexOf(item);
                if (index >= 0)
                {
                    bitHelper.MarkBit(index);
                }
            }

            // if anything unmarked, remove it. Perf can be optimized here if BitHelper had a
            // FindFirstUnmarked method.
            for (int i = 0; i < originalLastIndex; i++)
            {
                if (_slots[i].hashCode >= 0 && !bitHelper.IsMarked(i))
                {
                    Remove(_slots[i].value);
                }
            }
        }

        /// <summary>
        /// Used internally by set operations which have to rely on bit array marking. This is like
        /// Contains but returns index in slots array.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int InternalIndexOf(T item)
        {
            Debug.Assert(_buckets != null, "_buckets was null; callers should check first");

            int[]? buckets = _buckets;
            int collisionCount = 0;
            Slot[] slots = _slots;
            IEqualityComparer<T>? comparer = _comparer;

            if (comparer == null)
            {
                int hashCode = item == null ? 0 : InternalGetHashCode(item.GetHashCode());

                if (default(T)! != null) // TODO-NULLABLE: default(T) == null warning (https://github.com/dotnet/roslyn/issues/34757)
                {
                    // see note at "HashSet" level describing why "- 1" appears in for loop
                    for (int i = buckets![hashCode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                    {
                        if (slots[i].hashCode == hashCode && EqualityComparer<T>.Default.Equals(slots[i].value, item))
                        {
                            return i;
                        }

                        if (collisionCount >= slots.Length)
                        {
                            // The chain of entries forms a loop, which means a concurrent update has happened.
                            throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                        }
                        collisionCount++;
                    }
                }
                else
                {
                    // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
                    // https://github.com/dotnet/coreclr/issues/17273
                    // So cache in a local rather than get EqualityComparer per loop iteration
                    IEqualityComparer<T> defaultComparer = EqualityComparer<T>.Default;

                    // see note at "HashSet" level describing why "- 1" appears in for loop
                    for (int i = buckets![hashCode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                    {
                        if (slots[i].hashCode == hashCode && defaultComparer.Equals(slots[i].value, item))
                        {
                            return i;
                        }

                        if (collisionCount >= slots.Length)
                        {
                            // The chain of entries forms a loop, which means a concurrent update has happened.
                            throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                        }
                        collisionCount++;
                    }
                }
            }
            else
            {
                int hashCode = item == null ? 0 : InternalGetHashCode(comparer.GetHashCode(item));

                // see note at "HashSet" level describing why "- 1" appears in for loop
                for (int i = buckets![hashCode % buckets.Length] - 1; i >= 0; i = slots[i].next)
                {
                    if (slots[i].hashCode == hashCode && comparer.Equals(slots[i].value, item))
                    {
                        return i;
                    }

                    if (collisionCount >= slots.Length)
                    {
                        // The chain of entries forms a loop, which means a concurrent update has happened.
                        throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                    }
                    collisionCount++;
                }
            }
            // wasn't found
            return -1;
        }

        /// <summary>
        /// if other is a set, we can assume it doesn't have duplicate elements, so use this
        /// technique: if can't remove, then it wasn't present in this set, so add.
        ///
        /// As with other methods, callers take care of ensuring that other is a hashset using the
        /// same equality comparer.
        /// </summary>
        /// <param name="other"></param>
        private void SymmetricExceptWithUniqueHashSet(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                if (!Remove(item))
                {
                    AddIfNotPresent(item);
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
            int originalLastIndex = _lastIndex;
            int intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);

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
                int location = 0;
                bool added = AddOrGetLocation(item, out location);
                if (added)
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
                    // to check here. There's no point in checking items beyond originalLastIndex
                    // because they could not have been in the original collection
                    if (location < originalLastIndex && !itemsAddedFromOther.IsMarked(location))
                    {
                        itemsToRemove.MarkBit(location);
                    }
                }
            }

            // if anything marked, remove it
            for (int i = 0; i < originalLastIndex; i++)
            {
                if (itemsToRemove.IsMarked(i))
                {
                    Remove(_slots[i].value);
                }
            }
        }

        /// <summary>
        /// Add if not already in hashset. Returns an out param indicating index where added. This
        /// is used by SymmetricExcept because it needs to know the following things:
        /// - whether the item was already present in the collection or added from other
        /// - where it's located (if already present, it will get marked for removal, otherwise
        /// marked for keeping)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private bool AddOrGetLocation(T value, out int location)
        {
            Debug.Assert(_buckets != null, "_buckets is null, callers should have checked");

            IEqualityComparer<T>? comparer = _comparer;
            int hashCode = InternalGetHashCode(value, comparer);
            int bucket = hashCode % _buckets!.Length;
            int collisionCount = 0;
            Slot[] slots = _slots;
            for (int i = _buckets[bucket] - 1; i >= 0; i = slots[i].next)
            {
                if (slots[i].hashCode == hashCode && (comparer?.Equals(slots[i].value, value) ?? EqualityComparer<T>.Default.Equals(slots[i].value, value)))
                {
                    location = i;
                    return false; //already present
                }

                if (collisionCount >= slots.Length)
                {
                    // The chain of entries forms a loop, which means a concurrent update has happened.
                    throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                }
                collisionCount++;
            }
            int index;
            if (_freeList >= 0)
            {
                index = _freeList;
                _freeList = slots[index].next;
            }
            else
            {
                if (_lastIndex == slots.Length)
                {
                    IncreaseCapacity();
                    // this will change during resize
                    slots = _slots;
                    bucket = hashCode % _buckets.Length;
                }
                index = _lastIndex;
                _lastIndex++;
            }
            slots[index].hashCode = hashCode;
            slots[index].value = value;
            slots[index].next = _buckets[bucket] - 1;
            _buckets[bucket] = index + 1;
            _count++;
            _version++;
            location = index;
            return true;
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
        private unsafe ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
        {
            ElementCount result;

            // need special case in case this has no elements.
            if (_count == 0)
            {
                int numElementsInOther = 0;
                foreach (T item in other)
                {
                    numElementsInOther++;
                    // break right away, all we want to know is whether other has 0 or 1 elements
                    break;
                }
                result.uniqueCount = 0;
                result.unfoundCount = numElementsInOther;
                return result;
            }

            Debug.Assert((_buckets != null) && (_count > 0), "_buckets was null but count greater than 0");

            int originalLastIndex = _lastIndex;
            int intArrayLength = BitHelper.ToIntArrayLength(originalLastIndex);

            Span<int> span = stackalloc int[StackAllocThreshold];
            BitHelper bitHelper = intArrayLength <= StackAllocThreshold ?
                new BitHelper(span.Slice(0, intArrayLength), clear: true) :
                new BitHelper(new int[intArrayLength], clear: false);

            // count of items in other not found in this
            int unfoundCount = 0;
            // count of unique items in other found in this
            int uniqueFoundCount = 0;

            foreach (T item in other)
            {
                int index = InternalIndexOf(item);
                if (index >= 0)
                {
                    if (!bitHelper.IsMarked(index))
                    {
                        // item hasn't been seen yet
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
        /// Internal method used for HashSetEqualityComparer. Compares set1 and set2 according
        /// to specified comparer.
        ///
        /// Because items are hashed according to a specific equality comparer, we have to resort
        /// to n^2 search if they're using different equality comparers.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        internal static bool HashSetEquals(HashSet<T>? set1, HashSet<T>? set2, IEqualityComparer<T> comparer)
        {
            // handle null cases first
            if (set1 == null)
            {
                return (set2 == null);
            }
            else if (set2 == null)
            {
                // set1 != null
                return false;
            }

            // all comparers are the same; this is faster
            if (AreEqualityComparersEqual(set1, set2))
            {
                if (set1.Count != set2.Count)
                {
                    return false;
                }
                // suffices to check subset
                foreach (T item in set2)
                {
                    if (!set1.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {  // n^2 search because items are hashed according to their respective ECs
                foreach (T set2Item in set2)
                {
                    bool found = false;
                    foreach (T set1Item in set1)
                    {
                        if (comparer.Equals(set2Item, set1Item))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        private static bool AreEqualityComparersEqual(HashSet<T> set1, IEnumerable<T> set2)
        {
            if (set2 is HashSet<T> hashSet)
                return set1.EqualityComparer.Equals(hashSet.EqualityComparer);
            else if (set2 is LinkedHashSet<T> linkedHashSet)
                return set1.EqualityComparer.Equals(linkedHashSet.EqualityComparer);
            else if (set2 is SCG.HashSet<T> scgHashSet)
                return set1.EqualityComparer.Equals(scgHashSet.Comparer);
            return false;
        }

        /// <summary>
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        private static bool AreEqualityComparersEqual(HashSet<T> set1, HashSet<T> set2)
        {
            return set1.EqualityComparer.Equals(set2.EqualityComparer);
        }

        /// <summary>
        /// Workaround Comparers that throw ArgumentNullException for GetHashCode(null).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        /// <returns>hash code</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InternalGetHashCode(T item, IEqualityComparer<T>? comparer)
        {
            if (item == null)
            {
                return 0;
            }

            int hashCode = comparer?.GetHashCode(item) ?? item.GetHashCode();
            return hashCode & Lower31BitMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InternalGetHashCode(int hashCode)
        {
            return hashCode & Lower31BitMask;
        }

        #endregion

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
        /// using rules similar to those in the JDK's AbstactSet class. Two sets are considered
        /// equal when they both contain the same objects (in any order).
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="ISet{T}"/>
        /// and it contains the same elements; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
            => Equals(obj, SetEqualityComparer<T>.Default);

        /// <summary>
        /// Gets the hash code for the current set. The hash code is calculated 
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <seealso cref="GetHashCode(IEqualityComparer)"/>
        public override int GetHashCode()
            => GetHashCode(SetEqualityComparer<T>.Default);

        #endregion

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

        #endregion

        #region Nested Structures

        // used for set checking operations (using enumerables) that rely on counting
        internal struct ElementCount
        {
            internal int uniqueCount;
            internal int unfoundCount;
        }

        internal struct Slot
        {
            internal int hashCode;      // Lower 31 bits of hash code, -1 if unused
            internal int next;          // Index of next entry, -1 if last
            internal T value;
        }

        internal struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly HashSet<T> _set;
            private int _index;
            private readonly int _version;
            [AllowNull] private T _current;

            internal Enumerator(HashSet<T> set)
            {
                _set = set;
                _index = 0;
                _version = set._version;
                _current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_version != _set._version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                while (_index < _set._lastIndex)
                {
                    if (_set._slots[_index].hashCode >= 0)
                    {
                        _current = _set._slots[_index].value;
                        _index++;
                        return true;
                    }
                    _index++;
                }
                _index = _set._lastIndex + 1;
                _current = default;
                return false;
            }

            public T Current => _current;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _set._lastIndex + 1)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _set._version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                _index = 0;
                _current = default;
            }
        }

        #endregion
    }
}
