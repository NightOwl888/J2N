namespace J2N.Text.CodeGen.Metadata
{
    public sealed class MethodModel
    {
        public required string Name { get; init; }

        public required string ReturnType { get; init; }

        public bool ReturnsSelf { get; init; }

        public bool IsBuilderMethod { get; init; }

        public bool IsStatic { get; init; }

        public bool IsExtensionMethod { get; init; }

        public bool IsConstructorProjection { get; set; }

        public bool IsUnsafe { get; init; }

        public string? BodyText { get; init; }

        public DocumentationModel? Documentation { get; init; }

        public List<ParameterModel> Parameters { get; init; } = [];

        public List<GenericParameterModel> GenericParameters { get; set; } = [];

        public List<AttributeModel> Attributes { get; init; } = [];

        public string? ConditionalCompilationSymbol { get; set; }

        public bool Ignore { get; init; }

        public bool SkipSynchronization { get; init; }
    }
}