using J2N.Buffers;
using J2N.Collections.Generic;
using J2N.Numerics;
using J2N.Runtime.CompilerServices;
using J2N.Runtime.InteropServices;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable CS1591 // J2N TODO: Finish docs

namespace J2N.Text
{
    /// <summary>
    /// Represents a mutable string of characters and provides access to the underlying memory.
    /// </summary>
    /// <remarks>
    /// <see cref="OpenStringBuilder"/> differs from <see cref="StringBuilder"/> in the following ways:
    /// 
    /// <list type="bullet">
    ///     <item><description>
    ///         Rather than managing chunks of memory, <see cref="OpenStringBuilder"/> manages a single contigouous
    ///         block of <see cref="char"/>s.
    ///     </description></item>
    ///     <item><description>
    ///         Memory is directly accessible using <see cref="MemoryExtensions.AsSpan(OpenStringBuilder)"/> and
    ///         <see cref="MemoryExtensions.AsMemory(OpenStringBuilder)"/> overloads including the ability to slice.
    ///         So, there is no need to allocate memory to call methods that require System.Memory types, such as
    ///         <see cref="ReadOnlySpan{T}"/>.
    ///     </description></item>
    ///     <item><description>
    ///         Indexing through <see cref="this[int]"/> is significantly faster than with <see cref="StringBuilder"/>.
    ///     </description></item>
    /// </list>
    /// </remarks>
    public partial class OpenStringBuilder : IAppendable, ISpanAppendable, ICharSequence
                                                                          //, IEnumerable<char> // ICU4N TODO: Implement?

