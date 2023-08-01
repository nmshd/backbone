namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
public class ChangeClientSecretResponse
{
    public ChangeClientSecretResponse(string clientId, string displayName, string clientSecret)
    {
        ClientId = clientId;
        DisplayName = displayName;
        ClientSecret = clientSecret;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
}
