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

using J2N.Text;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace J2N.Numerics
{
    /// <summary>
    /// The abstract superclass of the classes which represent numeric base types
    /// (that is <see cref="System.Byte"/>, <see cref="System.SByte"/>, <see cref="System.Int16"/>, <see cref="System.Int32"/>,
    /// <see cref="System.Int64"/>, <see cref="System.Single"/>, and <see cref="System.Double"/>).
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class Number : IFormattable, ISpanFormattable
    {
        // From System.Convert

        // A typeof operation is fairly expensive (does a system call), so we'll cache these here
        // statically.  These are exactly lined up with the TypeCode, eg. ConvertType[TypeCode.Int16]
        // will give you the type of an short.
        internal static readonly Type?[] ConvertTypes = {
            null, //Type.GetType("System.Empty"), //typeof(System.Empty), // J2N: This type is internal, but isn't currently being used, anyway, so rather than using Reflection, we are setting to null for now
            typeof(object),
            typeof(System.DBNull),
            typeof(bool),
            typeof(char),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(object), // TypeCode is discontinuous so we need a placeholder.
            typeof(string)
        };

        // Need to special case Enum because typecode will be underlying type, e.g. Int32
        private static readonly Type EnumType = typeof(Enum);


        /// <summary>
        /// Returns this object's value as a <see cref="byte"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="byte"/>.
        /// <para/>
        /// Usage Note: This is similar to byteValue() in the JDK, however for an exact match
        /// use <see cref="ToSByte()"/>.
        /// </summary>
        /// <returns>the primitive <see cref="byte"/> value of this object.</returns>
        public virtual byte ToByte()
        {
            return (byte)ToInt32();
        }

        /// <summary>
        /// Returns this object's value as a <see cref="sbyte"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="sbyte"/>.
        /// <para/>
        /// Usage Note: This is the equivalent operation of byteValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="sbyte"/> value of this object.</returns>
        [CLSCompliant(false)]
        public virtual sbyte ToSByte()
        {
            return (sbyte)ToInt32();
        }

        /// <summary>
        /// Returns this object's value as a <see cref="double"/>. Might involve rounding.
        /// <para/>
        /// Usage Note: This is the equivalent operation of doubleValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="double"/> value of this object.</returns>
        public abstract double ToDouble();

        /// <summary>
        /// Returns this object's value as a <see cref="float"/>. Might involve rounding.
        /// <para/>
        /// Usage Note: This is the equivalent operation of floatValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="float"/> value of this object.</returns>
        public abstract float ToSingle();

        /// <summary>
        /// Returns this object's value as an <see cref="int"/>. Might involve rounding and/or
        /// truncating the value, so it fits into an <see cref="int"/>.
        /// <para/>
        /// Usage Note: This is the equivalent operation of intValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="int"/> value of this object.</returns>
        public abstract int ToInt32();

        /// <summary>
        /// Returns this object's value as a <see cref="long"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="long"/>.
        /// <para/>
        /// Usage Note: This is the equivalent operation of longValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="long"/> value of this object.</returns>
        public abstract long ToInt64();

        /// <summary>
        /// Returns this object's value as a <see cref="short"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="short"/>.
        /// <para/>
        /// Usage Note: This is the equivalent operation of shortValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="short"/> value of this object.</returns>
        public virtual short ToInt16()
        {
            return (short)ToInt32();
        }

        #region ToString

        /// <summary>
        /// Converts the value of the current object to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this object.</returns>
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
        ///         <term><see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(string?, IFormatProvider?)"/></term>
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
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Converts the value of the current object to its equivalent string representation
        /// using the specified format.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <returns>The string representation of the current object, formatted as specified by
        /// the <paramref name="format"/> parameter.</returns>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> includes an unsupported specifier. Supported format specifiers are defined by the derived type.
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
        ///         <term><see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The format parameter can be either a standard or a custom numeric format string. If format is <c>null</c> or an empty string (""), 
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
        public virtual string ToString(string? format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Converts the numeric value of the current object to its equivalent string representation using the
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
        ///         <term><see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(string?, IFormatProvider?)"/></term>
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
        public virtual string ToString(IFormatProvider? provider)
        {
            return ToString(null, provider);
        }

        /// <summary>
        /// Converts the value of the current object to its equivalent string representation using the specified format
        /// and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the current object, formatted as specified by the <paramref name="format"/>
        /// and <paramref name="provider"/> parameters.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> includes an unsupported specifier.
        /// Supported format specifiers are defined by the derived type.</exception>
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
        ///         <term><see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(string?)"/></term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="ToString(string?, IFormatProvider?)"/> method formats a numeric value in a specified format
        /// of a specified culture. To format a number by using the default ("J") format of the current culture, call the
        /// <see cref="ToString()"/> method. To format a number by using a specified format of the current culture, call the
        /// <see cref="ToString(string?)"/> method.
        /// <para/>
        /// The <paramref name="format"/> parameter can be either a standard or a custom numeric format string. If
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
        public abstract string ToString(string? format, IFormatProvider? provider);

        #endregion ToString

        #region TryFormat

        /// <summary>
        /// Tries to format the value of the current number instance into the provided span of characters.
        /// <para/>
        /// <b>Note to Inheritors:</b> This method implementation returns the value of <see cref="ToString(string?, IFormatProvider?)"/>
        /// which will cause a heap allocation. It is highly recommended to override this method to provide an optimal implementation.
        /// </summary>
        /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters that were written in
        /// <paramref name="destination"/>.</param>
        /// <param name="format">A span containing the characters that represent a standard or custom format string that
        /// defines the acceptable format for <paramref name="destination"/>.</param>
        /// <param name="provider">An optional object that supplies culture-specific formatting information for
        /// <paramref name="destination"/>.</param>
        /// <returns><c>true</c> if the formatting was successful; otherwise, <c>false</c>.</returns>
        public virtual bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            charsWritten = 0;
            string result = ToString(format.ToString(), provider);
#if NET6_0_OR_GREATER
            bool success = result.TryCopyTo(destination);
            if (success)
            {
                charsWritten = result.Length;
            }
            return success;
#else
            if (result.Length > destination.Length)
            {
                return false;
            }
            for (int i = 0; i < result.Length; i++)
            {
                destination[i] = result[i];
                charsWritten++;
            }
            return true;
#endif

        }

#endregion

        // From System.Convert
        internal static object DefaultToType(IConvertible value, Type targetType, IFormatProvider? provider)
        {
            Debug.Assert(value != null, "[Convert.DefaultToType]value!=null");
            ThrowHelper.ThrowIfNull(targetType, ExceptionArgument.targetType);

            if (ReferenceEquals(value!.GetType(), targetType))
            {
                return value;
            }

            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Boolean]))
                return value.ToBoolean(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Char]))
                return value.ToChar(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.SByte]))
                return value.ToSByte(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Byte]))
                return value.ToByte(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Int16]))
                return value.ToInt16(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.UInt16]))
                return value.ToUInt16(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Int32]))
                return value.ToInt32(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.UInt32]))
                return value.ToUInt32(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Int64]))
                return value.ToInt64(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.UInt64]))
                return value.ToUInt64(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Single]))
                return value.ToSingle(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Double]))
                return value.ToDouble(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Decimal]))
                return value.ToDecimal(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.DateTime]))
                return value.ToDateTime(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.String]))
                return value.ToString(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Object]))
                return (object)value;
            // Need to special case Enum because typecode will be underlying type, e.g. Int32
            if (ReferenceEquals(targetType, EnumType))
                return (Enum)value;
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.DBNull]))
                throw new InvalidCastException(SR.InvalidCast_DBNull);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Empty]))
                throw new InvalidCastException(SR.InvalidCast_Empty);

            throw new InvalidCastException(J2N.SR.Format(SR.InvalidCast_FromTo, value.GetType().FullName, targetType.FullName));
        }

        /// <summary>
        /// Converts "J" format to "G" and removes the precision specifier.
        /// This is just so we can pass through the value to the built-in
        /// .NET ToString() methods without them complaining. Someday this
        /// format might morph into something else, in which case we will
        /// remove this method. This is only intended for integral types
        /// that do not actually support the "J" format.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string? ConvertFormat(string? format)
        {
            if (string.IsNullOrEmpty(format))
                return null;

            char fmt = format![0];
            // Remove any precision or other characters that are passed
            // as we will ignore them
            if (fmt == 'J')
                return "G";
            if (fmt == 'j')
                return "g";

            return format;
        }
    }
}
