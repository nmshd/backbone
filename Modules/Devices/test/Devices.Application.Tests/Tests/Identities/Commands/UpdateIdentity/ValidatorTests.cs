using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new UpdateIdentityCommand { Address = UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), TierId = TierId.Generate() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new UpdateIdentityCommand { Address = "some-invalid-address", TierId = TierId.Generate() });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(UpdateIdentityCommand.Address),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "The ID is not valid. Check length, prefix and the used characters.");
    }

    [Fact]
    public void Fails_when_tier_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new UpdateIdentityCommand { Address = UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), TierId = "some-invalid-tier-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(UpdateIdentityCommand.TierId),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "The ID is not valid. Check length, prefix and the used characters.");
    }
}
