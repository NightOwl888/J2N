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

using System.Globalization;
using System.IO.Compression;

// Generates the compact Unicode character data file that J2N.Tests uses as the
// expected-value oracle for Character.Digit(), Character.GetNumericValue() and
// Character.IsWhiteSpace().
//
// The data is derived directly from the Unicode Character Database (UCD) published
// at unicode.org, which replaces the former dependency on ICU4N. Because the UCD is
// consumed directly, the Unicode version J2N targets is no longer tied to the
// Unicode version a given ICU4N release happens to ship.
//
// Usage:
//     dotnet run --project tools/GenerateUnicodeData -- <unicodeVersion> [outputFile]
//
// Example:
//     dotnet run --project tools/GenerateUnicodeData -- 10.0.0



string unicodeVersion = args.Length > 0 ? args[0] : "10.0.0";
string outputFile = args.Length > 1
    ? args[1]
    : Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..",
        "tests", "NUnit", "J2N.Tests", "Unicode", "UnicodeCharacterData.txt");

string baseUrl = $"https://www.unicode.org/Public/{unicodeVersion}/ucd";

Console.WriteLine($"Unicode version: {unicodeVersion}");

using var http = new HttpClient();

// The general category, decimal digit value and numeric value of most code points.
Console.WriteLine("Downloading UnicodeData.txt...");
string unicodeData = await http.GetStringAsync($"{baseUrl}/UnicodeData.txt").ConfigureAwait(false);

// Numeric values for CJK ideographs, which UnicodeData.txt does not carry.
Console.WriteLine("Downloading Unihan.zip...");
byte[] unihanZip = await http.GetByteArrayAsync($"{baseUrl}/Unihan.zip").ConfigureAwait(false);
string unihanNumericValues = ReadUnihanNumericValues(unihanZip);

var generalCategory = new string?[Ucd.MaxCodePoint + 1];
var decimalDigitValue = new int[Ucd.MaxCodePoint + 1];
var numericValue = new long[Ucd.MaxCodePoint + 1];
Array.Fill(decimalDigitValue, -1);
Array.Fill(numericValue, Ucd.NumericNone);

ParseUnihanNumericValues(unihanNumericValues, numericValue);
ParseUnicodeData(unicodeData, generalCategory, decimalDigitValue, numericValue);

WriteRunLengthEncodedData(outputFile, unicodeVersion, generalCategory, decimalDigitValue, numericValue);

Console.WriteLine($"Wrote {outputFile}");

static string ReadUnihanNumericValues(byte[] zipBytes)
{
    using var stream = new MemoryStream(zipBytes);
    using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
    ZipArchiveEntry entry = archive.GetEntry("Unihan_NumericValues.txt")
        ?? throw new InvalidOperationException("Unihan_NumericValues.txt was not found in Unihan.zip.");
    using var entryStream = entry.Open();
    using var reader = new StreamReader(entryStream);
    return reader.ReadToEnd();
}

