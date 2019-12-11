using J2N.Text;
using System;
using System.Diagnostics.CodeAnalysis;

namespace J2N.IO
{
    /// <summary>
    /// This class wraps a char sequence to be a <see cref="char"/> buffer.
    /// <para/>
    /// Implementation notice:
    /// <list type="bullet">
    ///     <item><description>Char sequence based buffer is always readonly.</description></item>
    /// </list>
    /// </summary>
    internal sealed class CharSequenceAdapter : CharBuffer
    {
        internal static CharSequenceAdapter Copy(CharSequenceAdapter other)
        {
            return new CharSequenceAdapter(other.sequence)
            {
                limit = other.limit,
                position = other.position,
                mark = other.mark
            };
        }

        internal readonly ICharSequence sequence;

        internal CharSequenceAdapter(ICharSequence chseq)
            : base(chseq.Length)
        {
            sequence = chseq;
        }

        public override CharBuffer AsReadOnlyBuffer() => Duplicate();

        public override CharBuffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Duplicate() => Copy(this);

        public override char Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return sequence[position++];
        }

        public override char Get(int index)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return sequence[index];
        }

        public override sealed CharBuffer Get(char[] destination, int offset, int length)
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
                throw new BufferUnderflowException();

            int newPosition = position + length;
            sequence.ToString().CopyTo(position, destination, offset, length);
            position = newPosition;
            return this;
        }

        //public override bool IsDirect => false;

        public override bool IsReadOnly => true;

        public override ByteOrder Order => ByteOrder.NativeOrder;

        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        protected override char[] ProtectedArray
        {
            get { throw new NotSupportedException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new NotSupportedException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override CharBuffer Put(char value)
        {
            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Put(int index, char value)
        {
            throw new ReadOnlyBufferException();
        }

        public override sealed CharBuffer Put(char[] source, int offset, int length)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)offset + (long)length > source.Length)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(offset)} + {nameof(length)} > {nameof(source.Length)}");
            if (length > Remaining)
                throw new BufferOverflowException();

            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Put(string source, int startIndex, int length)
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

            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Slice()
        {
            return new CharSequenceAdapter(sequence.Subsequence(position, limit - position)); // J2N: Corrected 2nd parameter
        }

        public override CharBuffer Subsequence(int startIndex, int length)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > Remaining)
                throw new ArgumentOutOfRangeException("", $"{nameof(startIndex)} + {nameof(length)} > {nameof(Remaining)}");

            CharSequenceAdapter result = Copy(this);
            result.position = position + startIndex;
            result.limit = position + startIndex + length;
            return result;
        }
    }
}
