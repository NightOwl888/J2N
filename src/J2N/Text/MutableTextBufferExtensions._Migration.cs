using J2N.CodeGeneration;
using System;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Globalization;
using J2N.Buffers;
using J2N.Collections;
using J2N.Collections.Generic;
using J2N.Numerics;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using J2N.Numerics.Formatters;

namespace J2N.Text
{
    public static partial class MutableTextBufferExtensions
    {
        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified sequence to this instance.
        /// <para/>
        /// NOTE: Unlike the Java implementation, this method does not add the word <c>"null"</c> to the <see cref="MutableTextBuffer"/>
        /// if <paramref name="value"/> is <see langword="null"/>. Instead, no operation is performed.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <seealso cref="ICharSequence" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ICharSequence? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>Appends the string representation of a specified subarray of Unicode characters to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="count">The number of characters to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>, and
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <seealso cref="ICharSequence" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ICharSequence? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, count);
            return text;
        }

        /// <summary>Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character sequence to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ICharSequence? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character sequence to insert.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> is not a position within <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ICharSequence? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, startIndex, count);
            return text;
        }


        /// <summary>Removes all characters from the current <see cref="MutableTextBuffer"/> instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// <see cref="Clear"/> is a convenience method that is equivalent to setting
        /// the <see cref="MutableTextBuffer.Length"/> property of the current instance to 0 (zero).
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Clear<TBuilder>(this TBuilder text)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ClearInternal();
            return text;
        }

        /// <summary>Appends a specified number of copies of the string representation of a Unicode character to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The character to append.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="repeatCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="OutOfMemoryException">Out of memory.</exception>
        /// <remarks>
        /// The <see cref="Append{TBuilder}(TBuilder, char, int)"/> method modifies the existing instance of this class;
        /// it does not return a new class instance. Because of this, you can call a method or property
        /// on the existing reference and you do not have to assign the return value to an
        /// <see cref="MutableTextBuffer"/> object, as the following example illustrates.
        /// <code>
        /// decimal value = 1346.19m;
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// sb.Append('*', 5).AppendFormat("{0:C2}", value).Append('*', 5);
        /// Console.WriteLine(sb);
        /// // The example displays the following output:
        /// //       *****$1,346.19*****
        /// </code>
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, char value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, repeatCount);
            return text;
        }

        /// <summary>Appends the string representation of a specified subarray of Unicode characters to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="charCount">The number of characters to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>, and <paramref name="startIndex"/>
        /// and <paramref name="charCount"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="charCount"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method appends the specified range of characters in <paramref name="value"/> to the current instance. If
        /// <paramref name="value"/> is <see langword="null"/> and <paramref name="startIndex"/> and <paramref name="charCount"/>
        /// are both zero, no changes are made.
        /// <para/>
        /// The <see cref="Append{TBuilder}(TBuilder, char[], int, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to a <see cref="MutableTextBuffer"/> object,
        /// as the following example illustrates.
        /// <code>
        /// char[] chars = { 'a', 'b', 'c', 'd', 'e'};
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// int startPosition = Array.IndexOf(chars, 'a');
        /// int endPosition = Array.IndexOf(chars, 'c');
        /// if (startPosition >= 0 &amp;&amp; endPosition >= 0) {
        ///    sb.Append("The array from positions ").Append(startPosition).
        ///              Append(" to ").Append(endPosition).Append(" contains ").
        ///              Append(chars, startPosition, endPosition + 1).Append(".");
        ///    Console.WriteLine(sb);
        /// }
        /// // The example displays the following output:
        /// //       The array from positions 0 to 2 contains abc.
        /// </code>
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, char[]? value, int startIndex, int charCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, charCount);
            return text;
        }

        /// <summary>Appends a copy of the specified string to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// The <see cref="Append{TBuilder}(TBuilder, string?)"/> method modifies the existing instance of this class;
        /// it does not return a new class instance. Because of this, you can call a method or
        /// property on the existing reference and you do not have to assign the return value
        /// to a <see cref="MutableTextBuffer"/> object, as the following example illustrates.
        /// <code>
        /// bool flag = false;
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// sb.Append("The value of the flag is ").Append(flag).Append(".");
        /// Console.WriteLine(sb.ToString());
        /// // The example displays the following output:
        /// //       The value of the flag is False.
        /// </code>
        /// <para/>
        /// If <paramref name="value"/> is <see langword="null"/>, no changes are made.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="string" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, string? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>Appends a copy of a specified substring to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>, and <paramref name="startIndex"/>
        /// and <paramref name="count"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method appends the specified range of characters in <paramref name="value"/> to the current instance. If
        /// <paramref name="value"/> is <see langword="null"/> and <paramref name="startIndex"/> and <paramref name="count"/>
        /// are both zero, no changes are made.
        /// <para/>
        /// The <see cref="Append{TBuilder}(TBuilder, string, int, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to a <see cref="MutableTextBuffer"/> object,
        /// as the following example illustrates.
        /// <code>
        /// string str = "First;George Washington;1789;1797";
        /// int index = 0;
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// int length = str.IndexOf(';', index);
        /// sb.Append(str, index, length).Append(" President of the United States: ");
        /// index += length + 1;
        /// length = str.IndexOf(';', index) - index;
        /// sb.Append(str, index, length).Append(", from ");
        /// index += length + 1;
        /// length = str.IndexOf(';', index) - index;
        /// sb.Append(str, index, length).Append(" to ");
        /// index += length + 1;
        /// sb.Append(str, index, str.Length - index);
        /// Console.WriteLine(sb);
        /// // The example displays the following output:
        /// //    First President of the United States: George Washington, from 1789 to 1797
        /// </code>
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you
        /// call the <see cref="Append{TBuilder}(TBuilder, string?)"/> and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/>
        /// methods to append small strings.
        /// </remarks>
        /// <seealso cref="string" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, string? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, count);
            return text;
        }

        /// <summary>Appends the string representation of a specified string builder to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string builder to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// The <see cref="Append{TBuilder}(TBuilder, StringBuilder?)"/> method modifies the existing instance of this class;
        /// it does not return a new class instance. Because of this, you can call a method or
        /// property on the existing reference and you do not have to assign the return value
        /// to a <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// If <paramref name="value"/> is <see langword="null"/>, no changes are made.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="StringBuilder" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, StringBuilder? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>Appends a copy of a specified substring of a string builder to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string builder that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>, and <paramref name="startIndex"/>
        /// and <paramref name="count"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method appends the specified range of characters in <paramref name="value"/> to the current instance. If
        /// <paramref name="value"/> is <see langword="null"/> and <paramref name="startIndex"/> and <paramref name="count"/>
        /// are both zero, no changes are made.
        /// <para/>
        /// The <see cref="Append{TBuilder}(TBuilder, StringBuilder?, int, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to a <see cref="MutableTextBuffer"/> object,
        /// as the following example illustrates.
        /// <code>
        /// string str = "First;George Washington;1789;1797";
        /// System.Text.StringBuilder builder = new System.Text.StringBuilder(str);
        /// int index = 0;
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// int length = str.IndexOf(';', index);
        /// sb.Append(builder, index, length).Append(" President of the United States: ");
        /// index += length + 1;
        /// length = str.IndexOf(';', index) - index;
        /// sb.Append(builder, index, length).Append(", from ");
        /// index += length + 1;
        /// length = str.IndexOf(';', index) - index;
        /// sb.Append(builder, index, length).Append(" to ");
        /// index += length + 1;
        /// sb.Append(builder, index, str.Length - index);
        /// Console.WriteLine(sb);
        /// // The example displays the following output:
        /// //    First President of the United States: George Washington, from 1789 to 1797
        /// </code>
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="StringBuilder" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, StringBuilder? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, count);
            return text;
        }

