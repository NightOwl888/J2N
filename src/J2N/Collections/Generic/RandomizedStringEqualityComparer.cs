// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// A randomized <see cref="EqualityComparer{String}"/> which uses a different seed on each
    /// construction as a general good hygiene + defense-in-depth mechanism. This implementation
    /// *does not* need to stay in sync with string.GetHashCode, which for stability
    /// is required to use an app-global seed.
    /// </summary>
    internal abstract class RandomizedStringEqualityComparer : SCG.EqualityComparer<string?>, IInternalStringEqualityComparer
    {
        private readonly MarvinSeed _seed;
        private readonly IEqualityComparer<string?> _underlyingComparer;

        private unsafe RandomizedStringEqualityComparer(IEqualityComparer<string?> underlyingComparer)
        {
            _underlyingComparer = underlyingComparer;

            // Use RandomHelpers to fill MarvinSeed with random bytes
            fixed (MarvinSeed* seed = &_seed)
            {
                RandomHelpers.GetRandomBytes((byte*)seed, sizeof(MarvinSeed));
            }
        }

        internal static RandomizedStringEqualityComparer Create(IEqualityComparer<string?> underlyingComparer, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return new OrdinalComparer(underlyingComparer);
            }
            else
            {
                return new OrdinalIgnoreCaseComparer(underlyingComparer);
            }
        }

        public IEqualityComparer<string?> GetUnderlyingEqualityComparer() => _underlyingComparer;

        private struct MarvinSeed
        {
            internal uint p0;
            internal uint p1;
        }

        private sealed class OrdinalComparer : RandomizedStringEqualityComparer, ISpanAlternateEqualityComparer<char, string?>
#if FEATURE_IALTERNATEEQUALITYCOMPARER
            , IAlternateEqualityComparer<ReadOnlySpan<char>, string?>
#endif
        {
            internal OrdinalComparer(IEqualityComparer<string?> wrappedComparer)
                : base(wrappedComparer)
            {
            }

#if FEATURE_IALTERNATEEQUALITYCOMPARER
            string IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();

#endif
            public override bool Equals(string? x, string? y) => string.Equals(x, y);

#if FEATURE_IALTERNATEEQUALITYCOMPARER
            bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> alternate, string? other)
            {
                // See explanation in System.OrdinalComparer.Equals.
                if (alternate.IsEmpty && other is null)
                {
                    return false;
                }

                return alternate.SequenceEqual(other);
            }
#endif
            public override int GetHashCode(string? obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                // The Ordinal version of Marvin32 operates over bytes.
                // The multiplication from # chars -> # bytes will never integer overflow.
                unsafe
                {
                    fixed (char* charPtr = obj)
                    {
                        // Treat the char* as a byte* to operate over bytes
                        byte* bytePtr = (byte*)charPtr;

                        // Compute the hash using the Marvin algorithm
                        return Marvin.ComputeHash32(ref *bytePtr, (uint)obj.Length * 2, _seed.p0, _seed.p1);
                    }
                }
            }

#if FEATURE_IALTERNATEEQUALITYCOMPARER
            int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> alternate) =>
                Marvin.ComputeHash32(
                    ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(alternate)),
                    (uint)alternate.Length * 2,
                    _seed.p0, _seed.p1);
#endif

            string ISpanAlternateEqualityComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();

            bool ISpanAlternateEqualityComparer<char, string?>.Equals(ReadOnlySpan<char> alternate, string? other)
            {
                // See explanation in System.OrdinalComparer.Equals.
                if (alternate.IsEmpty && other is null)
                {
                    return false;
                }

                return alternate.SequenceEqual(other);
            }

            int ISpanAlternateEqualityComparer<char, string?>.GetHashCode(ReadOnlySpan<char> alternate) =>
                Marvin.ComputeHash32(
                    ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(alternate)),
                    (uint)alternate.Length * 2,
                    _seed.p0, _seed.p1);
        }

        private sealed class OrdinalIgnoreCaseComparer : RandomizedStringEqualityComparer, ISpanAlternateEqualityComparer<char, string?>
#if FEATURE_IALTERNATEEQUALITYCOMPARER
            , IAlternateEqualityComparer<ReadOnlySpan<char>, string?>
#endif
        {
            internal OrdinalIgnoreCaseComparer(IEqualityComparer<string?> wrappedComparer)
                : base(wrappedComparer)
            {
            }

#if FEATURE_IALTERNATEEQUALITYCOMPARER
            string IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
#endif

            public override bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

#if FEATURE_IALTERNATEEQUALITYCOMPARER
            bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> alternate, string? other)
            {
                // See explanation in System.OrdinalComparer.Equals.
                if (alternate.IsEmpty && other is null)
                {
                    return false;
                }

                //return alternate.EqualsOrdinalIgnoreCase(other);
                return System.MemoryExtensions.Equals(alternate, other, StringComparison.OrdinalIgnoreCase);
            }
#endif
            public override int GetHashCode(string? obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                unsafe
                {
                    fixed (char* charPtr = obj)
                    {
                        // The OrdinalIgnoreCase version of Marvin32 operates over chars,
                        // so pass in the char count directly.
                        return Marvin.ComputeHash32OrdinalIgnoreCase(
                            ref *charPtr,
                            obj.Length,
                            _seed.p0,
                            _seed.p1);
                    }
                }
            }

#if FEATURE_IALTERNATEEQUALITYCOMPARER
            int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> alternate) =>
                Marvin.ComputeHash32OrdinalIgnoreCase(
                    ref MemoryMarshal.GetReference(alternate),
                    alternate.Length,
                    _seed.p0, _seed.p1);
#endif

            string ISpanAlternateEqualityComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();

            bool ISpanAlternateEqualityComparer<char, string?>.Equals(ReadOnlySpan<char> alternate, string? other)
            {
                // See explanation in System.OrdinalComparer.Equals.
                if (alternate.IsEmpty && other is null)
                {
                    return false;
                }

                return System.MemoryExtensions.Equals(alternate, other, StringComparison.OrdinalIgnoreCase);
            }

            int ISpanAlternateEqualityComparer<char, string?>.GetHashCode(ReadOnlySpan<char> alternate) =>
                Marvin.ComputeHash32OrdinalIgnoreCase(
                    ref MemoryMarshal.GetReference(alternate),
                    alternate.Length,
                    _seed.p0, _seed.p1);
        }
    }
}
