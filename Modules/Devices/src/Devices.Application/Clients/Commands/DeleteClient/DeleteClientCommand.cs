using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.DeleteClient;

public class DeleteClientCommand : IRequest
{
    public DeleteClientCommand(string clientId)
    {
        ClientId = clientId;
    }

    public string ClientId { get; set; }
}
