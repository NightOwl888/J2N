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
    ///     <item><description><see cref="float"/> and <see cref="double"/> values are displayed with a minimum of 1 fractional digit with a variable number
    ///     of fractional digits and .</description></item>
    ///     <item><description><see cref="float"/> and <see cref="double"/> negative zeros are displayed as -0.0 and other data types are patched to display
    ///     negative zero on all .NET target platforms lower than .NET Core 3.0.</description></item>
    ///     <item><description><see cref="bool"/> values are lowercased to <c>"true"</c> and <c>"false"</c>, rather than the default .NET <c>"True"</c> and <c>"False"</c>.</description></item>
    ///     <item><description><see cref="ICollection{T}"/> and <see cref="IDictionary{TKey, TValue}"/> types are formatted to include all of their element
    ///     values (and nested collection values).</description></item>
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

#pragma warning disable IDE0044 // Add readonly modifier
        private IFormatProvider? formatProvider; // NOTE: This needs to be [Serializable] to support serialization. Note on .NET Core serialization has been dropped on these implementations.
#pragma warning restore IDE0044 // Add readonly modifier

        /// <summary>
        /// Initializes a new instance of <see cref="StringFormatter"/>.
        /// </summary>
        public StringFormatter()
            : this(CultureType.CurrentCulture)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="StringFormatter"/> with the specified <paramref name="culture"/>.
        /// <para/>
        /// <b>NOTE:</b> This overload only supports serialization of built-in cultures. If you require serialization and have a custom implementation,
        /// you will need to provide a serializable wrapper to the <see cref="StringFormatter.StringFormatter(IFormatProvider)"/> constructor. Note that
        /// on .NET Core and newer .NET platforms serialization is not supported for <see cref="CultureInfo"/>, <see cref="NumberFormatInfo"/> and <see cref="DateTimeFormatInfo"/>.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> that specifies the culture-specific rules that will be used for formatting.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="culture"/> is <c>null</c>.</exception>
        public StringFormatter(CultureInfo culture)
            : this(CultureType.CustomCulture)
        {
            this.culture = culture ?? throw new ArgumentNullException(nameof(culture));
            this.cultureSymbol = this.culture.Name.ToCharArray(); // For deserialization
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StringFormatter"/> with the specified <paramref name="formatProvider"/>.
        /// <para/>
        /// <b>NOTE:</b> If binary serialization is required, the type passed must be annotated with the <see cref="SerializableAttribute"/> and otherwise be
        /// setup for serialization and deserialization. However, note that on .NET Core and newer .NET platforms serialization is not supported for
        /// <see cref="CultureInfo"/>, <see cref="NumberFormatInfo"/> and <see cref="DateTimeFormatInfo"/>.
        /// </summary>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that specifies the culture-specific rules that will be used for formatting.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="formatProvider"/> is <c>null</c>.</exception>
        public StringFormatter(IFormatProvider formatProvider)
            : this(CultureType.IFormatProvider)
        {
            this.formatProvider = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
        }

        internal StringFormatter(CultureType cultureType)
        {
            this.cultureType = cultureType;
        }

        /// <summary>
        /// Gets the culture of the current instance.
        /// </summary>
        [Obsolete("Store the CultureInfo in your subclass from the CultureInfo constructor. Note that .NET doesn't provide a reliable way to get from IFormatProvider > CultureInfo, so this will return the current cultue when using the IFormatProvider constructor.")]
        protected virtual CultureInfo Culture
        {
            get
            {
                return cultureType switch
                {
                    CultureType.CustomCulture => culture!,
                    CultureType.InvariantCulture => CultureInfo.InvariantCulture,
                    CultureType.CurrentCulture => CultureInfo.CurrentCulture,
                    CultureType.CurrentUICulture => CultureInfo.CurrentUICulture,
                    _ => CultureInfo.CurrentCulture,
                };
            }
        }

        /// <summary>
        /// Gets the <see cref="IFormatProvider"/> of the current instance.
        /// </summary>
        private IFormatProvider FormatProvider
        {
            get
            {
                return cultureType switch
                {
                    CultureType.IFormatProvider => formatProvider!,
                    CultureType.CustomCulture => culture!,
                    CultureType.InvariantCulture => CultureInfo.InvariantCulture,
                    CultureType.CurrentCulture => CultureInfo.CurrentCulture,
                    CultureType.CurrentUICulture => CultureInfo.CurrentUICulture,
                    _ => CultureInfo.CurrentCulture,
                };
            }
        }

        internal enum CultureType
        {
            CurrentCulture,
            CurrentUICulture,
            InvariantCulture,
            CustomCulture,
            IFormatProvider
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
                return NumberFormatInfo.GetInstance(FormatProvider);
            if (typeof(DateTimeFormatInfo).Equals(formatType))
                return DateTimeFormatInfo.GetInstance(FormatProvider);

            return null;
        }

        /// <summary>
        /// Formats the <paramref name="arg"/> with rules similar to Java.
        /// <list type="bullet">
        ///     <item><description><see cref="float"/> and <see cref="double"/> values are displayed with a minimum of 1 fractional digit with a variable number
        ///     of fractional digits and .</description></item>
        ///     <item><description><see cref="float"/> and <see cref="double"/> negative zeros are displayed as -0.0 and other data types are patched to display
        ///     negative zero on all .NET target platforms lower than .NET Core 3.0.</description></item>
        ///     <item><description><see cref="bool"/> values are lowercased to <c>"true"</c> and <c>"false"</c>, rather than the default .NET <c>"True"</c> and <c>"False"</c>.</description></item>
        ///     <item><description><see cref="ICollection{T}"/> and <see cref="IDictionary{TKey, TValue}"/> types are formatted to include all of their element
        ///     values (and nested collection values).</description></item>
        /// </list>
        /// </summary>
        /// <param name="format">The format. To utilize this formatter in <see cref="object.ToString()"/> overloads of value types, specify a standard or
        /// custom format (such as <c>"J"</c>), otherwise this formatter will be bypassed.</param>
        /// <param name="arg">The object to format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A string representing the formatted value, or <c>null</c> when this formatter is not applicable.</returns>
        public virtual string Format(string? format, object? arg, IFormatProvider? formatProvider)
        {
            if (arg is null)
                return "null";
            else if (arg is double d)
                return DotNetNumber.FormatDouble(d, format, formatProvider);
            else if (arg is float f)
                return DotNetNumber.FormatSingle(f, format, formatProvider);
            else if (arg is bool b)
                return FormatBoolean(b);
            else if (arg is IStructuralFormattable sf)
                return sf.ToString("{0}", this);

            // J2N: Technically, none of the other numeric types are supported. But,
            // we convert any request for "J" or "j" format into "G" or "g" (without any
            // precision) respectfully. This will allow the blanket use of "J" and "j" for these
            // types without throwing exceptions and leaves the door open for supporting that
            // format should the need arise in the future.
            else if (arg is int i)
                return J2N.Numerics.Int32.ToString(i, format, formatProvider);
            else if (arg is byte bt)
                return J2N.Numerics.Byte.ToString(bt, format, formatProvider);
            else if (arg is long l)
                return J2N.Numerics.Int64.ToString(l, format, formatProvider);
            else if (arg is short s)
                return J2N.Numerics.Int16.ToString(s, format, formatProvider);
            else if (arg is sbyte sb)
                return J2N.Numerics.SByte.ToString(sb, format, formatProvider);

//            // After this point, we don't have any implementations so we make the call
//            // to ConvertFormat() explicitly.
//            else if (arg is decimal dec)
//                return dec.ToString(Number.ConvertFormat(format), formatProvider);
//            else if (arg is ushort us)
//                return us.ToString(Number.ConvertFormat(format), formatProvider);
//            else if (arg is uint ui)
//                return ui.ToString(Number.ConvertFormat(format), formatProvider);
//            else if (arg is ulong ul)
//                return ul.ToString(Number.ConvertFormat(format), formatProvider);
//            else if (arg is System.Numerics.BigInteger bi)
//                return bi.ToString(Number.ConvertFormat(format), formatProvider);
//#if NET5_0_OR_GREATER
//            else if (arg is nint ni)
//                return ni.ToString(Number.ConvertFormat(format), formatProvider);
//            else if (arg is nuint nui)
//                return nui.ToString(Number.ConvertFormat(format), formatProvider);
//            else if (arg is Half h)
//                return h.ToString(Number.ConvertFormat(format), formatProvider);
//#endif

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
            if (cultureType == CultureType.CustomCulture)
            {
                this.culture = CultureInfo.GetCultureInfo(new string(this.cultureSymbol!)
#if FEATURE_CULTUREINFO_PREDEFINEDONLY
                    , predefinedOnly: true // We only support predefined cultures for serialization. End users must provide their own serializable IFormatProvider implementation for other types.
#endif
                    );
            }

        }
#endif

    }
}
