using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Module.Features.Identities.UpdateIdentity;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using UpdateIdentitySlice = Backbone.Modules.Devices.Module.Features.Identities.UpdateIdentity;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.UpdateIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new UpdateIdentitySlice.Command { Address = CreateRandomIdentityAddress(), TierId = TierId.Generate() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new UpdateIdentitySlice.Command { Address = "some-invalid-address", TierId = TierId.Generate() });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(UpdateIdentitySlice.Command.Address));
    }

    [Fact]
    public void Fails_when_tier_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new UpdateIdentitySlice.Command { Address = CreateRandomIdentityAddress(), TierId = "some-invalid-tier-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(UpdateIdentitySlice.Command.TierId));
    }
}
