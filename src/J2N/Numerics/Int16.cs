﻿using J2N.Globalization;
using J2N.Text;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Numerics
{
    using SR = J2N.Resources.Strings;

    /// <inheritdoc/>
    public sealed class Int16 : Number, IComparable<Int16>, IComparable, IConvertible, IEquatable<Int16>
    {
        /// <summary>
        /// The value which the receiver represents.
        /// </summary>
        private readonly short value;

        /// <summary>
        /// Constant for the number of bits needed to represent a <see cref="short"/> in
        /// two's complement form.
        /// </summary>
        public const int Size = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="Int16"/> class.
        /// </summary>
        /// <param name="value">The primitive <see cref="short"/> value to store in the new instance.</param>
        internal Int16(short value) // J2N: This has been marked deprecated in JDK 16, so we are marking it internal
        {
            this.value = value;
        }

        // J2N: Removed other overloads because all of the constructors are deprecated in JDK 16

        #region CompareTo

        /// <summary>
        /// Compares this instance to a specified <see cref="Int16"/> and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An <see cref="Int16"/> to compare, or <c>null</c>.</param>
        /// <returns>
        /// A signed integer that indicates the relative order of this instance and <paramref name="value"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term>Description </term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <term>This instance is less than <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <term>This instance is equal to <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <term>This instance is greater than <paramref name="value"/>, or <paramref name="value"/> is <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This method implements the <see cref="IComparable{T}"/> interface and performs slightly better than the <see cref="CompareTo(object?)"/>
        /// method because it does not have to convert the <paramref name="value"/> parameter to an object.
        /// </remarks>
        public int CompareTo(Int16? value)
        {
            if (value is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            return this.value - value.value;
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An object to compare, or <c>null</c>.</param>
        /// <returns>
        /// A signed integer that indicates the relative order of this instance and <paramref name="value"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term>Description </term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <term>This instance is less than <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <term>This instance is equal to <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <term>This instance is greater than <paramref name="value"/>, or <paramref name="value"/> is <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a <see cref="Int16"/>.</exception>
        /// <remarks>
        /// <paramref name="value"/> must be <c>null</c> or an instance of <see cref="Int16"/>; otherwise, an exception is thrown.
        /// <para/>
        /// Any instance of <see cref="Int16"/>, regardless of its value, is considered greater than <c>null</c>.
        /// <para/>
        /// This method is implemented to support the <see cref="IComparable"/> interface.
        /// </remarks>
        public int CompareTo(object? value)
        {
            if (value is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            if (!(value is Int16 other))
                throw new ArgumentException(SR.Arg_MustBeInt16);
            return this.value - other.value;
        }

        #endregion CompareTo

        #region Decode

        /// <summary>
        /// Decodes a <see cref="string"/> into an <see cref="Int16"/>. Accepts decimal, hexadecimal, and octal numbers given by the following grammar:
        /// <list type="bullet">
        ///     <item>
        ///         <term><i>DecodableString:</i></term>
        ///         <list type="bullet">
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <i>DecimalNumeral</i></term></item>
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <c>0x</c> <i>HexDigits</i></term></item>
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <c>0X</c> <i>HexDigits</i></term></item>
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <c>#</c> <i>HexDigits</i></term></item>
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <c>0</c> <i>OctalDigits</i></term></item>
        ///         </list>
        ///     </item>
        ///     <item>
        ///         <term><i>Sign:</i></term>
        ///         <list type="bullet">
        ///             <item><term><c>-</c></term></item>
        ///             <item><term><c>+</c></term></item>
        ///         </list>
        ///     </item>
        /// </list>
        /// <para/>
        /// <i>DecimalNumeral</i>, <i>HexDigits</i>, and <i>OctalDigits</i> are as defined in section
        /// <a href="https://docs.oracle.com/javase/specs/jls/se16/html/jls-3.html#jls-3.10.1">3.10.1</a> of
        /// The Java Language Specification, except that underscores are not accepted between digits.
        /// <para/>
        /// The sequence of characters following an optional sign and/or radix specifier (<c>"0x"</c>, <c>"0X"</c>, <c>"#"</c>, or leading zero) is
        /// parsed as by the <see cref="Parse(string?, int)"/> method with the indicated radix (10, 16, or 8).
        /// The sequence of characters must represent a positive value or an <see cref="OverflowException"/> is thrown.
        /// The result is negated if the first character of the specified <see cref="string"/> is
        /// the ASCII character \u002d ('-'). No whitespace characters are permitted in the <see cref="string"/>.
        /// </summary>
        /// <param name="s">A <see cref="string"/> that contains the number to convert.</param>
        /// <returns>A 16-bit signed integer that is equivalent to the number in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> contains a character that is not a valid digit in the base specified by its format.
        /// The exception message indicates that there are no digits to convert if the first character in <paramref name="s"/> is invalid;
        /// otherwise, the message indicates that <paramref name="s"/> contains invalid trailing characters.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> contains only a the ASCII character \u002d ('-') or \u002B ('+') sign and/or hexadecimal
        /// prefix (0X, 0x, or #) or octal prefix (0) with no digits.
        /// </exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number that is less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> represents a two's complement negative number. Only positive values are allowed without a negative sign.
        /// </exception>
        public static Int16 Decode(string s)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));

            int length = s.Length, i = 0;
            if (length == 0)
            {
                throw new FormatException(SR.Format_EmptyInputString);
            }
            char firstDigit = s[i];
            int sign = firstDigit == '-' ? -1 : 1;
            if (firstDigit == '-' || firstDigit == '+')
            {
                if (length == 1)
                {
                    throw new FormatException(J2N.SR.Format(SR.Format_InvalidString, s));
                }
                firstDigit = s[++i];
            }

            int @base = 10;
            if (firstDigit == '0')
            {
                if (++i == length)
                {
                    return ValueOf(0);
                }
                if ((firstDigit = s[i]) == 'x' || firstDigit == 'X')
                {
                    if (++i == length)
                    {
                        throw new FormatException(J2N.SR.Format(SR.Format_InvalidString, s));
                    }
                    @base = 16;
                }
                else
                {
                    @base = 8;
                }
            }
            else if (firstDigit == '#')
            {
                if (++i == length)
                {
                    throw new FormatException(J2N.SR.Format(SR.Format_InvalidString, s));
                }
                @base = 16;
            }

            // Special case: since StringToInt also checks for + or - at position i, we need to ensure the string passed doesn't include it.
            if (s[i] == '-' || s[i] == '+')
                throw new FormatException(J2N.SR.Format(SR.Format_InvalidString, s));

            int r = ParseNumbers.StringToInt(s, @base, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign, ref i, s.Length - i);

            if (r < short.MinValue || r > short.MaxValue)
                throw new OverflowException(SR.Overflow_Int16);
            // Only allow negative if it was passed as a sign in the string
            if (r < 0 && sign > 0)
                throw new OverflowException(SR.Overflow_Int16);
            return ValueOf((short)r);
        }

        #endregion Decode

        #region TryDecode

        /// <summary>
        /// Decodes a <see cref="string"/> into an <see cref="Int16"/>. Accepts decimal, hexadecimal, and octal numbers given by the following grammar:
        /// <list type="bullet">
        ///     <item>
        ///         <term><i>DecodableString:</i></term>
        ///         <list type="bullet">
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <i>DecimalNumeral</i></term></item>
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <c>0x</c> <i>HexDigits</i></term></item>
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <c>0X</c> <i>HexDigits</i></term></item>
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <c>#</c> <i>HexDigits</i></term></item>
        ///             <item><term><i>Sign</i> (<sub>opt</sub>) <c>0</c> <i>OctalDigits</i></term></item>
        ///         </list>
        ///     </item>
        ///     <item>
        ///         <term><i>Sign:</i></term>
        ///         <list type="bullet">
        ///             <item><term><c>-</c></term></item>
        ///             <item><term><c>+</c></term></item>
        ///         </list>
        ///     </item>
        /// </list>
        /// <para/>
        /// <i>DecimalNumeral</i>, <i>HexDigits</i>, and <i>OctalDigits</i> are as defined in section
        /// <a href="https://docs.oracle.com/javase/specs/jls/se16/html/jls-3.html#jls-3.10.1">3.10.1</a> of
        /// The Java Language Specification, except that underscores are not accepted between digits.
        /// <para/>
        /// The sequence of characters following an optional sign and/or radix specifier (<c>"0x"</c>, <c>"0X"</c>, <c>"#"</c>, or leading zero) is
        /// parsed as by the <see cref="Parse(string?, int)"/> method with the indicated radix (10, 16, or 8).
        /// The sequence of characters must represent a positive value.
        /// The result is negated if the first character of the specified <see cref="string"/> is
        /// the ASCII character \u002d ('-'). No whitespace characters are permitted in the <see cref="string"/>.
        /// </summary>
        /// <param name="s">A <see cref="string"/> that contains the number to convert.</param>
        /// <param name="result">When this method returns, contains the 16-bit signed integer value equivalent to the number contained in <paramref name="s"/>,
        /// if the conversion succeeded, or zero if the conversion failed. The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or
        /// <see cref="string.Empty"/>, is not of the correct format, or represents a number less than <see cref="short.MinValue"/> or greater than
        /// <see cref="short.MaxValue"/>. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        public static bool TryDecode(string s, [MaybeNullWhen(false)] out Int16 result)
        {
            result = default;

            if (s is null)
                return false;

            int length = s.Length, i = 0;
            if (length == 0)
            {
                return false;
            }
            char firstDigit = s[i];
            int sign = firstDigit == '-' ? -1 : 1;
            if (firstDigit == '-' || firstDigit == '+')
            {
                if (length == 1)
                {
                    return false;
                }
                firstDigit = s[++i];
            }

            int @base = 10;
            if (firstDigit == '0')
            {
                if (++i == length)
                {
                    result = ValueOf(0);
                    return true;
                }
                if ((firstDigit = s[i]) == 'x' || firstDigit == 'X')
                {
                    if (++i == length)
                    {
                        return false;
                    }
                    @base = 16;
                }
                else
                {
                    @base = 8;
                }
            }
            else if (firstDigit == '#')
            {
                if (++i == length)
                {
                    return false;
                }
                @base = 16;
            }

            // Special case: since StringToInt also checks for + or - at position i, we need to ensure the string passed doesn't include it.
            if (s[i] == '-' || s[i] == '+')
                return false;

            if (!ParseNumbers.TryStringToInt(s, @base, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign, ref i, s.Length - i, out int r) ||
                // Only allow negative if it was passed as a sign in the string
                (r < 0 && sign > 0) ||
                (r < short.MinValue || r > short.MaxValue))
            {
                return false;
            }

            result = ValueOf((short)r);
            return true;
        }

        #endregion TryDecode

        #region Equals

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Int16"/> value.
        /// </summary>
        /// <param name="obj">A <see cref="Int16"/> value to compare to this instance.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> has the same value as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>This method implements the <see cref="IEquatable{T}"/> interface, and performs slightly better than
        /// <see cref="Equals(object?)"/> because it does not have to convert the <paramref name="obj"/> parameter to an object.</remarks>
        public bool Equals(Int16? obj)
        {
            if (obj is null) return false;

            return ReferenceEquals(obj, this) || (value == obj.value);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is an instance of <see cref="Int16"/> and equals the value of
        /// this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return ReferenceEquals(obj, this) || (obj is Int16 other) && (value == other.value);
        }

        #endregion Equals

        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return value;
        }

        #endregion GetHashCode

        // Radix-based parsing (default in Java)

        #region Parse_CharSequence_Int32_Int32_Int32

#if FEATURE_READONLYSPAN

        /// <summary>
        /// Parses the <see cref="ReadOnlySpan{T}"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// </summary>
        /// <param name="s">The <see cref="ReadOnlySpan{T}"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> and <paramref name="length"/> refer to a location outside of <paramref name="s"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contains a character that is not a valid digit in the base specified by <paramref name="radix"/>.
        /// The exception message indicates that there are no digits to convert if the first character specified by
        /// <paramref name="startIndex"/> is invalid; otherwise, the message indicates that the range of characters in
        /// <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/> contains invalid trailing characters.
        /// <para/>
        /// -or-
        /// <para/>
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contain only a the ASCII character \u002d ('-') or \u002B ('+') sign and/or hexadecimal prefix 0X or 0x with no digits.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> is zero.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// represents a number that is less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(ReadOnlySpan{char}, int, int, int, out short)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static short Parse(ReadOnlySpan<char> s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            int r = ParseNumbers.StringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign: 1, ref startIndex, length);
            if (radix != 10 && r <= ushort.MaxValue)
                return (short)r;

            if (r < short.MinValue || r > short.MaxValue)
                throw new OverflowException(SR.Overflow_Int16);
            return (short)r;
        }

