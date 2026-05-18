using J2N.Text.CodeGen.Metadata;

namespace J2N.Text.CodeGen.Projection
{
    public sealed class BuilderProjection
    {
        public ProjectedTypeModel Project(
            TypeModel source,
            string facadeNamespace,
            string facadeName)
        {
            var projected = new ProjectedTypeModel
            {
                Source = source,
                Namespace = facadeNamespace,
                Name = facadeName
            };

            foreach (MethodModel method in source.Methods)
            {
                if (!ShouldIncludeMethod(method))
                {
                    continue;
                }

                projected.Methods.Add(
                    ProjectMethod(
                        method,
                        source,
                        source.SourceType,
                        facadeName));
            }

            foreach (PropertyModel property in source.Properties)
            {
                projected.Properties.Add(
                    ProjectProperty(
                        property,
                        source,
                        source.SourceType,
                        facadeName));
            }

            return projected;
        }

        private static MethodModel ProjectMethod(
            MethodModel method,
            TypeModel source,
            string sourceType,
            string facadeName)
        {
            return new MethodModel
            {
                Name = method.Name,

                ReturnType =
                    RewriteType(
                        method.ReturnType,
                        sourceType,
                        facadeName),

                ReturnsSelf =
                    method.ReturnType == sourceType,
                IsBuilderMethod =
                    method.ReturnType == sourceType,
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
                                    source.Name,
                                    facadeName),

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
                    method.Documentation is null
                        ? null
                        : new DocumentationModel
                        {
                            SummaryXml =
                                RewriteDocumentation(
                                    method.Documentation.SummaryXml,
                                    source.Name,
                                    facadeName),

                            RemarksXml =
                                RewriteDocumentation(
                                    method.Documentation.RemarksXml,
                                    source.Name,
                                    facadeName),

                            ReturnsXml =
                                RewriteDocumentation(
                                    method.Documentation.ReturnsXml,
                                    source.Name,
                                    facadeName)
                        },
            };
        }

        private static PropertyModel ProjectProperty(
            PropertyModel property,
            TypeModel source,
            string sourceType,
            string facadeName)
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
                                RewriteDocumentation(
                                    p.Documentation,
                                    source.Name,
                                    facadeName),

                            Modifier = p.Modifier,
                            IsThis = p.IsThis,
                        })
                        .ToList(),

                Attributes =
                    property.Attributes
                        .Select(CloneAttribute)
                        .ToList(),

                Documentation =
                    property.Documentation is null
                        ? null
                        : new DocumentationModel
                        {
                            SummaryXml =
                                RewriteDocumentation(
                                    property.Documentation.SummaryXml,
                                    source.Name,
                                    facadeName),

                            RemarksXml =
                                RewriteDocumentation(
                                    property.Documentation.RemarksXml,
                                    source.Name,
                                    facadeName),

                            ReturnsXml =
                                RewriteDocumentation(
                                    property.Documentation.ReturnsXml,
                                    source.Name,
                                    facadeName)
                        },
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
            string sourceType,
            string facadeType)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return xml;

            return xml.Replace(sourceType, facadeType);
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

        private static bool ShouldIncludeMethod(MethodModel method)
        {
            if (method.Name == "GetChunks")
            {
                return false;
            }

            return true;
        }
    }
}
