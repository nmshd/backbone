using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcesses;
public class GetDeletionProcessesResponse
{
    public GetDeletionProcessesResponse(Identity identity)
    {
        DeletionProcesses = identity.DeletionProcesses
            .Select(p => new IdentityDeletionProcessDTO(p))
            .ToList();
    }

    public List<IdentityDeletionProcessDTO> DeletionProcesses { get; set; }
}
