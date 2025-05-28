using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAsSupport;

public class GetDeletionProcessesAsSupportResponse : CollectionResponseBase<IdentityDeletionProcessOverviewDTO>
{
    public GetDeletionProcessesAsSupportResponse(IEnumerable<IdentityDeletionProcess> processes)
        : base(processes.Select(p => new IdentityDeletionProcessOverviewDTO(p)).OrderByDescending(p => p.CreatedAt))
    {
    }
}
