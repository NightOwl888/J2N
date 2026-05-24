// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace J2N
{
    internal static class BufferHelper
    {
        /// <summary>
        /// Copies a number of bytes specified as a long integer value from one address in memory to another.
        /// <para/>
        /// This API is not CLS-compliant.
        /// </summary>
        /// <param name="source">The address of the bytes to copy.</param>
        /// <param name="destination">The target address.</param>
        /// <param name="destinationSizeInBytes">The number of bytes available in the destination memory block.</param>
        /// <param name="sourceBytesToCopy">The number of bytes to copy.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sourceBytesToCopy"/> is greater than
        /// <paramref name="destinationSizeInBytes"/>.</exception>
        /// <remarks>This method copies <paramref name="sourceBytesToCopy"/> bytes from the address specified by
        /// <paramref name="source"/> to the address specified by <paramref name="destination"/>. If some regions
        /// of the source area and the destination overlap, the function ensures that the original source bytes
        /// in the overlapping region are copied before being overwritten.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemoryCopy(void* source, void* destination, long destinationSizeInBytes, long sourceBytesToCopy)
#if FEATURE_BUFFER_MEMORYCOPY
            => Buffer.MemoryCopy(source, destination, destinationSizeInBytes, sourceBytesToCopy);
#else
            => MemoryCopy(ref *(byte*)source, ref *(byte*)destination, (ulong)destinationSizeInBytes, (ulong)sourceBytesToCopy);
#endif

        /// <summary>
        /// Copies a number of bytes specified as an unsigned long integer value from one address in memory to another.
        /// <para/>
        /// This API is not CLS-compliant.
        /// </summary>
        /// <param name="source">The address of the bytes to copy.</param>
        /// <param name="destination">The target address.</param>
        /// <param name="destinationSizeInBytes">The number of bytes available in the destination memory block.</param>
        /// <param name="sourceBytesToCopy">The number of bytes to copy.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sourceBytesToCopy"/> is greater than
        /// <paramref name="destinationSizeInBytes"/>.</exception>
        /// <remarks>This method copies <paramref name="sourceBytesToCopy"/> bytes from the address specified by
        /// <paramref name="source"/> to the address specified by <paramref name="destination"/>. If some regions
        /// of the source area and the destination overlap, the function ensures that the original source bytes
        /// in the overlapping region are copied before being overwritten.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemoryCopy(void* source, void* destination, ulong destinationSizeInBytes, ulong sourceBytesToCopy)
#if FEATURE_BUFFER_MEMORYCOPY
            => Buffer.MemoryCopy(source, destination, destinationSizeInBytes, sourceBytesToCopy);
#else
            => MemoryCopy(ref *(byte*)source, ref *(byte*)destination, destinationSizeInBytes, sourceBytesToCopy);
#endif

        private static void MemoryCopy(ref byte src, ref byte dest, ulong destinationSizeInBytes, ulong sourceBytesToCopy)
        {
            // Edge case: Nothing to copy or identical pointers
            if (sourceBytesToCopy == 0 || Unsafe.AreSame(ref src, ref dest))
                return;

            // Validate bounds
            if (sourceBytesToCopy > destinationSizeInBytes)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sourceBytesToCopy);

            // Handle non-overlapping copy (fast path)
            if (Unsafe.IsAddressLessThan(ref dest, ref src) || !Unsafe.IsAddressLessThan(ref dest, ref Unsafe.Add(ref src, (nint)sourceBytesToCopy)))
            {
                // Forward copy (non-overlapping region)
                if (IntPtr.Size == 8) // 64-bit optimization
                {
                    while (sourceBytesToCopy >= 8)
                    {
                        Unsafe.WriteUnaligned(ref dest, Unsafe.ReadUnaligned<ulong>(ref src));
                        src = ref Unsafe.Add(ref src, 8);
                        dest = ref Unsafe.Add(ref dest, 8);
                        sourceBytesToCopy -= 8;
                    }
                }
                else // 32-bit optimization
                {
                    while (sourceBytesToCopy >= 4)
                    {
                        Unsafe.WriteUnaligned(ref dest, Unsafe.ReadUnaligned<uint>(ref src));
                        src = ref Unsafe.Add(ref src, 4);
                        dest = ref Unsafe.Add(ref dest, 4);
                        sourceBytesToCopy -= 4;
                    }
                }

                // Copy remaining bytes
                while (sourceBytesToCopy > 0)
                {
                    dest = src;
                    src = ref Unsafe.Add(ref src, 1);
                    dest = ref Unsafe.Add(ref dest, 1);
                    sourceBytesToCopy--;
                }
                return;
            }

            // Slow path for overlapping regions
            MemoryCopySlow(ref src, ref dest, sourceBytesToCopy);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static void MemoryCopySlow(ref byte src, ref byte dest, ulong sourceBytesToCopy)
            {
                if (!Unsafe.IsAddressLessThan(ref dest, ref src)) // Backward copy (dest < src)
                {
                    // Backward copy (overlapping and src < dest)
                    src = ref Unsafe.Add(ref src, (nint)sourceBytesToCopy - 1);
                    dest = ref Unsafe.Add(ref dest, (nint)sourceBytesToCopy - 1);

                    while (sourceBytesToCopy > 0)
                    {
                        dest = src;
                        src = ref Unsafe.Subtract(ref src, 1);
                        dest = ref Unsafe.Subtract(ref dest, 1);
                        sourceBytesToCopy--;
                    }
                }
                else // Forward copy (src < dest)
                {
                    while (sourceBytesToCopy > 0)
                    {
                        dest = src;
                        src = ref Unsafe.Add(ref src, 1);
                        dest = ref Unsafe.Add(ref dest, 1);
                        sourceBytesToCopy--;
                    }
                }
            }
        }

        internal static unsafe void Memmove<T>(ref T destination, ref T source, nuint elementCount) where T : unmanaged
        {
            if (elementCount == 0 || Unsafe.AreSame(ref destination, ref source))
            {
                return;
            }

            fixed (T* pDest = &destination)
            fixed (T* pSource = &source)
            {
                ulong byteCount = (ulong)(elementCount * (nuint)sizeof(T));
                MemoryCopy(pSource, pDest, byteCount, byteCount);
            }
        }

        internal static unsafe void Memcpy<T>(ref T destination, ref T source, int length) where T : unmanaged
        {
            Debug.Assert(length > 0, "Caller should validate length to ensure it is always greater than 0.");

            fixed (void* pDest = &destination, pSource = &source)
            {
#if FEATURE_BUFFER_MEMORYCOPY
                Buffer.MemoryCopy(pSource, pDest, length * sizeof(T), length * sizeof(T));
#else
                // Convert refs to arrays
                Span<T> destSpan = new Span<T>(pDest, length);
                Span<T> sourceSpan = new Span<T>(pSource, length);

                // Copy memory safely
                sourceSpan.CopyTo(destSpan);
#endif
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static unsafe void ZeroMemory(void* destination, ulong length)
        {
            Debug.Assert(destination is not null);
            Debug.Assert(length > 0);

            Span<byte> span = new Span<byte>(destination, (int)length);
            span.Clear();
        }
    }
}
