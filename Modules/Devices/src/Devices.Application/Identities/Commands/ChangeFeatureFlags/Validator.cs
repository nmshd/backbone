using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ChangeFeatureFlags;

public class Validator : AbstractValidator<ChangeFeatureFlagsCommand>
{
    public Validator()
    {
        RuleForEach(c => c.Keys).Must(k => FeatureFlagName.Validate(k) == null)
            .WithMessage("Invalid feature flag name. Check length and the used characters.")
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);
    }
}
