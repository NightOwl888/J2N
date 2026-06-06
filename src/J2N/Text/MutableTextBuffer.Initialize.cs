using J2N.CodeGeneration;
using J2N.Collections;
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
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
        public MutableTextBuffer Initialize()
        {
            m_MaxCapacity = Arrays.MaxArrayLength;
            m_Chars = allocator.Allocate(DefaultCapacity);
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class using the specified capacity.
        /// </summary>
        /// <param name="capacity">The suggested starting size of this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be stored
        /// in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/> property.
        /// If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/> value,
        /// the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// The string value of this instance is set to <see cref="string.Empty"/>. If capacity is zero, the
        /// implementation-specific default capacity is used.</remarks>
        /// <seealso cref="Capacity"/>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
        public MutableTextBuffer Initialize(int capacity)
            => Initialize(capacity, Arrays.MaxArrayLength);

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class using the specified string.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance. If <paramref name="value"/>
        /// is <c>null</c>, the new <see cref="MutableTextBuffer"/> will contain the empty string (that is, it
        /// contains <see cref="string.Empty"/>).</param>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="MutableTextBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</remarks>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
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
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
        public MutableTextBuffer Initialize(string? value, int startIndex, int length, int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);

            m_MaxCapacity = Arrays.MaxArrayLength;
            value ??= string.Empty;

            if (capacity > m_MaxCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException(capacity, ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_Capacity);

            if (startIndex > value.Length - length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(length, ExceptionArgument.length);
            }

            uint minimumCapacity = (uint)length + DefaultCapacity;

            // If the minimum capacity is greater than the maximum capacity, try again with the length.
            // We assume the user doesn't intend to append anything if length <= MaxCapacity but this is still
            // valid to create an instance with the whole length.
            if (minimumCapacity > m_MaxCapacity)
                minimumCapacity = (uint)length;

            // A valid string instance can never have a Length greater than Arrays.MaxArrayLength,
            // therefore once startIndex/length have been validated against the string,
            // length is guaranteed to be <= m_MaxCapacity.

            if ((uint)capacity < minimumCapacity)
                capacity = (int)minimumCapacity;

            m_Chars = allocator.Allocate(capacity);
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
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
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
            m_Chars = allocator.Allocate(capacity);
            return this;
        }

        #endregion BCL Constructors

        #region J2N Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">The length of <paramref name="value"/> is greater than the
        /// platform-specific maximum array capacity.</exception>
        /// <remarks>The characters from the span are copied to the heap memory of this instance.</remarks>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
        public MutableTextBuffer Initialize(ReadOnlySpan<char> value)
            => Initialize(value, DefaultCapacity);

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> with the specified sequence of characters.
        /// </summary>
        /// <param name="value">The characters used to initialize this instance.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="MutableTextBuffer"/>.</param>
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
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
        public MutableTextBuffer Initialize(ReadOnlySpan<char> value, int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);

            m_MaxCapacity = Arrays.MaxArrayLength;

            if (capacity > m_MaxCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException(capacity, ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_Capacity);

            int length = value.Length;
            uint minimumCapacity = (uint)length + DefaultCapacity;

            // If the minimum capacity is greater than the maximum capacity, try again with the length.
            // We assume the user doesn't intend to append anything if length <= MaxCapacity but this is still
            // valid to create an instance with the whole length.
            if (minimumCapacity > m_MaxCapacity)
                minimumCapacity = (uint)length;

            if (minimumCapacity > m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(length, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
            }

            if (capacity < minimumCapacity)
                capacity = (int)minimumCapacity;

            m_Chars = allocator.Allocate(capacity);
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
        /// <exception cref="ArgumentOutOfRangeException">The length of <paramref name="value"/> is greater than the
        /// platform-specific maximum array capacity.</exception>
        /// <remarks>If <paramref name="value"/> is <c>null</c>, the new <see cref="MutableTextBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).
        /// </remarks>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
        public MutableTextBuffer Initialize(StringBuilder? value)
        {
            m_MaxCapacity = Arrays.MaxArrayLength;

            if (value is null)
            {
                m_Position = 0;
                m_Chars = allocator.Allocate(DefaultCapacity);
                return this;
            }

            int length = value.Length;
            uint minimumCapacity = (uint)length + DefaultCapacity;

            // If the minimum capacity is greater than the maximum capacity, try again with the length.
            // We assume the user doesn't intend to append anything if length <= MaxCapacity but this is still
            // valid to create an instance with the whole length.
            if (minimumCapacity > m_MaxCapacity)
                minimumCapacity = (uint)length;

            if (minimumCapacity > m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(length, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
            }

            m_Chars = allocator.Allocate((int)minimumCapacity);
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero or greater than the platform-specific maximum array capacity.
        /// </exception>
        /// <remarks>The <paramref name="capacity"/> parameter defines the maximum number of characters that can be
        /// stored in the memory allocated by the current instance. Its value is assigned to the <see cref="Capacity"/>
        /// property. If the number of characters to be stored in the current instance exceeds this <paramref name="capacity"/>
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
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
        /// value, the <see cref="MutableTextBuffer"/> object allocates additional memory to store them.
        /// <para/>
        /// If <paramref name="capacity"/> is zero, the implementation-specific default capacity is used.
        /// </remarks>
        /// <seealso cref="Capacity"/>
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
        public MutableTextBuffer Initialize(StringBuilder? value, int startIndex, int length, int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);

            m_MaxCapacity = Arrays.MaxArrayLength;

            if (capacity > m_MaxCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException(capacity, ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_Capacity);

            if (startIndex > value?.Length - length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);
            }

            uint minimumCapacity = (uint)length + DefaultCapacity;

            // If the minimum capacity is greater than the maximum capacity, try again with the length.
            // We assume the user doesn't intend to append anything if length <= MaxCapacity but this is still
            // valid to create an instance with the whole length.
            if (minimumCapacity > m_MaxCapacity)
                minimumCapacity = (uint)length;

            if (minimumCapacity > m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(length, ExceptionArgument.length, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity); // J2N TODO: Tests
            }

            if ((uint)capacity < minimumCapacity)
                capacity = (int)minimumCapacity;

            m_Chars = allocator.Allocate(capacity);

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
        [CodeGenerationIgnore]
        [CodeGenerationConstructor]
        public MutableTextBuffer Initialize(ICharSequence? value) // Coverage for the JDK // J2N TODO: Add overloads to slice the ICharsequence and set capacity?
        {
            m_MaxCapacity = Arrays.MaxArrayLength;
            int length = value?.Length ?? 0;
            int capacity = length + DefaultCapacity;

            m_Chars = allocator.Allocate(capacity);

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
