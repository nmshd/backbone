using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Identities.ListIdentities;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using ListIdentitiesSlice = Backbone.Modules.Devices.Module.Features.Identities.ListIdentities;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.ListIdentities;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListIdentitiesSlice.Query { Addresses = [CreateRandomIdentityAddress()], Status = IdentityStatus.Active });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListIdentitiesSlice.Query { Addresses = ["some-invalid-address"], Status = IdentityStatus.Active });

        // Assert
        validationResult.ShouldHaveValidationErrorForIdInCollection(
            collectionWithInvalidId: nameof(ListIdentitiesSlice.Query.Addresses),
            indexWithInvalidId: 0);
    }
}
