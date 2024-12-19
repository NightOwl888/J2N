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
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace J2N.Collections.Generic.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="ICollection{T}"/> interface.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns a read-only <see cref="ICollection{T}"/> wrapper for the current collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection to make read-only.</param>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="ICollection{T}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// To prevent any modifications to the <see cref="ICollection{T}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlyCollection{T}"/> object does not expose methods that modify the collection. However, if
        /// changes are made to the underlying <see cref="ICollection{T}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<T> AsReadOnly<T>(this ICollection<T> collection)
        {
            return new ReadOnlyCollection<T>(collection);
        }

        /// <summary>
        /// Creates an array from a <see cref="ICollection{T}"/>.
        /// <para/>
        /// This is similar to the LINQ <see cref="IEnumerable{T}"/> extension method,
        /// but since it pre-allocates the exact number of array elements and always
        /// uses <see cref="ICollection{T}.CopyTo(T[], int)"/>, it is usually much faster.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="ICollection{T}"/> to create an array from.</param>
        /// <returns>An array that contains the elements from the input sequence.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToArray<T>(this ICollection<T> source)
        {
            ThrowHelper.ThrowIfNull(source, ExceptionArgument.source);

            int count = source.Count;
            if (count == 0)
                return Arrays.Empty<T>();

            var result = new T[count];
            source.CopyTo(result, 0);
            return result;
        }
    }
}
