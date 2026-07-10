using J2N.Text.CodeGen.Metadata;
using Microsoft.CodeAnalysis;

namespace J2N.Text.CodeGen.Projection
{
    public sealed class BuilderProjection
    {
        public ProjectedTypeModel Project(
            TypeModel source,
            string facadeNamespace,
            string facadeName,
            ProjectionOptions? options = null)
        {
            options ??= new ProjectionOptions();

            var projected = new ProjectedTypeModel
            {
                Source = source,
                Namespace = facadeNamespace,
                Name = facadeName
            };

            foreach (MethodModel method in source.Methods.Where(x => !x.Ignore))
            {
                if (method.DeclaredAccessibility != Accessibility.Public)
                    continue;

                projected.Methods.Add(
                    ProjectMethod(
                        method,
                        source,
                        source.SourceType,
                        facadeName,
                        options));
            }

            foreach (PropertyModel property in source.Properties.Where(x => !x.Ignore))
            {
                projected.Properties.Add(
                    ProjectProperty(
                        property,
                        source,
                        source.SourceType,
                        facadeName,
                        options));
            }

            return projected;
        }

        private static MethodModel ProjectMethod(
            MethodModel method,
            TypeModel source,
            string sourceType,
            string facadeName,
            ProjectionOptions options)
        {
            return new MethodModel
            {
                Name = method.Name,

                ReturnType =
                    method.ReturnsSelf
                        ? facadeName
                        : RewriteType(
                            method.ReturnType,
                            sourceType,
                            facadeName),

                ReturnsSelf = method.ReturnsSelf,

                //
                // Builder methods are ONLY methods explicitly marked
                // with [CodeGenerationReturnsSelf]
                //
                IsBuilderMethod = method.ReturnsSelf,

                IsUnsafe = method.IsUnsafe,
                IsStatic = method.IsStatic,
                IsExtensionMethod = method.IsExtensionMethod,

                Parameters =
                    method.Parameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Name,

                            TypeName =
                                RewriteType(
                                    p.TypeName,
                                    sourceType,
                                    facadeName),

                            SourceTypeName = p.SourceTypeName,

                            Documentation =
                                DocumentationRewriter.RewriteDocumentation(
                                    p.Documentation,
                                    source,
                                    source.Name,
                                    facadeName),

                            Modifier = p.Modifier,
                            IsThis = p.IsThis,
                            DefaultValueExpression = p.DefaultValueExpression,
                        })
                        .ToList(),

                GenericParameters =
                    method.GenericParameters
                        .Select(CloneGenericParameter)
                        .ToList(),

                Attributes =
                    method.Attributes
                        .Select(CloneAttribute)
                        .ToList(),

                Documentation =
                    RewriteDocumentation(
                        method.Documentation,
                        source,
                        facadeName,
                        options,
                        includeSynchronizationNote:
                            options.EmitSynchronizationNotes &&
                            method.SkipSynchronization,
                        forceBuilderReturns:
                            method.ReturnsSelf),

                //
                // NEW:
                //
                ConditionalCompilationSymbol =
                    method.Parameters.Any(p =>
                        p.TypeName is "Index" or "Range")
                            ? "FEATURE_INDEX_RANGE"
                            : null,

