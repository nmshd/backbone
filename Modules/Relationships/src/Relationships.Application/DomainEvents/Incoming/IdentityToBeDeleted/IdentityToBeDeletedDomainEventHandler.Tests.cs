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
        var mockEventBus = A.Fake<IEventBus>();

        A.CallTo(() => fakeRelationshipsRepository.ListWithoutContent(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._, A<bool>._))
            .Returns([relationshipToPeer1, relationshipToPeer2]);

        var gracePeriodEndsAt = DateTime.Parse("2022-01-01");

        var handler = CreateHandler(fakeRelationshipsRepository, mockEventBus);

        //Act
        await handler.Handle(new IdentityToBeDeletedDomainEvent { IdentityAddress = identityToBeDeleted, GracePeriodEndsAt = gracePeriodEndsAt });

        //Assert
        A.CallTo(() => mockEventBus.Publish(A<PeerToBeDeletedDomainEvent>.That.Matches(e => e.PeerOfIdentityToBeDeleted == peer1 &&
                                                                                            e.RelationshipId == relationshipToPeer1.Id &&
                                                                                            e.IdentityToBeDeleted == identityToBeDeleted &&
                                                                                            e.GracePeriodEndsAt == gracePeriodEndsAt)))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(A<PeerToBeDeletedDomainEvent>.That.Matches(e => e.PeerOfIdentityToBeDeleted == peer2 &&
                                                                                            e.RelationshipId == relationshipToPeer2.Id &&
                                                                                            e.IdentityToBeDeleted == identityToBeDeleted &&
                                                                                            e.GracePeriodEndsAt == gracePeriodEndsAt)))
            .MustHaveHappenedOnceExactly();
    }

    private static IdentityToBeDeletedDomainEventHandler CreateHandler(IRelationshipsRepository relationshipsRepository, IEventBus eventBus)
    {
        return new IdentityToBeDeletedDomainEventHandler(relationshipsRepository, eventBus);
    }
}
