using Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.ApproveDeletionProcess;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ApproveDeletionProcessCommand(IdentityDeletionProcessId.Generate()));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ApproveDeletionProcessCommand("invalid-deletion-process-id"));

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(ApproveDeletionProcessCommand.DeletionProcessId));
    }
}
