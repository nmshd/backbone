namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClients;

public class CreateClientResponse
{
    public CreateClientResponse(string clientId, string displayName, string clientSecret, string tierId)
    {
        ClientId = clientId;
        DisplayName = displayName;
        ClientSecret = clientSecret;
        TierId = tierId;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
    public string TierId { get; set; }
}
