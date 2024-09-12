// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

        private sealed class OrdinalComparer : RandomizedStringEqualityComparer
        {
            internal OrdinalComparer(IEqualityComparer<string?> wrappedComparer)
                : base(wrappedComparer)
            {
            }

            public override bool Equals(string? x, string? y) => string.Equals(x, y);

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
        }

        private sealed class OrdinalIgnoreCaseComparer : RandomizedStringEqualityComparer
        {
            private static readonly StringComparer _stringComparer = StringComparer.OrdinalIgnoreCase;

            internal OrdinalIgnoreCaseComparer(IEqualityComparer<string?> wrappedComparer)
                : base(wrappedComparer)
            {
            }

            public override bool Equals(string? x, string? y) => _stringComparer.Equals(x, y); //string.EqualsOrdinalIgnoreCase(x, y);

            public override int GetHashCode(string? obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                // J2N TODO: Finsh randomized implementation for OrdinalIgnoreCase
                return _stringComparer.GetHashCode(obj);

                //// The OrdinalIgnoreCase version of Marvin32 operates over chars,
                //// so pass in the char count directly.
                //return Marvin.ComputeHash32OrdinalIgnoreCase(
                //    ref obj.GetRawStringData(),
                //    obj.Length,
                //    _seed.p0, _seed.p1);
            }
        }
    }
}
