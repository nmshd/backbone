using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;

public class ValidateFileOwnershipTokenRequest
{
    [JsonConstructor]
    public ValidateFileOwnershipTokenRequest(string fileOwnershipToken)
    {
        FileOwnershipToken = fileOwnershipToken;
    }

    [JsonPropertyName("Value")]
    public string FileOwnershipToken { get; init; }
}
