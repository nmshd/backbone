using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipParticipantIsToBeDeletedTests : AbstractTestsBase
{
    [Fact]
    public void Raises_PeerToBeDeletedDomainEvent()
    {
        // Arrange
        var identityToBeDeleted = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(peer, identityToBeDeleted);
        var gracePeriodEndsAt = new DateTime(2023, 9, 13, 14, 35, 50);

        // Act
        relationship.ParticipantIsToBeDeleted(identityToBeDeleted, gracePeriodEndsAt);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        domainEvent.PeerOfIdentityToBeDeleted.Should().Be(peer);
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.IdentityToBeDeleted.Should().Be(identityToBeDeleted);
        domainEvent.GracePeriodEndsAt.Should().Be(gracePeriodEndsAt);
    }
}
