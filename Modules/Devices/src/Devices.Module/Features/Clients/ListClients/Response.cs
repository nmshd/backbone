using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Module.Features.Clients.Shared;

namespace Backbone.Modules.Devices.Module.Features.Clients.ListClients;

public class ListClientsResponse : CollectionResponseBase<ClientDTO>
{
    public ListClientsResponse(IEnumerable<OAuthClient> clients, IReadOnlyDictionary<string, int> numberOfIdentitiesByClient) : base(clients.Select(client =>
        new ClientDTO(client, numberOfIdentitiesByClient[client.ClientId])))
    {
    }
}
