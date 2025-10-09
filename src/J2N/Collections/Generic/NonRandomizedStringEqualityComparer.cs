// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Numerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace J2N.Collections.Generic
{
    /// <summary>
    /// NonRandomizedStringEqualityComparer is the comparer used by default with the Dictionary&lt;string,...&gt;
    /// We use NonRandomizedStringEqualityComparer as default comparer as it doesnt use the randomized string hashing which
    /// keeps the performance not affected till we hit collision threshold and then we switch to the comparer which is using
    /// randomized string hashing.
    /// </summary>
    // J2N: Although, this is public in .NET, we don't need to maintain compatibility with .NET Core 2.0 so this is internal.
    // This has never been exposed in the J2N serialization blob.
    internal class NonRandomizedStringEqualityComparer : IEqualityComparer<string?>, IInternalStringEqualityComparer, ISerializable
    {
        // Dictionary<...>.Comparer and similar methods need to return the original IEqualityComparer
        // that was passed in to the ctor. The caller chooses one of these singletons so that the
        // GetUnderlyingEqualityComparer method can return the correct value.

        //private static readonly NonRandomizedStringEqualityComparer WrappedAroundDefaultComparer = new DefaultComparer(EqualityComparer<string?>.Default);

        private static readonly NonRandomizedStringEqualityComparer WrappedAroundStringComparerOrdinal = new OrdinalComparer(StringComparer.Ordinal);

        //private static readonly NonRandomizedStringEqualityComparer WrappedAroundStringComparerOrdinalIgnoreCase = new OrdinalIgnoreCaseComparer(StringComparer.OrdinalIgnoreCase);

        private readonly IEqualityComparer<string?> _underlyingComparer;

        private NonRandomizedStringEqualityComparer(IEqualityComparer<string?> underlyingComparer)
        {
            Debug.Assert(underlyingComparer != null);
            _underlyingComparer = underlyingComparer!;
        }

        // This is used by the serialization engine.
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected NonRandomizedStringEqualityComparer(SerializationInfo information, StreamingContext context)
            : this(EqualityComparer<string?>.Default)
        {
        }

        public virtual bool Equals(string? x, string? y)
        {
            // This instance may have been deserialized into a class that doesn't guarantee
            // these parameters are non-null. Can't short-circuit the null checks.

            return string.Equals(x, y);
        }

        public virtual int GetHashCode(string? obj)
        {
            // This instance may have been deserialized into a class that doesn't guarantee
            // these parameters are non-null. Can't short-circuit the null checks.

            //return obj?.GetNonRandomizedHashCode() ?? 0;
            if (obj is null)
                return 0;
            return GetNonRandomizedHashCode(obj);
        }

        internal virtual RandomizedStringEqualityComparer GetRandomizedEqualityComparer()
        {
            return RandomizedStringEqualityComparer.Create(_underlyingComparer, ignoreCase: false);
        }

        // Gets the comparer that should be returned back to the caller when querying the
        // ICollection.Comparer property. Also used for serialization purposes.
        public virtual IEqualityComparer<string?> GetUnderlyingEqualityComparer() => _underlyingComparer;

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // We are doing this to stay compatible with .NET Framework.
            // Our own collection types will never call this (since this type is a wrapper),
            // but perhaps third-party collection types could try serializing an instance
            // of this.
            //System.Collections.Generic.EqualityComparer<string>.

            //info.SetType(typeof(System.Collections.Generic.GenericEqualityComparer<string>));
            info.SetType(typeof(EqualityComparer<string>));
        }

        // TODO https://github.com/dotnet/runtime/issues/102906:
        // This custom class exists because EqualityComparer<string>.Default doesn't implement IAlternateEqualityComparer<ROS<char>, string>.
        // If OrdinalComparer were used, then a dictionary created with a null/Default comparer would be using a comparer that does
        // implement IAlternateEqualityComparer<ROS<char>, string>, but only until it hits a collision and switches to the randomized comparer.
        // Once EqualityComparer<string>.Default implements IAlternateEqualityComparer<ROS<char>, string>, we can remove this class, and change
        // WrappedAroundDefaultComparer to be an instance of OrdinalComparer.
        private sealed class DefaultComparer : NonRandomizedStringEqualityComparer
        {
            internal DefaultComparer(IEqualityComparer<string?> wrappedComparer) : base(wrappedComparer)
            {
            }

            public override bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.Ordinal);

            public override int GetHashCode(string? obj)
            {
                //Debug.Assert(obj != null, "This implementation is only called from first-party collection types that guarantee non-null parameters.");
                //return obj.GetNonRandomizedHashCode();
                return obj is null ? 0 : GetNonRandomizedHashCode(obj);
            }
        }

        private sealed class OrdinalComparer : NonRandomizedStringEqualityComparer
