using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Module.Features.Tiers.Shared;

namespace Backbone.Modules.Devices.Module.Features.Tiers.ListTiers;

public class Response : PagedResponse<TierDTO>
{
    public Response(DbPaginationResult<Tier> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(dbPaginationResult.ItemsOnPage.Select(el => new TierDTO(el.Id, el.Name)),
        previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
