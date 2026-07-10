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
                    RewriteReturnType(
                        method,
                        options.PreserveSelfTypeGenerics,
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
                                RewriteParameterType(
                                    p,
                                    method,
                                    options.PreserveSelfTypeGenerics,
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
                    options.PreserveSelfTypeGenerics
                        ? method.GenericParameters
                            .Select(p =>
                                RewriteGenericParameter(
                                    p,
                                    sourceType,
                                    projectedBuilderType))
                            .ToList()
                        : [],

                Attributes =
                    method.Attributes
                        .Select(CloneAttribute)
                        .ToList(),

                Documentation =
                    DocumentationRewriter.RewriteDocumentation(
                        method.Documentation,
                        implementationModel,
                        sourceType,
                        projectedBuilderType,
                        new DocumentationRewriteOptions
                        {
                            IncludeSynchronizationNote = options.EmitSynchronizationNotes && method.SkipSynchronization,
                            ForceBuilderReturns = method.ReturnsSelf
                        }),

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

            // Generic type is exactly sourceType or its nullable counterpart
            if (IsSourceTypeMatch(first.TypeName, sourceType))
                return true;

            // Generic form:
            //
            // this T text where T : MutableTextBuffer
            //
            GenericParameterModel? genericParameter =
                method.GenericParameters.FirstOrDefault(
                    p => p.Name == first.TypeName);

            if (genericParameter is null)
            {
                return false;
            }

            return genericParameter.Constraints.Any(c => IsSourceTypeMatch(c, sourceType));
        }

        private static bool IsSourceTypeMatch(
            string typeName,
            string sourceType)
        {
            return typeName == sourceType
                || typeName == sourceType + "?";
        }

        private static string RewriteType(
            string typeName,
            string sourceType,
            string projectedBuilderType)
        {
            return typeName.Replace(sourceType, projectedBuilderType);
        }

        private static string RewriteParameterType(
            ParameterModel parameter,
            MethodModel method,
            bool preserveSelfTypeGenerics,
            string sourceType,
            string projectedBuilderType)
        {
            if (preserveSelfTypeGenerics)
            {
                return RewriteType(
                    parameter.TypeName,
                    sourceType,
                    projectedBuilderType);
            }

            GenericParameterModel? generic =
                method.GenericParameters.FirstOrDefault(
                    p => p.Name == parameter.TypeName);

            if (generic is not null)
            {
                return projectedBuilderType;
            }

            return RewriteType(
                parameter.TypeName,
                sourceType,
                projectedBuilderType);
        }

        private static string RewriteReturnType(
            MethodModel method,
            bool preserveSelfTypeGenerics,
            string sourceType,
            string projectedBuilderType)
        {
            if (preserveSelfTypeGenerics)
            {
                return RewriteType(
                    method.ReturnType,
                    sourceType,
                    projectedBuilderType);
            }

            if (method.GenericParameters.Any(p => p.Name == method.ReturnType))
            {
                return projectedBuilderType;
            }

            return RewriteType(
                method.ReturnType,
                sourceType,
                projectedBuilderType);
        }

        private static GenericParameterModel RewriteGenericParameter(
            GenericParameterModel parameter,
            string sourceType,
            string projectedBuilderType)
        {
            return new GenericParameterModel
            {
                Name = parameter.Name,

                Constraints =
                    parameter.Constraints
                        .Select(c =>
                            RewriteType(
                                c,
                                sourceType,
                                projectedBuilderType))
                        .ToList()
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

        private static string RemoveSuffix(string name, string suffix)
        {
             return name.EndsWith(suffix, StringComparison.Ordinal)
                ? name[..^suffix.Length]
                : name;
        }
    }
}