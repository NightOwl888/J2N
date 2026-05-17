namespace J2N.Text.CodeGen.Metadata
{
    public sealed class TypeModel
    {
        public required string Namespace { get; init; }

        public required string Name { get; init; }

        public required List<MethodModel> Methods { get; init; }
    }
}
