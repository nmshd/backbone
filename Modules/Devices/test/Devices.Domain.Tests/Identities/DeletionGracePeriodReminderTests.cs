using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionGracePeriodReminderTests : AbstractTestsBase
{
    [Fact]
    public void DeletionGracePeriodReminder1Sent_updates_GracePeriodReminder1SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithApprovedDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder1Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder1SentAt.Should().Be(currentDateTime);
    }

    [Fact]
    public void DeletionGracePeriodReminder1Sent_fails_when_no_approved_deletion_process_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder1Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void DeletionGracePeriodReminder2Sent_updates_GracePeriodReminder2SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithApprovedDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder2Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder2SentAt.Should().Be(currentDateTime);
    }


    [Fact]
    public void DeletionGracePeriodReminder2Sent_fails_when_no_approved_deletion_process_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder2Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void DeletionGracePeriodReminder3Sent_updates_GracePeriodReminder3SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithApprovedDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder3Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder3SentAt.Should().Be(currentDateTime);
    }


    [Fact]
    public void DeletionGracePeriodReminder3Sent_fails_when_no_approved_deletion_process_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder3Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.Should().HaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.Should().Be(DeletionProcessStatus.Approved);
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Approved);
    }

    private static Identity CreateIdentityWithApprovedDeletionProcess()
    {
        var identity = CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);
        identity.Devices.Add(device);
        Hasher.SetHasher(new DummyHasher([1, 2, 3]));

        identity.StartDeletionProcessAsOwner(device.Id);

        return identity;
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create([], "prod.enmeshed.eu");
        return new Identity("", address, [], TierId.Generate(), 1, CommunicationLanguage.DEFAULT_LANGUAGE);
    }

    public override void Dispose()
    {
        base.Dispose();
        Hasher.Reset();
    }
}
