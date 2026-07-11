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
                        extensionSource,
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
            TypeModel extensionSource,
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
                            ForceBuilderReturns = method.ReturnsSelf,
                            PreserveTypeParameterDocumentation = options.PreserveSelfTypeGenerics,
                            RewriteCref =
                                options.PreserveSelfTypeGenerics
                                    ? (cref) =>
                                        RewriteNonGenericExtensionMethodCref(
                                            cref,
                                            extensionSource,
                                            implementationModel,
                                            projectedBuilderType)
                                    : (cref) =>
                                        RewriteGenericExtensionMethodCref(
                                            cref,
                                            extensionSource,
                                            implementationModel,
                                            projectedBuilderType),
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

        public static string RewriteNonGenericExtensionMethodCref(
            string cref,
            TypeModel extensionSource,
            TypeModel implementationModel,
            string facadeType)
        {
            cref = DocumentationRewriter.RewriteCrefTarget(cref, implementationModel, facadeType);
            cref = DocumentationRewriter.RewriteCrefTarget(cref, extensionSource, facadeType);
            return cref;
        }

        public static string RewriteGenericExtensionMethodCref(
            string cref,
            TypeModel extensionSource,
            TypeModel implementationModel,
            string facadeType)
        {
            cref = DocumentationRewriter.RewriteCrefTarget(cref, implementationModel, facadeType);
            cref = DocumentationRewriter.RewriteCrefTarget(cref, extensionSource, facadeType);

            foreach (MethodModel method in extensionSource.Methods)
            {
                //
                // New rule:
                //
                // Insert{TBuilder}(TBuilder,...)
                // ->
                // Insert(TextBuilder,...)
                //
                if (!method.IsExtensionMethod)
                    continue;

                string genericMethod =
                    method.Name + "{";

                string qualifiedGenericMethod =
                    extensionSource.SourceType + "." + method.Name + "{";

                if (cref.StartsWith(genericMethod, StringComparison.Ordinal))
                {
                    return RewriteGenericExtensionMethodSignature(
                        cref,
                        method.Name.Length,
                        facadeType);
                }

                if (cref.StartsWith(qualifiedGenericMethod, StringComparison.Ordinal))
                {
                    return RewriteGenericExtensionMethodSignature(
                        cref,
                        qualifiedGenericMethod.Length - 1,
                        facadeType);
                }
            }

            return cref;
        }

        private static string RewriteGenericExtensionMethodSignature(
            string cref,
            int methodNameLength,
            string facadeType)
        {
            //
            // Find the end of the generic argument list.
            //
            int genericEnd =
                cref.IndexOf('}', methodNameLength);

            if (genericEnd < 0)
                return cref;

            //
            // Strip the generic argument list.
            //
            string rewritten =
                cref.Remove(
                    methodNameLength,
                    genericEnd - methodNameLength + 1);

            //
            // Rewrite only the first parameter.
            //
            int openParen =
                rewritten.IndexOf('(');

            if (openParen < 0)
                return rewritten;

            int firstComma =
                rewritten.IndexOf(',', openParen + 1);

            int closeParen =
                rewritten.IndexOf(')', openParen + 1);

            int endOfFirstParameter;

            if (firstComma >= 0)
            {
                endOfFirstParameter = firstComma;
            }
            else
            {
                endOfFirstParameter = closeParen;
            }

            if (endOfFirstParameter < 0)
                return rewritten;

            return
                rewritten.Substring(0, openParen + 1) +
                facadeType +
                rewritten.Substring(endOfFirstParameter);
        }
    }
}