using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.Modules.TestModule.Application.DomainEvents;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.RabbitMq
{
    public class DomainEventNamingExtensionsTests : AbstractTestsBase
    {
        [Fact]
        public void GetEventNameFromObject()
        {
            // Arrange
            var testEvent = new TestDomainEvent();

            // Act
            var eventName = testEvent.GetEventName();

            // Assert
            eventName.Should().Be("Test");
        }

        [Fact]
        public void GetEventNameFromType()
        {
            // Act
            var eventName = typeof(TestDomainEvent).GetEventName();

            // Assert
            eventName.Should().Be("Test");
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
