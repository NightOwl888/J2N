namespace J2N.Text.CodeGen.Metadata
{
    public sealed class MethodModel
    {
        public required string Name { get; init; }

        public required string ReturnType { get; init; }

        public bool ReturnsSelf { get; init; }

        public bool IsBuilderMethod { get; init; }

        public bool IsUnsafe { get; init; }

        public DocumentationModel? Documentation { get; init; }

        public List<ParameterModel> Parameters { get; init; } = [];

        public List<AttributeModel> Attributes { get; init; } = [];
    }
}