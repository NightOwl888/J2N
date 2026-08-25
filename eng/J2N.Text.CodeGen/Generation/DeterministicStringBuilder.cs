namespace J2N.Text.CodeGen.Generation
{
    /// <summary>
    /// A StringBuilder wrapper that always uses LF line endings in the <see cref="AppendLine()"/>
    /// and <see cref="AppendLine(string?)"/> overloads.
    /// </summary>
    /// <remarks>
    /// This StringBuilder is for generating deterministic source text, not for producing platform-native text.
    /// </remarks>
    public class DeterministicStringBuilder
    {
        private const char NewLine = '\n';
        private readonly System.Text.StringBuilder _sb;

        public DeterministicStringBuilder()
        {
            _sb = new System.Text.StringBuilder();
        }

        public DeterministicStringBuilder(string? value)
        {
            _sb = new System.Text.StringBuilder(value);
        }

        public DeterministicStringBuilder Append(string? value) { _sb.Append(value); return this; }
        public DeterministicStringBuilder Append(ReadOnlySpan<char> value) { _sb.Append(value); return this; }
        public DeterministicStringBuilder Append(char value) { _sb.Append(value); return this; }

        // Force LF regardless of OS environment
        public DeterministicStringBuilder AppendLine(string? value)
        {
            _sb.Append(value).Append(NewLine);
            return this;
        }

        public DeterministicStringBuilder AppendLine()
        {
            _sb.Append(NewLine);
            return this;
        }

        public DeterministicStringBuilder Clear()
        {
            _sb.Clear();
            return this;
        }

        public int Length => _sb.Length;
        public override string ToString() => _sb.ToString();
    }
}
