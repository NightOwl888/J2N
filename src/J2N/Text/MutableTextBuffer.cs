using J2N.Buffers;
using J2N.Collections;
using J2N.Collections.Generic;
using J2N.Numerics;
using J2N.Numerics.Formatters;
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
    ///         Memory is directly accessible using <see cref="TextMemoryExtensions.AsSpan(MutableTextBuffer)"/> and
    ///         <see cref="TextMemoryExtensions.AsMemory(MutableTextBuffer)"/> overloads including the ability to slice.
    ///         So, there is no need to allocate memory to call methods that require System.Memory types, such as
    ///         <see cref="ReadOnlySpan{T}"/>.
    ///     </description></item>
    ///     <item><description>
    ///         Indexing through <see cref="this[int]"/> is significantly faster than with <see cref="StringBuilder"/>.
    ///     </description></item>
    /// </list>
    /// </remarks>
    public partial class MutableTextBuffer : IAppendable, ISpanAppendable, ICharSequence
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
        /// The default capacity of an <see cref="MutableTextBuffer"/>.
        /// </summary>
        internal const int DefaultCapacity = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTextBuffer"/> class.
        /// </summary>
        /// <remarks>
        /// The string value of this instance is set to <see cref="string.Empty"/>, and the capacity is set to
        /// the implementation-specific default capacity.
        /// </remarks>
        public MutableTextBuffer()
        {
            // J2N: We rely on Initialize() to properly set up the state, but it is considered
            // an optional operation.
            m_MaxCapacity = int.MaxValue;
            m_Chars = Arrays.Empty<char>();
        }


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
            char[] newArray = AllocateBuffer(CalculateNewArrayLength(count));


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
            ReleaseBuffer(m_Chars);

            // Wire in the new array
            m_Chars = newArray;
            m_Position += count;

            //AssertInvariants();
        }

        /// <summary>
        /// Gets or sets the maximum number of characters that can be contained in the memory allocated by the current instance.
        /// </summary>
        /// <value>The maximum number of characters that can be contained in the memory allocated by the current instance.
        /// Its value can range from <see cref="Length"/> to <see cref="MaxCapacity"/>.</value>
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
        /// The maximum capacity for this implementation is <see cref="int.MaxValue"/>.
        /// However, this value is implementation-specific and might be different in other or
        /// later implementations. You can explicitly set the maximum capacity of a <see cref="MutableTextBuffer"/>
        /// object by calling the <see cref="MutableTextBuffer(int, int)"/> constructor.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
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
        /// you may call the <see cref="TextMemoryExtensions.AsSpan(MutableTextBuffer)"/> method
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
        /// you may call the <see cref="TextMemoryExtensions.AsSpan(MutableTextBuffer, int)"/> method
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
        /// you may call the <see cref="TextMemoryExtensions.AsSpan(MutableTextBuffer, int, int)"/> method
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
        public MutableTextBuffer Clear()
        {
            this.Length = 0;
            return this;
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
        /// Returns an object that can be used to iterate through the chunks of characters represented in a
        /// <see cref="ReadOnlyMemory{Char}"/> created from this <see cref="MutableTextBuffer"/> instance.
        /// </summary>
        /// <returns>An enumerator for the chunks in the <see cref="ReadOnlyMemory{Char}"/>.</returns>
        /// <remarks>This API is for compatibility with <c>StringBuilder.GetChuncks()</c> method.
        /// <see cref="MutableTextBuffer"/> will never have more than a single chunk of memory so it is generally more efficient
        /// to use <see cref="TextMemoryExtensions.AsSpan(MutableTextBuffer)"/> or
        /// <see cref="TextMemoryExtensions.AsMemory(MutableTextBuffer)"/> when you need to access the underlying memory.</remarks>
        public ChunkEnumerator GetChunks() => new ChunkEnumerator(this);


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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char"/>
        public MutableTextBuffer Append(char value, int repeatCount)
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method appends the specified range of characters in <paramref name="value"/> to the current instance. If
        /// <paramref name="value"/> is <c>null</c> and <paramref name="startIndex"/> and <paramref name="charCount"/>
        /// are both zero, no changes are made.
        /// <para/>
        /// The <see cref="Append(char[], int, int)"/> method modifies the existing instance of this class; it does
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char"/>
        public MutableTextBuffer Append(char[]? value, int startIndex, int charCount)
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

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="Append(string)"/> method modifies the existing instance of this class;
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="string"/>
        public MutableTextBuffer Append(string? value)
        {
            if (value is not null)
            {
                Append(ref MemoryMarshal.GetReference(value.AsSpan()), value.Length);
            }

            return this;
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="string"/>
        public MutableTextBuffer Append(string? value, int startIndex, int count)
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

        /// <summary>
        /// Appends the string representation of a specified string builder to this instance.
        /// </summary>
        /// <param name="value">The string builder to append.</param>
        /// <returns>A reference to this instance after the append operation is completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// The <see cref="Append(StringBuilder)"/> method modifies the existing instance of this class;
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="StringBuilder"/>
        public MutableTextBuffer Append(StringBuilder? value)
        {
            if (value != null && value.Length != 0)
            {
                return AppendCore(value, 0, value.Length);
            }
            return this;
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="StringBuilder"/>
        public MutableTextBuffer Append(StringBuilder? value, int startIndex, int count)
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

        private MutableTextBuffer AppendCore(StringBuilder value, int startIndex, int count)
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

        public MutableTextBuffer Append(MutableTextBuffer? value)
        {
            if (value != null && value.Length != 0)
            {
                return AppendCore(value, 0, value.Length);
            }
            return this;
        }

        public MutableTextBuffer Append(MutableTextBuffer? value, int startIndex, int count)
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

        private MutableTextBuffer AppendCore(MutableTextBuffer value, int startIndex, int count)
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

        /// <summary>
        /// Appends the default line terminator to the end of the current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed
        /// <see cref="MaxCapacity"/>.</exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendLine() => Append(Environment.NewLine);

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of the
        /// current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed
        /// <see cref="MaxCapacity"/>.</exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="string"/>
        public MutableTextBuffer AppendLine(string? value)
        {
            Append(value);
            return Append(Environment.NewLine);
        }

        /// <summary>
        /// Appends a copy of the specified sequence of characters followed by the default line terminator to the end of the
        /// current <see cref="MutableTextBuffer"/> object.
        /// </summary>
        /// <param name="value">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed
        /// <see cref="MaxCapacity"/>.</exception>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="ReadOnlySpan{Char}"/>
        public MutableTextBuffer AppendLine(ReadOnlySpan<char> value)
        {
            Append(value);
            return Append(Environment.NewLine);
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
        /// <see cref="this[int]"/>, <see cref="TextMemoryExtensions.AsSpan(MutableTextBuffer, int, int)"/> or <see cref="CopyTo(int, Span{char}, int)"/>.
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
                throw new ArgumentException(SR.Arg_LongerThanSrcString);
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
        /// <see cref="this[int]"/> or <see cref="TextMemoryExtensions.AsSpan(MutableTextBuffer, int, int)"/>.
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
                throw new ArgumentException(SR.Arg_LongerThanSrcString);
            }

            //AssertInvariants();

            m_Chars.AsSpan(sourceIndex, count).CopyTo(destination);
        }

        /// <summary>
        /// Inserts one or more copies of a specified string into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The string to insert.</param>
        /// <param name="count">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after insertion has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of <paramref name="value"/>
        /// times <paramref name="count"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// This <see cref="MutableTextBuffer"/> object is not changed if <paramref name="value"/> is <c>null</c>, 
        /// <paramref name="value"/> is not <c>null</c> but its length is zero, or <paramref name="count"/> is zero.
        /// </remarks>
        public MutableTextBuffer Insert(int index, string? value, int count) => Insert(index, value.AsSpan(), count);

        /// <summary>
        /// Inserts one or more copies of a specified sequence of characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The sequence of characters to insert.</param>
        /// <param name="count">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after insertion has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of <paramref name="value"/>
        /// times <paramref name="count"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// This <see cref="MutableTextBuffer"/> object is not changed if the length of <paramref name="value"/> is zero or
        /// <paramref name="count"/> is zero.
        /// </remarks>
        public MutableTextBuffer Insert(int index, ReadOnlySpan<char> value, int count) // J2N: Made public to match ValueStringBuilder API
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
        public MutableTextBuffer Remove(int startIndex, int length)
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

        /// <summary>
        /// Removes the character at the specified index from this instance.
        /// </summary>
        /// <param name="index">The zero-based position in this instance of the character to remove.</param>
        /// <returns>A reference to this instance after the excise operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or
        /// greater than the length of this instance.</exception>
        /// <remarks>
        /// The current method removes the specified character from the current instance. The characters at
        /// (<paramref name="index"/> + 1) are moved to <paramref name="index"/>, and
        /// the string value of the current instance is shortened by 1. The capacity of the
        /// current instance is unaffected.
        /// </remarks>
        public MutableTextBuffer RemoveAt(int index) // Coverage for the JDK (deleteCharAt)
        {
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);

            int currentLength = Length;
            if ((uint)index > (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index, ExceptionArgument.index);
            }

            RemoveCore(index, 1, zeroBeyondPosition: true);

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
        public MutableTextBuffer Append(bool value) => Append(value, format: BooleanFormat.Lowercase);

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
        public MutableTextBuffer Append(bool value, BooleanFormat format)
        {
            string text = FormatBoolean(value, format);
            Append(ref MemoryMarshal.GetReference(text.AsSpan()), text.Length);
            return this;
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char"/>
        public MutableTextBuffer Append(char value)
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

        /// <summary>
        /// Appends the string representation of a specified 8-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="sbyte"/>
        [CLSCompliant(false)]
        public MutableTextBuffer Append(sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<sbyte, SByteFormatter>(3, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified 8-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="byte"/>
        public MutableTextBuffer Append(byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<byte, ByteFormatter>(4, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified 16-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="short"/>
        public MutableTextBuffer Append(short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<short, Int16Formatter>(4, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified 32-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="int"/>
        public MutableTextBuffer Append(int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<int, Int32Formatter>(6, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified 64-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="long"/>
        public MutableTextBuffer Append(long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<long, Int64Formatter>(10, value, format.AsSpan(), provider);
#endif
        /// <summary>
        /// Appends the string representation of a specified single-precision floating-point number to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture using the "J" format, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="float"/>
        public MutableTextBuffer Append(float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
            => AppendNumberCore<float, SingleFormatter>(6, value, format.AsSpan(), provider);

        /// <summary>
        /// Appends the string representation of a specified double-precision floating-point number to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture using the "J" format, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="double"/>
        public MutableTextBuffer Append(double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
            => AppendNumberCore<double, DoubleFormatter>(14, value, format.AsSpan(), provider);

        /// <summary>
        /// Appends the string representation of a specified decimal to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="decimal"/>
        // J2N TODO: Since BigDecimal in Java doesn't use the same default format as this, we will need to change the default before this can be made public
        internal MutableTextBuffer Append(decimal value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => Append(value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo));
#endif

        /// <summary>
        /// Appends the string representation of a specified 16-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="ushort"/>
        [CLSCompliant(false)]
        public MutableTextBuffer Append(ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<ushort, UInt16Formatter>(4, value, format.AsSpan(), provider);
#endif
        /// <summary>
        /// Appends the string representation of a specified 32-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="uint"/>
        [CLSCompliant(false)]
        public MutableTextBuffer Append(uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<uint, UInt32Formatter>(6, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Appends the string representation of a specified 64-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// This method allows similar options as the <c>AppendFormat</c> methods, but has better performance because
        /// the value being formatted is not boxed.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="ulong"/>
        [CLSCompliant(false)]
        public MutableTextBuffer Append(ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => AppendSpanFormattable(value, format, provider);
#else
            => AppendNumberCore<ulong, UInt64Formatter>(10, value, format.AsSpan(), provider);
#endif

        // J2N: Helper method for supported types so we don't need to duplicate all of this business logic
        // on every number type.

        private MutableTextBuffer AppendNumberCore<T, TFormatter>(
            int ensureAdditionalCapacityBeyondPos, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TFormatter : struct, INumberFormatter<T>
        {
            provider ??= NumberFormatInfo.InvariantInfo; // For JDK-style formatting

            if ((uint)m_Position + (uint)ensureAdditionalCapacityBeyondPos > (uint)m_Chars.Length)
            {
                // Check if the valueCount will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the MutableTextBuffer.
                int newLength = m_Position + ensureAdditionalCapacityBeyondPos;
                if (newLength > m_MaxCapacity)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                Grow(ensureAdditionalCapacityBeyondPos);
            }

            int charsWritten;
            while (!default(TFormatter).TryFormat(value, format, provider, m_Chars.AsSpan(m_Position), out charsWritten))
            {
                // Check if the valueCount will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the MutableTextBuffer.
                int newLength = m_Chars.Length * 2;
                if (newLength > m_MaxCapacity)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                // J2N: This effectively doubles the buffer
                Grow(m_Chars.Length + 1); // rare
            }

            m_Position += charsWritten;

            return this;
        }

        #endregion Append Number


        private MutableTextBuffer AppendSpanFormattable<T>(T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            where T : ISpanFormattable
#else
            where T : Number
#endif
        {
            Debug.Assert(typeof(T).Assembly.Equals(typeof(object).Assembly) || typeof(T).Assembly.Equals(typeof(Number).Assembly), "Implementation trusts the results of TryFormat because T is expected to be something known");

            provider ??= NumberFormatInfo.InvariantInfo; // For JDK-style formatting
            int charsWritten;
            while (!value.TryFormat(m_Chars.AsSpan(m_Position), out charsWritten, format, provider))
            {
                int length = m_Chars.Length;
                int additionalCapacity = length - m_Position == length ? m_Chars.Length + 1 : m_Chars.Length; // Ensure we request enough to cause a re-grow

                // Check if the valueCount will put us over m_MaxCapacity.
                // Doing the check here prevents corruption of the MutableTextBuffer.
                int newLength = m_Position + additionalCapacity;
                if (newLength > m_MaxCapacity)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                }

                Grow(additionalCapacity);
            }

            m_Position += charsWritten;

            return this;
        }

        /// <summary>
        /// Appends the string representation of a specified object to this instance using the specified format
        /// and culture-specific format information.
        /// </summary>
        /// <param name="value">The object to append.</param>
        /// <param name="format">A standard or custom format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        /// <remarks>
        /// <paramref name="format"/> and <paramref name="provider"/> are only applied if the object implements <see cref="ISpanFormattable"/>,
        /// <see cref="IFormattable"/>, <c>IStructuralFormattable</c>, or subclasses <see cref="Number"/>.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="object"/>
        public MutableTextBuffer Append(object? value, string? format = null, IFormatProvider? provider = null)
        {
            if (value is null)
                return this; // no-op
#if FEATURE_SPANFORMATTABLE
            else if (value is ISpanFormattable spanFormattable) // J2N: Check for ISpanFormattable reference types, as this will improve performance.
                return AppendSpanFormattable(spanFormattable, format, provider);
#else
            else if (value is Number number) // J2N: Check for Number-derived reference types, as this will improve performance.
                return AppendSpanFormattable(number, format.AsSpan(), provider);
#endif
            else if (value is IStructuralFormattable structuralFormattable)
                return Append(structuralFormattable.ToString(format, provider));
            else if (value is IFormattable formattable)
                return Append(formattable.ToString(format, provider));
            else if (value is ICharSequence csq)
                return Append(csq); // doesn't support format providers
            else
                return Append(value.ToString());
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified array to this instance.
        /// </summary>
        /// <param name="value">The array of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        /// <remarks>
        /// This method appends the characters in the specified array to the current instance in the same order they
        /// appear in value. If <paramref name="value"/> is <c>null</c>, no changes are made.
        /// <para/>
        /// The <see cref="Append(char[])"/> method modifies the existing instance of this class; it does not
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        /// <seealso cref="char"/>
        public MutableTextBuffer Append(char[]? value)
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

        /// <summary>
        /// Appends the string representation of a specified read-only character span to this instance.
        /// </summary>
        /// <param name="value">The read-only character span to append.</param>
        /// <returns>A reference to this instance after the append operation is completed.</returns>
        /// <seealso cref="ReadOnlySpan{Char}"/>
        public MutableTextBuffer Append(ReadOnlySpan<char> value)
        {
            Append(ref MemoryMarshal.GetReference(value), value.Length);
            return this;
        }

        /// <summary>
        /// Appends the string representation of a specified read-only character memory region to this instance.
        /// </summary>
        /// <param name="value">The read-only character memory region to append.</param>
        /// <returns>A reference to this instance after the append operation is completed.</returns>
        /// <seealso cref="ReadOnlyMemory{Char}"/>
        public MutableTextBuffer Append(ReadOnlyMemory<char> value) => Append(value.Span);

        // J2N TODO: API - String interpolation for J2N formatters

        ///// <summary>Appends the specified interpolated string to this instance.</summary>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //public MutableTextBuffer Append([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler) => this;

        ///// <summary>Appends the specified interpolated string to this instance.</summary>
        ///// <param name="provider">An object that supplies culture-specific formatting information.</param>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //public MutableTextBuffer Append(IFormatProvider? provider, [InterpolatedStringHandlerArgument("", nameof(provider))] ref AppendInterpolatedStringHandler handler) => this;

        ///// <summary>Appends the specified interpolated string followed by the default line terminator to the end of the current MutableTextBuffer object.</summary>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //public MutableTextBuffer AppendLine([InterpolatedStringHandlerArgument("")] ref AppendInterpolatedStringHandler handler) => AppendLine();

        ///// <summary>Appends the specified interpolated string followed by the default line terminator to the end of the current MutableTextBuffer object.</summary>
        ///// <param name="provider">An object that supplies culture-specific formatting information.</param>
        ///// <param name="handler">The interpolated string to append.</param>
        ///// <returns>A reference to this instance after the append operation has completed.</returns>
        //public MutableTextBuffer AppendLine(IFormatProvider? provider, [InterpolatedStringHandlerArgument("", nameof(provider))] ref AppendInterpolatedStringHandler handler) => AppendLine();

        #region AppendJoin

        public MutableTextBuffer AppendJoin(string? separator, params object?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public MutableTextBuffer AppendJoin(string? separator, params ReadOnlySpan<object?> values)
        {
            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public MutableTextBuffer AppendJoin<T>(string? separator, IEnumerable<T> values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public MutableTextBuffer AppendJoin(string? separator, params string?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public MutableTextBuffer AppendJoin(string? separator, params ReadOnlySpan<string?> values)
        {
            separator ??= string.Empty;
            return AppendJoinCore(ref MemoryMarshal.GetReference(separator.AsSpan()), separator.Length, values);
        }

        public MutableTextBuffer AppendJoin(char separator, params object?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            return AppendJoinCore(ref separator, 1, values);
        }

        public MutableTextBuffer AppendJoin(char separator, params ReadOnlySpan<object?> values) =>
            AppendJoinCore(ref separator, 1, values);

        public MutableTextBuffer AppendJoin<T>(char separator, IEnumerable<T> values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            return AppendJoinCore(ref separator, 1, values);
        }

        public MutableTextBuffer AppendJoin(char separator, params string?[] values)
        {
            if (values is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            return AppendJoinCore(ref separator, 1, values);
        }

        public MutableTextBuffer AppendJoin(char separator, params ReadOnlySpan<string?> values) =>
            AppendJoinCore(ref separator, 1, values);

        private MutableTextBuffer AppendJoinCore<T>(ref char separator, int separatorLength, IEnumerable<T> values)
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

        private MutableTextBuffer AppendJoinCore<T>(ref char separator, int separatorLength, ReadOnlySpan<T> values)
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
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// <para/>
        /// This instance of <see cref="MutableTextBuffer"/> is not changed if <paramref name="value"/> is <c>null</c>,
        /// or <paramref name="value"/> is not <c>null</c> but its length is zero.
        /// </remarks>
        public MutableTextBuffer Insert(int index, string? value)
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
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This matches the behavior of Java's StringBuilder. To match the behavior
        /// of .NET, call <see cref="Insert(int, bool, BooleanFormat)"/> and specify <see cref="BooleanFormat.TitleCase"/>.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// </remarks>
        /// <seealso cref="bool"/>
        public MutableTextBuffer Insert(int index, bool value) => Insert(index, value, BooleanFormat.Lowercase);

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
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// </remarks>
        /// <seealso cref="bool"/>
        /// <seealso cref="BooleanFormat"/>
        public MutableTextBuffer Insert(int index, bool value, BooleanFormat format)
        {
            string text = FormatBoolean(value, format);
            // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
            // we want exceeding the maximum capacity to throw an OutOfMemoryException.
            Insert(index, text.AsSpan(), 1);
            return this;
        }

        #region Insert Number

        /// <summary>
        /// Inserts the string representation of a specified 8-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="sbyte"/>
        [CLSCompliant(false)]
        public MutableTextBuffer Insert(int index, sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format.AsSpan(), provider);
#else
            => InsertNumberCore<sbyte, SByteFormatter>(index, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified 8-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="byte"/>
        public MutableTextBuffer Insert(int index, byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<byte, ByteFormatter>(index, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified 16-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="short"/>
        public MutableTextBuffer Insert(int index, short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<short, Int16Formatter>(index, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified 32-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="int"/>
        public MutableTextBuffer Insert(int index, int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<int, Int32Formatter>(index, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified 64-bit signed integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="long"/>
        public MutableTextBuffer Insert(int index, long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<long, Int64Formatter>(index, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified single-precision floating-point number to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture using the "J" format, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="float"/>
        public MutableTextBuffer Insert(int index, float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
            => InsertNumberCore<float, SingleFormatter>(index, value, format.AsSpan(), provider);

        /// <summary>
        /// Inserts the string representation of a specified double-precision floating-point number to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture using the "J" format, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="double"/>
        public MutableTextBuffer Insert(int index, double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
            => InsertNumberCore<double, DoubleFormatter>(index, value, format.AsSpan(), provider);

        /// <summary>
        /// Inserts the string representation of a specified decimal to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="decimal"/>
        // J2N TODO: Since BigDecimal in Java doesn't use the same default format as this, we will need to change the default before this can be made public
        internal MutableTextBuffer Insert(int index, decimal value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => Insert(index, value.ToString(format, provider ?? NumberFormatInfo.InvariantInfo), 1);
#endif

        /// <summary>
        /// Inserts the string representation of a specified 16-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="ushort"/>
        [CLSCompliant(false)]
        public MutableTextBuffer Insert(int index, ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<ushort, UInt16Formatter>(index, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified 32-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="uint"/>
        [CLSCompliant(false)]
        public MutableTextBuffer Insert(int index, uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<uint, UInt32Formatter>(index, value, format.AsSpan(), provider);
#endif

        /// <summary>
        /// Inserts the string representation of a specified 64-bit unsigned integer to this instance
        /// with the specified numeric format and culture-specific format information.
        /// <para/>
        /// Unless otherwise specified, formatting is performed in the invariant culture, which
        /// is similar to how the JDK formats numbers.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to format and append.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The current length of this <see cref="MutableTextBuffer"/> object plus the length of
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="ulong"/>
        [CLSCompliant(false)]
        public MutableTextBuffer Insert(int index, ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            => InsertSpanFormattable(index, value, format, provider);
#else
            => InsertNumberCore<ulong, UInt64Formatter>(index, value, format.AsSpan(), provider);
#endif

        // J2N: Helper method for supported types so we don't need to duplicate all of this business logic
        // on every number type.
        private MutableTextBuffer InsertNumberCore<T, TFormatter>(
            int index, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            where TFormatter : struct, INumberFormatter<T>
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            provider ??= NumberFormatInfo.InvariantInfo; // For JDK-style formatting
            char[]? arrayToReturnToPool = null;
            Span<char> buffer = stackalloc char[CharStackBufferSize];
            int charsWritten = 0;
            try
            {
                while (!default(TFormatter).TryFormat(value, format, provider, buffer, out charsWritten))
                {
                    // Check if the valueCount will put us over m_MaxCapacity.
                    // Doing the check here prevents corruption of the MutableTextBuffer.
                    int newLength = buffer.Length * 2;
                    if (newLength > m_MaxCapacity)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                    }
                    buffer = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(newLength);
                }

                // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
                // we want exceeding the maximum capacity to throw an OutOfMemoryException.
                Insert(index, buffer.Slice(0, charsWritten), 1);
            }
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }

            return this;
        }

        #endregion Insert Number

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
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="char"/>
        public MutableTextBuffer Insert(int index, char value)
        {
            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            Insert(index, ref value, 1);
            return this;
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
        /// <paramref name="value"/> exceeds <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="value"/> is <c>null</c>, the <see cref="MutableTextBuffer"/> is not changed.
        /// </remarks>
        /// <seealso cref="char"/>
        public MutableTextBuffer Insert(int index, char[]? value)
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="char"/>
        public MutableTextBuffer Insert(int index, char[]? value, int startIndex, int charCount)
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <seealso cref="char"/>
        public MutableTextBuffer Insert(int index, string? value, int startIndex, int count) // J2N: Added to cover the JDK better (rather than ICharSequence only)
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
                    return this;
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
            return this;
        }

        /// <summary>
        /// Inserts the string representation of an object into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The object to insert, or <c>null</c>.</param>
        /// <param name="format">A standard or custom format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the current length of this instance.
        /// </exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        /// <remarks>
        /// <paramref name="format"/> and <paramref name="provider"/> are only applied if the object implements <see cref="ISpanFormattable"/>,
        /// <see cref="IFormattable"/>, <c>IStructuralFormattable</c>, or subclasses <see cref="Number"/>.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity of this instance is adjusted as needed.
        /// <para/>
        /// If <paramref name="value"/> is <c>null</c>, the <see cref="MutableTextBuffer"/> is not changed.
        /// </remarks>
        /// <seealso cref="object"/>
        public MutableTextBuffer Insert(int index, object? value, string? format = null, IFormatProvider? provider = null)
        {
            if (value is null)
                return this; // no-op;
#if FEATURE_SPANFORMATTABLE
            else if (value is ISpanFormattable spanFormattable) // J2N: Check for ISpanFormattable reference types, as this will improve performance.
                return InsertSpanFormattable(index, spanFormattable, format, provider);
#else
            else if (value is Number number) // J2N: Check for Number-derived reference types, as this will improve performance.
                return InsertSpanFormattable(index, number, format.AsSpan(), provider);
#endif
            else if (value is IStructuralFormattable structuralFormattable)
                return Insert(index, structuralFormattable.ToString(format, provider), 1);
            else if (value is IFormattable formattable)
                return Insert(index, formattable.ToString(format, provider), 1);
            else if (value is ICharSequence csq)
                return Insert(index, csq); // doesn't support format providers
            else
                return Insert(index, value.ToString(), 1);
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
        public MutableTextBuffer Insert(int index, ReadOnlySpan<char> value) // J2N NOTE: Weird that upstream they made an overload of ReadOnlyMemory<char> for Append, but not Insert.
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

        private MutableTextBuffer InsertSpanFormattable<T>(int index, T value, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
#if FEATURE_SPANFORMATTABLE
            where T : ISpanFormattable
#else
            where T : Number
#endif
        {
            Debug.Assert(typeof(T).Assembly.Equals(typeof(object).Assembly) || typeof(T).Assembly.Equals(typeof(Number).Assembly), "Implementation trusts the results of TryFormat because T is expected to be something known");

            provider ??= NumberFormatInfo.InvariantInfo; // For JDK-style formatting

            if ((uint)index > (uint)Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);
            }

            char[]? arrayToReturnToPool = null;
            Span<char> buffer = stackalloc char[CharStackBufferSize];
            int charsWritten = 0;
            try
            {
                while (!value.TryFormat(buffer, out charsWritten, format, provider))
                {
                    // Check if the valueCount will put us over m_MaxCapacity.
                    // Doing the check here prevents corruption of the MutableTextBuffer.
                    int newLength = buffer.Length * 2;
                    if (newLength > m_MaxCapacity)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException(value, ExceptionArgument.valueCount, ExceptionResource.ArgumentOutOfRange_LengthGreaterThanCapacity);
                    }

                    buffer = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(newLength);
                }

                // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
                // we want exceeding the maximum capacity to throw an OutOfMemoryException.
                Insert(index, buffer.Slice(0, charsWritten), 1);
            }
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }

            return this;
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            return AppendFormat(null, format, MemoryMarshal.CreateReadOnlySpan(ref arg0, 1));
#else
            return AppendFormat(null, format, new ParamsArray(arg0));
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            TwoObjects two = new TwoObjects(arg0, arg1);
            return AppendFormat(null, format, (ReadOnlySpan<object?>)two);
#else
            return AppendFormat(null, format, new ParamsArray(arg0, arg1));
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            ThreeObjects three = new ThreeObjects(arg0, arg1, arg2);
            return AppendFormat(null, format, (ReadOnlySpan<object?>)three);
#else
            return AppendFormat(null, format, new ParamsArray(arg0, arg1, arg2));
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        {
            if (args is null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormat(..., span).
                ThrowHelper.ThrowArgumentNullException(format is null ? ExceptionArgument.format : ExceptionArgument.args);
            }

            return AppendFormat(null, format, args);
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args)
        {
            return AppendFormat(null, format, args);
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            return AppendFormat(provider, format, MemoryMarshal.CreateReadOnlySpan(ref arg0, 1));
#else
            return AppendFormat(provider, format, new ParamsArray(arg0));
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            TwoObjects two = new TwoObjects(arg0, arg1);
            return AppendFormat(provider, format, (ReadOnlySpan<object?>)two);
#else
            return AppendFormat(provider, format, new ParamsArray(arg0, arg1));
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0, object? arg1, object? arg2)
        {
#if FEATURE_INLINEARRAYATTRIBUTE
            ThreeObjects three = new ThreeObjects(arg0, arg1, arg2);
            return AppendFormat(provider, format, (ReadOnlySpan<object?>)three);
#else
            return AppendFormat(provider, format, new ParamsArray(arg0, arg1, arg2));
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
        {
            if (args is null)
            {
                // To preserve the original exception behavior, throw an exception about format if both
                // args and format are null. The actual null check for format is in AppendFormat(..., span).
                ThrowHelper.ThrowArgumentNullException(format is null ? ExceptionArgument.format : ExceptionArgument.args);
            }

            return AppendFormat(provider, format, (ReadOnlySpan<object?>)args);
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
        /// The length of the expanded string would exceed <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public MutableTextBuffer AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params ReadOnlySpan<object?> args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
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
        private MutableTextBuffer AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, ParamsArray args) // KEEP OVERLOADS FOR ReadOnlySpan<object?> and ParamsArray IN SYNC
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

        public MutableTextBuffer AppendFormat<TArg0>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(1);
            return AppendFormat(provider, format, arg0, 0, 0, default);
        }

        public MutableTextBuffer AppendFormat<TArg0, TArg1>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(2);
            return AppendFormat(provider, format, arg0, arg1, 0, default);
        }

        public MutableTextBuffer AppendFormat<TArg0, TArg1, TArg2>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            format.ValidateNumberOfArgs(3);
            return AppendFormat(provider, format, arg0, arg1, arg2, default);
        }

        public MutableTextBuffer AppendFormat(IFormatProvider? provider, CompositeFormat format, params object?[] args)
        {
            if (format is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.format);
            if (args is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.args);
            return AppendFormat(provider, format, (ReadOnlySpan<object?>)args);
        }

        public MutableTextBuffer AppendFormat(IFormatProvider? provider, CompositeFormat format, params ReadOnlySpan<object?> args)
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

        private MutableTextBuffer AppendFormat<TArg0, TArg1, TArg2>(IFormatProvider? provider, CompositeFormat format, TArg0 arg0, TArg1 arg1, TArg2 arg2, ReadOnlySpan<object?> args)
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

        /// <summary>
        /// Replaces all occurrences of a specified string in this instance with another specified string.
        /// </summary>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <c>null</c>.</param>
        /// <returns>A reference to this instance with all instances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/> in the
        /// current instance. If <paramref name="newValue"/> is <c>null</c> or <see cref="string.Empty"/>, all occurrences of
        /// <paramref name="oldValue"/> are removed.
        /// </remarks>
        /// <seealso cref="Remove(int, int)"/>
        public MutableTextBuffer Replace(string oldValue, string? newValue) => Replace(oldValue, newValue, 0, Length);

        /// <summary>
        /// Replaces all instances of one read-only character span with another in this builder.
        /// </summary>
        /// <param name="oldValue">The read-only character span to replace.</param>
        /// <param name="newValue">The read-only character span to replace <paramref name="oldValue"/> with.</param>
        /// <returns>A reference to this instance with with all instances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/> in the
        /// current instance. If <paramref name="newValue"/> is empty, all occurrences of <paramref name="oldValue"/> are removed.
        /// </remarks>
        /// <seealso cref="Remove(int, int)"/>
        public MutableTextBuffer Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue) => Replace(oldValue, newValue, 0, Length);


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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/>
        /// in the specified substring. If <paramref name="newValue"/> is <c>null</c> or <see cref="string.Empty"/>,
        /// all occurrences of <paramref name="oldValue"/> in the specified range are removed.
        /// </remarks>
        /// <seealso cref="Remove(int, int)"/>
        public MutableTextBuffer Replace(string oldValue, string? newValue, int startIndex, int count)
        {
            if (oldValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.oldValue);
            return Replace(oldValue.AsSpan(), newValue.AsSpan(), startIndex, count);
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal, case-sensitive comparison to identify occurrences of <paramref name="oldValue"/>
        /// in the specified substring. If <paramref name="newValue"/> is empty, all occurrences of <paramref name="oldValue"/>
        /// in the specified range are removed.
        /// </remarks>
        /// <seealso cref="Remove(int, int)"/>
        public MutableTextBuffer Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
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
            return this;
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
        public MutableTextBuffer Replace(char oldChar, char newChar)
        {
            return Replace(oldChar, newChar, 0, Length);
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
        public MutableTextBuffer Replace(char oldChar, char newChar, int startIndex, int count)
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

        /// <summary>
        /// Replaces the specified substring in this builder with the specified
        /// string, <paramref name="newValue"/>. The substring begins at the specified
        /// <paramref name="startIndex"/> and ends to the character at
        /// <c><paramref name="count"/> - <paramref name="startIndex"/></c> or
        /// to the end of the sequence if no such character exists. First the
        /// characters in the substring ar removed and then the specified
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
        public MutableTextBuffer Replace(int startIndex, int count, string newValue)
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

        /// <summary>
        /// Replaces the specified substring in this builder with the specified
        /// character span, <paramref name="newValue"/>. The substring begins at the specified
        /// <paramref name="startIndex"/> and ends to the character at
        /// <c><paramref name="count"/> - <paramref name="startIndex"/></c> or
        /// to the end of the sequence if no such character exists. First the
        /// characters in the substring ar removed and then the specified
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
        public MutableTextBuffer Replace(int startIndex, int count, ReadOnlySpan<char> newValue)
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="NullReferenceException"><paramref name="value"/> is a null pointer.</exception>
        /// <remarks>
        /// This method appends <paramref name="valueCount"/> characters starting at address <paramref name="value"/>
        /// to the current instance.
        /// <para/>
        /// The <see cref="Append(char*, int)"/> method modifies the existing instance of this class; it does
        /// not return a new class instance. Because of this, you can call a method or property on the existing
        /// reference and you do not have to assign the return value to an <see cref="MutableTextBuffer"/> object.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// <para/>
        /// <b>Notes to Callers</b>
        /// <para/>
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        [CLSCompliant(false)]
        public unsafe MutableTextBuffer Append(char* value, int valueCount)
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        /// <exception cref="NullReferenceException"><paramref name="value"/> is a null pointer.</exception>
        /// <remarks>
        /// This method inserts <paramref name="valueCount"/> characters starting at address <paramref name="value"/>
        /// to the current instance.
        /// <para/>
        /// Existing characters are shifted to make room for the new text. The capacity is adjusted as needed.
        /// </remarks>
        [CLSCompliant(false)]
        public unsafe MutableTextBuffer Insert(int index, char* value, int valueCount)
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
            char[] newBuffer = AllocateBuffer(newCapacity);
            oldBuffer.AsSpan(0, m_Position).CopyTo(newBuffer);
            ReleaseBuffer(oldBuffer);
            m_Chars = newBuffer;
        }

        /// <summary>
        /// Allocates a new character buffer for use by <see cref="MutableTextBuffer"/> when the
        /// existing buffer changes in size. This may happen when the buffer grows to accommodate
        /// more data or when calling <see cref="TrimExcess()"/> to shrink the buffer to fit its content.
        /// </summary>
        /// <param name="minimumLength">
        /// The minimum required length of the returned buffer. The returned array MUST have a
        /// length greater than or equal to this value.
        /// </param>
        /// <returns>
        /// A new <see cref="char"/> array that will become the active buffer for this instance.
        /// </returns>
        /// <remarks>
        /// This method is called internally whenever <see cref="MutableTextBuffer"/> needs to grow or
        /// shrink its underlying storage. Subclasses may override this method to control how new buffers
        /// are allocated. For example, buffers may be rented from <see cref="System.Buffers.ArrayPool{T}"/> or
        /// another pooling mechanism.
        /// <para/>
        /// Implementations must <em>not</em> perform any data copying. The base class is solely
        /// responsible for transferring existing content into the new buffer before it becomes active.
        /// <para/>
        /// The returned buffer should be considered newly allocated and uninitialized; the base
        /// class will overwrite the portion it requires.
        /// </remarks>
        protected virtual char[] AllocateBuffer(int minimumLength)
        {
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            return GC.AllocateUninitializedArray<char>(minimumLength); // J2N NOTE: If we decide to expose the actual array, we must use new char[] here.
#else
            return new char[minimumLength];
#endif
        }

        /// <summary>
        /// Releases a previously-used character buffer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer that is no longer used by this instance.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method is called internally after <see cref="MutableTextBuffer"/> has finished copying
        /// all required data out of the previous buffer and replaced it with a new one.
        /// Subclasses may override this method to return buffers to a pool or perform other
        /// cleanup logic.
        /// </para>
        /// <para>
        /// The default implementation does nothing.
        /// </para>
        /// <para>
        /// Implementations must assume that <paramref name="buffer"/> may contain arbitrary
        /// application data. It is the subclass's responsibility to avoid leaking sensitive
        /// information when using pooled or shared buffers.
        /// </para>
        /// </remarks>
        protected virtual void ReleaseBuffer(char[] buffer)
        {
            // By default, do nothing. Derived classes can override to return to pool, etc.
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
        /// to <c><see cref="Length"/> - <paramref name="startIndex"/>.</c>, provided it is not bounded by <see cref="MaxCapacity"/>.
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
        public MutableTextBuffer Delete(int startIndex, int count) // Coverage for the JDK
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
        /// IMPORTANT: This operation is done in-place. Although an <see cref="MutableTextBuffer"/>
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
        /// don't require an <see cref="MutableTextBuffer"/> instance.
        /// </summary>
        /// <returns>A reference to this <see cref="MutableTextBuffer"/>, for chaining.</returns>
        /// <seealso cref="StringExtensions.ReverseText(string)"/>
        /// <seealso cref="MemoryExtensions.ReverseText(Span{char})"/>
        /// <seealso cref="StringBuilderExtensions.Reverse(StringBuilder)"/>
        public MutableTextBuffer Reverse() // Coverage for the JDK
        {
            m_Chars.AsSpan(0, m_Position).ReverseText();
            return this;
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
        /// <paramref name="length"/> plus the current length of this instance exceeds <see cref="MaxCapacity"/>.
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
        /// When you instantiate an <see cref="MutableTextBuffer"/> object by calling the <see cref="MutableTextBuffer(int, int)"/>
        /// constructor, both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MaxCapacity"/> property. This can occur particularly when you call the <see cref="Append(string)"/>
        /// and <see cref="AppendFormat(string, object)"/> methods to append small strings.
        /// </remarks>
        public Span<char> AppendSpan(int length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);

            int pos = m_Position;
            if (pos > m_Chars.Length - length)
            {
                Grow(length);
            }
            Span<char> buffer = m_Chars.AsSpan(pos, length);
            buffer.Fill('\0'); // Ensure the buffer doesn't contain any sensitive data before providing it to the user
            m_Position += length;
            return buffer;
        }
    }
}
