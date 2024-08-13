using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListIdentitiesQuery([UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress()], IdentityStatus.Active));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListIdentitiesQuery(["some-invalid-address"], IdentityStatus.Active));

        // Assert
        validationResult.ShouldHaveValidationErrorForItem(
            propertyName: nameof(ListIdentitiesQuery.Addresses),
            expectedErrorCode: "error.platform.validation.invalidPropertyValue",
            expectedErrorMessage: "The ID is not valid. Check length, prefix and the used characters.");
    }
}
