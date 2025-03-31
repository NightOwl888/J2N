#define FEATURE_BINARYPRIMITIVES_INTEGRAL

// Based on: https://github.com/dotnet/runtime/blob/v9.0.3/src/libraries/System.Private.CoreLib/src/System/Buffers/Binary/BinaryPrimitives.ReverseEndianness.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
#if FEATURE_BINARYPRIMITIVES_INTEGRAL
using System.Buffers.Binary;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if FEATURE_VECTOR_SIMD
using System.Runtime.Intrinsics;
#endif

#pragma warning disable CS9191 // The ref parameter is equivalent to in
#pragma warning disable CS1591 // Missing XML comment

namespace J2N.Buffers.Binary
{
    public static partial class BinaryPrimitive
    {
        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="sbyte" /> value, which effectively does nothing for an <see cref="sbyte" />.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The passed-in value, unmodified.</returns>
        /// <remarks>This method effectively does nothing and was added only for consistency.</remarks>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReverseEndianness(sbyte value) => value;

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="short" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReverseEndianness(short value) => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="int" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReverseEndianness(int value) => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="long" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReverseEndianness(long value) => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="nint" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint ReverseEndianness(nint value) => processHelper.ReverseEndianness(value);


        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="nint" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReverseEndianness(float value) // J2N specific
        {
            int intValue = Unsafe.As<float, int>(ref Unsafe.AsRef(ref value));
            intValue = BinaryPrimitives.ReverseEndianness(intValue);
            return Unsafe.As<int, float>(ref intValue);
        }

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="nint" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReverseEndianness(double value) // J2N specific
        {
            long longValue = Unsafe.As<double, long>(ref Unsafe.AsRef(ref value));
            longValue = BinaryPrimitives.ReverseEndianness(longValue);
            return Unsafe.As<long, double>(ref longValue);
        }

        //#if FEATURE_INT128

        //        /// <summary>
        //        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="Int128" /> value.
        //        /// </summary>
        //        /// <param name="value">The value to reverse.</param>
        //        /// <returns>The reversed value.</returns>
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        public static Int128 ReverseEndianness(Int128 value)
        //        {
        //            return new Int128(
        //                ReverseEndianness(value.Lower),
        //                ReverseEndianness(value.Upper)
        //            );
        //        }