    {
        private const int CharStackBufferSize = 32;

        /// <summary>
        /// The character buffer.
        /// </summary>
        internal char[] m_Chars;

        /// <summary>
        /// The current position of the cursor. Also represents the current length.
        /// </summary>
        internal int m_Position;

        /// <summary>
        /// The maximum capacity this builder is allowed to have.
        /// </summary>
        internal int m_MaxCapacity;

        /// <summary>
        /// The default capacity of a <see cref="StringBuilder"/>.
        /// </summary>
        internal const int DefaultCapacity = 16;

        #region BCL Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuilder"/> class.
        /// </summary>
        public OpenStringBuilder()
        {
            m_MaxCapacity = int.MaxValue;
            m_Chars = new char[DefaultCapacity];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenStringBuilder"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of this builder.</param>
        public OpenStringBuilder(int capacity)
            : this(capacity, int.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenStringBuilder"/> class.
        /// </summary>
        /// <param name="value">The initial contents of this builder.</param>
        public OpenStringBuilder(string? value)
            : this(value.AsSpan(), DefaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenStringBuilder"/> class.
        /// </summary>
        /// <param name="value">The initial contents of this builder.</param>
        /// <param name="capacity">The initial capacity of this builder.</param>
        public OpenStringBuilder(string? value, int capacity)
            : this(value.AsSpan(), capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenStringBuilder"/> class.
        /// </summary>
        /// <param name="value">The initial contents of this builder.</param>
        /// <param name="startIndex">The index to start in <paramref name="value"/>.</param>
        /// <param name="length">The number of characters to read in <paramref name="value"/>.</param>
        /// <param name="capacity">The initial capacity of this builder.</param>
        public OpenStringBuilder(string? value, int startIndex, int length, int capacity)
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
            if (capacity == 0)
            {
                capacity = DefaultCapacity;
            }
            capacity = Math.Max(capacity, length);

            m_Chars = AllocateArray(capacity);
            m_Position = length;

            value.AsSpan(startIndex, length).CopyTo(m_Chars);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuilder"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of this builder.</param>
        /// <param name="maxCapacity">The maximum capacity of this builder.</param>
        public OpenStringBuilder(int capacity, int maxCapacity)
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
            m_Chars = AllocateArray(capacity);
        }

        #endregion BCL Constructors

        #region J2N Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenStringBuilder"/> class.
        /// </summary>
        /// <param name="value">The initial contents of this builder.</param>
        public OpenStringBuilder(ReadOnlySpan<char> value)
            : this(value, DefaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenStringBuilder"/> class.
        /// </summary>
        /// <param name="value">The initial contents of this builder.</param>
        /// <param name="capacity">The initial capacity of this builder.</param>
        public OpenStringBuilder(ReadOnlySpan<char> value, int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);

            int length = value.Length;
            m_MaxCapacity = int.MaxValue;
            if (capacity == 0)
            {
                capacity = length + DefaultCapacity;
            }
            capacity = Math.Max(capacity, length + DefaultCapacity);

            m_Chars = AllocateArray(capacity);
            m_Position = length;

            value.CopyTo(m_Chars);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenStringBuilder"/> class.
        /// </summary>
        /// <param name="value">The initial contents of this builder.</param>
        public OpenStringBuilder(StringBuilder? value)
        {
            m_MaxCapacity = int.MaxValue;

            if (value is null)
            {
                m_Position = 0;
                m_Chars = new char[DefaultCapacity];
                return;
            }

            int length = value.Length;
            m_Chars = AllocateArray(value.Capacity);
            value.CopyTo(0, m_Chars, 0, length);
            m_Position = length;
        }

        public OpenStringBuilder(StringBuilder? value, int startIndex, int length)
            : this(value, startIndex, length, DefaultCapacity)
        {
        }

        public OpenStringBuilder(StringBuilder? value, int startIndex, int length, int capacity)
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

            if (value is null)
            {
                m_Position = 0;
                m_Chars = new char[capacity];
                return;
            }

            m_Chars = AllocateArray(capacity);
            m_Position = length;

            value.CopyTo(startIndex, m_Chars, 0, length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenStringBuilder"/> class.
        /// </summary>
        /// <param name="value">The initial contents of this builder.</param>
        public OpenStringBuilder(ICharSequence? value)
        {
            m_MaxCapacity = int.MaxValue;
            int length = value?.Length ?? 0;
            int capacity = length + DefaultCapacity;

            if (value is null || !value.HasValue)
            {
                m_Position = 0;
                m_Chars = new char[capacity];
                return;
            }

            m_Chars = AllocateArray(capacity);
            m_Position = length;

            if (value is StringCharSequence str)
            {
                str.Value!.CopyTo(0, m_Chars, 0, str.Length);
            }
            else if (value is StringBuilderCharSequence sb)
            {
                sb.Value!.CopyTo(0, m_Chars, 0, sb.Length);
            }
            else if (value is OpenStringBuilder osb)
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

            m_Chars.AsSpan(value.Length).Fill('\0');
        }

        protected OpenStringBuilder(char[] initialBuffer) : this(initialBuffer, initialLength: 0) { }

        protected OpenStringBuilder(char[] initialBuffer, int initialLength)
        {
            m_Chars = initialBuffer ?? throw new ArgumentNullException(nameof(initialBuffer));
            m_Position = initialLength;
        }

        #endregion J2N Constructors

        private void MakeRoom(int index, int count/*, out Span<char> chunk, out int indexInChunk,bool doNotMoveFollowingChars*/)
        {
            //AssertInvariants();
            Debug.Assert(count > 0);
            Debug.Assert(index >= 0);

            if (count + Length > m_MaxCapacity || count + Length < count)
            {
                throw new ArgumentOutOfRangeException("requiredLength", SR.ArgumentOutOfRange_SmallCapacity);
            }

            //chunk = m_Chars.AsSpan();
            //indexInChunk = index;

            // Cool, we have some space in this block, and we don't have to copy much to get at it, so go ahead and use it.
            // This typically happens when someone repeatedly inserts small strings at a spot (usually the absolute front) of the buffer.
            if (/*!doNotMoveFollowingChars &&*/ m_Position <= DefaultCapacity * 2 && m_Chars.Length - m_Position >= count)
            {
                for (int i = m_Position; i > index;)
                {
                    --i;
                    m_Chars[i + count] = m_Chars[i];
                }
                m_Position += count;
                return;
            }

            // Allocate the new array
            char[] newArray = AllocateArray(CalculateNewArrayLength(count));


            if (m_Position > 0)
            {
                // Copy the head of the current buffer to the new buffer.
                int copyCount1 = index; //Math.Min(count, index);
                if (copyCount1 > 0)
                {
                    new ReadOnlySpan<char>(m_Chars, 0, copyCount1).CopyTo(newArray);
                }

                // Copy the tail of the current buffer to the new buffer, leaving a gap of length count.
                int copyCount2 = copyCount1 + count;
                if (copyCount2 > 0)
                {
                    new ReadOnlySpan<char>(m_Chars, copyCount1, m_Position - copyCount1).CopyTo(newArray.AsSpan(copyCount2));
                }
            }

            // Wire in the new array
            m_Chars = newArray;
            m_Position += count;

            //AssertInvariants();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char[] AllocateArray(int capacity, bool useUninitialized = true)
        {
            return
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
                useUninitialized ? GC.AllocateUninitializedArray<char>(capacity) :
#endif
                new char[capacity];
        }

        public int Capacity
        {
            get => m_Chars.Length;
            set
            {
                if (value < 0)
                    ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(value, ExceptionArgument.value);
                if (value > MaxCapacity)
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_Capacity);
                if (value < Length)
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);

                if (Capacity != value)
                {
                    m_Chars = ReplaceBuffer(m_Chars.AsSpan(0, m_Position), newCapacity: value);
                }
            }
        }

        /// <summary>
        /// Gets the maximum capacity this builder is allowed to have.
        /// </summary>
        public int MaxCapacity => m_MaxCapacity;

        /// <summary>
        /// Ensures that the capacity of this builder is at least the specified value.
        /// </summary>
        /// <param name="capacity">The new capacity for this builder.</param>
        /// <remarks>
        /// If <paramref name="capacity"/> is less than or equal to the current capacity of
        /// this builder, the capacity remains unchanged.
        /// </remarks>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);

            int currentCapacity = Capacity;
            if (currentCapacity < capacity)
            {
                int twice = (currentCapacity << 1) + 2;
                Capacity = twice > capacity ? twice : capacity;
            }
            return Capacity;
        }

        public override string ToString()
        {
            //AssertInvariants();

            if (Length == 0)
            {
                return string.Empty;
            }

            return m_Chars.AsSpan(0, m_Position).ToString();
        }

        public string ToString(int startIndex)
        {
            if ((uint)startIndex > this.Length)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual();

            //AssertInvariants();
            return m_Chars.AsSpan(startIndex, m_Position - startIndex).ToString();
        }

        /// <summary>
        /// Creates a string from a substring of this builder.
        /// </summary>
        /// <param name="startIndex">The index to start in this builder.</param>
        /// <param name="length">The number of characters to read in this builder.</param>
        public string ToString(int startIndex, int length)
        {
            int currentLength = this.Length;
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (startIndex > currentLength)
                ThrowHelper.ThrowArgumentOutOfRangeException(startIndex, ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_StartIndexLargerThanLength);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > currentLength - length)
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            //AssertInvariants();
            return m_Chars.AsSpan(startIndex, length).ToString();
        }

        public OpenStringBuilder Clear()
        {
            this.Length = 0;
            return this;
        }

        /// <summary>
        /// Gets or sets the length of this builder.
        /// </summary>
        public int Length
        {
            get => m_Position;
            set
            {
                // If the new length is less than 0 or greater than our Maximum capacity, bail.
                if (value < 0)
                    ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(value, ExceptionArgument.value);
                if (value > MaxCapacity)
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);

                int delta = value - Length;
                if (delta > 0)
                {
                    // Pad ourselves with null characters.
                    Append('\0', delta);
                }
                else
                {
                    if (value > Capacity)
                    {
                        Grow(value - Capacity);
                    }
                    m_Position = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the character at the specified character position in this instance.
        /// </summary>
        /// <param name="index">The position of the character.</param>
        /// <returns>The Unicode character at position <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside
        /// the bounds of this instance while setting a character.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is outside the bounds
        /// of this instance while getting a character.</exception>
        [IndexerName("Chars")]
        public char this[int index]
        {
            get
            {
                if ((uint)index >= (uint)m_Position)
                {
                    ThrowHelper.ThrowIndexOutOfRangeException();
                }

                return m_Chars[index];
            }
            set
            {
                if ((uint)index >= (uint)m_Position)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index, ExceptionArgument.index);
                }
                m_Chars[index] = value;
            }
        }

        public ChunkEnumerator GetChunks() => new ChunkEnumerator(this);


        // ChunkEnumerator supports both the IEnumerable and IEnumerator pattern so foreach
        // works (see GetChunks).  It needs to be public (so the compiler can use it
        // when building a foreach statement) but users typically don't use it explicitly.
        // (which is why it is a nested type).

        public struct ChunkEnumerator
        {
            private readonly OpenStringBuilder _firstChunk;
            private OpenStringBuilder? _currentChunk;


            // Implement IEnumerable.GetEnumerator() to return  'this' as the IEnumerator
            [EditorBrowsable(EditorBrowsableState.Never)] // Only here to make foreach work
            public ChunkEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                if (_currentChunk == _firstChunk)
                {
                    return false;
                }

                _currentChunk = _firstChunk;
                return true;
            }

            public ReadOnlyMemory<char> Current
            {
                get
                {
                    if (_currentChunk == null)
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();

                    return new ReadOnlyMemory<char>(_currentChunk.m_Chars, 0, _currentChunk.m_Position);
                }
            }

            internal ChunkEnumerator(OpenStringBuilder stringBuilder)
            {
                Debug.Assert(stringBuilder != null);
                _firstChunk = stringBuilder!;
                _currentChunk = null;   // MoveNext will find the last chunk if we do this.
            }
        }

        public OpenStringBuilder Append(char value, int repeatCount)
        {
            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            if (repeatCount == 0)
            {
                return this;
            }

            char[] chars = m_Chars;
            int pos = m_Position;

            // Try to fit the whole repeatCount in the current chunk
            // Use the same check as Span<T>.Slice for 64-bit so it can be folded
            // Since repeatCount can't be negative, there's no risk for it to overflow on 32 bit
            if (((nuint)(uint)pos + (nuint)(uint)repeatCount) <= (nuint)(uint)chars.Length)
            {
                chars.AsSpan(pos, repeatCount).Fill(value);
                m_Position += repeatCount;
            }
            else
            {
                AppendWithExpansion(value, repeatCount);
            }

            //AssertInvariants();
            return this;
        }

        private void AppendWithExpansion(char value, int repeatCount)
        {
            Debug.Assert(repeatCount > 0, "Invalid length; should have been validated by caller.");

            // Check if the repeatCount will put us over m_MaxCapacity
            if ((uint)(repeatCount + Length) > (uint)m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(repeatCount, ExceptionArgument.repeatCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
            }

            Grow(repeatCount);
            m_Chars.AsSpan(m_Position, repeatCount).Fill(value);
            m_Position += repeatCount;
        }

        public OpenStringBuilder Append(char[]? value, int startIndex, int charCount)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (charCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);

            if (value == null)
            {
                if (startIndex == 0 && charCount == 0)
                {
                    return this;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }
            if (charCount > value.Length - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(charCount, ExceptionArgument.charCount);
            }

            if (charCount != 0)
            {
                Append(ref value[startIndex], charCount);
            }

            return this;
        }

        public OpenStringBuilder Append(string? value)
        {
            if (value is not null)
            {
                Append(ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
            }

            return this;
        }

        public OpenStringBuilder Append(string? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return this;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (count != 0)
            {
                if (startIndex > value.Length - count)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
                }

                Append(ref MemoryMarshal.GetReference(value.AsSpan(startIndex)), count);
            }

            return this;
        }

        public OpenStringBuilder Append(StringBuilder? value)
        {
            if (value != null && value.Length != 0)
            {
                return AppendCore(value, 0, value.Length);
            }
            return this;
        }

        public OpenStringBuilder Append(StringBuilder? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return this;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (count == 0)
            {
                return this;
            }

            if (count > value.Length - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            return AppendCore(value, startIndex, count);
        }

        private OpenStringBuilder AppendCore(StringBuilder value, int startIndex, int count)
        {
            int newLength = Length + count;

            if ((uint)newLength > (uint)m_MaxCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(Capacity), SR.ArgumentOutOfRange_Capacity);
            }

            int pos = m_Position;
            if (pos > m_Chars.Length - count)
            {
                Grow(count);
            }

            value.CopyTo(startIndex, m_Chars, m_Position, count);
            m_Position += count;

            return this;
        }

        #region Custom Append

        public OpenStringBuilder Append(OpenStringBuilder? value)
        {
            if (value != null && value.Length != 0)
            {
                return AppendCore(value, 0, value.Length);
            }
            return this;
        }

        public OpenStringBuilder Append(OpenStringBuilder? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return this;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (count == 0)
            {
                return this;
            }

            if (count > value.Length - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            return AppendCore(value, startIndex, count);
        }

        private OpenStringBuilder AppendCore(OpenStringBuilder value, int startIndex, int count)
        {
            if (value == this)
            {
                return Append(value.AsSpan(startIndex, count));
            }

            int newLength = Length + count;

            if ((uint)newLength > (uint)m_MaxCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(Capacity), SR.ArgumentOutOfRange_Capacity);
            }

            int pos = m_Position;
            if (pos > m_Chars.Length - count)
            {
                Grow(count);
            }

            value.CopyTo(startIndex, m_Chars, m_Position, count);
            m_Position += count;

            return this;
        }

        #endregion Custom Append

        public OpenStringBuilder AppendLine() => Append(Environment.NewLine);

        public OpenStringBuilder AppendLine(string? value)
        {
            Append(value);
            return Append(Environment.NewLine);
        }

        public OpenStringBuilder AppendLine(ReadOnlySpan<char> value)
        {
            Append(value);
            return Append(Environment.NewLine);
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            if (destinationIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(destinationIndex, ExceptionArgument.destinationIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            if ((uint)sourceIndex > (uint)Length)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(sourceIndex, ExceptionArgument.sourceIndex);
            if ((uint)sourceIndex + (uint)count > Length)
                throw new ArgumentException(SR.Arg_LongerThanSrcString);
            if ((uint)destinationIndex + (uint)count > destination.Length)
                ThrowHelper.ThrowArgumentException(ExceptionResource.ArgumentOutOfRange_OffsetOut);

            //AssertInvariants();

            m_Chars.AsSpan(sourceIndex, count).CopyTo(new Span<char>(destination).Slice(destinationIndex));
        }

        public void CopyTo(int sourceIndex, Span<char> destination, int count)
        {
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if ((uint)sourceIndex > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(sourceIndex, ExceptionArgument.sourceIndex);
            }

            if (sourceIndex > Length - count)
            {
                throw new ArgumentException(SR.Arg_LongerThanSrcString);
            }

            //AssertInvariants();

            m_Chars.AsSpan(sourceIndex, count).CopyTo(destination);
        }

        public OpenStringBuilder Insert(int index, string? value, int count) => Insert(index, value.AsSpan(), count);

        private OpenStringBuilder Insert(int index, ReadOnlySpan<char> value, int count)
        {
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            }

            if (value.IsEmpty || count == 0)
            {
                return this;
            }

            // Ensure we don't insert more chars than we can hold, and we don't
            // have any integer overflow in our new length.
            long insertingChars = (long)value.Length * count;
            if (insertingChars > MaxCapacity - this.Length)
            {
                throw new OutOfMemoryException();
            }
            Debug.Assert(insertingChars + this.Length < int.MaxValue);

            MakeRoom(index, (int)insertingChars);

            int valueLength = value.Length;
            while (count > 0)
            {
                ReplaceInPlace(ref index, ref MemoryMarshal.GetReference(value), valueLength);
                --count;
            }

            return this;
        }

        public OpenStringBuilder Remove(int startIndex, int length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);

            if (length > m_Position - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(length, ExceptionArgument.length);
            }

            RemoveCore(startIndex, length, zeroBeyondPosition: true);

            return this;
        }

        private void RemoveCore(int startIndex, int length, bool zeroBeyondPosition)
        {
            Debug.Assert(length >= 0);
            Debug.Assert(startIndex >= 0);
            Debug.Assert(length <= m_Position - startIndex);

            if (m_Position == length && startIndex == 0)
            {
                m_Position = 0;
                return;
            }

            if (length > 0)
            {
                int endIndex = startIndex + length;
                m_Chars.AsSpan(endIndex).CopyTo(m_Chars.AsSpan(startIndex));
                m_Position -= length;
                if (zeroBeyondPosition)
                {
                    m_Chars.AsSpan(m_Position).Fill('\0'); // Zero out the remaining chars
                }
                
            }
        }

#pragma warning disable CA1830 // Prefer strongly-typed Append and Insert method overloads on StringBuilder. No need to fix for the builder itself
        public OpenStringBuilder Append(bool value, IFormatProvider? provider = null) // J2N TODO: API - Should we make a boolean parameter here instead? We only need to say "title case" or "lower case"
        {
            // J2N: Use lower-case formatting (the default in Java) if passed a StringFormatter,
            // otherwise use the .NET default formatting.
            Append(FormatBoolean(value, provider));
            return this;
        }
#pragma warning restore CA1830

        public OpenStringBuilder Append(char value)
        {
            int pos = m_Position;
            if ((uint)pos < (uint)m_Chars.Length)
            {
                m_Chars[pos] = value;
                m_Position = pos + 1;
            }
            else
            {
                AppendWithExpansion(value);
            }

            return this;
        }

        #region Append Number

        [CLSCompliant(false)]
        public OpenStringBuilder Append(sbyte value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        public OpenStringBuilder Append(byte value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        public OpenStringBuilder Append(short value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        public OpenStringBuilder Append(int value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        public OpenStringBuilder Append(long value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        public OpenStringBuilder Append(float value, string? format = null, IFormatProvider? provider = null)
        {
            provider ??= NumberFormatInfo.InvariantInfo;
            if (DotNetNumber.TryFormatSingle(value, format.AsSpan(), provider, m_Chars.AsSpan(m_Position), out int charsWritten))
            {
                m_Position += charsWritten;
            }
            else
            {
                Append(DotNetNumber.FormatSingle(value, format, provider));
            }
            return this;
        }

        public OpenStringBuilder Append(double value, string? format = null, IFormatProvider? provider = null)
        {
            provider ??= NumberFormatInfo.InvariantInfo;
            if (DotNetNumber.TryFormatDouble(value, format.AsSpan(), provider, m_Chars.AsSpan(m_Position), out int charsWritten))
            {
                m_Position += charsWritten;
            }
            else
            {
                Append(DotNetNumber.FormatDouble(value, format, provider));
            }
            return this;
        }

        public OpenStringBuilder Append(decimal value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        [CLSCompliant(false)]
        public OpenStringBuilder Append(ushort value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        [CLSCompliant(false)]
        public OpenStringBuilder Append(uint value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        [CLSCompliant(false)]
        public OpenStringBuilder Append(ulong value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif


        #endregion Append Number

        private OpenStringBuilder AppendSpanFormattable<T>(T value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            where T : ISpanFormattable
#else
            where T : Number
#endif
        {
            Debug.Assert(typeof(T).Assembly.Equals(typeof(object).Assembly) || typeof(T).Assembly.Equals(typeof(Number).Assembly), "Implementation trusts the results of TryFormat because T is expected to be something known");

            if (value.TryFormat(m_Chars.AsSpan(m_Position), out int charsWritten, format.AsSpan(), provider))
            {
                m_Position += charsWritten;
            }
            else
            {
                Append(value.ToString(format, provider));
            }

            return this;
        }

        public OpenStringBuilder Append(object? value, string? format = null, IFormatProvider? provider = null)
        {
            if (value is null)
                return this; // no-op
#if FEATURE_SPANFORMATTABLE
            else if (value is ISpanFormattable spanFormattable) // J2N: Check for ISpanFormattable reference types, as this will improve performance.
                return AppendSpanFormattable(spanFormattable, format, provider);
#else
            else if (value is Number number) // J2N: Check for Number-derived reference types, as this will improve performance.
                return AppendSpanFormattable(number, format, provider);
#endif
            else if (value is IFormattable formattable)
                return Append(formattable.ToString(format, provider));
            else if (value is ICharSequence csq)
                return Append(csq); // Not formattable
            else
                return Append(value.ToString());
        }


        public OpenStringBuilder Append(char[]? value)
        {
            if (value is not null)
            {
#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
                Append(ref MemoryMarshal.GetArrayDataReference(value), value.Length);
#else
                Append(ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
#endif
            }

            return this;
        }

        public OpenStringBuilder Append(ReadOnlySpan<char> value)
        {
            Append(ref MemoryMarshal.GetReference(value), value.Length);
            return this;
        }

        public OpenStringBuilder Append(ReadOnlyMemory<char> value) => Append(value.Span);

        // J2N TODO: API - String interpolation for J2N formatters

        ///// <summary>Appends the specified interpolated string to this instance.</summary>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //public OpenStringBuilder Append([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler) => this;

        ///// <summary>Appends the specified interpolated string to this instance.</summary>
        ///// <param name="provider">An object that supplies culture-specific formatting information.</param>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //public OpenStringBuilder Append(IFormatProvider? provider, [InterpolatedStringHandlerArgument("", nameof(provider))] ref AppendInterpolatedStringHandler handler) => this;

        ///// <summary>Appends the specified interpolated string followed by the default line terminator to the end of the current StringBuilder object.</summary>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //public OpenStringBuilder AppendLine([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler) => AppendLine();

        ///// <summary>Appends the specified interpolated string followed by the default line terminator to the end of the current StringBuilder object.</summary>
        ///// <param name="provider">An object that supplies culture-specific formatting information.</param>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //public OpenStringBuilder AppendLine(IFormatProvider? provider, [InterpolatedStringHandlerArgument("", nameof(provider))] ref AppendInterpolatedStringHandler handler) => AppendLine();

        #region AppendJoin

        public unsafe OpenStringBuilder AppendJoin(string? separator, params object?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public unsafe OpenStringBuilder AppendJoin(string? separator, params ReadOnlySpan<object?> values)
        {
            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public unsafe OpenStringBuilder AppendJoin<T>(string? separator, IEnumerable<T> values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public unsafe OpenStringBuilder AppendJoin(string? separator, params string?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public unsafe OpenStringBuilder AppendJoin(string? separator, params ReadOnlySpan<string?> values)
        {
            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public unsafe OpenStringBuilder AppendJoin(char separator, params object?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            return AppendJoinCore(ref separator, 1, values);
        }

        public unsafe OpenStringBuilder AppendJoin(char separator, params ReadOnlySpan<object?> values) =>
            AppendJoinCore(ref separator, 1, values);

        public unsafe OpenStringBuilder AppendJoin<T>(char separator, IEnumerable<T> values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            return AppendJoinCore(ref separator, 1, values);
        }

        public unsafe OpenStringBuilder AppendJoin(char separator, params string?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            return AppendJoinCore(ref separator, 1, values);
        }

        public unsafe OpenStringBuilder AppendJoin(char separator, params ReadOnlySpan<string?> values) =>
            AppendJoinCore(ref separator, 1, values);

        private unsafe OpenStringBuilder AppendJoinCore<T>(ref char separator, int separatorLength, IEnumerable<T> values)
        {
            Debug.Assert(values != null);
            Debug.Assert(!Unsafe.IsNullRef(ref separator));
            Debug.Assert(separatorLength >= 0);

            using (IEnumerator<T> en = values!.GetEnumerator())
            {
                if (!en.MoveNext())
                {
                    return this;
                }

                T value = en.Current;
                if (value != null)
                {
                    Append(value.ToString()); // J2N TODO: ISpanFormattable, IFormattable to override culture?
                }

                while (en.MoveNext())
                {
                    Append(ref separator, separatorLength);
                    value = en.Current;
                    if (value != null)
                    {
                        Append(value.ToString()); // J2N TODO: ISpanFormattable, IFormattable to override culture?
                    }
                }
            }
            return this;
        }

        private OpenStringBuilder AppendJoinCore<T>(ref char separator, int separatorLength, ReadOnlySpan<T> values)
        {
            if (values.IsEmpty)
            {
                return this;
            }

            if (values[0] != null)
            {
                Append(values[0]!.ToString()); // J2N TODO: ISpanFormattable, IFormattable to override culture?
            }

            for (int i = 1; i < values.Length; i++)
            {
                Append(ref separator, separatorLength);
                if (values[i] != null)
                {
                    Append(values[i]!.ToString()); // J2N TODO: ISpanFormattable, IFormattable to override culture?
                }
            }
            return this;
        }

        #endregion AppendJoin

        public OpenStringBuilder Insert(int index, string? value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value != null)
            {
                Insert(index, ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
            }

            return this;
        }

#pragma warning disable CA1830 // Prefer strongly-typed Append and Insert method overloads on StringBuilder. No need to fix for the builder itself
        // bool does not implement ISpanFormattable but its ToString override returns cached strings.
        public OpenStringBuilder Insert(int index, bool value, IFormatProvider? provider = null)
            // J2N: Use lower-case formatting (the default in Java) if passed a StringFormatter,
            // otherwise use the .NET default formatting.
            => Insert(index, FormatBoolean(value, provider), 1);
#pragma warning restore CA1830

        #region Insert Number

        [CLSCompliant(false)]
        public OpenStringBuilder Insert(int index, sbyte value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        public OpenStringBuilder Insert(int index, byte value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        public OpenStringBuilder Insert(int index, short value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        public OpenStringBuilder Insert(int index, int value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        public OpenStringBuilder Insert(int index, long value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        public OpenStringBuilder Insert(int index, float value, string? format = null, IFormatProvider? provider = null)
        {
            provider ??= NumberFormatInfo.InvariantInfo;
            Span<char> buffer = stackalloc char[CharStackBufferSize];
            if (DotNetNumber.TryFormatSingle(value, format.AsSpan(), provider, buffer, out int charsWritten))
            {
                // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
                // we want exceeding the maximum capacity to throw an OutOfMemoryException.
                return Insert(index, buffer.Slice(0, charsWritten), 1);
            }

            return Insert(index, DotNetNumber.FormatSingle(value, format, provider), 1);
        }

        public OpenStringBuilder Insert(int index, double value, string? format = null, IFormatProvider? provider = null)
        {
            provider ??= NumberFormatInfo.InvariantInfo;
            Span<char> buffer = stackalloc char[CharStackBufferSize];
            if (DotNetNumber.TryFormatDouble(value, format.AsSpan(), provider, buffer, out int charsWritten))
            {
                // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
                // we want exceeding the maximum capacity to throw an OutOfMemoryException.
                return Insert(index, buffer.Slice(0, charsWritten), 1);
            }

            return Insert(index, DotNetNumber.FormatDouble(value, format, provider), 1);
        }

        public OpenStringBuilder Insert(int index, decimal value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        [CLSCompliant(false)]
        public OpenStringBuilder Insert(int index, ushort value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        [CLSCompliant(false)]
        public OpenStringBuilder Insert(int index, uint value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        [CLSCompliant(false)]
        public OpenStringBuilder Insert(int index, ulong value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider ?? NumberFormatInfo.InvariantInfo);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        #endregion Insert Number

        public OpenStringBuilder Insert(int index, char value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            Insert(index, ref value, 1);
            return this;
        }

        public OpenStringBuilder Insert(int index, char[]? value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value != null)
            {
#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
                Insert(index, ref MemoryMarshal.GetArrayDataReference(value), value.Length);
#else
                Insert(index, ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
#endif
            }
            return this;
        }

        public OpenStringBuilder Insert(int index, char[]? value, int startIndex, int charCount)
        {
            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value == null)
            {
                if (startIndex == 0 && charCount == 0)
                {
                    return this;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (startIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            }

            if (charCount < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);
            }

            if (startIndex > value.Length - charCount)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            if (charCount > 0)
            {
                Insert(index, ref value[startIndex], charCount);
            }
            return this;
        }

        
        public OpenStringBuilder Insert(int index, object? value, string? format = null, IFormatProvider? provider = null)
        {
            if (value is null)
                return this; // no-op;
#if FEATURE_SPANFORMATTABLE
            else if (value is ISpanFormattable spanFormattable) // J2N: Check for ISpanFormattable reference types, as this will improve performance.
                return InsertSpanFormattable(index, spanFormattable, format, provider);
#else
            else if (value is Number number) // J2N: Check for Number-derived reference types, as this will improve performance.
                return InsertSpanFormattable(index, number, format, provider);
#endif
            else if (value is IFormattable formattable)
                return Insert(index, formattable.ToString(format, provider));
            else
                return Insert(index, value.ToString(), 1);
        }

        public OpenStringBuilder Insert(int index, ReadOnlySpan<char> value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value.Length != 0)
            {
                Insert(index, ref MemoryMarshal.GetReference(value), value.Length);
            }

            return this;
        }

        private OpenStringBuilder InsertSpanFormattable<T>(int index, T value, string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            where T : ISpanFormattable
#else
            where T : Number
#endif
        {
            Debug.Assert(typeof(T).Assembly.Equals(typeof(object).Assembly) || typeof(T).Assembly.Equals(typeof(Number).Assembly), "Implementation trusts the results of TryFormat because T is expected to be something known");

            Span<char> buffer = stackalloc char[CharStackBufferSize];
            if (value.TryFormat(buffer, out int charsWritten, format.AsSpan(), provider))
            {
                // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
                // we want exceeding the maximum capacity to throw an OutOfMemoryException.
                return Insert(index, buffer.Slice(0, charsWritten), 1);
            }

            return Insert(index, value.ToString(format, provider), 1);
        }


        #region AppendFormat

        public OpenStringBuilder AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            return AppendFormat(null, format, MemoryMarshal.CreateReadOnlySpan(ref arg0, 1));
#else
            return AppendFormat(null, format, new ParamsArray(arg0));
#endif
        }

        public OpenStringBuilder AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            TwoObjects two = new TwoObjects(arg0, arg1);
            return AppendFormat(null, format, (ReadOnlySpan<object?>)two);
#else
            return AppendFormat(null, format, new ParamsArray(arg0, arg1));
#endif
        }

        public OpenStringBuilder AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            ThreeObjects three = new ThreeObjects(arg0, arg1, arg2);
            return AppendFormat(null, format, (ReadOnlySpan<object?>)three);
#else
            return AppendFormat(null, format, new ParamsArray(arg0, arg1, arg2));
#endif
        }

        public OpenStringBuilder AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        {
            if (args is null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormat(..., span).
                ThrowHelper.ThrowArgumentNullException(format is null ? ExceptionArgument.format : ExceptionArgument.args);
            }

            return AppendFormat(null, format, args);
        }

        public OpenStringBuilder AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args)
        {
            return AppendFormat(null, format, args);
        }

        public OpenStringBuilder AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            return AppendFormat(provider, format, MemoryMarshal.CreateReadOnlySpan(ref arg0, 1));
#else
            return AppendFormat(provider, format, new ParamsArray(arg0));
#endif
        }

        public OpenStringBuilder AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            TwoObjects two = new TwoObjects(arg0, arg1);
            return AppendFormat(provider, format, (ReadOnlySpan<object?>)two);
#else
            return AppendFormat(provider, format, new ParamsArray(arg0, arg1));
#endif
        }

        public OpenStringBuilder AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            ThreeObjects three = new ThreeObjects(arg0, arg1, arg2);
            return AppendFormat(provider, format, (ReadOnlySpan<object?>)three);
#else
            return AppendFormat(provider, format, new ParamsArray(arg0, arg1, arg2));
#endif
        }

        public OpenStringBuilder AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        {
            if (args is null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormat(..., span).
                ThrowHelper.ThrowArgumentNullException(format is null ? ExceptionArgument.format : ExceptionArgument.args);
            }

            return AppendFormat(provider, format, (ReadOnlySpan<object?>)args);
        }

        public OpenStringBuilder AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);

            // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
            const int IndexLimit = 1_000_000; // Note:            0 <= ArgIndex < IndexLimit
            const int WidthLimit = 1_000_000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

            // Query the provider (if one was supplied) for an ICustomFormatter.  If there is one,
            // it needs to be used to transform all arguments.
            ICustomFormatter? cf = (ICustomFormatter?)provider?.GetFormat(typeof(ICustomFormatter));

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
                        return this;
                    }

                    ReadOnlySpan<char> remainder = format.AsSpan(pos);
                    int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                    if (countUntilNextBrace < 0)
                    {
                        Append(remainder);
                        return this;
                    }

                    // Append the text until the brace.
                    Append(remainder.Slice(0, countUntilNextBrace));
                    pos += countUntilNextBrace;

                    // Get the brace.  It must be followed by another character, either a copy of itself in the case of being
                    // escaped, or an arbitrary character that's part of the hole in the case of an opening brace.
                    char brace = format[pos];
                    ch = MoveNext(format, ref pos);
                    if (brace == ch)
                    {
                        Append(ch);
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
                            Append(' ', width - charsWritten);
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
                    Append(s);
                }
                else if (leftJustify)
                {
                    Append(s);
                    Append(' ', width - s.Length);
                }
                else
                {
                    Append(' ', width - s.Length);
                    Append(s);
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
        private OpenStringBuilder AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, ParamsArray args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);

            // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
            const int IndexLimit = 1_000_000; // Note:            0 <= ArgIndex < IndexLimit
            const int WidthLimit = 1_000_000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

            // Query the provider (if one was supplied) for an ICustomFormatter.  If there is one,
            // it needs to be used to transform all arguments.
            ICustomFormatter? cf = (ICustomFormatter?)provider?.GetFormat(typeof(ICustomFormatter));

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
                        return this;
                    }

                    ReadOnlySpan<char> remainder = format.AsSpan(pos);
                    int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                    if (countUntilNextBrace < 0)
                    {
                        Append(remainder);
                        return this;
                    }

                    // Append the text until the brace.
                    Append(remainder.Slice(0, countUntilNextBrace));
                    pos += countUntilNextBrace;

                    // Get the brace.  It must be followed by another character, either a copy of itself in the case of being
                    // escaped, or an arbitrary character that's part of the hole in the case of an opening brace.
                    char brace = format[pos];
                    ch = MoveNext(format, ref pos);
                    if (brace == ch)
                    {
                        Append(ch);
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
                            Append(' ', width - charsWritten);
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
                    Append(s);
                }
                else if (leftJustify)
                {
                    Append(s);
                    Append(' ', width - s.Length);
                }
                else
                {
                    Append(' ', width - s.Length);
                    Append(s);
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

        public OpenStringBuilder AppendFormat<TArg0>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(1);
            return AppendFormat(provider, format, arg0, 0, 0, default);
        }

        public OpenStringBuilder AppendFormat<TArg0, TArg1>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(2);
            return AppendFormat(provider, format, arg0, arg1, 0, default);
        }

        public OpenStringBuilder AppendFormat<TArg0, TArg1, TArg2>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(3);
            return AppendFormat(provider, format, arg0, arg1, arg2, default);
        }

        public OpenStringBuilder AppendFormat(IFormatProvider? provider, CompositeFormat format, params object?[] args)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            if (args is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.args);
            return AppendFormat(provider, format, (ReadOnlySpan<object?>)args);
        }

        public OpenStringBuilder AppendFormat(IFormatProvider? provider, CompositeFormat format, params ReadOnlySpan<object?> args)
        {
            //ArgumentNullException.ThrowIfNull(format);
            if (format is null)
                throw new ArgumentNullException(nameof(format));
            format.ValidateNumberOfArgs(args.Length);
            return args.Length switch
            {
                0 => AppendFormat(provider, format, 0, 0, 0, args),
                1 => AppendFormat(provider, format, args[0], 0, 0, args),
                2 => AppendFormat(provider, format, args[0], args[1], 0, args),
                _ => AppendFormat(provider, format, args[0], args[1], args[2], args),
            };
        }

        private OpenStringBuilder AppendFormat<TArg0, TArg1, TArg2>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2, ReadOnlySpan<object?> args)
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
            return Append(ref handler);
        }

#endif

        #endregion AppendFormat

        #region Replace

        public OpenStringBuilder Replace(string oldValue, string? newValue) => Replace(oldValue, newValue, 0, Length);

        public OpenStringBuilder Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue) => Replace(oldValue, newValue, 0, Length);


        #endregion Replace


        #region Equals

        /// <summary>
        /// Determines if the contents of this builder are equal to the contents of another builder..
        /// </summary>
        /// <param name="sb">The other builder.</param>
        public bool Equals([NotNullWhen(true)] OpenStringBuilder? sb)
        {
            if (sb == null)
            {
                return false;
            }
            if (Length != sb.Length)
            {
                return false;
            }
            if (sb == this)
            {
                return true;
            }
            return new ReadOnlySpan<char>(m_Chars, 0, m_Position).SequenceEqual(new ReadOnlySpan<char>(sb.m_Chars, 0, sb.m_Position));
        }

        /// <summary>
        /// Determines if the contents of this builder are equal to the contents of another builder..
        /// </summary>
        /// <param name="sb">The other builder.</param>
        public bool Equals([NotNullWhen(true)] StringBuilder? sb)
        {
            if (sb == null)
            {
                return false;
            }
            if (Length != sb.Length)
            {
                return false;
            }
#if FEATURE_STRINGBUILDER_GETCHUNKS
            int offset = 0;
            foreach (ReadOnlyMemory<char> chunk in sb.GetChunks())
            {
                ReadOnlySpan<char> thisChunk = new ReadOnlySpan<char>(m_Chars, offset, chunk.Length);
                if (!chunk.Span.SequenceEqual(thisChunk))
                    return false;

                offset += chunk.Length;
            }
            Debug.Assert(offset == Length);
            return true;
#else
            int length = m_Position;
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                sb.CopyTo(0, textChars, length);
#else
                Span<char> textChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                sb.CopyTo(0, arrayToReturnToPool, 0, length);
#endif
                return new ReadOnlySpan<char>(m_Chars, 0, m_Position).SequenceEqual(textChars.Slice(0, length));
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
#endif
        }

        /// <summary>
        /// Determines if the contents of this builder are equal to the contents of <see cref="ReadOnlySpan{Char}"/>.
        /// </summary>
        /// <param name="other">The <see cref="ReadOnlySpan{Char}"/>.</param>
        public bool Equals(ReadOnlySpan<char> other)
        {
            if (other.Length != Length)
            {
                return false;
            }

            return new ReadOnlySpan<char>(m_Chars, 0, m_Position).SequenceEqual(other);
        }

        #endregion

        #region Replace
        public OpenStringBuilder Replace(string oldValue, string? newValue, int startIndex, int count)
        {
            if (oldValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.oldValue);
            return Replace(oldValue.AsSpan(), newValue.AsSpan(), startIndex, count);
        }

        public OpenStringBuilder Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)startIndex > (uint)currentLength)
            {
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            }
            if (count < 0 || startIndex > currentLength - count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            }
            if (oldValue.Length == 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_EmptySpan, ExceptionArgument.oldValue);
            }

            var replacements = new ValueListBuilder<int>(stackalloc int[128]); // A list of replacement positions in a chunk to apply

            // Starting point.
            int indexInChunk = startIndex;
            while (count > 0)
            {
                //Debug.Assert(chunk != null, "chunk was null in replace");

                // While the remaining search space is at least as large as the old value being replaced,
                // find all occurrences of it contained entirely within the chunk. We stop searching
                // once we're within oldValue.Length from the end of the chunk (or count limit), at which point
                // we need to consider a value that bridges between two chunks.
                ReadOnlySpan<char> remainingChunk = m_Chars.AsSpan(indexInChunk, Math.Min(m_Position - indexInChunk, count));
                while (oldValue.Length <= remainingChunk.Length)
                {
                    // Find the next match.
                    int foundPos = remainingChunk.IndexOf(oldValue);
                    if (foundPos >= 0)
                    {
                        // We found one.  Add it as a location for the replacement.
                        indexInChunk += foundPos;
                        replacements.Append(indexInChunk);

                        // Move ahead to the next location.
                        remainingChunk = remainingChunk.Slice(foundPos + oldValue.Length);
                        indexInChunk += oldValue.Length;
                        count -= foundPos + oldValue.Length;

                        // If after accounting for moving past the match our count has
                        // gone to 0, break out to stop searching.
                        Debug.Assert(count >= 0, "count should never go negative");
                        if (count == 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        // No match found. Reposition to one character beyond the last starting
                        // location searched, which will be oldValue.Length - 1 from the end.
                        // Then break out so that we can start the cross-chunk matching from that location.
                        int move = remainingChunk.Length - (oldValue.Length - 1);
                        indexInChunk += move;
                        count -= move;
                        break;
                    }
                }

                Debug.Assert(oldValue.Length > Math.Min(count, m_Position - indexInChunk),
                    $"oldValue.Length = {oldValue.Length}, m_Position - indexInChunk = {m_Position - indexInChunk}, count == {count}");

                // Now do the more complicated cross-chunk matching.
                while (indexInChunk < m_Position && count > 0)
                {
                    if (StartsWith(indexInChunk, count, oldValue))
                    {
                        replacements.Append(indexInChunk);
                        indexInChunk += oldValue.Length;
                        count -= oldValue.Length;
                    }
                    else
                    {
                        indexInChunk++;
                        --count;
                    }
                }

                // We've either fully explored the chunk or we've reached our count limit.
                Debug.Assert(indexInChunk >= m_Position || count == 0,
                    $"indexInChunk = {indexInChunk}, m_Position == {m_Position}, count == {count}");

                // Replacing mutates the blocks, so we need to convert to a logical index and back afterwards.
                int index = indexInChunk; // + chunk.m_ChunkOffset;

                // Apply any replacements we accumulated.
                if (replacements.Length != 0)
                {
                    // Perform all replacements, and adjust the logical index if the new and old values
                    // have different lengths, such that the replacements would have impacted it.
                    ReplaceAll(replacements.AsSpan(), oldValue.Length, newValue);
                    index += (newValue.Length - oldValue.Length) * replacements.Length;
                    replacements.Length = 0;
                }

                //chunk = FindChunkForIndex(index);
                //indexInChunk = index - chunk.m_ChunkOffset;
                //Debug.Assert(chunk != null || count == 0, "Chunks ended prematurely!");

                indexInChunk = index - m_Position;
            }

            replacements.Dispose();

            //AssertInvariants();
            return this;
        }

        public OpenStringBuilder Replace(char oldChar, char newChar)
        {
            return Replace(oldChar, newChar, 0, Length);
        }

        public OpenStringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)startIndex > (uint)currentLength)
            {
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            }

            if (count < 0 || startIndex > currentLength - count)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(count, ExceptionArgument.count);
            }

            Span<char> span = m_Chars.AsSpan(startIndex, count);
            span.Replace(oldChar, newChar);

            //AssertInvariants();
            return this;
        }

        // JDK overloads

        public OpenStringBuilder Replace(int startIndex, int count, string newValue)
        {
            if (newValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.newValue);
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            ReplaceCore(startIndex, count, newValue.AsSpan());
            return this;
        }

        public OpenStringBuilder Replace(int startIndex, int count, ReadOnlySpan<char> newValue) // J2N TODO: Tests
        {
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            ReplaceCore(startIndex, count, newValue);
            return this;
        }

        private void ReplaceCore(int startIndex, int count, ReadOnlySpan<char> newValue)
        {
            Debug.Assert(startIndex >= 0 || startIndex <= m_Position);
            Debug.Assert(count >= 0);

            int end = startIndex + count;
            if (end > m_Position)
            {
                end = m_Position; // J2N TODO: Do we need this?
            }
            if (end > startIndex)
            {
                int stringLength = newValue.Length;
                int diff = end - startIndex - stringLength;
                if (diff > 0)
                { // replacing with fewer characters
                    RemoveCore(startIndex, diff, zeroBeyondPosition: false);
                }
                else if (diff < 0)
                {
                    // replacing with more characters...need some room
                    MakeRoom(startIndex, -diff);
                }
                // copy the chars based on the new length
                //newValue.CopyTo(m_Chars.AsSpan(startIndex, stringLength));
                int index = startIndex; // Need a copy in case it is modified so it doesn't affect the below insert.
                ReplaceInPlace(ref index, ref MemoryMarshal.GetReference(newValue), stringLength);
            }
            if (startIndex == end)
            {
                Insert(startIndex, ref MemoryMarshal.GetReference(newValue), newValue.Length);
            }
        }

        private void ReplaceAll(ReadOnlySpan<int> replacements, int removeCount, ReadOnlySpan<char> value) // Based on ReplaceAllInChunk()
        {
            Debug.Assert(!replacements.IsEmpty);

            // calculate the total amount of extra space or space needed for all the replacements.
            long longDelta = (value.Length - removeCount) * (long)replacements.Length;
            int delta = (int)longDelta;
            if (delta != longDelta)
            {
                throw new OutOfMemoryException();
            }

            int targetIndex = replacements[0];

            // Make the room needed for all the new characters if needed.
            if (delta > 0)
            {
                MakeRoom(targetIndex, delta);
            }

            char[] chars = m_Chars;
            // We made certain that characters after the insertion point are not moved,
            int i = 0;
            while (true)
            {
                // Copy in the new string for the ith replacement
                ReplaceInPlace(ref targetIndex, ref MemoryMarshal.GetReference(value), value.Length);
                int gapStart = replacements[i] + removeCount;
                i++;
                if ((uint)i >= replacements.Length)
                {
                    break;
                }

                int gapEnd = replacements[i];
                Debug.Assert(gapStart < chars.Length, "gap starts at end of buffer.  Should not happen");
                Debug.Assert(gapStart <= gapEnd, "negative gap size");
                Debug.Assert(gapEnd <= m_Position, "gap too big");
                if (delta != 0)     // can skip the sliding of gaps if source an target string are the same size.
                {
                    // Copy the gap data between the current replacement and the next replacement
                    ReplaceInPlace(ref targetIndex, ref chars[gapStart], gapEnd - gapStart);
                }
                else
                {
                    targetIndex += gapEnd - gapStart;
                    Debug.Assert(targetIndex <= m_Position, "gap not in chunk");
                }
            }

            // Remove extra space if necessary.
            if (delta < 0)
            {
                RemoveCore(targetIndex, -delta, zeroBeyondPosition: false);
            }
        }

        private bool StartsWith(int index, int count, ReadOnlySpan<char> value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (count == 0)
                {
                    return false;
                }

                if (value[i] != m_Chars[index])
                {
                    return false;
                }

                index++;
                --count;
            }

            return true;
        }

        private void ReplaceInPlace(ref int index, ref char value, int count)
        {
            if (count != 0)
            {
                while (true)
                {
                    int length = m_Position - index;
                    Debug.Assert(length >= 0, "Index isn't in the array.");

                    int lengthToCopy = Math.Min(length, count);
#if FEATURE_MEMORYMARSHAL_CREATESPAN
                    MemoryMarshal.CreateSpan(ref value, lengthToCopy).CopyTo(m_Chars.AsSpan(index));
#else
                    unsafe
                    {
                        fixed (char* pSource = &value)
                        {
                            new Span<char>(pSource, lengthToCopy).CopyTo(m_Chars.AsSpan(index));
                        }
                    }
#endif
                    // Advance the index.
                    index += lengthToCopy;
                    count -= lengthToCopy;
                    if (count == 0)
                    {
                        break;
                    }
                    value = ref Unsafe.Add(ref value, lengthToCopy);
                }
            }
        }

#endregion Replace

        [CLSCompliant(false)]
        public unsafe OpenStringBuilder Append(char* value, int valueCount)
        {
            // We don't check null value as this case will throw null reference exception anyway
            if (valueCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(valueCount, ExceptionArgument.valueCount);

            Append(ref *value, valueCount);
            return this;
        }

        /// <summary>Appends a specified number of chars starting from the specified reference.</summary>
        private void Append(ref char value, int valueCount)
        {
            Debug.Assert(valueCount >= 0, "Invalid length; should have been validated by caller.");
            if (valueCount != 0)
            {
                char[] chars = m_Chars;
                int pos = m_Position;
                if (((uint)pos + (uint)valueCount) <= (uint)chars.Length)
                {
#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
                    ref char destination = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars), pos);
#else
                    ref char destination = ref Unsafe.Add(ref MemoryMarshal.GetReference(chars.AsSpan()), pos);
#endif
                    if (valueCount <= 2)
                    {
                        destination = value;
                        if (valueCount == 2)
                        {
                            Unsafe.Add(ref destination, 1) = Unsafe.Add(ref value, 1);
                        }
                    }
                    else
                    {
                        BufferHelper.Memmove(ref destination, ref value, (nuint)valueCount);
                    }

                    m_Position += valueCount;
                }
                else
                {
                    AppendWithExpansion(ref value, valueCount);
                }
            }
        }

        private void AppendWithExpansion(ref char value, int valueCount)
        {
            // Check if the valueCount will put us over m_MaxCapacity.
            // Doing the check here prevents corruption of the StringBuilder.
            int newLength = Length + valueCount;
            if (newLength > m_MaxCapacity || newLength < valueCount)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(valueCount, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
            }

            Grow(valueCount);

            char[] chars = m_Chars;
            int pos = m_Position;

#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
            ref char destination = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars), pos);
#else
            ref char destination = ref Unsafe.Add(ref MemoryMarshal.GetReference(chars.AsSpan()), pos);
#endif
            if (valueCount <= 2)
            {
                destination = value;
                if (valueCount == 2)
                {
                    Unsafe.Add(ref destination, 1) = Unsafe.Add(ref value, 1);
                }
            }
            else
            {
                BufferHelper.Memmove(ref destination, ref value, (nuint)valueCount);
            }

            m_Position += valueCount;

            //AssertInvariants();
        }

