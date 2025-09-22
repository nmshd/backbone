namespace Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;

public class ClaimFileOwnershipRequest
{
    public required string OwnershipToken { get; init; }
}
