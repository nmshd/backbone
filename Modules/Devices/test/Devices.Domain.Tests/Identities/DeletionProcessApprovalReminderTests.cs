using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionProcessApprovalReminderTests
{
    [Fact]
    public void Cannot_send_first_reminder_without_deletion_process_waiting_approval()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        activeIdentity.StartDeletionProcessAsOwner(DeviceId.Parse("DVC"));

        // Act
        var acting = activeIdentity.DeletionProcessApprovalReminder1Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Throws_when_deletion_process_not_found_and_attempts_to_send_first_reminder()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();

        // Act
        var acting = activeIdentity.DeletionProcessApprovalReminder1Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Sends_first_approval_reminder()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        // Act
        activeIdentity.DeletionProcessApprovalReminder1Sent();

        // Assert
        deletionProcess.ApprovalReminder1SentAt.Should().Be(SystemTime.UtcNow);
        deletionProcess.AuditLog.Count.Should().Be(2);
        var reminderSentAuditLogEntry = deletionProcess.AuditLog.Second();
        reminderSentAuditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        reminderSentAuditLogEntry.Message.Should().Be("First approval reminder was sent.");
    }

    [Fact]
    public void Cannot_send_second_reminder_without_deletion_process_waiting_approval()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        activeIdentity.StartDeletionProcessAsOwner(DeviceId.Parse("DVC"));

        // Act
        var acting = activeIdentity.DeletionProcessApprovalReminder2Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Throws_when_deletion_process_not_found_and_attempts_to_send_second_reminder()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();

        // Act
        var acting = activeIdentity.DeletionProcessApprovalReminder2Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Sends_second_approval_reminder()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        // Act
        activeIdentity.DeletionProcessApprovalReminder2Sent();

        // Assert
        deletionProcess.ApprovalReminder2SentAt.Should().Be(SystemTime.UtcNow);
        deletionProcess.AuditLog.Count.Should().Be(2);
        var reminderSentAuditLogEntry = deletionProcess.AuditLog.Second();
        reminderSentAuditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        reminderSentAuditLogEntry.Message.Should().Be("Second approval reminder was sent.");
    }

    [Fact]
    public void Cannot_send_third_reminder_without_deletion_process_waiting_approval()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        activeIdentity.StartDeletionProcessAsOwner(DeviceId.Parse("DVC"));

        // Act
        var acting = activeIdentity.DeletionProcessApprovalReminder3Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Throws_when_deletion_process_not_found_and_attempts_to_send_third_reminder()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();

        // Act
        var acting = activeIdentity.DeletionProcessApprovalReminder3Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Sends_third_approval_reminder()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        // Act
        activeIdentity.DeletionProcessApprovalReminder3Sent();

        // Assert
        deletionProcess.ApprovalReminder3SentAt.Should().Be(SystemTime.UtcNow);
        deletionProcess.AuditLog.Count.Should().Be(2);
        var reminderSentAuditLogEntry = deletionProcess.AuditLog.Second();
        reminderSentAuditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        reminderSentAuditLogEntry.Message.Should().Be("Third approval reminder was sent.");
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        return new Identity("", address, Array.Empty<byte>(), TierId.Generate(), 1);
    }
}
