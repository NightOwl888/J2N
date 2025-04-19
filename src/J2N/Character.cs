#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Buffers;
using J2N.Text;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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
        private const int CharStackBufferSize = 64;
        private const int CodePointStackBufferSize = 256;

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

        /// <summary>Map from an ASCII char to its digit value (up to radix 36), e.g. arr['b'] == 11. 0xff means it's not a digit.</summary>
        private static readonly sbyte[] ASCIIDigits = unchecked(new sbyte[] {
            (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff,
            (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, // 15
            (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff,
            (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, // 31
            (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff,
            (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, // 47
            (sbyte)0x00, (sbyte)0x01, (sbyte)0x02, (sbyte)0x03, (sbyte)0x04, (sbyte)0x05, (sbyte)0x06, (sbyte)0x07,
            (sbyte)0x08, (sbyte)0x09, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, // 63
            (sbyte)0xff, (sbyte)0x0a, (sbyte)0x0b, (sbyte)0x0c, (sbyte)0x0d, (sbyte)0x0e, (sbyte)0x0f, (sbyte)0x10,
            (sbyte)0x11, (sbyte)0x12, (sbyte)0x13, (sbyte)0x14, (sbyte)0x15, (sbyte)0x16, (sbyte)0x17, (sbyte)0x18, // 79
            (sbyte)0x19, (sbyte)0x1a, (sbyte)0x1b, (sbyte)0x1c, (sbyte)0x1d, (sbyte)0x1e, (sbyte)0x1f, (sbyte)0x20,
            (sbyte)0x21, (sbyte)0x22, (sbyte)0x23, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, // 95
            (sbyte)0xff, (sbyte)0x0a, (sbyte)0x0b, (sbyte)0x0c, (sbyte)0x0d, (sbyte)0x0e, (sbyte)0x0f, (sbyte)0x10,
            (sbyte)0x11, (sbyte)0x12, (sbyte)0x13, (sbyte)0x14, (sbyte)0x15, (sbyte)0x16, (sbyte)0x17, (sbyte)0x18, // 111
            (sbyte)0x19, (sbyte)0x1a, (sbyte)0x1b, (sbyte)0x1c, (sbyte)0x1d, (sbyte)0x1e, (sbyte)0x1f, (sbyte)0x20,
            (sbyte)0x21, (sbyte)0x22, (sbyte)0x23, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, (sbyte)0xff, // 127
        });

        // Implementation Note:

        // The approach used with these values was to store 3 values for each range. This approach to storage
        // is used in all of the ranges of different characters whether they are stored as strings, or arrays
        // of various types.
        // 
        // The stored values:
        // 1. The beginning of the range (stored in the "keys")
        // 2. The end of the range (stored in "values" in the location key index * 2).
        // 3. The offset of the first character, which is subtracted from the beginning character of the range
        //    (stored in "values" in the location key index * 2 + 1).
        //
        // This allows the return value to be skewed from the actual numerical value of the code point and allows
        // for various rules by adding additional "ranges" of single characters. Some of the characters in an actual
        // range increment by a numerical value of 10 or 100, so these numbers, although contiguous in character value
        // can have a wildly varying numerical value.
        //
        // This is the original approach used in Apache Harmony, which has been carried over into J2N and extended
        // so we could upgrade the Unicode version.


        // Unicode 10.0 (Harmony was at 3.0.0)
        private const string digitKeys = "\u0660\u06f0\u07c0\u0966\u09e6\u0a66\u0ae6\u0b66\u0be6\u0c66\u0ce6\u0d66\u0de6\u0e50\u0ed0\u0f20\u1040\u1090\u17e0\u1810\u1946\u19d0\u1a80\u1a90\u1b50\u1bb0\u1c40\u1c50\ua620\ua8d0\ua900\ua9d0\ua9f0\uaa50\uabf0\uff10\uff21\uff41";

        // J2N NOTE: There were Ethiopian digits (\u1368 - \u1371) in Harmony that don't conform to Unicode and the behavior both of ICU4N
        // and the JDK do not recognize them as digits. Unlike recognizable digits, they are categorized as UUnicodeCategory.OtherNumber.
        // These have been omitted from J2N.
        private static readonly char[] digitValues = "\u0669\u0660\u06f9\u06f0\u07c9\u07c0\u096f\u0966\u09ef\u09e6\u0a6f\u0a66\u0aef\u0ae6\u0b6f\u0b66\u0bef\u0be6\u0c6f\u0c66\u0cef\u0ce6\u0d6f\u0d66\u0def\u0de6\u0e59\u0e50\u0ed9\u0ed0\u0f29\u0f20\u1049\u1040\u1099\u1090\u17e9\u17e0\u1819\u1810\u194f\u1946\u19d9\u19d0\u1a89\u1a80\u1a99\u1a90\u1b59\u1b50\u1bb9\u1bb0\u1c49\u1c40\u1c59\u1c50\ua629\ua620\ua8d9\ua8d0\ua909\ua900\ua9d9\ua9d0\ua9f9\ua9f0\uaa59\uaa50\uabf9\uabf0\uff19\uff10\uff3a\uff17\uff5a\uff37"
            .ToCharArray();

        /// <summary>
        /// Supplemental characters (surrogates) for digits. Implements Unicode 10.0.
        /// <para/>
        /// This class is strictly to lazy-load the range of values if and only if they are required.
        /// </summary>
        private static class DigitSupplemental
        {
            public static readonly uint[] Keys = new uint[] {
                0x000104a0, 0x00011066, 0x000110f0, 0x00011136,
                0x000111d0, 0x000112f0, 0x00011450, 0x000114d0,
                0x00011650, 0x000116c0, 0x00011730, 0x000118e0,
                0x00011c50, 0x00011d50, 0x00016a60, 0x00016b50,
                0x0001d7ce, 0x0001d7d8, 0x0001d7e2, 0x0001d7ec,
                0x0001d7f6, 0x0001e950
            };

            public static readonly uint[] Values = new uint[] {
                0x000104a9, 0x000104a0, 0x0001106f, 0x00011066, 0x000110f9, 0x000110f0, 0x0001113f, 0x00011136,
                0x000111d9, 0x000111d0, 0x000112f9, 0x000112f0, 0x00011459, 0x00011450, 0x000114d9, 0x000114d0,
                0x00011659, 0x00011650, 0x000116c9, 0x000116c0, 0x00011739, 0x00011730, 0x000118e9, 0x000118e0,
                0x00011c59, 0x00011c50, 0x00011d59, 0x00011d50, 0x00016a69, 0x00016a60, 0x00016b59, 0x00016b50,
                0x0001d7d7, 0x0001d7ce, 0x0001d7e1, 0x0001d7d8, 0x0001d7eb, 0x0001d7e2, 0x0001d7f5, 0x0001d7ec,
                0x0001d7ff, 0x0001d7f6, 0x0001e959, 0x0001e950
            };
        }


        // Unicode 10.0 (Harmony was at 3.0.0)
        private const string numericKeys = "\u00b2\u00b9\u00bc\u0660\u06f0\u07c0\u0966\u09e6\u09f4\u09f9\u0a66\u0ae6\u0b66\u0b72\u0be6\u0bf1\u0bf2\u0c66\u0c78\u0c7b\u0c7c\u0ce6\u0d58\u0d66\u0d71\u0d72\u0d73\u0de6\u0e50\u0ed0\u0f20\u0f2a\u1040\u1090\u1369\u1373\u1374\u1375\u1376\u1377\u1378\u1379\u137a\u137b\u137c\u16ee\u17e0\u17f0\u1810\u1946\u19d0\u19da\u1a80\u1a90\u1b50\u1bb0\u1c40\u1c50\u2070\u2074\u2080\u2150\u215f\u2160\u216c\u216d\u216e\u216f\u2170\u217c\u217d\u217e\u217f\u2180\u2181\u2182\u2185\u2186";

        private static readonly char[] numericValues = "\u00b3\u00b0\u00b9\u00b8\u00be\u0000\u0669\u0660\u06f9\u06f0\u07c9\u07c0\u096f\u0966\u09ef\u09e6\u09f8\u0000\u09f9\u09e9\u0a6f\u0a66\u0aef\u0ae6\u0b6f\u0b66\u0b77\u0000\u0bf0\u0be6\u0bf1\u0b8d\u0bf2\u080a\u0c6f\u0c66\u0c7e\u0c78\u0c7b\u0c78\u0c7e\u0c7b\u0cef\u0ce6\u0d5e\u0000\u0d70\u0d66\u0d71\u0d0d\u0d72\u098a\u0d78\u0000\u0def\u0de6\u0e59\u0e50\u0ed9\u0ed0\u0f29\u0f20\u0f33\u0000\u1049\u1040\u1099\u1090\u1372\u1368\u1373\u135f\u1374\u1356\u1375\u134d\u1376\u1344\u1377\u133b\u1378\u1332\u1379\u1329\u137a\u1320\u137b\u1317\u137c\uec6c\u16f0\u16dd\u17e9\u17e0\u17f9\u17f0\u1819\u1810\u194f\u1946\u19d9\u19d0\u19da\u19d9\u1a89\u1a80\u1a99\u1a90\u1b59\u1b50\u1bb9\u1bb0\u1c49\u1c40\u1c59\u1c50\u2070\u2070\u2079\u2070\u2089\u2080\u215e\u0000\u2182\u215e\u216b\u215f\u216c\u213a\u216d\u2109\u216e\u1f7a\u216f\u1d87\u217b\u216f\u217c\u214a\u217d\u2119\u217e\u1f8a\u217f\u1d97\u2180\u1d98\u2181\u0df9\u2182\ufa72\u2185\u217f\u2186\u2154"
            .ToCharArray();

        /// <summary>
        /// Intermediate range of characters with starting point 0x2187 because it is the first character
        /// that requires a value larger than <see cref="char"/> to calculate the result. Implements Unicode 10.0.
        /// <para/>
        /// This class is strictly to lazy-load the range of values if and only if they are required.
        /// </summary>
        private static class NumericIntermediate
        {
            public static readonly ushort[] Keys = new ushort[] {
                0x2187, 0x2188, 0x2189, 0x2460,
                0x2474, 0x2488, 0x24ea, 0x24eb,
                0x24f5, 0x24ff, 0x2776, 0x2780,
                0x278a, 0x2cfd, 0x3007, 0x3021,
                0x3038, 0x3039, 0x303a, 0x3192,
                0x3220, 0x3248, 0x3249, 0x324a,
                0x324b, 0x324c, 0x324d, 0x324e,
                0x324f, 0x3251, 0x3280, 0x32b1,
                0x3405, 0x3483, 0x382a, 0x3b4d,
                0x4e00, 0x4e03, 0x4e07, 0x4e09,
                0x4e5d, 0x4e8c, 0x4e94, 0x4e96,
                0x4ebf, 0x4ec0, 0x4edf, 0x4ee8,
                0x4f0d, 0x4f70, 0x5104, 0x5146,
                0x5169, 0x516b, 0x516d, 0x5341,
                0x5343, 0x5344, 0x5345, 0x534c,
                0x53c1, 0x53c2, 0x53c3, 0x53c4,
                0x56db, 0x58f1, 0x58f9, 0x5e7a,
                0x5efe, 0x5eff, 0x5f0c, 0x5f10,
                0x62fe, 0x634c, 0x67d2, 0x6f06,
                0x7396, 0x767e, 0x8086, 0x842c,
                0x8cae, 0x8cb3, 0x8d30, 0x9621,
                0x9646, 0x964c, 0x9678, 0x96f6,
                0xa620, 0xa6e6, 0xa6ef, 0xa830,
                0xa8d0, 0xa900, 0xa9d0, 0xa9f0,
                0xaa50, 0xabf0, 0xf96b, 0xf973,
                0xf978, 0xf9b2, 0xf9d1, 0xf9d3,
                0xf9fd, 0xff10, 0xff21, 0xff41,
            };

            public static readonly uint[] Values = new uint[] {
                0x2187, 0xffff5e37, 0x2188, 0xfffe9ae8, 0x2189, 0x2189, 0x249b, 0x245f,
                0x2487, 0x2473, 0x249b, 0x2487, 0x24ea, 0x24ea, 0x24f4, 0x24e0,
                0x24fe, 0x24f4, 0x24ff, 0x24ff, 0x277f, 0x2775, 0x2789, 0x277f,
                0x2793, 0x2789, 0x2cfd, 0x0000, 0x3007, 0x3007, 0x3029, 0x3020,
                0x3038, 0x302e, 0x3039, 0x3025, 0x303a, 0x301c, 0x3195, 0x3191,
                0x3229, 0x321f, 0x3248, 0x323e, 0x3249, 0x3235, 0x324a, 0x322c,
                0x324b, 0x3223, 0x324c, 0x321a, 0x324d, 0x3211, 0x324e, 0x3208,
                0x324f, 0x31ff, 0x325f, 0x323c, 0x3289, 0x327f, 0x32bf, 0x328d,
                0x3405, 0x3400, 0x3483, 0x3481, 0x382a, 0x3825, 0x3b4d, 0x3b46,
                0x4e00, 0x4dff, 0x4e03, 0x4dfc, 0x4e07, 0x26f7, 0x4e09, 0x4e06,
                0x4e5d, 0x4e54, 0x4e8c, 0x4e8a, 0x4e94, 0x4e8f, 0x4e96, 0x4e92,
                0x4ebf, 0xfa0a6dbf, 0x4ec0, 0x4eb6, 0x4edf, 0x4af7, 0x4ee8, 0x4ee5,
                0x4f0d, 0x4f08, 0x4f70, 0x4f0c, 0x5104, 0xfa0a7004, 0x5146, 0x0000,
                0x5169, 0x5167, 0x516b, 0x5163, 0x516d, 0x5167, 0x5341, 0x5337,
                0x5343, 0x4f5b, 0x5344, 0x5330, 0x5345, 0x5327, 0x534c, 0x5324,
                0x53c1, 0x53be, 0x53c2, 0x53bf, 0x53c3, 0x53c0, 0x53c4, 0x53c1,
                0x56db, 0x56d7, 0x58f1, 0x58f0, 0x58f9, 0x58f8, 0x5e7a, 0x5e79,
                0x5efe, 0x5ef5, 0x5eff, 0x5eeb, 0x5f0e, 0x5f0b, 0x5f10, 0x5f0e,
                0x62fe, 0x62f4, 0x634c, 0x6344, 0x67d2, 0x67cb, 0x6f06, 0x6eff,
                0x7396, 0x738d, 0x767e, 0x761a, 0x8086, 0x8082, 0x842c, 0x5d1c,
                0x8cae, 0x8cac, 0x8cb3, 0x8cb1, 0x8d30, 0x8d2e, 0x9621, 0x9239,
                0x9646, 0x9640, 0x964c, 0x95e8, 0x9678, 0x9672, 0x96f6, 0x96f6,
                0xa629, 0xa620, 0xa6ee, 0xa6e5, 0xa6ef, 0xa6ef, 0xa835, 0x0000,
                0xa8d9, 0xa8d0, 0xa909, 0xa900, 0xa9d9, 0xa9d0, 0xa9f9, 0xa9f0,
                0xaa59, 0xaa50, 0xabf9, 0xabf0, 0xf96b, 0xf968, 0xf973, 0xf969,
                0xf978, 0xf976, 0xf9b2, 0xf9b2, 0xf9d1, 0xf9cb, 0xf9d3, 0xf9cd,
                0xf9fd, 0xf9f3, 0xff19, 0xff10, 0xff3a, 0xff17, 0xff5a, 0xff37,
            };
        }

        /// <summary>
        /// Supplemental characters (surrogates) for numeric characters. Implements Unicode 10.0.
        /// <para/>
        /// This class is strictly to lazy-load the range of values if and only if they are required.
        /// </summary>
        private static class NumericSupplemental
        {
            public static readonly uint[] Keys = new uint[] {
                0x00010107, 0x00010111, 0x00010112, 0x00010113,
                0x00010114, 0x00010115, 0x00010116, 0x00010117,
                0x00010118, 0x00010119, 0x0001011a, 0x0001011b,
                0x0001011c, 0x0001011d, 0x0001011e, 0x0001011f,
                0x00010120, 0x00010121, 0x00010122, 0x00010123,
                0x00010124, 0x00010125, 0x00010126, 0x00010127,
                0x00010128, 0x00010129, 0x0001012a, 0x0001012b,
                0x0001012c, 0x0001012d, 0x0001012e, 0x0001012f,
                0x00010130, 0x00010131, 0x00010132, 0x00010133,
                0x00010140, 0x00010142, 0x00010143, 0x00010144,
                0x00010145, 0x00010146, 0x00010147, 0x00010148,
                0x00010149, 0x0001014a, 0x0001014b, 0x0001014c,
                0x0001014d, 0x0001014e, 0x0001014f, 0x00010150,
                0x00010151, 0x00010152, 0x00010153, 0x00010154,
                0x00010155, 0x00010156, 0x00010157, 0x00010158,
                0x00010159, 0x0001015a, 0x0001015c, 0x0001015d,
                0x0001015e, 0x0001015f, 0x00010160, 0x00010161,
                0x00010162, 0x00010163, 0x00010164, 0x00010165,
                0x00010166, 0x00010167, 0x00010168, 0x00010169,
                0x0001016a, 0x0001016b, 0x0001016c, 0x0001016d,
                0x0001016e, 0x0001016f, 0x00010170, 0x00010171,
                0x00010172, 0x00010173, 0x00010174, 0x00010175,
                0x00010176, 0x00010177, 0x00010178, 0x0001018a,
                0x0001018b, 0x000102e1, 0x000102eb, 0x000102ec,
                0x000102ed, 0x000102ee, 0x000102ef, 0x000102f0,
                0x000102f1, 0x000102f2, 0x000102f3, 0x000102f4,
                0x000102f5, 0x000102f6, 0x000102f7, 0x000102f8,
                0x000102f9, 0x000102fa, 0x000102fb, 0x00010320,
                0x00010321, 0x00010322, 0x00010323, 0x00010341,
                0x0001034a, 0x000103d1, 0x000103d3, 0x000103d4,
                0x000103d5, 0x000104a0, 0x00010858, 0x0001085b,
                0x0001085c, 0x0001085d, 0x0001085e, 0x0001085f,
                0x00010879, 0x0001087e, 0x0001087f, 0x000108a7,
                0x000108ab, 0x000108ad, 0x000108ae, 0x000108af,
                0x000108fb, 0x000108fc, 0x000108fd, 0x000108fe,
                0x000108ff, 0x00010916, 0x00010917, 0x00010918,
                0x00010919, 0x0001091a, 0x000109bc, 0x000109c0,
                0x000109ca, 0x000109cb, 0x000109cc, 0x000109cd,
                0x000109ce, 0x000109cf, 0x000109d2, 0x000109d3,
                0x000109d4, 0x000109d5, 0x000109d6, 0x000109d7,
                0x000109d8, 0x000109d9, 0x000109da, 0x000109db,
                0x000109dc, 0x000109dd, 0x000109de, 0x000109df,
                0x000109e0, 0x000109e1, 0x000109e2, 0x000109e3,
                0x000109e4, 0x000109e5, 0x000109e6, 0x000109e7,
                0x000109e8, 0x000109e9, 0x000109ea, 0x000109eb,
                0x000109ec, 0x000109ed, 0x000109ee, 0x000109ef,
                0x000109f0, 0x000109f1, 0x000109f2, 0x000109f3,
                0x000109f4, 0x000109f5, 0x000109f6, 0x000109f7,
                0x000109f8, 0x000109f9, 0x000109fa, 0x000109fb,
                0x000109fc, 0x000109fd, 0x000109fe, 0x000109ff,
                0x00010a40, 0x00010a44, 0x00010a45, 0x00010a46,
                0x00010a47, 0x00010a7d, 0x00010a7e, 0x00010a9d,
                0x00010a9e, 0x00010a9f, 0x00010aeb, 0x00010aec,
                0x00010aed, 0x00010aee, 0x00010aef, 0x00010b58,
                0x00010b5c, 0x00010b5d, 0x00010b5e, 0x00010b5f,
                0x00010b78, 0x00010b7c, 0x00010b7d, 0x00010b7e,
                0x00010b7f, 0x00010ba9, 0x00010bad, 0x00010bae,
                0x00010baf, 0x00010cfa, 0x00010cfb, 0x00010cfc,
                0x00010cfd, 0x00010cfe, 0x00010cff, 0x00010e60,
                0x00010e6a, 0x00010e6b, 0x00010e6c, 0x00010e6d,
                0x00010e6e, 0x00010e6f, 0x00010e70, 0x00010e71,
                0x00010e72, 0x00010e73, 0x00010e74, 0x00010e75,
                0x00010e76, 0x00010e77, 0x00010e78, 0x00010e79,
                0x00010e7a, 0x00010e7b, 0x00010e7c, 0x00010e7d,
                0x00010e7e, 0x00011052, 0x0001105c, 0x0001105d,
                0x0001105e, 0x0001105f, 0x00011060, 0x00011061,
                0x00011062, 0x00011063, 0x00011064, 0x00011065,
                0x00011066, 0x000110f0, 0x00011136, 0x000111d0,
                0x000111e1, 0x000111eb, 0x000111ec, 0x000111ed,
                0x000111ee, 0x000111ef, 0x000111f0, 0x000111f1,
                0x000111f2, 0x000111f3, 0x000111f4, 0x000112f0,
                0x00011450, 0x000114d0, 0x00011650, 0x000116c0,
                0x00011730, 0x0001173b, 0x000118e0, 0x000118eb,
                0x000118ec, 0x000118ed, 0x000118ee, 0x000118ef,
                0x000118f0, 0x000118f1, 0x000118f2, 0x00011c50,
                0x00011c5a, 0x00011c64, 0x00011c65, 0x00011c66,
                0x00011c67, 0x00011c68, 0x00011c69, 0x00011c6a,
                0x00011c6b, 0x00011c6c, 0x00011d50, 0x00012400,
                0x00012408, 0x0001240f, 0x00012415, 0x0001241e,
                0x00012423, 0x00012425, 0x0001242c, 0x0001242f,
                0x00012432, 0x00012433, 0x00012434, 0x00012437,
                0x0001243a, 0x0001243b, 0x0001243d, 0x0001243e,
                0x0001243f, 0x00012440, 0x00012442, 0x00012443,
                0x00012445, 0x00012447, 0x00012448, 0x00012449,
                0x0001244a, 0x0001244f, 0x00012453, 0x00012455,
                0x00012456, 0x00012458, 0x0001245a, 0x0001245b,
                0x0001245c, 0x0001245d, 0x0001245e, 0x0001245f,
                0x00012460, 0x00012461, 0x00012462, 0x00012463,
                0x00012464, 0x00012465, 0x00012466, 0x00012467,
                0x00012468, 0x00012469, 0x00016a60, 0x00016b50,
                0x00016b5b, 0x00016b5c, 0x00016b5d, 0x00016b5e,
                0x00016b5f, 0x00016b60, 0x00016b61, 0x0001d360,
                0x0001d36a, 0x0001d36b, 0x0001d36c, 0x0001d36d,
                0x0001d36e, 0x0001d36f, 0x0001d370, 0x0001d371,
                0x0001d7ce, 0x0001d7d8, 0x0001d7e2, 0x0001d7ec,
                0x0001d7f6, 0x0001e8c7, 0x0001e950, 0x0001f100,
                0x0001f101, 0x0001f10b, 0x0001f10c, 0x00020001,
                0x00020064, 0x000200e2, 0x00020121, 0x0002092a,
                0x00020983, 0x0002098c, 0x0002099c, 0x00020aea,
                0x00020afd, 0x00020b19, 0x00022390, 0x00022998,
                0x00023b1b, 0x0002626d, 0x0002f890,
            };

            public static readonly uint[] Values = new uint[] {
                0x00010110, 0x00010106, 0x00010111, 0x000100fd, 0x00010112, 0x000100f4, 0x00010113, 0x000100eb,
                0x00010114, 0x000100e2, 0x00010115, 0x000100d9, 0x00010116, 0x000100d0, 0x00010117, 0x000100c7,
                0x00010118, 0x000100be, 0x00010119, 0x000100b5, 0x0001011a, 0x00010052, 0x0001011b, 0x0000ffef,
                0x0001011c, 0x0000ff8c, 0x0001011d, 0x0000ff29, 0x0001011e, 0x0000fec6, 0x0001011f, 0x0000fe63,
                0x00010120, 0x0000fe00, 0x00010121, 0x0000fd9d, 0x00010122, 0x0000fd3a, 0x00010123, 0x0000f953,
                0x00010124, 0x0000f56c, 0x00010125, 0x0000f185, 0x00010126, 0x0000ed9e, 0x00010127, 0x0000e9b7,
                0x00010128, 0x0000e5d0, 0x00010129, 0x0000e1e9, 0x0001012a, 0x0000de02, 0x0001012b, 0x0000da1b,
                0x0001012c, 0x0000b30c, 0x0001012d, 0x00008bfd, 0x0001012e, 0x000064ee, 0x0001012f, 0x00003ddf,
                0x00010130, 0x000016d0, 0x00010131, 0xffffefc1, 0x00010132, 0xffffc8b2, 0x00010133, 0xffffa1a3,
                0x00010141, 0x00000000, 0x00010142, 0x00010141, 0x00010143, 0x0001013e, 0x00010144, 0x00010112,
                0x00010145, 0x0000ff51, 0x00010146, 0x0000edbe, 0x00010147, 0x00003df7, 0x00010148, 0x00010143,
                0x00010149, 0x0001013f, 0x0001014a, 0x00010118, 0x0001014b, 0x000100e7, 0x0001014c, 0x0000ff58,
                0x0001014d, 0x0000fd65, 0x0001014e, 0x0000edc6, 0x0001014f, 0x0001014a, 0x00010150, 0x00010146,
                0x00010151, 0x0001011f, 0x00010152, 0x000100ee, 0x00010153, 0x0000ff5f, 0x00010154, 0x0000fd6c,
                0x00010155, 0x0000da45, 0x00010156, 0x00003e06, 0x00010157, 0x0001014d, 0x00010158, 0x00010157,
                0x00010159, 0x00010158, 0x0001015b, 0x00010159, 0x0001015c, 0x0001015a, 0x0001015d, 0x0001015b,
                0x0001015e, 0x0001015c, 0x0001015f, 0x0001015a, 0x00010160, 0x00010156, 0x00010161, 0x00010157,
                0x00010162, 0x00010158, 0x00010163, 0x00010159, 0x00010164, 0x0001015a, 0x00010165, 0x00010147,
                0x00010166, 0x00010134, 0x00010167, 0x00010135, 0x00010168, 0x00010136, 0x00010169, 0x00010137,
                0x0001016a, 0x00010106, 0x0001016b, 0x0001003f, 0x0001016c, 0x0000ff78, 0x0001016d, 0x0000ff79,
                0x0001016e, 0x0000ff7a, 0x0001016f, 0x0000ff7b, 0x00010170, 0x0000ff7c, 0x00010171, 0x0000fd89,
                0x00010172, 0x0000edea, 0x00010173, 0x0001016e, 0x00010174, 0x00010142, 0x00010175, 0x00000000,
                0x00010176, 0x00000000, 0x00010177, 0x00000000, 0x00010178, 0x00000000, 0x0001018a, 0x0001018a,
                0x0001018b, 0x00000000, 0x000102ea, 0x000102e0, 0x000102eb, 0x000102d7, 0x000102ec, 0x000102ce,
                0x000102ed, 0x000102c5, 0x000102ee, 0x000102bc, 0x000102ef, 0x000102b3, 0x000102f0, 0x000102aa,
                0x000102f1, 0x000102a1, 0x000102f2, 0x00010298, 0x000102f3, 0x0001028f, 0x000102f4, 0x0001022c,
                0x000102f5, 0x000101c9, 0x000102f6, 0x00010166, 0x000102f7, 0x00010103, 0x000102f8, 0x000100a0,
                0x000102f9, 0x0001003d, 0x000102fa, 0x0000ffda, 0x000102fb, 0x0000ff77, 0x00010320, 0x0001031f,
                0x00010321, 0x0001031c, 0x00010322, 0x00010318, 0x00010323, 0x000102f1, 0x00010341, 0x000102e7,
                0x0001034a, 0x0000ffc6, 0x000103d2, 0x000103d0, 0x000103d3, 0x000103c9, 0x000103d4, 0x000103c0,
                0x000103d5, 0x00010371, 0x000104a9, 0x000104a0, 0x0001085a, 0x00010857, 0x0001085b, 0x00010851,
                0x0001085c, 0x00010848, 0x0001085d, 0x000107f9, 0x0001085e, 0x00010476, 0x0001085f, 0x0000e14f,
                0x0001087d, 0x00010878, 0x0001087e, 0x00010874, 0x0001087f, 0x0001086b, 0x000108aa, 0x000108a6,
                0x000108ac, 0x000108a7, 0x000108ad, 0x000108a3, 0x000108ae, 0x0001089a, 0x000108af, 0x0001084b,
                0x000108fb, 0x000108fa, 0x000108fc, 0x000108f7, 0x000108fd, 0x000108f3, 0x000108fe, 0x000108ea,
                0x000108ff, 0x0001089b, 0x00010916, 0x00010915, 0x00010917, 0x0001090d, 0x00010918, 0x00010904,
                0x00010919, 0x000108b5, 0x0001091b, 0x00010918, 0x000109bd, 0x00000000, 0x000109c9, 0x000109bf,
                0x000109ca, 0x000109b6, 0x000109cb, 0x000109ad, 0x000109cc, 0x000109a4, 0x000109cd, 0x0001099b,
                0x000109ce, 0x00010992, 0x000109cf, 0x00010989, 0x000109d2, 0x0001096e, 0x000109d3, 0x0001090b,
                0x000109d4, 0x000108a8, 0x000109d5, 0x00010845, 0x000109d6, 0x000107e2, 0x000109d7, 0x0001077f,
                0x000109d8, 0x0001071c, 0x000109d9, 0x000106b9, 0x000109da, 0x00010656, 0x000109db, 0x000105f3,
                0x000109dc, 0x0001020c, 0x000109dd, 0x0000fe25, 0x000109de, 0x0000fa3e, 0x000109df, 0x0000f657,
                0x000109e0, 0x0000f270, 0x000109e1, 0x0000ee89, 0x000109e2, 0x0000eaa2, 0x000109e3, 0x0000e6bb,
                0x000109e4, 0x0000e2d4, 0x000109e5, 0x0000bbc5, 0x000109e6, 0x000094b6, 0x000109e7, 0x00006da7,
                0x000109e8, 0x00004698, 0x000109e9, 0x00001f89, 0x000109ea, 0xfffff87a, 0x000109eb, 0xffffd16b,
                0x000109ec, 0xffffaa5c, 0x000109ed, 0xffff834d, 0x000109ee, 0xfffdfcae, 0x000109ef, 0xfffc760f,
                0x000109f0, 0xfffaef70, 0x000109f1, 0xfff968d1, 0x000109f2, 0xfff7e232, 0x000109f3, 0xfff65b93,
                0x000109f4, 0xfff4d4f4, 0x000109f5, 0xfff34e55, 0x000109f6, 0x00000000, 0x000109f7, 0x00000000,
                0x000109f8, 0x00000000, 0x000109f9, 0x00000000, 0x000109fa, 0x00000000, 0x000109fb, 0x00000000,
                0x000109fc, 0x00000000, 0x000109fd, 0x00000000, 0x000109fe, 0x00000000, 0x000109ff, 0x00000000,
                0x00010a43, 0x00010a3f, 0x00010a44, 0x00010a3a, 0x00010a45, 0x00010a31, 0x00010a46, 0x000109e2,
                0x00010a47, 0x0001065f, 0x00010a7d, 0x00010a7c, 0x00010a7e, 0x00010a4c, 0x00010a9d, 0x00010a9c,
                0x00010a9e, 0x00010a94, 0x00010a9f, 0x00010a8b, 0x00010aeb, 0x00010aea, 0x00010aec, 0x00010ae7,
                0x00010aed, 0x00010ae3, 0x00010aee, 0x00010ada, 0x00010aef, 0x00010a8b, 0x00010b5b, 0x00010b57,
                0x00010b5c, 0x00010b52, 0x00010b5d, 0x00010b49, 0x00010b5e, 0x00010afa, 0x00010b5f, 0x00010777,
                0x00010b7b, 0x00010b77, 0x00010b7c, 0x00010b72, 0x00010b7d, 0x00010b69, 0x00010b7e, 0x00010b1a,
                0x00010b7f, 0x00010797, 0x00010bac, 0x00010ba8, 0x00010bad, 0x00010ba3, 0x00010bae, 0x00010b9a,
                0x00010baf, 0x00010b4b, 0x00010cfa, 0x00010cf9, 0x00010cfb, 0x00010cf6, 0x00010cfc, 0x00010cf2,
                0x00010cfd, 0x00010ccb, 0x00010cfe, 0x00010c9a, 0x00010cff, 0x00010917, 0x00010e69, 0x00010e5f,
                0x00010e6a, 0x00010e56, 0x00010e6b, 0x00010e4d, 0x00010e6c, 0x00010e44, 0x00010e6d, 0x00010e3b,
                0x00010e6e, 0x00010e32, 0x00010e6f, 0x00010e29, 0x00010e70, 0x00010e20, 0x00010e71, 0x00010e17,
                0x00010e72, 0x00010e0e, 0x00010e73, 0x00010dab, 0x00010e74, 0x00010d48, 0x00010e75, 0x00010ce5,
                0x00010e76, 0x00010c82, 0x00010e77, 0x00010c1f, 0x00010e78, 0x00010bbc, 0x00010e79, 0x00010b59,
                0x00010e7a, 0x00010af6, 0x00010e7b, 0x00000000, 0x00010e7c, 0x00000000, 0x00010e7d, 0x00000000,
                0x00010e7e, 0x00000000, 0x0001105b, 0x00011051, 0x0001105c, 0x00011048, 0x0001105d, 0x0001103f,
                0x0001105e, 0x00011036, 0x0001105f, 0x0001102d, 0x00011060, 0x00011024, 0x00011061, 0x0001101b,
                0x00011062, 0x00011012, 0x00011063, 0x00011009, 0x00011064, 0x00011000, 0x00011065, 0x00010c7d,
                0x0001106f, 0x00011066, 0x000110f9, 0x000110f0, 0x0001113f, 0x00011136, 0x000111d9, 0x000111d0,
                0x000111ea, 0x000111e0, 0x000111eb, 0x000111d7, 0x000111ec, 0x000111ce, 0x000111ed, 0x000111c5,
                0x000111ee, 0x000111bc, 0x000111ef, 0x000111b3, 0x000111f0, 0x000111aa, 0x000111f1, 0x000111a1,
                0x000111f2, 0x00011198, 0x000111f3, 0x0001118f, 0x000111f4, 0x00010e0c, 0x000112f9, 0x000112f0,
                0x00011459, 0x00011450, 0x000114d9, 0x000114d0, 0x00011659, 0x00011650, 0x000116c9, 0x000116c0,
                0x0001173a, 0x00011730, 0x0001173b, 0x00011727, 0x000118ea, 0x000118e0, 0x000118eb, 0x000118d7,
                0x000118ec, 0x000118ce, 0x000118ed, 0x000118c5, 0x000118ee, 0x000118bc, 0x000118ef, 0x000118b3,
                0x000118f0, 0x000118aa, 0x000118f1, 0x000118a1, 0x000118f2, 0x00011898, 0x00011c59, 0x00011c50,
                0x00011c63, 0x00011c59, 0x00011c64, 0x00011c50, 0x00011c65, 0x00011c47, 0x00011c66, 0x00011c3e,
                0x00011c67, 0x00011c35, 0x00011c68, 0x00011c2c, 0x00011c69, 0x00011c23, 0x00011c6a, 0x00011c1a,
                0x00011c6b, 0x00011c11, 0x00011c6c, 0x00011c08, 0x00011d59, 0x00011d50, 0x00012407, 0x000123fe,
                0x0001240e, 0x00012405, 0x00012414, 0x0001240b, 0x0001241d, 0x00012414, 0x00012422, 0x0001241d,
                0x00012424, 0x00012421, 0x0001242b, 0x00012422, 0x0001242e, 0x0001242b, 0x00012431, 0x0001242c,
                0x00012432, 0xfffdd872, 0x00012433, 0xfffa8cb3, 0x00012436, 0x00012433, 0x00012439, 0x00012434,
                0x0001243a, 0x00012437, 0x0001243c, 0x00012438, 0x0001243d, 0x00012439, 0x0001243e, 0x0001243a,
                0x0001243f, 0x0001243b, 0x00012441, 0x0001243a, 0x00012442, 0x0001243b, 0x00012444, 0x0001243c,
                0x00012446, 0x0001243d, 0x00012447, 0x0001243e, 0x00012448, 0x0001243f, 0x00012449, 0x00012440,
                0x0001244e, 0x00012448, 0x00012452, 0x0001244e, 0x00012454, 0x0001244f, 0x00012455, 0x00012450,
                0x00012457, 0x00012454, 0x00012459, 0x00012457, 0x0001245a, 0x00000000, 0x0001245b, 0x00000000,
                0x0001245c, 0x00000000, 0x0001245d, 0x00000000, 0x0001245e, 0x00000000, 0x0001245f, 0x00000000,
                0x00012460, 0x00000000, 0x00012461, 0x00000000, 0x00012462, 0x00000000, 0x00012463, 0x00000000,
                0x00012464, 0x00000000, 0x00012465, 0x00000000, 0x00012466, 0x00000000, 0x00012467, 0x0001243f,
                0x00012468, 0x00012436, 0x0001246e, 0x00012465, 0x00016a69, 0x00016a60, 0x00016b59, 0x00016b50,
                0x00016b5b, 0x00016b51, 0x00016b5c, 0x00016af8, 0x00016b5d, 0x0001444d, 0x00016b5e, 0xfff2291e,
                0x00016b5f, 0xfa0b8a5f, 0x00016b60, 0x00000000, 0x00016b61, 0x00000000, 0x0001d369, 0x0001d35f,
                0x0001d36a, 0x0001d356, 0x0001d36b, 0x0001d34d, 0x0001d36c, 0x0001d344, 0x0001d36d, 0x0001d33b,
                0x0001d36e, 0x0001d332, 0x0001d36f, 0x0001d329, 0x0001d370, 0x0001d320, 0x0001d371, 0x0001d317,
                0x0001d7d7, 0x0001d7ce, 0x0001d7e1, 0x0001d7d8, 0x0001d7eb, 0x0001d7e2, 0x0001d7f5, 0x0001d7ec,
                0x0001d7ff, 0x0001d7f6, 0x0001e8cf, 0x0001e8c6, 0x0001e959, 0x0001e950, 0x0001f100, 0x0001f100,
                0x0001f10a, 0x0001f101, 0x0001f10b, 0x0001f10b, 0x0001f10c, 0x0001f10c, 0x00020001, 0x0001fffa,
                0x00020064, 0x00020060, 0x000200e2, 0x000200de, 0x00020121, 0x0002011c, 0x0002092a, 0x00020929,
                0x00020983, 0x00020965, 0x0002098c, 0x00020964, 0x0002099c, 0x00020974, 0x00020aea, 0x00020ae4,
                0x00020afd, 0x00020afa, 0x00020b19, 0x00020b16, 0x00022390, 0x0002238e, 0x00022998, 0x00022995,
                0x00023b1b, 0x00023b18, 0x0002626d, 0x00026269, 0x0002f890, 0x0002f887,
            };
        }

        /// <summary>
        /// Indicates whether <paramref name="codePoint"/> is a valid Unicode code point.
        /// </summary>
        /// <param name="codePoint">The code point to test.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a valid Unicode code point; <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidCodePoint(int codePoint)
        {
            // J2N: Optimized version of: (MinCodePoint <= codePoint && MaxCodePoint >= codePoint);
            return (uint)codePoint <= MaxCodePoint;
        }

        /// <summary>
        /// Indicates whether <paramref name="codePoint"/> is within the supplementary code
        /// </summary>
        /// <param name="codePoint">The code point to test.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is within the supplementary
        /// code point range; <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSupplementaryCodePoint(int codePoint)
        {
            // J2N: Optimized version of: (MinSupplementaryCodePoint <= codePoint && MaxCodePoint >= codePoint);
            return (uint)(codePoint - MinSupplementaryCodePoint) <= (MaxCodePoint - MinSupplementaryCodePoint);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c> or its <see cref="ICharSequence.HasValue"/> property returns <c>false</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than or equal to the length of <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        public static int CodePointAt(this ICharSequence seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null || !seq.HasValue)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq, ExceptionResource.ArgumentNull_NullOrNullValue);
            if (seq is StringBuilderCharSequence sb)
                return CodePointAt(sb.Value!, index);
            if (seq is StringBuffer stringBuffer)
                return CodePointAt(stringBuffer.builder, index);

            int len = seq.Length;
            if ((uint)index >= (uint)len)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

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
        public static int CodePointAt(this char[] seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            int len = seq.Length;
            if ((uint)index >= (uint)len)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

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
        /// <para/>
        /// <strong>Usage Note:</strong> This method is slow because it relies on the indexer of
        /// <see cref="StringBuilder"/> and it is designed to be used inside of a loop. Avoid.
        /// It is recommended to convert the contents of the
        /// <see cref="StringBuilder"/> to <see cref="ReadOnlySpan{T}"/>, <see cref="T:char[]"/>,
        /// or <see cref="string"/> and to call the appropriate overload, instead.
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
        public static int CodePointAt(this StringBuilder seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            int len = seq.Length;
            if ((uint)index >= (uint)len)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

            // J2N NOTE: Looking up 1 or 2 characters is faster through the StringBuilder than an indexer
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
        public static int CodePointAt(this string seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            int len = seq.Length;
            if ((uint)index >= (uint)len)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

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
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than or equal to the length of <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        public static int CodePointAt(this ReadOnlySpan<char> seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            int len = seq.Length;
            if ((uint)index >= (uint)len)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

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
        public static int CodePointAt(this char[] seq, int index, int limit) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            if ((uint)index >= (uint)limit)
                throw new ArgumentOutOfRangeException(nameof(index));
            if ((uint)limit > (uint)seq.Length)
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
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c> or its <see cref="ICharSequence.HasValue"/> property returns <c>false</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than 1 or greater than
        /// the length of <paramref name="seq"/>.</exception>
        public static int CodePointBefore(this ICharSequence seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null || !seq.HasValue)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq, ExceptionResource.ArgumentNull_NullOrNullValue);
            if (seq is StringBuilderCharSequence sb)
                return CodePointBefore(sb.Value!, index);
            if (seq is StringBuffer stringBuffer)
                return CodePointBefore(stringBuffer.builder, index);

            if ((uint)(index - 1) >= (uint)seq.Length) // J2N: Simplified version of (index < 1 || index > seq.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexBefore);

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
        public static int CodePointBefore(this char[] seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            if ((uint)(index - 1) >= (uint)seq.Length) // J2N: Simplified version of (index < 1 || index > seq.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexBefore);

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
        /// <para/>
        /// <strong>Usage Note:</strong> This method is slow because it relies on the indexer of
        /// <see cref="StringBuilder"/> and it is designed to be used inside of a loop. Avoid.
        /// It is recommended to convert the contents of the
        /// <see cref="StringBuilder"/> to <see cref="ReadOnlySpan{T}"/>, <see cref="T:char[]"/>,
        /// or <see cref="string"/> and to call the appropriate overload, instead.
        /// </summary>
        /// <param name="seq">The source sequence of <see cref="char"/> units.</param>
        /// <param name="index">The position in <paramref name="seq"/> following the code
        /// point that should be returned.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value before <paramref name="index"/>
        /// in <paramref name="seq"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than 1 or greater than
        /// the length of <paramref name="seq"/>.</exception>
        public static int CodePointBefore(this StringBuilder seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            if ((uint)(index - 1) >= (uint)seq.Length) // J2N: Simplified version of (index < 1 || index > seq.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexBefore);

            // J2N NOTE: Looking up 1 or 2 characters is faster through the StringBuilder than an indexer
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
        public static int CodePointBefore(this string seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            if ((uint)(index - 1) >= (uint)seq.Length) // J2N: Simplified version of (index < 1 || index > seq.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexBefore);

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
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than 1 or greater than
        /// the length of <paramref name="seq"/>.</exception>
        public static int CodePointBefore(this ReadOnlySpan<char> seq, int index) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if ((uint)(index - 1) >= (uint)seq.Length) // J2N: Simplified version of (index < 1 || index > seq.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexBefore);

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
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            int len = seq.Length;
            if ((uint)start >= (uint)len)
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
        /// and copies the value(s) into the <see cref="Span{T}"/> <paramref name="destination"/>, starting at
        /// index <paramref name="destinationIndex"/>.
        /// </summary>
        /// <param name="codePoint">The Unicode code point to encode.</param>
        /// <param name="destination">The destination span to copy the encoded value into.</param>
        /// <param name="destinationIndex">The index in <paramref name="destination"/> from where to start copying.</param>
        /// <returns>The number of <see cref="char"/> value units copied into <paramref name="destination"/>.</returns>
        /// <exception cref="ArgumentException">If <paramref name="codePoint"/> is not a valid Unicode code point.</exception>
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
        public static int ToChars(int codePoint, Span<char> destination, int destinationIndex)
        {
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);
            if ((uint)destinationIndex >= (uint)destination.Length)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(destinationIndex, ExceptionArgument.destinationIndex);

            if (codePoint < MinSupplementaryCodePoint) // BMP char
            {
                destination[destinationIndex] = (char)codePoint;
                return 1;
            }

            ToCharsSupplementary(codePoint, destination, destinationIndex);
            return 2;
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
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            if ((uint)destinationIndex >= (uint)destination.Length)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(destinationIndex, ExceptionArgument.destinationIndex);

            if (codePoint < MinSupplementaryCodePoint) // BMP char
            {
                destination[destinationIndex] = (char)codePoint;
                return 1;
            }

            ToCharsSupplementary(codePoint, destination, destinationIndex);
            return 2;
        }

        /// <summary>
        /// Converts the specified Unicode code point into a UTF-16 encoded sequence
        /// and returns it as a char array.
        /// <para/>
        /// This correponds to <see cref="char.ConvertFromUtf32(int)"/>, but returns
        /// a <see cref="T:char[]"/> rather than a <see cref="string"/>.
        /// <para/>
        /// Usage Note: This overload is discouraged because it produces a heap allocation
        /// every time it is called. The <see cref="ToChars(int, Span{char})"/> is a direct
        /// replacement that allows passing in a reusable buffer.
        /// </summary>
        /// <param name="codePoint">The Unicode code point to encode.</param>
        /// <returns>
        /// The UTF-16 encoded char sequence. If <paramref name="codePoint"/> is a
        /// supplementary code point (<see cref="IsSupplementaryCodePoint(int)"/>),
        /// then the returned array contains two characters, otherwise it contains
        /// just one character.
        /// </returns>
        /// <exception cref="ArgumentException">If <paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        public static char[] ToChars(int codePoint)
        {
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            if (codePoint < MinSupplementaryCodePoint) // BMP char
            {
                return new char[] { (char)codePoint };
            }

            ToCharsSupplementary(codePoint, out char high, out char low);
            return new char[] { high, low };
        }

        /// <summary>
        /// Converts the specified Unicode code point into a UTF-16 encoded sequence
        /// and returns it as a <see cref="ReadOnlySpan{Char}"/>.
        /// <para/>
        /// This correponds to <see cref="char.ConvertFromUtf32(int)"/>, but returns
        /// a <see cref="ReadOnlySpan{Char}"/> rather than a <see cref="string"/>.
        /// </summary>
        /// <param name="codePoint">The Unicode code point to encode.</param>
        /// <param name="buffer">The memory location to store the chars. Typically,
        /// it should be <c>stackalloc char[2]</c> since it will never be longer than 2 chars.
        /// Note that this <paramref name="buffer"/> is not intended for use by callers,
        /// it is just a memory location to use when returning the result. The caller is
        /// responsible for ensuring the memory location has a sufficient scope that is
        /// at least as long as the scope of the return value.</param>
        /// <returns>
        /// The UTF-16 encoded char sequence sliced to the proper length. If
        /// <paramref name="codePoint"/> is a supplementary code point
        /// (<see cref="IsSupplementaryCodePoint(int)"/>), then the returned
        /// span contains two characters, otherwise it contains just one character.
        /// </returns>
        /// <exception cref="ArgumentException">If <paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        public static ReadOnlySpan<char> ToChars(int codePoint, Span<char> buffer)
        {
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            if (codePoint < MinSupplementaryCodePoint) // BMP char
            {
                buffer[0] = (char)codePoint;
                return buffer.Slice(0, 1);
            }

            ToCharsSupplementary(codePoint, buffer, destinationIndex: 0);
            return buffer.Slice(0, 2);
        }

        /// <summary>
        /// Converts the specified Unicode code point into a UTF-16 encoded sequence
        /// and returns it as <see cref="char"/>.out parameters.
        /// <para/>
        /// This correponds to <see cref="char.ConvertFromUtf32(int)"/>, but returns
        /// out parameters that allocate on the stack rather than a <see cref="string"/>.
        /// </summary>
        /// <param name="codePoint">The Unicode code point to encode.</param>
        /// <param name="high">The high (first) character of the code point, or the only character if
        /// <paramref name="codePoint"/> is not a supplementary code point.</param>
        /// <param name="low">The low (second) character of the code point. This is only populated if
        /// <paramref name="codePoint"/> is a supplementary code point, in which case the return value is 2.</param>
        /// <returns>
        /// The length of the UTF-16 encoded char sequence. If <paramref name="codePoint"/> is a
        /// supplementary code point (<see cref="IsSupplementaryCodePoint(int)"/>),
        /// then the returned value indicates two characters, otherwise it indicates
        /// just one character (the <paramref name="high"/> value).
        /// </returns>
        /// <exception cref="ArgumentException">If <paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        public static int ToChars(int codePoint, out char high, out char low)
        {
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            if (codePoint < MinSupplementaryCodePoint) // BMP char
            {
                high = (char)codePoint;
                low = charNull;
                return 1;
            }

            ToCharsSupplementary(codePoint, out high, out low);
            return 2;
        }
        private static void ToCharsSupplementary(int codePoint, Span<char> destination, int destinationIndex)
        {
            if (destinationIndex == destination.Length - 1)
                ThrowHelper.ThrowArgumentOutOfRangeException(destinationIndex, ExceptionArgument.destinationIndex, ExceptionResource.ArgumentOutOfRange_OffsetOut);

            // See RFC 2781, Section 2.1
            // http://www.faqs.org/rfcs/rfc2781.html
            int cpPrime = codePoint - 0x10000;
            int high = 0xD800 | ((cpPrime >> 10) & 0x3FF);
            int low = 0xDC00 | (cpPrime & 0x3FF);
            destination[destinationIndex] = (char)high;
            destination[destinationIndex + 1] = (char)low;
        }
        private static void ToCharsSupplementary(int codePoint, out char high, out char low)
        {
            int cpPrime = codePoint - 0x10000;
            high = (char)(0xD800 | ((cpPrime >> 10) & 0x3FF));
            low = (char)(0xDC00 | (cpPrime & 0x3FF));
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
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c> or its <see cref="ICharSequence.HasValue"/> property returns <c>false</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within <paramref name="seq"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public static int CodePointCount(this ICharSequence seq, int startIndex, int length) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null || !seq.HasValue)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq, ExceptionResource.ArgumentNull_NullOrNullValue);
            if (seq is StringBuilderCharSequence sb)
                return CodePointCount(sb.Value!, startIndex, length);
            if (seq is StringBuffer stringBuffer)
                return CodePointCount(stringBuffer.builder, startIndex, length);

            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > seq.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

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
        public static int CodePointCount(this char[] seq, int startIndex, int length) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > seq.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

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
        public static int CodePointCount(this StringBuilder seq, int startIndex, int length) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > seq.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> chars = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                seq.CopyTo(0, chars, length);
#else
                Span<char> chars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                seq.CopyTo(0, arrayToReturnToPool, 0, length);
#endif
                int endIndex = startIndex + length;
                int n = length;
                for (int i = startIndex; i < endIndex;)
                {
                    if (char.IsHighSurrogate(chars[i++]) && i < endIndex
                        && char.IsLowSurrogate(chars[i]))
                    {
                        n--;
                        i++;
                    }
                }
                return n;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
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
        public static int CodePointCount(this string seq, int startIndex, int length) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > seq.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

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
        /// The text range begins at 0 and extends for the number
        /// of characters specified in <paramref name="seq"/>.
        /// Unpaired surrogates within the text range count as one code point each.
        /// </summary>
        /// <param name="seq">The char sequence.</param>
        public static int CodePointCount(this ReadOnlySpan<char> seq) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            int endIndex = seq.Length;
            int n = endIndex;
            for (int i = 0; i < endIndex;)
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
        /// <exception cref="ArgumentNullException">If <paramref name="seq"/> is <c>null</c> or its <see cref="ICharSequence.HasValue"/> property returns <c>false</c>.</exception>
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
        public static int OffsetByCodePoints(this ICharSequence seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null || !seq.HasValue)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq, ExceptionResource.ArgumentNull_NullOrNullValue);
            if (seq is StringBuilderCharSequence sb)
                return OffsetByCodePoints(sb.Value!, index, codePointOffset);
            if (seq is StringBuffer stringBuffer)
                return OffsetByCodePoints(stringBuffer.builder, index, codePointOffset);

            int length = seq.Length;
            if ((uint)index > (uint)length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

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
        public static int OffsetByCodePoints(this char[] seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            int length = seq.Length;
            if ((uint)index > (uint)length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

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
        public static int OffsetByCodePoints(this StringBuilder seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            int length = seq.Length;
            if ((uint)index > (uint)length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

            int x = index;
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> chars = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                seq.CopyTo(0, chars, length);
#else
                Span<char> chars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                seq.CopyTo(0, arrayToReturnToPool, 0, length);
#endif
                if (codePointOffset >= 0)
                {
                    int i;
                    for (i = 0; x < length && i < codePointOffset; i++)
                    {
                        if (char.IsHighSurrogate(chars[x++]))
                        {
                            if (x < length && char.IsLowSurrogate(chars[x]))
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
                        if (char.IsLowSurrogate(chars[--x]))
                        {
                            if (x > 0 && char.IsHighSurrogate(chars[x - 1]))
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
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
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
        public static int OffsetByCodePoints(this string seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            int length = seq.Length;
            if ((uint)index > (uint)length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

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
        public static int OffsetByCodePoints(this ReadOnlySpan<char> seq, int index, int codePointOffset) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            int length = seq.Length;
            if ((uint)index > (uint)length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(index, ExceptionArgument.index);

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
            if (seq is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seq);
            if (start < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(start, ExceptionArgument.start);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            if (count > seq.Length - start || index < start || index > start + count)
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentOutOfRangeException();
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#pragma warning restore IDE0079 // Remove unnecessary suppression

            return OffsetByCodePointsImpl(seq, start, count, index, codePointOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// The numeric value of <paramref name="c"/> in the specified <paramref name="radix"/>. Returns -1
        /// if the <paramref name="radix"/> lies between <see cref="MinRadix"/> and <see cref="MaxRadix"/> or
        /// if <paramref name="c"/> is not a decimal digit.
        /// </returns>
        public static int Digit(char c, int radix)
        {
            int result;
            if ((uint)(radix - Character.MinRadix) <= (Character.MaxRadix - Character.MinRadix)) // J2N: Optimized version of: (radix >= MinRadix && radix <= MaxRadix)
            {
                // Optimized for ASCII
                if (c < 128)
                {
                    result = ASCIIDigits[c];
                    return result < radix ? result : -1;
                }
                if (c < 256)
                {
                    return -1;
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

        /// <summary>
        /// Convenience method to determine the value of the specified character
        /// <paramref name="codePoint"/> in the supplied radix. The value of <paramref name="radix"/> must be
        /// between <see cref="MinRadix"/> and <see cref="MaxRadix"/>.
        /// </summary>
        /// <param name="codePoint">The character to determine the value of.</param>
        /// <param name="radix">The radix.</param>
        /// <returns>
        /// The numeric value of <paramref name="codePoint"/> in the specified <paramref name="radix"/>. Returns -1
        /// if the <paramref name="radix"/> lies between <see cref="MinRadix"/> and <see cref="MaxRadix"/> or
        /// if <paramref name="codePoint"/> is not a decimal digit.
        /// </returns>
        public static int Digit(int codePoint, int radix)
        {
            int result;
            if ((uint)(radix - Character.MinRadix) <= (Character.MaxRadix - Character.MinRadix)) // J2N: Optimized version of: (radix >= MinRadix && radix <= MaxRadix)
            {
                // Optimized for ASCII
                if (codePoint < 128)
                {
                    result = ASCIIDigits[codePoint];
                    return result < radix ? result : -1;
                }
                if (codePoint < 256)
                {
                    return -1;
                }
                int value = -1;
                if (codePoint < MinSupplementaryCodePoint) // BMP char
                {
                    result = BinarySearchRange(digitKeys, (char)codePoint);
                    if (result >= 0 && codePoint <= digitValues[result * 2])
                    {
                        value = (char)(codePoint - digitValues[result * 2 + 1]);
                    }
                }
                else
                {
                    result = BinarySearchRange(DigitSupplemental.Keys, codePoint);
                    if (result >= 0 && codePoint <= DigitSupplemental.Values[result * 2])
                    {
                        value = (char)(codePoint - DigitSupplemental.Values[result * 2 + 1]);
                    }
                }
                if (value >= radix)
                {
                    return -1;
                }
                return value;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsAsciiHexDigit(int c)
        {
            if (c < 128)
            {
                int value = ASCIIDigits[c];
                return value != -1 && value < 16; // Max hex radix
            }
            return false;
        }

        /// <summary>
        /// Search the sorted characters in the string and return the nearest index.
        /// </summary>
        /// <param name="data">The String to search.</param>
        /// <param name="c">The character to search for.</param>
        /// <returns>The nearest index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Search the sorted characters in the string and return the nearest index.
        /// </summary>
        /// <param name="data">The String to search.</param>
        /// <param name="codePoint">The character to search for.</param>
        /// <returns>The nearest index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BinarySearchRange(uint[] data, int codePoint)
        {
            uint value = 0;
            int low = 0, mid = -1, high = data.Length - 1;
            while (low <= high)
            {
                mid = (low + high) >> 1;
                value = data[mid];
                if (codePoint > value)
                    low = mid + 1;
                else if (codePoint == value)
                    return mid;
                else
                    high = mid - 1;
            }
            return mid - (codePoint < value ? 1 : 0);
        }

        /// <summary>
        /// Search the sorted characters in the string and return the nearest index.
        /// </summary>
        /// <param name="data">The String to search.</param>
        /// <param name="codePoint">The character to search for.</param>
        /// <returns>The nearest index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BinarySearchRange(ushort[] data, int codePoint)
        {
            ushort value = 0;
            int low = 0, mid = -1, high = data.Length - 1;
            while (low <= high)
            {
                mid = (low + high) >> 1;
                value = data[mid];
                if (codePoint > value)
                    low = mid + 1;
                else if (codePoint == value)
                    return mid;
                else
                    high = mid - 1;
            }
            return mid - (codePoint < value ? 1 : 0);
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
            if ((uint)(radix - Character.MinRadix) > (Character.MaxRadix - Character.MinRadix)) // J2N: Optimized version of: (radix < MinRadix || radix > MaxRadix)
                return charNull;
            if ((uint)digit >= (uint)radix) // J2N: Optimized version of: (digit < 0 || digit >= radix)
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
            // Optimized for ASCII
            if (c < 128)
            {
                return ASCIIDigits[c];
            }
            int result;
            if (c < 0x2187) // Normal
            {
                result = BinarySearchRange(numericKeys, c);
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
            }
            else //if (c < MinSupplementaryCodePoint) // Intermediate
            {
                result = BinarySearchRange(NumericIntermediate.Keys, c);
                if (result >= 0 && c <= NumericIntermediate.Values[result * 2])
                {
                    uint difference = NumericIntermediate.Values[result * 2 + 1];
                    if (difference == 0)
                    {
                        return -2;
                    }
                    return (int)(c - difference);
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the numeric value of the specified Unicode code point.
        /// </summary>
        /// <param name="codePoint">The Unicode character to get the numeric value of.</param>
        /// <returns>A non-negative numeric integer value if a numeric value for
        /// <paramref name="codePoint"/> exists, -1 if there is no numeric value for <paramref name="codePoint"/>,
        /// -2 if the numeric value can not be represented with an integer.</returns>
        public static int GetNumericValue(int codePoint)
        {
            // Optimized for ASCII
            if (codePoint < 128)
            {
                return ASCIIDigits[codePoint];
            }
            int result;
            if (codePoint < 0x2187) // Normal
            {
                result = BinarySearchRange(numericKeys, (char)codePoint);
                if (result >= 0 && codePoint <= numericValues[result * 2])
                {
                    char difference = numericValues[result * 2 + 1];
                    if (difference == 0)
                    {
                        return -2;
                    }
                    // Value is always positive, must be negative value
                    if (difference > codePoint)
                    {
                        return codePoint - (short)difference;
                    }
                    return codePoint - difference;
                }
            }
            else if (codePoint < MinSupplementaryCodePoint) // Intermediate (difference variable of type char not big enough to support all subtraction)
            {
                result = BinarySearchRange(NumericIntermediate.Keys, codePoint);
                if (result >= 0 && codePoint <= NumericIntermediate.Values[result * 2])
                {
                    uint difference = NumericIntermediate.Values[result * 2 + 1];
                    if (difference == 0)
                    {
                        return -2;
                    }
                    return (int)(codePoint - difference);
                }
            }
            else // Supplementary
            {
                result = BinarySearchRange(NumericSupplemental.Keys, codePoint);
                if (result >= 0 && codePoint <= NumericSupplemental.Values[result * 2])
                {
                    uint difference = NumericSupplemental.Values[result * 2 + 1];
                    if (difference == 0)
                    {
                        return -2;
                    }
                    return (int)(codePoint - difference);
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the general Unicode category of the specified character.
        /// <para/>
        /// This mimics the behavior of the Java Character.GetType(int) method, but returns the .NET <see cref="UnicodeCategory"/>
        /// enumeration for easy consumption.
        /// </summary>
        /// <param name="c">The character to get the category of.</param>
        /// <returns>The <see cref="UnicodeCategory"/> of <paramref name="c"/>.</returns>
        public static UnicodeCategory GetType(char c)
            => CharUnicodeInfo.GetUnicodeCategory(c);

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
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                return UnicodeCategory.OtherNotAssigned;
#if FEATURE_CHARUNICODEINFO_GETUNICODECATEGORY_CODEPOINT
            return CharUnicodeInfo.GetUnicodeCategory(codePoint);
#else
            if (codePoint >= 0x00d800 && codePoint <= 0x00dfff)
                return CharUnicodeInfo.GetUnicodeCategory((char)codePoint);
            else
                return CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(codePoint), 0);
#endif
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(int codePoint)
        {
            return GetType(codePoint) != UnicodeCategory.OtherNotAssigned;
        }

        /// <summary>
        /// Indicates whether the specified character is a digit.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a digit; <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsISOControl(char c)
        {
            return (c >= 0 && c <= 0x1f) || (c >= 0x7f && c <= 0x9f);
        }

        /// <summary>
        /// Indicates whether the specified code point is an ISO control character.
        /// </summary>
        /// <param name="codePoint">The code point to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is an ISO control character; <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Indicates whether the specified character (Unicode code point) is a whitespace character according to Java.
        /// A character is a Java whitespace character if and only if it satisfies one of the following criteria:
        /// <list type="bullet">
        ///     <item><description>It is a Unicode space character (<see cref="UnicodeCategory.SpaceSeparator"/>,
        ///     <see cref="UnicodeCategory.LineSeparator"/>, or <see cref="UnicodeCategory.ParagraphSeparator"/>)
        ///     but it is not also a non-breaking space ('\u00A0', '\u2007', '\u202F').</description></item>
        ///     <item><description>It is '\t', U+0009 HORIZONTAL TABULATION.</description></item>
        ///     <item><description>It is '\n', U+000A LINE FEED.</description></item>
        ///     <item><description>It is '\u000B', U+000B VERTICAL TABULATION.</description></item>
        ///     <item><description>It is '\f', U+000C FORM FEED.</description></item>
        ///     <item><description>It is '\r', U+000D CARRIAGE RETURN.</description></item>
        ///     <item><description>It is '\u001C', U+001C FILE SEPARATOR.</description></item>
        ///     <item><description>It is '\u001D', U+001D GROUP SEPARATOR.</description></item>
        ///     <item><description>It is '\u001E', U+001E RECORD SEPARATOR.</description></item>
        ///     <item><description>It is '\u001F', U+001F UNIT SEPARATOR.</description></item>
        /// </list>
        /// <para/>
        /// Usage Note: This method differs from <see cref="char.IsWhiteSpace(char)"/> in that it returns <c>false</c>
        /// for the following characters:
        /// <list type="bullet">
        ///     <item><description>'\u0085', NEXT LINE</description></item>
        ///     <item><description>'\u00A0', NO-BREAK SPACE</description></item>
        ///     <item><description>'\u2007', FIGURE SPACE</description></item>
        ///     <item><description>'\u202F', NARROW NO-BREAK SPACE</description></item>
        /// </list>
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a Java whitespace character, <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(char c)
        {
            return IsWhiteSpace((int)c);
        }

        /// <summary>
        /// Indicates whether the specified character (Unicode code point) is a whitespace character according to Java.
        /// A character is a Java whitespace character if and only if it satisfies one of the following criteria:
        /// <list type="bullet">
        ///     <item><description>It is a Unicode space character (<see cref="UnicodeCategory.SpaceSeparator"/>,
        ///     <see cref="UnicodeCategory.LineSeparator"/>, or <see cref="UnicodeCategory.ParagraphSeparator"/>)
        ///     but it is not also a non-breaking space ('\u00A0', '\u2007', '\u202F').</description></item>
        ///     <item><description>It is '\t', U+0009 HORIZONTAL TABULATION.</description></item>
        ///     <item><description>It is '\n', U+000A LINE FEED.</description></item>
        ///     <item><description>It is '\u000B', U+000B VERTICAL TABULATION.</description></item>
        ///     <item><description>It is '\f', U+000C FORM FEED.</description></item>
        ///     <item><description>It is '\r', U+000D CARRIAGE RETURN.</description></item>
        ///     <item><description>It is '\u001C', U+001C FILE SEPARATOR.</description></item>
        ///     <item><description>It is '\u001D', U+001D GROUP SEPARATOR.</description></item>
        ///     <item><description>It is '\u001E', U+001E RECORD SEPARATOR.</description></item>
        ///     <item><description>It is '\u001F', U+001F UNIT SEPARATOR.</description></item>
        /// </list>
        /// <para/>
        /// Usage Note: This method differs from <see cref="char.IsWhiteSpace(string, int)"/> in that it returns <c>false</c>
        /// for the following characters:
        /// <list type="bullet">
        ///     <item><description>'\u0085', NEXT LINE</description></item>
        ///     <item><description>'\u00A0', NO-BREAK SPACE</description></item>
        ///     <item><description>'\u2007', FIGURE SPACE</description></item>
        ///     <item><description>'\u202F', NARROW NO-BREAK SPACE</description></item>
        /// </list>
        /// </summary>
        /// <param name="codePoint">The character to check.</param>
        /// <returns><c>true</c> if <paramref name="codePoint"/> is a Java whitespace character, <c>false</c> otherwise.</returns>
        public static bool IsWhiteSpace(int codePoint)
        {
            // Optimized case for ASCII
            if ((codePoint >= 0x1c && codePoint <= 0x20) || (codePoint >= 0x9 && codePoint <= 0xd))
                return true;
            if (codePoint == 0x1680)
                return true;
            if (codePoint < 0x2000 || codePoint == 0x2007)
                return false;
            return codePoint < 0x200b || codePoint == 0x2028 || codePoint == 0x2029 || codePoint == 0x205f || codePoint == 0x3000;
        }

        /// <summary>
        /// Reverses the order of the first and second byte in the specified
        /// character.
        /// </summary>
        /// <param name="c">The character to reverse.</param>
        /// <returns>The character with reordered bytes.</returns>
        [Obsolete("Use ReverseEndianness(char) instead.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReverseBytes(char c)
            => ReverseEndianness(c);

        /// <summary>
        /// Reverses the order of the first and second byte in the specified
        /// character.
        /// </summary>
        /// <param name="value">The character to reverse.</param>
        /// <returns>The character with reordered bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReverseEndianness(char value)
            => (char)BinaryPrimitives.ReverseEndianness((ushort)value);

        /// <summary>
        /// Converts the character argument to
        /// lowercase using the current culture.
        /// </summary>
        /// <param name="c">The character to be converted.</param>
        /// <returns>The lowercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToLower(char c) => char.ToLower(c);

        /// <summary>
        /// Converts the character argument to
        /// lowercase using the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="c">The character to be converted.</param>
        /// <param name="culture">An object that specifies culture-specific casing rules.</param>
        /// <returns>The lowercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="culture"/> is <c>null</c>.</exception>
        public static int ToLower(char c, CultureInfo culture) => char.ToLower(c, culture);

        /// <summary>
        /// Converts the character (Unicode code point) argument to
        /// lowercase using the current culture.
        /// </summary>
        /// <param name="codePoint">The character (Unicode code point) to be converted.</param>
        /// <returns>The lowercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentException">If the <paramref name="codePoint"/> is invalid.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            if (culture is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.culture);
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            // Fast path - convert using char if not a surrogate pair
            if (codePoint < MinSupplementaryCodePoint)
                return char.ToLower((char)codePoint, culture);

            return ToLower_Supplementary(codePoint, culture);

            static int ToLower_Supplementary(int codePoint, CultureInfo culture)
            {
                Span<char> buffer = stackalloc char[2];
                ToChars(codePoint, buffer, 0);
                Span<char> result = stackalloc char[2];
                System.MemoryExtensions.ToLower(buffer, result, culture);
                return ToCodePoint(result[0], result[1]);
            }
        }


        /// <summary>
        /// Converts the character argument to
        /// lowercase using the invariant culture.
        /// </summary>
        /// <param name="c">The character to be converted.</param>
        /// <returns>The lowercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        public static int ToLowerInvariant(char c) => char.ToLowerInvariant(c);

        /// <summary>
        /// Converts the character (Unicode code point) argument to
        /// lowercase using the invariant culture.
        /// </summary>
        /// <param name="codePoint">The character (Unicode code point) to be converted.</param>
        /// <returns>The lowercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentException">If the <paramref name="codePoint"/> is invalid.</exception>
        public static int ToLowerInvariant(int codePoint)
        {
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            // Fast path - convert using char if not a surrogate pair
            if (codePoint < MinSupplementaryCodePoint)
                return char.ToLowerInvariant((char)codePoint);

            return ToLowerInvariant_Supplementary(codePoint);

            static int ToLowerInvariant_Supplementary(int codePoint)
            {
                Span<char> buffer = stackalloc char[2];
                ToChars(codePoint, buffer, 0);
                Span<char> result = stackalloc char[2];
                System.MemoryExtensions.ToLowerInvariant(buffer, result);
                return ToCodePoint(result[0], result[1]);
            }
        }

        /// <summary>
        /// Converts the character argument to
        /// uppercase using the current culture.
        /// </summary>
        /// <param name="c">The character to be converted.</param>
        /// <returns>The uppercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToUpper(char c) => char.ToUpper(c);

        /// <summary>
        /// Converts the character argument to
        /// uppercase using the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="c">The character to be converted.</param>
        /// <param name="culture">An object that specifies culture-specific casing rules.</param>
        /// <returns>The uppercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="culture"/> is <c>null</c>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToUpper(char c, CultureInfo culture) => char.ToUpper(c, culture);

        /// <summary>
        /// Converts the character (Unicode code point) argument to
        /// uppercase using the current culture.
        /// </summary>
        /// <param name="codePoint">The character (Unicode code point) to be converted.</param>
        /// <returns>The uppercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentException">If the <paramref name="codePoint"/> is invalid.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            if (culture is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.culture);
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            // Fast path - convert using char if not a surrogate pair
            if (codePoint < MinSupplementaryCodePoint)
                return char.ToUpper((char)codePoint, culture);

            return ToUpper_Supplementary(codePoint, culture);

            static int ToUpper_Supplementary(int codePoint, CultureInfo culture)
            {
                Span<char> buffer = stackalloc char[2];
                ToChars(codePoint, buffer, 0);
                Span<char> result = stackalloc char[2];
                System.MemoryExtensions.ToUpper(buffer, result, culture);
                return ToCodePoint(result[0], result[1]);
            }
        }

        /// <summary>
        /// Converts the character argument to
        /// uppercase using the invariant culture.
        /// </summary>
        /// <param name="c">The character to be converted.</param>
        /// <returns>The uppercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToUpperInvariant(char c) => char.ToUpperInvariant(c);

        /// <summary>
        /// Converts the character (Unicode code point) argument to
        /// uppercase using the invariant culture.
        /// </summary>
        /// <param name="codePoint">The character (Unicode code point) to be converted.</param>
        /// <returns>The uppercase equivalent of the character, if any;
        /// otherwise, the character itself.</returns>
        /// <exception cref="ArgumentException">If the <paramref name="codePoint"/> is invalid.</exception>
        public static int ToUpperInvariant(int codePoint)
        {
            if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            // Fast path - convert using char if not a surrogate pair
            if (codePoint < MinSupplementaryCodePoint)
                return char.ToUpperInvariant((char)codePoint);

            return ToUpperInvariant_Supplementary(codePoint);

            static int ToUpperInvariant_Supplementary(int codePoint)
            {
                Span<char> buffer = stackalloc char[2];
                ToChars(codePoint, buffer, 0);
                Span<char> result = stackalloc char[2];
                System.MemoryExtensions.ToUpperInvariant(buffer, result);
                return ToCodePoint(result[0], result[1]);
            }
        }

        // J2N: Since .NET's string class has no constructor that accepts an array of code points, we have extra helper methods that
        // allow us to make the conversion. Character seems like the most logical place to do this being that there is no way to dynamically
        // add a construtor to System.String and this is the place in J2N that deals the most with code points.

        /// <summary>
        /// Converts a sequence <paramref name="codePoints"/> to a <see cref="string"/> of UTF-16 code units.
        /// <para/>
        /// Usage Note: In the JDK, there is a constructor overload that accept code points and turn them into a string. This is
        /// the .NET equivalent of that constructor overload, however this overload is provided for convenience and assumes the
        /// whole array will be converted. <see cref="ToString(ReadOnlySpan{int}, int, int)"/> allows conversion of a partial array of code points to a
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="codePoints">A <see cref="ReadOnlySpan{T}"/> of UTF-32 code points.</param>
        /// <returns>A string containing the UTF-16 character equivalent of <paramref name="codePoints"/>.</returns>
        /// <exception cref="ArgumentException">One of the <paramref name="codePoints"/> is not a valid Unicode
        /// code point (see <see cref="IsValidCodePoint(int)"/>).</exception>
        public static string ToString(ReadOnlySpan<int> codePoints)
        {
            int length = codePoints.Length;
            // 256 code points max. Since we don't measure the char length, we will allocate up to 512 chars on the stack (1024 bytes).
            if (length <= CodePointStackBufferSize)
            {
                unsafe
                {
                    // Initialize our array on the stack with enough space
                    // assuming the entire string consists of surrogate pairs
                    // (worst case scenario).
                    char* buffer = stackalloc char[length * 2];
                    fixed (int* codePointsPtr = &MemoryMarshal.GetReference(codePoints))
                    {
                        int stringLength = WriteCodePointsToCharBuffer(buffer, codePointsPtr, startIndex: 0, length);
                        return new string(buffer, 0, stringLength);
                    }
                }
            }

            return ToStringSlow(codePoints, startIndex: 0, length);
        }

        /// <summary>
        /// Converts an array <paramref name="codePoints"/> to a <see cref="string"/> of UTF-16 code units.
        /// <para/>
        /// Usage Note: In the JDK, there is a constructor overload that accept code points and turn them into a string. This is
        /// the .NET equivalent of that constructor overload, however this overload is provided for convenience and assumes the
        /// whole array will be converted. <see cref="ToString(int[], int, int)"/> allows conversion of a partial array of code points to a
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="codePoints">An array of UTF-32 code points.</param>
        /// <returns>A string containing the UTF-16 character equivalent of <paramref name="codePoints"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="codePoints"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">One of the <paramref name="codePoints"/> is not a valid Unicode
        /// code point (see <see cref="IsValidCodePoint(int)"/>).</exception>
        public static string ToString(int[] codePoints)
        {
            if (codePoints is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.codePoints);

            int length = codePoints.Length;
            // 256 code points max. Since we don't measure the char length, we will allocate up to 512 chars on the stack (1024 bytes).
            if (length <= CodePointStackBufferSize)
            {
                unsafe
                {
                    // Initialize our array on the stack with enough space
                    // assuming the entire string consists of surrogate pairs
                    // (worst case scenario).
                    char* buffer = stackalloc char[length * 2];
                    fixed (int* codePointsPtr = codePoints)
                    {
                        int stringLength = WriteCodePointsToCharBuffer(buffer, codePointsPtr, startIndex: 0, length);
                        return new string(buffer, 0, stringLength);
                    }
                }
            }

            return ToStringSlow(codePoints, startIndex: 0, length);
        }

        /// <summary>
        /// Converts a sequence <paramref name="codePoints"/> to a <see cref="string"/> of UTF-16 code units.
        /// <para/>
        /// Usage Note: In the JDK, there is a constructor overload that accept code points and turn them into a string. This is
        /// the .NET equivalent of that constructor overload.
        /// </summary>
        /// <param name="codePoints">A <see cref="ReadOnlySpan{T}"/> of UTF-32 code points.</param>
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
        /// <exception cref="ArgumentException">One of the <paramref name="codePoints"/> is not a valid Unicode
        /// code point (see <see cref="IsValidCodePoint(int)"/>).</exception>
        public static string ToString(ReadOnlySpan<int> codePoints, int startIndex, int length)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > codePoints.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            // 256 code points max. Since we don't measure the char length, we will allocate up to 512 chars on the stack (1024 bytes).
            if (length <= CodePointStackBufferSize)
            {
                unsafe
                {
                    // Initialize our array on the stack with enough space
                    // assuming the entire string consists of surrogate pairs
                    // (worst case scenario).
                    char* buffer = stackalloc char[length * 2];
                    fixed (int* codePointsPtr = &MemoryMarshal.GetReference(codePoints))
                    {
                        int stringLength = WriteCodePointsToCharBuffer(buffer, codePointsPtr, startIndex, length);
                        return new string(buffer, 0, stringLength);
                    }
                }
            }

            return ToStringSlow(codePoints, startIndex, length);
        }

        /// <summary>
        /// Converts an array <paramref name="codePoints"/> to a <see cref="string"/> of UTF-16 code units.
        /// <para/>
        /// Usage Note: In the JDK, there is a constructor overload that accept code points and turn them into a string. This is
        /// the .NET equivalent of that constructor overload.
        /// </summary>
        /// <param name="codePoints">An array of UTF-32 code points.</param>
        /// <param name="startIndex">The index of the first code point to convert.</param>
        /// <param name="length">The number of code point elements to to convert to UTF-32 code units and include in the result.</param>
        /// <returns>A <see cref="string"/> containing the UTF-16 code units that correspond to the specified range of <paramref name="codePoints"/> elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="codePoints"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within <paramref name="codePoints"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">One of the <paramref name="codePoints"/> is not a valid Unicode
        /// code point (see <see cref="IsValidCodePoint(int)"/>).</exception>
        public static string ToString(int[] codePoints, int startIndex, int length)
        {
            if (codePoints is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.codePoints);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > codePoints.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            // 256 code points max. Since we don't measure the char length, we will allocate up to 512 chars on the stack (1024 bytes).
            if (length <= CodePointStackBufferSize)
            {
                unsafe
                {
                    // Initialize our array on the stack with enough space
                    // assuming the entire string consists of surrogate pairs
                    // (worst case scenario).
                    char* buffer = stackalloc char[length * 2];
                    fixed (int* codePointsPtr = codePoints)
                    {
                        int stringLength = WriteCodePointsToCharBuffer(buffer, codePointsPtr, startIndex, length);
                        return new string(buffer, 0, stringLength);
                    }
                }
            }

            return ToStringSlow(codePoints, startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetStringLength(ReadOnlySpan<int> codePoints, int startIndex, int length)
        {
            int result = 0;
            int end = startIndex + length; // 1 past the end index
            for (int j = startIndex; j < end; ++j)
            {
                result += CharCount(codePoints[j]);
            }
            return result;
        }

#if FEATURE_STRING_CREATE

        private static string ToStringSlow(int[] codePoints, int startIndex, int length)
        {
            int stringLength = GetStringLength(codePoints, startIndex, length);
            return string.Create(stringLength, (codePoints, startIndex, length), WriteString);
        }

        private static string ToStringSlow(ReadOnlySpan<int> codePoints, int startIndex, int length)
        {
            int stringLength = GetStringLength(codePoints, startIndex, length);
            int[] buffer = ArrayPool<int>.Shared.Rent(length);
            try
            {
                codePoints.Slice(startIndex, length).CopyTo(buffer);
                return string.Create(stringLength, (buffer, 0, length), WriteString);
            }
            finally
            {
                ArrayPool<int>.Shared.Return(buffer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static void WriteString(Span<char> chars, ValueTuple<int[], int, int> state)
        {
            fixed (char* charsPtr = &MemoryMarshal.GetReference(chars))
            fixed (int* codePointsPtr = state.Item1)
            {
                WriteCodePointsToCharBuffer(charsPtr, codePointsPtr, startIndex: state.Item2, length: state.Item3);
            }
        }

#else
        private unsafe static string ToStringSlow(ReadOnlySpan<int> codePoints, int startIndex, int length)
        {
            int stringLength = GetStringLength(codePoints, startIndex, length);
            char[] chars = ArrayPool<char>.Shared.Rent(stringLength);
            try
            {
                fixed (char* charsPtr = chars)
                fixed (int* codePointsPtr = &MemoryMarshal.GetReference(codePoints))
                {
                    WriteCodePointsToCharBuffer(charsPtr, codePointsPtr, startIndex, length);
                    return new string(charsPtr, 0, stringLength);
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(chars);
            }
        }
#endif

        /// <summary>
        /// Converts an an array <paramref name="codePoints"/> to a buffer of UTF-16 code units.
        /// </summary>
        /// <param name="buffer">A pointer to the first character of a <see cref="char"/> array.</param>
        /// <param name="codePoints">An array of UTF-32 code points.</param>
        /// <param name="startIndex">The index of the first code point to convert.</param>
        /// <param name="length">The number of code point elements to to convert to UTF-32 code units and include in the result.</param>
        /// <returns>The total number of <see cref="char"/>s copied to the buffer.</returns>
        /// <exception cref="ArgumentException">One of the <paramref name="codePoints"/> is not a valid Unicode
        /// code point (see <see cref="IsValidCodePoint(int)"/>).</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static int WriteCodePointsToCharBuffer(char* buffer, int* codePoints, int startIndex, int length)
        {
            int index = 0;
            int end = startIndex + length; // 1 past the end index
            for (int i = startIndex; i < end; i++)
            {
                int codePoint = codePoints[i];
                if ((uint)codePoint > MaxCodePoint) // Optimized version of !IsValidCodePoint()
                    ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

                if (codePoint < MinSupplementaryCodePoint) // BMP char
                {
                    buffer[index++] = (char)codePoint;
                }
                else // Surrogate pair
                {
                    // See RFC 2781, Section 2.1
                    // http://www.faqs.org/rfcs/rfc2781.html
                    int cpPrime = codePoint - 0x10000;
                    int high = 0xD800 | ((cpPrime >> 10) & 0x3FF);
                    int low = 0xDC00 | (cpPrime & 0x3FF);
                    buffer[index++] = (char)high;
                    buffer[index++] = (char)low;
                }
            }
            return index; // This is now the total length
        }
    }
}
