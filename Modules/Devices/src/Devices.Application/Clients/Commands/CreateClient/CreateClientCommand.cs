using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClient;

public class CreateClientCommand : IRequest<CreateClientResponse>
{
    public CreateClientCommand(string clientId, string displayName, string clientSecret, string defaultTier, int? maxIdentities)
    {
        ClientId = clientId;
        DisplayName = displayName;
        ClientSecret = clientSecret;
        DefaultTier = defaultTier;
        MaxIdentities = maxIdentities;
    }

    public string ClientId { get; set; }

    public string DisplayName { get; set; }

    public string ClientSecret { get; set; }

    public string DefaultTier { get; set; }

    public int? MaxIdentities { get; set; }
}
