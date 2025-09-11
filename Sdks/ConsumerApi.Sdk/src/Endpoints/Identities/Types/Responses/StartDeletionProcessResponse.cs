namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;

public class StartDeletionProcessResponse
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required DateTime GracePeriodEndsAt { get; set; }
}
