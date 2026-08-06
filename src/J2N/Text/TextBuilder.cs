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

using J2N.Buffers;
using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Represents a mutable string of characters and provides access to the underlying memory.
    /// </summary>
    /// <remarks>
    /// <see cref="TextBuilder"/> differs from <see cref="StringBuilder"/> in the following ways:
    /// 
    /// <list type="bullet">
    ///     <item><description>
    ///         Rather than managing chunks of memory, <see cref="TextBuilder"/> manages a single contiguous
    ///         block of <see cref="char"/>s.
    ///     </description></item>
    ///     <item><description>
    ///         Memory is directly accessible using <see cref="TextBuilderExtensions.AsSpan(TextBuilder?)"/> and
    ///         <see cref="TextBuilderExtensions.AsMemory(TextBuilder?)"/> overloads including the ability to slice.
    ///         There is no need to allocate memory and do a copy to call methods that require System.Memory types, such as
    ///         <see cref="ReadOnlySpan{T}"/> or to access a portion of the underlying chars.
    ///     </description></item>
    ///     <item><description>
    ///         <see cref="TextBuilder"/> is implicitly convertible to <see cref="ReadOnlySpan{Char}"/> to allow
    ///         passing the builder to low-level APIs without needing special overloads that accept <see cref="TextBuilder"/>.
    ///         This is similar to how <see cref="T:char[]"/> and <see cref="string"/> are implicitly converted to
    ///         <see cref="ReadOnlySpan{Char}"/>.
    ///     </description></item>
    ///     <description><item>
    ///         The <see cref="TextBuilder(int)"/> and <see cref="TextBuilder(int, int)"/> constructors allow
    ///         setting the initial capacity to zero, meaning no backing array is allocated unless it is needed.
    ///     </item></description>
    ///     <item><description>
    ///         Indexing through <see cref="this[int]"/> is significantly faster than with <see cref="StringBuilder"/>.
    ///     </description></item>
    ///     <item><description>
    ///         This class implements <see cref="IBufferWriter{Char}"/>, providing support for components that write
    ///         directly to a character buffer. This interface provides similar capabilities as the <c>Appendable</c>
    ///         interface in the JDK.
    ///     </description></item>
    ///     <item><description>
    ///         Rather than optimizing for operations that require moving or copying characters,
    ///         this implementation optimizes for memory reuse, reducing array allocations, and direct
    ///         support for System.Memory capabilities.
    ///     </description></item>
    /// </list>
    /// <para/>
    /// By default, <see cref="TextBuilder"/> follows typical .NET culture-sensitive behavior. When porting Java applications,
    /// consider setting <see cref="UseInvariantDefaults"/> during construction to use invariant defaults for culture-sensitive operations.
    /// </remarks>
    public partial class TextBuilder : IBufferWriter<char>, ISpannable<char>, ISpanCopyable<char>, ICopyable<char>
    {
        internal readonly MutableTextBuffer buffer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MutableTextBuffer CreateBuffer()
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            => new(UninitializedArrayAllocator<char>.Default)
#else
            => new(ArrayAllocator<char>.Default)
#endif
        {
            ClearExposedBuffers = true
        };

        #region BCL Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class.
        /// </summary>
        /// <remarks>
        /// The string value of this instance is set to <see cref="string.Empty"/>, and the capacity is set to
        /// the implementation-specific default capacity.
        /// </remarks>
        public TextBuilder()
        {
            buffer = CreateBuffer().Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class using the specified capacity.
        /// </summary>
        /// <param name="capacity">The suggested starting size of this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="TextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// The string value of this instance is set to <see cref="string.Empty"/>. If <paramref name="capacity"/> is zero,
        /// no backing array is allocated and the first mutation will grow the buffer, as needed.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public TextBuilder(int capacity)
        {
            buffer = CreateBuffer().Initialize(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class using the specified string.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="TextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="TextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</remarks>
        public TextBuilder(string? value)
        {
            buffer = CreateBuffer().Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class with the specified string
        /// and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="TextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="TextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="TextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public TextBuilder(string? value, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class from the specified
        /// substring and capacity.
        /// </summary>
        /// <param name="value">The string that contains the substring used to initialize the value of this instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="TextBuilder"/> will contain the empty
        /// string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="TextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="TextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public TextBuilder(string? value, int startIndex, int length, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, startIndex, length, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class that starts with a specified capacity
        /// and can grow to a specified maximum.
        /// </summary>
        /// <param name="capacity">The suggested starting size of the <see cref="TextBuilder"/>.</param>
        /// <param name="maxCapacity">The maximum number of characters the current string can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxCapacity"/> is less than one, <paramref name="capacity"/> is less than zero,
        /// or <paramref name="capacity"/> is greater than <paramref name="maxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="TextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, no backing array is allocated and the first mutation will
        /// grow the buffer, as needed.
        /// <para/>
        /// The <paramref name="maxCapacity"/> property defines the maximum number of characters that the current
        /// instance can hold. Its value is assigned to the <see cref="MaxCapacity"/> property. If the number of
        /// characters to be stored in the current instance exceeds this <paramref name="maxCapacity"/> value,
        /// the <see cref="TextBuilder"/> object does not allocate additional memory, but instead throws an exception.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="TextBuilder"/> object by calling the <see cref="TextBuilder(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="TextBuilder"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="TextBuilderExtensions.Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="TextBuilderExtensions.AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        /// <seealso cref="MaxCapacity"/>
        public TextBuilder(int capacity, int maxCapacity)
        {
            buffer = CreateBuffer().Initialize(capacity, maxCapacity);
        }

        #endregion BCL Constructors

        #region J2N Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">The length of <paramref name="value"/> is greater than the
        /// platform-specific maximum array capacity.</exception>
        /// <remarks>The characters from the span are copied to the heap memory of this instance.</remarks>
        public TextBuilder(ReadOnlySpan<char> value)
        {
            buffer = CreateBuffer().Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="TextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// <para/>
        /// -or-
        /// <para/>
        /// The length of <paramref name="value"/> is greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="TextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public TextBuilder(ReadOnlySpan<char> value, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class using the specified
        /// <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="TextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="TextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).
        /// </remarks>
        public TextBuilder(StringBuilder? value)
        {
            buffer = CreateBuffer().Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class with the specified
        /// <see cref="StringBuilder"/> and capacity.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> used to initialize the value of the instance.
        /// If <paramref name="value"/>is <c>null</c>, the new <see cref="TextBuilder"/> will contain
        /// the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="TextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="TextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public TextBuilder(StringBuilder? value, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> class from the specified
        /// substring and capacity.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the substring used to initialize the
        /// value of this instance. If <paramref name="value"/> is <c>null</c>, the new <see cref="TextBuilder"/>
        /// will contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="TextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="TextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public TextBuilder(StringBuilder? value, int startIndex, int length, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, startIndex, length, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="TextBuilder"/> will contain
        /// the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <remarks>
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="TextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).
        /// </remarks>
        public TextBuilder(ICharSequence? value) // Coverage for the JDK // J2N TODO: Add overloads to slice the ICharsequence and set capacity?
        {
            buffer = CreateBuffer().Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBuilder"/> with the specified <see cref="MutableTextBuffer"/>.
        /// </summary>
        /// <param name="buffer">The <see cref="MutableTextBuffer"/> to initialize the instance with.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        private protected TextBuilder(MutableTextBuffer buffer) // J2N TODO: Should we allow users to inject this?
        {
            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        }


        #endregion J2N Constructors


        /// <summary>
        /// Gets or sets a value indicating whether culture-sensitive operations use
        /// invariant defaults when the caller does not explicitly specify culture-
        /// specific behavior.
        /// </summary>
        /// <value>
        /// <see langword="false"/> to use the .NET default behavior of using the current
        /// culture for culture-sensitive operations; <see langword="true"/> to use
        /// invariant defaults instead. The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This setting affects culture-sensitive operations that rely on default
        /// formatting, parsing, casing, comparison, or other culture-specific behavior
        /// when the caller does not explicitly provide a culture, format provider,
        /// comparison option, or equivalent setting.
        /// <para/>
        /// Setting this property to <see langword="true"/> is recommended when porting
        /// Java applications that expect locale-independent behavior. Java APIs commonly
        /// use locale-independent defaults for operations such as numeric formatting,
        /// whereas .NET APIs generally use the current culture by default.
        /// <para/>
        /// This setting has no effect on operations where the caller explicitly supplies
        /// the culture-specific option to use, such as an
        /// <see cref="IFormatProvider"/>, <see cref="CultureInfo"/>, or
        /// <see cref="StringComparison"/> value.
        /// </remarks>
        public bool UseInvariantDefaults
        {
            get => buffer.UseInvariantDefaults;
            init => buffer.useInvariantDefaults = value;
        }

        #region Operator Overrides

        /// <summary>
        /// Defines an implicit conversion of a given <see cref="TextBuilder"/> to a read-only span of characters.
        /// </summary>
        /// <param name="value">A <see cref="TextBuilder"/> to implicitly convert.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlySpan<char>(TextBuilder? value) =>
            value != null ? value.AsSpan() : default;

        #endregion Operator Overrides

        #region ISpannable<char> Members

        bool ISpannable<char>.HasValue => true; // Cannot be null

        ReadOnlySpan<char> ISpannable<char>.AsSpan() => this.AsSpan();

        ReadOnlySpan<char> ISpannable<char>.AsSpan(int start) => this.AsSpan(start);

        ReadOnlySpan<char> ISpannable<char>.AsSpan(int start, int length) => this.AsSpan(start, length);

        #endregion ISpannable<char> Members
    }
}
