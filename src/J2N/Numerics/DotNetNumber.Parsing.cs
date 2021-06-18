// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Globalization;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Numerics
{
    using SR = J2N.Resources.Strings;

    // The Parse methods provided by the numeric classes convert a
    // string to a numeric value. The optional style parameter specifies the
    // permitted style of the numeric string. It must be a combination of bit flags
    // from the NumberStyle enumeration. The optional info parameter
    // specifies the NumberFormatInfo instance to use when parsing the
    // string. If the info parameter is null or omitted, the numeric
    // formatting information is obtained from the current culture.
    //
    // Numeric strings produced by the Format methods using the Currency,
    // Decimal, Engineering, Fixed point, General, or Number standard formats
    // (the C, D, E, F, G, and N format specifiers) are guaranteed to be parseable
    // by the Parse methods if the NumberStyle.Any style is
    // specified. Note, however, that the Parse methods do not accept
    // NaNs or Infinities.

    internal static partial class DotNetNumber
    {
        private const int Int32Precision = 10;
        private const int UInt32Precision = Int32Precision;
        private const int Int64Precision = 19;
        private const int UInt64Precision = 20;

        private const int DoubleMaxExponent = 309;
        private const int DoubleMinExponent = -324;

        private const int FloatingPointMaxExponent = DoubleMaxExponent;
        private const int FloatingPointMinExponent = DoubleMinExponent;

        private const int SingleMaxExponent = 39;
        private const int SingleMinExponent = -45;

        private const int HalfMaxExponent = 5;
        private const int HalfMinExponent = -8;

        //private static unsafe bool TryNumberToInt32(ref NumberBuffer number, ref int value)
        //{
        //    number.CheckConsistency();

        //    int i = number.Scale;
        //    if (i > Int32Precision || i < number.DigitsCount)
        //    {
        //        return false;
        //    }
        //    byte* p = number.GetDigitsPointer();
        //    Debug.Assert(p != null);
        //    int n = 0;
        //    while (--i >= 0)
        //    {
        //        if ((uint)n > (0x7FFFFFFF / 10))
        //        {
        //            return false;
        //        }
        //        n *= 10;
        //        if (*p != '\0')
        //        {
        //            n += (*p++ - '0');
        //        }
        //    }
        //    if (number.IsNegative)
        //    {
        //        n = -n;
        //        if (n > 0)
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (n < 0)
        //        {
        //            return false;
        //        }
        //    }
        //    value = n;
        //    return true;
        //}

        //private static unsafe bool TryNumberToInt64(ref NumberBuffer number, ref long value)
        //{
        //    number.CheckConsistency();

        //    int i = number.Scale;
        //    if (i > Int64Precision || i < number.DigitsCount)
        //    {
        //        return false;
        //    }
        //    byte* p = number.GetDigitsPointer();
        //    Debug.Assert(p != null);
        //    long n = 0;
        //    while (--i >= 0)
        //    {
        //        if ((ulong)n > (0x7FFFFFFFFFFFFFFF / 10))
        //        {
        //            return false;
        //        }
        //        n *= 10;
        //        if (*p != '\0')
        //        {
        //            n += (*p++ - '0');
        //        }
        //    }
        //    if (number.IsNegative)
        //    {
        //        n = -n;
        //        if (n > 0)
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (n < 0)
        //        {
        //            return false;
        //        }
        //    }
        //    value = n;
        //    return true;
        //}

        //private static unsafe bool TryNumberToUInt32(ref NumberBuffer number, ref uint value)
        //{
        //    number.CheckConsistency();

        //    int i = number.Scale;
        //    if (i > UInt32Precision || i < number.DigitsCount || number.IsNegative)
        //    {
        //        return false;
        //    }
        //    byte* p = number.GetDigitsPointer();
        //    Debug.Assert(p != null);
        //    uint n = 0;
        //    while (--i >= 0)
        //    {
        //        if (n > (0xFFFFFFFF / 10))
        //        {
        //            return false;
        //        }
        //        n *= 10;
        //        if (*p != '\0')
        //        {
        //            uint newN = n + (uint)(*p++ - '0');
        //            // Detect an overflow here...
        //            if (newN < n)
        //            {
        //                return false;
        //            }
        //            n = newN;
        //        }
        //    }
        //    value = n;
        //    return true;
        //}

        //private static unsafe bool TryNumberToUInt64(ref NumberBuffer number, ref ulong value)
        //{
        //    number.CheckConsistency();

        //    int i = number.Scale;
        //    if (i > UInt64Precision || i < number.DigitsCount || number.IsNegative)
        //    {
        //        return false;
        //    }
        //    byte* p = number.GetDigitsPointer();
        //    Debug.Assert(p != null);
        //    ulong n = 0;
        //    while (--i >= 0)
        //    {
        //        if (n > (0xFFFFFFFFFFFFFFFF / 10))
        //        {
        //            return false;
        //        }
        //        n *= 10;
        //        if (*p != '\0')
        //        {
        //            ulong newN = n + (ulong)(*p++ - '0');
        //            // Detect an overflow here...
        //            if (newN < n)
        //            {
        //                return false;
        //            }
        //            n = newN;
        //        }
        //    }
        //    value = n;
        //    return true;
        //}

        //internal static int ParseInt32(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info)
        //{
        //    ParsingStatus status = TryParseInt32(value, styles, info, out int result);
        //    if (status != ParsingStatus.OK)
        //    {
        //        ThrowOverflowOrFormatException(status, TypeCode.Int32);
        //    }

        //    return result;
        //}

        //internal static long ParseInt64(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info)
        //{
        //    ParsingStatus status = TryParseInt64(value, styles, info, out long result);
        //    if (status != ParsingStatus.OK)
        //    {
        //        ThrowOverflowOrFormatException(status, TypeCode.Int64);
        //    }

        //    return result;
        //}

        //internal static uint ParseUInt32(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info)
        //{
        //    ParsingStatus status = TryParseUInt32(value, styles, info, out uint result);
        //    if (status != ParsingStatus.OK)
        //    {
        //        ThrowOverflowOrFormatException(status, TypeCode.UInt32);
        //    }

        //    return result;
        //}

        //internal static ulong ParseUInt64(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info)
        //{
        //    ParsingStatus status = TryParseUInt64(value, styles, info, out ulong result);
        //    if (status != ParsingStatus.OK)
        //    {
        //        ThrowOverflowOrFormatException(status, TypeCode.UInt64);
        //    }

        //    return result;
        //}

        private static unsafe bool TryParseNumber(ref char* str, char* strEnd, NumberStyle styles, ref NumberBuffer number, NumberFormatInfo info)
        {
            Debug.Assert(str != null);
            Debug.Assert(strEnd != null);
            Debug.Assert(str <= strEnd);
            Debug.Assert((styles & NumberStyle.AllowHexSpecifier) == 0);

            const int StateSign = 0x0001;
            const int StateParens = 0x0002;
            const int StateDigits = 0x0004;
            const int StateNonZero = 0x0008;
            const int StateDecimal = 0x0010;
            const int StateCurrency = 0x0020;

            Debug.Assert(number.DigitsCount == 0);
            Debug.Assert(number.Scale == 0);
            Debug.Assert(!number.IsNegative);
            Debug.Assert(!number.HasNonZeroTail);

            number.CheckConsistency();

            string decSep;                  // decimal separator from NumberFormatInfo.
            string groupSep;                // group separator from NumberFormatInfo.
            string? currSymbol = null;       // currency symbol from NumberFormatInfo.

            bool parsingCurrency = false;
            if ((styles & NumberStyle.AllowCurrencySymbol) != 0)
            {
                currSymbol = info.CurrencySymbol;

                // The idea here is to match the currency separators and on failure match the number separators to keep the perf of VB's IsNumeric fast.
                // The values of decSep are setup to use the correct relevant separator (currency in the if part and decimal in the else part).
                decSep = info.CurrencyDecimalSeparator;
                groupSep = info.CurrencyGroupSeparator;
                parsingCurrency = true;
            }
            else
            {
                decSep = info.NumberDecimalSeparator;
                groupSep = info.NumberGroupSeparator;
            }

            int state = 0;
            char* p = str;
            char ch = p < strEnd ? *p : '\0';
            char* next;

            while (true)
            {
                // Eat whitespace unless we've found a sign which isn't followed by a currency symbol.
                // "-Kr 1231.47" is legal but "- 1231.47" is not.
                if (!IsWhite(ch) || (styles & NumberStyle.AllowLeadingWhite) == 0 || ((state & StateSign) != 0 && ((state & StateCurrency) == 0 && info.NumberNegativePattern != 2)))
                {
                    if ((((styles & NumberStyle.AllowLeadingSign) != 0) && (state & StateSign) == 0) && ((next = MatchChars(p, strEnd, info.PositiveSign)) != null || ((next = MatchChars(p, strEnd, info.NegativeSign)) != null && (number.IsNegative = true))))
                    {
                        state |= StateSign;
                        p = next - 1;
                    }
                    else if (ch == '(' && ((styles & NumberStyle.AllowParentheses) != 0) && ((state & StateSign) == 0))
                    {
                        state |= StateSign | StateParens;
                        number.IsNegative = true;
                    }
                    else if (currSymbol != null && (next = MatchChars(p, strEnd, currSymbol)) != null)
                    {
                        state |= StateCurrency;
                        currSymbol = null;
                        // We already found the currency symbol. There should not be more currency symbols. Set
                        // currSymbol to NULL so that we won't search it again in the later code path.
                        p = next - 1;
                    }
                    else
                    {
                        break;
                    }
                }
                ch = ++p < strEnd ? *p : '\0';
            }

            int digCount = 0;
            int digEnd = 0;
            int maxDigCount = number.Digits.Length - 1;

            while (true)
            {
                if (IsDigit(ch))
                {
                    state |= StateDigits;

                    if (ch != '0' || (state & StateNonZero) != 0)
                    {
                        if (digCount < maxDigCount)
                        {
                            number.Digits[digCount++] = (byte)(ch);
                            if ((ch != '0') || (number.Kind != NumberBufferKind.Integer))
                            {
                                digEnd = digCount;
                            }
                        }
                        else if (ch != '0')
                        {
                            // For decimal and binary floating-point numbers, we only
                            // need to store digits up to maxDigCount. However, we still
                            // need to keep track of whether any additional digits past
                            // maxDigCount were non-zero, as that can impact rounding
                            // for an input that falls evenly between two representable
                            // results.

                            number.HasNonZeroTail = true;
                        }

                        if ((state & StateDecimal) == 0)
                        {
                            number.Scale++;
                        }
                        state |= StateNonZero;
                    }
                    else if ((state & StateDecimal) != 0)
                    {
                        number.Scale--;
                    }
                }
                else if (((styles & NumberStyle.AllowDecimalPoint) != 0) && ((state & StateDecimal) == 0) && ((next = MatchChars(p, strEnd, decSep)) != null || (parsingCurrency && (state & StateCurrency) == 0) && (next = MatchChars(p, strEnd, info.NumberDecimalSeparator)) != null))
                {
                    state |= StateDecimal;
                    p = next - 1;
                }
                else if (((styles & NumberStyle.AllowThousands) != 0) && ((state & StateDigits) != 0) && ((state & StateDecimal) == 0) && ((next = MatchChars(p, strEnd, groupSep)) != null || (parsingCurrency && (state & StateCurrency) == 0) && (next = MatchChars(p, strEnd, info.NumberGroupSeparator)) != null))
                {
                    p = next - 1;
                }
                else
                {
                    break;
                }
                ch = ++p < strEnd ? *p : '\0';
            }

            bool negExp = false;
            number.DigitsCount = digEnd;
            number.Digits[digEnd] = (byte)('\0');
            if ((state & StateDigits) != 0)
            {
                if ((ch == 'E' || ch == 'e') && ((styles & NumberStyle.AllowExponent) != 0))
                {
                    char* temp = p;
                    ch = ++p < strEnd ? *p : '\0';
                    if ((next = MatchChars(p, strEnd, info.PositiveSign)) != null)
                    {
                        ch = (p = next) < strEnd ? *p : '\0';
                    }
                    else if ((next = MatchChars(p, strEnd, info.NegativeSign)) != null)
                    {
                        ch = (p = next) < strEnd ? *p : '\0';
                        negExp = true;
                    }
                    if (IsDigit(ch))
                    {
                        int exp = 0;
                        do
                        {
                            exp = exp * 10 + (ch - '0');
                            ch = ++p < strEnd ? *p : '\0';
                            if (exp > 1000)
                            {
                                exp = 9999;
                                while (IsDigit(ch))
                                {
                                    ch = ++p < strEnd ? *p : '\0';
                                }
                            }
                        } while (IsDigit(ch));
                        if (negExp)
                        {
                            exp = -exp;
                        }
                        number.Scale += exp;
                    }
                    else
                    {
                        p = temp;
                        ch = p < strEnd ? *p : '\0';
                    }
                }
                if ((styles & NumberStyle.AllowTypeSpecifier) != 0)
                {
                    // J2N: We treat a trailing f, F, d, D, m or M as if it were whitespace if the NumberStyle.AllowTypeSpecifier option is specified
                    if ((number.Kind == NumberBufferKind.FloatingPoint || number.Kind == NumberBufferKind.Decimal || number.Kind == NumberBufferKind.Unknown) && IsFloatTypeSuffix(ch))
                    {
                        ch = ++p < strEnd ? *p : '\0'; // skip
                    }
                    // J2N: We treat a trailing l, L, lu, Lu, lU, or LU as if it were whitespace if the NumberStyle.AllowTypeSpecifier option is specified
                    else if ((number.Kind == NumberBufferKind.Integer || number.Kind == NumberBufferKind.Unknown) && IsIntegralTypeSuffix(ch))
                    {
                        ch = ++p < strEnd ? *p : '\0'; // skip
                        if (IsUnsignedTypeSuffix(ch))
                        {
                            ch = ++p < strEnd ? *p : '\0'; // skip
                        }
                    }
                    // J2N: We treat a trailing u, U, ul, Ul, uL, or UL as if it were whitespace if the NumberStyle.AllowTypeSpecifier option is specified
                    else if ((number.Kind == NumberBufferKind.Integer || number.Kind == NumberBufferKind.Unknown) && IsUnsignedTypeSuffix(ch))
                    {
                        ch = ++p < strEnd ? *p : '\0'; // skip
                        if (IsIntegralTypeSuffix(ch))
                        {
                            ch = ++p < strEnd ? *p : '\0'; // skip
                        }
                    }
                }
                while (true)
                {
                    if (!IsWhite(ch) || (styles & NumberStyle.AllowTrailingWhite) == 0)
                    {
                        if ((styles & NumberStyle.AllowTrailingSign) != 0 && ((state & StateSign) == 0) && ((next = MatchChars(p, strEnd, info.PositiveSign)) != null || (((next = MatchChars(p, strEnd, info.NegativeSign)) != null) && (number.IsNegative = true))))
                        {
                            state |= StateSign;
                            p = next - 1;
                        }
                        else if (ch == ')' && ((state & StateParens) != 0))
                        {
                            state &= ~StateParens;
                        }
                        else if (currSymbol != null && (next = MatchChars(p, strEnd, currSymbol)) != null)
                        {
                            currSymbol = null;
                            p = next - 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    ch = ++p < strEnd ? *p : '\0';
                }
                if ((state & StateParens) == 0)
                {
                    if ((state & StateNonZero) == 0)
                    {
                        if (number.Kind != NumberBufferKind.Decimal)
                        {
                            number.Scale = 0;
                        }
                        if ((number.Kind == NumberBufferKind.Integer) && (state & StateDecimal) == 0)
                        {
                            number.IsNegative = false;
                        }
                    }
                    str = p;
                    return true;
                }
            }
            str = p;
            return false;
        }

        private static unsafe bool TryParseFloatingPointHexNumber(ref char* str, char* strEnd, NumberStyle styles, HexFloatingPointNumberBuffer number, NumberFormatInfo info)
        {
            Debug.Assert(str != null);
            Debug.Assert(strEnd != null);
            Debug.Assert(str <= strEnd);
            Debug.Assert((styles & NumberStyle.AllowHexSpecifier) != 0);

            const int StateSign = 0x0001;
            const int StateParens = 0x0002;
            const int StateDigits = 0x0004;
            const int StateNonZero = 0x0008;
            const int StateDecimal = 0x0010;
            //const int StateCurrency = 0x0020;

            Debug.Assert(number.DigitsCount == 0);
            Debug.Assert(number.Scale == 0);
            Debug.Assert(!number.IsNegative);
            //Debug.Assert(!number.HasNonZeroTail);

            //number.CheckConsistency();

            string decSep;                  // decimal separator from NumberFormatInfo.
            string groupSep;                // group separator from NumberFormatInfo.
            //string? currSymbol = null;       // currency symbol from NumberFormatInfo.

            //bool parsingCurrency = false;
            //if ((styles & NumberStyle.AllowCurrencySymbol) != 0)
            //{
            //    currSymbol = info.CurrencySymbol;

            //    // The idea here is to match the currency separators and on failure match the number separators to keep the perf of VB's IsNumeric fast.
            //    // The values of decSep are setup to use the correct relevant separator (currency in the if part and decimal in the else part).
            //    decSep = info.CurrencyDecimalSeparator;
            //    groupSep = info.CurrencyGroupSeparator;
            //    parsingCurrency = true;
            //}
            //else
            {
                decSep = info.NumberDecimalSeparator;
                groupSep = info.NumberGroupSeparator;
            }

            int state = 0;
            char* p = str;
            char ch = p < strEnd ? *p : '\0';
            char* next;

            while (true)
            {
                // Eat whitespace unless we've found a sign which isn't followed by a currency symbol.
                // "-Kr 1231.47" is legal but "- 1231.47" is not.
                if (!IsWhite(ch) || (styles & NumberStyle.AllowLeadingWhite) == 0 || ((state & StateSign) != 0 && (/*(state & StateCurrency) == 0 &&*/ info.NumberNegativePattern != 2)))
                {
                    if ((((styles & NumberStyle.AllowLeadingSign) != 0) && (state & StateSign) == 0) && ((next = MatchChars(p, strEnd, info.PositiveSign)) != null || ((next = MatchChars(p, strEnd, info.NegativeSign)) != null && (number.IsNegative = true))))
                    {
                        state |= StateSign;
                        p = next - 1;
                    }
                    else if (ch == '(' && ((styles & NumberStyle.AllowParentheses) != 0) && ((state & StateSign) == 0))
                    {
                        state |= StateSign | StateParens;
                        number.IsNegative = true;
                    }
                    //else if (currSymbol != null && (next = MatchChars(p, strEnd, currSymbol)) != null)
                    //{
                    //    state |= StateCurrency;
                    //    currSymbol = null;
                    //    // We already found the currency symbol. There should not be more currency symbols. Set
                    //    // currSymbol to NULL so that we won't search it again in the later code path.
                    //    p = next - 1;
                    //}
                    else
                    {
                        break;
                    }
                }
                ch = ++p < strEnd ? *p : '\0';
            }

            // Allow 0x or 0X to be specified in the string
            if ((styles & NumberStyle.AllowHexSpecifier) != 0 && ((next = MatchChars(p, strEnd, "0x")) != null || (next = MatchChars(p, strEnd, "0X")) != null))
            {
                p = next - 1;
                ch = ++p < strEnd ? *p : '\0';
            }

            int digCount = 0;
            int digEnd = 0;
            int maxDigCount = number.Significand.Length - 1; //number.Digits.Length - 1;

            while (true)
            {
                //if (IsDigit(ch))
                if (Character.IsAsciiHexDigit(ch))
                {
                    state |= StateDigits;

                    if (ch != '0' || (state & StateNonZero) != 0)
                    {
                        if (digCount < maxDigCount)
                        {
                            //number.Digits[digCount++] = (byte)(ch);
                            digCount++;
                            if ((state & StateDecimal) == 0)
                            {
                                number.IntegerPart[number.IntegerPartLength++] = ch;
                            }
                            else
                            {
                                number.DecimalPart[number.DecimalPartLength++] = ch;
                            }
                            number.Significand[number.SignificandLength++] = ch;

                            if ((ch != '0') /*|| (number.Kind != NumberBufferKind.Integer)*/)
                            {
                                digEnd = digCount;
                            }
                        }
                        //else if (ch != '0')
                        //{
                        //    // For decimal and binary floating-point numbers, we only
                        //    // need to store digits up to maxDigCount. However, we still
                        //    // need to keep track of whether any additional digits past
                        //    // maxDigCount were non-zero, as that can impact rounding
                        //    // for an input that falls evenly between two representable
                        //    // results.

                        //    number.HasNonZeroTail = true;
                        //}

                        if ((state & StateDecimal) == 0)
                        {
                            number.Scale++;
                        }
                        state |= StateNonZero;
                        number.SignificandIsZero = false;
                        if ((state & StateDecimal) == 0)
                        {
                            number.IntegerPartIsZero = false;
                        }
                        else
                        {
                            number.DecimalPartIsZero = false;
                        }
                    }
                    else if ((state & StateDecimal) != 0)
                    {
                        number.Scale--;
                    }
                }
                else if (((styles & NumberStyle.AllowDecimalPoint) != 0) && ((state & StateDecimal) == 0) && ((next = MatchChars(p, strEnd, decSep)) != null /*|| (parsingCurrency && (state & StateCurrency) == 0) && (next = MatchChars(p, strEnd, info.NumberDecimalSeparator)) != null*/))
                {
                    state |= StateDecimal;
                    p = next - 1;
                }
                //else if (((styles & NumberStyle.AllowThousands) != 0) && ((state & StateDigits) != 0) && ((state & StateDecimal) == 0) && ((next = MatchChars(p, strEnd, groupSep)) != null /*|| (parsingCurrency && (state & StateCurrency) == 0) && (next = MatchChars(p, strEnd, info.NumberGroupSeparator)) != null*/))
                //{
                //    p = next - 1;
                //}
                else
                {
                    break;
                }
                ch = ++p < strEnd ? *p : '\0';
            }

            //bool negExp = false;
            number.DigitsCount = digEnd;
            //number.Digits[digEnd] = (byte)('\0');
            if ((state & StateDigits) != 0)
            {
                //if ((ch == 'E' || ch == 'e') && ((styles & NumberStyle.AllowExponent) != 0))
                if ((ch == 'P' || ch == 'p') && ((styles & NumberStyle.AllowExponent) != 0))
                {
                    char* temp = p;
                    ch = ++p < strEnd ? *p : '\0';
                    if ((next = MatchChars(p, strEnd, info.PositiveSign)) != null)
                    {
                        ch = (p = next) < strEnd ? *p : '\0';
                    }
                    else if ((next = MatchChars(p, strEnd, info.NegativeSign)) != null)
                    {
                        ch = (p = next) < strEnd ? *p : '\0';
                        //negExp = true;
                        number.ExponentIsNegative = true;
                    }
                    if (IsDigit(ch))
                    {
                        int exp = 0;
                        do
                        {
                            exp = exp * 10 + (ch - '0');
                            //if (exp <= 1000)
                            {
                                number.Exponent[number.ExponentLength++] = ch;
                            }
                            ch = ++p < strEnd ? *p : '\0';
                            //if (exp > 1000)
                            //{
                            //    exp = 9999;
                            //    number.Exponent[0] = '9';
                            //    number.Exponent[1] = '9';
                            //    number.Exponent[2] = '9';
                            //    number.Exponent[3] = '9';
                            //    number.ExponentLength = 4;
                            //    while (IsDigit(ch))
                            //    {
                            //        ch = ++p < strEnd ? *p : '\0';
                            //    }
                            //}
                        } while (IsDigit(ch));
                        //if (negExp)
                        //{
                        //    exp = -exp;
                        //}
                        //number.Scale += exp;
                    }
                    else
                    {
                        p = temp;
                        ch = p < strEnd ? *p : '\0';
                    }
                }
                // J2N: We treat a trailing f, F, d, D, m or M as if it were whitespace if the NumberStyle.AllowTypeSpecifier option is specified
                if ((styles & NumberStyle.AllowTypeSpecifier) != 0 /*&& (number.Kind == NumberBufferKind.FloatingPoint || number.Kind == NumberBufferKind.Decimal || number.Kind == NumberBufferKind.Unknown)*/
                    && IsFloatTypeSuffix(ch))
                {
                    ch = ++p < strEnd ? *p : '\0'; // skip
                }
                while (true)
                {
                    if (!IsWhite(ch) || (styles & NumberStyle.AllowTrailingWhite) == 0)
                    {
                        if ((styles & NumberStyle.AllowTrailingSign) != 0 && ((state & StateSign) == 0) && ((next = MatchChars(p, strEnd, info.PositiveSign)) != null || (((next = MatchChars(p, strEnd, info.NegativeSign)) != null) && (number.IsNegative = true))))
                        {
                            state |= StateSign;
                            p = next - 1;
                        }
                        else if (ch == ')' && ((state & StateParens) != 0))
                        {
                            state &= ~StateParens;
                        }
                        //else if (currSymbol != null && (next = MatchChars(p, strEnd, currSymbol)) != null)
                        //{
                        //    currSymbol = null;
                        //    p = next - 1;
                        //}
                        else
                        {
                            break;
                        }
                    }
                    ch = ++p < strEnd ? *p : '\0';
                }
                if ((state & StateParens) == 0)
                {
                    //if ((state & StateNonZero) == 0)
                    //{
                    //    if (number.Kind != NumberBufferKind.Decimal)
                    //    {
                    //        number.Scale = 0;
                    //    }
                    //    if ((number.Kind == NumberBufferKind.Integer) && (state & StateDecimal) == 0)
                    //    {
                    //        number.IsNegative = false;
                    //    }
                    //}
                    str = p;
                    return true;
                }
            }
            str = p;
            return false;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static ParsingStatus TryParseInt32(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out int result)
        //{
        //    if ((styles & ~NumberStyle.Integer) == 0)
        //    {
        //        // Optimized path for the common case of anything that's allowed for integer style.
        //        return TryParseInt32IntegerStyle(value, styles, info, out result);
        //    }

        //    if ((styles & NumberStyle.AllowHexSpecifier) != 0)
        //    {
        //        result = 0;
        //        return TryParseUInt32HexNumberStyle(value, styles, out Unsafe.As<int, uint>(ref result));
        //    }

        //    return TryParseInt32Number(value, styles, info, out result);
        //}

        //private static unsafe ParsingStatus TryParseInt32Number(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out int result)
        //{
        //    result = 0;
        //    byte* pDigits = stackalloc byte[Int32NumberBufferLength];
        //    NumberBuffer number = new NumberBuffer(NumberBufferKind.Integer, pDigits, Int32NumberBufferLength);

        //    if (!TryStringToNumber(value, styles, ref number, info))
        //    {
        //        return ParsingStatus.Failed;
        //    }

        //    if (!TryNumberToInt32(ref number, ref result))
        //    {
        //        return ParsingStatus.Overflow;
        //    }

        //    return ParsingStatus.OK;
        //}

        ///// <summary>Parses int limited to styles that make up NumberStyle.Integer.</summary>
        //internal static ParsingStatus TryParseInt32IntegerStyle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out int result)
        //{
        //    Debug.Assert((styles & ~NumberStyle.Integer) == 0, "Only handles subsets of Integer format");

        //    if (value.IsEmpty)
        //        goto FalseExit;

        //    int index = 0;
        //    int num = value[0];

        //    // Skip past any whitespace at the beginning.
        //    if ((styles & NumberStyle.AllowLeadingWhite) != 0 && IsWhite(num))
        //    {
        //        do
        //        {
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto FalseExit;
        //            num = value[index];
        //        }
        //        while (IsWhite(num));
        //    }

        //    // Parse leading sign.
        //    int sign = 1;
        //    if ((styles & NumberStyle.AllowLeadingSign) != 0)
        //    {
        //        if (info.HasInvariantNumberSigns)
        //        {
        //            if (num == '-')
        //            {
        //                sign = -1;
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //            else if (num == '+')
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //        }
        //        else
        //        {
        //            value = value.Slice(index);
        //            index = 0;
        //            string positiveSign = info.PositiveSign, negativeSign = info.NegativeSign;
        //            if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign))
        //            {
        //                index += positiveSign.Length;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //            else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign))
        //            {
        //                sign = -1;
        //                index += negativeSign.Length;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //        }
        //    }

        //    bool overflow = false;
        //    int answer = 0;

        //    if (IsDigit(num))
        //    {
        //        // Skip past leading zeros.
        //        if (num == '0')
        //        {
        //            do
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto DoneAtEnd;
        //                num = value[index];
        //            } while (num == '0');
        //            if (!IsDigit(num))
        //                goto HasTrailingChars;
        //        }

        //        // Parse most digits, up to the potential for overflow, which can't happen until after 9 digits.
        //        answer = num - '0'; // first digit
        //        index++;
        //        for (int i = 0; i < 8; i++) // next 8 digits can't overflow
        //        {
        //            if ((uint)index >= (uint)value.Length)
        //                goto DoneAtEnd;
        //            num = value[index];
        //            if (!IsDigit(num))
        //                goto HasTrailingChars;
        //            index++;
        //            answer = 10 * answer + num - '0';
        //        }

        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEnd;
        //        num = value[index];
        //        if (!IsDigit(num))
        //            goto HasTrailingChars;
        //        index++;
        //        // Potential overflow now processing the 10th digit.
        //        overflow = answer > int.MaxValue / 10;
        //        answer = answer * 10 + num - '0';
        //        overflow |= (uint)answer > int.MaxValue + (((uint)sign) >> 31);
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;

        //        // At this point, we're either overflowing or hitting a formatting error.
        //        // Format errors take precedence for compatibility.
        //        num = value[index];
        //        while (IsDigit(num))
        //        {
        //            overflow = true;
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto OverflowExit;
        //            num = value[index];
        //        }
        //        goto HasTrailingChars;
        //    }
        //    goto FalseExit;

        //DoneAtEndButPotentialOverflow:
        //    if (overflow)
        //    {
        //        goto OverflowExit;
        //    }
        //DoneAtEnd:
        //    result = answer * sign;
        //    ParsingStatus status = ParsingStatus.OK;
        //Exit:
        //    return status;

        //FalseExit: // parsing failed
        //    result = 0;
        //    status = ParsingStatus.Failed;
        //    goto Exit;
        //OverflowExit:
        //    result = 0;
        //    status = ParsingStatus.Overflow;
        //    goto Exit;

        //HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
        //    // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
        //    if (IsWhite(num))
        //    {
        //        if ((styles & NumberStyle.AllowTrailingWhite) == 0)
        //            goto FalseExit;
        //        for (index++; index < value.Length; index++)
        //        {
        //            if (!IsWhite(value[index]))
        //                break;
        //        }
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;
        //    }

        //    if (!TrailingZeros(value, index))
        //        goto FalseExit;

        //    goto DoneAtEndButPotentialOverflow;
        //}

        ///// <summary>Parses long inputs limited to styles that make up NumberStyle.Integer.</summary>
        //internal static ParsingStatus TryParseInt64IntegerStyle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out long result)
        //{
        //    Debug.Assert((styles & ~NumberStyle.Integer) == 0, "Only handles subsets of Integer format");

        //    if (value.IsEmpty)
        //        goto FalseExit;

        //    int index = 0;
        //    int num = value[0];

        //    // Skip past any whitespace at the beginning.
        //    if ((styles & NumberStyle.AllowLeadingWhite) != 0 && IsWhite(num))
        //    {
        //        do
        //        {
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto FalseExit;
        //            num = value[index];
        //        }
        //        while (IsWhite(num));
        //    }

        //    // Parse leading sign.
        //    int sign = 1;
        //    if ((styles & NumberStyle.AllowLeadingSign) != 0)
        //    {
        //        if (info.HasInvariantNumberSigns)
        //        {
        //            if (num == '-')
        //            {
        //                sign = -1;
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //            else if (num == '+')
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //        }
        //        else
        //        {
        //            value = value.Slice(index);
        //            index = 0;
        //            string positiveSign = info.PositiveSign, negativeSign = info.NegativeSign;
        //            if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign))
        //            {
        //                index += positiveSign.Length;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //            else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign))
        //            {
        //                sign = -1;
        //                index += negativeSign.Length;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //        }
        //    }

        //    bool overflow = false;
        //    long answer = 0;

        //    if (IsDigit(num))
        //    {
        //        // Skip past leading zeros.
        //        if (num == '0')
        //        {
        //            do
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto DoneAtEnd;
        //                num = value[index];
        //            } while (num == '0');
        //            if (!IsDigit(num))
        //                goto HasTrailingChars;
        //        }

        //        // Parse most digits, up to the potential for overflow, which can't happen until after 18 digits.
        //        answer = num - '0'; // first digit
        //        index++;
        //        for (int i = 0; i < 17; i++) // next 17 digits can't overflow
        //        {
        //            if ((uint)index >= (uint)value.Length)
        //                goto DoneAtEnd;
        //            num = value[index];
        //            if (!IsDigit(num))
        //                goto HasTrailingChars;
        //            index++;
        //            answer = 10 * answer + num - '0';
        //        }

        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEnd;
        //        num = value[index];
        //        if (!IsDigit(num))
        //            goto HasTrailingChars;
        //        index++;
        //        // Potential overflow now processing the 19th digit.
        //        overflow = answer > long.MaxValue / 10;
        //        answer = answer * 10 + num - '0';
        //        overflow |= (ulong)answer > (ulong)long.MaxValue + (((uint)sign) >> 31);
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;

        //        // At this point, we're either overflowing or hitting a formatting error.
        //        // Format errors take precedence for compatibility.
        //        num = value[index];
        //        while (IsDigit(num))
        //        {
        //            overflow = true;
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto OverflowExit;
        //            num = value[index];
        //        }
        //        goto HasTrailingChars;
        //    }
        //    goto FalseExit;

        //DoneAtEndButPotentialOverflow:
        //    if (overflow)
        //    {
        //        goto OverflowExit;
        //    }
        //DoneAtEnd:
        //    result = answer * sign;
        //    ParsingStatus status = ParsingStatus.OK;
        //Exit:
        //    return status;

        //FalseExit: // parsing failed
        //    result = 0;
        //    status = ParsingStatus.Failed;
        //    goto Exit;
        //OverflowExit:
        //    result = 0;
        //    status = ParsingStatus.Overflow;
        //    goto Exit;

        //HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
        //    // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
        //    if (IsWhite(num))
        //    {
        //        if ((styles & NumberStyle.AllowTrailingWhite) == 0)
        //            goto FalseExit;
        //        for (index++; index < value.Length; index++)
        //        {
        //            if (!IsWhite(value[index]))
        //                break;
        //        }
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;
        //    }

        //    if (!TrailingZeros(value, index))
        //        goto FalseExit;

        //    goto DoneAtEndButPotentialOverflow;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static ParsingStatus TryParseInt64(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out long result)
        //{
        //    if ((styles & ~NumberStyle.Integer) == 0)
        //    {
        //        // Optimized path for the common case of anything that's allowed for integer style.
        //        return TryParseInt64IntegerStyle(value, styles, info, out result);
        //    }

        //    if ((styles & NumberStyle.AllowHexSpecifier) != 0)
        //    {
        //        result = 0;
        //        return TryParseUInt64HexNumberStyle(value, styles, out Unsafe.As<long, ulong>(ref result));
        //    }

        //    return TryParseInt64Number(value, styles, info, out result);
        //}

        //private static unsafe ParsingStatus TryParseInt64Number(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out long result)
        //{
        //    result = 0;
        //    byte* pDigits = stackalloc byte[Int64NumberBufferLength];
        //    NumberBuffer number = new NumberBuffer(NumberBufferKind.Integer, pDigits, Int64NumberBufferLength);

        //    if (!TryStringToNumber(value, styles, ref number, info))
        //    {
        //        return ParsingStatus.Failed;
        //    }

        //    if (!TryNumberToInt64(ref number, ref result))
        //    {
        //        return ParsingStatus.Overflow;
        //    }

        //    return ParsingStatus.OK;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static ParsingStatus TryParseUInt32(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out uint result)
        //{
        //    if ((styles & ~NumberStyle.Integer) == 0)
        //    {
        //        // Optimized path for the common case of anything that's allowed for integer style.
        //        return TryParseUInt32IntegerStyle(value, styles, info, out result);
        //    }

        //    if ((styles & NumberStyle.AllowHexSpecifier) != 0)
        //    {
        //        return TryParseUInt32HexNumberStyle(value, styles, out result);
        //    }

        //    return TryParseUInt32Number(value, styles, info, out result);
        //}

        //private static unsafe ParsingStatus TryParseUInt32Number(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out uint result)
        //{
        //    result = 0;
        //    byte* pDigits = stackalloc byte[UInt32NumberBufferLength];
        //    NumberBuffer number = new NumberBuffer(NumberBufferKind.Integer, pDigits, UInt32NumberBufferLength);

        //    if (!TryStringToNumber(value, styles, ref number, info))
        //    {
        //        return ParsingStatus.Failed;
        //    }

        //    if (!TryNumberToUInt32(ref number, ref result))
        //    {
        //        return ParsingStatus.Overflow;
        //    }

        //    return ParsingStatus.OK;
        //}

        ///// <summary>Parses uint limited to styles that make up NumberStyle.Integer.</summary>
        //internal static ParsingStatus TryParseUInt32IntegerStyle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out uint result)
        //{
        //    Debug.Assert((styles & ~NumberStyle.Integer) == 0, "Only handles subsets of Integer format");

        //    if (value.IsEmpty)
        //        goto FalseExit;

        //    int index = 0;
        //    int num = value[0];

        //    // Skip past any whitespace at the beginning.
        //    if ((styles & NumberStyle.AllowLeadingWhite) != 0 && IsWhite(num))
        //    {
        //        do
        //        {
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto FalseExit;
        //            num = value[index];
        //        }
        //        while (IsWhite(num));
        //    }

        //    // Parse leading sign.
        //    bool overflow = false;
        //    if ((styles & NumberStyle.AllowLeadingSign) != 0)
        //    {
        //        if (info.HasInvariantNumberSigns)
        //        {
        //            if (num == '+')
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //            else if (num == '-')
        //            {
        //                overflow = true;
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //        }
        //        else
        //        {
        //            value = value.Slice(index);
        //            index = 0;
        //            string positiveSign = info.PositiveSign, negativeSign = info.NegativeSign;
        //            if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign))
        //            {
        //                index += positiveSign.Length;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //            else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign))
        //            {
        //                overflow = true;
        //                index += negativeSign.Length;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //        }
        //    }

        //    int answer = 0;

        //    if (IsDigit(num))
        //    {
        //        // Skip past leading zeros.
        //        if (num == '0')
        //        {
        //            do
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto DoneAtEnd;
        //                num = value[index];
        //            } while (num == '0');
        //            if (!IsDigit(num))
        //                goto HasTrailingCharsZero;
        //        }

        //        // Parse most digits, up to the potential for overflow, which can't happen until after 9 digits.
        //        answer = num - '0'; // first digit
        //        index++;
        //        for (int i = 0; i < 8; i++) // next 8 digits can't overflow
        //        {
        //            if ((uint)index >= (uint)value.Length)
        //                goto DoneAtEndButPotentialOverflow;
        //            num = value[index];
        //            if (!IsDigit(num))
        //                goto HasTrailingChars;
        //            index++;
        //            answer = 10 * answer + num - '0';
        //        }

        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;
        //        num = value[index];
        //        if (!IsDigit(num))
        //            goto HasTrailingChars;
        //        index++;
        //        // Potential overflow now processing the 10th digit.
        //        overflow |= (uint)answer > uint.MaxValue / 10 || ((uint)answer == uint.MaxValue / 10 && num > '5');
        //        answer = answer * 10 + num - '0';
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;

        //        // At this point, we're either overflowing or hitting a formatting error.
        //        // Format errors take precedence for compatibility.
        //        num = value[index];
        //        while (IsDigit(num))
        //        {
        //            overflow = true;
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto OverflowExit;
        //            num = value[index];
        //        }
        //        goto HasTrailingChars;
        //    }
        //    goto FalseExit;

        //DoneAtEndButPotentialOverflow:
        //    if (overflow)
        //    {
        //        goto OverflowExit;
        //    }
        //DoneAtEnd:
        //    result = (uint)answer;
        //    ParsingStatus status = ParsingStatus.OK;
        //Exit:
        //    return status;

        //FalseExit: // parsing failed
        //    result = 0;
        //    status = ParsingStatus.Failed;
        //    goto Exit;
        //OverflowExit:
        //    result = 0;
        //    status = ParsingStatus.Overflow;
        //    goto Exit;

        //HasTrailingCharsZero:
        //    overflow = false;
        //HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
        //    // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
        //    if (IsWhite(num))
        //    {
        //        if ((styles & NumberStyle.AllowTrailingWhite) == 0)
        //            goto FalseExit;
        //        for (index++; index < value.Length; index++)
        //        {
        //            if (!IsWhite(value[index]))
        //                break;
        //        }
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;
        //    }

        //    if (!TrailingZeros(value, index))
        //        goto FalseExit;

        //    goto DoneAtEndButPotentialOverflow;
        //}

        ///// <summary>Parses uint limited to styles that make up NumberStyle.HexNumber.</summary>
        //private static ParsingStatus TryParseUInt32HexNumberStyle(ReadOnlySpan<char> value, NumberStyle styles, out uint result)
        //{
        //    Debug.Assert((styles & ~NumberStyle.HexNumber) == 0, "Only handles subsets of HexNumber format");

        //    if (value.IsEmpty)
        //        goto FalseExit;

        //    int index = 0;
        //    int num = value[0];

        //    // Skip past any whitespace at the beginning.
        //    if ((styles & NumberStyle.AllowLeadingWhite) != 0 && IsWhite(num))
        //    {
        //        do
        //        {
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto FalseExit;
        //            num = value[index];
        //        }
        //        while (IsWhite(num));
        //    }

        //    bool overflow = false;
        //    uint answer = 0;

        //    if (HexConverter.IsHexChar(num))
        //    {
        //        // Skip past leading zeros.
        //        if (num == '0')
        //        {
        //            do
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto DoneAtEnd;
        //                num = value[index];
        //            } while (num == '0');
        //            if (!HexConverter.IsHexChar(num))
        //                goto HasTrailingChars;
        //        }

        //        // Parse up through 8 digits, as no overflow is possible
        //        answer = (uint)HexConverter.FromChar(num); // first digit
        //        index++;
        //        for (int i = 0; i < 7; i++) // next 7 digits can't overflow
        //        {
        //            if ((uint)index >= (uint)value.Length)
        //                goto DoneAtEnd;
        //            num = value[index];

        //            uint numValue = (uint)HexConverter.FromChar(num);
        //            if (numValue == 0xFF)
        //                goto HasTrailingChars;
        //            index++;
        //            answer = 16 * answer + numValue;
        //        }

        //        // If there's another digit, it's an overflow.
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEnd;
        //        num = value[index];
        //        if (!HexConverter.IsHexChar(num))
        //            goto HasTrailingChars;

        //        // At this point, we're either overflowing or hitting a formatting error.
        //        // Format errors take precedence for compatibility. Read through any remaining digits.
        //        do
        //        {
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto OverflowExit;
        //            num = value[index];
        //        } while (HexConverter.IsHexChar(num));
        //        overflow = true;
        //        goto HasTrailingChars;
        //    }
        //    goto FalseExit;

        //DoneAtEndButPotentialOverflow:
        //    if (overflow)
        //    {
        //        goto OverflowExit;
        //    }
        //DoneAtEnd:
        //    result = answer;
        //    ParsingStatus status = ParsingStatus.OK;
        //Exit:
        //    return status;

        //FalseExit: // parsing failed
        //    result = 0;
        //    status = ParsingStatus.Failed;
        //    goto Exit;
        //OverflowExit:
        //    result = 0;
        //    status = ParsingStatus.Overflow;
        //    goto Exit;

        //HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
        //    // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
        //    if (IsWhite(num))
        //    {
        //        if ((styles & NumberStyle.AllowTrailingWhite) == 0)
        //            goto FalseExit;
        //        for (index++; index < value.Length; index++)
        //        {
        //            if (!IsWhite(value[index]))
        //                break;
        //        }
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;
        //    }

        //    if (!TrailingZeros(value, index))
        //        goto FalseExit;

        //    goto DoneAtEndButPotentialOverflow;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static ParsingStatus TryParseUInt64(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out ulong result)
        //{
        //    if ((styles & ~NumberStyle.Integer) == 0)
        //    {
        //        // Optimized path for the common case of anything that's allowed for integer style.
        //        return TryParseUInt64IntegerStyle(value, styles, info, out result);
        //    }

        //    if ((styles & NumberStyle.AllowHexSpecifier) != 0)
        //    {
        //        return TryParseUInt64HexNumberStyle(value, styles, out result);
        //    }

        //    return TryParseUInt64Number(value, styles, info, out result);
        //}

        //private static unsafe ParsingStatus TryParseUInt64Number(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out ulong result)
        //{
        //    result = 0;
        //    byte* pDigits = stackalloc byte[UInt64NumberBufferLength];
        //    NumberBuffer number = new NumberBuffer(NumberBufferKind.Integer, pDigits, UInt64NumberBufferLength);

        //    if (!TryStringToNumber(value, styles, ref number, info))
        //    {
        //        return ParsingStatus.Failed;
        //    }

        //    if (!TryNumberToUInt64(ref number, ref result))
        //    {
        //        return ParsingStatus.Overflow;
        //    }

        //    return ParsingStatus.OK;
        //}

        ///// <summary>Parses ulong limited to styles that make up NumberStyle.Integer.</summary>
        //internal static ParsingStatus TryParseUInt64IntegerStyle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out ulong result)
        //{
        //    Debug.Assert((styles & ~NumberStyle.Integer) == 0, "Only handles subsets of Integer format");

        //    if (value.IsEmpty)
        //        goto FalseExit;

        //    int index = 0;
        //    int num = value[0];

        //    // Skip past any whitespace at the beginning.
        //    if ((styles & NumberStyle.AllowLeadingWhite) != 0 && IsWhite(num))
        //    {
        //        do
        //        {
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto FalseExit;
        //            num = value[index];
        //        }
        //        while (IsWhite(num));
        //    }

        //    // Parse leading sign.
        //    bool overflow = false;
        //    if ((styles & NumberStyle.AllowLeadingSign) != 0)
        //    {
        //        if (info.HasInvariantNumberSigns)
        //        {
        //            if (num == '+')
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //            else if (num == '-')
        //            {
        //                overflow = true;
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //        }
        //        else
        //        {
        //            value = value.Slice(index);
        //            index = 0;
        //            string positiveSign = info.PositiveSign, negativeSign = info.NegativeSign;
        //            if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign))
        //            {
        //                index += positiveSign.Length;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //            else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign))
        //            {
        //                overflow = true;
        //                index += negativeSign.Length;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto FalseExit;
        //                num = value[index];
        //            }
        //        }
        //    }

        //    long answer = 0;

        //    if (IsDigit(num))
        //    {
        //        // Skip past leading zeros.
        //        if (num == '0')
        //        {
        //            do
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto DoneAtEnd;
        //                num = value[index];
        //            } while (num == '0');
        //            if (!IsDigit(num))
        //                goto HasTrailingCharsZero;
        //        }

        //        // Parse most digits, up to the potential for overflow, which can't happen until after 19 digits.
        //        answer = num - '0'; // first digit
        //        index++;
        //        for (int i = 0; i < 18; i++) // next 18 digits can't overflow
        //        {
        //            if ((uint)index >= (uint)value.Length)
        //                goto DoneAtEndButPotentialOverflow;
        //            num = value[index];
        //            if (!IsDigit(num))
        //                goto HasTrailingChars;
        //            index++;
        //            answer = 10 * answer + num - '0';
        //        }

        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;
        //        num = value[index];
        //        if (!IsDigit(num))
        //            goto HasTrailingChars;
        //        index++;
        //        // Potential overflow now processing the 20th digit.
        //        overflow |= (ulong)answer > ulong.MaxValue / 10 || ((ulong)answer == ulong.MaxValue / 10 && num > '5');
        //        answer = answer * 10 + num - '0';
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;

        //        // At this point, we're either overflowing or hitting a formatting error.
        //        // Format errors take precedence for compatibility.
        //        num = value[index];
        //        while (IsDigit(num))
        //        {
        //            overflow = true;
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto OverflowExit;
        //            num = value[index];
        //        }
        //        goto HasTrailingChars;
        //    }
        //    goto FalseExit;

        //DoneAtEndButPotentialOverflow:
        //    if (overflow)
        //    {
        //        goto OverflowExit;
        //    }
        //DoneAtEnd:
        //    result = (ulong)answer;
        //    ParsingStatus status = ParsingStatus.OK;
        //Exit:
        //    return status;

        //FalseExit: // parsing failed
        //    result = 0;
        //    status = ParsingStatus.Failed;
        //    goto Exit;
        //OverflowExit:
        //    result = 0;
        //    status = ParsingStatus.Overflow;
        //    goto Exit;

        //HasTrailingCharsZero:
        //    overflow = false;
        //HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
        //    // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
        //    if (IsWhite(num))
        //    {
        //        if ((styles & NumberStyle.AllowTrailingWhite) == 0)
        //            goto FalseExit;
        //        for (index++; index < value.Length; index++)
        //        {
        //            if (!IsWhite(value[index]))
        //                break;
        //        }
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;
        //    }

        //    if (!TrailingZeros(value, index))
        //        goto FalseExit;

        //    goto DoneAtEndButPotentialOverflow;
        //}

        ///// <summary>Parses ulong limited to styles that make up NumberStyle.HexNumber.</summary>
        //private static ParsingStatus TryParseUInt64HexNumberStyle(ReadOnlySpan<char> value, NumberStyle styles, out ulong result)
        //{
        //    Debug.Assert((styles & ~NumberStyle.HexNumber) == 0, "Only handles subsets of HexNumber format");

        //    if (value.IsEmpty)
        //        goto FalseExit;

        //    int index = 0;
        //    int num = value[0];

        //    // Skip past any whitespace at the beginning.
        //    if ((styles & NumberStyle.AllowLeadingWhite) != 0 && IsWhite(num))
        //    {
        //        do
        //        {
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto FalseExit;
        //            num = value[index];
        //        }
        //        while (IsWhite(num));
        //    }

        //    bool overflow = false;
        //    ulong answer = 0;

        //    if (HexConverter.IsHexChar(num))
        //    {
        //        // Skip past leading zeros.
        //        if (num == '0')
        //        {
        //            do
        //            {
        //                index++;
        //                if ((uint)index >= (uint)value.Length)
        //                    goto DoneAtEnd;
        //                num = value[index];
        //            } while (num == '0');
        //            if (!HexConverter.IsHexChar(num))
        //                goto HasTrailingChars;
        //        }

        //        // Parse up through 16 digits, as no overflow is possible
        //        answer = (uint)HexConverter.FromChar(num); // first digit
        //        index++;
        //        for (int i = 0; i < 15; i++) // next 15 digits can't overflow
        //        {
        //            if ((uint)index >= (uint)value.Length)
        //                goto DoneAtEnd;
        //            num = value[index];

        //            uint numValue = (uint)HexConverter.FromChar(num);
        //            if (numValue == 0xFF)
        //                goto HasTrailingChars;
        //            index++;
        //            answer = 16 * answer + numValue;
        //        }

        //        // If there's another digit, it's an overflow.
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEnd;
        //        num = value[index];
        //        if (!HexConverter.IsHexChar(num))
        //            goto HasTrailingChars;

        //        // At this point, we're either overflowing or hitting a formatting error.
        //        // Format errors take precedence for compatibility. Read through any remaining digits.
        //        do
        //        {
        //            index++;
        //            if ((uint)index >= (uint)value.Length)
        //                goto OverflowExit;
        //            num = value[index];
        //        } while (HexConverter.IsHexChar(num));
        //        overflow = true;
        //        goto HasTrailingChars;
        //    }
        //    goto FalseExit;

        //DoneAtEndButPotentialOverflow:
        //    if (overflow)
        //    {
        //        goto OverflowExit;
        //    }
        //DoneAtEnd:
        //    result = answer;
        //    ParsingStatus status = ParsingStatus.OK;
        //Exit:
        //    return status;

        //FalseExit: // parsing failed
        //    result = 0;
        //    status = ParsingStatus.Failed;
        //    goto Exit;
        //OverflowExit:
        //    result = 0;
        //    status = ParsingStatus.Overflow;
        //    goto Exit;

        //HasTrailingChars: // we've successfully parsed, but there are still remaining characters in the span
        //    // Skip past trailing whitespace, then past trailing zeros, and if anything else remains, fail.
        //    if (IsWhite(num))
        //    {
        //        if ((styles & NumberStyle.AllowTrailingWhite) == 0)
        //            goto FalseExit;
        //        for (index++; index < value.Length; index++)
        //        {
        //            if (!IsWhite(value[index]))
        //                break;
        //        }
        //        if ((uint)index >= (uint)value.Length)
        //            goto DoneAtEndButPotentialOverflow;
        //    }

        //    if (!TrailingZeros(value, index))
        //        goto FalseExit;

        //    goto DoneAtEndButPotentialOverflow;
        //}

        //internal static decimal ParseDecimal(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info)
        //{
        //    ParsingStatus status = TryParseDecimal(value, styles, info, out decimal result);
        //    if (status != ParsingStatus.OK)
        //    {
        //        ThrowOverflowOrFormatException(status, TypeCode.Decimal);
        //    }

        //    return result;
        //}

        //internal static unsafe bool TryNumberToDecimal(ref NumberBuffer number, ref decimal value)
        //{
        //    number.CheckConsistency();

        //    byte* p = number.GetDigitsPointer();
        //    int e = number.Scale;
        //    bool sign = number.IsNegative;
        //    uint c = *p;
        //    if (c == 0)
        //    {
        //        // To avoid risking an app-compat issue with pre 4.5 (where some app was illegally using Reflection to examine the internal scale bits), we'll only force
        //        // the scale to 0 if the scale was previously positive (previously, such cases were unparsable to a bug.)
        //        value = new decimal(0, 0, 0, sign, (byte)Math.Clamp(-e, 0, 28));
        //        return true;
        //    }

        //    if (e > DecimalPrecision)
        //        return false;

        //    ulong low64 = 0;
        //    while (e > -28)
        //    {
        //        e--;
        //        low64 *= 10;
        //        low64 += c - '0';
        //        c = *++p;
        //        if (low64 >= ulong.MaxValue / 10)
        //            break;
        //        if (c == 0)
        //        {
        //            while (e > 0)
        //            {
        //                e--;
        //                low64 *= 10;
        //                if (low64 >= ulong.MaxValue / 10)
        //                    break;
        //            }
        //            break;
        //        }
        //    }

        //    uint high = 0;
        //    while ((e > 0 || (c != 0 && e > -28)) &&
        //      (high < uint.MaxValue / 10 || (high == uint.MaxValue / 10 && (low64 < 0x99999999_99999999 || (low64 == 0x99999999_99999999 && c <= '5')))))
        //    {
        //        // multiply by 10
        //        ulong tmpLow = (uint)low64 * 10UL;
        //        ulong tmp64 = (uint)(low64 >> 32) * 10UL + (tmpLow >> 32);
        //        low64 = (uint)tmpLow + (tmp64 << 32);
        //        high = (uint)(tmp64 >> 32) + high * 10;

        //        if (c != 0)
        //        {
        //            c -= '0';
        //            low64 += c;
        //            if (low64 < c)
        //                high++;
        //            c = *++p;
        //        }
        //        e--;
        //    }

        //    if (c >= '5')
        //    {
        //        if ((c == '5') && ((low64 & 1) == 0))
        //        {
        //            c = *++p;

        //            bool hasZeroTail = !number.HasNonZeroTail;

        //            // We might still have some additional digits, in which case they need
        //            // to be considered as part of hasZeroTail. Some examples of this are:
        //            //  * 3.0500000000000000000001e-27
        //            //  * 3.05000000000000000000001e-27
        //            // In these cases, we will have processed 3 and 0, and ended on 5. The
        //            // buffer, however, will still contain a number of trailing zeros and
        //            // a trailing non-zero number.

        //            while ((c != 0) && hasZeroTail)
        //            {
        //                hasZeroTail &= (c == '0');
        //                c = *++p;
        //            }

        //            // We should either be at the end of the stream or have a non-zero tail
        //            Debug.Assert((c == 0) || !hasZeroTail);

        //            if (hasZeroTail)
        //            {
        //                // When the next digit is 5, the number is even, and all following
        //                // digits are zero we don't need to round.
        //                goto NoRounding;
        //            }
        //        }

        //        if (++low64 == 0 && ++high == 0)
        //        {
        //            low64 = 0x99999999_9999999A;
        //            high = uint.MaxValue / 10;
        //            e++;
        //        }
        //    }
        //NoRounding:

        //    if (e > 0)
        //        return false;

        //    if (e <= -DecimalPrecision)
        //    {
        //        // Parsing a large scale zero can give you more precision than fits in the decimal.
        //        // This should only happen for actual zeros or very small numbers that round to zero.
        //        value = new decimal(0, 0, 0, sign, DecimalPrecision - 1);
        //    }
        //    else
        //    {
        //        value = new decimal((int)low64, (int)(low64 >> 32), (int)high, sign, (byte)-e);
        //    }
        //    return true;
        //}

#if FEATURE_READONLYSPAN
        internal static double ParseDouble(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info)
        {
            if (!TryParseDouble(value, styles, info, out double result))
            {
                ThrowFormatException(new string(value));
            }

            return result;
        }
#endif

        internal static double ParseDouble(string value, NumberStyle styles, NumberFormatInfo info) // J2N TODO: ICharSequence?
        {
            if (!TryParseDouble(value, styles, info, out double result))
            {
                ThrowFormatException(value);
            }

            return result;
        }

#if FEATURE_READONLYSPAN
        internal static float ParseSingle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info)
        {
            if (!TryParseSingle(value, styles, info, out float result))
            {
                ThrowFormatException(new string(value));
            }

            return result;
        }
#endif

        internal static float ParseSingle(string value, NumberStyle styles, NumberFormatInfo info) // J2N TODO: ICharSequence?
        {
            if (!TryParseSingle(value, styles, info, out float result))
            {
                ThrowFormatException(value);
            }

            return result;
        }

        //internal static Half ParseHalf(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info)
        //{
        //    if (!TryParseHalf(value, styles, info, out Half result))
        //    {
        //        ThrowOverflowOrFormatException(ParsingStatus.Failed);
        //    }

        //    return result;
        //}

        //internal static unsafe ParsingStatus TryParseDecimal(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out decimal result)
        //{
        //    byte* pDigits = stackalloc byte[DecimalNumberBufferLength];
        //    NumberBuffer number = new NumberBuffer(NumberBufferKind.Decimal, pDigits, DecimalNumberBufferLength);

        //    result = 0;

        //    if (!TryStringToNumber(value, styles, ref number, info))
        //    {
        //        return ParsingStatus.Failed;
        //    }

        //    if (!TryNumberToDecimal(ref number, ref result))
        //    {
        //        return ParsingStatus.Overflow;
        //    }

        //    return ParsingStatus.OK;
        //}

#if FEATURE_READONLYSPAN
        internal static unsafe bool TryParseDouble(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out double result)
        {
            if ((styles & NumberStyle.AllowHexSpecifier) != 0)
            {
                return TryParseDoubleHexFloatStyle(value, styles, info, out result);
            }

            return TryParseDoubleFloatStyle(value, styles & ~NumberStyle.AllowHexSpecifier, info, out result);
        }
#endif

        internal static unsafe bool TryParseDouble(string value, NumberStyle styles, NumberFormatInfo info, out double result)
        {
            if ((styles & NumberStyle.AllowHexSpecifier) != 0)
            {
                return TryParseDoubleHexFloatStyle(value, styles, info, out result);
            }

            return TryParseDoubleFloatStyle(value, styles & ~NumberStyle.AllowHexSpecifier, info, out result);
        }

#if FEATURE_READONLYSPAN
        internal static unsafe bool TryParseDoubleFloatStyle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out double result)
        {
            byte* pDigits = stackalloc byte[DoubleNumberBufferLength];
            NumberBuffer number = new NumberBuffer(NumberBufferKind.FloatingPoint, pDigits, DoubleNumberBufferLength);

            if (!TryStringToNumber(value, styles, ref number, info))
            {
                ReadOnlySpan<char> valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.

                if (valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
                {
                    result = double.PositiveInfinity;
                }
                else if (valueTrim.EqualsOrdinalIgnoreCase(info.NegativeInfinitySymbol))
                {
                    result = double.NegativeInfinity;
                }
                else if (valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
                {
                    result = double.NaN;
                }
                else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Slice(info.PositiveSign.Length);

                    if (valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
                    {
                        result = double.PositiveInfinity;
                    }
                    else if (valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
                    {
                        result = double.NaN;
                    }
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                        valueTrim.Slice(info.NegativeSign.Length).EqualsOrdinalIgnoreCase(info.NaNSymbol))
                {
                    result = double.NaN;
                }
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else
            {
                result = NumberToDouble(ref number);
            }

            return true;
        }
#endif

        internal static unsafe bool TryParseDoubleFloatStyle(string value, NumberStyle styles, NumberFormatInfo info, out double result)
        {
            byte* pDigits = stackalloc byte[DoubleNumberBufferLength];
            NumberBuffer number = new NumberBuffer(NumberBufferKind.FloatingPoint, pDigits, DoubleNumberBufferLength);

            if (!TryStringToNumber(value, styles, ref number, info))
            {
                string valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.

                if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.PositiveInfinitySymbol))
                {
                    result = double.PositiveInfinity;
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NegativeInfinitySymbol))
                {
                    result = double.NegativeInfinity;
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NaNSymbol))
                {
                    result = double.NaN;
                }
                else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Substring(info.PositiveSign.Length);

                    if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.PositiveInfinitySymbol))
                    {
                        result = double.PositiveInfinity;
                    }
                    else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NaNSymbol))
                    {
                        result = double.NaN;
                    }
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                        StringComparer.OrdinalIgnoreCase.Equals(valueTrim.Substring(info.NegativeSign.Length), info.NaNSymbol))
                {
                    result = double.NaN;
                }
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else
            {
                result = NumberToDouble(ref number);
            }

            return true;
        }

