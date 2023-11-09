using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;

public class ListQuotasForIdentityQuery : IRequest<ListQuotasForIdentityResponse>
{
    public ListQuotasForIdentityQuery(PaginationFilter paginationFilter, string address)
    {
        PaginationFilter = paginationFilter;
        Address = address;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public string Address { get; set; }
}
