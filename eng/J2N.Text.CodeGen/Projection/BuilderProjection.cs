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
                    DocumentationRewriter.RewriteDocumentation(
                        method.Documentation,
                        source,
                        source.Name,
                        facadeName,
                        new DocumentationRewriteOptions
                        {
                            IncludeSynchronizationNote = options.EmitSynchronizationNotes && method.SkipSynchronization,
                            ForceBuilderReturns = method.ReturnsSelf
                        }),

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
                    DocumentationRewriter.RewriteDocumentation(
                        property.Documentation,
                        source,
                        source.Name,
                        facadeName,
                        new DocumentationRewriteOptions
                        {
                            IncludeSynchronizationNote = options.EmitSynchronizationNotes && (property.SkipGetterSynchronization || property.SkipSetterSynchronization),
                            ForceBuilderReturns = false
                        }),

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
