using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Quotas.Application.DTOs;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;

public class ListQuotasForIdentityResponse : PagedResponse<QuotaDTO>
{
    public ListQuotasForIdentityResponse(IEnumerable<QuotaDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
