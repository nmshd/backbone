using System.Text.Json;

namespace Backbone.ConsumerApi.Sdk;

public class Configuration
{
    public Configuration()
    {
        JsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public required string BaseUrl { get; set; }
    public required AuthenticationConfiguration Authentication { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }

    public class AuthenticationConfiguration
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
