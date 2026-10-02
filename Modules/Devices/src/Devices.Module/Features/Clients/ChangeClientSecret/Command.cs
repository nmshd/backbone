using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.ChangeClientSecret;

public class Command : IRequest<Response>
{
    public required string ClientId { get; set; }

    public required string NewSecret { get; set; }
}
