using J2N.Text.CodeGen.Metadata;

namespace J2N.Text.CodeGen.Projection
{
    public static class ProjectionEngine
    {
        public static ProjectedTypeModel Project(TypeModel model, ProjectionProfile profile)
        {
            var projectedMethods =
                model.Methods
                    .Where(m => !profile.ExcludedMembers.Contains(m.Name))
                    .ToList();

            var projectedProperties =
                model.Properties
                    .Where(p => !profile.ExcludedMembers.Contains(p.Name))
                    .ToList();

            return new ProjectedTypeModel
            {
                Source = model,

                Namespace = model.Namespace,
                Name = model.Name,

                Methods = projectedMethods,
                Properties = projectedProperties
            };
        }
    }
}
