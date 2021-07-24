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
using System.Reflection;


namespace J2N.Collections.ObjectModel
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Provides the base class for a generic read-only collection that is structurally equatable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <remarks>
    /// Public static (Shared in Visual Basic) members of this type are thread safe.
    /// Any instance members are not guaranteed to be thread safe.
    /// <para/>
    /// A <see cref="ReadOnlyCollection{T}"/> can support multiple readers concurrently, as long
    /// as the collection is not modified. Even so, enumerating through a collection is
    /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration,
    /// you can lock the collection during the entire enumeration. To allow the collection to be
    /// accessed by multiple threads for reading and writing, you must implement your own synchronization.
    /// </remarks>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class ReadOnlyCollection<T> : ICollection<T>,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyCollection<T>,
#endif
        ICollection, IStructuralEquatable, IStructuralFormattable
    {
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
        private static readonly bool TIsValueTypeOrStringOrStructuralEquatable = typeof(T).GetTypeInfo().IsValueType || typeof(IStructuralEquatable).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) || typeof(string).Equals(typeof(T));
#else
        private static readonly bool TIsValueTypeOrStringOrStructuralEquatable = typeof(T).IsValueType || typeof(IStructuralEquatable).IsAssignableFrom(typeof(T)) || typeof(string).Equals(typeof(T));
