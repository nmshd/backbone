using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Devices.Application.Clients.DTOs;
public class ClientDTO : IMapTo<OAuthClient>
{
    public ClientDTO(string clientId, string displayName)
    {
        ClientId = clientId;
        DisplayName = displayName;
    }

    public string ClientId { get; set; }

    public string DisplayName { get; set; }
}