#if FEATURE_READONLYSPAN

        internal static unsafe bool TryParseDoubleHexFloatStyle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out double result)
        {
            Debug.Assert(info != null);
            Debug.Assert((styles & ~(NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier | NumberStyle.AllowTrailingSign | NumberStyle.AllowParentheses)) == 0, "Only handles subsets of HexFloat format, trailing type, and alternate sign positions");

            var number = new DoubleNumberBuffer(value.Length);

            if (!TryStringToFloatingPointHexNumber(value, styles, number, info))
            {
                ReadOnlySpan<char> valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.

                if (valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
                {
                    result = double.PositiveInfinity;
                }
                else if (valueTrim.EqualsOrdinalIgnoreCase(info.NegativeInfinitySymbol))
                {
                    result = double.NegativeInfinity;
                }
                else if (valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
                {
                    result = double.NaN;
                }
                else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Slice(info.PositiveSign.Length);

                    if (valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
                    {
                        result = double.PositiveInfinity;
                    }
                    else if (valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
                    {
                        result = double.NaN;
                    }
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                        valueTrim.Slice(info.NegativeSign.Length).EqualsOrdinalIgnoreCase(info.NaNSymbol))
                {
                    result = double.NaN;
                }
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else if (!number.TryGetValue(out result))
            {
                return false;
            }

            return true;
        }
#endif

        internal static unsafe bool TryParseDoubleHexFloatStyle(string value, NumberStyle styles, NumberFormatInfo info, out double result)
        {
            Debug.Assert(info != null);
            Debug.Assert((styles & ~(NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier | NumberStyle.AllowTrailingSign | NumberStyle.AllowParentheses)) == 0, "Only handles subsets of HexFloat format, trailing type, and alternate sign positions");

            var number = new DoubleNumberBuffer(value.Length);

            if (!TryStringToFloatingPointHexNumber(value, styles, number, info))
            {
                string valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.

                if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.PositiveInfinitySymbol))
                {
                    result = double.PositiveInfinity;
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NegativeInfinitySymbol))
                {
                    result = double.NegativeInfinity;
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NaNSymbol))
                {
                    result = double.NaN;
                }
                else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Substring(info.PositiveSign.Length);

                    if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.PositiveInfinitySymbol))
                    {
                        result = double.PositiveInfinity;
                    }
                    else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NaNSymbol))
                    {
                        result = double.NaN;
                    }
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                        StringComparer.OrdinalIgnoreCase.Equals(valueTrim.Substring(info.NegativeSign.Length), info.NaNSymbol))
                {
                    result = double.NaN;
                }
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else if (!number.TryGetValue(out result))
            {
                return false;
            }

            return true;
        }

        //internal static unsafe bool TryParseHalf(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out Half result)
        //{
        //    byte* pDigits = stackalloc byte[HalfNumberBufferLength];
        //    NumberBuffer number = new NumberBuffer(NumberBufferKind.FloatingPoint, pDigits, HalfNumberBufferLength);

        //    if (!TryStringToNumber(value, styles, ref number, info))
        //    {
        //        ReadOnlySpan<char> valueTrim = value.Trim();

        //        // This code would be simpler if we only had the concept of `InfinitySymbol`, but
        //        // we don't so we'll check the existing cases first and then handle `PositiveSign` +
        //        // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.
        //        //
        //        // Additionally, since some cultures ("wo") actually define `PositiveInfinitySymbol`
        //        // to include `PositiveSign`, we need to check whether `PositiveInfinitySymbol` fits
        //        // that case so that we don't start parsing things like `++infini`.

        //        if (valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
        //        {
        //            result = Half.PositiveInfinity;
        //        }
        //        else if (valueTrim.EqualsOrdinalIgnoreCase(info.NegativeInfinitySymbol))
        //        {
        //            result = Half.NegativeInfinity;
        //        }
        //        else if (valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
        //        {
        //            result = Half.NaN;
        //        }
        //        else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
        //        {
        //            valueTrim = valueTrim.Slice(info.PositiveSign.Length);

        //            if (!info.PositiveInfinitySymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
        //            {
        //                result = Half.PositiveInfinity;
        //            }
        //            else if (!info.NaNSymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
        //            {
        //                result = Half.NaN;
        //            }
        //            else
        //            {
        //                result = (Half)0;
        //                return false;
        //            }
        //        }
        //        else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
        //                 !info.NaNSymbol.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
        //                 valueTrim.Slice(info.NegativeSign.Length).EqualsOrdinalIgnoreCase(info.NaNSymbol))
        //        {
        //            result = Half.NaN;
        //        }
        //        else
        //        {
        //            result = (Half)0;
        //            return false; // We really failed
        //        }
        //    }
        //    else
        //    {
        //        result = NumberToHalf(ref number);
        //    }

        //    return true;
        //}