#endif

        /// <summary>
        /// Parses the <see cref="string"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> and <paramref name="length"/> refer to a location outside of <paramref name="s"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contains a character that is not a valid digit in the base specified by <paramref name="radix"/>.
        /// The exception message indicates that there are no digits to convert if the first character specified by
        /// <paramref name="startIndex"/> is invalid; otherwise, the message indicates that the range of characters in
        /// <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/> contains invalid trailing characters.
        /// <para/>
        /// -or-
        /// <para/>
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contain only a the ASCII character \u002d ('-') or \u002B ('+') sign and/or hexadecimal prefix 0X or 0x with no digits.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> is zero.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// represents a number that is less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(string, int, int, int, out short)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static short Parse(string s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            int r = ParseNumbers.StringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign: 1, ref startIndex, length);
            if (radix != 10 && r <= ushort.MaxValue)
                return (short)r;

            if (r < short.MinValue || r > short.MaxValue)
                throw new OverflowException(SR.Overflow_Int16);
            return (short)r;
        }

        /// <summary>
        /// Parses the <see cref="T:char[]"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// </summary>
        /// <param name="s">The <see cref="T:char[]"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> and <paramref name="length"/> refer to a location outside of <paramref name="s"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contains a character that is not a valid digit in the base specified by <paramref name="radix"/>.
        /// The exception message indicates that there are no digits to convert if the first character specified by
        /// <paramref name="startIndex"/> is invalid; otherwise, the message indicates that the range of characters in
        /// <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/> contains invalid trailing characters.
        /// <para/>
        /// -or-
        /// <para/>
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contain only a the ASCII character \u002d ('-') or \u002B ('+') sign and/or hexadecimal prefix 0X or 0x with no digits.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> is zero.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// represents a number that is less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(char[], int, int, int, out short)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static short Parse(char[] s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            int r = ParseNumbers.StringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign: 1, ref startIndex, length);
            if (radix != 10 && r <= ushort.MaxValue)
                return (short)r;

            if (r < short.MinValue || r > short.MaxValue)
                throw new OverflowException(SR.Overflow_Int16);
            return (short)r;
        }

        /// <summary>
        /// Parses the <see cref="StringBuilder"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// </summary>
        /// <param name="s">The <see cref="StringBuilder"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> and <paramref name="length"/> refer to a location outside of <paramref name="s"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contains a character that is not a valid digit in the base specified by <paramref name="radix"/>.
        /// The exception message indicates that there are no digits to convert if the first character specified by
        /// <paramref name="startIndex"/> is invalid; otherwise, the message indicates that the range of characters in
        /// <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/> contains invalid trailing characters.
        /// <para/>
        /// -or-
        /// <para/>
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contain only a the ASCII character \u002d ('-') or \u002B ('+') sign and/or hexadecimal prefix 0X or 0x with no digits.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> is zero.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// represents a number that is less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(StringBuilder, int, int, int, out short)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static short Parse(StringBuilder s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            int r = ParseNumbers.StringToInt(s.ToString(startIndex, length), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2);
            if (radix != 10 && r <= ushort.MaxValue)
                return (short)r;

            if (r < short.MinValue || r > short.MaxValue)
                throw new OverflowException(SR.Overflow_Int16);
            return (short)r;
        }

        /// <summary>
        /// Parses the <see cref="ICharSequence"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// </summary>
        /// <param name="s">The <see cref="ICharSequence"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c> or <see cref="ICharSequence.HasValue"/> returns <c>false</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> and <paramref name="length"/> refer to a location outside of <paramref name="s"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contains a character that is not a valid digit in the base specified by <paramref name="radix"/>.
        /// The exception message indicates that there are no digits to convert if the first character specified by
        /// <paramref name="startIndex"/> is invalid; otherwise, the message indicates that the range of characters in
        /// <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/> contains invalid trailing characters.
        /// <para/>
        /// -or-
        /// <para/>
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contain only a the ASCII character \u002d ('-') or \u002B ('+') sign and/or hexadecimal prefix 0X or 0x with no digits.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> is zero.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// represents a number that is less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(ICharSequence, int, int, int, out short)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static short Parse(ICharSequence s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (s is null || !s.HasValue)
                throw new ArgumentNullException(nameof(s));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            int r;
            if (s is StringBuilderCharSequence stringBuilderCharSequence)
                r = ParseNumbers.StringToInt(stringBuilderCharSequence.Value!.ToString(startIndex, length), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2);
            else if (s is StringBuffer stringBuffer)
                r = ParseNumbers.StringToInt(stringBuffer.ToString(startIndex, length), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2);
            else
                r = ParseNumbers.StringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign: 1, ref startIndex, length);

            if (radix != 10 && r <= ushort.MaxValue)
                return (short)r;

            if (r < short.MinValue || r > short.MaxValue)
                throw new OverflowException(SR.Overflow_Int16);
            return (short)r;
        }

        #endregion Parse_CharSequence_Int32_Int32_Int32

        #region TryParse_CharSequence_Int32_Int32_Int32_Int16

