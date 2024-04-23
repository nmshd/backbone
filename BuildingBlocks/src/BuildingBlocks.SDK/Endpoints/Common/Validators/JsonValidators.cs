using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

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

        var generator = new JSchemaGenerator
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var schemaJson = generator.Generate(typeof(T));
        schema = JSchema.Parse(schemaJson.ToString());
        CACHED_SCHEMAS.Add(typeof(T), schema);
        var responseJson = JObject.Parse(json);
        return responseJson.IsValid(schema, out errors);
    }
}
