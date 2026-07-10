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

        public static XmlDocumentationElementModel RewriteElement(
            XmlDocumentationElementModel element,
            TypeModel source,
            string sourceType,
            string projectedType)
        {
            XmlDocumentationElementModel rewritten =
                new()
                {
                    ElementName = element.ElementName,
                    InnerXml =
                        RewriteDocumentation(
                            element.InnerXml,
                            source,
                            sourceType,
                            projectedType)
                };

            foreach ((string key, string value) in element.Attributes)
            {
                rewritten.Attributes.Add(
                    key,
                    RewriteDocumentation(
                        value,
                        source,
                        sourceType,
                        projectedType)
                    ?? value);
            }

            return rewritten;
        }
    }
}
