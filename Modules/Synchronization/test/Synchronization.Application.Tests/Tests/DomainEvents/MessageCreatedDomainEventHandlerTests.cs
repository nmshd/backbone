using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Domain.Entities.Relationships;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Tooling;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class MessageCreatedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event_for_each_recipient()
    {
        // Arrange
        var senderAddress = CreateRandomIdentityAddress();
        var recipient1Address = CreateRandomIdentityAddress();
        var recipient2Address = CreateRandomIdentityAddress();

        var relationshipToRecipient1 = new Relationship(new RelationshipId("REL11111111111111111"), senderAddress, recipient1Address, RelationshipStatus.Active);
        var relationshipToRecipient2 = new Relationship(new RelationshipId("REL22222222222222222"), senderAddress, recipient2Address, RelationshipStatus.Active);

        var mockSynchronizationDbContext = A.Fake<ISynchronizationDbContext>();
        var fakeRelationshipsRepository = RelationshipsRepositoryReturning([relationshipToRecipient1, relationshipToRecipient2]);

        var handler = CreateHandler(mockSynchronizationDbContext, fakeRelationshipsRepository);

        // Act
        await handler.Handle(new MessageCreatedDomainEvent
        {
            Id = "MSG11111111111111111",
            CreatedBy = senderAddress,
            CreationDate = SystemTime.UtcNow,
            Recipients =
            [
                new MessageCreatedDomainEvent.Recipient
                {
                    Address = recipient1Address,
                    RelationshipId = relationshipToRecipient1.Id
                },
                new MessageCreatedDomainEvent.Recipient
                {
                    Address = recipient2Address,
                    RelationshipId = relationshipToRecipient2.Id
                }
            ]
        });

        // Assert
        A.CallTo(() => mockSynchronizationDbContext.CreateExternalEvent(
                A<ExternalEvent>.That.Matches(e => e.Owner == recipient1Address &&
                                                   e.Type == ExternalEventType.MessageReceived)))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockSynchronizationDbContext.CreateExternalEvent(
                A<ExternalEvent>.That.Matches(e => e.Owner == recipient2Address &&
                                                   e.Type == ExternalEventType.MessageReceived)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(RelationshipStatus.Pending)]
    [InlineData(RelationshipStatus.Terminated)]
    public async Task Created_external_events_are_blocked_when_relationship_with_recipient_is_in_status_pending_or_terminated(RelationshipStatus relationshipStatus)
    {
        // Arrange
        var senderAddress = CreateRandomIdentityAddress();
        var recipientAddress = CreateRandomIdentityAddress();

        var relationshipToRecipient = new Relationship(new RelationshipId("REL11111111111111111"), senderAddress, recipientAddress, relationshipStatus);

        var mockSynchronizationDbContext = A.Fake<ISynchronizationDbContext>();
        var fakeRelationshipsRepository = RelationshipsRepositoryReturning([relationshipToRecipient]);

        var handler = CreateHandler(mockSynchronizationDbContext, fakeRelationshipsRepository);

        // Act
        await handler.Handle(new MessageCreatedDomainEvent
        {
            Id = "MSG11111111111111111",
            CreatedBy = senderAddress,
            CreationDate = SystemTime.UtcNow,
            Recipients =
            [
                new MessageCreatedDomainEvent.Recipient
                {
                    Address = recipientAddress,
                    RelationshipId = relationshipToRecipient.Id
                }
            ]
        });

        // Assert
        A.CallTo(() => mockSynchronizationDbContext.CreateExternalEvent(
                A<ExternalEvent>.That.Matches(e => e.Owner == recipientAddress &&
                                                   e.Type == ExternalEventType.MessageReceived &&
                                                   e.IsDeliveryBlocked)))
            .MustHaveHappenedOnceExactly();
    }

    private IRelationshipsRepository RelationshipsRepositoryReturning(List<Relationship> relationships)
    {
        var relationshipIds = relationships.Select(r => r.Id);

        var relationshipsRepository = A.Fake<IRelationshipsRepository>();

        A.CallTo(() => relationshipsRepository.ListRelationships(relationshipIds, A<CancellationToken>._))
            .Returns(relationships.ToList());

        return relationshipsRepository;
    }

    private MessageCreatedDomainEventHandler CreateHandler(ISynchronizationDbContext dbContext, IRelationshipsRepository relationshipsRepository)
    {
        var logger = A.Dummy<ILogger<MessageCreatedDomainEventHandler>>();
        return new MessageCreatedDomainEventHandler(dbContext, relationshipsRepository, logger);
    }
}
