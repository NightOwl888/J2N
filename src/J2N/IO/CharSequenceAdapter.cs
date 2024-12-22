#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

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

        public override sealed CharBuffer Get(char[] destination, int offset, int length) // J2N TODO: API - rename startIndex instead of offset
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > destination.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
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

        public override sealed CharBuffer Put(char[] source, int offset, int length) // J2N TODO: API - rename startIndex instead of offset
        {
            if (source is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > source.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferOverflowException();

            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Put(string source, int startIndex, int length)
        {
            if (source is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > source.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Slice()
        {
            return new CharSequenceAdapter(sequence.Subsequence(position, limit - position)); // J2N: Corrected 2nd parameter
        }

        public override CharBuffer Subsequence(int startIndex, int length)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > Remaining - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            CharSequenceAdapter result = Copy(this);
            result.position = position + startIndex;
            result.limit = position + startIndex + length;
            return result;
        }
    }
}
