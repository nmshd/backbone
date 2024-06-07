using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class ChangeTierTests : AbstractTestsBase
{
    [Fact]
    public void Changing_the_tier_raises_TierOfIdentityChangedDomainEvent()
    {
        // Arrange
        var identity = CreateIdentity();
        var oldTier = identity.TierId;
        identity.ClearDomainEvents();

        //Act
        identity.ChangeTier(TierId.Generate());

        // Assert
        var domainEvent = identity.Should().HaveASingleDomainEvent<TierOfIdentityChangedDomainEvent>();
        domainEvent.IdentityAddress.Should().Be(identity.Address);
        domainEvent.OldTierId.Should().Be(oldTier);
        domainEvent.NewTierId.Should().Be(identity.TierId);
    }

    [Fact]
    public void Changing_the_tier_to_valid_tier_is_successful()
    {
        // Arrange
        var identity = CreateIdentity();
        var newTier = TierId.Generate();

        // Act
        var acting = () => identity.ChangeTier(newTier);

        // Assert
        acting.Should().NotThrow();
        identity.TierId.Should().Be(newTier);
    }

    [Fact]
    public void Changing_the_tier_from_QueuedForDeletion_throws_DomainException()
    {
        // Arrange
        var identity = CreateIdentity(Tier.QUEUED_FOR_DELETION.Id);

        // Act
        var acting = () => identity.ChangeTier(TierId.Generate());

        // Assert
        acting.Should().Throw<DomainException>().Which.Message.Should().Be(DomainErrors.CannotChangeTierQueuedForDeletion().Message);
    }

    [Fact]
    public void Changing_the_tier_to_QueuedForDeletion_throws_DomainException()
    {
        // Arrange
        var identity = CreateIdentity();

        // Act
        var acting = () => identity.ChangeTier(Tier.QUEUED_FOR_DELETION.Id);

        // Assert
        acting.Should().Throw<DomainException>().Which.Message.Should().Be(DomainErrors.CannotChangeTierQueuedForDeletion().Message);
    }

    [Fact]
    public void Changing_the_tier_to_the_same_tier_throws_DomainException()
    {
        // Arrange
        var tierId = TierId.Generate();
        var identity = CreateIdentity(tierId);

        // Act
        var acting = () => identity.ChangeTier(tierId);

        // Assert
        acting.Should().Throw<DomainException>().Which.Message.Should().Contain("cannot be the same");
    }

    private static Identity CreateIdentity(TierId? tierId = null)
    {
        tierId ??= TierId.Generate();

        var address = IdentityAddress.Create([], "id1");
        return new Identity("", address, [], tierId, 1);
    }
}
