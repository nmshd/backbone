using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
public class UpdateIdentityCommand : IRequest<Identity>
{
    public string Address { get; set; }
    public string TierId { get; set; }
}
