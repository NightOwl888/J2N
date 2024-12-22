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

using J2N.Globalization;
using J2N.Text;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Numerics
{
    /// <summary>
    /// An immutable reference type that wraps the primitive <see cref="byte"/> type.
    /// <para/>
    /// In addition, this class provides methods for converting a <see cref="byte"/> to a <see cref="string"/> and
    /// a <see cref="string"/> to a <see cref="byte"/> that are compatible with Java.
    /// <para/>
    /// Instances of this class can be produced implicitly by setting a <see cref="byte"/> value to a variable declared
    /// as <see cref="Byte"/>
    /// <code>
    /// byte value = 4;
    /// Byte instance = value;
    /// </code>
    /// Or explicitly by calling one of the <see cref="GetInstance(byte)"/>, <see cref="Decode(string)"/>,
    /// or <see cref="TryDecode(string, out Byte)"/> methods.
    /// <para/>
    /// The <see cref="byte"/> value of a <see cref="Byte"/> can also be retrieved in several ways. For implicit
    /// conversion, simply assign a <see cref="byte"/> variable an instance of <see cref="Byte"/>.
    /// <code>
    /// Byte instance = Byte.GetInstance((byte)4);
    /// byte value = instance;
    /// </code>
    /// To explicitly get the value, call <see cref="ToByte()"/> or use the <see cref="Convert"/> class.
    /// <code>
    /// byte converted1 = instance.ToByte();
    /// byte converted2 = Convert.ToByte(instance, NumberFormatInfo.InvariantInfo);
    /// </code>
    /// <para/>
    /// In most cases, the number types in .NET will suffice. The main reason for creating an object to wrap numeric types is to
    /// provide a way to make strongly-typed instances that can co-exist in collections and arrays with reference types.
    /// For example, when creating a table object that has columns with a mix of number and string data types.
    /// When porting code from Java, there are sometimes cases where the design didn't factor in the use of value types,
    /// so these classes can be used rather than reworking the design.
    /// For more information about numbers classes, see
    /// <a href="https://docs.oracle.com/javase/tutorial/java/data/numberclasses.html">The Numbers Classes</a>.
    /// </summary>
    /// <seealso cref="Number"/>
    /// <seealso cref="IConvertible"/>
    /// <seealso cref="IFormattable"/>
    /// <seealso cref="IComparable"/>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerDisplay("{value}")]
    public sealed class Byte : Number, IComparable<Byte>, IComparable, IConvertible, IEquatable<Byte>
    {
        /// <summary>
        /// The value which the receiver represents.
        /// </summary>
        private readonly byte value; // Do not rename (binary serialization)

        /// <summary>
        /// Constant for the number of bits needed to represent a <see cref="byte"/> in
        /// two's complement form.
        /// </summary>
        public const int Size = 8;

        /// <summary>
        /// A cache of instances used by <see cref="GetInstance(byte)"/> and auto-boxing.
        /// </summary>
        private static readonly Byte[] Cache = new Byte[256];

        /// <summary>
        /// Initializes a new instance of the <see cref="Byte"/> class.
        /// </summary>
        /// <param name="value">The primitive <see cref="byte"/> value to store in the new instance.</param>
        internal Byte(byte value) // J2N: This has been marked deprecated in JDK 16, so we are marking it internal
        {
            this.value = value;
        }

        // J2N: Removed other overloads because all of the constructors are deprecated in JDK 16

        #region CompareTo

        /// <summary>
        /// Compares this instance to a specified <see cref="Byte"/> and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An <see cref="Byte"/> to compare, or <c>null</c>.</param>
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
        public int CompareTo(Byte? value)
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
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a <see cref="Byte"/>.</exception>
        /// <remarks>
        /// <paramref name="value"/> must be <c>null</c> or an instance of <see cref="Byte"/>; otherwise, an exception is thrown.
        /// <para/>
        /// Any instance of <see cref="Byte"/>, regardless of its value, is considered greater than <c>null</c>.
        /// <para/>
        /// This method is implemented to support the <see cref="IComparable"/> interface.
        /// </remarks>
        public int CompareTo(object? value)
        {
            if (value is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            if (!(value is Byte other))
                throw new ArgumentException(SR.Arg_MustBeByte);
            return this.value - other.value;
        }

        #endregion CompareTo

        #region Decode

        /// <summary>
        /// Decodes a <see cref="string"/> into an <see cref="Byte"/>. Accepts decimal, hexadecimal, and octal numbers given by the following grammar:
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
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// although it returns <see cref="Byte"/> which stores an unsigned value internally and may need to be converted depending on its usage.
        /// </summary>
        /// <param name="s">A <see cref="string"/> that contains the number to convert.</param>
        /// <returns>An 8-bit unsigned integer that is equivalent to the number in <paramref name="s"/>.</returns>
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
        /// <paramref name="s"/> represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> represents a two's complement negative number. Only positive values are allowed without a negative sign.
        /// </exception>
        public static Byte Decode(string s)
        {
            if (s is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);

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
                    return GetInstance(0);
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

            int r = ParseNumbers.StringToInt(s.AsSpan(), @base, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign, ref i, s.Length - i);

            if (r < sbyte.MinValue || r > byte.MaxValue)
                throw new OverflowException(SR.Overflow_Byte);
            // Only allow negative if it was passed as a sign in the string
            if (r < 0 && sign > 0)
                throw new OverflowException(SR.Overflow_Byte);
            return GetInstance((byte)r);
        }

        #endregion Decode

        #region TryDecode

        /// <summary>
        /// Decodes a <see cref="string"/> into an <see cref="Byte"/>. Accepts decimal, hexadecimal, and octal numbers given by the following grammar:
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
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// although it returns <see cref="Byte"/> which stores an unsigned value internally and may need to be converted depending on its usage.
        /// </summary>
        /// <param name="s">A <see cref="string"/> that contains the number to convert.</param>
        /// <param name="result">When this method returns, contains the 8-bit unsigned integer value equivalent to the number contained in <paramref name="s"/>,
        /// if the conversion succeeded, or zero if the conversion failed. The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or
        /// <see cref="string.Empty"/>, is not of the correct format, or represents a number less than <see cref="sbyte.MinValue"/> or greater than
        /// <see cref="byte.MaxValue"/>. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        public static bool TryDecode(string s, [MaybeNullWhen(false)] out Byte result)
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
                    result = GetInstance(0);
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

            if (!ParseNumbers.TryStringToInt(s.AsSpan(), @base, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign, ref i, s.Length - i, out int r) ||
                // Only allow negative if it was passed as a sign in the string
                (r < 0 && sign > 0) ||
                (r < sbyte.MinValue || r > byte.MaxValue))
            {
                return false;
            }

            result = GetInstance((byte)r);
            return true;
        }

        #endregion TryDecode

        #region Equals

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Byte"/> value.
        /// </summary>
        /// <param name="obj">A <see cref="Byte"/> value to compare to this instance.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> has the same value as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>This method implements the <see cref="IEquatable{T}"/> interface, and performs slightly better than
        /// <see cref="Equals(object?)"/> because it does not have to convert the <paramref name="obj"/> parameter to an object.</remarks>
        public bool Equals(Byte? obj)
        {
            if (obj is null) return false;

            return ReferenceEquals(obj, this) || (value == obj.value);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is an instance of <see cref="Byte"/> and equals the value of
        /// this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return ReferenceEquals(obj, this) || (obj is Byte other) && (value == other.value);
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

        #endregion

        // Radix-based parsing (default in Java)

        #region Parse_CharSequence_Int32_Int32_Int32

        /// <summary>
        /// Parses the <see cref="ReadOnlySpan{T}"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// </summary>
        /// <param name="s">The <see cref="ReadOnlySpan{T}"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
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
        /// represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(ReadOnlySpan{char}, int, int, int, out byte)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static byte Parse(ReadOnlySpan<char> s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            int r = ParseNumbers.StringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length);
            if (radix != 10 && r <= byte.MaxValue)
                return (byte)r;

            if (r < sbyte.MinValue || r > byte.MaxValue)
                throw new OverflowException(SR.Overflow_Byte);
            return (byte)r;
        }

        /// <summary>
        /// Parses the <see cref="string"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
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
        /// represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(string, int, int, int, out byte)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static byte Parse(string s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (s is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            int r = ParseNumbers.StringToInt(s.AsSpan(), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length);
            if (radix != 10 && r <= byte.MaxValue)
                return (byte)r;

            if (r < sbyte.MinValue || r > byte.MaxValue)
                throw new OverflowException(SR.Overflow_Byte);
            return (byte)r;
        }

        /// <summary>
        /// Parses the <see cref="T:char[]"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// </summary>
        /// <param name="s">The <see cref="T:char[]"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
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
        /// represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(char[], int, int, int, out byte)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static byte Parse(char[] s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (s is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            int r = ParseNumbers.StringToInt(s.AsSpan(), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length);
            if (radix != 10 && r <= byte.MaxValue)
                return (byte)r;

            if (r < sbyte.MinValue || r > byte.MaxValue)
                throw new OverflowException(SR.Overflow_Byte);
            return (byte)r;
        }

        /// <summary>
        /// Parses the <see cref="StringBuilder"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// </summary>
        /// <param name="s">The <see cref="StringBuilder"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
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
        /// represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(StringBuilder, int, int, int, out byte)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static byte Parse(StringBuilder s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (s is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            int r = ParseNumbers.StringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length);
            if (radix != 10 && r <= byte.MaxValue)
                return (byte)r;

            if (r < sbyte.MinValue || r > byte.MaxValue)
                throw new OverflowException(SR.Overflow_Byte);
            return (byte)r;
        }

        /// <summary>
        /// Parses the <see cref="ICharSequence"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// </summary>
        /// <param name="s">The <see cref="ICharSequence"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c> or its <see cref="ICharSequence.HasValue"/> property returns <c>false</c>.</exception>
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
        /// represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(ICharSequence, int, int, int, out byte)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static byte Parse(ICharSequence s, int startIndex, int length, int radix) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            if (s is null || !s.HasValue)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s, ExceptionResource.ArgumentNull_NullOrNullValue);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            int r;
            if (s is StringBuilderCharSequence stringBuilderCharSequence)
            {
                r = ParseNumbers.StringToInt(stringBuilderCharSequence.Value!, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length);
            }
            else if (s is StringBuffer stringBuffer)
            {
                lock (stringBuffer.SyncRoot)
                {
                    r = ParseNumbers.StringToInt(stringBuffer.builder, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length);
                }
            }
            else
            {
                r = ParseNumbers.StringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length);
            }

            if (radix != 10 && r <= byte.MaxValue)
                return (byte)r;

            if (r < sbyte.MinValue || r > byte.MaxValue)
                throw new OverflowException(SR.Overflow_Byte);
            return (byte)r;
        }

        #endregion Parse_CharSequence_Int32_Int32_Int32

        #region TryParse_CharSequence_Int32_Int32_Int32_Byte

        /// <summary>
        /// Parses the <see cref="ReadOnlySpan{Char}"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// <para/>
        /// Since <see cref="Parse(ReadOnlySpan{char}, int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="ReadOnlySpan{Char}"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
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
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(ReadOnlySpan<char> s, int startIndex, int length, int radix, out byte result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if ((uint)(radix - Character.MinRadix) > (Character.MaxRadix - Character.MinRadix)) // Check both bounds MinRadix and MaxRadix (inclusive) at once
                ThrowHelper.ThrowRadixArgumentOutOfRange(radix);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            if (ParseNumbers.TryStringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length, out int r))
            {
                if (radix != 10 && r <= byte.MaxValue)
                {
                    result = (byte)r;
                    return true;
                }

                if (r < sbyte.MinValue || r > byte.MaxValue)
                {
                    return false;
                }

                result = (byte)r;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the <see cref="string"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// <para/>
        /// Since <see cref="Parse(string, int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
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
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(string, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(string s, int startIndex, int length, int radix, out byte result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (s is null)
                return false;
            if ((uint)(radix - Character.MinRadix) > (Character.MaxRadix - Character.MinRadix)) // Check both bounds MinRadix and MaxRadix (inclusive) at once
                ThrowHelper.ThrowRadixArgumentOutOfRange(radix);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            if (ParseNumbers.TryStringToInt(s.AsSpan(), radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length, out int r))
            {
                if (radix != 10 && r <= byte.MaxValue)
                {
                    result = (byte)r;
                    return true;
                }

                if (r < sbyte.MinValue || r > byte.MaxValue)
                {
                    return false;
                }

                result = (byte)r;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the <see cref="T:char[]"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// <para/>
        /// Since <see cref="Parse(char[], int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="T:char[]"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
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
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(char[], int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(char[] s, int startIndex, int length, int radix, out byte result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (s is null)
                return false;
            if ((uint)(radix - Character.MinRadix) > (Character.MaxRadix - Character.MinRadix)) // Check both bounds MinRadix and MaxRadix (inclusive) at once
                ThrowHelper.ThrowRadixArgumentOutOfRange(radix);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);


            if (ParseNumbers.TryStringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length, out int r))
            {
                if (radix != 10 && r <= byte.MaxValue)
                {
                    result = (byte)r;
                    return true;
                }

                if (r < sbyte.MinValue || r > byte.MaxValue)
                {
                    return false;
                }

                result = (byte)r;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the <see cref="StringBuilder"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// <para/>
        /// Since <see cref="Parse(StringBuilder, int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="StringBuilder"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
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
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(StringBuilder, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(StringBuilder s, int startIndex, int length, int radix, out byte result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (s is null)
                return false;
            if ((uint)(radix - Character.MinRadix) > (Character.MaxRadix - Character.MinRadix)) // Check both bounds MinRadix and MaxRadix (inclusive) at once
                ThrowHelper.ThrowRadixArgumentOutOfRange(radix);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            if (ParseNumbers.TryStringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length, out int r))
            {
                if (radix != 10 && r <= byte.MaxValue)
                {
                    result = (byte)r;
                    return true;
                }

                if (r < sbyte.MinValue || r > byte.MaxValue)
                {
                    return false;
                }

                result = (byte)r;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the <see cref="ICharSequence"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length rather than an exclusive end index as would be the case in Java. To translate from
        /// Java, use <c>endIndex - beginIndex</c> to obtain the <paramref name="length"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// <para/>
        /// Since <see cref="Parse(ICharSequence, int, int, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="ICharSequence"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
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
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(ICharSequence, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(ICharSequence s, int startIndex, int length, int radix, out byte result) // KEEP OVERLOADS FOR ICharSequence, char[], ReadOnlySpan<char>, StringBuilder, and string IN SYNC
        {
            result = default;

            if (s is null || !s.HasValue)
                return false;
            if ((uint)(radix - Character.MinRadix) > (Character.MaxRadix - Character.MinRadix)) // Check both bounds MinRadix and MaxRadix (inclusive) at once
                ThrowHelper.ThrowRadixArgumentOutOfRange(radix);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > s.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            int r;
            if (s is StringBuilderCharSequence stringBuilderCharSequence && ParseNumbers.TryStringToInt(stringBuilderCharSequence.Value!, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length, out r))
            {
                goto ReturnResult;
            }

            if (s is StringBuffer stringBuffer)
            {
                lock (stringBuffer.SyncRoot)
                {
                    if (ParseNumbers.TryStringToInt(stringBuffer.builder, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length, out r))
                    {
                        goto ReturnResult;
                    }
                }
                return false;
            }

            if (ParseNumbers.TryStringToInt(s, radix, flags: ParseNumbers.IsTight | ParseNumbers.TreatAsI1, sign: 1, ref startIndex, length, out r))
            {
                goto ReturnResult;
            }

            return false;

        ReturnResult:

            if (radix != 10 && r <= byte.MaxValue)
            {
                result = (byte)r;
                return true;
            }

            if (r < sbyte.MinValue || r > byte.MaxValue)
            {
                return false;
            }

            result = (byte)r;
            return true;
        }

        #endregion TryParse_CharSequence_Int32_Int32_Int32_Byte

        #region Parse_CharSequence_Int32

        /// <summary>
        /// Parses the <see cref="string"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method. It differs in that
        /// it allows the use of the ASCII character \u002d ('-') or \u002B ('+') in any <paramref name="radix"/>.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>An 8-bit unsigned integer that is equivalent to the number in <paramref name="s"/>, or 0 (zero) if
        /// <paramref name="s"/> is <c>null</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of a long integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
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
        /// <paramref name="s"/> represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(ReadOnlySpan{char}, int, out byte)"/>
        /// <seealso cref="Parse(ReadOnlySpan{char}, int)"/>
        public static byte Parse(ReadOnlySpan<char> s, int radix) // J2N: Renamed from ParseByte()
        {
            int r = ParseNumbers.StringToInt(s, radix, ParseNumbers.IsTight | ParseNumbers.TreatAsI1);

            if (radix != 10 && r <= byte.MaxValue)
                return (byte)r;

            if (r < sbyte.MinValue || r > byte.MaxValue) // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
                throw new OverflowException(SR.Overflow_Byte);
            return (byte)r;
        }

        /// <summary>
        /// Parses the <see cref="string"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>. 
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method. It differs in that
        /// it allows the use of the ASCII character \u002d ('-') or \u002B ('+') in any <paramref name="radix"/>.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>An 8-bit unsigned integer that is equivalent to the number in <paramref name="s"/>, or 0 (zero) if
        /// <paramref name="s"/> is <c>null</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of a long integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
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
        /// <paramref name="s"/> represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(string?, int, out byte)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static byte Parse(string? s, int radix) // J2N: Renamed from ParseByte()
        {
            if (s == null)
            {
                return 0;
            }

            int r = ParseNumbers.StringToInt(s.AsSpan(), radix, ParseNumbers.IsTight | ParseNumbers.TreatAsI1);

            if (radix != 10 && r <= byte.MaxValue)
                return (byte)r;

            if (r < sbyte.MinValue || r > byte.MaxValue) // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
                throw new OverflowException(SR.Overflow_Byte);
            return (byte)r;
        }

        #endregion Parse_CharSequence_Int32

        #region TryParse_CharSequence_Int32_Byte

        /// <summary>
        /// Parses the <see cref="ReadOnlySpan{T}"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method. It differs in that it
        /// allows the use of the ASCII character \u002d ('-') or \u002B ('+') in any <paramref name="radix"/> and allows any 
        /// <paramref name="radix"/> value from <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// <para/>
        /// Since <see cref="Parse(ReadOnlySpan{char}, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="ReadOnlySpan{T}"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, int, int, int)"/>
        /// <seealso cref="Parse(ReadOnlySpan{char}, int)"/>
        public static bool TryParse(ReadOnlySpan<char> s, int radix, out byte result) // J2N: Renamed from ParseByte()
        {
            if ((uint)(radix - Character.MinRadix) > (Character.MaxRadix - Character.MinRadix)) // Check both bounds MinRadix and MaxRadix (inclusive) at once
                ThrowHelper.ThrowRadixArgumentOutOfRange(radix);

            if (!ParseNumbers.TryStringToInt(s, radix, ParseNumbers.IsTight | ParseNumbers.TreatAsI1, out int r))
            {
                result = default;
                return false;
            }

            if (radix != 10 && r <= byte.MaxValue)
            {
                result = (byte)r;
                return true;
            }

            if (r < sbyte.MinValue || r > byte.MaxValue)
            {
                result = default;
                return false;
            }

            result = (byte)r;
            return true;
        }

        /// <summary>
        /// Parses the <see cref="string"/> argument as a <see cref="byte"/> in the specified <paramref name="radix"/>.
        /// <para/>
        /// Usage Note: This method is similar to the <see cref="Convert.ToByte(string?, int)"/> method. It differs in that it
        /// allows the use of the ASCII character \u002d ('-') or \u002B ('+') in any <paramref name="radix"/> and allows any 
        /// <paramref name="radix"/> value from <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value returned is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// <para/>
        /// Since <see cref="Parse(string?, int)"/> throws many different exception types and in Java they are all normalized to
        /// <c>NumberFormatException</c>, this method can be used to mimic the same behavior by throwing <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <param name="result">The <see cref="byte"/> represented by the subsequence in the specified <paramref name="radix"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of an integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
        /// </remarks>
        /// <seealso cref="Parse(string, int, int, int)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static bool TryParse(string? s, int radix, out byte result) // J2N: Renamed from ParseByte()
        {
            if ((uint)(radix - Character.MinRadix) > (Character.MaxRadix - Character.MinRadix)) // Check both bounds MinRadix and MaxRadix (inclusive) at once
                ThrowHelper.ThrowRadixArgumentOutOfRange(radix);

            if (s == null)
            {
                result = 0;
                return true;
            }

            if (!ParseNumbers.TryStringToInt(s.AsSpan(), radix, ParseNumbers.IsTight | ParseNumbers.TreatAsI1, out int r))
            {
                result = default;
                return false;
            }

            if (radix != 10 && r <= byte.MaxValue)
            {
                result = (byte)r;
                return true;
            }

            if (r < sbyte.MinValue || r > byte.MaxValue)
            {
                result = default;
                return false;
            }

            result = (byte)r;
            return true;
        }

        #endregion TryParse_CharSequence_Int32_Byte

        // Culture-aware parsing (default in .NET)

        #region Parse_CharSequence_IFormatProvider

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its <see cref="byte"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing the characters representing the value to convert. The string is interpreted using the <see cref="NumberStyle.Integer"/> style.
        /// For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>. If provider is <c>null</c>,
        /// the thread current culture is used.</param>
        /// <returns>A byte value that is equivalent to the number contained in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> is not of the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number less than <see cref="sbyte.MinValue"/> or <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <remarks>
        /// The s parameter contains a number of the form:
        /// <para/>
        /// [ws][sign]digits[ws]
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
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using the <see cref="NumberStyle.Integer"/> style. In addition to the byte value's
        /// decimal digits, only leading and trailing spaces together with a leading sign are allowed. To explicitly define the style elements
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
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/>
        public static byte Parse(string s, IFormatProvider? provider) // J2N: Renamed from ParseByte()
        {
            return Parse(s, NumberStyle.Integer, provider);
        }

        #endregion Parse_CharSequence_IFormatProvider

        #region TryParse_CharSequence_Byte

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its 8-bit unsigned
        /// integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Byte class, so call <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A string containing the characters that represent the number to convert. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="result">When this method returns, contains the 8-bit unsigned integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="byte.MinValue"/> or greater than <see cref="byte.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(string?, out byte)"/> method is like the <see cref="Parse(string?, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(string?, out byte)"/> method does not throw an exception if the
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
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(string?, out byte)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/>
        /// <seealso cref="Number.ToString()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse([NotNullWhen(true)] string? s, out byte result) // Culture Sensitive!
        {
            return TryParse(s, NumberStyle.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 8-bit unsigned
        /// integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Byte class, so call <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="result">When this method returns, contains the 8-bit unsigned integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="byte.MinValue"/> or greater than <see cref="byte.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, out byte)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, out byte)"/> method does not throw an exception if the
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
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(ReadOnlySpan{char}, out byte)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, out byte)"/>
        /// <seealso cref="Number.ToString()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(ReadOnlySpan<char> s, out byte result) // Culture Sensitive!
        {
            return TryParse(s, NumberStyle.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        #endregion TryParse_CharSequence_Byte

        #region Parse_CharSequence_NumberStyle_IFormatProvider

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its <see cref="byte"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing the characters representing the value to convert. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>. If provider is <c>null</c>,
        /// the thread current culture is used.</param>
        /// <returns>A byte value that is equivalent to the number contained in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> is not of the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number less than <see cref="sbyte.MinValue"/> or <see cref="byte.MaxValue"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> includes non-zero, fractional digits.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <remarks>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or the positive sign) that are allowed in the
        /// <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/>
        /// enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/> parameter may include the following elements:
        /// <para/>
        /// [ws][$][sign]digits[.fractional-digits][e[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws][0x]hexdigits[hextype][ws]
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
        ///         <term><i>0x</i>/></term>
        ///         <term>The '0x' or '0X' characters, which indicate a hexadecimal number is to immediately follow.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'UL', 'Ul', 'uL', 'ul', 'LU', 'Lu', 'lU', or 'lu', which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6453-integer-literals">integral type suffix</a>
        ///         or 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6454-real-literals">real type suffix</a>
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hextype</i></term>
        ///         <term>The 'UL', 'Ul', 'uL', 'ul', 'LU', 'Lu', 'lU', or 'lu', which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6453-integer-literals">integral type suffix</a>
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. Real type suffixes are not supported.</term>
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
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="byte"/> data type
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
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# (suffixed with one of
        ///         'D', 'd', 'F', 'f', 'M', 'm', 'L', 'l', 'U', 'u', 'UL', 'ul', 'LU', or 'lu').</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value with or without a <c>0x</c> prefix.
        /// For example, "F3" and "0xF3" both parse successfully. The only other flags that can be present in <paramref name="style"/> are
        /// <see cref="NumberStyle.AllowLeadingWhite"/>, <see cref="NumberStyle.AllowTrailingWhite"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// (The <see cref="NumberStyle"/> enumeration has a composite number style, <see cref="NumberStyle.HexNumber"/>, that includes both white space
        /// flags.)
        /// The <see cref="NumberStyle.AllowTypeSpecifier"/> only allows 'L', 'l', 'U', 'u', 'UL', 'ul', 'LU', or 'lu' for hexadecimal numbers.
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
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/>
        public static byte Parse(string s, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseByte()
        {
            NumberStyleExtensions.ValidateParseStyleInteger(style);
            if (s is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);

            DotNetNumber.ParsingStatus status = DotNetNumber.TryParseInt32(s.AsSpan(), style, NumberFormatInfo.GetInstance(provider), out int i);
            if (status != DotNetNumber.ParsingStatus.OK)
            {
                if (status == DotNetNumber.ParsingStatus.Overflow)
                    DotNetNumber.ThrowOverflowException(TypeCode.Byte);
                if (status == DotNetNumber.ParsingStatus.Failed)
                    DotNetNumber.ThrowFormatException(s.ToString());
            }

            // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
            // For hex number styles AllowHexSpecifier >> 2 == 0x80 and cancels out MinValue so the check is effectively: (uint)i > byte.MaxValue
            // For integer styles it's zero and the effective check is (uint)(i - MinValue) > byte.MaxValue
            if ((uint)(i - sbyte.MinValue - ((int)(style & NumberStyle.AllowHexSpecifier) >> 2)) > byte.MaxValue)
            {
                DotNetNumber.ThrowOverflowException(TypeCode.Byte);
            }
            return (byte)i;
        }

        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its <see cref="byte"/> equivalent.
        /// </summary>
        /// <param name="s">A span containing the characters representing the value to convert. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>. If provider is <c>null</c>,
        /// the thread current culture is used.</param>
        /// <returns>A byte value that is equivalent to the number contained in <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> is not of the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number less than <see cref="sbyte.MinValue"/> or <see cref="byte.MaxValue"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> includes non-zero, fractional digits.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <remarks>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or the positive sign) that are allowed in the
        /// <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/>
        /// enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/> parameter may include the following elements:
        /// <para/>
        /// [ws][$][sign]digits[.fractional-digits][e[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws][0x]hexdigits[hextype][ws]
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
        ///         <term><i>0x</i>/></term>
        ///         <term>The '0x' or '0X' characters, which indicate a hexadecimal number is to immediately follow.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'UL', 'Ul', 'uL', 'ul', 'LU', 'Lu', 'lU', or 'lu', which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6453-integer-literals">integral type suffix</a>
        ///         or 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6454-real-literals">real type suffix</a>
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hextype</i></term>
        ///         <term>The 'UL', 'Ul', 'uL', 'ul', 'LU', 'Lu', 'lU', or 'lu', which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6453-integer-literals">integral type suffix</a>
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. Real type suffixes are not supported.</term>
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
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="byte"/> data type
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
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# (suffixed with one of
        ///         'D', 'd', 'F', 'f', 'M', 'm', 'L', 'l', 'U', 'u', 'UL', 'ul', 'LU', or 'lu').</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value with or without a <c>0x</c> prefix.
        /// For example, "F3" and "0xF3" both parse successfully. The only other flags that can be present in <paramref name="style"/> are
        /// <see cref="NumberStyle.AllowLeadingWhite"/>, <see cref="NumberStyle.AllowTrailingWhite"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// (The <see cref="NumberStyle"/> enumeration has a composite number style, <see cref="NumberStyle.HexNumber"/>, that includes both white space
        /// flags.)
        /// The <see cref="NumberStyle.AllowTypeSpecifier"/> only allows 'L', 'l', 'U', 'u', 'UL', 'ul', 'LU', or 'lu' for hexadecimal numbers.
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
        /// <seealso cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/>
        public static byte Parse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseByte()
        {
            NumberStyleExtensions.ValidateParseStyleInteger(style);
            DotNetNumber.ParsingStatus status = DotNetNumber.TryParseInt32(s, style, NumberFormatInfo.GetInstance(provider), out int i);
            if (status != DotNetNumber.ParsingStatus.OK)
            {
                if (status == DotNetNumber.ParsingStatus.Overflow)
                    DotNetNumber.ThrowOverflowException(TypeCode.Byte);
                if (status == DotNetNumber.ParsingStatus.Failed)
                    DotNetNumber.ThrowFormatException(s.ToString());
            }

            // For hex number styles AllowHexSpecifier >> 2 == 0x80 and cancels out MinValue so the check is effectively: (uint)i > byte.MaxValue
            // For integer styles it's zero and the effective check is (uint)(i - MinValue) > byte.MaxValue
            if ((uint)(i - sbyte.MinValue - ((int)(style & NumberStyle.AllowHexSpecifier) >> 2)) > byte.MaxValue)
            {
                DotNetNumber.ThrowOverflowException(TypeCode.Byte);
            }
            return (byte)i;
        }

        #endregion Parse_CharSequence_NumberStyle_IFormatProvider

        #region TryParse_CharSequence_NumberStyle_IFormatProvider_Byte

        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 8-bit unsigned integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: To exactly match Java, use <see cref="NumberStyle.AllowLeadingSign"/> for <paramref name="style"/> and
        /// <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>. We recommend factoring out
        /// exceptions when parsing, but if the Java code depends on exceptions, throw <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. The span is interpreted using
        /// the style specified by <paramref name="style"/>. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the 8-bit unsigned integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="byte.MinValue"/> or greater than <see cref="byte.MaxValue"/>. This parameter
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
        /// The <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/> method is like the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or a positive or negative sign)
        /// that are allowed in the <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of
        /// bit flags from the <see cref="NumberStyle"/> enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/>
        /// parameter may include the following elements:
        /// <code>
        /// [ws][$][sign][digits,]digits[.fractional-digits][e[sign]exponential-digits][type][ws]
        /// </code>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <code>
        /// [ws][0x]hexdigits[hextype][ws]
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
        ///         <term><i>0x</i>/></term>
        ///         <term>The '0x' or '0X' characters, which indicate a hexadecimal number is to immediately follow.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'UL', 'Ul', 'uL', 'ul', 'LU', 'Lu', 'lU', or 'lu', which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6453-integer-literals">integral type suffix</a>
        ///         or 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6454-real-literals">real type suffix</a>
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hextype</i></term>
        ///         <term>The 'UL', 'Ul', 'uL', 'ul', 'LU', 'Lu', 'lU', or 'lu', which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6453-integer-literals">integral type suffix</a>
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. Real type suffixes are not supported.</term>
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
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="byte"/> data type
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
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# (suffixed with one of
        ///         'D', 'd', 'F', 'f', 'M', 'm', 'L', 'l', 'U', 'u', 'UL', 'ul', 'LU', or 'lu').</term>
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
        /// The <see cref="NumberStyle"/> enum is a match in both symbol and value for the .NET <see cref="NumberStyles"/> enum.
        /// Therefore, simply casting the value will convert it properly between the two in both directions.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value with or without a <c>0x</c> prefix.
        /// For example, "F3" and "0xF3" both parse successfully. The only other flags that can be present in <paramref name="style"/> are
        /// <see cref="NumberStyle.AllowLeadingWhite"/>, <see cref="NumberStyle.AllowTrailingWhite"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// (The <see cref="NumberStyle"/> enumeration has a composite number style, <see cref="NumberStyle.HexNumber"/>, that includes both white space
        /// flags.)
        /// The <see cref="NumberStyle.AllowTypeSpecifier"/> only allows 'L', 'l', 'U', 'u', 'UL', 'ul', 'LU', or 'lu' for hexadecimal numbers.
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. If <paramref name="provider"/> is <c>null</c>, the <see cref="NumberFormatInfo"/> object for the current
        /// culture is used.
        /// </remarks>
        /// <seealso cref="Parse(string, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, out byte)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyle style, IFormatProvider? provider, out byte result)
        {
            NumberStyleExtensions.ValidateParseStyleInteger(style);
            if (s == null)
            {
                result = 0;
                return false;
            }
            // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte

            // For hex number styles AllowHexSpecifier >> 2 == 0x80 and cancels out MinValue so the check is effectively: (uint)i > byte.MaxValue
            // For integer styles it's zero and the effective check is (uint)(i - MinValue) > byte.MaxValue
            if (DotNetNumber.TryParseInt32(s.AsSpan(), style, NumberFormatInfo.GetInstance(provider), out int i) != DotNetNumber.ParsingStatus.OK
                || (uint)(i - sbyte.MinValue - ((int)(style & NumberStyle.AllowHexSpecifier) >> 2)) > byte.MaxValue)
            {
                result = 0;
                return false;
            }
            result = (byte)i;
            return true;
        }

        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 8-bit unsigned integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: To exactly match Java, use <see cref="NumberStyle.AllowLeadingSign"/> for <paramref name="style"/> and
        /// <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>. We recommend factoring out
        /// exceptions when parsing, but if the Java code depends on exceptions, throw <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. The span is interpreted using
        /// the style specified by <paramref name="style"/>. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the 8-bit unsigned integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>. This parameter
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
        /// The <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or a positive or negative sign)
        /// that are allowed in the <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of
        /// bit flags from the <see cref="NumberStyle"/> enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/>
        /// parameter may include the following elements:
        /// <code>
        /// [ws][$][sign][digits,]digits[.fractional-digits][e[sign]exponential-digits][type][ws]
        /// </code>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <code>
        /// [ws][0x]hexdigits[hextype][ws]
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
        ///         <term><i>0x</i>/></term>
        ///         <term>The '0x' or '0X' characters, which indicate a hexadecimal number is to immediately follow.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'UL', 'Ul', 'uL', 'ul', 'LU', 'Lu', 'lU', or 'lu', which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6453-integer-literals">integral type suffix</a>
        ///         or 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6454-real-literals">real type suffix</a>
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hextype</i></term>
        ///         <term>The 'UL', 'Ul', 'uL', 'ul', 'LU', 'Lu', 'lU', or 'lu', which is the
        ///         <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#6453-integer-literals">integral type suffix</a>
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. Real type suffixes are not supported.</term>
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
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="byte"/> data type
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
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# (suffixed with one of
        ///         'D', 'd', 'F', 'f', 'M', 'm', 'L', 'l', 'U', 'u', 'UL', 'ul', 'LU', or 'lu').</term>
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
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value with or without a <c>0x</c> prefix.
        /// For example, "F3" and "0xF3" both parse successfully. The only other flags that can be present in <paramref name="style"/> are
        /// <see cref="NumberStyle.AllowLeadingWhite"/>, <see cref="NumberStyle.AllowTrailingWhite"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// (The <see cref="NumberStyle"/> enumeration has a composite number style, <see cref="NumberStyle.HexNumber"/>, that includes both white space
        /// flags.)
        /// The <see cref="NumberStyle.AllowTypeSpecifier"/> only allows 'L', 'l', 'U', 'u', 'UL', 'ul', 'LU', or 'lu' for hexadecimal numbers.
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. If <paramref name="provider"/> is <c>null</c>, the <see cref="NumberFormatInfo"/> object for the current
        /// culture is used.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(ReadOnlySpan{char}, out byte)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider, out byte result)
        {
            NumberStyleExtensions.ValidateParseStyleInteger(style);
            // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
            // For hex number styles AllowHexSpecifier >> 2 == 0x80 and cancels out MinValue so the check is effectively: (uint)i > byte.MaxValue
            // For integer styles it's zero and the effective check is (uint)(i - MinValue) > byte.MaxValue
            if (DotNetNumber.TryParseInt32(s, style, NumberFormatInfo.GetInstance(provider), out int i) != DotNetNumber.ParsingStatus.OK
                || (uint)(i - sbyte.MinValue - ((int)(style & NumberStyle.AllowHexSpecifier) >> 2)) > byte.MaxValue)
            {
                result = 0;
                return false;
            }
            result = (byte)i;
            return true;
        }

        #endregion TryParse_CharSequence_NumberStyle_IFormatProvider_Byte

        #region ToString

        /// <summary>
        /// Converts the value of the current <see cref="Byte"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this object, which consists of a sequence of digits
        /// that range from 0 to 9 with no leading zeroes.</returns>
        /// <remarks>
        /// The <see cref="ToString()"/> method formats the current instance in the default ("J", or Java)
        /// format of the current culture. If you want to specify a different format, precision, or culture, use the
        /// other overloads of the <see cref="ToString(string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public override string ToString()
        {
            return value.ToString(null, null);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Byte"/> object to its equivalent string representation
        /// using the specified format.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <returns>The string representation of the current <see cref="Byte"/> object, formatted as specified by
        /// the <paramref name="format"/> parameter.</returns>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> includes an unsupported specifier. Supported format specifiers are listed in the Remarks section.
        /// </exception>
        /// <remarks>
        /// The <see cref="ToString(string?)"/> method formats the current instance in
        /// a specified format by using the conventions of the current culture. If you want to specify a different format or culture,
        /// use the other overloads of the <see cref="ToString(string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The format parameter can be either a standard or a custom numeric format string. All standard numeric format strings other
        /// than "R" (or "r") are supported, as are all custom numeric format characters. If format is <c>null</c> or an empty string (""), 
        /// the return value is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// The return value of this function is formatted using the <see cref="NumberFormatInfo"/> object for the thread current culture.
        /// For information about the thread current culture, see <see cref="System.Threading.Thread.CurrentCulture"/>. To provide formatting information
        /// for cultures other than the current culture, call the <see cref="ToString(string?, IFormatProvider?)"/> method.
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public override string ToString(string? format)
        {
            return value.ToString(ConvertFormat(format), null);
        }

        /// <summary>
        /// Converts the numeric value of the current <see cref="Byte"/> object to its equivalent string representation using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value of this object in the format specified by the <paramref name="provider"/> parameter.</returns>
        /// <remarks>
        /// The <see cref="ToString(IFormatProvider?)"/> method formats the current instance in
        /// the default ("J") format of a specified culture. If you want to specify a different format or culture, use the
        /// other overloads of the <see cref="ToString(string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The return value is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// The <paramref name="provider"/> parameter is an object that implements the <see cref="IFormatProvider"/> interface. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object that provides culture-specific information about the format of the string that is
        /// returned by this method. The object that implements <see cref="IFormatProvider"/> can be any of the following:
        /// <list type="bullet">
        ///     <item><description>A <see cref="CultureInfo"/> object that represents the culture whose formatting rules are to be used.</description></item>
        ///     <item><description>A <see cref="NumberFormatInfo"/> object that contains specific numeric formatting information for this value.</description></item>
        ///     <item><description>A custom object that implements <see cref="IFormatProvider"/>.</description></item>
        /// </list>
        /// <para/>
        /// If provider is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained from provider, the return value is formatted
        /// using the <see cref="NumberFormatInfo"/> object for the thread current culture. For information about the thread current culture, see
        /// <see cref="System.Threading.Thread.CurrentCulture"/>.
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public override string ToString(IFormatProvider? provider)
        {
            return value.ToString(null, provider);
        }

        /// <summary>
        /// Converts the value of the current <see cref="Byte"/> object to its equivalent string representation using the specified format
        /// and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the current <see cref="Byte"/> object, formatted as specified by the <paramref name="format"/>
        /// and <paramref name="provider"/> parameters.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> includes an unsupported specifier. Supported format specifiers are listed
        /// in the Remarks section.</exception>
        /// <remarks>
        /// The <see cref="ToString(string?, IFormatProvider?)"/> method formats the current instance in
        /// a specified format of a specified culture. If you want to specify a different format or culture, use the
        /// other overloads of the <see cref="ToString(string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="ToString(string?, IFormatProvider?)"/> method formats a <see cref="Byte"/> value in a specified format
        /// of a specified culture. To format a number by using the default ("J") format of the current culture, call the
        /// <see cref="ToString()"/> method. To format a number by using a specified format of the current culture, call the
        /// <see cref="ToString(string?)"/> method.
        /// <para/>
        /// The <paramref name="format"/> parameter can be either a standard or a custom numeric format string. All standard
        /// numeric format strings other than "R" (or "r") are supported, as are all custom numeric format characters. If
        /// <paramref name="format"/> is <c>null</c> or an empty string (""), the return value of this method is formatted
        /// with the Java numeric format specifier ("J").
        /// <para/>
        /// The <paramref name="provider"/> parameter is an object that implements the <see cref="IFormatProvider"/> interface. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object that provides culture-specific information about the format of the string that is
        /// returned by this method. The object that implements <see cref="IFormatProvider"/> can be any of the following:
        /// <list type="bullet">
        ///     <item><description>A <see cref="CultureInfo"/> object that represents the culture whose formatting rules are to be used.</description></item>
        ///     <item><description>A <see cref="NumberFormatInfo"/> object that contains specific numeric formatting information for this value.</description></item>
        ///     <item><description>A custom object that implements <see cref="IFormatProvider"/>.</description></item>
        /// </list>
        /// <para/>
        /// If provider is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained from provider, the return value is formatted
        /// using the <see cref="NumberFormatInfo"/> object for the thread current culture. For information about the thread current culture, see
        /// <see cref="System.Threading.Thread.CurrentCulture"/>.
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public override string ToString(string? format, IFormatProvider? provider)
        {
            return value.ToString(ConvertFormat(format), provider);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> to convert.</param>
        /// <returns>The string representation of the <paramref name="value"/> parameter, which consists of a sequence of digits
        /// that range from 0 to 9 with no leading zeroes.</returns>
        /// <remarks>
        /// The <see cref="ToString()"/> method formats the current instance in the default ("J", or Java)
        /// format of the current culture. If you want to specify a different format, precision, or culture, use the
        /// other overloads of the <see cref="ToString(byte, string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public static string ToString(byte value)
        {
            return value.ToString(null, null);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent string representation
        /// using the specified format.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> to convert.</param>
        /// <param name="format">A numeric format string.</param>
        /// <returns>The string representation of the <paramref name="value"/> parameter, formatted as specified by
        /// the <paramref name="format"/> parameter.</returns>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> includes an unsupported specifier. Supported format specifiers are listed in the Remarks section.
        /// </exception>
        /// <remarks>
        /// The <see cref="ToString(byte, string?)"/> method formats the current instance in
        /// a specified format by using the conventions of the current culture. If you want to specify a different format or culture,
        /// use the other overloads of the <see cref="ToString(byte, string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The format parameter can be either a standard or a custom numeric format string. All standard numeric format strings other
        /// than "R" (or "r") are supported, as are all custom numeric format characters. If format is <c>null</c> or an empty string (""), 
        /// the return value is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// The return value of this function is formatted using the <see cref="NumberFormatInfo"/> object for the thread current culture.
        /// For information about the thread current culture, see <see cref="System.Threading.Thread.CurrentCulture"/>. To provide formatting information
        /// for cultures other than the current culture, call the <see cref="ToString(byte, string?, IFormatProvider?)"/> method.
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public static string ToString(byte value, string? format)
        {
            return value.ToString(ConvertFormat(format), null);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent string representation using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the specified <paramref name="value"/> in the format specified
        /// by the <paramref name="provider"/> parameter.</returns>
        /// <remarks>
        /// The <see cref="ToString(byte, IFormatProvider?)"/> method formats the current instance in
        /// the default ("J") format of a specified culture. If you want to specify a different format or culture, use the
        /// other overloads of the <see cref="ToString(byte, string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The return value is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// The <paramref name="provider"/> parameter is an object that implements the <see cref="IFormatProvider"/> interface. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object that provides culture-specific information about the format of the string that is
        /// returned by this method. The object that implements <see cref="IFormatProvider"/> can be any of the following:
        /// <list type="bullet">
        ///     <item><description>A <see cref="CultureInfo"/> object that represents the culture whose formatting rules are to be used.</description></item>
        ///     <item><description>A <see cref="NumberFormatInfo"/> object that contains specific numeric formatting information for this value.</description></item>
        ///     <item><description>A custom object that implements <see cref="IFormatProvider"/>.</description></item>
        /// </list>
        /// <para/>
        /// If provider is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained from provider, the return value is formatted
        /// using the <see cref="NumberFormatInfo"/> object for the thread current culture. For information about the thread current culture, see
        /// <see cref="System.Threading.Thread.CurrentCulture"/>.
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public static string ToString(byte value, IFormatProvider? provider)
        {
            return value.ToString(null, provider);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent string representation using the specified format
        /// and culture-specific formatting information.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> to convert.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the specified <paramref name="value"/> parameter, formatted as specified by the <paramref name="format"/>
        /// and <paramref name="provider"/> parameters.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> includes an unsupported specifier. Supported format specifiers are listed
        /// in the Remarks section.</exception>
        /// <remarks>
        /// The <see cref="ToString(byte, string?, IFormatProvider?)"/> method formats the current instance in
        /// a specified format of a specified culture. If you want to specify a different format or culture, use the
        /// other overloads of the <see cref="ToString(byte, string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(byte, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(byte, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="ToString(byte, string?, IFormatProvider?)"/> method formats a <see cref="byte"/> value in a specified format
        /// of a specified culture. To format a number by using the default ("J") format of the current culture, call the
        /// <see cref="ToString(byte)"/> method. To format a number by using a specified format of the current culture, call the
        /// <see cref="ToString(byte, string?)"/> method.
        /// <para/>
        /// The <paramref name="format"/> parameter can be either a standard or a custom numeric format string. All standard
        /// numeric format strings other than "R" (or "r") are supported, as are all custom numeric format characters. If
        /// <paramref name="format"/> is <c>null</c> or an empty string (""), the return value of this method is formatted
        /// with the Java numeric format specifier ("J").
        /// <para/>
        /// The <paramref name="provider"/> parameter is an object that implements the <see cref="IFormatProvider"/> interface. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object that provides culture-specific information about the format of the string that is
        /// returned by this method. The object that implements <see cref="IFormatProvider"/> can be any of the following:
        /// <list type="bullet">
        ///     <item><description>A <see cref="CultureInfo"/> object that represents the culture whose formatting rules are to be used.</description></item>
        ///     <item><description>A <see cref="NumberFormatInfo"/> object that contains specific numeric formatting information for this value.</description></item>
        ///     <item><description>A custom object that implements <see cref="IFormatProvider"/>.</description></item>
        /// </list>
        /// <para/>
        /// If provider is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained from provider, the return value is formatted
        /// using the <see cref="NumberFormatInfo"/> object for the thread current culture. For information about the thread current culture, see
        /// <see cref="System.Threading.Thread.CurrentCulture"/>.
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public static string ToString(byte value, string? format, IFormatProvider? provider)
        {
            return value.ToString(ConvertFormat(format), provider);
        }

        #endregion ToString

        #region TryFormat

        /// <summary>
        /// Tries to format the value of the current 8-bit unsigned integer number instance into the provided span of characters.
        /// </summary>
        /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters that were written in
        /// <paramref name="destination"/>.</param>
        /// <param name="format">A span containing the characters that represent a standard or custom format string that
        /// defines the acceptable format for <paramref name="destination"/>.</param>
        /// <param name="provider">An optional object that supplies culture-specific formatting information for
        /// <paramref name="destination"/>.</param>
        /// <returns><c>true</c> if the formatting was successful; otherwise, <c>false</c>.</returns>
        public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            return DotNetNumber.TryFormatUInt32(value, format, provider, destination, out charsWritten);
        }

        /// <summary>
        /// Tries to format the value of the <paramref name="value"/> into the provided span of characters.
        /// </summary>
        /// <param name="value">The 8-bit unsigned integer number to format.</param>
        /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters that were written in
        /// <paramref name="destination"/>.</param>
        /// <param name="format">A span containing the characters that represent a standard or custom format string that
        /// defines the acceptable format for <paramref name="destination"/>.</param>
        /// <param name="provider">An optional object that supplies culture-specific formatting information for
        /// <paramref name="destination"/>.</param>
        /// <returns><c>true</c> if the formatting was successful; otherwise, <c>false</c>.</returns>
        public static bool TryFormat(byte value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            return DotNetNumber.TryFormatUInt32(value, format, provider, destination, out charsWritten);
        }

        #endregion TryFormat

        #region GetInstance (ValueOf)

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its <see cref="Byte"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing the characters representing the value to convert. The string is interpreted using the <see cref="NumberStyle.Integer"/> style.
        /// For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>. If provider is <c>null</c>,
        /// the thread current culture is used.</param>
        /// <returns>An immutable <see cref="Byte"/> instance that is equivalent to the number contained in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> is not of the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number less than <see cref="sbyte.MinValue"/> or <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <remarks>
        /// The s parameter contains a number of the form:
        /// <para/>
        /// [ws][sign]digits[ws]
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
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using the <see cref="NumberStyle.Integer"/> style. In addition to the byte value's
        /// decimal digits, only leading and trailing spaces together with a leading sign are allowed. To explicitly define the style elements
        /// together with the culture-specific formatting information that can be present in <paramref name="s"/>, use the
        /// <see cref="GetInstance(string, NumberStyle, IFormatProvider?)"/> method.
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="GetInstance(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
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
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        public static Byte GetInstance(string s, IFormatProvider? provider)
        {
            return GetInstance(Parse(s, provider));
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its <see cref="Byte"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing the characters representing the value to convert. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific information about the format of <paramref name="s"/>. If provider is <c>null</c>,
        /// the thread current culture is used.</param>
        /// <returns>An immutable <see cref="Byte"/> instance that is equivalent to the number contained in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> is not of the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> represents a number less than <see cref="sbyte.MinValue"/> or <see cref="byte.MaxValue"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> includes non-zero, fractional digits.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <remarks>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or the positive sign) that are allowed in the
        /// <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/>
        /// enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/> parameter may include the following elements:
        /// <para/>
        /// [ws][$][sign]digits[.fractional-digits][e[sign]exponential-digits][ws]
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
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="byte"/> data type
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
        /// the format of <paramref name="s"/>. When the <see cref="GetInstance(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
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
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/>
        /// <seealso cref="Parse(string, NumberStyle, IFormatProvider?)"/>
        public static Byte GetInstance(string s, NumberStyle style, IFormatProvider? provider)
        {
            return GetInstance(Parse(s, style, provider));
        }

        /// <summary>
        /// Parses the <see cref="string"/> argument as a <see cref="Byte"/> in the specified <paramref name="radix"/>. 
        /// <para/>
        /// Usage Note: The conversion operation is similar to the <see cref="Convert.ToByte(string?, int)"/> method. It differs in that
        /// it allows the use of the ASCII character \u002d ('-') or \u002B ('+') in any <paramref name="radix"/>.
        /// <para/>
        /// Supports any BMP (Basic Multilingual Plane) or SMP (Supplementary Mulitlingual Plane) digit as defined by Unicode 10.0.
        /// <para/>
        /// For compatibility with Java, this method successfully parses values from <see cref="sbyte.MinValue"/> to <see cref="byte.MaxValue"/>,
        /// but the value stored is type <see cref="byte"/> and may need to be converted to <see cref="sbyte"/> depending on how it is used.
        /// <para/>
        /// This is the same operation as Byte.valueOf(string) in the JDK when specifying a <paramref name="radix"/> of 10, or
        /// Byte.valueOf(string, int) for any valid <paramref name="radix"/>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="byte"/> representation to be parsed.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>An immutable <see cref="Byte"/> instance that is equivalent to the number in <paramref name="s"/>, or 0 (zero) if
        /// <paramref name="s"/> is <c>null</c>.</returns>
        /// <remarks>
        /// If <paramref name="radix"/> is 16, you can prefix the number specified by the <paramref name="s"/> parameter with "0x" or "0X".
        /// <para/>
        /// To specify a negative value for base (radix) 10 numeric representations, use the ASCII character \u002d ('-').
        /// <para/>
        /// For any other <paramref name="radix"/>,  negative values may either be specified with ASCII character \u002d ('-')
        /// (as in Java) or by specifying the two's complement representation (as in .NET), but not both.
        /// In the latter case, the highest-order binary bit of a long integer (bit 7) is interpreted as the sign bit.
        /// As a result, it is possible to write code in which a non-base 10 number that is out of the range of the <see cref="byte"/>
        /// data type is converted to a <see cref="byte"/> value without the method throwing an exception.
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
        /// <paramref name="s"/> represents a number that is less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.
        /// </exception>
        /// <seealso cref="TryParse(string?, int, out byte)"/>
        /// <seealso cref="Parse(string?, int)"/>
        public static Byte GetInstance(string s, int radix)
        {
            return GetInstance(Parse(s, radix));
        }

        /// <summary>
        /// Returns an immutable <see cref="Byte"/> instance for the specified <paramref name="value"/>.
        /// <para/>
        /// Usage Note: This is the same operation as Byte.valueOf() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value the returned instance represents.</param>
        /// <returns>An immutable <see cref="Byte"/> instance containing the <paramref name="value"/>,
        /// which may be retrieved from a cache.</returns>
        public static Byte GetInstance(byte value)
        {
            lock (Cache)
            {
                int idx = value - byte.MinValue;
                Byte result = Cache[idx];
                return result ?? (Cache[idx] = new Byte(value));
            }
        }

        #endregion GetInstance (ValueOf)

        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator byte(Byte value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Byte(byte value) => GetInstance(value);

        #region IConvertible implementation

        /// <inheritdoc/>
        public override byte ToByte()
        {
            return value;
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
        /// Returns the <see cref="TypeCode"/> for value type <see cref="byte"/>.
        /// </summary>
        /// <returns>The enumerated constant, <see cref="TypeCode.Byte"/>.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Byte;
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
            return value;
        }

        short IConvertible.ToInt16(IFormatProvider? provider)
        {
            return Convert.ToInt16(value);
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
            throw new InvalidCastException(J2N.SR.Format(SR.InvalidCast_FromTo, "Byte", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider? provider)
        {
            return /*Convert.*/DefaultToType((IConvertible)this.value, type, provider);
        }

        #endregion IConvertible implementation
    }
}
