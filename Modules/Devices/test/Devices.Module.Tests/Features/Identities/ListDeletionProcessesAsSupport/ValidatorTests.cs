using Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsSupport;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using ListDeletionProcessesAsSupportSlice = Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsSupport;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.ListDeletionProcessesAsSupport;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDeletionProcessesAsSupportSlice.Query { IdentityAddress = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDeletionProcessesAsSupportSlice.Query { IdentityAddress = "some-invalid-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(ListDeletionProcessesAsSupportSlice.Query.IdentityAddress));
    }
}
