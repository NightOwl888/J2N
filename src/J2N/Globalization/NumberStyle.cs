// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Globalization;

namespace J2N.Globalization
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Determines the styles permitted in numeric string arguments that are passed to
    /// the Parse and TryParse methods of the integral and floating-point numeric types
    /// in the <see cref="J2N.Numerics"/> namespace.
    /// <para/>
    /// Usage Note: This enum is compatible in both symbol and value to <see cref="NumberStyles"/>,
    /// however it is not recommended to cast due to strict flag validation.
    /// To convert from <see cref="NumberStyles"/> to <see cref="NumberStyle"/>, call the
    /// <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
    /// <code>
    /// NumberStyles myStyle = NumberStyles.Float | NumberStyles.AllowParentheses;<br/>
    /// NumberStyle myStyleConverted = myStyle.ToNumberStyle();
    /// </code>
    /// You may convert the other way, as well.
    /// <code>
    /// NumberStyle myStyle = NumberStyle.Float | NumberStyle.AllowParentheses;<br/>
    /// NumberStyles myStyleConverted = myStyle.ToNumberStyles();
    /// </code>
    /// </summary>
    [Flags]
    public enum NumberStyle
    {
        /// <summary>
        /// Indicates that no style elements, such as leading or trailing white space, thousands
        /// separators, or a decimal separator, can be present in the parsed string. The
        /// string to be parsed must consist of integral decimal digits only.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Indicates that leading white-space characters can be present in the parsed string.
        /// Valid white-space characters have the Unicode values U+0009, U+000A, U+000B,
        /// U+000C, U+000D, and U+0020. Note that this is a subset of the characters for
        /// which the <see cref="char.IsWhiteSpace(char)"/> method returns <c>true</c>.
        /// </summary>
        AllowLeadingWhite = 0x00000001,

        /// <summary>
        /// Indicates that trailing white-space characters can be present in the parsed string.
        /// Valid white-space characters have the Unicode values U+0009, U+000A, U+000B,
        /// U+000C, U+000D, and U+0020. Note that this is a subset of the characters for
        /// which the <see cref="char.IsWhiteSpace(char)"/> method returns <c>true</c>.
        /// </summary>
        AllowTrailingWhite = 0x00000002,

        /// <summary>
        /// Indicates that the numeric string can have a leading sign. Valid leading sign
        /// characters are determined by the <see cref="NumberFormatInfo.PositiveSign"/>
        /// and <see cref="NumberFormatInfo.NegativeSign"/> properties.
        /// </summary>
        AllowLeadingSign = 0x00000004,

        /// <summary>
        /// Indicates that the numeric string can have a trailing sign. Valid trailing sign
        /// characters are determined by the <see cref="NumberFormatInfo.PositiveSign"/>
        /// and <see cref="NumberFormatInfo.NegativeSign"/>.
        /// </summary>
        AllowTrailingSign = 0x00000008,

        /// <summary>
        /// Indicates that the numeric string can have one pair of parentheses enclosing
        /// the number. The parentheses indicate that the string to be parsed represents
        /// a negative number.
        /// </summary>
        AllowParentheses = 0x00000010,

        /// <summary>
        /// Indicates that the numeric string can have a decimal point. If the <see cref="NumberStyle"/>
        /// value includes the <see cref="AllowCurrencySymbol"/> flag
        /// and the parsed string includes a currency symbol, the decimal separator character
        /// is determined by the <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>
        /// property. Otherwise, the decimal separator character is determined by the <see cref="NumberFormatInfo.NumberDecimalSeparator"/>
        /// property.
        /// </summary>
        AllowDecimalPoint = 0x00000020,

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
        AllowThousands = 0x00000040,

        /// <summary>
        /// Indicates that the numeric string can be in exponential notation. The <see cref="AllowExponent"/>
        /// flag allows the parsed string to contain an exponent that begins with the "E"
        /// or "e" character and that is followed by an optional positive or negative sign
        /// and an integer. In other words, it successfully parses strings in the form <i>nnn</i>E<i>xx</i>,
        /// <i>nnn</i>E+<i>xx</i> and <i>nnn</i>E-<i>xx</i>. It does not allow a decimal separator or sign in the significand
        /// or mantissa; to allow these elements in the string to be parsed, use the <see cref="AllowDecimalPoint"/>
        /// style that includes these individual flags.
        /// </summary>
        AllowExponent = 0x00000080,

        /// <summary>
        /// Indicates that the numeric string can contain a currency symbol. Valid currency
        /// symbols are determined by the <see cref="NumberFormatInfo.CurrencySymbol"/>
        /// property.
        /// </summary>
        AllowCurrencySymbol = 0x00000100,

        /// <summary>
        /// Indicates that the numeric string represents a hexadecimal value. Valid hexadecimal
        /// values include the numeric digits 0-9 and the hexadecimal digits A-F and a-f.
        /// Strings that are parsed using this style can only be prefixed with "0x" or "&amp;h"
        /// when parsing floating point hexadecimal types.
        /// A string that is parsed with the <see cref="AllowHexSpecifier"/>
        /// style will always be interpreted as a hexadecimal value.
        /// The <see cref="NumberStyle"/> enumeration includes 2 composite styles,
        /// <see cref="HexNumber"/> and <see cref="HexFloat"/>, that can be used to
        /// specifiy either hexadecimal integral or floating-point types along with
        /// a default set of options that are used in most cases.
        /// </summary>
        AllowHexSpecifier = 0x00000200,


        /// <summary>
        /// Indicates that the numeric string represents a number expressed as a
        /// C# or Java literal string, such as <c>3.14159f</c> or <c>4.972135238332232d</c>.
        /// This option only applies to floating point numbers <see cref="float"/> and <see cref="double"/>, however,
        /// the real type suffix 'd', 'D', 'f', 'F', 'm' or 'M' are all valid for each floating point type.
        /// If this option is used with <see cref="AllowHexSpecifier"/>, then 'd', 'D', 'f', or 'F' are all treated as
        /// hexadecimal values unless an exponent (beginning with 'p') is also supplied (i.e. <c>0x1.8p1f</c>) so it must be
        /// used in conjunction with <see cref="AllowExponent"/> for hexadecimal values.
        /// Note the type is simply ignored during the parse when you specify this option (which is how it works in Java).
        /// </summary>
        AllowTypeSpecifier = 0x00000400, // J2N specific


        /// <summary>
        /// Indicates that the <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// and <see cref="AllowLeadingSign"/> styles are used. This
        /// is a composite number style.
        /// </summary>
        Integer = AllowLeadingWhite | AllowTrailingWhite | AllowLeadingSign,

        /// <summary>
        /// Indicates that <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// and <see cref="AllowHexSpecifier"/> styles are used. This
        /// is a composite number style.
        /// </summary>
        HexNumber = AllowLeadingWhite | AllowTrailingWhite | AllowHexSpecifier,

        /// <summary>
        /// Indicates that the <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// <see cref="AllowLeadingSign"/>, <see cref="AllowTrailingSign"/>,
        /// <see cref="AllowDecimalPoint"/>, and <see cref="AllowThousands"/>
        /// styles are used. This is a composite number style.
        /// </summary>
        Number = AllowLeadingWhite | AllowTrailingWhite | AllowLeadingSign | AllowTrailingSign |
                   AllowDecimalPoint | AllowThousands,

        /// <summary>
        /// Indicates that the <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// <see cref="AllowLeadingSign"/>, <see cref="AllowDecimalPoint"/>,
        /// and <see cref="AllowExponent"/> styles are used. This is
        /// a composite number style.
        /// </summary>
        Float = AllowLeadingWhite | AllowTrailingWhite | AllowLeadingSign |
                   AllowDecimalPoint | AllowExponent,

        /// <summary>
        /// Indicates that the <see cref="AllowLeadingWhite"/>, <see cref="AllowTrailingWhite"/>,
        /// <see cref="AllowLeadingSign"/>, <see cref="AllowDecimalPoint"/>,
        /// <see cref="AllowExponent"/>, and <see cref="AllowHexSpecifier"/> styles are used. This is
        /// a composite number style.
        /// </summary>
        HexFloat = AllowLeadingWhite | AllowTrailingWhite | AllowLeadingSign |
                   AllowDecimalPoint | AllowExponent | AllowHexSpecifier, // J2N specific

        /// <summary>
        /// Indicates that all styles except <see cref="AllowExponent"/>
        /// <see cref="AllowHexSpecifier"/> and <see cref="AllowTypeSpecifier"/> are used. This is a composite
        /// number style.
        /// </summary>
        Currency = AllowLeadingWhite | AllowTrailingWhite | AllowLeadingSign | AllowTrailingSign |
                  AllowParentheses | AllowDecimalPoint | AllowThousands | AllowCurrencySymbol,

        /// <summary>
        /// Indicates that all styles except <see cref="AllowHexSpecifier"/>
        /// and <see cref="AllowTypeSpecifier"/>are used. This is a composite number style.
        /// </summary>
        Any = AllowLeadingWhite | AllowTrailingWhite | AllowLeadingSign | AllowTrailingSign |
                   AllowParentheses | AllowDecimalPoint | AllowThousands | AllowCurrencySymbol | AllowExponent,
    }

    /// <summary>
    /// Extensions to <see cref="NumberStyle"/> and <see cref="NumberStyles"/>.
    /// </summary>
    public static class NumberStyleExtensions
    {
        /// <summary>
        /// The <see cref="NumberStyle"/> flags that are supported in <see cref="NumberStyles"/> (in .NET).
        /// <para/>
        /// This list should be kept up to date with changes in the .NET platform, but serves as a safety
        /// net to ensure no new flags are considered until we have manually included them.
        /// </summary>
        private const NumberStyle ValidNumberStyles = (NumberStyle.AllowLeadingWhite | NumberStyle.AllowTrailingWhite
                                                        | NumberStyle.AllowLeadingSign | NumberStyle.AllowTrailingSign
                                                        | NumberStyle.AllowParentheses | NumberStyle.AllowDecimalPoint
                                                        | NumberStyle.AllowThousands | NumberStyle.AllowExponent
                                                        | NumberStyle.AllowCurrencySymbol | NumberStyle.AllowHexSpecifier);

        /// <summary>
        /// The <see cref="NumberStyle"/> flags that are supported in J2N.
        /// </summary>
        private const NumberStyle ValidNumberStyle =    (NumberStyle.AllowLeadingWhite | NumberStyle.AllowTrailingWhite
                                                        | NumberStyle.AllowLeadingSign | NumberStyle.AllowTrailingSign
                                                        | NumberStyle.AllowParentheses | NumberStyle.AllowDecimalPoint
                                                        | NumberStyle.AllowThousands | NumberStyle.AllowExponent
                                                        | NumberStyle.AllowCurrencySymbol | NumberStyle.AllowHexSpecifier
                                                        | NumberStyle.AllowTypeSpecifier);

        /// <summary>
        /// Converts a <see cref="NumberStyle"/> to <see cref="NumberStyles"/> and removes all of the
        /// flags that are not valid on the .NET platform.
        /// </summary>
        /// <param name="style">The flags to convert.</param>
        /// <returns>The flags in <paramref name="style"/> that are supported on the .NET platform.</returns>
        public static NumberStyles ToNumberStyles(this NumberStyle style)
        {
            return (NumberStyles)(style & ValidNumberStyles);
        }

        /// <summary>
        /// Converts a <see cref="NumberStyles"/> to <see cref="NumberStyle"/> and removes all of the
        /// flags that are not valid in J2N.
        /// </summary>
        /// <param name="style">The flags to convert.</param>
        /// <returns>The flags in <paramref name="style"/> that are supported in J2N.</returns>
        public static NumberStyle ToNumberStyle(this NumberStyles style)
        {
            return (NumberStyle)style & ValidNumberStyles;
        }

        internal static void ValidateParseStyleInteger(NumberStyle style)
        {
            // Check for undefined flags or invalid hex number flags.
            // Since we are cascading the call to .NET in this case, we must not allow any custom J2N flags here.
            if ((style & (~ValidNumberStyles | NumberStyle.AllowHexSpecifier)) != 0
                && (style & ~NumberStyle.HexNumber) != 0)
            {
                throwInvalid(style);

                void throwInvalid(NumberStyle value)
                {
                    if ((value & ~ValidNumberStyles) != 0)
                    {
                        throw new ArgumentException(J2N.SR.Format(SR.Argument_InvalidNumberStyle, value & ~ValidNumberStyles), nameof(style));
                    }

                    throw new ArgumentException(J2N.SR.Format(SR.Arg_InvalidHexStyle, value & ~NumberStyle.HexNumber), nameof(style));
                }
            }
        }

        internal static void ValidateParseStyleFloatingPoint(NumberStyle style)
        {
            // Check for undefined flags or hex number
            if ((style & (~ValidNumberStyle | NumberStyle.AllowHexSpecifier)) != 0
                && ((style & ~(NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier)) != 0

                // Make sure if this is a hex number and AllowTypeSpecifier is enabled, that AllowExponent is also enabled
                || (style & NumberStyle.AllowHexSpecifier) != 0 && (style & NumberStyle.AllowTypeSpecifier) != 0 && (style & NumberStyle.AllowExponent) == 0)
                
                // Make sure if this is any number that the AllowCurrencySymbol and AllowTypeSpecifier are not both enabled
                || (style & NumberStyle.AllowTypeSpecifier) != 0 && (style & NumberStyle.AllowCurrencySymbol) != 0)
            {
                throwInvalid(style);

                void throwInvalid(NumberStyle value)
                {
                    if ((value & NumberStyle.AllowHexSpecifier) != 0 && (value & NumberStyle.AllowTypeSpecifier) != 0 && (value & NumberStyle.AllowExponent) == 0)
                    {
                        throw new ArgumentException(SR.Arg_ExponentRequiredIfTypeSpecifierUsed, nameof(style));
                    }

                    if ((value & ~ValidNumberStyle) != 0)
                    {
                        throw new ArgumentException(J2N.SR.Format(SR.Argument_InvalidNumberStyle, value & ~ValidNumberStyle), nameof(style));
                    }

                    if ((value & NumberStyle.AllowTypeSpecifier) != 0 && (value & NumberStyle.AllowCurrencySymbol) != 0)
                    {
                        throw new ArgumentException(SR.Arg_TypeSpecifierNotAllowedIfCurrencyUsed, nameof(style));
                    }

                    throw new ArgumentException(J2N.SR.Format(SR.Arg_InvalidHexFloatStyle, value & ~(NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier)), nameof(style));
                }
            }
        }
    }
}