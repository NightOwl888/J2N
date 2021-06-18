using J2N.Globalization;
using J2N.Text;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace J2N.Numerics
{
    /// <summary>
    /// Parses hex string to a single or double precision floating point number.
    /// </summary>
    internal class HexStringParser
    {
        private const int DOUBLE_EXPONENT_WIDTH = 11;

        private const int DOUBLE_MANTISSA_WIDTH = 52;

        private const int FLOAT_EXPONENT_WIDTH = 8;

        private const int FLOAT_MANTISSA_WIDTH = 23;

        private const int HEX_RADIX = 16;

        private const int MAX_SIGNIFICANT_LENGTH = 15;

        /// <summary>
        /// Patch for the .NET Regex class because it only supports Unicode 3.0.1 and does not support the xDigit property.
        /// This brings it up to Unicode 10.0.
        /// </summary>
        private const string XDIGIT = "(?:[\\p{Nd}A-Fa-f\\u07C0-\\u07C9\\u0BE6\\u0DE6-\\u0DEF\\u1090-\\u1099\\u1946-\\u194F\\u19D0-\\u19D9\\u1A80-\\u1A89\\u1A90-\\u1A99\\u1B50-\\u1B59\\u1BB0-\\u1BB9\\u1C40-\\u1C49\\u1C50-\\u1C59\\uA620-\\uA629\\uA8D0-\\uA8D9\\uA900-\\uA909\\uA9D0-\\uA9D9\\uA9F0-\\uA9F9\\uAA50-\\uAA59\\uABF0-\\uABF9\\uFF21-\\uFF26\\uFF41-\\uFF46]|\\uD801[\\uDCA0-\\uDCA9]|\\uD804[\\uDC66-\\uDC6F]|\\uD804[\\uDCF0-\\uDCF9]|\\uD804[\\uDD36-\\uDD3F]|\\uD804[\\uDDD0-\\uDDD9]|\\uD804[\\uDEF0-\\uDEF9]|\\uD805[\\uDC50-\\uDC59]|\\uD805[\\uDCD0-\\uDCD9]|\\uD805[\\uDE50-\\uDE59]|\\uD805[\\uDEC0-\\uDEC9]|\\uD805[\\uDF30-\\uDF39]|\\uD806[\\uDCE0-\\uDCE9]|\\uD807[\\uDC50-\\uDC59]|\\uD807[\\uDD50-\\uDD59]|\\uD81A[\\uDE60-\\uDE69]|\\uD81A[\\uDF50-\\uDF59]|\\uD835[\\uDFCE-\\uDFFF]|\\uD83A[\\uDD50-\\uDD59])";

        /// <summary>
        /// Patch for the .NET Regex class because it only supports Unicode 3.0.1 and does not support the Digit property.
        /// This brings it up to Unicode 10.0.
        /// </summary>
        private const string DIGIT = "(?:[\\p{Nd}\\u07C0-\\u07C9\\u0BE6\\u0DE6-\\u0DEF\\u1090-\\u1099\\u1946-\\u194F\\u19D0-\\u19D9\\u1A80-\\u1A89\\u1A90-\\u1A99\\u1B50-\\u1B59\\u1BB0-\\u1BB9\\u1C40-\\u1C49\\u1C50-\\u1C59\\uA620-\\uA629\\uA8D0-\\uA8D9\\uA900-\\uA909\\uA9D0-\\uA9D9\\uA9F0-\\uA9F9\\uAA50-\\uAA59\\uABF0-\\uABF9]|\\uD801[\\uDCA0-\\uDCA9]|\\uD804[\\uDC66-\\uDC6F]|\\uD804[\\uDCF0-\\uDCF9]|\\uD804[\\uDD36-\\uDD3F]|\\uD804[\\uDDD0-\\uDDD9]|\\uD804[\\uDEF0-\\uDEF9]|\\uD805[\\uDC50-\\uDC59]|\\uD805[\\uDCD0-\\uDCD9]|\\uD805[\\uDE50-\\uDE59]|\\uD805[\\uDEC0-\\uDEC9]|\\uD805[\\uDF30-\\uDF39]|\\uD806[\\uDCE0-\\uDCE9]|\\uD807[\\uDC50-\\uDC59]|\\uD807[\\uDD50-\\uDD59]|\\uD81A[\\uDE60-\\uDE69]|\\uD81A[\\uDF50-\\uDF59]|\\uD835[\\uDFCE-\\uDFFF]|\\uD83A[\\uDD50-\\uDD59])";

        //private const string NONDIGIT = "[^\\p{Nd}]" // Rep


        //private const string HEX_SIGNIFICANT = "0[xX](\\p{XDigit}+\\.?|\\p{XDigit}*\\.\\p{XDigit}+)"; //$NON-NLS-1$
        //private const string HEX_SIGNIFICANT = "(?<hex>0[xX]?)(?<significant>" + XDIGIT + "+[^pP\\x00-\\x20]*|" + XDIGIT + "*[^pP]*" + XDIGIT + "+)"; //$NON-NLS-1$
        private const string HEX_SIGNIFICANT = "(?<hex>0[xX])(?<significant>" + XDIGIT + "+[^pP\\x00-\\x20]*|" + XDIGIT + "*[^pP]*" + XDIGIT + "+)"; //$NON-NLS-1$

        //private const string BINARY_EXPONENT = "(?<exponentWhole>(?:[pP](?<exponent>[+-]?" + DIGIT + "+))?)"; //$NON-NLS-1$
        private const string BINARY_EXPONENT = "(?<exponentWhole>(?:[pP](?<exponent>[+-]?" + DIGIT + "+)))"; //$NON-NLS-1$

        private const string FLOAT_TYPE_SUFFIX = "(?<type>[fFdDmM]?)"; //$NON-NLS-1$ // J2N: Added m and M for System.Decimal

        private const string WHITE = "[\\x00-\\x20]";

        private const string NONWHITE = "[^\\x00-\\x20]";

        //private const string HEX_PATTERN = "[\\x00-\\x20]*([+-]?)" + HEX_SIGNIFICANT //$NON-NLS-1$
        //        + BINARY_EXPONENT + FLOAT_TYPE_SUFFIX + "[\\x00-\\x20]*"; //$NON-NLS-1$

        private const string HEX_PATTERN = "(?<leadWhite>" + WHITE + "*)(?<leadSign>" + NONWHITE + "*" + WHITE + "?)" + HEX_SIGNIFICANT //$NON-NLS-1$
                + BINARY_EXPONENT + FLOAT_TYPE_SUFFIX + "(?<trailSign>" + WHITE + "?" + NONWHITE + "*)(?<trailWhite>" + WHITE + "*)"; //$NON-NLS-1$

        private static readonly Regex PATTERN = new Regex(HEX_PATTERN, RegexOptions.Compiled);

        private readonly int EXPONENT_WIDTH;

        private readonly int MANTISSA_WIDTH;

        private readonly long EXPONENT_BASE;

        private readonly long MAX_EXPONENT;

        private readonly long MIN_EXPONENT;

        private readonly long MANTISSA_MASK;

        private long sign;

        private long exponent;

        private long mantissa;

        private string abandonedNumber = ""; //$NON-NLS-1$

        public HexStringParser(int exponent_width, int mantissa_width)
        {
            this.EXPONENT_WIDTH = exponent_width;
            this.MANTISSA_WIDTH = mantissa_width;

            this.EXPONENT_BASE = ~(-1L << (exponent_width - 1));
            this.MAX_EXPONENT = ~(-1L << exponent_width);
            this.MIN_EXPONENT = -(MANTISSA_WIDTH + 1);
            this.MANTISSA_MASK = ~(-1L << mantissa_width);
        }

        /*
         * Parses the hex string to a double number.
         */
        public static double ParseDouble(String hexString)
        {
            HexStringParser parser = new HexStringParser(DOUBLE_EXPONENT_WIDTH,
                    DOUBLE_MANTISSA_WIDTH);
            long result = parser.Parse(hexString);
            return BitConversion.Int64BitsToDouble(result);
        }

        /// <summary>
        /// Parses the hex string to a <see cref="double"/> number.
        /// </summary>
        /// <param name="hexString">The hex string to parse.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of <paramref name="hexString"/>.</param>
        /// <param name="info">Culture-sensitive number formatting information.</param>
        /// <returns>A single-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="hexString"/>.</returns>
        /// <exception cref="FormatException"></exception>
        public static double ParseDouble(string hexString, NumberStyle style, NumberFormatInfo info)
        {
            if (hexString is null)
                throw new ArgumentNullException(nameof(hexString));
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            HexStringParser parser = new HexStringParser(FLOAT_EXPONENT_WIDTH,
                    FLOAT_MANTISSA_WIDTH);
            if (!parser.TryParse(hexString, style, info, out long result))
            {
                DotNetNumber.ThrowFormatException(hexString);
            }
            return BitConversion.Int64BitsToDouble(result);
        }

        /// <summary>
        /// Parses the hex string to a <see cref="float"/> number.
        /// </summary>
        /// <param name="hexString">The hex string to parse.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of <paramref name="hexString"/>.</param>
        /// <param name="info">Culture-sensitive number formatting information.</param>
        /// <returns>A single-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="hexString"/>.</returns>
        /// <exception cref="FormatException"></exception>
        public static float ParseSingle(string hexString, NumberStyle style, NumberFormatInfo info)
        {
            if (hexString is null)
                throw new ArgumentNullException(nameof(hexString));
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            HexStringParser parser = new HexStringParser(FLOAT_EXPONENT_WIDTH,
                    FLOAT_MANTISSA_WIDTH);
            if (!parser.TryParse(hexString, style, info, out long result))
            {
                DotNetNumber.ThrowFormatException(hexString);
            }
            return BitConversion.Int32BitsToSingle((int)result);
        }

        

        private long Parse(string hexString)
        {
            string[] hexSegments = GetSegmentsFromHexString(hexString);
            string signStr = hexSegments[0];
            string significantStr = hexSegments[1];
            string exponentStr = hexSegments[2];

            ParseHexSign(signStr);
            ParseExponent(exponentStr);
            ParseMantissa(significantStr);

            sign <<= (MANTISSA_WIDTH + EXPONENT_WIDTH);
            exponent <<= MANTISSA_WIDTH;
            return sign | exponent | mantissa;
        }

        //private bool TryParse(string hexString, NumberStyle styles, NumberFormatInfo info, out long result)
        //{
        //    if (!TryGetSegmentsFromHexString(hexString, out string[]? hexSegments))
        //    {
        //        result = default;
        //        return false;
        //    }
        //    string signStr = hexSegments[0];
        //    string significantStr = hexSegments[1];
        //    string exponentStr = hexSegments[2];

        //    ParseHexSign(signStr);
        //    ParseExponent(exponentStr);
        //    if (!TryParseMantissa(significantStr, info))
        //    {
        //        result = default;
        //        return false;
        //    }

        //    sign <<= (MANTISSA_WIDTH + EXPONENT_WIDTH);
        //    exponent <<= MANTISSA_WIDTH;
        //    result =  sign | exponent | mantissa;
        //    return true;
        //}

        private bool TryParse(string hexString, NumberStyle styles, NumberFormatInfo info, out long result)
        {
            result = default;

            //if (!TryGetSegmentsFromHexString(hexString, out string[]? hexSegments))
            //{
            //    return false;
            //}
            //string signStr = hexSegments[0];
            //string significantStr = hexSegments[1];
            //string exponentStr = hexSegments[2];

            //string leadingWhitespaceStr = hexSegments[0];
            //string leadingSignStr = hexSegments[1];
            //string hexSpecifierStr = hexSegments[2];
            //string significantStr = hexSegments[3];
            //string exponentStr = hexSegments[4];
            //string typeSuffixStr = hexSegments[5];
            //string trailingSignStr = hexSegments[6];
            //string trailingWhitespaceStr = hexSegments[7];

            Match matcher = PATTERN.Match(hexString);
            if (!matcher.Success)
            {
                result = default;
                return false;
            }

            //string[] hexSegments = new string[3];
            //hexSegments[0] = matcher.Groups[1].Value;
            //hexSegments[1] = matcher.Groups[2].Value;
            //hexSegments[2] = matcher.Groups[3].Value;

            string leadingWhitespaceStr = matcher.Groups["leadWhite"].Value;
            string leadingSignStr = matcher.Groups["leadSign"].Value;
            string hexSpecifierStr = matcher.Groups["hex"].Value;
            string significantStr = matcher.Groups["significant"].Value;
            //hexSegments[4] = matcher.Groups[5].Value;
            string exponentStr = matcher.Groups["exponent"].Value;
            string typeSuffixStr = matcher.Groups["type"].Value;
            string trailingSignStr = matcher.Groups["trailSign"].Value;
            string trailingWhitespaceStr = matcher.Groups["trailWhite"].Value;

            if (!TryValidateFeature(leadingWhitespaceStr, (styles & NumberStyle.AllowLeadingWhite) != 0))
            {
                return false;
            }
            if (!TryParseHexSign(leadingSignStr, trailingSignStr, styles, info))
            {
                return false;
            }
            if (!TryValidateFeature(hexSpecifierStr, (styles & NumberStyle.AllowHexSpecifier) != 0))
            {
                return false;
            }
            ParseExponent(exponentStr);
            if (!TryParseMantissa(significantStr, info))
            {
                return false;
            }
            if (!TryValidateFeature(typeSuffixStr, (styles & NumberStyle.AllowTypeSpecifier) != 0))
            {
                return false;
            }
            if (!TryValidateFeature(trailingWhitespaceStr, (styles & NumberStyle.AllowTrailingWhite) != 0))
            {
                return false;
            }

            sign <<= (MANTISSA_WIDTH + EXPONENT_WIDTH);
            exponent <<= MANTISSA_WIDTH;
            result = sign | exponent | mantissa;
            return true;
        }

        /*
         * Analyzes the hex string and extracts the sign and digit segments.
         */
        private static string[] GetSegmentsFromHexString(string hexString)
        {
            //Matcher matcher = PATTERN.matcher(hexString);
            Match matcher = PATTERN.Match(hexString);
            if (!matcher.Success)
            {
                throw new FormatException();
            }

            string[] hexSegments = new string[3];
            hexSegments[0] = matcher.Groups[1].Value;
            hexSegments[1] = matcher.Groups[2].Value;
            hexSegments[2] = matcher.Groups[3].Value;

            return hexSegments;
        }

        /// <summary>
        /// Analyzes the hex string and extracts the sign and digit segments.
        /// </summary>
        private static bool TryGetSegmentsFromHexString(string hexString, [MaybeNullWhen(false)] out string[] result)
        {
            Match matcher = PATTERN.Match(hexString);
            if (!matcher.Success)
            {
                result = default;
                return false;
            }

            //string[] hexSegments = new string[3];
            //hexSegments[0] = matcher.Groups[1].Value;
            //hexSegments[1] = matcher.Groups[2].Value;
            //hexSegments[2] = matcher.Groups[3].Value;

            string[] hexSegments = new string[9];
            hexSegments[0] = matcher.Groups[1].Value; // Leading whitespace
            hexSegments[1] = matcher.Groups[2].Value; // Leading sign
            hexSegments[2] = matcher.Groups[3].Value; // Hex specifier
            hexSegments[3] = matcher.Groups[4].Value; // Significant
            hexSegments[4] = matcher.Groups[5].Value; // Exponent with specifier
            hexSegments[5] = matcher.Groups[6].Value; // Exponent
            hexSegments[6] = matcher.Groups[7].Value; // Type suffix
            hexSegments[7] = matcher.Groups[8].Value; // Trailing sign
            hexSegments[8] = matcher.Groups[9].Value; // Trailing whitespace

            result = hexSegments;
            return true;
        }

        //private bool TryParseLeadingWhitespace(string leadingWhitespaceStr, NumberStyle style)
        //{
        //    return leadingWhitespaceStr.Length == 0 || (leadingWhitespaceStr.Length > 0 && (style & NumberStyle.AllowLeadingWhite) != 0);
        //}

        //private bool TryParseWhitespace(string value, bool allowed)
        //{
        //    return value.Length == 0 || allowed;
        //}

        private bool TryValidateFeature(string value, bool allowed)
        {
            return value.Length == 0 || allowed;
        }

        /// <summary>
        /// Parses the sign field.
        /// </summary>
        /// <param name="signStr"></param>
        private void ParseHexSign(string signStr)
        {
            this.sign = signStr.Equals("-") ? 1 : 0; //$NON-NLS-1$
        }

        /// <summary>
        /// Parses the sign field.
        /// </summary>
        /// <param name="leadingSignStr"></param>
        /// <param name="trailingSignStr"></param>
        /// <param name="style"></param>
        /// <param name="info"></param>
        private bool TryParseHexSign(string leadingSignStr, string trailingSignStr, NumberStyle style, NumberFormatInfo info)
        {
            string negativeSign = info.NegativeSign;

            Debug.Assert(info != null);

            if ((style & NumberStyle.AllowLeadingSign) != 0)
            {
                if (negativeSign.Length == 0)
                {
                    this.sign = 0; // Nothing to match - just assume positive
                }
                // negative sign followed by optional white space
                else if (leadingSignStr.RegionMatches(0, negativeSign, 0, negativeSign.Length, StringComparison.Ordinal))
                {
                    if (leadingSignStr.Length == negativeSign.Length + 1)
                    {
                        if (!DotNetNumber.IsWhite(leadingSignStr[negativeSign.Length]))
                        {
                            this.sign = 0;
                            return false;
                        }
                    }
                    else if (leadingSignStr.Length > negativeSign.Length)
                    {
                        // extra garbage not allowed
                        this.sign = 0;
                        return false;
                    }

                    // Everything checked out - we have a negative sign followed by an optional space
                    this.sign = 1;
                    return true;
                }
                else if (leadingSignStr.Length > 0)
                {
                    // not a negative sign
                    this.sign = 0;
                    return false;
                }
                else
                {
                    this.sign = 0; // Positive (unless we match another negative method below)
                }
            }
            else if (leadingSignStr.Length > 0 && (style & NumberStyle.AllowParentheses) == 0)
            {
                this.sign = 0;
                return false; // Sign not allowed
            }

            if ((style & NumberStyle.AllowTrailingSign) != 0)
            {
                if (negativeSign.Length == 0)
                {
                    this.sign = 0; // Nothing to match - just assume positive
                }
                // negative sign preceded by white space
                else if (DotNetNumber.IsWhite(trailingSignStr[0]) && trailingSignStr.RegionMatches(1, negativeSign, 0, negativeSign.Length, StringComparison.Ordinal))
                {
                    // Everything checked out - we have a negative sign preceded by an optional space
                    this.sign = 1;
                    return true;
                }
                else if (trailingSignStr.RegionMatches(0, negativeSign, 0, negativeSign.Length, StringComparison.Ordinal))
                {
                    // Everything checked out - we have a negative sign
                    this.sign = 1;
                    return true;
                }
                else
                {
                    this.sign = 0; // Positive (unless we match another negative method below)
                }
            }
            else if (trailingSignStr.Length > 0 && (style & NumberStyle.AllowParentheses) == 0)
            {
                this.sign = 0;
                return false; // Sign not allowed
            }

            // No spaces allowed with parentheses
            if ((style & NumberStyle.AllowParentheses) != 0 && leadingSignStr.Length == 1 && trailingSignStr.Length == 1)
            {
                // Validate we have a pair
                if (leadingSignStr[0] == '(')
                {
                    if (trailingSignStr[0] == ')')
                    {
                        // Everything checked out - we have a valid pair of parentheses
                        sign = 1;
                        return true;
                    }
                    else
                    {
                        // Invalid
                        sign = 0;
                        return false;
                    }
                }
            }

            return true;
        }



        /// <summary>
        /// Parses the exponent field.
        /// </summary>
        /// <param name="exponentStr"></param>
        private void ParseExponent(string exponentStr)
        {
            // No exponent, raise to the power of 1
            if (exponentStr.Length == 0)
            {
                exponent = 1;
                CheckedAddExponent(EXPONENT_BASE);
                return;
            }

            char leadingChar = exponentStr[0];
            int expSign = (leadingChar == '-' ? -1 : 1);
            if (!Character.IsDigit(leadingChar))
            {
                exponentStr = exponentStr.Substring(1);
            }

            //try
            //{
            //    exponent = expSign * Int64.Parse(exponentStr, CultureInfo.InvariantCulture);
            //    CheckedAddExponent(EXPONENT_BASE);
            //}
            //catch (FormatException)
            //{
            //    exponent = expSign * long.MaxValue;
            //}
            if (long.TryParse(exponentStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out long tempExponent))
            {
                exponent = expSign * tempExponent;
                CheckedAddExponent(EXPONENT_BASE);
            }
            else
            {
                exponent = expSign * long.MaxValue;
            }
        }

        private static readonly Regex DecimalPoint = new Regex("\\.", RegexOptions.Compiled);

        /*
         * Parses the mantissa field.
         */
        private void ParseMantissa(string significantStr)
        {
            //string[] strings = significantStr.split("\\."); //$NON-NLS-1$
            string[] strings = DecimalPoint.Split(significantStr).TrimEnd()!;
            string strIntegerPart = strings[0];
            string strDecimalPart = strings.Length > 1 ? strings[1] : ""; //$NON-NLS-1$

            string significand = GetNormalizedSignificand(strIntegerPart, strDecimalPart);
            if (significand.Equals("0"))
            { //$NON-NLS-1$
                SetZero();
                return;
            }

            int offset = GetOffset(strIntegerPart, strDecimalPart);
            CheckedAddExponent(offset);

            if (exponent >= MAX_EXPONENT)
            {
                SetInfinite();
                return;
            }

            if (exponent <= MIN_EXPONENT)
            {
                SetZero();
                return;
            }

            if (significand.Length > MAX_SIGNIFICANT_LENGTH)
            {
                abandonedNumber = significand.Substring(MAX_SIGNIFICANT_LENGTH);
                significand = significand.Substring(0, MAX_SIGNIFICANT_LENGTH); // J2N: Checked 2nd param
            }

            mantissa = Int64.Parse(significand, HEX_RADIX);

            if (exponent >= 1)
            {
                ProcessNormalNumber();
            }
            else
            {
                ProcessSubNormalNumber();
            }

        }

        /// <summary>
        /// Parses the mantissa field.
        /// </summary>
        /// <param name="significantStr">The significant <see cref="string"/>.</param>
        /// <param name="info">The culture sensitive number format information.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise <c>false</c>. Note that this method sets the <see cref="mantissa"/> if successful.</returns>
        private bool TryParseMantissa(string significantStr, NumberFormatInfo info)
        {
            string[] strings = significantStr.Split(new string[] { info.NumberDecimalSeparator }, options: StringSplitOptions.RemoveEmptyEntries); //$NON-NLS-1$
            string strIntegerPart = strings[0].Trim();
            string strDecimalPart = strings.Length > 1 ? strings[1].Trim() : string.Empty; //$NON-NLS-1$

            string significand = GetNormalizedSignificand(strIntegerPart, strDecimalPart);
            if (significand.Equals("0"))
            { //$NON-NLS-1$
                SetZero();
                return true;
            }

            if (!TryGetOffset(strIntegerPart, strDecimalPart, out int offset))
            {
                return false;
            }
            CheckedAddExponent(offset);

            if (exponent >= MAX_EXPONENT)
            {
                SetInfinite();
                return true;
            }

            if (exponent <= MIN_EXPONENT)
            {
                SetZero();
                return true;
            }

            if (significand.Length > MAX_SIGNIFICANT_LENGTH)
            {
                abandonedNumber = significand.Substring(MAX_SIGNIFICANT_LENGTH);
                significand = significand.Substring(0, MAX_SIGNIFICANT_LENGTH); // J2N: Checked 2nd param
            }

            if (!Int64.TryParse(significand, HEX_RADIX, out mantissa))
            {
                return false;
            }

            if (exponent >= 1)
            {
                ProcessNormalNumber();
            }
            else
            {
                ProcessSubNormalNumber();
            }
            return true;
        }

        private void SetInfinite()
        {
            exponent = MAX_EXPONENT;
            mantissa = 0;
        }

        private void SetZero()
        {
            exponent = 0;
            mantissa = 0;
        }

        /// <summary>
        /// Sets the exponent variable to <see cref="long.MaxValue"/> or -<see cref="long.MaxValue"/> if
        /// overflow or underflow happens.
        /// </summary>
        /// <param name="offset"></param>
        private void CheckedAddExponent(long offset)
        {
            long result = exponent + offset;
            int expSign = exponent.Signum();
            if (expSign * offset.Signum() > 0 && expSign * result.Signum() < 0)
            {
                exponent = expSign * long.MaxValue;
            }
            else
            {
                exponent = result;
            }
        }

        private void ProcessNormalNumber()
        {
            int desiredWidth = MANTISSA_WIDTH + 2;
            FitMantissaInDesiredWidth(desiredWidth);
            Round();
            mantissa = mantissa & MANTISSA_MASK;
        }

        private void ProcessSubNormalNumber()
        {
            int desiredWidth = MANTISSA_WIDTH + 1;
            desiredWidth += (int)exponent;//lends bit from mantissa to exponent
            exponent = 0;
            FitMantissaInDesiredWidth(desiredWidth);
            Round();
            mantissa = mantissa & MANTISSA_MASK;
        }

        /// <summary>
        /// Adjusts the mantissa to desired width for further analysis.
        /// </summary>
        private void FitMantissaInDesiredWidth(int desiredWidth)
        {
            int bitLength = CountBitsLength(mantissa);
            if (bitLength > desiredWidth)
            {
                DiscardTrailingBits(bitLength - desiredWidth);
            }
            else
            {
                mantissa <<= (desiredWidth - bitLength);
            }
        }

        /// <summary>
        /// Stores the discarded bits to abandonedNumber.
        /// </summary>
        private void DiscardTrailingBits(int num)
        {
            long mask = ~(-1L << num);
            abandonedNumber += (mantissa & mask);
            mantissa >>= num;
        }

        //private static readonly Regex Zeros = new Regex("0+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// The value is rounded up or down to the nearest infinitely precise result.
        /// If the value is exactly halfway between two infinitely precise results,
        /// then it should be rounded up to the nearest infinitely precise even.
        /// </summary>
        private void Round()
        {
            //string result = abandonedNumber.replaceAll("0+", ""); //$NON-NLS-1$ //$NON-NLS-2$
            //string result = Zeros.Replace(abandonedNumber, "");
            string result = abandonedNumber.Replace("0", string.Empty);
            bool moreThanZero = (result.Length > 0 ? true : false);

            int lastDiscardedBit = (int)(mantissa & 1L);
            mantissa >>= 1;
            int tailBitInMantissa = (int)(mantissa & 1L);

            if (lastDiscardedBit == 1 && (moreThanZero || tailBitInMantissa == 1))
            {
                int oldLength = CountBitsLength(mantissa);
                mantissa += 1L;
                int newLength = CountBitsLength(mantissa);

                //Rounds up to exponent when whole bits of mantissa are one-bits.
                if (oldLength >= MANTISSA_WIDTH && newLength > oldLength)
                {
                    CheckedAddExponent(1);
                }
            }
        }

        /// <summary>
        /// Returns the normalized significand after removing the leading zeros.
        /// </summary>
        /// <param name="strIntegerPart">The integer part, as a <see cref="string"/>.</param>
        /// <param name="strDecimalPart">The decimal part, as a <see cref="string"/>.</param>
        /// <returns>The normalized significand after removing the leading zeros.</returns>
        private string GetNormalizedSignificand(string strIntegerPart, string strDecimalPart)
        {
            string significand = strIntegerPart + strDecimalPart;
            significand = LeadingZeros.Replace(significand, "", 1); //$NON-NLS-1$//$NON-NLS-2$
            if (significand.Length == 0)
            {
                significand = "0"; //$NON-NLS-1$
            }
            return significand;
        }

        private static readonly Regex LeadingZeros = new Regex("^0+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /*
         * Calculates the offset between the normalized number and unnormalized
         * number. In a normalized representation, significand is represented by the
         * characters "0x1." followed by a lowercase hexadecimal representation of
         * the rest of the significand as a fraction.
         */
        private int GetOffset(string strIntegerPart, string strDecimalPart)
        {
            //strIntegerPart = strIntegerPart.replaceFirst("^0+", "");

            strIntegerPart = LeadingZeros.Replace(strIntegerPart, "", 1); //$NON-NLS-1$ //$NON-NLS-2$

            //If the Integer part is a nonzero number.
            if (strIntegerPart.Length != 0)
            {
                string leadingNumber2 = strIntegerPart.Substring(0, 1); // J2N: Checked 2nd param
                return (strIntegerPart.Length - 1) * 4 + CountBitsLength(Int64.Parse(leadingNumber2, HEX_RADIX)) - 1;
            }

            //If the Integer part is a zero number.
            int i;
            for (i = 0; i < strDecimalPart.Length && strDecimalPart[i] == '0'; i++) ;
            if (i == strDecimalPart.Length)
            {
                return 0;
            }
            string leadingNumber = strDecimalPart.Substring(i, 1); // J2N: Corrected 2nd parameter
            return (-i - 1) * 4 + CountBitsLength(Int64.Parse(leadingNumber, HEX_RADIX)) - 1;
        }

        /// <summary>
        /// Calculates the offset between the normalized number and unnormalized
        /// number. In a normalized representation, significand is represented by the
        /// characters "0x1." followed by a lowercase hexadecimal representation of
        /// the rest of the significand as a fraction.
        /// </summary>
        /// <param name="strIntegerPart">The integer part, as a <see cref="string"/>.</param>
        /// <param name="strDecimalPart">The decimal part, as a <see cref="string"/>.</param>
        /// <param name="result">The offset between the nomralized number and unnormalized number.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise <c>false</c>.</returns>
        private bool TryGetOffset(string strIntegerPart, string strDecimalPart, out int result)
        {
            result = default;
            string leadingNumber;
            long leadingNumberValue;

            strIntegerPart = LeadingZeros.Replace(strIntegerPart, "", 1);

            //If the Integer part is a nonzero number.
            if (strIntegerPart.Length != 0)
            {
                leadingNumber = strIntegerPart.Substring(0, 1); // J2N: Checked 2nd param
                if (!Int64.TryParse(leadingNumber, HEX_RADIX, out leadingNumberValue))
                {
                    return false;
                }
                result = (strIntegerPart.Length - 1) * 4 + CountBitsLength(leadingNumberValue) - 1;
                return true;
            }

            //If the Integer part is a zero number.
            int i;
            for (i = 0; i < strDecimalPart.Length && strDecimalPart[i] == '0'; i++) ;
            if (i == strDecimalPart.Length)
            {
                result = 0;
                return true;
            }
            leadingNumber = strDecimalPart.Substring(i, 1); // J2N: Corrected 2nd parameter
            if (!Int64.TryParse(leadingNumber, HEX_RADIX, out leadingNumberValue))
            {
                return false;
            }
            result = (-i - 1) * 4 + CountBitsLength(leadingNumberValue) - 1;
            return true;
        }

        private int CountBitsLength(long value)
        {
            int leadingZeros = BitOperation.LeadingZeroCount(value);
            return Int64.SIZE - leadingZeros;
        }
    }
}
