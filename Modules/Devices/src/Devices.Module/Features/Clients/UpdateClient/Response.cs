using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Module.Features.Clients.UpdateClient;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("UpdateClientResponse")]
public class Response
{
    public Response(OAuthClient client)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        DefaultTier = client.DefaultTier;
        CreatedAt = client.CreatedAt;
        MaxIdentities = client.MaxIdentities;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
}
