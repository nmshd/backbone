using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using FakeItEasy;

namespace Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityToBeDeleted;

public class IdentityToBeDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task Publishes_PeerToBeDeletedDomainEvent()
    {
        //Arrange
        var identityToBeDeleted = CreateRandomIdentityAddress();

        var peer1 = CreateRandomIdentityAddress();
        var peer2 = CreateRandomIdentityAddress();

        var relationshipToPeer1 = TestData.CreateActiveRelationship(peer1, identityToBeDeleted);
        var relationshipToPeer2 = TestData.CreateActiveRelationship(peer2, identityToBeDeleted);

        var fakeRelationshipsRepository = A.Dummy<IRelationshipsRepository>();

        A.CallTo(() => fakeRelationshipsRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._, A<bool>._))
            .Returns([relationshipToPeer1, relationshipToPeer2]);

        var gracePeriodEndsAt = DateTime.Parse("2022-01-01");

        var handler = CreateHandler(fakeRelationshipsRepository);

        //Act
        await handler.Handle(new IdentityToBeDeletedDomainEvent(identityToBeDeleted, gracePeriodEndsAt));

        //Assert
        var event1 = relationshipToPeer1.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        event1.PeerOfIdentityToBeDeleted.Should().Be(peer1);
        event1.RelationshipId.Should().Be(relationshipToPeer1.Id);
        event1.IdentityToBeDeleted.Should().Be(identityToBeDeleted);
        event1.GracePeriodEndsAt.Should().Be(gracePeriodEndsAt);

        var event2 = relationshipToPeer2.Should().HaveASingleDomainEvent<PeerToBeDeletedDomainEvent>();
        event2.PeerOfIdentityToBeDeleted.Should().Be(peer2);
        event2.RelationshipId.Should().Be(relationshipToPeer2.Id);
        event2.IdentityToBeDeleted.Should().Be(identityToBeDeleted);
        event2.GracePeriodEndsAt.Should().Be(gracePeriodEndsAt);
    }

    private static IdentityToBeDeletedDomainEventHandler CreateHandler(IRelationshipsRepository relationshipsRepository)
    {
        return new IdentityToBeDeletedDomainEventHandler(relationshipsRepository, A.Dummy<IEventBus>());
    }
}
