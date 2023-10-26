using Backbone.Modules.Quotas.Application.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIndividualQuotas;

public class ListIndividualQuotasResponse : PagedResponse<IndividualQuotaDTO>
{
    public ListIndividualQuotasResponse(IEnumerable<IndividualQuotaDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
