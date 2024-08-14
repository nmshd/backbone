using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Tiers.DTOs;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;

public class ListTiersResponse : PagedResponse<TierDTO>
{
    public ListTiersResponse(DbPaginationResult<Tier> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(dbPaginationResult.ItemsOnPage.Select(el => new TierDTO(el.Id, el.Name)),
        previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
