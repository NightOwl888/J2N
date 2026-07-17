using System;

namespace J2N.Text
{
    public sealed class SpannableCharSequence : ICharSequence, ISpannable<char>
    {
        private readonly ReadOnlyMemory<char> memory;

        public SpannableCharSequence(ReadOnlyMemory<char> memory)
        {
            this.memory = memory;
        }

        public bool HasValue => true;

        public int Length => memory.Length;

        public char this[int index] => memory.Span[index];

        public ReadOnlySpan<char> AsSpan() => memory.Span;

        public ReadOnlySpan<char> AsSpan(int start) => memory.Span.Slice(start);

        public ReadOnlySpan<char> AsSpan(int start, int length)
            => memory.Span.Slice(start, length);

        public ICharSequence Subsequence(int startIndex, int length)
            => new CharArrayCharSequence(memory.ToArray());

        public override string ToString() => memory.Span.ToString();
    }
}
