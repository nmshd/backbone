﻿using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class CancelDeletionProcessAsOwnerTests : AbstractTestsBase
{
    [Fact]
    public void Cancel_deletion_process()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2024-01-01"));
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var tierIdBeforeDeletion = identity.TierIdBeforeDeletion;

        // Act
        var deletionProcess = identity.CancelDeletionProcessAsOwner(identity.DeletionProcesses[0].Id, identity.Devices[0].Id);

        // Assert
        identity.TierId.Should().Be(tierIdBeforeDeletion);
        identity.TierIdBeforeDeletion.Should().Be(null);
        identity.Status.Should().Be(IdentityStatus.Active);
        deletionProcess.Status.Should().Be(DeletionProcessStatus.Cancelled);
        deletionProcess.CancelledAt.Should().Be(DateTime.Parse("2024-01-01"));
        deletionProcess.CancelledByDevice.Should().Be(identity.Devices[0].Id);
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        identity.Devices.Add(new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE));
        var deviceId = identity.Devices[0].Id;
        var deletionProcessId = IdentityDeletionProcessId.Create("IDP00000000000000001").Value;

        // Act
        var acting = () => identity.CancelDeletionProcessAsOwner(deletionProcessId, deviceId);

        // Assert
        acting.Should().Throw<DomainException>().Which.Message.Should().Contain("IdentityDeletionProcess");
    }

    [Fact]
    public void Throws_when_deletion_process_is_in_wrong_status()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval();

        // Act
        var acting = () => identity.CancelDeletionProcessAsOwner(identity.DeletionProcesses[0].Id, identity.Devices[0].Id);

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void Raises_IdentityDeletionProcessStatusChangedDomainEvent_when_Cancelling()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        identity.DeletionProcesses[0].ClearDomainEvents();

        // Act
        var deletionProcess = identity.CancelDeletionProcessAsOwner(identity.DeletionProcesses[0].Id, identity.Devices[0].Id);

        var domainEvent = deletionProcess.Should().HaveASingleDomainEvent<IdentityDeletionProcessStatusChangedDomainEvent>();
        domainEvent.DeletionProcessId.Should().Be(deletionProcess.Id);
        domainEvent.Address.Should().Be(identity.Address);
        domainEvent.Initiator.Should().Be(identity.Address);
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.Should().HaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.OldStatus.Should().Be(DeletionProcessStatus.Approved);
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Cancelled);
    }
}
