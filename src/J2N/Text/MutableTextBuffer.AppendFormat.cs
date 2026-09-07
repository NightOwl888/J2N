// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.CodeGeneration;
using J2N.Numerics;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Text
{
    internal partial class MutableTextBuffer
    {
        #region AppendFormat

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, object?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendFormatInternal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            AppendFormatCore(null, format, MemoryMarshal.CreateReadOnlySpan(ref arg0, 1));
#else
            AppendFormatCore(null, format, new ParamsArray(arg0));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, object?, object?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            TwoObjects two = new TwoObjects(arg0, arg1);
            AppendFormatCore(null, format, (ReadOnlySpan<object?>)two);
#else
            AppendFormatCore(null, format, new ParamsArray(arg0, arg1));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, object?, object?, object?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            ThreeObjects three = new ThreeObjects(arg0, arg1, arg2);
            AppendFormatCore(null, format, (ReadOnlySpan<object?>)three);
#else
            AppendFormatCore(null, format, new ParamsArray(arg0, arg1, arg2));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, object?[])"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        {
            if (args is null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormat(..., span).
                ThrowHelper.ThrowArgumentNullException(format is null ? ExceptionArgument.format : ExceptionArgument.args);
            }

            AppendFormatCore(null, format, args);
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, ReadOnlySpan{object?})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args)
        {
            AppendFormatCore(null, format, args);
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, IFormatProvider?, string, object?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            AppendFormatCore(provider, format, MemoryMarshal.CreateReadOnlySpan(ref arg0, 1));
#else
            AppendFormatCore(provider, format, new ParamsArray(arg0));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, IFormatProvider?, string, object?, object?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            TwoObjects two = new TwoObjects(arg0, arg1);
            AppendFormatCore(provider, format, (ReadOnlySpan<object?>)two);
#else
            AppendFormatCore(provider, format, new ParamsArray(arg0, arg1));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, IFormatProvider?, string, object?, object?, object?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            ThreeObjects three = new ThreeObjects(arg0, arg1, arg2);
            AppendFormatCore(provider, format, (ReadOnlySpan<object?>)three);
#else
            AppendFormatCore(provider, format, new ParamsArray(arg0, arg1, arg2));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, IFormatProvider?, string, object?[])"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        {
            if (args is null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormat(..., span).
                ThrowHelper.ThrowArgumentNullException(format is null ? ExceptionArgument.format : ExceptionArgument.args);
            }

            AppendFormatCore(provider, format, (ReadOnlySpan<object?>)args);
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, IFormatProvider?, string, ReadOnlySpan{object?})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation] // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        internal void AppendFormatInternal(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
        {
            AppendFormatCore(provider, format, args);
        }

        // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        // J2N TODO: Correct business logic to prefer Java-style formatters for numbers, dates, arrays, and collections
        // and sync those updates with the other overload. We also need to support J2N's reference type numbers that derive from Number.
        // J2N TODO: Investigate whether we can support Java's and/or ICU's MessageFormat syntax for the format parameter.
        private void AppendFormatCore(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);

            // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
            const int IndexLimit = 1_000_000; // Note:            0 <= ArgIndex < IndexLimit
            const int WidthLimit = 1_000_000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

            // Query the provider (if one was supplied) for an ICustomFormatter.  If there is one,
            // it needs to be used to transform all arguments.
            ICustomFormatter? cf = (ICustomFormatter?)provider?.GetFormat(typeof(ICustomFormatter));

            // J2N: Override the default culture if not provided.
            provider ??= DefaultCulture;

            // Repeatedly find the next hole and process it.
            int pos = 0;
            char ch;
            while (true)
            {
                // Skip until either the end of the input or the first unescaped opening brace, whichever comes first.
                // Along the way we need to also unescape escaped closing braces.
                while (true)
                {
                    // Find the next brace.  If there isn't one, the remainder of the input is text to be appended, and we're done.
                    if ((uint)pos >= (uint)format.Length)
                    {
                        return;
                    }

                    ReadOnlySpan<char> remainder = format.AsSpan(pos);
                    int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                    if (countUntilNextBrace < 0)
                    {
                        AppendInternal(remainder);
                        return;
                    }

                    // Append the text until the brace.
                    AppendInternal(remainder.Slice(0, countUntilNextBrace));
                    pos += countUntilNextBrace;

                    // Get the brace.  It must be followed by another character, either a copy of itself in the case of being
                    // escaped, or an arbitrary character that's part of the hole in the case of an opening brace.
                    char brace = format[pos];
                    ch = MoveNext(format, ref pos);
                    if (brace == ch)
                    {
                        AppendInternal(ch);
                        pos++;
                        continue;
                    }

                    // This wasn't an escape, so it must be an opening brace.
                    if (brace != '{')
                    {
                        ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_UnexpectedClosingBrace);
                    }

                    // Proceed to parse the hole.
                    break;
                }

                // We're now positioned just after the opening brace of an argument hole, which consists of
                // an opening brace, an index, an optional width preceded by a comma, and an optional format
                // preceded by a colon, with arbitrary amounts of spaces throughout.
                int width = 0;
                bool leftJustify = false;
                ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null

                // First up is the index parameter, which is of the form:
                //     at least on digit
                //     optional any number of spaces
                // We've already read the first digit into ch.
                Debug.Assert(format[pos - 1] == '{');
                Debug.Assert(ch != '{');
                int index = ch - '0';
                if ((uint)index >= 10u)
                {
                    ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_ExpectedAsciiDigit);
                }

                // Common case is a single digit index followed by a closing brace.  If it's not a closing brace,
                // proceed to finish parsing the full hole format.
                ch = MoveNext(format, ref pos);
                if (ch != '}')
                {
                    // Continue consuming optional additional digits.
                    while (Character.IsAsciiDigit(ch) && index < IndexLimit)
                    {
                        index = index * 10 + ch - '0';
                        ch = MoveNext(format, ref pos);
                    }

                    // Consume optional whitespace.
                    while (ch == ' ')
                    {
                        ch = MoveNext(format, ref pos);
                    }

                    // Parse the optional alignment, which is of the form:
                    //     comma
                    //     optional any number of spaces
                    //     optional -
                    //     at least one digit
                    //     optional any number of spaces
                    if (ch == ',')
                    {
                        // Consume optional whitespace.
                        do
                        {
                            ch = MoveNext(format, ref pos);
                        }
                        while (ch == ' ');

                        // Consume an optional minus sign indicating left alignment.
                        if (ch == '-')
                        {
                            leftJustify = true;
                            ch = MoveNext(format, ref pos);
                        }

                        // Parse alignment digits. The read character must be a digit.
                        width = ch - '0';
                        if ((uint)width >= 10u)
                        {
                            ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_ExpectedAsciiDigit);
                        }
                        ch = MoveNext(format, ref pos);
                        while (Character.IsAsciiDigit(ch) && width < WidthLimit)
                        {
                            width = width * 10 + ch - '0';
                            ch = MoveNext(format, ref pos);
                        }

                        // Consume optional whitespace
                        while (ch == ' ')
                        {
                            ch = MoveNext(format, ref pos);
                        }
                    }

                    // The next character needs to either be a closing brace for the end of the hole,
                    // or a colon indicating the start of the format.
                    if (ch != '}')
                    {
                        if (ch != ':')
                        {
                            // Unexpected character
                            ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_UnclosedFormatItem);
                        }

                        // Search for the closing brace; everything in between is the format,
                        // but opening braces aren't allowed.
                        int startingPos = pos;
                        while (true)
                        {
                            ch = MoveNext(format, ref pos);

                            if (ch == '}')
                            {
                                // Argument hole closed
                                break;
                            }

                            if (ch == '{')
                            {
                                // Braces inside the argument hole are not supported
                                ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_UnclosedFormatItem);
                            }
                        }

                        startingPos++;
                        itemFormatSpan = format.AsSpan(startingPos, pos - startingPos);
                    }
                }

                // Construct the output for this arg hole.
                Debug.Assert(format[pos] == '}');
                pos++;
                string? s = null;
                string? itemFormat = null;

                if ((uint)index >= (uint)args.Length)
                {
                    ThrowHelper.ThrowFormatIndexOutOfRange();
                }
                object? arg = args[index];

                if (cf != null)
                {
                    if (!itemFormatSpan.IsEmpty)
                    {
                        itemFormat = itemFormatSpan.ToString();
                    }

                    s = cf.Format(itemFormat, arg, provider);
                }

                if (s == null)
                {
                    // If arg is ISpanFormattable and the beginning doesn't need padding,
                    // try formatting it into the remaining current chunk.
                    if ((leftJustify || width == 0) &&
#if FEATURE_SPANFORMATTABLE
                        arg is ISpanFormattable spanFormattableArg &&
                        spanFormattableArg.TryFormat(m_Chars.AsSpan(m_Position), out int charsWritten, itemFormatSpan, provider))
#else
                        arg is Number numberArg &&
                        numberArg.TryFormat(m_Chars.AsSpan(m_Position), out int charsWritten, itemFormatSpan, provider))
