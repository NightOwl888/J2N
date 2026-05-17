using System.Text.Json;

namespace J2N.Text.CodeGen.Metadata
{
    public static class MetadataLoader
    {
        public static TypeModel Load(string path)
        {
            string json = File.ReadAllText(path);

            var model =
                JsonSerializer.Deserialize<TypeModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (model is null)
                throw new InvalidOperationException("Failed to deserialize metadata JSON.");

            // validate immediately after deserialization
            TypeModelValidator.Validate(model);

            return model;
        }
    }
}
