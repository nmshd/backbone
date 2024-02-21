namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class StartDeletionProcessResponse
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required DateTime ApprovedAt { get; set; }
    public required string ApprovedByDevice { get; set; }

    public required DateTime GracePeriodEndsAt { get; set; }
}
