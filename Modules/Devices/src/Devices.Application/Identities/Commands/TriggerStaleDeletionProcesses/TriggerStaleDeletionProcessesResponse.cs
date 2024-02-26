using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerStaleDeletionProcesses;
public class TriggerStaleDeletionProcessesResponse
{
    public TriggerStaleDeletionProcessesResponse(IdentityDeletionProcess deletionProcess)
    {
        Status = deletionProcess.Status;
        Id = deletionProcess.Id;
    }

    public DeletionProcessStatus Status { get; set; }
    public string Id { get; set; }
}
