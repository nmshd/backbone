using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcessAsSupport;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new StartDeletionProcessAsSupportCommand(CreateRandomIdentityAddress()));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new StartDeletionProcessAsSupportCommand("invalid-identity-address"));

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(StartDeletionProcessAsSupportCommand.IdentityAddress));
    }
}
