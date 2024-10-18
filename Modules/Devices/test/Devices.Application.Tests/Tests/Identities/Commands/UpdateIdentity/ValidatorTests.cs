using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new UpdateIdentityCommand { Address = CreateRandomIdentityAddress(), TierId = TierId.Generate() });

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
        validationResult.ShouldHaveValidationErrorForId(nameof(UpdateIdentityCommand.Address));
    }

    [Fact]
    public void Fails_when_tier_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new UpdateIdentityCommand { Address = CreateRandomIdentityAddress(), TierId = "some-invalid-tier-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(UpdateIdentityCommand.TierId));
    }
}
