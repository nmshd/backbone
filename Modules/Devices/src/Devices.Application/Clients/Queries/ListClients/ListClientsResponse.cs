using Backbone.Modules.Devices.Application.Clients.DTOs;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
public class ListClientsResponse
{
    public IEnumerable<ClientDTO> Items { get; set; }
}
