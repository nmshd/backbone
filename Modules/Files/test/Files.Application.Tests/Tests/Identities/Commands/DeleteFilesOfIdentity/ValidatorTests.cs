using Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Files.Application.Tests.Tests.Identities.Commands.DeleteFilesOfIdentity;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteFilesOfIdentityCommand(TestDataGenerator.CreateRandomIdentityAddress()));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new DeleteFilesOfIdentityCommand("invalid-identity-address"));

        // Assert
        validationResult.ShouldHaveValidationErrorForId(
            propertyWithInvalidId: nameof(DeleteFilesOfIdentityCommand.IdentityAddress));
    }
}