        //#endif

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="byte" /> value, which effectively does nothing for an <see cref="byte" />.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The passed-in value, unmodified.</returns>
        /// <remarks>This method effectively does nothing and was added only for consistency.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReverseEndianness(byte value) => value;

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="ushort" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        [CLSCompliant(false)]
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReverseEndianness(ushort value) => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="char" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char ReverseEndianness(char value) => (char)ReverseEndianness((ushort)value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="uint" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        [CLSCompliant(false)]
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReverseEndianness(uint value) => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="ulong" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        [CLSCompliant(false)]
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReverseEndianness(ulong value) => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="nuint" /> value.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>The reversed value.</returns>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint ReverseEndianness(nuint value) => processHelper.ReverseEndianness(value);

        //#if FEATURE_INT128

        //        /// <summary>
        //        /// Reverses a primitive value by performing an endianness swap of the specified <see cref="UInt128" /> value.
        //        /// </summary>
        //        /// <param name="value">The value to reverse.</param>
        //        /// <returns>The reversed value.</returns>
        //        [CLSCompliant(false)]
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        public static UInt128 ReverseEndianness(UInt128 value)
        //        {
        //            return new UInt128(
        //                ReverseEndianness(value.Lower),
        //                ReverseEndianness(value.Upper)
        //            );
        //        }

        //#endif

        /// <summary>Copies every primitive value from <paramref name="source"/> to <paramref name="destination"/>, reversing each primitive by performing an endianness swap as part of writing each.</summary>
        /// <param name="source">The source span to copy.</param>
        /// <param name="destination">The destination to which the source elements should be copied.</param>
        /// <remarks>The source and destination spans may overlap. The same span may be passed as both the source and the destination in order to reverse each element's endianness in place.</remarks>
        /// <exception cref="ArgumentException">The <paramref name="destination"/>'s length is smaller than that of the <paramref name="source"/>.</exception>
        [CLSCompliant(false)]
        public static void ReverseEndianness(ReadOnlySpan<ushort> source, Span<ushort> destination) =>
#if FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            BinaryPrimitives.ReverseEndianness(source, destination);
#else
            ReverseEndianness(MemoryMarshal.Cast<ushort, short>(source), MemoryMarshal.Cast<ushort, short>(destination), Int16EndiannessReverser.Instance);
#endif

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        public static void ReverseEndianness(ReadOnlySpan<short> source, Span<short> destination) =>
#if FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            BinaryPrimitives.ReverseEndianness(source, destination);
#else
            ReverseEndianness(source, destination, Int16EndiannessReverser.Instance);
#endif

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        [CLSCompliant(false)]
        public static void ReverseEndianness(ReadOnlySpan<uint> source, Span<uint> destination) =>
#if FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            BinaryPrimitives.ReverseEndianness(source, destination);
#else
            ReverseEndianness(MemoryMarshal.Cast<uint, int>(source), MemoryMarshal.Cast<uint, int>(destination), Int32EndiannessReverser.Instance);
#endif

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        public static void ReverseEndianness(ReadOnlySpan<int> source, Span<int> destination) =>
#if FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            BinaryPrimitives.ReverseEndianness(source, destination);
#else
            ReverseEndianness(source, destination, Int32EndiannessReverser.Instance);
#endif

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        [CLSCompliant(false)]
        public static void ReverseEndianness(ReadOnlySpan<ulong> source, Span<ulong> destination) =>
#if FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            BinaryPrimitives.ReverseEndianness(source, destination);
#else
            ReverseEndianness(MemoryMarshal.Cast<ulong, long>(source), MemoryMarshal.Cast<ulong, long>(destination), Int64EndiannessReverser.Instance);
#endif

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        public static void ReverseEndianness(ReadOnlySpan<long> source, Span<long> destination) =>
#if FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            BinaryPrimitives.ReverseEndianness(source, destination);
#else
            ReverseEndianness(source, destination, Int64EndiannessReverser.Instance);
#endif

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        [CLSCompliant(false)]
        public static void ReverseEndianness(ReadOnlySpan<nuint> source, Span<nuint> destination) =>
#if FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            BinaryPrimitives.ReverseEndianness(source, destination);
#else
            processHelper.ReverseEndianness(source, destination);
#endif

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        public static void ReverseEndianness(ReadOnlySpan<nint> source, Span<nint> destination) =>
#if FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            BinaryPrimitives.ReverseEndianness(source, destination);
#else
            processHelper.ReverseEndianness(source, destination);
#endif

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        public static void ReverseEndianness(ReadOnlySpan<float> source, Span<float> destination) // J2N specific
            => ReverseEndianness(source, destination, SingleEndiannessReverser.Instance);

        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        public static void ReverseEndianness(ReadOnlySpan<double> source, Span<double> destination) // J2N specific
            => ReverseEndianness(source, destination, DoubleEndiannessReverser.Instance);


#if !FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
        private readonly struct Int16EndiannessReverser : IEndiannessReverser<short>
        {
            public static Int16EndiannessReverser Instance = new Int16EndiannessReverser();

            public short Reverse(short value) =>
                ReverseEndianness(value);

#if FEATURE_VECTOR_SIMD
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector128<short> Reverse(Vector128<short> vector) =>
                Vector128.ShiftLeft(vector, 8) | Vector128.ShiftRightLogical(vector, 8);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector256<short> Reverse(Vector256<short> vector) =>
                Vector256.ShiftLeft(vector, 8) | Vector256.ShiftRightLogical(vector, 8);
#endif
        }

        private readonly struct Int32EndiannessReverser : IEndiannessReverser<int>
        {
            public static Int32EndiannessReverser Instance = new Int32EndiannessReverser();

            public int Reverse(int value) =>
                ReverseEndianness(value);

#if FEATURE_VECTOR_SIMD
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector128<int> Reverse(Vector128<int> vector) =>
                Vector128.Shuffle(vector.AsByte(), Vector128.Create((byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12)).AsInt32();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector256<int> Reverse(Vector256<int> vector) =>
                Vector256.Shuffle(vector.AsByte(), Vector256.Create((byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12, 19, 18, 17, 16, 23, 22, 21, 20, 27, 26, 25, 24, 31, 30, 29, 28)).AsInt32();
#endif
        }

        private readonly struct Int64EndiannessReverser : IEndiannessReverser<long>
        {
            public static Int64EndiannessReverser Instance = new Int64EndiannessReverser();

            public long Reverse(long value) =>
                ReverseEndianness(value);

#if FEATURE_VECTOR_SIMD
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector128<long> Reverse(Vector128<long> vector) =>
                Vector128.Shuffle(vector.AsByte(), Vector128.Create((byte)7, 6, 5, 4, 3, 2, 1, 0, 15, 14, 13, 12, 11, 10, 9, 8)).AsInt64();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector256<long> Reverse(Vector256<long> vector) =>
                Vector256.Shuffle(vector.AsByte(), Vector256.Create((byte)7, 6, 5, 4, 3, 2, 1, 0, 15, 14, 13, 12, 11, 10, 9, 8, 23, 22, 21, 20, 19, 18, 17, 16, 31, 30, 29, 28, 27, 26, 25, 24)).AsInt64();
#endif
        }

#endif

        private readonly struct SingleEndiannessReverser : IEndiannessReverser<float>
        {
            public static SingleEndiannessReverser Instance = new SingleEndiannessReverser();

            public float Reverse(float value) =>
                ReverseEndianness(value);

#if FEATURE_VECTOR_SIMD
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector128<float> Reverse(Vector128<float> vector) =>
                Vector128.Shuffle(vector.AsByte(), Vector128.Create((byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12)).AsSingle();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector256<float> Reverse(Vector256<float> vector) =>
                Vector256.Shuffle(vector.AsByte(), Vector256.Create((byte)3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12, 19, 18, 17, 16, 23, 22, 21, 20, 27, 26, 25, 24, 31, 30, 29, 28)).AsSingle();
#endif
        }

        private readonly struct DoubleEndiannessReverser : IEndiannessReverser<double>
        {
            public static DoubleEndiannessReverser Instance = new DoubleEndiannessReverser();

            public double Reverse(double value) =>
                ReverseEndianness(value);

#if FEATURE_VECTOR_SIMD
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector128<double> Reverse(Vector128<double> vector) =>
                Vector128.Shuffle(vector.AsByte(), Vector128.Create((byte)7, 6, 5, 4, 3, 2, 1, 0, 15, 14, 13, 12, 11, 10, 9, 8)).AsDouble();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector256<double> Reverse(Vector256<double> vector) =>
                Vector256.Shuffle(vector.AsByte(), Vector256.Create((byte)7, 6, 5, 4, 3, 2, 1, 0, 15, 14, 13, 12, 11, 10, 9, 8, 23, 22, 21, 20, 19, 18, 17, 16, 31, 30, 29, 28, 27, 26, 25, 24)).AsDouble();
#endif
        }

        //private static void ReverseEndianness<T, TReverser>(ReadOnlySpan<T> source, Span<T> destination)
        //    where T : struct
        //    where TReverser : IEndiannessReverser<T>

        private static void ReverseEndianness<T>(ReadOnlySpan<T> source, Span<T> destination, IEndiannessReverser<T> reverser)
            where T : struct
        {
            if (destination.Length < source.Length)
            {
                ThrowDestinationTooSmall();
            }

            ref T sourceRef = ref MemoryMarshal.GetReference(source);
            ref T destRef = ref MemoryMarshal.GetReference(destination);

            if (Unsafe.AreSame(ref sourceRef, ref destRef) ||
                !source.Overlaps(destination, out int elementOffset) ||
                elementOffset < 0)
            {
                // Either there's no overlap between the source and the destination, or there's overlap but the
                // destination starts at or before the source.  That means we can safely iterate from beginning
                // to end of the source and not have to worry about writing into the destination and clobbering
                // source data we haven't yet read.

                int i = 0;

#if FEATURE_VECTOR_SIMD
                if (Vector256.IsHardwareAccelerated)
                {
                    while (i <= source.Length - Vector256<T>.Count)
                    {
                        //Vector256.StoreUnsafe(TReverser.Reverse(Vector256.LoadUnsafe(ref sourceRef, (uint)i)), ref destRef, (uint)i);
                        Vector256.StoreUnsafe(reverser.Reverse(Vector256.LoadUnsafe(ref sourceRef, (uint)i)), ref destRef, (uint)i);
                        i += Vector256<T>.Count;
                    }
                }

                if (Vector128.IsHardwareAccelerated)
                {
                    while (i <= source.Length - Vector128<T>.Count)
                    {
                        //Vector128.StoreUnsafe(TReverser.Reverse(Vector128.LoadUnsafe(ref sourceRef, (uint)i)), ref destRef, (uint)i);
                        Vector128.StoreUnsafe(reverser.Reverse(Vector128.LoadUnsafe(ref sourceRef, (uint)i)), ref destRef, (uint)i);
                        i += Vector128<T>.Count;
                    }
                }
#endif
                while (i < source.Length)
                {
                    //Unsafe.Add(ref destRef, i) = TReverser.Reverse(Unsafe.Add(ref sourceRef, i));
                    Unsafe.Add(ref destRef, i) = reverser.Reverse(Unsafe.Add(ref sourceRef, i));
                    i++;
                }
            }
            else
            {
                // There's overlap between the source and the destination, and the source starts before the destination.
                // That means if we were to iterate from beginning to end, reading from the source and writing to the
                // destination, we'd overwrite source elements not yet read.  To avoid that, we iterate from end to beginning.

                int i = source.Length;

#if FEATURE_VECTOR_SIMD
                if (Vector256.IsHardwareAccelerated)
                {
                    while (i >= Vector256<T>.Count)
                    {
                        i -= Vector256<T>.Count;
                        //Vector256.StoreUnsafe(TReverser.Reverse(Vector256.LoadUnsafe(ref sourceRef, (uint)i)), ref destRef, (uint)i);
                        Vector256.StoreUnsafe(reverser.Reverse(Vector256.LoadUnsafe(ref sourceRef, (uint)i)), ref destRef, (uint)i);
                    }
                }

                if (Vector128.IsHardwareAccelerated)
                {
                    while (i >= Vector128<T>.Count)
                    {
                        i -= Vector128<T>.Count;
                        //Vector128.StoreUnsafe(TReverser.Reverse(Vector128.LoadUnsafe(ref sourceRef, (uint)i)), ref destRef, (uint)i);
                        Vector128.StoreUnsafe(reverser.Reverse(Vector128.LoadUnsafe(ref sourceRef, (uint)i)), ref destRef, (uint)i);
                    }
                }
#endif
                while (i > 0)
                {
                    i--;
                    //Unsafe.Add(ref destRef, i) = TReverser.Reverse(Unsafe.Add(ref sourceRef, i));
                    Unsafe.Add(ref destRef, i) = reverser.Reverse(Unsafe.Add(ref sourceRef, i));
                }
            }
        }

        private interface IEndiannessReverser<T> where T : struct
        {
            T Reverse(T value);
#if FEATURE_VECTOR_SIMD
            Vector128<T> Reverse(Vector128<T> vector);
            Vector256<T> Reverse(Vector256<T> vector);
#endif
        }


        //#if FEATURE_INT128

        //        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        //        [CLSCompliant(false)]
        //        public static void ReverseEndianness(ReadOnlySpan<UInt128> source, Span<UInt128> destination) =>
        //            ReverseEndianness(MemoryMarshal.Cast<UInt128, Int128>(source), MemoryMarshal.Cast<UInt128, Int128>(destination));

        //        /// <inheritdoc cref="ReverseEndianness(ReadOnlySpan{ushort}, Span{ushort})" />
        //        public static void ReverseEndianness(ReadOnlySpan<Int128> source, Span<Int128> destination)
        //        {
        //            if (destination.Length < source.Length)
        //            {
        //                ThrowDestinationTooSmall();
        //            }

        //            if (Unsafe.AreSame(ref MemoryMarshal.GetReference(source), ref MemoryMarshal.GetReference(destination)) ||
        //                !source.Overlaps(destination, out int elementOffset) ||
        //                elementOffset < 0)
        //            {
        //                // Iterate from beginning to end
        //                for (int i = 0; i < source.Length; i++)
        //                {
        //                    destination[i] = ReverseEndianness(source[i]);
        //                }
        //            }
        //            else
        //            {
        //                // Iterate from end to beginning
        //                for (int i = source.Length - 1; i >= 0; i--)
        //                {
        //                    destination[i] = ReverseEndianness(source[i]);
        //                }
        //            }
        //        }

        //#endif


        [DoesNotReturn]
        private static void ThrowDestinationTooSmall() =>
            throw new ArgumentException(SR.Arg_BufferTooSmall, "destination");



        private static readonly IProcessHelper processHelper =
            Environment.Is64BitProcess ? new _64BitProcessHelper() : new _32BitProcessHelper();

        private interface IProcessHelper
        {
            nint ReverseEndianness(nint value);
            nuint ReverseEndianness(nuint value);

#if !FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            void ReverseEndianness(ReadOnlySpan<nint> source, Span<nint> destination);
            void ReverseEndianness(ReadOnlySpan<nuint> source, Span<nuint> destination);
#endif
        }

        private sealed partial class _64BitProcessHelper : IProcessHelper
        {
            public nint ReverseEndianness(nint value)
                => (nint)BinaryPrimitives.ReverseEndianness((ulong)value);
            public nuint ReverseEndianness(nuint value)
                => (nuint)BinaryPrimitives.ReverseEndianness((ulong)value);

#if !FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            public void ReverseEndianness(ReadOnlySpan<nint> source, Span<nint> destination)
                => BinaryPrimitive.ReverseEndianness(MemoryMarshal.Cast<nint, long>(source), MemoryMarshal.Cast<nint, long>(destination), Int64EndiannessReverser.Instance);

            public void ReverseEndianness(ReadOnlySpan<nuint> source, Span<nuint> destination)
                => BinaryPrimitive.ReverseEndianness(MemoryMarshal.Cast<nuint, long>(source), MemoryMarshal.Cast<nuint, long>(destination), Int64EndiannessReverser.Instance);
#endif
        }

        private sealed partial class _32BitProcessHelper : IProcessHelper
        {
            public nint ReverseEndianness(nint value)
                => (nint)BinaryPrimitives.ReverseEndianness((uint)value);

            public nuint ReverseEndianness(nuint value)
                => (nuint)BinaryPrimitives.ReverseEndianness((uint)value);

#if !FEATURE_BINARYPRIMITVES_REVERSEENDIANNESS_SEQUENCE_INTEGRAL
            public void ReverseEndianness(ReadOnlySpan<nint> source, Span<nint> destination)
                => BinaryPrimitive.ReverseEndianness(MemoryMarshal.Cast<nint, int>(source), MemoryMarshal.Cast<nint, int>(destination), Int32EndiannessReverser.Instance);

            public void ReverseEndianness(ReadOnlySpan<nuint> source, Span<nuint> destination)
                => BinaryPrimitive.ReverseEndianness(MemoryMarshal.Cast<nuint, int>(source), MemoryMarshal.Cast<nuint, int>(destination), Int32EndiannessReverser.Instance);
#endif
        }
    }
}
