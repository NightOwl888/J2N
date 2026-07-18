using J2N.Buffers;
using J2N.CodeGeneration;
using J2N.Collections;
using J2N.Numerics;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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
    internal partial class MutableTextBuffer : IBufferWriter<char>,
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
        /// The default capacity of a <see cref="MutableTextBuffer"/>.
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

        /// <summary>
        /// Gets the underlying storage of the builder.
        /// </summary>
        /// <remarks>
        /// This property does not clear the underlying storage, but returns the raw unfiltered bytes
        /// in writable form.
        /// </remarks>
        [CodeGenerationIgnore]
        public Span<char> RawChars => m_Chars;

        #region Buffer Capacity

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
        /// When you instantiate a <see cref="MutableTextBuffer"/> object by calling <see cref="MutableTextBuffer.Initialize(int, int)"/>,
        /// both the length and the capacity of the <see cref="MutableTextBuffer"/> instance can grow beyond
        /// the value of its <see cref="MutableTextBuffer.MaxCapacity"/> property. This can occur particularly when you call the <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, string?)"/>
        /// and <see cref="MutableTextBufferExtensions.AppendFormat{TBuilder}(TBuilder, string, object?)"/> methods to append small strings.
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
        /// Sets the capacity of a <see cref="MutableTextBuffer"/> object to the actual number of characters
        /// it contains.
        /// </summary>
        /// <remarks>
        /// This method is similar to <c>trimToSize()</c> in the JDK.
        /// <para/>
        /// You can use the <see cref="TrimExcess()"/> method to minimize a <see cref="MutableTextBuffer"/> object's
        /// memory overhead once it is known that no new characters will be added. To completely clear an
        /// <see cref="MutableTextBuffer"/> object and release all memory referenced by it, call this method
        /// after calling the <see cref="MutableTextBufferExtensions.Clear{TBuilder}(TBuilder)"/> method or setting <see cref="Length"/> property to 0.
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

        #endregion Buffer Capacity

        #region Buffer Length

        /// <summary>
        /// Removes all characters from the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Clear{TBuilder}(TBuilder)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void ClearInternal()
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                    AppendInternal('\0', delta);
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

        #endregion Buffer Length

        #region this[index]

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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)m_Position)
                {
                    ThrowHelper.ThrowIndexOutOfRangeException();
                }

                return m_Chars[index];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if ((uint)index >= (uint)m_Position)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index, ExceptionArgument.index);
                }
                m_Chars[index] = value;
            }
        }

        #endregion this[index]

        #region AppendLine

        /// <summary>
        /// Appends the default line terminator to the end of the current instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendLine{TBuilder}(TBuilder)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendLineInternal() => AppendInternal(Environment.NewLine);

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendLine{TBuilder}(TBuilder, string?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendLineInternal(string? value)
        {
            AppendInternal(value);
            AppendInternal(Environment.NewLine);
        }

        /// <summary>
        /// Appends a copy of the specified span followed by the default line terminator to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.AppendLine{TBuilder}(TBuilder, ReadOnlySpan{char})"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendLineInternal(ReadOnlySpan<char> value)
        {
            AppendInternal(value);
            AppendInternal(Environment.NewLine);
        }

        #endregion AppendLine

        #region CopyTo

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
        /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
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

        #endregion CopyTo

        #region Append/Insert bool

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, bool)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(bool value) => AppendInternal(value, format: BooleanFormat.Lowercase);

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Append{TBuilder}(TBuilder, bool, BooleanFormat)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void AppendInternal(bool value, BooleanFormat format)
        {
            string text = FormatBoolean(value, format);
            Append(ref MemoryMarshal.GetReference(text.AsSpan()), text.Length);
        }

        /// <summary>
        /// Inserts the string representation of a specified Boolean value to this instance
        /// at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, bool)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, bool value) => InsertInternal(index, value, BooleanFormat.Lowercase);

        /// <summary>
        /// Inserts the string representation of a specified Boolean value to this instance
        /// at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, bool, BooleanFormat)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertInternal(int index, bool value, BooleanFormat format)
        {
            string text = FormatBoolean(value, format);
            // We don't use Insert(int, ReadOnlySpan<char>) for exception compatibility;
            // we want exceeding the maximum capacity to throw an OutOfMemoryException.
            InsertInternal(index, text.AsSpan(), 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatBoolean(bool value, BooleanFormat format) =>
            // J2N: System.Boolean ignores the IFormatProvider that is passed to it,
            // so we are using a boolean flag for users to be able to specify whether to use
            // title casing (.NET) or lower casing (Java).
            format == BooleanFormat.Lowercase ? StringFormatter.FormatBoolean(value) : value.ToString();

        #endregion Append/Insert bool

        #region Append/Insert object

        /// <summary>
        /// Appends the string representation of a specified object to this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="J2N.MutableTextBufferExtensions.Append{TBuilder}(TBuilder, object?, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
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
                AppendInternal(structuralFormattable.ToString(format, provider));
            else if (value is IFormattable formattable)
                AppendInternal(formattable.ToString(format, provider));
            else if (value is ICharSequence csq)
                AppendInternal(csq); // doesn't support format providers
            else
                AppendInternal(value.ToString());
        }

        /// <summary>
        /// Inserts the string representation of a specified object into this instance
        /// at the specified character position.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="J2N.MutableTextBufferExtensions.Insert{TBuilder}(TBuilder, int, object?, string?, IFormatProvider?)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
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
                InsertInternal(index, structuralFormattable.ToString(format, provider), 1);
            else if (value is IFormattable formattable)
                InsertInternal(index, formattable.ToString(format, provider), 1);
            else if (value is ICharSequence csq)
                InsertInternal(index, csq); // doesn't support format providers
            else
                InsertInternal(index, value.ToString(), 1);
        }

        #endregion Append/Insert object

        #region InsertFromSelf

