using System;
using System.Text;
#nullable enable

namespace J2N.Text.Tests
{
    public partial class MutableTextBuffer_Tests : StringBuilder_Tests
    {
        protected override MutableTextBuffer MutableTextBufferFactory()
            => new MutableTextBuffer();

        protected override MutableTextBuffer MutableTextBufferFactory(int capacity)
            => new MutableTextBuffer(capacity);

        protected override MutableTextBuffer MutableTextBufferFactory(int capacity, int maxCapacity)
            => new MutableTextBuffer(capacity, maxCapacity);

        protected override MutableTextBuffer MutableTextBufferFactory(string? value)
            => new MutableTextBuffer(value);

        protected override MutableTextBuffer MutableTextBufferFactory(string? value, int startIndex, int length, int capacity)
            => new MutableTextBuffer(value, startIndex, length, capacity);

        protected override MutableTextBuffer MutableTextBufferFactory(string? value, int capacity)
            => new MutableTextBuffer(value, capacity);

        protected override MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value)
            => new MutableTextBuffer(value);

        protected override MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value, int capacity)
            => new MutableTextBuffer(value, capacity);

        protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value)
            => new MutableTextBuffer(value);

        protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int capacity)
            => new MutableTextBuffer(value, capacity);

        protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int startIndex, int length, int capacity)
            => new MutableTextBuffer(value, startIndex, length, capacity);

        protected override MutableTextBuffer MutableTextBufferFactory(ICharSequence? value)
            => new MutableTextBuffer(value);
    }
}
