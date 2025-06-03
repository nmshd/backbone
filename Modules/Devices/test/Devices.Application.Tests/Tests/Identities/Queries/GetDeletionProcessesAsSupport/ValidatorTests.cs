using Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAsSupport;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessesAsSupport;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDeletionProcessesAsSupportQuery(CreateRandomIdentityAddress()));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDeletionProcessesAsSupportQuery("some-invalid-address"));

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(ListDeletionProcessesAsSupportQuery.IdentityAddress));
    }
}
