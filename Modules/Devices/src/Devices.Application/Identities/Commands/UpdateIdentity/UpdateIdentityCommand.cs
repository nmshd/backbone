using Backbone.Modules.Devices.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
public class UpdateIdentityCommand : IRequest<Identity>
{
    public string Address { get; set; }
    public string TierId { get; set; }
}
