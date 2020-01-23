using J2N.Text;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace J2N.IO
{
    /// <summary>
    /// A buffer of chars.
    /// </summary>
    /// <remarks>
    /// A char buffer can be created in either one of the following ways:
    /// <list type="bullet">
    ///     <item><description><see cref="Allocate(int)"/> a new char array and create a buffer based on it;</description></item>
    ///     <item><description><see cref="Wrap(char[])"/> an existing <see cref="T:char[]"/> to create a new buffer;</description></item>
    ///     <item><description><see cref="Wrap(string)"/> an existing <see cref="string"/> to create a new buffer;</description></item>
    ///     <item><description><see cref="Wrap(StringBuilder)"/> an existing <see cref="StringBuilder"/> to create a new buffer;</description></item>
    ///     <item><description><see cref="Wrap(ICharSequence)"/> an existing char sequence to create a new buffer;</description></item>
    ///     <item><description>Use <see cref="ByteBuffer.AsCharBuffer()"/> to create a char buffer based on a byte buffer.</description></item>
    /// </list>
    /// </remarks>
    public abstract class CharBuffer : Buffer, IComparable<CharBuffer>, ICharSequence, IAppendable
    {
        /// <summary>
        /// Creates a char buffer based on a newly allocated char array.
        /// </summary>
        /// <param name="capacity">The capacity of the new buffer.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="capacity"/> is less than 0.</exception>
        public static CharBuffer Allocate(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            return new ReadWriteCharArrayBuffer(capacity);
        }

        /// <summary>
        /// Creates a new char buffer by wrapping the given char array.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Wrap(array, 0, array.Length)</c>.
        /// </summary>
        /// <param name="array">The char array which the new buffer will be based on.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static CharBuffer Wrap(char[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            return Wrap(array, 0, array.Length);
        }

        /// <summary>
        /// Creates a new char buffer by wrapping the given char array.
        /// <para/>
        /// The new buffer's position will be <paramref name="startIndex"/>, limit will be
        /// <c>start + length</c>, capacity will be the length of the array.
        /// </summary>
        /// <param name="array">The char array which the new buffer will be based on.</param>
        /// <param name="startIndex">The start index, must not be negative and not greater than
        /// <c>array.Length</c>.</param>
        /// <param name="length">The length, must not be negative and not greater than
        /// <c>array.Length - <paramref name="startIndex"/></c>.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static CharBuffer Wrap(char[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int len = array.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)startIndex + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(startIndex)} + {nameof(length)} > {nameof(array.Length)}");

            return new ReadWriteCharArrayBuffer(array)
            {
                position = startIndex,
                limit = startIndex + length
            };
        }

        // *** Added for .NET Compatibility ***

        /// <summary>
        /// Creates a new char buffer by wrapping the given <see cref="string"/>.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Wrap(characterSequence, 0, characterSequence.Length)</c>.
        /// </summary>
        /// <param name="characterSequence">The <see cref="string"/> which the new buffer will be based on.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="characterSequence"/> is <c>null</c>.</exception>
        public static CharBuffer Wrap(string characterSequence)
        {
            if (characterSequence == null)
                throw new ArgumentNullException(nameof(characterSequence));

            return new CharSequenceAdapter(characterSequence.AsCharSequence()); // J2N TODO: Create StringAdapter?
        }

        /// <summary>
        /// Creates a new char buffer by wrapping the given <see cref="string"/>, <paramref name="characterSequence"/>.
        /// <para/>
        /// The new buffer's position will be <paramref name="startIndex"/>, limit will be
        /// <paramref name="length"/>, capacity will be the length of the <see cref="string"/>. The new
        /// buffer is read-only.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the second parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="length"/>.
        /// </summary>
        /// <param name="characterSequence">The <see cref="string"/> which the new buffer will be based on.</param>
        /// <param name="startIndex">The start index, must not be negative and not greater than <c>characterSequence.Length</c>.</param>
        /// <param name="length">The end index, must be no less than <paramref name="startIndex"/> and no
        /// greater than <c>characterSequence.Length</c>.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="characterSequence"/> is <c>null</c>.</exception>
        public static CharBuffer Wrap(string characterSequence, int startIndex, int length)
        {
            if (characterSequence == null)
                throw new ArgumentNullException(nameof(characterSequence));

            int len = characterSequence.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)startIndex + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(startIndex)} + {nameof(length)} > {nameof(characterSequence.Length)}");

            return new CharSequenceAdapter(characterSequence.AsCharSequence()) // J2N TODO: Create StringAdapter?
            {
                position = startIndex,
                limit = startIndex + length
            };
        }

        /// <summary>
        /// Creates a new char buffer by wrapping the given <see cref="StringBuilder"/>.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Wrap(characterSequence, 0, characterSequence.Length)</c>.
        /// </summary>
        /// <param name="characterSequence">The <see cref="StringBuilder"/> which the new buffer will be based on.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="characterSequence"/> is <c>null</c>.</exception>
        public static CharBuffer Wrap(StringBuilder characterSequence)
        {
            if (characterSequence == null)
                throw new ArgumentNullException(nameof(characterSequence));

            return new CharSequenceAdapter(characterSequence.AsCharSequence()); // J2N TODO: Create StringBuilderAdapter?
        }

        /// <summary>
        /// Creates a new char buffer by wrapping the given <see cref="StringBuilder"/>, <paramref name="characterSequence"/>.
        /// <para/>
        /// The new buffer's position will be <paramref name="startIndex"/>, limit will be
        /// <paramref name="length"/>, capacity will be the length of the <see cref="StringBuilder"/>. The new
        /// buffer is read-only.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the second parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="length"/>.
        /// </summary>
        /// <param name="characterSequence">The <see cref="StringBuilder"/> which the new buffer will be based on.</param>
        /// <param name="startIndex">The start index, must not be negative and not greater than <c>characterSequence.Length</c>.</param>
        /// <param name="length">The end index, must be no less than <paramref name="startIndex"/> and no
        /// greater than <c>characterSequence.Length</c>.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="characterSequence"/> is <c>null</c>.</exception>
        public static CharBuffer Wrap(StringBuilder characterSequence, int startIndex, int length)
        {
            if (characterSequence == null)
                throw new ArgumentNullException(nameof(characterSequence));

            int len = characterSequence.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)startIndex + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(startIndex)} + {nameof(length)} > {nameof(characterSequence.Length)}");

            return new CharSequenceAdapter(characterSequence.AsCharSequence()) // J2N TODO: Create StringAdapter?
            {
                position = startIndex,
                limit = startIndex + length
            };
        }

        // ** End Added for .NET Compatibility ***

        /// <summary>
        /// Creates a new char buffer by wrapping the given char sequence.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Wrap(characterSequence, 0, characterSequence.Length)</c>.
        /// </summary>
        /// <param name="characterSequence">The char sequence which the new buffer will be based on.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="characterSequence"/> is <c>null</c>.</exception>
        public static CharBuffer Wrap(ICharSequence characterSequence)
        {
            if (characterSequence == null)
                throw new ArgumentNullException(nameof(characterSequence));

            return new CharSequenceAdapter(characterSequence);
        }

        /// <summary>
        /// Creates a new char buffer by wrapping the given char sequence, <paramref name="characterSequence"/>.
        /// <para/>
        /// The new buffer's position will be <paramref name="startIndex"/>, limit will be
        /// <paramref name="length"/>, capacity will be the length of the char sequence. The new
        /// buffer is read-only.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the second parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="length"/>.
        /// </summary>
        /// <param name="characterSequence">The char sequence which the new buffer will be based on.</param>
        /// <param name="startIndex">The start index, must not be negative and not greater than <c>characterSequence.Length</c>.</param>
        /// <param name="length">The end index, must be no less than <paramref name="startIndex"/> and no
        /// greater than <c>characterSequence.Length</c>.</param>
        /// <returns>The created char buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="characterSequence"/> is <c>null</c>.</exception>
        public static CharBuffer Wrap(ICharSequence characterSequence, int startIndex, int length)
        {
            if (characterSequence == null)
                throw new ArgumentNullException(nameof(characterSequence));

            int len = characterSequence.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)startIndex + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(startIndex)} + {nameof(length)} > {nameof(characterSequence.Length)}");

            return new CharSequenceAdapter(characterSequence)
            {
                position = startIndex,
                limit = startIndex + length
            };
        }

        /// <summary>
        /// Constructs a <see cref="CharBuffer"/> with given capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the buffer.</param>
        internal CharBuffer(int capacity)
            : base(capacity)
        { }

        /// <summary>
        /// Gets the char array which this buffer is based on, if there is one.
        /// </summary>
        /// <exception cref="ReadOnlyBufferException">If this buffer is based on an array, but it is read-only.</exception>
        /// <exception cref="NotSupportedException">If this buffer is not based on an array.</exception>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        public char[] Array => ProtectedArray;

        /// <summary>
        /// Gets the offset of the char array which this buffer is based on, if
        /// there is one.
        /// <para/>
        /// The offset is the index of the array corresponds to the zero position of
        /// the buffer.
        /// </summary>
        /// <exception cref="ReadOnlyBufferException">If this buffer is based on an array but it is read-only.</exception>
        /// <exception cref="NotSupportedException">If this buffer is not based on an array.</exception>
        public int ArrayOffset => ProtectedArrayOffset;

        /// <summary>
        /// Returns a read-only buffer that shares its content with this buffer.
        /// <para/>
        /// The returned buffer is guaranteed to be a new instance, even if this
        /// buffer is read-only itself. The new buffer's position, limit, capacity
        /// and mark are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means this
        /// buffer's change of content will be visible to the new buffer. The two
        /// buffer's position, limit and mark are independent.
        /// </summary>
        /// <returns>A read-only version of this buffer.</returns>
        public abstract CharBuffer AsReadOnlyBuffer();

        /// <summary>
        /// Returns the character located at the specified <paramref name="index"/> in the buffer. The
        /// <paramref name="index"/> value is referenced from the current buffer position.
        /// </summary>
        /// <param name="index">The index referenced from the current buffer position. It must
        /// not be less than zero but less than the value obtained from a
        /// call to <see cref="Buffer.Remaining"/>.</param>
        /// <returns>The character located at the specified <paramref name="index"/> (referenced from the
        /// current position) in the buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is invalid.</exception>
        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= Remaining)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return Get(position + index);
            }
        }

        /// <summary>
        /// Compacts this char buffer.
        /// <para/>
        /// The remaining chars will be moved to the head of the buffer,
        /// starting from position zero. Then the position is set to
        /// <see cref="Buffer.Remaining"/>; the limit is set to capacity; the mark is cleared.
        /// </summary>
        /// <returns>This buffer.</returns>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract CharBuffer Compact();

        /// <summary>
        /// Compare the remaining chars of this buffer to another char
        /// buffer's remaining chars.
        /// </summary>
        /// <param name="other">Another char buffer.</param>
        /// <returns>A negative value if this is less than <paramref name="other"/>; 0 if
        /// this equals to <paramref name="other"/>; a positive valie if this is
        /// greater than <paramref name="other"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <c>null</c>.</exception>
        public virtual int CompareTo(CharBuffer other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            int compareRemaining = (Remaining < other.Remaining) ? Remaining
                    : other.Remaining;
            int thisPos = position;
            int otherPos = other.position;
            char thisByte, otherByte;
            while (compareRemaining > 0)
            {
                thisByte = Get(thisPos);
                otherByte = other.Get(otherPos);
                if (thisByte != otherByte)
                {
                    // NOTE: IN .NET comparison should return
                    // the diff, not be hard coded to 1/-1
                    return thisByte - otherByte;
                    //return thisByte < otherByte ? -1 : 1;
                }
                thisPos++;
                otherPos++;
                compareRemaining--;
            }
            return Remaining - other.Remaining;
        }

        /// <summary>
        /// Returns a duplicated buffer that shares its content with this buffer.
        /// <para/>
        /// The duplicated buffer's initial position, limit, capacity and mark are
        /// the same as this buffer's. The duplicated buffer's read-only property and
        /// byte order are the same as this buffer's, too.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A duplicated buffer that shares its content with this buffer.</returns>
        public abstract CharBuffer Duplicate();

        /// <summary>
        /// Checks whether this char buffer is equal to another object.
        /// <para/>
        /// If <paramref name="other"/> is not a char buffer then <c>false</c> is returned. Two
        /// char buffers are equal if and only if their remaining chars are exactly
        /// the same. Position, limit, capacity and mark are not considered.
        /// </summary>
        /// <param name="other">The object to compare with this char buffer.</param>
        /// <returns><c>true</c> if this char buffer is equal to <paramref name="other"/>, <c>false</c> otherwise.</returns>
        public override bool Equals(object other)
        {
            if (!(other is CharBuffer))
            {
                return false;
            }
            CharBuffer otherBuffer = (CharBuffer)other;

            if (Remaining != otherBuffer.Remaining)
            {
                return false;
            }

            int myPosition = position;
            int otherPosition = otherBuffer.position;
            bool equalSoFar = true;
            while (equalSoFar && (myPosition < limit))
            {
                equalSoFar = Get(myPosition++) == otherBuffer.Get(otherPosition++);
            }

            return equalSoFar;
        }

        /// <summary>
        /// Returns the char at the current position and increases the position by 1.
        /// </summary>
        /// <returns>The char at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is equal or greater than limit.</exception>
        public abstract char Get();

        /// <summary>
        /// Reads chars from the current position into the specified char array and
        /// increases the position by the number of chars read.
        /// <para/>
        /// Calling this method has the same effect as <c>Get(destination, 0, destination.Length)</c>.
        /// </summary>
        /// <param name="destination">The destination char array.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferUnderflowException">If <c>destination.Length</c> is greater than <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> is <c>null</c>.</exception>
        public virtual CharBuffer Get(char[] destination)
        {
            if (destination == null)
                throw new ArgumentOutOfRangeException(nameof(destination));

            return Get(destination, 0, destination.Length);
        }

        /// <summary>
        /// Reads chars from the current position into the specified char array,
        /// starting from the specified offset, and increases the position by the
        /// number of chars read.
        /// </summary>
        /// <param name="destination">The target char array.</param>
        /// <param name="offset">The offset of the char array, must not be negative and not
        /// greater than <c>destination.Length</c>.</param>
        /// <param name="length">The number of chars to read, must be no less than zero and no
        /// greater than <c>destination.Length - offset</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="offset"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="BufferUnderflowException">If <paramref name="length"/> is greater than <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> is <c>null</c>.</exception>
        public virtual CharBuffer Get(char[] destination, int offset, int length)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            int len = destination.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)offset + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(offset)} + {nameof(length)} > {nameof(destination.Length)}");

            if (length > Remaining)
            {
                throw new BufferUnderflowException();
            }
            for (int i = offset; i < offset + length; i++)
            {
                destination[i] = Get();
            }
            return this;
        }

        /// <summary>
        /// Returns a char at the specified <paramref name="index"/>; the position is not changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and less than limit.</param>
        /// <returns>A char at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract char Get(int index);

        /// <summary>
        /// Indicates whether this buffer is based on a char array and is read/write.
        /// <para/>
        /// Returns <c>true</c> if this buffer is based on a byte array and provides
        /// read/write access, <c>false</c> otherwise.
        /// </summary>
        public bool HasArray => ProtectedHasArray;

        /// <summary>
        /// Gets a value indicating whether the current <see cref="ICharSequence"/>
        /// has a valid value of its underlying type.
        /// </summary>
        bool ICharSequence.HasValue => true;

        /// <summary>
        /// Calculates this buffer's hash code from the remaining chars. The
        /// position, limit, capacity and mark don't affect the hash code.
        /// </summary>
        /// <returns>The hash code calculated from the remaining chars.</returns>
        public override int GetHashCode()
        {
            int myPosition = position;
            int hash = 0;
            while (myPosition < limit)
            {
                hash += Get(myPosition++);
            }
            return hash;
        }

        ///// <summary>
        ///// Indicates whether this buffer is direct. A direct buffer will try its
        ///// best to take advantage of native memory APIs and it may not stay in the
        ///// heap, so it is not affected by garbage collection.
        ///// <para/>
        ///// A char buffer is direct if it is based on a byte buffer and the byte
        ///// buffer is direct.
        ///// </summary>
        //public abstract bool IsDirect { get; }

        /// <summary>
        /// Gets the number of remaining chars.
        /// </summary>
        public int Length => Remaining;

        /// <summary>
        /// Gets the byte order used by this buffer when converting chars from/to
        /// bytes.
        /// <para/>
        /// If this buffer is not based on a byte buffer, then this always returns
        /// the platform's native byte order.
        /// </summary>
        public abstract ByteOrder Order { get; }

        /// <summary>
        /// Child class implements this method to realize <see cref="Array"/>.
        /// </summary>
        /// <seealso cref="Array"/>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        protected abstract char[] ProtectedArray { get; }

        /// <summary>
        /// Child class implements this method to realize <see cref="ArrayOffset"/>.
        /// </summary>
        /// <seealso cref="ArrayOffset"/>
        protected abstract int ProtectedArrayOffset { get; }

        /// <summary>
        /// Child class implements this method to realize <see cref="HasArray"/>.
        /// </summary>
        /// <seealso cref="HasArray"/>
        protected abstract bool ProtectedHasArray { get; }

        /// <summary>
        /// Writes the given char to the current position and increases the position
        /// by 1.
        /// </summary>
        /// <param name="c">The char to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is equal or greater than limit.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract CharBuffer Put(char c);

        /// <summary>
        /// Writes chars from the given char array <paramref name="source"/> to the current position and
        /// increases the position by the number of chars written.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Put(source, 0, source.Length)</c>.
        /// </summary>
        /// <param name="source">The source char array.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <c>source.Length</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public CharBuffer Put(char[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Put(source, 0, source.Length);
        }

        /// <summary>
        /// Writes chars from the given char array <paramref name="source"/>, starting from the specified <paramref name="offset"/>,
        /// to the current position and increases the position by the number of chars
        /// written.
        /// </summary>
        /// <param name="source">The source char array.</param>
        /// <param name="offset">The offset of char array, must not be negative and not greater
        /// than <c>source.Length</c>.</param>
        /// <param name="length">The number of chars to write, must be no less than zero and no
        /// greater than <c>source.Length - offset</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <paramref name="length"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="offset"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public virtual CharBuffer Put(char[] source, int offset, int length)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int len = source.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)offset + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(offset)} + {nameof(length)} > {nameof(source.Length)}");

            if (length > Remaining)
            {
                throw new BufferOverflowException();
            }
            for (int i = offset; i < offset + length; i++)
            {
                Put(source[i]);
            }
            return this;
        }

        /// <summary>
        /// Writes all the remaining chars of the <paramref name="source"/> char buffer to this
        /// buffer's current position, and increases both buffers' position by the
        /// number of chars copied.
        /// </summary>
        /// <param name="source">The source char buffer.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <c>src.Remaining</c> is greater than this buffer's <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="source"/> is this buffer.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        public virtual CharBuffer Put(CharBuffer source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source == this)
                throw new ArgumentException();
            if (source.Remaining > Remaining)
                throw new BufferOverflowException();
            if (IsReadOnly)
                throw new ReadOnlyBufferException(); // J2N: Harmony has a bug - it shouldn't read the source and change its position unless this buffer is writable

            char[] contents = new char[source.Remaining];
            source.Get(contents);
            Put(contents);
            return this;
        }

        /// <summary>
        /// Writes a char to the specified index of this buffer; the position is not
        /// changed.
        /// </summary>
        /// <param name="index">The index, must be no less than zero and less than the limit.</param>
        /// <param name="value">The char to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract CharBuffer Put(int index, char value);

        /// <summary>
        /// Writes all chars of the given string to the current position of this
        /// buffer, and increases the position by the length of string.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Put(source, 0, source.Length)</c>.
        /// </summary>
        /// <param name="source">The string to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than the length of string.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public CharBuffer Put(string source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Put(source, 0, source.Length);
        }

        /// <summary>
        /// Writes chars of the given string to the current position of this buffer,
        /// and increases the position by the number of chars written.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="length"/>.
        /// </summary>
        /// <param name="source">The string to write.</param>
        /// <param name="startIndex">The first char to write, must not be negative and not greater
        /// than <c>str.Length</c>.</param>
        /// <param name="length">The last char to write (excluding), must be less than
        /// <paramref name="startIndex"/> and not greater than <c>source.Length</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <c>end - start</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public virtual CharBuffer Put(string source, int startIndex, int length)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int len = source.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)startIndex + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(startIndex)} + {nameof(length)} > {nameof(source.Length)}");
            if (length > Remaining)
                throw new BufferOverflowException();

            for (int i = startIndex; i < startIndex + length; i++)
            {
                Put(source[i]);
            }
            return this;
        }

        /// <summary>
        /// Returns a sliced buffer that shares its content with this buffer.
        /// <para/>
        /// The sliced buffer's capacity will be this buffer's <see cref="Buffer.Remaining"/>,
        /// and its zero position will correspond to this buffer's current position.
        /// The new buffer's position will be 0, limit will be its capacity, and its
        /// mark is cleared. The new buffer's read-only property and byte order are
        /// same as this buffer.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A sliced buffer that shares its content with this buffer.</returns>
        public abstract CharBuffer Slice();

        /// <summary>
        /// Returns a new char buffer representing a sub-sequence of this buffer's
        /// current remaining content.
        /// <para/>
        /// The new buffer's position will be <c>Position + <paramref name="startIndex"/></c>, limit will
        /// be <c>Position + <paramref name="startIndex"/> + <paramref name="length"/></c>, capacity will be the same as this buffer.
        /// The new buffer's read-only property and byte order are the same as this
        /// buffer.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the second parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="length"/>.
        /// </summary>
        /// <param name="startIndex">
        /// The start index of the sub-sequence, referenced from the
        /// current buffer position. Must not be less than zero and not
        /// greater than the value obtained from a call to <see cref="Buffer.Remaining"/>.
        /// </param>
        /// <param name="length">
        /// The length of the sub-sequence including <paramref name="startIndex"/>.
        /// Must not be less than zero and <paramref name="startIndex"/> + <paramref name="length"/> 
        /// must not be greater than the value obtained from a call to
        /// <see cref="Buffer.Remaining"/>.
        /// </param>
        /// <returns>A new char buffer represents a sub-sequence of this buffer's
        /// current remaining content.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public abstract CharBuffer Subsequence(int startIndex, int length);

        ICharSequence ICharSequence.Subsequence(int startIndex, int length) => Subsequence(startIndex, length);

        /// <summary>
        /// Returns a string representing the current remaining chars of this buffer.
        /// </summary>
        /// <returns>A string representing the current remaining chars of this buffer.</returns>
        public override string ToString()
        {
            StringBuilder strbuf = new StringBuilder();
            for (int i = position; i < limit; i++)
            {
                strbuf.Append(Get(i));
            }
            return strbuf.ToString();
        }

        /// <summary>
        /// Writes the given char <paramref name="value"/> to the current position and increases the position
        /// by 1.
        /// </summary>
        /// <param name="value">The char to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is equal or greater than limit.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(char value)
        {
            return Put(value);
        }

        /// <summary>
        /// Writes all chars of the given character sequence <paramref name="value"/> to the
        /// current position of this buffer, and increases the position by the length
        /// of the <paramref name="value"/>.
        /// <para/>
        /// Calling this method has the same effect as <c>Append(value)</c>.
        /// If the <see cref="T:char[]"/> is <c>null</c> the string "null" will be
        /// written to the buffer.
        /// </summary>
        /// <param name="value">The <see cref="T:char[]"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than the length of <paramref name="value"/>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(char[] value)
        {
            if (value != null)
            {
                return Put(value);
            }
            return Put("null"); //$NON-NLS-1$
        }

        /// <summary>
        /// Writes chars of the given <see cref="T:char[]"/> to the current position of
        /// this buffer, and increases the position by the number of chars written.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:char[]"/> to write.</param>
        /// <param name="startIndex">The first char to write, must not be negative and not greater
        /// than <c>csq.Length</c>.</param>
        /// <param name="count">The last char to write (excluding), must be less than
        /// <paramref name="startIndex"/> and not greater than <c>value.Length</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <paramref name="count"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(char[] value, int startIndex, int count)
        {
            ICharSequence cs;
            if (value == null)
                cs = "null".Subsequence(startIndex, count); //$NON-NLS-1$
            else
                cs = value.Subsequence(startIndex, count);
            if (cs.Length > 0)
            {
                return Put(cs.ToString());
            }
            return this;
        }

        /// <summary>
        /// Writes all chars of the given character sequence <paramref name="value"/> to the
        /// current position of this buffer, and increases the position by the length
        /// of the <paramref name="value"/>.
        /// <para/>
        /// Calling this method has the same effect as <c>Append(value.ToString())</c>.
        /// If the <see cref="StringBuilder"/> is <c>null</c> the string "null" will be
        /// written to the buffer.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than the length of <paramref name="value"/>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(StringBuilder value)
        {
            if (value != null)
            {
                return Put(value.ToString());
            }
            return Put("null"); //$NON-NLS-1$
        }

        /// <summary>
        /// Writes chars of the given <see cref="StringBuilder"/> to the current position of
        /// this buffer, and increases the position by the number of chars written.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> to write.</param>
        /// <param name="startIndex">The first char to write, must not be negative and not greater
        /// than <c>csq.Length</c>.</param>
        /// <param name="count">The last char to write (excluding), must be less than
        /// <paramref name="startIndex"/> and not greater than <c>value.Length</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <paramref name="count"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(StringBuilder value, int startIndex, int count)
        {
            string cs;
            if (value == null)
                cs = "null".Substring(startIndex, count);
            else
                cs = value.ToString(startIndex, count);

            if (cs.Length > 0)
            {
                return Put(cs);
            }
            return this;
        }

        /// <summary>
        /// Writes all chars of the given character sequence <paramref name="value"/> to the
        /// current position of this buffer, and increases the position by the length
        /// of the <paramref name="value"/>.
        /// <para/>
        /// Calling this method has the same effect as <c>Append(value)</c>.
        /// If the <see cref="string"/> is <c>null</c> the string "null" will be
        /// written to the buffer.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than the length of <paramref name="value"/>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(string value)
        {
            if (value != null)
            {
                return Put(value);
            }
            return Put("null"); //$NON-NLS-1$
        }

        /// <summary>
        /// Writes chars of the given <see cref="string"/> to the current position of
        /// this buffer, and increases the position by the number of chars written.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to write.</param>
        /// <param name="startIndex">The first char to write, must not be negative and not greater
        /// than <c>csq.Length</c>.</param>
        /// <param name="count">The last char to write (excluding), must be less than
        /// <paramref name="startIndex"/> and not greater than <c>value.Length</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <paramref name="count"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(string value, int startIndex, int count)
        {
            if (value == null)
                value = "null";

            string cs = value.Substring(startIndex, count);
            if (cs.Length > 0)
            {
                return Put(cs);
            }
            return this;
        }

        /// <summary>
        /// Writes all chars of the given character sequence <paramref name="value"/> to the
        /// current position of this buffer, and increases the position by the length
        /// of the <paramref name="value"/>.
        /// <para/>
        /// Calling this method has the same effect as <c>Append(value.ToString())</c>.
        /// If the <see cref="ICharSequence"/> is <c>null</c> the string "null" will be
        /// written to the buffer.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than the length of <paramref name="value"/>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(ICharSequence value)
        {
            if (value != null)
            {
                return Put(value.ToString());
            }
            return Put("null"); //$NON-NLS-1$
        }

        /// <summary>
        /// Writes chars of the given <see cref="ICharSequence"/> to the current position of
        /// this buffer, and increases the position by the number of chars written.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> to write.</param>
        /// <param name="startIndex">The first char to write, must not be negative and not greater
        /// than <c>csq.Length</c>.</param>
        /// <param name="count">The last char to write (excluding), must be less than
        /// <paramref name="startIndex"/> and not greater than <c>value.Length</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <paramref name="count"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="count"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public virtual CharBuffer Append(ICharSequence value, int startIndex, int count)
        {
            if (value == null)
            {
                value = "null".AsCharSequence(); //$NON-NLS-1$
            }
            ICharSequence cs = value.Subsequence(startIndex, count);
            if (cs.Length > 0)
            {
                return Put(cs.ToString());
            }
            return this;
        }

        /// <summary>
        /// Reads characters from this buffer and puts them into <paramref name="target"/>. The
        /// number of chars that are copied is either the number of remaining chars
        /// in this buffer or the number of remaining chars in <paramref name="target"/>,
        /// whichever is smaller.
        /// </summary>
        /// <param name="target">The target char buffer.</param>
        /// <returns>The number of chars copied or -1 if there are no chars left to be
        /// read from this buffer.</returns>
        /// <exception cref="ArgumentException">If <paramref name="target"/> is this buffer.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of <paramref name="target"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="target"/> is <c>null</c>.</exception>
        public virtual int Read(CharBuffer target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            int remaining = Remaining;
            if (target == this)
            {
                if (remaining == 0)
                {
                    return -1;
                }
                throw new ArgumentException();
            }
            if (remaining == 0)
            {
                return limit > 0 && target.Remaining == 0 ? 0 : -1;
            }
            remaining = Math.Min(target.Remaining, remaining);
            if (remaining > 0)
            {
                char[] chars = new char[remaining];
                Get(chars);
                target.Put(chars);
            }
            return remaining;
        }


        #region IAppendable Members

        IAppendable IAppendable.Append(char value) => Append(value);

        IAppendable IAppendable.Append(string value) => Append(value);

        IAppendable IAppendable.Append(string value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(StringBuilder value) => Append(value);

        IAppendable IAppendable.Append(StringBuilder value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(char[] value) => Append(value);

        IAppendable IAppendable.Append(char[] value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(ICharSequence value) => Append(value);

        IAppendable IAppendable.Append(ICharSequence value, int startIndex, int count) => Append(value, startIndex, count);

        #endregion
    }
}
