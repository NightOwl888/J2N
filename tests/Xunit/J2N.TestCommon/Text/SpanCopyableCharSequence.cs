using System;

namespace J2N.Text
{
    public sealed class SpanCopyableCharSequence : ICharSequence, ISpanCopyable<char>
    {
        private readonly ReadOnlyMemory<char> memory;

        public SpanCopyableCharSequence(ReadOnlyMemory<char> memory)
        {
            this.memory = memory;
        }

        public bool HasValue => true;

        public int Length => memory.Length;

        public char this[int index] => memory.Span[index];

        public void CopyTo(int sourceIndex, Span<char> destination, int count)
        {
            memory.Span.Slice(sourceIndex, count)
                .CopyTo(destination);
        }

        public ICharSequence Subsequence(int startIndex, int length)
            => new CharArrayCharSequence(memory.ToArray());

        public override string ToString() => memory.Span.ToString();
    }
}
