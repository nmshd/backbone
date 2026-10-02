using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsOwner;

public class ListDeletionProcessesAsOwnerResponse : CollectionResponseBase<IdentityDeletionProcessOverviewDTO>
{
    public ListDeletionProcessesAsOwnerResponse(IEnumerable<IdentityDeletionProcess> processes)
        : base(processes.Select(p => new IdentityDeletionProcessOverviewDTO(p)))
    {
    }
}
