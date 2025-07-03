namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;

public class GetOwnIdentityResponse
{
    public required string Address { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required byte IdentityVersion { get; set; }
    public required string TierId { get; set; }
    public required string Status { get; set; }
    public DateTime? DeletionGracePeriodEndsAt { get; set; }
}
