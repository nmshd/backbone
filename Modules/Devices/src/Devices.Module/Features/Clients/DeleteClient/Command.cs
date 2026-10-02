using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.DeleteClient;

public class Command : IRequest
{
    public required string ClientId { get; init; }
}
