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
    ///         Memory is directly accessible using <see cref="SynchronizedTextBuilderExtensions.AsSpan(SynchronizedTextBuilder?)"/> and
    ///         <see cref="SynchronizedTextBuilderExtensions.AsMemory(SynchronizedTextBuilder?)"/> overloads including the ability to slice.
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
    /// <para/>
    /// By default, <see cref="SynchronizedTextBuilder"/> follows typical .NET culture-sensitive behavior. When porting Java applications,
    /// consider setting <see cref="UseInvariantDefaults"/> during construction to use invariant defaults for culture-sensitive operations.
    /// </remarks>
    internal partial class SynchronizedTextBuilder : IBufferWriter<char>,
        ICopyable<char>, ISpanCopyable<char>
    {
        private readonly object syncRoot = new();
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
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
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="SynchronizedTextBuilderExtensions.Append{TBuilder}(TBuilder, string)"/>
        /// and <see cref="SynchronizedTextBuilderExtensions.AppendFormat{TBuilder}(TBuilder, string, object)"/> methods to append small strings.
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
        /// <exception cref="ArgumentOutOfRangeException">The length of <paramref name="value"/> is greater than the
        /// platform-specific maximum array capacity.</exception>
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

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="SynchronizedTextBuilder"/>.
        /// </summary>
        public object SyncRoot => syncRoot;
    }
}
