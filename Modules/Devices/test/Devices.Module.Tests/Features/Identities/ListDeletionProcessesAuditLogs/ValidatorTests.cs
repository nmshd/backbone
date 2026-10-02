using ListDeletionProcessesAuditLogsSlice = Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;
using Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.ListDeletionProcessesAuditLogs;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDeletionProcessesAuditLogsSlice.Query { IdentityAddress = CreateRandomIdentityAddress() });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDeletionProcessesAuditLogsSlice.Query { IdentityAddress = "some-invalid-address" });

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(ListDeletionProcessesAuditLogsSlice.Query.IdentityAddress));
    }
}