#if FEATURE_READONLYSPAN
        internal static unsafe bool TryParseSingle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out float result)
        {
            if ((styles & NumberStyle.AllowHexSpecifier) != 0)
            {
                return TryParseSingleHexFloatStyle(value, styles, info, out result);
            }

            return TryParseSingleFloatStyle(value, styles & ~NumberStyle.AllowHexSpecifier, info, out result);
        }
#endif

        internal static unsafe bool TryParseSingle(string value, NumberStyle styles, NumberFormatInfo info, out float result)
        {
            if ((styles & NumberStyle.AllowHexSpecifier) != 0)
            {
                return TryParseSingleHexFloatStyle(value, styles, info, out result);
            }

            return TryParseSingleFloatStyle(value, styles & ~NumberStyle.AllowHexSpecifier, info, out result);
        }


#if FEATURE_READONLYSPAN
        internal static unsafe bool TryParseSingleFloatStyle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out float result)
        {
            byte* pDigits = stackalloc byte[SingleNumberBufferLength];
            NumberBuffer number = new NumberBuffer(NumberBufferKind.FloatingPoint, pDigits, SingleNumberBufferLength);

            if (!TryStringToNumber(value, styles, ref number, info))
            {
                ReadOnlySpan<char> valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.
                //
                // Additionally, since some cultures ("wo") actually define `PositiveInfinitySymbol`
                // to include `PositiveSign`, we need to check whether `PositiveInfinitySymbol` fits
                // that case so that we don't start parsing things like `++infini`.

                if (valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
                {
                    result = float.PositiveInfinity;
                }
                else if (valueTrim.EqualsOrdinalIgnoreCase(info.NegativeInfinitySymbol))
                {
                    result = float.NegativeInfinity;
                }
                else if (valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
                {
                    result = float.NaN;
                }
                else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Slice(info.PositiveSign.Length);

                    if (!info.PositiveInfinitySymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
                    {
                        result = float.PositiveInfinity;
                    }
                    else if (!info.NaNSymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
                    {
                        result = float.NaN;
                    }
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         !info.NaNSymbol.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         valueTrim.Slice(info.NegativeSign.Length).EqualsOrdinalIgnoreCase(info.NaNSymbol))
                {
                    result = float.NaN;
                }
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else
            {
                result = NumberToSingle(ref number);
            }

            return true;
        }
#endif

        internal static unsafe bool TryParseSingleFloatStyle(string value, NumberStyle styles, NumberFormatInfo info, out float result)
        {
            byte* pDigits = stackalloc byte[SingleNumberBufferLength];
            NumberBuffer number = new NumberBuffer(NumberBufferKind.FloatingPoint, pDigits, SingleNumberBufferLength);

            if (!TryStringToNumber(value, styles, ref number, info))
            {
                string valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.
                //
                // Additionally, since some cultures ("wo") actually define `PositiveInfinitySymbol`
                // to include `PositiveSign`, we need to check whether `PositiveInfinitySymbol` fits
                // that case so that we don't start parsing things like `++infini`.

                if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.PositiveInfinitySymbol))
                {
                    result = float.PositiveInfinity;
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NegativeInfinitySymbol))
                {
                    result = float.NegativeInfinity;
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NaNSymbol))
                {
                    result = float.NaN;
                }
                else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Substring(info.PositiveSign.Length);

                    if (!info.PositiveInfinitySymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.PositiveInfinitySymbol))
                    {
                        result = float.PositiveInfinity;
                    }
                    else if (!info.NaNSymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NaNSymbol))
                    {
                        result = float.NaN;
                    }
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         !info.NaNSymbol.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         StringComparer.OrdinalIgnoreCase.Equals(valueTrim.Substring(info.NegativeSign.Length), info.NaNSymbol))
                {
                    result = float.NaN;
                }
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else
            {
                result = NumberToSingle(ref number);
            }

            return true;
        }

