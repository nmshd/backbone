using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using CancelDeletionProcessSlice = Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.CancelDeletionProcess;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new CancelDeletionProcessSlice.Command { DeletionProcessId = IdentityDeletionProcessId.Generate() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new CancelDeletionProcessSlice.Command { DeletionProcessId = "invalid-deletion-process-id" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(CancelDeletionProcessSlice.Command.DeletionProcessId));
    }
}
