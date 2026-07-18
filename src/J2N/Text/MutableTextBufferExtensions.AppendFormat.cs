#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using J2N.CodeGeneration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace J2N.Text
{
    internal static partial class MutableTextBufferExtensions
    {
        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">An object to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 1.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items. The index of the format items must be 0,
        /// to correspond to <paramref name="arg0"/>, the single object in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of <paramref name="arg0"/>.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="arg0"/> represents the object to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of <paramref name="arg0"/>. If the format item includes <c>formatString</c>
        /// and <paramref name="arg0"/> implements the <see cref="IFormattable"/> interface, then <c>arg0.ToString(formatString, null)</c>
        /// defines the formatting. Otherwise, <c>arg0.ToString()</c> defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <paramref name="arg0"/> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(format, arg0);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of either of two arguments.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 2.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to <paramref name="arg0"/>
        /// and <paramref name="arg1"/>, the two objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="arg0"/> and <paramref name="arg1"/> represent the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of either <paramref name="arg0"/> or <paramref name="arg1"/>. If the format item includes <c>formatString</c>
        /// and the corresponding argument implements the <see cref="IFormattable"/> interface, then the argument's <c>ToString(formatString, null)</c>
        /// defines the formatting. Otherwise, the argument's <c>ToString()</c> defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <paramref name="arg0"/> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(format, arg0, arg1);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of either of three arguments.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 3.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to <paramref name="arg0"/>
        /// and <paramref name="arg1"/>, the two objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="arg0"/>, <paramref name="arg1"/>, and <paramref name="arg2"/> represent the objects to be formatted.
        /// Each format item in <paramref name="format"/> is replaced with the string representation of either <paramref name="arg0"/>, <paramref name="arg1"/>,
        /// or <paramref name="arg2"/>. If the format item includes <c>formatString</c> and the corresponding argument implements the
        /// <see cref="IFormattable"/> interface, then the argument's <c>ToString(formatString, null)</c> defines the formatting. Otherwise,
        /// the argument's <c>ToString()</c> defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <paramref name="arg0"/> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(format, arg0, arg1, arg2);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> or <paramref name="args"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> array.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="args"/> represents the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the corresponding object in <paramref name="args"/>. If the format item includes <c>formatString</c>
        /// and the corresponding object in <paramref name="args"/> implements the <see cref="IFormattable"/> interface, then
        /// <c>args[index].ToString(formatString, null)</c> defines the formatting. Otherwise, <c>args[index].ToString()</c>
        /// defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <c>args[0]</c> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(format, args);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter span.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">A span of objects to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> or <paramref name="args"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> span.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="args"/> represents the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the corresponding object in <paramref name="args"/>. If the format item includes <c>formatString</c>
        /// and the corresponding object in <paramref name="args"/> implements the <see cref="IFormattable"/> interface, then
        /// <c>args[index].ToString(formatString, null)</c> defines the formatting. Otherwise, <c>args[index].ToString()</c>
        /// defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <c>args[0]</c> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(format, args);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a single argument using a specified
        /// format provider.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">An object to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 1 (one).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items. The index of the format items must be zero (0),
        /// to correspond to <paramref name="arg0"/>, the single object in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of <paramref name="arg0"/>.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <c>args</c>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> if it is a numeric value.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> if it is a date and time value.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for <paramref name="arg0"/>.Typically, such an implementation also implements the
        ///     <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <see langword="null"/>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="arg0"/> represents the object to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of <paramref name="arg0"/>. If the format item includes <c>formatString</c>
        /// and <paramref name="arg0"/> implements the <see cref="IFormattable"/> interface, then <c>arg0.ToString(formatString, null)</c>
        /// defines the formatting. Otherwise, <c>arg0.ToString()</c> defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(provider, format, arg0);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of either of two arguments using a specified
        /// format provider.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 2 (two).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <c>args</c>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> or <paramref name="arg1"/> if they are numeric values.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> or <paramref name="arg1"/> if they are date and time values.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> or <paramref name="arg1"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for <paramref name="arg0"/> or <paramref name="arg1"/>.Typically, such an implementation also implements the
        ///     <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <see langword="null"/>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="arg0"/> and <paramref name="arg1"/> represent the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the object that has the corresponding index. If the format item includes <c>formatString</c>
        /// and the corresponding argument implements the <see cref="IFormattable"/> interface, then the argument's <c>ToString(formatString, null)</c>
        /// defines the formatting. Otherwise, the argument's <c>ToString()</c> defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(provider, format, arg0, arg1);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of either of three arguments using a specified
        /// format provider.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 3 (three).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <c>args</c>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/>, <paramref name="arg1"/>, or <paramref name="arg2"/> if they are a numeric values.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/>, <paramref name="arg1"/>, or <paramref name="arg2"/> if they are date and time values.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/>, <paramref name="arg1"/>, or <paramref name="arg2"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for <paramref name="arg0"/>, <paramref name="arg1"/>, or <paramref name="arg2"/>.Typically, such an
        ///     implementation also implements the <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <see langword="null"/>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="arg0"/>, <paramref name="arg1"/>, and <paramref name="arg2"/> represent the objects to be formatted.
        /// Each format item in <paramref name="format"/> is replaced with the string representation of the object that has the
        /// corresponding index. If the format item includes <c>formatString</c> and the corresponding argument implements the
        /// <see cref="IFormattable"/> interface, then the argument's <c>ToString(formatString, null)</c> defines the formatting. Otherwise,
        /// the argument's <c>ToString()</c> defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(provider, format, arg0, arg1, arg2);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding argument in a
        /// parameter array using a specified format provider.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> array.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <paramref name="args"/>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     numeric values in <paramref name="args"/>.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     date and time values in <paramref name="args"/>.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///      one or more of the objects in <paramref name="args"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for one or more of the objects in <paramref name="args"/>.Typically, such an implementation also implements the
        ///     <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <see langword="null"/>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="args"/> represents the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the corresponding object in <paramref name="args"/>. If the format item includes
        /// <c>formatString</c> and the corresponding object in <paramref name="args"/> implements the <see cref="IFormattable"/> interface, then
        /// <c>args[index].ToString(formatString, null)</c> defines the formatting. Otherwise, <c>args[index].ToString()</c>
        /// defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(provider, format, args);
            return text;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding argument in a
        /// parameter span using a specified format provider.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An span of objects to format.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> span.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <see langword="null"/>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <paramref name="args"/>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     numeric values in <paramref name="args"/>.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     date and time values in <paramref name="args"/>.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///      one or more of the objects in <paramref name="args"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for one or more of the objects in <paramref name="args"/>.Typically, such an implementation also implements the
        ///     <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <see langword="null"/>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="args"/> represents the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the corresponding object in <paramref name="args"/>. If the format item includes
        /// <c>formatString</c> and the corresponding object in <paramref name="args"/> implements the <see cref="IFormattable"/> interface, then
        /// <c>args[index].ToString(formatString, null)</c> defines the formatting. Otherwise, <c>args[index].ToString()</c>
        /// defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendFormat<TBuilder>(this TBuilder text, IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendFormatInternal(provider, format, args);
            return text;
        }

    }
}
