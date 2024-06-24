using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.DTOs;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;

public class ListIdentitiesResponse : CollectionResponseBase<IdentitySummaryDTO>
{
    public ListIdentitiesResponse(IEnumerable<IdentitySummaryDTO> items) : base(items)
    {
    }
}
