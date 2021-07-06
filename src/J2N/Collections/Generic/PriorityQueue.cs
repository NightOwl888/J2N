#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace J2N.Collections.Generic
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// A <see cref="PriorityQueue{T}"/> holds elements on a priority heap, which orders the elements
    /// according to their natural order or according to the comparator specified at
    /// construction time. If the queue uses natural ordering, only elements that are
    /// comparable are permitted to be inserted into the queue.
    /// <para/>
    /// The least element of the specified ordering is stored at the head of the
    /// queue and the greatest element is stored at the tail of the queue.
    /// <para/>
    /// A <see cref="PriorityQueue{T}"/> is not synchronized.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class PriorityQueue<T> : ICollection<T>, IStructuralFormattable
    {
        private const int DefaultCapacity = 11;
        private const double DefaultInitialCapacityRatio = 1.1;
        private const int DefaultCapacityRatio = 2;
        private int count;
        private IComparer<T>? comparer;
        private readonly IEqualityComparer<T> equalityComparer;
        private T[] elements;

        #region Constructors

        /// <summary>
        /// Constructs a priority queue with an initial capacity of <c>11</c>. Ordering
        /// is determined by J2N's default <see cref="Comparer{T}.Default"/>.
        /// </summary>
        public PriorityQueue()
            : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Constructs a priority queue with the specified capacity. Ordering
        /// is determined by J2N's default <see cref="Comparer{T}.Default"/>.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
        public PriorityQueue(int capacity)
            : this(capacity, null, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="PriorityQueue{T}"/> with the specified initial capacity
        /// that orders its elements according to the specified comparer.
        /// </summary>
        /// <param name="capacity">The initial capacity for this priority queue.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> that will be used to order this
        /// priority queue. If <c>null</c>, <typeparamref name="T"/> must implement <see cref="IComparable{T}"/>
        /// or an <see cref="InvalidOperationException"/> will be thrown when attempting to add an item.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
        public PriorityQueue(int capacity, IComparer<T>? comparer)
            : this(capacity, comparer, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="PriorityQueue{T}"/> with the specified initial capacity
        /// that orders its elements according to the specified comparer.
        /// </summary>
        /// <param name="capacity">The initial capacity for this priority queue.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> that will be used to order this
        /// priority queue. If <c>null</c>, <typeparamref name="T"/> must implement <see cref="IComparable{T}"/>
        /// or an <see cref="InvalidOperationException"/> will be thrown when attempting to add an item.
        /// </param>
        /// <param name="equalityComparer">The equality comparer used to locate items in the queue. If <c>null</c>,
        /// J2N's default <see cref="EqualityComparer{T}.Default"/> is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
        public PriorityQueue(int capacity, IComparer<T>? comparer, IEqualityComparer<T>? equalityComparer)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(SR.ArgumentOutOfRange_NeedCapacityAtLeast1);
            }

            elements = NewElementArray(capacity);
            this.comparer = comparer;
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Constructs a priority queue with capacity of <c>11</c> and the specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> that will be used to order this
        /// priority queue. If <c>null</c>, <typeparamref name="T"/> must implement <see cref="IComparable{T}"/>
        /// or an <see cref="InvalidOperationException"/> will be thrown when attempting to add an item.
        /// </param>
        public PriorityQueue(IComparer<T> comparer)
            : this(DefaultCapacity, comparer, null)
        {
        }

        /// <summary>
        /// Constructs a priority queue with capacity of <c>11</c> and the specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> that will be used to order this
        /// priority queue. If <c>null</c>, <typeparamref name="T"/> must implement <see cref="IComparable{T}"/>
        /// or an <see cref="InvalidOperationException"/> will be thrown when attempting to add an item.
        /// </param>
        /// <param name="equalityComparer">The equality comparer used to locate items in the queue. If <c>null</c>,
        /// J2N's default <see cref="EqualityComparer{T}.Default"/> is used.</param>
        public PriorityQueue(IComparer<T>? comparer, IEqualityComparer<T>? equalityComparer)
            : this(DefaultCapacity, comparer, equalityComparer)
        {
        }

        /// <summary>
        /// Creates a <see cref="PriorityQueue{T}"/> containing the elements in the
        /// specified collection. If the specified collection is an instance of
        /// a <see cref="SortedSet{T}"/> or is another <see cref="PriorityQueue{T}"/>, this
        /// priority queue will be ordered using the same <see cref="IComparer{T}"/>.
        /// Otherwise, this priority queue will be ordered according to the
        /// <see cref="IComparable{T}"/> natural ordering of its elements.
        /// </summary>
        /// <param name="collection">Collection to be inserted into priority queue.</param>
        /// <see cref="ArgumentNullException">If the specified collection or any
        /// of its elements are <c>null</c>.</see>
        /// <exception cref="InvalidOperationException">
        /// The collection is a <see cref="SortedSet{T}"/> or a <see cref="PriorityQueue{T}"/> and does not provide an <see cref="IComparer{T}"/> instance
        /// and <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// The collection is neither a <see cref="SortedSet{T}"/> or a <see cref="PriorityQueue{T}"/> instance
        /// and <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/>.
        /// </exception>
        public PriorityQueue(IEnumerable<T> collection)
            : this(collection, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="PriorityQueue{T}"/> containing the elements in the
        /// specified collection, comparer and equality comparer.
        /// If the specified collection is an instance of
        /// a <see cref="SortedSet{T}"/> or is another <see cref="PriorityQueue{T}"/>, this
        /// priority queue will be ordered using the same <see cref="IComparer{T}"/>.
        /// Otherwise, this priority queue will be ordered according to the
        /// <see cref="IComparable{T}"/> natural ordering of its elements.
        /// </summary>
        /// <param name="collection">Collection to be inserted into priority queue.</param>
        /// <param name="equalityComparer">The equality comparer used to locate items in the queue. If <c>null</c>,
        /// J2N's default <see cref="EqualityComparer{T}.Default"/> is used.</param>
        /// <exception cref="ArgumentNullException">If the specified collection or any
        /// of its elements are <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The collection is a <see cref="SortedSet{T}"/> or a <see cref="PriorityQueue{T}"/> and does not provide an <see cref="IComparer{T}"/> instance
        /// and <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// The collection is neither a <see cref="SortedSet{T}"/> or a <see cref="PriorityQueue{T}"/> instance
        /// and <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/>.
        /// </exception>
#pragma warning disable CS8618
        public PriorityQueue(IEnumerable<T> collection, IEqualityComparer<T>? equalityComparer)
#pragma warning restore CS8618
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

            if (collection is PriorityQueue<T> priorityQueue)
                InitFrom(priorityQueue);
            else if (collection is SortedSet<T> sortedSet)
                InitFrom(sortedSet);
            else
                InitFrom(collection, null);
        }

        /// <summary>
        /// Creates a <see cref="PriorityQueue{T}"/> containing the elements in the
        /// specified collection, comparer and equality comparer.
        /// If <paramref name="comparer"/> is <c>null</c>, this priority queue will be ordered according to the
        /// <see cref="IComparable{T}"/> natural ordering of its elements.
        /// </summary>
        /// <param name="collection">Collection to be inserted into priority queue.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> that will be used to order this
        /// priority queue. If <c>null</c>, <typeparamref name="T"/> must implement <see cref="IComparable{T}"/>
        /// or an <see cref="InvalidOperationException"/> will be thrown when attempting to add an item.
        /// </param>
        /// <param name="equalityComparer">The equality comparer used to locate items in the queue. If <c>null</c>,
        /// J2N's default <see cref="EqualityComparer{T}.Default"/> is used.</param>
        /// <exception cref="ArgumentNullException">If the specified collection or any
        /// of its elements are <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The collection is a <see cref="SortedSet{T}"/> or a <see cref="PriorityQueue{T}"/> and does not provide an <see cref="IComparer{T}"/> instance
        /// and <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// The collection is neither a <see cref="SortedSet{T}"/> or a <see cref="PriorityQueue{T}"/> instance
        /// and <typeparamref name="T"/> does not implement <see cref="IComparable{T}"/>.
        /// </exception>
#pragma warning disable CS8618
        public PriorityQueue(IEnumerable<T> collection, IComparer<T>? comparer, IEqualityComparer<T>? equalityComparer)
#pragma warning restore CS8618
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

            InitFrom(collection, comparer);
        }

        #endregion Constructors

        #region Initialization

        private void InitFrom(PriorityQueue<T> collection)
        {
            InitSize(collection);
            comparer = collection.Comparer;
            System.Array.Copy(collection.elements, 0, elements, 0, collection.Count);
            count = collection.Count;
        }

        private void InitFrom(SortedSet<T> collection)
        {
            InitSize(collection);
            comparer = collection.Comparer;
            foreach (var value in collection)
            {
                elements[count++] = value;
            }
        }

        private void InitFrom(IEnumerable<T> collection, IComparer<T>? comparer)
        {
            InitSize(collection);
            this.comparer = comparer;
            AddRange(collection);
        }

        #endregion Initialization

        #region Priority queue operations

        /// <summary>
        /// Gets the enumerator of the priority queue, which will not return elements
        /// in any specified ordering.
        /// </summary>
        /// <returns>The enumerator of the priority queue.</returns>
        /// <remarks>
        /// Returned enumerator does not iterate elements in sorted order.</remarks>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Gets the size of the priority queue. If the size of the queue is greater
        /// than the <see cref="int.MaxValue"/>, then it returns <see cref="int.MaxValue"/>.
        /// </summary>
        public virtual int Count => count;

        /// <summary>
        /// Removes all of the elements from this priority queue.
        /// </summary>
        public virtual void Clear()
        {
            elements.Fill(default!);
            count = 0;
        }

        /// <summary>
        /// Inserts the specified element into this priority queue.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="PriorityQueue{T}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <returns>always <c>true</c></returns>
        /// <exception cref="InvalidOperationException">The specified element cannot be
        /// compared with elements currently in this priority queue
        /// according to the priority queue's ordering.</exception>
        /// <remarks>This is similar to the Offer() method in the JDK.</remarks>
        public virtual bool Enqueue(T item) // J2N: renamed from Offer(), made item nullable to match .NET
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), SR.ArgumentNull_CollectionDoesntSupportNull);
            }
            GrowToSize(count + 1);
            elements[count] = item;
            SiftUp(count++);
            return true;
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the queue.
        /// </summary>
        /// <returns>The object that is removed from the beginning of the queue.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="PriorityQueue{T}"/> is empty.</exception>
        /// <remarks>This is similar to the Remove() method in the JDK.</remarks>
        public virtual T Dequeue() // J2N: Renamed from Remove()
        {
            if (count == 0)
                ThrowForEmptyQueue();

            T result = elements[0];
            RemoveAt(0);
            return result;
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the queue.
        /// </summary>
        /// <param name="result">The output head of the queue, or <c>null</c> if <see cref="Count"/> is 0.</param>
        /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
        /// <remarks>This is similar in behavior to the Poll() method in the JDK.</remarks>
        public virtual bool TryDequeue(out T result) // J2N: Restructured from Poll()
        {
            if (count != 0)
            {
                result = elements[0];
                RemoveAt(0);
                return true;
            }
            result = default!;
            return false;
        }

        /// <summary>
        /// Retrieves, but does not remove, the beginning of this queue.
        /// <para/>
        /// This implementation throws an <see cref="InvalidOperationException"/> if
        /// the queue is empty.
        /// </summary>
        /// <returns>The beginning of this queue.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="PriorityQueue{T}"/> is empty.</exception>
        /// <remarks>This is similar to the Element() method in the JDK.</remarks>
        public virtual T Peek() // J2N: Renamed from Element()
        {
            if (count == 0)
                ThrowForEmptyQueue();
            return elements[0];
        }

        /// <summary>
        /// Tries to get but does not remove the beginning of the queue.
        /// </summary>
        /// <param name="result">The output beginning of the queue, or <c>null</c> if <see cref="Count"/> is 0.</param>
        /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
        /// <remarks>This is similar in behavior to the Peek() method in the JDK.</remarks>
        public virtual bool TryPeek(out T result) // J2N: Restructured from Peek()
        {
            if (count != 0)
            {
                result = elements[0];
                return true;
            }
            result = default!;
            return false;
        }

        /// <summary>
        /// Gets the <see cref="IComparer{T}"/> used to order the elements in this
        /// queue, or <c>null</c> if this queue is sorted according to
        /// the <see cref="IComparable{T}"/> natural ordering of its elements.
        /// </summary>
        public virtual IComparer<T>? Comparer => this.comparer;

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> used to test elements in this
        /// queue for equality.
        /// </summary>
        public virtual IEqualityComparer<T> EqualityComparer => this.equalityComparer;

        /// <summary>
        /// Removes the specified object from the priority queue.
        /// </summary>
        /// <param name="item">the object to be removed.</param>
        /// <returns><c>true</c> if the object was in the priority queue, <c>false</c> if the object</returns>
        public virtual bool Remove(T item)
        {
            if (item == null || count == 0)
            {
                return false;
            }
            for (int i = 0; i < count; i++)
            {
                //if (item.Equals(elements[i]))
                if (equalityComparer.Equals(item, elements[i]))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds the specified object to the priority queue.
        /// </summary>
        /// <param name="item">the object to be added.</param>
        /// <exception cref="InvalidCastException">The specified element cannot be
        /// compared with elements currently in this priority queue
        /// according to the priority queue's ordering.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The element is not allowed to be added to the queue.
        /// This can only happen if <see cref="Enqueue(T)"/> is overridden and returns <c>false</c>.</exception>
        public virtual bool Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (Enqueue(item))
                return true;
            throw new InvalidOperationException();
        }

        void ICollection<T>.Add(T item) => Add(item);

        private struct Enumerator : IEnumerator<T>
        {
            private int currentIndex;
            private T current;
            private readonly PriorityQueue<T> priorityQueue;

            public Enumerator(PriorityQueue<T> priorityQueue)
            {
                this.priorityQueue = priorityQueue ?? throw new ArgumentNullException(nameof(priorityQueue));
                currentIndex = -1;
                current = default!;
            }

            public T Current => current;

            object? IEnumerator.Current => current;

            public void Dispose()
            {
                current = default!;
            }

            public bool MoveNext()
            {
                if (currentIndex < priorityQueue.count - 1)
                {
                    current = priorityQueue.elements[++currentIndex];
                    return true;
                }
                return false;
            }

            // J2N: Remove() not implemented

            public void Reset()
            {
                currentIndex = -1;
                current = default!;
            }
        }

        // J2N: readObject omitted

        private static T[] NewElementArray(int capacity)
        {
            return new T[capacity];
        }

        // J2N: writeObject omitted

        /// <summary>
        /// Removes the element at the specified <paramref name="index"/> from queue.
        /// <para/>
        /// Normally this method leaves the elements at up to <paramref name="index"/>-1,
        /// inclusive, untouched.  Under these circumstances, it returns
        /// <c>null</c>.  Occasionally, in order to maintain the heap invariant,
        /// it must swap a later element of the list with one earlier than
        /// <paramref name="index"/>.  Under these circumstances, this method returns the element
        /// that was previously at the end of the list and is now at some
        /// position before <paramref name="index"/>.
        /// </summary>
        private void RemoveAt(int index)
        {
            count--;
            elements[index] = elements[count];
            SiftDown(index);
            elements[count] = default!;
        }

        private int Compare(T o1, T o2)
        {
            if (comparer != null)
                return comparer.Compare(o1, o2);
            if (o1 is IComparable<T> o1Comparable)
                return o1Comparable.CompareTo(o2);
            throw new InvalidOperationException(SR.InvalidOperation_ComparerRequired);
        }

        private void SiftUp(int childIndex)
        {
            T target = elements[childIndex];
            int parentIndex;
            while (childIndex > 0)
            {
                parentIndex = (childIndex - 1) / 2;
                T parent = elements[parentIndex];
                if (Compare(parent, target) <= 0 )
                {
                    break;
                }
                elements[childIndex] = parent;
                childIndex = parentIndex;
            }
            elements[childIndex] = target;
        }

        private void SiftDown(int rootIndex)
        {
            T target = elements[rootIndex];
            int childIndex;
            while ((childIndex = rootIndex * 2 + 1) < count)
            {
                if (childIndex + 1 < count
                        && Compare(elements[childIndex + 1], elements[childIndex]) < 0)
                {
                    childIndex++;
                }
                if (Compare(target, elements[childIndex]) <= 0)
                {
                    break;
                }
                elements[rootIndex] = elements[childIndex];
                rootIndex = childIndex;
            }
            elements[rootIndex] = target;
        }

        private void InitSize(ICollection<T> collection)
        {
            if (collection.Count == 0)
            {
                elements = NewElementArray(1);
            }
            else
            {
                int capacity = (int)Math.Ceiling(collection.Count
                        * DefaultInitialCapacityRatio);
                elements = NewElementArray(capacity);
            }
        }

        private void InitSize(IEnumerable<T> collection)
        {
            int size;
            if (collection is ICollection<T> col)
                size = col.Count;
            else
                size = collection.Count();

            if (size == 0)
            {
                elements = NewElementArray(1);
            }
            else
            {
                int capacity = (int)Math.Ceiling(size * DefaultInitialCapacityRatio);
                elements = NewElementArray(capacity);
            }
        }

        private void GrowToSize(int size)
        {
            if (size > elements.Length)
            {
                T[] newElements = NewElementArray(size * DefaultCapacityRatio);
                System.Array.Copy(elements, 0, newElements, 0, elements.Length);
                elements = newElements;
            }
        }

        private void ThrowForEmptyQueue()
        {
            Debug.Assert(count == 0);
            throw new InvalidOperationException(SR.InvalidOperation_EmptyQueue);
        }

        #endregion Priority queue operations

        #region AbstractQueue operations

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the <see cref="PriorityQueue{T}"/>.
        /// The collection itself cannot be <c>null</c>, but it can contain elements that are <c>null</c>, if type
        /// <typeparamref name="T"/> is a reference type.</param>
        /// <returns><c>true</c> if 1 or more items are added to the <see cref="PriorityQueue{T}"/>; othewise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> is the same instance as this collection.</exception>
        public virtual bool AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (ReferenceEquals(this, collection))
            {
                throw new ArgumentException(SR.Argument_CollectionMustNotBeThis, nameof(collection));
            }
            bool result = false;
            foreach (var value in collection)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(SR.ArgumentNull_CollectionDoesntSupportNull);
                }
                if (Add(value))
                {
                    result = true;
                }
            }
            return result;
        }

        #endregion

        #region ICollection<T> Members

        private int IndexOf(T o)
        {
            for (int i = 0; i < count; i++)
                if (equalityComparer.Equals(o, elements[i]))
                    return i;
            return -1;
        }

        /// <summary>
        /// Returns <c>true</c> if this queue contains the specified element.
        /// More formally, returns <c>true</c> if and only if this queue contains
        /// at least one element <paramref name="item"/> such that <c>o.Equals(item)</c>.
        /// </summary>
        /// <param name="item">The object to locate in the priority queue</param>
        /// <returns><c>true</c> if item is found in the priority queue; otherwise, <c>false</c>.</returns>
        public virtual bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        /// <summary>
        /// Copies the elements of a <see cref="PriorityQueue{T}"/> collection to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="PriorityQueue{T}"/> object.
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
        /// It is not guaranteed that items will be copied in the sorted order.
        /// <para/>
        /// Calling this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, SR.ArgumentOutOfRange_NeedNonNegNum);

            // will array, starting at arrayIndex, be able to hold elements? Note: not
            // checking arrayIndex >= array.Length (consistency with list of allowing
            // count of 0; subsequent check takes care of the rest)
            if (arrayIndex > array.Length || count > array.Length - arrayIndex)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            int size = this.count;
            if (array.Length < size)
            {
                elements.CopyTo(array, arrayIndex);
            }
            else
            {
                System.Array.Copy(elements, 0, array, arrayIndex, size);
                if (array.Length > size)
                {
                    array[size] = default!;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <remarks>
        /// For priority queue this property returns <c>false</c>.
        /// </remarks>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>Enumerator</returns>
        /// <remarks>
        /// Returned enumerator does not iterate elements in sorted order.</remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion ICollection<T> Members

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
        public virtual string ToString(IFormatProvider? formatProvider)
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
    }
}
