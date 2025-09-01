using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

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
        deletionProcess.Status.ShouldBe(DeletionProcessStatus.WaitingForApproval);

        AssertAuditLogEntryWasCreated(deletionProcess);
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.MessageKey.ShouldBe(MessageKey.StartedBySupport);
        auditLogEntry.DeviceIdHash.ShouldBeNull();
        auditLogEntry.NewStatus.ShouldBe(DeletionProcessStatus.WaitingForApproval);
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
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed");
    }

    [Fact]
    public void Raises_IdentityDeletionProcessStartedDomainEvent()
    {
        //Arrange
        var activeIdentity = CreateIdentity();

        //Act
        var deletionProcess = activeIdentity.StartDeletionProcessAsSupport();

        //Assert
        var domainEvent = deletionProcess.ShouldHaveASingleDomainEvent<IdentityDeletionProcessStartedDomainEvent>();
        domainEvent.Address.ShouldBe(activeIdentity.Address);
        domainEvent.DeletionProcessId.ShouldBe(deletionProcess.Id);
        domainEvent.Initiator.ShouldBe(null);
    }

    private static void AssertDeletionProcessWasStarted(Identity activeIdentity)
    {
        activeIdentity.DeletionProcesses.ShouldHaveCount(1);
        var deletionProcess = activeIdentity.DeletionProcesses[0];
        deletionProcess.ShouldNotBeNull();

        deletionProcess.Id.ShouldNotBeNull();
        deletionProcess.Id.Value.ShouldHaveCount(20);

        deletionProcess.CreatedAt.ShouldBe(SystemTime.UtcNow);

        deletionProcess.AuditLog.ShouldHaveCount(1);
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.CreatedAt.ShouldBe(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.ShouldBeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.ShouldBeNull();
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
