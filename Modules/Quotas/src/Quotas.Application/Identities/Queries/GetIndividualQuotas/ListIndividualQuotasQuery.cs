using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIndividualQuotas;

public class ListIndividualQuotasQuery : IRequest<ListIndividualQuotasResponse>
{
    public ListIndividualQuotasQuery(PaginationFilter paginationFilter, string address)
    {
        PaginationFilter = paginationFilter;
        Address = address;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public string Address { get; set; }
}
