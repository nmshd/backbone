using Backbone.Modules.Devices.Domain.OpenIddict;

namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
public class ChangeClientSecretResponse
{
    public ChangeClientSecretResponse(CustomOpenIddictEntityFrameworkCoreApplication client)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        ClientSecret = client.ClientSecret;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
}
