using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Devices.Application.Tiers.DTOs;

namespace Backbone.Devices.Application.Tiers.Queries.ListTiers;
public class ListTiersResponse : PagedResponse<TierDTO>
{
    public ListTiersResponse(IEnumerable<TierDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
