using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
public class UpdateIdentityCommand : IRequest
{
    public required string Address { get; set; }
    public required string TierId { get; set; }
}
