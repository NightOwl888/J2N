namespace J2N.Text.CodeGen.Metadata
{
    public sealed class TypeModel
    {
        public List<string> Usings { get; set; } = [];

        public string Namespace { get; set; } = "";
        public string Name { get; set; } = "";

        public string SourceType { get; set; } = "";

        public bool IsSealed { get; set; }

        public List<MethodModel> Methods { get; set; } = [];
        public List<PropertyModel> Properties { get; set; } = [];
    }
}
