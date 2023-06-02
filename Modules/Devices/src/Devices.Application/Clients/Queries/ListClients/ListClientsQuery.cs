using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListOAuthClients;
public class ListClientsQuery : IRequest<ListClientsResponse>
{
    public ListClientsQuery(PaginationFilter paginationFilter)
    {
        PaginationFilter = paginationFilter;
    }
    public PaginationFilter PaginationFilter { get; set; }
}
