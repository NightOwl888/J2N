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
    /// A contract that indicates a sorted collection of unique elements that is distinct,
    /// sorted by a <see cref="IComparer{T}"/>, and has navigation capabilities.
    /// </summary>
    /// <typeparam name="T">The type of element in the collection.</typeparam>
    public interface INavigableCollection<T> : IDistinctSortedCollection<T>
    {
        /// <summary>
        /// Gets the first (lowest) value in the <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="result">Upon successful return, contains the first (lowest) value.</param>
        /// <returns><see langword="true"/> if a first value exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>first()</c> method in the JDK.
        /// </remarks>
        bool TryGetFirst([MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Gets the last (highest) value in the <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="result">Upon successful return, contains the last (highest) value.</param>
        /// <returns><see langword="true"/> if a last value exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>last()</c> method in the JDK.
        /// </remarks>
        bool TryGetLast([MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Removes the first (lowest) value in the <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><see langword="true"/>  if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>pollFirst()</c> method in the JDK.
        /// </remarks>
        bool RemoveFirst([MaybeNullWhen(false)] out T value); // J2N: The parameter naming of "value" instead of "result" is intentional here because this is a mutation, not a query

        /// <summary>
        /// Removes the last (highest) value in the <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><see langword="true"/>  if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>pollLast()</c> method in the JDK.
        /// </remarks>
        bool RemoveLast([MaybeNullWhen(false)] out T value); // J2N: The parameter naming of "value" instead of "result" is intentional here because this is a mutation, not a query

        /// <summary>
        /// Gets the entry in the <see cref="INavigableCollection{T}"/> whose value
        /// is the predecessor of the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the predecessor of.</param>
        /// <param name="result">The predecessor, if any.</param>
        /// <returns><see langword="true"/> if a predecessor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is referred to as <c>strict predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>lower()</c> method in the JDK.
        /// </remarks>
        bool TryGetPredecessor([AllowNull] T item, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Gets the entry in the <see cref="INavigableCollection{T}"/> whose value
        /// is the successor of the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the successor of.</param>
        /// <param name="result">The successor, if any.</param>
        /// <returns><see langword="true"/> if a successor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is referred to as <c>strict successor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>higher()</c> method in the JDK.
        /// </remarks>
        bool TryGetSuccessor([AllowNull] T item, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Gets the value in the <see cref="INavigableCollection{T}"/> whose value
        /// is the greatest element less than or equal to <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the floor of.</param>
        /// <param name="result">The floor, if any.</param>
        /// <returns><see langword="true"/> if a floor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is referred to as <c>weak predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>floor()</c> method in the JDK.
        /// </remarks>
        bool TryGetFloor([AllowNull] T item, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Gets the value in the <see cref="INavigableCollection{T}"/> whose value
        /// is the least element greater than or equal to <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the ceiling of.</param>
        /// <param name="result">The ceiling, if any.</param>
        /// <returns><see langword="true"/> if a ceiling to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is referred to as <b>weak successor</b> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>ceiling()</c> method in the JDK.
        /// </remarks>
        bool TryGetCeiling([AllowNull] T item, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> that iterates over the
        /// <see cref="INavigableCollection{T}"/> in reverse order.
        /// </summary>
        /// <returns>An enumerable that iterates over the <see cref="INavigableCollection{T}"/> in reverse order.</returns>
        /// <remarks>
        /// This corresponds roughly to the <c>descendingIterator()</c> method in the JDK.
        /// </remarks>
        IEnumerable<T> Reverse();

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// Usage Note: In Java, the <paramref name="toItem"/> of TreeSet.subSet() is exclusive. To match the behavior, call
        /// <see cref="GetView(T, bool, T, bool)"/>, setting <c>fromInclusive</c> to <see langword="true"/>
        /// and <c>toInclusive</c> to <see langword="false"/>.
        /// </summary>
        /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="fromItem"/> is after <paramref name="toItem"/>
        /// in the current view order according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="fromItem"/> and <paramref name="toItem"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="fromItem"/> and
        /// <paramref name="toItem"/> (inclusive), as defined by the current view order and the comparer.
        /// This method does not copy elements from the <see cref="INavigableCollection{T}"/>, but provides a window
        /// into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>subSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetView([AllowNull] T fromItem, [AllowNull] T toItem);

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// Usage Note: To match the behavior of the JDK, call this overload with <paramref name="fromInclusive"/>
        /// set to <see langword="true"/> and <paramref name="toInclusive"/> set to <see langword="false"/>.
        /// </summary>
        /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="fromInclusive">If <see langword="true"/>, <paramref name="fromItem"/> will be included in the range;
        /// otherwise, it is an exclusive bound.</param>
        /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
        /// <param name="toInclusive">If <see langword="true"/>, <paramref name="toItem"/> will be included in the range;
        /// otherwise, it is an exclusive bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="fromItem"/> is after <paramref name="toItem"/>
        /// in the current view order according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="fromItem"/> and <paramref name="toItem"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="fromItem"/> and
        /// <paramref name="toItem"/>, as defined by the current view order and the comparer. Each bound may either be inclusive
        /// (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the values of <paramref name="fromInclusive"/>
        /// and <paramref name="toInclusive"/>. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>subSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetView([AllowNull] T fromItem, bool fromInclusive, [AllowNull] T toItem, bool toInclusive);

        /// <summary>
        /// Returns the view of a subset in a <see cref="INavigableCollection{T}"/> with no lower bound.
        /// <para/>
        /// Usage Note: To match the default behavior of the JDK, call the <see cref="GetViewBefore(T, bool)"/>
        /// overload with <c>inclusive</c> set to <see langword="false"/>.
        /// </summary>
        /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall before <paramref name="toItem"/>
        /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>headSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewBefore([AllowNull] T toItem);

        /// <summary>
        /// Returns the view of a subset in a <see cref="INavigableCollection{T}"/> with no lower bound.
        /// <para/>
        /// Usage Note: To match the default behavior of the JDK, call this overload with <paramref name="inclusive"/>
        /// set to <see langword="false"/>.
        /// </summary>
        /// <param name="toItem">The last desired value in the view (highest in ascending order, lowest in descending order).</param>
        /// <param name="inclusive">If <see langword="true"/>, <paramref name="toItem"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>
        /// This method returns a view of the range of elements that fall before <paramref name="toItem"/>, as defined by
        /// the current view order and comparer. The upper bound may either be inclusive (<see langword="true"/>)
        /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>.
        /// This method does not copy elements from the <see cref="INavigableCollection{T}"/>, but provides a window
        /// into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>headSet()</c> method in the JDK.
        /// </returns>
        INavigableCollection<T> GetViewBefore([AllowNull] T toItem, bool inclusive);

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableCollection{T}"/> with no upper bound.
        /// </summary>
        /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="fromItem"/>
        /// (inclusive), as defined by the current view order and comparer. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>tailSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewAfter([AllowNull] T fromItem);

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableCollection{T}"/> with no upper bound.
        /// </summary>
        /// <param name="fromItem">The first desired value in the view (lowest in ascending order, highest in descending order).</param>
        /// <param name="inclusive">If <see langword="true"/>, <paramref name="fromItem"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="fromItem"/>, as defined
        /// by the current view order and comparer. The lower bound may either be inclusive (<see langword="true"/>)
        /// or exclusive (<see langword="false"/>) depending on the value of <paramref name="inclusive"/>. This method
        /// does not copy elements from the <see cref="INavigableCollection{T}"/>, but provides a window into the
        /// underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>tailSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewAfter([AllowNull] T fromItem, bool inclusive);

        /// <summary>
        /// Returns a reverse order view of the elements of the current <see cref="INavigableCollection{T}"/>.
        /// </summary>
        /// <returns>A view that contains the values of the current <see cref="INavigableCollection{T}"/> in reverse order.</returns>
        /// <remarks>
        /// This method returns a reverse order view of the range of elements of this <see cref="INavigableCollection{T}"/>, as
        /// defined by the comparer. This method does not copy elements from the <see cref="SortedSet{T}"/>, but provides a
        /// window into the underlying <see cref="SortedSet{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="SortedSet{T}"/>.
        /// <para/>
        /// This corresponds to the <c>descendingSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewDescending();
    }
}
