using MediatR;

namespace Backbone.Devices.Application.Clients.Commands.UpdateClient;

public class UpdateClientCommand : IRequest<UpdateClientResponse>
{
    public UpdateClientCommand(string clientId, string defaultTier)
    {
        ClientId = clientId;
        DefaultTier = defaultTier;
    }

    public string ClientId { get; set; }

    public string DefaultTier { get; set; }
}
