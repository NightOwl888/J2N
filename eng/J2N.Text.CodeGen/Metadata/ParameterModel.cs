namespace J2N.Text.CodeGen.Metadata
{
    public sealed class ParameterModel
    {
        public required string Name { get; init; }

        public required string TypeName { get; init; }

        public string? SourceTypeName { get; init; }

        public string? Documentation { get; init; }

        public bool IsThis { get; init; }

        public string? Modifier { get; init; }
    }
}
