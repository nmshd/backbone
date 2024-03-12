using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcesses;

public class GetDeletionProcessesResponse : CollectionResponseBase<IdentityDeletionProcessDTO>
{
    public GetDeletionProcessesResponse(IEnumerable<IdentityDeletionProcess> processes)
        : base((IEnumerable<IdentityDeletionProcessDTO>)processes.Select(p => new IdentityDeletionProcessAsSupportDTO(p)))
    {
    }
}
