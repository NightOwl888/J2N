using System.Text.Json;

namespace J2N.Text.CodeGen.Metadata
{
    public static class MetadataLoader
    {
        public static TypeModel Load(string path)
        {
            string json = File.ReadAllText(path);

            TypeModel? model =
                JsonSerializer.Deserialize<TypeModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (model is null)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize metadata file '{path}'.");
            }

            return model;
        }
    }
}