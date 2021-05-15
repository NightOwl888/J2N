#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using J2N.Collections;
using J2N.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace J2N.Text
{
    /// <summary>
    /// Provides number and boolean formatting rules similar to how they are done in Java.
    /// <list type="bullet">
    ///     <item><description><see cref="float"/> and <see cref="double"/> values are displayed with a minimum of 1 fractional digit.</description></item>
    ///     <item><description><see cref="float"/> values with fractional digits are displayed to 7 decimals.</description></item>
    ///     <item><description><see cref="float"/> and <see cref="double"/> negative zeros are displayed with the same rules as the
    ///         current culture's <see cref="NumberFormatInfo.NumberNegativePattern"/> and <see cref="NumberFormatInfo.NegativeSign"/>.</description></item>
    ///     <item><description><see cref="bool"/> values are lowercased to <c>true</c> and <c>false</c>, rather than the default .NET <c>True</c> and <c>False</c>.</description></item>
    ///     <item><description>Collection and array types are formatted to display their values (and nested collection values).</description></item>
    /// </list>
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class StringFormatter : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// Gets a <see cref="StringFormatter"/> that uses the culture from the current thread to format values.
        /// </summary>
        public static StringFormatter CurrentCulture { get; } = new StringFormatter(CultureType.CurrentCulture);

        /// <summary>
        /// Gets a <see cref="StringFormatter"/> that uses the UI culture from the current thread to format values.
        /// </summary>
        public static StringFormatter CurrentUICulture { get; } = new StringFormatter(CultureType.CurrentUICulture);

        ///// <summary>
        ///// Gets a <see cref="StringFormatter"/> that uses the default culture for threads in the current application domain to format values.
        ///// </summary>
        //public static StringFormatter DefaultThreadCurrentCulture { get; } = new StringFormatter(CultureType.DefaultThreadCurrentCulture);

        ///// <summary>
        ///// Gets a <see cref="StringFormatter"/> that uses the default UI culture for threads in the current application domain to format values.
        ///// </summary>
        //public static StringFormatter DefaultThreadCurrentUICulture { get; } = new StringFormatter(CultureType.DefaultThreadCurrentUICulture);

        /// <summary>
        /// Gets a <see cref="StringFormatter"/> that uses the invariant culture to format values.
        /// This is the default setting in Java.
        /// </summary>
        public static StringFormatter InvariantCulture { get; } = new StringFormatter(CultureType.InvariantCulture);

        private readonly char[]? cultureSymbol; // For deserialization
#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private CultureInfo? culture; // not readonly for deserialization
        private readonly CultureType? cultureType;

        /// <summary>
        /// Initializes a new instance of <see cref="StringFormatter"/>.
        /// </summary>
        public StringFormatter()
            : this(CultureType.CurrentCulture)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="StringFormatter"/> with the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> that specifies the culture-specific rules that will be used for formatting.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="culture"/> is <c>null</c>.</exception>
        public StringFormatter(CultureInfo culture)
            : this(CultureType.CustomCulture)
        {
            this.culture = culture ?? throw new ArgumentNullException(nameof(culture));
            this.cultureSymbol = this.culture.Name.ToCharArray(); // For deserialization
        }

        internal StringFormatter(CultureType cultureType)
        {
            this.cultureType = cultureType;
        }

        /// <summary>
        /// Gets the culture of the current instance.
        /// </summary>
        protected virtual CultureInfo Culture
        {
            get
            {
                switch (cultureType)
                {
                    case CultureType.CustomCulture:
                        return culture!;
                    case CultureType.InvariantCulture:
                        return CultureInfo.InvariantCulture;
                    case CultureType.CurrentCulture:
                        return CultureInfo.CurrentCulture;
                    case CultureType.CurrentUICulture:
                        return CultureInfo.CurrentUICulture;
#if FEATURE_CULTUREINFO_DEFAULTTHREADCURRENTCULTURE
                    case CultureType.DefaultThreadCurrentCulture:
                        return CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.CurrentCulture;
#endif
#if FEATURE_CULTUREINFO_DEFAULTTHREADCURRENTUICULTURE
                    case CultureType.DefaultThreadCurrentUICulture:
                        return CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;
#endif
                    default:
                        return CultureInfo.CurrentCulture;
                }
            }
        }

        internal enum CultureType
        {
            CurrentCulture,
            CurrentUICulture,
            DefaultThreadCurrentCulture,
            DefaultThreadCurrentUICulture,
            InvariantCulture,
            CustomCulture
        }

        /// <summary>
        /// Gets the format provider.
        /// </summary>
        /// <param name="formatType">The format type that is requested.</param>
        /// <returns>The requested format provider, or <c>null</c> if it is not applicable.</returns>
        public virtual object? GetFormat(Type? formatType)
        {
            if (typeof(ICustomFormatter).Equals(formatType))
                return this;
            if (typeof(NumberFormatInfo).Equals(formatType))
                return GetNumberFormatInfo(Culture);
            if (typeof(DateTimeFormatInfo).Equals(formatType))
                return GetDateTimeFormatInfo(Culture);

            return null;
        }

        /// <summary>
        /// Formats the <paramref name="arg"/> with rules similar to Java.
        /// <list type="bullet">
        ///     <item><description><see cref="float"/> and <see cref="double"/> values are displayed with a minimum of 1 fractional digit.</description></item>
        ///     <item><description><see cref="float"/> values with fractional digits are displayed to 7 decimals.</description></item>
        ///     <item><description><see cref="float"/> and <see cref="double"/> negative zeros are displayed with the same rules as the
        ///         current culture's <see cref="NumberFormatInfo.NumberNegativePattern"/> and <see cref="NumberFormatInfo.NegativeSign"/>.</description></item>
        ///     <item><description><see cref="bool"/> values are lowercased to <c>"true"</c> and <c>"false"</c>, rather than the default .NET "True" and "False".</description></item>
        ///     <item><description><see cref="ICollection{T}"/> and <see cref="IDictionary{TKey, TValue}"/> types are formatted to include all of their element values.</description></item>
        /// </list>
        /// </summary>
        /// <param name="format">The format. To utilize this formatter, use <c>"{0}"</c> or <c>"{0:J}"</c>, otherwise it will be bypassed.</param>
        /// <param name="arg">The object to format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A string representing the formatted value, or <c>null</c> when this formatter is not applicable.</returns>
        public virtual string Format(string? format, object? arg, IFormatProvider? formatProvider)
        {
            if (!this.Equals(formatProvider))
                return null!; // Not handled by this formatter

            if (!(string.IsNullOrEmpty(format) || format == "J" || format == "j"))
                return null!; // Not handled by this formatter

            if (arg is null)
                return "null";
            else if (arg is double d)
                return FormatDouble(d, GetNumberFormatInfo(Culture));
            else if (arg is float f)
                return FormatSingle(f, GetNumberFormatInfo(Culture));
            else if (arg is bool b)
                return FormatBoolean(b);
            else if (arg is IStructuralFormattable sf)
                return sf.ToString("{0}", this);

            var argType = arg.GetType();
            if (argType.IsArray)
            {
                return Arrays.ToString((Array)arg, this);
            }
            else if (
                argType.ImplementsGenericInterface(typeof(ICollection<>)) ||
                argType.ImplementsGenericInterface(typeof(IDictionary<,>)))
            {
                return CollectionUtil.ToStringImpl(arg, argType, this);
            }

            return null!; // Not handled by this formatter
        }

        private NumberFormatInfo? GetNumberFormatInfo(IFormatProvider provider)
        {
            return provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
        }

        private DateTimeFormatInfo? GetDateTimeFormatInfo(IFormatProvider provider)
        {
            return provider.GetFormat(typeof(DateTimeFormatInfo)) as DateTimeFormatInfo;
        }

        private static string FormatNegativeZero(NumberFormatInfo? numberFormat)
        {
            if (numberFormat is null)
                numberFormat = CultureInfo.InvariantCulture.NumberFormat; // Default to invariant if we cannot resolve NumberFormatInfo

            string sep = numberFormat.NumberDecimalSeparator;
            switch (numberFormat.NumberNegativePattern)
            {
                case 0: // (1,234.00)
                    return $"(0{sep}0)";
                case 2: // - 1,234.00
                    return numberFormat.NegativeSign + $" 0{sep}0";
                case 3: // 1,234.00-
                    return $"0{sep}0" + numberFormat.NegativeSign;
                case 4: // 1,234.00 -
                    return $"0{sep}0 " + numberFormat.NegativeSign;
                default: // (1): -1,234.00
                    return numberFormat.NegativeSign + $"0{sep}0";
            }
        }

        private static string FormatDouble(double d, NumberFormatInfo? numberFormat)
        {
            if (numberFormat is null || numberFormat.Equals(CultureInfo.InvariantCulture.NumberFormat))
            {
                //return J2N.Numerics.FloatingDecimal.ToJavaFormatString(d);
                return RyuDouble.DoubleToString(d, RoundingMode.Conservative); // J2N: Conservative rounding is closer to the JDK
            }

            if ((long)d == d)
            {
                // Special case: negative zero
                if (d.IsNegativeZero())
                    return FormatNegativeZero(numberFormat);

                // Special case: When we have an integer value,
                // the standard .NET formatting removes the decimal point
                // and everything to the right. But we need to always
                // have at least 1 decimal place to match Java.
                return d.ToString("0.0", numberFormat);
            }

            return d.ToString("R", numberFormat);
        }

        private static string FormatSingle(float f, NumberFormatInfo? numberFormat)
        {
            if (numberFormat is null || numberFormat.Equals(CultureInfo.InvariantCulture.NumberFormat))
            {
                //return J2N.Numerics.FloatingDecimal.ToJavaFormatString(f);
                //return J2N.Numerics.RyuConversion.FloatToString(f);

                return RyuFloat.FloatToString(f, RoundingMode.Conservative); // J2N: Conservative rounding is closer to the JDK
            }

            if ((int)f == f)
            {
                // Special case: negative zero
                if (f.IsNegativeZero())
                    return FormatNegativeZero(numberFormat);

                // Special case: When we have an integer value,
                // the standard .NET formatting removes the decimal point
                // and everything to the right. But we need to always
                // have at least 1 decimal place to match Java.
                return f.ToString("0.0", numberFormat);
            }

            // J2N NOTE: Although the MSDN documentation says that 
            // round-trip on float will be limited to 7 decimals, it appears
            // not to be the case. Also, when specifying "0.0######", we only
            // get a result to 6 decimal places maximum. So, we must round before
            // doing a round-trip format to guarantee 7 decimal places.
            return Math.Round(f, 7).ToString("R", numberFormat);
        }

        private static string FormatBoolean(bool b)
        {
            return b ? "true" : "false";
        }

#if FEATURE_SERIALIZABLE
        [System.Runtime.Serialization.OnDeserialized]
        internal void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
        {
            // We only need to deserialize custom cultures. Note that if it is not a built-in
            // culture, this will fail.
            // J2N TODO: On newer .NET platforms, there is an overload that accepts a predefinedOnly parameter
            // that when set to false allows retrieving made-up cultures. Need to investigate.
            if (cultureType == CultureType.CustomCulture)
                this.culture = CultureInfo.GetCultureInfo(new string(this.cultureSymbol!));
        }
#endif

    }
}
