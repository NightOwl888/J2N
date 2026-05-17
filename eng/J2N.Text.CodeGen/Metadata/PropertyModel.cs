namespace J2N.Text.CodeGen.Metadata
{
    public sealed class PropertyModel
    {
        public required string Name { get; init; }

        public required string TypeName { get; init; }

        public bool HasGetter { get; init; }

        public bool HasSetter { get; init; }

        public bool IsIndexer { get; init; }

        public List<ParameterModel> IndexParameters { get; init; } = new();
    }
}
