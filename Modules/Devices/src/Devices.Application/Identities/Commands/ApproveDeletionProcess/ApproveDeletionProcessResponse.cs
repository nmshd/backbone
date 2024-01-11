using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class ApproveDeletionProcessResponse
{
    public ApproveDeletionProcessResponse(IdentityDeletionProcess deletionProcess)
    {
        Id = deletionProcess.Id;
        Status = deletionProcess.Status;
        CreatedAt = deletionProcess.CreatedAt;
        ApprovedAt = deletionProcess.ApprovedAt!.Value;
        ApprovedByDevice = deletionProcess.ApprovedByDevice;
    }

    public string Id { get; }
    public DeletionProcessStatus Status { get; set; }
    public DateTime CreatedAt { get; }
    public DateTime ApprovedAt { get; set; }
    public string ApprovedByDevice { get; set; }
}
