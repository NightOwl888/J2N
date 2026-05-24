namespace J2N.Text.CodeGen.Metadata
{
    public sealed class PropertyModel
    {
        public required string Name { get; init; }

        public required string TypeName { get; init; }

        public bool HasGetter { get; init; }

        public bool HasSetter { get; init; }

        public bool IsIndexer { get; init; }

        public bool IsUnsafe { get; init; }

        public bool IsStatic { get; init; }

        public DocumentationModel? Documentation { get; init; }

        public List<ParameterModel> IndexParameters { get; init; } = [];

        public List<AttributeModel> Attributes { get; init; } = [];

        public bool Ignore { get; init; }

        public bool SkipGetterSynchronization { get; init; }

        public bool SkipSetterSynchronization { get; init; }

        public string? GetterSynchronizationNoteXml { get; init; }

        public string? SetterSynchronizationNoteXml { get; init; }
    }
}