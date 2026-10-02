using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Messages.Domain.Tests.Entities;

public class CreationTests : AbstractTestsBase
{
    [Fact]
    public void Raises_MessageCreatedDomainEvent_when_created()
    {
        // Arrange
        var sender = CreateRandomIdentityAddress();
        var relationshipId = RelationshipId.New();
        var recipient = new RecipientInformation(CreateRandomIdentityAddress(), relationshipId, []);

        // Act
        var message = new Message(
            sender,
            CreateRandomDeviceId(),
            [],
            [],
            [recipient]
        );

        // Assert
        var domainEvent = message.ShouldHaveASingleDomainEvent<MessageCreatedDomainEvent>();
        domainEvent.DomainEventId.ShouldBe($"{message.Id}/Created");
        domainEvent.CreatedBy.ShouldBe(sender);
        domainEvent.Id.ShouldBe(message.Id);
        domainEvent.Recipients.ShouldHaveCount(1);
        domainEvent.Recipients.First().Address.ShouldBe(recipient.Address);
        domainEvent.Recipients.First().RelationshipId.ShouldBe(relationshipId);
    }
}
