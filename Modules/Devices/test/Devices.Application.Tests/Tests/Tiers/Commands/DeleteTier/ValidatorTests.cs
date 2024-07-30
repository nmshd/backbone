using Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Tiers.Commands.DeleteTier;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new DeleteTierCommand(TierId.Generate()));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_tier_id_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new DeleteTierCommand("invalid-tier_id"));

        validationResult.ShouldHaveValidationErrorFor(x => x.TierId);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
