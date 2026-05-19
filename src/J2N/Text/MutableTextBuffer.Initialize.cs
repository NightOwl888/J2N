using System;
using System.Text;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        #region BCL Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class.
        /// </summary>
        /// <remarks>
        /// The string value of this instance is set to <see cref="string.Empty"/>, and the capacity is set to
        /// the implementation-specific default capacity.
        /// </remarks>
        public MutableTextBuffer Initialize()
        {
            m_MaxCapacity = int.MaxValue;
            // J2N: We assume that subclasses will not expose or call this constructor if they want
            // full control over how the buffer is allocated.
            m_Chars = new char[DefaultCapacity];
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class using the specified capacity.
        /// </summary>
        /// <param name="capacity">The suggested starting size of this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// The string value of this instance is set to <see cref="string.Empty"/>. If capacity is zero, the
        /// implementation-specific default capacity is used.</remarks>
        /// <seealso cref="Capacity"/>
        public MutableTextBuffer Initialize(int capacity)
            => Initialize(capacity, int.MaxValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class using the specified string.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="MutableTextBuffer"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="MutableTextBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</remarks>
        public MutableTextBuffer Initialize(string? value)
            => Initialize(value, DefaultCapacity);

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class with the specified string
        /// and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="MutableTextBuffer"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="MutableTextBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public MutableTextBuffer Initialize(string? value, int capacity)
            => Initialize(value, 0, value?.Length ?? 0, capacity);

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class from the specified
        /// substring and capacity.
        /// </summary>
        /// <param name="value">The string that contains the substring used to initialize the value of this instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="MutableTextBuffer"/> will contain the empty
        /// string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="MutableTextBuffer"/>.</param>
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
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public MutableTextBuffer Initialize(string? value, int startIndex, int length, int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);

            value ??= string.Empty;

            if (startIndex > value.Length - length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(length, ExceptionArgument.length);
            }

            m_MaxCapacity = int.MaxValue;

            int minimumCapacity = length + DefaultCapacity;
            if (capacity < minimumCapacity)
                capacity = minimumCapacity;

            // J2N: We assume that subclasses will not expose or call this constructor if they want
            // full control over how the buffer is allocated.
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            m_Chars = GC.AllocateUninitializedArray<char>(capacity); // J2N NOTE: If we decide to expose the actual array, we must use new char[] here.
#else
            m_Chars = new char[capacity];
#endif
            m_Position = length;

            value.AsSpan(startIndex, length).CopyTo(m_Chars);
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class that starts with a specified capacity
        /// and can grow to a specified maximum.
        /// </summary>
        /// <param name="capacity">The suggested starting size of the <see cref="MutableTextBuffer"/>.</param>
        /// <param name="maxCapacity">The maximum number of characters the current string can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxCapacity"/> is less than one, <paramref name="capacity"/> is less than zero,
        /// or <paramref name="capacity"/> is greater than <paramref name="maxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// <para/>
        /// The <paramref name="maxCapacity"/> property defines the maximum number of characters that the current
        /// instance can hold. Its value is assigned to the <see cref="MaxCapacity"/> property. If the number of
        /// characters to be stored in the current instance exceeds this <paramref name="maxCapacity"/> value,
        /// the <see cref="MutableTextBuffer"/> object does not allocate additional memory, but instead throws an exception.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="Initialize(int, int)"/>
        /// method, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        /// <seealso cref="MaxCapacity"/>
        public MutableTextBuffer Initialize(int capacity, int maxCapacity)
        {
            if (capacity > maxCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException(capacity, ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_Capacity);
            if (maxCapacity <= 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegativeNonZero(maxCapacity, ExceptionArgument.maxCapacity);
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);

            if (capacity == 0)
            {
                capacity = Math.Min(DefaultCapacity, maxCapacity);
            }

            m_MaxCapacity = maxCapacity;
            // J2N: We assume that subclasses will not expose or call this method if they want
            // full control over how the buffer is allocated.
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            m_Chars = GC.AllocateUninitializedArray<char>(capacity); // J2N NOTE: If we decide to expose the actual array, we must use new char[] here.
#else
            m_Chars = new char[capacity];
#endif
            return this;
        }

        #endregion BCL Constructors

        #region J2N Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <remarks>The characters from the span are copied to the heap memory of this instance.</remarks>
        public MutableTextBuffer Initialize(ReadOnlySpan<char> value)
            => Initialize(value, DefaultCapacity);

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="MutableTextBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public MutableTextBuffer Initialize(ReadOnlySpan<char> value, int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);

            int length = value.Length;
            m_MaxCapacity = int.MaxValue;

            int minimumCapacity = length + DefaultCapacity;
            if (capacity < minimumCapacity)
                capacity = minimumCapacity;


            // J2N: We assume that subclasses will not expose or call this method if they want
            // full control over how the buffer is allocated.
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            m_Chars = GC.AllocateUninitializedArray<char>(capacity); // J2N NOTE: If we decide to expose the actual array, we must use new char[] here.
#else
            m_Chars = new char[capacity];
#endif
            m_Position = length;

            value.CopyTo(m_Chars);
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class using the specified
        /// <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="MutableTextBuffer"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="MutableTextBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).
        /// <para/>
        /// If value is non-<c>null</c>, <see cref="Capacity"/> is set using the <see cref="StringBuilder.Capacity"/>.
        /// </remarks>
        public MutableTextBuffer Initialize(StringBuilder? value)
        {
            m_MaxCapacity = int.MaxValue;

            if (value is null)
            {
                m_Position = 0;
                m_Chars = new char[DefaultCapacity];
                return this;
            }

            int length = value.Length;
            int capacity = length + DefaultCapacity;

            // J2N: We assume that subclasses will not expose or call this constructor if they want
            // full control over how the buffer is allocated.
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            m_Chars = GC.AllocateUninitializedArray<char>(capacity); // J2N NOTE: If we decide to expose the actual array, we must use new char[] here.
#else
            m_Chars = new char[capacity];
#endif
            value.CopyTo(0, m_Chars, 0, length);
            m_Position = length;
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class with the specified
        /// <see cref="StringBuilder"/> and capacity.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> used to initialize the value of the instance.
        /// If <paramref name="value"/>is <c>null</c>, the new <see cref="MutableTextBuffer"/> will contain
        /// the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="MutableTextBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public MutableTextBuffer Initialize(StringBuilder? value, int capacity)
            => Initialize(value, 0, value?.Length ?? 0, capacity);

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class from the specified
        /// substring and capacity.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the substring used to initialize the
        /// value of this instance. If <paramref name="value"/> is <c>null</c>, the new <see cref="MutableTextBuffer"/>
        /// will contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="MutableTextBuffer"/>.</param>
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
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        public MutableTextBuffer Initialize(StringBuilder? value, int startIndex, int length, int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);

            m_MaxCapacity = int.MaxValue;

            if (startIndex > value?.Length - length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);
            }

            int minimumCapacity = length + DefaultCapacity;
            if (capacity < minimumCapacity)
                capacity = minimumCapacity;

            // J2N: We assume that subclasses will not expose or call this constructor if they want
            // full control over how the buffer is allocated.
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            m_Chars = GC.AllocateUninitializedArray<char>(capacity); // J2N NOTE: If we decide to expose the actual array, we must use new char[] here.
#else
            m_Chars = new char[capacity];
#endif

            if (value is null)
            {
                m_Position = 0;
                return this;
            }

            value.CopyTo(startIndex, m_Chars, 0, length);
            m_Position = length;
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="MutableTextBuffer"/> will contain
        /// the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="MutableTextBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</remarks>
        public MutableTextBuffer Initialize(ICharSequence? value) // Coverage for the JDK // J2N TODO: Add overloads to slice the ICharsequence and set capacity?
        {
            m_MaxCapacity = int.MaxValue;
            int length = value?.Length ?? 0;
            int capacity = length + DefaultCapacity;

            // J2N: We assume that subclasses will not expose or call this constructor if they want
            // full control over how the buffer is allocated.
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            m_Chars = GC.AllocateUninitializedArray<char>(capacity); // J2N NOTE: If we decide to expose the actual array, we must use new char[] here.
#else
            m_Chars = new char[capacity];
#endif

            if (value is null || !value.HasValue)
            {
                m_Position = 0;
                return this;
            }

            if (value is StringCharSequence str)
            {
                str.Value!.CopyTo(0, m_Chars, 0, str.Length);
            }
            else if (value is StringBuilderCharSequence sb)
            {
                sb.Value!.CopyTo(0, m_Chars, 0, sb.Length);
            }
            else if (value is MutableTextBuffer osb)
            {
                osb.CopyTo(0, m_Chars, 0, osb.Length);
            }
            else if (value is CharArrayCharSequence chars)
            {
                chars.Value!.CopyTo(m_Chars, 0);
            }
            else if (value is StringBuffer sbuffer)
            {
                lock (sbuffer.SyncRoot)
                {
                    sbuffer.builder.CopyTo(0, m_Chars, 0, sbuffer.Length);
                }
            }
            else
            {
                for (int i = 0; i < value.Length; i++)
                {
                    m_Chars[i] = value[i];
                }
            }

            m_Position = length;
            return this;
        }

        #endregion J2N Constructors
    }
}
