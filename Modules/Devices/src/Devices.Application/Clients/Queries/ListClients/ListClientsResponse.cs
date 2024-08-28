using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;

public class ListClientsResponse : CollectionResponseBase<ClientDTO>
{
    public ListClientsResponse(IEnumerable<OAuthClient> clients, IReadOnlyDictionary<string, int> numberOfIdentitiesByClient) : base(clients.Select(client =>
        new ClientDTO(client, numberOfIdentitiesByClient[client.ClientId])))
    {
    }
}
