using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.UpdateIdentity;

public class UpdateIdentityCommand : IRequest
{
    public required string Address { get; init; }
    public required string TierId { get; init; }
}
