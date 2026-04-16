// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Dependency of SortedSet, SortedDictionary

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Internal helper functions for working with enumerables.
    /// </summary>
    [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Using Microsoft's code styles")]
    internal static partial class EnumerableHelpers
    {
        /// <summary>Calls Reset on an enumerator instance.</summary>
        /// <remarks>Enables Reset to be called without boxing on a struct enumerator that lacks a public Reset.</remarks>
        internal static void Reset<T>(ref T enumerator) where T : IEnumerator => enumerator.Reset();

        /// <summary>Gets an enumerator singleton for an empty collection.</summary>
        internal static IEnumerator<T> GetEmptyEnumerator<T>() =>
            ((IEnumerable<T>)Arrays.Empty<T>()).GetEnumerator();

        /// <summary>Converts an enumerable to an array using the same logic as <see cref="List{T}"/>.</summary>
        /// <param name="source">The enumerable to convert.</param>
        /// <param name="length">The number of items stored in the resulting array, 0-indexed.</param>
        /// <returns>
        /// The resulting array.  The length of the array may be greater than <paramref name="length"/>,
        /// which is the actual number of elements in the array.
        /// </returns>
        internal static T[] ToArray<T>(IEnumerable<T> source, out int length)
        {
            if (source is ICollection<T> ic)
            {
                int count = ic.Count;
                if (count != 0)
                {
                    // Allocate an array of the desired size, then copy the elements into it. Note that this has the same
                    // issue regarding concurrency as other existing collections like List<T>. If the collection size
                    // concurrently changes between the array allocation and the CopyTo, we could end up either getting an
                    // exception from overrunning the array (if the size went up) or we could end up not filling as many
                    // items as 'count' suggests (if the size went down).  This is only an issue for concurrent collections
                    // that implement ICollection<T>, which as of .NET 4.6 is just ConcurrentDictionary<TKey, TValue>.
                    T[] arr = new T[count];
                    ic.CopyTo(arr, 0);
                    length = count;
                    return arr;
                }
            }
            else
            {
                using (var en = source.GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        const int DefaultCapacity = 4;
                        T[] arr = new T[DefaultCapacity];
                        arr[0] = en.Current;
                        int count = 1;

                        while (en.MoveNext())
                        {
                            if (count == arr.Length)
                            {
                                // This is the same growth logic as in List<T>:
                                // If the array is currently empty, we make it a default size.  Otherwise, we attempt to
                                // double the size of the array.  Doubling will overflow once the size of the array reaches
                                // 2^30, since doubling to 2^31 is 1 larger than Int32.MaxValue.  In that case, we instead
                                // constrain the length to be Arrays.MaxArrayLength (this overflow check works because of the
                                // cast to uint).
                                int newLength = count << 1;
                                if ((uint)newLength > Arrays.MaxArrayLength)
                                {
                                    newLength = Arrays.MaxArrayLength <= count ? count + 1 : Arrays.MaxArrayLength;
                                }

                                Array.Resize(ref arr, newLength);
                            }

                            arr[count++] = en.Current;
                        }

                        length = count;
                        return arr;
                    }
                }
            }

            length = 0;
            return Arrays.Empty<T>();
        }

        /// <summary>
        /// Converts a sorted enumerable to an array, removing any duplicates. The
        /// <paramref name="source"/> data must already be sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The enumerable to convert.</param>
        /// <param name="length">The number of items stored in the resulting array, 0-indexed.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> to use when comparing elements for equality.
        /// If <c>null</c>, the default J2N comparer will be used.</param>
        /// <returns>
        /// The resulting array.  The length of the array may be greater than <paramref name="length"/>,
        /// which is the actual number of elements in the array.
        /// </returns>
        internal static T[] ToDistinctArray<T>(IEnumerable<T> source, out int length, IComparer<T>? comparer)
            => ToDistinctArray(source, out length, ComparerToEqualityComparerAdapter<T>.Create(comparer));

        /// <summary>
        /// Converts a sorted enumerable to an array, removing any duplicates. The
        /// <paramref name="source"/> data must already be sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The enumerable to convert.</param>
        /// <param name="length">The number of items stored in the resulting array, 0-indexed.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use when comparing elements for equality.
        /// If <c>null</c>, the default J2N comparer will be used.</param>
        /// <returns>
        /// The resulting array.  The length of the array may be greater than <paramref name="length"/>,
        /// which is the actual number of elements in the array.
        /// </returns>
        internal static T[] ToDistinctArray<T>(IEnumerable<T> source, out int length, IEqualityComparer<T>? comparer)
        {
            comparer ??= EqualityComparer<T>.Default;
            // Fast path: ICollection<T> gives us max size
            if (source is ICollection<T> ic)
            {
                int count = ic.Count;
                if (count == 0)
                {
                    length = 0;
                    return Arrays.Empty<T>();
                }

                // Allocate an array of the desired size, then copy the elements into it (skipping duplicates). Note that this has the same
                // issue regarding concurrency as other existing collections like List<T>. If the collection size
                // concurrently changes between the array allocation and the CopyTo, we could end up either getting an
                // exception from overrunning the array (if the size went up) or we could end up not filling as many
                // items as 'count' suggests (if the size went down).  This is only an issue for concurrent collections
                // that implement ICollection<T>, which as of .NET 4.6 is just ConcurrentDictionary<TKey, TValue>.
                T[] arr = new T[count];
                int write = 0;
                bool hasPrev = false;
                T prev = default!;

                foreach (T current in source)
                {
                    if (!hasPrev || !comparer.Equals(current, prev))
                    {
                        arr[write++] = current;
                        prev = current;
                        hasPrev = true;
                    }
                }

                length = write;
                return arr;
            }

            // Fallback: enumerator-only (rare here, but consistent)
            using (IEnumerator<T> en = source.GetEnumerator())
            {
                if (!en.MoveNext())
                {
                    length = 0;
                    return Arrays.Empty<T>();
                }

                const int DefaultCapacity = 4;
                T[] arr = new T[DefaultCapacity];

                T prev = en.Current;
                arr[0] = prev;
                int write = 1;

                while (en.MoveNext())
                {
                    T current = en.Current;
                    if (!comparer.Equals(current, prev))
                    {
                        if (write == arr.Length)
                        {
                            // This is the same growth logic as in List<T>:
                            // If the array is currently empty, we make it a default size.  Otherwise, we attempt to
                            // double the size of the array.  Doubling will overflow once the size of the array reaches
                            // 2^30, since doubling to 2^31 is 1 larger than Int32.MaxValue.  In that case, we instead
                            // constrain the length to be Arrays.MaxArrayLength (this overflow check works because of the
                            // cast to uint).
                            int newLength = write << 1;
                            if ((uint)newLength > Arrays.MaxArrayLength)
                            {
                                newLength = Arrays.MaxArrayLength <= write ? write + 1 : Arrays.MaxArrayLength;
                            }

                            Array.Resize(ref arr, newLength);
                        }

                        arr[write++] = current;
                        prev = current;
                    }
                }

                length = write;
                return arr;
            }
        }

        private sealed class ComparerToEqualityComparerAdapter<T> : IEqualityComparer<T>
        {
            private readonly IComparer<T> _comparer;

            private ComparerToEqualityComparerAdapter(IComparer<T>? comparer)
            {
                _comparer = comparer ?? Comparer<T>.Default;
            }

            public static ComparerToEqualityComparerAdapter<T> Create(IComparer<T>? comparer)
            {
                return new ComparerToEqualityComparerAdapter<T>(comparer);
            }

            public bool Equals(T? x, T? y)
            {
                return _comparer.Compare(x!, y!) == 0;
            }

            public int GetHashCode(T? obj)
            {
                throw new NotSupportedException();
            }
        }
    }
}
