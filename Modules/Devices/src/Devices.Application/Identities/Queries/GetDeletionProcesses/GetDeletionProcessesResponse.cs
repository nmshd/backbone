using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcesses;
public class GetDeletionProcessesResponse
{
    public GetDeletionProcessesResponse(IReadOnlyList<IdentityDeletionProcess> processes)
    {
        DeletionProcesses = processes
            .Select(p => new IdentityDeletionProcessDTO(p))
            .ToList();
    }

    public List<IdentityDeletionProcessDTO> DeletionProcesses { get; set; }
}
