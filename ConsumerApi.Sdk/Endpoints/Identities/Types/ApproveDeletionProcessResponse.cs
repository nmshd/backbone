namespace Backbone.ConsumerApi.Sdk.Endpoints.Identities.Types;

public class ApproveDeletionProcessResponse
{
    public required string Id { get; set; }
    public required string Status { get; set;  }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ApprovedAt { get; set; }
    public required string ApprovedByDevice { get; set; }
}
