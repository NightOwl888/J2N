using J2N.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    internal static partial class DotNetNumber
    {
        // As a shortcut to working out how to convert a floating point hex number to a binary representation, we
        // use similar logic to Apache Harmony by using substrings. However, to reduce the number of allocations,
        // the state machine engine of .NET was used instead of a Regex to generate these substrings, which allows
        // us to break out early if any of the validation checks don't pass rather than having an extremely complex
        // Regex with many optional capturing groups. We store the strings as char[] buffers
        // along with a length so we don't have to re-allocate memory in most cases to analyze them further.
        internal abstract class HexFloatingPointNumberBuffer
        {
            protected const int MaxSignificantLength = 15;
            protected const int HexRadix = 16;

            public int DigitsCount;
            public int Scale; // NOTE: Scale is not the same as in NumberBuffer - we only use it to count leading zeros of the decimal part, but it is ignored by exponent. Exponent is done in this class.
            public bool IsNegative;
            //public bool HasNonZeroTail;

            public FloatingPointInfo FloatingPointInfo;

            public char[] IntegerPart;
            public int IntegerPartLength;
            public bool IntegerPartIsZero = true;

            public char[] DecimalPart;
            public int DecimalPartLength;
            public bool DecimalPartIsZero = true;

            public char[] Significand;
            public int SignificandLength;
            public bool SignificandIsZero = true;

            public char[] Exponent;
            public int ExponentLength;
            public bool ExponentIsNegative;

            internal HexFloatingPointNumberBuffer(int bufferLength, FloatingPointInfo floatingPointInfo)
            {
                IntegerPart = new char[bufferLength];
                DecimalPart = new char[bufferLength];
                Significand = new char[bufferLength];
                Exponent = new char[bufferLength];

                FloatingPointInfo = floatingPointInfo;
            }
        }

        internal class SingleNumberBuffer : HexFloatingPointNumberBuffer
        {
            private long sign;

            private long exponent;

            private long mantissa;

            private string abandonedNumber = ""; //$NON-NLS-1$

            public SingleNumberBuffer(int bufferLength)
                : base(bufferLength, FloatingPointInfo.Single)
            {
            }

            /// <summary>
            /// Parses the hex string to a <see cref="float"/> number.
            /// </summary>
            /// <param name="hexString">The hex string to parse.</param>
            /// <param name="style">A bitwise combination of enumeration values that indicates the permitted format of <paramref name="hexString"/>.</param>
            /// <param name="info">Culture-sensitive number formatting information.</param>
            /// <returns>A single-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="hexString"/>.</returns>
            /// <exception cref="FormatException"></exception>
            public bool TryGetValue(out float result)
            {
                if (!TryParse(out int value))
                {
                    result = default;
                    return false;
                    //DotNetNumber.ThrowFormatException(string.Empty /*hexString*/); // J2N TODO: Need to somehow get the input string here ?
                }
                result = BitConversion.Int32BitsToSingle(value);
                return true;
            }

            private bool TryParse(/*string hexString, NumberStyle styles, NumberFormatInfo info,*/ out int result)
            {
                result = default;

                ////if (!TryGetSegmentsFromHexString(hexString, out string[]? hexSegments))
                ////{
                ////    return false;
                ////}
                ////string signStr = hexSegments[0];
                ////string significantStr = hexSegments[1];
                ////string exponentStr = hexSegments[2];

                ////string leadingWhitespaceStr = hexSegments[0];
                ////string leadingSignStr = hexSegments[1];
                ////string hexSpecifierStr = hexSegments[2];
                ////string significantStr = hexSegments[3];
                ////string exponentStr = hexSegments[4];
                ////string typeSuffixStr = hexSegments[5];
                ////string trailingSignStr = hexSegments[6];
                ////string trailingWhitespaceStr = hexSegments[7];

                //Match matcher = PATTERN.Match(hexString);
                //if (!matcher.Success)
                //{
                //    result = default;
                //    return false;
                //}

                ////string[] hexSegments = new string[3];
                ////hexSegments[0] = matcher.Groups[1].Value;
                ////hexSegments[1] = matcher.Groups[2].Value;
                ////hexSegments[2] = matcher.Groups[3].Value;

                //string leadingWhitespaceStr = matcher.Groups["leadWhite"].Value;
                //string leadingSignStr = matcher.Groups["leadSign"].Value;
                //string hexSpecifierStr = matcher.Groups["hex"].Value;
                //string significantStr = matcher.Groups["significant"].Value;
                ////hexSegments[4] = matcher.Groups[5].Value;
                //string exponentStr = matcher.Groups["exponent"].Value;
                //string typeSuffixStr = matcher.Groups["type"].Value;
                //string trailingSignStr = matcher.Groups["trailSign"].Value;
                //string trailingWhitespaceStr = matcher.Groups["trailWhite"].Value;

                //if (!TryValidateFeature(leadingWhitespaceStr, (styles & NumberStyle.AllowLeadingWhite) != 0))
                //{
                //    return false;
                //}
                //if (!TryParseHexSign(leadingSignStr, trailingSignStr, styles, info))
                //{
                //    return false;
                //}
                //if (!TryValidateFeature(hexSpecifierStr, (styles & NumberStyle.AllowHexSpecifier) != 0))
                //{
                //    return false;
                //}
                sign = IsNegative ? 1 : 0; //$NON-NLS-1$
                ParseExponent(/*exponentStr*/);
                if (!TryParseMantissa(/*significantStr, info*/))
                {
                    return false;
                }
                //if (!TryValidateFeature(typeSuffixStr, (styles & NumberStyle.AllowTrailingFloatType) != 0))
                //{
                //    return false;
                //}
                //if (!TryValidateFeature(trailingWhitespaceStr, (styles & NumberStyle.AllowTrailingWhite) != 0))
                //{
                //    return false;
                //}

                sign <<= (FloatingPointInfo.DenormalMantissaBits + FloatingPointInfo.ExponentBits); // (MANTISSA_WIDTH + EXPONENT_WIDTH);
                exponent <<= FloatingPointInfo.DenormalMantissaBits; //MANTISSA_WIDTH;
                result = (int)(sign | exponent | mantissa);
                return true;
            }


            /// <summary>
            /// Parses the exponent field.
            /// </summary>
            /// <param name="exponentStr"></param>
            protected virtual void ParseExponent(/*string exponentStr*/)
            {
                // No exponent, raise to the power of 1
                //if (exponentStr.Length == 0)
                if (ExponentLength == 0)
                {
                    exponent = 1;
                    CheckedAddExponent(FloatingPointInfo.ExponentBias /*EXPONENT_BASE*/);
                    return;
                }

                //char leadingChar = exponentStr[0];
                //int expSign = (leadingChar == '-' ? -1 : 1);
                int expSign = ExponentIsNegative ? -1 : 1;
                //if (!Character.IsDigit(leadingChar))
                //{
                //    exponentStr = exponentStr.Substring(1);
                //}

                //try
                //{
                //    exponent = expSign * Int64.Parse(exponentStr, CultureInfo.InvariantCulture);
                //    CheckedAddExponent(EXPONENT_BASE);
                //}
                //catch (FormatException)
                //{
                //    exponent = expSign * long.MaxValue;
                //}
                //if (long.TryParse(exponentStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out long tempExponent))
                if (Int64.TryParse(Exponent, 0, ExponentLength, radix: 10, out long tempExponent))
                {
                    exponent = expSign * tempExponent;
                    CheckedAddExponent(FloatingPointInfo.ExponentBias /*EXPONENT_BASE*/);
                }
                else
                {
                    exponent = expSign * long.MaxValue;
                }
            }


            /// <summary>
            /// Parses the mantissa field.
            /// </summary>
            /// <param name="significantStr">The significant <see cref="string"/>.</param>
            /// <param name="info">The culture sensitive number format information.</param>
            /// <returns><c>true</c> if the operation succeeded; otherwise <c>false</c>. Note that this method sets the <see cref="mantissa"/> if successful.</returns>
            protected virtual bool TryParseMantissa(/*string significantStr, NumberFormatInfo info*/)
            {
                //string[] strings = significantStr.Split(new string[] { info.NumberDecimalSeparator }, options: StringSplitOptions.RemoveEmptyEntries); //$NON-NLS-1$
                //string strIntegerPart = strings[0].Trim();
                //string strDecimalPart = strings.Length > 1 ? strings[1].Trim() : string.Empty; //$NON-NLS-1$

                //string significand = GetNormalizedSignificand(strIntegerPart, strDecimalPart);
                //if (significand.Equals("0"))
                if (SignificandIsZero)
                { //$NON-NLS-1$
                    SetZero();
                    return true;
                }

                if (!TryGetOffset(/*strIntegerPart, strDecimalPart,*/ out int offset))
                {
                    return false;
                }
                CheckedAddExponent(offset);

                if (exponent >= FloatingPointInfo.MaxExponent /*MAX_EXPONENT*/)
                {
                    SetInfinite();
                    return true;
                }

                if (exponent <= FloatingPointInfo.MinExponent /*MIN_EXPONENT*/)
                {
                    SetZero();
                    return true;
                }

                if (SignificandLength > MaxSignificantLength)
                {
                    abandonedNumber = new string(Significand, MaxSignificantLength, SignificandLength - MaxSignificantLength); // significand.Substring(MAX_SIGNIFICANT_LENGTH);
                    //significand = significand.Substring(0, MAX_SIGNIFICANT_LENGTH); // J2N: Checked 2nd param
                    SignificandLength = MaxSignificantLength;
                }

                if (!Int64.TryParse(Significand, 0, SignificandLength, HexRadix, out mantissa))
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
                exponent = FloatingPointInfo.MaxExponent;
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
                int desiredWidth = FloatingPointInfo.NormalMantissaBits + 1;
                FitMantissaInDesiredWidth(desiredWidth);
                Round();
                mantissa = mantissa & (long)FloatingPointInfo.DenormalMantissaMask;
            }

            private void ProcessSubNormalNumber()
            {
                int desiredWidth = FloatingPointInfo.DenormalMantissaBits + 1;
                desiredWidth += (int)exponent;//lends bit from mantissa to exponent
                exponent = 0;
                FitMantissaInDesiredWidth(desiredWidth);
                Round();
                mantissa = mantissa & (long)FloatingPointInfo.DenormalMantissaMask;
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
                    if (oldLength >= FloatingPointInfo.DenormalMantissaBits && newLength > oldLength)
                    {
                        CheckedAddExponent(1);
                    }
                }
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
            private bool TryGetOffset(/*string strIntegerPart, string strDecimalPart,*/ out int result)
            {
                result = default;
                //string leadingNumber;
                long leadingNumberValue;

                //strIntegerPart = LeadingZeros.Replace(strIntegerPart, "", 1);

                //If the Integer part is a nonzero number.
                //if (strIntegerPart.Length != 0)
                if (!IntegerPartIsZero)
                {
                    //leadingNumber = strIntegerPart.Substring(0, 1); // J2N: Checked 2nd param
                    //if (!Int64.TryParse(leadingNumber, HEX_RADIX, out leadingNumberValue))
                    if (!Int64.TryParse(IntegerPart, 0, 1, HexRadix, out leadingNumberValue))
                    {
                        return false;
                    }
                    //result = (strIntegerPart.Length - 1) * 4 + CountBitsLength(leadingNumberValue) - 1;
                    result = (IntegerPartLength - 1) * 4 + CountBitsLength(leadingNumberValue) - 1;
                    return true;
                }

                //If the Integer part is a zero number.
                //int i;
                //for (i = 0; i < strDecimalPart.Length && strDecimalPart[i] == '0'; i++) ;
                //if (i == strDecimalPart.Length)
                if (DecimalPartIsZero)
                {
                    result = 0;
                    return true;
                }
                //leadingNumber = strDecimalPart.Substring(i, 1); // J2N: Corrected 2nd parameter
                //if (!Int64.TryParse(leadingNumber, HEX_RADIX, out leadingNumberValue))
                if (!Int64.TryParse(DecimalPart, 0, 1, HexRadix, out leadingNumberValue))
                {
                    return false;
                }
                //int i;
                //for (i = 0; i < DecimalPartLength && DecimalPart[i] == '0'; i++) ;
                //result = (-i - 1) * 4 + CountBitsLength(leadingNumberValue) - 1;
                result = (Scale - 1) * 4 + CountBitsLength(leadingNumberValue) - 1;
                return true;
            }
            private int CountBitsLength(long value)
            {
                int leadingZeros = BitOperation.LeadingZeroCount(value);
                return Int64.SIZE - leadingZeros;
            }
        }
    }
}
