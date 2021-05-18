using System;
using System.Globalization;

namespace J2N.Globalization
{
    /// <summary>
    /// Determines the styles permitted in numeric string arguments that are passed to
    /// the Parse and TryParse methods of the integral and floating-point numeric types
    /// in the <see cref="J2N.Numerics"/> namespace.
    /// <para/>
    /// Usage Note: This enum is compatible in both symbol and value to <see cref="NumberStyles"/>.
    /// To convert from <see cref="NumberStyles"/> to <see cref="NumberStyle"/>, simply
    /// do a cast.
    /// <code>
    /// NumberStyles myStyle = NumberStyles.Float | NumberStyles.AllowParentheses;<br/>
    /// NumberStyle myStyleConverted = (NumberStyle)myStyle;
    /// </code>
    /// You may cast the other way, as well.
    /// <code>
    /// NumberStyle myStyle = NumberStyle.Float | NumberStyle.AllowParentheses;<br/>
    /// NumberStyles myStyleConverted = (NumberStyles)myStyle;
    /// </code>
    /// However, the stock .NET parser will ignore additional symbols that were added to this enum when parsing.
    /// </summary>
    [Flags]
    public enum NumberStyle
    {
        /// <summary>
        /// Indicates that no style elements, such as leading or trailing white space, thousands
        /// separators, or a decimal separator, can be present in the parsed string. The
        /// string to be parsed must consist of integral decimal digits only.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that leading white-space characters can be present in the parsed string.
        /// Valid white-space characters have the Unicode values U+0009, U+000A, U+000B,
        /// U+000C, U+000D, and U+0020. Note that this is a subset of the characters for
        /// which the <see cref="char.IsWhiteSpace(char)"/> method returns <c>true</c>.
        /// </summary>
        AllowLeadingWhite = 1,

        /// Indicates that trailing white-space characters can be present in the parsed string.
        /// Valid white-space characters have the Unicode values U+0009, U+000A, U+000B,
        /// U+000C, U+000D, and U+0020. Note that this is a subset of the characters for
        /// which the <see cref="char.IsWhiteSpace(char)"/> method returns <c>true</c>.
        /// </summary>
        AllowTrailingWhite = 2,

        /// <summary>
        /// Indicates that the numeric string can have a leading sign. Valid leading sign
        /// characters are determined by the <see cref="NumberFormatInfo.PositiveSign"/>
        /// and <see cref="NumberFormatInfo.NegativeSign"/> properties.
        /// </summary>
        AllowLeadingSign = 4,

        /// <summary>
        /// Indicates that the <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// and <see cref="AllowLeadingSign"/> styles are used. This
        /// is a composite number style.
        /// </summary>
        Integer = 7,

        /// <summary>
        /// Indicates that the numeric string can have a trailing sign. Valid trailing sign
        /// characters are determined by the <see cref="NumberFormatInfo.PositiveSign"/>
        /// and <see cref="NumberFormatInfo.NegativeSign"/>.
        /// </summary>
        AllowTrailingSign = 8,

        /// <summary>
        /// Indicates that the numeric string can have one pair of parentheses enclosing
        /// the number. The parentheses indicate that the string to be parsed represents
        /// a negative number.
        /// </summary>
        AllowParentheses = 16,

        /// <summary>
        /// Indicates that the numeric string can have a decimal point. If the <see cref="NumberStyle"/>
        /// value includes the <see cref="AllowCurrencySymbol"/> flag
        /// and the parsed string includes a currency symbol, the decimal separator character
        /// is determined by the <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>
        /// property. Otherwise, the decimal separator character is determined by the <see cref="NumberFormatInfo.NumberDecimalSeparator"/>
        /// property.
        /// </summary>
        AllowDecimalPoint = 32,

