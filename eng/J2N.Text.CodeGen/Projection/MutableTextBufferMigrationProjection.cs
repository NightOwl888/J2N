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
                        implementationModel,
                        targetSourceType,
                        projectedBuilderType,
                        options));
            }

            return projected;
        }

        private static MethodModel ProjectMethod(
            MethodModel method,
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
                    TypeName = "T",
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
                        })
                );

            var methodModel = new MethodModel
            {
                Name = method.Name,

                //ReturnType =
                //    RewriteReturnType(
                //        method,
                //        options.PreserveSelfTypeGenerics,
                //        sourceType,
                //        projectedBuilderType),

                ReturnType = "T",

                ReturnsSelf = method.ReturnsSelf,
                IsBuilderMethod = method.IsBuilderMethod,
                IsUnsafe = method.IsUnsafe,
                IsExtensionMethod = method.IsExtensionMethod,

                Parameters = parameters,

                //GenericParameters =
                //    options.PreserveSelfTypeGenerics
                //        ? method.GenericParameters
                //            .Select(p =>
                //                RewriteGenericParameter(
                //                    p,
                //                    sourceType,
                //                    projectedBuilderType))
                //            .ToList()
                //        : [],

                GenericParameters =
                [
                    new GenericParameterModel
                    {
                        Name = "T",
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

                //Documentation =
                //    DocumentationRewriter.RewriteDocumentation(
                //        method.Documentation,
                //        implementationModel,
                //        sourceType,
                //        projectedBuilderType,
                //        new DocumentationRewriteOptions
                //        {
                //            IncludeSynchronizationNote = options.EmitSynchronizationNotes && method.SkipSynchronization,
                //            ForceBuilderReturns = method.ReturnsSelf
                //        }),

                //Documentation =
                //    DocumentationRewriter.RewriteDocumentation(
                //        method.Documentation,
                //        implementationModel,
                //        sourceType,
                //        projectedBuilderType,
                //        new DocumentationRewriteOptions
                //        {
                //            ForceBuilderReturns = true,

                //            AdditionalThisParameter =
                //                new XmlDocumentationElementModel
                //                {
                //                    ElementName = "param",
                //                    InnerXml = "The target builder.",
                //                    Attributes =
                //                    {
                //                        ["name"] = "text"
                //                    }
                //                }
                //        }),

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
                                        ["name"] = "T"
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
                                }
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
    }
}