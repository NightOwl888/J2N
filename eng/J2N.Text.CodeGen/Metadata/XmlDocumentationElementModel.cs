namespace J2N.Text.CodeGen.Metadata
{
    public sealed class XmlDocumentationElementModel
    {
        public required string ElementName { get; init; }

        public Dictionary<string, string> Attributes { get; } = [];

        public string? InnerXml { get; set; }
    }
}
