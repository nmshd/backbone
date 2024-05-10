using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using NJsonSchema.NewtonsoftJson.Generation;
using JsonSchemaGenerator = NJsonSchema.Generation.JsonSchemaGenerator;

namespace Backbone.AdminApi.Tests.Integration.Support;

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

        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        var generator = new JsonSchemaGenerator(settings);

        var schemaJson = generator.Generate(typeof(T));

        var generatedSchema = schemaJson.ToJson();

        schema = JSchema.Parse(generatedSchema);

        schema.AllowAdditionalProperties = true;

        CACHED_SCHEMAS.Add(typeof(T), schema);

        var responseJson = JObject.Parse(json);

        return responseJson.IsValid(schema, out errors);
    }
}
