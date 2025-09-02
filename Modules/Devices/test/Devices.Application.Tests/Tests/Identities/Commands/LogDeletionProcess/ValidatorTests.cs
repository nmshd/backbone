using Backbone.Modules.Devices.Application.Identities.Commands.LogDeletionProcess;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.LogDeletionProcess;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new LogDeletionProcessCommand
        {
            IdentityAddress = CreateRandomIdentityAddress(),
            AggregateType = "aggregateType"
        });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new LogDeletionProcessCommand
        {
            IdentityAddress = "invalid-identity-address",
            AggregateType = "aggregateType"
        });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(LogDeletionProcessCommand.IdentityAddress));
    }
}