#endif
                    {
                        if ((uint)charsWritten > (uint)(m_Chars.Length - m_Position))
                        {
                            // Untrusted ISpanFormattable implementations might return an erroneous charsWritten value,
                            // and m_Position might end up being used in Unsafe code, so fail if we get back an
                            // out-of-range charsWritten value.
                            ThrowHelper.ThrowFormatInvalidString();
                        }

                        m_Position += charsWritten;

                        // Pad the end, if needed.
                        if (leftJustify && width > charsWritten)
                        {
                            AppendInternal(' ', width - charsWritten);
                        }

                        // Continue to parse other characters.
                        continue;
                    }

                    // Otherwise, fallback to trying IFormattable or calling ToString.
                    if (arg is IFormattable formattableArg)
                    {
                        if (itemFormatSpan.Length != 0)
                        {
                            itemFormat ??= itemFormatSpan.ToString();
                        }
                        s = formattableArg.ToString(itemFormat, provider);
                    }
                    else
                    {
                        s = arg?.ToString();
                    }

                    s ??= string.Empty;
                }

                // Append it to the final output of the Format String.
                if (width <= s.Length)
                {
                    AppendInternal(s);
                }
                else if (leftJustify)
                {
                    AppendInternal(s);
                    AppendInternal(' ', width - s.Length);
                }
                else
                {
                    AppendInternal(' ', width - s.Length);
                    AppendInternal(s);
                }

                // Continue parsing the rest of the format string.
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static char MoveNext(string format, ref int pos)
            {
                pos++;
                if ((uint)pos >= (uint)format.Length)
                {
                    ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_UnclosedFormatItem);
                }
                return format[pos];
            }
        }

