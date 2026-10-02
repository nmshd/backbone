using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Identities.UpdateIdentity;

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.Address).ValidId<Command, IdentityAddress>();
        RuleFor(c => c.TierId).ValidId<Command, TierId>();
    }
}
