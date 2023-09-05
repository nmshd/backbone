using Backbone.Modules.Devices.Domain.OpenIddict;

namespace Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;
public class UpdateClientResponse
{
    public UpdateClientResponse(CustomOpenIddictEntityFrameworkCoreApplication client)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        TierId = client.TierId;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string TierId { get; set; }
}