#if FEATURE_IALTERNATEEQUALITYCOMPARER
            , IAlternateEqualityComparer<ReadOnlySpan<char>, string?>
#endif
        {
            internal OrdinalComparer(IEqualityComparer<string?> wrappedComparer) : base(wrappedComparer)
            {
            }

            public override bool Equals(string? x, string? y) => string.Equals(x, y);

            public override int GetHashCode(string? obj)
            {
                Debug.Assert(obj != null, "This implementation is only called from first-party collection types that guarantee non-null parameters.");
                return GetNonRandomizedHashCode(obj!);
            }

#if FEATURE_IALTERNATEEQUALITYCOMPARER
            int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> span) =>
                GetNonRandomizedHashCode(span);

            bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> span, string? target)
            {
                // See explanation in StringEqualityComparer.Equals.
                if (span.IsEmpty && target is null)
                {
                    return false;
                }

                return span.SequenceEqual(target);
            }

            string IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
#endif
        }

        // J2N: This might be too difficult to implement. It ends up calling native globalization code.
        // private sealed class OrdinalIgnoreCaseComparer : NonRandomizedStringEqualityComparer
        // #if FEATURE_IALTERNATEEQUALITYCOMPARER
        //    , IAlternateEqualityComparer<ReadOnlySpan<char>, string?>
        // #endif
        // {
        //     internal OrdinalIgnoreCaseComparer(IEqualityComparer<string?> wrappedComparer)
        //         : base(wrappedComparer)
        //     {
        //     }
        //
        //     public override bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        //
        //     public override int GetHashCode(string? obj)
        //     {
        //         Debug.Assert(obj != null, "This implementation is only called from first-party collection types that guarantee non-null parameters.");
        //         return GetNonRandomizedHashCodeOrdinalIgnoreCase(obj);
        //     }
        // #if FEATURE_IALTERNATEEQUALITYCOMPARER
        //     int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> span) =>
        //         string.GetNonRandomizedHashCodeOrdinalIgnoreCase(span);
        //
        //     bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> span, string? target)
        //     {
        //         // See explanation in StringEqualityComparer.Equals.
        //         if (span.IsEmpty && target is null)
        //         {
        //             return false;
        //         }
        //
        //         return span.EqualsOrdinalIgnoreCase(target);
        //     }
        //
        //     string IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span) =>
        //         span.ToString();
        // #endif
        //
        //     internal override RandomizedStringEqualityComparer GetRandomizedEqualityComparer()
        //     {
        //         return RandomizedStringEqualityComparer.Create(_underlyingComparer, ignoreCase: true);
        //     }
        //
        //     internal unsafe int GetNonRandomizedHashCodeOrdinalIgnoreCase(string value) // From String class
        //     {
        //         uint hash1 = (5381 << 16) + 5381;
        //         uint hash2 = hash1;
        //
        //         fixed (char* src = value)
        //         {
        //             Debug.Assert(src[value.Length] == '\0', "src[this.Length] == '\\0'");
        //             Debug.Assert(((int)src) % 4 == 0, "Managed string should start at 4 bytes boundary");
        //
        //             uint* ptr = (uint*)src;
        //             int length = value.Length;
        //
        //             // We "normalize to lowercase" every char by ORing with 0x0020. This casts
        //             // a very wide net because it will change, e.g., '^' to '~'. But that should
        //             // be ok because we expect this to be very rare in practice.
        //             const uint NormalizeToLowercase = 0x0020_0020u; // valid both for big-endian and for little-endian
        //
        //             while (length > 2)
        //             {
        //                 uint p0 = ptr[0];
        //                 uint p1 = ptr[1];
        //                 if (!AllCharsInUInt32AreAscii(p0 | p1))
        //                 {
        //                     goto NotAscii;
        //                 }
        //
        //                 length -= 4;
        //                 // Where length is 4n-1 (e.g. 3,7,11,15,19) this additionally consumes the null terminator
        //                 hash1 = (BitOperation.RotateLeft(hash1, 5) + hash1) ^ (p0 | NormalizeToLowercase);
        //                 hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (p1 | NormalizeToLowercase);
        //                 ptr += 2;
        //             }
        //
        //             if (length > 0)
        //             {
        //                 uint p0 = ptr[0];
        //                 if (!AllCharsInUInt32AreAscii(p0))
        //                 {
        //                     goto NotAscii;
        //                 }
        //
        //                 // Where length is 4n-3 (e.g. 1,5,9,13,17) this additionally consumes the null terminator
        //                 hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (p0 | NormalizeToLowercase);
        //             }
        //         }
        //
        //         return (int)(hash1 + (hash2 * 1566083941));
        //
        //     NotAscii:
        //         return GetNonRandomizedHashCodeOrdinalIgnoreCaseSlow(value);
        //
        //         static int GetNonRandomizedHashCodeOrdinalIgnoreCaseSlow(string str)
        //         {
        //             int length = str.Length;
        //
        //             // We allocate one char more than the length to accommodate a null terminator.
        //             // That lets the reading always be performed two characters at a time, as odd-length
        //             // inputs will have a final terminator to backstop the last read.
        //             char[]? borrowedArr = null;
        //             Span<char> scratch = (uint)length < 256 ?
        //                 stackalloc char[256] :
        //                 (borrowedArr = ArrayPool<char>.Shared.Rent(length + 1));
        //
        //             int charsWritten = OrdinalCasing.ToUpperOrdinal(str, scratch);
        //             Debug.Assert(charsWritten == length);
        //             scratch[length] = '\0';
        //
        //             const uint NormalizeToLowercase = 0x0020_0020u;
        //             uint hash1 = (5381 << 16) + 5381;
        //             uint hash2 = hash1;
        //
        //             // Duplicate the main loop, can be removed once JIT gets "Loop Unswitching" optimization
        //             fixed (char* src = scratch)
        //             {
        //                 uint* ptr = (uint*)src;
        //                 while (length > 2)
        //                 {
        //                     length -= 4;
        //                     hash1 = (BitOperation.RotateLeft(hash1, 5) + hash1) ^ (ptr[0] | NormalizeToLowercase);
        //                     hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (ptr[1] | NormalizeToLowercase);
        //                     ptr += 2;
        //                 }
        //
        //                 if (length > 0)
        //                 {
        //                     hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (ptr[0] | NormalizeToLowercase);
        //                 }
        //             }
        //
        //             if (borrowedArr != null)
        //             {
        //                 ArrayPool<char>.Shared.Return(borrowedArr);
        //             }
        //             return (int)(hash1 + (hash2 * 1566083941));
        //         }
        //     }
        //
        //     /// <summary>
        //     /// Returns true iff the UInt32 represents two ASCII UTF-16 characters in machine endianness.
        //     /// </summary>
        //     /// <remarks>
        //     /// From Utf16Utility in .NET
        //     /// </remarks>
        //     [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //     private static bool AllCharsInUInt32AreAscii(uint value)
        //     {
        //         return (value & ~0x007F_007Fu) == 0;
        //     }
        // }

        public static IEqualityComparer<string>? GetStringComparer(object comparer)
        {
            // Special-case EqualityComparer<string>.Default, StringComparer.Ordinal, and StringComparer.OrdinalIgnoreCase.
            // We use a non-randomized comparer for improved perf, falling back to a randomized comparer if the
            // hash buckets become unbalanced.

            // J2N: EqualityComparer<string>.Default returns StringComparer.Ordinal, so we don't need to handle it separately.
            // if (ReferenceEquals(comparer, EqualityComparer<string>.Default))
            // {
            //     return WrappedAroundDefaultComparer;
            // }

            if (ReferenceEquals(comparer, StringComparer.Ordinal))
            {
                return WrappedAroundStringComparerOrdinal;
            }

            // J2N TODO: Finish implementation for OrdinalIgnoreCase
            //if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
            //{
            //    return WrappedAroundStringComparerOrdinalIgnoreCase;
            //}

            return null;
        }

        // Use this if and only if 'Denial of Service' attacks are not a concern (i.e. never used for free-form user input),
        // or are otherwise mitigated
        private static unsafe int GetNonRandomizedHashCode(string value) // From String class
        {
            fixed (char* src = value)
            {
                Debug.Assert(src[value.Length] == '\0', "src[this.Length] == '\\0'");
                Debug.Assert(((int)src) % 4 == 0, "Managed string should start at 4 bytes boundary");

                uint hash1 = (5381 << 16) + 5381;
                uint hash2 = hash1;

                uint* ptr = (uint*)src;
                int length = value.Length;

                while (length > 2)
                {
                    length -= 4;
                    // Where length is 4n-1 (e.g. 3,7,11,15,19) this additionally consumes the null terminator
                    hash1 = (BitOperation.RotateLeft(hash1, 5) + hash1) ^ ptr[0];
                    hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ ptr[1];
                    ptr += 2;
                }

                if (length > 0)
                {
                    // Where length is 4n-3 (e.g. 1,5,9,13,17) this additionally consumes the null terminator
                    hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ ptr[0];
                }

                return (int)(hash1 + (hash2 * 1566083941));
            }
        }

        private static unsafe int GetNonRandomizedHashCode(ReadOnlySpan<char> span) // From String class
        {
            uint hash1 = (5381 << 16) + 5381;
            uint hash2 = hash1;

            int length = span.Length;
            fixed (char* src = &MemoryMarshal.GetReference(span))
            {
                uint* ptr = (uint*)src;

                LengthSwitch:
                switch (length)
                {
                    default:
                        do
                        {
                            length -= 4;
                            hash1 = BitOperation.RotateLeft(hash1, 5) + hash1 ^ Unsafe.ReadUnaligned<uint>(ptr);
                            hash2 = BitOperation.RotateLeft(hash2, 5) + hash2 ^ Unsafe.ReadUnaligned<uint>(ptr + 1);
                            ptr += 2;
                        }
                        while (length >= 4);
                        goto LengthSwitch;

                    case 3:
                        hash1 = BitOperation.RotateLeft(hash1, 5) + hash1 ^ Unsafe.ReadUnaligned<uint>(ptr);
                        uint p1 = *(char*)(ptr + 1);
                        if (!BitConverter.IsLittleEndian)
                        {
                            p1 <<= 16;
                        }

                        hash2 = BitOperation.RotateLeft(hash2, 5) + hash2 ^ p1;
                        break;

                    case 2:
                        hash2 = BitOperation.RotateLeft(hash2, 5) + hash2 ^ Unsafe.ReadUnaligned<uint>(ptr);
                        break;

                    case 1:
                        uint p0 = *(char*)ptr;
                        if (!BitConverter.IsLittleEndian)
                        {
                            p0 <<= 16;
                        }

                        hash2 = BitOperation.RotateLeft(hash2, 5) + hash2 ^ p0;
                        break;

                    case 0:
                        break;
                }
            }

            return (int)(hash1 + (hash2 * 1_566_083_941));
        }
    }
}
