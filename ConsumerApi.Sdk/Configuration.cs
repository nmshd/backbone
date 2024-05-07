using System.Text.Json;
using Backbone.ConsumerApi.Sdk.Authentication;

namespace Backbone.ConsumerApi.Sdk;

public class Configuration
{
    public required AuthenticationConfiguration Authentication { get; init; }
    public JsonSerializerOptions JsonSerializerOptions { get; init; } = new() { PropertyNameCaseInsensitive = true };

    public class AuthenticationConfiguration
    {
        public required ClientCredentials ClientCredentials { get; set; }
        public required UserCredentials? UserCredentials { get; set; }
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
