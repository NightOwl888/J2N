using J2N.Collections.ObjectModel;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using SCG = System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#if FEATURE_CONTRACTBLOCKS
using System.Diagnostics.Contracts;
#endif
#if !FEATURE_CASEINSENSITIVECOMPARER
using CaseInsensitiveComparer = System.StringComparer; // To fixup documentation - this type doesn't exist on .NET Standard 1.x
#endif
#nullable enable

namespace J2N.Collections.Generic
{
    using SR = J2N.Resources.Strings;

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
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class List<T> : IList<T>, IList,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyList<T>,
#endif
        IStructuralEquatable, IStructuralFormattable
#if FEATURE_SERIALIZABLE
        , System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
#endif
    {
        private const int MaxArrayLength = 0X7FEFFFFF;
        private const int defaultCapacity = 4;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private readonly SCG.List<T> list;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private int version;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private object? syncRoot;

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
        public List() : this(0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that contains elements
        /// copied from the specified collection and has sufficient capacity to accommodate the
        /// number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        public List(IEnumerable<T> collection)
        {
            list = new SCG.List<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that is empty and has the
        /// specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public List(int capacity)
        {
            list = new SCG.List<T>(capacity);
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
        protected List(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            siInfo = info;
            int capacity = info.GetInt32(CountName);
            list = new SCG.List<T>(capacity);
        }
#endif

        #endregion

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
            get => list.Capacity;
            set => list.Capacity = value;
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
        public int Count => list.Count;


        bool IList.IsFixedSize => false;


        // Is this List read-only?
        bool ICollection<T>.IsReadOnly => false;

        bool IList.IsReadOnly => false;

        // Is this List synchronized (thread-safe)?
        bool ICollection.IsSynchronized => false;

        // Synchronization root for this object.
        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                {
                    if (list is ICollection col)
                        syncRoot = col.SyncRoot;
                    System.Threading.Interlocked.CompareExchange<object?>(ref syncRoot, new object(), null);
                }
                return syncRoot;
            }
        }

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
            get => list[index];
            set
            {
                list[index] = value;
                version++;
            }
        }

        object? IList.this[int index]
        {
            get => ((IList)list)[index];
            set
            {
                ((IList)list)[index] = value;
                version++;
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Add(T item)
        {
            list.Add(item);
            version++;
        }

        int IList.Add(object? item)
        {
            int result = ((IList)list).Add(item);
            version++;
            return result;
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
        {
#if FEATURE_CONTRACTBLOCKS
            Contract.Ensures(Count >= Contract.OldValue(Count));
#endif

            InsertRange(list.Count, collection);
        }

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
#if FEATURE_CONTRACTBLOCKS
            Contract.Ensures(Contract.Result<ReadOnlyList<T>>() != null);
#endif
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return list.BinarySearch(index, count, item, comparer ?? Comparer<T>.Default);
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public int BinarySearch(T item)
        {
            return BinarySearch(item, null);
        }

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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public int BinarySearch(T item, IComparer<T>? comparer)
        {
            return list.BinarySearch(item, comparer ?? Comparer<T>.Default);
        }

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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Clear()
        {
            list.Clear();
            version++;
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(T item)
            => list.Contains(item);

        bool IList.Contains(object? item)
            => ((IList)list).Contains(item);

#if FEATURE_CONVERTER
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
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

#if FEATURE_CONTRACTBLOCKS
            Contract.EndContractBlock();
#endif

            int size = this.list.Count;
            List<TOutput> list = new List<TOutput>(size);
            for (int i = 0; i < size; i++)
            {
                list.Add(converter(this.list[i]));
            }
            return list;
        }
#endif

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
            => list.CopyTo(array);

        void ICollection.CopyTo(Array array, int arrayIndex)
            => ((ICollection)list).CopyTo(array, arrayIndex);

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
            => list.CopyTo(index, array, arrayIndex, count);

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
            => list.CopyTo(array, arrayIndex);

        // Ensures that the capacity of this list is at least the given minimum
        // value. If the currect capacity of the list is less than min, the
        // capacity is increased to twice the current capacity or to min,
        // whichever is larger.
        private void EnsureCapacity(int min)
        {
            int size = list.Count;
            if (size < min)
            {
                int newCapacity = size == 0 ? defaultCapacity : size * 2;
                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if ((uint)newCapacity > MaxArrayLength) newCapacity = MaxArrayLength;
                if (newCapacity < min) newCapacity = min;
                list.Capacity = newCapacity;
            }
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
            => list.Exists(match);

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
            => list.Find(match);

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
            if (match == null)
                throw new ArgumentNullException(nameof(match));

#if FEATURE_CONTRACTBLOCKS
            Contract.EndContractBlock();
#endif

            List<T> list = new List<T>();
            for (int i = 0; i < this.list.Count; i++)
            {
                if (match(this.list[i]))
                {
                    list.Add(this.list[i]);
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
            => list.FindIndex(match);

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
            => list.FindIndex(startIndex, match);

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
            => list.FindIndex(startIndex, count, match);

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
            => list.FindLast(match);

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
            => list.FindLastIndex(match);

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
            => list.FindLastIndex(startIndex, match);

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
            => list.FindLastIndex(startIndex, count, match);

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
            => list.ForEach(action);

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
            => new Enumerator(this);

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
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (this.list.Count - index < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

#if FEATURE_CONTRACTBLOCKS
            Contract.Ensures(Contract.Result<List<T>>() != null);
            Contract.EndContractBlock();
#endif
            List<T> list = new List<T>(count);
            for (int i = 0; i < count; i++)
                list.Add(this.list[index + i]);
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public int IndexOf(T item)
            => list.IndexOf(item);

        int IList.IndexOf(object? item)
            => ((IList)list).IndexOf(item);

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
            => list.IndexOf(item, index);

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
            => list.IndexOf(item, index, count);

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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            version++;
        }

        void IList.Insert(int index, object? item)
        {
            ((IList)list).Insert(index, item);
            version++;
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
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            int size = list.Count;
            if ((uint)index > (uint)size)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_Index);
#if FEATURE_CONTRACTBLOCKS
            Contract.EndContractBlock();
#endif

            if (collection is ICollection<T> c)
            {    // if collection is ICollection<T>
                int count = c.Count;
                if (count > 0)
                {
                    EnsureCapacity(size + count);
                    if (index < size)
                    {
                        Copy(list, index, list, index + count, size - index);
                    }

                    // If we're inserting a List into itself, we want to be able to deal with that.
                    if (this == c)
                    {
                        // Copy first part of _items to insert location
                        Copy(list, 0, list, index, index);
                        // Copy last part of _items back to inserted location
                        Copy(list, index + count, list, index * 2, size - index);
                    }
                    else
                    {
                        T[] itemsToInsert = new T[count];
                        c.CopyTo(itemsToInsert, 0);
                        Copy(itemsToInsert, 0, list, index, count);
                    }
                    // Internal size already set by Copy()
                }
            }
            else
            {
                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Insert(index++, en.Current);
                    }
                }
            }
            version++;
        }

        // For moving items between lists (same signature as Array.Copy())
        private void Copy(IList<T> source, int sourceIndex, IList<T> destination, int destinationIndex, int count)
        {
            // Allocate any extra space needed at the end of the list.
            int toAdd = (destinationIndex + count) - destination.Count;
            while (toAdd-- > 0)
            {
                destination.Add(default!);
            }

            // Copy the items starting with the last index and ending with the first index in both source and destination.
            // This ensures if we are copying the list to itself, we don't overwrite any items and copy them again.
            for (int i = (destinationIndex + count) - 1, j = (sourceIndex + count) - 1; i >= destinationIndex; i--, j--)
            {
                destination[i] = source[j];
            }
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public int LastIndexOf(T item)
            => list.LastIndexOf(item);

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
            => list.LastIndexOf(item, index);

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
            => list.LastIndexOf(item, index, count);

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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Remove(T item)
        {
            if (list.Remove(item))
            {
                version++;
                return true;
            }
            return false;
        }

        void IList.Remove(object? item)
        {
            ((IList)list).Remove(item);
            version++;
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
        {
            int removed = list.RemoveAll(match);
            if (removed > 0)
                version++;
            return removed;
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            version++;
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
        {
            list.RemoveRange(index, count);
            if (count > 0)
                version++;
        }

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="List{T}"/>.
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="M:Array.Reverse{T}(T[])"/> to reverse the order of the elements.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Reverse()
        {
            list.Reverse();
            version++;
        }

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
        {
            list.Reverse(index, count);
            version++;
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Sort()
        {
            list.Sort(Comparer<T>.Default);
            version++;
        }

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
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            list.Sort(index, count, comparer ?? Comparer<T>.Default);
            version++;
        }

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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Sort(IComparer<T> comparer)
        {
            list.Sort(comparer ?? Comparer<T>.Default);
            version++;
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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Sort(Comparison<T> comparison)
            => list.Sort(comparison);

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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public T[] ToArray()
            => list.ToArray();

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
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void TrimExcess()
            => list.TrimExcess();

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
            => list.TrueForAll(match);

        #endregion

        #region Custom Serialization

#if FEATURE_SERIALIZABLE
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
        protected virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            int count = list.Count;
            info.AddValue(CountName, count);
            info.AddValue(VersionName, version);

            if (count > 0)
            {
                T[] items = new T[count];
                list.CopyTo(items, 0);
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

            int savedCount = siInfo.GetInt32(CountName);

            if (savedCount != 0)
            {
                T[]? items = (T[]?)siInfo.GetValue(ItemsName, typeof(T[]));

                if (items == null)
                {
                    throw new System.Runtime.Serialization.SerializationException(SR.Serialization_MissingValues);
                }

                for (int i = 0; i < items.Length; i++)
                {
                    list.Add(items[i]);
                }
            }

            version = siInfo.GetInt32(VersionName);
            if (list.Count != savedCount)
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
            => ListEqualityComparer<T>.Equals(this, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current list using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current list.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
            => ListEqualityComparer<T>.GetHashCode(this, comparer);

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
            => CollectionUtil.ToString(formatProvider, format, this);

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
                index = 0;
                version = list.version;
                current = default!;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                List<T> localList = list;

                if (version == localList.version && ((uint)index < (uint)localList.list.Count))
                {
                    current = localList.list[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (version != list.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                index = list.list.Count + 1;
                current = default!;
                return false;
            }

            public T Current => current;

            object? IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == list.list.Count + 1)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                if (version != list.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                index = 0;
                current = default!;
            }
        }

        #endregion
    }
}
