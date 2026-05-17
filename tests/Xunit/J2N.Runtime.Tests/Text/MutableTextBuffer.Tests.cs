using System;
using System.Text;
#nullable enable

namespace J2N.Text.Tests
{
    public partial class OpenStringBuilder_Tests : Abstract_OpenStringBuilder_Tests
    {
        protected override OpenStringBuilder OpenStringBuilderFactory()
            => new OpenStringBuilder();

        protected override OpenStringBuilder OpenStringBuilderFactory(int capacity)
            => new OpenStringBuilder(capacity);

        protected override OpenStringBuilder OpenStringBuilderFactory(int capacity, int maxCapacity)
            => new OpenStringBuilder(capacity, maxCapacity);

        protected override OpenStringBuilder OpenStringBuilderFactory(string? value)
            => new OpenStringBuilder(value);

        protected override OpenStringBuilder OpenStringBuilderFactory(string? value, int startIndex, int length, int capacity)
            => new OpenStringBuilder(value, startIndex, length, capacity);

        protected override OpenStringBuilder OpenStringBuilderFactory(string? value, int capacity)
            => new OpenStringBuilder(value, capacity);

        protected override OpenStringBuilder OpenStringBuilderFactory(ReadOnlySpan<char> value)
            => new OpenStringBuilder(value);

        protected override OpenStringBuilder OpenStringBuilderFactory(ReadOnlySpan<char> value, int capacity)
            => new OpenStringBuilder(value, capacity);

        protected override OpenStringBuilder OpenStringBuilderFactory(StringBuilder? value)
            => new OpenStringBuilder(value);

        protected override OpenStringBuilder OpenStringBuilderFactory(StringBuilder? value, int capacity)
            => new OpenStringBuilder(value, capacity);

        protected override OpenStringBuilder OpenStringBuilderFactory(StringBuilder? value, int startIndex, int length, int capacity)
            => new OpenStringBuilder(value, startIndex, length, capacity);

        protected override OpenStringBuilder OpenStringBuilderFactory(ICharSequence? value)
            => new OpenStringBuilder(value);
    }
}