#if FEATURE_READONLYSPAN

        internal static unsafe bool TryParseSingleHexFloatStyle(ReadOnlySpan<char> value, NumberStyle styles, NumberFormatInfo info, out float result)
        {
            Debug.Assert(info != null);
            Debug.Assert((styles & ~(NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier | NumberStyle.AllowTrailingSign | NumberStyle.AllowParentheses)) == 0, "Only handles subsets of HexFloat format, trailing type, and alternate sign positions");

            var number = new SingleNumberBuffer(value.Length);

            if (!TryStringToFloatingPointHexNumber(value, styles, number, info))
            {
                ReadOnlySpan<char> valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.
                //
                // Additionally, since some cultures ("wo") actually define `PositiveInfinitySymbol`
                // to include `PositiveSign`, we need to check whether `PositiveInfinitySymbol` fits
                // that case so that we don't start parsing things like `++infini`.

                if (valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
                {
                    result = float.PositiveInfinity;
                }
                else if (valueTrim.EqualsOrdinalIgnoreCase(info.NegativeInfinitySymbol))
                {
                    result = float.NegativeInfinity;
                }
                else if (valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
                {
                    result = float.NaN;
                }
                else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Slice(info.PositiveSign.Length);

                    if (!info.PositiveInfinitySymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && valueTrim.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol))
                    {
                        result = float.PositiveInfinity;
                    }
                    else if (!info.NaNSymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && valueTrim.EqualsOrdinalIgnoreCase(info.NaNSymbol))
                    {
                        result = float.NaN;
                    }
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         !info.NaNSymbol.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         valueTrim.Slice(info.NegativeSign.Length).EqualsOrdinalIgnoreCase(info.NaNSymbol))
                {
                    result = float.NaN;
                }
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else if(!number.TryGetValue(out result))
            {
                return false;
            }

            return true;
        }
#endif

        internal static unsafe bool TryParseSingleHexFloatStyle(string value, NumberStyle styles, NumberFormatInfo info, out float result)
        {
            Debug.Assert(info != null);
            Debug.Assert((styles & ~(NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier | NumberStyle.AllowTrailingSign | NumberStyle.AllowParentheses)) == 0, "Only handles subsets of HexFloat format, trailing type, and alternate sign positions");

            var number = new SingleNumberBuffer(value.Length);

            if (!TryStringToFloatingPointHexNumber(value, styles, number, info))
            {
                string valueTrim = value.Trim();

                // This code would be simpler if we only had the concept of `InfinitySymbol`, but
                // we don't so we'll check the existing cases first and then handle `PositiveSign` +
                // `PositiveInfinitySymbol` and `PositiveSign/NegativeSign` + `NaNSymbol` last.
                //
                // Additionally, since some cultures ("wo") actually define `PositiveInfinitySymbol`
                // to include `PositiveSign`, we need to check whether `PositiveInfinitySymbol` fits
                // that case so that we don't start parsing things like `++infini`.

                if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.PositiveInfinitySymbol))
                {
                    result = float.PositiveInfinity;
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NegativeInfinitySymbol))
                {
                    result = float.NegativeInfinity;
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NaNSymbol))
                {
                    result = float.NaN;
                }
                else if (valueTrim.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase))
                {
                    valueTrim = valueTrim.Substring(info.PositiveSign.Length);

                    if (!info.PositiveInfinitySymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.PositiveInfinitySymbol))
                    {
                        result = float.PositiveInfinity;
                    }
                    else if (!info.NaNSymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && StringComparer.OrdinalIgnoreCase.Equals(valueTrim, info.NaNSymbol))
                    {
                        result = float.NaN;
                    }
                    else
                    {
                        result = 0;
                        return false;
                    }
                }
                else if (valueTrim.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         !info.NaNSymbol.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) &&
                         StringComparer.OrdinalIgnoreCase.Equals(valueTrim.Substring(info.NegativeSign.Length), info.NaNSymbol))
                {
                    result = float.NaN;
                }
                else
                {
                    result = 0;
                    return false; // We really failed
                }
            }
            else if (!number.TryGetValue(out result))
            {
                return false;
            }

            return true;
        }

