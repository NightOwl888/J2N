// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Numerics;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Collections.Generic
{
    using SR = J2N.Resources.Strings;

    internal partial class ArraySortHelper<T>
    {
        // From Array class

        // This is the threshold where Introspective sort switches to Insertion sort.
        // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
        // Large value types may benefit from a smaller number.
        internal const int IntrosortSizeThreshold = 16;

        // From ArraySortHelper<T> class

        internal static void Sort(Span<T> keys, Comparison<T> comparer)
        {
            Debug.Assert(comparer != null, "Check the arguments in the caller!");

            // Add a try block here to detect bogus comparisons
            try
            {
                IntrospectiveSort(keys, comparer!);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException(J2N.SR.Format(SR.Arg_BogusIComparer, comparer));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(SR.InvalidOperation_IComparerFailed, e);
            }
        }

        private static void SwapIfGreater(Span<T> keys, Comparison<T> comparer, int i, int j)
        {
            Debug.Assert(i != j);

            if (comparer(keys[i], keys[j]) > 0)
            {
                T key = keys[i];
                keys[i] = keys[j];
                keys[j] = key;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(Span<T> a, int i, int j)
        {
            Debug.Assert(i != j);

            T t = a[i];
            a[i] = a[j];
            a[j] = t;
        }

        internal static void IntrospectiveSort(Span<T> keys, Comparison<T> comparer)
        {
            Debug.Assert(comparer != null);

            if (keys.Length > 1)
            {
                IntroSort(keys, 2 * (BitOperation.Log2((uint)keys.Length) + 1), comparer!);
            }
        }

        private static void IntroSort(Span<T> keys, int depthLimit, Comparison<T> comparer)
        {
            Debug.Assert(!keys.IsEmpty);
            Debug.Assert(depthLimit >= 0);
            Debug.Assert(comparer != null);

            int partitionSize = keys.Length;
            while (partitionSize > 1)
            {
                if (partitionSize <= /*Array.*/IntrosortSizeThreshold)
                {

                    if (partitionSize == 2)
                    {
                        SwapIfGreater(keys, comparer!, 0, 1);
                        return;
                    }

                    if (partitionSize == 3)
                    {
                        SwapIfGreater(keys, comparer!, 0, 1);
                        SwapIfGreater(keys, comparer!, 0, 2);
                        SwapIfGreater(keys, comparer!, 1, 2);
                        return;
                    }

                    InsertionSort(keys.Slice(0, partitionSize), comparer!);
                    return;
                }

                if (depthLimit == 0)
                {
                    HeapSort(keys.Slice(0, partitionSize), comparer!);
                    return;
                }
                depthLimit--;

                int p = PickPivotAndPartition(keys.Slice(0, partitionSize), comparer!);

                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                //IntroSort(keys[(p + 1)..partitionSize], depthLimit, comparer!);
                IntroSort(keys.Slice(p + 1, partitionSize - (p + 1)), depthLimit, comparer!); // J2N: Removed index/range so we can compile on .NET Framework
                partitionSize = p;
            }
        }

        private static int PickPivotAndPartition(Span<T> keys, Comparison<T> comparer)
        {
            Debug.Assert(keys.Length >= /*Array.*/IntrosortSizeThreshold);
            Debug.Assert(comparer != null);

            int hi = keys.Length - 1;

            // Compute median-of-three.  But also partition them, since we've done the comparison.
            int middle = hi >> 1;

            // Sort lo, mid and hi appropriately, then pick mid as the pivot.
            SwapIfGreater(keys, comparer!, 0, middle);  // swap the low with the mid point
            SwapIfGreater(keys, comparer!, 0, hi);   // swap the low with the high
            SwapIfGreater(keys, comparer!, middle, hi); // swap the middle with the high

            T pivot = keys[middle];
            Swap(keys, middle, hi - 1);
            int left = 0, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

            while (left < right)
            {
                while (comparer!(keys[++left], pivot) < 0) ;
                while (comparer!(pivot, keys[--right]) < 0) ;

                if (left >= right)
                    break;

                Swap(keys, left, right);
            }

            // Put pivot in the right location.
            if (left != hi - 1)
            {
                Swap(keys, left, hi - 1);
            }
            return left;
        }

        private static void HeapSort(Span<T> keys, Comparison<T> comparer)
        {
            Debug.Assert(comparer != null);
            Debug.Assert(!keys.IsEmpty);

            int n = keys.Length;
            for (int i = n >> 1; i >= 1; i--)
            {
                DownHeap(keys, i, n, 0, comparer!);
            }

            for (int i = n; i > 1; i--)
            {
                Swap(keys, 0, i - 1);
                DownHeap(keys, 1, i - 1, 0, comparer!);
            }
        }

        private static void DownHeap(Span<T> keys, int i, int n, int lo, Comparison<T> comparer)
        {
            Debug.Assert(comparer != null);
            Debug.Assert(lo >= 0);
            Debug.Assert(lo < keys.Length);

            T d = keys[lo + i - 1];
            while (i <= n >> 1)
            {
                int child = 2 * i;
                if (child < n && comparer!(keys[lo + child - 1], keys[lo + child]) < 0)
                {
                    child++;
                }

                if (!(comparer!(d, keys[lo + child - 1]) < 0))
                    break;

                keys[lo + i - 1] = keys[lo + child - 1];
                i = child;
            }

            keys[lo + i - 1] = d;
        }

        private static void InsertionSort(Span<T> keys, Comparison<T> comparer)
        {
            for (int i = 0; i < keys.Length - 1; i++)
            {
                T t = keys[i + 1];

                int j = i;
                while (j >= 0 && comparer(t, keys[j]) < 0)
                {
                    keys[j + 1] = keys[j];
                    j--;
                }

                keys[j + 1] = t;
            }
        }

        internal unsafe static void Sort(T[]? array, int startIndex, int length, Comparison<T> comparer)
        {
            Debug.Assert(array != null, "Check the arguments in the caller!");
            Debug.Assert(comparer != null, "Check the arguments in the caller!");

            // Add a try block here to detect bogus comparisons
            try
            {
                IntrospectiveSort(array, startIndex, length, comparer!);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException(J2N.SR.Format(SR.Arg_BogusIComparer, comparer!));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(SR.InvalidOperation_IComparerFailed, e);
            }
        }

        private static void SwapIfGreater(T[] keys, Comparison<T> comparer, int i, int j)
        {
            Debug.Assert(i != j);

            if (comparer(keys[i], keys[j]) > 0)
            {
                T key = keys[i];
                keys[i] = keys[j];
                keys[j] = key;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(T[] a, int i, int j)
        {
            Debug.Assert(i != j);

            T t = a[i];
            a[i] = a[j];
            a[j] = t;
        }

        internal static void IntrospectiveSort(T[]? array, int startIndex, int length, Comparison<T> comparer)
        {
            Debug.Assert(comparer != null);

            if (length > 1)
            {
                IntroSort(array, startIndex, length, 2 * (BitOperation.Log2((uint)length) + 1), comparer!);
            }
        }

        private static void IntroSort(T[]? keys, int startIndex, int length, int depthLimit, Comparison<T> comparer)
        {
            Debug.Assert(keys != null);
            Debug.Assert(depthLimit >= 0);
            Debug.Assert(comparer != null);

            int partitionSize = length;
            while (partitionSize > 1)
            {
                if (partitionSize <= /*Array.*/IntrosortSizeThreshold)
                {

                    if (partitionSize == 2)
                    {
                        SwapIfGreater(keys!, comparer!, 0, 1);
                        return;
                    }

                    if (partitionSize == 3)
                    {
                        SwapIfGreater(keys!, comparer!, 0, 1);
                        SwapIfGreater(keys!, comparer!, 0, 2);
                        SwapIfGreater(keys!, comparer!, 1, 2);
                        return;
                    }

                    InsertionSort(keys!, startIndex, partitionSize, comparer!);
                    return;
                }

                if (depthLimit == 0)
                {
                    HeapSort(keys!, startIndex, partitionSize, comparer!);
                    return;
                }
                depthLimit--;

                int p = PickPivotAndPartition(keys!, startIndex, partitionSize, comparer!);

                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSort(keys!, startIndex + p + 1, partitionSize - (p + 1), depthLimit, comparer!);
                partitionSize = p;
            }
        }

        private static int PickPivotAndPartition(T[] keys, int startIndex, int length, Comparison<T> comparer)
        {
            Debug.Assert(keys.Length >= /*Array.*/IntrosortSizeThreshold);
            Debug.Assert(comparer != null);

            int hi = length - 1;

            // Compute median-of-three.  But also partition them, since we've done the comparison.
            int middle = hi >> 1;

            // Add our offset into the array
            hi += startIndex;
            middle += startIndex;

            // Sort lo, mid and hi appropriately, then pick mid as the pivot.
            SwapIfGreater(keys, comparer!, startIndex, middle);  // swap the low with the mid point
            SwapIfGreater(keys, comparer!, startIndex, hi);   // swap the low with the high
            SwapIfGreater(keys, comparer!, middle, hi); // swap the middle with the high

            T pivot = keys[middle];
            Swap(keys, middle, hi - 1);
            int left = startIndex, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

            while (left < right)
            {
                while (comparer!(keys[++left], pivot) < 0) ;
                while (comparer!(pivot, keys[--right]) < 0) ;

                if (left >= right)
                    break;

                Swap(keys, left, right);
            }

            // Put pivot in the right location.
            if (left != hi - 1)
            {
                Swap(keys, left, hi - 1);
            }
            return left - startIndex;
        }

        private static void HeapSort(T[] keys, int startIndex, int length, Comparison<T> comparer)
        {
            Debug.Assert(comparer != null);
            Debug.Assert(keys != null);

            int n = length;
            for (int i = n >> 1; i >= 1; i--)
            {
                DownHeap(keys!, i, n, startIndex, comparer!);
            }

            for (int i = n; i > 1; i--)
            {
                Swap(keys!, startIndex, startIndex + i - 1);
                DownHeap(keys!, 1, i - 1, startIndex, comparer!);
            }
        }

        private static void DownHeap(T[] keys, int i, int n, int lo, Comparison<T> comparer)
        {
            Debug.Assert(comparer != null);
            Debug.Assert(lo >= 0);
            Debug.Assert(lo < keys.Length);

            T d = keys[lo + i - 1];
            while (i <= n >> 1)
            {
                int child = 2 * i;
                if (child < n && comparer!(keys[lo + child - 1], keys[lo + child]) < 0)
                {
                    child++;
                }

                if (!(comparer!(d, keys[lo + child - 1]) < 0))
                    break;

                keys[lo + i - 1] = keys[lo + child - 1];
                i = child;
            }

            keys[lo + i - 1] = d;
        }

        private static void InsertionSort(T[] keys, int startIndex, int length, Comparison<T> comparer)
        {
            int limit = startIndex + length - 1; // end of the range
            for (int i = startIndex; i < limit; i++)
            {
                T t = keys[i + 1];

                int j = i;
                while (j >= 0 && comparer(t, keys[j]) < 0)
                {
                    keys[j + 1] = keys[j];
                    j--;
                }

                keys[j + 1] = t;
            }
        }
    }
}
