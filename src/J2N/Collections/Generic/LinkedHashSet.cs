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

using J2N.Collections.ObjectModel;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using SCG = System.Collections.Generic;


namespace J2N.Collections.Generic
{
    /// <summary>
    /// <see cref="LinkedHashSet{T}"/> is a variant of <see cref="HashSet{T}"/>. Its entries are kept in a
    /// doubly-linked list. The iteration order is the order in which entries were inserted.
    /// <para/>
    /// Like <see cref="HashSet{T}"/>, <see cref="LinkedHashSet{T}"/> is not thread safe, so access by multiple threads
    /// must be synchronized by an external mechanism.
    /// </summary>
    /// <remarks>
    /// This implementation uses an internal variation of <see cref="SCG.HashSet{T}"/> as a backing set. It calls
    /// <see cref="SCG.HashSet{T}.TrimExcess()"/> when needed to ensure insertion order.
    /// This is done by tracking whether an operation has removed items from <see cref="LinkedHashSet{T}"/>. If so,
    /// the next operation that adds items to the <see cref="LinkedHashSet{T}"/> will call <see cref="TrimExcess()"/>
    /// automatically, which also toggles off the tracking flag. This ensures the performance impact is negligible
    /// when calling methods that add or remove items multiple times in succession, however, there will be an impact
    /// when alternating between the remove and add operations in rapid succession or when calling
    /// <see cref="SymmetricExceptWith(IEnumerable{T})"/>, which does both a remove and an add operation.
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
    /// <typeparam name="T">The type of elements in the set.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class LinkedHashSet<T> : ICollection<T>, ISet<T>,
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
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private readonly Net5.HashSet<T> hashSet;
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private bool requiresTrimExcess;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class that is empty
        /// and uses <see cref="EqualityComparer{T}.Default"/> for the set type.
        /// </summary>
        /// <remarks>
        /// The capacity of a <see cref="LinkedHashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="LinkedHashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public LinkedHashSet() : this((IEqualityComparer<T>?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class that is empty, but has reserved
        /// space for <paramref name="capacity"/> items and uses <see cref="EqualityComparer{T}.Default"/> for the set type.
        /// </summary>
        /// <param name="capacity">The initial size of the <see cref="LinkedHashSet{T}"/>.</param>
        /// <remarks>
        /// Since resizes are relatively expensive (require rehashing), this attempts to minimize the need
        /// to resize by setting the initial capacity based on the value of the <paramref name="capacity"/>.
        /// </remarks>
        public LinkedHashSet(int capacity) : this(capacity, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class that uses <see cref="EqualityComparer{T}.Default"/>
        /// for the set type, contains elements copied from the specified collection, and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The capacity of a <see cref="LinkedHashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="LinkedHashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// If <paramref name="collection"/> contains duplicates, the set will contain one of each unique element.No exception will
        /// be thrown. Therefore, the size of the resulting set is not identical to the size of <paramref name="collection"/>.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in the <paramref name="collection"/> parameter.
        /// </remarks>
        public LinkedHashSet(IEnumerable<T> collection) : this(collection, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class that is empty and uses the
        /// specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <remarks>
        /// The capacity of a <see cref="LinkedHashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="LinkedHashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public LinkedHashSet(IEqualityComparer<T>? comparer)
        {
            hashSet = new Net5.HashSet<T>(comparer ?? EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class that uses the specified equality comparer
        /// for the set type, and has sufficient capacity to accommodate <paramref name="capacity"/> elements.
        /// </summary>
        /// <param name="capacity">The initial size of the <see cref="HashSet{T}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <remarks>Since resizes are relatively expensive (require rehashing), this attempts to minimize the need to resize
        /// by setting the initial capacity based on the value of the <paramref name="capacity"/>.</remarks>
        public LinkedHashSet(int capacity, IEqualityComparer<T>? comparer)
        {
            hashSet = new Net5.HashSet<T>(capacity, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class that uses the specified equality comparer for the set type,
        /// contains elements copied from the specified collection, and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The capacity of a <see cref="LinkedHashSet{T}"/> object is the number of elements that the object can hold. A <see cref="LinkedHashSet{T}"/>
        /// object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// If <paramref name="collection"/> contains duplicates, the set will contain one of each unique element. No exception will be thrown. Therefore,
        /// the size of the resulting set is not identical to the size of <paramref name="collection"/>.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in the <paramref name="collection"/> parameter.
        /// </remarks>
        public LinkedHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
        {
            hashSet = new Net5.HashSet<T>(collection, comparer);
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
        protected LinkedHashSet(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            hashSet = new HashSetWrapper(info, context);
        }
#endif

        #endregion

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlySet{T}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="LinkedHashSet{T}"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="LinkedHashSet{T}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlySet{T}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="LinkedHashSet{T}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlySet<T> AsReadOnly()
            => new ReadOnlySet<T>(this);

        #endregion AsReadOnly

        #region Nested Class: HashSetWrapper

#if FEATURE_SERIALIZABLE
        [Serializable]
        private class HashSetWrapper : Net5.HashSet<T>
        {
            [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
            public HashSetWrapper(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
#endif

        #endregion

        #region HashSet<T> Members

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> object that is used
        /// to determine equality for the values in the set.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public IEqualityComparer<T> EqualityComparer => hashSet.EqualityComparer;

        /// <summary>
        /// Copies the elements of a <see cref="LinkedHashSet{T}"/> object to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="LinkedHashSet{T}"/> object.
        /// The array must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void CopyTo(T[] array)
            => hashSet.CopyTo(array);

        /// <summary>
        /// Copies the specified number of elements of a <see cref="LinkedHashSet{T}"/>
        /// object to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="LinkedHashSet{T}"/> object.
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
            => hashSet.CopyTo(array, arrayIndex, count);

        /// <summary>
        /// Returns an <see cref="IEqualityComparer"/> object that can be used
        /// for equality testing of a <see cref="LinkedHashSet{T}"/> object
        /// as well as any nested objects that implement <see cref="IStructuralEquatable"/>.
        /// <para/>
        /// Usage Note: This is exactly the same as <see cref="SetEqualityComparer{T}.Default"/>.
        /// It is included here to cover the <see cref="SCG.HashSet{T}"/> API.
        /// </summary>
        /// <returns>An <see cref="IEqualityComparer"/> object that can be used for deep
        /// equality testing of the <see cref="LinkedHashSet{T}"/> object.</returns>
        /// <remarks>
        /// The <see cref="IEqualityComparer"/> object checks for equality for multiple levels.
        /// Nested reference types that implement <see cref="IStructuralEquatable"/> are also compared.
        /// </remarks>
        public static IEqualityComparer<ISet<T>> CreateSetComparer()
        {
            return SetEqualityComparer<T>.Default;
        }

        /// <summary>
        /// Ensures that this hash set can hold the specified number of elements without growing.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public int EnsureCapacity(int capacity)
            => hashSet.EnsureCapacity(capacity);

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and returns the data
        /// needed to serialize a <see cref="LinkedHashSet{T}"/> object.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains
        /// the information required to serialize the <see cref="LinkedHashSet{T}"/> object.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure that
        /// contains the source and destination of the serialized stream associated with the <see cref="LinkedHashSet{T}"/> object.</param>
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
            => hashSet.GetObjectData(info, context);

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and raises the
        /// deserialization event when the deserialization is complete.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> object associated with the
        /// current <see cref="LinkedHashSet{T}"/> object is invalid.
        /// </exception>
        /// <remarks>
        /// Calling this method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public virtual void OnDeserialization(object? sender)
            => hashSet.OnDeserialization(sender);
#endif

        /// <summary>
        /// Removes all elements that match the conditions defined by the specified
        /// predicate from a <see cref="LinkedHashSet{T}"/> collection.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines
        /// the conditions of the elements to remove.</param>
        /// <returns>The number of elements that were removed from the
        /// <see cref="LinkedHashSet{T}"/> collection.</returns>
        public int RemoveWhere(Predicate<T> match)
        {
            int result = hashSet.RemoveWhere(match);
            requiresTrimExcess |= result > 0;
            return result;
        }

        /// <summary>
        /// Sets the capacity of a <see cref="LinkedHashSet{T}"/> object to the actual
        /// number of elements it contains, rounded up to a nearby, implementation-specific value.
        /// </summary>
        /// <remarks>
        /// You can use the <see cref="TrimExcess()"/> method to minimize a <see cref="LinkedHashSet{T}"/>
        /// object's memory overhead once it is known that no new elements will be added. To completely
        /// clear a <see cref="LinkedHashSet{T}"/> object and release all memory referenced by it,
        /// call this method after calling the <see cref="Clear()"/> method.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void TrimExcess()
        {
            hashSet.TrimExcess();
            requiresTrimExcess = false;
        }

        /// <summary>
        /// Searches the set for a given value and returns the equal value it finds, if any.
        /// </summary>
        /// <param name="equalValue">The value to search for.</param>
        /// <param name="actualValue">The value from the set that the search found, or the
        /// default value of <typeparamref name="T"/> when the search yielded no match.</param>
        /// <returns>A value indicating whether the search was successful.</returns>
        /// <remarks>
        /// This can be useful when you want to reuse a previously stored reference instead
        /// of a newly constructed one (so that more sharing of references can occur) or to
        /// look up a value that has more complete data than the value you currently have,
        /// although their comparer functions indicate they are equal.
        /// </remarks>
        public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
            => hashSet.TryGetValue(equalValue, out actualValue!);

        #endregion

        #region ISet<T> Members

        /// <summary>
        /// Gets the number of elements that are contained in a set.
        /// </summary>
        /// <remarks>
        /// The capacity of a <see cref="LinkedHashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="LinkedHashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// The capacity is always greater than or equal to <see cref="Count"/>. If <see cref="Count"/> exceeds the
        /// capacity while adding elements, the capacity is set to the first prime number that is greater than
        /// double the previous capacity.
        /// <para/>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => hashSet.Count;

        /// <summary>
        /// Gets a value indicating whether a collection is read-only.
        /// </summary>
        /// <remarks>
        /// Retrieving the value of this property is an O(1) operation.
        /// </remarks>
        public bool IsReadOnly => ((ISet<T>)hashSet).IsReadOnly;

        /// <summary>
        /// Adds the specified element to a set.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns><c>true</c> if the element is added to the <see cref="LinkedHashSet{T}"/> object;
        /// <c>false</c> if the element is already present.</returns>
        /// <remarks>
        /// If <see cref="Count"/> already equals the capacity of the <see cref="LinkedHashSet{T}"/> object,
        /// the capacity is automatically adjusted to accommodate the new item.
        /// <para/>
        /// If <see cref="Count"/> is less than the capacity of the internal array, this method is an
        /// O(1) operation. If the <see cref="LinkedHashSet{T}"/> object must be resized, this method
        /// becomes an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool Add(T item)
        {
            // Trim excess before we add to ensure insertion order
            if (requiresTrimExcess)
                TrimExcess();
            return hashSet.Add(item);
        }

        /// <summary>
        /// Removes all elements from a <see cref="LinkedHashSet{T}"/> object.
        /// </summary>
        /// <remarks>
        /// <see cref="Count"/> is set to zero and references to other objects from elements of the
        /// collection are also released. The capacity is set to 0 to ensure insertion order is preserved.
        /// <para/>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void Clear()
        {
            int previousCount = hashSet.Count;
            hashSet.Clear();
            requiresTrimExcess |= previousCount > hashSet.Count;
        }

        /// <summary>
        /// Determines whether a <see cref="LinkedHashSet{T}"/> object contains the specified element.
        /// </summary>
        /// <param name="item">The element to locate in the <see cref="LinkedHashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="LinkedHashSet{T}"/> object contains the specified element;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method is an O(1) operation.
        /// </remarks>
        public bool Contains(T item)
            => hashSet.Contains(item);

        /// <summary>
        /// Copies the elements of a <see cref="LinkedHashSet{T}"/> collection to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// the elements copied from the <see cref="LinkedHashSet{T}"/> object.
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
            => hashSet.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes all elements in the specified collection from the current <see cref="LinkedHashSet{T}"/> object.
        /// </summary>
        /// <param name="other">The collection of items to remove from the <see cref="LinkedHashSet{T}"/> object.</param>
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
            if (hashSet.Count == 0)
                return;

            // special case if other is this; a set minus itself is the empty set
            if (other == this)
            {
                Clear();
                return;
            }

            // remove every element in other from this
            foreach (T element in other)
            {
                requiresTrimExcess |= hashSet.Remove(element);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="LinkedHashSet{T}"/> object.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object for the <see cref="LinkedHashSet{T}"/> object.</returns>
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
            => hashSet.GetEnumerator();

        /// <summary>
        /// Modifies the current <see cref="LinkedHashSet{T}"/> object to contain only elements that are
        /// present in that object and in the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// If the collection represented by the other parameter is a <see cref="LinkedHashSet{T}"/> collection with
        /// the same equality comparer as the current <see cref="LinkedHashSet{T}"/> object, this method is an O(<c>n</c>) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c>
        /// is the number of elements in <paramref name="other"/>.
        /// </remarks>
        [System.Security.SecurityCritical]
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            int previousCount = hashSet.Count;

            // intersection of anything with empty set is empty set, so return if count is 0
            if (previousCount == 0)
                return;

            // set intersecting with itself is the same set
            if (other == this)
                return;

            hashSet.IntersectWith(other);
            requiresTrimExcess |= previousCount > hashSet.Count;
        }

        /// <summary>
        /// Determines whether a <see cref="LinkedHashSet{T}"/> object is a proper subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="LinkedHashSet{T}"/> object is a proper subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a proper subset of any other collection. Therefore, this method returns <c>true</c> if the
        /// collection represented by the current <see cref="LinkedHashSet{T}"/> object is empty unless the other
        /// parameter is also an empty set.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is greater than or equal to the number of
        /// elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="LinkedHashSet{T}"/> collection with the same equality
        /// comparer as the current <see cref="LinkedHashSet{T}"/> object, then this method is an O(n) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c> is the
        /// number of elements in other.
        /// </remarks>
        [System.Security.SecurityCritical]
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // no set is a proper subset of itself.
            if (other == this)
                return false;

            return hashSet.IsProperSubsetOf(other);
        }

        /// <summary>
        /// Determines whether a <see cref="LinkedHashSet{T}"/> object is a proper superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="LinkedHashSet{T}"/> object is a proper superset of other; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a proper superset of any other collection. Therefore, this method returns <c>true</c> if the
        /// collection represented by the other parameter is empty unless the current <see cref="LinkedHashSet{T}"/> collection is also empty.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is less than or equal to the number of elements in other.
        /// <para/>
        /// If the collection represented by other is a <see cref="LinkedHashSet{T}"/> collection with the same equality
        /// comparer as the current <see cref="LinkedHashSet{T}"/> object, this method is an O(<c>n</c>) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other and <c>m</c>
        /// is <see cref="Count"/>.
        /// </remarks>
        [System.Security.SecurityCritical]
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // a set is never a strict superset of itself
            if (other == this)
                return false;

            return hashSet.IsProperSupersetOf(other);
        }

        /// <summary>
        /// Determines whether a <see cref="LinkedHashSet{T}"/> object is a subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="LinkedHashSet{T}"/> object is a subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// An empty set is a subset of any other collection, including an empty set; therefore, this method returns
        /// <c>true</c> if the collection represented by the current <see cref="LinkedHashSet{T}"/> object is empty,
        /// even if the <paramref name="other"/> parameter is an empty set.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is greater than the number of
        /// elements in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="LinkedHashSet{T}"/> collection with the same
        /// equality comparer as the current <see cref="LinkedHashSet{T}"/> object, this method is an O(<c>n</c>) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is <see cref="Count"/> and <c>m</c>
        /// is the number of elements in other.
        /// </remarks>
        [System.Security.SecurityCritical]
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // Set is always a subset of itself
            if (other == this)
                return true;

            return hashSet.IsSubsetOf(other);
        }

        /// <summary>
        /// Determines whether a <see cref="LinkedHashSet{T}"/> object is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="LinkedHashSet{T}"/> object is a superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// All collections, including the empty set, are supersets of the empty set. Therefore, this method returns
        /// <c>true</c> if the collection represented by the other parameter is empty, even if the current
        /// <see cref="LinkedHashSet{T}"/> object is empty.
        /// <para/>
        /// This method always returns <c>false</c> if <see cref="Count"/> is less than the number of elements
        /// in <paramref name="other"/>.
        /// <para/>
        /// If the collection represented by other is a <see cref="LinkedHashSet{T}"/> collection with the same
        /// equality comparer as the current <see cref="LinkedHashSet{T}"/> object, this method is an O(<c>n</c>) operation.
        /// Otherwise, this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other
        /// and <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // a set is always a superset of itself
            if (other == this)
                return true;

            return hashSet.IsSupersetOf(other);
        }

        /// <summary>
        /// Determines whether the current <see cref="LinkedHashSet{T}"/> object and a specified collection
        /// share common elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="LinkedHashSet{T}"/> object and <paramref name="other"/> share
        /// at least one common element; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in other.
        /// </remarks>
        public bool Overlaps(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            if (hashSet.Count == 0)
                return false;

            // set overlaps itself
            if (other == this)
                return true;

            return hashSet.Overlaps(other);
        }

        /// <summary>
        /// Removes the specified element from a <see cref="LinkedHashSet{T}"/> object.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns><c>true</c> if the element is successfully found and removed;
        /// otherwise, <c>false</c>. This method returns <c>false</c> if item is not
        /// found in the <see cref="LinkedHashSet{T}"/> object.</returns>
        /// <remarks>
        /// If the <see cref="LinkedHashSet{T}"/> object does not contain the specified
        /// element, the object remains unchanged. No exception is thrown.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public bool Remove(T item)
        {
            bool result = hashSet.Remove(item);
            requiresTrimExcess |= result;
            return result;
        }

        /// <summary>
        /// Determines whether a <see cref="LinkedHashSet{T}"/> object and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <returns><c>true</c> if the <see cref="LinkedHashSet{T}"/> object is equal to <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The <see cref="SetEquals(IEnumerable{T})"/> method ignores duplicate entries and the order of elements in the
        /// <paramref name="other"/> parameter.
        /// <para/>
        /// If the collection represented by other is a <see cref="LinkedHashSet{T}"/> collection with the same equality
        /// comparer as the current <see cref="LinkedHashSet{T}"/> object, this method is an O(<c>n</c>) operation. Otherwise,
        /// this method is an O(<c>n</c> + <c>m</c>) operation, where <c>n</c> is the number of elements in other and
        /// <c>m</c> is <see cref="Count"/>.
        /// </remarks>
        [System.Security.SecurityCritical]
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // a set is equal to itself
            if (other == this)
                return true;

            return hashSet.SetEquals(other);
        }

        /// <summary>
        /// Modifies the current <see cref="LinkedHashSet{T}"/> object to contain only elements that are present either
        /// in that object or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        ///// <remarks>
        ///// If the other parameter is a <see cref="LinkedHashSet{T}"/> collection with the same equality comparer as
        ///// the current <see cref="LinkedHashSet{T}"/> object, this method is an O(<c>n</c>) operation. Otherwise,
        ///// this method is an O(<c>n</c> + <c>m</c>) operation, where n is the number of elements in other and
        ///// <c>m</c> is <see cref="Count"/>.
        ///// </remarks>
        [System.Security.SecurityCritical]
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.other);

            // if set is empty, then symmetric difference is other
            if (hashSet.Count == 0)
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

            // Use the same comparer for fastest operations
            var temp = new SCG.HashSet<T>(other, hashSet.EqualityComparer);
            temp.ExceptWith(this);

            // We separate this into ExceptWith and UnionWith at extra cost to ensure we call TrimExcess()
            // between the two operations (if needed), which in turn ensures we maintain insertion order.
            // IMPORTANT: Don't call the operations on the hashSet private variable!.
            ExceptWith(other);
            UnionWith(temp);
        }

        /// <summary>
        /// Modifies the current <see cref="LinkedHashSet{T}"/> object to contain all elements that are present
        /// in itself, the specified collection, or both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="LinkedHashSet{T}"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        /// <remarks>
        /// This method is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in the
        /// <paramref name="other"/> parameter.
        /// </remarks>
        public void UnionWith(IEnumerable<T> other)
        {
            // Trim excess before we add to ensure insertion order
            if (requiresTrimExcess)
                TrimExcess();
            hashSet.UnionWith(other);
        }

        void ICollection<T>.Add(T item)
            => Add(item); // NOTE: Don't call the op on hashSet, we need to track whether trim is needed.

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

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
        /// using rules similar to those in the JDK's AbstractSet class. Two sets are considered
        /// equal when they both contain the same objects (in any order).
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="ISet{T}"/>
        /// and it contains the same elements; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
            => Equals(obj, SetEqualityComparer<T>.Default);

        /// <summary>
        /// Gets the hash code for the current list. The hash code is calculated
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
