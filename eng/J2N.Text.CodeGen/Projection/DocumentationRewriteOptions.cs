using J2N.Text.CodeGen.Metadata;

namespace J2N.Text.CodeGen.Projection
{
    internal sealed class DocumentationRewriteOptions
    {
        public bool IncludeSynchronizationNote { get; init; }

        public bool ForceBuilderReturns { get; init; }

        /// <summary>
        /// Determines whether generic &lt;typeparam/&gt; elements are preserved in
        /// the rewritten documentation.
        /// </summary>
        public bool PreserveTypeParameterDocumentation { get; init; } = true;

        /// <summary>
        /// Inserts the synthetic extension-method "this" parameter before all
        /// existing <param/> elements.
        /// </summary>
        public XmlDocumentationElementModel? AdditionalThisParameter { get; init; }

        public XmlDocumentationElementModel? AdditionalTypeParameter { get; init; }

        public Func<string, string>? RewriteCref { get; init; }
    }
}
