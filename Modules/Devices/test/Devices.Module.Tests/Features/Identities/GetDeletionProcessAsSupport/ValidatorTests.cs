using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsSupport;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using GetDeletionProcessAsSupportSlice = Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsSupport;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.GetDeletionProcessAsSupport;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult =
            validator.TestValidate(new GetDeletionProcessAsSupportSlice.Query { IdentityAddress = CreateRandomIdentityAddress(), DeletionProcessId = IdentityDeletionProcessId.Generate() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetDeletionProcessAsSupportSlice.Query { IdentityAddress = "invalid-identity-address", DeletionProcessId = IdentityDeletionProcessId.Generate() });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetDeletionProcessAsSupportSlice.Query.IdentityAddress));
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetDeletionProcessAsSupportSlice.Query { IdentityAddress = CreateRandomIdentityAddress(), DeletionProcessId = "invalid-deletion-process-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetDeletionProcessAsSupportSlice.Query.DeletionProcessId));
    }
}
