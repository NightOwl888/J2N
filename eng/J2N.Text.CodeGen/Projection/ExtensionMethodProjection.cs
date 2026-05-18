using J2N.Text.CodeGen.Metadata;

namespace J2N.Text.CodeGen.Projection
{
    public sealed class ExtensionMethodProjection
    {
        public ProjectedTypeModel Project(
            TypeModel extensionSource,
            string targetSourceType,
            string facadeNamespace,
            string facadeType)
        {
            var projected = new ProjectedTypeModel
            {
                Source = extensionSource,
                Namespace = facadeNamespace,
                Name = extensionSource.Name
            };

            foreach (MethodModel method in extensionSource.Methods)
            {
                if (!method.IsExtensionMethod)
                    continue;

                if (!TargetsMutableTextBuffer(method, targetSourceType))
                    continue;

                projected.Methods.Add(
                    ProjectMethod(
                        method,
                        targetSourceType,
                        facadeType));
            }

            return projected;
        }

        private static MethodModel ProjectMethod(
            MethodModel method,
            string sourceType,
            string facadeType)
        {
            return new MethodModel
            {
                Name = method.Name,

                ReturnType =
                    RewriteType(
                        method.ReturnType,
                        sourceType,
                        facadeType),

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
                                    facadeType),

                            SourceTypeName = p.SourceTypeName,

                            Documentation =
                                RewriteDocumentation(
                                    p.Documentation,
                                    sourceType,
                                    facadeType),

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
                                    sourceType,
                                    facadeType),

                            RemarksXml =
                                RewriteDocumentation(
                                    method.Documentation.RemarksXml,
                                    sourceType,
                                    facadeType),

                            ReturnsXml =
                                RewriteDocumentation(
                                    method.Documentation.ReturnsXml,
                                    sourceType,
                                    facadeType)
                        },

                BodyText =
                    RewriteBody(
                        method.BodyText,
                        sourceType,
                        facadeType)
            };
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
            string facadeType)
        {
            return typeName.Replace(sourceType, facadeType);
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

        private static string? RewriteBody(
            string? body,
            string sourceType,
            string facadeType)
        {
            if (string.IsNullOrWhiteSpace(body))
                return body;

            string result = body;

            result =
                result.Replace(
                    sourceType,
                    facadeType);

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