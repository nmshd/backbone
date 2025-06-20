using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessAsSupport;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult =
            validator.TestValidate(new GetDeletionProcessAsSupportQuery { IdentityAddress = CreateRandomIdentityAddress(), DeletionProcessId = IdentityDeletionProcessId.Generate() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetDeletionProcessAsSupportQuery { IdentityAddress = "invalid-identity-address", DeletionProcessId = IdentityDeletionProcessId.Generate() });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetDeletionProcessAsSupportQuery.IdentityAddress));
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetDeletionProcessAsSupportQuery { IdentityAddress = CreateRandomIdentityAddress(), DeletionProcessId = "invalid-deletion-process-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetDeletionProcessAsSupportQuery.DeletionProcessId));
    }
}