#pragma warning disable CS1591 // J2N TODO: Finish docs

        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, MutableTextBuffer? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, MutableTextBuffer? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, count);
            return text;
        }

#pragma warning restore CS1591 // J2N TODO: Finish docs

        /// <summary>Appends the default line terminator to the end of the current <see cref="MutableTextBuffer"/> object.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLine<TBuilder>(this TBuilder text)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLineInternal();
            return text;
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of the
        /// current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="string" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLine<TBuilder>(this TBuilder text, string? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLineInternal(value);
            return text;
        }

        /// <summary>
        /// Appends a copy of the specified sequence of characters followed by the default line terminator to the end of the
        /// current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="ReadOnlySpan{Char}" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder AppendLine<TBuilder>(this TBuilder text, ReadOnlySpan<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendLineInternal(value);
            return text;
        }

        /// <summary>Inserts one or more copies of a specified string into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The string to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="repeatCount"/> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of <paramref name="value"/>
        /// times <paramref name="repeatCount"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// This <see cref="MutableTextBuffer"/> object is not changed if <paramref name="value"/> is <see langword="null"/>,
        /// <paramref name="value"/> is not <see langword="null"/> but its length is zero, or <paramref name="repeatCount"/> is zero.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, string? value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, repeatCount);
            return text;
        }

        /// <summary>Inserts one or more copies of a specified sequence of characters into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The sequence of characters to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="repeatCount"/> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of <paramref name="value"/>
        /// times <paramref name="repeatCount"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// This <see cref="MutableTextBuffer"/> object is not changed if the length of <paramref name="value"/> is zero or
        /// <paramref name="repeatCount"/> is zero.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ReadOnlySpan<char> value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, repeatCount);
            return text;
        }

        /// <summary>Inserts one or more copies of a specified sequence of characters into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The sequence of characters to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="repeatCount"/> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of <paramref name="value"/>
        /// times <paramref name="repeatCount"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// This <see cref="MutableTextBuffer"/> object is not changed if the length of <paramref name="value"/> is zero or
        /// <paramref name="repeatCount"/> is zero.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, StringBuilder? value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, repeatCount);
            return text;
        }

        /// <summary>Inserts one or more copies of a specified sequence of characters into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The sequence of characters to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="repeatCount"/> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of <paramref name="value"/>
        /// times <paramref name="repeatCount"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// This <see cref="MutableTextBuffer"/> object is not changed if the length of <paramref name="value"/> is zero or
        /// <paramref name="repeatCount"/> is zero.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ICharSequence? value, int repeatCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, repeatCount);
            return text;
        }

        /// <summary>Removes the specified range of characters from this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="startIndex">The zero-based position in this instance where removal begins.</param>
        /// <param name="length">The number of characters to remove.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="startIndex"/> or <paramref name="length"/> is less than zero,
        /// or <paramref name="startIndex"/> + <paramref name="length"/> is greater than the length of this instance.
        /// </exception>
        /// <remarks>
        /// The current method removes the specified range of characters from the current instance. The characters at
        /// (<paramref name="startIndex"/> + <paramref name="length"/>) are moved to <paramref name="startIndex"/>, and
        /// the string value of the current instance is shortened by <paramref name="length"/>. The capacity of the
        /// current instance is unaffected.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Remove<TBuilder>(this TBuilder text, int startIndex, int length)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.RemoveInternal(startIndex, length);
            return text;
        }

        /// <summary>Removes the character at the specified index from this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The zero-based position in this instance of the character to remove.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or
        /// greater than or equal to the length of this instance.
        /// </exception>
        /// <remarks>
        /// The current method removes the specified character from the current instance. The characters at
        /// (<paramref name="index"/> + 1) are moved to <paramref name="index"/>, and
        /// the string value of the current instance is shortened by 1. The capacity of the
        /// current instance is unaffected.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder RemoveAt<TBuilder>(this TBuilder text, int index)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.RemoveAtInternal(index);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance
        /// in lowercase.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The Boolean value to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// This matches the behavior of Java's StringBuilder. To match the behavior
        /// of .NET, call <see cref="Insert{TBuilder}(TBuilder, int, bool, BooleanFormat)"/> and specify <see cref="BooleanFormat.TitleCase"/>.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="bool" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, bool value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance
        /// in the specified format.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The Boolean value to append.</param>
        /// <param name="format">
        /// The format to use. Specify <see cref="BooleanFormat.Lowercase"/> to match Java.
        /// Specify <see cref="BooleanFormat.TitleCase"/> to match .NET.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>The capacity of this instance is adjusted as needed.</remarks>
        /// <seealso cref="bool" />
        /// <seealso cref="BooleanFormat" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, bool value, BooleanFormat format)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, format);
            return text;
        }

        /// <summary>Appends the string representation of a specified <see cref="char"/> object to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// The <see cref="Append{TBuilder}(TBuilder, char)"/> method modifies the existing instance of this class;
        /// it does not return a new class instance. Because of this, you can call a method or property
        /// on the existing reference and you do not have to assign the return value to a <see cref="MutableTextBuffer"/>
        /// object, as the following example illustrates.
        /// <code>
        /// string str = "Characters in a string.";
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// foreach (var ch in str)
        ///    sb.Append(" '").Append(ch).Append("' ");
        /// 
        /// Console.WriteLine("Characters in the string:");
        /// Console.WriteLine("  {0}", sb);
        /// // The example displays the following output:
        /// //    Characters in the string:
        /// //       'C'  'h'  'a'  'r'  'a'  'c'  't'  'e'  'r'  's'  ' '  'i'  'n'  ' '  'a'  ' '  's'  't' 'r'  'i'  'n'  'g'  '.'
        /// </code>
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, char value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>Appends the string representation of the Unicode characters in a specified array to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The array of characters to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method appends the characters in the specified array to the current instance in the same order they
        /// appear in value. If <paramref name="value"/> is <see langword="null"/>, no changes are made.
        /// <para/>
        /// The <see cref="Append{TBuilder}(TBuilder, char[])"/> method modifies the existing instance of this class; it does not
        /// return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to a <see cref="MutableTextBuffer"/> object,
        /// as the following example illustrates.
        /// <code>
        /// char[] chars = { 'a', 'e', 'i', 'o', 'u' };
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// sb.Append("The characters in the array: ").Append(chars);
        /// Console.WriteLine(sb);
        /// // The example displays the following output:
        /// //      The characters in the array: aeiou
        /// </code>
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, char[]? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>Appends the string representation of a specified read-only character span to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The read-only character span to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ReadOnlySpan{Char}" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ReadOnlySpan<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        /// <summary>Appends the string representation of a specified read-only character memory region to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The read-only character memory region to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ReadOnlyMemory{Char}" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ReadOnlyMemory<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }



        /// <summary>Inserts a string into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The string to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// <para/>
        /// This instance of <see cref="MutableTextBuffer"/> is not changed if <paramref name="value"/> is <see langword="null"/>,
        /// or <paramref name="value"/> is not <see langword="null"/> but its length is zero.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, string? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        /// <summary>Inserts a <see cref="StringBuilder"/> into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, StringBuilder? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> is not a position within <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, StringBuilder? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, startIndex, count);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified Boolean value to this instance
        /// in lowercase at the specifed character position.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This matches the behavior of Java's StringBuilder. To match the behavior
        /// of .NET, call <see cref="Insert{TBuilder}(TBuilder, int, bool, BooleanFormat)"/> and specify <see cref="BooleanFormat.TitleCase"/>.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// </remarks>
        /// <seealso cref="bool" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, bool value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified Boolean value to this instance
        /// in the specified format at the specified position.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="format">
        /// The format to use. Specify <see cref="BooleanFormat.Lowercase"/> to match Java.
        /// Specify <see cref="BooleanFormat.TitleCase"/> to match .NET.
        /// </param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.</remarks>
        /// <seealso cref="bool" />
        /// <seealso cref="BooleanFormat" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, bool value, BooleanFormat format)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, format);
            return text;
        }

        /// <summary>Inserts the string representation of a specified Unicode character into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.</remarks>
        /// <seealso cref="char" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, char value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified array of Unicode characters into this
        /// instance at the specified character position.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character array to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="value"/> is <see langword="null"/>, the <see cref="MutableTextBuffer"/> is not changed.
        /// </remarks>
        /// <seealso cref="char" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, char[]? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters
        /// into this instance at the specified character position.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>, and <paramref name="startIndex"/>
        /// and <paramref name="charCount"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/>, or <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="charCount"/> is not a position within <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.</remarks>
        /// <seealso cref="char" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, char[]? value, int startIndex, int charCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, startIndex, charCount);
            return text;
        }

        /// <summary>
        /// Inserts the specified substring into this instance at the specified character position.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>, and <paramref name="startIndex"/>
        /// and <paramref name="count"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/>, or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> is not a position within <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.</remarks>
        /// <seealso cref="char" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, string? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, startIndex, count);
            return text;
        }

        /// <summary>Inserts the sequence of characters into this instance at the specified character position.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character span to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// The existing characters are shifted to make room for the character sequence in the
        /// <paramref name="value"/> to insert it. The capacity is adjusted as needed.
        /// </remarks>
        /// <seealso cref="ReadOnlySpan{Char}" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ReadOnlySpan<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }


        /// <summary>Replaces all occurrences of a specified string in this instance with another specified string.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <see langword="null"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/> in the
        /// current instance. If <paramref name="newValue"/> is <see langword="null"/> or <see cref="string.Empty"/>, all occurrences of
        /// <paramref name="oldValue"/> are removed.
        /// </remarks>
        /// <seealso cref="Remove{TBuilder}(TBuilder, int, int)" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, string oldValue, string? newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldValue, newValue);
            return text;
        }

        /// <summary>Replaces all instances of one read-only character span with another in this builder.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldValue">The read-only character span to replace.</param>
        /// <param name="newValue">The read-only character span to replace <paramref name="oldValue"/> with.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/> in the
        /// current instance. If <paramref name="newValue"/> is empty, all occurrences of <paramref name="oldValue"/> are removed.
        /// </remarks>
        /// <seealso cref="Remove{TBuilder}(TBuilder, int, int)" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldValue, newValue);
            return text;
        }

        /// <summary>Replaces, within a substring of this instance, all occurrences of a specified string with another specified string.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <see langword="null"/>.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a character position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/>
        /// in the specified substring. If <paramref name="newValue"/> is <see langword="null"/> or <see cref="string.Empty"/>,
        /// all occurrences of <paramref name="oldValue"/> in the specified range are removed.
        /// </remarks>
        /// <seealso cref="Remove{TBuilder}(TBuilder, int, int)" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, string oldValue, string? newValue, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldValue, newValue, startIndex, count);
            return text;
        }

        /// <summary>Replaces all instances of one read-only character span with another in a substring of this builder.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldValue">The read-only character span to replace.</param>
        /// <param name="newValue">The read-only character span to replace <paramref name="oldValue"/> with.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a character position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/>
        /// in the specified substring. If <paramref name="newValue"/> is empty, all occurrences of <paramref name="oldValue"/>
        /// in the specified range are removed.
        /// </remarks>
        /// <seealso cref="Remove{TBuilder}(TBuilder, int, int)" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldValue, newValue, startIndex, count);
            return text;
        }

        /// <summary>Replaces all occurrences of a specified character in this instance with another specified character.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldChar">The character to replace.</param>
        /// <param name="newChar">The character that replaces <paramref name="oldChar"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of
        /// <paramref name="oldChar"/> in the current instance. The size of the current
        /// <see cref="MutableTextBuffer"/> instance is unchanged after the replacement.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, char oldChar, char newChar)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldChar, newChar);
            return text;
        }

        /// <summary>Replaces, within a substring of this instance, all occurrences of a specified character with another specified character.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="oldChar">The character to replace.</param>
        /// <param name="newChar">The character that replaces <paramref name="oldChar"/>.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a character position not within this instance.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of
        /// <paramref name="oldChar"/> in the current instance within the specified substring. The size of the current
        /// <see cref="MutableTextBuffer"/> instance is unchanged after the replacement.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, char oldChar, char newChar, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(oldChar, newChar, startIndex, count);
            return text;
        }

        /// <summary>
        /// Replaces the specified substring in this builder with the specified
        /// string, <paramref name="newValue"/>. The substring begins at the specified
        /// <paramref name="startIndex"/> and ends to the character at
        /// <c><paramref name="count"/> - <paramref name="startIndex"/></c> or
        /// to the end of the sequence if no such character exists. First the
        /// characters in the substring are removed and then the specified
        /// <paramref name="newValue"/> is inserted at <paramref name="startIndex"/>.
        /// This <see cref="ValueStringBuilder"/> will be lengthened to accommodate the
        /// specified <paramref name="newValue"/> if necessary.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="startIndex">The inclusive begin index in this builder.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="newValue"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than or equal to <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, int startIndex, int count, string newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(startIndex, count, newValue);
            return text;
        }

        /// <summary>
        /// Replaces the specified substring in this builder with the specified
        /// character span, <paramref name="newValue"/>. The substring begins at the specified
        /// <paramref name="startIndex"/> and ends to the character at
        /// <c><paramref name="count"/> - <paramref name="startIndex"/></c> or
        /// to the end of the sequence if no such character exists. First the
        /// characters in the substring are removed and then the specified
        /// <paramref name="newValue"/> is inserted at <paramref name="startIndex"/>.
        /// This <see cref="ValueStringBuilder"/> will be lengthened to accommodate the
        /// specified <paramref name="newValue"/> if necessary.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="startIndex">The inclusive begin index in this builder.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than or equal to <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        /// <remarks>This method allows <paramref name="newValue"/> to be this instance or a slice of this instance.</remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Replace<TBuilder>(this TBuilder text, int startIndex, int count, ReadOnlySpan<char> newValue)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReplaceInternal(startIndex, count, newValue);
            return text;
        }

        /// <summary>Appends an array of Unicode characters starting at a specified address to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">A pointer to an array of characters.</param>
        /// <param name="valueCount">The number of characters in the array.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="valueCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="NullReferenceException"><paramref name="value"/> is a null pointer.</exception>
        /// <remarks>
        /// This method appends <paramref name="valueCount"/> characters starting at address <paramref name="value"/>
        /// to the current instance.
        /// <para/>
        /// The <see cref="Append{TBuilder}(TBuilder, char*, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to a <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        [CLSCompliant(false)]
        [CodeGenerationGenerateForwarder]
        public static unsafe TBuilder Append<TBuilder>(this TBuilder text, char* value, int valueCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, valueCount);
            return text;
        }

        /// <summary>Inserts an array of Unicode characters starting at a specified address into this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A pointer to an array of characters.</param>
        /// <param name="valueCount">The number of characters in the array.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="valueCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="NullReferenceException"><paramref name="value"/> is a null pointer.</exception>
        /// <remarks>
        /// This method inserts <paramref name="valueCount"/> characters starting at address <paramref name="value"/>
        /// to the current instance.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// </remarks>
        [CLSCompliant(false)]
        [CodeGenerationGenerateForwarder]
        public static unsafe TBuilder Insert<TBuilder>(this TBuilder text, int index, char* value, int valueCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, valueCount);
            return text;
        }

        /// <summary>
        /// Deletes a sequence of characters specified by <paramref name="startIndex"/> and <paramref name="count"/>.
        /// Shifts any remaining characters to the left.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// <para/>
        /// This method differs from <see cref="Remove{TBuilder}(TBuilder, int, int)"/> in that it will automatically
        /// adjust the <paramref name="count"/> if <c><paramref name="startIndex"/> + <paramref name="count"/> > <see cref="MutableTextBuffer.Length"/></c>
        /// to <c><see cref="MutableTextBuffer.Length"/> - <paramref name="startIndex"/>.</c>, provided it is not bounded by <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of characters to delete.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        [CodeGenerationGenerateForwarder]
        public static TBuilder Delete<TBuilder>(this TBuilder text, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.DeleteInternal(startIndex, count);
            return text;
        }

        /// <summary>
        /// Causes this character sequence to be replaced by the reverse of
        /// the sequence. If there are any surrogate pairs included in the
        /// sequence, these are treated as single characters for the
        /// reverse operation. Thus, the order of the high-low surrogates
        /// is never reversed.
        /// <para/>
        /// IMPORTANT: This operation is done in-place. Although a <see cref="MutableTextBuffer"/>
        /// is returned, it is the SAME instance as the one that is passed in.
        /// <para/>
        /// Let <c>n</c> be the character length of this character sequence
        /// (not the length in <see cref="char"/> values) just prior to
        /// execution of the <see cref="Reverse{TBuilder}(TBuilder)"/> method. Then the
        /// character at index <c>k</c> in the new character sequence is
        /// equal to the character at index <c>n-k-1</c> in the old
        /// character sequence.
        /// <para/>
        /// Note that the reverse operation may result in producing
        /// surrogate pairs that were unpaired low-surrogates and
        /// high-surrogates before the operation. For example, reversing
        /// "&#92;uDC00&#92;uD800" produces "&#92;uD800&#92;uDC00" which is
        /// a valid surrogate pair.
        /// <para/>
        /// Usage Note: This is the same operation as Java's StringBuilder.reverse()
        /// method. However, J2N also provides <see cref="J2N.Text.StringExtensions.ReverseText(string)"/>
        /// and <see cref="J2N.MemoryExtensions.ReverseText(Span{char})"/> which
        /// don't require a <see cref="MutableTextBuffer"/> instance.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="StringExtensions.ReverseText(string)" />
        /// <seealso cref="MemoryExtensions.ReverseText(Span{char})" />
        /// <seealso cref="StringBuilderExtensions.Reverse(StringBuilder)" />
        [CodeGenerationGenerateForwarder]
        public static TBuilder Reverse<TBuilder>(this TBuilder text)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.ReverseInternal();
            return text;
        }

