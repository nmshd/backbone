using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ChangeFeatureFlags;

public class Validator : AbstractValidator<ChangeFeatureFlagsCommand>
{
    public Validator(IOptions<ApplicationConfiguration> configuration)
    {
        RuleForEach(c => c.Keys).Must(k => FeatureFlagName.Validate(k) == null)
            .WithMessage("Invalid feature flag name. Check length and the used characters.")
            .WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        RuleFor(c => c.Count).LessThanOrEqualTo(configuration.Value.MaxNumberOfFeatureFlagsPerIdentity)
            .WithMessage(ApplicationErrors.Devices.MaxNumberOfFeatureFlagsExceeded(configuration.Value.MaxNumberOfFeatureFlagsPerIdentity).Message)
            .WithErrorCode(ApplicationErrors.Devices.MaxNumberOfFeatureFlagsExceeded(configuration.Value.MaxNumberOfFeatureFlagsPerIdentity).Code);

    }
}
