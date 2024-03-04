using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class ApproveDeletionProcessResponse
{
    public ApproveDeletionProcessResponse(IdentityDeletionProcess deletionProcess)
    {
        Id = deletionProcess.Id;
        Status = deletionProcess.Status;
        CreatedAt = deletionProcess.CreatedAt;
        ApprovedAt = deletionProcess.ApprovedAt ?? throw new Exception($"The '{nameof(IdentityDeletionProcess.ApprovedAt)}' property of the given deletion process must not be null.");
        ApprovedByDevice = deletionProcess.ApprovedByDevice ?? throw new Exception($"The '{nameof(IdentityDeletionProcess.ApprovedByDevice)}' property of the given deletion process must not be null.");
    }

    public string Id { get; }
    public DeletionProcessStatus Status { get; }
    public DateTime CreatedAt { get; }
    public DateTime ApprovedAt { get; }
    public string ApprovedByDevice { get; }
}
