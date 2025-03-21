using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetFeatureFlags;

public class Validator : AbstractValidator<GetFeatureFlagsQuery>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<GetFeatureFlagsQuery, IdentityAddress>();
    }
}
