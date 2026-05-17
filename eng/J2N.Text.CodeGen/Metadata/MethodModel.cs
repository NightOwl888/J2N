namespace J2N.Text.CodeGen.Metadata
{
    public sealed class MethodModel
    {
        public required string Name { get; init; }

        public required string ReturnType { get; init; }

        public required List<ParameterModel> Parameters { get; init; }

        public bool ReturnsSelf { get; init; }
    }
}
