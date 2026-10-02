using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Relationships;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using RelationshipReactivationCompletedSlice = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipReactivationCompleted;

namespace Backbone.Modules.Synchronization.Module.Tests.Features.ExternalEvents.RelationshipReactivationCompleted;

public class HandlerTests : AbstractTestsBase
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

    private static RelationshipReactivationCompletedSlice.Handler CreateHandler(ISynchronizationDbContext dbContext)
    {
        var logger = A.Dummy<ILogger<RelationshipReactivationCompletedSlice.Handler>>();

        return new RelationshipReactivationCompletedSlice.Handler(dbContext, logger);
    }
}
