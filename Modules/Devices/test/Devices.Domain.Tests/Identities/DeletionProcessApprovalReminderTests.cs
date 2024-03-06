using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionProcessApprovalReminderTests
{
    [Fact]
    public void DeletionProcessApprovalReminder1Sent_updates_ApprovalReminder1SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        identity.DeletionProcessApprovalReminder1Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.WaitingForApproval)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.ApprovalReminder1SentAt.Should().Be(currentDateTime);
    }

    [Fact]
    public void DeletionProcessApprovalReminder1Sent_fails_when_no_deletion_process_waiting_for_approval_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionProcessApprovalReminder1Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessHasWrongStatus");
    }

    [Fact]
    public void DeletionProcessApprovalReminder2Sent_updates_ApprovalReminder2SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        identity.DeletionProcessApprovalReminder2Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.WaitingForApproval)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.ApprovalReminder2SentAt.Should().Be(currentDateTime);
    }


    [Fact]
    public void DeletionProcessApprovalReminder2Sent_fails_when_no_deletion_process_waiting_for_approval_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionProcessApprovalReminder2Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessHasWrongStatus");
    }

    [Fact]
    public void DeletionProcessApprovalReminder3Sent_updates_ApprovalReminder3SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var identity = CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        identity.DeletionProcessApprovalReminder3Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.WaitingForApproval)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.ApprovalReminder3SentAt.Should().Be(currentDateTime);
    }


    [Fact]
    public void DeletionProcessApprovalReminder3Sent_fails_when_no_deletion_process_waiting_for_approval_exists()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionProcessApprovalReminder3Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessHasWrongStatus");
    }

    [Fact]
    public void GetEndOfApprovalPeriod_returns_expected_date()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        IdentityDeletionConfiguration.MaxApprovalTime = 10;

        var deletionProcess = IdentityDeletionProcess.StartAsOwner(CreateRandomIdentityAddress(), CreateRandomDeviceId());

        // Act
        var endOfApprovalPeriod = deletionProcess.GetEndOfApprovalPeriod();

        // Assert
        endOfApprovalPeriod.Should().Be(DateTime.Parse("2000-01-11"));
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.Should().HaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
    }

    private static Identity CreateIdentityWithDeletionProcessWaitingForApproval()
    {
        var identity = CreateIdentity();
        Hasher.SetHasher(new DummyHasher([1, 2, 3]));
        identity.StartDeletionProcessAsSupport();

        return identity;
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        return new Identity("", address, Array.Empty<byte>(), TierId.Generate(), 1);
    }

    [Fact]
    public void Dispose()
    {
        Hasher.Reset();
    }
}
