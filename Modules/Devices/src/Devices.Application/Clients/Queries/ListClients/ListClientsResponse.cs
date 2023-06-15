using Backbone.Modules.Devices.Application.Clients.DTOs;
using Enmeshed.BuildingBlocks.Application.CQRS.BaseClasses;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
public class ListClientsResponse : EnumerableResponseBase<ClientDTO>
{
    public ListClientsResponse(IEnumerable<ClientDTO> items) : base(items) { }
}
