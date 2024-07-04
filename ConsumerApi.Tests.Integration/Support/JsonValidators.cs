using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NJsonSchema.NewtonsoftJson.Generation;
using JsonSchema = NJsonSchema.JsonSchema;
using JsonSchemaGenerator = NJsonSchema.Generation.JsonSchemaGenerator;

namespace Backbone.ConsumerApi.Tests.Integration.Support;

public class JsonValidators
{
    private static readonly Dictionary<Type, JsonSchema> CACHED_SCHEMAS = new();

    public static async Task<(bool IsValid, IList<string> Errors)> ValidateJsonSchema<T>(string json)
    {
        var errors = new List<string>();

        if (CACHED_SCHEMAS.TryGetValue(typeof(T), out var schema))
        {
            var parsedJson = JToken.Parse(json);
            var validationResults = schema.Validate(parsedJson);

            errors.AddRange(validationResults.Select(validationResult => validationResult.ToString()));

            return (validationResults.Count == 0, errors);
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

        schema = await JsonSchema.FromJsonAsync(generatedSchema);

        schema.AllowAdditionalProperties = true;

        CACHED_SCHEMAS.Add(typeof(T), schema);

        var responseJson = JToken.Parse(json);
        var validationErrors = schema.Validate(responseJson);

        errors.AddRange(validationErrors.Select(validationError => validationError.ToString()));

        return (validationErrors.Count == 0, errors);
    }
}
