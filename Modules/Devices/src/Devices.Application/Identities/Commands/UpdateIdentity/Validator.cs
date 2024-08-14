using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;

public class Validator : AbstractValidator<UpdateIdentityCommand>
{
    public Validator()
    {
        RuleFor(x => x.Address).ValidId<UpdateIdentityCommand, IdentityAddress>();
        RuleFor(c => c.TierId).ValidId<UpdateIdentityCommand, TierId>();
    }
}
