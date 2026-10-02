using CreateTierSlice = Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;
using Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Validator = Backbone.Modules.Devices.Module.Features.Tiers.CreateTier.Validator;

namespace Backbone.Modules.Devices.Module.Tests.Tests.Tiers.Commands.CreateTier;

public class ValidatorTests : AbstractTestsBase
{
    [Theory]
    [InlineData("tr")]
    [InlineData("a-tier-name-with-more-than-30-characters")]
    public void Validation_fails_for_invalid_tier_name(string value)
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new CreateTierSlice.Command { Name = value });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(CreateTierSlice.Command.Name),
            expectedErrorCode: "error.platform.validation.invalidTierName",
            expectedErrorMessage: $"Tier Name length must be between {TierName.MIN_LENGTH} and {TierName.MAX_LENGTH}");
    }
}