#if FEATURE_INDEX_RANGE
        /// <summary>
        /// Inserts a copy of the specified range from this buffer at the specified index.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.InsertFromSelf{TBuilder}(TBuilder, int, Range)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void InsertFromSelfInternal(int index, Range range)
        {
            var (startIndex, count) = range.GetOffsetAndLength(Length);
            InsertFromSelfInternal(index, startIndex, count);
        }
#endif

        /// <summary>
        /// Inserts a copy of the specified range of characters from this buffer at the specified index.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.InsertFromSelf{TBuilder}(TBuilder, int, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void InsertFromSelfInternal(int index, int startIndex, int count)
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

        #endregion InsertFromSelf

        #region Reverse

        /// <summary>
        /// Causes this character sequence to be replaced by the reverse of the sequence.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Reverse{TBuilder}(TBuilder)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void ReverseInternal() // Coverage for the JDK
        {
            m_Chars.AsSpan(0, m_Position).ReverseText();
        }

        #endregion Reverse

        #region Equals

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="sb">An object to compare with this instance, or <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the characters in this instance and <paramref name="sb"/> are the same;
        /// otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The current instance and <paramref name="sb"/> are equal if the strings assigned to both
        /// <see cref="MutableTextBuffer"/> objects are the same. To determine equality, the
        /// <see cref="Equals(MutableTextBuffer)"/> method uses ordinal comparison.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <param name="sb">An object to compare with this instance, or <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the characters in this instance and <paramref name="sb"/> are the same;
        /// otherwise, <see langword="false"/>.</returns>
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
        /// <returns><see langword="true"/> if the characters in this instance and <paramref name="span"/> are the same;
        /// otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <see cref="Equals(MutableTextBuffer)"/> method performs an ordinal comparison to determine
        /// whether the characters in the current instance and span are equal.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ReadOnlySpan<char> span)
        {
            if (span.Length != Length)
            {
                return false;
            }

            return new ReadOnlySpan<char>(m_Chars, 0, m_Position).SequenceEqual(span);
        }

        #endregion

        #region ToString

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

        #endregion ToString

        #region Dispose

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
        /// <see langword="true"/> to release managed resources; otherwise, <see langword="false"/>.
        /// </param>
        /// <remarks>
        /// Derived classes overriding this method should release any managed state
        /// when <paramref name="disposing"/> is <see langword="true"/>, and then call the base
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

        #endregion Dispose

        #region Operator Overrides

        /// <summary>
        /// Defines an implicit conversion of a given <see cref="MutableTextBuffer"/> to a read-only span of characters.
        /// </summary>
        /// <param name="value">A <see cref="MutableTextBuffer"/> to implicitly convert.</param>
        [CodeGenerationIgnore]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlySpan<char>(MutableTextBuffer? value) =>
            value != null ? value.AsSpan() : default;

        #endregion Operator Overrides

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
        /// Inserts a character buffer into this builder at the specified position.
        /// </summary>
        /// <param name="index">The index to insert in this builder.</param>
        /// <param name="value">The reference to the start of the buffer.</param>
        /// <param name="valueCount">The number of characters in the buffer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Insert(int index, ref char value, int valueCount)
        {
            Debug.Assert((uint)index <= (uint)Length, "Callers should check that index is a legal value.");

            if (valueCount > 0)
            {
                MakeRoom(index, valueCount);
                ReplaceInPlace(ref index, ref value, valueCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        /// Creates a gap at a logical index with the specified count,
        /// allocating new buffer if necessary.
        /// </summary>
        /// <param name="index">The logical index in this builder.</param>
        /// <param name="count">The number of characters in the gap.</param>
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

        #region ISpannable<char> Members

        ReadOnlySpan<char> ISpannable<char>.AsSpan() => this.AsSpan();

        ReadOnlySpan<char> ISpannable<char>.AsSpan(int start) => this.AsSpan(start);

        ReadOnlySpan<char> ISpannable<char>.AsSpan(int start, int length) => this.AsSpan(start, length);

        #endregion ISpannable<char> Members
    }
}
