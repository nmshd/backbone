using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.PeerIdentityDeleted;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Tests.TestHelpers;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming.PeerIdentityDeleted;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Relationships.Application.Tests.Tests.DomainEvents;
public class PeerIdentityDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public static async Task Publishes_PeerFromRelationshipDeletedDomainEvent()
    {
        // Arrange
        var identity = TestDataGenerator.CreateRandomIdentityAddress();
        var deletedIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var relationship = TestData.CreateActiveRelationship(identity, deletedIdentity);

        var request = new PeerIdentityDeletedDomainEvent(relationship.Id, deletedIdentity);

        var mockEventBus = A.Fake<IEventBus>();
        var fakeRelationshipRepository = A.Fake<IRelationshipsRepository>();
        var handler = new PeerIdentityDeletedDomainEventHandler(fakeRelationshipRepository, mockEventBus);

        A.CallTo(() => fakeRelationshipRepository.FindRelationship(relationship.Id, deletedIdentity, A<CancellationToken>._, A<bool>._)).Returns(relationship);

        // Act
        await handler.Handle(request);

        // Assert
        A.CallTo(
                () => mockEventBus.Publish(A<PeerFromRelationshipDeletedDomainEvent>.That.Matches(e =>
                    e.IdentityAddress == identity &&
                    e.RelationshipId == relationship.Id &&
                    e.PeerAddress == deletedIdentity)
                ))
            .MustHaveHappenedOnceExactly();
    }
}
