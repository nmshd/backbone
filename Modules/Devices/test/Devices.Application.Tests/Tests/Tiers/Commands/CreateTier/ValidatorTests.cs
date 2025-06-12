using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Validator = Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier.Validator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Commands.CreateTier;

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
        var validationResult = validator.TestValidate(new CreateTierCommand { Name = value });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(CreateTierCommand.Name),
            expectedErrorCode: "error.platform.validation.invalidTierName",
            expectedErrorMessage: $"Tier Name length must be between {TierName.MIN_LENGTH} and {TierName.MAX_LENGTH}");
    }
}
