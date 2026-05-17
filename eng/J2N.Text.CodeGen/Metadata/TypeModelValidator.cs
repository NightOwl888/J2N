using System.Text;

namespace J2N.Text.CodeGen.Metadata
{
    public static class TypeModelValidator
    {
        public static void Validate(TypeModel model)
        {
            var errors = new List<string>();

            // --- Root validation ---
            if (string.IsNullOrWhiteSpace(model.Namespace))
                errors.Add("TypeModel.Namespace is required.");

            if (string.IsNullOrWhiteSpace(model.Name))
                errors.Add("TypeModel.Name is required.");

            if (string.IsNullOrWhiteSpace(model.SourceType))
                errors.Add("TypeModel.SourceType is required.");

            // --- Methods ---
            if (model.Methods is not null)
            {
                for (int i = 0; i < model.Methods.Count; i++)
                {
                    var m = model.Methods[i];

                    if (string.IsNullOrWhiteSpace(m.Name))
                        errors.Add($"Methods[{i}].Name is required.");

                    if (string.IsNullOrWhiteSpace(m.ReturnType))
                        errors.Add($"Methods[{i}].ReturnType is required.");

                    if (m.Parameters is null)
                        errors.Add($"Methods[{i}].Parameters cannot be null.");
                }
            }

            // --- Properties (including indexers) ---
            if (model.Properties is not null)
            {
                for (int i = 0; i < model.Properties.Count; i++)
                {
                    var p = model.Properties[i];

                    if (string.IsNullOrWhiteSpace(p.Name))
                        errors.Add($"Properties[{i}].Name is required.");

                    if (string.IsNullOrWhiteSpace(p.TypeName))
                        errors.Add($"Properties[{i}].TypeName is required.");

                    // Indexer validation
                    if (p.IsIndexer)
                    {
                        if (p.IndexParameters is null || p.IndexParameters.Count == 0)
                        {
                            errors.Add($"Properties[{i}] is an indexer but has no IndexParameters.");
                        }
                    }
                    else
                    {
                        if (p.IndexParameters is { Count: > 0 })
                        {
                            errors.Add($"Properties[{i}] is not an indexer but defines IndexParameters.");
                        }
                    }
                }
            }

            // --- Fail fast ---
            if (errors.Count > 0)
            {
                var sb = new StringBuilder();

                sb.AppendLine("TypeModel validation failed:");

                foreach (var error in errors)
                    sb.AppendLine(" - " + error);

                throw new InvalidOperationException(sb.ToString());
            }
        }
    }
}
