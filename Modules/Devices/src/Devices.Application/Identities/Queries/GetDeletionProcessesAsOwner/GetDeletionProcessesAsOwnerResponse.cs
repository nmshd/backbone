using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAsOwner;

public class GetDeletionProcessesAsOwnerResponse : CollectionResponseBase<IdentityDeletionProcessOverviewDTO>
{
    public GetDeletionProcessesAsOwnerResponse(IEnumerable<IdentityDeletionProcess> processes)
        : base(processes.Select(p => new IdentityDeletionProcessOverviewDTO(p)))
    {
    }
}
