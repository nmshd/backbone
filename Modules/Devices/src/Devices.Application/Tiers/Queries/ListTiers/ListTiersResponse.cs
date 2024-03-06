using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Tiers.DTOs;

namespace Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
public class ListTiersResponse : PagedResponse<TierDTO>
{
    public ListTiersResponse(IEnumerable<TierDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
