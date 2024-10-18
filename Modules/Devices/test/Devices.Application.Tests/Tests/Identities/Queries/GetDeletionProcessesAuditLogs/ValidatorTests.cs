﻿using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessesAuditLogs;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetDeletionProcessesAuditLogsQuery(CreateRandomIdentityAddress()));

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_identity_address_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new GetDeletionProcessesAuditLogsQuery("some-invalid-address"));

        // Assert
        validationResult.ShouldHaveValidationErrorForId(nameof(GetDeletionProcessesAuditLogsQuery.IdentityAddress));
    }
}
