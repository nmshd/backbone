using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAsSupport;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessesAsSupport;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetDeletionProcessesAsSupportQuery(UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress()));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetDeletionProcessesAsSupportQuery("some-invalid-address"));

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(GetDeletionProcessesAsSupportQuery.IdentityAddress),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "The ID is not valid. Check length, prefix and the used characters.");
    }
}
