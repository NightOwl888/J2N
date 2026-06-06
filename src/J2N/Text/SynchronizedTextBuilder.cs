using J2N.Buffers;
using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Represents a mutable string of characters and provides access to the underlying memory.
    /// </summary>
    /// <remarks>
    /// <see cref="SynchronizedTextBuilder"/> differs from <see cref="StringBuilder"/> in the following ways:
    /// 
    /// <list type="bullet">
    ///     <item><description>
    ///         This implementation synchronizes operations using a lock.
    ///     </description></item>
    ///     <item><description>
    ///         Rather than managing chunks of memory, <see cref="SynchronizedTextBuilder"/> manages a single contiguous
    ///         block of <see cref="char"/>s.
    ///     </description></item>
    ///     <item><description>
    ///         Memory is directly accessible using <see cref="TextMemoryExtensions.AsSpan(SynchronizedTextBuilder?)"/> and
    ///         <see cref="TextMemoryExtensions.AsMemory(SynchronizedTextBuilder?)"/> overloads including the ability to slice.
    ///         So, there is no need to allocate memory to call methods that require System.Memory types, such as
    ///         <see cref="ReadOnlySpan{T}"/>. So, no allocation is necessary to read the results.
    ///     </description></item>
    ///     <item><description>
    ///         Indexing through <see cref="this[int]"/> is significantly faster than with <see cref="StringBuilder"/>.
    ///     </description></item>
    ///     <item><description>
    ///         Rather than optimizing for operations that require moving or copying characters,
    ///         this implementation optimizes for memory reuse, reducing array allocations.
    ///     </description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SynchronizedTextBuilder : IAppendable, ISpanAppendable, ICharSequence, IBufferWriter<char>
    {
        private readonly object syncRoot = new();
        internal readonly MutableTextBuffer buffer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MutableTextBuffer CreateBuffer() => new(UninitializedArrayAllocator<char>.Default)
        {
            ClearExposedBuffers = true
        };

        #region BCL Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class.
        /// </summary>
        /// <remarks>
        /// The string value of this instance is set to <see cref="string.Empty"/>, and the capacity is set to
        /// the implementation-specific default capacity.
        /// </remarks>
        public SynchronizedTextBuilder()
        {
            buffer = CreateBuffer().Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class using the specified capacity.
        /// </summary>
        /// <param name="capacity">The suggested starting size of this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="SynchronizedTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// The string value of this instance is set to <see cref="string.Empty"/>. If capacity is zero, the
        /// implementation-specific default capacity is used.</remarks>
        /// <seealso cref="Capacity"/>
        public SynchronizedTextBuilder(int capacity)
        {
            buffer = CreateBuffer().Initialize(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class using the specified string.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</remarks>
        public SynchronizedTextBuilder(string? value)
        {
            buffer = CreateBuffer().Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class with the specified string
        /// and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="SynchronizedTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="SynchronizedTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public SynchronizedTextBuilder(string? value, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class from the specified
        /// substring and capacity.
        /// </summary>
        /// <param name="value">The string that contains the substring used to initialize the value of this instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will contain the empty
        /// string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="SynchronizedTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="SynchronizedTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public SynchronizedTextBuilder(string? value, int startIndex, int length, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, startIndex, length, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class that starts with a specified capacity
        /// and can grow to a specified maximum.
        /// </summary>
        /// <param name="capacity">The suggested starting size of the <see cref="SynchronizedTextBuilder"/>.</param>
        /// <param name="maxCapacity">The maximum number of characters the current string can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxCapacity"/> is less than one, <paramref name="capacity"/> is less than zero,
        /// or <paramref name="capacity"/> is greater than <paramref name="maxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="SynchronizedTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// <para/>
        /// The <paramref name="maxCapacity"/> property defines the maximum number of characters that the current
        /// instance can hold. Its value is assigned to the <see cref="MaxCapacity"/> property. If the number of
        /// characters to be stored in the current instance exceeds this <paramref name="maxCapacity"/> value,
        /// the <see cref="SynchronizedTextBuilder"/> object does not allocate additional memory, but instead throws an exception.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="SynchronizedTextBuilder"/> object by calling the <see cref="SynchronizedTextBuilder(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="SynchronizedTextBuilder"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        /// <seealso cref="MaxCapacity"/>
        public SynchronizedTextBuilder(int capacity, int maxCapacity)
        {
            buffer = CreateBuffer().Initialize(capacity, maxCapacity);
        }

        #endregion BCL Constructors

        #region J2N Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <remarks>The characters from the span are copied to the heap memory of this instance.</remarks>
        public SynchronizedTextBuilder(ReadOnlySpan<char> value)
        {
            buffer = CreateBuffer().Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="SynchronizedTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="SynchronizedTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public SynchronizedTextBuilder(ReadOnlySpan<char> value, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class using the specified
        /// <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).
        /// </remarks>
        public SynchronizedTextBuilder(StringBuilder? value)
        {
            buffer = CreateBuffer().Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class with the specified
        /// <see cref="StringBuilder"/> and capacity.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> used to initialize the value of the instance.
        /// If <paramref name="value"/>is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will contain
        /// the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="SynchronizedTextBuilder"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="SynchronizedTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public SynchronizedTextBuilder(StringBuilder? value, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> class from the specified
        /// substring and capacity.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the substring used to initialize the
        /// value of this instance. If <paramref name="value"/> is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/>
        /// will contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="SynchronizedTextBuilder"/>.</param>
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
        /// value, the <see cref="SynchronizedTextBuilder"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public SynchronizedTextBuilder(StringBuilder? value, int startIndex, int length, int capacity)
        {
            buffer = CreateBuffer().Initialize(value, startIndex, length, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedTextBuilder"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will contain
        /// the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="SynchronizedTextBuilder"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</remarks>
        public SynchronizedTextBuilder(ICharSequence? value) // Coverage for the JDK // J2N TODO: Add overloads to slice the ICharsequence and set capacity?
        {
            buffer = CreateBuffer().Initialize(value);
        }

        #endregion J2N Constructors


        /// <summary>
        /// 
        /// Gets or sets a flag indicating to use invariant default settings when not otherwise specified by the user.
        /// This setting affects culture-aware features such as formatting and comparing.
        /// 
        /// </summary>
        public bool UseInvariantDefaults
        {
            get => buffer.UseInvariantDefaults;
            init => buffer.useInvariantDefaults = value;
        }

        #region ISpanAppendable Members

        ISpanAppendable ISpanAppendable.Append(ReadOnlySpan<char> value) => Append(value);

        #endregion ISpanAppendable Members

        #region IAppendable Members
        IAppendable IAppendable.Append(char value) => Append(value);

        IAppendable IAppendable.Append(string? value) => Append(value);

        IAppendable IAppendable.Append(string? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(StringBuilder? value) => Append(value);

        IAppendable IAppendable.Append(StringBuilder? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(char[]? value) => Append(value);

        IAppendable IAppendable.Append(char[]? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(ICharSequence? value) => Append(value);

        IAppendable IAppendable.Append(ICharSequence? value, int startIndex, int count) => Append(value, startIndex, count);

        #endregion IAppendable Members

        #region ICharSequence Members

        bool ICharSequence.HasValue => true;

        #endregion ICharSequence Members


        // ChunkEnumerator supports both the IEnumerable and IEnumerator pattern so foreach
        // works (see GetChunks).  It needs to be public (so the compiler can use it
        // when building a foreach statement) but users typically don't use it explicitly.
        // (which is why it is a nested type).

        /// <summary>
        /// Supports simple iteration over the chunks of an <see cref="MutableTextBuffer"/> instance.
        /// </summary>
        /// <remarks>
        /// A <see cref="ChunkEnumerator"/> is returned by the <see cref="GetChunks()"/> method. It supports both the
        /// <see cref="System.Collections.IEnumerable"/> and <see cref="System.Collections.IEnumerator"/> patterns so
        /// that the chunks can be enumerated with foreach in C# or For Each in Visual Basic.
        /// <para/>
        /// <see cref="ChunkEnumerator"/> is a public structure so that language compilers can use it to build a
        /// foreach statement. However, developers typically don't use it explicitly (which is why it is a nested type).
        /// </remarks>
        public struct ChunkEnumerator
        {
            private readonly MutableTextBuffer _firstChunk;
            private MutableTextBuffer? _currentChunk;

            /// <summary>
            /// Provides an <see cref="System.Collections.IEnumerable.GetEnumerator()"/> implementation that
            /// returns <c>this</c> as the <see cref="System.Collections.IEnumerator"/>.
            /// </summary>
            /// <returns>An enumerator object that can be used to iterate through the chunks.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)] // Only here to make foreach work
#pragma warning disable IDE0251 // Make member 'readonly'
            public ChunkEnumerator GetEnumerator() => this;
#pragma warning restore IDE0251 // Make member 'readonly'

            /// <summary>
            /// Advances the enumerator to the next chunk in the collection.
            /// </summary>
            /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
            /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                if (_currentChunk == _firstChunk)
                {
                    return false;
                }

                _currentChunk = _firstChunk;
                return true;
            }

            /// <summary>
            /// Gets the chunk and the current position of the collection.
            /// </summary>
            /// <value>The chunk at the current position of the collection.</value>
            public ReadOnlyMemory<char> Current
            {
                get
                {
                    if (_currentChunk == null)
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();

                    return new ReadOnlyMemory<char>(_currentChunk.m_Chars, 0, _currentChunk.m_Position);
                }
            }

            internal ChunkEnumerator(MutableTextBuffer stringBuilder)
            {
                Debug.Assert(stringBuilder != null);
                _firstChunk = stringBuilder!;
                _currentChunk = null;   // MoveNext will find the last chunk if we do this.
            }
        }

        /// <summary>
        /// 
        ///  Returns an object that can be used to iterate through the chunks of characters represented in a
        ///  <see cref="ReadOnlyMemory{Char}" /> created from this <see cref="SynchronizedTextBuilder" /> instance.
        /// 
        /// </summary>
        /// <returns>
        /// An enumerator for the chunks in the <see cref="ReadOnlyMemory{Char}" />.
        /// </returns>
        /// <remarks>
        /// This API is for compatibility with <c>StringBuilder.GetChuncks()</c> method.
        ///  <see cref="SynchronizedTextBuilder" /> will never have more than a single chunk of memory so it is generally more efficient
        ///  to use <see cref="TextMemoryExtensions.AsSpan(SynchronizedTextBuilder)" /> or
        ///  <see cref="TextMemoryExtensions.AsMemory(SynchronizedTextBuilder)" /> when you need to access the underlying memory.
        /// </remarks>
        public ChunkEnumerator GetChunks() => new(buffer);

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="SynchronizedTextBuilder"/>.
        /// </summary>
        public object SyncRoot => syncRoot;
    }
}