#if !FEATURE_INLINEARRAYATTRIBUTE
        // J2N TODO: API - change format to ReadOnlySpan<char> before making public
        private void AppendFormatCore(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, ParamsArray args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);

            // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
            const int IndexLimit = 1_000_000; // Note:            0 <= ArgIndex < IndexLimit
            const int WidthLimit = 1_000_000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

            // Query the provider (if one was supplied) for an ICustomFormatter.  If there is one,
            // it needs to be used to transform all arguments.
            ICustomFormatter? cf = (ICustomFormatter?)provider?.GetFormat(typeof(ICustomFormatter));

            // J2N: Override the default culture if not provided.
            provider ??= DefaultCulture;

            // Repeatedly find the next hole and process it.
            int pos = 0;
            char ch;
            while (true)
            {
                // Skip until either the end of the input or the first unescaped opening brace, whichever comes first.
                // Along the way we need to also unescape escaped closing braces.
                while (true)
                {
                    // Find the next brace.  If there isn't one, the remainder of the input is text to be appended, and we're done.
                    if ((uint)pos >= (uint)format.Length)
                    {
                        return;
                    }

                    ReadOnlySpan<char> remainder = format.AsSpan(pos);
                    int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                    if (countUntilNextBrace < 0)
                    {
                        AppendInternal(remainder);
                        return;
                    }

                    // Append the text until the brace.
                    AppendInternal(remainder.Slice(0, countUntilNextBrace));
                    pos += countUntilNextBrace;

                    // Get the brace.  It must be followed by another character, either a copy of itself in the case of being
                    // escaped, or an arbitrary character that's part of the hole in the case of an opening brace.
                    char brace = format[pos];
                    ch = MoveNext(format, ref pos);
                    if (brace == ch)
                    {
                        AppendInternal(ch);
                        pos++;
                        continue;
                    }

                    // This wasn't an escape, so it must be an opening brace.
                    if (brace != '{')
                    {
                        ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_UnexpectedClosingBrace);
                    }

                    // Proceed to parse the hole.
                    break;
                }

                // We're now positioned just after the opening brace of an argument hole, which consists of
                // an opening brace, an index, an optional width preceded by a comma, and an optional format
                // preceded by a colon, with arbitrary amounts of spaces throughout.
                int width = 0;
                bool leftJustify = false;
                ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null

                // First up is the index parameter, which is of the form:
                //     at least on digit
                //     optional any number of spaces
                // We've already read the first digit into ch.
                Debug.Assert(format[pos - 1] == '{');
                Debug.Assert(ch != '{');
                int index = ch - '0';
                if ((uint)index >= 10u)
                {
                    ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_ExpectedAsciiDigit);
                }

                // Common case is a single digit index followed by a closing brace.  If it's not a closing brace,
                // proceed to finish parsing the full hole format.
                ch = MoveNext(format, ref pos);
                if (ch != '}')
                {
                    // Continue consuming optional additional digits.
                    while (Character.IsAsciiDigit(ch) && index < IndexLimit)
                    {
                        index = index * 10 + ch - '0';
                        ch = MoveNext(format, ref pos);
                    }

                    // Consume optional whitespace.
                    while (ch == ' ')
                    {
                        ch = MoveNext(format, ref pos);
                    }

                    // Parse the optional alignment, which is of the form:
                    //     comma
                    //     optional any number of spaces
                    //     optional -
                    //     at least one digit
                    //     optional any number of spaces
                    if (ch == ',')
                    {
                        // Consume optional whitespace.
                        do
                        {
                            ch = MoveNext(format, ref pos);
                        }
                        while (ch == ' ');

                        // Consume an optional minus sign indicating left alignment.
                        if (ch == '-')
                        {
                            leftJustify = true;
                            ch = MoveNext(format, ref pos);
                        }

                        // Parse alignment digits. The read character must be a digit.
                        width = ch - '0';
                        if ((uint)width >= 10u)
                        {
                            ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_ExpectedAsciiDigit);
                        }
                        ch = MoveNext(format, ref pos);
                        while (Character.IsAsciiDigit(ch) && width < WidthLimit)
                        {
                            width = width * 10 + ch - '0';
                            ch = MoveNext(format, ref pos);
                        }

                        // Consume optional whitespace
                        while (ch == ' ')
                        {
                            ch = MoveNext(format, ref pos);
                        }
                    }

                    // The next character needs to either be a closing brace for the end of the hole,
                    // or a colon indicating the start of the format.
                    if (ch != '}')
                    {
                        if (ch != ':')
                        {
                            // Unexpected character
                            ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_UnclosedFormatItem);
                        }

                        // Search for the closing brace; everything in between is the format,
                        // but opening braces aren't allowed.
                        int startingPos = pos;
                        while (true)
                        {
                            ch = MoveNext(format, ref pos);

                            if (ch == '}')
                            {
                                // Argument hole closed
                                break;
                            }

                            if (ch == '{')
                            {
                                // Braces inside the argument hole are not supported
                                ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_UnclosedFormatItem);
                            }
                        }

                        startingPos++;
                        itemFormatSpan = format.AsSpan(startingPos, pos - startingPos);
                    }
                }

                // Construct the output for this arg hole.
                Debug.Assert(format[pos] == '}');
                pos++;
                string? s = null;
                string? itemFormat = null;

                if ((uint)index >= (uint)args.Length)
                {
                    ThrowHelper.ThrowFormatIndexOutOfRange();
                }
                object? arg = args[index];

                if (cf != null)
                {
                    if (!itemFormatSpan.IsEmpty)
                    {
                        itemFormat = itemFormatSpan.ToString();
                    }

                    s = cf.Format(itemFormat, arg, provider);
                }

                if (s == null)
                {
                    // If arg is ISpanFormattable and the beginning doesn't need padding,
                    // try formatting it into the remaining current chunk.
                    if ((leftJustify || width == 0) &&
#if FEATURE_SPANFORMATTABLE
                        arg is ISpanFormattable spanFormattableArg &&
                        spanFormattableArg.TryFormat(m_Chars.AsSpan(m_Position), out int charsWritten, itemFormatSpan, provider))
#else
                        arg is Number numberArg &&
                        numberArg.TryFormat(m_Chars.AsSpan(m_Position), out int charsWritten, itemFormatSpan, provider))
