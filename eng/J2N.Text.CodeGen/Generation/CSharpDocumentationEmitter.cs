#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

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
