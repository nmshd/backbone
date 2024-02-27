using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
public class RejectDeletionProcessResponse
{
    public RejectDeletionProcessResponse(IdentityDeletionProcess deletionProcess)
    {
        Id = deletionProcess.Id;
        Status = deletionProcess.Status;
        CreatedAt = deletionProcess.CreatedAt;
        RejectedByDevice = deletionProcess.RejectedByDevice;
    }

    public string Id { get; }
    public DeletionProcessStatus Status { get; }
    public DateTime CreatedAt { get; }
    public string RejectedByDevice { get; }
}
