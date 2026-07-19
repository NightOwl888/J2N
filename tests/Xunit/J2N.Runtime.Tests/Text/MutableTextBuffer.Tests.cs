using J2N.Buffers;
using System;
using System.Text;
#nullable enable

namespace J2N.Text.Tests
{
    public partial class MutableTextBuffer_Tests : StringBuilder_Tests
    {
        public static readonly IArrayAllocator<char> DefaultAllocator =
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            UninitializedArrayAllocator<char>.Default;
#else
            ArrayAllocator<char>.Default;
#endif

        private protected override MutableTextBuffer MutableTextBufferFactory(MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize();
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(int capacity, int maxCapacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(capacity, maxCapacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int startIndex, int length, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, startIndex, length, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int startIndex, int length, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, startIndex, length, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(ICharSequence? value, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int capacity, IArrayAllocator<char> allocator, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(allocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, capacity);
        }

    }
}
