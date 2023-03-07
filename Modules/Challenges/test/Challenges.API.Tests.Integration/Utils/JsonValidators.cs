using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace Challenges.API.Tests.Integration.Extensions;
public class JsonValidators
{
    public static bool ValidateJsonSchema<T>(string json, out IList<string> errors)
    {
        var generator = new JSchemaGenerator
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var schemaJson = generator.Generate(typeof(T));
        var schema = JSchema.Parse(schemaJson.ToString());
        var responseJson = JObject.Parse(json);
        return responseJson.IsValid(schema, out errors);
    }
}
