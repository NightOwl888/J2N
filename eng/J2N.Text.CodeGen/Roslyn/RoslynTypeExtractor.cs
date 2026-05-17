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
            string? returnType = method.ReturnType.ToString();
            var parameters = method.ParameterList.Parameters
                .Select(p => new ParameterModel
                {
                    Name = p.Identifier.Text,
                    TypeName = p.Type?.ToString() ?? "object",
                    Documentation =
                        ExtractParamDocumentation(
                            method,
                            p.Identifier.Text)
                })
                .ToList();

            return new MethodModel
            {
                Name = method.Identifier.Text,
                ReturnType = returnType,
                ReturnsSelf = false,
                IsBuilderMethod = false,
                IsUnsafe =
                    IsUnsafeType(returnType)
                    || parameters.Any(p => IsUnsafeType(p.TypeName)),
                Documentation = ExtractDocumentation(method),
                Parameters = parameters,
                GenericParameters = ExtractGenericParameters(method),
                Attributes = ExtractAttributes(method.AttributeLists),
            };
        }

        private static PropertyModel ExtractProperty(PropertyDeclarationSyntax property)
        {
            string? typeName = property.Type.ToString();

            return new PropertyModel
            {
                Name = property.Identifier.Text,
                TypeName = typeName,
                HasGetter =
                    property.AccessorList?.Accessors
                        .Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration)
                    ?? false,
                HasSetter =
                    property.AccessorList?.Accessors
                        .Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration)
                    ?? false,
                IsIndexer = false,
                IsUnsafe = IsUnsafeType(typeName),
                Documentation = ExtractDocumentation(property),
                Attributes = ExtractAttributes(property.AttributeLists),
            };
        }

        private static PropertyModel ExtractIndexer(IndexerDeclarationSyntax indexer)
        {
            string? typeName = indexer.Type.ToString();
            var parameters = indexer.ParameterList.Parameters
                .Select(p => new ParameterModel
                {
                    Name = p.Identifier.Text,
                    TypeName = p.Type?.ToString() ?? "object"
                })
                .ToList();

            return new PropertyModel
            {
                Name = "this",
                TypeName = typeName,
                HasGetter =
                    indexer.AccessorList?.Accessors
                        .Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration) ?? false,
                HasSetter =
                    indexer.AccessorList?.Accessors
                        .Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration) ?? false,
                IsIndexer = true,
                IsUnsafe =
                    IsUnsafeType(typeName)
                    || parameters.Any(p => IsUnsafeType(p.TypeName)),
                Documentation = ExtractDocumentation(indexer),
                IndexParameters = parameters,
                Attributes = ExtractAttributes(indexer.AttributeLists),
            };
        }

        private static List<AttributeModel> ExtractAttributes(SyntaxList<AttributeListSyntax> attributeLists)
        {
            var result = new List<AttributeModel>();

            foreach (AttributeListSyntax list in attributeLists)
            {
                foreach (AttributeSyntax attribute in list.Attributes)
                {
                    var model = new AttributeModel
                    {
                        Name = attribute.Name.ToString()
                    };

                    if (attribute.ArgumentList is not null)
                    {
                        foreach (AttributeArgumentSyntax arg in attribute.ArgumentList.Arguments)
                        {
                            model.Arguments.Add(arg.ToString());
                        }
                    }

                    result.Add(model);
                }
            }

            return result;
        }

        private static List<GenericParameterModel> ExtractGenericParameters(MethodDeclarationSyntax method)
        {
            var result = new List<GenericParameterModel>();

            if (method.TypeParameterList is null)
            {
                return result;
            }

            foreach (TypeParameterSyntax parameter in method.TypeParameterList.Parameters)
            {
                var model = new GenericParameterModel
                {
                    Name = parameter.Identifier.Text
                };

                foreach (TypeParameterConstraintClauseSyntax clause in method.ConstraintClauses)
                {
                    if (clause.Name.ToString() != model.Name)
                    {
                        continue;
                    }

                    foreach (TypeParameterConstraintSyntax constraint in clause.Constraints)
                    {
                        model.Constraints.Add(constraint.ToString());
                    }
                }

                result.Add(model);
            }

            return result;
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
                SummaryXml = GetInnerXml(root.Element("summary")),
                RemarksXml = GetInnerXml(root.Element("remarks")),
                ReturnsXml = GetInnerXml(root.Element("returns"))
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

            return param is null
                ? null
                : NormalizeXml(param.Nodes());
        }

        private static string NormalizeXml(IEnumerable<XNode> nodes)
        {
            string raw =
                string.Concat(nodes.Select(n => n.ToString()));

            string[] lines =
                raw.Replace("\r\n", "\n")
                    .Split('\n');

            return string.Join(
                Environment.NewLine,
                lines.Select(l => l.TrimEnd()));
        }

        private static string? GetInnerXml(XElement? element)
        {
            if (element is null)
                return null;

            return string.Concat(
                element.Nodes()
                    .Select(n => n.ToString()));
        }

        private static bool IsUnsafeType(string typeName)
        {
            return typeName.Contains('*');
        }
    }
}
