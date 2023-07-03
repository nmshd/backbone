using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace AdminApi.Tests.Integration.Support;

public class JsonValidators
{
    private static readonly Dictionary<Type, JSchema> CachedSchemas = new();

    public static bool ValidateJsonSchema<T>(string json, out IList<string> errors)
    {
        if (CachedSchemas.TryGetValue(typeof(T), out var schema))
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
        CachedSchemas.Add(typeof(T), schema);
        var responseJson = JObject.Parse(json);
        return responseJson.IsValid(schema, out errors);
    }
}
