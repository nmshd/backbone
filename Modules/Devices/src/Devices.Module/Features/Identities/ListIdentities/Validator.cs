using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListIdentities;

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleForEach(x => x.Addresses).ValidId<Query, IdentityAddress>();
    }
}
