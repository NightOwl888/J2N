using J2N.Text;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using SR2 = J2N.Resources.Strings;


namespace J2N
{
    /// <summary>
    /// Static extensions to supplement <see cref="System.Char"/>. This class also provides a
    /// number of utility methods for working with characters.
    /// <para/>
    /// Character data is based upon the Unicode Standard, 4.0. The Unicode
    /// specification, character tables and other information are available at <a
    /// href="http://www.unicode.org/">http://www.unicode.org/</a>.
    /// <para/>
    /// Unicode characters are referred to as <i>code points</i>. The range of valid
    /// code points is U+0000 to U+10FFFF. The <i>Basic Multilingual Plane (BMP)</i>
    /// is the code point range U+0000 to U+FFFF. Characters above the BMP are
    /// referred to as <i>Supplementary Characters</i>. On the .NET platform, UTF-16
    /// encoding and <see cref="char"/> pairs are used to represent code points in the
    /// supplementary range. A pair of <see cref="char"/> values that represent a
    /// supplementary character are made up of a <i>high surrogate</i> with a value
    /// range of 0xD800 to 0xDBFF and a <i>low surrogate</i> with a value range of
    /// 0xDC00 to 0xDFFF.
    /// <para/>
    /// On the .NET platform a <see cref="char"/> value represents either a single BMP code
    /// point or a UTF-16 unit that's part of a surrogate pair. Typically, the <see cref="string"/>
    /// data type is used to represent all Unicode code points. This class supplements <see cref="System.Char"/>
    /// with static methods to provide support for using <see cref="int"/> type
    /// to represent code points, similar to how it is done in Java.
    /// <para/>
    /// Most of this corresponds to the java.Util.Character type in the JDK.
    /// </summary>
    public static class Character
    {
        private const char charNull = '\0';
        private const char charZero = '0';
        private const char charA = 'a';

        /// <summary>
        /// The minimum radix used for conversions between characters and integers.
        /// </summary>
        public const int MaxRadix = 36;
        /// <summary>
        /// The maximum radix used for conversions between characters and integers.
        /// </summary>
        public const int MinRadix = 2;

        // TODO: Directionality/BiDi

        /// <summary>
        /// The maximum value of a high surrogate or leading surrogate unit in UTF-16
        /// encoding, <c>'\uDBFF'</c>.
        /// </summary>
        public const char MinHighSurrogate = '\uD800';
        /// <summary>
        /// The minimum value of a high surrogate or leading surrogate unit in UTF-16
        /// encoding, <c>'\uD800'</c>.
        /// </summary>
        public const char MaxHighSurrogate = '\uDBFF';

        /// <summary>
        /// The minimum value of a low surrogate or trailing surrogate unit in UTF-16
        /// encoding, <c>'\uDC00'</c>.
        /// </summary>
        public const char MinLowSurrogate = '\uDC00';
        /// <summary>
        /// The maximum value of a low surrogate or trailing surrogate unit in UTF-16
        /// encoding, <c>'\uDFFF'</c>.
        /// </summary>
        public const char MaxLowSurrogate = '\uDFFF';

        /// <summary>
        /// The minimum value of a surrogate unit in UTF-16 encoding, <c>'\uD800'</c>.
        /// </summary>
        public const char MinSurrogate = '\uD800';
        /// <summary>
        /// The maximum value of a surrogate unit in UTF-16 encoding, <c>'\uDFFF'</c>.
        /// </summary>
        public const char MaxSurrogate = '\uDFFF';

        /// <summary>
        /// The minimum value of a supplementary code point, <c>U+010000</c>.
        /// </summary>
        public const int MinSupplementaryCodePoint = 0x010000;

        /// <summary>
        /// The minimum code point value, <c>U+0000</c>.
        /// </summary>
        public const int MinCodePoint = 0x000000;
        /// <summary>
        /// The maximum code point value, <c>U+10FFFF</c>.
        /// </summary>
        public const int MaxCodePoint = 0x10FFFF;

        private const string digitKeys = "0Aa\u0660\u06f0\u0966\u09e6\u0a66\u0ae6\u0b66\u0be7\u0c66\u0ce6\u0d66\u0e50\u0ed0\u0f20\u1040\u1369\u17e0\u1810\uff10\uff21\uff41";

        private static readonly char[] digitValues = "90Z7zW\u0669\u0660\u06f9\u06f0\u096f\u0966\u09ef\u09e6\u0a6f\u0a66\u0aef\u0ae6\u0b6f\u0b66\u0bef\u0be6\u0c6f\u0c66\u0cef\u0ce6\u0d6f\u0d66\u0e59\u0e50\u0ed9\u0ed0\u0f29\u0f20\u1049\u1040\u1371\u1368\u17e9\u17e0\u1819\u1810\uff19\uff10\uff3a\uff17\uff5a\uff37"
            .ToCharArray();

        // Unicode 3.0.0 (NOT the same as Unicode 3.0.1)
        private const string numericKeys = "0Aa\u00b2\u00b9\u00bc\u0660\u06f0\u0966\u09e6\u09f4\u09f9\u0a66\u0ae6\u0b66\u0be7\u0bf1\u0bf2\u0c66\u0ce6\u0d66\u0e50\u0ed0\u0f20\u1040\u1369\u1373\u1374\u1375\u1376\u1377\u1378\u1379\u137a\u137b\u137c\u16ee\u17e0\u1810\u2070\u2074\u2080\u2153\u215f\u2160\u216c\u216d\u216e\u216f\u2170\u217c\u217d\u217e\u217f\u2180\u2181\u2182\u2460\u2474\u2488\u24ea\u2776\u2780\u278a\u3007\u3021\u3038\u3039\u303a\u3280\uff10\uff21\uff41";

        private static readonly char[] numericValues = "90Z7zW\u00b3\u00b0\u00b9\u00b8\u00be\u0000\u0669\u0660\u06f9\u06f0\u096f\u0966\u09ef\u09e6\u09f7\u09f3\u09f9\u09e9\u0a6f\u0a66\u0aef\u0ae6\u0b6f\u0b66\u0bf0\u0be6\u0bf1\u0b8d\u0bf2\u080a\u0c6f\u0c66\u0cef\u0ce6\u0d6f\u0d66\u0e59\u0e50\u0ed9\u0ed0\u0f29\u0f20\u1049\u1040\u1372\u1368\u1373\u135f\u1374\u1356\u1375\u134d\u1376\u1344\u1377\u133b\u1378\u1332\u1379\u1329\u137a\u1320\u137b\u1317\u137c\uec6c\u16f0\u16dd\u17e9\u17e0\u1819\u1810\u2070\u2070\u2079\u2070\u2089\u2080\u215e\u0000\u215f\u215e\u216b\u215f\u216c\u213a\u216d\u2109\u216e\u1f7a\u216f\u1d87\u217b\u216f\u217c\u214a\u217d\u2119\u217e\u1f8a\u217f\u1d97\u2180\u1d98\u2181\u0df9\u2182\ufa72\u2473\u245f\u2487\u2473\u249b\u2487\u24ea\u24ea\u277f\u2775\u2789\u277f\u2793\u2789\u3007\u3007\u3029\u3020\u3038\u302e\u3039\u3025\u303a\u301c\u3289\u327f\uff19\uff10\uff3a\uff17\uff5a\uff37"
            .ToCharArray();

        /// <summary>
        /// Indicates whether <paramref name="codePoint"/> is a valid Unicode code point.
        /// </summary>
        /// <param name="codePoint">The code point to test.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a valid Unicode code point; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsValidCodePoint(int codePoint)
        {
            return (MinCodePoint <= codePoint && MaxCodePoint >= codePoint);
        }

        /// <summary>
        /// Indicates whether <paramref name="codePoint"/> is within the supplementary code
        /// </summary>
        /// <param name="codePoint">The code point to test.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is within the supplementary
        /// code point range; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsSupplementaryCodePoint(int codePoint)
        {
            return (MinSupplementaryCodePoint <= codePoint && MaxCodePoint >= codePoint);
        }

        // IsHighSurrogate - use char.IsHighSurrogate
        // IsLowSurrogate - use char.IsLowSurrogate
        // IsSurrogatePair - use char.IsSurrogatePair

