using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionProcessGracePeriodTests : AbstractTestsBase
{
    [Fact]
    public void DeletionGracePeriodReminder1Sent_updates_GracePeriodReminder1SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithActiveDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder1Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Active)!;
        AssertAuditLogEntryWasCreated(deletionProcess, MessageKey.GracePeriodReminder1Sent);
        deletionProcess.GracePeriodReminder1SentAt.ShouldBe(currentDateTime);
    }

    [Fact]
    public void DeletionGracePeriodReminder1Sent_fails_when_no_active_deletion_process_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder1Sent;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void DeletionGracePeriodReminder2Sent_updates_GracePeriodReminder2SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithActiveDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder2Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Active)!;
        AssertAuditLogEntryWasCreated(deletionProcess, MessageKey.GracePeriodReminder2Sent);
        deletionProcess.GracePeriodReminder2SentAt.ShouldBe(currentDateTime);
    }


    [Fact]
    public void DeletionGracePeriodReminder2Sent_fails_when_no_active_deletion_process_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder2Sent;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void DeletionGracePeriodReminder3Sent_updates_GracePeriodReminder3SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithActiveDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder3Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Active)!;
        AssertAuditLogEntryWasCreated(deletionProcess, MessageKey.GracePeriodReminder3Sent);
        deletionProcess.GracePeriodReminder3SentAt.ShouldBe(currentDateTime);
    }


    [Fact]
    public void DeletionGracePeriodReminder3Sent_fails_when_no_active_deletion_process_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder3Sent;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess, MessageKey expectedMessageKey)
    {
        deletionProcess.AuditLog.ShouldHaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.CreatedAt.ShouldBe(SystemTime.UtcNow);
        auditLogEntry.MessageKey.ShouldBe(expectedMessageKey);
        auditLogEntry.IdentityAddressHash.ShouldBeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.ShouldBe(DeletionProcessStatus.Active);
        auditLogEntry.NewStatus.ShouldBe(DeletionProcessStatus.Active);
    }

    private static Identity CreateIdentityWithActiveDeletionProcess()
    {
        var identity = TestDataGenerator.CreateIdentity();
        Hasher.SetHasher(new DummyHasher([1, 2, 3]));
        identity.StartDeletionProcess(identity.Devices.First().Id);

        return identity;
    }

    public override void Dispose()
    {
        base.Dispose();
        Hasher.Reset();
    }
}
