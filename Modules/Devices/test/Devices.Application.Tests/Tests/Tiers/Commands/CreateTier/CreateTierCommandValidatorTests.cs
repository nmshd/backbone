using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Commands.CreateTier;

public class CreateTierCommandValidatorTests
{

    [Theory]
    [InlineData("tr")]
    [InlineData("a-tier-name-with-more-than-30-characters")]
    public void Validation_fails_for_invalid_tier_name(string value)
    {
        var validator = new CreateTierCommandValidator();
        var validationResult = validator.TestValidate(new CreateTierCommand(value));

        validationResult.ShouldHaveValidationErrorFor(x => x.Name);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidTierName");
        validationResult.Errors.First().ErrorMessage.Should().Contain("Tier Name length must be between");
    }
}
