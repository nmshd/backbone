using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.RabbitMq;

public class EventBusRabbitMqTests : AbstractTestsBase
{
    [Fact]
    public void GetQueueName()
    {
        // Act
        var queueName = EventBusRabbitMq.GetQueueName<TestDomainEventHandler, TestDomainEvent>();

        // Assert
        queueName.Should().Be("TestModule.Test");
    }
}
