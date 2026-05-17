namespace J2N.Text.CodeGen.Projection
{
    public sealed class ProjectionProfile
    {
        // Identity of the projection (e.g. "TextBuilder", "OpenStringBuilder")
        public required string Name { get; init; }

        // Members to exclude by name
        public HashSet<string> ExcludedMembers { get; init; } = new();

        // If true, generates fluent builder returns
        public bool EnableBuilderPattern { get; init; } = true;

        // If false, strips XML docs (later use)
        public bool IncludeXmlDocs { get; init; } = true;
    }
}
