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
    public void DeletionGracePeriodReminderSent_should_update_GracePeriodReminderSentAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01");
        SystemTime.Set(currentDateTime);
        var activeIdentity = CreateIdentity();

        Hasher.SetHasher(new DummyHasher(new byte[] { 1, 2, 3 }));

        var deletionProcess = activeIdentity.StartDeletionProcessAsOwner(new Device(activeIdentity).Id);

        // Act
        activeIdentity.DeletionGracePeriodReminder1Sent();
        activeIdentity.DeletionGracePeriodReminder2Sent();
        activeIdentity.DeletionGracePeriodReminder3Sent();

        // Assert
        activeIdentity.DeletionProcesses.Should().HaveCount(1);
        AssertAuditLogEntriesWereCreated(deletionProcess);
        deletionProcess.GracePeriodReminder1SentAt.Should().NotBeNull().And.Be(currentDateTime);
        deletionProcess.GracePeriodReminder2SentAt.Should().NotBeNull().And.Be(currentDateTime);
        deletionProcess.GracePeriodReminder3SentAt.Should().NotBeNull().And.Be(currentDateTime);
    }

    private static void AssertAuditLogEntriesWereCreated(IdentityDeletionProcess deletionProcess)
    {
        deletionProcess.AuditLog.Should().HaveCount(4);

        for (var i = 1; i <= 3; i++)
        {
            var auditLogEntry = deletionProcess.AuditLog[i];
            auditLogEntry.ProcessId.Should().Be(deletionProcess.Id);
            auditLogEntry.CreatedAt.Should().Be(SystemTime.UtcNow);
            auditLogEntry.IdentityAddressHash.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
            auditLogEntry.OldStatus.Should().Be(DeletionProcessStatus.Approved);
            auditLogEntry.NewStatus.Should().Be(DeletionProcessStatus.Approved);
        }
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
