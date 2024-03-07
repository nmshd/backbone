namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types.Responses;

public class StartDeletionProcessResponse
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime ApprovedAt { get; set; }
    public required string ApprovedByDevice { get; set; }

    public DateTime GracePeriodEndsAt { get; set; }
}
