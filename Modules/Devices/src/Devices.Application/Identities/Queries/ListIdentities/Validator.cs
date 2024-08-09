using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;

public class Validator : AbstractValidator<ListIdentitiesQuery>
{
    public Validator()
    {
        RuleForEach(x => x.Addresses).ValidId<ListIdentitiesQuery, IdentityAddress>();
    }
}
