using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;

public class ListIdentitiesResponse : CollectionResponseBase<IdentitySummaryDTO>
{
    public ListIdentitiesResponse(IEnumerable<Identity> items) : base(items.Select(i => new IdentitySummaryDTO(i)))
    {
    }
}
