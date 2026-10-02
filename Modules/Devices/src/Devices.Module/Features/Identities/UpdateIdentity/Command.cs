using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.UpdateIdentity;

public class Command : IRequest
{
    public required string Address { get; init; }
    public required string TierId { get; init; }
}
