using System.Text.Json;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.Tooling.JsonConverters;

namespace Backbone.ConsumerApi.Sdk;

public class Configuration
{
    public Configuration()
    {
        JsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        JsonSerializerOptions.Converters.Add(new UrlSafeBase64ToByteArrayJsonConverter());
        JsonSerializerOptions.Converters.Add(new ApiErrorData.JsonConverter());
    }

    public required AuthenticationConfiguration Authentication { get; init; }
    public JsonSerializerOptions JsonSerializerOptions { get; init; }

    public class AuthenticationConfiguration
    {
        public required ClientCredentials ClientCredentials { get; set; }
        public required UserCredentials? UserCredentials { get; set; }
    }
}
