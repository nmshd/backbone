using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
public class RejectDeletionProcessResponse
{
    public RejectDeletionProcessResponse(IdentityDeletionProcess deletionProcess)
    {
        Id = deletionProcess.Id;
        Status = deletionProcess.Status;
        RejectedAt = deletionProcess.RejectedAt ?? throw new Exception($"The '{nameof(IdentityDeletionProcess.RejectedAt)}' property of the given deletion process must not be null.");
        RejectedByDevice = deletionProcess.RejectedByDevice ?? throw new Exception($"The '{nameof(IdentityDeletionProcess.RejectedByDevice)}' property of the given deletion process must not be null.");
    }

    public string Id { get; }
    public DeletionProcessStatus Status { get; }
    public DateTime RejectedAt { get; }
    public string RejectedByDevice { get; }
}
