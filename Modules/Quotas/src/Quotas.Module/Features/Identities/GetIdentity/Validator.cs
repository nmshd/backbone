using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Quotas.Module.Features.Identities.GetIdentity;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(x => x.Address).ValidId<Query, IdentityAddress>();
    }
}
