using System.Xml.Linq;
using J2N.Text.CodeGen.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace J2N.Text.CodeGen.Roslyn
{
    public sealed class RoslynTypeExtractor
    {
        public TypeModel Extract(string sourceText)
        {
            SyntaxTree tree =
                CSharpSyntaxTree.ParseText(sourceText);

            CompilationUnitSyntax root =
                tree.GetCompilationUnitRoot();

            ClassDeclarationSyntax classNode =
                root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .First();

            string ns =
                root.DescendantNodes()
                    .OfType<NamespaceDeclarationSyntax>()
                    .First()
                    .Name
                    .ToString();

            var model = new TypeModel
            {
                Namespace = ns,
                Name = classNode.Identifier.Text,
                SourceType = classNode.Identifier.Text
            };

            foreach (UsingDirectiveSyntax usingDirective in root.Usings)
            {
                model.Usings.Add(usingDirective.Name!.ToString());
            }

            foreach (MethodDeclarationSyntax method in classNode.Members.OfType<MethodDeclarationSyntax>())
            {
                if (!method.Modifiers.Any(SyntaxKind.PublicKeyword))
                    continue;

                model.Methods.Add(ExtractMethod(method));
            }

            foreach (PropertyDeclarationSyntax property in classNode.Members.OfType<PropertyDeclarationSyntax>())
            {
                if (!property.Modifiers.Any(SyntaxKind.PublicKeyword))
                    continue;

                model.Properties.Add(ExtractProperty(property));
            }

            foreach (IndexerDeclarationSyntax indexer in classNode.Members.OfType<IndexerDeclarationSyntax>())
            {
                if (!indexer.Modifiers.Any(SyntaxKind.PublicKeyword))
                    continue;

                model.Properties.Add(ExtractIndexer(indexer));
            }

            return model;
        }

        private static MethodModel ExtractMethod(MethodDeclarationSyntax method)
        {
            return new MethodModel
            {
                Name = method.Identifier.Text,
                ReturnType = method.ReturnType.ToString(),
                ReturnsSelf = false,
                IsBuilderMethod = false,
                Documentation = ExtractDocumentation(method),
                Parameters =
                    method.ParameterList.Parameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Identifier.Text,
                            TypeName = p.Type?.ToString() ?? "object",
                            Documentation =
                                ExtractParamDocumentation(
                                    method,
                                    p.Identifier.Text)
                        })
                        .ToList()
            };
        }

        private static PropertyModel ExtractProperty(PropertyDeclarationSyntax property)
        {
            return new PropertyModel
            {
                Name = property.Identifier.Text,
                TypeName = property.Type.ToString(),
                HasGetter =
                    property.AccessorList?.Accessors
                        .Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration)
                    ?? false,
                HasSetter =
                    property.AccessorList?.Accessors
                        .Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration)
                    ?? false,
                IsIndexer = false,
                Documentation = ExtractDocumentation(property)
            };
        }

        private static PropertyModel ExtractIndexer(IndexerDeclarationSyntax indexer)
        {
            return new PropertyModel
            {
                Name = "this",
                TypeName = indexer.Type.ToString(),
                HasGetter =
                    indexer.AccessorList.Accessors
                        .Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration),
                HasSetter =
                    indexer.AccessorList.Accessors
                        .Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration),
                IsIndexer = true,
                Documentation = ExtractDocumentation(indexer),
                IndexParameters =
                    indexer.ParameterList.Parameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Identifier.Text,
                            TypeName = p.Type?.ToString() ?? "object"
                        })
                        .ToList()
            };
        }

        private static DocumentationModel? ExtractDocumentation(MemberDeclarationSyntax member)
        {
            string xml =
                string.Concat(
                    member.GetLeadingTrivia()
                        .Select(t => t.ToFullString())
                        .Where(s => s.TrimStart().StartsWith("///")));

            if (string.IsNullOrWhiteSpace(xml))
                return null;

            string normalized =
                string.Join(
                    Environment.NewLine,
                    xml.Split(Environment.NewLine)
                        .Select(l =>
                        {
                            string trimmed = l.TrimStart();

                            if (trimmed.StartsWith("///"))
                                return trimmed.Substring(3);

                            return trimmed;
                        }));

            XElement root =
                XElement.Parse("<root>" + normalized + "</root>");

            return new DocumentationModel
            {
                Summary = Normalize(root.Element("summary")?.Value),
                Remarks = Normalize(root.Element("remarks")?.Value),
                Returns = Normalize(root.Element("returns")?.Value)
            };
        }

        private static string? ExtractParamDocumentation(
            MemberDeclarationSyntax member,
            string paramName)
        {
            string xml =
                string.Concat(
                    member.GetLeadingTrivia()
                        .Select(t => t.ToFullString())
                        .Where(s => s.TrimStart().StartsWith("///")));

            if (string.IsNullOrWhiteSpace(xml))
                return null;

            string normalized =
                string.Join(
                    Environment.NewLine,
                    xml.Split(Environment.NewLine)
                        .Select(l =>
                        {
                            string trimmed = l.TrimStart();

                            if (trimmed.StartsWith("///"))
                                return trimmed.Substring(3);

                            return trimmed;
                        }));

            XElement root =
                XElement.Parse("<root>" + normalized + "</root>");

            XElement? param =
                root.Elements("param")
                    .FirstOrDefault(x =>
                        x.Attribute("name")?.Value == paramName);

            return Normalize(param?.Value);
        }

        private static string? Normalize(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            return string.Join(
                Environment.NewLine,
                text.Split('\n')
                    .Select(x => x.Trim()));
        }
    }
}
