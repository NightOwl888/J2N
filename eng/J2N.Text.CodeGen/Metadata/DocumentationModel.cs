namespace J2N.Text.CodeGen.Metadata
{
    public sealed class DocumentationModel
    {
        public string? SummaryXml { get; init; }

        public string? RemarksXml { get; init; }

        public string? ReturnsXml { get; init; }

        public string? SynchronizationNoteXml { get; set; }
    }
}