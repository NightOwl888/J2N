// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        internal abstract class FloatingPointHexNumberBuffer
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


            private long sign;

            private long exponent;

            private long mantissa;

            private string abandonedNumber = ""; //$NON-NLS-1$

            internal FloatingPointHexNumberBuffer(int bufferLength, FloatingPointInfo floatingPointInfo)
            {
                IntegerPart = new char[bufferLength];
                DecimalPart = new char[bufferLength];
                Significand = new char[bufferLength];
                Exponent = new char[bufferLength];

                FloatingPointInfo = floatingPointInfo;
            }

            protected virtual bool TryParse(out long result)
            {
                result = default;
                sign = IsNegative ? 1 : 0; //$NON-NLS-1$
                ParseExponent();
                if (!TryParseMantissa())
                {
                    return false;
                }

                sign <<= (FloatingPointInfo.DenormalMantissaBits + FloatingPointInfo.ExponentBits);
                exponent <<= FloatingPointInfo.DenormalMantissaBits;
                result = (sign | exponent | mantissa);
                return true;
            }

            /// <summary>
            /// Parses the exponent field.
            /// </summary>
            private void ParseExponent()
            {
                // No exponent, raise to the power of 1
                if (ExponentLength == 0)
                {
                    exponent = 1;
                    CheckedAddExponent(FloatingPointInfo.ExponentBias);
                    return;
                }

                int expSign = ExponentIsNegative ? -1 : 1;
                if (Int64.TryParse(Exponent, 0, ExponentLength, radix: 10, out long tempExponent))
                {
                    exponent = expSign * tempExponent;
                    CheckedAddExponent(FloatingPointInfo.ExponentBias);
                }
                else
                {
                    exponent = expSign * long.MaxValue;
                }
            }


            /// <summary>
            /// Parses the mantissa field.
            /// </summary>
            private bool TryParseMantissa()
            {
                if (SignificandIsZero)
                { //$NON-NLS-1$
                    SetZero();
                    return true;
                }

                if (!TryGetOffset(out int offset))
                {
                    return false;
                }
                CheckedAddExponent(offset);

                if (exponent >= FloatingPointInfo.MaxExponent)
                {
                    SetInfinite();
                    return true;
                }

                if (exponent <= FloatingPointInfo.MinExponent)
                {
                    SetZero();
                    return true;
                }

                if (SignificandLength > MaxSignificantLength)
                {
                    abandonedNumber = new string(Significand, MaxSignificantLength, SignificandLength - MaxSignificantLength);
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
                int i;
                for (i = 0; i < abandonedNumber.Length && abandonedNumber[i] == '0'; i++) ;
                bool moreThanZero = i != abandonedNumber.Length;

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

            // J2N: The significand is normalized on the first pass using DotNetNumber.TryParseFloatingPointHexNumber(), so there
            // is no need for a separate method for it here.

            /// <summary>
            /// Calculates the offset between the normalized number and unnormalized
            /// number. In a normalized representation, significand is represented by the
            /// characters "0x1." followed by a lowercase hexadecimal representation of
            /// the rest of the significand as a fraction.
            /// </summary>
            private bool TryGetOffset(out int result)
            {
                result = default;
                long leadingNumberValue;

                //If the Integer part is a nonzero number.
                if (!IntegerPartIsZero)
                {
                    if (!Int64.TryParse(IntegerPart, 0, 1, HexRadix, out leadingNumberValue))
                    {
                        return false;
                    }
                    result = (IntegerPartLength - 1) * 4 + CountBitsLength(leadingNumberValue) - 1;
                    return true;
                }

                //If the Integer part is a zero number.
                if (DecimalPartIsZero)
                {
                    result = 0;
                    return true;
                }
                if (!Int64.TryParse(DecimalPart, 0, 1, HexRadix, out leadingNumberValue))
                {
                    return false;
                }
                result = (Scale - 1) * 4 + CountBitsLength(leadingNumberValue) - 1;
                return true;
            }

            private int CountBitsLength(long value)
            {
                int leadingZeros = BitOperation.LeadingZeroCount(value);
                return Int64.Size - leadingZeros;
            }
        }

        internal class SingleHexNumberBuffer : FloatingPointHexNumberBuffer
        {
            public SingleHexNumberBuffer(int bufferLength)
                : base(bufferLength, FloatingPointInfo.Single)
            {
            }

            /// <summary>
            /// Parses the hex string to a <see cref="float"/> number.
            /// </summary>
            /// <param name="result">A single-precision floating-point number equivalent to the numeric value or symbol.</param>
            public bool TryGetValue(out float result)
            {
                if (!TryParse(out long value))
                {
                    result = default;
                    return false;
                }
                result = BitConversion.Int32BitsToSingle((int)value);
                return true;
            }
        }

        internal class DoubleNumberBuffer : FloatingPointHexNumberBuffer
        {
            public DoubleNumberBuffer(int bufferLength)
                : base(bufferLength, FloatingPointInfo.Double)
            {
            }

            /// <summary>
            /// Parses the hex string to a <see cref="float"/> number.
            /// </summary>
            /// <param name="result">A double-precision floating-point number equivalent to the numeric value or symbol.</param>
            public bool TryGetValue(out double result)
            {
                if (!TryParse(out long value))
                {
                    result = default;
                    return false;
                }
                result = BitConversion.Int64BitsToDouble(value);
                return true;
            }
        }
    }
}
