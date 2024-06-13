using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;
public class IdentityStatusTests : AbstractTestsBase
{
    [Fact]
    public void Set_as_Active_when_identity_is_created()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();

        // Act
        var status = identity.Status;

        // Assert
        status.Should().Be(IdentityStatus.Active);

        identity.DomainEvents.Should().HaveCount(0);
    }

    [Fact]
    public void Raises_IdentityToBeDeletedDomainEvent()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = identity.Devices[0];

        // Act
        identity.StartDeletionProcessAsOwner(device.Id);

        // Assert
        var domainEvent = identity.Should().HaveLastRisenADomainEvent<IdentityToBeDeletedDomainEvent>();
        domainEvent.IdentityAddress.Should().Be(identity.Address);
    }
}
