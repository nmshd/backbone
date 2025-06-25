using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;

public class UpdateIdentityCommand : IRequest
{
    public required string Address { get; init; }
    public required string TierId { get; init; }
}
