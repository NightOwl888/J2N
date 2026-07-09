using J2N.Text.CodeGen.Metadata;

namespace J2N.Text.CodeGen.Projection
{
    public sealed class ExtensionMethodProjection
    {
        public ProjectedTypeModel Project(
            TypeModel extensionSource,
            TypeModel implementationModel,
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

            Dictionary<string, string> implementationLookup =
                implementationModel.Methods
                    .Where(m => m.IsExtensionImplementation)
                    .ToDictionary(
                        m => RemoveSuffix(m.Name, "Internal"),
                        m => m.Name);

            foreach (MethodModel method in extensionSource.Methods.Where(x => !x.Ignore))
            {
                if (!method.IsExtensionMethod)
                    continue;

                if (!TargetsMutableTextBuffer(method, targetSourceType))
                    continue;

                projected.Methods.Add(
                    ProjectMethod(
                        method,
                        implementationModel,
                        targetSourceType,
                        projectedBuilderType,
                        implementationLookup,
                        options));
            }

            return projected;
        }

        private static MethodModel ProjectMethod(
            MethodModel method,
            TypeModel implementationModel,
            string sourceType,
            string projectedBuilderType,
            IDictionary<string, string> implementationLookup,
            ProjectionOptions options)
        {
            var methodModel = new MethodModel
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
                                DocumentationRewriter.RewriteDocumentation(
                                    p.Documentation,
                                    implementationModel,
                                    sourceType,
                                    projectedBuilderType),

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
                    MergeSynchronizationDocumentation(
                        method.Documentation is null
                            ? null
                            : new DocumentationModel
                            {
                                SummaryXml =
                                    DocumentationRewriter.RewriteDocumentation(
                                        method.Documentation.SummaryXml,
                                        implementationModel,
                                        sourceType,
                                        projectedBuilderType),

                                RemarksXml =
                                    DocumentationRewriter.RewriteDocumentation(
                                        method.Documentation.RemarksXml,
                                        implementationModel,
                                        sourceType,
                                        projectedBuilderType),

                                ReturnsXml =
                                    DocumentationRewriter.RewriteDocumentation(
                                        method.Documentation.ReturnsXml,
                                        implementationModel,
                                        sourceType,
                                        projectedBuilderType),

                                SynchronizationNoteXml =
                                    DocumentationRewriter.RewriteDocumentation(
                                        method.Documentation.SynchronizationNoteXml,
                                        implementationModel,
                                        sourceType,
                                        projectedBuilderType)
                            },
                            includeSynchronizationNote: options.EmitSynchronizationNotes && method.SkipSynchronization),

                GenerateForwarder = method.GenerateForwarder,

                BodyText =
                    method.GenerateForwarder
                        ? null
                        : RewriteBody(
                            method.BodyText,
                            sourceType,
                            projectedBuilderType,
                            new HashSet<string>(implementationLookup.Values)),

                ForwardTarget = method.GenerateForwarder && implementationLookup.TryGetValue(method.Name, out string? targetName)
                    ? targetName
                    : null,

                ForwardTargetObject = method.GenerateForwarder
                    ? (projectedBuilderType == "MutableTextBuffer" ? null : "buffer")
                    : null,

                SkipSynchronization =
                    method.SkipSynchronization,
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
            string projectedBuilderType,
            ISet<string> extensionMethodTargetMethods)
        {
            if (string.IsNullOrWhiteSpace(body))
                return body;



            string result = body;

            foreach (string methodName in extensionMethodTargetMethods)
            {
                result =
                    result.Replace(
                        $"text.{methodName}(",
                        $"text.buffer.{methodName}(");
            }

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

        private static string RemoveSuffix(string name, string suffix)
        {
             return name.EndsWith(suffix, StringComparison.Ordinal)
                ? name[..^suffix.Length]
                : name;
        }
    }
}