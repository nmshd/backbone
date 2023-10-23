using MediatR;

namespace Backbone.Devices.Application.Clients.Commands.DeleteClient;

public class DeleteClientCommand : IRequest
{
    public DeleteClientCommand(string clientId)
    {
        ClientId = clientId;
    }

    public string ClientId { get; set; }
}
