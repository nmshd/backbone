using System.Text.Json;

namespace Backbone.ConsumerApi.Sdk;

public class Configuration
{
    public required string BaseUrl { get; init; }
    public string ApiVersion { get; init; } = "v1";
    public required AuthenticationConfiguration Authentication { get; init; }
    public JsonSerializerOptions JsonSerializerOptions { get; init; } = new() { PropertyNameCaseInsensitive = true };

    public class AuthenticationConfiguration
    {
        public required string ClientId { get; init; }
        public required string ClientSecret { get; init; }
        public required string Username { get; init; }
        public required string Password { get; init; }
    }

    public object CloneWith(AuthenticationConfiguration newAuthentication)
    {
        return new Configuration()
        {
            Authentication = newAuthentication,
            JsonSerializerOptions = JsonSerializerOptions,
            BaseUrl = null!,
            ApiVersion = null!
        };
    }
}
