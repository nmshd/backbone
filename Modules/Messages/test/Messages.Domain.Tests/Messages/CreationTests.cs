using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

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
        domainEvent.CreatedBy.ShouldBe(sender);
        domainEvent.Id.ShouldBe(message.Id);
        domainEvent.Recipients.ShouldHaveCount(1);
        domainEvent.Recipients.First().Address.ShouldBe(recipient.Address);
        domainEvent.Recipients.First().RelationshipId.ShouldBe(relationshipId);
    }
}
