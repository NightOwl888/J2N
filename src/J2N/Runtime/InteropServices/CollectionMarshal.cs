// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Runtime.InteropServices
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// An unsafe class that provides a set of methods to access the underlying data representations of collections.
    /// </summary>
    public static class CollectionMarshal
    {
        /// <summary>
        /// Get a <see cref="Span{T}"/> view over a <see cref="List{T}"/>'s data.
        /// Items should not be added or removed from the <see cref="List{T}"/> while the <see cref="Span{T}"/> is in use.
        /// </summary>
        /// <param name="list">The list to get the data view over.</param>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Span<T> AsSpan<T>(List<T>? list)
        {
            Span<T> span = default;
            if (list is not null)
            {
                int size = list._size;
                T[] items = list._items;
                Debug.Assert(items is not null, "Implementation depends on List<T> always having an array.");

                if ((uint)size > (uint)items!.Length)
                {
                    // List<T> was erroneously mutated concurrently with this call, leading to a count larger than its array.
                    throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
                }

                Debug.Assert(typeof(T[]) == list._items.GetType(), "Implementation depends on List<T> always using a T[] and not U[] where U : T.");
#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
                // Per the comments, this is the equivalent of the Span<T>(ref T reference, int length) constructor.
                span = MemoryMarshal.CreateSpan<T>(ref MemoryMarshal.GetArrayDataReference(items), size);
#else
                span = items.AsSpan(0, size);
#endif
            }

            return span;
        }

        /// <summary>
        /// Sets the count of the <see cref="List{T}"/> to the specified value.
        /// </summary>
        /// <param name="list">The list to set the count of.</param>
        /// <param name="count">The value to set the list's count to.</param>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> is negative.
        /// </exception>
        /// <remarks>
        /// When increasing the count, uninitialized data is being exposed.
        /// </remarks>
        public static void SetCount<T>(List<T> list, int count)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }

            list._version++;

            if (count > list.Capacity)
            {
                list.Grow(count);
            }
#if FEATURE_RUNTIMEHELPERS_ISREFERENCETYPEORCONTAINSREFERENCES
            else if (count < list._size && RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#else
            else if (count < list._size && List<T>.TIsNullableType)
#endif
            {
                Array.Clear(list._items, count, list._size - count);
            }

            list._size = count;
        }
    }
}
