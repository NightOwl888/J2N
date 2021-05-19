using J2N.Text;
using System;
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

        private const string XDIGIT = "(?:[\\p{Nd}A-Fa-f\\u07C0-\\u07C9\\u0BE6\\u0DE6-\\u0DEF\\u1090-\\u1099\\u1946-\\u194F\\u19D0-\\u19D9\\u1A80-\\u1A89\\u1A90-\\u1A99\\u1B50-\\u1B59\\u1BB0-\\u1BB9\\u1C40-\\u1C49\\u1C50-\\u1C59\\uA620-\\uA629\\uA8D0-\\uA8D9\\uA900-\\uA909\\uA9D0-\\uA9D9\\uA9F0-\\uA9F9\\uAA50-\\uAA59\\uABF0-\\uABF9\\uFF21-\\uFF26\\uFF41-\\uFF46]|\\uD801[\\uDCA0-\\uDCA9]|\\uD804[\\uDC66-\\uDC6F]|\\uD804[\\uDCF0-\\uDCF9]|\\uD804[\\uDD36-\\uDD3F]|\\uD804[\\uDDD0-\\uDDD9]|\\uD804[\\uDEF0-\\uDEF9]|\\uD805[\\uDC50-\\uDC59]|\\uD805[\\uDCD0-\\uDCD9]|\\uD805[\\uDE50-\\uDE59]|\\uD805[\\uDEC0-\\uDEC9]|\\uD805[\\uDF30-\\uDF39]|\\uD806[\\uDCE0-\\uDCE9]|\\uD807[\\uDC50-\\uDC59]|\\uD807[\\uDD50-\\uDD59]|\\uD81A[\\uDE60-\\uDE69]|\\uD81A[\\uDF50-\\uDF59]|\\uD835[\\uDFCE-\\uDFFF]|\\uD83A[\\uDD50-\\uDD59])";

        //private const string HEX_SIGNIFICANT = "0[xX](\\p{XDigit}+\\.?|\\p{XDigit}*\\.\\p{XDigit}+)"; //$NON-NLS-1$
        private const string HEX_SIGNIFICANT = "0[xX](" + XDIGIT + "+\\.?|" + XDIGIT + "*\\." + XDIGIT + "+)"; //$NON-NLS-1$

        private const string BINARY_EXPONENT = "[pP]([+-]?\\d+)"; //$NON-NLS-1$

        private const string FLOAT_TYPE_SUFFIX = "[fFdD]?"; //$NON-NLS-1$

        private const string HEX_PATTERN = "[\\x00-\\x20]*([+-]?)" + HEX_SIGNIFICANT //$NON-NLS-1$
                + BINARY_EXPONENT + FLOAT_TYPE_SUFFIX + "[\\x00-\\x20]*"; //$NON-NLS-1$

        private static readonly Regex PATTERN = new Regex(HEX_PATTERN, RegexOptions.Compiled | RegexOptions.CultureInvariant);

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

        /*
         * Parses the hex string to a float number.
         */
        public static float ParseFloat(string hexString)
        {
            HexStringParser parser = new HexStringParser(FLOAT_EXPONENT_WIDTH,
                    FLOAT_MANTISSA_WIDTH);
            int result = (int)parser.Parse(hexString);
            return BitConversion.Int32BitsToSingle(result);
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

        /*
         * Parses the sign field.
         */
        private void ParseHexSign(string signStr)
        {
            this.sign = signStr.Equals("-") ? 1 : 0; //$NON-NLS-1$
        }

        /*
         * Parses the exponent field.
         */
        private void ParseExponent(string exponentStr)
        {
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
                significand = significand.Substring(0, MAX_SIGNIFICANT_LENGTH);
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

        /*
         * Sets the exponent variable to Long.MAX_VALUE or -Long.MAX_VALUE if
         * overflow or underflow happens.
         */
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

        /*
         * Adjusts the mantissa to desired width for further analysis.
         */
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

        /*
         * Stores the discarded bits to abandonedNumber.
         */
        private void DiscardTrailingBits(int num)
        {
            long mask = ~(-1L << num);
            abandonedNumber += (mantissa & mask);
            mantissa >>= num;
        }

        private static readonly Regex Zeros = new Regex("0+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /*
         * The value is rounded up or down to the nearest infinitely precise result.
         * If the value is exactly halfway between two infinitely precise results,
         * then it should be rounded up to the nearest infinitely precise even.
         */
        private void Round()
        {
            //string result = abandonedNumber.replaceAll("0+", ""); //$NON-NLS-1$ //$NON-NLS-2$
            string result = Zeros.Replace(abandonedNumber, "");
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

        /*
         * Returns the normalized significand after removing the leading zeros.
         */
        private string GetNormalizedSignificand(string strIntegerPart, string strDecimalPart)
        {
            string significand = strIntegerPart + strDecimalPart;
            //significand = significand.replaceFirst("^0+", ""); //$NON-NLS-1$//$NON-NLS-2$
            significand = LeadingZeros.Replace(significand, "", 1);
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
            //strIntegerPart = strIntegerPart.replaceFirst("^0+", ""); //$NON-NLS-1$ //$NON-NLS-2$

            strIntegerPart = LeadingZeros.Replace(strIntegerPart, "", 1);

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

        private int CountBitsLength(long value)
        {
            int leadingZeros = BitOperation.LeadingZeroCount(value);
            //return sizeof(long) - leadingZeros;
            return Int64.SIZE - leadingZeros; // J2N TODO: Make constant on Int64
        }
    }
}
