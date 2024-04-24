using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NJsonSchema.NewtonsoftJson.Generation;
using JsonSchemaGenerator = NJsonSchema.Generation.JsonSchemaGenerator;

namespace Backbone.BuildingBlocks.SDK.Endpoints.Common.Validators;

public class JsonValidators
{
    private static readonly Dictionary<Type, JSchema> CACHED_SCHEMAS = new();

    public static bool ValidateJsonSchema<T>(string json, out IList<string> errors)
    {
        if (CACHED_SCHEMAS.TryGetValue(typeof(T), out var schema))
        {
            var parsedJson = JObject.Parse(json);
            return parsedJson.IsValid(schema, out errors);
        }

        var settings = new NewtonsoftJsonSchemaGeneratorSettings();

        var generator = new JsonSchemaGenerator(settings);

        var schemaJson = generator.Generate(typeof(T));

        var generatedSchema = schemaJson.ToJson();

        schema = JSchema.Parse(generatedSchema);

        schema.AllowAdditionalProperties = true;

        CACHED_SCHEMAS.Add(typeof(T), schema);

        try
        {
            var responseJson = JObject.Parse(json);
            return responseJson.IsValid(schema, out errors);
        }
        catch (JsonReaderException)
        {
            var responseJson = JArray.Parse(json);
            return responseJson.IsValid(schema, out errors);
        }
    }
}
