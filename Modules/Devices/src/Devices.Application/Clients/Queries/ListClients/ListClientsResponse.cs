
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;

public class ListClientsResponse : PagedResponse<ClientDTO>
{
    public ListClientsResponse(IEnumerable<ClientDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}

