using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class DeletionProcessTests : IDisposable
{
    [Fact]
    public void DeletionStarted_sets_status_and_creates_valid_DeletionProcess()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01 06:00:00");
        SystemTime.Set(currentDateTime);
        var activeIdentity = CreateIdentity();
        activeIdentity.StartDeletionProcessAsOwner(new Device(activeIdentity).Id);

        // Act
        activeIdentity.DeletionStarted();

        // Assert
        activeIdentity.Status.Should().Be(IdentityStatus.Deleting);
        activeIdentity.DeletionProcesses.Should().HaveCount(1);
        activeIdentity.DeletionProcesses.First().DeletionStartedAt.Should().Be(currentDateTime);

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
