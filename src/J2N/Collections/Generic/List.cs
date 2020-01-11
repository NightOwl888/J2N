using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using SCG = System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if FEATURE_CONTRACTBLOCKS
using System.Diagnostics.Contracts;
#endif
#nullable enable

namespace J2N.Collections.Generic
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// <para/>
    /// <see cref="List{T}"/> adds the following features to <see cref="System.Collections.Generic.List{T}"/>:
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
    public class List<T> : IList<T>, IList, IReadOnlyList<T>, IStructuralEquatable, IStructuralFormattable
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
        // J2N: Providing implementation only to link up XML doc comments properly
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
        // J2N: Providing implementation only to link up XML doc comments properly
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

        public void AddRange(IEnumerable<T> collection)
        {
#if FEATURE_CONTRACTBLOCKS
            Contract.Ensures(Count >= Contract.OldValue(Count));
#endif

            InsertRange(list.Count, collection);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return list.BinarySearch(index, count, item, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int BinarySearch(T item)
        {
            return BinarySearch(item, null);
        }
        /// <summary>
        /// Testing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int BinarySearch(T item, IComparer<T>? comparer)
        {
            return list.BinarySearch(item, comparer ?? Comparer<T>.Default);
        }

        public void Clear()
        {
            list.Clear();
            version++;
        }

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

        public void CopyTo(T[] array)
            => list.CopyTo(array);

        void ICollection.CopyTo(Array array, int arrayIndex)
            => ((ICollection)list).CopyTo(array, arrayIndex);

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
            => list.CopyTo(index, array, arrayIndex, count);

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

        public bool Exists(Predicate<T> match)
            => list.Exists(match);

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
        /// The <see cref="Predicate{T}"/> is a delegate to a method that returns true if the object passed
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

        public int FindIndex(Predicate<T> match)
            => list.FindIndex(match);

        public int FindIndex(int startIndex, Predicate<T> match)
            => list.FindIndex(startIndex, match);

        public int FindIndex(int startIndex, int count, Predicate<T> match)
            => list.FindIndex(startIndex, count, match);

        public T FindLast(Predicate<T> match)
            => list.FindLast(match);

        public int FindLastIndex(Predicate<T> match)
            => list.FindLastIndex(match);

        public int FindLastIndex(int startIndex, Predicate<T> match)
            => list.FindLastIndex(startIndex, match);

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
            => list.FindLastIndex(startIndex, count, match);

        public void ForEach(Action<T> action)
            => list.ForEach(action);

        public IEnumerator<T> GetEnumerator()
            => new Enumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

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

        public int IndexOf(T item)
            => list.IndexOf(item);

        int IList.IndexOf(object? item)
            => ((IList)list).IndexOf(item);

        public int IndexOf(T item, int index)
            => list.IndexOf(item, index);

        public int IndexOf(T item, int index, int count)
            => list.IndexOf(item, index, count);

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

        public int LastIndexOf(T item)
            => list.LastIndexOf(item);

        public int LastIndexOf(T item, int index)
            => list.LastIndexOf(item, index);

        public int LastIndexOf(T item, int index, int count)
            => list.LastIndexOf(item, index, count);

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

        public int RemoveAll(Predicate<T> match)
        {
            int removed = list.RemoveAll(match);
            if (removed > 0)
                version++;
            return removed;
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            version++;
        }

        public void RemoveRange(int index, int count)
        {
            list.RemoveRange(index, count);
            if (count > 0)
                version++;
        }

        public void Reverse()
        {
            list.Reverse();
            version++;
        }

        public void Reverse(int index, int count)
        {
            list.Reverse(index, count);
            version++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Sort()
        {
            list.Sort(Comparer<T>.Default);
            version++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            list.Sort(index, count, comparer ?? Comparer<T>.Default);
            version++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer)
        {
            list.Sort(comparer ?? Comparer<T>.Default);
            version++;
        }

        public void Sort(Comparison<T> comparison)
            => list.Sort(comparison);

        public T[] ToArray()
            => list.ToArray();

        public void TrimExcess()
            => list.TrimExcess();

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

        void System.Runtime.Serialization.IDeserializationCallback.OnDeserialization(object sender) => OnDeserialization(sender);

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface, and raises the deserialization
        /// event when the deserialization is completed.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The <see cref="System.Runtime.Serialization.SerializationInfo"/> object associated
        /// with the current <see cref="SortedSet{T}"/> object is invalid.</exception>
        /// <remarks>Calling this method is an <c>O(n)</c> operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        protected virtual void OnDeserialization(object sender)
        {
            if (siInfo == null)
            {
                return; // Somebody had a dependency on this class and fixed us up before the ObjectManager got to it.
            }

            int savedCount = siInfo.GetInt32(CountName);

            if (savedCount != 0)
            {
                T[] items = (T[])siInfo.GetValue(ItemsName, typeof(T[]));

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
        internal struct Enumerator<T1> : IEnumerator<T1>, IEnumerator where T1 : T
        {
            private readonly List<T1> list;
            private int index;
            private int version;
            private T1 current;

            internal Enumerator(List<T1> list)
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
                List<T1> localList = list;

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

            public T1 Current => current;

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
