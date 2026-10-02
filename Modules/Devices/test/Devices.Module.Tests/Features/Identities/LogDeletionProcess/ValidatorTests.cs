using Backbone.Modules.Devices.Module.Features.Identities.LogDeletionProcess;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using LogDeletionProcessSlice = Backbone.Modules.Devices.Module.Features.Identities.LogDeletionProcess;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.LogDeletionProcess;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new LogDeletionProcessSlice.Command
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
        var validationResult = validator.TestValidate(new LogDeletionProcessSlice.Command
        {
            IdentityAddress = "invalid-identity-address",
            AggregateType = "aggregateType"
        });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(LogDeletionProcessSlice.Command.IdentityAddress));
    }
}
