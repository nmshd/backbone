﻿using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Domain.Tests.Identities;

public class IdentityTests
{
    [Fact]
    public void Raises_TierOfIdentityChangedDomainEvent_on_updating()
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

    private static Identity CreateIdentity()
    {
        var address = IdentityAddress.Create([], "id1");
        return new Identity("", address, [], TierId.Generate(), 1);
    }
}
