#region Copyright 2019-2025 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
    internal interface INavigableCollection<T> : IDistinctSortedCollection<T>
    {
        /// <summary>
        /// Gets the first (lowest) value in the <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// </summary>
        /// <remarks>
        /// If the <see cref="INavigableCollection{T}"/> has no elements, then the <see cref="First"/> property returns
        /// the default value of <typeparamref name="T"/>.
        /// <para/>
        /// This corresponds to the <c>first()</c> method in the JDK.
        /// </remarks>
        T? First { get; }

        /// <summary>
        /// Gets the last (highest) value in the <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// </summary>
        /// <remarks>
        /// If the <see cref="INavigableCollection{T}"/> has no elements, then the <see cref="Last"/> property returns
        /// the default value of <typeparamref name="T"/>.
        /// <para/>
        /// This corresponds to the <c>last()</c> method in the JDK.
        /// </remarks>
        T? Last { get; }

        /// <summary>
        /// Gets the first (lowest) value in the <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="result">Upon successful return, contains the first (lowest) value.</param>
        /// <returns><see langword="true"/> if a first value exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>first()</c> method in the JDK. Calling <see cref="TryGetFirst(out T)"/> is
        /// generally a better fit than using <see cref="First"/>, since using <see cref="First"/> requires to
        /// check for <see cref="ICollection{T}.Count"/> > 0 on value types to determine whether a first value
        /// exists in the collection.
        /// </remarks>
        bool TryGetFirst([MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Gets the last (highest) value in the <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// </summary>
        /// <param name="result">Upon successful return, contains the last (highest) value.</param>
        /// <returns><see langword="true"/> if a last value exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This corresponds to the <c>last()</c> method in the JDK. Calling <see cref="TryGetLast(out T)"/> is
        /// generally a better fit than using <see cref="Last"/>, since using <see cref="Last"/> requires to
        /// check for <see cref="ICollection{T}.Count"/> > 0 on value types to determine whether a last value
        /// exists in the collection.
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
        /// Returns a view of a subset in a <see cref="INavigableCollection{T}"/>.
        /// </summary>
        /// <param name="lowerValue">The lowest desired value in the view.</param>
        /// <param name="upperValue">The highest desired value in the view.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="lowerValue"/> is more than <paramref name="upperValue"/>
        /// according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="lowerValue"/> and <paramref name="upperValue"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="lowerValue"/> and
        /// <paramref name="upperValue"/> (inclusive), as defined by the comparer. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>subSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewBetween(T? lowerValue, T? upperValue);

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// Usage Note: To match the default behavior of the JDK, call this overload with <paramref name="lowerValueInclusive"/>
        /// set to <see langword="true"/> and <paramref name="upperValueInclusive"/> set to <see langword="false"/>.
        /// </summary>
        /// <param name="lowerValue">The lowest value in the range for the view.</param>
        /// <param name="lowerValueInclusive">If <see langword="true"/>, <paramref name="lowerValue"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <param name="upperValue">The highest desired value in the view.</param>
        /// <param name="upperValueInclusive">If <see langword="true"/>, <paramref name="upperValue"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="lowerValue"/> is more than <paramref name="upperValue"/>
        /// according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="lowerValue"/> and <paramref name="upperValue"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="lowerValue"/> and
        /// <paramref name="upperValue"/>, as defined by the comparer. Each bound may either be inclusive
        /// (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the values of <paramref name="lowerValueInclusive"/>
        /// and <paramref name="upperValueInclusive"/>. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>subSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewBetween(T? lowerValue, bool lowerValueInclusive, T? upperValue, bool upperValueInclusive);

        /// <summary>
        /// Returns the view of a subset in a <see cref="INavigableCollection{T}"/> with no lower bound.
        /// </summary>
        /// <param name="upperValue">The highest desired value in the view.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall before <paramref name="upperValue"/>
        /// (inclusive), as defined by the comparer. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>headSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewBefore(T? upperValue);

        /// <summary>
        /// Returns the view of a subset in a <see cref="INavigableCollection{T}"/> with no lower bound.
        /// <para/>
        /// Usage Note: To match the default behavior of the JDK, call this overload with <paramref name="upperValueInclusive"/>
        /// set to <see langword="false"/>.
        /// </summary>
        /// <param name="upperValue">The highest desired value in the view.</param>
        /// <param name="upperValueInclusive">If <see langword="true"/>, <paramref name="upperValue"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>
        /// This method returns a view of the range of elements that fall before <paramref name="upperValue"/>, as defined by the comparer.
        /// The upper bound may either be inclusive (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the
        /// value of <paramref name="upperValueInclusive"/>. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>headSet()</c> method in the JDK.
        /// </returns>
        INavigableCollection<T> GetViewBefore(T? upperValue, bool upperValueInclusive);

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableCollection{T}"/> with no upper bound.
        /// </summary>
        /// <param name="lowerValue">The lowest value in the range for the view.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="lowerValue"/>
        /// (inclusive), as defined by the comparer. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>tailSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewAfter(T? lowerValue);

        /// <summary>
        /// Returns a view of a subset in a <see cref="INavigableCollection{T}"/> with no upper bound.
        /// </summary>
        /// <param name="lowerValue">The lowest value in the range for the view.</param>
        /// <param name="lowerValueInclusive">If <see langword="true"/>, <paramref name="lowerValue"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <remarks>
        /// This method returns a view of the range of elements that fall after <paramref name="lowerValue"/>, as defined by the comparer.
        /// The lower bound may either be inclusive (<see langword="true"/>) or exclusive (<see langword="false"/>) depending on the
        /// value of <paramref name="lowerValueInclusive"/>. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// <para/>
        /// This corresponds to the <c>tailSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewAfter(T? lowerValue,  bool lowerValueInclusive);

        /// <summary>
        /// Returns a reverse order view of the elements of the current <see cref="INavigableCollection{T}"/>.
        /// </summary>
        /// <returns>A view that contains the values of the current <see cref="INavigableCollection{T}"/> in reverse order.</returns>
        /// <remarks>
        /// This method returns a reverse order view of the range of elements of this <see cref="INavigableCollection{T}"/>, as defined by the comparer.
        /// <para/>
        /// This corresponds to the <c>descendingSet()</c> method in the JDK.
        /// </remarks>
        INavigableCollection<T> GetViewDescending();

        /// <summary>
        /// Gets the entry in the <see cref="INavigableCollection{T}"/> whose value
        /// is the predecessor of the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the predecessor of.</param>
        /// <param name="result">The predessor, if any.</param>
        /// <returns><see langword="true"/> if a predecessor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>strict predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>lower()</c> method in the JDK.
        /// </remarks>
        bool TryGetPredecessor(T item, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Gets the entry in the <see cref="INavigableCollection{T}"/> whose value
        /// is the sucessor of the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the successor of.</param>
        /// <param name="result">The successor, if any.</param>
        /// <returns><see langword="true"/> if a successor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>strict successor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>higher()</c> method in the JDK.
        /// </remarks>
        bool TryGetSuccessor(T item, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Gets the value in the <see cref="INavigableCollection{T}"/> whose value
        /// is the greatest element less than or equal to <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the floor of.</param>
        /// <param name="result">The floor, if any.</param>
        /// <returns><see langword="true"/> if a floor to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <c>weak predecessor</c> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>floor()</c> method in the JDK.
        /// </remarks>
        bool TryGetFloor(T item, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Gets the value in the <see cref="INavigableCollection{T}"/> whose value
        /// is the least element greater than or equal to <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The entry to get the ceiling of.</param>
        /// <param name="result">The ceiling, if any.</param>
        /// <returns><see langword="true"/> if a ceiling to <paramref name="item"/> exists; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This method is a O(log <c>n</c>) operation.
        /// <para/>
        /// This is referred to as <b>weak successor</b> in order theory.
        /// <para/>
        /// Usage Note: This corresponds to the <c>ceiling()</c> method in the JDK.
        /// </remarks>
        bool TryGetCeiling(T item, [MaybeNullWhen(false)] out T result);
    }
}
