using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
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
        var relationshipReactivationCompletedIntegrationEvent = new RelationshipReactivationCompletedDomainEvent("someRelationshipId", identityAddress);

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new RelationshipReactivationCompletedDomainEventHandler(mockDbContext,
            A.Fake<ILogger<RelationshipReactivationCompletedDomainEventHandler>>());

        // Act
        await handler.Handle(relationshipReactivationCompletedIntegrationEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<RelationshipReactivationCompletedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }
}
