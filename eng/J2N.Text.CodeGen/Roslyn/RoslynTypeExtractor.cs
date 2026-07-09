using System.Xml.Linq;
using J2N.Text.CodeGen.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace J2N.Text.CodeGen.Roslyn
{
    public sealed class RoslynTypeExtractor
    {
        public TypeModel Extract(
            IEnumerable<string> sourceTexts,
            IEnumerable<string> infrastructureSourceTexts,
            string fullTypeName)
        {
            var parseOptions =
                new CSharpParseOptions(
                    preprocessorSymbols:
                    [
                        "FEATURE_INDEX_RANGE",
                        "FEATURE_MEMORYMARSHAL_CREATEREADONLYSPAN",
                        "FEATURE_MEMORYMARSHAL_GETARRAYDATAREFERENCE"
                    ]);


            IEnumerable<string> allSources =
                sourceTexts.Concat(infrastructureSourceTexts);

            List<SyntaxTree> trees =
                allSources
                    .Select(text =>
                        CSharpSyntaxTree.ParseText(
                            text,
                            parseOptions))
                    .Cast<SyntaxTree>()
                    .ToList();

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.GCSettings).Assembly.Location),
            };

            var compilation =
                CSharpCompilation.Create(
                    assemblyName: "CodeGen",
                    syntaxTrees: trees,
                    references: references);

            Dictionary<SyntaxTree, SemanticModel> semanticModels =
                trees.ToDictionary(
                    t => t,
                    t => compilation.GetSemanticModel(t));

            INamedTypeSymbol? typeSymbol =
                compilation.GlobalNamespace
                    .GetNamespaceMembers()
                    .SelectMany(GetAllNamespaces)
                    .SelectMany(n => n.GetTypeMembers())
                    .FirstOrDefault(t =>
                        t.ToDisplayString() == fullTypeName);

            if (typeSymbol is null)
            {
                throw new InvalidOperationException(
                    $"Type '{fullTypeName}' not found.");
            }

            var model = new TypeModel
            {
                Namespace = typeSymbol.ContainingNamespace.ToDisplayString(),
                Name = typeSymbol.Name,
                SourceType = typeSymbol.Name
            };

            foreach (SyntaxTree tree in trees)
            {
                CompilationUnitSyntax root =
                    tree.GetCompilationUnitRoot();

                foreach (UsingDirectiveSyntax usingDirective in root.Usings)
                {
                    string ns = usingDirective.Name?.ToString() ?? "";

                    if (!string.IsNullOrWhiteSpace(ns))
                    {
                        model.Usings.Add(ns);
                    }
                }
            }

            IEnumerable<MemberDeclarationSyntax> members =
                typeSymbol.DeclaringSyntaxReferences
                    .Select(r => r.GetSyntax())
                    .OfType<ClassDeclarationSyntax>()
                    .SelectMany(c => c.Members);

            foreach (MethodDeclarationSyntax method in members.OfType<MethodDeclarationSyntax>())
            {
                bool include =
                    method.Modifiers.Any(SyntaxKind.PublicKeyword)
                    || method.Modifiers.Any(SyntaxKind.InternalKeyword);

                if (!include)
                    continue;

                SemanticModel semanticModel =
                    semanticModels[method.SyntaxTree];

                model.Methods.Add(
                    ExtractMethod(
                        method,
                        semanticModel));
            }

            foreach (PropertyDeclarationSyntax property in members.OfType<PropertyDeclarationSyntax>())
            {
                if (!property.Modifiers.Any(SyntaxKind.PublicKeyword))
                    continue;

                SemanticModel semanticModel =
                    semanticModels[property.SyntaxTree];

                model.Properties.Add(
                    ExtractProperty(
                        property,
                        semanticModel));
            }

            foreach (IndexerDeclarationSyntax indexer in members.OfType<IndexerDeclarationSyntax>())
            {
                if (!indexer.Modifiers.Any(SyntaxKind.PublicKeyword))
                    continue;

                SemanticModel semanticModel =
                    semanticModels[indexer.SyntaxTree];

                model.Properties.Add(
                    ExtractIndexer(
                        indexer,
                        semanticModel));
            }

            return model;
        }

        private static IEnumerable<INamespaceSymbol> GetAllNamespaces(INamespaceSymbol root)
        {
            yield return root;

            foreach (INamespaceSymbol child in root.GetNamespaceMembers())
            {
                foreach (INamespaceSymbol descendant in GetAllNamespaces(child))
                {
                    yield return descendant;
                }
            }
        }

        private static MethodModel ExtractMethod(MethodDeclarationSyntax method, SemanticModel model)
        {
            IMethodSymbol? methodSymbol = model.GetDeclaredSymbol(method);

            if (methodSymbol is null)
            {
                throw new InvalidOperationException(
                    $"Unable to resolve symbol for method '{method.Identifier.Text}'.");
            }

            DocumentationModel? docs = ExtractDocumentation(method);

            string? returnType = method.ReturnType.ToString();
            var parameters = method.ParameterList.Parameters
                .Select(p =>
                {
                    string parameterType =
                        p.Type?.ToString() ?? "object";

                    return new ParameterModel
                    {
                        Name = p.Identifier.Text,

                        TypeName = parameterType,
                        SourceTypeName = parameterType,

                        Documentation =
                            ExtractParamDocumentation(
                                method,
                                p.Identifier.Text),

                        Modifier =
                            string.Join(
                                " ",
                                p.Modifiers.Select(m => m.Text)),

                        IsThis =
                            p.Modifiers.Any(SyntaxKind.ThisKeyword),

                        DefaultValueExpression =
                            p.Default?.Value.ToString(),
                    };
                }).ToList();

            return new MethodModel
            {
                Name = method.Identifier.Text,
                ReturnType = returnType,
                ReturnsSelf =
                    methodSymbol.HasAttribute(CodeGenerationAttributeNames.ReturnsSelf),
                Ignore =
                    methodSymbol.HasAttribute(
                        CodeGenerationAttributeNames.Ignore),
                IsBuilderMethod = false,
                IsExtensionMethod =
                    method.ParameterList.Parameters.FirstOrDefault()?
                        .Modifiers.Any(SyntaxKind.ThisKeyword)
                    ?? false,
                IsConstructorProjection =
                    methodSymbol.HasAttribute(
                        CodeGenerationAttributeNames.Constructor),
                IsStatic =
                    method.Modifiers.Any(SyntaxKind.StaticKeyword),
                DeclaredAccessibility = methodSymbol.DeclaredAccessibility,
                IsUnsafe =
                    IsUnsafeType(returnType)
                    || parameters.Any(p => IsUnsafeType(p.TypeName)),
                BodyText = method.Body?.ToFullString()
                    ?? method.ExpressionBody?.ToFullString(),
                Documentation = docs,
                Parameters = parameters,
                GenericParameters = ExtractGenericParameters(method),
                Attributes = ExtractAttributes(method.AttributeLists),
                SkipSynchronization =
                    methodSymbol.HasAttribute(
                        CodeGenerationAttributeNames.SkipSynchronization),
                IsExtensionImplementation =
                    methodSymbol.HasAttribute(
                        CodeGenerationAttributeNames.ExtensionImplementation),
                GenerateForwarder =
                    methodSymbol.HasAttribute(
                        CodeGenerationAttributeNames.GenerateForwarder),
            };
        }

        private static PropertyModel ExtractProperty(PropertyDeclarationSyntax property, SemanticModel model)
        {
            IPropertySymbol? propertySymbol = model.GetDeclaredSymbol(property);

            if (propertySymbol is null)
            {
                throw new InvalidOperationException(
                    $"Unable to resolve symbol for property '{property.Identifier.Text}'.");
            }

            string typeName = property.Type.ToString();

            bool hasGetter = false;
            bool hasSetter = false;

            if (property.ExpressionBody is not null)
            {
                hasGetter = true;
            }
            else if (property.AccessorList is not null)
            {
                foreach (AccessorDeclarationSyntax accessor in property.AccessorList.Accessors)
                {
                    switch (accessor.Kind())
                    {
                        case SyntaxKind.GetAccessorDeclaration:
                            hasGetter = true;
                            break;

                        case SyntaxKind.SetAccessorDeclaration:
                            hasSetter = true;
                            break;
                    }
                }
            }

            bool skipGetterSynchronization =
                propertySymbol.GetMethod?
                    .HasAttribute(
                        CodeGenerationAttributeNames.SkipSynchronization)
                ?? false;

            bool skipSetterSynchronization =
                propertySymbol.SetMethod?
                    .HasAttribute(
                        CodeGenerationAttributeNames.SkipSynchronization)
                ?? false;

            return new PropertyModel
            {
                Name = property.Identifier.Text,
                TypeName = typeName,
                HasGetter = hasGetter,
                HasSetter = hasSetter,
                IsIndexer = false,
                IsStatic =
                    property.Modifiers.Any(SyntaxKind.StaticKeyword),
                IsUnsafe = IsUnsafeType(typeName),

                Documentation = ExtractDocumentation(property),

                Attributes = ExtractAttributes(property.AttributeLists),

                Ignore =
                    propertySymbol.HasAttribute(
                        CodeGenerationAttributeNames.Ignore),

                SkipGetterSynchronization =
                    skipGetterSynchronization,

                SkipSetterSynchronization =
                    skipSetterSynchronization,
            };
        }

        private static PropertyModel ExtractIndexer(IndexerDeclarationSyntax indexer, SemanticModel model)
        {
            string? typeName = indexer.Type.ToString();

            IPropertySymbol? indexerSymbol = model.GetDeclaredSymbol(indexer);

            if (indexerSymbol is null)
            {
                throw new InvalidOperationException(
                    $"Unable to resolve symbol for indexer '{typeName}'.");
            }

            var parameters = indexer.ParameterList.Parameters
                .Select(p =>
                {
                    string parameterType =
                        p.Type?.ToString() ?? "object";

                    return new ParameterModel
                    {
                        Name = p.Identifier.Text,

                        TypeName = parameterType,
                        SourceTypeName = parameterType,

                        Documentation =
                            ExtractParamDocumentation(
                                indexer,
                                p.Identifier.Text),

                        Modifier =
                            string.Join(
                                " ",
                                p.Modifiers.Select(m => m.Text)),

                        IsThis =
                            p.Modifiers.Any(SyntaxKind.ThisKeyword),

                        DefaultValueExpression =
                            p.Default?.Value.ToString(),
                    };
                }).ToList();

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
                Ignore =
                    indexerSymbol.HasAttribute(
                        CodeGenerationAttributeNames.Ignore),
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

        private static DocumentationCommentTriviaSyntax? GetDocumentationTrivia(
            MemberDeclarationSyntax member)
        {
            return member.GetLeadingTrivia()
                .Select(t => t.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();
        }

        private static DocumentationModel? ExtractDocumentation(
            MemberDeclarationSyntax member)
        {
            DocumentationCommentTriviaSyntax? docs =
                GetDocumentationTrivia(member);

            if (docs is null)
                return null;

            return new DocumentationModel
            {
                SummaryXml =
                    GetXmlElementInnerText(docs, "summary"),

                RemarksXml =
                    GetXmlElementInnerText(docs, "remarks"),

                ReturnsXml =
                    GetXmlElementInnerText(docs, "returns"),

                SynchronizationNoteXml =
                    GetXmlElementInnerText(docs, "synchronizationNote")
            };
        }

        private static string? ExtractParamDocumentation(
            MemberDeclarationSyntax member,
            string paramName)
        {
            DocumentationCommentTriviaSyntax? docs =
                GetDocumentationTrivia(member);

            if (docs is null)
                return null;

            XmlElementSyntax? paramElement =
                docs.Content
                    .OfType<XmlElementSyntax>()
                    .FirstOrDefault(e =>
                        e.StartTag?.Name.LocalName.Text == "param"
                        && e.StartTag.Attributes
                            .OfType<XmlNameAttributeSyntax>()
                            .Any(a =>
                                a.Name?.LocalName.Text == "name"
                                && a.Identifier?.Identifier.ValueText == paramName));

            if (paramElement is null)
                return null;

            return NormalizeDocumentationContent(
                paramElement.Content);
        }

        private static string? GetXmlElementInnerText(
            DocumentationCommentTriviaSyntax docs,
            string elementName)
        {
            XmlElementSyntax? element =
                docs.Content
                    .OfType<XmlElementSyntax>()
                    .FirstOrDefault(e =>
                        e.StartTag?.Name.LocalName.Text == elementName);

            if (element is null)
                return null;

            return NormalizeDocumentationContent(
                element.Content);
        }

        private static string NormalizeDocumentationContent(
            SyntaxList<XmlNodeSyntax> content)
        {
            string raw =
                string.Concat(content.Select(c => c.ToString()));

            string normalized =
                raw.Replace("\r\n", "\n")
                   .Replace('\r', '\n');

            string[] lines =
                normalized.Split('\n');

            return string.Join(
                Environment.NewLine,
                lines.Select(l => l.TrimEnd()));
        }

        private static bool IsUnsafeType(string typeName)
        {
            return typeName.Contains('*');
        }
    }
}
