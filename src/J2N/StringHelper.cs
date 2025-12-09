using J2N.Globalization;
using J2N.Numerics;
using J2N.Text.Unicode;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace J2N
{
    /// <summary>
    /// Legacy support for static members missing from <see cref="string"/>.
    /// </summary>
    internal static class StringHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetHashCodeOrdinalIgnoreCase(string value)
        {
            Debug.Assert(value != null, "Caller should skip this call and return 0 for null values. This method is set up to match string.Empty.GetHashCode() so it matches the ReadOnlySpan<char> overload.");

            ulong seed = Marvin.DefaultSeed;
            unsafe
            {
                fixed (char* charPtr = value)
                {
                    // Compute the hash using the Marvin algorithm
                    return Marvin.ComputeHash32OrdinalIgnoreCase(ref *charPtr, value!.Length /* in chars, not bytes */, (uint)seed, (uint)(seed >> 32));
                }
            }
        }

        // Gets a hash code for the string. If strings A and B are such that A.Equals(B), then
        // they will return the same hash code.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetHashCode(string value)
        {
            Debug.Assert(value != null, "Caller should skip this call and return 0 for null values. This method is set up to match string.Empty.GetHashCode() so it matches the ReadOnlySpan<char> overload.");

            ulong seed = Marvin.DefaultSeed;
            unsafe
            {
                fixed (char* charPtr = value)
                {
                    // Multiplication below will not overflow since going from positive Int32 to UInt32.
                    return Marvin.ComputeHash32(ref Unsafe.As<char, byte>(ref *charPtr), (uint)value!.Length * 2 /* in bytes, not chars */, (uint)seed, (uint)(seed >> 32));
                }
            }
        }

        // A span-based equivalent of String.GetHashCode(). Computes an ordinal hash code.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(ReadOnlySpan<char> value)
        {
            ulong seed = Marvin.DefaultSeed;

            // Multiplication below will not overflow since going from positive Int32 to UInt32.
            return Marvin.ComputeHash32(ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(value)), (uint)value.Length * 2 /* in bytes, not chars */, (uint)seed, (uint)(seed >> 32));
        }

        // A span-based equivalent of String.GetHashCode(StringComparison). Uses the specified comparison type.
        public static int GetHashCode(ReadOnlySpan<char> value, StringComparison comparisonType)
        {
            switch (comparisonType)
            {
                case StringComparison.Ordinal:
                    return GetHashCode(value);

                case StringComparison.OrdinalIgnoreCase:
                    return GetHashCodeOrdinalIgnoreCase(value);

#if FEATURE_STRING_GETHASHCODE_READONLYSPAN_STRINGCOMPARISON
                case StringComparison.CurrentCulture:
                case StringComparison.CurrentCultureIgnoreCase:
                case StringComparison.InvariantCulture:
                case StringComparison.InvariantCultureIgnoreCase:
                    return string.GetHashCode(value, comparisonType);
#else
                case StringComparison.CurrentCulture:
                    return StringComparer.CurrentCulture.GetHashCode(value.ToString());

                case StringComparison.CurrentCultureIgnoreCase:
                    return StringComparer.CurrentCultureIgnoreCase.GetHashCode(value.ToString());

                case StringComparison.InvariantCulture:
                    return StringComparer.InvariantCulture.GetHashCode(value.ToString());

                case StringComparison.InvariantCultureIgnoreCase:
                    return StringComparer.InvariantCultureIgnoreCase.GetHashCode(value.ToString());
#endif
                default:
                    ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison, ExceptionArgument.comparisonType);
                    Debug.Fail("Should not reach this point.");
                    return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetHashCodeOrdinalIgnoreCase(ReadOnlySpan<char> value)
        {
            ulong seed = Marvin.DefaultSeed;
            return Marvin.ComputeHash32OrdinalIgnoreCase(ref MemoryMarshal.GetReference(value), value.Length /* in chars, not bytes */, (uint)seed, (uint)(seed >> 32));
        }

        // Important GetNonRandomizedHashCode{OrdinalIgnoreCase} notes:
        //
        // Use if and only if 'Denial of Service' attacks are not a concern (i.e. never used for free-form user input),
        // or are otherwise mitigated.
        //
        // The string-based implementation relies on System.String being null terminated. All reads are performed
        // two characters at a time, so for odd-length strings, the final read will include the null terminator.
        // This implementation must not be used as-is with spans, or otherwise arbitrary char refs/pointers, as
        // they're not guaranteed to be null-terminated.
        //
        // For spans, we must produce the exact same value as is used for strings: consumers like Dictionary<>
        // rely on str.GetNonRandomizedHashCode() == GetNonRandomizedHashCode(str.AsSpan()). As such, we must
        // restructure the comparison so that for odd-length spans, we simulate the null terminator and include
        // it in the hash computation exactly as does str.GetNonRandomizedHashCode().

        internal static unsafe int GetNonRandomizedHashCode(string value)
        {
            Debug.Assert(value != null, "Caller should skip this call and return 0 for null values. This method is set up to match string.Empty.GetHashCode() so it matches the ReadOnlySpan<char> overload.");

            fixed (char* src = value)
            {
                Debug.Assert(src[value!.Length] == '\0', "src[Length] == '\\0'");
                Debug.Assert(((int)src) % 4 == 0, "Managed string should start at 4 bytes boundary");

                uint hash1 = (5381 << 16) + 5381;
                uint hash2 = hash1;

                uint* ptr = (uint*)src;
                int length = value.Length;

                while (length > 2)
                {
                    length -= 4;
                    hash1 = (BitOperation.RotateLeft(hash1, 5) + hash1) ^ ptr[0];
                    hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ ptr[1];
                    ptr += 2;
                }

                if (length > 0)
                {
                    hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ ptr[0];
                }

                return (int)(hash1 + (hash2 * 1566083941));
            }
        }

        internal static unsafe int GetNonRandomizedHashCode(this ReadOnlySpan<char> span)
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

        // We "normalize to lowercase" every char by ORing with 0x0020. This casts
        // a very wide net because it will change, e.g., '^' to '~'. But that should
        // be ok because we expect this to be very rare in practice. These are valid
        // for both for big-endian and for little-endian.
        private const uint NormalizeToLowercase = 0x0020_0020u;


        internal static unsafe int GetNonRandomizedHashCodeOrdinalIgnoreCase(string value)
        {
            Debug.Assert(value != null, "Caller should skip this call and return 0 for null values. This method is set up to match string.Empty.GetHashCode() so it matches the ReadOnlySpan<char> overload.");

            uint hash1 = (5381 << 16) + 5381;
            uint hash2 = hash1;

            int length = value!.Length;
            fixed (char* src = value)
            {
                Debug.Assert(src[value.Length] == '\0', "src[this.Length] == '\\0'");
                Debug.Assert(((int)src) % 4 == 0, "Managed string should start at 4 bytes boundary");

                uint* ptr = (uint*)src;

                while (length > 2)
                {
                    uint p0 = ptr[0];
                    uint p1 = ptr[1];
                    if (!Utf16Utility.AllCharsInUInt32AreAscii(p0 | p1))
                    {
                        goto NotAscii;
                    }

                    length -= 4;
                    hash1 = (BitOperation.RotateLeft(hash1, 5) + hash1) ^ (p0 | NormalizeToLowercase);
                    hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (p1 | NormalizeToLowercase);
                    ptr += 2;
                }

                if (length > 0)
                {
                    uint p0 = ptr[0];
                    if (!Utf16Utility.AllCharsInUInt32AreAscii(p0))
                    {
                        goto NotAscii;
                    }

                    hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (p0 | NormalizeToLowercase);
                }
            }

            return (int)(hash1 + (hash2 * 1566083941));

        NotAscii:
            return GetNonRandomizedHashCodeOrdinalIgnoreCaseSlow(hash1, hash2, value.AsSpan(value.Length - length));
        }

        internal static unsafe int GetNonRandomizedHashCodeOrdinalIgnoreCase(ReadOnlySpan<char> span)
        {
            uint hash1 = (5381 << 16) + 5381;
            uint hash2 = hash1;

            uint p0, p1;
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
                            p0 = Unsafe.ReadUnaligned<uint>(ptr);
                            p1 = Unsafe.ReadUnaligned<uint>(ptr + 1);
                            if (!Utf16Utility.AllCharsInUInt32AreAscii(p0 | p1))
                            {
                                goto NotAscii;
                            }

                            length -= 4;
                            hash1 = (BitOperation.RotateLeft(hash1, 5) + hash1) ^ (p0 | NormalizeToLowercase);
                            hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (p1 | NormalizeToLowercase);
                            ptr += 2;
                        }
                        while (length >= 4);
                        goto LengthSwitch;

                    case 3:
                        p0 = Unsafe.ReadUnaligned<uint>(ptr);
                        p1 = *(char*)(ptr + 1);
                        if (!BitConverter.IsLittleEndian)
                        {
                            p1 <<= 16;
                        }

                        if (!Utf16Utility.AllCharsInUInt32AreAscii(p0 | p1))
                        {
                            goto NotAscii;
                        }

                        hash1 = (BitOperation.RotateLeft(hash1, 5) + hash1) ^ (p0 | NormalizeToLowercase);
                        hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (p1 | NormalizeToLowercase);
                        break;

                    case 2:
                        p0 = Unsafe.ReadUnaligned<uint>(ptr);
                        if (!Utf16Utility.AllCharsInUInt32AreAscii(p0))
                        {
                            goto NotAscii;
                        }

                        hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (p0 | NormalizeToLowercase);
                        break;

                    case 1:
                        p0 = *(char*)ptr;
                        if (!BitConverter.IsLittleEndian)
                        {
                            p0 <<= 16;
                        }

                        if (p0 > 0x7f)
                        {
                            goto NotAscii;
                        }

                        hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (p0 | NormalizeToLowercase);
                        break;

                    case 0:
                        break;
                }
            }

            return (int)(hash1 + (hash2 * 1566083941));

        NotAscii:
            return GetNonRandomizedHashCodeOrdinalIgnoreCaseSlow(hash1, hash2, span.Slice(span.Length - length));
        }

        private static unsafe int GetNonRandomizedHashCodeOrdinalIgnoreCaseSlow(uint hash1, uint hash2, ReadOnlySpan<char> str)
        {
            int length = str.Length;

            // We allocate one char more than the length to accommodate a null terminator.
            // That lets the reading always be performed two characters at a time, as odd-length
            // inputs will have a final terminator to backstop the last read.
            char[]? borrowedArr = null;
            Span<char> scratch = (uint)length < 256 ?
                stackalloc char[256] :
                (borrowedArr = ArrayPool<char>.Shared.Rent(length + 1));

            int charsWritten = Ordinal.ToUpperOrdinal(str, scratch);
            Debug.Assert(charsWritten == length);
            scratch[length] = '\0';

            // Duplicate the main loop, can be removed once JIT gets "Loop Unswitching" optimization
            fixed (char* src = scratch)
            {
                uint* ptr = (uint*)src;
                while (length > 2)
                {
                    length -= 4;
                    hash1 = (BitOperation.RotateLeft(hash1, 5) + hash1) ^ (ptr[0] | NormalizeToLowercase);
                    hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (ptr[1] | NormalizeToLowercase);
                    ptr += 2;
                }

                if (length > 0)
                {
                    hash2 = (BitOperation.RotateLeft(hash2, 5) + hash2) ^ (ptr[0] | NormalizeToLowercase);
                }
            }

            if (borrowedArr != null)
            {
                ArrayPool<char>.Shared.Return(borrowedArr);
            }

            return (int)(hash1 + (hash2 * 1566083941));
        }
    }
}
