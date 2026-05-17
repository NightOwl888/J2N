using System;
using System.Text;
#nullable enable

namespace J2N.Text.Tests
{
    public partial class OpenStringBuilder_Tests : StringBuilder_Tests
    {
        protected override MutableTextBuffer OpenStringBuilderFactory()
            => new MutableTextBuffer();

        protected override MutableTextBuffer OpenStringBuilderFactory(int capacity)
            => new MutableTextBuffer(capacity);

        protected override MutableTextBuffer OpenStringBuilderFactory(int capacity, int maxCapacity)
            => new MutableTextBuffer(capacity, maxCapacity);

        protected override MutableTextBuffer OpenStringBuilderFactory(string? value)
            => new MutableTextBuffer(value);

        protected override MutableTextBuffer OpenStringBuilderFactory(string? value, int startIndex, int length, int capacity)
            => new MutableTextBuffer(value, startIndex, length, capacity);

        protected override MutableTextBuffer OpenStringBuilderFactory(string? value, int capacity)
            => new MutableTextBuffer(value, capacity);

        protected override MutableTextBuffer OpenStringBuilderFactory(ReadOnlySpan<char> value)
            => new MutableTextBuffer(value);

        protected override MutableTextBuffer OpenStringBuilderFactory(ReadOnlySpan<char> value, int capacity)
            => new MutableTextBuffer(value, capacity);

        protected override MutableTextBuffer OpenStringBuilderFactory(StringBuilder? value)
            => new MutableTextBuffer(value);

        protected override MutableTextBuffer OpenStringBuilderFactory(StringBuilder? value, int capacity)
            => new MutableTextBuffer(value, capacity);

        protected override MutableTextBuffer OpenStringBuilderFactory(StringBuilder? value, int startIndex, int length, int capacity)
            => new MutableTextBuffer(value, startIndex, length, capacity);

        protected override MutableTextBuffer OpenStringBuilderFactory(ICharSequence? value)
            => new MutableTextBuffer(value);
    }
}
