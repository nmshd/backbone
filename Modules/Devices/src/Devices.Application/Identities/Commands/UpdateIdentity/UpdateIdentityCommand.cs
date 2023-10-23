using MediatR;

namespace Backbone.Devices.Application.Identities.Commands.UpdateIdentity;
public class UpdateIdentityCommand : IRequest
{
    public string Address { get; set; }
    public string TierId { get; set; }
}
