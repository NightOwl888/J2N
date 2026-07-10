using J2N.Text.CodeGen.Metadata;
using System.Text.RegularExpressions;

namespace J2N.Text.CodeGen.Projection
{
    internal static class DocumentationRewriter
    {
        private static readonly Dictionary<string, int> DocumentationElementOrder = new(StringComparer.Ordinal)
        {
            ["summary"] = 0,

            ["typeparam"] = 10,
            ["param"] = 20,

            ["returns"] = 30,
            ["value"] = 40,

            ["exception"] = 50,
            ["permission"] = 60,

            ["remarks"] = 70,
            ["example"] = 80,

            ["seealso"] = 90,
            ["see"] = 100,
        };

        public static void SortDocumentationElements(
            DocumentationModel documentation)
        {
            List<XmlDocumentationElementModel> ordered =
                documentation.Elements
                    .OrderBy(e =>
                        DocumentationElementOrder.TryGetValue(
                            e.ElementName,
                            out int order)
                                ? order
                                : int.MaxValue)
                    .ToList();

            documentation.Elements.Clear();
            documentation.Elements.AddRange(ordered);
        }

        public static string? RewriteDocumentation(
            string? xml,
            TypeModel source,
            string sourceType,
            string facadeType)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return xml;

            string result =
                Regex.Replace(
                    xml,
                    @"cref\s*=\s*""([^""]+)""",
                    match =>
                    {
                        string cref =
                            match.Groups[1].Value;

                        string rewritten =
                            RewriteCrefTarget(
                                cref,
                                source,
                                facadeType);

                        return $"cref=\"{rewritten}\"";
                    });

            result = result.Replace(sourceType, facadeType);

            return result;
        }

        private static string RewriteCrefTarget(
            string cref,
            TypeModel source,
            string facadeType)
        {
            foreach (MethodModel method in source.Methods)
            {
                if (!method.IsConstructorProjection)
                    continue;

                // ---------------------------------------
                // Method(...)
                // -> TextBuilder(...)
                // ---------------------------------------

                string shortMethodPrefix =
                    method.Name + "(";

                if (cref.StartsWith(shortMethodPrefix, StringComparison.Ordinal))
                {
                    return facadeType +
                        cref.Substring(method.Name.Length);
                }

                // ---------------------------------------
                // MutableTextBuffer.Method(...)
                // -> TextBuilder(...)
                // ---------------------------------------

                string qualifiedMethodPrefix =
                    source.SourceType + "." + method.Name;

                if (cref.StartsWith(qualifiedMethodPrefix, StringComparison.Ordinal))
                {
                    return facadeType +
                        cref.Substring(qualifiedMethodPrefix.Length);
                }

                // ---------------------------------------
                // Method
                // -> TextBuilder
                // ---------------------------------------

                if (string.Equals(
                    cref,
                    method.Name,
                    StringComparison.Ordinal))
                {
                    return facadeType;
                }

                // ---------------------------------------
                // MutableTextBuffer.Method
                // -> TextBuilder
                // ---------------------------------------

                string qualifiedMethodName =
                    source.SourceType + "." + method.Name;

                if (string.Equals(
                    cref,
                    qualifiedMethodName,
                    StringComparison.Ordinal))
                {
                    return facadeType;
                }
            }

            return cref;
        }

        public static DocumentationModel? RewriteDocumentation(
            DocumentationModel? documentation,
            TypeModel implementationModel,
            string sourceType,
            string projectedBuilderType)
        {
            if (documentation is null)
                return null;

            DocumentationModel result = documentation.Clone();

            result.SummaryXml =
                RewriteDocumentation(
                    result.SummaryXml,
                    implementationModel,
                    sourceType,
                    projectedBuilderType);

            result.RemarksXml =
                RewriteDocumentation(
                    result.RemarksXml,
                    implementationModel,
                    sourceType,
                    projectedBuilderType);

            result.ReturnsXml =
                RewriteDocumentation(
                    result.ReturnsXml,
                    implementationModel,
                    sourceType,
                    projectedBuilderType);

            result.ValueXml =
                RewriteDocumentation(
                    result.ValueXml,
                    implementationModel,
                    sourceType,
                    projectedBuilderType);

            result.ExampleXml =
                RewriteDocumentation(
                    result.ExampleXml,
                    implementationModel,
                    sourceType,
                    projectedBuilderType);

            RewriteElementCollection(
                result.Exceptions,
                implementationModel,
                sourceType,
                projectedBuilderType);

            RewriteElementCollection(
                result.Permissions,
                implementationModel,
                sourceType,
                projectedBuilderType);

            RewriteElementCollection(
                result.SeeAlsos,
                implementationModel,
                sourceType,
                projectedBuilderType);

            RewriteElementCollection(
                result.Sees,
                implementationModel,
                sourceType,
                projectedBuilderType);

            RewriteElementCollection(
                result.TypeParameters,
                implementationModel,
                sourceType,
                projectedBuilderType);

            result.SynchronizationNoteXml =
                RewriteDocumentation(
                    result.SynchronizationNoteXml,
                    implementationModel,
                    sourceType,
                    projectedBuilderType);

            SortDocumentationElements(result);
            return result;
        }

        private static void RewriteElementCollection(
            IEnumerable<XmlDocumentationElementModel> elements,
            TypeModel implementationModel,
            string sourceType,
            string projectedBuilderType)
        {
            foreach (XmlDocumentationElementModel element in elements)
            {
                if (element.InnerXml is not null)
                {
                    element.InnerXml =
                        RewriteDocumentation(
                            element.InnerXml,
                            implementationModel,
                            sourceType,
                            projectedBuilderType);
                }

                List<string> keys =
                    element.Attributes.Keys.ToList();

                foreach (string key in keys)
                {
                    element.Attributes[key] =
                        RewriteDocumentation(
                            element.Attributes[key],
                            implementationModel,
                            sourceType,
                            projectedBuilderType)
                        ?? element.Attributes[key];
                }
            }
        }
    }
}
