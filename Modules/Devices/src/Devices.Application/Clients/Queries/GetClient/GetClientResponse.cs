using Backbone.Modules.Devices.Application.Clients.DTOs;

namespace Backbone.Modules.Devices.Application.Clients.Queries.GetClient;
public class GetClientResponse : ClientDTO
{
    public GetClientResponse(ClientDTO client) : base(client.ClientId, client.DisplayName, client.TierId) { }
}
