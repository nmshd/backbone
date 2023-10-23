using MediatR;

namespace Backbone.Devices.Application.Clients.Commands.CreateClient;

public class CreateClientCommand : IRequest<CreateClientResponse>
{
    public CreateClientCommand(string clientId, string displayName, string clientSecret, string defaultTier)
    {
        ClientId = clientId;
        DisplayName = displayName;
        ClientSecret = clientSecret;
        DefaultTier = defaultTier;
    }

    public string ClientId { get; set; }

    public string DisplayName { get; set; }

    public string ClientSecret { get; set; }

    public string DefaultTier { get; set; }
}