        /// <summary>
        /// Calculates the number of <see cref="char"/> values required to represent the
        /// specified Unicode code point. This method checks if the <paramref name="codePoint"/>
        /// is greater than or equal to <c>0x10000</c>, in which case <c>2</c> is
        /// returned, otherwise <c>1</c>. To test if the code point is valid, use
        /// the <see cref="IsValidCodePoint(int)"/> method.
        /// </summary>
        /// <param name="codePoint">The code point for which to calculate the number of required
        /// chars.</param>
        /// <returns><c>2</c> if <c><paramref name="codePoint"/> >= 0x10000</c>; <c>1</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static int CharCount(int codePoint)
        {
            // A given codepoint can be represented in .NET either by 1 char (up to UTF16),
            // or by if it's a UTF32 codepoint, in which case the current char will be a surrogate
            return codePoint >= MinSupplementaryCodePoint ? 2 : 1;
        }

        /// <summary>
        /// Converts a surrogate pair into a Unicode code point. This method assumes
        /// that the pair are valid surrogates. If the pair are <i>not</i> valid
        /// surrogates, then the result is indeterminate. The
        /// <see cref="char.IsSurrogatePair(char, char)"/> method should be used prior to this
        /// method to validate the pair.
        /// <para/>
        /// Roughly corresponds to <see cref="char.ConvertToUtf32(char, char)"/> in .NET, but
        /// doesn't do any validation on the input parameters.
        /// </summary>
        /// <param name="high">The high surrogate unit.</param>
        /// <param name="low">The low surrogate unit.</param>
        /// <returns>The Unicode code point corresponding to the surrogate unit pair.</returns>
        /// <seealso cref="char.IsSurrogatePair(char, char)"/>.
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static int ToCodePoint(char high, char low)
        {
            // Optimized form of:
            // return ((high - MIN_HIGH_SURROGATE) << 10)
            //         + (low - MIN_LOW_SURROGATE)
            //         + MIN_SUPPLEMENTARY_CODE_POINT;
            return ((high << 10) + low) + (MinSupplementaryCodePoint
                                           - (MinHighSurrogate << 10)
                                           - MinLowSurrogate);
        }

        /// <summary>
        /// Returns the code point at <paramref name="index"/> in the specified sequence of
        /// character units. If the unit at <paramref name="index"/> is a high-surrogate unit,
        /// <c><paramref name="index"/> + 1</c> is less than the length of the sequence and the unit at
        /// <c><paramref name="index"/> + 1</c> is a low-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <paramref name="index"/> is returned.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">the position in <paramref name="seq"/> from which to retrieve the code
        /// point.</param>
        /// <returns>the Unicode code point or <see cref="char"/> value at <paramref name="index"/> in
        /// <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than or equal to the length of <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        public static int CodePointAt(this ICharSequence seq, int index) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (index < 0 || index >= len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char high = seq[index++];
            if (index >= len)
                return high;
            char low = seq[index];
            if (char.IsSurrogatePair(high, low))
                return ToCodePoint(high, low);
            return high;
        }