        /// <summary>
        /// Inserts a character buffer into this builder at the specified position.
        /// </summary>
        /// <param name="index">The index to insert in this builder.</param>
        /// <param name="value">The pointer to the start of the buffer.</param>
        /// <param name="valueCount">The number of characters in the buffer.</param>
        [CLSCompliant(false)]
        public unsafe OpenStringBuilder Insert(int index, char* value, int valueCount) // J2N TODO: API - tests
        {
            // We don't check null value as this case will throw null reference exception anyway
            if (valueCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(valueCount, ExceptionArgument.valueCount);

            Insert(index, ref *value, valueCount);
            return this;
        }

        /// <summary>
        /// Inserts a character buffer into this builder at the specified position.
        /// </summary>
        /// <param name="index">The index to insert in this builder.</param>
        /// <param name="value">The reference to the start of the buffer.</param>
        /// <param name="valueCount">The number of characters in the buffer.</param>
        private void Insert(int index, ref char value, int valueCount)
        {
            Debug.Assert((uint)index <= (uint)Length, "Callers should check that index is a legal value.");

            if (valueCount > 0)
            {
                MakeRoom(index, valueCount);
                ReplaceInPlace(ref index, ref value, valueCount);
            }
        }

        private static string FormatBoolean(bool value, IFormatProvider? provider) =>
            provider is null || provider is StringFormatter ? StringFormatter.FormatBoolean(value) : value.ToString(provider);


        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendWithExpansion(char value)
        {
            Grow(1);
            m_Chars[m_Position] = value;
            m_Position++;
        }

        /// <summary>
        /// Resize the internal buffer either by doubling current buffer size or
        /// by adding <paramref name="additionalCapacityBeyondPos"/> to
        /// <see cref="m_Position"/> whichever is greater.
        /// </summary>
        /// <param name="additionalCapacityBeyondPos">
        /// Number of chars requested beyond current position.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);
            Debug.Assert(m_Position > m_Chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

            // Check to ensure we don't exceed MaxCapacity
            if (((uint)additionalCapacityBeyondPos + (uint)Length) > (uint)m_MaxCapacity)
            {
                throw new ArgumentOutOfRangeException("requiredLength", SR.ArgumentOutOfRange_SmallCapacity);
            }

            m_Chars = ReplaceBuffer(m_Chars.AsSpan(0, m_Position), CalculateNewArrayLength(additionalCapacityBeyondPos));
        }

        private int CalculateNewArrayLength(int additionalCapacityBeyondPos)
        {
            const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

            // Increase to at least the required size (m_Position + additionalCapacityBeyondPos), but try
            // to double the size if possible, bounding the doubling to not go beyond the max array length.
            int newCapacity = (int)Math.Max(
                (uint)(m_Position + additionalCapacityBeyondPos),
                Math.Min((uint)m_Chars.Length * 2, ArrayMaxLength));
            return newCapacity;
        }

        protected virtual char[] ReplaceBuffer(ReadOnlySpan<char> value, int newCapacity)
        {
            // Make sure to let the array allocation throw an exception if the caller has a bug and the desired capacity is negative.
            // This could also go negative if the actual required length wraps around.
            char[] temp = new char[newCapacity];
            value.CopyTo(temp);
            return temp;
        }



        // J2N-specific methods

        // For testing
        internal char[] ToCharArray() => m_Position == m_Chars.Length ? m_Chars : m_Chars.AsSpan(0, m_Position).ToArray();


        /// <summary>
        /// Deletes a sequence of characters specified by <paramref name="startIndex"/> and <paramref name="count"/>.
        /// Shifts any remaining characters to the left.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// <para/>
        /// This method differs from <see cref="OpenStringBuilder.Remove(int, int)"/> in that it will automatically
        /// adjust the <paramref name="count"/> if <c><paramref name="startIndex"/> + <paramref name="count"/> > <see cref="OpenStringBuilder.Length"/></c>
        /// to <c><see cref="OpenStringBuilder.Length"/> - <paramref name="startIndex"/>.</c>, provided it is not bounded by <see cref="OpenStringBuilder.MaxCapacity"/>.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of characters to delete.</param>
        /// <returns>This <see cref="OpenStringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than <see cref="OpenStringBuilder.Length"/>.
        /// </exception>
        public OpenStringBuilder Delete(int startIndex, int count)
        {
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            int pos = m_Position;
            if (startIndex + count > pos)
                count = pos - startIndex;
            if (count > 0)
                RemoveCore(startIndex, count, zeroBeyondPosition: true);
            return this;
        }

        /// <summary>
        /// Causes this character sequence to be replaced by the reverse of
        /// the sequence. If there are any surrogate pairs included in the
        /// sequence, these are treated as single characters for the
        /// reverse operation. Thus, the order of the high-low surrogates
        /// is never reversed.
        /// <para/>
        /// IMPORTANT: This operation is done in-place. Although a <see cref="StringBuilder"/>
        /// is returned, it is the SAME instance as the one that is passed in.
        /// <para/>
        /// Let <c>n</c> be the character length of this character sequence
        /// (not the length in <see cref="char"/> values) just prior to
        /// execution of the <see cref="Reverse()"/> method. Then the
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
        /// don't require a <see cref="StringBuilder"/> instance.
        /// </summary>
        /// <returns>A reference to this <see cref="StringBuilder"/>, for chaining.</returns>
        /// <seealso cref="J2N.Text.StringExtensions.ReverseText(string)"/>
        /// <seealso cref="J2N.MemoryExtensions.ReverseText(Span{char})"/>
        /// <seealso cref="J2N.Text.StringBuilderExtensions.Reverse(StringBuilder)"/>
        public OpenStringBuilder Reverse()
        {
            m_Chars.AsSpan(0, m_Position).ReverseText();
            return this;
        }

        /// <summary>
        /// Trims off any extra capacity beyond the current length. Note, this method
        /// is NOT guaranteed to change the capacity.
        /// </summary>
        public void TrimExcess()
        {
            if (m_Position < m_Chars.Length)
            {
                m_Chars = ReplaceBuffer(m_Chars.AsSpan(0, m_Position), m_Position);
            }
        }
    }
}