        /// <summary>
        /// Indicates that the numeric string can have group separators, such as symbols
        /// that separate hundreds from thousands. If the <see cref="NumberStyle"/>
        /// value includes the <see cref="AllowCurrencySymbol"/> flag
        /// and the string to be parsed includes a currency symbol, the valid group separator
        /// character is determined by the <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>
        /// property. Otherwise, the valid group separator character is determined by the
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/> property, and the
        /// number of digits in each group is determined by the <see cref="NumberFormatInfo.NumberGroupSizes"/>
        /// property.
        /// </summary>
        AllowThousands = 64,

        /// <summary>
        /// Indicates that the <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// <see cref="AllowLeadingSign"/>, <see cref="AllowTrailingSign"/>,
        /// <see cref="AllowDecimalPoint"/>, and <see cref="AllowThousands"/>
        /// styles are used. This is a composite number style.
        /// </summary>
        Number = 111,

        /// <summary>
        /// Indicates that the numeric string can be in exponential notation. The <see cref="AllowExponent"/>
        /// flag allows the parsed string to contain an exponent that begins with the "E"
        /// or "e" character and that is followed by an optional positive or negative sign
        /// and an integer. In other words, it successfully parses strings in the form <i>nnn</i>E<i>xx</i>,
        /// <i>nnn</i>E+<i>xx</i> and <i>nnn</i>E-<i>xx</i>. It does not allow a decimal separator or sign in the significand
        /// or mantissa; to allow these elements in the string to be parsed, use the <see cref="AllowDecimalPoint"/>
        /// style that includes these individual flags.
        /// </summary>
        AllowExponent = 128,

        /// <summary>
        /// Indicates that the <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// <see cref="AllowLeadingSign"/>, <see cref="AllowDecimalPoint"/>,
        /// and <see cref="AllowExponent"/> styles are used. This is
        /// a composite number style.
        /// </summary>
        Float = 167,

        /// <summary>
        /// Indicates that the numeric string can contain a currency symbol. Valid currency
        /// symbols are determined by the <see cref="NumberFormatInfo.CurrencySymbol"/>
        /// property.
        /// </summary>
        AllowCurrencySymbol = 256,

        /// <summary>
        /// Indicates that all styles except <see cref="AllowExponent"/>
        /// and <see cref="AllowHexSpecifier"/> are used. This is a composite
        /// number style.
        /// </summary>
        Currency = 383,

        /// <summary>
        /// Indicates that all styles except <see cref="AllowHexSpecifier"/>
        /// are used. This is a composite number style.
        /// </summary>
        Any = 511,

        /// <summary>
        /// Indicates that the numeric string represents a hexadecimal value. Valid hexadecimal
        /// values include the numeric digits 0-9 and the hexadecimal digits A-F and a-f.
        /// Strings that are parsed using this style cannot be prefixed with "0x" or "&amp;h".
        /// A string that is parsed with the <see cref="AllowHexSpecifier"/>
        /// style will always be interpreted as a hexadecimal value. The only flags that
        /// can be combined with <see cref="AllowHexSpecifier"/> are
        /// <see cref="AllowLeadingWhite"/> and <see cref="AllowTrailingWhite"/>.
        /// The <see cref="NumberStyle"/> enumeration includes a composite style,
        /// <see cref="HexNumber"/>, that consists of these three flags.
        /// </summary>
        AllowHexSpecifier = 512,

        /// <summary>
        /// Indicates that <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// and <see cref="AllowHexSpecifier"/> styles are used. This
        /// is a composite number style.
        /// </summary>
        HexNumber = 515,

        // J2N specific flags

        /// <summary>
        /// Indicates that the numeric string represents a floating point number expressed as a
        /// C# or Java literal string, such as <c>3.14159f</c> or <c>4.972135238332232d</c>.
        /// Note the type is simply ignored during the parse when you specify this option (which is how it works in Java).
        /// However, performance is better in .NET 5+ and .NET Core 3+ if you don't choose this option,
        /// so it is recommended only to use it if you have strings like this to parse.
        /// </summary>
        AllowTrailingFloatType = 1024,
    }
}