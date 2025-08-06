using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Commands.DeleteClient;

public class DeleteClientCommand : IRequest
{
    public required string ClientId { get; init; }
}
