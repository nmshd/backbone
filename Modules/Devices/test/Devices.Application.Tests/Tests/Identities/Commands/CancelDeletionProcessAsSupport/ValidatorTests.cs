using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsSupport;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new CancelDeletionAsSupportCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), IdentityDeletionProcessId.Generate()));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new CancelDeletionAsSupportCommand("invalid-identity-address", IdentityDeletionProcessId.Generate()));

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(CancelDeletionAsSupportCommand.Address),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "The ID is not valid. Check length, prefix and the used characters.");
    }

    [Fact]
    public void Fails_when_deletion_process_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new CancelDeletionAsSupportCommand(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress(), "invalid-deletion-process-id"));

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(CancelDeletionAsSupportCommand.DeletionProcessId),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "The ID is not valid. Check length, prefix and the used characters.");
    }
}
