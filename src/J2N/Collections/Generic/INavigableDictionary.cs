#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// A contract that indicates a sorted collection of key-value pairs that is distinct,
    /// sorted by a <see cref="IComparer{TKey}"/>, and has navigation capabilities.
    /// </summary>
    /// <typeparam name="TKey">The type of key in the collection.</typeparam>
    /// <typeparam name="TValue">The type of value in the collection.</typeparam>
    public interface INavigableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDistinctSortedCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Gets the <see cref="IComparer{T}"/> used to order the elements of the <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="INavigableDictionary{TKey, TValue}"/> requires a comparer implementation to perform key comparisons.
        /// In typical implementations of <see cref="INavigableDictionary{TKey, TValue}"/>, you can specify an implementation
        /// of the <see cref="IComparer{T}"/> generic interface by using a constructor
        /// that accepts a comparer parameter. If you do not, J2N's default generic equality comparer, <see cref="Comparer{T}.Default"/>,
        /// is used. If type <typeparamref name="TKey"/> implements the <see cref="IComparable{T}"/> generic interface,
        /// the default comparer uses that implementation (except for some types that have been overridden to match Java's
        /// default behavior).
        /// </remarks>
        // J2N: Comparer is discoverable through both IDictionary<TKey, TValue> and ISortedCollection<TKey>, but we want to return
        // the implementation that matches the TKey comparer rather than the KeyValuePair<TKey, TValue> comparer for dictionary compatibility.
        new IComparer<TKey> Comparer { get; }

        /// <summary>
        /// Gets a collection corresponding to the keys in the <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <value>A <see cref="INavigableCollection{TKey}"/> over <typeparamref name="TKey"/> containing the keys in
        /// the <see cref="INavigableDictionary{TKey, TValue}"/>.</value>
        /// <remarks>
        /// The keys in the returned <see cref="INavigableCollection{TKey}"/> are sorted according
        /// to the <see cref="ISortedCollection{TKey}.Comparer"/> property and are in the same order as the associated values in
        /// the <see cref="IDictionary{TKey, TValue}.Values"/> property.
        /// <para/>
        /// The returned <see cref="INavigableCollection{TKey}"/> is not a static copy; instead,
        /// the <see cref="INavigableCollection{TKey}"/> refers back to the keys in the original
        /// <see cref="INavigableDictionary{TKey, TValue}"/>. Therefore, changes to the <see cref="INavigableDictionary{TKey, TValue}"/>
        /// continue to be reflected in the <see cref="INavigableCollection{TKey}"/>.
        /// <para/>
        /// This roughly corresponds to the <c>navigableKeySet()</c> method in the JDK. To get similar functionality
        /// as the <c>descendingKeySet()</c> method in the JDK, call <see cref="Reverse()"/> for a single pass of the values
        /// or <see cref="GetViewDescending()"/> on the returned <see cref="INavigableCollection{TKey}"/> for a persistent reverse view.
        /// </remarks>
        new INavigableCollection<TKey> Keys { get; }

        /// <summary>
        /// Gets the entry in the <see cref="INavigableDictionary{TKey, TValue}"/> whose key
        /// is the first (lowest) value, as defined by the comparer.
        /// </summary>
        /// <param name="key">Upon successful return, contains the first (lowest) key in the collection.</param>
        /// <param name="value">Upon successful return, contains the value corresponding to the first (lowest) key in the collection.</param>
        /// <returns><see langword="true"/> if a first <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Usage Note: This corresponds to both the <c>firstKey()</c> and <c>firstEntry()</c> methods in the JDK.
        /// </remarks>
        bool TryGetFirst([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value);

        /// <summary>
        /// Gets the entry in the <see cref="INavigableDictionary{TKey, TValue}"/> whose key
        /// is the last (highest) value, as defined by the comparer.
        /// </summary>
        /// <param name="key">Upon successful return, contains the last (highest) key in the collection.</param>
        /// <param name="value">Upon successful return, contains the value corresponding to the last (highest) key in the collection.</param>
        /// <returns><see langword="true"/> if a last <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Usage Note: This corresponds to both the <c>lastKey()</c> and <c>lastEntry()</c> methods in the JDK.
        /// </remarks>
        bool TryGetLast([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value);

        /// <summary>
        /// Removes the first (lowest) element in the <see cref="INavigableDictionary{TKey, TValue}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="key">The key of the element before it is removed.</param>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>pollFirstEntry()</c> method in the JDK.
        /// </remarks>
        bool RemoveFirst([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value);

        /// <summary>
        /// Removes the last (highest) element in the <see cref="INavigableDictionary{TKey, TValue}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="key">The key of the element before it is removed.</param>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>pollLastEntry()</c> method in the JDK.
        /// </remarks>
        bool RemoveLast([MaybeNullWhen(false)] out TKey key, [MaybeNullWhen(false)] out TValue value);

        /// <summary>
        /// Gets the entry in the <see cref="INavigableDictionary{TKey, TValue}"/> whose key
        /// is the predecessor of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the predecessor of.</param>
        /// <param name="resultKey">Upon successful return, contains the key of the predecessor.</param>
        /// <param name="resultValue">Upon successful return, contains the value of the predecessor.</param>
        /// <returns><see langword="true"/> if a predecessor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is referred to as <c>strict predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>lowerEntry()</c> method in the JDK.
        /// </remarks>
        bool TryGetPredecessor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue);

        /// <summary>
        /// Gets the entry in the <see cref="INavigableDictionary{TKey, TValue}"/> whose key
        /// is the successor of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the successor of.</param>
        /// <param name="resultKey">Upon successful return, contains the key of the successor.</param>
        /// <param name="resultValue">Upon successful return, contains the value of the successor.</param>
        /// <returns><see langword="true"/> if a successor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is referred to as <c>strict successor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>higherEntry()</c> method in the JDK.
        /// </remarks>
        bool TryGetSuccessor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue);

        /// <summary>
        /// Gets the entry in the <see cref="INavigableDictionary{TKey, TValue}"/> whose key
        /// is the greatest element less than or equal to the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the floor of.</param>
        /// <param name="resultKey">Upon successful return, contains the key of the floor.</param>
        /// <param name="resultValue">Upon successful return, contains the value of the floor.</param>
        /// <returns><see langword="true"/> if a floor to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is referred to as <c>weak predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>floorEntry()</c> method in the JDK.
        /// </remarks>
        bool TryGetFloor([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue);

        /// <summary>
        /// Gets the entry in the <see cref="INavigableDictionary{TKey, TValue}"/> whose key
        /// is the least element greater than or equal to the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to get the ceiling of.</param>
        /// <param name="resultKey">Upon successful return, contains the key of the ceiling.</param>
        /// <param name="resultValue">Upon successful return, contains the value of the ceiling.</param>
        /// <returns><see langword="true"/> if a ceiling to <paramref name="key"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is referred to as <b>weak successor</b> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>ceilingEntry()</c> method in the JDK.
        /// </remarks>
        bool TryGetCeiling([AllowNull] TKey key, [MaybeNullWhen(false)] out TKey resultKey, [MaybeNullWhen(false)] out TValue resultValue);

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> that iterates over the
        /// <see cref="INavigableDictionary{TKey, TValue}"/> in reverse order.
        /// </summary>
        /// <returns>An enumerable that iterates over the <see cref="INavigableDictionary{TKey, TValue}"/> in reverse order.</returns>
        /// <remarks>
        /// This corresponds roughly to the <c>descendingKeySet()</c> method in the JDK.
        /// </remarks>
        IEnumerable<KeyValuePair<TKey, TValue>> Reverse();

        /// <summary>
        /// Returns a view of a sub dictionary in a <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// Usage Note: In Java, the upper bound of TreeMap.subMap() is exclusive. To match the behavior, call
        /// <see cref="GetView(TKey, bool, TKey, bool)"/>,
        /// setting <c>fromInclusive</c> to <see langword="true"/> and <c>toInclusive</c> to <see langword="false"/>.
        /// </summary>
        /// <param name="fromKey">The first desired key in the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
        /// <returns>A sub dictionary view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="fromKey"/> is after <paramref name="toKey"/>
        /// in the current view order according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="fromKey"/> and <paramref name="toKey"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="fromKey"/> and
        /// <paramref name="toKey"/> (inclusive), as defined by the current view order and the comparer.
        /// This method does not copy elements from the <see cref="INavigableDictionary{TKey, TValue}"/>, but provides a
        /// window into the underlying <see cref="INavigableDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>subMap()</c> method in the JDK.
        /// </remarks>
        INavigableDictionary<TKey, TValue> GetView([AllowNull] TKey fromKey, [AllowNull] TKey toKey);

        /// <summary>
        /// Returns a view of a sub dictionary in a <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// Usage Note: To match the behavior of the JDK, call this overload with <paramref name="fromInclusive"/>
        /// set to <see langword="true"/> and <paramref name="toInclusive"/> set to <see langword="false"/>.
        /// </summary>
        /// <param name="fromKey">The first desired key in the range for the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="fromInclusive">If <see langword="true"/>, <paramref name="fromKey"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
        /// <param name="toInclusive">If <see langword="true"/>, <paramref name="toKey"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>A sub dictionary view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="fromKey"/> is after <paramref name="toKey"/>
        /// in the current view order according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="fromKey"/> and <paramref name="toKey"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="fromKey"/> and
        /// <paramref name="toKey"/>, as defined by the current view order and comparer. Each bound may either be inclusive
        /// (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the values of <paramref name="fromInclusive"/>
        /// and <paramref name="toInclusive"/>. This method does not copy elements from the
        /// <see cref="INavigableDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="INavigableDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>subMap()</c> method in the JDK.
        /// </remarks>
        INavigableDictionary<TKey, TValue> GetView([AllowNull] TKey fromKey, bool fromInclusive, [AllowNull] TKey toKey, bool toInclusive);

        /// <summary>
        /// Returns the view of a subset in a <see cref="INavigableDictionary{TKey, TValue}"/> with no lower bound.
        /// </summary>
        /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall before <paramref name="toKey"/>
        /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
        /// <see cref="INavigableDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="INavigableDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>headMap()</c> method in the JDK.
        /// </remarks>
        INavigableDictionary<TKey, TValue> GetViewBefore([AllowNull] TKey toKey);

        /// <summary>
        /// Returns the view of a subset in a <see cref="INavigableDictionary{TKey, TValue}"/> with no lower bound.
        /// <para/>
        /// Usage Note: To match the default behavior of the JDK, call this overload with <paramref name="inclusive"/>
        /// set to <see langword="false"/>.
        /// </summary>
        /// <param name="toKey">The last desired key in the view (highest in ascending order, lowest in descending order).</param>
        /// <param name="inclusive">If <see langword="true"/>, <paramref name="toKey"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>
        /// This method returns a view of the range of elements that fall before <paramref name="toKey"/>, as defined
        /// by the current view order and comparer. The upper bound may either be inclusive (<see langword="true"/>) or
        /// exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>. 
        /// This method does not copy elements from the <see cref="INavigableDictionary{TKey, TValue}"/>, but provides
        /// a window into the underlying <see cref="INavigableDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>headMap()</c> method in the JDK.
        /// </returns>
        INavigableDictionary<TKey, TValue> GetViewBefore([AllowNull] TKey toKey, bool inclusive);

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableDictionary{TKey, TValue}"/> with no upper bound.
        /// </summary>
        /// <param name="fromKey">The first desired key in the range for the view (lowest in ascending order, highest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="fromKey"/>
        /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
        /// <see cref="INavigableDictionary{TKey, TValue}"/>, but provides a window into the underlying <see cref="INavigableDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>tailMap()</c> method in the JDK.
        /// </remarks>
        INavigableDictionary<TKey, TValue> GetViewAfter([AllowNull] TKey fromKey);

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableDictionary{TKey, TValue}"/> with no upper bound.
        /// </summary>
        /// <param name="fromKey">The first desired key in the range for the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="inclusive">If <see langword="true"/>, <paramref name="fromKey"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="fromKey"/>, 
        /// as defined by the current view order and comparer. The lower bound may either be inclusive (<see langword="true"/>)
        /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>.
        /// This method does not copy elements from the <see cref="INavigableDictionary{TKey, TValue}"/>, but
        /// provides a window into the underlying <see cref="INavigableDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>tailMap()</c> method in the JDK.
        /// </remarks>
        INavigableDictionary<TKey, TValue> GetViewAfter([AllowNull] TKey fromKey, bool inclusive);

        /// <summary>
        /// Returns a reverse order view of the elements of the current <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A view that contains the values of the current <see cref="INavigableDictionary{TKey, TValue}"/> in reverse order.</returns>
        /// <remarks>
        /// This method returns a reverse order view of the range of elements of this <see cref="INavigableDictionary{TKey, TValue}"/>,
        /// as defined by the comparer. This method does not copy elements from the <see cref="INavigableDictionary{TKey, TValue}"/>, but provides a
        /// window into the underlying <see cref="INavigableDictionary{TKey, TValue}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableDictionary{TKey, TValue}"/>.
        /// <para/>
        /// This corresponds to the <c>descendingMap()</c> method in the JDK.
        /// </remarks>
        INavigableDictionary<TKey, TValue> GetViewDescending();
    }
}
