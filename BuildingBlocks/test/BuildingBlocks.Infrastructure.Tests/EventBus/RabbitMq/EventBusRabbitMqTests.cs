using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Backbone.Modules.TestModule.Application.DomainEvents;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.RabbitMq;

public class EventBusRabbitMqTests : AbstractTestsBase
{
    [Fact]
    public void GetQueueName()
    {
        // Act
        var queueName = EventBusRabbitMq.GetQueueName<TestDomainEventHandler, TestDomainEvent>();

        // Assert
        queueName.ShouldBe("TestModule.Test");
    }
}
