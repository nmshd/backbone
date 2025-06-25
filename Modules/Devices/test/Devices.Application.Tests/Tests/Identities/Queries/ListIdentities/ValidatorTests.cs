using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListIdentitiesQuery { Addresses = [CreateRandomIdentityAddress()], Status = IdentityStatus.Active });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListIdentitiesQuery { Addresses = ["some-invalid-address"], Status = IdentityStatus.Active });

        // Assert
        validationResult.ShouldHaveValidationErrorForIdInCollection(
            collectionWithInvalidId: nameof(ListIdentitiesQuery.Addresses),
            indexWithInvalidId: 0);
    }
}
