using J2N.Text.CodeGen.Metadata;

namespace J2N.Text.CodeGen.Projection
{
    public sealed class ExtensionMethodProjection
    {
        public ProjectedTypeModel Project(
            TypeModel extensionSource,
            string targetSourceType,
            string facadeNamespace,
            string projectedBuilderType,
            string projectedTypeName,
            ProjectionOptions? options = null)
        {
            options ??= new ProjectionOptions();

            var projected = new ProjectedTypeModel
            {
                Source = extensionSource,
                Namespace = facadeNamespace,
                Name = projectedTypeName,
            };

            foreach (MethodModel method in extensionSource.Methods.Where(x => !x.Ignore))
            {
                if (!method.IsExtensionMethod)
                    continue;

                if (!TargetsMutableTextBuffer(method, targetSourceType))
                    continue;

                projected.Methods.Add(
                    ProjectMethod(
                        method,
                        targetSourceType,
                        projectedBuilderType,
                        options));
            }

            return projected;
        }

        private static MethodModel ProjectMethod(
            MethodModel method,
            string sourceType,
            string projectedBuilderType,
            ProjectionOptions options)
        {
            var methodModel =  new MethodModel
            {
                Name = method.Name,

                ReturnType =
                    RewriteType(
                        method.ReturnType,
                        sourceType,
                        projectedBuilderType),

                ReturnsSelf = method.ReturnsSelf,
                IsBuilderMethod = method.IsBuilderMethod,
                IsUnsafe = method.IsUnsafe,
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
                                    projectedBuilderType),

                            SourceTypeName = p.SourceTypeName,

                            Documentation =
                                RewriteDocumentation(
                                    p.Documentation,
                                    sourceType,
                                    projectedBuilderType),

                            Modifier = p.Modifier,
                            IsThis = p.IsThis,
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
                    MergeSynchronizationDocumentation(
                        method.Documentation is null
                            ? null
                            : new DocumentationModel
                            {
                                SummaryXml =
                                    RewriteDocumentation(
                                        method.Documentation.SummaryXml,
                                        sourceType,
                                        projectedBuilderType),

                                RemarksXml =
                                    RewriteDocumentation(
                                        method.Documentation.RemarksXml,
                                        sourceType,
                                        projectedBuilderType),

                                ReturnsXml =
                                    RewriteDocumentation(
                                        method.Documentation.ReturnsXml,
                                        sourceType,
                                        projectedBuilderType),

                                SynchronizationNoteXml =
                                    RewriteDocumentation(
                                        method.Documentation.SynchronizationNoteXml,
                                        sourceType,
                                        projectedBuilderType)
                            },
                            includeSynchronizationNote: options.EmitSynchronizationNotes && method.SkipSynchronization),

                BodyText =
                    RewriteBody(
                        method.BodyText,
                        sourceType,
                        projectedBuilderType)
            };

            methodModel.ConditionalCompilationSymbol =
                methodModel.Parameters.Any(p =>
                    p.TypeName is "Index" or "Range")
                        ? "FEATURE_INDEX_RANGE"
                        : null;

            return methodModel;
        }

        private static bool TargetsMutableTextBuffer(
            MethodModel method,
            string sourceType)
        {
            if (method.Parameters.Count == 0)
                return false;

            ParameterModel first = method.Parameters[0];

            if (!first.IsThis)
                return false;

            return first.TypeName.Contains(sourceType);
        }

        private static string RewriteType(
            string typeName,
            string sourceType,
            string projectedBuilderType)
        {
            return typeName.Replace(sourceType, projectedBuilderType);
        }

        private static string? RewriteDocumentation(
            string? xml,
            string sourceType,
            string projectedBuilderType)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return xml;

            return xml.Replace(sourceType, projectedBuilderType);
        }

        private static DocumentationModel? MergeSynchronizationDocumentation(DocumentationModel? docs, bool includeSynchronizationNote)
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

        private static string? RewriteBody(
            string? body,
            string sourceType,
            string projectedBuilderType)
        {
            if (string.IsNullOrWhiteSpace(body))
                return body;

            string result = body;

            result =
                result.Replace(
                    sourceType,
                    projectedBuilderType);

            result =
                result.Replace(
                    ".m_Chars",
                    ".buffer.m_Chars");

            result =
                result.Replace(
                    "(text?.buffer",
                    "(text");

            result =
                result.Replace(
                    "(text.buffer",
                    "(text");

            result =
                result.Replace(
                    "text.m_Chars",
                    "text.buffer.m_Chars");

            return result;
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