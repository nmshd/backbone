using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;

public class UpdateClientCommand : IRequest<UpdateClientResponse>
{
    public UpdateClientCommand(string clientId, string defaultTier, int? maxIdentities)
    {
        ClientId = clientId;
        DefaultTier = defaultTier;
        MaxIdentities = maxIdentities;
    }

    public string ClientId { get; set; }

    public string DefaultTier { get; set; }

    public int? MaxIdentities { get; set; }
}
