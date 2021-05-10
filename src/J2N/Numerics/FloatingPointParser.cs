using J2N.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    /// <summary>
    /// Used to parse a string and return either a single or double precision
    /// floating point number.
    /// </summary>
    internal sealed class FloatingPointParser
    {
        /*
     * All number with exponent larger than MAX_EXP can be treated as infinity.
     * All number with exponent smaller than MIN_EXP can be treated as zero.
     * Exponent is 10 based.
     * Eg. double's min value is 5e-324, so double "1e-325" should be parsed as 0.0 
     */
        private const int FLOAT_MIN_EXP = -46;
        private const int FLOAT_MAX_EXP = 38;
        private const int DOUBLE_MIN_EXP = -324;
        private const int DOUBLE_MAX_EXP = 308;

        private sealed class StringExponentPair
        {
            internal string s;

            internal int e;

            internal bool negative;

            internal StringExponentPair(string s, int e, bool negative)
            {
                this.s = s;
                this.e = e;
                this.negative = negative;
            }
        }

        /////**
        //// * Takes a String and an integer exponent. The String should hold a positive
        //// * integer value (or zero). The exponent will be used to calculate the
        //// * floating point number by taking the positive integer the String
        //// * represents and multiplying by 10 raised to the power of the
        //// * exponent. Returns the closest double value to the real number
        //// * 
        //// * @param s
        //// *            the String that will be parsed to a floating point
        //// * @param e
        //// *            an int represent the 10 to part
        //// * @return the double closest to the real number
        //// * 
        //// * @exception NumberFormatException
        //// *                if the String doesn't represent a positive integer value
        //// */
        ////private static native double parseDblImpl(String s, int e);

        /////**
        //// * Takes a String and an integer exponent. The String should hold a positive
        //// * integer value (or zero). The exponent will be used to calculate the
        //// * floating point number by taking the positive integer the String
        //// * represents and multiplying by 10 raised to the power of the
        //// * exponent. Returns the closest float value to the real number
        //// * 
        //// * @param s
        //// *            the String that will be parsed to a floating point
        //// * @param e
        //// *            an int represent the 10 to part
        //// * @return the float closest to the real number
        //// * 
        //// * @exception NumberFormatException
        //// *                if the String doesn't represent a positive integer value
        //// */
        ////private static native float parseFltImpl(String s, int e);

        /**
         * Takes a String and does some initial parsing. Should return a
         * StringExponentPair containing a String with no leading or trailing white
         * space and trailing zeroes eliminated. The exponent of the
         * StringExponentPair will be used to calculate the floating point number by
         * taking the positive integer the String represents and multiplying by 10
         * raised to the power of the exponent.
         * 
         * @param s
         *            the String that will be parsed to a floating point
         * @param length
         *            the length of s
         * @return a StringExponentPair with necessary values
         * 
         * @exception NumberFormatException
         *                if the String doesn't pass basic tests
         */
        private static StringExponentPair InitialParse(String s, int length)
        {
            bool negative = false;
            char c;
            int start, end, @decimal, shift;
            int e = 0;

            start = 0;
            if (length == 0)
                throw new FormatException(s);

            c = s[length - 1];
            if (c == 'D' || c == 'd' || c == 'F' || c == 'f')
            {
                length--;
                if (length == 0)
                    throw new FormatException(s);
            }

            end = Math.Max(s.IndexOf('E'), s.IndexOf('e'));
            if (end > -1)
            {
                if (end + 1 == length)
                    throw new FormatException(s);

                int exponent_offset = end + 1;
                if (s[exponent_offset] == '+')
                {
                    if (s[exponent_offset + 1] == '-')
                    {
                        throw new FormatException(s);
                    }
                    exponent_offset++; // skip the plus sign
                    if (exponent_offset == length)
                        throw new FormatException(s);
                }
                string strExp = s.Substring(exponent_offset, length - exponent_offset); // J2N: Corrected 2nd parameter
                try
                {
                    e = Int32.ParseInt32(strExp);
                }
                catch (FormatException) // J2N TODO: TryParse
                {
                    // strExp is not empty, so there are 2 situations the exception be thrown
                    // if the string is invalid we should throw exception, if the actual number
                    // is out of the range of Integer, we can still parse the original number to
                    // double or float
                    char ch;
                    for (int i = 0; i < strExp.Length; i++)
                    {
                        ch = strExp[i];
                        if (ch < '0' || ch > '9')
                        {
                            if (i == 0 && ch == '-')
                                continue;
                            // ex contains the exponent substring
                            // only so throw a new exception with
                            // the correct string
                            throw new FormatException(s);
                        }
                    }
                    e = strExp[0] == '-' ? int.MinValue : int.MaxValue;
                }
            }
            else
            {
                end = length;
            }
            if (length == 0)
                throw new FormatException(s);

            c = s[start];
            if (c == '-')
            {
                ++start;
                --length;
                negative = true;
            }
            else if (c == '+')
            {
                ++start;
                --length;
            }
            if (length == 0)
                throw new FormatException(s);

            @decimal = s.IndexOf('.');
            if (@decimal > -1)
            {
                shift = end - @decimal - 1;
                //prevent e overflow, shift >= 0
                if (e >= 0 || e - int.MinValue > shift)
                {
                    e -= shift;
                }
                s = s.Substring(start, @decimal - start) + s.Substring(@decimal + 1, end - (@decimal + 1)); // J2N: Corrected 2nd parameters
            }
            else
            {
                s = s.Substring(start, end - start); // J2N: Corrected 2nd parameter
            }

            if ((length = s.Length) == 0)
                throw new FormatException();

            end = length;
            while (end > 1 && s[end - 1] == '0')
                --end;

            start = 0;
            while (start < end - 1 && s[start] == '0')
                start++;

            if (end != length || start != 0)
            {
                shift = length - end;
                if (e <= 0 || int.MaxValue - e > shift)
                {
                    e += shift;
                }
                s = s.Substring(start, end - start);
            }

            // Trim the length of very small numbers, natives can only handle down
            // to E-309
            int APPROX_MIN_MAGNITUDE = -359;
            int MAX_DIGITS = 52;
            length = s.Length;
            if (length > MAX_DIGITS && e < APPROX_MIN_MAGNITUDE)
            {
                int d = Math.Min(APPROX_MIN_MAGNITUDE - e, length - 1);
                s = s.Substring(0, length - d); // J2N: Checked 2nd parameter
                e += d;
            }

            return new StringExponentPair(s, e, negative);
        }

        /*
         * Assumes the string is trimmed.
         */
        private static double parseDblName(string namedDouble, int length)
        {
            // Valid strings are only +Nan, NaN, -Nan, +Infinity, Infinity,
            // -Infinity.
            if ((length != 3) && (length != 4) && (length != 8) && (length != 9))
            {
                throw new FormatException();
            }

            bool negative = false;
            int cmpstart = 0;
            switch (namedDouble[0])
            {
                case '-':
                    negative = true; // fall through
                    cmpstart = 1;
                    break;
                case '+':
                    cmpstart = 1;
                    break;
                default:
                    break;
            }

            if (namedDouble.RegionMatches(/*false,*/ cmpstart, "Infinity", 0, 8, StringComparison.Ordinal))
            {
                return negative ? double.NegativeInfinity
                        : double.PositiveInfinity;
            }

            if (namedDouble.RegionMatches(/*false,*/ cmpstart, "NaN", 0, 3, StringComparison.Ordinal))
            {
                return double.NaN;
            }

            throw new FormatException();
        }

        /*
         * Assumes the string is trimmed.
         */
        private static float ParseFltName(String namedFloat, int length)
        {
            // Valid strings are only +Nan, NaN, -Nan, +Infinity, Infinity,
            // -Infinity.
            if ((length != 3) && (length != 4) && (length != 8) && (length != 9))
            {
                throw new FormatException();
            }

            bool negative = false;
            int cmpstart = 0;
            switch (namedFloat[0])
            {
                case '-':
                    negative = true; // fall through
                    cmpstart = 1;
                    break;
                case '+':
                    cmpstart = 1;
                    break;
                default:
                    break;
            }

            if (namedFloat.RegionMatches(/*false,*/ cmpstart, "Infinity", 0, 8, StringComparison.Ordinal))
            {
                return negative ? float.NegativeInfinity : float.PositiveInfinity;
            }

            if (namedFloat.RegionMatches(/*false,*/ cmpstart, "NaN", 0, 3, StringComparison.Ordinal))
            {
                return float.NaN;
            }

            throw new FormatException();
        }

        /*
         * Answers true if the string should be parsed as a hex encoding.
         * Assumes the string is trimmed.
         */
        internal static bool ParseAsHex(string s)
        {
            int length = s.Length;
            if (length < 2)
            {
                return false;
            }
            char first = s[0];
            char second = s[1];
            if (first == '+' || first == '-')
            {
                // Move along
                if (length < 3)
                {
                    return false;
                }
                first = second;
                second = s[2];
            }
            return (first == '0') && (second == 'x' || second == 'X');
        }

        /**
         * Returns the closest double value to the real number in the string.
         * 
         * @param s
         *            the String that will be parsed to a floating point
         * @return the double closest to the real number
         * 
         * @exception NumberFormatException
         *                if the String doesn't represent a double
         */
        public static double ParseDouble(string s, IFormatProvider? provider)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));

            s = s.Trim();
            int length = s.Length;

            if (length == 0)
            {
                throw new FormatException(s);
            }

            //// See if this could be a named double
            //char last = s[length - 1];
            //if ((last == 'y') || (last == 'N'))
            //{
            //    return parseDblName(s, length);
            //}

            // See if it could be a hexadecimal representation
            if (ParseAsHex(s))
            {
                return HexStringParser.ParseDouble(s);
            }

            //StringExponentPair info = initialParse(s, length);

            //// two kinds of situation will directly return 0.0
            //// 1. info.s is 0
            //// 2. actual exponent is less than Double.MIN_EXPONENT
            //if ("0".Equals(info.s) || (info.e + info.s.Length - 1 < DOUBLE_MIN_EXP))
            //{
            //    return info.negative ? -0.0 : 0.0;
            //}
            //// if actual exponent is larger than Double.MAX_EXPONENT, return infinity
            //// prevent overflow, check twice
            //if ((info.e > DOUBLE_MAX_EXP) || (info.e + info.s.Length - 1 > DOUBLE_MAX_EXP))
            //{
            //    return info.negative ? double.NegativeInfinity : double.PositiveInfinity;
            //}
            //double result = parseDblImpl(info.s, info.e);
            //if (info.negative)
            //    result = -result;

            //return result;

            return double.Parse(s, provider);
        }

        /**
         * Returns the closest float value to the real number in the string.
         * 
         * @param s
         *            the String that will be parsed to a floating point
         * @return the float closest to the real number
         * 
         * @exception NumberFormatException
         *                if the String doesn't represent a float
         */
        public static float ParseFloat(string s, IFormatProvider? provider)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));

            s = s.Trim();
            int length = s.Length;

            if (length == 0)
            {
                throw new FormatException(s);
            }

            //// See if this could be a named float
            //char last = s[length - 1];
            //if ((last == 'y') || (last == 'N'))
            //{
            //    return parseFltName(s, length);
            //}

            // See if it could be a hexadecimal representation
            if (ParseAsHex(s))
            {
                return HexStringParser.ParseFloat(s);
            }

            //StringExponentPair info = initialParse(s, length);

            //// two kinds of situation will directly return 0.0f
            //// 1. info.s is 0
            //// 2. actual exponent is less than Float.MIN_EXPONENT
            //if ("0".equals(info.s) || (info.e + info.s.length() - 1 < FLOAT_MIN_EXP))
            //{
            //    return info.negative ? -0.0f : 0.0f;
            //}
            //// if actual exponent is larger than Float.MAX_EXPONENT, return infinity
            //// prevent overflow, check twice
            //if ((info.e > FLOAT_MAX_EXP) || (info.e + info.s.length() - 1 > FLOAT_MAX_EXP))
            //{
            //    return info.negative ? Float.NEGATIVE_INFINITY : Float.POSITIVE_INFINITY;
            //}
            //float result = parseFltImpl(info.s, info.e);
            //if (info.negative)
            //    result = -result;

            //return result;

            return float.Parse(s, provider);
        }
    }
}
