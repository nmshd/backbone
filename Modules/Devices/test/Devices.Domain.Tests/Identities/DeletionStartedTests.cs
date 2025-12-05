using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Domain.Tests.Identities.TestDoubles;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionStartedTests : AbstractTestsBase
{
    [Fact]
    public void DeletionStarted_sets_status_and_creates_valid_DeletionProcess()
    {
        // Arrange
        var identity = CreateIdentityWithActiveDeletionProcess();

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays).AddDays(1)); // past deletion grace period

        Hasher.SetHasher(new DummyHasher([1, 2, 3]));

        // Act
        identity.DeletionStarted();

        // Assert
        identity.Status.ShouldBe(IdentityStatus.Deleting);
        var deletionProcess = identity.DeletionProcesses.First();
        deletionProcess.DeletionStartedAt.ShouldBe(SystemTime.UtcNow);
        AssertAuditLogEntryWasCreated(deletionProcess);
    }

    private static void AssertAuditLogEntryWasCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.ShouldHaveCount(2);

        var auditLogEntry = deletionProcess.AuditLog[1];
        auditLogEntry.MessageKey.ShouldBe(MessageKey.DeletionStarted);
        auditLogEntry.CreatedAt.ShouldBe(SystemTime.UtcNow);
        auditLogEntry.IdentityAddressHash.ShouldBeEquivalentTo(new byte[] { 1, 2, 3 });
        auditLogEntry.OldStatus.ShouldBe(DeletionProcessStatus.Active);
        auditLogEntry.NewStatus.ShouldBe(DeletionProcessStatus.Deleting);
    }

    [Fact]
    public void Fails_to_start_if_GracePeriod_is_not_over()
    {
        // Arrange
        var identity = CreateIdentityWithActiveDeletionProcess();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.gracePeriodHasNotYetExpired");
    }

    [Fact]
    public void Fails_to_start_if_no_deletion_process_exists()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var acting = identity.DeletionStarted;

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus");
    }

    [Fact]
    public void Raises_domain_events()
    {
        //Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithActiveDeletionProcess();

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays).AddDays(1));

        //Act
        activeIdentity.DeletionStarted();

        //Assert
        var domainEvent = activeIdentity.ShouldHaveASingleDomainEvent<IdentityDeletedDomainEvent>();
        domainEvent.IdentityAddress.ShouldBe(activeIdentity.Address);
    }

    private static Identity CreateIdentityWithActiveDeletionProcess()
    {
        var identity = TestDataGenerator.CreateIdentity();
        identity.StartDeletionProcess(identity.Devices.First().Id);

        return identity;
    }

    public override void Dispose()
    {
        base.Dispose();
        Hasher.Reset();
    }
}