#endif
                    {
                        if ((uint)charsWritten > (uint)(m_Chars.Length - m_Position))
                        {
                            // Untrusted ISpanFormattable implementations might return an erroneous charsWritten value,
                            // and m_Position might end up being used in Unsafe code, so fail if we get back an
                            // out-of-range charsWritten value.
                            ThrowHelper.ThrowFormatInvalidString();
                        }

                        m_Position += charsWritten;

                        // Pad the end, if needed.
                        if (leftJustify && width > charsWritten)
                        {
                            AppendInternal(' ', width - charsWritten);
                        }

                        // Continue to parse other characters.
                        continue;
                    }

                    // Otherwise, fallback to trying IFormattable or calling ToString.
                    if (arg is IFormattable formattableArg)
                    {
                        if (itemFormatSpan.Length != 0)
                        {
                            itemFormat ??= itemFormatSpan.ToString();
                        }
                        s = formattableArg.ToString(itemFormat, provider);
                    }
                    else
                    {
                        s = arg?.ToString();
                    }

                    s ??= string.Empty;
                }

                // Append it to the final output of the Format String.
                if (width <= s.Length)
                {
                    AppendInternal(s);
                }
                else if (leftJustify)
                {
                    AppendInternal(s);
                    AppendInternal(' ', width - s.Length);
                }
                else
                {
                    AppendInternal(' ', width - s.Length);
                    AppendInternal(s);
                }

                // Continue parsing the rest of the format string.
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static char MoveNext(string format, ref int pos)
            {
                pos++;
                if ((uint)pos >= (uint)format.Length)
                {
                    ThrowHelper.ThrowFormatInvalidString(pos, ExceptionResource.Format_UnclosedFormatItem);
                }
                return format[pos];
            }
        }