#if FEATURE_READONLYSPAN

        /// <summary>
        /// Parses the <see cref="ReadOnlySpan{Char}"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// <para/>
        /// Since <see cref="Parse(ReadOnlySpan{char}, int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="ReadOnlySpan{Char}"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(ReadOnlySpan<char> s, int startIndex, int length, int radix, out short result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (startIndex < 0)
                return false;
            if (length < 0)
                return false;
            if (startIndex > s.Length - length) // Checks for int overflow
                return false;

            if (ParseNumbers.TryStringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign: 1, ref startIndex, length, out int r))
            {
                if (radix != 10 && r <= ushort.MaxValue)
                {
                    result = (short)r;
                    return true;
                }

                if (r < short.MinValue || r > short.MaxValue)
                {
                    return false;
                }

                result = (short)r;
                return true;
            }

            return false;
        }

#endif

        /// <summary>
        /// Parses the <see cref="string"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// <para/>
        /// Since <see cref="Parse(string, int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(string, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(string s, int startIndex, int length, int radix, out short result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (s is null)
                return false;
            if (startIndex < 0)
                return false;
            if (length < 0)
                return false;
            if (startIndex > s.Length - length) // Checks for int overflow
                return false;

            if (ParseNumbers.TryStringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign: 1, ref startIndex, length, out int r))
            {
                if (radix != 10 && r <= ushort.MaxValue)
                {
                    result = (short)r;
                    return true;
                }

                if (r < short.MinValue || r > short.MaxValue)
                {
                    return false;
                }

                result = (short)r;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the <see cref="T:char[]"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// <para/>
        /// Since <see cref="Parse(char[], int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="T:char[]"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(char[], int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(char[] s, int startIndex, int length, int radix, out short result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (s is null)
                return false;
            if (startIndex < 0)
                return false;
            if (length < 0)
                return false;
            if (startIndex > s.Length - length) // Checks for int overflow
                return false;

            if (ParseNumbers.TryStringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign: 1, ref startIndex, length, out int r))
            {
                if (radix != 10 && r <= ushort.MaxValue)
                {
                    result = (short)r;
                    return true;
                }

                if (r < short.MinValue || r > short.MaxValue)
                {
                    return false;
                }

                result = (short)r;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the <see cref="StringBuilder"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// <para/>
        /// Since <see cref="Parse(StringBuilder, int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="StringBuilder"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(StringBuilder, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(StringBuilder s, int startIndex, int length, int radix, out short result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (s is null)
                return false;
            if (startIndex < 0)
                return false;
            if (length < 0)
                return false;
            if (startIndex > s.Length - length) // Checks for int overflow
                return false;

            if (ParseNumbers.TryStringToInt(s.ToString(startIndex, length), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, out int r))
            {
                if (radix != 10 && r <= ushort.MaxValue)
                {
                    result = (short)r;
                    return true;
                }

                if (r < short.MinValue || r > short.MaxValue)
                {
                    return false;
                }

                result = (short)r;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the <see cref="ICharSequence"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// <para/>
        /// Since <see cref="Parse(ICharSequence, int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="ICharSequence"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(ICharSequence, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(ICharSequence s, int startIndex, int length, int radix, out short result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (s is null || !s.HasValue)
                return false;
            if (startIndex < 0)
                return false;
            if (length < 0)
                return false;
            if (startIndex > s.Length - length) // Checks for int overflow
                return false;

            int r;
            if ((s is StringBuilderCharSequence stringBuilderCharSequence && ParseNumbers.TryStringToInt(stringBuilderCharSequence.Value!.ToString(startIndex, length), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, out r)) ||
                (s is StringBuffer stringBuffer && ParseNumbers.TryStringToInt(stringBuffer.ToString(startIndex, length), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, out r)) ||
                ParseNumbers.TryStringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI2, sign: 1, ref startIndex, length, out r))
            {
                if (radix != 10 && r <= ushort.MaxValue)
                {
                    result = (short)r;
                    return true;
                }

                if (r < short.MinValue || r > short.MaxValue)
                {
                    return false;
                }

                result = (short)r;
                return true;
            }

            return false;
        }

        #endregion TryParse_CharSequence_Int32_Int32_Int32_Int16

        #region Parse_CharSequence_Int32

        /// <summary>
        /// Parses the <see cref="string"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method. It differs in that
        /// it allows the use of the ASCII character \u002d ('-') or \u002B ('+') in any <paramref name="radix"/>.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>A 16-bit signed integer that is equivalent to the number in <paramref name="s"/>, or 0 (zero) if
        /// <paramref name="s"/> is <c>null</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of a long integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> contains a character that is not a valid digit in the base specified by <paramref name="radix"/>.
        /// The exception message indicates that there are no digits to convert if the first character in <paramref name="s"/> is invalid;
        /// otherwise, the message indicates that <paramref name="s"/> contains invalid trailing characters.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> contains only a the ASCII character \u002d ('-') or \u002B ('+') sign and/or hexadecimal
        /// prefix 0X or 0x with no digits.
        /// </exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number that is less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(string?, int, out short)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static short Parse(string? s, int radix) // J2N: Renamed from ParseShort()
        {
            if (s == null)
            {
                return 0;
            }

            int r = ParseNumbers.StringToInt(s, radix, ParseNumbers.IsTight | ParseNumbers.TreatAsI2);
            if (radix != 10 && r <= ushort.MaxValue)
                return (short)r;

            if (r < short.MinValue || r > short.MaxValue)
                throw new OverflowException(SR.Overflow_Int16);
            return (short)r;
        }

        #endregion Parse_CharSequence_Int32

        #region TryParse_CharSequence_Int32_Int16

        /// <summary>
        /// Parses the <see cref="string"/> argument as a signed <see cref="short"/> in the specified <paramref name="radix"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToInt16(string?, int)"/> method. It differs in that it
        /// allows the use of the ASCII character \u002d ('-') or \u002B ('+') in any <paramref name="radix"/> and allows any 
        /// <paramref name="radix"/> value from <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// However, supplementary digits must appear as surrogate pair UTF-16 characters (i.e. "\ud801\udca0") in
        /// <paramref name="s"/>. Supplementary digits expressed as single UTF-32 characters (i.e. "\U000104A0") are not supported.
        /// <para/>
        /// Since <see cref="Parse(string?, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="short"/> representation to be parsed.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The signed <see cref="short"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 15) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="short"/>
        /// data type is converted to a <see cref="short"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(string, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(string? s, int radix, out short result) // J2N: Renamed from ParseShort()
        {
            if (s == null)
            {
                result = 0;
                return true;
            }

            if (!ParseNumbers.TryStringToInt(s, radix, ParseNumbers.IsTight | ParseNumbers.TreatAsI2, out int r))
            {
                result = default;
                return false;
            }

            if (radix != 10 && r <= ushort.MaxValue)
            {
                result = (short)r;
                return true;
            }

            if (r < short.MinValue || r > short.MaxValue)
            {
                result = default;
                return false;
            }

            result = (short)r;
            return true;
        }

        #endregion TryParse_CharSequence_Int32_Int16

        // Culture-aware parsing (default in .NET)

        #region Parse_CharSequence_IFormatProvider

        /// <summary>
        /// Converts the string representation of a number in a specified culture-specific format to its
        /// 16-bit signed integer equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A 16-bit signed integer equivalent to the number specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> is not in a valid <see cref="NumberStyle.Integer"/> format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="s"/> parameter contains a number of the form:
        /// <para/>
        /// [ws][sign]digits[ws]
        /// <para/>
        /// Elements in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol or a positive sign symbol as defined by the <see cref="NumberFormatInfo"/> object returned from the
        ///         <see cref="IFormatProvider.GetFormat(Type?)"/> <paramref name="provider"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using the <see cref="NumberStyle.Integer"/> style. In addition to decimal digits, only
        /// leading and trailing spaces together with a leading sign are allowed in <paramref name="s"/>. To explicitly define the style elements
        /// together with the culture-specific formatting information that can be present in <paramref name="s"/>, use the
        /// <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method.
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// </remarks>
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/>
        /// <seealso cref="ValueOf(string, IFormatProvider?)"/>
        public static short Parse(string s, IFormatProvider? provider) // J2N: Renamed from ParseShort()
        {
            return short.Parse(s, provider);
        }

        #endregion Parse_CharSequence_IFormatProvider

        #region TryParse_CharSequence_Int16

        /// <summary>
        /// Converts the string representation of a number to its 16-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Integer class, so call <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="result">When this method returns, contains the 16-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(string?, out short)"/> method is like the <see cref="Parse(string, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(string?, out short)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="s"/> parameter contains a number of the form:
        /// <code>
        /// [ws][sign]digits[ws]
        /// </code>
        /// Items in square brackets ([ and ]) are optional. The following table describes each element.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>Optional white space.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>An optional sign.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A sequence of digits ranging from 0 to 9.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using the <see cref="NumberStyle.Integer"/> style.
        /// In addition to the decimal digits, only leading and trailing spaces together with a leading sign are
        /// allowed. To explicitly define the style elements together with the culture-specific formatting
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(string?, out short)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        /// <seealso cref="Number.ToString()"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse([NotNullWhen(true)] string? s, out short result)
        {
            return short.TryParse(s, out result);
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 16-bit signed
        /// integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Integer class, so call <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out short)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out short)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert.</param>
        /// <param name="result">When this method returns, contains the 16-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, out short)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, out short)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="s"/> parameter contains a number of the form:
        /// <code>
        /// [ws][sign]digits[ws]
        /// </code>
        /// Items in square brackets ([ and ]) are optional. The following table describes each element.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>Optional white space.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>An optional sign.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A sequence of digits ranging from 0 to 9.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using the <see cref="NumberStyle.Integer"/> style.
        /// In addition to the decimal digits, only leading and trailing spaces together with a leading sign are
        /// allowed. To explicitly define the style elements together with the culture-specific formatting
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out short)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(ReadOnlySpan{char}, out short)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out short)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, out short)"/>
        /// <seealso cref="Number.ToString()"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse(ReadOnlySpan<char> s, out short result)
        {
            return short.TryParse(s, out result);
        }
