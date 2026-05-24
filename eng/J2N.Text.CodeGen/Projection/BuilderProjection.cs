using J2N.Text.CodeGen.Metadata;
using Microsoft.CodeAnalysis;
using System.Text.RegularExpressions;

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
                                RewriteDocumentation(
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
                    CreateProjectedDocumentation(
                        method,
                        source,
                        facadeName,
                        options),

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
            DocumentationModel? rewrittenDocs = property.Documentation is null
                ? null
                : new DocumentationModel
                {
                    SummaryXml =
                        RewriteDocumentation(
                            property.Documentation.SummaryXml,
                            source,
                            source.Name,
                            facadeName),

                    RemarksXml =
                        RewriteDocumentation(
                            property.Documentation.RemarksXml,
                            source,
                            source.Name,
                            facadeName),

                    ReturnsXml =
                        RewriteDocumentation(
                            property.Documentation.ReturnsXml,
                            source,
                            source.Name,
                            facadeName),

                    SynchronizationNoteXml =
                        RewriteDocumentation(
                            property.Documentation.SynchronizationNoteXml,
                            source,
                            source.Name,
                            facadeName)
                };

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
                                RewriteDocumentation(
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
                    MergeSynchronizationDocumentation(
                        rewrittenDocs,
                        includeSynchronizationNote: options.EmitSynchronizationNotes &&
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

        private static string? RewriteDocumentation(
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

                string methodPrefix =
                    method.Name + "(";

                if (cref.StartsWith(
                    methodPrefix,
                    StringComparison.Ordinal))
                {
                    return facadeType
                        + cref.Substring(method.Name.Length);
                }
            }

            return cref;
        }

        private static DocumentationModel? CreateProjectedDocumentation(
            MethodModel method,
            TypeModel source,
            string facadeName,
            ProjectionOptions options)
        {
            DocumentationModel? docs = method.Documentation;

            //
            // If there was no documentation at all,
            // but this is a builder-returning API,
            // synthesize the minimum required docs.
            //
            if (docs is null)
            {
                if (!method.ReturnsSelf)
                {
                    return null;
                }

                return new DocumentationModel
                {
                    ReturnsXml =
                        "A reference to this instance after the operation has completed."
                };
            }

            string? returnsXml =
                RewriteDocumentation(
                    docs.ReturnsXml,
                    source,
                    source.Name,
                    facadeName);

            //
            // Builder methods always get standardized return docs.
            //
            if (method.ReturnsSelf)
            {
                returnsXml =
                    "A reference to this instance after the operation has completed.";
            }

            return MergeSynchronizationDocumentation(
                new DocumentationModel
                {
                    SummaryXml =
                        RewriteDocumentation(
                            docs.SummaryXml,
                            source,
                            source.Name,
                            facadeName),

                    RemarksXml =
                        RewriteDocumentation(
                            docs.RemarksXml,
                            source,
                            source.Name,
                            facadeName),

                    ReturnsXml = returnsXml,

                    SynchronizationNoteXml =
                        RewriteDocumentation(
                            docs.SynchronizationNoteXml,
                            source,
                            source.Name,
                            facadeName),
                },
                includeSynchronizationNote: options.EmitSynchronizationNotes && method.SkipSynchronization);
        }

        private static DocumentationModel? MergeSynchronizationDocumentation(
            DocumentationModel? docs,
            bool includeSynchronizationNote)
        {
            if (docs is null)
                return null;

            string? remarksXml = docs.RemarksXml;

            if (includeSynchronizationNote
                && !string.IsNullOrWhiteSpace(docs.SynchronizationNoteXml))
            {
                if (string.IsNullOrWhiteSpace(remarksXml))
                {
                    remarksXml = docs.SynchronizationNoteXml;
                }
                else
                {
                    remarksXml +=
                        "<para/>"
                        + docs.SynchronizationNoteXml;
                }
            }

            return new DocumentationModel
            {
                SummaryXml = docs.SummaryXml,
                RemarksXml = remarksXml,
                ReturnsXml = docs.ReturnsXml
            };
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
