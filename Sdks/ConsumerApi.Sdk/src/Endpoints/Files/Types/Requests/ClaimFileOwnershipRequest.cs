using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;

public class ClaimFileOwnershipRequest
{
    [JsonConstructor]
    public ClaimFileOwnershipRequest(string fileOwnershipToken)
    {
        FileOwnershipToken = fileOwnershipToken;
    }

    [JsonPropertyName("Value")]
    public string FileOwnershipToken { get; init; }
}
