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
    public void DeletionGracePeriodReminder1Sent_should_update_GracePeriodReminder1SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var activeIdentity = CreateIdentity();

        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));

        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(new Device(activeIdentity).Id);

        // Act
        activeIdentity.DeletionGracePeriodReminder1Sent();

        // Assert
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder1SentAt.Should().NotBeNull().And.Be(currentDateTime);
    }

    [Fact]
    public void DeletionGracePeriodReminder2Sent_should_update_GracePeriodReminder2SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var activeIdentity = CreateIdentity();

        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));

        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(new Device(activeIdentity).Id);

        // Act
        activeIdentity.DeletionGracePeriodReminder2Sent();

        // Assert
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder2SentAt.Should().NotBeNull().And.Be(currentDateTime);
    }

    [Fact]
    public void DeletionGracePeriodReminder3Sent_should_update_GracePeriodReminder3SentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var activeIdentity = CreateIdentity();

        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));

        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(new Device(activeIdentity).Id);

        // Act
        activeIdentity.DeletionGracePeriodReminder3Sent();

        // Assert
        AssertAuditLogEntryWasCreated(deletionProcess);
        deletionProcess.GracePeriodReminder3SentAt.Should().NotBeNull().And.Be(currentDateTime);
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
