using Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.RejectDeletionProcess;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new RejectDeletionProcessCommand(IdentityDeletionProcessId.Generate()));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new RejectDeletionProcessCommand("invalid-deletion-process-id"));

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(RejectDeletionProcessCommand.DeletionProcessId),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "The ID is not valid. Check length, prefix and the used characters.");
    }
}