#endif

        internal readonly ICollection<T> collection; // internal for testing
        private readonly StructuralEqualityComparer structuralEqualityComparer;
        private readonly IFormatProvider toStringFormatProvider;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private object? syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollection{T}"/> class that is a read-only wrapper around the specified collection.
        /// </summary>
        /// <param name="collection">The collection to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// To prevent any modifications to <paramref name="collection"/>, expose <paramref name="collection"/> only through this wrapper.
        /// <para/>
        /// A collection that is read-only is simply a collection with a wrapper that prevents modifying the collection;
        /// therefore, if changes are made to the underlying collection, the read-only collection reflects those changes.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public ReadOnlyCollection(ICollection<T> collection)
            : this(collection, TIsValueTypeOrStringOrStructuralEquatable ? StructuralEqualityComparer.Default : StructuralEqualityComparer.Aggressive, StringFormatter.CurrentCulture)
        {
        }

        internal ReadOnlyCollection(ICollection<T> collection, StructuralEqualityComparer structuralEqualityComparer, IFormatProvider toStringFormatProvider)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
            this.toStringFormatProvider = toStringFormatProvider ?? throw new ArgumentNullException(nameof(toStringFormatProvider));
        }

        /// <summary>
        /// Returns the <see cref="ICollection{T}"/> that the <see cref="ReadOnlyCollection{T}"/> wraps.
        /// </summary>
        protected ICollection<T> Items => collection;

        #region ICollection<T> Members

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ReadOnlyCollection{T}"/> instance.
        /// </summary>
        public int Count => collection.Count;

        bool ICollection<T>.IsReadOnly => true;

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                {
                    if (collection is ICollection col)
                        syncRoot = col.SyncRoot;
                    System.Threading.Interlocked.CompareExchange<object?>(ref syncRoot, new object(), null);
                }
                return syncRoot;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                if (collection is ICollection col)
                    return col.IsSynchronized;
                return false;
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyCollection{T}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if value is found in the <see cref="ReadOnlyCollection{T}"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method determines equality using the equality comparer of the wrapped set.
        /// <para/>
        /// This method performs a linear search; therefore, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool Contains(T item) => collection.Contains(item);

        /// <summary>
        /// Copies the entire <see cref="ReadOnlyCollection{T}"/> to a compatible one-dimensional <see cref="Array"/>,
        /// starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements
        /// copied from <see cref="ReadOnlyCollection{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ReadOnlySet{T}"/>
        /// is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// This method uses <see cref="Array.Copy(Array, int, Array, int, int)"/> to copy the elements.
        /// <para/>
        /// The elements are copied to the <see cref="Array"/> in the same order that the enumerator
        /// iterates through the <see cref="ReadOnlyCollection{T}"/>.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is Count.
        /// </remarks>
        public void CopyTo(T[] array, int index) => collection.CopyTo(array, index);

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException(SR.Arg_NonZeroLowerBound);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - index < Count)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            if (array is T[] items)
            {
                collection.CopyTo(items, index);
            }
            else
            {
                //
                // Catch the obvious case assignment will fail.
                // We can found all possible problems by doing the check though.
                // For example, if the element type of the Array is derived from T,
                // we can't figure out if we can successfully copy the element beforehand.
                //

#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
#pragma warning disable CS8604 // Possible null reference argument.
                TypeInfo targetType = array.GetType().GetElementType().GetTypeInfo();
#pragma warning restore CS8604 // Possible null reference argument.
                TypeInfo sourceType = typeof(T).GetTypeInfo();
#else
#pragma warning disable CS8604 // Possible null reference argument.
                Type targetType = array.GetType();
#pragma warning restore CS8604 // Possible null reference argument.
                Type sourceType = typeof(T);
#endif
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType);
                }

                //
                // We can't cast array of value type to object[], so we don't support 
                // widening of primitive types here.
                //
                if (!(array is object?[] objects))
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType);
                }

                try
                {
                    foreach (var item in collection)
                    {
                        objects[index++] = item;
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="ReadOnlyCollection{T}"/>.</returns>
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
        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException(SR.NotSupported_ReadOnlyCollection);
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)collection).GetEnumerator();

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
            if (other is null)
                return false;
            if (ReferenceEquals(collection, other))
                return true;

            if (collection is IList<T> list)
            {
                if (!(other is IList<T> otherList))
                    return false;

                if (comparer is ListEqualityComparer<T> listComparer)
                    return listComparer.Equals(list, otherList);

                if (comparer is StructuralEqualityComparer)
                {
                    ListEqualityComparer<T> toUse;
                    if (StructuralEqualityComparer.Aggressive.Equals(comparer))
                        toUse = ListEqualityComparer<T>.Aggressive;
                    else
                        toUse = ListEqualityComparer<T>.Default;

                    return toUse.Equals(list, otherList);
                }
            }
            else if (collection is ISet<T> set)
            {
                if (!(other is ISet<T> otherSet))
                    return false;

                if (comparer is SetEqualityComparer<T> setComparer)
                    return setComparer.Equals(set, otherSet);

                if (comparer is StructuralEqualityComparer)
                {
                    SetEqualityComparer<T> toUse;
                    if (StructuralEqualityComparer.Aggressive.Equals(comparer))
                        toUse = SetEqualityComparer<T>.Aggressive;
                    else
                        toUse = SetEqualityComparer<T>.Default;

                    return toUse.Equals(set, otherSet);
                }
            }
            else if (collection.GetType().ImplementsGenericInterface(typeof(IDictionary<,>)))
            {
                if (!other.GetType().ImplementsGenericInterface(typeof(IDictionary<,>)))
                    return false;

                if (comparer is IDictionaryEqualityComparer dictionaryComparer)
                    return dictionaryComparer.Equals(collection, other);

                // J2N TODO: Convert a non-generic StructuralEqualityComparer mode to a generic one
                //if (comparer is StructuralEqualityComparer)
                //{
                //    DictionaryEqualityComparer<object, object>

                //    //IEqualityComparer toUse;
                //    //if (StructuralEqualityComparer.Aggressive.Equals(comparer))
                //    //    toUse = DictionaryEqualityComparer

                //}
            }

            // Custom collection type
            return comparer.Equals(collection, other);
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
            if (collection is IList<T> list)
            {
                if (comparer is ListEqualityComparer<T> listComparer)
                    return listComparer.GetHashCode(list);

                if (comparer is StructuralEqualityComparer)
                {
                    ListEqualityComparer<T> toUse;
                    if (StructuralEqualityComparer.Aggressive.Equals(comparer))
                        toUse = ListEqualityComparer<T>.Aggressive;
                    else
                        toUse = ListEqualityComparer<T>.Default;

                    return toUse.GetHashCode(list);
                }

                return comparer.GetHashCode(list);
            }
            else if (collection is ISet<T> set)
            {
                if (comparer is SetEqualityComparer<T> setComparer)
                    return setComparer.GetHashCode(set);

                if (comparer is StructuralEqualityComparer)
                {
                    SetEqualityComparer<T> toUse;
                    if (StructuralEqualityComparer.Aggressive.Equals(comparer))
                        toUse = SetEqualityComparer<T>.Aggressive;
                    else
                        toUse = SetEqualityComparer<T>.Default;

                    return toUse.GetHashCode(set);
                }
            }
            else if (collection.GetType().ImplementsGenericInterface(typeof(IDictionary<,>)))
            {
                if (comparer is IDictionaryEqualityComparer dictionaryComparer)
                    return dictionaryComparer.GetHashCode(collection);

                // J2N TODO: Convert a non-generic StructuralEqualityComparer mode to a generic one
                //if (comparer is StructuralEqualityComparer)
                //{
                //    DictionaryEqualityComparer<object, object>

                //    //IEqualityComparer toUse;
                //    //if (StructuralEqualityComparer.Aggressive.Equals(comparer))
                //    //    toUse = DictionaryEqualityComparer

                //}
            }

            // Custom collection type
            return comparer.GetHashCode(collection);
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
            => Equals(obj, structuralEqualityComparer);

        /// <summary>
        /// Gets the hash code for the current list. The hash code is calculated 
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => GetHashCode(structuralEqualityComparer);

        #endregion

        #region ToString

        /// <summary>
        /// Returns a string that represents the current collection using the specified
        /// <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current collection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
            => CollectionUtil.ToString(formatProvider, format, collection);

        /// <summary>
        /// Returns a string that represents the current collection using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current collection.</returns>
        public override string ToString()
            => ToString("{0}", toStringFormatProvider);


        /// <summary>
        /// Returns a string that represents the current collection using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current collection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current collection using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current collection.</returns>
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
