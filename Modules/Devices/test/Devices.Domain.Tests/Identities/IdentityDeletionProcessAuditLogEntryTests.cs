using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class IdentityDeletionProcessAuditLogEntryTests
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
}
