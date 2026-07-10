namespace J2N.Text.CodeGen.Metadata
{
    public sealed class DocumentationModel
    {
        public List<XmlDocumentationElementModel> Elements { get; } = [];

        public string? SummaryXml { get; set; }

        public string? RemarksXml { get; set; }

        public string? ReturnsXml { get; set; }

        public string? ValueXml { get; set; }

        public string? ExampleXml { get; set; }

        public List<XmlDocumentationElementModel> Exceptions { get; init; } = [];

        public List<XmlDocumentationElementModel> Permissions { get; init; } = [];

        public List<XmlDocumentationElementModel> SeeAlsos { get; init; } = [];

        public List<XmlDocumentationElementModel> Sees { get; init; } = [];

        public List<XmlDocumentationElementModel> TypeParameters { get; init; } = [];

        public string? SynchronizationNoteXml { get; set; }

        public DocumentationModel Clone()
        {
            DocumentationModel result = new()
            {
                SummaryXml = SummaryXml,
                RemarksXml = RemarksXml,
                ReturnsXml = ReturnsXml,
                ValueXml = ValueXml,
                ExampleXml = ExampleXml,
                SynchronizationNoteXml = SynchronizationNoteXml,
            };

            result.Elements.AddRange(
                Elements.Select(CloneElement));

            result.Exceptions.AddRange(
                Exceptions.Select(CloneElement));

            result.Permissions.AddRange(
                Permissions.Select(CloneElement));

            result.SeeAlsos.AddRange(
                SeeAlsos.Select(CloneElement));

            result.Sees.AddRange(
                Sees.Select(CloneElement));

            result.TypeParameters.AddRange(
                TypeParameters.Select(CloneElement));

            return result;
        }

        private static XmlDocumentationElementModel CloneElement(
            XmlDocumentationElementModel element)
        {
            XmlDocumentationElementModel clone = new()
            {
                ElementName = element.ElementName,
                InnerXml = element.InnerXml,
            };

            foreach ((string key, string value) in element.Attributes)
            {
                clone.Attributes.Add(key, value);
            }

            return clone;
        }
    }
}