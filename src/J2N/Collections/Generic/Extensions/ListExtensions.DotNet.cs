// Partially sourced from: https://github.com/dotnet/runtime/blob/1d6259029e6f0c729c359a95746018f78186ebae/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/CollectionExtensions.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace J2N.Collections.Generic.Extensions
{
    // J2N NOTE: This file contains the .NET runtime extensions for List, which originally were in CollectionExtensions.
    public static partial class ListExtensions
    {
        /// <summary>Adds the elements of the specified span to the end of the <see cref="List{T}"/>.</summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to which the elements should be added.</param>
        /// <param name="source">The span whose elements should be added to the end of the <see cref="List{T}"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is null.</exception>
        internal static void AddRange<T>(this List<T> list, ReadOnlySpan<T> source)
        {
            if (list is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.list);
            }

            // J2N: delegate to virtual method to support SubLists
            list.DoAddRange(source);
        }

        /// <summary>Inserts the elements of a span into the <see cref="List{T}"/> at the specified index.</summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list into which the elements should be inserted.</param>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="source">The span whose elements should be added to the <see cref="List{T}"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than <paramref name="list"/>'s <see cref="List{T}.Count"/>.</exception>
        internal static void InsertRange<T>(this List<T> list, int index, ReadOnlySpan<T> source)
        {
            if (list is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.list);
            }

            // J2N: delegate to virtual method to support SubLists
            list.DoInsertRange(index, source);
        }

        /// <summary>Copies the entire <see cref="List{T}"/> to a span.</summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list from which the elements are copied.</param>
        /// <param name="destination">The span that is the destination of the elements copied from <paramref name="list"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is null.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="List{T}"/> is greater than the number of elements that the destination span can contain.</exception>
        internal static void CopyTo<T>(this List<T> list, Span<T> destination)
        {
            if (list is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.list);
            }

            // J2N: delegate to DoCopyTo which uses virtual properties to support SubLists
            list.DoCopyTo(destination);
        }
    }
}
