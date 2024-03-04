using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;

public class StartDeletionProcessAsOwnerResponse
{
    public StartDeletionProcessAsOwnerResponse(IdentityDeletionProcess deletionProcess)
    {
        Id = deletionProcess.Id;
        Status = deletionProcess.Status;
        CreatedAt = deletionProcess.CreatedAt;
        ApprovedAt = deletionProcess.ApprovedAt ?? throw new Exception($"The '{nameof(IdentityDeletionProcess.ApprovedAt)}' property of the given deletion process must not be null.");
        ApprovedByDevice = deletionProcess.ApprovedByDevice ?? throw new Exception($"The '{nameof(IdentityDeletionProcess.ApprovedByDevice)}' property of the given deletion process must not be null.");
        GracePeriodEndsAt = deletionProcess.GracePeriodEndsAt.GetValueOrDefault();
    }

    public string Id { get; set; }
    public DeletionProcessStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime ApprovedAt { get; set; }
    public DeviceId ApprovedByDevice { get; set; }

    public DateTime GracePeriodEndsAt { get; set; }
}