        /// <summary>
        /// Returns the code point at <paramref name="index"/> in the specified sequence of
        /// character units. If the unit at <paramref name="index"/> is a high-surrogate unit,
        /// <c><paramref name="index"/> + 1</c> is less than the length of the sequence and the unit at
        /// <c><paramref name="index"/> + 1</c> is a low-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <paramref name="index"/> is returned.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">the position in <paramref name="seq"/> from which to retrieve the code
        /// point.</param>
        /// <returns>the Unicode code point or <see cref="char"/> value at <paramref name="index"/> in
        /// <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than or equal to the length of <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        public static int CodePointAt(this char[] seq, int index) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (index < 0 || index >= len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char high = seq[index++];
            if (index >= len)
                return high;
            char low = seq[index];
            if (char.IsSurrogatePair(high, low))
                return ToCodePoint(high, low);
            return high;
        }

        /// <summary>
        /// Returns the code point at <paramref name="index"/> in the specified sequence of
        /// character units. If the unit at <paramref name="index"/> is a high-surrogate unit,
        /// <c><paramref name="index"/> + 1</c> is less than the length of the sequence and the unit at
        /// <c><paramref name="index"/> + 1</c> is a low-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <paramref name="index"/> is returned.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">the position in <paramref name="seq"/> from which to retrieve the code
        /// point.</param>
        /// <returns>the Unicode code point or <see cref="char"/> value at <paramref name="index"/> in
        /// <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than or equal to the length of <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        public static int CodePointAt(this StringBuilder seq, int index) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (index < 0 || index >= len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char high = seq[index++];
            if (index >= len)
                return high;
            char low = seq[index];
            if (char.IsSurrogatePair(high, low))
                return ToCodePoint(high, low);
            return high;
        }

        /// <summary>
        /// Returns the code point at <paramref name="index"/> in the specified sequence of
        /// character units. If the unit at <paramref name="index"/> is a high-surrogate unit,
        /// <c><paramref name="index"/> + 1</c> is less than the length of the sequence and the unit at
        /// <c><paramref name="index"/> + 1</c> is a low-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <paramref name="index"/> is returned.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">the position in <paramref name="seq"/> from which to retrieve the code
        /// point.</param>
        /// <returns>the Unicode code point or <see cref="char"/> value at <paramref name="index"/> in
        /// <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than or equal to the length of <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        public static int CodePointAt(this string seq, int index) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (index < 0 || index >= len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char high = seq[index++];
            if (index >= len)
                return high;
            char low = seq[index];
            if (char.IsSurrogatePair(high, low))
                return ToCodePoint(high, low);
            return high;
        }

        /// <summary>
        /// Returns the code point at <paramref name="index"/> in the specified array of
        /// character units, where <paramref name="index"/> has to be less than <paramref name="limit"/>.
        /// If the unit at <paramref name="index"/> is a high-surrogate unit, <c><paramref name="index"/> + 1</c>
        /// is less than <paramref name="limit"/> and the unit at <c><paramref name="index"/> + 1</c> is a
        /// low-surrogate unit, then the supplementary code point represented by the
        /// pair is returned; otherwise the <see cref="char"/> value at <paramref name="index"/> is
        /// returned.
        /// </summary>
        /// <param name="seq">The source array of <see cref="char"/> units.</param>
        /// <param name="index">The position in <paramref name="seq"/> from which to get the code point.</param>
        /// <param name="limit">The index after the last unit in <paramref name="seq"/> that can be used.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value at <paramref name="index"/> in
        /// <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or is greater than or equal to <paramref name="limit"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="limit"/> is less than zero or greater than the length of <paramref name="seq"/>.
        /// </exception>
        public static int CodePointAt(this char[] seq, int index, int limit)
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            if (index < 0 || index >= limit)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (limit < 0 || limit > seq.Length)
                throw new ArgumentOutOfRangeException(nameof(limit));

            char high = seq[index++];
            if (index >= limit)
                return high;
            char low = seq[index];
            if (char.IsSurrogatePair(high, low))
                return ToCodePoint(high, low);
            return high;
        }

        /// <summary>
        /// Returns the code point that precedes <paramref name="index"/> in the specified
        /// sequence of character units. If the unit at <c><paramref name="index"/> - 1</c> is a
        /// low-surrogate unit, <c><paramref name="index"/> - 2</c> is not negative and the unit at
        /// <c><paramref name="index"/> - 2</c> is a high-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <c><paramref name="index"/> - 1</c> is returned.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">The position in <paramref name="seq"/> following the code
        /// point that should be returned.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value before <paramref name="index"/>
        /// in <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than 1 or greater than
        /// the length of <paramref name="seq"/>.</exception>
        public static int CodePointBefore(this ICharSequence seq, int index) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (index < 1 || index > len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char low = seq[--index];
            if (--index < 0)
                return low;
            char high = seq[index];
            if (char.IsSurrogatePair(high, low))
            {
                return ToCodePoint(high, low);
            }
            return low;
        }

        /// <summary>
        /// Returns the code point that precedes <paramref name="index"/> in the specified
        /// sequence of character units. If the unit at <c><paramref name="index"/> - 1</c> is a
        /// low-surrogate unit, <c><paramref name="index"/> - 2</c> is not negative and the unit at
        /// <c><paramref name="index"/> - 2</c> is a high-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <c><paramref name="index"/> - 1</c> is returned.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">The position in <paramref name="seq"/> following the code
        /// point that should be returned.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value before <paramref name="index"/>
        /// in <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than 1 or greater than
        /// the length of <paramref name="seq"/>.</exception>
        public static int CodePointBefore(this char[] seq, int index) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (index < 1 || index > len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char low = seq[--index];
            if (--index < 0)
                return low;
            char high = seq[index];
            if (char.IsSurrogatePair(high, low))
            {
                return ToCodePoint(high, low);
            }
            return low;
        }

        /// <summary>
        /// Returns the code point that precedes <paramref name="index"/> in the specified
        /// sequence of character units. If the unit at <c><paramref name="index"/> - 1</c> is a
        /// low-surrogate unit, <c><paramref name="index"/> - 2</c> is not negative and the unit at
        /// <c><paramref name="index"/> - 2</c> is a high-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <c><paramref name="index"/> - 1</c> is returned.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">The position in <paramref name="seq"/> following the code
        /// point that should be returned.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value before <paramref name="index"/>
        /// in <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than 1 or greater than
        /// the length of <paramref name="seq"/>.</exception>
        public static int CodePointBefore(this StringBuilder seq, int index) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (index < 1 || index > len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char low = seq[--index];
            if (--index < 0)
                return low;
            char high = seq[index];
            if (char.IsSurrogatePair(high, low))
            {
                return ToCodePoint(high, low);
            }
            return low;
        }

        /// <summary>
        /// Returns the code point that precedes <paramref name="index"/> in the specified
        /// sequence of character units. If the unit at <c><paramref name="index"/> - 1</c> is a
        /// low-surrogate unit, <c><paramref name="index"/> - 2</c> is not negative and the unit at
        /// <c><paramref name="index"/> - 2</c> is a high-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <c><paramref name="index"/> - 1</c> is returned.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">The position in <paramref name="seq"/> following the code
        /// point that should be returned.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value before <paramref name="index"/>
        /// in <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than 1 or greater than
        /// the length of <paramref name="seq"/>.</exception>
        public static int CodePointBefore(this string seq, int index) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (index < 1 || index > len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char low = seq[--index];
            if (--index < 0)
                return low;
            char high = seq[index];
            if (char.IsSurrogatePair(high, low))
            {
                return ToCodePoint(high, low);
            }
            return low;
        }

        /// <summary>
        /// Returns the code point that preceds the <paramref name="index"/> in the specified
        /// array of character units and is not less than <paramref name="start"/>. If the unit
        /// at <c><paramref name="index"/> - 1</c> is a low-surrogate unit, <c><paramref name="index"/> - 2</c> is not
        /// less than <paramref name="start"/> and the unit at <c><paramref name="index"/> - 2</c> is a
        /// high-surrogate unit, then the supplementary code point represented by the
        /// pair is returned; otherwise the <see cref="char"/> value at <c><paramref name="index"/> - 1</c>
        /// is returned.
        /// </summary>
        /// <param name="seq">The source array of <see cref="char"/> units.</param>
        /// <param name="index">the position in <paramref name="seq"/> following the code point that
        /// should be returned.</param>
        /// <param name="start">The index of the first element in <paramref name="seq"/>.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value before <paramref name="index"/>
        /// in <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than or equal to <paramref name="start"/>
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="start"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="start"/> is equal or greater than the length of <paramref name="seq"/>.
        /// </exception>
        public static int CodePointBefore(this char[] seq, int index, int start)
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (start < 0 || start >= len)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (index <= start || index > len)
                throw new ArgumentOutOfRangeException(nameof(index));

            char low = seq[--index];
            if (--index < start)
            {
                return low;
            }
            char high = seq[index];
            if (char.IsSurrogatePair(high, low))
            {
                return ToCodePoint(high, low);
            }
            return low;
        }

        /// <summary>
        /// Converts the specified Unicode code point into a UTF-16 encoded sequence
        /// and copies the value(s) into the char array <paramref name="destination"/>, starting at
        /// index <paramref name="destinationIndex"/>.
        /// </summary>
        /// <param name="codePoint">The Unicode code point to encode.</param>
        /// <param name="destination">The destination array to copy the encoded value into.</param>
        /// <param name="destinationIndex">The index in <paramref name="destination"/> from where to start copying.</param>
        /// <returns>The number of <see cref="char"/> value units copied into <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentException">If <paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="destinationIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="destinationIndex"/> is greater than or equal to <c><paramref name="destination"/>.Length</c>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="destinationIndex"/> equals <c><paramref name="destination"/>.Length - 1</c> when 
        /// <paramref name="codePoint"/> is a supplementary code point (<see cref="IsSupplementaryCodePoint(int)"/>).
        /// </exception>
        public static int ToChars(int codePoint, char[] destination, int destinationIndex)
        {
            if (!IsValidCodePoint(codePoint))
                throw new ArgumentException(J2N.SR.Format(SR2.Argument_InvalidCodePoint, codePoint));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destinationIndex < 0 || destinationIndex >= destination.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));

            if (IsSupplementaryCodePoint(codePoint))
            {
                if (destinationIndex == destination.Length - 1)
                    throw new ArgumentOutOfRangeException(nameof(destinationIndex));
                // See RFC 2781, Section 2.1
                // http://www.faqs.org/rfcs/rfc2781.html
                int cpPrime = codePoint - 0x10000;
                int high = 0xD800 | ((cpPrime >> 10) & 0x3FF);
                int low = 0xDC00 | (cpPrime & 0x3FF);
                destination[destinationIndex] = (char)high;
                destination[destinationIndex + 1] = (char)low;
                return 2;
            }

            destination[destinationIndex] = (char)codePoint;
            return 1;
        }

        /// <summary>
        /// Converts the specified Unicode code point into a UTF-16 encoded sequence
        /// and returns it as a char array.
        /// <para/>
        /// This correponds to <see cref="char.ConvertFromUtf32(int)"/>, but returns
        /// a <see cref="T:char[]"/> rather than a <see cref="string"/>.
        /// </summary>
        /// <param name="codePoint">The Unicode code point to encode.</param>
        /// <returns>
        /// the UTF-16 encoded char sequence. If <paramref name="codePoint"/> is a
        /// supplementary code point (<see cref="IsSupplementaryCodePoint(int)"/>),
        /// then the returned array contains two characters, otherwise it contains
        /// just one character.
        /// </returns>
        /// <exception cref="ArgumentException">If <paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        public static char[] ToChars(int codePoint)
        {
            if (!IsValidCodePoint(codePoint))
            {
                throw new ArgumentException(J2N.SR.Format(SR2.Argument_InvalidCodePoint, codePoint));
            }

            if (IsSupplementaryCodePoint(codePoint))
            {
                int cpPrime = codePoint - 0x10000;
                int high = 0xD800 | ((cpPrime >> 10) & 0x3FF);
                int low = 0xDC00 | (cpPrime & 0x3FF);
                return new char[] { (char)high, (char)low };
            }
            return new char[] { (char)codePoint };
        }

        /// <summary>
        /// Returns the number of Unicode code points in the text range of the specified char sequence.
        /// The text range begins at the specified <paramref name="startIndex"/> and extends for the number
        /// of characters specified in <paramref name="length"/>. 
        /// Unpaired surrogates within the text range count as one code point each.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index. To convert from
        /// Java, use <c>endIndex - startIndex</c> to obtain the length.
        /// </summary>
        /// <param name="seq">The char sequence.</param>
        /// <param name="startIndex">The index to the first char of the text range.</param>
        /// <param name="length">The number of characters to consider in the count from <paramref name="seq"/>.</param>
        /// <returns>The number of Unicode code points in the specified text range.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public static int CodePointCount(this ICharSequence seq, int startIndex, int length) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > len)
                throw new ArgumentOutOfRangeException(nameof(length), SR2.ArgumentOutOfRange_IndexLength);

            int endIndex = startIndex + length;
            int n = length;
            for (int i = startIndex; i < endIndex;)
            {
                if (char.IsHighSurrogate(seq[i++]) && i < endIndex
                    && char.IsLowSurrogate(seq[i]))
                {
                    n--;
                    i++;
                }
            }
            return n;
        }

        /// <summary>
        /// Returns the number of Unicode code points in the text range of the specified char sequence.
        /// The text range begins at the specified <paramref name="startIndex"/> and extends for the number
        /// of characters specified in <paramref name="length"/>. 
        /// Unpaired surrogates within the text range count as one code point each.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index. To convert from
        /// Java, use <c>endIndex - startIndex</c> to obtain the length.
        /// </summary>
        /// <param name="seq">The char sequence.</param>
        /// <param name="startIndex">The index to the first char of the text range.</param>
        /// <param name="length">The number of characters to consider in the count from <paramref name="seq"/>.</param>
        /// <returns>The number of Unicode code points in the specified text range.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public static int CodePointCount(this char[] seq, int startIndex, int length) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > len)
                throw new ArgumentOutOfRangeException(nameof(length), SR2.ArgumentOutOfRange_IndexLength);

            int endIndex = startIndex + length;
            int n = length;
            for (int i = startIndex; i < endIndex;)
            {
                if (char.IsHighSurrogate(seq[i++]) && i < endIndex
                    && char.IsLowSurrogate(seq[i]))
                {
                    n--;
                    i++;
                }
            }
            return n;
        }

        /// <summary>
        /// Returns the number of Unicode code points in the text range of the specified char sequence.
        /// The text range begins at the specified <paramref name="startIndex"/> and extends for the number
        /// of characters specified in <paramref name="length"/>. 
        /// Unpaired surrogates within the text range count as one code point each.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index. To convert from
        /// Java, use <c>endIndex - startIndex</c> to obtain the length.
        /// </summary>
        /// <param name="seq">The char sequence.</param>
        /// <param name="startIndex">The index to the first char of the text range.</param>
        /// <param name="length">The number of characters to consider in the count from <paramref name="seq"/>.</param>
        /// <returns>The number of Unicode code points in the specified text range.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public static int CodePointCount(this StringBuilder seq, int startIndex, int length) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > len)
                throw new ArgumentOutOfRangeException(nameof(length), SR2.ArgumentOutOfRange_IndexLength);

            int endIndex = startIndex + length;
            int n = length;
            for (int i = startIndex; i < endIndex;)
            {
                if (char.IsHighSurrogate(seq[i++]) && i < endIndex
                    && char.IsLowSurrogate(seq[i]))
                {
                    n--;
                    i++;
                }
            }
            return n;
        }

        /// <summary>
        /// Returns the number of Unicode code points in the text range of the specified char sequence.
        /// The text range begins at the specified <paramref name="startIndex"/> and extends for the number
        /// of characters specified in <paramref name="length"/>. 
        /// Unpaired surrogates within the text range count as one code point each.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index. To convert from
        /// Java, use <c>endIndex - startIndex</c> to obtain the length.
        /// </summary>
        /// <param name="seq">The char sequence.</param>
        /// <param name="startIndex">The index to the first char of the text range.</param>
        /// <param name="length">The number of characters to consider in the count from <paramref name="seq"/>.</param>
        /// <returns>The number of Unicode code points in the specified text range.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public static int CodePointCount(this string seq, int startIndex, int length) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int len = seq.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > len)
                throw new ArgumentOutOfRangeException(nameof(length), SR2.ArgumentOutOfRange_IndexLength);

            int endIndex = startIndex + length;
            int n = length;
            for (int i = startIndex; i < endIndex;)
            {
                if (char.IsHighSurrogate(seq[i++]) && i < endIndex
                    && char.IsLowSurrogate(seq[i]))
                {
                    n--;
                    i++;
                }
            }
            return n;
        }

        /// <summary>
        /// Returns the index within the given char sequence that is offset from the given <paramref name="index"/> by
        /// <paramref name="codePointOffset"/> code points. Unpaired surrogates within the text range given by 
        /// <paramref name="index"/> and <paramref name="codePointOffset"/> count as one code point each.
        /// </summary>
        /// <param name="seq">The character sequence.</param>
        /// <param name="index">The index to be offset.</param>
        /// <param name="codePointOffset">The number of code points to look backwards or forwards; may
        /// be a negative or positive value.</param>
        /// <returns>The index within the char sequence, offset by <paramref name="codePointOffset"/> code points.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of the character sequence <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is positive and the subsequence starting with <paramref name="index"/> has fewer than
        /// <paramref name="codePointOffset"/> code points.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is negative and the subsequence before <paramref name="index"/> has fewer than
        /// the absolute value of <paramref name="codePointOffset"/> code points.
        /// </exception>
        public static int OffsetByCodePoints(this ICharSequence seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int length = seq.Length;
            if (index < 0 || index > length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int x = index;
            if (codePointOffset >= 0)
            {
                int i;
                for (i = 0; x < length && i < codePointOffset; i++)
                {
                    if (char.IsHighSurrogate(seq[x++]))
                    {
                        if (x < length && char.IsLowSurrogate(seq[x]))
                        {
                            x++;
                        }
                    }
                }
                if (i < codePointOffset)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            else
            {
                int i;
                for (i = codePointOffset; x > 0 && i < 0; i++)
                {
                    if (char.IsLowSurrogate(seq[--x]))
                    {
                        if (x > 0 && char.IsHighSurrogate(seq[x - 1]))
                        {
                            x--;
                        }
                    }
                }
                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            return x;
        }

        /// <summary>
        /// Returns the index within the given char sequence that is offset from the given <paramref name="index"/> by
        /// <paramref name="codePointOffset"/> code points. Unpaired surrogates within the text range given by 
        /// <paramref name="index"/> and <paramref name="codePointOffset"/> count as one code point each.
        /// </summary>
        /// <param name="seq">The character sequence.</param>
        /// <param name="index">The index to be offset.</param>
        /// <param name="codePointOffset">The number of code points to look backwards or forwards; may
        /// be a negative or positive value.</param>
        /// <returns>The index within the char sequence, offset by <paramref name="codePointOffset"/> code points.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of the character sequence <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is positive and the subsequence starting with <paramref name="index"/> has fewer than
        /// <paramref name="codePointOffset"/> code points.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is negative and the subsequence before <paramref name="index"/> has fewer than
        /// the absolute value of <paramref name="codePointOffset"/> code points.
        /// </exception>
        public static int OffsetByCodePoints(this char[] seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int length = seq.Length;
            if (index < 0 || index > length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int x = index;
            if (codePointOffset >= 0)
            {
                int i;
                for (i = 0; x < length && i < codePointOffset; i++)
                {
                    if (char.IsHighSurrogate(seq[x++]))
                    {
                        if (x < length && char.IsLowSurrogate(seq[x]))
                        {
                            x++;
                        }
                    }
                }
                if (i < codePointOffset)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            else
            {
                int i;
                for (i = codePointOffset; x > 0 && i < 0; i++)
                {
                    if (char.IsLowSurrogate(seq[--x]))
                    {
                        if (x > 0 && char.IsHighSurrogate(seq[x - 1]))
                        {
                            x--;
                        }
                    }
                }
                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            return x;
        }

        /// <summary>
        /// Returns the index within the given char sequence that is offset from the given <paramref name="index"/> by
        /// <paramref name="codePointOffset"/> code points. Unpaired surrogates within the text range given by 
        /// <paramref name="index"/> and <paramref name="codePointOffset"/> count as one code point each.
        /// </summary>
        /// <param name="seq">The character sequence.</param>
        /// <param name="index">The index to be offset.</param>
        /// <param name="codePointOffset">The number of code points to look backwards or forwards; may
        /// be a negative or positive value.</param>
        /// <returns>The index within the char sequence, offset by <paramref name="codePointOffset"/> code points.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of the character sequence <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is positive and the subsequence starting with <paramref name="index"/> has fewer than
        /// <paramref name="codePointOffset"/> code points.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is negative and the subsequence before <paramref name="index"/> has fewer than
        /// the absolute value of <paramref name="codePointOffset"/> code points.
        /// </exception>
        public static int OffsetByCodePoints(this StringBuilder seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int length = seq.Length;
            if (index < 0 || index > length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int x = index;
            if (codePointOffset >= 0)
            {
                int i;
                for (i = 0; x < length && i < codePointOffset; i++)
                {
                    if (char.IsHighSurrogate(seq[x++]))
                    {
                        if (x < length && char.IsLowSurrogate(seq[x]))
                        {
                            x++;
                        }
                    }
                }
                if (i < codePointOffset)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            else
            {
                int i;
                for (i = codePointOffset; x > 0 && i < 0; i++)
                {
                    if (char.IsLowSurrogate(seq[--x]))
                    {
                        if (x > 0 && char.IsHighSurrogate(seq[x - 1]))
                        {
                            x--;
                        }
                    }
                }
                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            return x;
        }

        /// <summary>
        /// Returns the index within the given char sequence that is offset from the given <paramref name="index"/> by
        /// <paramref name="codePointOffset"/> code points. Unpaired surrogates within the text range given by 
        /// <paramref name="index"/> and <paramref name="codePointOffset"/> count as one code point each.
        /// </summary>
        /// <param name="seq">The character sequence.</param>
        /// <param name="index">The index to be offset.</param>
        /// <param name="codePointOffset">The number of code points to look backwards or forwards; may
        /// be a negative or positive value.</param>
        /// <returns>The index within the char sequence, offset by <paramref name="codePointOffset"/> code points.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of the character sequence <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is positive and the subsequence starting with <paramref name="index"/> has fewer than
        /// <paramref name="codePointOffset"/> code points.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is negative and the subsequence before <paramref name="index"/> has fewer than
        /// the absolute value of <paramref name="codePointOffset"/> code points.
        /// </exception>
        public static int OffsetByCodePoints(this string seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            int length = seq.Length;
            if (index < 0 || index > length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int x = index;
            if (codePointOffset >= 0)
            {
                int i;
                for (i = 0; x < length && i < codePointOffset; i++)
                {
                    if (char.IsHighSurrogate(seq[x++]))
                    {
                        if (x < length && char.IsLowSurrogate(seq[x]))
                        {
                            x++;
                        }
                    }
                }
                if (i < codePointOffset)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            else
            {
                int i;
                for (i = codePointOffset; x > 0 && i < 0; i++)
                {
                    if (char.IsLowSurrogate(seq[--x]))
                    {
                        if (x > 0 && char.IsHighSurrogate(seq[x - 1]))
                        {
                            x--;
                        }
                    }
                }
                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            return x;
        }

        /// <summary>
        /// Returns the index within the given <see cref="char"/> subarray <paramref name="seq"/> that is offset from the given <paramref name="index"/>
        /// by <paramref name="codePointOffset"/> code points. The <paramref name="start"/> and <paramref name="count"/> arguments specify
        /// a subarray of the <see cref="char"/> array. Unpaired surrogates within the text range given by index and <paramref name="codePointOffset"/>
        /// count as one code point each.
        /// </summary>
        /// <param name="seq">The character array to find the index in.</param>
        /// <param name="start">The inclusive index that marks the beginning of the subsequence.</param>
        /// <param name="count">The number of <see cref="char"/> values to include within the subsequence.</param>
        /// <param name="index">The start index in the subsequence of the <see cref="char"/> array.</param>
        /// <param name="codePointOffset">The number of code points to look backwards or forwards; may
        /// be a negative or positive value.</param>
        /// <returns>The index in <paramref name="seq"/> that is <paramref name="codePointOffset"/> code points
        /// away from <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="start"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="start"/> plus <paramref name="count"/> is greater than the length of <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than <paramref name="start"/> or greater than <paramref name="start"/>
        /// plus <paramref name="count"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is positive and the text range starting with <paramref name="index"/>
        /// and ending with <c><paramref name="start"/> + <paramref name="count"/> - 1</c> has fewer than
        /// <paramref name="codePointOffset"/> code points.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is negative and the text range starting with <paramref name="start"/>
        /// and ending with <c><paramref name="index"/> - 1</c> has fewer than the absolte value of
        /// <paramref name="codePointOffset"/> code points.
        /// </exception>
        public static int OffsetByCodePoints(this char[] seq, int start, int count,
                                         int index, int codePointOffset)
        {
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count > seq.Length - start || index < start || index > start + count)
                throw new ArgumentOutOfRangeException();

            return OffsetByCodePointsImpl(seq, start, count, index, codePointOffset);
        }

        private static int OffsetByCodePointsImpl(char[] seq, int start, int count,
                                          int index, int codePointOffset)
        {
            int x = index;
            if (codePointOffset >= 0)
            {
                int limit = start + count;
                int i;
                for (i = 0; x < limit && i < codePointOffset; i++)
                {
                    if (char.IsHighSurrogate(seq[x++]) && x < limit && char.IsLowSurrogate(seq[x]))
                    {
                        x++;
                    }
                }
                if (i < codePointOffset)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            else
            {
                int i;
                for (i = codePointOffset; x > start && i < 0; i++)
                {
                    if (char.IsLowSurrogate(seq[--x]) && x > start && char.IsHighSurrogate(seq[x - 1]))
                    {
                        x--;
                    }
                }
                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(codePointOffset));
                }
            }
            return x;
        }

        /// <summary>
        /// Convenience method to determine the value of the specified character
        /// <paramref name="c"/> in the supplied radix. The value of <paramref name="radix"/> must be
        /// between <see cref="MinRadix"/> and <see cref="MaxRadix"/>.
        /// </summary>
        /// <param name="c">The character to determine the value of.</param>
        /// <param name="radix">The radix.</param>
        /// <returns>
        /// The value of <paramref name="c"/> in <paramref name="radix"/> if <paramref name="radix"/> lies
        /// between <see cref="MinRadix"/> and <see cref="MaxRadix"/>; -1 otherwise.
        /// </returns>
        public static int Digit(char c, int radix)
        {
            int result = -1;
            if (radix >= MinRadix && radix <= MaxRadix)
            {
                if (c < 128)
                {
                    // Optimized for ASCII
                    if ('0' <= c && c <= '9')
                    {
                        result = c - '0';
                    }
                    else if ('a' <= c && c <= 'z')
                    {
                        result = c - ('a' - 10);
                    }
                    else if ('A' <= c && c <= 'Z')
                    {
                        result = c - ('A' - 10);
                    }
                    return result < radix ? result : -1;
                }
                result = BinarySearchRange(digitKeys, c);
                if (result >= 0 && c <= digitValues[result * 2])
                {
                    int value = (char)(c - digitValues[result * 2 + 1]);
                    if (value >= radix)
                    {
                        return -1;
                    }
                    return value;
                }
            }
            return -1;
        }

        // TODO: Digit(int, int)

        /// <summary>
        /// Search the sorted characters in the string and return the nearest index.
        /// </summary>
        /// <param name="data">The String to search.</param>
        /// <param name="c">The character to search for.</param>
        /// <returns>The nearest index.</returns>
        private static int BinarySearchRange(string data, char c)
        {
            char value = (char)0;
            int low = 0, mid = -1, high = data.Length - 1;
            while (low <= high)
            {
                mid = (low + high) >> 1;
                value = data[mid];
                if (c > value)
                    low = mid + 1;
                else if (c == value)
                    return mid;
                else
                    high = mid - 1;
            }
            return mid - (c < value ? 1 : 0);
        }

        /// <summary>
        /// Returns the character which represents the specified digit in the
        /// specified radix. The <paramref name="radix"/> must be between <see cref="MinRadix"/> and
        /// <see cref="MaxRadix"/> inclusive; <paramref name="digit"/> must not be negative and
        /// smaller than <paramref name="radix"/>. If any of these conditions does not hold, <c>'\0'</c>
        /// is returned.
        /// </summary>
        /// <param name="digit">The integer value.</param>
        /// <param name="radix">The radix.</param>
        /// <returns>The <see cref="char"/> which represents the <paramref name="digit"/> in the
        /// <paramref name="radix"/>.</returns>
        public static char ForDigit(int digit, int radix)
        {
            // if radix or digit is out of range,
            // return the null character.
            if (radix < MinRadix)
                return charNull;
            if (radix > MaxRadix)
                return charNull;
            if (digit < 0)
                return charNull;
            if (digit >= radix)
                return charNull;

            // if digit is less than 10,
            // return '0' plus digit
            if (digit < 10)
                return (char)((int)charZero + digit);

            // otherwise, return 'a' plus digit.
            return (char)((int)charA + digit - 10);
        }

        /// <summary>
        /// Gets the numeric value of the specified Unicode character.
        /// </summary>
        /// <param name="c">The Unicode character to get the numeric value of.</param>
        /// <returns>A non-negative numeric integer value if a numeric value for
        /// <paramref name="c"/> exists, -1 if there is no numeric value for <paramref name="c"/>,
        /// -2 if the numeric value can not be represented with an integer.</returns>
        public static int GetNumericValue(char c)
        {
            if (c < 128)
            {
                // Optimized for ASCII
                if (c >= '0' && c <= '9')
                {
                    return c - '0';
                }
                if (c >= 'a' && c <= 'z')
                {
                    return c - ('a' - 10);
                }
                if (c >= 'A' && c <= 'Z')
                {
                    return c - ('A' - 10);
                }
                return -1;
            }
            int result = BinarySearchRange(numericKeys, c);
            if (result >= 0 && c <= numericValues[result * 2])
            {
                char difference = numericValues[result * 2 + 1];
                if (difference == 0)
                {
                    return -2;
                }
                // Value is always positive, must be negative value
                if (difference > c)
                {
                    return c - (short)difference;
                }
                return c - difference;
            }
            return -1;
        }

        // TODO: GetNumericValue(int)

        /// <summary>
        /// Gets the general Unicode category of the specified character.
        /// <para/>
        /// Usage Note: A safe way to get unicode category. The .NET <see cref="char.ConvertFromUtf32(int)"/>
        /// method is similar. However, if the value falls between
        /// 0x00d800 and 0x00dfff, that method throws an exception. So this is a wrapper that converts the
        /// codepoint to a char in those cases.
        /// <para/>
        /// This mimics the behavior of the Java Character.GetType(int) method, but returns the .NET <see cref="UnicodeCategory"/>
        /// enumeration for easy consumption.
        /// </summary>
        /// <param name="c">The character to get the category of.</param>
        /// <returns>The <see cref="UnicodeCategory"/> of <paramref name="c"/>.</returns>
        public static UnicodeCategory GetType(char c)
        {
            if (!IsValidCodePoint(c))
                return UnicodeCategory.OtherNotAssigned;
            if (c >= 0x00d800 && c <= 0x00dfff)
                return CharUnicodeInfo.GetUnicodeCategory(c);
            else
                return CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(c), 0);
        }

        /// <summary>
        /// Gets the general Unicode category of the specified code point.
        /// <para/>
        /// Usage Note: A safe way to get unicode category. The .NET <see cref="char.ConvertFromUtf32(int)"/>
        /// method is similar. However, if the value falls between
        /// 0x00d800 and 0x00dfff, that method throws an exception. So this is a wrapper that converts the
        /// codepoint to a char in those cases.
        /// <para/>
        /// This mimics the behavior of the Java Character.GetType(int) method, but returns the .NET <see cref="UnicodeCategory"/>
        /// enumeration for easy consumption.
        /// </summary>
        /// <param name="codePoint">The Unicode code point to get the category of.</param>
        /// <returns>The <see cref="UnicodeCategory"/> of <paramref name="codePoint"/>.</returns>
        public static UnicodeCategory GetType(int codePoint)
        {
            if (!IsValidCodePoint(codePoint))
                return UnicodeCategory.OtherNotAssigned;
            if (codePoint >= 0x00d800 && codePoint <= 0x00dfff)
                return CharUnicodeInfo.GetUnicodeCategory((char)codePoint);
            else
                return CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(codePoint), 0);
        }

        // TODO: GetDirectionality(char)
        // TODO: GetDirectionality(int)
        // TODO: IsMirrored(char)
        // TODO: IsMirrored(int)

        /// <summary>
        /// Indicates whether the specified character is defined in the Unicode
        /// specification.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the general Unicode category of the character is
        /// not <see cref="UnicodeCategory.OtherNotAssigned"/>; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsDefined(char c)
        {
            return GetType(c) != UnicodeCategory.OtherNotAssigned;
        }

        /// <summary>
        /// Indicates whether the specified code point is defined in the Unicode
        /// specification.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if the general Unicode category of the code point is
        /// not <see cref="UnicodeCategory.OtherNotAssigned"/>; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsDefined(int codePoint)
        {
            return GetType(codePoint) != UnicodeCategory.OtherNotAssigned;
        }

        /// <summary>
        /// Indicates whether the specified character is a digit.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a digit; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }

        /// <summary>
        /// Indicates whether the specified code point is a digit.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a digit; <c>false</c> otherwise.</returns>
        public static bool IsDigit(int codePoint)
        {
            // Optimized case for ASCII
            if ('0' <= codePoint && codePoint <= '9')
            {
                return true;
            }
            if (codePoint < 1632)
            {
                return false;
            }
            return GetType(codePoint) == UnicodeCategory.DecimalDigitNumber;
        }

        /// <summary>
        /// Indicates whether the specified character is ignorable in a .NET or
        /// Unicode identifier.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is ignorable; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsIdentifierIgnorable(char c)
        {
            return (c >= 0 && c <= 8) || (c >= 0xe && c <= 0x1b)
                    || (c >= 0x7f && c <= 0x9f) || GetType(c) == UnicodeCategory.Format;
        }

        /// <summary>
        /// Indicates whether the specified code point is ignorable in a .NET or
        /// Unicode identifier.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is ignorable; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsIdentifierIgnorable(int codePoint)
        {
            return (codePoint >= 0 && codePoint <= 8) || (codePoint >= 0xe && codePoint <= 0x1b)
                    || (codePoint >= 0x7f && codePoint <= 0x9f) || GetType(codePoint) == UnicodeCategory.Format;
        }

        /// <summary>
        /// Indicates whether the specified character is an ISO control character.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is an ISO control character; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsISOControl(char c)
        {
            return (c >= 0 && c <= 0x1f) || (c >= 0x7f && c <= 0x9f);
        }

        /// <summary>
        /// Indicates whether the specified code point is an ISO control character.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is an ISO control character; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsISOControl(int codePoint)
        {
            return (codePoint >= 0 && codePoint <= 0x1f) || (codePoint >= 0x7f && codePoint <= 0x9f);
        }

        // TODO: IsCSharpIdentifierPart(char)
        // TODO: IsCSharpIdentifierPart(int)
        // TODO: IsVisualBasicIdentifierPart(char)
        // TODO: IsVisualBasicIdentifierPart(int)

        // TODO: IsCSharpIdentifierStart(char)
        // TODO: IsCSharpIdentifierStart(int)
        // TODO: IsVisualBasicIdentifierStart(char)
        // TODO: IsVisualBasicIdentifierStart(int)


        /// <summary>
        /// Indicates whether the specified character is a letter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a letter; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsLetter(char c)
        {
            return char.IsLetter(c);
        }

        /// <summary>
        /// Indicates whether the specified code point is a letter.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a letter; <c>false</c> otherwise.</returns>
        public static bool IsLetter(int codePoint)
        {
            if (('A' <= codePoint && codePoint <= 'Z') || ('a' <= codePoint && codePoint <= 'z'))
                return true;
            if (codePoint < 128)
                return false;
            UnicodeCategory unicodeCategory = GetType(codePoint);

            return unicodeCategory == UnicodeCategory.LowercaseLetter ||
                   unicodeCategory == UnicodeCategory.UppercaseLetter ||
                   unicodeCategory == UnicodeCategory.TitlecaseLetter ||
                   unicodeCategory == UnicodeCategory.ModifierLetter ||
                   unicodeCategory == UnicodeCategory.OtherLetter;
        }

        /// <summary>
        /// Indicates whether the specified character is a letter or a digit.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a letter or a digit; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsLetterOrDigit(char c)
        {
            return char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// Indicates whether the specified code point is a letter or a digit.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a letter or a digit; <c>false</c> otherwise.</returns>
        public static bool IsLetterOrDigit(int codePoint)
        {
            // Optimized case for ASCII
            if (('A' <= codePoint && codePoint <= 'Z') || ('a' <= codePoint && codePoint <= 'z') || ('0' <= codePoint && codePoint <= '9'))
                return true;
            if (codePoint < 128)
                return false;
            UnicodeCategory unicodeCategory = GetType(codePoint);
            return unicodeCategory == UnicodeCategory.LowercaseLetter ||
                   unicodeCategory == UnicodeCategory.UppercaseLetter ||
                   unicodeCategory == UnicodeCategory.TitlecaseLetter ||
                   unicodeCategory == UnicodeCategory.ModifierLetter ||
                   unicodeCategory == UnicodeCategory.OtherLetter ||
                   unicodeCategory == UnicodeCategory.DecimalDigitNumber;
        }

        /// <summary>
        /// Indicates whether the specified character is a lower case letter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a lower case letter; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsLower(char c)
        {
            return char.IsLower(c);
        }

        /// <summary>
        /// Indicates whether the specified code point is a lower case letter.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a lower case letter; <c>false</c> otherwise.</returns>
        public static bool IsLower(int codePoint)
        {
            // Optimized case for ASCII
            if ('a' <= codePoint && codePoint <= 'z')
                return true;
            if (codePoint < 128)
                return false;

            return GetType(codePoint) == UnicodeCategory.LowercaseLetter;
        }

        /// <summary>
        /// Indicates whether the specified character is a Java space.
        /// </summary>
        /// <param name="ch">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="ch"/> is a Java space; <c>false</c> otherwise.</returns>
        [Obsolete("Use char.IsWhiteSpace()")]
        public static bool IsSpace(char ch)
        {
            return (ch <= 0x0020) &&
                (((((1L << 0x0009) |
                (1L << 0x000A) |
                (1L << 0x000C) |
                (1L << 0x000D) |
                (1L << 0x0020)) >> ch) & 1L) != 0);
        }

        /// <summary>
        /// Indicates whether the specified character is a Unicode space character.
        /// That is, if it is a member of one of the Unicode categories Space
        /// Separator, Line Separator, or Paragraph Separator.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a Unicode space character; <c>false</c> otherwise.</returns>
        public static bool IsSpaceChar(char c)
        {
            if (c == 0x20 || c == 0xa0 || c == 0x1680)
            {
                return true;
            }
            if (c < 0x2000)
            {
                return false;
            }
            return c <= 0x200b || c == 0x2028 || c == 0x2029 || c == 0x202f
                    || c == 0x3000;
        }

        /// <summary>
        /// Indicates whether the specified character is a Unicode space character.
        /// That is, if it is a member of one of the Unicode categories Space
        /// Separator, Line Separator, or Paragraph Separator.
        /// </summary>
        /// <param name="codePoint">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a Unicode space character; <c>false</c> otherwise.</returns>
        public static bool IsSpaceChar(int codePoint)
        {
            if (codePoint == 0x20 || codePoint == 0xa0 || codePoint == 0x1680)
            {
                return true;
            }
            if (codePoint < 0x2000)
            {
                return false;
            }
            UnicodeCategory unicodeCategory = GetType(codePoint);
            return unicodeCategory == UnicodeCategory.SpaceSeparator ||
                unicodeCategory == UnicodeCategory.LineSeparator ||
                unicodeCategory == UnicodeCategory.ParagraphSeparator;
        }

        /// <summary>
        /// Indicates whether the specified character is a titlecase character.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a titlecase character, <c>false</c> otherwise.</returns>
        public static bool IsTitleCase(char c)
        {
            if (c == '\u01c5' || c == '\u01c8' || c == '\u01cb' || c == '\u01f2')
            {
                return true;
            }
            if (c >= '\u1f88' && c <= '\u1ffc')
            {
                // 0x1f88 - 0x1f8f, 0x1f98 - 0x1f9f, 0x1fa8 - 0x1faf
                if (c > '\u1faf')
                {
                    return c == '\u1fbc' || c == '\u1fcc' || c == '\u1ffc';
                }
                int last = c & 0xf;
                return last >= 8 && last <= 0xf;
            }
            return false;
        }

        /// <summary>
        /// Indicates whether the specified code point is a titlecase character.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a titlecase character, <c>false</c> otherwise.</returns>
        public static bool IsTitleCase(int codePoint)
        {
            if (codePoint == '\u01c5' || codePoint == '\u01c8' || codePoint == '\u01cb' || codePoint == '\u01f2')
            {
                return true;
            }
            return GetType(codePoint) == UnicodeCategory.TitlecaseLetter;
        }

        /// <summary>
        /// Indicates whether the specified character is valid as part of a Unicode
        /// identifier other than the first character.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is valid as part of a Unicode
        /// identifier, <c>false</c> otherwise.</returns>
        public static bool IsUnicodeIdentifierPart(char c)
        {
            UnicodeCategory unicodeCategory = GetType(c);
            return (unicodeCategory == UnicodeCategory.LowercaseLetter ||
                unicodeCategory == UnicodeCategory.UppercaseLetter ||
                unicodeCategory == UnicodeCategory.TitlecaseLetter ||
                unicodeCategory == UnicodeCategory.ModifierLetter ||
                unicodeCategory == UnicodeCategory.OtherLetter) ||
                unicodeCategory == UnicodeCategory.ConnectorPunctuation ||
                unicodeCategory == UnicodeCategory.DecimalDigitNumber ||
                unicodeCategory == UnicodeCategory.LetterNumber ||
                unicodeCategory == UnicodeCategory.NonSpacingMark ||
                unicodeCategory == UnicodeCategory.SpacingCombiningMark ||
                IsIdentifierIgnorable(c);
        }

        /// <summary>
        /// Indicates whether the specified code point is valid as part of a Unicode
        /// identifier other than the first character.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is valid as part of a Unicode
        /// identifier, <c>false</c> otherwise.</returns>
        public static bool IsUnicodeIdentifierPart(int codePoint)
        {
            UnicodeCategory unicodeCategory = GetType(codePoint);
            return (unicodeCategory == UnicodeCategory.LowercaseLetter ||
                unicodeCategory == UnicodeCategory.UppercaseLetter ||
                unicodeCategory == UnicodeCategory.TitlecaseLetter ||
                unicodeCategory == UnicodeCategory.ModifierLetter ||
                unicodeCategory == UnicodeCategory.OtherLetter) ||
                unicodeCategory == UnicodeCategory.ConnectorPunctuation ||
                unicodeCategory == UnicodeCategory.DecimalDigitNumber ||
                unicodeCategory == UnicodeCategory.LetterNumber ||
                unicodeCategory == UnicodeCategory.NonSpacingMark ||
                unicodeCategory == UnicodeCategory.SpacingCombiningMark ||
                IsIdentifierIgnorable(codePoint);
        }

        /// <summary>
        /// Indicates whether the specified character is a valid initial character
        /// for a Unicode identifier.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is valid as a first character for a Unicode
        /// identifier, <c>false</c> otherwise.</returns>
        public static bool IsUnicodeIdentifierStart(char c)
        {
            UnicodeCategory unicodeCategory = GetType(c);
            return (unicodeCategory == UnicodeCategory.LowercaseLetter ||
                unicodeCategory == UnicodeCategory.UppercaseLetter ||
                unicodeCategory == UnicodeCategory.TitlecaseLetter ||
                unicodeCategory == UnicodeCategory.ModifierLetter ||
                unicodeCategory == UnicodeCategory.OtherLetter) ||
                unicodeCategory == UnicodeCategory.LetterNumber;
        }

        /// <summary>
        /// Indicates whether the specified code point is a valid initial character
        /// for a Unicode identifier.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is valid as a first character for a Unicode
        /// identifier, <c>false</c> otherwise.</returns>
        public static bool IsUnicodeIdentifierStart(int codePoint)
        {
            UnicodeCategory unicodeCategory = GetType(codePoint);
            return (unicodeCategory == UnicodeCategory.LowercaseLetter ||
                unicodeCategory == UnicodeCategory.UppercaseLetter ||
                unicodeCategory == UnicodeCategory.TitlecaseLetter ||
                unicodeCategory == UnicodeCategory.ModifierLetter ||
                unicodeCategory == UnicodeCategory.OtherLetter) ||
                unicodeCategory == UnicodeCategory.LetterNumber;
        }

        /// <summary>
        /// Indicates whether the specified character is an upper case letter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is an upper case letter, <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsUpper(char c)
        {
            return char.IsUpper(c);
        }

        /// <summary>
        /// Indicates whether the specified code point is an upper case letter.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is an upper case letter, <c>false</c> otherwise.</returns>
        public static bool IsUpper(int codePoint)
        {
            // Optimized case for ASCII
            if ('A' <= codePoint && codePoint <= 'Z')
            {
                return true;
            }
            if (codePoint < 128)
            {
                return false;
            }

            return GetType(codePoint) == UnicodeCategory.UppercaseLetter;
        }

        /// <summary>
        /// Indicates whether the specified character is a whitespace character in
        /// .NET.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a whitespace character, <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsWhiteSpace(char c)
        {
            return char.IsWhiteSpace(c);
        }

        /// <summary>
        /// Indicates whether the specified character is a whitespace character in
        /// .NET.
        /// </summary>
        /// <param name="codePoint">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a whitespace character, <c>false</c> otherwise.</returns>
        public static bool IsWhiteSpace(int codePoint)
        {
            // Optimized case for ASCII
            if ((codePoint >= 0x1c && codePoint <= 0x20) || (codePoint >= 0x9 && codePoint <= 0xd))
                return true;
            if (codePoint == 0x1680)
                return true;
            if (codePoint < 0x2000 || codePoint == 0x2007)
                return false;
            return codePoint <= 0x200b || codePoint == 0x2028 || codePoint == 0x2029 || codePoint == 0x3000;
            // Port Note: char.IsWhiteSpace(char.ConvertFromUtf32(codePoint)) incorrectly returns true for codepoint 160 (and possibly other cases)
        }

        /// <summary>
        /// Reverses the order of the first and second byte in the specified
        /// character.
        /// </summary>
        /// <param name="c">The character to reverse.</param>
        /// <returns>The character with reordered bytes.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static char ReverseBytes(char c)
        {
            return (char)((c << 8) | (c >> 8));
        }

        // J2N NOTE: Harmony didn't implement ToUpper or ToLower, but they
        // are required by Lucene.Net

        /// <summary>
        /// Converts the character (Unicode code point) argument to
        /// lowercase using the current culture.
        /// </summary>
        /// <param name="codePoint">The character (Unicode code point) to be converted.</param>
        /// <returns>The lowercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentException">If the <paramref name="codePoint"/> is invalid.</exception>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static int ToLower(int codePoint) => ToLower(codePoint, CultureInfo.CurrentCulture);

        /// <summary>
        /// Converts the character (Unicode code point) argument to
        /// lowercase using the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="codePoint">The character (Unicode code point) to be converted.</param>
        /// <param name="culture">An object that specifies culture-specific casing rules.</param>
        /// <returns>The lowercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentException">If the <paramref name="codePoint"/> is invalid.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="culture"/> is <c>null</c>.</exception>
        public static int ToLower(int codePoint, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (!IsValidCodePoint(codePoint))
                throw new ArgumentException(J2N.SR.Format(SR2.Argument_InvalidCodePoint, codePoint));

            // Fast path - convert using char if not a surrogate pair
            if (CharCount(codePoint) == 1)
                return culture.TextInfo.ToLower((char)codePoint);
                
            var str = culture.TextInfo.ToLower(char.ConvertFromUtf32(codePoint));
            return CodePointAt(str, 0);
        }

        /// <summary>
        /// Converts the character (Unicode code point) argument to
        /// uppercase using the current culture.
        /// </summary>
        /// <param name="codePoint">The character (Unicode code point) to be converted.</param>
        /// <returns>The uppercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentException">If the <paramref name="codePoint"/> is invalid.</exception>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static int ToUpper(int codePoint) => ToUpper(codePoint, CultureInfo.CurrentCulture);

        /// <summary>
        /// Converts the character (Unicode code point) argument to
        /// uppercase using the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="codePoint">The character (Unicode code point) to be converted.</param>
        /// <param name="culture">An object that specifies culture-specific casing rules.</param>
        /// <returns>The uppercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentException">If the <paramref name="codePoint"/> is invalid.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="culture"/> is <c>null</c>.</exception>
        public static int ToUpper(int codePoint, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (!IsValidCodePoint(codePoint))
                throw new ArgumentException(J2N.SR.Format(SR2.Argument_InvalidCodePoint, codePoint));

            // Fast path - convert using char if not a surrogate pair
            if (CharCount(codePoint) == 1)
                return culture.TextInfo.ToUpper((char)codePoint);

            var str = culture.TextInfo.ToUpper(char.ConvertFromUtf32(codePoint));
            return CodePointAt(str, 0);
        }

        // J2N: Since .NET's string class has no constructor that accepts an array of code points, we have extra helper methods that
        // allow us to make the conversion. Character seems like the most logical place to do this being that there is no way to dynamically
        // add a construtor to System.String and this is the place in J2N that deals the most with code points.

        /// <summary>
        /// Converts an an array <paramref name="codePoints"/> to a <see cref="string"/> of UTF-16 code units.
        /// <para/>
        /// Usage Note: In the JDK, there is a constructor overload that accept code points and turn them into a string. This is
        /// the .NET equivalent of that constructor overload, however this overload is provided for convenience and assumes the
        /// whole array will be converted. <see cref="ToString(int[], int, int)"/> allows conversion of a partial array of code points to a
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="codePoints">An array of UTF-32 code points.</param>
        /// <returns>A string containing the UTF-16 character equivalent of <paramref name="codePoints"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="codePoints"/> is <c>null</c>.</exception>
        public static string ToString(int[] codePoints)
        {
            if (codePoints is null)
                throw new ArgumentNullException(nameof(codePoints));

            return ToString(codePoints, 0, codePoints.Length);
        }

        /// <summary>
        /// Converts an an array <paramref name="codePoints"/> to a <see cref="string"/> of UTF-16 code units.
        /// <para/>
        /// Usage Note: In the JDK, there is a constructor overload that accept code points and turn them into a string. This is
        /// the .NET equivalent of that constructor overload.
        /// </summary>
        /// <param name="codePoints">An array of UTF-32 code points.</param>
        /// <param name="startIndex">The index of the first code point to convert.</param>
        /// <param name="length">The number of code point elements to to convert to UTF-32 code units and include in the result.</param>
        /// <returns>A <see cref="string"/> containing the UTF-16 code units that correspond to the specified range of <paramref name="codePoints"/> elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within <paramref name="codePoints"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="codePoints"/> is <c>null</c>.</exception>
        public static string ToString(int[] codePoints, int startIndex, int length)
        {
            if (codePoints is null)
                throw new ArgumentNullException(nameof(codePoints));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR2.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR2.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > codePoints.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR2.ArgumentOutOfRange_IndexLength);

            int countThreashold = 1024; // If the number of chars exceeds this, we count them instead of allocating count * 2
            
            int arrayLength = 0;
            // if we go over the threashold, count the number of 
            // chars we will need so we can allocate the precise amount of memory
            if (length > countThreashold)
            {
                int end = startIndex + length;
                for (int j = startIndex; j < end; ++j)
                {
                    arrayLength += CharCount(codePoints[j]);
                }
            }
            // If we don't have any characters at this point,
            // as a last resort, assume each codepoint
            // is 2 characters (since it cannot be longer than this)
            if (arrayLength < 1)
            {
                arrayLength = length * 2;
            }

            // Initialize our array to our exact or oversized length.
            // It is now safe to assume we have enough space for all of the characters.
            char[] buffer = new char[arrayLength];
            int totalLength = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                totalLength += ToChars(codePoints[i], buffer, totalLength);
            }
            // Size the result to the exact length that is counted as the code points
            // are converted.
            return new string(buffer, 0, totalLength);
        }
    }
}
