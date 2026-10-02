using GetIdentitySlice = Backbone.Modules.Devices.Module.Features.Identities.GetIdentity;
using Backbone.Modules.Devices.Module.Features.Identities.GetIdentity;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Module.Tests.Tests.Identities.Queries.GetIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetIdentitySlice.Query { Address = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetIdentitySlice.Query { Address = "some-invalid-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetIdentitySlice.Query.Address));
    }
}
