# GenerateUnicodeData

Generates `tests/NUnit/J2N.Tests/Unicode/UnicodeCharacterData.txt`, the data file that
`J2N.Tests` embeds and uses as the expected-value oracle for:

- `Character.Digit(int, int)`
- `Character.GetNumericValue(int)`
- `Character.IsWhiteSpace(int)`

## Why this exists

These tests previously compared `J2N.Character` against `ICU4N.UChar`. That meant every test
run had to restore and load ICU4N, and it pinned the Unicode version J2N could be validated
against to whichever version a given ICU4N release shipped. The test project was also stuck on
an old ICU4N version, because newer ones take a dependency on J2N itself, which risks the code
under test being shadowed by the copy ICU4N pulls in.

This tool reads the [Unicode Character Database](https://www.unicode.org/Public/) directly, so
the expected values come from the standard rather than from another implementation of it, and
moving to a newer Unicode version is a matter of re-running the tool with a new version number.

## Usage

```sh
dotnet run --project tools/GenerateUnicodeData -- <unicodeVersion> [outputFile]
```

For example, to regenerate the current data file:

```sh
dotnet run --project tools/GenerateUnicodeData -- 10.0.0
```

The version defaults to `10.0.0` (matching the Unicode version implemented by
`J2N.Character`) and the output path defaults to the embedded resource in `J2N.Tests`.

Bumping the Unicode version of the generated data will cause the `*_Against_UnicodeData` tests
to fail wherever `J2N.Character`'s tables have not also been updated to that version; those
failures are the list of work required to move J2N to the newer version.

## Inputs

Both files are downloaded from `https://www.unicode.org/Public/<version>/ucd`:

| File | Provides |
| --- | --- |
| `UnicodeData.txt` | General category, decimal digit value, and numeric value for most code points |
| `Unihan.zip` (`Unihan_NumericValues.txt`) | Numeric values for CJK ideographs, which `UnicodeData.txt` does not carry |

## Output format

A run-length encoded, `;`-delimited text file whose rows tile `U+0000..U+10FFFF` with no gaps:

```
startCodePoint;endCodePoint;digitValue;numericValue;isWhiteSpace
```

Code points are hexadecimal. `digitValue` and `numericValue` use `-1` for "no value" and `-2`
for "value cannot be represented as an `Int32`" (for example a fraction such as `1/2`).
