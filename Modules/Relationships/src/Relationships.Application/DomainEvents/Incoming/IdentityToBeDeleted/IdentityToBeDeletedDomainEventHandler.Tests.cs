using System.Linq.Expressions;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityToBeDeleted;

public class IdentityToBeDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task Publishes_PeerToBeDeletedDomainEvent()
    {
        //Arrange
        var identityToBeDeleted = TestDataGenerator.CreateRandomIdentityAddress();
        var randomDateTime = new DateTime(2015, 7, 23, 14, 35, 50);

        var peer1 = TestDataGenerator.CreateRandomIdentityAddress();
        var peer2 = TestDataGenerator.CreateRandomIdentityAddress();

        var relationshipToPeer1 = TestData.CreateActiveRelationship(peer1, identityToBeDeleted);
        var relationshipToPeer2 = TestData.CreateActiveRelationship(peer2, identityToBeDeleted);

        var fakeRelationshipsRepository = A.Dummy<IRelationshipsRepository>();

        A.CallTo(() => fakeRelationshipsRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._))
            .Returns(new List<Relationship> { relationshipToPeer1, relationshipToPeer2 });

        var handler = CreateHandler(fakeRelationshipsRepository);

        //Act
        await handler.Handle(new IdentityToBeDeletedDomainEvent(identityToBeDeleted, randomDateTime));

        //Assert
        var event1 = relationshipToPeer1.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        event1.PeerOfIdentityToBeDeleted.Should().Be(peer1);
        event1.RelationshipId.Should().Be(relationshipToPeer1.Id);
        event1.IdentityToBeDeleted.Should().Be(identityToBeDeleted);

        var event2 = relationshipToPeer2.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        event2.PeerOfIdentityToBeDeleted.Should().Be(peer2);
        event2.RelationshipId.Should().Be(relationshipToPeer2.Id);
        event2.IdentityToBeDeleted.Should().Be(identityToBeDeleted);
    }

    private static IdentityToBeDeletedDomainEventHandler CreateHandler(IRelationshipsRepository relationshipsRepository)
    {
        return new IdentityToBeDeletedDomainEventHandler(relationshipsRepository);
    }
}
