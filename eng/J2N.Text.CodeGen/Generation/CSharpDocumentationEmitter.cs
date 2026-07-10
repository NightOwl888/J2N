using J2N.Text.CodeGen.Metadata;
using System.Text;

namespace J2N.Text.CodeGen.Generation
{
    public static class CSharpDocumentationEmitter
    {
        public static void EmitDocumentation(
            StringBuilder sb,
            DocumentationModel? docs)
        {
            if (docs is null)
            {
                return;
            }

            foreach (XmlDocumentationElementModel element in docs.Elements)
            {
                EmitDocumentationElement(sb, element);
            }
        }

        private static void EmitDocumentationElement(
            StringBuilder sb,
            XmlDocumentationElementModel element)
        {
            if (string.IsNullOrWhiteSpace(element.InnerXml))
            {
                sb.Append("        /// <");
                sb.Append(element.ElementName);

                foreach (var attribute in element.Attributes)
                {
                    sb.Append(' ');
                    sb.Append(attribute.Key);
                    sb.Append("=\"");
                    sb.Append(attribute.Value);
                    sb.Append('"');
                }

                sb.Append(" />");
                sb.AppendLine();

                return;
            }

            string[] lines =
                NormalizeDocumentationLines(element.InnerXml)
                    .ToArray();

            if (lines.Length == 1)
            {
                sb.Append("        /// <");
                sb.Append(element.ElementName);

                foreach (var attribute in element.Attributes)
                {
                    sb.Append(' ');
                    sb.Append(attribute.Key);
                    sb.Append("=\"");
                    sb.Append(attribute.Value);
                    sb.Append('"');
                }

                sb.Append('>');
                sb.Append(lines[0]);
                sb.Append("</");
                sb.Append(element.ElementName);
                sb.AppendLine(">");

                return;
            }

            sb.Append("        /// <");
            sb.Append(element.ElementName);

            foreach (KeyValuePair<string, string> attribute in element.Attributes)
            {
                sb.Append(' ');
                sb.Append(attribute.Key);
                sb.Append("=\"");
                sb.Append(attribute.Value);
                sb.Append('"');
            }

            sb.AppendLine(">");

            if (!string.IsNullOrWhiteSpace(element.InnerXml))
            {
                foreach (string line in NormalizeDocumentationLines(element.InnerXml))
                {
                    sb.Append("        /// ");

                    if (line.Length != 0)
                    {
                        sb.Append(line);
                    }

                    sb.AppendLine();
                }
            }

            sb.Append("        /// </");
            sb.Append(element.ElementName);
            sb.AppendLine(">");
        }

        private static IEnumerable<string> NormalizeDocumentationLines(
            string text)
        {
            string normalized =
                text.Replace("\r\n", "\n")
                    .Replace('\r', '\n');

            foreach (string line in normalized.Split('\n'))
            {
                string result = line.TrimEnd();

                if (result.StartsWith("///"))
                {
                    result = result[3..];

                    if (result.StartsWith(" "))
                    {
                        result = result[1..];
                    }
                }

                yield return result;
            }
        }
    }
}
