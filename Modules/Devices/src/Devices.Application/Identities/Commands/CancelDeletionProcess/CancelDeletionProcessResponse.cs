 using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;
public class CancelDeletionProcessResponse
{
    public CancelDeletionProcessResponse(IdentityDeletionProcess deletionProcess)
    {
        Id = deletionProcess.Id;
        Status = deletionProcess.Status;
        CancelledAt = deletionProcess.CancelledAt ?? throw new Exception($"The '{nameof(IdentityDeletionProcess.CancelledAt)}' property of the given deletion process must not be null.");
        CancelledByDevice = deletionProcess.CancelledByDevice ?? throw new Exception($"The '{nameof(IdentityDeletionProcess.CancelledByDevice)}' property of the given deletion process must not be null.");
    }

    public string Id { get; }
    public DeletionProcessStatus Status { get; }
    public DateTime CancelledAt { get; }
    public string CancelledByDevice { get; }
}
