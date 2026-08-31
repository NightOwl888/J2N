#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;
using System.Globalization;
using System.IO;

namespace J2N.Unicode
{
    /// <summary>
    /// The expected <see cref="Character.Digit(int, int)"/>, <see cref="Character.GetNumericValue(int)"/>
    /// and <see cref="Character.IsWhiteSpace(int)"/> values for every Unicode code point, loaded from an
    /// embedded data file that is generated from the Unicode Character Database (UCD).
    /// <para/>
    /// This is the oracle the <see cref="TestCharacter"/> tests compare J2N against. It replaces the former
    /// dependency on ICU4N: the data comes straight from unicode.org, so the Unicode version J2N targets is
    /// no longer pinned to whichever Unicode version a given ICU4N release happens to ship, and the test
    /// projects no longer need to restore and load ICU4N.
    /// <para/>
    /// To regenerate the data file (for example, to move to a newer Unicode version), run:
    /// <code>
    /// dotnet run --project tools/GenerateUnicodeData -- &lt;unicodeVersion&gt;
    /// </code>
    /// </summary>
    public static class UnicodeCharacterData
    {
        private const string ResourceName = "UnicodeCharacterData.txt";

        /// <summary>
        /// The value reported when a code point has no digit or numeric value.
        /// </summary>
        public const int None = -1;

        /// <summary>
        /// The value reported when a code point has a numeric value that cannot be
        /// represented as an <see cref="int"/> (for example a fraction such as 1/2).
        /// </summary>
        public const int NotRepresentable = -2;

        private static readonly int[] digitValues = new int[Character.MaxCodePoint + 1];
        private static readonly int[] numericValues = new int[Character.MaxCodePoint + 1];
        private static readonly bool[] whiteSpaceValues = new bool[Character.MaxCodePoint + 1];

        /// <summary>
        /// The Unicode version the data file was generated from.
        /// </summary>
        public static string UnicodeVersion { get; private set; } = string.Empty;

        static UnicodeCharacterData()
        {
            using Stream stream = typeof(UnicodeCharacterData).FindAndGetManifestResourceStream(ResourceName)
                ?? throw new InvalidOperationException(
                    $"The embedded resource '{ResourceName}' was not found. Run 'dotnet run --project tools/GenerateUnicodeData' to generate it.");
            using var reader = new StreamReader(stream);

            int expectedCodePoint = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                if (line[0] == '#')
                {
                    const string VersionPrefix = "# Unicode-Version:";
                    if (line.StartsWith(VersionPrefix, StringComparison.Ordinal))
                    {
                        UnicodeVersion = line.Substring(VersionPrefix.Length).Trim();
                    }
                    continue;
                }

                string[] fields = line.Split(';');
                int start = int.Parse(fields[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                int end = int.Parse(fields[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                int digit = int.Parse(fields[2], CultureInfo.InvariantCulture);
                int numeric = int.Parse(fields[3], CultureInfo.InvariantCulture);
                bool whiteSpace = fields[4] == "1";

                // The rows are expected to tile U+0000..U+10FFFF exactly. Verifying that here means a
                // truncated or corrupt data file fails loudly rather than silently testing against zeros.
                if (start != expectedCodePoint)
                {
                    throw new InvalidOperationException(
                        $"The data in '{ResourceName}' is not contiguous: expected the row starting at U+{expectedCodePoint:X4} but found U+{start:X4}.");
                }

                for (int codePoint = start; codePoint <= end; codePoint++)
                {
                    digitValues[codePoint] = digit;
                    numericValues[codePoint] = numeric;
                    whiteSpaceValues[codePoint] = whiteSpace;
                }

                expectedCodePoint = end + 1;
            }

            if (expectedCodePoint != Character.MaxCodePoint + 1)
            {
                throw new InvalidOperationException(
                    $"The data in '{ResourceName}' ends at U+{expectedCodePoint - 1:X4} but should cover through U+{Character.MaxCodePoint:X4}.");
            }
        }

        /// <summary>
        /// Gets the expected value of <paramref name="codePoint"/> when interpreted in the
        /// supplied <paramref name="radix"/>, or -1 if it is not a digit in that radix.
        /// </summary>
        public static int Digit(int codePoint, int radix)
        {
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
            {
                return None;
            }
            if ((uint)codePoint > Character.MaxCodePoint)
            {
                return None;
            }

            int value = digitValues[codePoint];
            return value >= 0 && value < radix ? value : None;
        }

        /// <summary>
        /// Gets the expected numeric value of <paramref name="codePoint"/>, -1 if it has no
        /// numeric value, or -2 if the value cannot be represented as an <see cref="int"/>.
        /// </summary>
        public static int GetNumericValue(int codePoint)
        {
            if ((uint)codePoint > Character.MaxCodePoint)
            {
                return None;
            }
            return numericValues[codePoint];
        }

        /// <summary>
        /// Gets a value indicating whether <paramref name="codePoint"/> is expected to be
        /// treated as whitespace.
        /// </summary>
        public static bool IsWhiteSpace(int codePoint)
        {
            if ((uint)codePoint > Character.MaxCodePoint)
            {
                return false;
            }
            return whiteSpaceValues[codePoint];
        }
    }
}
