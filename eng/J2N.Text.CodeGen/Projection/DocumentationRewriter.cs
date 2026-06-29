using J2N.Text.CodeGen.Metadata;
using System.Text.RegularExpressions;

namespace J2N.Text.CodeGen.Projection
{
    internal static class DocumentationRewriter
    {
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
    }
}
