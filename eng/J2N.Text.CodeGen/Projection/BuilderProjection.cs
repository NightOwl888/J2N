using J2N.Text.CodeGen.Metadata;
using System.ComponentModel.DataAnnotations;

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
                projected.Methods.Add(ProjectMethod(method, source.SourceType, facadeName));
            }

            foreach (PropertyModel property in source.Properties)
            {
                projected.Properties.Add(ProjectProperty(property));
            }

            return projected;
        }

        private static MethodModel ProjectMethod(
            MethodModel method,
            string sourceType,
            string facadeName)
        {
            return new MethodModel
            {
                Name = method.Name,

                ReturnType = RewriteType(
                    method.ReturnType,
                    sourceType: sourceType,
                    facadeName),

                ReturnsSelf = method.ReturnsSelf,
                IsBuilderMethod = method.IsBuilderMethod,
                IsUnsafe = method.IsUnsafe,

                Parameters =
                    method.Parameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Name,

                            TypeName = RewriteType(
                                p.TypeName,
                                sourceType,
                                facadeName),

                            Documentation = p.Documentation,
                        })
                        .ToList(),

                GenericParameters =
                    method.GenericParameters
                        .Select(CloneGenericParameter)
                        .ToList(),

                Attributes = method.Attributes
                    .Select(CloneAttribute)
                    .ToList(),

                Documentation = method.Documentation,
            };
        }

        private static PropertyModel ProjectProperty(
            PropertyModel property)
        {
            return new PropertyModel
            {
                Name = property.Name,
                TypeName = property.TypeName,
                HasGetter = property.HasGetter,
                HasSetter = property.HasSetter,
                IsIndexer = property.IsIndexer,
                IsUnsafe = property.IsUnsafe,

                IndexParameters =
                    property.IndexParameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Name,
                            TypeName = p.TypeName,
                            Documentation = p.Documentation,
                        })
                        .ToList(),

                Attributes = property.Attributes
                    .Select(CloneAttribute)
                    .ToList(),

                Documentation = property.Documentation,
            };
        }

        private static string RewriteType(
            string typeName,
            string sourceType,
            string facadeName)
        {
            return typeName.Replace(sourceType, facadeName);
        }

        private static AttributeModel CloneAttribute(AttributeModel attribute)
        {
            return new AttributeModel
            {
                Name = attribute.Name,
                Arguments = attribute.Arguments.ToList()
            };
        }

        private static GenericParameterModel CloneGenericParameter(GenericParameterModel parameter)
        {
            return new GenericParameterModel
            {
                Name = parameter.Name,
                Constraints = parameter.Constraints.ToList()
            };
        }
    }
}
