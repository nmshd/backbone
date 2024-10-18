using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;

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
        var domainEvent = message.Should().HaveASingleDomainEvent<MessageCreatedDomainEvent>();
        domainEvent.CreatedBy.Should().Be(sender);
        domainEvent.Id.Should().Be(message.Id);
        domainEvent.Recipients.Should().HaveCount(1);
        domainEvent.Recipients.First().Address.Should().Be(recipient.Address);
        domainEvent.Recipients.First().RelationshipId.Should().Be(relationshipId);
    }
}
