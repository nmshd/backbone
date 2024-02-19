namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class StartDeletionProcessResponse
{
    public string Id { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime ApprovedAt { get; set; }
    public string ApprovedByDevice { get; set; }

    public DateTime GracePeriodEndsAt { get; set; }
}
