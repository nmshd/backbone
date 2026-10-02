using ListDeletionProcessesAuditLogs = Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;
using Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Module.Tests.Tests.Identities.Queries.GetDeletionProcessesAuditLogs;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDeletionProcessesAuditLogs.Query { IdentityAddress = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDeletionProcessesAuditLogs.Query { IdentityAddress = "some-invalid-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(ListDeletionProcessesAuditLogs.Query.IdentityAddress));
    }
}