#endif

        // J2N TODO: API - CompositeFormat overloads
#if FEATURE_COMPOSITEFORMAT

        [CodeGenerationExtensionImplementation]
        internal void AppendFormatInternal<TArg0>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(1);
            AppendFormatCore(provider, format, arg0, 0, 0, default);
        }

        [CodeGenerationExtensionImplementation]
        internal void AppendFormatInternal<TArg0, TArg1>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(2);
            AppendFormatCore(provider, format, arg0, arg1, 0, default);
        }

        [CodeGenerationExtensionImplementation]
        internal void AppendFormatInternal<TArg0, TArg1, TArg2>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(3);
            AppendFormatCore(provider, format, arg0, arg1, arg2, default);
        }

        [CodeGenerationExtensionImplementation]
        internal void AppendFormatInternal(IFormatProvider? provider, CompositeFormat format, params object?[] args)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            if (args is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.args);
            AppendFormatCore(provider, format, (ReadOnlySpan<object?>)args);
        }

        [CodeGenerationExtensionImplementation]
        internal void AppendFormatInternal(IFormatProvider? provider, CompositeFormat format, params ReadOnlySpan<object?> args)
        {
            //ArgumentNullException.ThrowIfNull(format);
            if (format is null)
                throw new ArgumentNullException(nameof(format));
            format.ValidateNumberOfArgs(args.Length);
            args.Length switch
            {
                0 => AppendFormatCore(provider, format, 0, 0, 0, args),
                1 => AppendFormatCore(provider, format, args[0], 0, 0, args),
                2 => AppendFormatCore(provider, format, args[0], args[1], 0, args),
                _ => AppendFormatCore(provider, format, args[0], args[1], args[2], args),
            };
        }

        private void AppendFormatCore<TArg0, TArg1, TArg2>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2, ReadOnlySpan<object?> args)
        {
            // Create the interpolated string handler.
            var handler = new AppendInterpolatedStringHandler(format._literalLength, format._formattedCount, this, provider);

            // Append each segment.
            foreach ((string? Literal, int ArgIndex, int Alignment, string? Format) segment in format._segments)
            {
                if (segment.Literal is string literal)
                {
                    handler.AppendLiteral(literal);
                }
                else
                {
                    int index = segment.ArgIndex;
                    switch (index)
                    {
                        case 0:
                            handler.AppendFormatted(arg0, segment.Alignment, segment.Format);
                            break;

                        case 1:
                            handler.AppendFormatted(arg1, segment.Alignment, segment.Format);
                            break;

                        case 2:
                            handler.AppendFormatted(arg2, segment.Alignment, segment.Format);
                            break;

                        default:
                            Debug.Assert(index > 2);
                            handler.AppendFormatted(args[index], segment.Alignment, segment.Format);
                            break;
                    }
                }
            }

            // Complete the operation.
            Append(ref handler);
        }

#endif

        #endregion AppendFormat
    }
}
