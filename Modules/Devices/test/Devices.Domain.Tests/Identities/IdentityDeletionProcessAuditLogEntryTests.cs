﻿using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class IdentityDeletionProcessAuditLogEntryTests : AbstractTestsBase
{
    [Fact]
    public void AssociateUsernames_happy_path()
    {
        // Arrange
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DeletionCompleted(CreateRandomIdentityAddress());

        // Act
        auditLogEntry.AssociateUsernames([Username.Parse("USR1111111111111111"), Username.Parse("USR2222222222222222")]);

        // Assert
        auditLogEntry.UsernameHashesBase64.ShouldNotBeNull();
        auditLogEntry.UsernameHashesBase64.ShouldHaveCount(2);
        auditLogEntry.UsernameHashesBase64.ShouldContain("jj3azydpiPwK4iFxo/wpCP6pP5Yf5MStnu/hyzMUZ14=", "FQao/LArcVbFzRs4RgYCU5JycRx9zmJMxY5ApJ0Nk8E=");
    }

    [Fact]
    public void IsAssociatedToUser_returns_true_if_user_is_associated()
    {
        // Arrange
        var username1 = Username.Parse("USR1111111111111111");
        var username2 = Username.Parse("USR2222222222222222");
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DeletionCompleted(CreateRandomIdentityAddress());
        auditLogEntry.AssociateUsernames([username1, username2]);

        // Act
        var resultForUsername1 = IdentityDeletionProcessAuditLogEntry.IsAssociatedToUser(username1).Compile()(auditLogEntry);
        var resultForUsername2 = IdentityDeletionProcessAuditLogEntry.IsAssociatedToUser(username2).Compile()(auditLogEntry);

        // Assert
        resultForUsername1.ShouldBeTrue();
        resultForUsername2.ShouldBeTrue();
    }

    [Fact]
    public void IsAssociatedToUser_returns_false_if_user_is_not_associated()
    {
        // Arrange
        var unassociatedUsername = Username.Parse("USR3333333333333333");
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DeletionCompleted(CreateRandomIdentityAddress());
        auditLogEntry.AssociateUsernames([Username.Parse("USR1111111111111111"), Username.Parse("USR2222222222222222")]);

        // Act
        var result = IdentityDeletionProcessAuditLogEntry.IsAssociatedToUser(unassociatedUsername).Compile()(auditLogEntry);

        // Assert
        result.ShouldBeFalse();
    }
}
