namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;

public class CreateIdentityResponse
{
    public required string Address { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required CreateIdentityResponseDevice Device { get; set; }
}

public class CreateIdentityResponseDevice
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
}