#if FEATURE_INDEX_RANGE
        /// <summary>Inserts a copy of the specified range of characters from this buffer at the specified index.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The index at which the copied range will be inserted.</param>
        /// <param name="range">The range of characters to copy and insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="range"/> extends beyond the bounds
        /// of the buffer.
        /// </exception>
        /// <remarks>
        /// This operation supports overlapping source and destination ranges.
        /// <para/>
        /// <paramref name="index"/> refers to the original buffer before insertion takes place.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder InsertFromSelf<TBuilder>(this TBuilder text, int index, Range range)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertFromSelfInternal(index, range);
            return text;
        }

#endif

        /// <summary>
        /// Inserts a copy of a range of characters from this buffer
        /// at the specified index, expanding the <see cref="MutableTextBuffer.Length"/> by
        /// <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="index">The index at which the copied range will be inserted.</param>
        /// <param name="startIndex">The starting index of the source range to copy.</param>
        /// <param name="count">The number of characters to copy.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/>, or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> or <paramref name="startIndex"/> is greater
        /// than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater
        /// than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        /// <remarks>
        /// This operation supports overlapping source and destination ranges.
        /// <para/>
        /// <paramref name="index"/> refers to the original buffer before insertion
        /// takes place.
        /// </remarks>
        [CodeGenerationGenerateForwarder]
        public static TBuilder InsertFromSelf<TBuilder>(this TBuilder text, int index, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertFromSelfInternal(index, startIndex, count);
            return text;
        }


    }
}
