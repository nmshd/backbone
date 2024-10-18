using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipDeletionOfParticipantStartedTests : AbstractTestsBase
{
    [Fact]
    public void Raises_PeerDeletedDomainEvent()
    {
        // Arrange
        var identityToBeDeleted = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(peer, identityToBeDeleted);

        // Act
        relationship.DeletionOfParticipantStarted(identityToBeDeleted);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<PeerDeletedDomainEvent>();
        domainEvent.PeerOfDeletedIdentity.Should().Be(peer);
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.DeletedIdentity.Should().Be(identityToBeDeleted);
    }
}