// kPrimaryNumeric, kAccountingNumeric and kOtherNumeric all contribute to the
// numeric value of a CJK ideograph. The file lists at most one of them per code point.
static void ParseUnihanNumericValues(string text, long[] numericValue)
{
    foreach (string line in EnumerateLines(text))
    {
        if (line.Length == 0 || line[0] == '#')
        {
            continue;
        }

        string[] fields = line.Split('\t');
        if (fields.Length < 3)
        {
            continue;
        }

        int codePoint = int.Parse(fields[0].AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        numericValue[codePoint] = ParseNumericValue(fields[2].Trim());
    }
}

static void ParseUnicodeData(string text, string?[] generalCategory, int[] decimalDigitValue, long[] numericValue)
{
    // Ranges are expressed as a pair of "<..., First>" / "<..., Last>" rows.
    int rangeStart = -1;
    string rangeCategory = string.Empty, rangeDigitField = string.Empty, rangeNumericField = string.Empty;

    foreach (string line in EnumerateLines(text))
    {
        if (line.Length == 0)
        {
            continue;
        }

        string[] fields = line.Split(';');
        int codePoint = int.Parse(fields[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        string name = fields[1];
        string category = fields[2];
        string digitField = fields[6];
        string numericField = fields[8];

        if (name.EndsWith(", First>", StringComparison.Ordinal))
        {
            rangeStart = codePoint;
            rangeCategory = category;
            rangeDigitField = digitField;
            rangeNumericField = numericField;
            continue;
        }

        if (name.EndsWith(", Last>", StringComparison.Ordinal))
        {
            for (int c = rangeStart; c <= codePoint; c++)
            {
                generalCategory[c] = rangeCategory;
                Assign(c, rangeDigitField, rangeNumericField, decimalDigitValue, numericValue);
            }
            rangeStart = -1;
            continue;
        }

        generalCategory[codePoint] = category;
        Assign(codePoint, digitField, numericField, decimalDigitValue, numericValue);
    }

    static void Assign(int codePoint, string digitField, string numericField, int[] decimalDigitValue, long[] numericValue)
    {
        if (digitField.Length != 0)
        {
            decimalDigitValue[codePoint] = int.Parse(digitField, CultureInfo.InvariantCulture);
        }
        if (numericField.Length != 0)
        {
            numericValue[codePoint] = ParseNumericValue(numericField);
        }
    }
}

static void WriteRunLengthEncodedData(string outputFile, string unicodeVersion, string?[] generalCategory, int[] decimalDigitValue, long[] numericValue)
{
    string? directory = Path.GetDirectoryName(Path.GetFullPath(outputFile));
    if (directory is not null)
    {
        Directory.CreateDirectory(directory);
    }

    using var writer = new StreamWriter(Path.GetFullPath(outputFile)) { NewLine = "\n" };
    writer.WriteLine("# Expected Character.Digit(), Character.GetNumericValue() and Character.IsWhiteSpace()");
    writer.WriteLine("# values, derived from the Unicode Character Database (https://www.unicode.org/Public/).");
    writer.WriteLine("#");
    writer.WriteLine("# Generated by tools/GenerateUnicodeData - DO NOT EDIT BY HAND.");
    writer.WriteLine($"# Unicode-Version: {unicodeVersion}");
    writer.WriteLine("#");
    writer.WriteLine("# Format: startCodePoint;endCodePoint;digitValue;numericValue;isWhiteSpace");
    writer.WriteLine("# Code points are hexadecimal; all rows together cover U+0000..U+10FFFF.");
    writer.WriteLine("# digitValue and numericValue use -1 for \"no value\" and -2 for");
    writer.WriteLine("# \"value cannot be represented as an Int32\".");

    int runStart = 0;
    (int Digit, int Numeric, bool WhiteSpace)? previous = null;
    int rowCount = 0;

    for (int codePoint = 0; codePoint <= Ucd.MaxCodePoint; codePoint++)
    {
        var current = (
            Digit: GetDigitValue(codePoint, generalCategory, decimalDigitValue),
            Numeric: GetNumericValue(codePoint, numericValue),
            WhiteSpace: IsWhiteSpace(codePoint, generalCategory));

        if (previous is null)
        {
            previous = current;
            runStart = codePoint;
            continue;
        }

        // Only code points that carry no digit or numeric value can be coalesced into a
        // run; where a value exists it increments per code point, so each one is its own row.
        if (current == previous.Value && current.Digit == -1 && current.Numeric == -1)
        {
            continue;
        }

        WriteRow(writer, runStart, codePoint - 1, previous.Value);
        rowCount++;
        previous = current;
        runStart = codePoint;
    }

    WriteRow(writer, runStart, Ucd.MaxCodePoint, previous!.Value);
    rowCount++;

    Console.WriteLine($"{rowCount} rows");

    static void WriteRow(StreamWriter writer, int start, int end, (int Digit, int Numeric, bool WhiteSpace) value)
    {
        writer.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"{start:X};{end:X};{value.Digit};{value.Numeric};{(value.WhiteSpace ? 1 : 0)}"));
    }
}

// Mirrors java.lang.Character.digit()/ICU's u_digit(): the Latin and fullwidth
// letters act as radix digits in addition to the code points with General_Category=Nd.
static int GetDigitValue(int codePoint, string?[] generalCategory, int[] decimalDigitValue)
{
    int value;
    if (codePoint >= 'a' && codePoint <= 'z')
    {
        value = codePoint - 'a' + 10;
    }
    else if (codePoint >= 'A' && codePoint <= 'Z')
    {
        value = codePoint - 'A' + 10;
    }
    else if (codePoint >= 0xFF41 && codePoint <= 0xFF5A)
    {
        value = codePoint - 0xFF41 + 10;
    }
    else if (codePoint >= 0xFF21 && codePoint <= 0xFF3A)
    {
        value = codePoint - 0xFF21 + 10;
    }
    else if (generalCategory[codePoint] == "Nd")
    {
        value = decimalDigitValue[codePoint];
    }
    else
    {
        value = -1;
    }

    return value >= 0 ? value : -1;
}

static int GetNumericValue(int codePoint, long[] numericValue)
{
    if (codePoint >= 'a' && codePoint <= 'z')
    {
        return codePoint - 'a' + 10;
    }
    if (codePoint >= 'A' && codePoint <= 'Z')
    {
        return codePoint - 'A' + 10;
    }
    if (codePoint >= 0xFF41 && codePoint <= 0xFF5A)
    {
        return codePoint - 0xFF41 + 10;
    }
    if (codePoint >= 0xFF21 && codePoint <= 0xFF3A)
    {
        return codePoint - 0xFF21 + 10;
    }
    return (int)numericValue[codePoint];
}

// Java whitespace: the Unicode space separators except the non-breaking ones,
// plus the ASCII control characters Java treats as whitespace.
static bool IsWhiteSpace(int codePoint, string?[] generalCategory)
{
    if (codePoint == 0x00A0 || codePoint == 0x2007 || codePoint == 0x202F)
    {
        return false;
    }

    string? category = generalCategory[codePoint];
    if (category is "Zs" or "Zl" or "Zp")
    {
        return true;
    }

    return (codePoint >= 0x09 && codePoint <= 0x0D)
        || (codePoint >= 0x1C && codePoint <= 0x1F);
}

static long ParseNumericValue(string field)
{
    // Fractions (for example "1/2") and values outside the range of Int32
    // cannot be represented, which both Java and ICU report as -2.
    if (field.IndexOf('/') >= 0)
    {
        return Ucd.NumericNotRepresentable;
    }

    if (!long.TryParse(field, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long value))
    {
        return Ucd.NumericNotRepresentable;
    }

    return value >= 0 && value <= int.MaxValue ? value : Ucd.NumericNotRepresentable;
}

static IEnumerable<string> EnumerateLines(string text)
{
    using var reader = new StringReader(text);
    while (reader.ReadLine() is string line)
    {
        yield return line;
    }
}

internal static class Ucd
{
    public const int MaxCodePoint = 0x10FFFF;
    public const long NumericNone = -1;
    public const long NumericNotRepresentable = -2;
}
