namespace J2N.Text.CodeGen.Metadata
{
    public sealed class GenericParameterModel
    {
        public required string Name { get; init; }

        public List<string> Constraints { get; init; } = [];
    }
}