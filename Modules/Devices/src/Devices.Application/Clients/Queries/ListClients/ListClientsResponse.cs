using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.Clients.DTOs;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
public class ListClientsResponse : CollectionResponseBase<ClientOverviewDTO>
{
    public ListClientsResponse(IEnumerable<ClientOverviewDTO> items) : base(items) { }
}
