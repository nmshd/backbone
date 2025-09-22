using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Domain.Entities.Relationships;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class RelationshipReactivationCompletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var relationshipReactivationCompletedIntegrationEvent = CreateReactivationCompletedDomainEventForRelationship(RelationshipId.New());

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(relationshipReactivationCompletedIntegrationEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<RelationshipReactivationCompletedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Unblocks_MessageReceivedExternalEvents()
    {
        // Arrange
        var idOfReactivatedRelationship = RelationshipId.New();

        var relationshipReactivationCompletedIntegrationEvent = CreateReactivationCompletedDomainEventForRelationship(idOfReactivatedRelationship);

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var messageReceivedExternalEvent =
            new MessageReceivedExternalEvent(CreateRandomIdentityAddress(), new MessageReceivedExternalEvent.EventPayload { Id = "MSG11111111111111111" }, idOfReactivatedRelationship);
        messageReceivedExternalEvent.BlockDelivery();

        A.CallTo(() => mockDbContext.GetBlockedExternalEventsWithTypeAndContext(ExternalEventType.MessageReceived, A<string>._, A<CancellationToken>._))
            .Returns([messageReceivedExternalEvent]);

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(relationshipReactivationCompletedIntegrationEvent);

        // Assert
        messageReceivedExternalEvent.IsDeliveryBlocked.ShouldBeFalse();
        A.CallTo(() => mockDbContext.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static RelationshipReactivationCompletedDomainEvent CreateReactivationCompletedDomainEventForRelationship(RelationshipId idOfReactivatedRelationship)
    {
        return new RelationshipReactivationCompletedDomainEvent
        {
            NewRelationshipStatus = "Active",
            RelationshipId = idOfReactivatedRelationship,
            Peer = CreateRandomIdentityAddress()
        };
    }

    private static RelationshipReactivationCompletedDomainEventHandler CreateHandler(ISynchronizationDbContext dbContext)
    {
        var logger = A.Dummy<ILogger<RelationshipReactivationCompletedDomainEventHandler>>();

        return new RelationshipReactivationCompletedDomainEventHandler(dbContext, logger);
    }
}
