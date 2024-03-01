using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleDeletionProcesses;
public class CancelStaleIdentityDeletionProcessesResponse
{
    public List<IdentityDeletionProcessId> CanceledIdentityDeletionPrecessIds { get; set; } = [];
}
