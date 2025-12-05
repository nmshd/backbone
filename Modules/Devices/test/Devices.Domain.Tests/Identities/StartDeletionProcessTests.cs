using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class StartDeletionProcessTests : AbstractTestsBase
{
    [Fact]
    public void Start_deletion_process()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));
        var activeIdentity = CreateIdentity();
        var activeDevice = new Device(activeIdentity, CommunicationLanguage.DEFAULT_LANGUAGE);
        activeIdentity.Devices.Add(activeDevice);

        Hasher.SetHasher(new DummyHasher([1, 2, 3]));

        // Act
        var deletionProcess = activeIdentity.StartDeletionProcess(activeDevice.Id);

        // Assert
        activeIdentity.DeletionGracePeriodEndsAt.ShouldBe(DateTime.Parse("2000-01-15"));
        activeIdentity.TierId.Value.ShouldBe(Tier.QUEUED_FOR_DELETION.Id.Value);
        activeIdentity.Status.ShouldBe(IdentityStatus.ToBeDeleted);

        AssertDeletionProcessWasStarted(activeIdentity);
        deletionProcess.Status.ShouldBe(DeletionProcessStatus.Active);
        deletionProcess.CreatedByDevice.ShouldBe(activeDevice.Id);
        deletionProcess.GracePeriodEndsAt.ShouldBe(DateTime.Parse("2000-01-15"));

        AssertAuditLogEntryWasCreated(deletionProcess);
        var auditLogEntry = deletionProcess.AuditLog[0];
        auditLogEntry.MessageKey.ShouldBe(MessageKey.StartedByOwner);
        auditLogEntry.DeviceIdHash.ShouldBeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.NewStatus.ShouldBe(DeletionProcessStatus.Active);
    }

    [Fact]
    public void Throws_when_device_not_owned_by_identity()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2020-01-01"));
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE);

        identity.Devices.Add(device);

        // Act
        var acting = () => identity.StartDeletionProcess(DeviceId.Parse("DVC"));

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.recordNotFound", "Device");
    }

    [Fact]
    public void Only_one_active_deletion_process_is_allowed_when_started()
    {
        // Arrange
        var activeIdentity = CreateIdentity();
        var activeDevice = new Device(activeIdentity, CommunicationLanguage.DEFAULT_LANGUAGE);
        activeIdentity.Devices.Add(activeDevice);

        activeIdentity.StartDeletionProcess(activeDevice.Id);

        // Act
        var acting = () => activeIdentity.StartDeletionProcess(activeDevice.Id);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed");
    }

    [Fact]
    public void Raises_domain_events()
    {
        //Arrange
        var activeIdentity = TestDataGenerator.CreateIdentity();
        var tierBeforeDeletion = activeIdentity.TierId;
        var activeDevice = activeIdentity.Devices[0];

        //Act
        activeIdentity.StartDeletionProcess(activeDevice.Id);

        //Assert
        var (tierOfIdentityChangedDomainEvent, identityToBeDeletedDomainEvent) = activeIdentity.ShouldHaveDomainEvents<TierOfIdentityChangedDomainEvent, IdentityToBeDeletedDomainEvent>();

        tierOfIdentityChangedDomainEvent.IdentityAddress.ShouldBe(activeIdentity.Address);
        tierOfIdentityChangedDomainEvent.OldTierId.ShouldBe(tierBeforeDeletion);
        tierOfIdentityChangedDomainEvent.NewTierId.ShouldBe(Tier.QUEUED_FOR_DELETION.Id);

        identityToBeDeletedDomainEvent.IdentityAddress.ShouldBe(activeIdentity.Address);
    }

    [Fact]
    public void Passing_a_lengthOfDeletionGracePeriod_overrides_the_configured_value()
    {
        //Arrange
        var activeIdentity = TestDataGenerator.CreateIdentity();
        var activeDevice = activeIdentity.Devices[0];
        SystemTime.Set("2000-01-01");

        //Act
        activeIdentity.StartDeletionProcess(activeDevice.Id, 1);

        // Assert
        activeIdentity.DeletionGracePeriodEndsAt.ShouldBe(DateTime.Parse("2000-01-02"));
        activeIdentity.DeletionProcesses.First().GracePeriodEndsAt.ShouldBe(DateTime.Parse("2000-01-02"));
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
        auditLogEntry.MessageKey.ShouldBe(MessageKey.StartedByOwner);
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
