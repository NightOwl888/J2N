using J2N.Buffers;
using J2N.CodeGeneration;
using J2N.Collections;
using J2N.Collections.Generic;
using J2N.Numerics;
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
    /// <see cref="MutableTextBuffer"/> differs from <see cref="StringBuilder"/> in the following ways:
    /// 
    /// <list type="bullet">
    ///     <item><description>
    ///         Rather than managing chunks of memory, <see cref="MutableTextBuffer"/> manages a single contiguous
    ///         block of <see cref="char"/>s.
    ///     </description></item>
    ///     <item><description>
    ///         Memory is directly accessible using <see cref="MutableTextBufferExtensions.AsSpan(MutableTextBuffer?)"/> and
    ///         <see cref="MutableTextBufferExtensions.AsMemory(MutableTextBuffer?)"/> overloads including the ability to slice.
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
    public partial class MutableTextBuffer : IAppendable, ISpanAppendable, ICharSequence, IBufferWriter<char>,
        ISpannable<char>, ICopyable<char>, ISpanCopyable<char>, IDisposable
        //, IEnumerable<char> // ICU4N TODO: Implement?
    {
        private const int CharStackBufferSize = 32;

        private readonly IArrayAllocator<char> allocator;

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
        /// The default capacity of an <see cref="MutableTextBuffer"/>.
        /// </summary>
        internal const int DefaultCapacity = 16;

        /// <summary>
        /// Whether to clear unwritten buffers before returning them to the user.
        /// </summary>
        private bool clearExposedBuffers = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class.
        /// </summary>
        /// <remarks>
        /// The string value of this instance is set to <see cref="string.Empty"/>, and the capacity is set to
        /// the implementation-specific default capacity.
        /// </remarks>
        public MutableTextBuffer(IArrayAllocator<char> allocator)
        {
            if (allocator is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.allocator);
            this.allocator = allocator;
            // J2N: We rely on Initialize() to properly set up the state, but it is considered
            // an optional operation.
            m_MaxCapacity = Arrays.MaxArrayLength;
            m_Chars = Arrays.Empty<char>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the buffers that are returned with unwritten
        /// chars beyond <see cref="Length"/> are cleared of potentially sensitive data.
        /// The default value is <see langword="false"/>.
        /// </summary>
        [CodeGenerationIgnore]
        public bool ClearExposedBuffers
        {
            get => clearExposedBuffers;
            init => clearExposedBuffers = value;
        }

        private void MakeRoom(int index, int count)
        {
            //AssertInvariants();
            Debug.Assert(count > 0);
            Debug.Assert(index >= 0);
            Debug.Assert(index <= m_Position);

            if (count + Length > m_MaxCapacity || count + Length < count)
            {
                throw new ArgumentOutOfRangeException("requiredLength", SR.ArgumentOutOfRange_SmallCapacity);
            }

            // Cool, we have some space in this block, and we don't have to copy much to get at it, so go ahead and use it.
            // This typically happens when someone repeatedly inserts small strings at a spot (usually the absolute front) of the buffer.
            if (m_Chars.Length - m_Position >= count)
            {
                new ReadOnlySpan<char>(m_Chars, index, m_Position - index)
                    .CopyTo(m_Chars.AsSpan(index + count));

                m_Position += count;
                return;
            }

            // Allocate the new array
            char[] newArray = allocator.Allocate(CalculateNewArrayLength(count));

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

            // We are done with the old array
            allocator.Return(m_Chars);

            // Wire in the new array
            m_Chars = newArray;
            m_Position += count;

            //AssertInvariants();
        }

        /// <summary>
        /// Gets the underlying storage of the builder.
        /// </summary>
        /// <remarks>
        /// This property does not clear the underlying storage, but returns the raw unfiltered bytes
        /// in writable form.
        /// </remarks>
        [CodeGenerationIgnore]
        public Span<char> RawChars => m_Chars;

        /// <summary>
        /// Gets or sets the maximum number of characters that can be contained in the memory allocated by the current instance.
        /// </summary>
        /// <value>The maximum number of characters that can be contained in the memory allocated by the current instance.
        /// Its value can range from <see cref="Length"/> to <see cref="MutableTextBuffer.MaxCapacity"/>.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value specified for a set operation is less than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The value specified for a set operation is greater than the maximum capacity.
        /// </exception>
        /// <remarks>
        /// <see cref="Capacity"/> does not affect the string value of the current instance. <see cref="Capacity"/> can
        /// be decreased as long as it is not less than <see cref="Length"/>.
        /// <para/>
        /// The <see cref="MutableTextBuffer"/> dynamically allocates more space when required and increases
        /// <see cref="Capacity"/> accordingly. For performance reasons, a <see cref="MutableTextBuffer"/> might
        /// allocate more memory than needed. The amount of memory allocated is implementation-specific.
        /// </remarks>
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
                    ReplaceBuffer(newCapacity: value);
                }
            }
        }

        /// <summary>
        /// Gets the maximum capacity of this instance.
        /// </summary>
        /// <value>The maximum number of characters this instance can hold.</value>
        /// <remarks>
        /// The maximum capacity for this implementation is <c>Array.MaxLength</c> on .NET 6.0
        /// or higher. On earlier versions of .NET, the maximum capacity is <c>2_146_435_071</c>.
        /// You can explicitly set the maximum capacity of a <see cref="MutableTextBuffer"/>
        /// object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        public int MaxCapacity => m_MaxCapacity;

        /// <summary>
        /// Ensures that the capacity of this builder is at least the specified value.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// If the current capacity is less than the <paramref name="capacity"/> parameter,
        /// memory for this instance is reallocated to hold at least <paramref name="capacity"/> number
        /// of characters; otherwise, no memory is changed.
        /// </remarks>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            if (capacity > m_MaxCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException(capacity, ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_Capacity);

            if (capacity > m_Chars.Length)
            {
                ReplaceBuffer(CalculateNewArrayLength(capacity - m_Position));
            }

            return m_Chars.Length;
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="string"/>.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        /// <remarks>
        /// This method causes a heap allocation. As an allocation-free alternative,
        /// you may call the <see cref="MutableTextBufferExtensions.AsSpan(MutableTextBuffer?)"/> method
        /// to get a <see cref="ReadOnlySpan{T}"/> representing the characters of this
        /// <see cref="MutableTextBuffer"/> instance.
        /// <para/>
        /// Call the <see cref="ToString()"/> method to convert the
        /// <see cref="MutableTextBuffer"/> object to a <see cref="string"/> object before
        /// you can pass the string represented by the <see cref="MutableTextBuffer"/> object to
        /// a method that has a <see cref="string"/> parameter or display it in the user interface.
        /// </remarks>
        public override string ToString()
        {
            //AssertInvariants();

            if (Length == 0)
            {
                return string.Empty;
            }

            return m_Chars.AsSpan(0, m_Position).ToString();
        }

        /// <summary>
        /// Converts the value of a substring of this instance to a <see cref="string"/>.
        /// </summary>
        /// <param name="startIndex">The starting position of the substring in this instance.</param>
        /// <returns>A string whose value is the same as the specified substring of this instance.
        /// That is, from <paramref name="startIndex"/> to the end of the string.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 or greater than
        /// <see cref="Length"/>.</exception>
        /// <remarks>
        /// This method causes a heap allocation. As an allocation-free alternative,
        /// you may call the <see cref="MutableTextBufferExtensions.AsSpan(MutableTextBuffer?, int)"/> method
        /// to get a <see cref="ReadOnlySpan{T}"/> representing the characters of the substring of this
        /// <see cref="MutableTextBuffer"/> instance.
        /// <para/>
        /// Call the <see cref="ToString(int)"/> method to convert the
        /// <see cref="MutableTextBuffer"/> object to a <see cref="string"/> object before
        /// you can pass the string represented by the <see cref="MutableTextBuffer"/> object to
        /// a method that has a <see cref="string"/> parameter or display it in the user interface.
        /// </remarks>
        public string ToString(int startIndex)
        {
            if ((uint)startIndex > (uint)Length)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual();

            //AssertInvariants();
            return m_Chars.AsSpan(startIndex, m_Position - startIndex).ToString();
        }

        /// <summary>
        /// Converts the value of a substring of this instance to a <see cref="string"/>.
        /// </summary>
        /// <param name="startIndex">The starting position of the substring in this instance.</param>
        /// <param name="length">The length of the substring.</param>
        /// <returns>A string whose value is the same as the specified substring of this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// The sum of <paramref name="startIndex"/> and <paramref name="length"/> is greater than the length
        /// of the current instance.
        /// </exception>
        /// <remarks>
        /// This method causes a heap allocation. As an allocation-free alternative,
        /// you may call the <see cref="MutableTextBufferExtensions.AsSpan(MutableTextBuffer?, int, int)"/> method
        /// to get a <see cref="ReadOnlySpan{T}"/> representing the characters of the substring of this
        /// <see cref="MutableTextBuffer"/> instance.
        /// <para/>
        /// Call the <see cref="ToString(int)"/> method to convert the
        /// <see cref="MutableTextBuffer"/> object to a <see cref="string"/> object before
        /// you can pass the string represented by the <see cref="MutableTextBuffer"/> object to
        /// a method that has a <see cref="string"/> parameter or display it in the user interface.
        /// </remarks>
        public string ToString(int startIndex, int length)
        {
            int currentLength = Length;
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

        /// <summary>
        /// Removes all characters from the current <see cref="MutableTextBuffer"/> instance.
        /// </summary>
        /// <returns>An object whose <see cref="Length"/> is 0 (zero).</returns>
        /// <remarks><see cref="Clear"/> is a convenience method that is equivalent to setting
        /// the <see cref="Length"/> property of the current instance to 0 (zero).</remarks>
        [CodeGenerationReturnsSelf]
        public void Clear()
        {
            this.Length = 0;
        }

        /// <summary>
        /// Gets or sets the length of the current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <value>The length of this instance.</value>
        /// <exception cref="ArgumentOutOfRangeException">The value specified for a set operation
        /// is less than zero or greater than <see cref="MaxCapacity"/>.</exception>
        /// <remarks>
        /// The length of a <see cref="MutableTextBuffer"/> object is defined by its number of 
        /// <see cref="char"/> objects.
        /// <para/>
        /// Like the <see cref="string.Length"/> property, the <see cref="Length"/> property indicates
        /// the length of the current string object. Unlike the <see cref="string.Length"/> property,
        /// which is read-only, the <see cref="Length"/> property allows you to modify the length of
        /// the string stored to the <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// If the specified length is less than the current length, the current <see cref="MutableTextBuffer"/>
        /// object is truncated to the specified length. If the specified length is greater than the current
        /// length, the end of the string value of the current <see cref="MutableTextBuffer"/> object is padded
        /// with the Unicode NULL character (U+0000).
        /// <para/>
        /// If the specified length is greater than the current capacity, <see cref="Capacity"/> increases so
        /// that it is greater than or equal to the specified length.
        /// </remarks>
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
        /// <value>The Unicode character at position <paramref name="index"/>.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside
        /// the bounds of this instance while setting a character.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is outside the bounds
        /// of this instance while getting a character.</exception>
        /// <remarks>
        /// The index parameter is the position of a character within the <see cref="MutableTextBuffer"/>.
        /// The first character in the string is at index 0. The length of a string is the number of
        /// characters it contains. The last accessible character of a <see cref="MutableTextBuffer"/> instance
        /// is at index Length - 1.
        /// <para/>
        /// <see cref="this[int]"/> is the default property of the <see cref="MutableTextBuffer"/>  class.
        /// In C#, it is an indexer. This means that individual characters can be retrieved from the <see cref="this[int]"/>
        /// property as shown in the following example, which counts the number of alphabetic, white-space, and punctuation
        /// characters in a string.
        /// <code>
        /// using System;
        /// using System.Text;
        /// 
        /// public class Example
        /// {
        ///     public static void Main()
        ///     {
        ///         int nAlphabeticChars = 0;
        ///         int nWhitespace = 0;
        ///         int nPunctuation = 0;
        ///         MutableTextBuffer sb = new MutableTextBuffer("This is a simple sentence.");
        ///
        ///         for (int ctr = 0; ctr &lt; sb.Length; ctr++)
        ///         {
        ///             char ch = sb[ctr];
        ///             if (char.IsLetter(ch)) { nAlphabeticChars++; continue; }
        ///             if (char.IsWhiteSpace(ch)) { nWhitespace++; continue; }
        ///             if (char.IsPunctuation(ch)) nPunctuation++;
        ///         }
        ///
        ///         Console.WriteLine("The sentence '{0}' has:", sb);
        ///         Console.WriteLine("   Alphabetic characters: {0}", nAlphabeticChars);
        ///         Console.WriteLine("   White-space characters: {0}", nWhitespace);
        ///         Console.WriteLine("   Punctuation characters: {0}", nPunctuation);
        ///     }
        /// }
        /// // The example displays the following output:
        /// //       The sentence 'This is a simple sentence.' has:
        /// //          Alphabetic characters: 21
        /// //          White-space characters: 4
        /// //          Punctuation characters: 1
        /// </code>
        /// <para/>
        /// Unlike the <see cref="StringBuilder"/> class, <see cref="MutableTextBuffer"/>'s indexer does
        /// not suffer from degraded performance due to chunky memory, since <see cref="MutableTextBuffer"/>
        /// uses a single contiguous block of characters in memory.
        /// </remarks>
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

        /// <summary>
        /// Appends a specified number of copies of the string representation of a Unicode character to this instance.
        /// </summary>
        /// <param name="value">The character to append.</param>
        /// <param name="repeatCount">The number of times to append value.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="repeatCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="OutOfMemoryException">Out of memory.</exception>
        /// <remarks>
        /// The <see cref="Append(char, int)"/> method modifies the existing instance of this class;
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char"/>
        [CodeGenerationReturnsSelf]
        public void Append(char value, int repeatCount)
        {
            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            if (repeatCount == 0)
            {
                return;
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

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="charCount">The number of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and <paramref name="startIndex"/>
        /// and <paramref name="charCount"/> are not zero.</exception>
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
        /// <paramref name="value"/> is <c>null</c> and <paramref name="startIndex"/> and <paramref name="charCount"/>
        /// are both zero, no changes are made.
        /// <para/>
        /// The <see cref="MutableTextBuffer.Append(char[], int, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to an <see cref="MutableTextBuffer"/> object,
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char"/>
        [CodeGenerationReturnsSelf]
        public void Append(char[]? value, int startIndex, int charCount)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (charCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);

            if (value == null)
            {
                if (startIndex == 0 && charCount == 0)
                {
                    return;
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
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="MutableTextBuffer.Append(string?)"/> method modifies the existing instance of this class;
        /// it does not return a new class instance. Because of this, you can call a method or
        /// property on the existing reference and you do not have to assign the return value
        /// to an <see cref="MutableTextBuffer"/> object, as the following example illustrates.
        /// <code>
        /// bool flag = false;
        /// J2N.Text.MutableTextBuffer sb = new J2N.Text.MutableTextBuffer();
        /// sb.Append("The value of the flag is ").Append(flag).Append(".");
        /// Console.WriteLine(sb.ToString());
        /// // The example displays the following output:
        /// //       The value of the flag is False.
        /// </code>
        /// <para/>
        /// If <paramref name="value"/> is <c>null</c>, no changes are made.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="string"/>
        [CodeGenerationReturnsSelf]
        public void Append(string? value)
        {
            if (value is not null)
            {
                Append(ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
            }
        }

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// </summary>
        /// <param name="value">The string that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and <paramref name="startIndex"/>
        /// and <paramref name="count"/> are not zero.</exception>
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
        /// <paramref name="value"/> is <c>null</c> and <paramref name="startIndex"/> and <paramref name="count"/>
        /// are both zero, no changes are made.
        /// <para/>
        /// The <see cref="Append(string, int, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to an <see cref="MutableTextBuffer"/> object,
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you
        /// call the <see cref="MutableTextBuffer.Append(string?)"/> and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/>
        /// methods to append small strings.
        /// </remarks>
        /// <seealso cref="string"/>
        [CodeGenerationReturnsSelf]
        public void Append(string? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
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
        }

        /// <summary>
        /// Appends the string representation of a specified string builder to this instance.
        /// </summary>
        /// <param name="value">The string builder to append.</param>
        /// <returns>A reference to this instance after the append operation is completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="MutableTextBuffer.Append(StringBuilder?)"/> method modifies the existing instance of this class;
        /// it does not return a new class instance. Because of this, you can call a method or
        /// property on the existing reference and you do not have to assign the return value
        /// to an <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// If <paramref name="value"/> is <c>null</c>, no changes are made.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="StringBuilder"/>
        [CodeGenerationReturnsSelf]
        public void Append(StringBuilder? value)
        {
            if (value != null && value.Length != 0)
            {
                AppendCore(value, 0, value.Length);
            }
        }

        /// <summary>
        /// Appends a copy of a specified substring of a string builder to this instance.
        /// </summary>
        /// <param name="value">The string builder that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and <paramref name="startIndex"/>
        /// and <paramref name="count"/> are not zero.</exception>
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
        /// <paramref name="value"/> is <c>null</c> and <paramref name="startIndex"/> and <paramref name="count"/>
        /// are both zero, no changes are made.
        /// <para/>
        /// The <see cref="MutableTextBuffer.Append(StringBuilder?, int, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to an <see cref="MutableTextBuffer"/> object,
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="StringBuilder"/>
        [CodeGenerationReturnsSelf]
        public void Append(StringBuilder? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (count == 0)
            {
                return;
            }

            if (count > value.Length - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            AppendCore(value, startIndex, count);
        }

        private void AppendCore(StringBuilder value, int startIndex, int count)
        {
            uint newLength = (uint)Length + (uint)count;

            if (newLength > (uint)m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.Capacity, ExceptionResource.ArgumentOutOfRange_Capacity);
            }

            int pos = m_Position;
            if (pos > m_Chars.Length - count)
            {
                Grow(count);
            }

            value.CopyTo(startIndex, m_Chars, m_Position, count);
            m_Position += count;
        }

        #region Custom Append

        [CodeGenerationReturnsSelf]
        public void Append(MutableTextBuffer? value)
        {
            if (value != null && value.Length != 0)
            {
                AppendCore(value, 0, value.Length);
            }
        }

        [CodeGenerationReturnsSelf]
        public void Append(MutableTextBuffer? value, int startIndex, int count)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (count == 0)
            {
                return;
            }

            if (count > value.Length - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            AppendCore(value, startIndex, count);
        }

        private void AppendCore(MutableTextBuffer value, int startIndex, int count)
        {
            if (value == this)
            {
                Append(value.AsSpan(startIndex, count));
                return;
            }

            uint newLength = (uint)Length + (uint)count;

            if (newLength > (uint)m_MaxCapacity)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.Capacity, ExceptionResource.ArgumentOutOfRange_Capacity);
            }

            int pos = m_Position;
            if (pos > m_Chars.Length - count)
            {
                Grow(count);
            }

            value.CopyTo(startIndex, m_Chars, m_Position, count);
            m_Position += count;
        }

        #endregion Custom Append

        /// <summary>
        /// Appends the default line terminator to the end of the current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendLine() => Append(Environment.NewLine);

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of the
        /// current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="string"/>
        [CodeGenerationReturnsSelf]
        public void AppendLine(string? value)
        {
            Append(value);
            Append(Environment.NewLine);
        }

        /// <summary>
        /// Appends a copy of the specified sequence of characters followed by the default line terminator to the end of the
        /// current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <param name="value">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed
        /// <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="ReadOnlySpan{Char}"/>
        [CodeGenerationReturnsSelf]
        public void AppendLine(ReadOnlySpan<char> value)
        {
            Append(value);
            Append(Environment.NewLine);
        }

        /// <summary>
        /// Copies the characters from a specified segment of this instance to a specified segment of a destination
        /// <see cref="char"/> array.
        /// </summary>
        /// <param name="sourceIndex">The starting position in this instance where characters will be copied from.
        /// The index is zero-based.</param>
        /// <param name="destination">The array where characters will be copied.</param>
        /// <param name="destinationIndex">The starting position in <paramref name="destination"/> where characters will be copied.
        /// The index is zero-based.</param>
        /// <param name="count">The number of characters to be copied.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceIndex"/>, <paramref name="destinationIndex"/>, or <paramref name="count"/>, is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="sourceIndex"/> is greater than the length of this instance.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourceIndex"/> + <paramref name="count"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="destinationIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="destination"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="CopyTo(int, char[], int, int)"/> method is intended to be used in the rare situation when you need to
        /// efficiently copy successive sections of a <see cref="MutableTextBuffer"/> object to an array. The array should be a
        /// fixed size, preallocated, reusable, and possibly globally accessible.
        /// <para/>
        /// To access the characters for processing without allocating any heap memory, better alternatives are to use
        /// <see cref="this[int]"/>, <see cref="MutableTextBufferExtensions.AsSpan(MutableTextBuffer?, int, int)"/> or <see cref="CopyTo(int, Span{char}, int)"/>.
        /// </remarks>
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
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_LongerThanSrcString);
            if ((uint)destinationIndex + (uint)count > destination.Length)
                ThrowHelper.ThrowArgumentException(ExceptionResource.ArgumentOutOfRange_OffsetOut);

            //AssertInvariants();

            m_Chars.AsSpan(sourceIndex, count).CopyTo(new Span<char>(destination).Slice(destinationIndex));
        }

        /// <summary>
        /// Copies the characters from a specified segment of this instance to a destination <see cref="char"/> span.
        /// </summary>
        /// <param name="sourceIndex">The starting position in this instance where characters will be copied from.
        /// The index is zero-based.</param>
        /// <param name="destination">The writable span where characters will be copied.</param>
        /// <param name="count">The number of characters to be copied.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceIndex"/> or <paramref name="count"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="sourceIndex"/> is greater than <see cref="Length"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourceIndex"/> + <paramref name="count"/> is greater than <see cref="Length"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="CopyTo(int, Span{char}, int)"/> method is intended to be used in the rare situation
        /// when you need to efficiently copy successive sections of a <see cref="MutableTextBuffer"/> object to a span.
        /// <para/>
        /// To access the characters for processing without alocating any heap memory, better alternatives are to use
        /// <see cref="this[int]"/> or <see cref="MutableTextBufferExtensions.AsSpan(MutableTextBuffer?, int, int)"/>.
        /// </remarks>
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
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_LongerThanSrcString);
            }

            //AssertInvariants();

            m_Chars.AsSpan(sourceIndex, count).CopyTo(destination);
        }

        /// <summary>
        /// Inserts one or more copies of a specified string into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The string to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after insertion has completed.</returns>
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
        /// This <see cref="MutableTextBuffer"/> object is not changed if <paramref name="value"/> is <c>null</c>, 
        /// <paramref name="value"/> is not <c>null</c> but its length is zero, or <paramref name="repeatCount"/> is zero.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, string? value, int repeatCount) => Insert(index, value.AsSpan(), repeatCount);

        /// <summary>
        /// Inserts one or more copies of a specified sequence of characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The sequence of characters to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after insertion has completed.</returns>
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
        [CodeGenerationReturnsSelf]
        public void Insert(int index, ReadOnlySpan<char> value, int repeatCount) // J2N: Made public to match ValueStringBuilder API
        {
            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            }

            if (value.IsEmpty || repeatCount == 0)
            {
                return;
            }

            // Ensure we don't insert more chars than we can hold, and we don't
            // have any integer overflow in our new length.
            long insertingChars = (long)value.Length * repeatCount;
            if (insertingChars > MaxCapacity - m_Position)
            {
                throw new OutOfMemoryException();
            }
            Debug.Assert(insertingChars + m_Position < int.MaxValue);

            int destinationLength = (int)insertingChars;
            if (destinationLength == 0)
                return;

            if (m_Chars.AsSpan().Overlaps(value))
            {
                InsertOverlappingRepeated(index, value, destinationLength, repeatCount);
                return;
            }

            InsertRepeated(index, value, destinationLength);
        }

        private void InsertOverlappingRepeated(int index, ReadOnlySpan<char> value, int destinationLength, int repeatCount)
        {
            char[]? buffer = null;
            try
            {
                int valueLength = value.Length;
                Span<char> temp = valueLength <= CharStackBufferSize
                    ? stackalloc char[valueLength]
                    : (buffer = ArrayPool<char>.Shared.Rent(valueLength)).AsSpan(0, valueLength);

                value.CopyTo(temp);
                InsertRepeated(index, temp, destinationLength);
            }
            finally
            {
                if (buffer is not null)
                    ArrayPool<char>.Shared.Return(buffer);
            }
        }

        private void InsertRepeated(int index, ReadOnlySpan<char> value, int destinationLength)
        {
            MakeRoom(index, destinationLength);

            Span<char> destination =
                m_Chars.AsSpan(index, destinationLength);

            // We only copy from the source once. The remainder of the copies
            // are from destination to destination. This allows for more opportunities
            // for the BCL to optimize the copy.
            value.CopyTo(destination);

            int copied = value.Length;

            while (copied < destinationLength)
            {
                int remaining = destinationLength - copied;
                int copyLength = copied < remaining ? copied : remaining;

                destination.Slice(0, copyLength)
                    .CopyTo(destination.Slice(copied));

                copied += copyLength;
            }
        }

        /// <summary>
        /// Inserts one or more copies of a specified sequence of characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The sequence of characters to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after insertion has completed.</returns>
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
        [CodeGenerationReturnsSelf]
        public void Insert(int index, StringBuilder? value, int repeatCount)
        {
            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            }

            if (value is null || repeatCount == 0)
            {
                return;
            }

            // Ensure we don't insert more chars than we can hold, and we don't
            // have any integer overflow in our new length.
            long insertingChars = (long)value.Length * repeatCount;
            if (insertingChars > MaxCapacity - m_Position)
            {
                throw new OutOfMemoryException();
            }
            Debug.Assert(insertingChars + m_Position < int.MaxValue);

            int destinationLength = (int)insertingChars;

            if (destinationLength == 0)
                return;

            MakeRoom(index, destinationLength);

            int copied = value.Length;

            // We only copy from the source once. The remainder of the copies
            // are from destination to destination. This allows for more opportunities
            // for the BCL to optimize the copy.
            value.CopyTo(0, m_Chars, index, copied);

            Span<char> destination =
                m_Chars.AsSpan(index, destinationLength);

            while (copied < destinationLength)
            {
                int remaining = destinationLength - copied;
                int copyLength = copied < remaining ? copied : remaining;

                destination.Slice(0, copyLength)
                    .CopyTo(destination.Slice(copied));

                copied += copyLength;
            }
        }

        /// <summary>
        /// Inserts one or more copies of a specified sequence of characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The sequence of characters to insert.</param>
        /// <param name="repeatCount">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after insertion has completed.</returns>
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
        [CodeGenerationReturnsSelf]
        public void Insert(int index, ICharSequence? value, int repeatCount)
        {
            if (value is ISpannable<char> spannable)
            {
                Insert(index, spannable.AsSpan(), repeatCount);
                return;
            }

            if (repeatCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(repeatCount, ExceptionArgument.repeatCount);

            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            }

            if (value is null || repeatCount == 0)
            {
                return;
            }

            // Ensure we don't insert more chars than we can hold, and we don't
            // have any integer overflow in our new length.
            long insertingChars = (long)value.Length * repeatCount;
            if (insertingChars > MaxCapacity - m_Position)
            {
                throw new OutOfMemoryException();
            }
            Debug.Assert(insertingChars + m_Position < int.MaxValue);

            int destinationLength = (int)insertingChars;
            if (destinationLength == 0)
                return;

            MakeRoom(index, destinationLength);

            int copied = value.Length;

            Span<char> destination =
                m_Chars.AsSpan(index, destinationLength);

            // We only copy from the source once. The remainder of the copies
            // are from destination to destination. This allows for more opportunities
            // for the BCL to optimize the copy.
            if (value is ISpanCopyable<char> spanCopyable)
            {
                spanCopyable.CopyTo(0, destination, copied);
            }
            else if (value is ICopyable<char> copyable)
            {
                copyable.CopyTo(0, m_Chars, index, copied);
            }
            else
            {
                for (int i = 0; i < copied; i++)
                {
                    destination[i] = value[i];
                }
            }

            while (copied < destinationLength)
            {
                int remaining = destinationLength - copied;
                int copyLength = copied < remaining ? copied : remaining;

                destination.Slice(0, copyLength)
                    .CopyTo(destination.Slice(copied));

                copied += copyLength;
            }
        }

        /// <summary>
        /// Removes the specified range of characters from this instance.
        /// </summary>
        /// <param name="startIndex">The zero-based position in this instance where removal begins.</param>
        /// <param name="length">The number of characters to remove.</param>
        /// <returns>A reference to this instance after the excise operation has completed.</returns>
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
        [CodeGenerationReturnsSelf]
        public void Remove(int startIndex, int length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);

            if (length > m_Position - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(length, ExceptionArgument.length);
            }

            RemoveCore(startIndex, length);
        }

        /// <summary>
        /// Removes the character at the specified index from this instance.
        /// </summary>
        /// <param name="index">The zero-based position in this instance of the character to remove.</param>
        /// <returns>A reference to this instance after the excise operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or
        /// greater than or equal to the length of this instance.</exception>
        /// <remarks>
        /// The current method removes the specified character from the current instance. The characters at
        /// (<paramref name="index"/> + 1) are moved to <paramref name="index"/>, and
        /// the string value of the current instance is shortened by 1. The capacity of the
        /// current instance is unaffected.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void RemoveAt(int index) // Coverage for the JDK (deleteCharAt)
        {
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);

            int currentLength = Length;
            if ((uint)index >= (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index, ExceptionArgument.index);
            }

            RemoveCore(index, 1);
        }

        private void RemoveCore(int startIndex, int length)
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
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<char> GetClearedWritableSpan(int start, int length)
        {
            Span<char> span = m_Chars.AsSpan(start, length);
            span.Fill('\0');
            return span;
        }

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance
        /// in lowercase.
        /// </summary>
        /// <param name="value">The Boolean value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <remarks>
        /// This matches the behavior of Java's StringBuilder. To match the behavior
        /// of .NET, call <see cref="Insert(int, bool, BooleanFormat)"/> and specify <see cref="BooleanFormat.TitleCase"/>.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="bool"/>
        [CodeGenerationReturnsSelf]
        public void Append(bool value) => Append(value, format: BooleanFormat.Lowercase);

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance
        /// in the specified format.
        /// </summary>
        /// <param name="value">The Boolean value to append.</param>
        /// <param name="format">The format to use. Specify <see cref="BooleanFormat.Lowercase"/> to match Java.
        /// Specify <see cref="BooleanFormat.TitleCase"/> to match .NET.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <remarks>The capacity of this instance is adjusted as needed. </remarks>
        /// <seealso cref="bool"/>
        /// <seealso cref="BooleanFormat"/>
        [CodeGenerationReturnsSelf]
        public void Append(bool value, BooleanFormat format)
        {
            string text = FormatBoolean(value, format);
            Append(ref MemoryMarshal.GetReference(text.AsSpan()), text.Length);
        }

        /// <summary>
        /// Appends the string representation of a specified <see cref="char"/> object to this instance.
        /// </summary>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <remarks>
        /// The <see cref="Append(char)"/> method modifies the existing instance of this class;
        /// it does not return a new class instance. Because of this, you can call a method or property
        /// on the existing reference and you do not have to assign the return value to an <see cref="MutableTextBuffer"/>
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char"/>
        [CodeGenerationReturnsSelf]
        public void Append(char value)
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
        }

        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(object? value, string? format, IFormatProvider? provider)
        {
            if (value is null)
                return; // no-op
#if FEATURE_SPANFORMATTABLE
            else if (value is ISpanFormattable spanFormattable) // J2N: Check for ISpanFormattable reference types, as this will improve performance.
                AppendSpanFormattable(spanFormattable, format, provider);
#else
            else if (value is Number number) // J2N: Check for Number-derived reference types, as this will improve performance.
                AppendSpanFormattable(number, format, provider);
#endif
            else if (value is IStructuralFormattable structuralFormattable)
                Append(structuralFormattable.ToString(format, provider));
            else if (value is IFormattable formattable)
                Append(formattable.ToString(format, provider));
            else if (value is ICharSequence csq)
                Append(csq); // doesn't support format providers
            else
                Append(value.ToString());
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified array to this instance.
        /// </summary>
        /// <param name="value">The array of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method appends the characters in the specified array to the current instance in the same order they
        /// appear in value. If <paramref name="value"/> is <c>null</c>, no changes are made.
        /// <para/>
        /// The <see cref="MutableTextBuffer.Append(char[])"/> method modifies the existing instance of this class; it does not
        /// return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to an <see cref="MutableTextBuffer"/> object,
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char"/>
        [CodeGenerationReturnsSelf]
        public void Append(char[]? value)
        {
            if (value is not null)
            {
#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
                Append(ref MemoryMarshal.GetArrayDataReference(value), value.Length);
#else
                Append(ref MemoryMarshal.GetReference(value), value.Length);
#endif
            }
        }

        /// <summary>
        /// Appends the string representation of a specified read-only character span to this instance.
        /// </summary>
        /// <param name="value">The read-only character span to append.</param>
        /// <returns>A reference to this instance after the append operation is completed.</returns>
        /// <seealso cref="ReadOnlySpan{Char}"/>
        [CodeGenerationReturnsSelf]
        public void Append(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
                return;

            if (m_Chars.AsSpan().Overlaps(value, out int sourceOffset))
            {
                AppendOverlapping(sourceOffset, value.Length);
                return;
            }

            Append(ref MemoryMarshal.GetReference(value), value.Length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendOverlapping(int sourceOffset, int valueCount)
        {
            // If the append fits in the existing array, Memmove already
            // supports overlap perfectly.
            if ((uint)m_Position + (uint)valueCount <= (uint)m_Chars.Length)
            {
                Append(ref m_Chars[sourceOffset], valueCount);
                return;
            }

            // If not, we snapshot the source so the operation is safe
            AppendWithSnapshot(sourceOffset, valueCount);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void AppendWithSnapshot(int sourceOffset, int valueCount)
            {
                char[]? buffer = null;
                try
                {
                    Span<char> temp = valueCount <= CharStackBufferSize
                        ? stackalloc char[valueCount]
                        : (buffer = ArrayPool<char>.Shared.Rent(valueCount)).AsSpan(0, valueCount);

                    m_Chars.AsSpan(sourceOffset, valueCount).CopyTo(temp);
                    Append(ref MemoryMarshal.GetReference(temp), temp.Length);
                }
                finally
                {
                    if (buffer is not null)
                        ArrayPool<char>.Shared.Return(buffer);
                }
            }
        }

        /// <summary>
        /// Appends the string representation of a specified read-only character memory region to this instance.
        /// </summary>
        /// <param name="value">The read-only character memory region to append.</param>
        /// <returns>A reference to this instance after the append operation is completed.</returns>
        /// <seealso cref="ReadOnlyMemory{Char}"/>
        [CodeGenerationReturnsSelf]
        public void Append(ReadOnlyMemory<char> value) => Append(value.Span);

        // J2N TODO: API - String interpolation for J2N formatters

        ///// <summary>Appends the specified interpolated string to this instance.</summary>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //[CodeGenerationReturnsSelf]
        //public void Append([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler) => this;

        ///// <summary>Appends the specified interpolated string to this instance.</summary>
        ///// <param name="provider">An object that supplies culture-specific formatting information.</param>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //[CodeGenerationReturnsSelf]
        //public void Append(IFormatProvider? provider, [InterpolatedStringHandlerArgument("", nameof(provider))] ref AppendInterpolatedStringHandler handler) => this;

        ///// <summary>Appends the specified interpolated string followed by the default line terminator to the end of the current MutableTextBuffer object.</summary>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //[CodeGenerationReturnsSelf]
        //public void AppendLine([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler) => AppendLine();

        ///// <summary>Appends the specified interpolated string followed by the default line terminator to the end of the current MutableTextBuffer object.</summary>
        ///// <param name="provider">An object that supplies culture-specific formatting information.</param>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //[CodeGenerationReturnsSelf]
        //public void AppendLine(IFormatProvider? provider, [InterpolatedStringHandlerArgument("", nameof(provider))] ref AppendInterpolatedStringHandler handler) => AppendLine();

        #region AppendJoin

        [CodeGenerationReturnsSelf]
        public void AppendJoin(string? separator, params object?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        [CodeGenerationReturnsSelf]
        public void AppendJoin(string? separator, params ReadOnlySpan<object?> values)
        {
            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        [CodeGenerationReturnsSelf]
        public void AppendJoin<T>(string? separator, IEnumerable<T> values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        [CodeGenerationReturnsSelf]
        public void AppendJoin(string? separator, params string?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        [CodeGenerationReturnsSelf]
        public void AppendJoin(string? separator, params ReadOnlySpan<string?> values)
        {
            separator ??= string.Empty;
            AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        [CodeGenerationReturnsSelf]
        public void AppendJoin(char separator, params object?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            AppendJoinCore(ref separator, 1, values);
        }

        [CodeGenerationReturnsSelf]
        public void AppendJoin(char separator, params ReadOnlySpan<object?> values) =>
            AppendJoinCore(ref separator, 1, values);

        [CodeGenerationReturnsSelf]
        public void AppendJoin<T>(char separator, IEnumerable<T> values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            AppendJoinCore(ref separator, 1, values);
        }

        [CodeGenerationReturnsSelf]
        public void AppendJoin(char separator, params string?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            AppendJoinCore(ref separator, 1, values);
        }

        [CodeGenerationReturnsSelf]
        public void AppendJoin(char separator, params ReadOnlySpan<string?> values) =>
            AppendJoinCore(ref separator, 1, values);

        private void AppendJoinCore<T>(ref char separator, int separatorLength, IEnumerable<T> values)
        {
            Debug.Assert(values != null);
            Debug.Assert(!Unsafe.IsNullRef(ref separator));
            Debug.Assert(separatorLength >= 0);

            using (IEnumerator<T> en = values!.GetEnumerator())
            {
                if (!en.MoveNext())
                {
                    return;
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
        }

        private void AppendJoinCore<T>(ref char separator, int separatorLength, ReadOnlySpan<T> values)
        {
            if (values.IsEmpty)
            {
                return;
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
        }

        #endregion AppendJoin

        /// <summary>
        /// Inserts a string into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The string to insert.</param>
        /// <returns>A reference to this instance after the insert operation has completed.</returns>
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
        /// This instance of <see cref="MutableTextBuffer"/> is not changed if <paramref name="value"/> is <c>null</c>,
        /// or <paramref name="value"/> is not <c>null</c> but its length is zero.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, string? value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value != null)
            {
                Insert(index, ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
            }
        }

        /// <summary>
        /// Inserts a <see cref="StringBuilder"/> into this instance at the specified character position.
        /// </summary>
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
        [CodeGenerationReturnsSelf]
        public void Insert(int index, StringBuilder? value)
        {
            if (value is null)
                return;

            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            int count = value.Length;
            if (count > 0)
            {
                MakeRoom(index, count);

                value.CopyTo(0, m_Chars, index, count);
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="count"/>.
        /// </summary>
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
        [CodeGenerationReturnsSelf]
        public void Insert(int index, StringBuilder? value, int startIndex, int count)
        {
            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (startIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            }

            if (startIndex > value.Length - count)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            if (count > 0)
            {
                MakeRoom(index, count);
                value.CopyTo(startIndex, m_Chars, index, count);
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified Boolean value to this instance
        /// in lowercase at the specifed character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the insert operation has completed.</returns>
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
        /// of .NET, call <see cref="Insert(int, bool, BooleanFormat)"/> and specify <see cref="BooleanFormat.TitleCase"/>.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// </remarks>
        /// <seealso cref="bool"/>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, bool value) => Insert(index, value, BooleanFormat.Lowercase);

        /// <summary>
        /// Inserts the string representation of a specified Boolean value to this instance
        /// in the specified format at the specified position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <param name="format">The format to use. Specify <see cref="BooleanFormat.Lowercase"/> to match Java.
        /// Specify <see cref="BooleanFormat.TitleCase"/> to match .NET.</param>
        /// <returns>A reference to this instance after the insert operation has completed.</returns>
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
        /// </remarks>
        /// <seealso cref="bool"/>
        /// <seealso cref="BooleanFormat"/>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, bool value, BooleanFormat format)
        {
            string text = FormatBoolean(value, format);
            // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
            // we want exceeding the maximum capacity to throw an OutOfMemoryException.
            Insert(index, text.AsSpan(), 1);
        }

        /// <summary>
        /// Inserts the string representation of a specified Unicode character into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the insert operation has completed.</returns>
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
        /// </remarks>
        /// <seealso cref="char"/>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, char value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            Insert(index, ref value, 1);
        }

        /// <summary>
        /// Inserts the string representation of a specified array of Unicode characters into this
        /// instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character array to insert.</param>
        /// <returns>A reference to this instance after the insert operation has completed.</returns>
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
        /// If <paramref name="value"/> is <c>null</c>, the <see cref="MutableTextBuffer"/> is not changed.
        /// </remarks>
        /// <seealso cref="char"/>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, char[]? value)
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
                Insert(index, ref MemoryMarshal.GetReference(value), value.Length);
#endif
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters
        /// into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the insert operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and <paramref name="startIndex"/>
        /// and <paramref name="charCount"/> are not zero.</exception>
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
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="char"/>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, char[]? value, int startIndex, int charCount)
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
                    return;
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
        }

        /// <summary>
        /// Inserts the string representation of a specified string
        /// into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the insert operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and <paramref name="startIndex"/>
        /// and <paramref name="count"/> are not zero.</exception>
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
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="char"/>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, string? value, int startIndex, int count) // J2N: Added to cover the JDK better (rather than ICharSequence only)
        {
            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value == null)
            {
                if (startIndex == 0 && count == 0)
                {
                    return;
                }
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            }

            if (startIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.charCount);
            }

            if (startIndex > value.Length - count)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);
            }

            if (count > 0)
            {
                Insert(index, ref MemoryMarshal.GetReference(value.AsSpan(startIndex)), count);
            }
        }

        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, object? value, string? format, IFormatProvider? provider)
        {
            if (value is null)
                return; // no-op;
#if FEATURE_SPANFORMATTABLE
            else if (value is ISpanFormattable spanFormattable) // J2N: Check for ISpanFormattable reference types, as this will improve performance.
                InsertSpanFormattable(index, spanFormattable, format, provider);
#else
            else if (value is Number number) // J2N: Check for Number-derived reference types, as this will improve performance.
                InsertSpanFormattable(index, number, format, provider);
#endif
            else if (value is IStructuralFormattable structuralFormattable)
                Insert(index, structuralFormattable.ToString(format, provider), 1);
            else if (value is IFormattable formattable)
                Insert(index, formattable.ToString(format, provider), 1);
            else if (value is ICharSequence csq)
                Insert(index, csq); // doesn't support format providers
            else
                Insert(index, value.ToString(), 1);
        }

        /// <summary>
        /// Inserts the sequence of characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character span to insert.</param>
        /// <returns>A reference to this instance after the insert operation has completed.</returns>
        /// <remarks>The existing characters are shifted to make room for the character sequence in the
        /// <paramref name="value"/> to insert it. The capacity is adjusted as needed.</remarks>
        /// <seealso cref="ReadOnlySpan{Char}"/>
        [CodeGenerationReturnsSelf]
        public void Insert(int index, ReadOnlySpan<char> value) // J2N NOTE: Weird that upstream they made an overload of ReadOnlyMemory<char> for Append, but not Insert.
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            if (value.Length != 0)
            {
                // There is a slight danger that value is actually a slice of m_Chars.
                if (m_Chars.AsSpan().Overlaps(value, out int sourceOffset))
                {
                    InsertOverlapping(index, sourceOffset, value.Length);
                    return;
                }

                Insert(index, ref MemoryMarshal.GetReference(value), value.Length);
            }
        }

        private void InsertOverlapping(int index, int sourceOffset, int count)
        {
            bool entirelyWithinLiveBuffer =
                (uint)sourceOffset <= (uint)m_Position &&
                (uint)sourceOffset + (uint)count <= (uint)m_Position;

            if (entirelyWithinLiveBuffer)
            {
                InsertFromSelf(index, sourceOffset, count);
                return;
            }

            // Snapshot the value to a temporary buffer and then insert
            InsertWithSnapshot(index, sourceOffset, count);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void InsertWithSnapshot(int index, int sourceOffset, int count)
            {
                char[]? buffer = null;
                try
                {
                    Span<char> temp = count <= CharStackBufferSize
                        ? stackalloc char[count]
                        : (buffer = ArrayPool<char>.Shared.Rent(count)).AsSpan(0, count);

                    m_Chars.AsSpan(sourceOffset, count).CopyTo(temp);
                    Insert(index, ref MemoryMarshal.GetReference(temp), count);
                }
                finally
                {
                    if (buffer is not null)
                        ArrayPool<char>.Shared.Return(buffer);
                }
            }
        }

        #region AppendFormat

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">An object to format.</param>
        /// <returns>A reference to this instance with format appended. Each format item in <paramref name="format"/> is replaced
        /// by the string representation of <paramref name="arg0"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 1.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items. The index of the format items must be 0,
        /// to correspond to <paramref name="arg0"/>, the single object in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of <paramref name="arg0"/>.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="arg0"/> represents the object to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of <paramref name="arg0"/>. If the format item includes <c>formatString</c>
        /// and <paramref name="arg0"/> implements the <see cref="IFormattable"/> interface, then <c>arg0.ToString(formatString, null)</c>
        /// defines the formatting. Otherwise, <c>arg0.ToString()</c> defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <paramref name="arg0"/> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            AppendFormat(null, format, MemoryMarshal.CreateReadOnlySpan(ref arg0, 1));
#else
            AppendFormat(null, format, new ParamsArray(arg0));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of either of two arguments.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <returns>A reference to this instance with format appended. Each format item in <paramref name="format"/> is replaced by the
        /// string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 2.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to <paramref name="arg0"/>
        /// and <paramref name="arg1"/>, the two objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="arg0"/> and <paramref name="arg1"/> represent the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of either <paramref name="arg0"/> or <paramref name="arg1"/>. If the format item includes <c>formatString</c>
        /// and the corresponding argument implements the <see cref="IFormattable"/> interface, then the argument's <c>ToString(formatString, null)</c>
        /// defines the formatting. Otherwise, the argument's <c>ToString()</c> defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <paramref name="arg0"/> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            TwoObjects two = new TwoObjects(arg0, arg1);
            AppendFormat(null, format, (ReadOnlySpan<object?>)two);
#else
            AppendFormat(null, format, new ParamsArray(arg0, arg1));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of either of three arguments.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        /// <returns>A reference to this instance with format appended. Each format item in <paramref name="format"/> is replaced by the
        /// string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 3.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to <paramref name="arg0"/>
        /// and <paramref name="arg1"/>, the two objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="arg0"/>, <paramref name="arg1"/>, and <paramref name="arg2"/> represent the objects to be formatted.
        /// Each format item in <paramref name="format"/> is replaced with the string representation of either <paramref name="arg0"/>, <paramref name="arg1"/>,
        /// or <paramref name="arg2"/>. If the format item includes <c>formatString</c> and the corresponding argument implements the
        /// <see cref="IFormattable"/> interface, then the argument's <c>ToString(formatString, null)</c> defines the formatting. Otherwise,
        /// the argument's <c>ToString()</c> defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <paramref name="arg0"/> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            ThreeObjects three = new ThreeObjects(arg0, arg1, arg2);
            AppendFormat(null, format, (ReadOnlySpan<object?>)three);
#else
            AppendFormat(null, format, new ParamsArray(arg0, arg1, arg2));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <returns>A reference to this instance with format appended. Each format item in <paramref name="format"/> is replaced by the string representation
        /// of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> or <paramref name="args"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> array.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="args"/> represents the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the corresponding object in <paramref name="args"/>. If the format item includes <c>formatString</c>
        /// and the corresponding object in <paramref name="args"/> implements the <see cref="IFormattable"/> interface, then
        /// <c>args[index].ToString(formatString, null)</c> defines the formatting. Otherwise, <c>args[index].ToString()</c>
        /// defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <c>args[0]</c> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        {
            if (args is null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormat(..., span).
                ThrowHelper.ThrowArgumentNullException(format is null ? ExceptionArgument.format : ExceptionArgument.args);
            }

            AppendFormat(null, format, args);
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding argument in a parameter span.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">A span of objects to format.</param>
        /// <returns>A reference to this instance with format appended. Each format item in <paramref name="format"/> is replaced by the string representation
        /// of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> or <paramref name="args"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> span.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// <paramref name="args"/> represents the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the corresponding object in <paramref name="args"/>. If the format item includes <c>formatString</c>
        /// and the corresponding object in <paramref name="args"/> implements the <see cref="IFormattable"/> interface, then
        /// <c>args[index].ToString(formatString, null)</c> defines the formatting. Otherwise, <c>args[index].ToString()</c>
        /// defines the formatting.
        /// <para/>
        /// If the string assigned to format is "Thank you for your donation of {0:####} cans of food to our charitable organization."
        /// and <c>args[0]</c> is an integer with the value 10, the return value will be "Thank you for your donation of 10 cans
        /// of food to our charitable organization."
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args)
        {
            AppendFormat(null, format, args);
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a single argument using a specified
        /// format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">An object to format.</param>
        /// <returns>A reference to this instance after the append operation has completed. After the append operation,
        /// this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format"/> in which any
        /// format specification is replaced by the string representation of <paramref name="arg0"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 1 (one).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items. The index of the format items must be zero (0),
        /// to correspond to <paramref name="arg0"/>, the single object in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of <paramref name="arg0"/>.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <c>args</c>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> if it is a numeric value.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> if it is a date and time value.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for <paramref name="arg0"/>.Typically, such an implementation also implements the
        ///     <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <c>null</c>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="arg0"/> represents the object to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of <paramref name="arg0"/>. If the format item includes <c>formatString</c>
        /// and <paramref name="arg0"/> implements the <see cref="IFormattable"/> interface, then <c>arg0.ToString(formatString, null)</c>
        /// defines the formatting. Otherwise, <c>arg0.ToString()</c> defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            AppendFormat(provider, format, MemoryMarshal.CreateReadOnlySpan(ref arg0, 1));
#else
            AppendFormat(provider, format, new ParamsArray(arg0));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of either of two arguments using a specified
        /// format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <returns>A reference to this instance after the append operation has completed. After the append operation,
        /// this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format"/> in which any
        /// format specification is replaced by the string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 2 (two).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <c>args</c>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> or <paramref name="arg1"/> if they are numeric values.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> or <paramref name="arg1"/> if they are date and time values.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/> or <paramref name="arg1"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for <paramref name="arg0"/> or <paramref name="arg1"/>.Typically, such an implementation also implements the
        ///     <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <c>null</c>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="arg0"/> and <paramref name="arg1"/> represent the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the object that has the corresponding index. If the format item includes <c>formatString</c>
        /// and the corresponding argument implements the <see cref="IFormattable"/> interface, then the argument's <c>ToString(formatString, null)</c>
        /// defines the formatting. Otherwise, the argument's <c>ToString()</c> defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            TwoObjects two = new TwoObjects(arg0, arg1);
            AppendFormat(provider, format, (ReadOnlySpan<object?>)two);
#else
            AppendFormat(provider, format, new ParamsArray(arg0, arg1));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of either of three arguments using a specified
        /// format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        /// <returns>A reference to this instance after the append operation has completed. After the append operation,
        /// this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format"/> in which any
        /// format specification is replaced by the string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 3 (three).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <c>args</c>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/>, <paramref name="arg1"/>, or <paramref name="arg2"/> if they are a numeric values.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/>, <paramref name="arg1"/>, or <paramref name="arg2"/> if they are date and time values.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///     <paramref name="arg0"/>, <paramref name="arg1"/>, or <paramref name="arg2"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for <paramref name="arg0"/>, <paramref name="arg1"/>, or <paramref name="arg2"/>.Typically, such an
        ///     implementation also implements the <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <c>null</c>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="arg0"/>, <paramref name="arg1"/>, and <paramref name="arg2"/> represent the objects to be formatted.
        /// Each format item in <paramref name="format"/> is replaced with the string representation of the object that has the
        /// corresponding index. If the format item includes <c>formatString</c> and the corresponding argument implements the
        /// <see cref="IFormattable"/> interface, then the argument's <c>ToString(formatString, null)</c> defines the formatting. Otherwise,
        /// the argument's <c>ToString()</c> defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            ThreeObjects three = new ThreeObjects(arg0, arg1, arg2);
            AppendFormat(provider, format, (ReadOnlySpan<object?>)three);
#else
            AppendFormat(provider, format, new ParamsArray(arg0, arg1, arg2));
#endif
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding argument in a
        /// parameter array using a specified format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <returns>A reference to this instance after the append operation has completed. After the append operation,
        /// this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format"/> in which any
        /// format specification is replaced by the string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> array.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <paramref name="args"/>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     numeric values in <paramref name="args"/>.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     date and time values in <paramref name="args"/>.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///      one or more of the objects in <paramref name="args"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for one or more of the objects in <paramref name="args"/>.Typically, such an implementation also implements the
        ///     <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <c>null</c>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="args"/> represents the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the corresponding object in <paramref name="args"/>. If the format item includes
        /// <c>formatString</c> and the corresponding object in <paramref name="args"/> implements the <see cref="IFormattable"/> interface, then
        /// <c>args[index].ToString(formatString, null)</c> defines the formatting. Otherwise, <c>args[index].ToString()</c>
        /// defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        {
            if (args is null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormat(..., span).
                ThrowHelper.ThrowArgumentNullException(format is null ? ExceptionArgument.format : ExceptionArgument.args);
            }

            AppendFormat(provider, format, (ReadOnlySpan<object?>)args);
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items,
        /// to this instance. Each format item is replaced by the string representation of a corresponding argument in a
        /// parameter span using a specified format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An span of objects to format.</param>
        /// <returns>A reference to this instance after the append operation has completed. After the append operation,
        /// this instance contains any data that existed before the operation, suffixed by a copy of <paramref name="format"/> in which any
        /// format specification is replaced by the string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> span.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of the expanded string would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting">
        /// composite formatting feature</a> of the .NET Framework to convert the value of an object to its text
        /// representation and embed that representation in the current <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The <paramref name="format"/> parameter consists of zero or more runs of text intermixed with
        /// zero or more indexed placeholders, called format items, that correspond to objects in the parameter list of this method.
        /// The formatting process replaces each format item with the string representation of the corresponding object.
        /// <para/>
        /// The syntax of a format item is as follows:
        /// <para/>
        /// <i>{index[,length][:formatString]}</i>
        /// <para/>
        /// Elements in square brackets are optional. The following table describes each element.
        /// <list type="table">
        ///   <listheader>
        ///     <description>Element</description>
        ///     <description>Descripton</description>
        ///   </listheader>
        ///   <item>
        ///     <description><i>index</i></description>
        ///     <description>
        ///       The zero-based position in the parameter list of the object to be formatted.
        ///       If the object specified by index is <c>null</c>, the format item is replaced by <see cref="String.Empty"/>.
        ///       If there is no parameter in the index position, a <see cref="FormatException"/> is thrown.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>,length</i></description>
        ///     <description>
        ///       The minimum number of characters in the string representation of the parameter. If positive,
        ///       the parameter is right-aligned; if negative, it is left-aligned.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description><i>:formatString</i></description>
        ///     <description>A standard or custom format string that is supported by the parameter.</description>
        ///   </item>
        /// </list>
        /// <para/>
        /// The provider parameter specifies an <see cref="IFormatProvider"/> implementation that can provide formatting information
        /// for the objects in <paramref name="args"/>. <paramref name="provider"/> can be any of the following:
        /// <list type="bullet">
        ///   <item><description>A <see cref="CultureInfo"/> object that provides culture-specific formatting information.</description></item>
        ///   <item><description>A <see cref="NumberFormatInfo"/> object that provides culture-specific formatting information for
        ///     numeric values in <paramref name="args"/>.</description></item>
        ///   <item><description>A <see cref="DateTimeFormatInfo"/> object that provides culture-specific formatting information for
        ///     date and time values in <paramref name="args"/>.</description></item>
        ///   <item><description>A <see cref="StringFormatter"/> object that provides culture-specific formatting information for
        ///      one or more of the objects in <paramref name="args"/> with rules similar to the JDK.</description></item>
        ///   <item><description></description>A custom <see cref="IFormatProvider"/> implementation that provides formatting
        ///     information for one or more of the objects in <paramref name="args"/>.Typically, such an implementation also implements the
        ///     <see cref="ICustomFormatter"/> interface.</item>
        /// </list>
        /// <para/>
        /// If the <paramref name="provider"/> parameter is <c>null</c>, formatting information is obtained from the current culture.
        /// <para/>
        /// <paramref name="args"/> represents the objects to be formatted. Each format item in <paramref name="format"/> is replaced
        /// with the string representation of the corresponding object in <paramref name="args"/>. If the format item includes
        /// <c>formatString</c> and the corresponding object in <paramref name="args"/> implements the <see cref="IFormattable"/> interface, then
        /// <c>args[index].ToString(formatString, null)</c> defines the formatting. Otherwise, <c>args[index].ToString()</c>
        /// defines the formatting.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
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
                        return;
                    }

                    ReadOnlySpan<char> remainder = format.AsSpan(pos);
                    int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                    if (countUntilNextBrace < 0)
                    {
                        Append(remainder);
                        return;
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
        private void AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, ParamsArray args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
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
                        return;
                    }

                    ReadOnlySpan<char> remainder = format.AsSpan(pos);
                    int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                    if (countUntilNextBrace < 0)
                    {
                        Append(remainder);
                        return;
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

        [CodeGenerationReturnsSelf]
        public void AppendFormat<TArg0>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(1);
            AppendFormat(provider, format, arg0, 0, 0, default);
        }

        [CodeGenerationReturnsSelf]
        public void AppendFormat<TArg0, TArg1>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(2);
            AppendFormat(provider, format, arg0, arg1, 0, default);
        }

        [CodeGenerationReturnsSelf]
        public void AppendFormat<TArg0, TArg1, TArg2>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(3);
            AppendFormat(provider, format, arg0, arg1, arg2, default);
        }

        [CodeGenerationReturnsSelf]
        public void AppendFormat(IFormatProvider? provider, CompositeFormat format, params object?[] args)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            if (args is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.args);
            AppendFormat(provider, format, (ReadOnlySpan<object?>)args);
        }

        [CodeGenerationReturnsSelf]
        public void AppendFormat(IFormatProvider? provider, CompositeFormat format, params ReadOnlySpan<object?> args)
        {
            //ArgumentNullException.ThrowIfNull(format);
            if (format is null)
                throw new ArgumentNullException(nameof(format));
            format.ValidateNumberOfArgs(args.Length);
            args.Length switch
            {
                0 => AppendFormat(provider, format, 0, 0, 0, args),
                1 => AppendFormat(provider, format, args[0], 0, 0, args),
                2 => AppendFormat(provider, format, args[0], args[1], 0, args),
                _ => AppendFormat(provider, format, args[0], args[1], args[2], args),
            };
        }

        private void AppendFormat<TArg0, TArg1, TArg2>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2, ReadOnlySpan<object?> args)
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

        #region Replace

        /// <summary>
        /// Replaces all occurrences of a specified string in this instance with another specified string.
        /// </summary>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <c>null</c>.</param>
        /// <returns>A reference to this instance with all instances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/> in the
        /// current instance. If <paramref name="newValue"/> is <c>null</c> or <see cref="string.Empty"/>, all occurrences of
        /// <paramref name="oldValue"/> are removed.
        /// </remarks>
        /// <seealso cref="Remove(int, int)"/>
        [CodeGenerationReturnsSelf]
        public void Replace(string oldValue, string? newValue) => Replace(oldValue, newValue, 0, Length);

        /// <summary>
        /// Replaces all instances of one read-only character span with another in this builder.
        /// </summary>
        /// <param name="oldValue">The read-only character span to replace.</param>
        /// <param name="newValue">The read-only character span to replace <paramref name="oldValue"/> with.</param>
        /// <returns>A reference to this instance with with all instances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/> in the
        /// current instance. If <paramref name="newValue"/> is empty, all occurrences of <paramref name="oldValue"/> are removed.
        /// </remarks>
        /// <seealso cref="Remove(int, int)"/>
        [CodeGenerationReturnsSelf]
        public void Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue) => Replace(oldValue, newValue, 0, Length);


        #endregion Replace


        #region Equals

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="sb">An object to compare with this instance, or <c>null</c>.</param>
        /// <returns><c>true</c> if the characters in this instance and <paramref name="sb"/> are the same;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The current instance and <paramref name="sb"/> are equal if the strings assigned to both
        /// <see cref="MutableTextBuffer"/> objects are the same. To determine equality, the
        /// <see cref="Equals(MutableTextBuffer)"/> method uses ordinal comparison.
        /// </remarks>
        public bool Equals([NotNullWhen(true)] MutableTextBuffer? sb)
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
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="sb">An object to compare with this instance, or <c>null</c>.</param>
        /// <returns><c>true</c> if the characters in this instance and <paramref name="sb"/> are the same;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The current instance and <paramref name="sb"/> are equal if the strings assigned to both
        /// objects are the same. To determine equality, the <see cref="Equals(MutableTextBuffer)"/>
        /// method uses ordinal comparison.
        /// </remarks>
        /// <remarks>
        /// The <see cref="Equals(StringBuilder)"/> method performs an ordinal comparison to determine
        /// whether the characters in the current instance and span are equal.
        /// </remarks>
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
        /// Returns a value indicating whether the characters in this instance are equal to the
        /// characters in a specified read-only character span.
        /// </summary>
        /// <param name="span">The character span to compare with the current instance.</param>
        /// <returns><c>true</c> if the characters in this instance and <paramref name="span"/> are the same;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="Equals(MutableTextBuffer)"/> method performs an ordinal comparison to determine
        /// whether the characters in the current instance and span are equal.
        /// </remarks>
        public bool Equals(ReadOnlySpan<char> span)
        {
            if (span.Length != Length)
            {
                return false;
            }

            return new ReadOnlySpan<char>(m_Chars, 0, m_Position).SequenceEqual(span);
        }

        #endregion

        #region Replace

        /// <summary>
        /// Replaces, within a substring of this instance, all occurrences of a specified string with another specified string.
        /// </summary>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <c>null</c>.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance with all instances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>
        /// in the range from <paramref name="startIndex"/> to <paramref name="startIndex"/> + <paramref name="count"/> - 1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <c>null</c>.</exception>
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
        /// in the specified substring. If <paramref name="newValue"/> is <c>null</c> or <see cref="string.Empty"/>,
        /// all occurrences of <paramref name="oldValue"/> in the specified range are removed.
        /// </remarks>
        /// <seealso cref="Remove(int, int)"/>
        [CodeGenerationReturnsSelf]
        public void Replace(string oldValue, string? newValue, int startIndex, int count)
        {
            if (oldValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.oldValue);
            Replace(oldValue.AsSpan(), newValue.AsSpan(), startIndex, count);
        }

        /// <summary>
        /// Replaces all instances of one read-only character span with another in a substring of this builder.
        /// </summary>
        /// <param name="oldValue">The read-only character span to replace.</param>
        /// <param name="newValue">The read-only character span to replace <paramref name="oldValue"/> with.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance with all instances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>
        /// in the range from <paramref name="startIndex"/> to <paramref name="startIndex"/> + <paramref name="count"/> - 1.</returns>
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
        /// <seealso cref="Remove(int, int)"/>
        [CodeGenerationReturnsSelf]
        public void Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
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
            try
            {
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
            }
            finally
            {
                replacements.Dispose();
            }

            //AssertInvariants();
        }

        /// <summary>
        /// Replaces all occurrences of a specified character in this instance with another specified character.
        /// </summary>
        /// <param name="oldChar">The character to replace.</param>
        /// <param name="newChar">The character that replaces <paramref name="oldChar"/>.</param>
        /// <returns>A reference to this instance with all occurrences of <paramref name="oldChar"/>
        /// replaced by <paramref name="newChar"/>.</returns>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of
        /// <paramref name="oldChar"/> in the current instance. The size of the current
        /// <see cref="MutableTextBuffer"/> instance is unchanged after the replacement.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void Replace(char oldChar, char newChar)
        {
            Replace(oldChar, newChar, 0, Length);
        }

        /// <summary>
        /// Replaces, within a substring of this instance, all occurrences of a specified character with another specified character.
        /// </summary>
        /// <param name="oldChar">The character to replace.</param>
        /// <param name="newChar">The character that replaces <paramref name="oldChar"/>.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring to search within.</param>
        /// <returns>A reference to this instance with <paramref name="oldChar"/> replaced by <paramref name="newChar"/>
        /// in the range from <paramref name="startIndex"/> to <paramref name="startIndex"/> + <paramref name="count"/> -1.</returns>
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
        [CodeGenerationReturnsSelf]
        public void Replace(char oldChar, char newChar, int startIndex, int count)
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
        }

        // JDK overloads

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
        /// <param name="startIndex">The inclusive begin index in this builder.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="newValue"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than or equal to <see cref="Length"/>.
        /// </exception>
        [CodeGenerationReturnsSelf]
        public void Replace(int startIndex, int count, string newValue)
        {
            if (newValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.newValue);
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            ReplaceCore(startIndex, count, newValue);
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
        /// <param name="startIndex">The inclusive begin index in this builder.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than or equal to <see cref="Length"/>.
        /// </exception>
        /// <remarks>
        /// This method allows <paramref name="newValue"/> to be this instance or a slice of this instance.
        /// </remarks>
        [CodeGenerationReturnsSelf]
        public void Replace(int startIndex, int count, ReadOnlySpan<char> newValue)
        {
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLessOrEqual(startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            ReplaceCore(startIndex, count, newValue);
        }

        private void ReplaceCore(int startIndex, int count, ReadOnlySpan<char> newValue)
        {
            Debug.Assert(startIndex >= 0 && startIndex <= m_Position);
            Debug.Assert(count >= 0);

            // Clamp to end of buffer (Harmony/JDK behavior)
            int end = count > m_Position - startIndex
                ? m_Position
                : startIndex + count; // Overflow not possible here

            int replacedLength = end - startIndex;

            if (m_Chars.AsSpan().Overlaps(newValue, out int sourceOffset))
            {
                ReplaceCoreOverlapping(startIndex, count, sourceOffset, newValue.Length, replacedLength, end);
                return;
            }

            int delta = newValue.Length - replacedLength;
            if (delta > 0)
            {
                // Need more space.
                //
                // Insert the additional space immediately after the replaced region.
                // This preserves the replacement area while shifting only the tail.
                MakeRoom(end, delta);
            }
            else if (delta < 0)
            {
                // Need less space.
                //
                // Remove only the excess characters after the replacement area.
                RemoveCore(startIndex + newValue.Length, -delta);
            }

            // Overwrite the replacement area.
            if (!newValue.IsEmpty)
            {
                newValue.CopyTo(m_Chars.AsSpan(startIndex));
            }
        }

        private void ReplaceCoreOverlapping(int startIndex, int count, int sourceOffset, int sourceLength, int replacedLength, int end)
        {
            Debug.Assert(startIndex <= int.MaxValue - sourceLength);

            // Fast path: Exact self-replacement (no-op)
            if (sourceOffset == startIndex && sourceLength == replacedLength)
            {
                return;
            }

            // Common case: Source entirely before the replacement region
            if ((uint)sourceOffset + (uint)sourceLength <= startIndex)
            {
                ReadOnlySpan<char> source = m_Chars.AsSpan(sourceOffset, sourceLength);

                int delta = sourceLength - replacedLength;

                if (delta > 0)
                {
                    MakeRoom(end, delta);
                }
                else if (delta < 0)
                {
                    RemoveCore(startIndex + sourceLength, -delta);
                }

                if (sourceLength > 0)
                {
                    source.CopyTo(m_Chars.AsSpan(startIndex));
                }
                return;
            }

            char[]? arrayToReturn = null;
            try
            {
                Span<char> temp = sourceLength <= CharStackBufferSize
                    ? stackalloc char[sourceLength]
                    : (arrayToReturn = allocator.Allocate(sourceLength)).AsSpan(0, sourceLength);

                m_Chars.AsSpan(sourceOffset, sourceLength)
                    .CopyTo(temp);

                int delta = sourceLength - replacedLength;

                if (delta > 0)
                {
                    MakeRoom(end, delta);
                }
                else if (delta < 0)
                {
                    RemoveCore(startIndex + sourceLength, -delta);
                }

                temp.CopyTo(m_Chars.AsSpan(startIndex));
            }
            finally
            {
                if (arrayToReturn is not null)
                    allocator.Return(arrayToReturn);
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
                RemoveCore(targetIndex, -delta);
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
            if (count == 0)
                return;

#if FEATURE_MEMORYMARSHAL_CREATESPAN
            MemoryMarshal.CreateSpan(ref value, count)
                .CopyTo(m_Chars.AsSpan(index));
#else
            unsafe
            {
                fixed (char* pSource = &value)
                {
                    new ReadOnlySpan<char>(pSource, count)
                        .CopyTo(m_Chars.AsSpan(index));
                }
            }
#endif

            index += count;
        }

        #endregion Replace

        /// <summary>
        /// Appends an array of Unicode characters starting at a specified address to this instance.
        /// </summary>
        /// <param name="value">A pointer to an array of characters.</param>
        /// <param name="valueCount">The number of characters in the array.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
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
        /// The <see cref="MutableTextBuffer.Append(char*, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to an <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        [CLSCompliant(false)]
        [CodeGenerationReturnsSelf]
        public unsafe void Append(char* value, int valueCount)
        {
            // We don't check null value as this case will throw null reference exception anyway
            if (valueCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(valueCount, ExceptionArgument.valueCount);

            if (valueCount == 0)
                return;

            if (Overlaps(value, valueCount, out int sourceOffset))
            {
                AppendOverlapping(sourceOffset, valueCount);
                return;
            }

            Append(ref *value, valueCount);
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
                    ref char destination = ref Unsafe.Add(ref MemoryMarshal.GetReference(chars), pos);
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
            // Doing the check here prevents corruption of the MutableTextBuffer.
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
            ref char destination = ref Unsafe.Add(ref MemoryMarshal.GetReference(chars), pos);
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
        /// Inserts an array of Unicode characters starting at a specified address into this instance.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A pointer to an array of characters.</param>
        /// <param name="valueCount">The number of characters in the array.</param>
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
        [CodeGenerationReturnsSelf]
        public unsafe void Insert(int index, char* value, int valueCount)
        {
            // We don't check null value as this case will throw null reference exception anyway
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }
            if (valueCount < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(valueCount, ExceptionArgument.valueCount);
            }
            // Check if the valueCount will put us over m_MaxCapacity.
            // Doing the check here prevents corruption of the MutableTextBuffer.
            int newLength = m_Position + valueCount;
            if (newLength > m_MaxCapacity || newLength < valueCount)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(valueCount, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
            }

            if (Overlaps(value, valueCount, out int sourceOffset))
            {
                InsertOverlapping(index, sourceOffset, valueCount);
                return;
            }

            Insert(index, ref *value, valueCount);
        }

        /// <summary>
        /// Determines whether <paramref name="value"/> points into the current backing
        /// array and, if so, returns its character offset.
        /// </summary>
        /// <param name="value">The source pointer.</param>
        /// <param name="valueCount">The number of characters that will be read.</param>
        /// <param name="sourceOffset">
        /// Receives the offset into <see cref="m_Chars"/> if this method returns
        /// <see langword="true"/>; otherwise -1.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the entire source range lies within
        /// <see cref="m_Chars"/>; otherwise <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe bool Overlaps(char* value, int valueCount, out int sourceOffset)
        {
            Debug.Assert(valueCount >= 0, "Invalid length; should have been validated by caller.");

            fixed (char* buffer = m_Chars)
            {
                nuint offset = (nuint)(value - buffer);

                if (offset <= (nuint)(m_Chars.Length - valueCount))
                {
                    sourceOffset = (int)offset;
                    return true;
                }
            }

            sourceOffset = -1;
            return false;
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

        private static string FormatBoolean(bool value, BooleanFormat format) =>
            // J2N: System.Boolean ignores the IFormatProvider that is passed to it,
            // so we are using a boolean flag for users to be able to specify whether to use
            // title casing (.NET) or lower casing (Java).
            format == BooleanFormat.Lowercase ? StringFormatter.FormatBoolean(value) : value.ToString();


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

            ReplaceBuffer(CalculateNewArrayLength(additionalCapacityBeyondPos));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateNewArrayLength(int additionalCapacityBeyondPos)
        {
            // J2N: Changed growth calculation to more closely parallel the JDK.
            uint minimum = (uint)(m_Position + additionalCapacityBeyondPos);

            uint preferred = Math.Min(
                ((uint)m_Chars.Length * 2) + 2,
                (uint)Arrays.MaxArrayLength);

            return (int)Math.Max(minimum, preferred);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReplaceBuffer(int newCapacity)
        {
            Debug.Assert(newCapacity >= m_Position);

            char[] oldBuffer = m_Chars;

            // Make sure to let the array allocation throw an exception if the caller has a bug and the desired capacity is negative.
            // This could also go negative if the actual required length wraps around.
            char[] newBuffer = allocator.Allocate(newCapacity);
            oldBuffer.AsSpan(0, m_Position).CopyTo(newBuffer);
            allocator.Return(oldBuffer);
            m_Chars = newBuffer;
        }

        /// <summary>
        /// Releases ownership of the underlying character buffer back to the
        /// <see cref="IArrayAllocator{T}"/> provided to this instance.
        /// </summary>
        /// <remarks>
        /// Once this method has been called, the current instance no longer owns
        /// the underlying buffer and further use of the instance is unsupported.
        /// <para/>
        /// Depending on the allocator implementation, the underlying array may be:
        /// <list type="bullet">
        ///     <item>
        ///         <description>Returned to an array pool for reuse.</description>
        ///     </item>
        ///     <item>
        ///         <description>Cleared before reuse.</description>
        ///     </item>
        ///     <item>
        ///         <description>Left uncleared for performance reasons.</description>
        ///     </item>
        ///     <item>
        ///         <description>Ignored entirely for non-pooled allocators.</description>
        ///     </item>
        /// </list>
        /// <para/>
        /// This method may be called multiple times safely.
        /// </remarks>
        [CodeGenerationIgnore]
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources owned by the current instance.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release managed resources; otherwise, <c>false</c>.
        /// </param>
        /// <remarks>
        /// Derived classes overriding this method should release any managed state
        /// when <paramref name="disposing"/> is <c>true</c>, and then call the base
        /// implementation.
        /// <para/>
        /// This implementation releases ownership of the underlying character buffer
        /// back to the configured <see cref="IArrayAllocator{T}"/>.
        /// </remarks>
        [CodeGenerationIgnore]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                char[]? array = m_Chars;

                if (array.Length != 0)
                {
                    m_Chars = Arrays.Empty<char>();
                    allocator.Return(array);
                }
            }
        }

        // J2N-specific methods

        // For testing
        internal char[] ToCharArray() => m_Position == m_Chars.Length ? m_Chars : m_Chars.AsSpan(0, m_Position).ToArray();

        // For testing
        internal char[] RawArray => m_Chars;

        /// <summary>
        /// Deletes a sequence of characters specified by <paramref name="startIndex"/> and <paramref name="count"/>.
        /// Shifts any remaining characters to the left.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// <para/>
        /// This method differs from <see cref="Remove(int, int)"/> in that it will automatically
        /// adjust the <paramref name="count"/> if <c><paramref name="startIndex"/> + <paramref name="count"/> > <see cref="Length"/></c>
        /// to <c><see cref="Length"/> - <paramref name="startIndex"/>.</c>, provided it is not bounded by <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of characters to delete.</param>
        /// <returns>This <see cref="MutableTextBuffer"/>, for chaining.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than <see cref="MutableTextBuffer.Length"/>.
        /// </exception>
        [CodeGenerationReturnsSelf]
        public void Delete(int startIndex, int count) // Coverage for the JDK
        {
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            int pos = m_Position;
            if ((uint)startIndex + (uint)count > pos)
                count = pos - startIndex;
            if (count > 0)
                RemoveCore(startIndex, count);
        }

        /// <summary>
        /// Causes this character sequence to be replaced by the reverse of
        /// the sequence. If there are any surrogate pairs included in the
        /// sequence, these are treated as single characters for the
        /// reverse operation. Thus, the order of the high-low surrogates
        /// is never reversed.
        /// <para/>
        /// IMPORTANT: This operation is done in-place. Although an <see cref="MutableTextBuffer"/>
        /// is returned, it is the SAME instance as the one that is passed in.
        /// <para/>
        /// Let <c>n</c> be the character length of this character sequence
        /// (not the length in <see cref="char"/> values) just prior to
        /// execution of the <see cref="MutableTextBuffer.Reverse()"/> method. Then the
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
        /// don't require an <see cref="MutableTextBuffer"/> instance.
        /// </summary>
        /// <returns>A reference to this <see cref="MutableTextBuffer"/>, for chaining.</returns>
        /// <seealso cref="StringExtensions.ReverseText(string)"/>
        /// <seealso cref="MemoryExtensions.ReverseText(Span{char})"/>
        /// <seealso cref="StringBuilderExtensions.Reverse(StringBuilder)"/>
        [CodeGenerationReturnsSelf]
        public void Reverse() // Coverage for the JDK
        {
            m_Chars.AsSpan(0, m_Position).ReverseText();
        }

        /// <summary>
        /// Sets the capacity of an <see cref="MutableTextBuffer"/> object to the actual number of characters
        /// it contains.
        /// </summary>
        /// <remarks>
        /// This method is similar to <c>trimToSize()</c> in the JDK.
        /// <para/>
        /// You can use the <see cref="TrimExcess()"/> method to minimize an <see cref="MutableTextBuffer"/> object's
        /// memory overhead once it is known that no new characters will be added. To completely clear an
        /// <see cref="MutableTextBuffer"/> object and release all memory referenced by it, call this method
        /// after calling the <see cref="Clear()"/> method or setting <see cref="Length"/> property to 0.
        /// <para/>
        /// If the capacity is already equal to the current length, this method has no effect.
        /// </remarks>
        public void TrimExcess() // Coverage for the JDK
        {
            if (m_Position < m_Chars.Length)
            {
                ReplaceBuffer(m_Position);
            }
        }

        /// <summary>
        /// Appends and returns a writable <see cref="Span{Char}"/> of the specified length to this builder.
        /// Writes to the returned span will update the value of this instance.
        /// </summary>
        /// <param name="length">The number of characters to append to this instance.</param>
        /// <returns>>A <see cref="Span{Char}"/> wrapping a block of memory that is appended to the existing
        /// sequence of characters. The span may be written to by the caller to update this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> plus the current length of this instance exceeds <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method allows callers to append a block of a specific length to this instance that can be written
        /// to after the fact. This is most useful for passing a span to an API that writes directly into a character buffer,
        /// which can save a copy operation if the data fits in the returned span.
        /// <para/>
        /// The capacity is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBuffer.Append(string?)"/>
        /// and <see cref="MutableTextBuffer.AppendFormat(string, object?)"/> methods to append small strings.
        /// </remarks>
        /// <synchronizationNote>
        /// The returned span provides direct access to the underlying memory of the <see cref="SynchronizedTextBuilder"/>.
        /// Callers must synchronize externally using <see cref="SynchronizedTextBuilder.SyncRoot"/> for the duration of the
        /// span usage if concurrent mutation is possible.
        /// </synchronizationNote>
        // J2N TODO: This idea was borrowed from ValueStringBuilder, but is effectively the same operation as IBufferWriter<T>.GetSpan(int).
        // There is a slight difference in that GetSpan() allows passing 0 to get a "default" buffer length and it does not move the m_Position -
        // it reserves that operation for the Advance(int) method after the writes are completed.
        [CodeGenerationSkipSynchronization]
        public Span<char> AppendSpan(int length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);

            int pos = m_Position;
            if (pos > m_Chars.Length - length)
            {
                Grow(length);
            }
            Span<char> buffer;

            if (!clearExposedBuffers)
            {
                buffer = m_Chars.AsSpan(pos, length);
            }
            else
            {
                // Ensure the buffer doesn't contain any sensitive data before providing it to the user
                buffer = GetClearedWritableSpan(pos, length);
            }
            m_Position += length;
            return buffer;
        }

#if FEATURE_INDEX_RANGE
        /// <summary>
        /// Inserts a copy of the specified range from this buffer at the specified index.
        /// </summary>
        /// <param name="index">The index at which the copied range will be inserted.</param>
        /// <param name="range">The range of characters to copy and insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than <see cref="Length"/>.
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
        [CodeGenerationReturnsSelf]
        public void InsertFromSelf(int index, Range range)
        {
            var (startIndex, count) = range.GetOffsetAndLength(Length);
            InsertFromSelf(index, startIndex, count);
        }
#endif

        /// <summary>
        /// Inserts a copy of a range of characters from this buffer
        /// at the specified index, expanding the <see cref="Length"/> by
        /// <paramref name="count"/>.
        /// </summary>
        /// <param name="index">The index at which the copied range will be inserted.</param>
        /// <param name="startIndex">The starting index of the source range to copy.</param>
        /// <param name="count">The number of characters to copy.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/>, or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> or <paramref name="startIndex"/> is greater
        /// than <see cref="Length"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater
        /// than <see cref="Length"/>.
        /// </exception>
        /// <remarks>
        /// This operation supports overlapping source and destination ranges.
        /// <para/>
        /// <paramref name="index"/> refers to the original buffer before insertion
        /// takes place.
        /// </remarks>
        // J2N: This idea was borrowed from the ReplaceableString in ICU4N. We do a copy operation within the current buffer
        // using low-level operations rather than relying on the high-level Replace and CopyTo() operations to do it.
        // This allows us to optimize ReplaceableString (thus Transliterator) in ICU4N much better.
        [CodeGenerationReturnsSelf]
        public void InsertFromSelf(int index, int startIndex, int count)
        {
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            int pos = m_Position;

            if ((uint)index > (uint)pos)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            if ((uint)startIndex > (uint)pos)
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(startIndex, ExceptionArgument.startIndex);

            // Combination validation
            if (count > pos - startIndex)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_LongerThanSrcString);

            // Fast path
            if (count == 0)
                return;

            if ((uint)pos + (uint)count > m_Chars.Length)
            {
                Grow(count);
            }

#if FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE
            ref char chars = ref MemoryMarshal.GetArrayDataReference(m_Chars);
#else
            ref char chars = ref MemoryMarshal.GetReference(m_Chars);
#endif

            int tailCount = pos - index;

            //
            // STEP 1:
            // Shift tail right to open insertion gap.
            //

            if (tailCount > 0)
            {
#if FEATURE_MEMORYMARSHAL_CREATEREADONLYSPAN
                MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref chars, index), tailCount)
                    .CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref chars, index + count), tailCount));
#else
                BufferHelper.Memmove(
                    ref Unsafe.Add(ref chars, index + count),
                    ref Unsafe.Add(ref chars, index),
                    (nuint)tailCount);
#endif
            }

            //
            // STEP 2:
            // Source shifts if insertion before source.
            //

            if (index < startIndex)
            {
                startIndex += count;
            }

            //
            // STEP 3:
            // Copy source into insertion gap.
            //

#if FEATURE_MEMORYMARSHAL_CREATEREADONLYSPAN
            MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref chars, startIndex), count)
                .CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref chars, index), count));
#else
            BufferHelper.Memmove(
                ref Unsafe.Add(ref chars, index),
                ref Unsafe.Add(ref chars, startIndex),
                (nuint)count);
#endif

            m_Position += count;
        }

        #region ISpannable<char> Members

        ReadOnlySpan<char> ISpannable<char>.AsSpan() => this.AsSpan();

        ReadOnlySpan<char> ISpannable<char>.AsSpan(int start) => this.AsSpan(start);

        ReadOnlySpan<char> ISpannable<char>.AsSpan(int start, int length) => this.AsSpan(start, length);

        #endregion ISpannable<char> Members
    }
}
