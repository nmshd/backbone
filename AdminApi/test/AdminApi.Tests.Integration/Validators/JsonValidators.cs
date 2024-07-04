using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using JsonSchemaGenerator = NJsonSchema.Generation.JsonSchemaGenerator;

namespace Backbone.AdminApi.Tests.Integration.Validators;

public class JsonValidators
{
    private static readonly Dictionary<Type, JsonSchema> CACHED_SCHEMAS = new();

    public static async Task<(bool IsValid, IList<string> Errors)> ValidateJsonSchema<T>(string json)
    {
        var errors = new List<string>();

        if (CACHED_SCHEMAS.TryGetValue(typeof(T), out var schema))
        {
            try
            {
                var parsedJson = JObject.Parse(json);
                var validationResults = schema.Validate(parsedJson);

                errors.AddRange(validationResults.Select(validationResult => validationResult.ToString()));

                return (validationResults.Count == 0, errors);
            }
            catch (JsonReaderException)
            {
                var parsedJson = JArray.Parse(json);
                var validationResults = schema.Validate(parsedJson);

                errors.AddRange(validationResults.Select(validationResult => validationResult.ToString()));

                return (validationResults.Count == 0, errors);
            }
        }

        var settings = new NewtonsoftJsonSchemaGeneratorSettings();

        var generator = new JsonSchemaGenerator(settings);

        var schemaJson = generator.Generate(typeof(T));

        var generatedSchema = schemaJson.ToJson();

        schema = await JsonSchema.FromJsonAsync(generatedSchema);

        schema.AllowAdditionalProperties = true;

        CACHED_SCHEMAS.Add(typeof(T), schema);

        try
        {
            var responseJson = JObject.Parse(json);
            var validationResults = schema.Validate(responseJson);

            errors.AddRange(validationResults.Select(validationResult => validationResult.ToString()));

            return (validationResults.Count == 0, errors);
        }
        catch (JsonReaderException)
        {
            var responseJson = JArray.Parse(json);
            var validationResults = schema.Validate(responseJson);

            errors.AddRange(validationResults.Select(validationResult => validationResult.ToString()));

            return (validationResults.Count == 0, errors);
        }
    }
}