#if FEATURE_READONLYSPAN
        internal static unsafe bool TryStringToNumber(ReadOnlySpan<char> value, NumberStyle styles, ref NumberBuffer number, NumberFormatInfo info)
        {
            Debug.Assert(info != null);
            fixed (char* stringPointer = &MemoryMarshal.GetReference(value))
            {
                char* p = stringPointer;
                if (!TryParseNumber(ref p, p + value.Length, styles, ref number, info)
                    || ((int)(p - stringPointer) < value.Length && !TrailingZeros(value, (int)(p - stringPointer))))
                {
                    number.CheckConsistency();
                    return false;
                }
            }

            number.CheckConsistency();
            return true;
        }
#endif
        internal static unsafe bool TryStringToNumber(string value, NumberStyle styles, ref NumberBuffer number, NumberFormatInfo info)
        {
            Debug.Assert(info != null);
            fixed (char* stringPointer = value)
            {
                char* p = stringPointer;
                if (!TryParseNumber(ref p, p + value.Length, styles, ref number, info)
                    || ((int)(p - stringPointer) < value.Length && !TrailingZeros(value, (int)(p - stringPointer))))
                {
                    number.CheckConsistency();
                    return false;
                }
            }

            number.CheckConsistency();
            return true;
        }

#if FEATURE_READONLYSPAN
        internal static unsafe bool TryStringToFloatingPointHexNumber(ReadOnlySpan<char> value, NumberStyle styles, HexFloatingPointNumberBuffer number, NumberFormatInfo info)
        {
            Debug.Assert(info != null);
            fixed (char* stringPointer = &MemoryMarshal.GetReference(value))
            {
                char* p = stringPointer;
                if (!TryParseFloatingPointHexNumber(ref p, p + value.Length, styles, number, info)
                    || ((int)(p - stringPointer) < value.Length && !TrailingZeros(value, (int)(p - stringPointer))))
                {
                    //number.CheckConsistency();
                    return false;
                }
            }

            //number.CheckConsistency();
            return true;
        }
