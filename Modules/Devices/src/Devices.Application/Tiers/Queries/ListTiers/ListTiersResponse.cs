using Backbone.Modules.Devices.Application.Tiers.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
public class ListTiersResponse : PagedResponse<TierDTO>
{
    public ListTiersResponse(IEnumerable<TierDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords)     { }
}
