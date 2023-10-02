using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Devices.Application.Clients.DTOs;

public class ClientDTO : IMapTo<OAuthClient>
{
    public ClientDTO(string clientId, string displayName, TierId defaultTier)
    {
        ClientId = clientId;
        DisplayName = displayName;
        DefaultTier = defaultTier;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string DefaultTier { get; set; }
}
