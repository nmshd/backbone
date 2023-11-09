using Backbone.Modules.Quotas.Application.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;

public class ListQuotasForIdentityResponse : PagedResponse<QuotaDTO>
{
    public ListQuotasForIdentityResponse(IEnumerable<QuotaDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
