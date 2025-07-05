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

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// <para/>
    /// <see cref="List{T}"/> is similar to <see cref="System.Collections.Generic.List{T}"/>, but adds the following features:
    /// <list type="bullet">
    ///     <item><description>
    ///         Overrides the <see cref="Equals(object)"/> and <see cref="GetHashCode()"/> methods to compare lists
    ///         using structural equality by default. Also, <see cref="IStructuralEquatable"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Overrides the <see cref="ToString()"/> methods to list the contents of the list
    ///         by default. Also, <see cref="IFormatProvider"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    /// </list>
    /// <para/>
    /// Usage Note: This class is intended to be a direct replacement for <see cref="System.Collections.Generic.List{T}"/> in order
    /// to provide default structural equality and formatting behavior similar to Java. Note that the <see cref="ToString()"/>
    /// method uses the current culture by default to behave like other components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Using Microsoft's code styles")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public partial class List<T> : IList<T>, IList,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyList<T>,
#endif
        IStructuralEquatable, IStructuralFormattable
#if FEATURE_SERIALIZABLE
        , System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
#endif
    {
        private const int DefaultCapacity = 4;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        internal T[] _items;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        internal int _size;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        internal int _version;

        private static readonly T[] s_emptyArray = Arrays.Empty<T>();

#if FEATURE_SERIALIZABLE
        private System.Runtime.Serialization.SerializationInfo? siInfo; //A temporary variable which we need during deserialization.

        // names for serialization
        private const string CountName = "Count"; // Do not rename (binary serialization) - used to allocate during deserialzation, not actually a field
        private const string ItemsName = "Items"; // Do not rename (binary serialization)
        private const string VersionName = "Version"; // Do not rename (binary serialization)
#endif

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that is empty
        /// and has the default initial capacity.
        /// </summary>
        public List()
        {
            _items = s_emptyArray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that contains elements
        /// copied from the specified collection and has sufficient capacity to accommodate the
        /// number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        public List(IEnumerable<T> collection)
        {
            if (collection is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count == 0)
                {
                    _items = s_emptyArray;
                }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    _size = count;
                }
            }
            else
            {
                _items = s_emptyArray;
                using (IEnumerator<T> en = collection!.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that is empty and has the
        /// specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public List(int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);

            if (capacity == 0)
                _items = s_emptyArray;
            else
                _items = new T[capacity];
        }

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that contains serialized data.
        /// </summary>
        /// <param name="info">The object that contains the information that is required to serialize
        /// the <see cref="List{T}"/> object.</param>
        /// <param name="context">The structure that contains the source and destination of the serialized
        /// stream associated with the <see cref="List{T}"/> object.</param>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute an object that is transmitted over a stream.
        /// </remarks>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected List(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            siInfo = info;
            int capacity = info.GetInt32(CountName);
            _items = new T[capacity];
        }
#endif

        #endregion

        #region Bounds Checking for SubList

        // Tracks the lower bound of a SubList
        internal virtual int Offset => 0;

        // Tracks the length of a SubList
        internal virtual int Size => _size;

        internal virtual int AncestralVersion => _version;

        internal virtual void CoModificationCheck()
        {
        }

        #endregion Bounds Checking for SubList

        #region GetView

        /// <summary>
        /// Returns a view of a sublist in a <see cref="List{T}"/>.
        /// <para/>
        /// IMPORTANT: This method uses .NET semantics. That is, the second parameter is a count rather than an exclusive end
        /// index as would be the case in Java's subList() method. To translate from Java, use <c>toIndex - fromIndex</c> to
        /// obtain the value of <paramref name="count"/>.
        /// </summary>
        /// <param name="index">The first index in the view (inclusive).</param>
        /// <param name="count">The number of elements to include in the view.</param>
        /// <returns>A sublist view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> refer to a location outside of the list.
        /// </exception>
        /// <remarks>This method returns a view of the range of elements that are specified by <paramref name="index"/>
        /// and <paramref name="count"/>. Unlike <see cref="GetRange(int, int)"/>, this method does not copy elements from
        /// the <see cref="List{T}"/>, but provides a window into the underlying <see cref="List{T}"/> itself.
        /// You can make changes to the view and create child views of the view. However, any structural change to a parent view
        /// or the original <see cref="List{T}"/> will cause all methods of the view or any enumerator based on the view
        /// to throw an <see cref="InvalidOperationException"/>. Structural modifications are any edit that will change the <see cref="Count"/>
        /// or otherwise perturb it in such a way that enumerations in progress will be invalid. A view is only valid until one of its ancestors
        /// is structurally modified, at which point you will need to create a new view.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public virtual List<T> GetView(int index, int count)
        {
            CoModificationCheck();
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            if (Size - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            return new SubList(this, index, count);
        }

        #endregion GetView

        #region SCG.List<T> Members

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        /// <remarks>
        /// <see cref="Capacity"/> is the number of elements that the <see cref="List{T}"/> can store
        /// before resizing is required. <see cref="Count"/> is the number of elements that are actually
        /// in the <see cref="List{T}"/>.
        /// <para/>
        /// <see cref="Capacity"/> is always greater than or equal to <see cref="Count"/>. If <see cref="Count"/>
        /// exceeds <see cref="Capacity"/> while adding elements, the capacity is increased by automatically
        /// reallocating the internal array before copying the old elements and adding the new elements.
        /// <para/>
        /// If the capacity is significantly larger than the count and you want to reduce the memory used by the
        /// <see cref="List{T}"/>, you can decrease capacity by calling the <see cref="TrimExcess()"/> method or by setting the
        /// <see cref="Capacity"/> property explicitly to a lower value. When the value of <see cref="Capacity"/>
        /// is set explicitly, the internal array is also reallocated to accommodate the specified capacity,
        /// and all the elements are copied.
        /// <para/>
        /// Retrieving the value of this property is an O(1) operation; setting the property is an O(<c>n</c>)
        /// operation, where <c>n</c> is the new capacity.
        /// </remarks>
        public int Capacity
        {
            get => _items.Length;
            set => DoSetCapacity(value); // Hack so we can override
        }


        // Returns true if we re-allocated the array
        [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "Using property name for clarity for end user")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA2208 doesn't fire on all target frameworks")]
        internal virtual bool DoSetCapacity(int value)
        {
            if (value < _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
            }

            if (value != _items.Length)
            {
                if (value > 0)
                {
                    T[] newItems = new T[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, newItems, _size);
                    }
                    _items = newItems;
                }
                else
                {
                    _items = s_emptyArray;
                }
                _version++; // J2N: Unlike .NET, we consider reallocating the array a "modification" to the list to ensure sublists use the same array reference
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="List{T}"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="Capacity"/> is the number of elements that the <see cref="List{T}"/> can store
        /// before resizing is required. <see cref="Count"/> is the number of elements that are actually
        /// in the <see cref="List{T}"/>.
        /// <para/>
        /// <see cref="Capacity"/> is always greater than or equal to <see cref="Count"/>. If <see cref="Count"/>
        /// exceeds <see cref="Capacity"/> while adding elements, the capacity is increased by automatically
        /// reallocating the internal array before copying the old elements and adding the new elements.
        /// <para/>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => Size;


        bool IList.IsFixedSize => false;


        // Is this List read-only?
        internal virtual bool IsReadOnly => false;
        bool ICollection<T>.IsReadOnly => IsReadOnly;

        bool IList.IsReadOnly => IsReadOnly;

        // Is this List synchronized (thread-safe)?
        bool ICollection.IsSynchronized => false;

        // Synchronization root for this object.
        object ICollection.SyncRoot => this;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </exception>
        /// <remarks>
        /// <see cref="List{T}"/> accepts null as a valid value for reference types and
        /// allows duplicate elements.
        /// <para/>
        /// This property provides the ability to access a specific element in the collection
        /// by using the following syntax: <c>myCollection[index]</c>.
        /// <para/>
        /// Retrieving the value of this property is an O(1) operation; setting the property
        /// is also an O(1) operation.
        /// </remarks>
        public T this[int index]
        {
            get
            {
                CoModificationCheck();
                if ((uint)index >= (uint)Size)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index);
                }
                Debug.Assert(_size - Offset >= index);
                return _items[index + Offset];
            }
            set => DoSet(index, value);
        }

        internal virtual void DoSet(int index, T value)
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index);
            }
            _items[index] = value;
            _version++;
        }

        private static bool IsCompatibleObject(object? value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
            return (value is T) || (value == null && default(T) == null);
        }

        object? IList.this[int index]
        {
            get => this[index];
            set
            {
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);

                try
                {
                    this[index] = (T)value!;
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                }
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="List{T}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <remarks>
        /// <see cref="List{T}"/> accepts <c>null</c> as a valid value for reference types and allows duplicate elements.
        /// <para/>
        /// If <see cref="Count"/> already equals <see cref="Capacity"/>, the capacity of the <see cref="List{T}"/>
        /// is increased by automatically reallocating the internal array, and the existing elements are copied
        /// to the new array before the new element is added.
        /// <para/>
        /// If <see cref="Count"/> is less than <see cref="Capacity"/>, this method is an O(1) operation. If the
        /// capacity needs to be increased to accommodate the new element, this method becomes an O(<c>n</c>)
        /// operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
            => DoAdd(item); // Hack so we can override

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void DoAdd(T item)
        {
            _version++;
            T[] array = _items;
            int size = _size;
            if ((uint)size < (uint)array.Length)
            {
                _size = size + 1;
                array[size] = item;
            }
            else
            {
                AddWithResize(item);
            }
        }

        // Non-inline from List.Add to improve its code quality as uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T item)
        {
            Debug.Assert(_size == _items.Length);
            int size = _size;
            Grow(size + 1);
            _size = size + 1;
            _items[size] = item;
        }

        int IList.Add(object? item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);

            try
            {
                Add((T)item!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }

            return Count - 1;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the
        /// <see cref="List{T}"/>. The collection itself cannot be <c>null</c>, but it can contain elements
        /// that are <c>null</c>, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The order of the elements in the collection is preserved in the <see cref="List{T}"/>.
        /// <para/>
        /// If the new <see cref="Count"/> (the current <see cref="Count"/> plus the size of the collection)
        /// will be greater than <see cref="Capacity"/>, the capacity of the <see cref="List{T}"/> is increased
        /// by automatically reallocating the internal array to accommodate the new elements, and the existing
        /// elements are copied to the new array before the new elements are added.
        /// <para/>
        /// If the <see cref="List{T}"/> can accommodate the new elements without increasing the <see cref="Capacity"/>,
        /// this method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements to be added. If the capacity
        /// needs to be increased to accommodate the new elements, this method becomes an O(<c>n</c> + <c>m</c>) operation,
        /// where <c>n</c> is the number of elements to be added and <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public void AddRange(IEnumerable<T> collection)
            => InsertRange(Size, collection);

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlyList{T}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="List{T}"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="List{T}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlyList{T}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="List{T}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlyList<T> AsReadOnly()
        {
            CoModificationCheck();
            return new ReadOnlyList<T>(this);
        }

        /// <summary>
        /// Searches a range of elements in the sorted <see cref="List{T}"/> for an element using
        /// the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be <c>null</c> for reference types.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing
        /// elements, or <c>null</c> to use J2N's default comparer <see cref="Comparer{T}.Default"/>.</param>
        /// <returns>The zero-based index of item in the sorted <see cref="List{T}"/>, if item is found;
        /// otherwise, a negative number that is the bitwise complement of the index of the next element
        /// that is larger than item or, if there is no larger element, the bitwise complement of <see cref="Count"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid
        /// range in the <see cref="List{T}"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is <c>null</c>, and J2N's default comparer
        /// <see cref="Comparer{T}.Default"/> cannot find an implementation of the <see cref="IComparable{T}"/> generic interface
        /// or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// The comparer customizes how the elements are compared. For example, you can use a <see cref="CaseInsensitiveComparer"/>
        /// instance as the comparer to perform case-insensitive string searches.
        /// <para/>
        /// If <paramref name="comparer"/> is provided, the elements of the <see cref="List{T}"/> are compared to the specified value using the
        /// specified <see cref="IComparer{T}"/> implementation.
        /// <para/>
        /// If <paramref name="comparer"/> is <c>null</c>, J2N's default comparer <see cref="Comparer{T}.Default"/> checks whether type <typeparamref name="T"/>
        /// implements the <see cref="IComparable{T}"/> generic interface and uses that implementation, if available. If not,
        /// <see cref="Comparer{T}.Default"/> checks whether type <typeparamref name="T"/> implements the <see cref="IComparable"/> interface.
        /// If type <typeparamref name="T"/> does not implement either interface, <see cref="Comparer{T}.Default"/> throws <see cref="InvalidOperationException"/>.
        /// <para/>
        /// The <see cref="List{T}"/> must already be sorted according to the comparer implementation; otherwise, the result is incorrect.
        /// <para/>
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception when using the <see cref="IComparable{T}"/>
        /// generic interface. When sorting, <c>null</c> is considered to be less than any other object.
        /// <para/>
        /// If the <see cref="List{T}"/> contains more than one element with the same value, the method returns only one of the occurrences,
        /// and it might return any one of the occurrences, not necessarily the first one.
        /// <para/>
        /// If the <see cref="List{T}"/> does not contain the specified value, the method returns a negative integer. You can apply
        /// the bitwise complement operation (~) to this negative integer to get the index of the first element that is larger than
        /// the search value. When inserting the value into the <see cref="List{T}"/>, this index should be used as the insertion
        /// point to maintain the sort order.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation, where <c>n</c> is the number of elements in the range.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
        {
            CoModificationCheck();
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            if (Size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            int offset = Offset;
            return Array.BinarySearch<T>(_items, index + offset, count, item, comparer ?? Comparer<T>.Default) - offset;
        }

        /// <summary>
        /// Searches the entire sorted <see cref="List{T}"/> for an element using the default comparer and returns the
        /// zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be <c>null</c> for reference types.</param>
        /// <returns>The zero-based index of item in the sorted <see cref="List{T}"/>, if item is found; otherwise,
        /// a negative number that is the bitwise complement of the index of the next element that is larger than
        /// item or, if there is no larger element, the bitwise complement of <see cref="Count"/>.</returns>
        /// <exception cref="InvalidOperationException">The default comparer <see cref="Comparer{T}.Default"/>
        /// cannot find an implementation of the <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/>
        /// interface for type <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// This method uses J2N's default comparer <see cref="Comparer{T}.Default"/> for type <typeparamref name="T"/> to determine
        /// the order of list elements. The <see cref="Comparer{T}.Default"/> property checks whether type T implements the <see cref="IComparable{T}"/>
        /// generic interface and uses that implementation, if available. If not, <see cref="Comparer{T}.Default"/> checks whether type
        /// <typeparamref name="T"/> implements the <see cref="IComparable"/> interface. If type <typeparamref name="T"/> does not implement
        /// either interface, <see cref="Comparer{T}.Default"/> throws an <see cref="InvalidOperationException"/>.
        /// <para/>
        /// The <see cref="List{T}"/> must already be sorted according to the comparer implementation; otherwise, the result is incorrect.
        /// <para/>
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception when using the <see cref="IComparable{T}"/>
        /// generic interface. When sorting, <c>null</c> is considered to be less than any other object.
        /// <para/>
        /// If the <see cref="List{T}"/> contains more than one element with the same value, the method returns only one of the occurrences,
        /// and it might return any one of the occurrences, not necessarily the first one.
        /// <para/>
        /// If the <see cref="List{T}"/> does not contain the specified value, the method returns a negative integer. You can apply the
        /// bitwise complement operation (~) to this negative integer to get the index of the first element that is larger than the
        /// search value. When inserting the value into the <see cref="List{T}"/>, this index should be used as the insertion point
        /// to maintain the sort order.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation, where <c>n</c> is the number of elements in the range.
        /// </remarks>
        public int BinarySearch(T item)
            => BinarySearch(0, Size, item, null);

        /// <summary>
        /// Searches the entire sorted <see cref="List{T}"/> for an element using the specified comparer and
        /// returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be <c>null</c> for reference types.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing elements.
        /// <para/>
        /// -or-
        /// <para/>
        /// <c>null</c> to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <returns>The zero-based index of item in the sorted <see cref="List{T}"/>, if item is found; otherwise,
        /// a negative number that is the bitwise complement of the index of the next element that is larger than
        /// item or, if there is no larger element, the bitwise complement of <see cref="Count"/>.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is <c>null</c>, and the default
        /// comparer <see cref="Comparer{T}.Default"/> cannot find an implementation of the <see cref="IComparable{T}"/>
        /// generic interface or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// The comparer customizes how the elements are compared. For example, you can use a <see cref="CaseInsensitiveComparer"/>
        /// instance as the comparer to perform case-insensitive string searches.
        /// <para/>
        /// If <paramref name="comparer"/> is provided, the elements of the <see cref="List{T}"/> are compared to the specified value using the
        /// specified <see cref="IComparer{T}"/> implementation.
        /// <para/>
        /// If <paramref name="comparer"/> is <c>null</c>, J2N's default comparer <see cref="Comparer{T}.Default"/> checks whether type <typeparamref name="T"/>
        /// implements the <see cref="IComparable{T}"/> generic interface and uses that implementation, if available. If not,
        /// <see cref="Comparer{T}.Default"/> checks whether type <typeparamref name="T"/> implements the <see cref="IComparable"/> interface.
        /// If type <typeparamref name="T"/> does not implement either interface, <see cref="Comparer{T}.Default"/> throws <see cref="InvalidOperationException"/>.
        /// <para/>
        /// The <see cref="List{T}"/> must already be sorted according to the comparer implementation; otherwise, the result is incorrect.
        /// <para/>
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception when using the <see cref="IComparable{T}"/>
        /// generic interface. When sorting, <c>null</c> is considered to be less than any other object.
        /// <para/>
        /// If the <see cref="List{T}"/> contains more than one element with the same value, the method returns only one of the occurrences,
        /// and it might return any one of the occurrences, not necessarily the first one.
        /// <para/>
        /// If the <see cref="List{T}"/> does not contain the specified value, the method returns a negative integer. You can apply
        /// the bitwise complement operation (~) to this negative integer to get the index of the first element that is larger than
        /// the search value. When inserting the value into the <see cref="List{T}"/>, this index should be used as the insertion
        /// point to maintain the sort order.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation, where <c>n</c> is the number of elements in the range.
        /// </remarks>
        public int BinarySearch(T item, IComparer<T>? comparer)
            => BinarySearch(0, Size, item, comparer);

        /// <summary>
        /// Removes all elements from the <see cref="List{T}"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="Count"/> is set to 0, and references to other objects from elements
        /// of the collection are also released.
        /// <para/>
        /// <see cref="Capacity"/> remains unchanged. To reset the capacity of the <see cref="List{T}"/>, call
        /// the <see cref="TrimExcess()"/> method or set the <see cref="Capacity"/> property directly. Decreasing
        /// the capacity reallocates memory and copies all the elements in the <see cref="List{T}"/>. Trimming an
        /// empty <see cref="List{T}"/> sets the capacity of the <see cref="List{T}"/> to the default capacity.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
            => DoClear(); // Hack so we can override

        // NOTE: Don't call Clear() from SubList, call RemoveRange() instead
        internal virtual void DoClear()
        {
            _version++;
            if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
            {
                int size = _size;
                _size = 0;
                if (size > 0)
                {
                    Array.Clear(_items, 0, size); // Clear the elements so that the gc can reclaim the references.
                }
            }
            else
            {
                _size = 0;
            }
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if item is found in the <see cref="List{T}"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method determines equality by using the default equality comparer, as defined by the object's implementation of
        /// the <see cref="IEquatable{T}.Equals(T)"/> method for <typeparamref name="T"/> (the type of values in the list).
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool Contains(T item)
        {
            CoModificationCheck();

            // PERF: IndexOf calls Array.IndexOf, which internally
            // calls EqualityComparer<T>.Default.IndexOf, which
            // is specialized for different types. This
            // boosts performance since instead of making a
            // virtual method call each iteration of the loop,
            // via EqualityComparer<T>.Default.Equals, we
            // only make one virtual call to EqualityComparer.IndexOf.

            return _size != 0 && IndexOf(item) != -1;
        }

        bool IList.Contains(object? item)
        {
            CoModificationCheck();
            if (IsCompatibleObject(item))
            {
                return Contains((T)item!);
            }
            return false;
        }

        /// <summary>
        /// Converts the elements in the current <see cref="List{T}"/> to another type, and
        /// returns a list containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <param name="converter">A <see cref="Converter{TInput, TOutput}"/> delegate that
        /// converts each element from one type to another type.</param>
        /// <returns>A <see cref="List{T}"/> of the target type containing the converted
        /// elements from the current <see cref="List{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="Converter{TInput, TOutput}"/> is a delegate to a method that converts an object
        /// to the target type. The elements of the current <see cref="List{T}"/> are individually passed
        /// to the <see cref="Converter{TInput, TOutput}"/> delegate, and the converted elements are saved
        /// in the new <see cref="List{T}"/>.
        /// <para/>
        /// The current <see cref="List{T}"/> remains unchanged.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            CoModificationCheck();
            if (converter is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter);

            int size = Size;
            List<TOutput> list = new List<TOutput>(size);
            for (int i = Offset; i < size; i++)
            {
                list._items[i] = converter(_items[i]);
            }
            list._size = size;
            return list;
        }

        /// <summary>
        /// Copies the entire <see cref="List{T}"/> to a compatible one-dimensional array, starting
        /// at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the
        /// elements copied from <see cref="List{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="List{T}"/> is greater
        /// than the number of elements that the destination <paramref name="array"/> can contain.</exception>
        /// <remarks>
        /// This method uses <see cref="Array.Copy(Array, int, Array, int, int)"/> to copy the elements.
        /// <para/>
        /// The elements are copied to the <see cref="Array"/> in the same order in which the enumerator iterates
        /// through the <see cref="List{T}"/>.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void CopyTo(T[] array)
            => CopyTo(array, 0);

        // Copies this List into array, which must be of a
        // compatible array type.
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            CoModificationCheck();
            if ((array != null) && (array.Rank != 1))
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);

            try
            {
                // Array.Copy will check for NULL.
                Array.Copy(_items, Offset, array!, arrayIndex, Size);
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
            }
        }

        /// <summary>
        /// Copies a range of elements from the <see cref="List{T}"/> to a compatible
        /// one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source <see cref="List{T}"/>
        /// at which copying begins.</param>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the
        /// elements copied from <see cref="List{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="arrayIndex"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> is equal to or greater than the <see cref="Count"/> of the
        /// source <see cref="List{T}"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// The number of elements in the source <see cref="List{T}"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        /// <remarks>
        /// This method uses <see cref="Array.Copy(Array, int, Array, int, int)"/> to copy the elements.
        /// <para/>
        /// The elements are copied to the <see cref="Array"/> in the same order in which the enumerator iterates
        /// through the <see cref="List{T}"/>.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <paramref name="count"/>.
        /// </remarks>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            CoModificationCheck();
            if (Size - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, index + Offset, array, arrayIndex, count);
        }

        /// <summary>
        /// Copies the entire <see cref="List{T}"/> to a compatible one-dimensional array, starting at the
        /// specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the
        /// elements copied from <see cref="List{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="List{T}"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// This method uses <see cref="Array.Copy(Array, int, Array, int, int)"/> to copy the elements.
        /// <para/>
        /// The elements are copied to the <see cref="Array"/> in the same order in which the enumerator iterates
        /// through the <see cref="List{T}"/>.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CoModificationCheck();
            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, Offset, array, arrayIndex, Size);
        }

        /// <summary>
        /// Ensures that the capacity of this list is at least the specified <paramref name="capacity"/>.
        /// If the current capacity of the list is less than specified <paramref name="capacity"/>,
        /// the capacity is increased by continuously twice current capacity until it is at least the specified <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this list.</returns>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            }

            if (_items.Length < capacity)
            {
                Grow(capacity);
                _version++;
            }

            return _items.Length;
        }

        /// <summary>
        /// Increase the capacity of this list to at least the specified <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        private void Grow(int capacity)
        {
            Debug.Assert(_items.Length < capacity);

            int newcapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newcapacity > ArrayExtensions.MaxLength) newcapacity = ArrayExtensions.MaxLength;

            // If the computed capacity is still less than specified, set to the original argument.
            // Capacities exceeding ArrayExtensions.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
            if (newcapacity < capacity) newcapacity = capacity;

            Capacity = newcapacity;
        }

        /// <summary>
        /// Determines whether the <see cref="List{T}"/> contains elements that match the conditions
        /// defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns><c>true</c> if the <see cref="List{T}"/> contains one or more elements that match
        /// the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate, moving backward in the <see cref="List{T}"/>, starting with the
        /// last element and ending with the first element. Processing is stopped when a match is found.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// <see cref="Count"/>.
        /// </remarks>
        public bool Exists(Predicate<T> match)
            => FindIndex(match) != -1;

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the first occurrence within the entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns>The first element that matches the conditions defined by the specified predicate,
        /// if found; otherwise, the default value for type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate, moving backward in the <see cref="List{T}"/>, starting with the
        /// last element and ending with the first element. Processing is stopped when a match is found.
        /// <para/>
        /// IMPORTANT: When searching a list containing value types, make sure the default value for the type
        /// does not satisfy the search predicate. Otherwise, there is no way to distinguish between a default
        /// value indicating that no match was found and a list element that happens to have the default value
        /// for the type. If the default value satisfies the search predicate, use the
        /// <see cref="FindIndex(Predicate{T})"/> method instead.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// <see cref="Count"/>.
        /// </remarks>
        [return: MaybeNull]
        public T Find(Predicate<T> match)
        {
            CoModificationCheck();
            if (match is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);

            int offset = Offset;
            int limit = Size + offset;
            for (int i = offset; i < limit; i++)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default;
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the elements to search for.</param>
        /// <returns>A <see cref="List{T}"/> containing all the elements that match the conditions
        /// defined by the specified predicate, if found; otherwise, an empty <see cref="List{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed
        /// to it matches the conditions defined in the delegate. The elements of the current <see cref="List{T}"/>
        /// are individually passed to the <see cref="Predicate{T}"/> delegate, and the elements that match
        /// the conditions are saved in the returned <see cref="List{T}"/>.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where
        /// <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public List<T> FindAll(Predicate<T> match)
        {
            CoModificationCheck();
            if (match is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);

            List<T> list = new List<T>();
            int offset = Offset;
            int limit = Size + offset;
            for (int i = Offset; i < limit; i++)
            {
                if (match(_items[i]))
                {
                    list.Add(_items[i]);
                }
            }
            return list;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the first occurrence within the entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched forward starting at the first element and ending at
        /// the last element.
        /// <para/>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate. The delegate has the signature:
        /// <code>
        /// public bool methodName(T obj)
        /// </code>
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// <see cref="Count"/>.
        /// </remarks>
        public int FindIndex(Predicate<T> match)
            => FindIndex(0, Size, match);

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the first occurrence within the range of elements in
        /// the <see cref="List{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the
        /// range of valid indexes for the <see cref="List{T}"/>.</exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched forward starting at <paramref name="startIndex"/> and ending at
        /// the last element.
        /// <para/>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate. The delegate has the signature:
        /// <code>
        /// public bool methodName(T obj)
        /// </code>
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// the number of elements from <paramref name="startIndex"/> to the end of the <see cref="List{T}"/>.
        /// </remarks>
        public int FindIndex(int startIndex, Predicate<T> match)
            => FindIndex(startIndex, Size - startIndex, match);

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the first occurrence within the range of elements
        /// in the <see cref="List{T}"/> that starts at the specified index and contains the specified
        /// number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="List{T}"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="List{T}"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched forward starting at <paramref name="startIndex"/> and ending at
        /// <paramref name="startIndex"/> plus <paramref name="count"/> minus 1, if <paramref name="count"/> is greater
        /// than 0.
        /// <para/>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate. The delegate has the signature:
        /// <code>
        /// public bool methodName(T obj)
        /// </code>
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// <paramref name="count"/>.
        /// </remarks>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            CoModificationCheck();
            if ((uint)startIndex > (uint)Size)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0 || startIndex > Size - count)
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count(count);
            if (match is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);

            int offset = Offset;
            int endIndex = startIndex + offset + count;
            for (int i = startIndex + offset; i < endIndex; i++)
            {
                if (match(_items[i])) return i - offset;
            }
            return -1;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the last occurrence within the entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns>The last element that matches the conditions defined by the specified predicate,
        /// if found; otherwise, the default value for type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate, moving backward in the <see cref="List{T}"/>, starting with the
        /// last element and ending with the first element. Processing is stopped when a match is found.
        /// <para/>
        /// IMPORTANT: When searching a list containing value types, make sure the default value for the type
        /// does not satisfy the search predicate. Otherwise, there is no way to distinguish between a default
        /// value indicating that no match was found and a list element that happens to have the default value
        /// for the type. If the default value satisfies the search predicate, use the
        /// <see cref="FindLastIndex(Predicate{T})"/> method instead.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// <see cref="Count"/>.
        /// </remarks>
        [return: MaybeNull]
        public T FindLast(Predicate<T> match)
        {
            CoModificationCheck();
            if (match is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);

            int offset = Offset;
            int limit = Size + offset;
            for (int i = limit - 1; i >= offset; i--)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the last occurrence within the entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched backward starting at the last element and ending at
        /// the first element.
        /// <para/>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// <see cref="Count"/>.
        /// </remarks>
        public int FindLastIndex(Predicate<T> match)
            => FindLastIndex(Size - 1, Size, match);

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the last occurrence within the range of elements in
        /// the <see cref="List{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="List{T}"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched backward starting at <paramref name="startIndex"/> and ending at the first element.
        /// <para/>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// the number of elements from the beginning of the <see cref="List{T}"/> to <paramref name="startIndex"/>.
        /// </remarks>
        public int FindLastIndex(int startIndex, Predicate<T> match)
            => FindLastIndex(startIndex, startIndex + 1, match);

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the last occurrence within the range of elements in
        /// the <see cref="List{T}"/> that contains the specified number of elements and ends at the
        /// specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the element to search for.</param>
        /// <returns>The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="List{T}"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="List{T}"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched backward starting at <paramref name="startIndex"/> and ending at
        /// <paramref name="startIndex"/> minus <paramref name="count"/> plus 1, if <paramref name="count"/> is greater
        /// than 0.
        /// <para/>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed to it matches the
        /// conditions defined in the delegate. The elements of the current <see cref="List{T}"/> are individually
        /// passed to the <see cref="Predicate{T}"/> delegate.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c> is
        /// <paramref name="count"/>.
        /// </remarks>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            CoModificationCheck();
            if (match is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);

            if (Size == 0)
            {
                // Special case for 0 length List
                if (startIndex != -1)
                {
                    ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
                }
            }
            else
            {
                // Make sure we're not out of range
                if ((uint)startIndex >= (uint)Size)
                {
                    ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess(startIndex);
                }
            }

            // 2nd have of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            if (count < 0 || startIndex - count + 1 < 0)
            {
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count(count);
            }

            int offset = Offset;
            int endIndex = startIndex + offset - count;
            for (int i = startIndex + offset; i > endIndex; i--)
            {
                if (match(_items[i]))
                {
                    return i - offset;
                }
            }
            return -1;
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> delegate to perform on
        /// each element of the <see cref="List{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">An element in the collection has been modified.</exception>
        /// <remarks>
        /// The <see cref="Action{T}"/> is a delegate to a method that performs an action on the object passed to it.
        /// The elements of the current <see cref="List{T}"/> are individually passed to the <see cref="Action{T}"/> delegate.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// <para/>
        /// Modifying the underlying collection in the body of the <see cref="Action{T}"/> delegate is not supported
        /// and causes undefined behavior.
        /// </remarks>
        public void ForEach(Action<T> action)
        {
            CoModificationCheck();
            if (action is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.action);

            int version = _version;
            int offset = Offset;
            int limit = Size + offset;

            for (int i = offset; i < limit; i++)
            {
                if (version != _version)
                {
                    break;
                }
                action(_items[i]);
            }

            if (version != _version)
                ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="List{T}"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> for the <see cref="List{T}"/>.</returns>
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
            CoModificationCheck();
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="List{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based <see cref="List{T}"/> index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>A shallow copy of a range of elements in the source <see cref="List{T}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do
        /// not denote a valid range of elements in the <see cref="List{T}"/>.</exception>
        /// <remarks>
        /// A shallow copy of a collection of reference types, or a subset of that collection, contains
        /// only the references to the elements of the collection. The objects themselves are not copied.
        /// The references in the new list point to the same objects as the references in the original list.
        /// <para/>
        /// A shallow copy of a collection of value types, or a subset of that collection, contains the
        /// elements of the collection. However, if the elements of the collection contain references to
        /// other objects, those objects are not copied. The references in the elements of the new collection
        /// point to the same objects as the references in the elements of the original collection.
        /// <para/>
        /// In contrast, a deep copy of a collection copies the elements and everything directly or indirectly
        /// referenced by the elements.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is count.
        /// </remarks>
        public List<T> GetRange(int index, int count)
        {
            CoModificationCheck();
            if (index < 0)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            if (Size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            List<T> list = new List<T>(count);
            Array.Copy(_items, index + Offset, list._items, 0, count);
            list._size = count;
            return list;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within
        /// the entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the entire
        /// <see cref="List{T}"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched forward starting at the first element and ending at
        /// the last element.
        /// <para/>
        /// This method determines equality using J2N's default equality comparer <see cref="EqualityComparer{T}.Default"/>
        /// for <typeparamref name="T"/>, the type of values in the list.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c>
        /// is <see cref="Count"/>.
        /// </remarks>
        public int IndexOf(T item)
        {
            CoModificationCheck();
            int offset = Offset;
            int result = Array.IndexOf(_items, item, offset, Size);
            return result > -1 ? result - offset : result;
        }

        int IList.IndexOf(object? item)
        {
            CoModificationCheck();
            if (IsCompatibleObject(item))
            {
                return IndexOf((T)item!);
            }
            return -1;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within
        /// the range of elements in the <see cref="List{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="List{T}"/>
        /// that extends from <paramref name="index"/> to the last element, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes
        /// for the <see cref="List{T}"/>.</exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched forward starting at <paramref name="index"/> and ending at
        /// the last element.
        /// <para/>
        /// This method determines equality using J2N's default equality comparer <see cref="EqualityComparer{T}.Default"/>
        /// for <typeparamref name="T"/>, the type of values in the list.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c>
        /// is the number of elements from <paramref name="index"/> to the end of the <see cref="List{T}"/>.
        /// </remarks>
        public int IndexOf(T item, int index)
        {
            CoModificationCheck();
            if ((uint)index > (uint)Size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);

            int offset = Offset;
            int result = Array.IndexOf(_items, item, index + offset, Size - index);
            return result > -1 ? result - offset : result;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within
        /// the range of elements in the <see cref="List{T}"/> that starts at the specified index and contains
        /// the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="List{T}"/>
        /// that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is outside the range of valid indexes for the <see cref="List{T}"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="List{T}"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched forward starting at <paramref name="index"/> and ending at
        /// <paramref name="index"/> plus <paramref name="count"/> minus 1, if <paramref name="count"/> is greater than 0.
        /// <para/>
        /// This method determines equality using J2N's default equality comparer <see cref="EqualityComparer{T}.Default"/>
        /// for <typeparamref name="T"/>, the type of values in the list.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c>
        /// is <paramref name="count"/>.
        /// </remarks>
        public int IndexOf(T item, int index, int count)
        {
            CoModificationCheck();
            if ((uint)index > (uint)Size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            if (count < 0 || index > Size - count)
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count(count);

            int offset = Offset;
            int result = Array.IndexOf(_items, item, index + offset, count);
            return result > -1 ? result - offset : result;
        }

        /// <summary>
        /// Inserts an element into the <see cref="List{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be <c>null</c> for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than <see cref="Count"/>.
        /// </exception>
        /// <remarks>
        /// <see cref="List{T}"/> accepts <c>null</c> as a valid value for reference types and allows duplicate elements.
        /// <para/>
        /// If <see cref="Count"/> already equals <see cref="Capacity"/>, the capacity of the <see cref="List{T}"/> is increased
        /// by automatically reallocating the internal array, and the existing elements are copied to the new array before the
        /// new element is added.
        /// <para/>
        /// If index is equal to <see cref="Count"/>, item is added to the end of <see cref="List{T}"/>.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void Insert(int index, T item)
            => DoInsert(index, item); // Hack so we can override

        internal virtual void DoInsert(int index, T item)
        {
            // Note that insertions at the end are legal.
            if ((uint)index > (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(index, ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);
            }
            if (_size == _items.Length) Grow(_size + 1);
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = item;
            _size++;
            _version++;
        }

        void IList.Insert(int index, object? item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);

            try
            {
                Insert(index, (T)item!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="List{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="List{T}"/>.
        /// The collection itself cannot be <c>null</c>, but it can contain elements that are <c>null</c>, if type
        /// <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than <see cref="Count"/>.
        /// </exception>
        /// <remarks>
        /// <see cref="List{T}"/> accepts <c>null</c> as a valid value for reference types and allows duplicate elements.
        /// <para/>
        /// If the new <see cref="Count"/> (the current <see cref="Count"/> plus the size of the collection) will be
        /// greater than <see cref="Capacity"/>, the capacity of the <see cref="List{T}"/> is increased by automatically
        /// reallocating the internal array to accommodate the new elements, and the existing elements are copied to the
        /// new array before the new elements are added.
        /// <para/>
        /// If index is equal to <see cref="Count"/>, the elements are added to the end of <see cref="List{T}"/>.
        /// <para/>
        /// The order of the elements in the collection is preserved in the <see cref="List{T}"/>.
        /// <para/>
        /// This method is an O(<c>n</c> * <c>m</c>) operation, where <c>n</c> is the number of elements to be added and
        /// <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public void InsertRange(int index, IEnumerable<T> collection)
            => DoInsertRange(index, collection); // Hack so we can override

        internal virtual int DoInsertRange(int index, IEnumerable<T> collection)
        {
            if (collection is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            if ((uint)index > (uint)_size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);

            int count = 0;
            // A sublist that is a descendant of this list
            if (collection is List<T>.SubList subList && subList._items == _items)
            {
                count = subList.Count;
                if (count > 0)
                {
                    int offset = Offset + subList.Offset;
                    int subListIndex = index - offset;

                    if (_items.Length - _size < count)
                    {
                        Grow(_size + count);
                    }

                    // We need to fixup our sublist reference if it is broken by EnsureCapacity
                    if (subList._items != _items)
                    {
                        subList._items = _items;
                    }

                    if (index < _size)
                    {
                        Array.Copy(_items, index, _items, index + count, _size - index);
                    }

                    // We're inserting a SubList which is a descendant into this list,
                    // so we already have the elements in the local array.

                    // Copy first part of _items to insert location
                    Array.Copy(_items, offset, _items, index, subListIndex);
                    // Copy last part of _items back to inserted location
                    Array.Copy(_items, index + count, _items, subListIndex * 2, count - subListIndex);

                    _size += count;
                }
            }
            else if (collection is ICollection<T> c)
            {
                count = c.Count;
                if (count > 0)
                {
                    EnsureCapacity(_size + count);
                    if (index < _size)
                    {
                        Array.Copy(_items, index, _items, index + count, _size - index);
                    }

                    // If we're inserting a List into itself, we want to be able to deal with that.
                    if (this == c)
                    {
                        // Copy first part of _items to insert location
                        Array.Copy(_items, 0, _items, index, index);
                        // Copy last part of _items back to inserted location
                        Array.Copy(_items, index + count, _items, index * 2, _size - index);
                    }
                    else
                    {
                        c.CopyTo(_items, index);
                    }
                    _size += count;
                }
            }
            else
            {
                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Insert(index++, en.Current);
                        count++;
                    }
                }
            }
            _version++;
            return count;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the
        /// entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the entire
        /// <see cref="List{T}"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched backward starting at the last element and ending at
        /// the first element.
        /// <para/>
        /// This method determines equality using J2N's default equality comparer <see cref="EqualityComparer{T}.Default"/>
        /// for <typeparamref name="T"/>, the type of values in the list.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c>
        /// is <see cref="Count"/>.
        /// </remarks>
        public int LastIndexOf(T item)
        {
            CoModificationCheck();
            int size = Size;
            if (size == 0)
            {  // Special case for empty list
                return -1;
            }
            else
            {
                return LastIndexOf(item, size - 1, size);
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range
        /// of elements in the <see cref="List{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in
        /// the <see cref="List{T}"/> that extends from the first element to <paramref name="index"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the
        /// <see cref="List{T}"/>.</exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched backward starting at <paramref name="index"/> and ending at
        /// the first element.
        /// <para/>
        /// This method determines equality using J2N's default equality comparer <see cref="EqualityComparer{T}.Default"/>
        /// for <typeparamref name="T"/>, the type of values in the list.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c>
        /// is the number of elements from the beginning of the <see cref="List{T}"/> to <paramref name="index"/>.
        /// </remarks>
        public int LastIndexOf(T item, int index)
        {
            CoModificationCheck();
            if (index >= Size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index);
            return LastIndexOf(item, index, index + 1);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range
        /// of elements in the <see cref="List{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="List{T}"/>
        /// that contains <paramref name="count"/> number of elements and ends at <paramref name="index"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is outside the range of valid indexes for the <see cref="List{T}"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="List{T}"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="List{T}"/> is searched backward starting at <paramref name="index"/> and ending at
        /// <paramref name="index"/> minus <paramref name="count"/> plus 1, if <paramref name="count"/> is greater than 0.
        /// <para/>
        /// This method determines equality using J2N's default equality comparer <see cref="EqualityComparer{T}.Default"/>
        /// for <typeparamref name="T"/>, the type of values in the list.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where <c>n</c>
        /// is <paramref name="count"/>.
        /// </remarks>
        public int LastIndexOf(T item, int index, int count)
        {
            CoModificationCheck();
            if ((Count != 0) && (index < 0))
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
            if ((Count != 0) && (count < 0))
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);

            if (Size == 0)
            {  // Special case for empty list
                return -1;
            }

            if (index >= Size)
                ThrowHelper.ThrowArgumentOutOfRangeException(index, ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            if (count > index + 1)
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);

            int offset = Offset;
            int result = Array.LastIndexOf(_items, item, index + offset, count);
            return result > -1 ? result - offset : result;
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="List{T}"/>. The value can be
        /// <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if item is successfully removed; otherwise, <c>false</c>. This method
        /// also returns <c>false</c> if item was not found in the <see cref="List{T}"/>.</returns>
        /// <remarks>
        /// If type <typeparamref name="T"/> implements the <see cref="IEquatable{T}"/> generic interface,
        /// the equality comparer is the <see cref="IEquatable{T}.Equals(T)"/> method of that interface;
        /// otherwise, the default equality comparer is <see cref="Object.Equals(object)"/>.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool Remove(T item)
            => DoRemove(item); // Hack so we can override

        internal virtual bool DoRemove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        void IList.Remove(object? item)
        {
            CoModificationCheck();
            if (IsCompatibleObject(item))
            {
                Remove((T)item!);
            }
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions
        /// of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="List{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed
        /// to it matches the conditions defined in the delegate. The elements of the current <see cref="List{T}"/>
        /// are individually passed to the <see cref="Predicate{T}"/> delegate, and the elements that match
        /// the conditions are renived from the <see cref="List{T}"/>.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation, where
        /// <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public int RemoveAll(Predicate<T> match)
            => DoRemoveAll(match); // Hack so we can override

        internal virtual int DoRemoveAll(Predicate<T> match)
            => DoRemoveAll(0, Size, match);

        internal virtual int DoRemoveAll(int startIndex, int count, Predicate<T> match)
        {
            int size = Size;
            if ((uint)startIndex > (uint)size)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess(startIndex);
            if (count < 0 || startIndex > size - count)
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count(count);
            if (match is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);

            int offset = Offset;
            int freeIndex = offset;   // the first free slot in items array
            uint start = (uint)startIndex + (uint)offset;
            uint limit = start + (uint)count; // The first index at the end of the range (this is outside of the valid range)

            // Find the first item which needs to be removed.
            while (freeIndex < start || freeIndex < limit && !match(_items[freeIndex])) freeIndex++;
            if (freeIndex >= limit) return 0;

            int current = freeIndex + 1;
            while (current < limit)
            {
                // Find the first item which needs to be kept.
                while (current < limit && match(_items[current])) current++;

                if (current < limit)
                {
                    // copy item to the free slot.
                    _items[freeIndex++] = _items[current++];
                }
            }

            // Free up any remaining space in parent list
            while (current < _size)
            {
                // copy item to the free slot.
                _items[freeIndex++] = _items[current++];
            }

            if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_items, freeIndex, _size - freeIndex); // Clear the elements so that the gc can reclaim the references.
            }

            int result = _size - freeIndex;
            _size = freeIndex;
            _version++;
            return result;
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </exception>
        /// <remarks>
        /// When you call <see cref="RemoveAt(int)"/> to remove an item, the remaining items in the list
        /// are renumbered to replace the removed item. For example, if you remove the item at index 3,
        /// the item at index 4 is moved to the 3 position. In addition, the number of items in the list
        /// (as represented by the <see cref="Count"/> property) is reduced by 1.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is (<see cref="Count"/> - <paramref name="index"/>).
        /// </remarks>
        public void RemoveAt(int index)
            => DoRemoveAt(index);

        internal virtual void DoRemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index);

            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
            {
                _items[_size] = default!;
            }
            _version++;
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/>
        /// do not denote a valid range of elements in the <see cref="List{T}"/>.</exception>
        /// <remarks>
        /// The items are removed and all the elements following them in the <see cref="List{T}"/> have
        /// their indexes reduced by <paramref name="count"/>.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void RemoveRange(int index, int count)
            => DoRemoveRange(index, count); // Hack so we can override

        internal virtual void DoRemoveRange(int index, int count)
        {
            if (index < 0)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            if (_size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            if (count > 0)
            {
                _size -= count;
                if (index < _size)
                {
                    Array.Copy(_items, index + count, _items, index, _size - index);
                }

                _version++;
                if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
                {
                    Array.Clear(_items, _size, count);
                }
            }
        }

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="List{T}"/>.
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="M:Array.Reverse{T}(T[])"/> to reverse the order of the elements.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void Reverse()
            => Reverse(0, Count);

        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/>
        /// do not denote a valid range of elements in the <see cref="List{T}"/>.</exception>
        /// <remarks>
        /// This method uses <see cref="M:Array.Reverse{T}(T[], int, int)"/> to reverse the order of the elements.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void Reverse(int index, int count)
            => DoReverse(index, count); // Hack so we can override

        internal virtual void DoReverse(int index, int count)
        {
            CoModificationCheck();
            if (index < 0)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            if (Size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            if (count > 1)
            {
                Array.Reverse(_items, index + Offset, count);
            }
            _version++;
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="List{T}"/> using J2N's default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">The default comparer <see cref="Comparer{T}.Default"/>
        /// cannot find an implementation of the <see cref="IComparable{T}"/> generic interface or the <see cref="IComparable"/>
        /// interface for type <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// This method uses J2N's default comparer <see cref="Comparer{T}.Default"/> for type <typeparamref name="T"/> to determine
        /// the order of list elements. The <see cref="Comparer{T}.Default"/> comparer checks whether
        /// type <typeparamref name="T"/> implements the <see cref="IComparable{T}"/> generic interface and uses that implementation,
        /// if available. If not, <see cref="Comparer{T}.Default"/> checks whether type <typeparamref name="T"/> implements the
        /// <see cref="IComparable"/> interface. If type <typeparamref name="T"/> does not implement either interface,
        /// <see cref="Comparer{T}.Default"/> throws an <see cref="InvalidOperationException"/>.
        /// <para/>
        /// This method uses <see cref="Array.Sort(Array, int, int, IComparer)"/>, which applies the introspective sort as follows:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the partition size is less than or equal to 16 elements, it uses an insertion sort algorithm
        ///     </description></item>
        ///     <item><description>
        ///         If the number of partitions exceeds 2 log <c>n</c>, where <c>n</c> is the range of the input array,
        ///         it uses a <a href="https://en.wikipedia.org/wiki/Heapsort">Heapsort</a> algorithm.
        ///     </description></item>
        ///     <item><description>
        ///         Otherwise, it uses a Quicksort algorithm.
        ///     </description></item>
        /// </list>
        /// <para/>
        /// This implementation performs an unstable sort; that is, if two elements are equal, their order might not be preserved.
        /// In contrast, a stable sort preserves the order of elements that are equal.
        /// <para/>
        /// On average, this method is an O(<c>n</c> log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/>; in the worst
        /// case it is an O(<c>n</c><sup>2</sup>) operation.
        /// </remarks>
        public void Sort()
            => Sort(0, Count, null);

        /// <summary>
        /// Sorts the elements in the entire <see cref="List{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when
        /// comparing elements, or <c>null</c> to use J2N's default comparer <see cref="Comparer{T}.Default"/>.</param>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is <c>null</c>, and the default
        /// comparer <see cref="Comparer{T}.Default"/> cannot find implementation of the <see cref="IComparable{T}"/>
        /// generic interface or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentException">The implementation of <paramref name="comparer"/> caused an error during
        /// the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</exception>
        /// <remarks>
        /// If <paramref name="comparer"/> is provided, the elements of the <see cref="List{T}"/> are sorted using the specified
        /// <see cref="IComparer{T}"/>.
        /// <para/>
        /// If <paramref name="comparer"/> is <c>null</c>, the default comparer <see cref="Comparer{T}.Default"/> checks whether
        /// type <typeparamref name="T"/> implements the <see cref="IComparable{T}"/> generic interface and uses that implementation,
        /// if available. If not, <see cref="Comparer{T}.Default"/> checks whether type <typeparamref name="T"/> implements the
        /// <see cref="IComparable"/> interface. If type <typeparamref name="T"/> does not implement either interface,
        /// <see cref="Comparer{T}.Default"/> throws an <see cref="InvalidOperationException"/>.
        /// <para/>
        /// This method uses <see cref="Array.Sort(Array, int, int, IComparer)"/>, which applies the introspective sort as follows:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the partition size is less than or equal to 16 elements, it uses an insertion sort algorithm
        ///     </description></item>
        ///     <item><description>
        ///         If the number of partitions exceeds 2 log <c>n</c>, where <c>n</c> is the range of the input array,
        ///         it uses a <a href="https://en.wikipedia.org/wiki/Heapsort">Heapsort</a> algorithm.
        ///     </description></item>
        ///     <item><description>
        ///         Otherwise, it uses a Quicksort algorithm.
        ///     </description></item>
        /// </list>
        /// <para/>
        /// This implementation performs an unstable sort; that is, if two elements are equal, their order might not be preserved.
        /// In contrast, a stable sort preserves the order of elements that are equal.
        /// <para/>
        /// On average, this method is an O(<c>n</c> log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/>; in the worst
        /// case it is an O(<c>n</c><sup>2</sup>) operation.
        /// </remarks>
        public void Sort(IComparer<T>? comparer)
            => Sort(0, Count, comparer);

        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="List{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements,
        /// or <c>null</c> to use J2N's default comparer <see cref="Comparer{T}.Default"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// The implementation of <paramref name="comparer"/> caused an error during the sort. For example,
        /// <paramref name="comparer"/> might not return 0 when comparing an item with itself.
        /// </exception>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is <c>null</c>, and J2N's
        /// default comparer <see cref="Comparer{T}.Default"/> cannot find implementation of the <see cref="IComparable{T}"/>
        /// generic interface or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// If <paramref name="comparer"/> is provided, the elements of the <see cref="List{T}"/> are sorted using the specified
        /// <see cref="IComparer{T}"/> implementation.
        /// <para/>
        /// If <paramref name="comparer"/> is <c>null</c>, the default comparer <see cref="Comparer{T}.Default"/> checks whether
        /// type <typeparamref name="T"/> implements the <see cref="IComparable{T}"/> generic interface and uses that implementation,
        /// if available. If not, <see cref="Comparer{T}.Default"/> checks whether type <typeparamref name="T"/> implements the
        /// <see cref="IComparable"/> interface. If type <typeparamref name="T"/> does not implement either interface,
        /// <see cref="Comparer{T}.Default"/> throws an <see cref="InvalidOperationException"/>.
        /// <para/>
        /// This method uses <see cref="Array.Sort(Array, int, int, IComparer)"/>, which applies the introspective sort as follows:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the partition size is less than or equal to 16 elements, it uses an insertion sort algorithm
        ///     </description></item>
        ///     <item><description>
        ///         If the number of partitions exceeds 2 log <c>n</c>, where <c>n</c> is the range of the input array,
        ///         it uses a <a href="https://en.wikipedia.org/wiki/Heapsort">Heapsort</a> algorithm.
        ///     </description></item>
        ///     <item><description>
        ///         Otherwise, it uses a Quicksort algorithm.
        ///     </description></item>
        /// </list>
        /// <para/>
        /// This implementation performs an unstable sort; that is, if two elements are equal, their order might not be preserved.
        /// In contrast, a stable sort preserves the order of elements that are equal.
        /// <para/>
        /// On average, this method is an O(<c>n</c> log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/>; in the worst
        /// case it is an O(<c>n</c><sup>2</sup>) operation.
        /// </remarks>
        public void Sort(int index, int count, IComparer<T>? comparer)
            => DoSort(index, count, comparer); // Hack so we can override

        internal virtual void DoSort(int index, int count, IComparer<T>? comparer)
        {
            if (index < 0)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            if (Size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            if (count > 1)
            {
                Array.Sort<T>(_items, index + Offset, count, comparer ?? Comparer<T>.Default);
            }
            _version++;
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="List{T}"/> using the specified <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">The <see cref="Comparison{T}"/> to use when comparing elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The implementation of <paramref name="comparison"/> caused an error during the sort.
        /// For example, <paramref name="comparison"/> might not return 0 when comparing an item with itself.</exception>
        /// <remarks>
        /// If comparison is provided, the elements of the <see cref="List{T}"/> are sorted using the method represented by the delegate.
        /// <para/>
        /// If comparison is <c>null</c>, an <see cref="ArgumentNullException"/> is thrown.
        /// <para/>
        /// This method uses <see cref="Array.Sort(Array, int, int)"/>, which applies the introspective sort as follows:
        /// <list type="bullet">
        ///     <item><description>
        ///         If the partition size is less than or equal to 16 elements, it uses an insertion sort algorithm
        ///     </description></item>
        ///     <item><description>
        ///         If the number of partitions exceeds 2 log <c>n</c>, where <c>n</c> is the range of the input array,
        ///         it uses a <a href="https://en.wikipedia.org/wiki/Heapsort">Heapsort</a> algorithm.
        ///     </description></item>
        ///     <item><description>
        ///         Otherwise, it uses a Quicksort algorithm.
        ///     </description></item>
        /// </list>
        /// <para/>
        /// This implementation performs an unstable sort; that is, if two elements are equal, their order might not be preserved.
        /// In contrast, a stable sort preserves the order of elements that are equal.
        /// <para/>
        /// On average, this method is an O(<c>n</c> log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/>; in the worst
        /// case it is an O(<c>n</c><sup>2</sup>) operation.
        /// </remarks>
        /// <seealso cref="Comparison{T}"/>
        public void Sort(Comparison<T> comparison)
            => DoSort(0, Size, comparison); // Hack so we can override

        internal virtual void DoSort(int index, int count, Comparison<T> comparison)
        {
            int size = Size;
            if ((uint)index > (uint)size)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            if (count < 0 || index > size - count)
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count(count);
            if (comparison is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparison);

            if (size > 1)
            {
                ArraySortHelper<T>.Sort(new Span<T>(_items, index, count), comparison);
            }
            _version++;
        }

        /// <summary>
        /// Copies the elements of the <see cref="List{T}"/> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="List{T}"/>.</returns>
        /// <remarks>
        /// The elements are copied using <see cref="Array.Copy(Array, int, Array, int, int)"/>, which
        /// is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public T[] ToArray()
        {
            CoModificationCheck();
            int size = Size;
            if (size == 0)
            {
                return s_emptyArray;
            }

            T[] array = new T[size];
            Array.Copy(_items, Offset, array, 0, size);
            return array;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="List{T}"/>, if that
        /// number is less than a threshold value.
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize a collection's memory overhead if no new elements will
        /// be added to the collection. The cost of reallocating and copying a large <see cref="List{T}"/>
        /// can be considerable, however, so the <see cref="TrimExcess()"/> method does nothing if the
        /// list is at more than 90 percent of capacity. This avoids incurring a large reallocation
        /// cost for a relatively small gain.
        /// <para/>
        /// NOTE: The current threshold of 90 percent might change in future releases.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// <para/>
        /// To reset a <see cref="List{T}"/> to its initial state, call the <see cref="Clear()"/> method
        /// before calling the <see cref="TrimExcess()"/> method. Trimming an empty <see cref="List{T}"/> sets the
        /// capacity of the <see cref="List{T}"/> to the default capacity.
        /// <para/>
        /// The capacity can also be set using the <see cref="Capacity"/> property.
        /// </remarks>
        public void TrimExcess()
        {
            CoModificationCheck();
            int threshold = (int)(((double)_items.Length) * 0.9);
            if (_size < threshold)
            {
                Capacity = _size;
            }
        }

        /// <summary>
        /// Determines whether every element in the <see cref="List{T}"/> matches the
        /// conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines
        /// the conditions to check against the elements.</param>
        /// <returns><c>true</c> if every element in the <see cref="List{T}"/> matches
        /// the conditions defined by the specified predicate; otherwise, <c>false</c>.
        /// If the list has no elements, the return value is <c>true</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns <c>true</c> if the object passed
        /// to it matches the conditions defined in the delegate. The elements of the current <see cref="List{T}"/>
        /// are individually passed to the <see cref="Predicate{T}"/> delegate, and processing is stopped when the
        /// delegate returns <c>false</c> for any element. The elements are processed in order, and all calls are
        /// made on a single thread.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool TrueForAll(Predicate<T> match)
        {
            CoModificationCheck();
            if (match is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);

            int size = Size;
            for (int i = Offset; i < size; i++)
            {
                if (!match(_items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Custom Serialization

#if FEATURE_SERIALIZABLE
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) => GetObjectData(info, context);

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and returns the data that you must have to serialize a
        /// <see cref="List{T}"/> object.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains the information that is required
        /// to serialize the <see cref="List{T}"/> object.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure that contains the source and destination
        /// of the serialized stream associated with the <see cref="List{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <c>null</c>.</exception>
        /// <remarks>Calling this method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);

            info.AddValue(CountName, _size);
            info.AddValue(VersionName, _version);

            if (_size > 0)
            {
                T[] items = new T[_size];
                Array.Copy(_items, 0, items, 0, _size);
                info.AddValue(ItemsName, items, typeof(T[]));
            }
        }

        void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object? sender) => OnDeserialization(sender);

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface, and raises the deserialization
        /// event when the deserialization is completed.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The <see cref="System.Runtime.Serialization.SerializationInfo"/> object associated
        /// with the current <see cref="SortedSet{T}"/> object is invalid.</exception>
        /// <remarks>Calling this method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        protected virtual void OnDeserialization(object? sender)
        {
            if (siInfo == null)
            {
                return; // Somebody had a dependency on this class and fixed us up before the ObjectManager got to it.
            }

            _size = siInfo.GetInt32(CountName);

            if (_size != 0)
            {
                T[]? items = (T[]?)siInfo.GetValue(ItemsName, typeof(T[]));

                if (items is null)
                {
                    throw new System.Runtime.Serialization.SerializationException(SR.Serialization_MissingValues);
                }

                Array.Copy(items, _items, _size);
            }

            _version = siInfo.GetInt32(VersionName);
            if (_items.Length != _size)
            {
                throw new System.Runtime.Serialization.SerializationException(SR.Serialization_MismatchedCount);
            }

            siInfo = null;
        }

#endif

        #endregion

        #region Structural Equality

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current list
        /// using rules provided by the specified <paramref name="comparer"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to determine
        /// whether the current object and <paramref name="other"/> are structurally equal.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is structurally equal to the current list;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public virtual bool Equals(object? other, IEqualityComparer comparer)
        {
            CoModificationCheck();
            return ListEqualityComparer<T>.Equals(this, other, comparer);
        }

        /// <summary>
        /// Gets the hash code representing the current list using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current list.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
        {
            CoModificationCheck();
            return ListEqualityComparer<T>.GetHashCode(this, comparer);
        }

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current list
        /// using rules similar to those in the JDK's AbstactList class. Two lists are considered
        /// equal when they both contain the same objects in the same order.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="IList{T}"/>
        /// and it contains the same elements in the same order; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
            => Equals(obj, ListEqualityComparer<T>.Default);

        /// <summary>
        /// Gets the hash code for the current list. The hash code is calculated
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => GetHashCode(ListEqualityComparer<T>.Default);

        #endregion

        #region ToString

        /// <summary>
        /// Returns a string that represents the current list using the specified
        /// <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
        {
            CoModificationCheck();
            return CollectionUtil.ToString(formatProvider, format, this);
        }

        /// <summary>
        /// Returns a string that represents the current list using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        public override string ToString()
            => ToString("{0}", StringFormatter.CurrentCulture);


        /// <summary>
        /// Returns a string that represents the current list using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current list using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
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

        #region Nested Structure: Enumerator

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly List<T> list;
            private int index;
            private readonly int version;
            [AllowNull, MaybeNull] private T current;

            internal Enumerator(List<T> list)
            {
                this.list = list;
                index = list.Offset;
                version = list._version;
                current = default!;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                List<T> localList = list;
                localList.CoModificationCheck();
                if (version == localList._version && ((uint)index < ((uint)localList.Size + (uint)localList.Offset)))
                {
                    current = localList._items[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                List<T> localList = list;
                if (version != localList._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                index = localList.Size + localList.Offset + 1;
                current = default!;
                return false;
            }

            public T Current => current;

            object? IEnumerator.Current
            {
                get
                {
                    List<T> localList = list;
                    int offset = localList.Offset;
                    if (index == offset || index == localList.Size + offset + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                list.CoModificationCheck();
                if (version != list._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }

                index = list.Offset;
                current = default!;
            }
        }

        #endregion
    }
}
