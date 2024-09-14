// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Buffers;
using J2N.Text;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Numerics
{
    using SR = J2N.Resources.Strings;

    /// <summary>Methods for parsing numbers and strings.</summary>
    internal static class ParseNumbers
    {
        private const int CharStackBufferSize = 32;

        internal const int LeftAlign = 0x0001;
        internal const int RightAlign = 0x0004;
        internal const int PrefixSpace = 0x0008;
        internal const int PrintSign = 0x0010;
        internal const int PrintBase = 0x0020;
        internal const int PrintAsI1 = 0x0040;
        internal const int PrintAsI2 = 0x0080;
        internal const int PrintAsI4 = 0x0100;
        internal const int TreatAsUnsigned = 0x0200;
        internal const int TreatAsI1 = 0x0400;
        internal const int TreatAsI2 = 0x0800;
        internal const int IsTight = 0x1000;
        internal const int NoSpace = 0x2000;
        internal const int PrintRadixBase = 0x4000;

        #region StringToLong

        public static long StringToLong(ReadOnlySpan<char> s, int radix, int flags) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int pos = 0;
            return StringToLong(s, radix, flags, sign: 1, ref pos, s.Length - pos);
        }

        public static long StringToLong(ReadOnlySpan<char> s, int radix, int flags, int sign, ref int currPos, int length) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int i = currPos;

            // Do some bounds checking.
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                throw new ArgumentOutOfRangeException(nameof(radix), radix, SR.ArgumentOutOfRange_Radix);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (i > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length == 0)
                throw new FormatException(SR.Format_EmptyInputString); // J2N specific - deviating from .NET which throws ArgumentOutOfRange here because of inconsistent behavior with long.Parse()

            if (i < 0)
                throw new ArgumentOutOfRangeException(nameof(currPos), currPos, SR.ArgumentOutOfRange_Index);

            int end = i + length; // Calculate the exclusive end index now, so we don't lose track when we increment i later

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            if (((flags & IsTight) == 0) && ((flags & NoSpace) == 0))
            {
                EatWhiteSpace(s, ref i);
                if (i == end)
                    throw new FormatException(SR.Format_EmptyInputString);
            }

            // Check for a sign
            if (s[i] == '-')
            {
                if ((flags & TreatAsUnsigned) != 0)
                    throw new OverflowException(SR.Overflow_NegativeUnsigned);

                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            int grabNumbersStart = i;
            if (!TryGrabLongs(radix, s, ref i, end, isUnsigned: (flags & TreatAsUnsigned) != 0, out long result))
            {
                DotNetNumber.ThrowOverflowException(GetLongOverflowTypeCode(flags));
            }

            // Check if they passed us a string with no parsable digits.
            if (i == grabNumbersStart)
                throw new FormatException(SR.Format_NoParsibleDigits);

            if ((flags & IsTight) != 0)
            {
                // If we've got effluvia left at the end of the string, complain.
                if (i < end)
                    throw new FormatException(SR.Format_ExtraJunkAtEnd);
            }

            // Put the current index back into the correct place.
            currPos = i;

            if (IsLongOverflow(result, radix, flags, sign))
                DotNetNumber.ThrowOverflowException(GetLongOverflowTypeCode(flags));

            // Return the value properly signed.
            result *= sign;
            return result;
        }

        public static long StringToLong(ICharSequence s, int radix, int flags) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int pos = 0;
            return StringToLong(s, radix, flags, sign: 1, ref pos, s.Length - pos);
        }

        public static long StringToLong(ICharSequence s, int radix, int flags, int sign, ref int currPos, int length) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int i = currPos;

            // Do some bounds checking.
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                throw new ArgumentOutOfRangeException(nameof(radix), radix, SR.ArgumentOutOfRange_Radix);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (i > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length == 0)
                throw new FormatException(SR.Format_EmptyInputString); // J2N specific - deviating from .NET which throws ArgumentOutOfRange here because of inconsistent behavior with long.Parse()

            if (i < 0)
                throw new ArgumentOutOfRangeException(nameof(currPos), currPos, SR.ArgumentOutOfRange_Index);

            int end = i + length; // Calculate the exclusive end index now, so we don't lose track when we increment i later

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            if (((flags & IsTight) == 0) && ((flags & NoSpace) == 0))
            {
                EatWhiteSpace(s, ref i);
                if (i == end)
                    throw new FormatException(SR.Format_EmptyInputString);
            }

            // Check for a sign
            if (s[i] == '-')
            {
                if ((flags & TreatAsUnsigned) != 0)
                    throw new OverflowException(SR.Overflow_NegativeUnsigned);

                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            int grabNumbersStart = i;
            if (!TryGrabLongs(radix, s, ref i, end, isUnsigned: (flags & TreatAsUnsigned) != 0, out long result))
            {
                DotNetNumber.ThrowOverflowException(GetLongOverflowTypeCode(flags));
            }

            // Check if they passed us a string with no parsable digits.
            if (i == grabNumbersStart)
                throw new FormatException(SR.Format_NoParsibleDigits);

            if ((flags & IsTight) != 0)
            {
                // If we've got effluvia left at the end of the string, complain.
                if (i < end)
                    throw new FormatException(SR.Format_ExtraJunkAtEnd);
            }

            // Put the current index back into the correct place.
            currPos = i;

            if (IsLongOverflow(result, radix, flags, sign))
                DotNetNumber.ThrowOverflowException(GetLongOverflowTypeCode(flags));

            // Return the value properly signed.
            result *= sign;
            return result;
        }

        public static long StringToLong(StringBuilder s, int radix, int flags)
        {
            int pos = 0;
            return StringToLong(s, radix, flags, sign: 1, ref pos, s.Length - pos);
        }

        public static long StringToLong(StringBuilder s, int radix, int flags, int sign, ref int currPos, int length)
        {
#if FEATURE_STRINGBUILDER_GETCHUNKS
            if (s.TryAsSpan(currPos, length, out ReadOnlySpan<char> chunk))
            {
                int pos = 0;
                long result = StringToLong(chunk, radix, flags, sign, ref pos, length);
                currPos += pos;
                return result;
            }
#endif

            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> sSpan = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                s.CopyTo(currPos, sSpan, length);
#else
                Span<char> sSpan = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                s.CopyTo(currPos, arrayToReturnToPool, 0, length);
#endif
                int pos = 0;
                long result = StringToLong(sSpan.Slice(0, length), radix, flags, sign, ref pos, length);
                currPos += pos;
                return result;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        #endregion StringToLong

        #region TryStringToLong

        public static bool TryStringToLong(ReadOnlySpan<char> s, int radix, int flags, out long result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int pos = 0;
            return TryStringToLong(s, radix, flags, sign: 1, ref pos, s.Length - pos, out result);
        }

        public static bool TryStringToLong(ReadOnlySpan<char> s, int radix, int flags, int sign, ref int currPos, int length, out long result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            result = default;

            int i = currPos;

            // Do some bounds checking.
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                throw new ArgumentOutOfRangeException(nameof(radix), SR.ArgumentOutOfRange_Radix);
            if (i < 0)
                throw new ArgumentOutOfRangeException(nameof(currPos), currPos, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (i > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length == 0)
                return false;

            int end = i + length; // Calculate the exclusive end index now, so we don't lose track when we increment i later

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            if (((flags & IsTight) == 0) && ((flags & NoSpace) == 0))
            {
                EatWhiteSpace(s, ref i);
                if (i == end)
                    return false;
            }

            // Check for a sign
            if (s[i] == '-')
            {
                if ((flags & TreatAsUnsigned) != 0)
                    return false;

                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            int grabNumbersStart = i;
            if (!TryGrabLongs(radix, s, ref i, end, isUnsigned: (flags & TreatAsUnsigned) != 0, out result))
            {
                result = default;
                return false;
            }

            // Check if they passed us a string with no parsable digits.
            if (i == grabNumbersStart)
            {
                result = default;
                return false;
            }

            if ((flags & IsTight) != 0)
            {
                // If we've got effluvia left at the end of the string, complain.
                if (i < end)
                {
                    result = default;
                    return false;
                }
            }

            // Put the current index back into the correct place.
            currPos = i;

            if (IsLongOverflow(result, radix, flags, sign))
            {
                result = default;
                return false;
            }

            // Return the value properly signed.
            result *= sign;
            return true;
        }

        public static bool TryStringToLong(ICharSequence s, int radix, int flags, out long result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int pos = 0;
            return TryStringToLong(s, radix, flags, sign: 1, ref pos, s.Length - pos, out result);
        }

        public static bool TryStringToLong(ICharSequence s, int radix, int flags, int sign, ref int currPos, int length, out long result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            result = default;

            int i = currPos;

            // Do some bounds checking.
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                throw new ArgumentOutOfRangeException(nameof(radix), SR.ArgumentOutOfRange_Radix);
            if (i < 0)
                throw new ArgumentOutOfRangeException(nameof(currPos), currPos, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (i > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length == 0)
                return false;

            int end = i + length; // Calculate the exclusive end index now, so we don't lose track when we increment i later

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            if (((flags & IsTight) == 0) && ((flags & NoSpace) == 0))
            {
                EatWhiteSpace(s, ref i);
                if (i == end)
                    return false;
            }

            // Check for a sign
            if (s[i] == '-')
            {
                if ((flags & TreatAsUnsigned) != 0)
                    return false;

                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            int grabNumbersStart = i;
            if (!TryGrabLongs(radix, s, ref i, end, isUnsigned: (flags & TreatAsUnsigned) != 0, out result))
            {
                result = default;
                return false;
            }

            // Check if they passed us a string with no parsable digits.
            if (i == grabNumbersStart)
            {
                result = default;
                return false;
            }

            if ((flags & IsTight) != 0)
            {
                // If we've got effluvia left at the end of the string, complain.
                if (i < end)
                {
                    result = default;
                    return false;
                }
            }

            // Put the current index back into the correct place.
            currPos = i;

            if (IsLongOverflow(result, radix, flags, sign))
            {
                result = default;
                return false;
            }

            // Return the value properly signed.
            result *= sign;
            return true;
        }

        public static bool TryStringToLong(StringBuilder s, int radix, int flags, out long result)
        {
            int pos = 0;
            return TryStringToLong(s, radix, flags, sign: 1, ref pos, s.Length - pos, out result);
        }

        public static bool TryStringToLong(StringBuilder s, int radix, int flags, int sign, ref int currPos, int length, out long result)
        {
#if FEATURE_STRINGBUILDER_GETCHUNKS
            if (s.TryAsSpan(currPos, length, out ReadOnlySpan<char> chunk))
            {
                int pos = 0;
                bool success = TryStringToLong(chunk, radix, flags, sign, ref pos, length, out result);
                currPos += pos;
                return success;
            }
#endif

            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> sSpan = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                s.CopyTo(currPos, sSpan, length);
#else
                Span<char> sSpan = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                s.CopyTo(currPos, arrayToReturnToPool, 0, length);
#endif
                int pos = 0;
                bool success = TryStringToLong(sSpan.Slice(0, length), radix, flags, sign, ref pos, length, out result);
                currPos += pos;
                return success;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        #endregion TryStringToLong

        #region StringToInt

        public static int StringToInt(ReadOnlySpan<char> s, int radix, int flags) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int pos = 0;
            return StringToInt(s, radix, flags, sign: 1, ref pos, s.Length - pos);
        }

        public static int StringToInt(ReadOnlySpan<char> s, int radix, int flags, int sign, ref int currPos, int length) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            // They're requied to tell me where to start parsing.
            int i = currPos;

            // Do some bounds checking.
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                throw new ArgumentOutOfRangeException(nameof(radix), SR.ArgumentOutOfRange_Radix);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (i > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length == 0)
                throw new FormatException(SR.Format_EmptyInputString); // J2N specific - deviating from .NET which throws ArgumentOutOfRange here because of inconsistent behavior with long.Parse()

            if (i < 0)
                throw new ArgumentOutOfRangeException(nameof(currPos), currPos, SR.ArgumentOutOfRange_Index);

            int end = i + length; // Calculate the exclusive end index now, so we don't lose track when we increment i later

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            if (((flags & IsTight) == 0) && ((flags & NoSpace) == 0))
            {
                EatWhiteSpace(s, ref i);
                if (i == end)
                    throw new FormatException(SR.Format_EmptyInputString);
            }

            // Check for a sign
            if (s[i] == '-')
            {
                if ((flags & TreatAsUnsigned) != 0)
                    throw new OverflowException(SR.Overflow_NegativeUnsigned);

                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            int grabNumbersStart = i;
            if (!TryGrabInts(radix, s, ref i, end, isUnsigned: (flags & TreatAsUnsigned) != 0, out int result))
            {
                DotNetNumber.ThrowOverflowException(GetIntOverflowTypeCode(flags));
            }

            // Check if they passed us a string with no parsable digits.
            if (i == grabNumbersStart)
                throw new FormatException(SR.Format_NoParsibleDigits);

            if ((flags & IsTight) != 0)
            {
                // If we've got effluvia left at the end of the string, complain.
                if (i < end)
                    throw new FormatException(SR.Format_ExtraJunkAtEnd);
            }

            // Put the current index back into the correct place.
            currPos = i;

            if (IsIntOverflow(result, radix, flags, sign))
                DotNetNumber.ThrowOverflowException(GetIntOverflowTypeCode(flags));

            // Return the value properly signed.
            result *= sign;
            return result;
        }

        public static int StringToInt(ICharSequence s, int radix, int flags) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int pos = 0;
            return StringToInt(s, radix, flags, sign: 1, ref pos, s.Length - pos);
        }

        public static int StringToInt(ICharSequence s, int radix, int flags, int sign, ref int currPos, int length) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            // They're requied to tell me where to start parsing.
            int i = currPos;

            // Do some bounds checking.
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                throw new ArgumentOutOfRangeException(nameof(radix), SR.ArgumentOutOfRange_Radix);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (i > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length == 0)
                throw new FormatException(SR.Format_EmptyInputString); // J2N specific - deviating from .NET which throws ArgumentOutOfRange here because of inconsistent behavior with long.Parse()

            if (i < 0)
                throw new ArgumentOutOfRangeException(nameof(currPos), currPos, SR.ArgumentOutOfRange_Index);

            int end = i + length; // Calculate the exclusive end index now, so we don't lose track when we increment i later

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            if (((flags & IsTight) == 0) && ((flags & NoSpace) == 0))
            {
                EatWhiteSpace(s, ref i);
                if (i == end)
                    throw new FormatException(SR.Format_EmptyInputString);
            }

            // Check for a sign
            if (s[i] == '-')
            {
                if ((flags & TreatAsUnsigned) != 0)
                    throw new OverflowException(SR.Overflow_NegativeUnsigned);

                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            int grabNumbersStart = i;
            if (!TryGrabInts(radix, s, ref i, end, isUnsigned: (flags & TreatAsUnsigned) != 0, out int result))
            {
                DotNetNumber.ThrowOverflowException(GetIntOverflowTypeCode(flags));
            }

            // Check if they passed us a string with no parsable digits.
            if (i == grabNumbersStart)
                throw new FormatException(SR.Format_NoParsibleDigits);

            if ((flags & IsTight) != 0)
            {
                // If we've got effluvia left at the end of the string, complain.
                if (i < end)
                    throw new FormatException(SR.Format_ExtraJunkAtEnd);
            }

            // Put the current index back into the correct place.
            currPos = i;

            if (IsIntOverflow(result, radix, flags, sign))
                DotNetNumber.ThrowOverflowException(GetIntOverflowTypeCode(flags));

            // Return the value properly signed.
            result *= sign;
            return result;
        }

        public static int StringToInt(StringBuilder s, int radix, int flags)
        {
            int pos = 0;
            return StringToInt(s, radix, flags, sign: 1, ref pos, s.Length - pos);
        }

        public static int StringToInt(StringBuilder s, int radix, int flags, int sign, ref int currPos, int length)
        {
#if FEATURE_STRINGBUILDER_GETCHUNKS
            if (s.TryAsSpan(currPos, length, out ReadOnlySpan<char> chunk))
            {
                int pos = 0;
                int result = StringToInt(chunk, radix, flags, sign, ref pos, length);
                currPos += pos;
                return result;
            }
#endif

            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> sSpan = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                s.CopyTo(currPos, sSpan, length);
#else
                Span<char> sSpan = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                s.CopyTo(currPos, arrayToReturnToPool, 0, length);
#endif
                int pos = 0;
                int result = StringToInt(sSpan.Slice(0, length), radix, flags, sign, ref pos, length);
                currPos += pos;
                return result;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        #endregion StringToInt

        #region TryStringToInt

        public static bool TryStringToInt(ReadOnlySpan<char> s, int radix, int flags, out int result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int pos = 0;
            return TryStringToInt(s, radix, flags, sign: 1, ref pos, s.Length - pos, out result);
        }

        public static bool TryStringToInt(ReadOnlySpan<char> s, int radix, int flags, int sign, ref int currPos, int length, out int result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            result = default;

            // They're requied to tell me where to start parsing.
            int i = currPos;

            // Do some bounds checking.
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                throw new ArgumentOutOfRangeException(nameof(radix), SR.ArgumentOutOfRange_Radix);
            if (i < 0)
                throw new ArgumentOutOfRangeException(nameof(currPos), currPos, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (i > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length == 0)
                return false;

            int end = i + length; // Calculate the exclusive end index now, so we don't lose track when we increment i later

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            if (((flags & IsTight) == 0) && ((flags & NoSpace) == 0))
            {
                EatWhiteSpace(s, ref i);
                if (i == end)
                    return false;
            }

            // Check for a sign
            if (s[i] == '-')
            {
                if ((flags & TreatAsUnsigned) != 0)
                    return false;

                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            int grabNumbersStart = i;
            bool isUnsigned = (flags & TreatAsUnsigned) != 0;
            if (!TryGrabInts(radix, s, ref i, end, isUnsigned, out result))
            {
                result = default;
                return false;
            }

            // Check if they passed us a string with no parsable digits.
            if (i == grabNumbersStart)
            {
                result = default;
                return false;
            }

            if ((flags & IsTight) != 0)
            {
                // If we've got effluvia left at the end of the string, complain.
                if (i < end)
                {
                    result = default;
                    return false;
                }
            }

            // Put the current index back into the correct place.
            currPos = i;

            if (IsIntOverflow(result, radix, flags, sign))
            {
                result = default;
                return false;
            }

            // Return the value properly signed.
            result *= sign;
            return true;
        }

        public static bool TryStringToInt(ICharSequence s, int radix, int flags, out int result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            int pos = 0;
            return TryStringToInt(s, radix, flags, sign: 1, ref pos, s.Length - pos, out result);
        }

        public static bool TryStringToInt(ICharSequence s, int radix, int flags, int sign, ref int currPos, int length, out int result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC, KEEP Try... versions IN SYNC
        {
            result = default;

            // They're requied to tell me where to start parsing.
            int i = currPos;

            // Do some bounds checking.
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                throw new ArgumentOutOfRangeException(nameof(radix), SR.ArgumentOutOfRange_Radix);
            if (i < 0)
                throw new ArgumentOutOfRangeException(nameof(currPos), currPos, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (i > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length == 0)
                return false;

            int end = i + length; // Calculate the exclusive end index now, so we don't lose track when we increment i later

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            if (((flags & IsTight) == 0) && ((flags & NoSpace) == 0))
            {
                EatWhiteSpace(s, ref i);
                if (i == end)
                    return false;
            }

            // Check for a sign
            if (s[i] == '-')
            {
                if ((flags & TreatAsUnsigned) != 0)
                    return false;

                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            int grabNumbersStart = i;
            bool isUnsigned = (flags & TreatAsUnsigned) != 0;
            if (!TryGrabInts(radix, s, ref i, end, isUnsigned, out result))
            {
                result = default;
                return false;
            }

            // Check if they passed us a string with no parsable digits.
            if (i == grabNumbersStart)
            {
                result = default;
                return false;
            }

            if ((flags & IsTight) != 0)
            {
                // If we've got effluvia left at the end of the string, complain.
                if (i < end)
                {
                    result = default;
                    return false;
                }
            }

            // Put the current index back into the correct place.
            currPos = i;

            if (IsIntOverflow(result, radix, flags, sign))
            {
                result = default;
                return false;
            }

            // Return the value properly signed.
            result *= sign;
            return true;
        }

        public static bool TryStringToInt(StringBuilder s, int radix, int flags, out int result)
        {
            int pos = 0;
            return TryStringToInt(s, radix, flags, sign: 1, ref pos, s.Length - pos, out result);
        }

        public static bool TryStringToInt(StringBuilder s, int radix, int flags, int sign, ref int currPos, int length, out int result)
        {
#if FEATURE_STRINGBUILDER_GETCHUNKS
            if (s.TryAsSpan(currPos, length, out ReadOnlySpan<char> chunk))
            {
                int pos = 0;
                bool success = TryStringToInt(chunk, radix, flags, sign, ref pos, length, out result);
                currPos += pos;
                return success;
            }
#endif

            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> sSpan = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                s.CopyTo(currPos, sSpan, length);
#else
                Span<char> sSpan = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                s.CopyTo(currPos, arrayToReturnToPool, 0, length);
#endif
                int pos = 0;
                bool success = TryStringToInt(sSpan.Slice(0, length), radix, flags, sign, ref pos, length, out result);
                currPos += pos;
                return success;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        #endregion TryStringToInt

        #region IntToString

        //public static string IntToString(int n, int radix, int width, char paddingChar, int flags)
        //{
        //    Span<char> buffer = stackalloc char[66]; // Longest possible string length for an integer in binary notation with prefix

        //    if (radix < MinRadix || radix > MaxRadix)
        //        throw new ArgumentException(SR.Arg_InvalidBase, nameof(radix));

        //    // If the number is negative, make it positive and remember the sign.
        //    // If the number is MIN_VALUE, this will still be negative, so we'll have to
        //    // special case this later.
        //    bool isNegative = false;
        //    uint l;
        //    if (n < 0)
        //    {
        //        isNegative = true;

        //        // For base 10, write out -num, but other bases write out the
        //        // 2's complement bit pattern
        //        l = (10 == radix) ? (uint)-n : (uint)n;
        //    }
        //    else
        //    {
        //        l = (uint)n;
        //    }

        //    // The conversion to a uint will sign extend the number.  In order to ensure
        //    // that we only get as many bits as we expect, we chop the number.
        //    if ((flags & PrintAsI1) != 0)
        //    {
        //        l &= 0xFF;
        //    }
        //    else if ((flags & PrintAsI2) != 0)
        //    {
        //        l &= 0xFFFF;
        //    }

        //    // Special case the 0.
        //    int index;
        //    if (0 == l)
        //    {
        //        buffer[0] = '0';
        //        index = 1;
        //    }
        //    else
        //    {
        //        index = 0;
        //        for (int i = 0; i < buffer.Length; i++) // for (...;i<buffer.Length;...) loop instead of do{...}while(l!=0) to help JIT eliminate span bounds checks
        //        {
        //            uint div = l / (uint)radix; // TODO https://github.com/dotnet/runtime/issues/5213
        //            uint charVal = l - (div * (uint)radix);
        //            l = div;

        //            buffer[i] = (charVal < 10) ?
        //                (char)(charVal + '0') :
        //                (char)(charVal + 'a' - 10);

        //            if (l == 0)
        //            {
        //                index = i + 1;
        //                break;
        //            }
        //        }

        //        Debug.Assert(l == 0, $"Expected {l} == 0");
        //    }

        //    // If they want the base, append that to the string (in reverse order)
        //    if (radix != 10 && ((flags & PrintBase) != 0))
        //    {
        //        if (16 == radix)
        //        {
        //            buffer[index++] = 'x';
        //            buffer[index++] = '0';
        //        }
        //        else if (8 == radix)
        //        {
        //            buffer[index++] = '0';
        //        }
        //    }

        //    if (10 == radix)
        //    {
        //        // If it was negative, append the sign, else if they requested, add the '+'.
        //        // If they requested a leading space, put it on.
        //        if (isNegative)
        //        {
        //            buffer[index++] = '-';
        //        }
        //        else if ((flags & PrintSign) != 0)
        //        {
        //            buffer[index++] = '+';
        //        }
        //        else if ((flags & PrefixSpace) != 0)
        //        {
        //            buffer[index++] = ' ';
        //        }
        //    }

        //    // Figure out the size of and allocate the resulting string
        //    string result = string.FastAllocateString(Math.Max(width, index));
        //    unsafe
        //    {
        //        // Put the characters into the string in reverse order.
        //        // Fill the remaining space, if there is any, with the correct padding character.
        //        fixed (char* resultPtr = result)
        //        {
        //            char* p = resultPtr;
        //            int padding = result.Length - index;

        //            if ((flags & LeftAlign) != 0)
        //            {
        //                for (int i = 0; i < padding; i++)
        //                {
        //                    *p++ = paddingChar;
        //                }

        //                for (int i = 0; i < index; i++)
        //                {
        //                    *p++ = buffer[index - i - 1];
        //                }
        //            }
        //            else
        //            {
        //                for (int i = 0; i < index; i++)
        //                {
        //                    *p++ = buffer[index - i - 1];
        //                }

        //                for (int i = 0; i < padding; i++)
        //                {
        //                    *p++ = paddingChar;
        //                }
        //            }

        //            Debug.Assert((p - resultPtr) == result.Length, $"Expected {p - resultPtr} == {result.Length}");
        //        }
        //    }
        //    return result;
        //}

        #endregion IntToString

        #region LongToString

        //public static string LongToString(long n, int radix, int width, char paddingChar, int flags)
        //{
        //    Span<char> buffer = stackalloc char[67]; // Longest possible string length for an integer in binary notation with prefix

        //    if (radix < MinRadix || radix > MaxRadix)
        //        throw new ArgumentException(SR.Arg_InvalidBase, nameof(radix));

        //    // If the number is negative, make it positive and remember the sign.
        //    ulong ul;
        //    bool isNegative = false;
        //    if (n < 0)
        //    {
        //        isNegative = true;

        //        // For base 10, write out -num, but other bases write out the
        //        // 2's complement bit pattern
        //        ul = (10 == radix) ? (ulong)(-n) : (ulong)n;
        //    }
        //    else
        //    {
        //        ul = (ulong)n;
        //    }

        //    if ((flags & PrintAsI1) != 0)
        //    {
        //        ul &= 0xFF;
        //    }
        //    else if ((flags & PrintAsI2) != 0)
        //    {
        //        ul &= 0xFFFF;
        //    }
        //    else if ((flags & PrintAsI4) != 0)
        //    {
        //        ul &= 0xFFFFFFFF;
        //    }

        //    // Special case the 0.
        //    int index;
        //    if (0 == ul)
        //    {
        //        buffer[0] = '0';
        //        index = 1;
        //    }
        //    else
        //    {
        //        index = 0;
        //        for (int i = 0; i < buffer.Length; i++) // for loop instead of do{...}while(l!=0) to help JIT eliminate span bounds checks
        //        {
        //            ulong div = ul / (ulong)radix; // TODO https://github.com/dotnet/runtime/issues/5213
        //            int charVal = (int)(ul - (div * (ulong)radix));
        //            ul = div;

        //            buffer[i] = (charVal < 10) ?
        //                (char)(charVal + '0') :
        //                (char)(charVal + 'a' - 10);

        //            if (ul == 0)
        //            {
        //                index = i + 1;
        //                break;
        //            }
        //        }
        //        Debug.Assert(ul == 0, $"Expected {ul} == 0");
        //    }

        //    // If they want the base, append that to the string (in reverse order)
        //    if (radix != 10 && ((flags & PrintBase) != 0))
        //    {
        //        if (16 == radix)
        //        {
        //            buffer[index++] = 'x';
        //            buffer[index++] = '0';
        //        }
        //        else if (8 == radix)
        //        {
        //            buffer[index++] = '0';
        //        }
        //        else if ((flags & PrintRadixBase) != 0)
        //        {
        //            buffer[index++] = '#';
        //            buffer[index++] = (char)((radix % 10) + '0');
        //            buffer[index++] = (char)((radix / 10) + '0');
        //        }
        //    }

        //    if (10 == radix)
        //    {
        //        // If it was negative, append the sign.
        //        if (isNegative)
        //        {
        //            buffer[index++] = '-';
        //        }

        //        // else if they requested, add the '+';
        //        else if ((flags & PrintSign) != 0)
        //        {
        //            buffer[index++] = '+';
        //        }

        //        // If they requested a leading space, put it on.
        //        else if ((flags & PrefixSpace) != 0)
        //        {
        //            buffer[index++] = ' ';
        //        }
        //    }

        //    // Figure out the size of and allocate the resulting string
        //    string result = string.FastAllocateString(Math.Max(width, index));
        //    unsafe
        //    {
        //        // Put the characters into the string in reverse order.
        //        // Fill the remaining space, if there is any, with the correct padding character.
        //        fixed (char* resultPtr = result)
        //        {
        //            char* p = resultPtr;
        //            int padding = result.Length - index;

        //            if ((flags & LeftAlign) != 0)
        //            {
        //                for (int i = 0; i < padding; i++)
        //                {
        //                    *p++ = paddingChar;
        //                }

        //                for (int i = 0; i < index; i++)
        //                {
        //                    *p++ = buffer[index - i - 1];
        //                }
        //            }
        //            else
        //            {
        //                for (int i = 0; i < index; i++)
        //                {
        //                    *p++ = buffer[index - i - 1];
        //                }

        //                for (int i = 0; i < padding; i++)
        //                {
        //                    *p++ = paddingChar;
        //                }
        //            }

        //            Debug.Assert((p - resultPtr) == result.Length, $"Expected {p - resultPtr} == {result.Length}");
        //        }
        //    }
        //    return result;
        //}

        #endregion LongToString

        #region EatWhiteSpace

        private static void EatWhiteSpace(ReadOnlySpan<char> s, ref int i) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC
        {
            int localIndex = i;
            for (; localIndex < s.Length && char.IsWhiteSpace(s[localIndex]); localIndex++) ;
            i = localIndex;
        }

        private static void EatWhiteSpace(ICharSequence s, ref int i) // KEEP OVERLOADS FOR ICharSequence and  ReadOnlySpan<char> IN SYNC
        {
            int localIndex = i;
            for (; localIndex < s.Length && char.IsWhiteSpace(s[localIndex]); localIndex++) ;
            i = localIndex;
        }

        #endregion EatWhiteSpace

        #region TryGrabLongs

        private static bool TryGrabLongs(int radix, ReadOnlySpan<char> s, ref int i, int end, bool isUnsigned, out long result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC
        {
            ulong unsignedResult = 0;
            ulong maxVal;

            // Allow all non-decimal numbers to set the sign bit.
            if (radix == 10 && !isUnsigned)
            {
                maxVal = 0x7FFFFFFFFFFFFFFF / 10;

                // Read all of the digits and convert to a number
                while (i < end && IsDigit(s, i, end, radix, out int value, out int charCount))
                {
                    // Check for overflows - this is sufficient & correct.
                    if (unsignedResult > maxVal || ((long)unsignedResult) < 0)
                    {
                        result = default;
                        return false;
                    }

                    unsignedResult = unsignedResult * (ulong)radix + (ulong)value;
                    i += charCount;
                }

                if ((long)unsignedResult < 0 && unsignedResult != 0x8000000000000000)
                {
                    result = default;
                    return false;
                }
            }
            else
            {
                Debug.Assert(radix >= Character.MinRadix && radix <= Character.MaxRadix);
                maxVal = 0xffffffffffffffff / (uint)radix;

                // Read all of the digits and convert to a number
                while (i < end && IsDigit(s, i, end, radix, out int value, out int charCount))
                {
                    // Check for overflows - this is sufficient & correct.
                    if (unsignedResult > maxVal)
                    {
                        result = default;
                        return false;
                    }

                    ulong temp = unsignedResult * (ulong)radix + (ulong)value;

                    if (temp < unsignedResult) // this means overflow as well
                    {
                        result = default;
                        return false;
                    }

                    unsignedResult = temp;
                    i += charCount;
                }
            }

            result = (long)unsignedResult;
            return true;
        }

        private static bool TryGrabLongs(int radix, ICharSequence s, ref int i, int end, bool isUnsigned, out long result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC
        {
            ulong unsignedResult = 0;
            ulong maxVal;

            // Allow all non-decimal numbers to set the sign bit.
            if (radix == 10 && !isUnsigned)
            {
                maxVal = 0x7FFFFFFFFFFFFFFF / 10;

                // Read all of the digits and convert to a number
                while (i < end && IsDigit(s, i, end, radix, out int value, out int charCount))
                {
                    // Check for overflows - this is sufficient & correct.
                    if (unsignedResult > maxVal || ((long)unsignedResult) < 0)
                    {
                        result = default;
                        return false;
                    }

                    unsignedResult = unsignedResult * (ulong)radix + (ulong)value;
                    i += charCount;
                }

                if ((long)unsignedResult < 0 && unsignedResult != 0x8000000000000000)
                {
                    result = default;
                    return false;
                }
            }
            else
            {
                Debug.Assert(radix >= Character.MinRadix && radix <= Character.MaxRadix);
                maxVal = 0xffffffffffffffff / (uint)radix;

                // Read all of the digits and convert to a number
                while (i < end && IsDigit(s, i, end, radix, out int value, out int charCount))
                {
                    // Check for overflows - this is sufficient & correct.
                    if (unsignedResult > maxVal)
                    {
                        result = default;
                        return false;
                    }

                    ulong temp = unsignedResult * (ulong)radix + (ulong)value;

                    if (temp < unsignedResult) // this means overflow as well
                    {
                        result = default;
                        return false;
                    }

                    unsignedResult = temp;
                    i += charCount;
                }
            }

            result = (long)unsignedResult;
            return true;
        }

        #endregion TryGrabLongs

        #region TryGrabInts

        private static bool TryGrabInts(int radix, ReadOnlySpan<char> s, ref int i, int end, bool isUnsigned, out int result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC
        {
            uint unsignedResult = 0;
            uint maxVal;

            // Allow all non-decimal numbers to set the sign bit.
            if (radix == 10 && !isUnsigned)
            {
                maxVal = (0x7FFFFFFF / 10);

                // Read all of the digits and convert to a number
                while (i < end && IsDigit(s, i, end, radix, out int value, out int charCount))
                {
                    // Check for overflows - this is sufficient & correct.
                    if (unsignedResult > maxVal || (int)unsignedResult < 0)
                    {
                        result = default;
                        return false;
                    }
                    unsignedResult = unsignedResult * (uint)radix + (uint)value;
                    i += charCount;
                }
                if ((int)unsignedResult < 0 && unsignedResult != 0x80000000)
                {
                    result = default;
                    return false;
                }
            }
            else
            {
                Debug.Assert(radix >= Character.MinRadix && radix <= Character.MaxRadix);
                maxVal = 0xffffffff / (uint)radix;

                // Read all of the digits and convert to a number
                while (i < end && IsDigit(s, i, end, radix, out int value, out int charCount))
                {
                    // Check for overflows - this is sufficient & correct.
                    if (unsignedResult > maxVal)
                    {
                        result = default;
                        return false;
                    }

                    uint temp = unsignedResult * (uint)radix + (uint)value;

                    if (temp < unsignedResult) // this means overflow as well
                    {
                        result = default;
                        return false;
                    }

                    unsignedResult = temp;
                    i += charCount;
                }
            }

            result = (int)unsignedResult;
            return true;
        }

        private static bool TryGrabInts(int radix, ICharSequence s, ref int i, int end, bool isUnsigned, out int result) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC
        {
            uint unsignedResult = 0;
            uint maxVal;

            // Allow all non-decimal numbers to set the sign bit.
            if (radix == 10 && !isUnsigned)
            {
                maxVal = (0x7FFFFFFF / 10);

                // Read all of the digits and convert to a number
                while (i < end && IsDigit(s, i, end, radix, out int value, out int charCount))
                {
                    // Check for overflows - this is sufficient & correct.
                    if (unsignedResult > maxVal || (int)unsignedResult < 0)
                    {
                        result = default;
                        return false;
                    }
                    unsignedResult = unsignedResult * (uint)radix + (uint)value;
                    i += charCount;
                }
                if ((int)unsignedResult < 0 && unsignedResult != 0x80000000)
                {
                    result = default;
                    return false;
                }
            }
            else
            {
                Debug.Assert(radix >= Character.MinRadix && radix <= Character.MaxRadix);
                maxVal = 0xffffffff / (uint)radix;

                // Read all of the digits and convert to a number
                while (i < end && IsDigit(s, i, end, radix, out int value, out int charCount))
                {
                    // Check for overflows - this is sufficient & correct.
                    if (unsignedResult > maxVal)
                    {
                        result = default;
                        return false;
                    }

                    uint temp = unsignedResult * (uint)radix + (uint)value;

                    if (temp < unsignedResult) // this means overflow as well
                    {
                        result = default;
                        return false;
                    }

                    unsignedResult = temp;
                    i += charCount;
                }
            }

            result = (int)unsignedResult;
            return true;
        }

        #endregion TryGrabInts

        #region IsLongOverflow

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLongOverflow(long value, int radix, int flags, int sign)
        {
            // Return the value properly signed.
            if ((ulong)value == 0x8000000000000000 && sign == 1 && radix == 10 && ((flags & TreatAsUnsigned) == 0))
                return true;

            // Don't allow both a sign in the string and a negative value in the string
            // except when result is the smallest negative long, which is a special case.
            else if ((ulong)value != 0x8000000000000000 && value < 0 && sign == -1 && ((flags & TreatAsUnsigned) == 0))
                return true;

            return false;
        }

        #endregion

        #region IsIntOverflow

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsIntOverflow(int value, int radix, int flags, int sign)
        {
            if ((flags & TreatAsI1) != 0 && ((flags & TreatAsUnsigned) != 0))
            {
                if ((uint)value > 0xFF)
                {
                    return true;
                }
            }
            else if ((flags & TreatAsI1) != 0 && (uint)value == 0x80 && radix == 10 && sign == 1 && ((flags & TreatAsUnsigned) == 0))
            {
                return true;
            }
            else if ((flags & TreatAsI1) != 0 && (uint)value != 0x80 && (sbyte)value < 0 && sign == -1 && ((flags & TreatAsUnsigned) == 0))
            {
                return true;
            }
            else if ((flags & TreatAsI2) != 0 && ((flags & TreatAsUnsigned) != 0))
            {
                if ((uint)value > 0xFFFF)
                {
                    return true;
                }
            }
            else if ((flags & TreatAsI2) != 0 && (uint)value == 0x8000 && radix == 10 && sign == 1 && ((flags & TreatAsUnsigned) == 0))
            {
                return true;
            }
            else if ((flags & TreatAsI2) != 0 && (uint)value != 0x8000 && (short)value < 0 && sign == -1 && ((flags & TreatAsUnsigned) == 0))
            {
                return true;
            }
            else if ((uint)value == 0x80000000 && sign == 1 && radix == 10 && ((flags & TreatAsUnsigned) == 0))
            {
                return true;
            }
            // Don't allow both a sign in the string and a negative value in the string
            // except when result is the smallest negative int, which is a special case.
            else if ((uint)value != 0x80000000 && value < 0 && sign == -1 && ((flags & TreatAsUnsigned) == 0))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region GetLongOverflowTypeCode

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TypeCode GetLongOverflowTypeCode(int flags)
        {
            return (flags & TreatAsUnsigned) != 0 ? TypeCode.UInt64 : TypeCode.Int64;
        }

        #endregion

        #region GetIntOverflowTypeCode

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TypeCode GetIntOverflowTypeCode(int flags)
        {
            bool isUnsigned = (flags & TreatAsUnsigned) != 0;
            TypeCode result = isUnsigned ? TypeCode.UInt32 : TypeCode.Int32;
            if ((flags & TreatAsI1) != 0)
                result = isUnsigned ? TypeCode.Byte : TypeCode.SByte;
            if ((flags & TreatAsI2) != 0)
                result = isUnsigned ? TypeCode.UInt16 : TypeCode.Int16;

            return result;
        }

        #endregion

        #region IsDigit

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(ReadOnlySpan<char> s, int i, int end, int radix, out int result, out int charCount) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC
        {
            if (char.IsHighSurrogate(s[i]) && i + 1 < end && char.IsLowSurrogate(s[i + 1]))
            {
                result = Character.Digit(Character.ToCodePoint(s[i++], s[i++]), radix);
                charCount = result == -1 ? 0 : 2;
                return result != -1;
            }
            result = Character.Digit(s[i++], radix);
            charCount = result == -1 ? 0 : 1;
            return result != -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(ICharSequence s, int i, int end, int radix, out int result, out int charCount) // KEEP OVERLOADS FOR ICharSequence and ReadOnlySpan<char> IN SYNC
        {
            if (char.IsHighSurrogate(s[i]) && i + 1 < end && char.IsLowSurrogate(s[i + 1]))
            {
                result = Character.Digit(Character.ToCodePoint(s[i++], s[i++]), radix);
                charCount = result == -1 ? 0 : 2;
                return result != -1;
            }
            result = Character.Digit(s[i++], radix);
            charCount = result == -1 ? 0 : 1;
            return result != -1;
        }

        #endregion IsDigit
    }
}