using J2N.Text.CodeGen.Metadata;

namespace J2N.Text.CodeGen.Projection
{
    public sealed class ProjectedTypeModel
    {
        public required TypeModel Source { get; init; }

        public required string Namespace { get; init; }
        public required string Name { get; init; }

        public List<MethodModel> Methods { get; init; } = new();
        public List<PropertyModel> Properties { get; init; } = new();
    }
}