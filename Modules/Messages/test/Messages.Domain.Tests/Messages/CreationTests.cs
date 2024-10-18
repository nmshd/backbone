using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.Data;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class CreationTests : AbstractTestsBase
{
    [Fact]
    public void Raises_MessageCreatedDomainEvent_when_created()
    {
        // Arrange
        var sender = CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(CreateRandomIdentityAddress(), []);

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
        domainEvent.Recipients.First().Should().Be(recipient.Address);
    }
}
