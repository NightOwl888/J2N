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
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Represents a mutable string of characters and provides access to the underlying memory, which
    /// uses a dedicated array pool for array reuse.
    /// </summary>
    /// <remarks>
    /// <see cref="PooledTextBuilder"/> differs from <see cref="StringBuilder"/> in the following ways:
    /// 
    /// <list type="bullet">
    ///     <item><description>
    ///         This implementation reuses buffers from an array pool and returns them to the pool when growing
    ///         or shrinking the backing buffer.
    ///     </description></item>
    ///     <item><description>
    ///         Rather than managing chunks of memory, <see cref="PooledTextBuilder"/> manages a single contiguous
    ///         block of <see cref="char"/>s.
    ///     </description></item>
    ///     <item><description>
    ///         Memory is directly accessible using <see cref="TextBuilderExtensions.AsSpan(TextBuilder?)"/> and
    ///         <see cref="TextBuilderExtensions.AsMemory(TextBuilder?)"/> overloads including the ability to slice.
    ///         There is no need to allocate memory and do a copy to call methods that require System.Memory types, such as
    ///         <see cref="ReadOnlySpan{T}"/> or to access a portion of the underlying chars.
    ///     </description></item>
    ///     <item><description>
    ///         <see cref="PooledTextBuilder"/> is implicitly convertible to <see cref="ReadOnlySpan{Char}"/> to allow
    ///         passing the builder to low-level APIs without needing special overloads that accept <see cref="PooledTextBuilder"/>.
    ///         This is similar to how <see cref="T:char[]"/> and <see cref="string"/> are implicitly converted to
    ///         <see cref="ReadOnlySpan{Char}"/>.
    ///     </description></item>
    ///     <item><description>
    ///         The <see cref="PooledTextBuilder(int)"/> and <see cref="PooledTextBuilder(int, int)"/> constructors allow
    ///         setting the initial capacity to zero, meaning no backing array is allocated unless it is needed.
    ///     </description></item>
    ///     <item><description>
    ///         Indexing through <see cref="TextBuilder.this[int]"/> is significantly faster than with <see cref="StringBuilder"/>.
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
    /// By default, <see cref="PooledTextBuilder"/> follows typical .NET culture-sensitive behavior. When porting Java applications,
    /// consider setting <see cref="TextBuilder.UseInvariantDefaults"/> during construction to use invariant defaults for culture-sensitive operations.
    /// <para/>
    /// Unlike most <see cref="IDisposable"/> implementations, calling <see cref="Dispose()"/> does not permanently invalidate the
    /// instance. The current character buffer is released to the configured <see cref="IArrayAllocator{Char}"/>, and a new buffer
    /// will be allocated automatically if the instance is used again. However, a final call to <see cref="Dispose()"/> is required
    /// to return the array to the pool before the instance goes out of scope to prevent leaking resources and/or undefined behavior.
    /// </remarks>
    public sealed partial class PooledTextBuilder : TextBuilder, IBufferWriter<char>, ISpannable<char>, ISpanCopyable<char>,
        ICopyable<char>, IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MutableTextBuffer CreateBuffer() => new(PooledArrayAllocator<char>.Uncleared)
        {
            ClearExposedBuffers = true
        };

        #region BCL Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class.
        /// </summary>
        /// <remarks>
        /// The string value of this instance is set to <see cref="string.Empty"/>, and the capacity is set to
        /// the implementation-specific default capacity.
        /// </remarks>
        public PooledTextBuilder()
            : base(CreateBuffer().Initialize())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class using the specified capacity.
        /// </summary>
        /// <param name="capacity">The suggested starting size of this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="TextBuilder.Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="PooledTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// The string value of this instance is set to <see cref="string.Empty"/>. If <paramref name="capacity"/> is zero,
        /// no backing array is allocated and the first mutation will grow the buffer, as needed.
        /// </remarks>
        /// <seealso cref="TextBuilder.Capacity"/>
        public PooledTextBuilder(int capacity)
            : base(CreateBuffer().Initialize(capacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class using the specified string.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="PooledTextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="PooledTextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</remarks>
        public PooledTextBuilder(string? value)
            : base(CreateBuffer().Initialize(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class with the specified string
        /// and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="PooledTextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="PooledTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="TextBuilder.Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="PooledTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="TextBuilder.Capacity"/>
        public PooledTextBuilder(string? value, int capacity)
            : base(CreateBuffer().Initialize(value, capacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class from the specified
        /// substring and capacity.
        /// </summary>
        /// <param name="value">The string that contains the substring used to initialize the value of this instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="PooledTextBuilder"/> will contain the empty
        /// string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="PooledTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="TextBuilder.Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="PooledTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="TextBuilder.Capacity"/>
        public PooledTextBuilder(string? value, int startIndex, int length, int capacity)
            : base(CreateBuffer().Initialize(value, startIndex, length, capacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class that starts with a specified capacity
        /// and can grow to a specified maximum.
        /// </summary>
        /// <param name="capacity">The suggested starting size of the <see cref="PooledTextBuilder"/>.</param>
        /// <param name="maxCapacity">The maximum number of characters the current string can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxCapacity"/> is less than one, <paramref name="capacity"/> is less than zero,
        /// or <paramref name="capacity"/> is greater than <paramref name="maxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="TextBuilder.Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="PooledTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, no backing array is allocated and the first mutation will
        /// grow the buffer, as needed.
        /// <para/>
        /// The <paramref name="maxCapacity"/> property defines the maximum number of characters that the current
        /// instance can hold. Its value is assigned to the <see cref="TextBuilder.MaxCapacity"/> property. If the number of
        /// characters to be stored in the current instance exceeds this <paramref name="maxCapacity"/> value,
        /// the <see cref="PooledTextBuilder"/> object does not allocate additional memory, but instead throws an exception.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="PooledTextBuilder"/> object by calling the <see cref="PooledTextBuilder(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="PooledTextBuilder"/> instance can grow beyond
        /// the value of its <see cref="TextBuilder.MaxCapacity"/> property. This can occur particularly when you call the <see cref="TextBuilderExtensions.Append{TBuilder}(TBuilder, string)"/>
        /// and <see cref="TextBuilderExtensions.AppendFormat{TBuilder}(TBuilder, string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="TextBuilder.Capacity"/>
        /// <seealso cref="TextBuilder.MaxCapacity"/>
        public PooledTextBuilder(int capacity, int maxCapacity)
            : base(CreateBuffer().Initialize(capacity, maxCapacity))
        {
        }

        #endregion BCL Constructors

        #region J2N Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">The length of <paramref name="value"/> is greater than the
        /// platform-specific maximum array capacity.</exception>
        /// <remarks>The characters from the span are copied to the heap memory of this instance.</remarks>
        public PooledTextBuilder(ReadOnlySpan<char> value)
            : base(CreateBuffer().Initialize(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="PooledTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// <para/>
        /// -or-
        /// <para/>
        /// The length of <paramref name="value"/> is greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="TextBuilder.Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="PooledTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="TextBuilder.Capacity"/>
        public PooledTextBuilder(ReadOnlySpan<char> value, int capacity)
            : base(CreateBuffer().Initialize(value, capacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class using the specified
        /// <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="PooledTextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="PooledTextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).
        /// </remarks>
        public PooledTextBuilder(StringBuilder? value)
            : base(CreateBuffer().Initialize(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class with the specified
        /// <see cref="StringBuilder"/> and capacity.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> used to initialize the value of the instance.
        /// If <paramref name="value"/>is <c>null</c>, the new <see cref="PooledTextBuilder"/> will contain
        /// the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="PooledTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="TextBuilder.Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="PooledTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="TextBuilder.Capacity"/>
        public PooledTextBuilder(StringBuilder? value, int capacity)
            : base(CreateBuffer().Initialize(value, capacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> class from the specified
        /// substring and capacity.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the substring used to initialize the
        /// value of this instance. If <paramref name="value"/> is <c>null</c>, the new <see cref="PooledTextBuilder"/>
        /// will contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="PooledTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="TextBuilder.Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="PooledTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="TextBuilder.Capacity"/>
        public PooledTextBuilder(StringBuilder? value, int startIndex, int length, int capacity)
            : base(CreateBuffer().Initialize(value, startIndex, length, capacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledTextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="PooledTextBuilder"/> will contain
        /// the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <remarks>
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="PooledTextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).
        /// </remarks>
        public PooledTextBuilder(ICharSequence? value) // Coverage for the JDK // J2N TODO: Add overloads to slice the ICharsequence and set capacity?
            : base(CreateBuffer().Initialize(value))
        {
        }

        #endregion J2N Constructors

        /// <summary>
        /// Releases ownership of the underlying array and returns it to the underlying array pool.
        /// </summary>
        /// <remarks>
        /// This method releases ownership of the underlying buffer back to the
        /// array pool. If the instance later allocates a
        /// new buffer through subsequent use, that buffer becomes owned by the instance and will
        /// likewise be released by a subsequent call to <see cref="Dispose()"/>.
        /// <para/>
        /// Calling <see cref="Dispose()"/> multiple times without allocating a new buffer between
        /// calls has no effect.
        /// <para/>
        /// This method guarantees that a particular buffer is returned to the allocator at most once.
        /// It does not make concurrent access to <see cref="PooledTextBuilder"/> thread-safe.
        /// </remarks>
        public void Dispose() => buffer.Dispose();
    }
}
