using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListFeatureFlags;

public class Validator : AbstractValidator<ListFeatureFlagsQuery>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<ListFeatureFlagsQuery, IdentityAddress>();
    }
}
