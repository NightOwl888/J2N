using System;

namespace J2N.Text
{
    public sealed class SimpleCharSequence : ICharSequence
    {
        private readonly ReadOnlyMemory<char> memory;

        public SimpleCharSequence(ReadOnlyMemory<char> memory)
        {
            this.memory = memory;
        }

        public bool HasValue => true;

        public int Length => memory.Length;

        public char this[int index] => memory.Span[index];

        public ICharSequence Subsequence(int startIndex, int length)
            => new CharArrayCharSequence(memory.ToArray());

        public override string ToString() => memory.Span.ToString();
    }
}
