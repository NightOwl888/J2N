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

                Parameters =
                    method.Parameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Name,

                            TypeName = RewriteType(
                                p.TypeName,
                                sourceType,
                                facadeName)
                        })
                        .ToList()
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

                IndexParameters =
                    property.IndexParameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Name,
                            TypeName = p.TypeName
                        })
                        .ToList()
            };
        }

        private static string RewriteType(
            string typeName,
            string sourceType,
            string facadeName)
        {
            return typeName.Replace(sourceType, facadeName);
        }
    }
}
