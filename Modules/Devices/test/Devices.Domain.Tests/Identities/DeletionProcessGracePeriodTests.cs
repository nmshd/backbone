﻿using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class DeletionProcessGracePeriodTests : IDisposable
{
    [Fact]
    public void DeletionGracePeriodReminder1Sent_updates_GracePeriodReminder1SentAt()
    {
        // Arrange
        var currentDateTime = SetupSystemTime();
        var identity = CreateIdentityWithApprovedDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder1Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder1SentAt.Should().Be(currentDateTime);
    }

    [Fact]
    public void Only_identities_with_an_approved_deletion_process_call_DeletionGracePeriodReminder1Sent()
    {
        // Arrange
        SetupSystemTime();
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder1Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.noApprovedDeletionProcessFound");
    }

    [Fact]
    public void DeletionGracePeriodReminder2Sent_updates_GracePeriodReminder2SentAt()
    {
        // Arrange
        var currentDateTime = SetupSystemTime();
        var identity = CreateIdentityWithApprovedDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder2Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder2SentAt.Should().Be(currentDateTime);
    }


    [Fact]
    public void Only_identities_with_an_approved_deletion_process_call_DeletionGracePeriodReminder2Sent()
    {
        // Arrange
        SetupSystemTime();
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder2Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.noApprovedDeletionProcessFound");
    }

    [Fact]
    public void DeletionGracePeriodReminder3Sent_updates_GracePeriodReminder3SentAt()
    {
        // Arrange
        var currentDateTime = SetupSystemTime();
        var identity = CreateIdentityWithApprovedDeletionProcess();

        // Act
        identity.DeletionGracePeriodReminder3Sent();

        // Assert
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!;
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder3SentAt.Should().Be(currentDateTime);
    }


    [Fact]
    public void Only_identities_with_an_approved_deletion_process_call_DeletionGracePeriodReminder3Sent()
    {
        // Arrange
        SetupSystemTime();
        var identity = CreateIdentity();

        // Act
        var acting = identity.DeletionGracePeriodReminder3Sent;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.noApprovedDeletionProcessFound");
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
        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));
        identity.StartDeletionProcessAsOwner(new Device(identity).Id);

        return identity;
    }

    private static DateTime SetupSystemTime()
    {
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        return currentDateTime;
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create(Array.Empty<byte>(), "id1");
        return new Identity("", address, Array.Empty<byte>(), TierId.Generate(), 1);
    }

    public void Dispose()
    {
        Hasher.Reset();
    }
}
