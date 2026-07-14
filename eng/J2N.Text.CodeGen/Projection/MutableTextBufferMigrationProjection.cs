using J2N.Text.CodeGen.Metadata;

namespace J2N.Text.CodeGen.Projection
{
    public sealed class MutableTextBufferMigrationProjection
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

            foreach (MethodModel method in extensionSource.Methods.Where(x => !x.Ignore))
            {
                if (method.IsStatic)
                    continue;

                if (!method.ReturnsSelf)
                    continue;

                projected.Methods.Add(
                    ProjectMethod(
                        method,
                        extensionSource,
                        implementationModel,
                        targetSourceType,
                        projectedBuilderType,
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
            ProjectionOptions options)
        {
            var parameters = new List<ParameterModel>
            {
                //new ParameterModel
                //{
                //    Name = "text",
                //    TypeName = projectedBuilderType,
                //    IsThis = true
                //}
                new ParameterModel
                {
                    Name = "text",
                    TypeName = "TBuilder",
                    Modifier = "this",
                    IsThis = true
                }
            };

            parameters.AddRange(
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

                            Attributes =
                                p.Attributes
                                    .Select(CloneAttribute)
                                    .ToList()
                        })
                );

            var methodModel = new MethodModel
            {
                Name = method.Name,

                ReturnType = "TBuilder",

                ReturnsSelf = method.ReturnsSelf,
                IsBuilderMethod = method.IsBuilderMethod,
                IsUnsafe = method.IsUnsafe,
                IsExtensionMethod = method.IsExtensionMethod,

                Parameters = parameters,

                GenericParameters =
                [
                    new GenericParameterModel
                    {
                        Name = "TBuilder",
                        Constraints =
                        {
                            projectedBuilderType
                        }
                    }
                ],

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
                            ForceBuilderReturns = true,

                            AdditionalTypeParameter =
                                new XmlDocumentationElementModel
                                {
                                    ElementName = "typeparam",
                                    InnerXml = "The type of the target builder.",
                                    Attributes =
                                    {
                                        ["name"] = "TBuilder"
                                    }
                                },

                            AdditionalThisParameter =
                                new XmlDocumentationElementModel
                                {
                                    ElementName = "param",
                                    InnerXml = "The target builder.",
                                    Attributes =
                                    {
                                        ["name"] = "text"
                                    }
                                },

                            RewriteCref =
                                cref =>
                                    RewriteMigrationCref(
                                        cref,
                                        extensionSource,
                                        implementationModel,
                                        projectedBuilderType)
                                        }),

                GenerateForwarder = true,

                BodyText = null,

                ForwardTarget = method.Name + "Internal",

                ForwardTargetObject = null,

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

        private static AttributeModel CloneAttribute(
            AttributeModel attribute)
        {
            return new AttributeModel
            {
                Name = attribute.Name,
                Arguments = attribute.Arguments.ToList()
            };
        }

        private static string RewriteMigrationCref(
            string cref,
            TypeModel extensionSource,
            TypeModel implementationModel,
            string projectedBuilderType)
        {
            //
            // Apply every existing rewrite EXCEPT the constructor rewrite.
            //
            cref = RewriteMigrationSelfReference(
                cref,
                extensionSource,
                projectedBuilderType);

            return cref;
        }

        private static string RewriteMigrationSelfReference(
            string cref,
            TypeModel source,
            string projectedBuilderType)
        {
            foreach (MethodModel method in source.Methods)
            {
                if (!method.ReturnsSelf)
                    continue;

                //
                // Append(...)
                //
                string shortPrefix =
                    method.Name + "(";

                if (cref.StartsWith(shortPrefix, StringComparison.Ordinal))
                {
                    return RewriteMigrationMethodSignature(
                        cref,
                        method.Name.Length,
                        projectedBuilderType);
                }

                //
                // MutableTextBuffer.Append(...)
                //
                string qualifiedPrefix =
                    source.SourceType + "." + method.Name;

                if (cref.StartsWith(qualifiedPrefix, StringComparison.Ordinal))
                {
                    return RewriteMigrationMethodSignature(
                        cref,
                        qualifiedPrefix.Length,
                        projectedBuilderType);
                }
            }

            return cref;
        }

        private static string RewriteMigrationMethodSignature(
            string cref,
            int methodNameLength,
            string projectedBuilderType)
        {
            int openParen =
                cref.IndexOf('(', methodNameLength);

            if (openParen < 0)
                return cref;

            //
            // Remove the source type qualifier if present.
            //
            int dot =
                cref.LastIndexOf('.', openParen);

            string methodName;

            if (dot >= 0)
            {
                methodName =
                    cref.Substring(
                        dot + 1,
                        openParen - dot - 1);
            }
            else
            {
                methodName =
                    cref.Substring(
                        0,
                        openParen);
            }

            string parameters =
                cref.Substring(openParen + 1);

            if (parameters.StartsWith(")", StringComparison.Ordinal))
            {
                return
                    methodName +
                    "{TBuilder}(TBuilder)";
            }

            return
                methodName +
                "{TBuilder}(TBuilder, " +
                parameters;
        }
    }
}