                SkipSynchronization =
                    method.SkipSynchronization,
            };
        }

        private static PropertyModel ProjectProperty(
            PropertyModel property,
            TypeModel source,
            string sourceType,
            string facadeName,
            ProjectionOptions options)
        {
            return new PropertyModel
            {
                Name = property.Name,

                TypeName =
                    RewriteType(
                        property.TypeName,
                        sourceType,
                        facadeName),

                HasGetter = property.HasGetter,
                HasSetter = property.HasSetter,
                IsIndexer = property.IsIndexer,
                IsUnsafe = property.IsUnsafe,
                IsStatic = property.IsStatic,

                IndexParameters =
                    property.IndexParameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Name,

                            TypeName =
                                RewriteType(
                                    p.TypeName,
                                    sourceType,
                                    facadeName),

                            SourceTypeName = p.SourceTypeName,

                            Documentation =
                                DocumentationRewriter.RewriteDocumentation(
                                    p.Documentation,
                                    source,
                                    source.Name,
                                    facadeName),

                            Modifier = p.Modifier,
                            IsThis = p.IsThis,
                            DefaultValueExpression = p.DefaultValueExpression,
                        })
                        .ToList(),

                Attributes =
                    property.Attributes
                        .Select(CloneAttribute)
                        .ToList(),

                Documentation =
                    RewriteDocumentation(
                        property.Documentation,
                        source,
                        facadeName,
                        options,
                        includeSynchronizationNote:
                            options.EmitSynchronizationNotes &&
                            (property.SkipGetterSynchronization || property.SkipSetterSynchronization)),


                SkipGetterSynchronization =
                    property.SkipGetterSynchronization,

                SkipSetterSynchronization =
                    property.SkipSetterSynchronization,
            };
        }

        private static string RewriteType(
            string typeName,
            string sourceType,
            string facadeName)
        {
            return typeName.Replace(sourceType, facadeName);
        }

        private static DocumentationModel? RewriteDocumentation(
            DocumentationModel? documentation,
            TypeModel source,
            string facadeName,
            ProjectionOptions options,
            bool includeSynchronizationNote,
            bool forceBuilderReturns = false)
        {
            if (documentation is null)
            {
                if (!forceBuilderReturns)
                    return null;

                documentation = new DocumentationModel();
            }

            DocumentationModel result = new();

            foreach (XmlDocumentationElementModel element in documentation.Elements)
            {
                //
                // synchronizationNote is optionally merged into remarks.
                //
                if (element.ElementName == "synchronizationNote")
                {
                    continue;
                }

                XmlDocumentationElementModel rewritten =
                    RewriteDocumentationElement(
                        element,
                        source,
                        facadeName);

                result.Elements.Add(rewritten);
            }

            if (includeSynchronizationNote)
            {
                XmlDocumentationElementModel? sync =
                    documentation.Elements.FirstOrDefault(
                        e => e.ElementName == "synchronizationNote");

                if (sync is not null)
                {
                    XmlDocumentationElementModel? remarks =
                        result.Elements.FirstOrDefault(
                            e => e.ElementName == "remarks");

                    if (remarks is null)
                    {
                        result.Elements.Add(
                            new XmlDocumentationElementModel
                            {
                                ElementName = "remarks",
                                InnerXml = sync.InnerXml
                            });
                    }
                    else
                    {
                        remarks.InnerXml +=
                            Environment.NewLine +
                            "<para/>" +
                            Environment.NewLine +
                            sync.InnerXml;
                    }
                }
            }

            if (forceBuilderReturns)
            {
                XmlDocumentationElementModel? returns =
                    result.Elements.FirstOrDefault(
                        e => e.ElementName == "returns");

                if (returns is null)
                {
                    result.Elements.Add(
                        new XmlDocumentationElementModel
                        {
                            ElementName = "returns",
                            InnerXml =
                                "A reference to this instance after the operation has completed."
                        });
                }
                else
                {
                    returns.InnerXml =
                        "A reference to this instance after the operation has completed.";
                }
            }

            DocumentationRewriter.SortDocumentationElements(result);

            return result;
        }

        private static XmlDocumentationElementModel RewriteDocumentationElement(
            XmlDocumentationElementModel element,
            TypeModel source,
            string facadeName)
        {
            XmlDocumentationElementModel rewritten =
                new()
                {
                    ElementName = element.ElementName,
                    InnerXml =
                        DocumentationRewriter.RewriteDocumentation(
                            element.InnerXml,
                            source,
                            source.Name,
                            facadeName)
                };

            foreach (KeyValuePair<string, string> attribute in element.Attributes)
            {
                rewritten.Attributes.Add(
                    attribute.Key,
                    DocumentationRewriter.RewriteDocumentation(
                        attribute.Value,
                        source,
                        source.Name,
                        facadeName)
                    ?? attribute.Value);
            }

            return rewritten;
        }

        private static AttributeModel CloneAttribute(
            AttributeModel attribute)
        {
            return new AttributeModel
            {
                Name = attribute.Name,
                Arguments = attribute.Arguments.ToList()
            };
        }

        private static GenericParameterModel CloneGenericParameter(
            GenericParameterModel parameter)
        {
            return new GenericParameterModel
            {
                Name = parameter.Name,
                Constraints = parameter.Constraints.ToList()
            };
        }
    }
}
