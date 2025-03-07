using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Backbone.Modules.TestModule.Application.DomainEvents;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.EventBusRabbitMQ
{
    public class DomainEventNamingHelpersTests : AbstractTestsBase
    {
        [Fact]
        public void GetEventName()
        {
            // Arrange
            var testEvent = new TestDomainEvent();

            // Act
            var eventName = testEvent.GetEventName();

            // Assert
            eventName.Should().Be("Test");
        }

        [Fact]
        public void GetQueueName()
        {
            // Arrange

            // Act
            var queueName = DomainEventNamingHelpers.GetQueueName<TestDomainEventHandler, TestDomainEvent>();

            // Assert
            queueName.Should().Be("TestModule.Test");
        }
    }
}

#pragma warning disable IDE0130
namespace Backbone.Modules.TestModule.Application.DomainEvents
#pragma warning restore IDE0130
{
    internal class TestDomainEvent : DomainEvent;

    internal class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public Task Handle(TestDomainEvent domainEvent)
        {
            throw new NotImplementedException();
        }
    }
}
