using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerStaleDeletionProcesses;
public class TriggerStaleDeletionProcessesResponse
{
    public List<IdentityDeletionProcess> IdentityDeletionProcesses { get; set; } = [];
}
