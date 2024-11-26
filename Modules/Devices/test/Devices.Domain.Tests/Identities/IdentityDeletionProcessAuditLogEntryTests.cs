using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

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
        auditLogEntry.UsernameHashesBase64.Should().NotBeEmpty();
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
        resultForUsername1.Should().BeTrue();
        resultForUsername2.Should().BeTrue();
    }

    [Fact]
    public void IsAssociatedToUser_returns_false_if_user_is_not_associated()
    {
        // Arrange
        var unauthorizedUsername = Username.Parse("USR3333333333333333");
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DeletionCompleted(CreateRandomIdentityAddress());
        auditLogEntry.AssociateUsernames([Username.Parse("USR1111111111111111"), Username.Parse("USR2222222222222222")]);

        // Act
        var resultForUnauthorizedUsername = IdentityDeletionProcessAuditLogEntry.IsAssociatedToUser(unauthorizedUsername).Compile()(auditLogEntry);

        // Assert
        resultForUnauthorizedUsername.Should().BeFalse();
    }
}
