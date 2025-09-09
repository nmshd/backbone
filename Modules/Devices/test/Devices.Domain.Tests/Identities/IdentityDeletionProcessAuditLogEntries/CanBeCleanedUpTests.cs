using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Tests.Identities.IdentityDeletionProcessAuditLogEntries;

public class CanBeCleanedUpTests : AbstractTestsBase
{
    [Fact]
    public void Can_be_cleaned_up_when_older_than_2_years_and_corresponding_identity_is_deleted()
    {
        // Arrange
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DeletionCompleted(CreateRandomIdentityAddress());
        auditLogEntry.AssociateUsernames([Username.New()]);
        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.AuditLogRetentionPeriodInDays + 1));

        // Act
        var result = IdentityDeletionProcessAuditLogEntry.CanBeCleanedUp.Compile()(auditLogEntry);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Cannot_be_cleaned_up_when_younger_than_2_years()
    {
        // Arrange
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DeletionCompleted(CreateRandomIdentityAddress());
        auditLogEntry.AssociateUsernames([Username.New()]);

        // Act
        var result = IdentityDeletionProcessAuditLogEntry.CanBeCleanedUp.Compile()(auditLogEntry);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Cannot_be_cleaned_up_when_corresponding_identity_is_not_deleted()
    {
        // Arrange
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DeletionCompleted(CreateRandomIdentityAddress());
        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.AuditLogRetentionPeriodInDays + 1));

        // Act
        var result = IdentityDeletionProcessAuditLogEntry.CanBeCleanedUp.Compile()(auditLogEntry);

        // Assert
        result.ShouldBeFalse();
    }
}