#endif

        #endregion TryParse_CharSequence_Int16

        #region Parse_CharSequence_NumberStyle_IFormatProvider

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its
        /// 16-bit signed integer equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be
        /// present in <paramref name="s"/>. A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A 16-bit signed integer equivalent to the number specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="s"/> is not in a format compliant with <paramref name="style"/>.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> includes non-zero fractional digits.
        /// </exception>
        /// <remarks>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or the positive sign) that are allowed in the
        /// <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/>
        /// enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/> parameter may include the following elements:
        /// <para/>
        /// [ws][$][sign][digits,]digits[.fractional-digits][e[sign]exponential-digits][ws]
        /// <para/>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws]hexdigits[ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if <paramref name="style"/> includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="short"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value without a prefix.
        /// For example, "F3" parses successfully, but "0xF3" does not. The only other flags that can be present in <paramref name="style"/> are
        /// <see cref="NumberStyle.AllowLeadingWhite"/> and <see cref="NumberStyle.AllowTrailingWhite"/>. (The <see cref="NumberStyle"/> enumeration
        /// has a composite number style, <see cref="NumberStyle.HexNumber"/>, that includes both white space flags.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// </remarks>
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/>
        /// <seealso cref="ValueOf(string, NumberStyle, IFormatProvider?)"/>
        public static short Parse(string s, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseShort()
        {
            NumberStyleExtensions.ValidateParseStyleInteger(style);
            return short.Parse(s, style.ToNumberStyles(), provider);
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its
        /// 16-bit signed integer equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be
        /// present in <paramref name="s"/>. A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A 16-bit signed integer equivalent to the number specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="s"/> is not in a format compliant with <paramref name="style"/>.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> includes non-zero fractional digits.
        /// </exception>
        /// <remarks>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or the positive sign) that are allowed in the
        /// <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/>
        /// enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/> parameter may include the following elements:
        /// <para/>
        /// [ws][$][sign][digits,]digits[.fractional-digits][e[sign]exponential-digits][ws]
        /// <para/>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws]hexdigits[ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if <paramref name="style"/> includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="short"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value without a prefix.
        /// For example, "F3" parses successfully, but "0xF3" does not. The only other flags that can be present in <paramref name="style"/> are
        /// <see cref="NumberStyle.AllowLeadingWhite"/> and <see cref="NumberStyle.AllowTrailingWhite"/>. (The <see cref="NumberStyle"/> enumeration
        /// has a composite number style, <see cref="NumberStyle.HexNumber"/>, that includes both white space flags.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// </remarks>
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/>
        /// <seealso cref="ValueOf(string, NumberStyle, IFormatProvider?)"/>
        public static short Parse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseShort()
        {
            NumberStyleExtensions.ValidateParseStyleInteger(style);
            return short.Parse(s, style.ToNumberStyles(), provider);
        }
#endif
        #endregion Parse_CharSequence_NumberStyle_IFormatProvider

        #region TryParse_CharSequence_NumberStyle_IFormatProvider_Int16

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its
        /// 16-bit signed integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: To exactly match Java, use <see cref="NumberStyle.AllowLeadingSign"/> for <paramref name="style"/> and
        /// <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>. We recommend factoring out
        /// exceptions when parsing, but if the Java code depends on exceptions, throw <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">A string containing a number to convert. The string is interpreted using the style specified by <paramref name="style"/>.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the 16-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <remarks>
        /// The <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/> method is like the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out short)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or a positive or negative sign)
        /// that are allowed in the <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of
        /// bit flags from the <see cref="NumberStyle"/> enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/>
        /// parameter may include the following elements:
        /// <code>
        /// [ws][$][sign][digits,]digits[.fractional-digits][e[sign]exponential-digits][ws]
        /// </code>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <code>
        /// [ws]hexdigits[ws]
        /// </code>
        /// Items in square brackets ([ and ]) are optional. The following table describes each element.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>Optional white space.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyPositivePattern"/>
        ///         property of the <see cref="NumberFormatInfo"/> object returned by the <see cref="IFormatProvider.GetFormat(Type?)"/> method of the
        ///         <paramref name="provider"/> parameter. The currency symbol can appear in <paramref name="s"/> if style includes the
        ///         <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>An optional sign.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A sequence of digits ranging from 0 to 9.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i></term>
        ///         <term>One or more occurrences of the digit 0. Fractional digits can appear in <paramref name="s"/> only if style includes
        ///         the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</i></term>
        ///         <term></term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="short"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value without a prefix.
        /// For example, "C9AF3" parses successfully, but "0xC9AF3" does not. The only other flags that can be present in <paramref name="style"/>
        /// are <see cref="NumberStyle.AllowLeadingWhite"/> and <see cref="NumberStyle.AllowTrailingWhite"/>. (The <see cref="NumberStyle"/>
        /// enumeration has a composite style, <see cref="NumberStyle.HexNumber"/>, that includes both white space flags.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. If <paramref name="provider"/> is <c>null</c>, the <see cref="NumberFormatInfo"/> object for the current
        /// culture is used.
        /// </remarks>
        /// <seealso cref="Parse(string, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, out short)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyle style, IFormatProvider? provider, out short result)
        {
            NumberStyleExtensions.ValidateParseStyleInteger(style);
            return short.TryParse(s, style.ToNumberStyles(), provider, out result);
        }


#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 16-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: To exactly match Java, use <see cref="NumberStyle.AllowLeadingSign"/> for <paramref name="style"/> and
        /// <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>. We recommend factoring out
        /// exceptions when parsing, but if the Java code depends on exceptions, throw <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. The span is interpreted using
        /// the style specified by <paramref name="style"/>.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the 16-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out short)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out short)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or a positive or negative sign)
        /// that are allowed in the <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of
        /// bit flags from the <see cref="NumberStyle"/> enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/>
        /// parameter may include the following elements:
        /// <code>
        /// [ws][$][sign][digits,]digits[.fractional-digits][e[sign]exponential-digits][ws]
        /// </code>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <code>
        /// [ws]hexdigits[ws]
        /// </code>
        /// Items in square brackets ([ and ]) are optional. The following table describes each element.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>Optional white space.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyPositivePattern"/>
        ///         property of the <see cref="NumberFormatInfo"/> object returned by the <see cref="IFormatProvider.GetFormat(Type?)"/> method of the
        ///         <paramref name="provider"/> parameter. The currency symbol can appear in <paramref name="s"/> if style includes the
        ///         <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>An optional sign.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A sequence of digits ranging from 0 to 9.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i></term>
        ///         <term>One or more occurrences of the digit 0. Fractional digits can appear in <paramref name="s"/> only if style includes
        ///         the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="short"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The type suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, s must be a hexadecimal value without a prefix.
        /// For example, "C9AF3" parses successfully, but "0xC9AF3" does not. The only other flags that can be present in style
        /// are <see cref="NumberStyle.AllowLeadingWhite"/> and <see cref="NumberStyle.AllowTrailingWhite"/>. (The <see cref="NumberStyle"/>
        /// enumeration has a composite style, <see cref="NumberStyle.HexNumber"/>, that includes both white space flags.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. If <paramref name="provider"/> is <c>null</c>, the <see cref="NumberFormatInfo"/> object for the current
        /// culture is used.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(ReadOnlySpan{char}, out short)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider, out short result)
        {
            NumberStyleExtensions.ValidateParseStyleInteger(style);
            return short.TryParse(s, style.ToNumberStyles(), provider, out result);
        }
#endif

        #endregion TryParse_CharSequence_NumberStyle_IFormatProvider_Int16

        //    public override string ToString()
        //{
        //    return Integer.toString(value);
        //}

        /// <inheritdoc/>
        public override string ToString(string? format, IFormatProvider? provider)
        {
            return ToString(format, provider, value);
        }

        /**
         * Returns a string containing a concise, human-readable description of the
         * specified short value with radix 10.
         *
         * @param value
         *             the short to convert to a string.
         * @return a printable representation of {@code value}.
         */
        public static string ToString(short value)
        {
            return Int32.ToString(value);
        }

        /**
 * Returns a string containing a concise, human-readable description of the
 * specified short value with radix 10.
 *
 * @param value
 *             the short to convert to a string.
 * @return a printable representation of {@code value}.
 */
        public static string ToString(short value, IFormatProvider? provider)
        {
            return Int32.ToString(value, provider);
        }

        /////**
        //// * Parses the specified string as a signed decimal short value.
        //// *
        //// * @param string
        //// *            the string representation of a short value.
        //// * @return a {@code Short} instance containing the short value represented
        //// *         by {@code string}.
        //// * @throws NumberFormatException
        //// *             if {@code string} is {@code null}, has a length of zero or
        //// *             can not be parsed as a short value.
        //// * @see #parseShort(String)
        //// */
        ////public static Int16 ValueOf(string value)
        ////{
        ////    return ValueOf(Parse(value));
        ////}

        /**
         * Parses the specified string as a signed decimal short value.
         *
         * @param string
         *            the string representation of a short value.
         * @return a {@code Short} instance containing the short value represented
         *         by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a short value.
         * @see #parseShort(String)
         */
        public static Int16 ValueOf(string value, IFormatProvider? provider)
        {
            return ValueOf(Parse(value, provider));
        }

        /**
         * Parses the specified string as a signed decimal short value.
         *
         * @param string
         *            the string representation of a short value.
         * @return a {@code Short} instance containing the short value represented
         *         by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a short value.
         * @see #parseShort(String)
         */
        public static Int16 ValueOf(string value, NumberStyle style, IFormatProvider? provider)
        {
            return ValueOf(Parse(value, style, provider));
        }

        /**
         * Parses the specified string as a signed short value using the specified
         * radix.
         *
         * @param string
         *            the string representation of a short value.
         * @param radix
         *            the radix to use when parsing.
         * @return a {@code Short} instance containing the short value represented
         *         by {@code string} using {@code radix}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null} or has a length of zero,
         *             {@code radix < Character.MIN_RADIX},
         *             {@code radix > Character.MAX_RADIX}, or if {@code string}
         *             can not be parsed as a short value.
         * @see #parseShort(String, int)
         */
        public static Int16 ValueOf(string value, int radix)
        {
            return ValueOf(Parse(value, radix));
        }

        #region ReverseBytes

        /// <summary>
        /// Returns the value obtained by reversing the order of the bytes in the
        /// two's complement representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="short"/> value for which to reverse the byte order.</param>
        /// <returns>The value obtained by reversing the bytes in the specified
        /// <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static short ReverseBytes(short value) // J2N: Only used as a proxy for testing purposes
        {
            return value.ReverseBytes();
        }

        #endregion ReverseBytes

        /**
         * Returns a {@code Short} instance for the specified short value.
         * <p>
         * If it is not necessary to get a new {@code Short} instance, it is
         * recommended to use this method instead of the constructor, since it
         * maintains a cache of instances which may result in better performance.
         *
         * @param s
         *            the short value to store in the instance.
         * @return a {@code Short} instance containing {@code s}.
         * @since 1.5
         */
        public static Int16 ValueOf(short s)
        {
            if (s < -128 || s > 127)
            {
                return new Int16(s);
            }
            return ValueOfCache.Cache[s + 128];
        }

        private static class ValueOfCache
        {
            /// <summary>
            /// A cache of instances used by <see cref="ValueOf(short)"/> and auto-boxing.
            /// </summary>
            internal static readonly Int16[] Cache = LoadCache();

            private static Int16[] LoadCache()
            {
                var cache = new Int16[256];

                for (short i = -128; i <= 127; i++)
                {
                    cache[i + 128] = new Int16(i);
                }
                return cache;

            }
        }

        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator short(Int16 value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Int16(short value) => ValueOf(value);

        #region IConvertible implementation

        /// <inheritdoc/>
        public override byte ToByte()
        {
            return (byte)value;
        }

        /// <inheritdoc/>
        public override double ToDouble()
        {
            return value;
        }

        /// <inheritdoc/>
        public override short ToInt16()
        {
            return value;
        }

        /// <inheritdoc/>
        public override int ToInt32()
        {
            return value;
        }

        /// <inheritdoc/>
        public override long ToInt64()
        {
            return value;
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public override sbyte ToSByte()
        {
            return (sbyte)value;
        }

        /// <inheritdoc/>
        public override float ToSingle()
        {
            return value;
        }

        //
        // IConvertible implementation
        //

        /// <summary>
        /// Returns the <see cref="TypeCode"/> for value type <see cref="short"/>.
        /// </summary>
        /// <returns>The enumerated constant, <see cref="TypeCode.Int16"/>.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Int16;
        }


        bool IConvertible.ToBoolean(IFormatProvider? provider)
        {
            return Convert.ToBoolean(value);
        }

        char IConvertible.ToChar(IFormatProvider? provider)
        {
            return Convert.ToChar(value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider? provider)
        {
            return Convert.ToSByte(value);
        }

        byte IConvertible.ToByte(IFormatProvider? provider)
        {
            return Convert.ToByte(value);
        }

        short IConvertible.ToInt16(IFormatProvider? provider)
        {
            return value;
        }

        ushort IConvertible.ToUInt16(IFormatProvider? provider)
        {
            return Convert.ToUInt16(value);
        }

        int IConvertible.ToInt32(IFormatProvider? provider)
        {
            return Convert.ToInt32(value);
        }

        uint IConvertible.ToUInt32(IFormatProvider? provider)
        {
            return Convert.ToUInt32(value);
        }

        long IConvertible.ToInt64(IFormatProvider? provider)
        {
            return Convert.ToInt64(value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider? provider)
        {
            return Convert.ToUInt64(value);
        }

        float IConvertible.ToSingle(IFormatProvider? provider)
        {
            return Convert.ToSingle(value);
        }

        double IConvertible.ToDouble(IFormatProvider? provider)
        {
            return Convert.ToDouble(value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider? provider)
        {
            return Convert.ToDecimal(value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider? provider)
        {
            throw new InvalidCastException(J2N.SR.Format(SR.InvalidCast_FromTo, "Int16", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider? provider)
        {
            return /*Convert.*/DefaultToType((IConvertible)this.value, type, provider);
        }

        #endregion
    }
}
