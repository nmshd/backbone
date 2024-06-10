using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityToBeDeleted;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Tests.TestHelpers;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.IdentityToBeDeleted;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.DomainEvents.Incoming;
public class IdentityToBeDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task FindRelationships_method_is_called()
    {
        //Arrange
        var identity = TestDataGenerator.CreateRandomIdentityAddress();
        var mockRelationshipsRepository = A.Fake<IRelationshipsRepository>();

        var handler = CreateHandler(mockRelationshipsRepository);

        //Act
        await handler.Handle(new IdentityToBeDeletedDomainEvent(identity));

        //Assert
        A.CallTo(() => mockRelationshipsRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public static async Task Publishes_PeerToBeDeletedDomainEvent()
    {
        //Arrange
        var identity = TestDataGenerator.CreateRandomIdentityAddress();
        var identityToBeDeleted = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = TestData.CreateActiveRelationship(identity, identityToBeDeleted);

        var fakeRelationshipsRepository = A.Dummy<IRelationshipsRepository>();
        var mockEventBus = A.Fake<IEventBus>();

        A.CallTo(() => fakeRelationshipsRepository.FindRelationships(A<Expression<Func<Relationship, bool>>>._, A<CancellationToken>._))
            .Returns(new List<Relationship>() { relationship });

        var handler = CreateHandler(fakeRelationshipsRepository, mockEventBus);

        //Act
        await handler.Handle(new IdentityToBeDeletedDomainEvent(identityToBeDeleted));

        //Assert
        A.CallTo(() => mockEventBus.Publish(A<PeerToBeDeletedDomainEvent>.That.Matches(e =>
            e.IdentityAddress == identity &&
            e.RelationshipId == relationship.Id &&
            e.PeerIdentityAddress == identityToBeDeleted))
        ).MustHaveHappenedOnceExactly();
    }

    private static IdentityToBeDeletedDomainEventHandler CreateHandler(IRelationshipsRepository relationshipsRepository, IEventBus? eventBus = null)
    {
        return new IdentityToBeDeletedDomainEventHandler(relationshipsRepository, eventBus ?? A.Dummy<IEventBus>());
    }
}
