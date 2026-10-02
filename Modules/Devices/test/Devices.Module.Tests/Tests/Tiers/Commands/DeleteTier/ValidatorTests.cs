using DeleteTierSlice = Backbone.Modules.Devices.Module.Features.Tiers.DeleteTier;
using Backbone.Modules.Devices.Module.Features.Tiers.DeleteTier;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Module.Tests.Tests.Tiers.Commands.DeleteTier;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteTierSlice.Command { TierId = TierId.Generate() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_tier_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteTierSlice.Command { TierId = "invalid-tier_id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(DeleteTierSlice.Command.TierId));
    }
}
