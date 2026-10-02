using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListIdentities;

public class ListIdentitiesResponse : CollectionResponseBase<IdentitySummaryDTO>
{
    public ListIdentitiesResponse(IEnumerable<Identity> items) : base(items.Select(i => new IdentitySummaryDTO(i)))
    {
    }
}
