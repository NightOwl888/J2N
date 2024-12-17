#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Collections.Generic;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace J2N.Collections.ObjectModel
{
    /// <summary>
    /// Provides the base class for a generic read-only set that is structurally equatable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <remarks>
    /// Public static (Shared in Visual Basic) members of this type are thread safe.
    /// Any instance members are not guaranteed to be thread safe.
    /// <para/>
    /// A <see cref="ReadOnlySet{T}"/> can support multiple readers concurrently, as long
    /// as the collection is not modified. Even so, enumerating through a collection is
    /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration,
    /// you can lock the collection during the entire enumeration. To allow the collection to be
    /// accessed by multiple threads for reading and writing, you must implement your own synchronization.
    /// </remarks>
    [SuppressMessage("Style", "IDE0019:Use pattern matching", Justification = "Following Microsoft's code style")]
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class ReadOnlySet<T> : ISet<T>, ICollection,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyCollection<T>,
#endif
#if FEATURE_READONLYSET
        IReadOnlySet<T>,
#endif
        IStructuralEquatable, IStructuralFormattable
    {
        private static readonly bool TIsValueTypeOrStringOrStructuralEquatable = typeof(T).IsValueType || typeof(IStructuralEquatable).IsAssignableFrom(typeof(T)) || typeof(string).Equals(typeof(T));

        private readonly ISet<T> set;
        private readonly SetEqualityComparer<T> structuralEqualityComparer;
        private readonly IFormatProvider toStringFormatProvider;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private object? syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySet{T}"/> class that is a read-only wrapper around the specified set.
        /// </summary>
        /// <param name="set">The set to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="set"/> is <c>null</c>.</exception>
        /// <remarks>
        /// To prevent any modifications to <paramref name="set"/>, expose <paramref name="set"/> only through this wrapper.
        /// <para/>
        /// A collection that is read-only is simply a collection with a wrapper that prevents modifying the collection;
        /// therefore, if changes are made to the underlying collection, the read-only collection reflects those changes.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public ReadOnlySet(ISet<T> set)
            : this(set, TIsValueTypeOrStringOrStructuralEquatable ? SetEqualityComparer<T>.Default : SetEqualityComparer<T>.Aggressive, StringFormatter.CurrentCulture)
        {
        }

        internal ReadOnlySet(ISet<T> set, SetEqualityComparer<T> structuralEqualityComparer, IFormatProvider toStringFormatProvider)
        {
            this.set = set ?? throw new ArgumentNullException(nameof(set));
            this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
            this.toStringFormatProvider = toStringFormatProvider ?? throw new ArgumentNullException(nameof(toStringFormatProvider));
        }

        /// <summary>
        /// Returns the <see cref="ISet{T}"/> that the <see cref="ReadOnlySet{T}"/> wraps.
        /// </summary>
        protected internal ISet<T> Items => set; // internal for testing

        #region ISet<T> Members

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ReadOnlySet{T}"/> instance.
        /// </summary>
        public int Count => set.Count;

        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="ReadOnlySet{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlySet{T}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if value is found in the <see cref="ReadOnlySet{T}"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method determines equality using the equality comparer of the wrapped set.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool Contains(T item) => set.Contains(item);

        /// <summary>
        /// Copies the entire <see cref="ReadOnlySet{T}"/> to a compatible one-dimensional <see cref="Array"/>,
        /// starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements
        /// copied from <see cref="ReadOnlySet{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ReadOnlySet{T}"/>
        /// is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// This method uses <see cref="Array.Copy(Array, int, Array, int, int)"/> to copy the elements.
        /// <para/>
        /// The elements are copied to the <see cref="Array"/> in the same order that the enumerator
        /// iterates through the <see cref="ReadOnlySet{T}"/>.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is Count.
        /// </remarks>
        public void CopyTo(T[] array, int index) => set.CopyTo(array, index);

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ReadOnlySet{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="ReadOnlySet{T}"/>.</returns>
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
        public IEnumerator<T> GetEnumerator() => set.GetEnumerator();

        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        /// <summary>
        /// Determines whether a <see cref="ReadOnlySet{T}"/> object is a proper subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ReadOnlySet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="ReadOnlySet{T}"/> object is a proper subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a proper subset of any other collection. Therefore, this method returns <c>true</c> if the
        /// collection represented by the current <see cref="ReadOnlySet{T}"/> object is empty unless the other
        /// parameter is also an empty set.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is greater than or equal to the number of
        /// elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="ReadOnlySet{T}"/> collection with the same equality
        /// comparer as the current <see cref="ReadOnlySet{T}"/> object, then this method is an O(n) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c> is the
        /// number of elements in other.
        /// </remarks>
        [System.Security.SecurityCritical]
        public bool IsProperSubsetOf(IEnumerable<T> other) => set.IsProperSubsetOf(other);

        /// <summary>
        /// Determines whether a <see cref="ReadOnlySet{T}"/> object is a proper superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ReadOnlySet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="ReadOnlySet{T}"/> object is a proper superset of other; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a proper superset of any other collection. Therefore, this method returns <c>true</c> if the
        /// collection represented by the other parameter is empty unless the current <see cref="ReadOnlySet{T}"/> collection is also empty.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is less than or equal to the number of elements in other.
        /// <para/>
        /// If the collection represented by other is a <see cref="ReadOnlySet{T}"/> collection with the same equality
        /// comparer as the current <see cref="ReadOnlySet{T}"/> object, this method is an O(<c>n</c>) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other and <c>m</c>
        /// is <see cref="Count"/>.
        /// </remarks>
        public bool IsProperSupersetOf(IEnumerable<T> other) => set.IsProperSupersetOf(other);

        /// <summary>
        /// Determines whether a <see cref="ReadOnlySet{T}"/> object is a subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ReadOnlySet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="ReadOnlySet{T}"/> object is a subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a subset of any other collection, including an empty set; therefore, this method returns
        /// <c>true</c> if the collection represented by the current <see cref="ReadOnlySet{T}"/> object is empty,
        /// even if the <paramref name="other"/> parameter is an empty set.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is greater than the number of
        /// elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="ReadOnlySet{T}"/> collection with the same
        /// equality comparer as the current <see cref="ReadOnlySet{T}"/> object, this method is an O(<c>n</c>) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c>
        /// is the number of elements in other.
        /// </remarks>
        [System.Security.SecurityCritical]
        public bool IsSubsetOf(IEnumerable<T> other) => set.IsSubsetOf(other);

        /// <summary>
        /// Determines whether a <see cref="ReadOnlySet{T}"/> object is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ReadOnlySet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="ReadOnlySet{T}"/> object is a superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// All collections, including the empty set, are supersets of the empty set. Therefore, this method returns
        /// <c>true</c> if the collection represented by the other parameter is empty, even if the current
        /// <see cref="ReadOnlySet{T}"/> object is empty.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is less than the number of elements
        /// in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="ReadOnlySet{T}"/> collection with the same
        /// equality comparer as the current <see cref="ReadOnlySet{T}"/> object, this method is an O(<c>n</c>) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other
        /// and <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public bool IsSupersetOf(IEnumerable<T> other) => set.IsSupersetOf(other);

        /// <summary>
        /// Determines whether the current <see cref="ReadOnlySet{T}"/> object and a specified collection
        /// share common elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ReadOnlySet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="ReadOnlySet{T}"/> object and <paramref name="other"/> share
        /// at least one common element; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in other.
        /// </remarks>
        public bool Overlaps(IEnumerable<T> other) => set.Overlaps(other);

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        /// <summary>
        /// Determines whether a <see cref="ReadOnlySet{T}"/> object and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ReadOnlySet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="ReadOnlySet{T}"/> object is equal to <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="SetEquals(IEnumerable{T})"/> method ignores duplicate entries and the order of elements in the
        /// <paramref name="other"/> parameter.
        /// <para/>
        /// If the collection represented by other is a <see cref="ReadOnlySet{T}"/> collection with the same equality
        /// comparer as the current <see cref="ReadOnlySet{T}"/> object, this method is an O(<c>n</c>) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other and
        /// <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        [System.Security.SecurityCritical]
        public bool SetEquals(IEnumerable<T> other) => set.SetEquals(other);


        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)set).GetEnumerator();

        #endregion

        #region IReadOnlySet<T> Members

#if FEATURE_READONLYSET

        bool IReadOnlySet<T>.Contains(T item) => set.Contains(item);

        bool IReadOnlySet<T>.IsProperSubsetOf(IEnumerable<T> other) => set.IsProperSubsetOf(other);

        bool IReadOnlySet<T>.IsProperSupersetOf(IEnumerable<T> other) => set.IsProperSupersetOf(other);

        bool IReadOnlySet<T>.IsSubsetOf(IEnumerable<T> other) => set.IsSubsetOf(other);

        bool IReadOnlySet<T>.IsSupersetOf(IEnumerable<T> other) => set.IsSupersetOf(other);

        bool IReadOnlySet<T>.Overlaps(IEnumerable<T> other) => set.Overlaps(other);

        bool IReadOnlySet<T>.SetEquals(IEnumerable<T> other) => set.SetEquals(other);
#endif

        #endregion

        #region ICollection Members

        bool ICollection.IsSynchronized
        {
            get
            {
                if (set is ICollection col)
                    return col.IsSynchronized;
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                {
                    if (set is ICollection col)
                        syncRoot = col.SyncRoot;
                    System.Threading.Interlocked.CompareExchange<object?>(ref syncRoot, new object(), null);
                }
                return syncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException(SR.Arg_NonZeroLowerBound, nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - index < Count)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            T[]? tarray = array as T[];
            if (tarray != null)
            {
                CopyTo(tarray, index);
            }
            else
            {
                object?[]? objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                }

                try
                {
                    foreach (var item in set)
                        objects[index++] = item;
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType, nameof(array));
                }
            }
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
            => SetEqualityComparer<T>.Equals(set, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current set using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current set.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
            => SetEqualityComparer<T>.GetHashCode(set, comparer);

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
            => Equals(obj, structuralEqualityComparer);

        /// <summary>
        /// Gets the hash code for the current set. The hash code is calculated 
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <seealso cref="GetHashCode(IEqualityComparer)"/>
        public override int GetHashCode()
            => GetHashCode(structuralEqualityComparer);

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
            => CollectionUtil.ToString(formatProvider, format, set);

        /// <summary>
        /// Returns a string that represents the current set using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        public override string ToString()
            => ToString("{0}", toStringFormatProvider);

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
            => ToString(format, toStringFormatProvider);

        #endregion
    }
}
