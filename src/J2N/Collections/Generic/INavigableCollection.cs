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
        /// Usage Note: To match the behavior of the JDK, call this overload with <paramref name="lowerValueInclusive"/>
        /// set to <c>true</c> and <paramref name="upperValueInclusive"/> set to <c>false</c>.
        /// </summary>
        /// <param name="lowerValue">The lowest value in the range for the view.</param>
        /// <param name="lowerValueInclusive">If <c>true</c>, <paramref name="lowerValue"/> will be included in the range;
        /// otherwise, it is an exclusive lower bound.</param>
        /// <param name="upperValue">The highest desired value in the view.</param>
        /// <param name="upperValueInclusive">If <c>true</c>, <paramref name="upperValue"/> will be included in the range;
        /// otherwise, it is an exclusive upper bound.</param>
        /// <returns>A subset view that contains only the values in the specified range.</returns>
        /// <exception cref="ArgumentException"><paramref name="lowerValue"/> is more than <paramref name="upperValue"/>
        /// according to the comparer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A tried operation on the view was outside the range
        /// specified by <paramref name="lowerValue"/> and <paramref name="upperValue"/>.</exception>
        /// <remarks>
        /// This method returns a view of the range of elements that fall between <paramref name="lowerValue"/> and
        /// <paramref name="upperValue"/>, as defined by the comparer. Each bound may either be inclusive
        /// (<c>true</c>) or exclusive (<c>false</c>) depending on the values of <paramref name="lowerValueInclusive"/>
        /// and <paramref name="upperValueInclusive"/>. This method does not copy elements from the
        /// <see cref="INavigableCollection{T}"/>, but provides a window into the underlying <see cref="INavigableCollection{T}"/> itself.
        /// You can make changes in both the view and in the underlying <see cref="INavigableCollection{T}"/>.
        /// </remarks>
        INavigableCollection<T> GetViewBetween(T? lowerValue, bool lowerValueInclusive, T? upperValue, bool upperValueInclusive);

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
