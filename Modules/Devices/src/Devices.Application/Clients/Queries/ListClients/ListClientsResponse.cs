using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Devices.Application.Clients.DTOs;

namespace Backbone.Devices.Application.Clients.Queries.ListClients;
public class ListClientsResponse : EnumerableResponseBase<ClientDTO>
{
    public ListClientsResponse(IEnumerable<ClientDTO> items) : base(items) { }
}
