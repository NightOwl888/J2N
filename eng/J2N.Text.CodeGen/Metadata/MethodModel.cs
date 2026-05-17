namespace J2N.Text.CodeGen.Metadata
{
    public sealed class MethodModel
    {
        public required string Name { get; init; }

        public required string ReturnType { get; init; }

        public required bool ReturnsSelf { get; init; }

        public bool IsBuilderMethod { get; init; } = true;

        public List<ParameterModel> Parameters { get; init; } = new();
    }
}
