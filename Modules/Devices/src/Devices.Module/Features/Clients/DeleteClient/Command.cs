using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.DeleteClient;

public class DeleteClientCommand : IRequest
{
    public required string ClientId { get; init; }
}
