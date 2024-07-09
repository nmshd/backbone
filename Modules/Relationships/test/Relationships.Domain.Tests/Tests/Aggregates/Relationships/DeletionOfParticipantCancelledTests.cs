using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;

public class DeletionOfParticipantCancelledTests : AbstractTestsBase
{
    [Fact]
    public void Raises_PeerDeletionCancelledDomainEvent()
    {
        // Arrange
        var identityToBeDeleted = TestDataGenerator.CreateRandomIdentityAddress();
        var peer = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = CreateActiveRelationship(peer, identityToBeDeleted);

        // Act
        relationship.DeletionOfParticipantCancelled(identityToBeDeleted);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<PeerDeletionCancelledDomainEvent>();
        domainEvent.PeerOfIdentityWithDeletionCancelled.Should().Be(peer);
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.IdentityWithDeletionCancelled.Should().Be(identityToBeDeleted);
    }
}
