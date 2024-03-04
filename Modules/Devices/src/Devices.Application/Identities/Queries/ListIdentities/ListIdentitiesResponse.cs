using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.DTOs;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class ListIdentitiesResponse : PagedResponse<IdentitySummaryDTO>
{
    public ListIdentitiesResponse(IEnumerable<IdentitySummaryDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
