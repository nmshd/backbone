using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;

public class ParticipantIsToBeDeletedTests
{
    [Fact]
    public void Raises_PeerToBeDeletedDomainEvent()
    {
        // Arrange
        var identityToBeDeleted = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(peer, identityToBeDeleted);

        // Act
        relationship.ParticipantIsToBeDeleted(identityToBeDeleted);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        domainEvent.PeerOfIdentityToBeDeleted.Should().Be(peer);
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.IdentityToBeDeleted.Should().Be(identityToBeDeleted);
    }
}
