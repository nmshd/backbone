using Backbone.BuildingBlocks.Domain;
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
    public void DeletionStarted_sets_status_and_deletionStartedAt()
    {
        // Arrange
        var currentDateTime = DateTime.Parse("2000-01-01 06:00:00");
        SystemTime.Set(currentDateTime);
        var activeIdentity = CreateIdentity();

        // Act
        activeIdentity.DeletionStarted();

        // Assert
        activeIdentity.IdentityStatus.Should().Be(IdentityStatus.Deleting);
        activeIdentity.DeletionStartedAt.Should().Be(currentDateTime);

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
