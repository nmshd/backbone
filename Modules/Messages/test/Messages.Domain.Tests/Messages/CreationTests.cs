using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class CreationTests : AbstractTestsBase
{
    [Fact]
    public void Raises_MessageCreatedDomainEvent_when_created()
    {
        // Arrange
        var sender = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(TestDataGenerator.CreateRandomIdentityAddress(), []);

        // Act
        var message = new Message(
            sender,
            TestDataGenerator.CreateRandomDeviceId(),
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
