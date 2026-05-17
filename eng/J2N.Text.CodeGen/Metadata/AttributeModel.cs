namespace J2N.Text.CodeGen.Metadata
{
    public sealed class AttributeModel
    {
        public required string Name { get; init; }

        public List<string> Arguments { get; init; } = [];
    }
}