#endif
        internal static unsafe bool TryStringToFloatingPointHexNumber(string value, NumberStyle styles, HexFloatingPointNumberBuffer number, NumberFormatInfo info)
        {
            Debug.Assert(info != null);
            fixed (char* stringPointer = value)
            {
                char* p = stringPointer;
                if (!TryParseFloatingPointHexNumber(ref p, p + value.Length, styles, number, info)
                    || ((int)(p - stringPointer) < value.Length && !TrailingZeros(value, (int)(p - stringPointer))))
                {
                    //number.CheckConsistency();
                    return false;
                }
            }

            //number.CheckConsistency();
            return true;
        }

#if FEATURE_READONLYSPAN
        private static bool TrailingZeros(ReadOnlySpan<char> value, int index)
        {
            // For compatibility, we need to allow trailing zeros at the end of a number string
            for (int i = index; (uint)i < (uint)value.Length; i++)
            {
                if (value[i] != '\0')
                {
                    return false;
                }
            }

            return true;
        }
#endif
        private static bool TrailingZeros(string value, int index)
        {
            // For compatibility, we need to allow trailing zeros at the end of a number string
            for (int i = index; (uint)i < (uint)value.Length; i++)
            {
                if (value[i] != '\0')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsSpaceReplacingChar(char c) => c == '\u00a0' || c == '\u202f';

        private static unsafe char* MatchChars(char* p, char* pEnd, string value)
        {
            Debug.Assert(p != null && pEnd != null && p <= pEnd && value != null);
            fixed (char* stringPointer = value)
            {
                char* str = stringPointer;
                if (*str != '\0')
                {
                    // We only hurt the failure case
                    // This fix is for French or Kazakh cultures. Since a user cannot type 0xA0 or 0x202F as a
                    // space character we use 0x20 space character instead to mean the same.
                    while (true)
                    {
                        char cp = p < pEnd ? *p : '\0';
                        if (cp != *str && !(IsSpaceReplacingChar(*str) && cp == '\u0020'))
                        {
                            break;
                        }
                        p++;
                        str++;
                        if (*str == '\0')
                            return p;
                    }
                }
            }

            return null;
        }

        // Ternary op is a workaround for https://github.com/dotnet/runtime/issues/4207
        internal static bool IsWhite(int ch) => ch == 0x20 || (uint)(ch - 0x09) <= (0x0D - 0x09) ? true : false;

        private static bool IsDigit(int ch) => ((uint)ch - '0') <= 9;

        private static bool IsFloatTypeSuffix(int ch) => ch == 'f' || ch == 'F' || ch == 'd' || ch == 'D' || ch == 'm' || ch == 'M';

        private static bool IsIntegralTypeSuffix(int ch) => ch == 'L' || ch == 'l';

        private static bool IsUnsignedTypeSuffix(int ch) => ch == 'u' || ch == 'U';

        //internal enum ParsingStatus
        //{
        //    OK,
        //    Failed,
        //    Overflow
        //}

        //[DoesNotReturn]
        //internal static void ThrowOverflowOrFormatException(ParsingStatus status, TypeCode type = 0) => throw GetException(status, type);

        [DoesNotReturn]
        internal static void ThrowFormatException(string value) => throw new FormatException(J2N.SR.Format(SR.Format_InvalidString, value));

        [DoesNotReturn]
        internal static void ThrowOverflowException(TypeCode type) => throw GetException(type);

        private static Exception GetException(TypeCode type)
        {
            //if (status == ParsingStatus.Failed)
            //    return new FormatException(SR.Format_InvalidString, value);

            string s;
            switch (type)
            {
                case TypeCode.SByte:
                    s = SR.Overflow_SByte;
                    break;
                case TypeCode.Byte:
                    s = SR.Overflow_Byte;
                    break;
                case TypeCode.Int16:
                    s = SR.Overflow_Int16;
                    break;
                case TypeCode.UInt16:
                    s = SR.Overflow_UInt16;
                    break;
                case TypeCode.Int32:
                    s = SR.Overflow_Int32;
                    break;
                case TypeCode.UInt32:
                    s = SR.Overflow_UInt32;
                    break;
                case TypeCode.Int64:
                    s = SR.Overflow_Int64;
                    break;
                case TypeCode.UInt64:
                    s = SR.Overflow_UInt64;
                    break;
                default:
                    Debug.Assert(type == TypeCode.Decimal);
                    s = SR.Overflow_Decimal;
                    break;
            }
            return new OverflowException(s);
        }

        internal static double NumberToDouble(ref NumberBuffer number)
        {
            number.CheckConsistency();
            double result;

            if ((number.DigitsCount == 0) || (number.Scale < DoubleMinExponent))
            {
                result = 0;
            }
            else if (number.Scale > DoubleMaxExponent)
            {
                result = double.PositiveInfinity;
            }
            else
            {
                ulong bits = NumberToFloatingPointBits(ref number, in FloatingPointInfo.Double);
                result = BitConversion.Int64BitsToDouble((long)(bits));
            }

            return number.IsNegative ? -result : result;
        }

        //internal static Half NumberToHalf(ref NumberBuffer number)
        //{
        //    number.CheckConsistency();
        //    Half result;

        //    if ((number.DigitsCount == 0) || (number.Scale < HalfMinExponent))
        //    {
        //        result = default;
        //    }
        //    else if (number.Scale > HalfMaxExponent)
        //    {
        //        result = Half.PositiveInfinity;
        //    }
        //    else
        //    {
        //        ushort bits = (ushort)(NumberToFloatingPointBits(ref number, in FloatingPointInfo.Half));
        //        result = new Half(bits);
        //    }

        //    return number.IsNegative ? Half.Negate(result) : result;
        //}

        internal static float NumberToSingle(ref NumberBuffer number)
        {
            number.CheckConsistency();
            float result;

            if ((number.DigitsCount == 0) || (number.Scale < SingleMinExponent))
            {
                result = 0;
            }
            else if (number.Scale > SingleMaxExponent)
            {
                result = float.PositiveInfinity;
            }
            else
            {
                uint bits = (uint)(NumberToFloatingPointBits(ref number, in FloatingPointInfo.Single));
                result = BitConversion.Int32BitsToSingle((int)(bits));
            }

            return number.IsNegative ? -result : result;
        }
    }

    internal static class MemoryExtensions
    {
#if FEATURE_READONLYSPAN
        // From MemoryExtensions class in .NET Runtime
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool EqualsOrdinalIgnoreCase(this ReadOnlySpan<char> span, ReadOnlySpan<char> value)
        {
            if (span.Length != value.Length)
                return false;
            if (value.Length == 0)  // span.Length == value.Length == 0
                return true;
            return EqualsIgnoreCase(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(value), span.Length);
        }
#endif

        // J2N: For now, we are just calling this on .NET Standard 2.1+

#if FEATURE_READONLYSPAN
        // From Ordinal class in .NET Runtime
        internal static bool EqualsIgnoreCase(ref char charA, ref char charB, int length)
        {
            IntPtr byteOffset = IntPtr.Zero;

#if TARGET_64BIT // J2N: We don't have a separate 32 bit compilation, but if it is ever added, we can enable this
            // Read 4 chars (64 bits) at a time from each string
            while ((uint)length >= 4)
            {
                ulong valueA = Unsafe.ReadUnaligned<ulong>(ref Unsafe.As<char, byte>(ref Unsafe.AddByteOffset(ref charA, byteOffset)));
                ulong valueB = Unsafe.ReadUnaligned<ulong>(ref Unsafe.As<char, byte>(ref Unsafe.AddByteOffset(ref charB, byteOffset)));

                // A 32-bit test - even with the bit-twiddling here - is more efficient than a 64-bit test.
                ulong temp = valueA | valueB;
                if (!/*Utf16Utility.*/AllCharsInUInt32AreAscii((uint)temp | (uint)(temp >> 32)))
                {
                    goto NonAscii; // one of the inputs contains non-ASCII data
                }

                // Generally, the caller has likely performed a first-pass check that the input strings
                // are likely equal. Consider a dictionary which computes the hash code of its key before
                // performing a proper deep equality check of the string contents. We want to optimize for
                // the case where the equality check is likely to succeed, which means that we want to avoid
                // branching within this loop unless we're about to exit the loop, either due to failure or
                // due to us running out of input data.

                if (!/*Utf16Utility.*/UInt64OrdinalIgnoreCaseAscii(valueA, valueB))
                {
                    return false;
                }

                byteOffset += 8;
                length -= 4;
            }
#endif

            // Read 2 chars (32 bits) at a time from each string
#if TARGET_64BIT // J2N: We don't have a separate 32 bit compilation, but if it is ever added, we can enable this
            if ((uint)length >= 2)
#else
            while ((uint)length >= 2)
#endif
            {
                uint valueA = Unsafe.ReadUnaligned<uint>(ref Unsafe.As<char, byte>(ref Unsafe.AddByteOffset(ref charA, byteOffset)));
                uint valueB = Unsafe.ReadUnaligned<uint>(ref Unsafe.As<char, byte>(ref Unsafe.AddByteOffset(ref charB, byteOffset)));

                if (!/*Utf16Utility.*/AllCharsInUInt32AreAscii(valueA | valueB))
                {
                    //goto NonAscii; // one of the inputs contains non-ASCII data
                    throw new NotSupportedException("Only ASCII characters are supported."); // J2N: Since we only use for float/double parsing, we don't need to support non-ascii here. But maybe, someday if this is made into a Span comparer.
                }

                // Generally, the caller has likely performed a first-pass check that the input strings
                // are likely equal. Consider a dictionary which computes the hash code of its key before
                // performing a proper deep equality check of the string contents. We want to optimize for
                // the case where the equality check is likely to succeed, which means that we want to avoid
                // branching within this loop unless we're about to exit the loop, either due to failure or
                // due to us running out of input data.

                if (!/*Utf16Utility.*/UInt32OrdinalIgnoreCaseAscii(valueA, valueB))
                {
                    return false;
                }

                byteOffset += 4;
                length -= 2;
            }

            if (length != 0)
            {
                Debug.Assert(length == 1);

                uint valueA = Unsafe.AddByteOffset(ref charA, byteOffset);
                uint valueB = Unsafe.AddByteOffset(ref charB, byteOffset);

                if ((valueA | valueB) > 0x7Fu)
                {
                    //goto NonAscii; // one of the inputs contains non-ASCII data
                    throw new NotSupportedException("Only ASCII characters are supported."); // J2N: Since we only use for float/double parsing, we don't need to support non-ascii here. But maybe, someday if this is made into a Span comparer.
                }

                if (valueA == valueB)
                {
                    return true; // exact match
                }

                valueA |= 0x20u;
                if ((uint)(valueA - 'a') > (uint)('z' - 'a'))
                {
                    return false; // not exact match, and first input isn't in [A-Za-z]
                }

                // The ternary operator below seems redundant but helps RyuJIT generate more optimal code.
                // See https://github.com/dotnet/runtime/issues/4207.
                return (valueA == (valueB | 0x20u)) ? true : false;
            }

            Debug.Assert(length == 0);
            return true;

        //NonAscii:
        //    // The non-ASCII case is factored out into its own helper method so that the JIT
        //    // doesn't need to emit a complex prolog for its caller (this method).
        //    return CompareStringIgnoreCase(ref Unsafe.AddByteOffset(ref charA, byteOffset), length, ref Unsafe.AddByteOffset(ref charB, byteOffset), length) == 0;
        }
#endif

        // From Utf16Utility in System.Text.Unicode
        /// <summary>
        /// Returns true iff the UInt32 represents two ASCII UTF-16 characters in machine endianness.
        /// </summary>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static bool AllCharsInUInt32AreAscii(uint value)
        {
            return (value & ~0x007F_007Fu) == 0;
        }

        // From Utf16Utility in System.Text.Unicode
        /// <summary>
        /// Given two UInt32s that represent two ASCII UTF-16 characters each, returns true iff
        /// the two inputs are equal using an ordinal case-insensitive comparison.
        /// </summary>
        /// <remarks>
        /// This is a branchless implementation.
        /// </remarks>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static bool UInt32OrdinalIgnoreCaseAscii(uint valueA, uint valueB)
        {
            // Intrinsified in mono interpreter
            // ASSUMPTION: Caller has validated that input values are ASCII.
            Debug.Assert(AllCharsInUInt32AreAscii(valueA));
            Debug.Assert(AllCharsInUInt32AreAscii(valueB));

            // Generate a mask of all bits which are different between A and B. Since [A-Z]
            // and [a-z] differ by the 0x20 bit, we'll left-shift this by 2 now so that
            // this is moved over to the 0x80 bit, which nicely aligns with the calculation
            // we're going to do on the indicator flag later.
            //
            // n.b. All of the logic below assumes we have at least 2 "known zero" bits leading
            // each of the 7-bit ASCII values. This assumption won't hold if this method is
            // ever adapted to deal with packed bytes instead of packed chars.

            uint differentBits = (valueA ^ valueB) << 2;

            // Now, we want to generate a mask where for each word in the input, the mask contains
            // 0xFF7F if the word is [A-Za-z], 0xFFFF if the word is not [A-Za-z]. We know each
            // input word is ASCII (only low 7 bit set), so we can use a combination of addition
            // and logical operators as follows.
            //
            // original input   +05         |A0         +1A
            // ====================================================
            //         00 .. 3F -> 05 .. 44 -> A5 .. E4 -> BF .. FE
            //               40 ->       45 ->       E5 ->       FF
            // ([A-Z]) 41 .. 5A -> 46 .. 5F -> E6 .. FF -> 00 .. 19
            //         5B .. 5F -> 60 .. 64 -> E0 .. E4 -> FA .. FE
            //               60 ->       65 ->       E5 ->       FF
            // ([a-z]) 61 .. 7A -> 66 .. 7F -> E6 .. FF -> 00 .. 19
            //         7B .. 7F -> 80 .. 84 -> A0 .. A4 -> BA .. BE
            //
            // This combination of operations results in the 0x80 bit of each word being set
            // iff the original word value was *not* [A-Za-z].

            uint indicator = valueA + 0x0005_0005u;
            indicator |= 0x00A0_00A0u;
            indicator += 0x001A_001Au;
            indicator |= 0xFF7F_FF7Fu; // normalize each word to 0xFF7F or 0xFFFF

            // At this point, 'indicator' contains the mask of bits which are *not* allowed to
            // differ between the inputs, and 'differentBits' contains the mask of bits which
            // actually differ between the inputs. If these masks have any bits in common, then
            // the two values are *not* equal under an OrdinalIgnoreCase comparer.

            return (differentBits & indicator) == 0;
        }
    }
}

