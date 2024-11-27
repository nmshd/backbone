using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class StartDeletionProcessAsSupportTests : AbstractTestsBase
{
    [Fact]
    public void Start_deletion_process()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();

        Hasher.SetHasher(new DummyHasher([1, 2, 3]));

        // Act
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        // Assert
        AssertDeletionProcessWasStarted(activeIdentity);
        deletionProcess.Status.Should().Be(DeletionProcessStatus.WaitingForApproval);

        AssertAuditLogEntryWasCreated(deletionProcess);
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.MessageKey.Should().Be(MessageKey.StartedBySupport);
        auditLogEntry.DeviceIdHash.Should().BeNull();
        auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.WaitingForApproval);
    }

    [Fact]
    public void Only_one_active_deletion_process_is_allowed_when_started()
    {
        // Arrange
        var activeIdentity = CreateIdentity();

        activeIdentity.StartDeletionProcessAsSupport();

        // Act
        var acting = activeIdentity.StartDeletionProcessAsSupport;

        // Assert
        acting.Should().Throw<DomainException>().Which.Code.Should().Be("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed");
    }

    [Fact]
    public void Raises_IdentityDeletionProcessStartedDomainEvent()
    {
        //Arrange
        var activeIdentity = CreateIdentity();

        //Act
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        //Assert
        var domainEvent = deletionProcess.Should().HaveASingleDomainEvent<IdentityDeletionProcessStartedDomainEvent>();
        domainEvent.Address.Should().Be(activeIdentity.Address);
        domainEvent.DeletionProcessId.Should().Be(deletionProcess.Id);
        domainEvent.Initiator.Should().Be(null);
    }

    private static void AssertDeletionProcessWasStarted(Identity activeIdentity)
    {
        activeIdentity.DeletionProcesses.Should().HaveCount(1);
        var deletionProcess = activeIdentity.DeletionProcesses[0];
        deletionProcess.Should().NotBeNull();

        deletionProcess.Id.Should().NotBeNull();
        deletionProcess.Id.Value.Should().HaveLength(20);

        deletionProcess.CreatedAt.Should().Be(SystemTime.UtcNow);

        deletionProcess.AuditLog.Should().HaveCount(1);
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
        auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.Should().BeNull();
    }

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create([], "prod.enmeshed.eu");
        return new Identity("", address, [], TierId.Generate(), 1, CommunicationLanguage.DEFAULT_LANGUAGE);
    }
}
