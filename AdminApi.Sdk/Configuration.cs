using System.Text.Json;

namespace Backbone.AdminApi.Sdk;

public class Configuration
{
    public required string BaseUrl { get; set; }
    public required string ApiKey { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };
}
