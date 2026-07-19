using J2N.Buffers;
using System;
using System.Text;
#nullable enable

namespace J2N.Text.Tests
{
    public partial class MutableTextBuffer_Tests : StringBuilder_Tests
    {
        private protected override MutableTextBuffer MutableTextBufferFactory()
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize();

        private protected override MutableTextBuffer MutableTextBufferFactory(int capacity)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(capacity);

        private protected override MutableTextBuffer MutableTextBufferFactory(int capacity, int maxCapacity)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(capacity, maxCapacity);

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value);

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int startIndex, int length, int capacity)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value, startIndex, length, capacity);

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int capacity)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value, capacity);

        private protected override MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value);

        private protected override MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value, int capacity)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value, capacity);

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value);

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int capacity)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value, capacity);

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int startIndex, int length, int capacity)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value, startIndex, length, capacity);

        private protected override MutableTextBuffer MutableTextBufferFactory(ICharSequence? value)
            => new MutableTextBuffer(UninitializedArrayAllocator<char>.Default).Initialize(value);

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int capacity, IArrayAllocator<char> allocator)
            => new MutableTextBuffer(allocator).Initialize(value, capacity);
    }
}
