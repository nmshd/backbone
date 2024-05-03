using System.Text.Json;
using Backbone.Tooling.JsonConverters;

namespace Backbone.AdminApi.Sdk;

public class Configuration
{
    public Configuration()
    {
        JsonSerializerOptions.Converters.Add(new UrlSafeBase64ToByteArrayJsonConverter());
    }

    public required string ApiKey { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };
}
