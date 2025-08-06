using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsOwner;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new CancelDeletionProcessAsOwnerCommand { DeletionProcessId = IdentityDeletionProcessId.Generate() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new CancelDeletionProcessAsOwnerCommand { DeletionProcessId = "invalid-deletion-process-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(CancelDeletionProcessAsOwnerCommand.DeletionProcessId));
    }
}
