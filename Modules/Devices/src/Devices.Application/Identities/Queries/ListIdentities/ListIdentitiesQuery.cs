using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class ListIdentitiesQuery : IRequest<ListIdentitiesResponse>
{
    public ListIdentitiesQuery(PaginationFilter paginationFilter)
    {
        PaginationFilter = paginationFilter;
    }
    public PaginationFilter PaginationFilter { get; set; }
}
