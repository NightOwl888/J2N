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
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    public static partial class MutableTextBufferExtensions
    {
        #region Append char

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, char value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        #endregion Append char

        #region Append char[]

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, char[]? value)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, char[]? value, int startIndex, int charCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, charCount);
            return text;
        }

        #endregion Append char[]

        #region Append char*

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static unsafe TBuilder Append<TBuilder>(this TBuilder text, char* value, int valueCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, valueCount);
            return text;
        }

        #endregion Append char*

        #region Append string

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, string? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, count);
            return text;
        }

        #endregion Append string

        #region Append ReadOnlySpan<char>

        /// <summary>Appends the string representation of a specified read-only character span to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The read-only character span to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ReadOnlySpan{Char}" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ReadOnlySpan<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        #endregion Append ReadOnlySpan<char>

        #region Append ReadOnlyMemory<char>

        /// <summary>Appends the string representation of a specified read-only character memory region to this instance.</summary>
        /// <typeparam name="TBuilder">The type of the target builder.</typeparam>
        /// <param name="text">The target builder.</param>
        /// <param name="value">The read-only character memory region to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ReadOnlyMemory{Char}" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, ReadOnlyMemory<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value);
            return text;
        }

        #endregion Append ReadOnlyMemory<char>

        #region Append StringBuilder

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Append<TBuilder>(this TBuilder text, StringBuilder? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendInternal(value, startIndex, count);
            return text;
        }

        #endregion Append StringBuilder

        // J2N: Moved ICharSequence overloads to J2N namespace


        #region Insert char

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, char value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        #endregion Insert char

        #region Insert char[]

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, char[]? value, int startIndex, int charCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, startIndex, charCount);
            return text;
        }

        #endregion Insert char[]

        #region Insert char*

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static unsafe TBuilder Insert<TBuilder>(this TBuilder text, int index, char* value, int valueCount)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, valueCount);
            return text;
        }

        #endregion Insert char*

        #region Insert string

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, string? value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, string? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, startIndex, count);
            return text;
        }

        #endregion

        #region Insert ReadOnlySpan<char>

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, ReadOnlySpan<char> value)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value);
            return text;
        }

        #endregion Insert ReadOnlySpan<char>

        #region Insert StringBuilder

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        public static TBuilder Insert<TBuilder>(this TBuilder text, int index, StringBuilder? value, int startIndex, int count)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.InsertInternal(index, value, startIndex, count);
            return text;
        }

        #endregion Insert StringBuilder

        // J2N: Moved ICharSequence overloads to J2N namespace
    }
}
