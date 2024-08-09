using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;

public class Validator : AbstractValidator<GetIdentityQuery>
{
    public Validator()
    {
        RuleFor(x => x.Address).ValidId<GetIdentityQuery, IdentityAddress>();
    }
}
