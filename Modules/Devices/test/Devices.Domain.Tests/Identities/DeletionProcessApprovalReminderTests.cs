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
    public void Cannot_send_reminder_without_deletion_process_waiting_approval()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();

        // Act
        var acting = () => activeIdentity.DeletionProcessApprovalReminder1Sent(IdentityDeletionProcessId.Generate());

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Throws_when_deletion_process_not_found()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(DeviceId.Parse("DVC"));

        // Act
        var acting = () => activeIdentity.DeletionProcessApprovalReminder1Sent(deletionProcess.Id);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.cannotSendReminderWithoutDeletionProcessWaitingForApproval");
    }

    [Fact]
    public void Send_first_approval_reminder()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        // Act
        activeIdentity.DeletionProcessApprovalReminder1Sent(deletionProcess.Id);

        // Assert
        var identityDeletionProcess = activeIdentity.DeletionProcesses.FirstOrDefault()!;
        identityDeletionProcess.ApprovalReminder1SentAt.Should().Be(SystemTime.UtcNow);
        identityDeletionProcess.AuditLog.Count.Should().Be(2);
        var reminderSentAuditLogEntry = identityDeletionProcess.AuditLog.Second();
        reminderSentAuditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        reminderSentAuditLogEntry.Message.Should().Be("Approval reminder 1 was sent.");
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        return new Identity("", address, Array.Empty<byte>(), TierId.Generate(), 1);
    }
}
