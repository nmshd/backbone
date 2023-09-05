using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;

public class UpdateClientCommand : IRequest<UpdateClientResponse>
{
    public UpdateClientCommand(string clientId, string newTierId)
    {
        ClientId = clientId;
        TierId = newTierId;
    }

    public string ClientId { get; set; }

    public string TierId { get; set; }
}
